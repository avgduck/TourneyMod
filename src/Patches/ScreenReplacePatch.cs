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
            ReplaceScreen<ScreenMenuVersus, ScreenMenuLocal>(ref __result);
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
}