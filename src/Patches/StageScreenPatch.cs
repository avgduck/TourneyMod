using HarmonyLib;
using LLBML.Players;
using LLGUI;
using LLScreen;
using TourneyMod.UI;

namespace TourneyMod.Patches;

internal static class StageScreenPatch
    {
        // GameStatesLobby.OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPrefix]
        private static void OpenStageSelect_Prefix(HPNLMFHPHFD __instance)
        {
            IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            if (localLobby == null || !SetTracker.Is1v1) return;
            
            ScreenStageStrike.Open(); // ScreenPlayersStage screenStage
        }
        // GameStatesLobby.OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPostfix]
        private static void OpenStageSelect_Postfix(HPNLMFHPHFD __instance)
        {
            IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            if (localLobby == null || !SetTracker.Is1v1) return;

            localLobby.GFHABHIBKHK(); // GameStatesLobbyLocal.ShowActiveCursors()
        }
        
        // GameStatesLobby.CloseStageSelect()
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.EKFCNNPDJHH))]
        [HarmonyPostfix]
        private static void CloseStageSelect_Postfix(HPNLMFHPHFD __instance)
        {
            IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            if (localLobby == null || !SetTracker.Is1v1) return;
            
            ScreenStageStrike.Close();
        }
        
        // GameStatesLobby.ProcessMsgStageSelect(Message message)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.LKBFKGGCFHE))]
        [HarmonyPrefix]
        private static void ProcessStageSelect_Prefix(HPNLMFHPHFD __instance, Message EIMJOIEPMNA)
        {
            IJDANPONMLL localLobby = __instance as IJDANPONMLL;
            if (localLobby == null || !SetTracker.Is1v1) return;

            Message message = EIMJOIEPMNA;
            if (message.msg == Msg.SEL_STAGE) __instance.EKFCNNPDJHH(); // GameStatesLobby.CloseStageSelect()
        }

        [HarmonyPatch(typeof(ScreenPlayersStage), nameof(ScreenPlayersStage.OnOpen))]
        [HarmonyPrefix]
        private static bool ScreenPlayersStage_OnOpen_Prefix(ScreenPlayersStage __instance)
        {
            if (!ScreenStageStrike.IsOpen) return true;
            
            ScreenStageStrike.Instance.OnOpen(__instance);
            return false;
        }

        [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.SetPlayerLayout))]
        [HarmonyPostfix]
        private static void SetPlayerLayout_Postfix(ScreenPlayers __instance)
        {
            if (!ScreenLobbyOverlay.IsOpen || !SetTracker.Is1v1) return;
            
            ScreenLobbyOverlay.Instance.OnOpen(__instance);
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
            
            ScreenStageStrike.UpdateCursorColors(SetTracker.Instance.ControllingPlayer);
        }

        // GameStatesGameResult.UpdateState(GameState state)
        [HarmonyPatch(typeof(OEAINNHEMKA), nameof(OEAINNHEMKA.UpdateState))]
        [HarmonyPostfix]
        private static void ResultUpdateState_Postfix(OEAINNHEMKA __instance)
        {
            if (!SetTracker.Is1v1) return;
            
            Player.ForAll((Player player) =>
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
            if (!SetTracker.Is1v1) return;
            
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