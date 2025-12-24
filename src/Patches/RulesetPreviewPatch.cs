using BepInEx;
using HarmonyLib;
using TourneyMod.UI;

namespace TourneyMod.Patches;

internal static class RulesetPreviewPatch
{
    internal static bool isInModMenu = false;
        
    [HarmonyPatch(typeof(ModMenu.ModMenu), "HandleModSubSettingsClick")]
    [HarmonyPrefix]
    private static void HandleModSubSettingsClick_Prefix(PluginInfo plugin)
    {
        isInModMenu = plugin.Metadata.GUID == Plugin.GUID;
    }

    [HarmonyPatch(typeof(ModMenu.ModMenu), "HandleQuitClick")]
    [HarmonyPrefix]
    private static void HandleQuitClick()
    {
        isInModMenu = false;
    }
    
    [HarmonyPatch(typeof(ScreenMenu), nameof(ScreenMenu.OnOpen))]
    [HarmonyPostfix]
    private static void OnOpen_Postfix(ScreenMenu __instance)
    {
        RulesetPreviewWindow.Create(__instance.gameObject.transform, false);
    }
}