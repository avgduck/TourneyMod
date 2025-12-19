using HarmonyLib;
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
            ScreenMenuVersus screenMenuVersus = __result.GetComponent<ScreenMenuVersus>();
            if (screenMenuVersus == null) return;
            ScreenMenuLocal screenMenuLocal = __result.AddComponent<ScreenMenuLocal>();
            screenMenuLocal.Init(screenMenuVersus);
            GameObject.DestroyImmediate(screenMenuVersus);
        }
        else if (FLMBCGMOCKC == ScreenType.PLAYERS_STAGE && SetTracker.Instance.IsTrackingSet)
        {
            ScreenPlayersStage screenPlayersStage = __result.GetComponent<ScreenPlayersStage>();
            if (screenPlayersStage == null) return;
            ScreenStageStrike screenStageStrike = __result.AddComponent<ScreenStageStrike>();
            screenStageStrike.Init(screenPlayersStage);
            GameObject.DestroyImmediate(screenPlayersStage);
        }
    }
}