using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyJson;

namespace TourneyMod;

internal static class RulesetIO
{
    private const string DIRECTORY_NAME_DEFAULT = "rulesets-default";
    private const string DIRECTORY_NAME_CUSTOM = "rulesets-custom";
    
    internal static DirectoryInfo RulesetsDirectoryDefault;
    internal static DirectoryInfo RulesetsDirectoryCustom;

    internal static List<Ruleset> RulesetsDefault;
    internal static List<Ruleset> RulesetsCustom;

    internal static void Init()
    {
        string defaultPath = Path.Combine(Path.GetDirectoryName(Plugin.Instance.Info.Location), DIRECTORY_NAME_DEFAULT);
        if (!Directory.Exists(defaultPath)) Directory.CreateDirectory(defaultPath);
        RulesetsDirectoryDefault = new DirectoryInfo(defaultPath);
        
        DirectoryInfo moddingFolder = LLBML.Utils.ModdingFolder.GetModSubFolder(Plugin.Instance.Info);
        string customPath = Path.Combine(moddingFolder.FullName, DIRECTORY_NAME_CUSTOM);
        if (!Directory.Exists(customPath)) Directory.CreateDirectory(customPath);
        RulesetsDirectoryCustom = new DirectoryInfo(customPath);

        RulesetsDefault = new List<Ruleset>();
        foreach (FileInfo file in RulesetsDirectoryDefault.GetFiles().OrderBy(f => f.Name))
        {
            Ruleset ruleset = LoadRulesetFile(file);
            Plugin.LogGlobal.LogInfo($"Loaded default ruleset: {ruleset}");
            RulesetsDefault.Add(ruleset);
        }
        
        RulesetsCustom = new List<Ruleset>();
        foreach (FileInfo file in RulesetsDirectoryCustom.GetFiles().OrderBy(f => f.Name))
        {
            Ruleset ruleset = LoadRulesetFile(file);
            Plugin.LogGlobal.LogInfo($"Loaded custom ruleset: {ruleset}");
            RulesetsCustom.Add(ruleset);
        }
    }

    private static Ruleset LoadRulesetFile(FileInfo file)
    {
        string json = JsonIO.ReadFile(file);
        Ruleset ruleset = json.FromJson<Ruleset>();
        return ruleset;
    }

    internal static Ruleset GetRulesetById(string id)
    {
        Ruleset ruleset = RulesetsDefault.Find(rs => rs.id == id);
        if (ruleset != null) return ruleset;
        
        ruleset = RulesetsCustom.Find(rs => rs.id == id);
        if (ruleset != null) return ruleset;
        
        return null;
    }
}