using LLBML.Players;
using UnityEngine;

namespace TourneyMod.UI;

internal class PlayerCursor
{
    internal Texture2D[,] textures = new Texture2D[5, 2];
    internal Sprite[,] sprites = new Sprite[5, 2];

    internal Texture2D GetTexture(Team team, bool active)
    {
        return textures[(int)team, active ? 0 : 1];
    }
    
    internal Sprite GetSprite(Team team, bool active)
    {
        return sprites[(int)team, active ? 0 : 1];
    }
}