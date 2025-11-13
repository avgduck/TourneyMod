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
        Stage.STADIUM,
        Stage.STREETS,
    ];

    internal Stage[] StagesCounterpick =
    [
        Stage.POOL,
        Stage.SUBWAY,
        Stage.FACTORY,
        Stage.CONSTRUCTION,
    ];

    internal static SetTracker Instance { get; private set; }
    internal static bool IsTrackingSet => Instance != null;
    private List<Match> completedMatches;
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
        completedMatches = new List<Match>();
        matchCount = 0;
        
        RecalculateStageBans();
    }

    internal void StartMatch(Stage stage)
    {
        Plugin.LogGlobal.LogInfo($"Starting new match on stage {stage}");
        currentMatch = new Match(stage);
    }

    internal void EndMatch(int[] scores)
    {
        currentMatch.SetScores(scores);
        Plugin.LogGlobal.LogInfo($"Ending match with stocks {string.Join(", ", [scores[0].ToString(), scores[1].ToString(), scores[2].ToString(), scores[3].ToString()])}");

        if (currentMatch.GetWinner() == -1) return;
        
        Plugin.LogGlobal.LogInfo($"Match ended with winner P{currentMatch.GetWinner()+1}");
        completedMatches.Add(currentMatch);
        matchCount++;
        currentMatch = null;
        
        RecalculateStageBans();
    }

    private void RecalculateStageBans()
    {
        stageBans = new List<StageBan>();
        if (matchCount == 0)
        {
            foreach (Stage stage in StagesCounterpick)
            {
                stageBans.Add(new StageBan(stage, StageBan.BanReason.COUNTERPICK));
            }
        }

        foreach (Match match in completedMatches)
        {
            int winner = match.GetWinner();
            
            StageBan stageBan = stageBans.Find(ban => ban.stage == match.stage);
            if (stageBan != null)
            {
                if (stageBan.reason == StageBan.BanReason.DSR)
                {
                    stageBan.banPlayer = -1;
                }

                continue;
            }
            
            stageBans.Add(new StageBan(match.stage, StageBan.BanReason.DSR, winner));
        }
    }

    internal List<StageBan> GetStageBans()
    {
        return stageBans;
    }

    internal void BanStage(Stage stage, int playerNumber)
    {
        StageBan existingBan = stageBans.Find((ban) => ban.stage == stage);
        StageBan newBan = new StageBan(stage, StageBan.BanReason.BAN, playerNumber);
        if (existingBan != null)
        {
            stageBans.Remove(existingBan);
        }
        stageBans.Add(newBan);
    }

    internal bool CheckPlayerInteraction(Stage stage, int playerNumber)
    {
        StageBan stageBan = stageBans.Find((ban) => ban.stage == stage);
        return CheckPlayerInteraction(stageBan, playerNumber);
    }

    internal bool CheckPlayerInteraction(StageBan stageBan, int playerNumber)
    {
        if (stageBan == null) return true;
        if (stageBan.reason != SetTracker.StageBan.BanReason.DSR) return false;
        if (stageBan.banPlayer == playerNumber || stageBan.banPlayer == -1) return false;
        return true;
    }

    private class Match
    {
        internal Stage stage;
        internal int[] scores;

        internal Match(Stage stage)
        {
            this.stage = stage;
        }

        internal void SetScores(int[] scores)
        {
            this.scores = scores;
        }

        internal int GetWinner()
        {
            if (scores == null) return -1;
            int deadPlayer = -1;
            int alivePlayer = -1;
            for (int playerNumber = 0; playerNumber < 4; playerNumber++)
            {
                if (scores[playerNumber] == 0) deadPlayer = playerNumber;
                else if (scores[playerNumber] > 0) alivePlayer = playerNumber;
            }

            if (deadPlayer == -1 || alivePlayer == -1) return -1;
            return alivePlayer;
        }
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