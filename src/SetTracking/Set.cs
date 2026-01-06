using System.Collections.Generic;
using System.Linq;
using LLBML.Players;
using LLHandlers;
using TourneyMod.Rulesets;

namespace TourneyMod.SetTracking;

internal class Set
{
    internal List<Match> CompletedMatches { get; private set; } = new List<Match>();
    internal Match CurrentMatch { get; private set; }
    internal Ruleset ActiveRuleset { get; private set; }

    internal bool IsFreePickMode = false;
    internal bool IsFreePickForced => ActiveRuleset.banAmounts.Length == 0;
    internal bool IsGame1 => CompletedMatches.Count == 0;
    internal int GameNumber => CompletedMatches.Count + 1;
    internal int[] WinCounts
    {
        get
        {
            int[] winCounts = [0, 0, 0, 0];
            CompletedMatches.ForEach(match =>
            {
                Team winner = match.Winner;
                if (winner == Team.NONE) return;
                winCounts[(int)match.Winner]++;
            });
            return winCounts;
        }
    }
    internal Team LastWinner => IsGame1 ? Team.NONE : CompletedMatches.Last().Winner;

    internal Set(Ruleset ruleset)
    {
        ActiveRuleset = ruleset;
    }

    internal void StartMatch(Stage stage, Character[] selectedCharacters, Character[] playedCharacters)
    {
        SetTracker.Log.LogInfo($"Starting new match: stage {stage}, characters selected {Plugin.PrintArray(selectedCharacters, true)} played {Plugin.PrintArray(playedCharacters, true)}");
        CurrentMatch = new Match();
        CurrentMatch.Start(stage, selectedCharacters, playedCharacters);
    }

    internal void EndMatch(PlayerScore[] scores)
    {
        CurrentMatch.End(scores);
        Team winner = CurrentMatch.Winner;
        
        SetTracker.Log.LogInfo($"Ending match with scores {Plugin.PrintArray(scores, true)}. winning team: {winner}");
        if (winner == Team.NONE) return;
        
        CompletedMatches.Add(CurrentMatch);
        CurrentMatch = null;
    }
}