using HarmonyLib;
using LLScreen;
using TourneyMod.UI;

namespace TourneyMod.Patches;

internal static class ScreenLobbyPatch
{
    [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.SetPlayerLayout))]
    [HarmonyPostfix]
    private static void SetPlayerLayout_Postfix(ScreenPlayers __instance)
    {
        if (!ScreenLobbyOverlay.IsActive) return;
            
        ScreenLobbyOverlay.Instance.OnOpen(__instance);
    }
    
    [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.DoUpdate))]
    [HarmonyPostfix]
    private static void ScreenPlayers_DoUpdate_Postfix()
    {
        if (!ScreenLobbyOverlay.IsActive) return;
        if (!ScreenLobbyOverlay.Instance.IsOpen) return; 
            
        ScreenLobbyOverlay.Instance.UpdateSetCount();
    }

    [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.ShowCpuButtons))]
    [HarmonyPrefix]
    private static bool ShowCpuButtons(ScreenPlayers __instance, bool visible)
    {
        if (!visible || Plugin.Instance.ActiveTourneyMode == TourneyMode.NONE) return true;

        __instance.ShowCpuButtons(false);
        return false;
    }
}