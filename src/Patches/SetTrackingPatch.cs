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
        if (SetTracker.Instance.ActiveTourneyMode is TourneyMode.NONE) return;
        if (SetTracker.Instance.CurrentSet.IsGame1 && !SetTracker.Instance.CurrentSet.IsTiebreaker) return;

        Player p = __instance.player;
        int stocksRemaining = SetTracker.Instance.CurrentSet.PlayerStockLock[p.nr];
        if (stocksRemaining < 1) return;

        __instance.playerData.stocks = Mathf.Clamp(stocksRemaining, 1, GameSettings.current.stocks);
    }
    
    // void Rules::GameStarted()
    [HarmonyPatch(typeof(NCMFHODLNAJ), nameof(NCMFHODLNAJ.KHCHLIEACLP))]
    [HarmonyPrefix]
    private static void Rules_GameStarted_Prefix()
    {
        if (!SetTracker.Instance.IsTrackingSet) return;
        GameStart();
    }
    
    // void GameStatesGame::GameDone(bool disconnect)
    [HarmonyPatch(typeof(OGONAGCFDPK), nameof(OGONAGCFDPK.NPNPJAGHINC))]
    [HarmonyPrefix]
    private static void GameStatesGame_GameDone_Prefix(bool MKIGJKIBGIH)
    {
        if (MKIGJKIBGIH) return;
        if (!SetTracker.Instance.IsTrackingSet) return;
        GameDone(false);
    }
    
    // void GameStatesGame::GameTimeOut()
    [HarmonyPatch(typeof(OGONAGCFDPK), nameof(OGONAGCFDPK.DJILJJJMJGH))]
    [HarmonyPrefix]
    private static void GameStatesGame_GameTimeOut_Prefix()
    {
        if (!SetTracker.Instance.IsTrackingSet) return;
        GameDone(true);
    }

    private static void GameStart()
    {
        Stage stage = HPNLMFHPHFD.ELPLKHOLJID.OOEPDFABFIP; // GameStatesLobby.curSettings.stage
        PlayerCharacter[] playerCharacters = [PlayerCharacter.EMPTY, PlayerCharacter.EMPTY, PlayerCharacter.EMPTY, PlayerCharacter.EMPTY];
        Player.ForAllInMatch((Player player) =>
        {
            playerCharacters[player.nr] = new PlayerCharacter(player.CharacterSelected, player.CharacterVariant);
        });
        SetTracker.Instance.CurrentSet.StartMatch(stage, playerCharacters);
    }

    private static void GameDone(bool isTimeout)
    {
        PlayerScore[] scores = [new(), new(), new(), new()];
        Player.ForAllInMatch((Player player) =>
        {
            if (GameSettings.current.UsePoints) return;
                    
            PlayerData data = player.playerEntity.playerData;
            scores[player.nr].Stocks = data.stocks;
            scores[player.nr].Team = data.team;
            scores[player.nr].Hp = data.hp;
        });
        SetTracker.Instance.CurrentSet.EndMatch(scores, isTimeout);
    }
}