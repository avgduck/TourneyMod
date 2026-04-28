using LLGUI;
using LLHandlers;
using TMPro;
using UnityEngine;

namespace TourneyMod.UI.Lobby;

public class PlayerTagMenu : MonoBehaviour
{
    private RectTransform rectTransform;
    
    private static readonly Vector2 MAIN_SCALE = new Vector2(320f, 320f);
    private static readonly Vector2 MAIN_POSITION = new Vector2(0f, 319.5f - MAIN_SCALE.y / 2f + 13f);

    private const int TAG_FONT_SIZE = 16;

    private static readonly Vector2 NEWTAG_SCALE = new Vector2(320f, 20f);
    private static readonly Vector2 NEWTAG_POSITION = new Vector2(0f, MAIN_SCALE.y / 2f - NEWTAG_SCALE.y / 2f);

    private static readonly Vector2 PAGES_SCALE = new Vector2(320f, 20f);
    private static readonly Vector2 PAGES_POSITION = new Vector2(0f, -MAIN_SCALE.y / 2f + NEWTAG_SCALE.y / 2f);

    private static readonly Vector2 PAGE_NUMBER_SCALE = new Vector2(240f, 20f);
    private static readonly Vector2 PAGE_NUMBER_POSITION = new Vector2(0f, 0f);
    
    private static readonly Vector2 PAGE_BUTTON_SCALE = new Vector2(40f, 20f);
    private static readonly Vector2 PAGE_BUTTON_FORWARD_POSITION = new Vector2(MAIN_SCALE.x / 2f - PAGE_BUTTON_SCALE.x / 2f, 0f);
    private static readonly Vector2 PAGE_BUTTON_BACK_POSITION = new Vector2(-MAIN_SCALE.x / 2f + PAGE_BUTTON_SCALE.x / 2f, 0f);

    private LLButton btNewTag;
    
    private RectTransform pnPages;
    private TextMeshProUGUI lbPageNumber;
    private LLButton btPageBack;
    private LLButton btPageForward;
    
    internal static PlayerTagMenu CreateMenu(Transform parent)
    {
        RectTransform panel = null;
        UIUtils.CreateBorderPanel(ref panel, "Player Tag Menu", parent, MAIN_POSITION, MAIN_SCALE, Color.black, Color.yellow, 2);
        PlayerTagMenu playerTagMenu = panel.gameObject.AddComponent<PlayerTagMenu>();
        playerTagMenu.rectTransform = panel;
        playerTagMenu.Init();
        playerTagMenu.gameObject.SetActive(false);
        return playerTagMenu;
    }

    private void Init()
    {
        UIUtils.CreateButton(ref btNewTag, "btNewTag", rectTransform, NEWTAG_POSITION, NEWTAG_SCALE, Color.yellow);
        btNewTag.textMesh.fontSize = TAG_FONT_SIZE;
        btNewTag.SetText("+ new tag");
        
        UIUtils.CreatePanel(ref pnPages, "pnPages", rectTransform, PAGES_POSITION, PAGES_SCALE, Color.yellow);
        
        UIUtils.CreateText(ref lbPageNumber, "lbPageNumber", pnPages, PAGE_NUMBER_POSITION, PAGE_NUMBER_SCALE);
        lbPageNumber.fontSize = TAG_FONT_SIZE;
        TextHandler.SetText(lbPageNumber, "1/1");
        
        UIUtils.CreateButton(ref btPageForward, "btPageForward", pnPages, PAGE_BUTTON_FORWARD_POSITION, PAGE_BUTTON_SCALE, Color.clear);
        btPageForward.textMesh.fontSize = TAG_FONT_SIZE;
        btPageForward.SetText(">");
        
        UIUtils.CreateButton(ref btPageBack, "btPageBack", pnPages, PAGE_BUTTON_BACK_POSITION, PAGE_BUTTON_SCALE, Color.clear);
        btPageBack.textMesh.fontSize = TAG_FONT_SIZE;
        btPageBack.SetText("<");
    }
}