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

    internal bool IsMode1v1 => ActiveTourneyMode switch
    {
        TourneyMode.LOCAL_1V1 or TourneyMode.LOCAL_CREW => true,
        _ => false
    };
    internal bool IsModeDoubles => ActiveTourneyMode switch
    {
        TourneyMode.LOCAL_DOUBLES => true,
        _ => false
    };
    
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
        Log.LogInfo($"Starting new set in tourney mode {Plugin.GetModeName(ActiveTourneyMode)}, using ruleset {Plugin.Instance.SelectedRulesets[ActiveTourneyMode].Id}");
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

    internal Team GetPlayerTeam(int playerNumber)
    {
        if (IsMode1v1) return playerNumber switch
        {
            0 => Team.RED,
            1 => Team.BLUE,
            _ => Team.NONE
        };
        if (IsModeDoubles) return playerNumber switch
        {
            0 or 1 => Team.RED,
            2 or 3 => Team.BLUE,
            _ => Team.NONE
        };

        return Team.NONE;
    }
    
    internal void ApplyGameOptions(GameSettings settings, GameOptions gameOptions)
    {
        settings.stocks = gameOptions.stocks;
        settings.timeInfinite = gameOptions.timeInfinite;
        settings.time = gameOptions.time;
        settings.energy = gameOptions.energy;
        settings.useHP = gameOptions.hpFactor;
        settings.MinSpeed = gameOptions.minBallSpeed;
        settings.mMinSpeed = gameOptions.minBallSpeed;
        settings.ballType = gameOptions.ballType;
        settings.PowerupSelection = gameOptions.powerupSelection;
        settings.havePowerups = gameOptions.powerupSelection;
    }

    internal void ApplyInfiniteTimer(GameSettings settings)
    {
        settings.timeInfinite = true;
    }
}