using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using LLBML.Utils;
using TourneyMod.Patches;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using TourneyMod.UI;

namespace TourneyMod;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(DEPENDENCY_LLBML, BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(DEPENDENCY_MODMENU, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(DEPENDENCY_CURSORSPEED, BepInDependency.DependencyFlags.SoftDependency)]
[BepInIncompatibility(INCOMPATIBILITY_STAGESELECT)]
[BepInProcess("LLBlaze.exe")]
internal class Plugin : BaseUnityPlugin
{
    public const string GUID = "avgduck.plugins.llb.tourneymod";
    internal const string DEPENDENCY_LLBML = "fr.glomzubuk.plugins.llb.llbml";
    internal const string DEPENDENCY_MODMENU = "no.mrgentle.plugins.llb.modmenu";
    internal const string DEPENDENCY_CURSORSPEED = "com.github.daioutzu.cursorspeed";
    internal const string INCOMPATIBILITY_STAGESELECT = "com.github.daioutzu.stageselect";
    
    internal static Plugin Instance { get; private set; }
    internal static ManualLogSource LogGlobal { get; private set; }

    private const string defaultRulesetId = "all_stages";
    internal Dictionary<TourneyMode, string> SelectedRulesetIds;
    internal Dictionary<TourneyMode, Ruleset> SelectedRulesets;
    
    internal bool TourneyMenuOpen = false;
    internal bool RulesetsMenuOpen = false;
    internal bool RecolorCursors = false;

    private void Awake()
    {
        Instance = this;
        LogGlobal = this.Logger;

        SelectedRulesetIds = new Dictionary<TourneyMode, string>();
        SelectedRulesets = new Dictionary<TourneyMode, Ruleset>();
        
        SetTracker.Init();
        StageStrikeTracker.Init();
        UIUtils.Init();
        
        HarmonyPatches.PatchAll();
        RulesetIO.Init();

        VoteButton.ActiveVoteButtons = new List<VoteButton>();

        Configs.BindConfigs();
        Config.SettingChanged += (sender, args) => OnConfigChanged();
        OnConfigChanged();
        ModDependenciesUtils.RegisterToModMenu(this.Info, GetModMenuText());
    }

    private void OnConfigChanged()
    {
        SelectedRulesetIds[TourneyMode.NONE] = defaultRulesetId;
        SelectedRulesetIds[TourneyMode.LOCAL_1V1] = Configs.RulesetLocal1v1.Value;
        SelectedRulesetIds[TourneyMode.LOCAL_DOUBLES] = Configs.RulesetLocalDoubles.Value;
        SelectedRulesetIds[TourneyMode.LOCAL_CREW] = Configs.RulesetLocalCrew.Value;
        SelectedRulesetIds[TourneyMode.ONLINE_1V1] = Configs.RulesetOnline1v1.Value;
        
        SelectedRulesetIds.ToList().ForEach(entry =>
        {
            TourneyMode mode = entry.Key;
            string id = entry.Value;

            Ruleset ruleset = RulesetIO.GetRulesetById(id);
            if (ruleset == null)
            {
                //LogGlobal.LogError($"Error loading ruleset for tourney mode {mode}: ruleset `{id}` does not exist! Loading ruleset `{defaultRulesetId}` instead...");
                ruleset = RulesetIO.GetRulesetById(defaultRulesetId);
            }
            if (!SelectedRulesets.ContainsKey(mode) || SelectedRulesets[mode] != ruleset) LogGlobal.LogInfo($"Loaded ruleset for tourney mode {GetModeName(mode)}: `{ruleset.Id}`");

            SelectedRulesets[mode] = ruleset;
        });
    }

    internal static string PrintArray<T>(T[] arr, bool includeBrackets)
    {
        string s = "";
        if (includeBrackets) s += "[";

        for (int i = 0; i < arr.Length; i++)
        {
            if (i != 0) s += ", ";
            s += arr[i].ToString();
        }
        
        if (includeBrackets) s += "]";
        return s;
    }

    internal static string GetModeName(TourneyMode tourneyMode, bool capitalized = false)
    {
        return tourneyMode switch
        {
            TourneyMode.LOCAL_1V1 => capitalized ? "Local 1v1" : "local 1v1",
            TourneyMode.LOCAL_DOUBLES => capitalized ? "Local Doubles" : "local doubles",
            TourneyMode.LOCAL_CREW => capitalized ? "Crew Battle" : "crew battle",
            TourneyMode.ONLINE_1V1 => capitalized ? "Online 1v1" : "online 1v1",
            _ => capitalized ? "Vanilla" : "none"
        };
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
