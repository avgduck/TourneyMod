using System.Collections.Generic;
using LLBML.Players;
using LLBML.Settings;
using LLHandlers;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace TourneyMod;

internal class SetTracker
{
    internal static SetTracker Instance { get; private set; }
    internal static bool IsTrackingSet => Instance != null;

    internal Ruleset ruleset;
    private List<Match> completedMatches;
    private int matchCount;
    private Match currentMatch;
    private List<StageBan> stageBans;
    internal int controlStartPlayer;
    internal int ControllingPlayer { get; private set; }
    internal bool IsFreePickMode { get; private set; }

    internal enum InteractMode
    {
        PICK,
        BAN
    }

    internal InteractMode CurrentInteractMode { get; private set; }
    internal int[] TotalBansRemaining { get; private set; }
    internal int CurrentBansRemaining { get; private set; }
    private int banIndex = 0;

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
        ruleset = RulesetIO.GetRulesetById(Configs.SelectedRulesetId.Value);
        Plugin.LogGlobal.LogInfo($"Loaded ruleset {ruleset}");
        controlStartPlayer = ruleset.game1FirstPlayer;
        banIndex = 0;
        IsFreePickMode = false;
        UpdateInteractMode();

        RecalculateStageBans();
    }

    internal static bool Is1v1 {
        get
        {
            if (GameSettings.current.gameMode != GameMode._1v1) return false;
            
            bool anyAIs = false;
            Player.ForAllInMatch((Player player) =>
            {
                if (player.IsAI) anyAIs = true;
            });

            return !anyAIs;
        }
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

        int winner = currentMatch.GetWinner();
        if (winner == -1) return;
        
        Plugin.LogGlobal.LogInfo($"Match ended with winner P{winner+1}");
        completedMatches.Add(currentMatch);
        matchCount++;
        currentMatch = null;

        int loser = winner == 0 ? 1 : 0;
        controlStartPlayer = ruleset.laterGamesFirstPlayer == Ruleset.FirstPlayer.WINNER ? winner : loser;
        banIndex = 0;
        UpdateInteractMode();
        
        RecalculateStageBans();
    }

    private void RecalculateStageBans()
    {
        stageBans = new List<StageBan>();
        if (matchCount == 0)
        {
            foreach (Stage stage in ruleset.stagesCounterpick)
            {
                stageBans.Add(new StageBan(stage, StageBan.BanReason.COUNTERPICK));
            }
        }

        if (ruleset.dsrMode == Ruleset.DsrMode.OFF) return;
        
        Match[] lastWins = new Match[4];
        foreach (Match match in completedMatches)
        {
            int winner = match.GetWinner();
            lastWins[winner] = match;
        }
        
        foreach (Match match in completedMatches)
        {
            int winner = match.GetWinner();
            Match lastWin = lastWins[winner];
            
            StageBan stageBan = stageBans.Find(ban => ban.stage == match.stage);
            if (stageBan != null && stageBan.reason != StageBan.BanReason.DSR) continue;
            if (ruleset.dsrMode == Ruleset.DsrMode.LAST_WIN && match != lastWin) continue;

            if (stageBan != null)
            {
                if (stageBan.banPlayer != winner) stageBan.banPlayer = -1;
            }
            else
            {
                stageBans.Add(new StageBan(match.stage, StageBan.BanReason.DSR, winner));
            }
        }
    }

    internal void ResetBans()
    {
        banIndex = 0;
        RecalculateStageBans();
        UpdateInteractMode();
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

        banIndex++;
        UpdateInteractMode();
    }

    private void SwapControllingPlayer()
    {
        ControllingPlayer = ControllingPlayer == 0 ? 1 : 0;
    }

    internal void ToggleFreePickMode()
    {
        IsFreePickMode = !IsFreePickMode;
        UpdateInteractMode();
    }
    
    private void UpdateInteractMode()
    {
        TotalBansRemaining = [0, 0, 0, 0];
        int banRulesCount = ruleset.banAmounts.Length;
        if (banRulesCount == 0 || IsFreePickMode)
        {
            CurrentInteractMode = InteractMode.PICK;
            return;
        }

        ControllingPlayer = controlStartPlayer;
        int[] banAmounts = ruleset.banAmounts[matchCount < banRulesCount ? matchCount : banRulesCount - 1];
        foreach (int banAmount in banAmounts)
        {
            TotalBansRemaining[ControllingPlayer] += banAmount;
            SwapControllingPlayer();
        }
        
        ControllingPlayer = controlStartPlayer;
        int banSum = 0;
        foreach (int banAmount in banAmounts)
        {
            CurrentBansRemaining = banAmount;

            if (banAmount == 0) break;
            for (int i = 0; i < banAmount; i++)
            {
                if (banSum == banIndex)
                {
                    CurrentInteractMode = InteractMode.BAN;
                    return;
                }
                banSum++;
                TotalBansRemaining[ControllingPlayer]--;
                CurrentBansRemaining--;
            }

            SwapControllingPlayer();
        }

        CurrentInteractMode = InteractMode.PICK;
    }

    internal bool CheckPlayerInteraction(Stage stage, int playerNumber)
    {
        StageBan stageBan = stageBans.Find((ban) => ban.stage == stage);
        return CheckPlayerInteraction(stageBan, playerNumber);
    }

    internal bool CheckPlayerInteraction(StageBan stageBan, int playerNumber)
    {
        if (IsFreePickMode) return true;
        if (playerNumber != ControllingPlayer) return false;
        if (stageBan == null) return true;
        if (stageBan.reason != SetTracker.StageBan.BanReason.DSR) return false;
        if (stageBan.banPlayer == playerNumber || stageBan.banPlayer == -1) return false;
        return true;
    }

    internal int GetGameNumber()
    {
        return matchCount + 1;
    }

    internal int[] GetWinCounts()
    {
        int[] winCounts = [0, 0, 0, 0];
        
        foreach (Match match in completedMatches)
        {
            int winner = match.GetWinner();
            winCounts[winner]++;
        }

        return winCounts;
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