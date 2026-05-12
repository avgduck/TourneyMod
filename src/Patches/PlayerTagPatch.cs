using HarmonyLib;
using LLGUI;
using LLHandlers;
using TourneyMod.UI.PlayerTags;
using UnityEngine;

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
    
    [HarmonyPatch(typeof(OptionsBarInputConfig), nameof(OptionsBarInputConfig.AddController))]
    [HarmonyPrefix]
    private static bool OptionsBarInputConfig_AddController_Prefix(OptionsBarInputConfig __instance, JBKFDDKLDDG inputConfigController)
    {
        CustomOptionsBarInputConfig custom = __instance as CustomOptionsBarInputConfig;
        if (custom == null) return true;
        
        custom.CustomAddController(inputConfigController);
        return false;
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
}