using System.Collections.Generic;
using BepInEx.Configuration;

namespace TourneyMod;

public static class Configs
{
    internal static ConfigEntry<string> RulesetLocal1v1 { get; private set; }
    internal static ConfigEntry<string> RulesetLocalDoubles { get; private set; }
    internal static ConfigEntry<string> RulesetLocalCrew { get; private set; }
    internal static ConfigEntry<string> RulesetOnline1v1 { get; private set; }

    internal static void BindConfigs()
    {
        ConfigFile config = Plugin.Instance.Config;

        RulesetLocal1v1 = config.Bind<string>("Selected Rulesets", "RulesetLocal1v1", "standard_online", "Selected ruleset for local 1v1 mode");
        RulesetLocalDoubles = config.Bind<string>("Selected Rulesets", "RulesetLocalDoubles", "standard_online", "Selected ruleset for local doubles mode");
        RulesetLocalCrew = config.Bind<string>("Selected Rulesets", "RulesetLocalCrew", "standard_online", "Selected ruleset for crew battle mode");
        RulesetOnline1v1 = config.Bind<string>("Selected Rulesets", "RulesetOnline1v1", "standard_online", "Selected ruleset for online 1v1 mode");
    }
}