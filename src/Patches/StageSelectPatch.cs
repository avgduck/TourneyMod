using HarmonyLib;
using LLScreen;
using TourneyMod.UI.StageSelect;

namespace TourneyMod.Patches;

internal static class StageSelectPatch
{
    [HarmonyPatch(typeof(ScreenPlayersStage), nameof(ScreenPlayersStage.SelectionDone))]
    [HarmonyPrefix]
    private static bool ScreenPlayersStage_SelectionDone_Prefix(ScreenPlayersStage __instance)
    {
        IStageSelect stageSelect = __instance as IStageSelect;
        if (stageSelect == null) return true;
        stageSelect.OnStageSelected();
        return false;
    }
    
    /*
    // THIS CODE PREVENTS STAGE SELECTIONS FROM GOING THROUGH AND ALLOWS "VOTING" IN LOCAL - TESTING ONLY
    // void GameStatsLobby::ProcessMsgStageSelect(Message message)
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.LKBFKGGCFHE))]
    [HarmonyPrefix]
    private static bool GameStatesLobby_ProcessMsgStageSelect_Prefix(HPNLMFHPHFD __instance, Message EIMJOIEPMNA)
    {
        if (EIMJOIEPMNA.msg == Msg.SEL_STAGE)
        {
            __instance.CFKCIJCEILI.SelectionDone();
            return false;
        }
        return true;
    }

    // void GameStatsLobbyOnline::StageSelected(int playerNr, int stageIndex)
    [HarmonyPatch(typeof(HDLIJDBFGKN), nameof(HDLIJDBFGKN.MNLFJDLDHEN))]
    [HarmonyPrefix]
    private static bool GameStatesLobbyOnline_StageSelected_Prefix()
    {
        return false;
    }
    */
}