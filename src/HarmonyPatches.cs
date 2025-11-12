using HarmonyLib;
using LLScreen;
using UnityEngine;

namespace TourneyMod;

public static class HarmonyPatches
{
    internal static void PatchAll()
    {
        Harmony harmony = new Harmony(Plugin.GUID);
        harmony.PatchAll(typeof(ScreenPlayersStage_Patch));
        Plugin.LogGlobal.LogInfo("Daio's stage select screen patch applied");
        harmony.PatchAll(typeof(ScreenInjection));
        Plugin.LogGlobal.LogInfo("Custom screen injection patch applied");
    }

    internal static class ScreenInjection
    {
        // Assets.SpawnScreen(ScreenType screenType)
        [HarmonyPatch(typeof(JPLELOFJOOH), nameof(JPLELOFJOOH.HNHBCLJGPCE))]
        [HarmonyPostfix]
        private static void SpawnScreen_Postfix(ScreenType FLMBCGMOCKC, GameObject __result)
        {
            ScreenType screenType = FLMBCGMOCKC;
            if (__result == null) return;
            if (screenType != ScreenType.PLAYERS_STAGE) return;
            ScreenPlayersStage screenPlayersStage = __result.GetComponent<ScreenPlayersStage>();
            if (screenPlayersStage == null) return;
            
            Plugin.LogGlobal.LogInfo("Stage select screen spawned");
        }
        
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
        }
    }
}