using HarmonyLib;
using LLBML.Utils;

namespace TourneyMod.Patches;

internal static class HarmonyPatches
{
    internal static void PatchAll()
    {
        Harmony harmony = new Harmony(Plugin.GUID);
        
        harmony.PatchAll(typeof(SetTrackingPatch));
        Plugin.LogGlobal.LogInfo("Set tracking patch applied");
        
        harmony.PatchAll(typeof(ScreenReplacePatch));
        Plugin.LogGlobal.LogInfo("Screen replacement patch applied");
        harmony.PatchAll(typeof(CursorPatch));
        Plugin.LogGlobal.LogInfo("Cursor patch applied");
        harmony.PatchAll(typeof(ScreenResultsPatch));
        Plugin.LogGlobal.LogInfo("Results screen patch applied");
        
        /*
        harmony.PatchAll(typeof(StageSizePatch));
        Plugin.LogGlobal.LogInfo("Stage size check patch applied");
        */
    }
}