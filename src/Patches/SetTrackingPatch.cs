using System.Linq;
using GameplayEntities;
using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;
using LLBML.States;
using LLHandlers;
using TourneyMod.SetTracking;
using UnityEngine;

namespace TourneyMod.Patches;

internal static class SetTrackingPatch
{
    // void GameStates::Set(GameState newState, bool noLink = false)
    [HarmonyPatch(typeof(DNPFJHMAIBP), nameof(DNPFJHMAIBP.HOGJDNCMNFP))]
    [HarmonyPrefix]
    private static void GameStates_Set_Prefix(JOFJHDJHJGI CFDCLPJMFDP)
    {
        GameState newState = CFDCLPJMFDP;
        
        if (newState == GameState.LOBBY_LOCAL || newState == GameState.LOBBY_ONLINE || newState == GameState.LOBBY_TRAINING)
        {
            if (!SetTracker.Instance.IsTrackingSet) SetTracker.Instance.Start();
        }

        if (newState == GameState.MENU)
        {
            if (SetTracker.Instance.ActiveTourneyMode == TourneyMode.NONE && SetTracker.Instance.IsTrackingSet) SetTracker.Instance.End();
        }
    }
    
    // void GameStates::Set(GameState newState, bool noLink = false)
    [HarmonyPatch(typeof(DNPFJHMAIBP), nameof(DNPFJHMAIBP.HOGJDNCMNFP))]
    [HarmonyPostfix]
    private static void GameStates_Set_Postfix(JOFJHDJHJGI CFDCLPJMFDP)
    {
        GameState newState = CFDCLPJMFDP;

        if (!SetTracker.Instance.IsTrackingSet) return;
        
        if (newState == GameState.GAME_INTRO)
        {
            Stage stage = HPNLMFHPHFD.ELPLKHOLJID.OOEPDFABFIP; // GameStatesLobby.curSettings.stage
            PlayerCharacter[] playerCharacters = [PlayerCharacter.EMPTY, PlayerCharacter.EMPTY, PlayerCharacter.EMPTY, PlayerCharacter.EMPTY];
            Player.ForAllInMatch((Player player) =>
            {
                playerCharacters[player.nr] = new PlayerCharacter(player.CharacterSelected, player.CharacterVariant);
            });
            SetTracker.Instance.CurrentSet.StartMatch(stage, playerCharacters);
        }
        else if (newState == GameState.GAME_RESULT)
        {
            PlayerScore[] scores = [new(), new(), new(), new()];
            Player.ForAllInMatch((Player player) =>
            {
                if (GameSettings.current.UsePoints) return;
                    
                PlayerData data = player.playerEntity.playerData;
                scores[player.nr].Stocks = data.stocks;
                scores[player.nr].Team = data.team;
            });
            SetTracker.Instance.CurrentSet.EndMatch(scores);
        }
    }

    // void GameSettings::ResetGameModeSettings()
    [HarmonyPatch(typeof(JOMBNFKIHIC), nameof(JOMBNFKIHIC.ADDBHIFLMEI))]
    [HarmonyPostfix]
    private static void GameSettings_ResetGameModeSettings_Postfix(JOMBNFKIHIC __instance)
    {
        GameSettings settings = __instance;

        if (!SetTracker.Instance.IsTrackingSet) return;
        if (!SetTracker.Instance.CurrentSet.ActiveRuleset.HasGameOptions) return;
        SetTracker.Instance.ApplyGameOptions(settings, SetTracker.Instance.CurrentSet.ActiveRuleset.GameOptions);
    }

    [HarmonyPatch(typeof(PlayerEntity), nameof(PlayerEntity.Init))]
    [HarmonyPostfix]
    private static void PlayerEntity_Init_Postfix(PlayerEntity __instance)
    {
        if (SetTracker.Instance.ActiveTourneyMode is not TourneyMode.LOCAL_CREW) return;
        if (SetTracker.Instance.CurrentSet.IsGame1) return;

        Player p = __instance.player;
        int stocksRemaining = SetTracker.Instance.CurrentSet.CompletedMatches.Last().FinalScores[p.nr].Stocks;
        if (stocksRemaining < 1) return;

        __instance.playerData.stocks = Mathf.Clamp(stocksRemaining, 1, GameSettings.current.stocks);
    }
}