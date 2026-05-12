using LLGUI;
using LLScreen;
using UnityEngine;

namespace TourneyMod.UI.PlayerTags;

public class ScreenInput : ScreenOptions, ICustomScreen<ScreenOptions>
{
    internal PlayerTagMenuOptions[] playerTagMenus;
    
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

        Plugin.Instance.InputMenuEditingTags = [false, false, false, false, false];
    }

    public override void OnClose(ScreenType screenTypeNext)
    {
        base.OnClose(screenTypeNext);
        
        Plugin.Instance.InputMenuEditingTags = [false, false, false, false, false];
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
            
            CustomOptionsBarInputConfig custom = bar as CustomOptionsBarInputConfig;
            if (custom != null)
            {
                custom.inputElements.ForEach(element => element?.UpdateLooks());
                continue;
            }
        }
    }
}