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

        if (!SetTracker.Instance.IsTrackingSet)
        {
            if (newState == GameState.LOBBY_LOCAL || newState == GameState.LOBBY_ONLINE || newState == GameState.LOBBY_TRAINING) SetTracker.Instance.Start();
            return;
        }

        if (newState == GameState.MENU)
        {
            switch (SetTracker.Instance.ActiveTourneyMode)
            {
                case TourneyMode.NONE:
                    SetTracker.Instance.End();
                    break;
                
                case TourneyMode.LOCAL_1V1 or TourneyMode.LOCAL_DOUBLES:
                    SetTracker.Instance.SetTourneyMode(TourneyMode.NONE);
                    break;
                
                default:
                    break;
            }
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
            Character[] playedCharacters = [Character.NONE, Character.NONE, Character.NONE, Character.NONE];
            Player.ForAllInMatch(player =>
            {
                selectedCharacters[player.nr] = player.CharacterSelected;
                playedCharacters[player.nr] = player.Character;
            });
            SetTracker.Instance.CurrentSet.StartMatch(stage, selectedCharacters, playedCharacters);
        }
        else if (newState == GameState.GAME_RESULT)
        {
            int[] scores = [-1, -1, -1, -1];
            Player.ForAllInMatch(player =>
            {
                if (GameSettings.current.UsePoints) return;
                    
                PlayerData data = player.playerEntity.playerData;
                scores[player.nr] = data.stocks;
            });
            SetTracker.Instance.CurrentSet.EndMatch(scores);
        }
    }
}