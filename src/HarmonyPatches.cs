using HarmonyLib;
using LLScreen;

namespace TourneyMod;

public static class HarmonyPatches
{
    internal static void PatchAll()
    {
        Harmony harmony = new Harmony(Plugin.GUID);
        harmony.PatchAll(typeof(CustomStageScreen));
        Plugin.LogGlobal.LogInfo("Custom stage screen patch applied");
    }

    internal static class CustomStageScreen
    {
        // GameStatesLobby.OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPrefix]
        private static void OpenStageSelect_Prefix(HPNLMFHPHFD __instance)
        {
            IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            if (localLobby == null) return;
            
            ScreenStageStrike.Open(); // ScreenPlayersStage screenStage
        }
        // GameStatesLobby.OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPostfix]
        private static void OpenStageSelect_Postfix(HPNLMFHPHFD __instance)
        {
            IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            if (localLobby == null) return;

            localLobby.GFHABHIBKHK(); // GameStatesLobbyLocal.ShowActiveCursors()
        }
        
        // GameStatesLobby.CloseStageSelect()
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.EKFCNNPDJHH))]
        [HarmonyPostfix]
        private static void CloseStageSelect_Postfix(HPNLMFHPHFD __instance)
        {
            IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            if (localLobby == null) return;
            
            ScreenStageStrike.Close();
        }

        [HarmonyPatch(typeof(ScreenPlayersStage), nameof(ScreenPlayersStage.OnOpen))]
        [HarmonyPrefix]
        private static bool OnOpen_Prefix(ScreenPlayersStage __instance)
        {
            if (!ScreenStageStrike.IsOpen) return true;
            
            ScreenStageStrike.Instance.OnOpen(__instance);
            return false;
        }
    }
}