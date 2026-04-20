namespace TourneyMod.PlayerTags;

public class PlayerTag
{
    public string Name { get; private set; }

    internal void InitName(string name)
    {
        if (Name != null) Plugin.LogGlobal.LogWarning($"Failed to set name '{name}' for player tag with existing name '{Name}'");
        else Name = name;
    }
}