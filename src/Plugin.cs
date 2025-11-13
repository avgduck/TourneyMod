using BepInEx;
using BepInEx.Logging;
using LLHandlers;

namespace TourneyMod;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("LLBlaze.exe")]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "avgduck.plugins.llb.tourneymod";
    internal static ManualLogSource LogGlobal;

    internal const bool USE_SEWERS = false;
    
    internal static Stage[] StagesNeutral = [
        Stage.JUNKTOWN,
        Stage.ROOM21,
        (USE_SEWERS ? Stage.SEWERS : Stage.OUTSKIRTS),
        Stage.SUBWAY,
        Stage.STREETS,
    ];

    internal static Stage[] StagesCounterpick = [
        Stage.POOL,
        Stage.STADIUM,
        Stage.FACTORY,
        Stage.CONSTRUCTION,
    ];

    private void Awake()
    {
        LogGlobal = this.Logger;
        HarmonyPatches.PatchAll();
    }
}
