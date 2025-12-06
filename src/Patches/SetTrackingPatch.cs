using GameplayEntities;
using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;
using LLBML.States;
using LLHandlers;
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
        if (SetTracker.IsTrackingSet)
        {
            if (newState == GameState.MENU)
            {
                ScreenLobbyOverlay.Close();
                SetTracker.EndSet();
            }
            else if (newState == GameState.GAME_INTRO)
            {
                Stage stage = HPNLMFHPHFD.ELPLKHOLJID.OOEPDFABFIP; // GameStatesLobby.curSettings.stage
                SetTracker.Instance.StartMatch(stage);
            }
            else if (newState == GameState.GAME_RESULT)
            {
                int[] scores = [-1, -1, -1, -1];
                Player.ForAll((Player player) =>
                {
                    if (!player.IsInMatch) return;
                    if (GameSettings.current.UsePoints) return;
                        
                    PlayerData data = player.playerEntity.playerData;
                    scores[player.nr] = data.stocks;
                });
                SetTracker.Instance.EndMatch(scores);
            }
        }
        else if (newState == GameState.LOBBY_LOCAL || newState == GameState.LOBBY_TRAINING || newState == GameState.LOBBY_ONLINE)
        {
            SetTracker.StartSet();
            ScreenLobbyOverlay.Open();
        }
    }
}