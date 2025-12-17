using System.Collections.Generic;
using LLHandlers;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;

namespace TourneyMod.StageStriking;

internal class StrikeInfo
{
    internal Ruleset ActiveRuleset { get; private set; }
    internal int ControlStartPlayer { get; private set; }
    internal bool IsFreePickForced { get; private set; }

    internal List<StageBan> StageBans { get; private set; }
    internal int[] TotalBansRemaining { get; private set; }
    internal int CurrentBansRemaining { get; private set; }
    private int banIndex = 0;

    internal enum InteractMode
    {
        PICK,
        BAN
    }
    internal InteractMode CurrentInteractMode { get; private set; }
    internal bool IsFreePickMode { get; private set; }
    internal int ControllingPlayer { get; private set; }

    internal StrikeInfo(Ruleset ruleset)
    {
        ActiveRuleset = ruleset;
        ControlStartPlayer = ActiveRuleset.game1FirstPlayer;
        IsFreePickForced = ActiveRuleset.banAmounts.Length == 0;
        
        InitBans();
        UpdateInteractMode();
    }

    private void InitBans()
    {
        banIndex = 0;
        StageBans = new List<StageBan>();

        if (!SetTracker.Instance.IsTrackingSet) return;
        Set set = SetTracker.Instance.CurrentSet;

        if (set.IsGame1)
        {
            ActiveRuleset.stagesCounterpick.ForEach(stage => StageBans.Add(new StageBan(stage, StageBan.BanReason.COUNTERPICK)));
            return;
        }

        if (ActiveRuleset.dsrMode == Ruleset.DsrMode.OFF) return;

        Match[] lastWins = new Match[4];
        set.CompletedMatches.ForEach(match => lastWins[match.Winner] = match);
        
        set.CompletedMatches.ForEach(match =>
        {
            int winner = match.Winner;
            Match lastWin = lastWins[winner];

            StageBan previousBan = StageBans.Find(ban => ban.stage == match.PlayedStage);
            if (ActiveRuleset.dsrMode == Ruleset.DsrMode.LAST_WIN && match != lastWin) return;

            if (previousBan == null) StageBans.Add(new StageBan(match.PlayedStage, StageBan.BanReason.DSR, winner));
            else if (previousBan.banPlayer != winner) previousBan.banPlayer = -1;
        });
    }

    internal void BanStage(Stage stage, int playerNumber)
    {
        StageBan previousBan = StageBans.Find((ban) => ban.stage == stage);
        StageBan newBan = new StageBan(stage, StageBan.BanReason.BAN, playerNumber);
        if (previousBan != null) StageBans.Remove(previousBan);
        StageBans.Add(newBan);

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
        if (IsFreePickMode || IsFreePickForced)
        {
            CurrentInteractMode = InteractMode.PICK;
            return;
        }

        ControllingPlayer = ControlStartPlayer;
        int matchCount = SetTracker.Instance.IsTrackingSet ? SetTracker.Instance.CurrentSet.CompletedMatches.Count : 0;
        int banRulesCount = ActiveRuleset.banAmounts.Length;
        int[] banAmounts = ActiveRuleset.banAmounts[matchCount < banRulesCount ? matchCount : banRulesCount - 1];
        foreach (int banAmount in banAmounts)
        {
            TotalBansRemaining[ControllingPlayer] += banAmount;
            SwapControllingPlayer();
        }

        ControllingPlayer = ControlStartPlayer;
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
        StageBan stageBan = StageBans.Find((ban) => ban.stage == stage);
        return CheckPlayerInteraction(stageBan, playerNumber);
    }

    internal bool CheckPlayerInteraction(StageBan stageBan, int playerNumber)
    {
        if (IsFreePickMode || IsFreePickForced) return true;
        if (playerNumber != ControllingPlayer) return false;
        if (stageBan == null) return true;
        if (stageBan.reason != StageBan.BanReason.DSR) return false;
        if (stageBan.banPlayer == -1) return false;
        if (stageBan.banPlayer == playerNumber && CurrentInteractMode == InteractMode.PICK) return false;
        if (stageBan.banPlayer != playerNumber && CurrentInteractMode == InteractMode.BAN) return false;
        return true;
    }
}