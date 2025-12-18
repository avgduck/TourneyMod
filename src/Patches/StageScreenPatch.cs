using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;
using LLGUI;
using LLScreen;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using TourneyMod.UI;

namespace TourneyMod.Patches;

internal static class StageScreenPatch
    {
        // GameStatesLobby.OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPrefix]
        private static void OpenStageSelect_Prefix(HPNLMFHPHFD __instance)
        {
            //IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            //if (localLobby == null || !SetTracker.Is1v1) return;

            if (!SetTracker.Instance.IsTrackingSet) return;
            
            //Plugin.LogGlobal.LogInfo($"OpenStageSelect: IsOnline {GameSettings.IsOnline}, OnlineMode {GameSettings.OnlineMode}");
            if (GameSettings.IsOnline && GameSettings.OnlineMode != OnlineMode.HOSTED) return;
            StageStrikeTracker.Instance.Start();
            ScreenStageStrike.Open();
        }
        // GameStatesLobby.OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPostfix]
        private static void OpenStageSelect_Postfix(HPNLMFHPHFD __instance)
        {
            //IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            //if (localLobby == null || !SetTracker.Is1v1) return;

            //localLobby.LHCCKNCKCGD(); // GameStatesLobbyLocal.ShowActiveCursors()
            if (!SetTracker.Instance.IsTrackingSet) return;
            if (!ScreenStageStrike.IsOpen) return;
            HPNLMFHPHFD lobby = __instance;
            lobby.LHCCKNCKCGD(); // GameStatesLobby::ShowActiveCursors()
        }
        
        // GameStatesLobby.CloseStageSelect()
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.EKFCNNPDJHH))]
        [HarmonyPostfix]
        private static void CloseStageSelect_Postfix(HPNLMFHPHFD __instance)
        {
            //IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            //if (localLobby == null || !SetTracker.Is1v1) return;
            
            if (!SetTracker.Instance.IsTrackingSet) return;
            if (!ScreenStageStrike.IsOpen) return;
            StageStrikeTracker.Instance.End();
            ScreenStageStrike.Close();
        }
        
        // GameStatesLobby.ProcessMsgStageSelect(Message message)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.LKBFKGGCFHE))]
        [HarmonyPrefix]
        private static void ProcessStageSelect_Prefix(HPNLMFHPHFD __instance, Message EIMJOIEPMNA)
        {
            //IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            //if (localLobby == null || !SetTracker.Is1v1) return;

            if (!SetTracker.Instance.IsTrackingSet) return;
            if (!ScreenStageStrike.IsOpen) return;
            Message message = EIMJOIEPMNA;
            if (message.msg == Msg.SEL_STAGE) __instance.EKFCNNPDJHH(); // GameStatesLobby.CloseStageSelect()
        }

        [HarmonyPatch(typeof(ScreenPlayersStage), nameof(ScreenPlayersStage.OnOpen))]
        [HarmonyPrefix]
        private static bool ScreenPlayersStage_OnOpen_Prefix(ScreenPlayersStage __instance)
        {
            if (!SetTracker.Instance.IsTrackingSet) return true;
            if (!ScreenStageStrike.IsOpen) return true;
            
            ScreenStageStrike.Instance.OnOpen(__instance);
            return false;
        }

        [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.SetPlayerLayout))]
        [HarmonyPostfix]
        private static void SetPlayerLayout_Postfix(ScreenPlayers __instance)
        {
            if (!ScreenLobbyOverlay.IsActive) return;
            
            ScreenLobbyOverlay.Instance.OnOpen(__instance);
        }

        [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.DoUpdate))]
        [HarmonyPostfix]
        private static void ScreenPlayers_DoUpdate_Postfix()
        {
            if (!ScreenLobbyOverlay.IsActive) return;
            if (!ScreenLobbyOverlay.Instance.IsOpen) return; 
            
            ScreenLobbyOverlay.Instance.UpdateSetCount();
        } 

        [HarmonyPatch(typeof(LLCursor), nameof(LLCursor.ResizeHWCursor))]
        [HarmonyPostfix]
        private static void HWCursor_Postfix(LLCursor __instance)
        {
            if (__instance.state != CursorState.POINTER_HW) return;
            if (__instance.player == null) return;
            ScreenStageStrike.GenerateCursorImages(__instance);
        }

        [HarmonyPatch(typeof(UIInput), nameof(UIInput.HandleCursors))]
        [HarmonyPostfix]
        private static void HandleCursors_Postfix()
        {
            if (UIInput.uiControl != UIControl.PLAYER_POINTERS) return;
            if (!ScreenStageStrike.IsOpen)
            {
                ScreenStageStrike.ResetCursorColors();
                return;
            }
            
            ScreenStageStrike.UpdateCursorColors(StageStrikeTracker.Instance.CurrentStrikeInfo.ControllingPlayer);
        }

        // GameStatesGameResult.UpdateState(GameState state)
        [HarmonyPatch(typeof(OEAINNHEMKA), nameof(OEAINNHEMKA.UpdateState))]
        [HarmonyPostfix]
        private static void ResultUpdateState_Postfix(OEAINNHEMKA __instance)
        {
            if (GameSettings.IsOnline) return;
            PostScreen screenResults = __instance.APFKDEMGLHJ;
            if (screenResults == null) return;
            
            Player.ForAll(player =>
            {
                // KHMFCILNHHH.EOCBBKOIFNO -> RematchChoice.QUIT
                __instance.DABHMHOCDEN(player.nr, KHMFCILNHHH.EOCBBKOIFNO);
            });
        }
        
        // GameStatesGameResult.SetRematchChoice(int playerNumber, RematchChoice choice)
        [HarmonyPatch(typeof(OEAINNHEMKA), nameof(OEAINNHEMKA.DABHMHOCDEN))]
        [HarmonyPostfix]
        private static void SetRematchChoice_Postfix(OEAINNHEMKA __instance, int BKEOPDPFFPM, KHMFCILNHHH ONPJANKJDJH)
        {
            if (GameSettings.IsOnline) return;
            
            int playerNumber = BKEOPDPFFPM;
            KHMFCILNHHH rematchChoice = ONPJANKJDJH;
            PostScreen screenResults = __instance.APFKDEMGLHJ;

            screenResults.SetChoice(playerNumber, rematchChoice);
            // NIPJFJKNGHO.DLPDHJFPKMJ -> ResultButtons.REMATCH_QUIT
            // KHMFCILNHHH.EOCBBKOIFNO -> RematchChoice.QUIT
            if (playerNumber == 0 && screenResults.resultButtons == NIPJFJKNGHO.DLPDHJFPKMJ &&
                rematchChoice == KHMFCILNHHH.EOCBBKOIFNO)
            {
                // NIPJFJKNGHO.EOCBBKOIFNO -> ResultButtons.QUIT
                screenResults.ShowButtons(NIPJFJKNGHO.EOCBBKOIFNO);
            }
        }
    }