namespace TourneyMod.PlayerTags;

public class Bind
{
    public InputDevice inputDevice;
    public int control;
    public int controlId;
    public int keyCode;

    public enum InputDevice
    {
        KEYBOARD,
        CONTROLLER
    }
}