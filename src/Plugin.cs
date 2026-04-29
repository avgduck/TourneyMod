using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using LLBML.Utils;
using TourneyMod.Patches;
using TourneyMod.PlayerTags;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using TourneyMod.UI;

namespace TourneyMod;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(DEPENDENCY_LLBML, BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(DEPENDENCY_MODMENU, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(DEPENDENCY_CURSORSPEED, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(DEPENDENCY_CHARACTERREROLL, BepInDependency.DependencyFlags.SoftDependency)]
[BepInIncompatibility(INCOMPATIBILITY_STAGESELECT)]
[BepInProcess("LLBlaze.exe")]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "avgduck.plugins.llb.tourneymod";
    internal const string DEPENDENCY_LLBML = "fr.glomzubuk.plugins.llb.llbml";
    internal const string DEPENDENCY_MODMENU = "no.mrgentle.plugins.llb.modmenu";
    internal const string DEPENDENCY_CURSORSPEED = "com.github.daioutzu.cursorspeed";
    internal const string DEPENDENCY_CHARACTERREROLL = "avgduck.plugins.llb.characterreroll";
    internal const string INCOMPATIBILITY_STAGESELECT = "com.github.daioutzu.stageselect";
    
    internal static Plugin Instance { get; private set; }
    internal static ManualLogSource LogGlobal { get; private set; }

    private const string defaultRulesetId = "all_stages";
    internal Dictionary<TourneyMode, string> SelectedRulesetIds;
    internal Dictionary<TourneyMode, Ruleset> SelectedRulesets;

    internal string[] SelectedPlayerTagNames;
    internal PlayerTag[] SelectedPlayerTags;
    
    internal bool TourneyMenuOpen = false;
    internal bool RulesetsMenuOpen = false;
    internal bool SetPreviewMenuOpen = false;
    internal bool ScoreEditMenuOpen = false;
    internal bool RecolorCursors = false;

    private void Awake()
    {
        Instance = this;
        LogGlobal = this.Logger;
        
        Assets.Init();

        SelectedRulesetIds = new Dictionary<TourneyMode, string>();
        SelectedRulesets = new Dictionary<TourneyMode, Ruleset>();
        
        SetTracker.Init();
        StageStrikeTracker.Init();
        
        UIUtils.Init();
        Cursors.LoadCursorImages();
        
        HarmonyPatches.PatchAll();
        RulesetIO.Init();
        PlayerTagIO.Init();

        SelectedPlayerTagNames = ["", "", "", ""];
        SelectedPlayerTags = [PlayerTag.DEFAULT, PlayerTag.DEFAULT, PlayerTag.DEFAULT, PlayerTag.DEFAULT];

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

        SelectedPlayerTagNames[0] = Configs.SelectedTagPlayer1.Value;
        SelectedPlayerTagNames[1] = Configs.SelectedTagPlayer2.Value;
        SelectedPlayerTagNames[2] = Configs.SelectedTagPlayer3.Value;
        SelectedPlayerTagNames[3] = Configs.SelectedTagPlayer4.Value;

        for (int playerNr = 0; playerNr < 4; playerNr++)
        {
            PlayerTag selectedTag = PlayerTagIO.GetPlayerTagByName(SelectedPlayerTagNames[playerNr].ToLower());

            if (selectedTag == null)
            {
                //if (SelectedPlayerTags[playerNr] != PlayerTag.DEFAULT) LogGlobal.LogWarning($"Could not find P{playerNr} selected player tag '{SelectedPlayerTagNames[playerNr]}': setting to default");
                SelectedPlayerTags[playerNr] = PlayerTag.DEFAULT;
            }
            else
            {
                if (SelectedPlayerTags[playerNr] != selectedTag) LogGlobal.LogInfo($"Setting P{playerNr} selected player tag '{selectedTag.GetName()}'");
                SelectedPlayerTags[playerNr] = selectedTag;
            }
        }
    }

    internal void SelectPlayerTag(int playerNr, PlayerTag playerTag)
    {
        SelectedPlayerTags[playerNr] = playerTag;

        if (playerNr == 0) Configs.SelectedTagPlayer1.Value = playerTag.GetName();
        else if (playerNr == 1) Configs.SelectedTagPlayer2.Value = playerTag.GetName();
        else if (playerNr == 2) Configs.SelectedTagPlayer3.Value = playerTag.GetName();
        else if (playerNr == 3) Configs.SelectedTagPlayer4.Value = playerTag.GetName();
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
