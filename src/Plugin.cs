using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using LLBML.Players;
using LLBML.Utils;
using LLHandlers;
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

    internal string SelectedPlayerTagNameKeyboard;
    internal string[] SelectedPlayerTagNames;
    internal PlayerTag SelectedPlayerTagKeyboard;
    internal PlayerTag[] SelectedPlayerTags;
    
    internal bool TourneyMenuOpen = false;
    internal bool RulesetsMenuOpen = false;
    internal bool SetPreviewMenuOpen = false;
    internal bool ScoreEditMenuOpen = false;
    internal bool RecolorCursors = false;
    internal bool[] InputMenuEditingTags = [false, false, false, false, false];

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

        SelectedPlayerTagNameKeyboard = "";
        SelectedPlayerTagNames = ["", "", "", ""];
        SelectedPlayerTagKeyboard = PlayerTag.DEFAULT;
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

        SelectedPlayerTagNameKeyboard = Configs.SelectedTagKeyboard.Value;
        SelectedPlayerTagNames[0] = Configs.SelectedTagController1.Value;
        SelectedPlayerTagNames[1] = Configs.SelectedTagController2.Value;
        SelectedPlayerTagNames[2] = Configs.SelectedTagController3.Value;
        SelectedPlayerTagNames[3] = Configs.SelectedTagController4.Value;
        
        PlayerTag selectedTagKeyboard = PlayerTagIO.GetPlayerTagByName(SelectedPlayerTagNameKeyboard.ToLower());
        if (selectedTagKeyboard == null)
        {
            SelectedPlayerTagKeyboard = PlayerTag.DEFAULT;
        }
        else
        {
            if (SelectedPlayerTagKeyboard != selectedTagKeyboard) LogGlobal.LogInfo($"Setting keyboard selected player tag '{selectedTagKeyboard.GetName()}'");
            SelectedPlayerTagKeyboard = selectedTagKeyboard;
        }

        for (int controllerNr = 0; controllerNr < 4; controllerNr++)
        {
            PlayerTag selectedTag = PlayerTagIO.GetPlayerTagByName(SelectedPlayerTagNames[controllerNr].ToLower());

            if (selectedTag == null)
            {
                //if (SelectedPlayerTags[playerNr] != PlayerTag.DEFAULT) LogGlobal.LogWarning($"Could not find P{playerNr} selected player tag '{SelectedPlayerTagNames[playerNr]}': setting to default");
                SelectedPlayerTags[controllerNr] = PlayerTag.DEFAULT;
            }
            else
            {
                if (SelectedPlayerTags[controllerNr] != selectedTag) LogGlobal.LogInfo($"Setting controller {controllerNr} selected player tag '{selectedTag.GetName()}'");
                SelectedPlayerTags[controllerNr] = selectedTag;
            }
        }
    }

    internal void SelectPlayerTag(int playerNr, PlayerTag playerTag)
    {
        Player player = Player.GetPlayer(playerNr);
        Rewired.Player rePlayer = player.controller.GetInputPlayer();
        
        if (rePlayer.id == 0)
        {
            SelectedPlayerTagKeyboard = playerTag;
            Configs.SelectedTagKeyboard.Value = playerTag.GetName();
            Plugin.LogGlobal.LogInfo($"Setting keyboard selected player tag '{playerTag.GetName()}'");
            return;
        }

        int controllerNr = rePlayer.id - 1;
        SelectedPlayerTags[controllerNr] = playerTag;
        Plugin.LogGlobal.LogInfo($"Setting controller {controllerNr} selected player tag '{playerTag.GetName()}'");

        if (controllerNr == 0) Configs.SelectedTagController1.Value = playerTag.GetName();
        else if (controllerNr == 1) Configs.SelectedTagController2.Value = playerTag.GetName();
        else if (controllerNr == 2) Configs.SelectedTagController3.Value = playerTag.GetName();
        else if (controllerNr == 3) Configs.SelectedTagController4.Value = playerTag.GetName();
    }

    internal PlayerTag GetPlayerTag(int playerNr)
    {
        Player player = Player.GetPlayer(playerNr);
        return GetPlayerTag(player.controller);
    }
    
    internal PlayerTag GetPlayerTag(Controller controller)
    {
        Rewired.Player rePlayer = controller.GetInputPlayer();
        if (rePlayer == null) return PlayerTag.DEFAULT;

        if (rePlayer.id == 0)
        {
            return SelectedPlayerTagKeyboard;
        }

        int controllerNr = rePlayer.id - 1;
        return SelectedPlayerTags[controllerNr];
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
