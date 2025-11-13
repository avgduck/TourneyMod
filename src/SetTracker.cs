using System.Collections.Generic;
using LLHandlers;

namespace TourneyMod;

internal class SetTracker
{
    internal Stage[] StagesNeutral =
    [
        Stage.JUNKTOWN,
        Stage.ROOM21,
        (Plugin.USE_SEWERS ? Stage.SEWERS : Stage.OUTSKIRTS),
        Stage.SUBWAY,
        Stage.STREETS,
    ];

    internal Stage[] StagesCounterpick =
    [
        Stage.POOL,
        Stage.STADIUM,
        Stage.FACTORY,
        Stage.CONSTRUCTION,
    ];

    internal static SetTracker Instance { get; private set; }
    internal static bool IsTrackingSet => Instance != null;
    private List<Match> matches;
    private int matchCount;
    private Match currentMatch;
    private List<StageBan> stageBans;

    internal static void StartSet()
    {
        Plugin.LogGlobal.LogInfo("Starting set tracker");
        Instance = new SetTracker();
    }

    internal static void EndSet()
    {
        Plugin.LogGlobal.LogInfo("Ending set tracker");
        Instance = null;
    }
    
    private SetTracker()
    {
        matches = new List<Match>();
        matchCount = 0;
    }

    internal void StartMatch()
    {
        Plugin.LogGlobal.LogInfo("Starting new match");
        currentMatch = new Match();
    }

    internal void EndMatch()
    {
        matches.Add(currentMatch);
        matchCount++;
        Plugin.LogGlobal.LogInfo("Ending match: total " + matchCount);
        currentMatch = null;
    }

    internal List<StageBan> GetStageBans()
    {
        stageBans = new List<StageBan>();

        if (matchCount == 0)
        {
            foreach (Stage stage in StagesCounterpick)
            {
                stageBans.Add(new StageBan(stage, StageBan.BanReason.COUNTERPICK));
            }
        }
        
        return stageBans;
    }

    private class Match
    {
    }

    internal class StageBan
    {
        internal Stage stage;
        internal BanReason reason;
        internal int banPlayer;

        internal StageBan(Stage stage, BanReason reason, int banPlayer = -1)
        {
            this.stage = stage;
            this.reason = reason;
            this.banPlayer = banPlayer;
        }

        internal enum BanReason
        {
            COUNTERPICK,
            DSR,
            BAN
        }
    }
}