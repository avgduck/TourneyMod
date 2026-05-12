using System.Collections.Generic;
using System.Linq;
using LLBML.Players;
using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using TourneyMod.PlayerTags;
using UnityEngine;
using UnityEngine.UI;
using Controller = LLHandlers.Controller;
using InputAction = LLHandlers.InputAction;

namespace TourneyMod.UI.PlayerTags;

public class PlayerTagMenuOptions : MonoBehaviour
{
    internal static Vector2 BarStartPos;
    internal static Vector2 BarOffset;

    private RectTransform rectTransform;
    private int playerIndex;
    private Sprite barSprite;
    private bool ignoreMouse;
    private Color colHover;
    private ScreenInput screenInput;
    
    private static readonly Vector2 MAIN_POSITION = new Vector2(-250f + 200f, 0f);
    private static readonly Vector2 BAR_SCALE = new Vector2(140f, 40f);
    private const float ELEMENT_OFFSET = 160f;
    private static readonly Vector2 BUTTON_SCALE = new Vector2(120f, 28f);
    
    private const int TAG_FONT_SIZE = 14;
    private const int CONTROL_FONT_SIZE = 18;

    private static readonly Color COLOR_CONTROL = Color.yellow;
    private static readonly Color COLOR_HOVER = new Color(0.1176f, 0.6706f, 1f);
    private static readonly Color COLOR_TAG_CUSTOM = Color.white;
    private static readonly Color COLOR_TAG_DEFAULT = Color.red;

    private static readonly int ROWS = InputAction.EConfigurables().Count() + 5;

    private List<LLClickable> allControls;

    private RectTransform pnBrowse;
    private Image[] rowsBrowse = new Image[ROWS];
    
    private LLButton btNewTag;
    private RectTransform pnPages;
    private TextMeshProUGUI lbPageNumber;
    private LLButton btPageBack;
    private LLButton btPageForward;

    private RectTransform pnTagList;
    private List<PlayerTag> loadedTags;
    private LLButton[] btSelectTags;
    private static readonly int TAG_LIST_ROWS = ROWS - 2;
    private int maxPages;
    private int currentPage;

    internal static PlayerTagMenuOptions CreateMenu(Transform parent, int playerIndex, ScreenInput screenInput)
    {
        RectTransform panel = null;
        UIUtils.CreatePanel(ref panel, "Player Tag Menu", parent, MAIN_POSITION, Vector2.one, Color.clear);
        PlayerTagMenuOptions playerTagMenuOptions = panel.gameObject.AddComponent<PlayerTagMenuOptions>();
        playerTagMenuOptions.rectTransform = panel;
        playerTagMenuOptions.playerIndex = playerIndex;
        playerTagMenuOptions.barSprite = LLBML.Bundles.Assets.LoadFromBundle<Sprite>(BundleType.MENU_SPRITES, "_spriteOptionsBar", false);
        playerTagMenuOptions.screenInput = screenInput;
        playerTagMenuOptions.Init();
        playerTagMenuOptions.gameObject.SetActive(false);
        return playerTagMenuOptions;
    }

    private void Init()
    {
        allControls = new List<LLClickable>();
        ignoreMouse = playerIndex != 0;
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
        
        UIUtils.CreateButton(ref btNewTag, "btNewTag", pnBrowse, rowsBrowse[ROWS - 1].rectTransform.localPosition, BUTTON_SCALE, Color.clear);
        btNewTag.textMesh.color = COLOR_CONTROL;
        btNewTag.colDefault = COLOR_CONTROL;
        btNewTag.colHover = COLOR_HOVER;
        btNewTag.textMesh.fontSize = CONTROL_FONT_SIZE;
        btNewTag.SetText("+ new tag");
        btNewTag.ignoreMouseHover = ignoreMouse;
        btNewTag.soundHover = true;
        allControls.Add(btNewTag);
        
        UIUtils.CreatePanel(ref pnPages, "pnPages", pnBrowse, rowsBrowse[ROWS - 2].rectTransform.localPosition, BUTTON_SCALE, Color.clear);
        
        UIUtils.CreateText(ref lbPageNumber, "lbPageNumber", pnPages, Vector2.zero, new Vector2(BUTTON_SCALE.x / 2f, BUTTON_SCALE.y));
        lbPageNumber.color = COLOR_CONTROL;
        lbPageNumber.fontSize = CONTROL_FONT_SIZE;
        
        UIUtils.CreateButton(ref btPageBack, "btPageBack", pnPages, new Vector2(-BUTTON_SCALE.x / 2f + BUTTON_SCALE.x / 8f, 0f), new Vector2(BUTTON_SCALE.x / 4f, BUTTON_SCALE.y), Color.clear);
        btPageBack.textMesh.color = COLOR_CONTROL;
        btPageBack.colDefault = COLOR_CONTROL;
        btPageBack.colHover = COLOR_HOVER;
        btPageBack.textMesh.fontSize = CONTROL_FONT_SIZE;
        btPageBack.SetText("<");
        btPageBack.ignoreMouseHover = ignoreMouse;
        btPageBack.soundHover = true;
        btPageBack.onClick = OnClickPageBack;
        allControls.Add(btPageBack);
        
        UIUtils.CreateButton(ref btPageForward, "btPageForward", pnPages, new Vector2(BUTTON_SCALE.x / 2f - BUTTON_SCALE.x / 8f, 0f), new Vector2(BUTTON_SCALE.x / 4f, BUTTON_SCALE.y), Color.clear);
        btPageForward.textMesh.color = COLOR_CONTROL;
        btPageForward.colDefault = COLOR_CONTROL;
        btPageForward.colHover = COLOR_HOVER;
        btPageForward.textMesh.fontSize = CONTROL_FONT_SIZE;
        btPageForward.SetText(">");
        btPageForward.ignoreMouseHover = ignoreMouse;
        btPageForward.soundHover = true;
        btPageForward.onClick = OnClickPageForward;
        allControls.Add(btPageForward);
        
        UIUtils.CreatePanel(ref pnTagList, "pnTagList", pnBrowse, Vector2.zero, Vector2.one, Color.clear);

        btSelectTags = new LLButton[TAG_LIST_ROWS];
        for (int i = 0; i < TAG_LIST_ROWS; i++)
        {
            LLButton btSelectTag = null;
            Vector2 pos = rowsBrowse[i].rectTransform.localPosition;
            UIUtils.CreateButton(ref btSelectTag, "btSelectTag" + i, pnTagList, pos, BUTTON_SCALE, Color.clear);
            btSelectTag.colHover = COLOR_HOVER;
            btSelectTag.textMesh.fontSize = TAG_FONT_SIZE;
            btSelectTag.ignoreMouseHover = ignoreMouse;
            btSelectTag.soundHover = true;
            btSelectTags[i] = btSelectTag;
            allControls.Add(btSelectTag);
        }
    }

    internal void OpenBrowse()
    {
        pnBrowse.gameObject.SetActive(true);

        loadedTags = PlayerTagIO.PlayerTags.Values.OrderBy(pt => pt.GetName()).ToList();
        loadedTags.Insert(0, PlayerTag.DEFAULT);
        maxPages = 1 + (loadedTags.Count - 1) / TAG_LIST_ROWS;
        currentPage = 0;
        TextHandler.SetText(lbPageNumber, $"{currentPage+1}/{maxPages}");
        
        LoadPage();
        
        gameObject.SetActive(true);
    }
    
    private void LoadPage()
    {
        foreach (LLButton btSelectTag in btSelectTags)
        {
            btSelectTag.SetText("");
            btSelectTag.onClick = null;
            btSelectTag.SetActive(false);
            btSelectTag.OnHoverOut(-1);
        }

        for (int displayIndex = 0; displayIndex < TAG_LIST_ROWS; displayIndex++)
        {
            int tagIndex = currentPage * TAG_LIST_ROWS + displayIndex;
            if (tagIndex >= loadedTags.Count) break;

            PlayerTag displayTag = loadedTags[tagIndex];
            LLButton btSelectTag = btSelectTags[displayIndex];
            UIUtils.SetTextAutoSize(btSelectTag, displayTag.IsDefault ? "DEFAULT" : displayTag.GetName(), TAG_FONT_SIZE, BUTTON_SCALE);
            btSelectTag.textMesh.color = displayTag.IsDefault ? COLOR_TAG_DEFAULT : COLOR_TAG_CUSTOM;
            btSelectTag.colDefault = displayTag.IsDefault ? COLOR_TAG_DEFAULT : COLOR_TAG_CUSTOM;
            btSelectTag.onClick = playerNr => OnClickSelectTag(playerNr, displayTag);
            btSelectTag.SetActive(true);
        }
    }

    private void OnClickPageBack(int playerNr)
    {
        if (currentPage > 0) currentPage--;
        LoadPage();
        TextHandler.SetText(lbPageNumber, $"{currentPage+1}/{maxPages}");
    }
    
    private void OnClickPageForward(int playerNr)
    {
        if (currentPage < maxPages - 1) currentPage++;
        LoadPage();
        TextHandler.SetText(lbPageNumber, $"{currentPage+1}/{maxPages}");
    }

    private void OnClickSelectTag(int playerNr, PlayerTag playerTag)
    {
        Plugin.Instance.SelectPlayerTag(Controller.FromNr(playerIndex, false), playerTag);
        Close();
        screenInput.UpdateBarButtons();
    }

    internal void Close()
    {
        gameObject.SetActive(false);
    }

    internal void GetControls(ref List<LLClickable> list, LLClickable curFocus, LLCursor cursor)
    {
        if (!gameObject.activeSelf) return;

        if (pnBrowse.gameObject.activeSelf)
        {
            list.AddRange(btSelectTags);
            list.Add(btPageBack);
            list.Add(btNewTag);
        }
    }

    internal bool DirectMove(Vector2 move, LLClickable curFocus, bool shouldMove)
    {
        if (!shouldMove) return false;
        bool vert = move.y != 0f && Mathf.Abs(move.y) > Mathf.Abs(move.x);
        // GameStatesOptions.inputConfigControllers[playerIndex].cursor
        LLCursor cursor = HGFCCNMEEEF.inputConfigControllers[playerIndex].OBELDJGOOIJ;

        if ((curFocus == btPageBack || curFocus == btPageForward) && vert)
        {
            cursor.SetFocus(move.y > 0 ? btSelectTags[btSelectTags.Length - 1] : btNewTag);
            return true;
        }
        else if (curFocus == btPageBack)
        {
            cursor.SetFocus(btPageForward);
            return true;
        }
        else if (curFocus == btPageForward)
        {
            cursor.SetFocus(btPageBack);
            return true;
        }
        return false;
    }

    internal bool CheckControlFocus(LLClickable curFocus)
    {
        bool match = false;
        
        foreach (LLClickable control in allControls)
        {
            if (curFocus == control) match = true;
        }

        return match;
    }
}