using BepInEx.Configuration;

namespace TourneyMod;

public static class Configs
{
    internal static ConfigEntry<string> SelectedRulesetId { get; private set; }

    internal static void BindConfigs()
    {
        ConfigFile config = Plugin.Instance.Config;

        SelectedRulesetId = config.Bind<string>("Settings", "SelectedRulesetId", "standard_online", "Selected ruleset (default or custom) specified by the id field");
    }
}