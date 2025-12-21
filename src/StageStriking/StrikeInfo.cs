using System.Collections.Generic;
using System.Linq;
using LLBML.States;
using LLBML.Utils;
using LLHandlers;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;
using TourneyMod.UI;
using Random = UnityEngine.Random;

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
    internal int ControllingPlayer { get; private set; }

    internal StrikeInfo(Ruleset ruleset)
    {
        ActiveRuleset = ruleset;
        IsFreePickForced = ActiveRuleset.banAmounts.Length == 0;

        if (SetTracker.Instance.CurrentSet.IsGame1)
        {
            ControlStartPlayer = ActiveRuleset.game1FirstPlayer;
        }
        else
        {
            int winner = SetTracker.Instance.CurrentSet.LastWinner;
            int loser = winner == 0 ? 1 : 0;
            ControlStartPlayer = ActiveRuleset.laterGamesFirstPlayer == Ruleset.FirstPlayer.WINNER ? winner : loser;
        }
        
        UpdateInteractMode();
        StageStrikeTracker.Log.LogInfo($"Striking started with ruleset '{ActiveRuleset.Id}', game {SetTracker.Instance.CurrentSet.GameNumber}: {(SetTracker.Instance.CurrentSet.IsFreePickMode || IsFreePickForced ? "free pick mode active" : $"P{ControllingPlayer+1} first {CurrentInteractMode}. bans remaining ({TotalBansRemaining[0]}, {TotalBansRemaining[1]})")}");
        
        InitBans();
    }

    private void InitBans()
    {
        banIndex = 0;
        StageBans = new List<StageBan>();

        if (IsFreePickForced) return;
        if (!SetTracker.Instance.IsTrackingSet) return;
        Set set = SetTracker.Instance.CurrentSet;
        
        if (set.IsGame1)
        {
            ActiveRuleset.stagesCounterpick.ForEach(stage => StageBans.Add(new StageBan(stage, StageBan.BanReason.COUNTERPICK)));
            StageStrikeTracker.Log.LogInfo("Counterpick bans applied: " + Plugin.PrintArray(StageBans.Map(ban => ban.stage).ToArray(), false));
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
        
        StageStrikeTracker.Log.LogInfo("DSR bans applied: " + Plugin.PrintArray(StageBans.Map(ban => $"{ban.stage} ({ban.banPlayer switch {
            -1 => "both",
            _ => $"P{ban.banPlayer+1}"
        }})").ToArray(), false));
    }

    internal Stage PickStage(Stage stage, int playerNumber)
    {
        GameStates.Send(Msg.SEL_STAGE, playerNumber, (int)stage);
        StageStrikeTracker.Log.LogInfo($"P{playerNumber+1} picks {stage}");
        return stage;
    }

    internal Stage PickRandomStage(Ruleset.RandomMode randomMode)
    {
        List<Stage> randomStagePool = new List<Stage>();
        switch (randomMode)
        {
            case Ruleset.RandomMode.ANY:
                randomStagePool.AddRange(Ruleset.STAGES_3D);
                randomStagePool.AddRange(Ruleset.STAGES_2D);
                break;
                
            case Ruleset.RandomMode.ANY_3D:
                randomStagePool.AddRange(Ruleset.STAGES_3D);
                break;
                
            case Ruleset.RandomMode.ANY_2D:
                randomStagePool.AddRange(Ruleset.STAGES_2D);
                break;
                
            case Ruleset.RandomMode.ANY_LEGAL:
                randomStagePool.AddRange(ActiveRuleset.stagesNeutral);
                randomStagePool.AddRange(ActiveRuleset.stagesCounterpick);
                break;
                
            case Ruleset.RandomMode.OFF or Ruleset.RandomMode.BOTH:
                break;
        }
        
        if (randomStagePool.Count == 0) return Stage.NONE;
        Stage stage = randomStagePool[Random.RandomRangeInt(0, randomStagePool.Count)];
        GameStates.Send(Msg.SEL_STAGE, -1, (int)stage);
        StageStrikeTracker.Log.LogInfo($"Players voted random {randomMode}: picked {stage}");
        return stage;
    }

    internal void BanStage(Stage stage, int playerNumber)
    {
        StageBan previousBan = StageBans.Find((ban) => ban.stage == stage);
        StageBan newBan = new StageBan(stage, StageBan.BanReason.BAN, playerNumber);
        if (previousBan != null) StageBans.Remove(previousBan);
        StageBans.Add(newBan);

        banIndex++;
        UpdateInteractMode();
        StageStrikeTracker.Log.LogInfo($"P{playerNumber+1} bans {stage}. bans remaining ({TotalBansRemaining[0]}, {TotalBansRemaining[1]}). P{ControllingPlayer+1} next {CurrentInteractMode}");
    }
    
    private void SwapControllingPlayer()
    {
        ControllingPlayer = ControllingPlayer == 0 ? 1 : 0;
    }
    
    internal void ToggleFreePickMode()
    {
        SetTracker.Instance.CurrentSet.IsFreePickMode = !SetTracker.Instance.CurrentSet.IsFreePickMode;
        UpdateInteractMode();
        StageStrikeTracker.Log.LogInfo($"Free pick mode toggled {(SetTracker.Instance.CurrentSet.IsFreePickMode ? "ON" : $"OFF: P{ControllingPlayer+1} next {CurrentInteractMode}. bans remaining ({TotalBansRemaining[0]}, {TotalBansRemaining[1]})")}");
    }

    private void UpdateInteractMode()
    {
        TotalBansRemaining = [0, 0, 0, 0];
        if (SetTracker.Instance.CurrentSet.IsFreePickMode || IsFreePickForced)
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
        if (SetTracker.Instance.CurrentSet.IsFreePickMode || IsFreePickForced) return true;
        if (playerNumber != ControllingPlayer) return false;
        if (stageBan == null) return true;
        if (stageBan.reason != StageBan.BanReason.DSR) return false;
        if (stageBan.banPlayer == -1) return false;
        if (stageBan.banPlayer == playerNumber && CurrentInteractMode == InteractMode.PICK) return false;
        if (stageBan.banPlayer != playerNumber && CurrentInteractMode == InteractMode.BAN) return false;
        return true;
    }
}