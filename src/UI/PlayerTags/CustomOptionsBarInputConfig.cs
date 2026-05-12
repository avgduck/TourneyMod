using LLGUI;
using LLHandlers;
using UnityEngine;

namespace TourneyMod.UI.PlayerTags;

public class CustomOptionsBarInputConfig : OptionsBarInputConfig
{
    internal CustomInputConfigBarType customInputConfigBarType;
    internal ScreenInput screenInput;
    
    internal void Copy(OptionsBarInputConfig copy)
    {
        barType = copy.barType;
        lbText = copy.lbText;
        btButton = copy.btButton;
        imBar = copy.imBar;
        btImage = copy.btImage;
        imToggle = copy.imToggle;
        lbValue = copy.lbValue;
        rawSlider = copy.rawSlider;
        tfSlider = copy.tfSlider;
        enumSize = copy.enumSize;
        enumTextCodes = copy.enumTextCodes;
        buttonIndex = copy.buttonIndex;
        onChange = copy.onChange;
        curText = copy.curText;
        linkedConfigVar = copy.linkedConfigVar;
        msgAction = copy.msgAction;
        isEnabled = copy.isEnabled;
        curValueSlider = copy.curValueSlider;
        spriteToggle0 = copy.spriteToggle0;
        spriteToggle1 = copy.spriteToggle1;
        directMoveLast = copy.directMoveLast;
        
        pfOptionsBarInputConfigButton = copy.pfOptionsBarInputConfigButton;
        buttonOffset = copy.buttonOffset;
        inputAction = copy.inputAction;
        altInput = copy.altInput;
        inputConfigBarType = copy.inputConfigBarType;
        inputElements = copy.inputElements;
    }

    internal void CustomAddController(JBKFDDKLDDG inputConfigController)
    {
        if (inputElements.Count >= 5) return;

        Transform transform = Instantiate<GameObject>(pfOptionsBarInputConfigButton).transform;
        transform.SetParent(imBar.transform, false);
        transform.localScale = Vector3.one;
        
        LLButton component = transform.GetComponent<LLButton>();
        component.Init();
        component.SetFontSize(TextHandler.GetFontSize("MENU_SETTING"));
        component.ignoreMouseHover = !inputConfigController.GDEMBCKIDMA.IncludesMouse();
        
        inputElements.Add(new CustomInputConfigElement(component, inputConfigController, inputAction, altInput, customInputConfigBarType, screenInput));
        UpdateBar();
        UpdateButton(inputConfigController);
    }
}