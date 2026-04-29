using System.Linq;

namespace TourneyMod.UI.Lobby.PlayerTags;

public class Charset
{
    internal char[] lowercase;
    internal char[] uppercase;
    
    internal int length;
    
    internal Charset(char[] lowercase, char[] uppercase) {
        this.lowercase = lowercase;
        this.uppercase = uppercase;

        length = lowercase.Length != uppercase.Length ? 0 : lowercase.Length;
    }

    internal char GetChar(int index, bool upper)
    {
        if (index > length) return ' ';
        return upper ? uppercase[index] : lowercase[index];
    }

    internal string GetText(bool upper)
    {
        char[] set = upper ? uppercase : lowercase;
        return set.Aggregate("", (text, c) => text + c);
    }
}