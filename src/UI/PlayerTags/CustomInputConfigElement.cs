using LLGUI;
using TourneyMod.PlayerTags;
using UnityEngine;

namespace TourneyMod.UI.PlayerTags;

public class CustomInputConfigElement : InputConfigElement
{
    internal static readonly Color COLOR_TAG_DEFAULT = Color.red;
    internal static readonly Color COLOR_TAG_CUSTOM = Color.white;

    internal CustomInputConfigBarType customInputConfigBarType;
    
    public CustomInputConfigElement(LLButton _button, JBKFDDKLDDG _inputConfigController, int _inputAction, bool _altInput, CustomInputConfigBarType _customInputConfigBarType) : base(_button, _inputConfigController, _inputAction, _altInput, InputConfigBarType.EMPTY)
    {
        customInputConfigBarType = _customInputConfigBarType;
    }

    internal void CustomUpdateLooks()
    {
        if (customInputConfigBarType == CustomInputConfigBarType.TAG_MENU_TOGGLE)
        {
            PlayerTag playerTag = Plugin.Instance.GetPlayerTag(inputConfigController.GDEMBCKIDMA);
            SetText(playerTag.IsDefault ? "DEFAULT" : playerTag.GetName());
            button.colDefault = playerTag.IsDefault ? COLOR_TAG_DEFAULT : COLOR_TAG_CUSTOM;
            button.textMesh.color = playerTag.IsDefault ? COLOR_TAG_DEFAULT : COLOR_TAG_CUSTOM;
        }
    }
}