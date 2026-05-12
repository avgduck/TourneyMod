using System.Linq;
using LLGUI;
using LLHandlers;
using Rewired;
using UnityEngine;
using UnityEngine.UI;
using InputAction = LLHandlers.InputAction;

namespace TourneyMod.UI.PlayerTags;

public class PlayerTagMenuOptions : MonoBehaviour
{
    internal static Vector2 BarStartPos;
    internal static Vector2 BarOffset;

    private RectTransform rectTransform;
    private int playerIndex;
    private Sprite barSprite;
    
    private static readonly Vector2 MAIN_POSITION = new Vector2(-250f + 200f, 0f);
    private static readonly Vector2 BAR_SCALE = new Vector2(140f, 40f);
    private const float ELEMENT_OFFSET = 160f;
    
    private const int TAG_FONT_SIZE = 14;
    private const int CONTROL_FONT_SIZE = 18;

    private static readonly int ROWS = InputAction.EConfigurables().Count() + 5;

    private RectTransform pnBrowse;
    private Image[] rowsBrowse = new Image[ROWS];

    internal static PlayerTagMenuOptions CreateMenu(Transform parent, int playerIndex, Sprite barSprite)
    {
        RectTransform panel = null;
        UIUtils.CreatePanel(ref panel, "Player Tag Menu", parent, MAIN_POSITION, Vector2.one, Color.clear);
        PlayerTagMenuOptions playerTagMenuOptions = panel.gameObject.AddComponent<PlayerTagMenuOptions>();
        playerTagMenuOptions.rectTransform = panel;
        playerTagMenuOptions.playerIndex = playerIndex;
        playerTagMenuOptions.barSprite = LLBML.Bundles.Assets.LoadFromBundle<Sprite>(BundleType.MENU_SPRITES, "_spriteOptionsBar", false);
        playerTagMenuOptions.Init();
        playerTagMenuOptions.gameObject.SetActive(false);
        return playerTagMenuOptions;
    }

    private void Init()
    {
        InitBrowsePanel();
    }

    private void InitBrowsePanel()
    {
        UIUtils.CreatePanel(ref pnBrowse, "pnBrowse", rectTransform, Vector2.zero, Vector2.one, Color.clear);
        
        for (int rowIndex = 0; rowIndex < ROWS; rowIndex++)
        {
            Image img = null;
            UIUtils.CreateImage(ref img, barSprite, "row" + rowIndex, pnBrowse, new Vector2(Mathf.Round(BarStartPos.x + BarOffset.x*(rowIndex+1)) + ELEMENT_OFFSET*playerIndex, Mathf.Round(BarStartPos.y + BarOffset.y*(rowIndex+1))), BAR_SCALE);
            img.type = Image.Type.Sliced;
            rowsBrowse[rowIndex] = img;
        }
    }

    internal void OpenBrowse()
    {
        pnBrowse.gameObject.SetActive(true);
        
        gameObject.SetActive(true);
    }

    internal void Close()
    {
        gameObject.SetActive(false);
    }
}