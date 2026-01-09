using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TourneyMod;

internal static class Assets
{
    private static string pathAssets;
    private static Dictionary<string, FileInfo> assetFiles;

    internal static void Init()
    {
        pathAssets = Path.Combine(Path.GetDirectoryName(Plugin.Instance.Info.Location), "assets").Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        DirectoryInfo assetsDirectory = new DirectoryInfo(pathAssets);
        
        assetFiles = assetsDirectory.GetFiles("*", SearchOption.AllDirectories).ToDictionary(file => file.FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace(pathAssets + Path.AltDirectorySeparatorChar, ""));
    }

    private static FileInfo GetAssetFile(string asset)
    {
        assetFiles.TryGetValue(asset, out FileInfo file);
        return file;
    }
    
    private static void CopyStream(Stream input, Stream output)
    {
        byte[] buffer = new byte[8 * 1024];
        int len;
        while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, len);
        }
    }
    
    internal static Texture2D LoadTexture(string asset)
    {
        return LoadTexture(GetAssetFile(asset));
    }
    
    private static Texture2D LoadTexture(FileInfo file)
    {
        using FileStream fileStream = file.OpenRead();
        using MemoryStream memoryStream = new MemoryStream();
        
        CopyStream(fileStream, memoryStream);
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(memoryStream.ToArray());
        return tex;
    }
}