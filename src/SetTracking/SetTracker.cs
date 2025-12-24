using BepInEx.Logging;
using LLBML.Players;
using LLBML.Settings;
using TourneyMod.Rulesets;

namespace TourneyMod.SetTracking;

internal class SetTracker
{
    internal static SetTracker Instance { get; private set; }
    internal static ManualLogSource Log { get; private set; }
    internal static void Init()
    {
        Instance = new SetTracker();
        Log = BepInEx.Logging.Logger.CreateLogSource("TM SetTracking");
        Log.LogInfo("TourneyMod set tracking initialized");
    }
    
    internal Set CurrentSet { get; private set; }
    internal bool IsTrackingSet => CurrentSet != null;
    
    internal Ruleset DefaultRuleset { get; private set; }
    internal void FindDefaultRuleset()
    {
        DefaultRuleset = RulesetIO.GetRulesetById("all_stages");
    }
    
    internal TourneyMode ActiveTourneyMode = TourneyMode.NONE;
    
    internal int NumPlayersInMatch
    {
        get
        {
            int sum = 0;
            Player.ForAllInMatch(player =>
            {
                if (!player.IsAI) sum++;
            });
            return sum;
        }
    }

    internal void Start()
    {
        Log.LogInfo("Starting new set");
        bool forceDefault = ActiveTourneyMode == TourneyMode.NONE;
        if (forceDefault) Log.LogInfo("Tourney mode not active! Forcing default ruleset 'all_stages'");
        else if (Plugin.Instance.SelectedRuleset == null) Log.LogError("No valid ruleset selected! Forcing default ruleset 'all_stages'");
        else Log.LogInfo($"Using ruleset `{Plugin.Instance.SelectedRuleset.Id}`");
        CurrentSet = new Set(Plugin.Instance.SelectedRuleset != null && !forceDefault ? Plugin.Instance.SelectedRuleset : DefaultRuleset);
    }

    internal void End()
    {
        if (CurrentSet == null)
        {
            Log.LogWarning("Tried to end set, but there wasn't one active");
            return;
        }
        
        Log.LogInfo("Ending set");
        CurrentSet = null;
    }

    internal void Reset()
    {
        if (IsTrackingSet) End();
        Start();
    }
}