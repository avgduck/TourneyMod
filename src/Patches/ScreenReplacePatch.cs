using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using LLBML.Players;
using LLScreen;
using TourneyMod.SetTracking;
using TourneyMod.UI;
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
            ReplaceScreen<LLScreen.ScreenMenuMain, UI.ScreenMenuMain>(ref __result);
        }
        else if (screenType == ScreenType.MENU_VERSUS && Plugin.Instance.TourneyMenuOpen)
        {
            ReplaceScreen<ScreenMenuVersus, ScreenMenuTourney>(ref __result);
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
        UI.ScreenMenuMain menu = __instance as UI.ScreenMenuMain;
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
                UI.ScreenMenuMain menu = screenMenuMain as UI.ScreenMenuMain;
                if (menu == null) return;
                UIScreen.SetFocus(menu.btTourney);
                Plugin.Instance.TourneyMenuOpen = false;
            })
        );
        return cm.InstructionEnumeration();
    }

    /*
    // IEnumerator GameStatesLobbyOnline::CLeaveLobby()
    [HarmonyPatch(typeof(HDLIJDBFGKN), nameof(HDLIJDBFGKN.CNPOCEPGNCM))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GameStatesLobbyOnline_CLeaveLobby_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher cm = new CodeMatcher(instructions);
        return cm.InstructionEnumeration();
    }
    */
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
}