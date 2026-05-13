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

    private Image[] bgRows = new Image[ROWS];
    private List<LLClickable> allControls;

    private RectTransform pnBrowse;
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
    
    private const int KEYPAD_COLS = 3;
    private const int KEYPAD_ROWS = 4;

    private const float SPACING = 2f;
    private const int BORDER = 1;
    private const int KEYPAD_FONT_SIZE = 10;
    private static readonly Vector2 KEYPAD_BUTTON_SCALE = new Vector2((BUTTON_SCALE.x - SPACING * (KEYPAD_COLS - 1)) / KEYPAD_COLS, BUTTON_SCALE.y);
    
    private RectTransform pnCreate;
    private RectTransform pnTagEdit;
    private RectTransform pnCreateTagBorder;
    private TextMeshProUGUI lbCreateTag;
    private LLButton btBackspace;
    private LLButton btShift;
    private LLButton btNumbers;
    private LLButton btEnter;
    private RectTransform pnKeypad;
    private List<CharsetButton> btCharsets;
    private List<CharsetButton> btCharsetsAlpha;
    private List<CharsetButton> btCharsetsNumbers;
    
    private static readonly Charset[] charsetAlpha =
    [
        new Charset(['a', 'b', 'c'], ['A', 'B', 'C']),
        new Charset(['d', 'e', 'f'], ['D', 'E', 'F']),
        new Charset(['g', 'h', 'i'], ['G', 'H', 'I']),
        new Charset(['j', 'k', 'l'], ['J', 'K', 'L']),
        new Charset(['m', 'n', 'o'], ['M', 'N', 'O']),
        new Charset(['p', 'q', 'r', 's'], ['P', 'Q', 'R', 'S']),
        new Charset(['t', 'u', 'v'], ['T', 'U', 'V']),
        new Charset(['w', 'x', 'y', 'z'], ['W', 'X', 'Y', 'Z']),
    ];

    private static readonly Charset[] charsetNumbers =
    [
        new Charset(['1', '2', '3']),
        new Charset(['4', '5', '6']),
        new Charset(['7', '8', '9', '0']),
        new Charset(['\'', ',', '.', ';'], true),
        new Charset(['_', '-', '~'], true),
        new Charset(['!', '$', '@'], true),
        new Charset(['#', '&', '%'], true),
        new Charset(['(', ')', '[', ']'], true),
        new Charset(['+', '=', '^'], true)
    ];

    private const int TAG_MAX_LENGTH = 12;
    private const float TAG_REPEAT_TIME = 0.5f;
    private string tag;
    private bool upper;
    private bool numbers;

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
        for (int rowIndex = 0; rowIndex < ROWS; rowIndex++)
        {
            Image img = null;
            UIUtils.CreateImage(ref img, barSprite, "row" + rowIndex, rectTransform, new Vector2(Mathf.Round(BarStartPos.x + BarOffset.x*(rowIndex+1)) + ELEMENT_OFFSET*playerIndex, Mathf.Round(BarStartPos.y + BarOffset.y*(rowIndex+1))), BAR_SCALE);
            img.type = Image.Type.Sliced;
            bgRows[rowIndex] = img;
        }
        InitBrowsePanel();
        InitCreatePanel();
        pnCreate.gameObject.SetActive(false);
    }

    private void InitBrowsePanel()
    {
        UIUtils.CreatePanel(ref pnBrowse, "pnBrowse", rectTransform, Vector2.zero, Vector2.one, Color.clear);
        
        UIUtils.CreateButton(ref btNewTag, "btNewTag", pnBrowse, bgRows[ROWS - 1].rectTransform.localPosition, BUTTON_SCALE, Color.clear);
        btNewTag.textMesh.color = COLOR_CONTROL;
        btNewTag.colDefault = COLOR_CONTROL;
        btNewTag.colHover = COLOR_HOVER;
        btNewTag.textMesh.fontSize = CONTROL_FONT_SIZE;
        btNewTag.SetText("+ new tag");
        btNewTag.ignoreMouseHover = ignoreMouse;
        btNewTag.soundHover = true;
        btNewTag.onClick = playerNr => OpenCreate();
        allControls.Add(btNewTag);
        
        UIUtils.CreatePanel(ref pnPages, "pnPages", pnBrowse, bgRows[ROWS - 2].rectTransform.localPosition, BUTTON_SCALE, Color.clear);
        
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
            Vector2 pos = bgRows[i].rectTransform.localPosition;
            UIUtils.CreateButton(ref btSelectTag, "btSelectTag" + i, pnTagList, pos, BUTTON_SCALE, Color.clear);
            btSelectTag.colHover = COLOR_HOVER;
            btSelectTag.textMesh.fontSize = TAG_FONT_SIZE;
            btSelectTag.ignoreMouseHover = ignoreMouse;
            btSelectTag.soundHover = true;
            btSelectTags[i] = btSelectTag;
            allControls.Add(btSelectTag);
        }
    }

    private static readonly float createTagWidth = BUTTON_SCALE.x * 7f / 8f - SPACING / 2f;
    private static readonly float backspaceWidth = BUTTON_SCALE.x - createTagWidth - SPACING / 2f;
    private void InitCreatePanel()
    {
        UIUtils.CreatePanel(ref pnCreate, "pnCreate", rectTransform, Vector2.zero, Vector2.one, Color.clear);
        
        UIUtils.CreatePanel(ref pnTagEdit, "pnTagEdit", pnCreate, bgRows[ROWS - 1 - KEYPAD_ROWS].rectTransform.localPosition, BUTTON_SCALE, Color.clear);
        
        UIUtils.CreateBorderPanel(ref pnCreateTagBorder, "pnCreateTagBorder", pnTagEdit, new Vector2(-BUTTON_SCALE.x / 2f + createTagWidth / 2f, 0f), new Vector2(createTagWidth, BUTTON_SCALE.y), Color.clear, Color.yellow, BORDER);
        UIUtils.CreateText(ref lbCreateTag, "lbCreateTag", pnTagEdit, new Vector2(-BUTTON_SCALE.x / 2f + createTagWidth / 2f, 0f), new Vector2(createTagWidth, BUTTON_SCALE.y));
        lbCreateTag.fontSize = TAG_FONT_SIZE;
        
        UIUtils.CreateButton(ref btBackspace, "btBackspace", pnTagEdit, new Vector2(BUTTON_SCALE.x / 2f - backspaceWidth / 2f, 0f), new Vector2(backspaceWidth, BUTTON_SCALE.y), Color.yellow);
        btBackspace.textMesh.color = COLOR_TAG_CUSTOM;
        btBackspace.colDefault = COLOR_TAG_CUSTOM;
        btBackspace.colHover = COLOR_HOVER;
        btBackspace.textMesh.fontSize = KEYPAD_FONT_SIZE;
        btBackspace.SetText("<");
        btBackspace.onClick = OnClickBackspace;
        allControls.Add(btBackspace);
        
        UIUtils.CreatePanel(ref pnKeypad, "pnKeypad", pnCreate, Vector2.zero, Vector2.one, Color.clear);
        
        UIUtils.CreateButton(ref btShift, "btShift", pnKeypad, GetKeypadPosition(3, 0), KEYPAD_BUTTON_SCALE, Color.yellow);
        btShift.textMesh.color = COLOR_TAG_CUSTOM;
        btShift.colDefault = COLOR_TAG_CUSTOM;
        btShift.colHover = COLOR_HOVER;
        btShift.textMesh.fontSize = KEYPAD_FONT_SIZE;
        btShift.SetText("case");
        btShift.onClick = OnClickShift;
        allControls.Add(btShift);
        
        UIUtils.CreateButton(ref btNumbers, "btNumbers", pnKeypad, GetKeypadPosition(3, 1), KEYPAD_BUTTON_SCALE, Color.yellow);
        btNumbers.textMesh.color = COLOR_TAG_CUSTOM;
        btNumbers.colDefault = COLOR_TAG_CUSTOM;
        btNumbers.colHover = COLOR_HOVER;
        btNumbers.textMesh.fontSize = KEYPAD_FONT_SIZE;
        btNumbers.SetText("nums");
        btNumbers.onClick = OnClickNumbers;
        allControls.Add(btNumbers);
        
        UIUtils.CreateButton(ref btEnter, "btEnter", pnKeypad, GetKeypadPosition(3, 2), KEYPAD_BUTTON_SCALE, Color.yellow);
        btEnter.textMesh.color = COLOR_TAG_CUSTOM;
        btEnter.colDefault = COLOR_TAG_CUSTOM;
        btEnter.colHover = COLOR_HOVER;
        btEnter.textMesh.fontSize = KEYPAD_FONT_SIZE;
        btEnter.SetText("done");
        btEnter.onClick = OnClickEnter;
        allControls.Add(btEnter);

        btCharsets = new List<CharsetButton>();
        btCharsetsAlpha = new List<CharsetButton>();
        for (int i = 0; i < charsetAlpha.Length; i++)
        {
            CharsetButton btCharset = null;
            int col = (i + 0) % KEYPAD_COLS;
            int row = (i + 0) / KEYPAD_COLS;
            UIUtils.CreateCharsetButton(ref btCharset, "btCharsetAlpha" + i, pnKeypad, GetKeypadPosition(row, col), KEYPAD_BUTTON_SCALE, Color.gray);
            btCharset.textMesh.color = COLOR_TAG_CUSTOM;
            btCharset.colDefault = COLOR_TAG_CUSTOM;
            btCharset.colHover = COLOR_HOVER;
            btCharset.textMesh.fontSize = KEYPAD_FONT_SIZE;
            btCharset.SetCharset(charsetAlpha[i]);
            int btIndex = i;
            btCharset.onClick = playerNr => OnClickCharset(playerNr, btCharset, btIndex);
            btCharsets.Add(btCharset);
            btCharsetsAlpha.Add(btCharset);
        }
        
        btCharsetsNumbers = new List<CharsetButton>();
        for (int i = 0; i < charsetNumbers.Length; i++)
        {
            CharsetButton btCharset = null;
            int col = (i + 0) % KEYPAD_COLS;
            int row = (i + 0) / KEYPAD_COLS;
            UIUtils.CreateCharsetButton(ref btCharset, "btCharsetsNumbers" + i, pnKeypad, GetKeypadPosition(row, col), KEYPAD_BUTTON_SCALE, Color.gray);
            btCharset.textMesh.color = COLOR_TAG_CUSTOM;
            btCharset.colDefault = COLOR_TAG_CUSTOM;
            btCharset.colHover = COLOR_HOVER;
            btCharset.textMesh.fontSize = KEYPAD_FONT_SIZE;
            btCharset.SetCharset(charsetNumbers[i]);
            int btIndex = i;
            btCharset.onClick = playerNr => OnClickCharset(playerNr, btCharset, btIndex);
            btCharsets.Add(btCharset);
            btCharsetsNumbers.Add(btCharset);
        }
    }

    private Vector2 GetKeypadPosition(int row, int col)
    {
        Vector2 rowPos = bgRows[ROWS - KEYPAD_ROWS + row].rectTransform.localPosition;
        Vector2 left = rowPos + new Vector2(-BUTTON_SCALE.x / 2f + KEYPAD_BUTTON_SCALE.x / 2f, 0f);
        return left + new Vector2((KEYPAD_BUTTON_SCALE.x + SPACING) * col, 0f);
    }

    internal void OpenBrowse()
    {
        pnBrowse.gameObject.SetActive(true);
        pnCreate.gameObject.SetActive(false);

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

    private void OpenCreate()
    {
        pnBrowse.gameObject.SetActive(false);
        pnCreate.gameObject.SetActive(true);
        
        tag = "";
        TextHandler.SetText(lbCreateTag, tag);
        upper = false;
        numbers = false;
        btCharsets.ForEach(btCharset => btCharset.SetUpper(upper));
        btCharsetsAlpha.ForEach(btCharset => btCharset.gameObject.SetActive(true));
        btCharsetsNumbers.ForEach(btCharset => btCharset.gameObject.SetActive(false));
        
        gameObject.SetActive(true);
    }

    internal void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnClickBackspace(int playerNr)
    {
        if (tag.Length < 1) return;
        tag = tag.Remove(tag.Length - 1);
        UIUtils.SetTextAutoSize(lbCreateTag, tag, TAG_FONT_SIZE, new Vector2(createTagWidth - BORDER*2, BUTTON_SCALE.y));
        activeBtIndex = -1;
    }

    private void OnClickShift(int playerNr)
    {
        upper = !upper;
        btCharsets.ForEach(btCharset => btCharset.SetUpper(upper));
        btShift.SetText(upper ? "CASE" : "case");
        activeBtIndex = -1;
    }

    private void OnClickNumbers(int playerNr)
    {
        numbers = !numbers;
        btCharsetsAlpha.ForEach(btCharset => btCharset.gameObject.SetActive(!numbers));
        btCharsetsNumbers.ForEach(btCharset => btCharset.gameObject.SetActive(numbers));
        btNumbers.SetText(numbers ? "alpha" : "nums");
        activeBtIndex = -1;
    }

    private void OnClickEnter(int playerNr)
    {
        PlayerTag createdTag = PlayerTagIO.SavePlayerTag(tag);

        if (createdTag == null)
        {
            OpenBrowse();
        }
        else
        {
            OnClickSelectTag(playerNr, createdTag);
        }
    }

    private int activeBtIndex = -1;
    private int count = 0;
    private float repeatTimer = 0f;
    private void OnClickCharset(int playerNr, CharsetButton btCharset, int btIndex)
    {
        if (btIndex != activeBtIndex)
        {
            activeBtIndex = btIndex;
            count = 0;

            if (tag.Length >= TAG_MAX_LENGTH) return;
        }
        else
        {
            count++;
            if (tag.Length >= 1) tag = tag.Remove(tag.Length - 1);
        }

        tag += btCharset.GetChar(count);
        UIUtils.SetTextAutoSize(lbCreateTag, tag, TAG_FONT_SIZE, new Vector2(createTagWidth - BORDER*2, BUTTON_SCALE.y));
        repeatTimer = 0f;
    }

    private void Update()
    {
        if (activeBtIndex == -1) return;

        if (repeatTimer >= TAG_REPEAT_TIME)
        {
            repeatTimer = 0f;
            activeBtIndex = -1;
        }
        else
        {
            repeatTimer += Time.deltaTime;
        }
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

        if (pnBrowse.gameObject.activeSelf)
        {
            if ((curFocus == btPageBack || curFocus == btPageForward) && vert)
            {
                cursor.SetFocus(move.y > 0 ? btSelectTags[btSelectTags.Length - 1] : btNewTag);
                return true;
            }
            if (curFocus == btPageBack)
            {
                cursor.SetFocus(btPageForward);
                return true;
            }
            if (curFocus == btPageForward)
            {
                cursor.SetFocus(btPageBack);
                return true;
            }
        }
        else if (pnCreate.gameObject.activeSelf)
        {
            
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