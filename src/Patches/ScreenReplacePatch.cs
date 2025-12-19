using HarmonyLib;
using LLBML.Players;
using LLScreen;
using TourneyMod.SetTracking;
using TourneyMod.UI;
using UnityEngine;

namespace TourneyMod.Patches;

internal static class ScreenReplacePatch
{
    // GameObject Assets::SpawnScreen(ScreenType screenType)
    [HarmonyPatch(typeof(JPLELOFJOOH), nameof(JPLELOFJOOH.HNHBCLJGPCE))]
    [HarmonyPostfix]
    private static void Assets_SpawnScreen_Postfix(ref GameObject __result, ScreenType FLMBCGMOCKC)
    {
        if (FLMBCGMOCKC == ScreenType.MENU_VERSUS)
        {
            ReplaceScreen<ScreenMenuVersus, ScreenMenuLocal>(ref __result);
        }
        else if (FLMBCGMOCKC == ScreenType.PLAYERS && SetTracker.Instance.IsTrackingSet)
        {
            if (Plugin.Instance.ActiveTourneyMode == TourneyMode.NONE)
            {
                // TODO: add custom lobby screen with win tracking to other game modes
            }
            else
            {
                ReplaceScreen<ScreenPlayers, ScreenLobbyTourney>(ref __result);
            }
        }
        else if (FLMBCGMOCKC == ScreenType.PLAYERS_STAGE && SetTracker.Instance.IsTrackingSet)
        {
            ReplaceScreen<ScreenPlayersStage, ScreenStageStrike>(ref __result);
        }
    }
    
    private static void ReplaceScreen<T1, T2>(ref GameObject screen)
        where T1 : ScreenBase
        where T2 : T1, ICustomScreen<T1>
    {
        T1 screenVanilla = screen.GetComponent<T1>();
        if (screenVanilla == null)
        {
            Plugin.LogGlobal.LogError($"Error attempting to replace screen {typeof(T2)} with {typeof(T1)}");
            return;
        }

        T2 screenCustom = screen.AddComponent<T2>();
        screenCustom.Init(screenVanilla);
        GameObject.DestroyImmediate(screenVanilla);
    }
    
    // GameStatesLobby.RemovePlayer(Player p)
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.GNBKBMENOMO))]
    [HarmonyPostfix]
    private static void RemovePlayer_Postfix(ALDOKEMAOMB LGACHGEPNNH)
    {
        Player player = LGACHGEPNNH;
        VoteButton.RemovePlayer(player.nr);
    }

    [HarmonyPatch(typeof(ScreenPlayersStage), nameof(ScreenPlayersStage.SelectionDone))]
    [HarmonyPrefix]
    private static bool ScreenPlayersStage_SelectionDone_Prefix(ScreenPlayersStage __instance)
    {
        ScreenStageStrike screenStageStrike = __instance as ScreenStageStrike;
        if (screenStageStrike == null) return true;
        screenStageStrike.OnStageSelected();
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