using BepInEx.Logging;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;

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
    
    internal StrikeInfo CurrentStrikeInfo { get; private set; }
    internal bool IsTrackingStrikeInfo => CurrentStrikeInfo != null;

    internal void Start()
    {
        //Log.LogInfo("Starting new strike info");
        CurrentStrikeInfo = new StrikeInfo();
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