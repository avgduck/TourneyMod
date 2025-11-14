using GameplayEntities;
using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;
using LLBML.States;
using LLGUI;
using LLHandlers;
using LLScreen;

namespace TourneyMod;

internal static class HarmonyPatches
{
    internal static void PatchAll()
    {
        Harmony harmony = new Harmony(Plugin.GUID);
        harmony.PatchAll(typeof(SetTrackingPatch));
        Plugin.LogGlobal.LogInfo("Custom stage screen patch applied");
        harmony.PatchAll(typeof(StageScreenPatch));
        Plugin.LogGlobal.LogInfo("Custom stage screen patch applied");
    }

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
            else if (newState == GameState.LOBBY_LOCAL)
            {
                SetTracker.StartSet();
            }
        }
    }

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
        private static bool OnOpen_Prefix(ScreenPlayersStage __instance)
        {
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
            
            ScreenStageStrike.UpdateCursorColors(SetTracker.Instance.ControllingPlayer);
        }
    }
}