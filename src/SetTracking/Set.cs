using System.Collections.Generic;
using LLHandlers;

namespace TourneyMod.SetTracking;

internal class Set
{
    internal List<Match> CompletedMatches { get; private set; } = new List<Match>();
    private Match currentMatch;

    internal bool IsGame1 => CompletedMatches.Count == 0;
    internal int GameNumber => CompletedMatches.Count + 1;
    internal int[] WinCounts
    {
        get
        {
            int[] winCounts = [0, 0, 0, 0];
            CompletedMatches.ForEach(match => winCounts[match.Winner]++);
            return winCounts;
        }
    }

    internal void StartMatch(Stage stage)
    {
        SetTracker.Log.LogInfo($"Starting new match on stage {stage}");
        currentMatch = new Match();
        currentMatch.Start(stage);
    }

    internal void EndMatch(int[] scores)
    {
        SetTracker.Log.LogInfo($"Ending match with stocks {string.Join(", ", [scores[0].ToString(), scores[1].ToString(), scores[2].ToString(), scores[3].ToString()])}");
        currentMatch.End(scores);

        int winner = currentMatch.Winner;
        if (winner == -1) return;
        SetTracker.Log.LogInfo($"Match ended with winner P{winner+1}");
        
        CompletedMatches.Add(currentMatch);
        currentMatch = null;
    }
}