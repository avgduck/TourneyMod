using UnityEngine;

namespace TourneyMod.UI.Lobby;

public class PlayerTagMenu : MonoBehaviour
{
    private RectTransform rectTransform;
    
    private static readonly Vector2 SCALE = new Vector2(320f, 80f);
    private static readonly Vector2 POSITION = new Vector2(0f, 319.5f - SCALE.y / 2f + 13f);
    
    internal static PlayerTagMenu CreateMenu(Transform parent)
    {
        RectTransform panel = null;
        UIUtils.CreateBorderPanel(ref panel, "Player Tag Menu", parent, POSITION, SCALE, Color.black, Color.yellow, 2);
        PlayerTagMenu playerTagMenu = panel.gameObject.AddComponent<PlayerTagMenu>();
        playerTagMenu.rectTransform = panel;
        
        playerTagMenu.gameObject.SetActive(false);
        return playerTagMenu;
    }
}