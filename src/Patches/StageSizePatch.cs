using HarmonyLib;
using LLBML.Math;
using LLBML.Settings;

namespace TourneyMod.Patches;

internal static class StageSizePatch
{
    [HarmonyPatch(typeof(World), nameof(World.SetStageDimensions))]
    [HarmonyPostfix]
    private static void SetStageDimensions_Postfix(World __instance)
    {
        Plugin.LogGlobal.LogInfo($"Started game on stage {GameSettings.current.stage}, size ({((Vector2f)__instance.stageSize).x / World.FPIXEL_SIZE}, {((Vector2f)__instance.stageSize).y / World.FPIXEL_SIZE})");
    }
}