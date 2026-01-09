namespace TourneyMod.SetTracking;

internal struct PlayerCharacter(Character character, CharacterVariant variant)
{
    internal static PlayerCharacter EMPTY = new PlayerCharacter(Character.NONE, CharacterVariant.DEFAULT);
    
    internal Character character = character;
    internal CharacterVariant variant = variant;
    
    internal bool IsEmpty => character == Character.NONE && variant == CharacterVariant.DEFAULT;

    public override string ToString()
    {
        return $"{character.ToString()}/{variant.ToString()}";
    }
}