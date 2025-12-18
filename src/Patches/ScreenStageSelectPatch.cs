using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;
using LLGUI;
using LLScreen;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using TourneyMod.UI;

namespace TourneyMod.Patches;

internal static class ScreenStageSelectPatch
    {
        // GameStatesLobby.OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPrefix]
        private static void OpenStageSelect_Prefix(HPNLMFHPHFD __instance)
        {
            if (!SetTracker.Instance.IsTrackingSet) return;
            
            if (GameSettings.IsOnline && GameSettings.OnlineMode != OnlineMode.HOSTED) return;
            StageStrikeTracker.Instance.Start();
            ScreenStageStrike.Open();
        }
        // GameStatesLobby.OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPostfix]
        private static void OpenStageSelect_Postfix(HPNLMFHPHFD __instance)
        {
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
    }