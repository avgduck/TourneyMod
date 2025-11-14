using BepInEx;
using BepInEx.Logging;
using LLHandlers;

namespace TourneyMod;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("LLBlaze.exe")]
internal class Plugin : BaseUnityPlugin
{
    public const string GUID = "avgduck.plugins.llb.tourneymod";
    
    internal static Plugin Instance { get; private set; }
    internal static ManualLogSource LogGlobal;

    internal const bool USE_SEWERS = false;

    private void Awake()
    {
        Instance = this;
        LogGlobal = this.Logger;
        
        HarmonyPatches.PatchAll();
        RulesetIO.Init();
    }
}
