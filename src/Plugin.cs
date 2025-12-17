using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using LLBML.Utils;
using TourneyMod.Patches;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;

namespace TourneyMod;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(DEPENDENCY_LLBML, BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(DEPENDENCY_MODMENU, BepInDependency.DependencyFlags.SoftDependency)]
[BepInProcess("LLBlaze.exe")]
internal class Plugin : BaseUnityPlugin
{
    public const string GUID = "avgduck.plugins.llb.tourneymod";
    internal const string DEPENDENCY_LLBML = "fr.glomzubuk.plugins.llb.llbml";
    internal const string DEPENDENCY_MODMENU = "no.mrgentle.plugins.llb.modmenu";
    
    internal static Plugin Instance { get; private set; }
    internal static ManualLogSource LogGlobal { get; private set; }

    internal Ruleset SelectedRuleset;

    private void Awake()
    {
        Instance = this;
        LogGlobal = this.Logger;
        SetTracker.Init();
        StageStrikeTracker.Init();
        
        HarmonyPatches.PatchAll();
        RulesetIO.Init();
        
        StageStrikeTracker.Instance.FindDefaultRuleset();

        Configs.BindConfigs();
        Config.SettingChanged += (sender, args) => OnConfigChanged();
        OnConfigChanged();
        ModDependenciesUtils.RegisterToModMenu(this.Info, GetModMenuText());
    }

    private void OnConfigChanged()
    {
        string id = Configs.SelectedRulesetId.Value;
        SelectedRuleset = RulesetIO.GetRulesetById(id);

        if (SelectedRuleset == null)
        {
            //Plugin.LogGlobal.LogError($"Ruleset '{id}' does not exist!");
            return;
        }
        
        Plugin.LogGlobal.LogInfo($"Loaded ruleset {id}");
    }

    private List<string> GetModMenuText()
    {
        List<string> text = new List<string>();
        
        text.Add("Choose a ruleset from those currently loaded (shown below). Default rulesets are included with the mod download, and custom rulesets can be specified in your Modding Folder.");
        
        text.Add("");
        
        text.Add("<b>Default Rulesets:</b>");
        if (RulesetIO.RulesetsDefault.Count == 0)
        {
            text.Add("none");
        }
        else
        {
            RulesetIO.RulesetsDefault.ToList().ForEach(entry => text.Add($"- {entry.Value.name} [<b>{entry.Key}</b>]"));
        }
        
        text.Add("");
        
        text.Add("<b>Custom Rulesets:</b>");
        if (RulesetIO.RulesetsCustom.Count == 0)
        {
            text.Add("none");
        }
        else
        {
            RulesetIO.RulesetsCustom.ToList().ForEach(entry => text.Add($"- {entry.Value.name} [<b>{entry.Key}</b>]"));
        }

        return text;
    }
}
