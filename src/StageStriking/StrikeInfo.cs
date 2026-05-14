using System.Collections.Generic;
using System.Linq;
using LLBML.Players;
using LLBML.States;
using LLBML.Utils;
using LLHandlers;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;
using Random = UnityEngine.Random;

namespace TourneyMod.StageStriking;

internal class StrikeInfo
{
    internal Team ControlStartTeam { get; private set; }

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
    internal Team ControllingTeam { get; private set; }

    internal StrikeInfo()
    {
        if (SetTracker.Instance.CurrentSet.IsGame1)
        {
            Team startTeam = Ruleset.ConvertPlayerTeam(SetTracker.Instance.CurrentSet.ActiveRuleset.game1FirstTeam);
            Team rpsWinner = SetTracker.Instance.CurrentSet.RpsWinner;

            if (rpsWinner == Team.RED) ControlStartTeam = startTeam;
            else ControlStartTeam = (startTeam == Team.RED ? Team.BLUE : Team.RED);
        }
        else
        {
            Team winner = SetTracker.Instance.CurrentSet.LastWinner;
            Team loser = winner == Team.NONE ? Team.NONE : (winner == Team.RED ? Team.BLUE : Team.RED);
            ControlStartTeam = SetTracker.Instance.CurrentSet.ActiveRuleset.laterGamesFirstTeam == Ruleset.FirstTeam.WINNER ? winner : loser;
        }
        
        UpdateInteractMode();
        StageStrikeTracker.Log.LogInfo($"Striking started with ruleset '{SetTracker.Instance.CurrentSet.ActiveRuleset.Id}', game {SetTracker.Instance.CurrentSet.GameNumber}: {(SetTracker.Instance.CurrentSet.IsFreePickMode || SetTracker.Instance.CurrentSet.IsFreePickForced ? "free pick mode active" : $"{ControllingTeam} first {CurrentInteractMode}. bans remaining ({TotalBansRemaining[0]}, {TotalBansRemaining[1]})")}");
        
        InitBans();
    }

    private void InitBans()
    {
        banIndex = 0;
        StageBans = new List<StageBan>();

        if (SetTracker.Instance.CurrentSet.IsFreePickForced) return;
        if (!SetTracker.Instance.IsTrackingSet) return;
        Set set = SetTracker.Instance.CurrentSet;
        
        if (set.IsGame1)
        {
            SetTracker.Instance.CurrentSet.ActiveRuleset.stagesCounterpick.ForEach(stage => StageBans.Add(new StageBan(stage, StageBan.BanReason.COUNTERPICK, Team.NONE)));
            StageStrikeTracker.Log.LogInfo("Counterpick bans applied: " + Plugin.PrintArray(StageBans.Map(ban => ban.stage).ToArray(), false));
            return;
        }

        if (SetTracker.Instance.CurrentSet.ActiveRuleset.dsrMode == Ruleset.DsrMode.OFF) return;

        Match[] lastWins = new Match[4];
        set.CompletedMatches.ForEach(match =>
        {
            if (match.Winner == Team.NONE) return;
            lastWins[(int)match.Winner] = match;
        });
        
        set.CompletedMatches.ForEach(match =>
        {
            if (match.Winner == Team.NONE) return;
            Match lastWin = lastWins[(int)match.Winner];

            StageBan previousBan = StageBans.Find(ban => ban.stage == match.PlayedStage);
            if (SetTracker.Instance.CurrentSet.ActiveRuleset.dsrMode == Ruleset.DsrMode.LAST_WIN && match != lastWin) return;

            if (previousBan == null) StageBans.Add(new StageBan(match.PlayedStage, StageBan.BanReason.DSR, match.Winner));
            else if (previousBan.banTeam != match.Winner) previousBan.banTeam = Team.NONE;
        });
        
        StageStrikeTracker.Log.LogInfo("DSR bans applied: " + Plugin.PrintArray(StageBans.Map(ban => $"{ban.stage} ({(ban.banTeam == Team.NONE ? "both" : $"{ban.banTeam}")})").ToArray(), false));
    }

    internal Stage PickStage(Stage stage, int playerNumber)
    {
        Team team = SetTracker.Instance.GetPlayerTeam(playerNumber);
        
        GameStates.Send(Msg.SEL_STAGE, playerNumber, (int)stage);
        StageStrikeTracker.Log.LogInfo($"{team} picks {stage}");
        return stage;
    }

    internal Stage PickRandomStage(Ruleset.RandomStageMode randomStageMode)
    {
        List<Stage> randomStagePool = new List<Stage>();
        switch (randomStageMode)
        {
            case Ruleset.RandomStageMode.ANY:
                randomStagePool.AddRange(Ruleset.STAGES_3D);
                randomStagePool.AddRange(Ruleset.STAGES_2D);
                break;
                
            case Ruleset.RandomStageMode.ANY_3D:
                randomStagePool.AddRange(Ruleset.STAGES_3D);
                break;
                
            case Ruleset.RandomStageMode.ANY_2D:
                randomStagePool.AddRange(Ruleset.STAGES_2D);
                break;
                
            case Ruleset.RandomStageMode.ANY_LEGAL:
                randomStagePool.AddRange(SetTracker.Instance.CurrentSet.ActiveRuleset.stagesNeutral);
                randomStagePool.AddRange(SetTracker.Instance.CurrentSet.ActiveRuleset.stagesCounterpick);
                break;
                
            case Ruleset.RandomStageMode.OFF or Ruleset.RandomStageMode.BOTH:
                break;
        }
        
        if (randomStagePool.Count == 0) return Stage.NONE;
        Stage stage = randomStagePool[Random.RandomRangeInt(0, randomStagePool.Count)];
        GameStates.Send(Msg.SEL_STAGE, -1, (int)stage);
        StageStrikeTracker.Log.LogInfo($"Players voted random {randomStageMode}: picked {stage}");
        return stage;
    }

    internal void BanStage(Stage stage, int playerNumber)
    {
        Team team = SetTracker.Instance.GetPlayerTeam(playerNumber);
        
        StageBan previousBan = StageBans.Find((ban) => ban.stage == stage);
        StageBan newBan = new StageBan(stage, StageBan.BanReason.BAN, team);
        if (previousBan != null) StageBans.Remove(previousBan);
        StageBans.Add(newBan);

        banIndex++;
        UpdateInteractMode();
        StageStrikeTracker.Log.LogInfo($"{team} bans {stage}. bans remaining ({TotalBansRemaining[0]}, {TotalBansRemaining[1]}). {ControllingTeam} next {CurrentInteractMode}");
    }
    
    private void SwapControllingPlayer()
    {
        ControllingTeam = ControllingTeam == Team.RED ? Team.BLUE : Team.RED;
    }
    
    internal void ToggleFreePickMode()
    {
        SetTracker.Instance.CurrentSet.IsFreePickMode = !SetTracker.Instance.CurrentSet.IsFreePickMode;
        UpdateInteractMode();
        StageStrikeTracker.Log.LogInfo($"Free pick mode toggled {(SetTracker.Instance.CurrentSet.IsFreePickMode ? "ON" : $"OFF: {ControllingTeam} next {CurrentInteractMode}. bans remaining ({TotalBansRemaining[0]}, {TotalBansRemaining[1]})")}");
    }

    private void UpdateInteractMode()
    {
        ControllingTeam = ControlStartTeam;
        
        TotalBansRemaining = [0, 0, 0, 0];
        if (SetTracker.Instance.CurrentSet.IsFreePickMode || SetTracker.Instance.CurrentSet.IsFreePickForced || ControllingTeam == Team.NONE)
        {
            CurrentInteractMode = InteractMode.PICK;
            return;
        }
        
        int matchCount = SetTracker.Instance.CurrentSet.TotalWins;
        int banRulesCount = SetTracker.Instance.CurrentSet.ActiveRuleset.banAmounts.Length;
        int[] banAmounts = SetTracker.Instance.CurrentSet.ActiveRuleset.banAmounts[matchCount < banRulesCount ? matchCount : banRulesCount - 1];
        foreach (int banAmount in banAmounts)
        {
            TotalBansRemaining[(int)ControllingTeam] += banAmount;
            SwapControllingPlayer();
        }

        ControllingTeam = ControlStartTeam;
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
                TotalBansRemaining[(int)ControllingTeam]--;
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
        Team team = SetTracker.Instance.GetPlayerTeam(playerNumber);
        
        if (SetTracker.Instance.CurrentSet.IsFreePickMode || SetTracker.Instance.CurrentSet.IsFreePickForced) return true;
        if (team != ControllingTeam) return false;
        if (stageBan == null) return true;
        if (stageBan.reason != StageBan.BanReason.DSR) return false;
        if (stageBan.banTeam == Team.NONE) return false;
        if (stageBan.banTeam == team && CurrentInteractMode == InteractMode.PICK) return false;
        if (stageBan.banTeam != team && CurrentInteractMode == InteractMode.BAN) return false;
        return true;
    }
}