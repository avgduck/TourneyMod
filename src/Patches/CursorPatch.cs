using HarmonyLib;
using LLGUI;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using TourneyMod.UI;

namespace TourneyMod.Patches;

internal static class CursorPatch
    {
        // GameStatesLobby.OpenStageSelect(bool canGoBack, bool localSpectator = false, ScreenType = ScreenType.PLAYERS_STAGE)
        [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.KOLLNKIKIKM))]
        [HarmonyPostfix]
        private static void OpenStageSelect_Postfix(HPNLMFHPHFD __instance)
        {
            if (!SetTracker.Instance.IsTrackingSet) return;
            __instance.LHCCKNCKCGD(); // GameStatesLobby::ShowActiveCursors()
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