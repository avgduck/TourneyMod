using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace TourneyMod;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("LLBlaze.exe")]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "avgduck.plugins.llb.tourneymod";
    internal static ManualLogSource LogGlobal;

    private void Awake()
    {
        LogGlobal = this.Logger;

        Harmony harmony = new Harmony(GUID);
        harmony.PatchAll(typeof(ScreenPlayersStage_Patch));
    }
}
