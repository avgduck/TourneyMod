namespace TourneyMod.UI.PlayerTags;

public class Charset
{
    internal char[] lowercase;
    internal char[] uppercase;
    private bool spacing;

    internal int length;

    internal Charset(char[] set, bool spacing = false) : this(set, set, spacing) { }
    
    internal Charset(char[] lowercase, char[] uppercase, bool spacing = false) {
        this.lowercase = lowercase;
        this.uppercase = uppercase;
        this.spacing = spacing;

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
        
        string text = "";
        for (int i = 0; i < length; i++)
        {
            if (spacing && i > 0) text += " ";
            text += set[i];
        }

        return text;
    }
}