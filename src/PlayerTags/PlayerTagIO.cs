using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
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
            if (PlayerTags.ContainsKey(playerTag.GetName().ToLower()))
            {
                Plugin.LogGlobal.LogWarning($"Failed to load player tag '{playerTag.GetName()}': player tag with name '{PlayerTags[playerTag.GetName().ToLower()].GetName()}' (case insensitive) already exists");
                continue;
            }
            
            Plugin.LogGlobal.LogInfo($"Loaded player tag: {playerTag.GetName()}");
            PlayerTags.Add(playerTag.GetName().ToLower(), playerTag);
        }
    }

    private static PlayerTag LoadTagFile(FileInfo file)
    {
        if (!file.Name.Contains(".json")) return null;
        string name = file.Name.Replace(".json", "");

        string json = JsonIO.ReadFile(file);
        PlayerTag playerTag = json.FromJson<PlayerTag>();
        playerTag.SetName(name);
        return playerTag;
    }

    internal static PlayerTag SavePlayerTag(string name)
    {
        if (name.IsNullOrWhiteSpace()) return null;
        
        PlayerTag existing = GetPlayerTagByName(name);
        if (existing != null)
        {
            Plugin.LogGlobal.LogWarning($"Could not save player tag '{name}': a tag with name '{existing.GetName()}' (case insensitive) already exists!");
            return existing;
        }

        PlayerTag tag = new PlayerTag();
        tag.SetName(name);

        string path = Path.Combine(PlayerTagsDirectory.FullName, name + ".json");
        string json = tag.ToJson();
        JsonIO.WriteFile(path, json);
        PlayerTags.Add(name.ToLower(), tag);
        Plugin.LogGlobal.LogInfo($"Saved new player tag '{name}'");
        return tag;
    }

    internal static PlayerTag GetPlayerTagByName(string name)
    {
        if (PlayerTags.ContainsKey(name.ToLower())) return PlayerTags[name.ToLower()];

        return null;
    }
}