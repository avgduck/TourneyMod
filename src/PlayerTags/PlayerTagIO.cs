using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyJson;

namespace TourneyMod.PlayerTags;

public static class PlayerTagIO
{
    private const string DIRECTORY_NAME = "player-tags";
    internal static DirectoryInfo PlayerTagsDirectory;
    internal static Dictionary<string, PlayerTag> PlayerTags;

    public static void Init()
    {
        DirectoryInfo moddingFolder = LLBML.Utils.ModdingFolder.GetModSubFolder(Plugin.Instance.Info);
        string path = Path.Combine(moddingFolder.FullName, DIRECTORY_NAME);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        PlayerTagsDirectory = new DirectoryInfo(path);

        PlayerTags = new Dictionary<string, PlayerTag>();
        foreach (FileInfo file in PlayerTagsDirectory.GetFiles().OrderBy(f => f.Name))
        {
            PlayerTag playerTag = LoadTagFile(file);

            if (playerTag == null) continue;
            if (PlayerTags.ContainsKey(playerTag.Name))
            {
                Plugin.LogGlobal.LogWarning($"Failed to load player tag '{playerTag.Name}': player tag with name '{playerTag.Name}' already exists");
                continue;
            }
            
            Plugin.LogGlobal.LogInfo($"Loaded player tag: {playerTag.Name}");
            PlayerTags.Add(playerTag.Name, playerTag);
        }
    }

    private static PlayerTag LoadTagFile(FileInfo file)
    {
        if (!file.Name.Contains(".json")) return null;
        string name = file.Name.Replace(".json", "");

        string json = JsonIO.ReadFile(file);
        PlayerTag playerTag = json.FromJson<PlayerTag>();
        playerTag.InitName(name);
        return playerTag;
    }

    internal static PlayerTag GetPlayerTagByName(string name)
    {
        if (PlayerTags.ContainsKey(name)) return PlayerTags[name];

        return null;
    }
}