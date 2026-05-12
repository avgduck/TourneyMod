using System.Collections.Generic;
using LLGUI;
using LLHandlers;
using LLScreen;
using TourneyMod.PlayerTags;
using UnityEngine;

namespace TourneyMod.UI.PlayerTags;

public class ScreenInput : ScreenOptions, ICustomScreen<ScreenOptions>
{
    internal PlayerTagMenuOptions[] playerTagMenus;
    private bool[] EditingTags = [false, false, false, false, false];
    
    public void Init(ScreenOptions screenOptions)
    {
        screenType = screenOptions.screenType;
        layer = screenOptions.layer;
        isActive = screenOptions.isActive;
        msgEsc = screenOptions.msgEsc;
        msgMenu = screenOptions.msgMenu;
        msgCancel = screenOptions.msgCancel;

        pfBarButton = screenOptions.pfBarButton;
        pfBarToggle = screenOptions.pfBarToggle;
        pfBarImageToggle = screenOptions.pfBarImageToggle;
        pfBarSlider = screenOptions.pfBarSlider;
        pfBarInputConfig = screenOptions.pfBarInputConfig;
        pfBarEnumLanguage = screenOptions.pfBarEnumLanguage;
        pfBarEnum = screenOptions.pfBarEnum;
        tfBars = screenOptions.tfBars;
        offsetBars = screenOptions.offsetBars;
        posBarNext = screenOptions.posBarNext;
        optionBars = screenOptions.optionBars;
        btApply = screenOptions.btApply;
    }

    public override void OnOpen(ScreenType screenTypePrev)
    {
        base.OnOpen(screenTypePrev);

        EditingTags = [false, false, false, false, false];
    }

    public override void GetControls(ref List<LLClickable> list, bool vert, LLClickable curFocus, LLCursor cursor)
    {
        if (!vert) return;
        JBKFDDKLDDG inputConfigController = GetInputConfigController(cursor);

        playerTagMenus[inputConfigController.LNDBODJBNFM].GetControls(ref list, curFocus, cursor);
        foreach (OptionsBar optionsBar in optionBars)
        {
            optionsBar.GetControls(ref list, curFocus, cursor);
        }
    }

    public override bool DirectMove(Vector2 move, LLClickable curFocus, bool shouldMove)
    {
        int playerIndex = -1;

        for (int i = 0; i < playerTagMenus.Length; i++)
        {
            if (playerTagMenus[i].CheckControlFocus(curFocus))
            {
                playerIndex = i;
                break;
            }
        }

        return playerIndex != -1 && playerTagMenus[playerIndex].DirectMove(move, curFocus, shouldMove);
    }

    private JBKFDDKLDDG GetInputConfigController(LLCursor cursor)
    {
        // GameStatesOptions.inputConfigControllers
        for (int i = 0; i < HGFCCNMEEEF.inputConfigControllers.Count; i++)
        {
            JBKFDDKLDDG inputConfigController = HGFCCNMEEEF.inputConfigControllers[i];
            // inputConfigController.cursor
            if (inputConfigController.OBELDJGOOIJ == cursor) return inputConfigController;
        }

        return null;
    }

    internal CustomOptionsBarInputConfig AddCustomInputConfigBar(CustomInputConfigBarType customInputConfigBarType, string text)
    {
        GameObject gameObject = pfBarInputConfig;
        Transform transform = Instantiate<GameObject>(gameObject).transform;
        transform.SetParent(tfBars, false);
        transform.localPosition = new Vector3(Mathf.Round(posBarNext.x), Mathf.Round(posBarNext.y), 0f);
        transform.localScale = Vector3.one;

        OptionsBarInputConfig optionsBarInputConfig = transform.GetComponent<OptionsBarInputConfig>();
        CustomOptionsBarInputConfig customOptionsBarInputConfig = gameObject.AddComponent<CustomOptionsBarInputConfig>();
        
        customOptionsBarInputConfig.Copy(optionsBarInputConfig);
        customOptionsBarInputConfig.customInputConfigBarType = customInputConfigBarType;
        customOptionsBarInputConfig.screenInput = this;
        DestroyImmediate(optionsBarInputConfig);
        
        HNEDEAGADKO configVarNone = HNEDEAGADKO.NMJDMHNMDNJ;
        customOptionsBarInputConfig.Init(text, configVarNone, Msg.SEL_CONFIG, -1);
        optionBars.Add(customOptionsBarInputConfig);
        posBarNext += offsetBars;
        return customOptionsBarInputConfig;
    }

    internal void SetupPlayerTagMenu()
    {
        playerTagMenus = new PlayerTagMenuOptions[5];
        for (int playerIndex = 0; playerIndex < 5; playerIndex++)
        {
            playerTagMenus[playerIndex] = PlayerTagMenuOptions.CreateMenu(tfBars, playerIndex, this);
        }
    }

    internal void UpdateBarButtons()
    {
        foreach (OptionsBar optionsBar in optionBars)
        {
            OptionsBarInputConfig bar = optionsBar as OptionsBarInputConfig;
            if (bar == null) continue;
            
            CustomOptionsBarInputConfig custom = bar as CustomOptionsBarInputConfig;
            if (custom != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    PlayerTag tag = Plugin.Instance.GetPlayerTag(Controller.FromNr(i, false));
                    tag.SetEditing(false);
                }
                custom.inputElements.ForEach(element =>
                {
                    if (element == null) return;
                    // inputConfigController.isEditing
                    EditingTags[element.inputConfigController.LNDBODJBNFM] = element.inputConfigController.DJFKIGINECC;
                    PlayerTag tagSelf = Plugin.Instance.GetPlayerTag(element.inputConfigController.GDEMBCKIDMA);
                    if (EditingTags[element.inputConfigController.LNDBODJBNFM]) tagSelf.SetEditing(true);
                    element.UpdateLooks();
                    element.button.SetActive(!EditingTags[element.inputConfigController.LNDBODJBNFM]);
                });
                continue;
            }

            if (bar.inputConfigBarType is InputConfigBarType.BUTTON1 or InputConfigBarType.BUTTON2)
            {
                bar.inputElements.ForEach(element =>
                {
                    if (element == null) return;
                    // inputConfigController.index
                    bool tagMenuOpen = playerTagMenus[element.inputConfigController.LNDBODJBNFM].gameObject.activeSelf;
                    // inputConfigController.controller
                    bool tagDefault = Plugin.Instance.GetPlayerTag(element.inputConfigController.GDEMBCKIDMA).IsDefault;

                    PlayerTag tagSelf = Plugin.Instance.GetPlayerTag(element.inputConfigController.GDEMBCKIDMA);
                    bool editingOther = tagSelf.GetEditing() && !EditingTags[element.inputConfigController.LNDBODJBNFM];
                    
                    element.button.SetActive(!tagMenuOpen && !tagDefault && !editingOther);
                });
            }
        }

        bool blockGlobalInput = false;
        
        foreach (bool edit in EditingTags)
        {
            if (edit) blockGlobalInput = true;
        }
        foreach (PlayerTagMenuOptions tagMenu in playerTagMenus)
        {
            if (tagMenu.gameObject.activeSelf) blockGlobalInput = true;
        }
        
        UIScreen.blockGlobalInput = blockGlobalInput;
        // GameStatesMenu.SetBackButtonVisible(bool visible)
        IOGKKINMEFB.GMBFKKNCMOO(!blockGlobalInput);
    }
}