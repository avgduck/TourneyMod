using HarmonyLib;

namespace TourneyMod.Patches;

internal static class HarmonyPatches
{
    internal static void PatchAll()
    {
        Harmony harmony = new Harmony(Plugin.GUID);
        
        harmony.PatchAll(typeof(SetTrackingPatch));
        Plugin.LogGlobal.LogInfo("Set tracking patch applied");
        
        harmony.PatchAll(typeof(StageScreenPatch));
        Plugin.LogGlobal.LogInfo("Stage select screen patch applied");
    }
}