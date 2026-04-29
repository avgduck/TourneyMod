namespace TourneyMod.PlayerTags;

public class PlayerTag
{
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