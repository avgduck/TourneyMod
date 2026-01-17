using LLBML.Players;

namespace TourneyMod.SetTracking;

internal struct PlayerCharacter(Character character, CharacterVariant variant, Team team)
{
    internal static PlayerCharacter EMPTY = new PlayerCharacter(Character.NONE, CharacterVariant.DEFAULT, Team.NONE);
    
    internal Character character = character;
    internal CharacterVariant variant = variant;
    internal Team team = team;
    
    internal bool IsEmpty => team == Team.NONE;

    public override string ToString()
    {
        return $"{character.ToString()}/{variant.ToString()}/{team}";
    }
}