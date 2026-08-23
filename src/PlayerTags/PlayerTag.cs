using System.Collections.Generic;
using LLHandlers;

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
        movementKeys = MovementKeys.ARROWS;
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

    internal MovementKeys GetMovementKeys()
    {
        if (movementKeys == MovementKeys.NONE)
        {
            if (!IsDefault) Plugin.LogGlobal.LogWarning($"Tag '{name}' keyboard movement keys not found: creating defaults");
            SetMovementKeys(MovementKeys.ARROWS);
        }

        return movementKeys;
    }

    internal void SetMovementKeys(MovementKeys mk)
    {
        movementKeys = mk;
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