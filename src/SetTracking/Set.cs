using System.Collections.Generic;
using LLHandlers;

namespace TourneyMod.SetTracking;

internal class Set
{
    internal List<Match> CompletedMatches { get; private set; } = new List<Match>();
    internal Match CurrentMatch { get; private set; }

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

    internal void StartMatch(Stage stage, Character[] selectedCharacters, Character[] playedCharacters)
    {
        SetTracker.Log.LogInfo($"Starting new match: stage {stage}, characters selected {Plugin.PrintArray(selectedCharacters, true)} played {Plugin.PrintArray(playedCharacters, true)}");
        CurrentMatch = new Match();
        CurrentMatch.Start(stage, selectedCharacters, playedCharacters);
    }

    internal void EndMatch(int[] scores)
    {
        CurrentMatch.End(scores);
        int winner = CurrentMatch.Winner;
        
        SetTracker.Log.LogInfo($"Ending match with stocks {Plugin.PrintArray(scores, true)}. winner: {(winner != -1 ? $"P{winner+1}" : "none")}");
        if (winner == -1) return;
        
        CompletedMatches.Add(CurrentMatch);
        CurrentMatch = null;
    }
}