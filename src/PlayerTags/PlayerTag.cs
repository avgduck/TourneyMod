using System.Collections.Generic;
using LLHandlers;
using UnityEngine;

namespace TourneyMod.PlayerTags;

public class PlayerTag
{
    internal static readonly PlayerTag DEFAULT = new PlayerTag(true);

    internal bool IsDefault { get; private set; }
    private PlayerTag(bool isDefault)
    {
        IsDefault = isDefault;
        name = "";
        isEditing = false;
        inputBindings = new Dictionary<string, List<InputConfigAssignment>>();
        movementKeys = MovementKeys.NONE;
        customMovementKeys = [
            [KeyCode.LeftArrow, KeyCode.Joystick8Button19],
            [KeyCode.RightArrow, KeyCode.Joystick8Button19],
            [KeyCode.UpArrow, KeyCode.Joystick8Button19],
            [KeyCode.DownArrow, KeyCode.Joystick8Button19]
        ];
        customTauntKeys = [
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4
        ];
    }

    internal PlayerTag()
    {
        IsDefault = false;
        isEditing = false;
        inputBindings = new Dictionary<string, List<InputConfigAssignment>>();
    }
    
    private string name;
    private bool isEditing;

    public MovementKeys movementKeys;
    public KeyCode[][] customMovementKeys;
    public KeyCode[] customTauntKeys;
    public Dictionary<string, List<InputConfigAssignment>> inputBindings;

    internal void SetName(string name)
    {
        if (this.name != null) Plugin.LogGlobal.LogWarning($"Failed to set name '{name}' for player tag with existing name '{this.name}'");
        else this.name = name;
    }

    internal string GetName()
    {
        return name;
    }

    internal void SetEditing(bool isEditing)
    {
        this.isEditing = isEditing;
    }

    internal bool GetEditing()
    {
        return isEditing;
    }

    internal KeyCode[][] GetCustomMovementKeys()
    {
        if (customMovementKeys == null || customMovementKeys.Length != 4)
        {
            if (movementKeys != MovementKeys.NONE)
            {
                if (!IsDefault) Plugin.LogGlobal.LogWarning($"Tag '{name}' migrating keyboard movement keys to new system");
                SetCustomMovementKeys(Plugin.Instance.GetCustomMovementKeys(movementKeys));
            }
            else
            {
                if (!IsDefault) Plugin.LogGlobal.LogWarning($"Tag '{name}' keyboard custom movement keys not found: creating defaults");
                SetCustomMovementKeys(Plugin.Instance.GetCustomMovementKeys(MovementKeys.ARROWS));
            }
        }

        return customMovementKeys;
    }

    internal void SetMovementKeys(MovementKeys mk)
    {
        //movementKeys = mk;
        SetCustomMovementKeys(Plugin.Instance.GetCustomMovementKeys(mk));
        if (!IsDefault) PlayerTagIO.SavePlayerTag(this);
    }

    internal void SetCustomMovementKeys(KeyCode[][] cmk)
    {
        movementKeys = MovementKeys.NONE;
        customMovementKeys = cmk;
        if (!IsDefault) PlayerTagIO.SavePlayerTag(this);
    }

    internal KeyCode[] GetCustomTauntKeys()
    {
        if (customTauntKeys == null || customTauntKeys.Length != 4)
        {
            SetCustomTauntKeys([KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4]);
        }

        return customTauntKeys;
    }

    internal void SetCustomTauntKeys(KeyCode[] ctk)
    {
        customTauntKeys = ctk;
        if (!IsDefault) PlayerTagIO.SavePlayerTag(this);
    }

    internal List<InputConfigAssignment> GetBindings(string hardwareName)
    {
        List<InputConfigAssignment> list;
        if (!inputBindings.ContainsKey(hardwareName))
        {
            if (!IsDefault) Plugin.LogGlobal.LogWarning($"Tag '{name}' bindings for hardware '{hardwareName}' not found: creating defaults");
            list = InputHandler.GetDefaultConfig(hardwareName);
            SetBindings(hardwareName, list);
        }
        else
        {
            list = inputBindings[hardwareName];
        }
        
        return list;
    }

    internal void SetBindings(string hardwareName, List<InputConfigAssignment> list)
    {
        if (inputBindings.ContainsKey(hardwareName)) inputBindings[hardwareName] = list;
        else inputBindings.Add(hardwareName, list);

        if (!IsDefault)
        {
            PlayerTagIO.SavePlayerTag(this);
            Plugin.Instance.UpdateAllWithTag(this);
        }
    }
}