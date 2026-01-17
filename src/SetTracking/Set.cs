using System.Collections.Generic;
using System.Linq;
using LLBML.Math;
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
    internal int[] PlayerStockLock = [0, 0, 0, 0];
    internal Stage StageLock = Stage.NONE;

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
    internal bool IsTiebreaker { get; private set; }

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

    internal void EndMatch(PlayerScore[] scores, bool isTimeout)
    {
        CurrentMatch.End(scores, GameNumber, isTimeout, IsTiebreaker);
        Team winner = CurrentMatch.Winner;
        
        if (isTimeout)
        {
            int maxStocks = 0;
            Team stockWinner = Team.NONE;
            for (int playerNr = 0; playerNr < 4; playerNr++)
            {
                int score = scores[playerNr].Stocks;
                if (score == 0) continue;
                if (score > maxStocks)
                {
                    maxStocks = score;
                    stockWinner = SetTracker.Instance.GetPlayerTeam(playerNr);
                }
                else if (score == maxStocks)
                {
                    stockWinner = Team.NONE;
                }
            }
            
            SetTracker.Log.LogInfo($"Match ended in timeout! Checking stocks {Plugin.PrintArray(scores, true)}: winner {stockWinner}");
            
            if (stockWinner != Team.NONE)
            {
                winner = stockWinner;
            }
            else
            {
                Floatf maxHp = Floatf.zero;
                Team hpWinner = Team.NONE;
                for (int playerNr = 0; playerNr < 4; playerNr++)
                {
                    int score = scores[playerNr].Stocks;
                    if (score < maxStocks) continue;
                    
                    Floatf hp = scores[playerNr].Hp;
                    if (Floatf.Equals(hp, Floatf.zero)) continue;
                    if (Floatf.GreaterThan(hp, maxHp))
                    {
                        maxHp = hp;
                        hpWinner = SetTracker.Instance.GetPlayerTeam(playerNr);
                    }
                    else if (Floatf.Equals(hp, maxHp))
                    {
                        hpWinner = Team.NONE;
                    }
                }
                
                SetTracker.Log.LogInfo($"Tie on stocks! Checking hp {Plugin.PrintArray(scores.Select(s => $"({s.Team}, {Floatf.ToFloat(s.Hp)})").ToArray(), true)}: winner {hpWinner}");

                if (hpWinner != Team.NONE)
                {
                    winner = hpWinner;
                }
                else
                {
                    winner = Team.NONE;
                }
            }
        }
        else
        {
            SetTracker.Log.LogInfo($"Ending match with scores {Plugin.PrintArray(scores, true)}");
        }
        
        SetTracker.Log.LogInfo($"Determined match winner: {winner}");
        CurrentMatch.Winner = winner;
        Player.ForAll((Player player) =>
        {
            PlayerCharacterLock[player.nr] = PlayerCharacter.EMPTY;
            PlayerStockLock[player.nr] = 0;
        });
        if (CurrentMatch.Winner == Team.NONE)
        {
            if (isTimeout)
            {
                SetTracker.Log.LogInfo("Timeout tiebreaker needed!");
                Player.ForAllInMatch((Player player) =>
                {
                    PlayerCharacterLock[player.nr] = new PlayerCharacter(player.CharacterSelected, player.CharacterVariant, player.Team);
                    PlayerStockLock[player.nr] = 1;
                });
                StageLock = CurrentMatch.PlayedStage;
                IsTiebreaker = true;
            }
            else
            {
                return;
            }
        }
        else
        {
            StageLock = Stage.NONE;
            IsTiebreaker = false;
            
            if (ActiveRuleset.winnerCharacterLock || SetTracker.Instance.ActiveTourneyMode is TourneyMode.LOCAL_CREW)
            {
                Player.ForAllInMatch((Player player) =>
                {
                    PlayerCharacterLock[player.nr] = player.Team == winner ? new PlayerCharacter(player.CharacterSelected, player.CharacterVariant, player.Team) : PlayerCharacter.EMPTY;
                    PlayerStockLock[player.nr] = SetTracker.Instance.ActiveTourneyMode is TourneyMode.LOCAL_CREW ? scores[player.nr].Stocks : 0;
                });
            }
        }

        LastWinnerOverride = Team.NONE;
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