using HarmonyLib;
using LLScreen;
using TourneyMod.UI;
using UnityEngine;

namespace TourneyMod.Patches;

internal static class ScreenMenuPatch
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
    }
}