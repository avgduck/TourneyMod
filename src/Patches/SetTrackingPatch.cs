using GameplayEntities;
using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;
using LLBML.States;
using LLHandlers;
using TourneyMod.SetTracking;

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
            Character[] selectedCharacters = [Character.NONE, Character.NONE, Character.NONE, Character.NONE];
            Player.ForAllInMatch(player =>
            {
                selectedCharacters[player.nr] = player.CharacterSelected;
            });
            SetTracker.Instance.CurrentSet.StartMatch(stage, selectedCharacters);
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
}