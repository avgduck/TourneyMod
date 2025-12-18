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
        
        harmony.PatchAll(typeof(StageScreenPatch));
        Plugin.LogGlobal.LogInfo("Stage select screen patch applied");
        
        /*
        harmony.PatchAll(typeof(StageSizePatch));
        Plugin.LogGlobal.LogInfo("Stage size check patch applied");
        */
        
        if (!ModDependenciesUtils.IsModLoaded(Plugin.DEPENDENCY_MODMENU))
        {
            Plugin.LogGlobal.LogWarning("ModMenu is not loaded. Skipping preview patch...");
            return;
        }
        
        harmony.PatchAll(typeof(RulesetPreviewPatch));
        Plugin.LogGlobal.LogInfo("Ruleset preview patch applied");
    }
}