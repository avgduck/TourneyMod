using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using LLBML.Utils;
using TourneyMod.Patches;
using TourneyMod.Rulesets;

namespace TourneyMod;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("LLBlaze.exe")]
internal class Plugin : BaseUnityPlugin
{
    public const string GUID = "avgduck.plugins.llb.tourneymod";
    
    internal static Plugin Instance { get; private set; }
    internal static ManualLogSource LogGlobal { get; private set; }

    private void Awake()
    {
        Instance = this;
        LogGlobal = this.Logger;
        
        HarmonyPatches.PatchAll();
        RulesetIO.Init();

        Configs.BindConfigs();
        ModDependenciesUtils.RegisterToModMenu(this.Info, GetModMenuText());
    }

    private List<string> GetModMenuText()
    {
        List<string> text = new List<string>();
        
        text.Add("Choose a ruleset from those currently loaded (shown below). Default rulesets are included with the mod download, and custom rulesets can be specified in your Modding Folder.");
        text.Add("");
        text.Add("");
        
        text.Add("<b>Default Rulesets:</b>");
        if (RulesetIO.RulesetsDefault.Count == 0)
        {
            text.Add("none");
        }
        else
        {
            RulesetIO.RulesetsDefault.ForEach(ruleset => text.AddRange(ruleset.GetDescription()));
        }
        
        text.Add("");
        text.Add("");
        
        text.Add("<b>Custom Rulesets:</b>");
        if (RulesetIO.RulesetsCustom.Count == 0)
        {
            text.Add("none");
        }
        else
        {
            RulesetIO.RulesetsCustom.ForEach(ruleset => text.AddRange(ruleset.GetDescription()));
        }

        return text;
    }
}
