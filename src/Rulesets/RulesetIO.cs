using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyJson;

namespace TourneyMod.Rulesets;

internal static class RulesetIO
{
    private const string DIRECTORY_NAME_DEFAULT = "rulesets-default";
    private const string DIRECTORY_NAME_CUSTOM = "rulesets-custom";
    
    internal static DirectoryInfo RulesetsDirectoryDefault;
    internal static DirectoryInfo RulesetsDirectoryCustom;

    internal static Dictionary<string, Ruleset> RulesetsDefault;
    internal static Dictionary<string, Ruleset> RulesetsCustom;

    internal static void Init()
    {
        string defaultPath = Path.Combine(Path.GetDirectoryName(Plugin.Instance.Info.Location), DIRECTORY_NAME_DEFAULT);
        if (!Directory.Exists(defaultPath)) Directory.CreateDirectory(defaultPath);
        RulesetsDirectoryDefault = new DirectoryInfo(defaultPath);
        
        DirectoryInfo moddingFolder = LLBML.Utils.ModdingFolder.GetModSubFolder(Plugin.Instance.Info);
        string customPath = Path.Combine(moddingFolder.FullName, DIRECTORY_NAME_CUSTOM);
        if (!Directory.Exists(customPath)) Directory.CreateDirectory(customPath);
        RulesetsDirectoryCustom = new DirectoryInfo(customPath);

        RulesetsDefault = new Dictionary<string, Ruleset>();
        foreach (FileInfo file in RulesetsDirectoryDefault.GetFiles().OrderBy(f => f.Name))
        {
            Ruleset ruleset = LoadRulesetFile(file);

            if (ruleset == null) continue;
            if (RulesetsDefault.ContainsKey(ruleset.Id))
            {
                Plugin.LogGlobal.LogWarning($"Failed to load default ruleset '{ruleset.Id}': ruleset with id '{ruleset.Id}' already exists");
                continue;
            }
            
            Plugin.LogGlobal.LogInfo($"Loaded default ruleset: {ruleset.Id}");
            RulesetsDefault.Add(ruleset.Id, ruleset);
        }
        
        RulesetsCustom = new Dictionary<string, Ruleset>();
        foreach (FileInfo file in RulesetsDirectoryCustom.GetFiles().OrderBy(f => f.Name))
        {
            Ruleset ruleset = LoadRulesetFile(file);
            
            if (ruleset == null) continue;
            if (RulesetsDefault.ContainsKey(ruleset.Id) || RulesetsCustom.ContainsKey(ruleset.Id))
            {
                Plugin.LogGlobal.LogWarning($"Failed to load custom ruleset '{ruleset.Id}': ruleset with id '{ruleset.Id}' already exists");
                continue;
            }
            
            Plugin.LogGlobal.LogInfo($"Loaded custom ruleset: {ruleset.Id}");
            RulesetsCustom.Add(ruleset.Id, ruleset);
        }
    }

    private static Ruleset LoadRulesetFile(FileInfo file)
    {
        if (!file.Name.Contains(".json")) return null;
        string id = file.Name.Replace(".json", "");
        
        string json = JsonIO.ReadFile(file);
        Ruleset ruleset = json.FromJson<Ruleset>();
        ruleset.InitId(id);
        return ruleset;
    }

    internal static Ruleset GetRulesetById(string id)
    {
        if (RulesetsDefault.ContainsKey(id)) return RulesetsDefault[id];
        if (RulesetsCustom.ContainsKey(id)) return RulesetsCustom[id];
        
        return null;
    }
}