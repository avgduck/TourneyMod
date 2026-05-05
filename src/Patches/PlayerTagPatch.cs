using HarmonyLib;
using LLHandlers;

namespace TourneyMod.Patches;

public class PlayerTagPatch
{
    [HarmonyPatch(typeof(HGFCCNMEEEF), nameof(HGFCCNMEEEF.JLGCJFEFDLI))]
    [HarmonyPostfix]
    private static void GameStatesOptions_InputConfigAddController_Postfix(Controller GDEMBCKIDMA)
    {
        Controller controller = GDEMBCKIDMA;
        //Plugin.LogGlobal.LogWarning($"InputConfigAddController: {controller}");
    }
}