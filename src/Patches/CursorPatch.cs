using HarmonyLib;
using LLBML.Players;
using LLGUI;
using TourneyMod.StageStriking;
using TourneyMod.UI;

namespace TourneyMod.Patches;

internal static class CursorPatch
    {
        private struct CursorInfo(UIControl control, CursorState mainState, CursorState[] playerStates)
        {
            internal UIControl control = control;
            internal CursorState mainState = mainState;
            internal CursorState[] playerStates = playerStates;
        }
        
        // void GameStatesLobby::OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPrefix]
        private static void OpenStageSelect_Prefix(HPNLMFHPHFD __instance, ref CursorInfo __state)
        {
            CursorState[] playerStates = new CursorState[4];
            Player.ForAll(player => playerStates[player.nr] = player.cursor.GetState());
            __state = new CursorInfo(UIInput.GetControl(), UIInput.mainCursor.GetState(), playerStates);
        }
        
        // void GameStatesLobby::OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPostfix]
        private static void OpenStageSelect_Postfix(HPNLMFHPHFD __instance, ref CursorInfo __state)
        {
            //__instance.LHCCKNCKCGD(); // GameStatesLobby::ShowActiveCursors()

            UIInput.SetControl(__state.control);
            
            if (__state.control == UIControl.MAIN_POINTER)
            {
                UIInput.mainCursor.SetState(__state.mainState);
                //UIInput.mainCursor.SetRelPos(0.5f, 0.5f);
            }
            else if (__state.control == UIControl.PLAYER_POINTERS)
            {
                CursorState[] playerStates = __state.playerStates;
                Player.ForAll(player => player.cursor.SetState(playerStates[player.nr]));
            }
        }

        [HarmonyPatch(typeof(LLCursor), nameof(LLCursor.ResizeHWCursor))]
        [HarmonyPostfix]
        private static void HWCursor_Postfix(LLCursor __instance)
        {
            if (__instance.state != CursorState.POINTER_HW) return;
            if (__instance.player == null) return;
            UIUtils.GenerateCursorImages(__instance);
        }

        [HarmonyPatch(typeof(UIInput), nameof(UIInput.HandleCursors))]
        [HarmonyPostfix]
        private static void HandleCursors_Postfix()
        {
            if (UIInput.uiControl != UIControl.PLAYER_POINTERS) return;
            if (!Plugin.Instance.RecolorCursors)
            {
                UIUtils.ResetCursorColors();
                return;
            }
            
            UIUtils.UpdateCursorColors(StageStrikeTracker.Instance.CurrentStrikeInfo.ControllingPlayer);
        }
    }