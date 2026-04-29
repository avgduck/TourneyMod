using LLGUI;

namespace TourneyMod.UI.Lobby.PlayerTags;

public class CharsetButton : LLButton
{
    private Charset charset;
    private bool upper;

    internal void SetCharset(Charset charset)
    {
        this.charset = charset;
        upper = false;
        
        SetText(charset.GetText(upper));
    }

    internal void SetUpper(bool upper)
    {
        this.upper = upper;
        SetText(charset.GetText(upper));
    }

    internal char GetChar(int count)
    {
        return charset.GetChar(count % charset.length, upper);
    }
}