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
        UpdateBarButtons();
    }

    public override void OnClose(ScreenType screenTypeNext)
    {
        base.OnClose(screenTypeNext);
        
        EditingTags = [false, false, false, false, false];
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
    }
}