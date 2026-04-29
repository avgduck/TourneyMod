namespace TourneyMod.PlayerTags;

public class PlayerTag
{
    internal static readonly PlayerTag DEFAULT = new PlayerTag(true);

    internal bool IsDefault { get; private set; }
    private PlayerTag(bool isDefault)
    {
        IsDefault = isDefault;
    }

    internal PlayerTag()
    {
        IsDefault = false;
    }
    
    private string name;

    internal void SetName(string name)
    {
        if (this.name != null) Plugin.LogGlobal.LogWarning($"Failed to set name '{name}' for player tag with existing name '{this.name}'");
        else this.name = name;
    }

    internal string GetName()
    {
        return name;
    }
}