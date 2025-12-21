using BepInEx.Logging;
using TourneyMod.Rulesets;

namespace TourneyMod.StageStriking;

internal class StageStrikeTracker
{
    internal static StageStrikeTracker Instance { get; private set; }
    internal static ManualLogSource Log { get; private set; }
    internal static void Init()
    {
        Instance = new StageStrikeTracker();
        Log = BepInEx.Logging.Logger.CreateLogSource("TM StageStriking");
        Log.LogInfo("TourneyMod stage striking initialized");
    }

    private Ruleset defaultRuleset;
    internal void FindDefaultRuleset()
    {
        defaultRuleset = RulesetIO.GetRulesetById("all_stages");
    }
    
    internal StrikeInfo CurrentStrikeInfo { get; private set; }
    internal bool IsTrackingStrikeInfo => CurrentStrikeInfo != null;

    internal void Start()
    {
        //Log.LogInfo("Starting new strike info");
        bool forceDefault = Plugin.Instance.ActiveTourneyMode == TourneyMode.NONE;
        if (forceDefault) Log.LogInfo("Tourney mode not active! Forcing default ruleset 'all_stages'");
        if (Plugin.Instance.SelectedRuleset == null) Log.LogError("No valid ruleset selected! Forcing default ruleset 'all_stages'");
        CurrentStrikeInfo = new StrikeInfo(Plugin.Instance.SelectedRuleset != null && !forceDefault ? Plugin.Instance.SelectedRuleset : defaultRuleset);
    }

    internal void End()
    {
        if (CurrentStrikeInfo == null)
        {
            Log.LogWarning("Tried to end strike info, but there wasn't one active");
            return;
        }
        
        //Log.LogInfo("Ending strike info");
        CurrentStrikeInfo = null;
    }
}