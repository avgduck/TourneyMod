using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using LLGUI;
using LLHandlers;
using LLScreen;
using Rewired;
using TourneyMod.PlayerTags;
using TourneyMod.UI.PlayerTags;
using UnityEngine;
using Controller = LLHandlers.Controller;
using ControllerType = Rewired.ControllerType;
using InputAction = LLHandlers.InputAction;

namespace TourneyMod.Patches;

public class PlayerTagPatch
{
    [HarmonyPatch(typeof(HGFCCNMEEEF), nameof(HGFCCNMEEEF.JLGCJFEFDLI))]
    [HarmonyPostfix]
    private static void GameStatesOptions_InputConfigAddController_Postfix(Controller GDEMBCKIDMA)
    {
        Controller controller = GDEMBCKIDMA;
        //Plugin.LogGlobal.LogWarning($"InputConfigAddController: {controller}");
    }
    
    [HarmonyPatch(typeof(HGFCCNMEEEF), nameof(HGFCCNMEEEF.PCBBCFNFDJL))]
    [HarmonyPrefix]
    private static bool GameStatesOptions_SetupBarsInput_Prefix(HGFCCNMEEEF __instance)
    {
        ScreenInput screenInput = __instance.CNINOFJOLNP as ScreenInput;
        if (screenInput == null) return true;

        HNEDEAGADKO configVarNone = HNEDEAGADKO.NMJDMHNMDNJ;
        HNEDEAGADKO configVarInput = HNEDEAGADKO.CGJIJHCPEPE;
        
        screenInput.offsetBars = new Vector3(-6.4f, -36f, 0f);
        screenInput.posBarNext -= screenInput.offsetBars;
        PlayerTagMenuOptions.BarStartPos = screenInput.posBarNext;
        PlayerTagMenuOptions.BarOffset = screenInput.offsetBars;
        
        OptionsBarInputConfig optionsBarInputTitle = (OptionsBarInputConfig)screenInput.AddBar(OptionsBarType.INPUT_CONFIG, string.Empty, configVarNone);
        optionsBarInputTitle.inputConfigBarType = InputConfigBarType.TITLES;

        foreach (int inputAction in InputAction.EConfigurables())
        {
            string inputActionName = TextHandler.GetInputActionName(inputAction);
            
            OptionsBarInputConfig optionsBarInputAction = (OptionsBarInputConfig)screenInput.AddBar(OptionsBarType.INPUT_CONFIG, inputActionName, configVarInput);
            optionsBarInputAction.inputAction = inputAction;

            if (inputAction == InputAction.SWING || inputAction == InputAction.BUNT || inputAction == InputAction.JUMP || inputAction == InputAction.GRAB)
            {
                OptionsBarInputConfig optionsBarInputActionAlt = (OptionsBarInputConfig)screenInput.AddBar(OptionsBarType.INPUT_CONFIG, string.Empty, configVarInput);
                optionsBarInputActionAlt.inputAction = inputAction;
                optionsBarInputActionAlt.altInput = true;
            }
        }

        OptionsBarInputConfig optionsBarInputMovement = (OptionsBarInputConfig)screenInput.AddBar(OptionsBarType.INPUT_CONFIG, TextHandler.Get("OPTIONS_MOVEMENT", new string[0]), configVarNone);
        optionsBarInputMovement.inputConfigBarType = InputConfigBarType.MOVEMENT;

        CustomOptionsBarInputConfig customOptionsBarInputConfig = screenInput.AddCustomInputConfigBar(CustomInputConfigBarType.TAG_MENU_TOGGLE, "Tag");
        
        OptionsBarInputConfig optionsBarInputButton1 = (OptionsBarInputConfig)screenInput.AddBar(OptionsBarType.INPUT_CONFIG, string.Empty, configVarNone);
        optionsBarInputButton1.inputConfigBarType = InputConfigBarType.BUTTON1;
        OptionsBarInputConfig optionsBarInputButton2 = (OptionsBarInputConfig)screenInput.AddBar(OptionsBarType.INPUT_CONFIG, string.Empty, configVarNone);
        optionsBarInputButton2.inputConfigBarType = InputConfigBarType.BUTTON2;
        
        screenInput.SetupPlayerTagMenu();
        
        return false;
    }

    [HarmonyPatch(typeof(HGFCCNMEEEF), nameof(HGFCCNMEEEF.GDOBNEONNII))]
    [HarmonyPostfix]
    private static void GameStatesOptions_InitInputConfig_Postfix(HGFCCNMEEEF __instance)
    {
        ScreenInput screenInput = __instance.CNINOFJOLNP as ScreenInput;
        if (screenInput == null) return;
        
        screenInput.UpdateBarButtons();
    }
    
    [HarmonyPatch(typeof(OptionsBarInputConfig), nameof(OptionsBarInputConfig.AddController))]
    [HarmonyPrefix]
    private static bool OptionsBarInputConfig_AddController_Prefix(OptionsBarInputConfig __instance, JBKFDDKLDDG inputConfigController)
    {
        CustomOptionsBarInputConfig custom = __instance as CustomOptionsBarInputConfig;
        if (custom == null) return true;
        
        custom.CustomAddController(inputConfigController);
        return false;
    }

    [HarmonyPatch(typeof(ScreenOptions), nameof(ScreenOptions.RemoveController))]
    [HarmonyPostfix]
    private static void ScreenOptions_RemoveController_Prefix(ScreenOptions __instance, JBKFDDKLDDG inputConfigController)
    {
        ScreenInput screenInput = __instance as ScreenInput;
        if (screenInput == null) return;
        
        screenInput.OnRemoveController(inputConfigController);
    }
    
    [HarmonyPatch(typeof(InputConfigElement), nameof(InputConfigElement.UpdateLooks))]
    [HarmonyPrefix]
    private static bool InputConfigElement_UpdateLooks_Prefix(InputConfigElement __instance)
    {
        CustomInputConfigElement custom = __instance as CustomInputConfigElement;
        if (custom == null) return true;
        
        custom.CustomUpdateLooks();
        return false;
    }
    
    [HarmonyPatch(typeof(InputConfigElement), nameof(InputConfigElement.ButtonClick))]
    [HarmonyPrefix]
    private static bool InputConfigElement_ButtonClick_Prefix(InputConfigElement __instance, int playerNr)
    {
        CustomInputConfigElement custom = __instance as CustomInputConfigElement;
        if (custom == null) return true;
        
        custom.CustomButtonClick(playerNr);
        return false;
    }

    [HarmonyPatch(typeof(InputConfigElement), nameof(InputConfigElement.ButtonClick))]
    [HarmonyPostfix]
    private static void InputConfigElement_ButtonClick_Postfix(InputConfigElement __instance)
    {
        ScreenInput screenInput = UIScreen.GetScreen(ScreenType.OPTIONS) as ScreenInput;
        if (screenInput != null) screenInput.UpdateBarButtons();
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.LoadConfig))]
    [HarmonyPostfix]
    private static void InputHandler_LoadConfig_Postfix(Rewired.Player rePlayer, string hardwareName, Rewired.ControllerType controllerType)
    {
        Plugin.Instance.LoadTagConfig(rePlayer, hardwareName, controllerType);
        Plugin.Instance.LoadTagMovementKeys();
    }

    [HarmonyPatch(typeof(JBKFDDKLDDG), nameof(JBKFDDKLDDG.HHBBAKCECEP))]
    [HarmonyPrefix]
    private static void InputConfigController_Button1_Prefix(JBKFDDKLDDG __instance, ref bool __state)
    {
        __state = __instance.DJFKIGINECC;
    }
    [HarmonyPatch(typeof(JBKFDDKLDDG), nameof(JBKFDDKLDDG.HHBBAKCECEP))]
    [HarmonyPostfix]
    private static void InputConfigController_Button1_Postfix(JBKFDDKLDDG __instance, bool __state)
    {
        if (!__state) return;
        
        Controller controller = __instance.GDEMBCKIDMA;
        PlayerTag playerTag = Plugin.Instance.GetPlayerTag(controller);
        // InputConfig.GetInputConfig(...)
        if (controller.IncludesMouse()) playerTag.SetMovementKeys(InputHandler.movementKeys);
        playerTag.SetBindings(controller.GetHardwareName(), PPHBCKEFJEP.JMNLMPPOEDC(__instance.PIPEFDJDICP));
    }
    
    [HarmonyPatch(typeof(JBKFDDKLDDG), nameof(JBKFDDKLDDG.GMCFPNDNNJP))]
    [HarmonyPrefix]
    private static void InputConfigController_Button2_Prefix(JBKFDDKLDDG __instance, ref bool __state)
    {
        __state = __instance.DJFKIGINECC;
    }
    [HarmonyPatch(typeof(JBKFDDKLDDG), nameof(JBKFDDKLDDG.GMCFPNDNNJP))]
    [HarmonyPostfix]
    private static void InputConfigController_Button2_Postfix(JBKFDDKLDDG __instance, bool __state)
    {
        if (__state) return;
        
        Controller controller = __instance.GDEMBCKIDMA;
        PlayerTag playerTag = Plugin.Instance.GetPlayerTag(controller);
        // InputConfig.GetInputConfig(...)
        if (!controller.IncludesMouse()) InputHandler.SetMovementKeys(Plugin.Instance.SelectedPlayerTagKeyboard.GetMovementKeys());
        if (controller.IncludesMouse()) playerTag.SetMovementKeys(InputHandler.movementKeys);
        playerTag.SetBindings(controller.GetHardwareName(), PPHBCKEFJEP.JMNLMPPOEDC(__instance.PIPEFDJDICP));
    }

    /*
     * identical to the original method, but the output of map.ElementMapsWithAction(...) is sorted by id
     * for SOME REASON, when the main input is a trigger, the order of the output gets swapped, causing the alt input to be first
     * this causes a vile bug that makes the input bindings completely unpredictable/unusable
     * (bad on the input screen when you can see the mistake, catastrophic on swapping player tags in the lobby screen when you can't)
     */
    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.GetActionElementMap))]
    [HarmonyPrefix]
    private static bool InputHandler_GetActionElementMap_Prefix(ref ActionElementMap __result, ControllerMap map, int inputAction, bool altInput)
    {
        ActionElementMap result = null;
        bool flag = true;
        
        foreach (ActionElementMap actionElementMap in map.ElementMapsWithAction(inputAction).OrderBy(actionElementMap => actionElementMap.id))
        {
            if (!flag || !altInput)
            {
                result = actionElementMap;
                break;
            }
            
            flag = false;
        }

        __result = result;
        return false;
    }
    
    /*
    [HarmonyPatch(typeof(HGFCCNMEEEF), nameof(HGFCCNMEEEF.BFGFOCFBGMJ))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GameStatesOptions_UpdateInputConfig_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
    {
        CodeMatcher cm = new CodeMatcher(instructions, ilGenerator);
        cm.Start();
        // ControllerPollingInfo controllerPollingInfo = inputConfigController.controller.Poll();
        // ControllerPollingInfo controllerPollingInfo = jbkfddklddg.GDEMBCKIDMA.Poll();
        cm.MatchForward(true,
            new CodeMatch(OpCodes.Ldloc_0), // load inputConfigController
            new CodeMatch(OpCodes.Ldflda, AccessTools.Field(typeof(JBKFDDKLDDG), nameof(JBKFDDKLDDG.GDEMBCKIDMA))), // load inputConfigController.controller
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Controller), nameof(Controller.Poll))), // call controller.Poll()
            new CodeMatch(OpCodes.Stloc_2) // store it in controllerPollingInfo
        );
        cm.Advance(1); // move past stloc.2
        cm.Insert(
            new CodeInstruction(OpCodes.Ldloc_0), // inputConfigController
            new CodeInstruction(OpCodes.Ldloc_2), // controllerPollingInfo
            Transpilers.EmitDelegate<Action<JBKFDDKLDDG, ControllerPollingInfo>>((inputConfigController, controllerPollingInfo) =>
            {
                if (!controllerPollingInfo.success) return;
                //Plugin.LogGlobal.LogWarning($"controller '{controllerPollingInfo.controller}' poll success:");
                //if (inputConfigController.NNGJKLIIDNI == ControllerType.Keyboard) Plugin.LogGlobal.LogWarning($"kb {controllerPollingInfo.keyboardKey}");
                //else Plugin.LogGlobal.LogWarning($"pad {controllerPollingInfo.elementType}/{controllerPollingInfo.elementIdentifierId} '{controllerPollingInfo.elementIdentifierName.ToLower()}'");
            })
        );
        // if (!waitingElement.curAssignment.SameAs(inputConfigAssignment)
        // if (!dopgijccnld.curAssignment.SameAs(inputConfigAssignment)
        cm.MatchForward(false,
            new CodeMatch(OpCodes.Ldloc_S), // load waitingElement
            new CodeMatch(OpCodes.Ldflda, AccessTools.Field(typeof(InputConfigElement), nameof(InputConfigElement.curAssignment))), // load waitingElement.curAssignment
            new CodeMatch(OpCodes.Ldloc_S), // load inputConfigAssignment
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(InputConfigAssignment), nameof(InputConfigAssignment.SameAs))), // call curAssignment.SameAs(inputConfigAssignment)
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Leave)
        );
        LocalBuilder refWaitingElement = (LocalBuilder)cm.Instruction.operand;
        cm.Advance(2);
        LocalBuilder refInputConfigAssignment = (LocalBuilder)cm.Instruction.operand;
        cm.Advance(4); // go past leave
        cm.Insert(
            new CodeInstruction(OpCodes.Ldloc_0), // inputConfigController
            new CodeInstruction(OpCodes.Ldloc_S, refWaitingElement),
            new CodeInstruction(OpCodes.Ldloc_S, refInputConfigAssignment),
            Transpilers.EmitDelegate<Action<JBKFDDKLDDG, InputConfigElement, InputConfigAssignment>>((inputConfigController, waitingElement, inputConfigAssignment) =>
            {
                Plugin.LogGlobal.LogWarning($"Attempting to set input for '{InputHandler.ActionToDesc(inputConfigAssignment.inputAction)}'/{inputConfigAssignment.altInput}: {waitingElement.curAssignment.elementType}/{waitingElement.curAssignment.elementId} -> {inputConfigAssignment.elementType}/{inputConfigAssignment.elementId}");

                if (inputConfigAssignment.altInput)
                {
                    // InputConfig.GetAssignment(...)
                    InputConfigAssignment assignment = PPHBCKEFJEP.LIPIINMJBCC(inputConfigController.PIPEFDJDICP, waitingElement.inputAction, false);
                    Plugin.LogGlobal.LogWarning($"Cur assignment main: {assignment.elementType}/{assignment.elementId}: same {inputConfigAssignment.SameInputAs(assignment)}");
                }
            })
        );
        cm.CreateLabel(out Label label);
        cm.MatchBack(false,
            new CodeMatch(OpCodes.Brfalse)
        );
        cm.SetOperandAndAdvance(label);
        return cm.InstructionEnumeration();
    }
    
    [HarmonyPatch(typeof(InputConfigElement), nameof(InputConfigElement.UpdateLooks))]
    [HarmonyPostfix]
    private static void InputConfigElement_UpdateLooks_Postfix(InputConfigElement __instance)
    {
        if (__instance.inputConfigBarType != InputConfigBarType.ACTION) return;

        ActionElementMap actionElementMap = InputHandler.GetActionElementMap(__instance.inputConfigController.PIPEFDJDICP, __instance.inputAction, __instance.altInput);
        if (actionElementMap == null) __instance.SetText("-");
        else __instance.SetText($"{actionElementMap.elementType}/{actionElementMap.elementIdentifierId}");
    }

    [HarmonyPatch(typeof(PPHBCKEFJEP), nameof(PPHBCKEFJEP.IJJPHFJAMGK))]
    [HarmonyPrefix]
    private static void InputConfig_SetAssignment_Prefix(ControllerMap PIPEFDJDICP, InputConfigAssignment ACIHFIBJNKM)
    {
        Plugin.LogGlobal.LogWarning($"SetAssignment '{InputHandler.ActionToDesc(ACIHFIBJNKM.inputAction)}'/{ACIHFIBJNKM.altInput}: {ACIHFIBJNKM.elementType}/{ACIHFIBJNKM.elementId}");
    }
    
    [HarmonyPatch(typeof(PPHBCKEFJEP), nameof(PPHBCKEFJEP.IJJPHFJAMGK))]
    [HarmonyPostfix]
    private static void InputConfig_SetAssignment_Postfix(ControllerMap PIPEFDJDICP, InputConfigAssignment ACIHFIBJNKM)
    {
        // InputConfig.GetAssignment(...)
        InputConfigAssignment assignment = PPHBCKEFJEP.LIPIINMJBCC(PIPEFDJDICP, ACIHFIBJNKM.inputAction, ACIHFIBJNKM.altInput);
        Plugin.LogGlobal.LogWarning($"GetAssignment '{InputHandler.ActionToDesc(assignment.inputAction)}'/{assignment.altInput}: {assignment.elementType}/{assignment.elementId}");
        if (ACIHFIBJNKM.altInput)
        {
            InputConfigAssignment mainAssignment = PPHBCKEFJEP.LIPIINMJBCC(PIPEFDJDICP, ACIHFIBJNKM.inputAction, false);
            Plugin.LogGlobal.LogWarning($"GetAssignment '{InputHandler.ActionToDesc(mainAssignment.inputAction)}'/false: {mainAssignment.elementType}/{mainAssignment.elementId}");
        }
        
        Plugin.LogGlobal.LogWarning($"{PIPEFDJDICP.ElementMapsWithAction(ACIHFIBJNKM.inputAction).OrderBy(actionElementMap => actionElementMap.id).Join(map => map.ToString(), " | ")}");
    }
    */

}