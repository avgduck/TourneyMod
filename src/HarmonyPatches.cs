using HarmonyLib;
using LLScreen;
using UnityEngine;

namespace TourneyMod;

public static class HarmonyPatches
{
    internal static void PatchAll()
    {
        Harmony harmony = new Harmony(Plugin.GUID);
        //harmony.PatchAll(typeof(ScreenPlayersStage_Patch));
        //Plugin.LogGlobal.LogInfo("Daio's stage select screen patch applied");
        harmony.PatchAll(typeof(CustomStageScreen));
        Plugin.LogGlobal.LogInfo("Custom stage screen patch applied");
    }

    internal static class CustomStageScreen
    {
        // GameStatesLobby.OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPostfix]
        private static void OpenStageSelect_Postfix(HPNLMFHPHFD __instance)
        {
            Plugin.LogGlobal.LogWarning($"OpenStageSelect: {__instance.GetType()}");
            IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            if (localLobby == null)
            {
                Plugin.LogGlobal.LogInfo("Not in a local lobby");
                return;
            }
            localLobby.GFHABHIBKHK(); // GameStatesLobbyLocal.ShowActiveCursors()
            ScreenStageStrike.Open(__instance.CFKCIJCEILI); // ScreenPlayersStage screenStage
        }
        
        // GameStatesLobby.CloseStageSelect()
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.EKFCNNPDJHH))]
        [HarmonyPostfix]
        private static void CloseStageSelect_Postfix(HPNLMFHPHFD __instance)
        {
            Plugin.LogGlobal.LogWarning($"CloseStageSelect: {__instance.GetType()}");
            IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            if (localLobby == null)
            {
                Plugin.LogGlobal.LogInfo("Not in a local lobby");
                return;
            }
            ScreenStageStrike.Close();
        }
    }
}