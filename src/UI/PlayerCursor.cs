using LLBML.Players;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;
using UnityEngine;

namespace TourneyMod.UI;

internal class PlayerCursor
{
    internal Texture2D texRedActive;
    internal Texture2D texBlueActive;
    internal Texture2D texYellowActive;
    internal Texture2D texGreenActive;
    internal Texture2D texNoneActive;
    
    internal Texture2D texRedInactive;
    internal Texture2D texBlueInactive;
    internal Texture2D texYellowInactive;
    internal Texture2D texGreenInactive;
    internal Texture2D texNoneInactive;
    
    internal Sprite spriteRedActive;
    internal Sprite spriteBlueActive;
    internal Sprite spriteYellowActive;
    internal Sprite spriteGreenActive;
    internal Sprite spriteNoneActive;
    
    internal Sprite spriteRedInactive;
    internal Sprite spriteBlueInactive;
    internal Sprite spriteYellowInactive;
    internal Sprite spriteGreenInactive;
    internal Sprite spriteNoneInactive;

    internal Texture2D GetTexture(Team team, bool active)
    {
        return Ruleset.ConvertTeam(team) switch
        {
            PlayerTeam.RED => active ? texRedActive : texRedInactive,
            PlayerTeam.BLUE => active ? texBlueActive : texBlueInactive,
            PlayerTeam.YELLOW => active ? texYellowActive : texYellowInactive,
            PlayerTeam.GREEN => active ? texGreenActive : texGreenInactive,
            _ => active ? texNoneActive : texNoneInactive
        };
    }
    
    internal Sprite GetSprite(Team team, bool active)
    {
        return Ruleset.ConvertTeam(team) switch
        {
            PlayerTeam.RED => active ? spriteRedActive : spriteRedInactive,
            PlayerTeam.BLUE => active ? spriteBlueActive : spriteBlueInactive,
            PlayerTeam.YELLOW => active ? spriteYellowActive : spriteYellowInactive,
            PlayerTeam.GREEN => active ? spriteGreenActive : spriteGreenInactive,
            _ => active ? spriteNoneActive : spriteNoneInactive
        };
    }
}