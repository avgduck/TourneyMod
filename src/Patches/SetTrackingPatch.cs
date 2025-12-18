using GameplayEntities;
using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;
using LLBML.States;
using LLHandlers;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using TourneyMod.UI;

namespace TourneyMod.Patches;

internal static class SetTrackingPatch
{
    // GameStates.set(GameState newState, bool noLink = false)
    [HarmonyPatch(typeof(DNPFJHMAIBP), nameof(DNPFJHMAIBP.HOGJDNCMNFP))]
    [HarmonyPostfix]
    private static void SetGameState_Postfix(JOFJHDJHJGI CFDCLPJMFDP)
    {
        GameState newState = CFDCLPJMFDP;
        if (SetTracker.Instance.IsTrackingSet)
        {
            if (newState == GameState.MENU)
            {
                ScreenLobbyOverlay.Close();
                SetTracker.Instance.End();
            }
            else if (newState == GameState.GAME_INTRO)
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
                Player.ForAll(player =>
                {
                    if (!player.IsInMatch) return;
                    if (GameSettings.current.UsePoints) return;
                        
                    PlayerData data = player.playerEntity.playerData;
                    scores[player.nr] = data.stocks;
                });
                SetTracker.Instance.CurrentSet.EndMatch(scores);
            }
        }
        else if (newState == GameState.LOBBY_LOCAL || newState == GameState.LOBBY_ONLINE || newState == GameState.LOBBY_TRAINING)
        {
            SetTracker.Instance.Start();
            ScreenLobbyOverlay.Open();
        }
    }
}