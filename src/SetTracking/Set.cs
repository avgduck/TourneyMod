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
    internal PlayerCharacter[] PlayerCharacterLock = [PlayerCharacter.EMPTY, PlayerCharacter.EMPTY, PlayerCharacter.EMPTY, PlayerCharacter.EMPTY];

    internal bool IsFreePickMode = false;
    internal bool IsFreePickForced => ActiveRuleset.banAmounts.Length == 0;
    
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
            for (int teamNr = 0; teamNr < 4; teamNr++)
            {
                winCounts[teamNr] += WinCountOverride[teamNr];
            }
            return winCounts;
        }
    }

    internal int TotalWins => WinCounts.Sum();
    internal bool IsGame1 => TotalWins == 0;
    internal int GameNumber => TotalWins + 1;
    internal Team LastWinner => LastWinnerOverride == Team.NONE 
        ? (IsGame1 ? Team.NONE : CompletedMatches.Last().Winner)
        : LastWinnerOverride;
    
    internal int[] WinCountOverride { get; private set; }
    internal int TotalOverrideWins => WinCountOverride.Sum();
    internal Team LastWinnerOverride { get; private set; }

    internal Set(Ruleset ruleset)
    {
        ActiveRuleset = ruleset;
        WinCountOverride = [0, 0, 0, 0];
        LastWinnerOverride = Team.NONE;
    }

    internal void StartMatch(Stage stage, PlayerCharacter[] playerCharacters)
    {
        SetTracker.Log.LogInfo($"Starting new match: stage {stage}, characters selected {Plugin.PrintArray(playerCharacters, true)}");
        CurrentMatch = new Match();
        CurrentMatch.Start(stage, playerCharacters);
    }

    internal void EndMatch(PlayerScore[] scores)
    {
        CurrentMatch.End(scores);
        Team winner = CurrentMatch.Winner;
        
        SetTracker.Log.LogInfo($"Ending match with scores {Plugin.PrintArray(scores, true)}. winning team: {winner}");
        if (winner == Team.NONE) return;

        LastWinnerOverride = Team.NONE;
        if (ActiveRuleset.winnerCharacterLock || SetTracker.Instance.ActiveTourneyMode is TourneyMode.LOCAL_CREW)
        {
            Player.ForAll((Player player) =>
            {
                PlayerCharacterLock[player.nr] = player.Team == winner ? new PlayerCharacter(player.CharacterSelected, player.CharacterVariant) : PlayerCharacter.EMPTY;
            });
        }
        
        CompletedMatches.Add(CurrentMatch);
        CurrentMatch = null;
    }

    internal void AdjustWinCountOverride(Team team, int change)
    {
        if (team == Team.NONE) return;
        if (WinCountOverride[(int)team] == 0 && change < 0) return;
        WinCountOverride[(int)team] += change;

        if (TotalOverrideWins == 0) LastWinnerOverride = Team.NONE;
        else if (WinCountOverride[(int)team] == 0)
        {
            if (team == Team.RED) LastWinnerOverride = Team.BLUE;
            else if (team == Team.BLUE) LastWinnerOverride = Team.RED;
        }
        else if (change > 0) LastWinnerOverride = team;
    }

    internal void SetLastWinnerOverride(Team team)
    {
        LastWinnerOverride = team;
    }
}