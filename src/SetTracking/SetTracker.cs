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
        Log.LogInfo($"Starting new set in tourney mode {ActiveTourneyMode}, using ruleset {Plugin.Instance.SelectedRulesets[ActiveTourneyMode].Id}");
        CurrentSet = new Set(Plugin.Instance.SelectedRulesets[ActiveTourneyMode]);
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