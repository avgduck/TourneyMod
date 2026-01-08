using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using LLBML;
using LLBML.Players;
using LLBML.States;
using LLBML.Utils;
using LLScreen;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;
using TourneyMod.UI;
using TourneyMod.UI.Lobby;
using TourneyMod.UI.Menu;
using TourneyMod.UI.StageSelect;
using UnityEngine;
using ScreenMenuMain = LLScreen.ScreenMenuMain;

namespace TourneyMod.Patches;

internal static class ScreenReplacePatch
{
    // GameObject Assets::SpawnScreen(ScreenType screenType)
    [HarmonyPatch(typeof(JPLELOFJOOH), nameof(JPLELOFJOOH.HNHBCLJGPCE))]
    [HarmonyPostfix]
    private static void Assets_SpawnScreen_Postfix(ref GameObject __result, ScreenType FLMBCGMOCKC)
    {
        ScreenType screenType = FLMBCGMOCKC;
        if (screenType == ScreenType.MENU_MAIN)
        {
            ReplaceScreen<LLScreen.ScreenMenuMain, UI.Menu.ScreenMenuMain>(ref __result);
        }
        else if (screenType == ScreenType.MENU_VERSUS && Plugin.Instance.TourneyMenuOpen)
        {
            ReplaceScreen<ScreenMenuVersus, ScreenMenuTourney>(ref __result);
        }
        else if (screenType == ScreenType.UNLOCKS_STAGES && Plugin.Instance.RulesetsMenuOpen)
        {
            ReplaceScreen<ScreenUnlocksStages, ScreenMenuRulesets>(ref __result);
        }
        else if (screenType == ScreenType.PLAYERS && SetTracker.Instance.IsTrackingSet)
        {
            if (SetTracker.Instance.ActiveTourneyMode == TourneyMode.NONE)
            {
                // TODO: add custom lobby screen with win tracking to other game modes
            }
            else
            {
                ReplaceScreen<ScreenPlayers, ScreenLobbyTourney>(ref __result);
            }
        }
        else if (screenType == ScreenType.PLAYERS_STAGE && SetTracker.Instance.IsTrackingSet)
        {
            ReplaceScreen<ScreenPlayersStage, ScreenStageStrike>(ref __result);
        }
        else if (screenType == ScreenType.PLAYERS_STAGE_RANKED && SetTracker.Instance.IsTrackingSet)
        {
            ReplaceScreen<ScreenPlayersStageComp, ScreenStageStrikeRanked>(ref __result);
        }
    }
    
    private static void ReplaceScreen<T1, T2>(ref GameObject screen)
        where T1 : ScreenBase
        where T2 : T1, ICustomScreen<T1>
    {
        T1 screenVanilla = screen.GetComponent<T1>();
        if (screenVanilla == null)
        {
            Plugin.LogGlobal.LogError($"Error attempting to replace screen {typeof(T2)} with {typeof(T1)}");
            return;
        }

        T2 screenCustom = screen.AddComponent<T2>();
        screenCustom.Init(screenVanilla);
        GameObject.DestroyImmediate(screenVanilla);
    }

    [HarmonyPatch(typeof(IOGKKINMEFB), nameof(IOGKKINMEFB.CJAOMBCFJJO))]
    [HarmonyPostfix]
    private static void GameStatesMenu_SetMenu_Postfix()
    {
        IMenuTitle menu = UIScreen.GetScreen(1) as IMenuTitle;
        if (menu == null) return;
        // ScreenMenu GameStatesMenu.screenMenu
        IOGKKINMEFB.PPGAIOHGPAK.SetTitle(menu.GetCustomTitle());
    }

    [HarmonyPatch(typeof(OGKPCMDOMPF), nameof(OGKPCMDOMPF.CJAOMBCFJJO))]
    [HarmonyPostfix]
    private static void GameStatesUnlocks_SetMenu_Postfix()
    {
        IMenuTitle menu = UIScreen.GetScreen(1) as IMenuTitle;
        if (menu == null) return;
        // ScreenMenu GameStatesMenu.screenMenu
        IOGKKINMEFB.PPGAIOHGPAK.SetTitle(menu.GetCustomTitle());
    }
    
    // GameStatesLobby.RemovePlayer(Player p)
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.GNBKBMENOMO))]
    [HarmonyPostfix]
    private static void RemovePlayer_Postfix(ALDOKEMAOMB LGACHGEPNNH)
    {
        Player player = LGACHGEPNNH;
        VoteButton.RemovePlayer(player.nr);
    }

    [HarmonyPatch(typeof(LLScreen.ScreenMenuMain), nameof(LLScreen.ScreenMenuMain.Awake))]
    [HarmonyPrefix]
    private static bool ScreenMenuMain_Awake_Prefix(LLScreen.ScreenMenuMain __instance)
    {
        UI.Menu.ScreenMenuMain menu = __instance as UI.Menu.ScreenMenuMain;
        if (menu == null) return true;
        return false;
    }
    
    //void GameStatesMenu::MenuProcessMsg(Message message)
    [HarmonyPatch(typeof(IOGKKINMEFB), nameof(IOGKKINMEFB.DAHCMIOPGDM))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GameStatesMenu_MenuProcessMsg_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher cm = new CodeMatcher(instructions);
        cm.End();
        cm.MatchBack(false,
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldfld, typeof(ScreenMenuMain).GetField("btVersus")),
            new CodeMatch(OpCodes.Call)
        );
        CodeInstruction instruction = cm.Instruction;
        cm.Advance(3);
        cm.Insert(
            instruction,
            Transpilers.EmitDelegate<Action<LLScreen.ScreenMenuMain>>(screenMenuMain =>
            {
                if (!Plugin.Instance.TourneyMenuOpen) return;
                UI.Menu.ScreenMenuMain menu = screenMenuMain as UI.Menu.ScreenMenuMain;
                if (menu == null) return;
                UIScreen.SetFocus(menu.btTourney);
                Plugin.Instance.TourneyMenuOpen = false;
                Plugin.Instance.RulesetsMenuOpen = false;
            })
        );
        return cm.InstructionEnumeration();
    }
    
    //void GameStatesUnlocks::ProcessMsg(GameState state, Message message)
    [HarmonyPatch(typeof(OGKPCMDOMPF), nameof(OGKPCMDOMPF.ProcessMsg))]
    [HarmonyPrefix]
    private static bool GameStatesUnlocks_ProcessMsg_Prefix(JOFJHDJHJGI OHBPPCEFBHI, Message EIMJOIEPMNA)
    {
        Msg msg = EIMJOIEPMNA.msg;
        if (!Plugin.Instance.RulesetsMenuOpen) return true;
        if (msg != Msg.BACK) return true;
        
        OGKPCMDOMPF.screenMenu.SetActive(true);
        OGKPCMDOMPF.screenMenu.btBack.OnHoverOut(-1);
        // void GameStatesMenu::JumpTo(ScreenType screenType, ScreenTransition transition = ScreenTransition.NONE, bool backSound = false, ...)
        GameStates.ClearMessages();
        IOGKKINMEFB.CDAGGNOHLNK(ScreenType.MENU_VERSUS, ScreenTransition.MOVE_RIGHT, true);
        return false;
    }
    
    // void GameStatesMenu::JumpTo(ScreenType subMenu, ScreenTransition transition = ScreenTransition.NONE, bool backSound = false, string errorTitle = "", string errorMessage = "", int errorTitleSize = -1, string focusButton = null, bool toIntro = false)
    [HarmonyPatch(typeof(IOGKKINMEFB), nameof(IOGKKINMEFB.CDAGGNOHLNK))]
    [HarmonyPrefix]
    private static void GameStatesMenu_JumpTo_Prefix(ref ScreenType FJOFNHPCBPD)
    {
        if (FJOFNHPCBPD == ScreenType.MENU_ONLINE && Plugin.Instance.TourneyMenuOpen) FJOFNHPCBPD = ScreenType.MENU_VERSUS;
    }

    [HarmonyPatch(typeof(ScreenPlayersStage), nameof(ScreenPlayersStage.SelectionDone))]
    [HarmonyPrefix]
    private static bool ScreenPlayersStage_SelectionDone_Prefix(ScreenPlayersStage __instance)
    {
        IStageSelect stageSelect = __instance as IStageSelect;
        if (stageSelect == null) return true;
        stageSelect.OnStageSelected();
        return false;
    }
    
    /*
    // THIS CODE PREVENTS STAGE SELECTIONS FROM GOING THROUGH AND ALLOWS "VOTING" IN LOCAL - TESTING ONLY
    // void GameStatsLobby::ProcessMsgStageSelect(Message message)
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.LKBFKGGCFHE))]
    [HarmonyPrefix]
    private static bool GameStatesLobby_ProcessMsgStageSelect_Prefix(HPNLMFHPHFD __instance, Message EIMJOIEPMNA)
    {
        if (EIMJOIEPMNA.msg == Msg.SEL_STAGE)
        {
            __instance.CFKCIJCEILI.SelectionDone();
            return false;
        }
        return true;
    }
    
    // void GameStatsLobbyOnline::StageSelected(int playerNr, int stageIndex)
    [HarmonyPatch(typeof(HDLIJDBFGKN), nameof(HDLIJDBFGKN.MNLFJDLDHEN))]
    [HarmonyPrefix]
    private static bool GameStatesLobbyOnline_StageSelected_Prefix()
    {
        return false;
    }
    */

    [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.ShowCpuButtons))]
    [HarmonyPrefix]
    private static void ScreenPlayers_ShowCpuButtons_Prefix(ScreenPlayers __instance, ref bool visible)
    {
        ScreenLobbyTourney screenLobbyTourney = __instance as ScreenLobbyTourney;
        if (screenLobbyTourney == null) return;
        visible = false;
    }
    
    [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.UpdateTeamButtons))]
    [HarmonyPrefix]
    private static bool ScreenPlayers_UpdateTeamButtons_Prefix(ScreenPlayers __instance)
    {
        ScreenLobbyTourney screenLobbyTourney = __instance as ScreenLobbyTourney;
        if (screenLobbyTourney == null) return true;

        foreach (PlayersSelection playerSelection in __instance.playerSelections)
        {
            playerSelection.btTeam.SetActive(false);
        }

        return false;
    }

    // void Player::ResetTeam(GameMode gameMode, bool changeAnyway)
    [HarmonyPatch(typeof(ALDOKEMAOMB), nameof(ALDOKEMAOMB.MLFHMGNCMNA))]
    [HarmonyPrefix]
    private static bool Player_ResetTeam_Prefix(ALDOKEMAOMB __instance)
    {
        if (SetTracker.Instance.ActiveTourneyMode == TourneyMode.NONE) return true;

        Player player = __instance;
        if (SetTracker.Instance.IsMode1v1) player.Team = player.nr == 0 ? Team.RED : Team.BLUE;
        else if (SetTracker.Instance.IsModeDoubles) player.Team = player.nr <= 1 ? Team.RED : Team.BLUE;
        
        return false;
    }
    
    // bool GameStatesLobby::IsStartEnabled()
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.DJHHLDPLFMD))]
    [HarmonyPrefix]
    private static bool GameStatesLobby_IsStartEnabled_Prefix(ref bool __result)
    {
        if (!SetTracker.Instance.IsModeDoubles) return true;

        int nReady = 0;
        Player.ForAllInMatch(player =>
        {
            if (player.selected) nReady++;
        });
        __result = nReady == 4;
        
        return false;
    }
}