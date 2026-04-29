using System.Collections.Generic;
using System.Linq;
using LLGUI;
using LLHandlers;
using TMPro;
using TourneyMod.PlayerTags;
using UnityEngine;

namespace TourneyMod.UI.Lobby.PlayerTags;

public class PlayerTagMenu : MonoBehaviour
{
    private RectTransform rectTransform;
    
    private static readonly Vector2 MAIN_SCALE = new Vector2(320f, 320f);
    private static readonly Vector2 MAIN_POSITION = new Vector2(0f, 319.5f - MAIN_SCALE.y / 2f + 13f);

    private const int TAG_FONT_SIZE = 18;
    private const int CONTROL_FONT_SIZE = 24;
    private const int BORDER_SIZE = 2;
    private const float PADDING = 6f;
    private const float SPACING = 4f;

    private static readonly Vector2 NEWTAG_SCALE = new Vector2(320f, 30f);
    private static readonly Vector2 NEWTAG_POSITION = new Vector2(0f, MAIN_SCALE.y / 2f - NEWTAG_SCALE.y / 2f);

    private static readonly Vector2 PAGES_SCALE = new Vector2(320f, 30f);
    private static readonly Vector2 PAGES_POSITION = new Vector2(0f, -MAIN_SCALE.y / 2f + NEWTAG_SCALE.y / 2f);

    private static readonly Vector2 PAGE_NUMBER_SCALE = new Vector2(240f, 30f);
    private static readonly Vector2 PAGE_NUMBER_POSITION = new Vector2(0f, 0f);
    
    private static readonly Vector2 PAGE_BUTTON_SCALE = new Vector2(40f, 30f);
    private static readonly Vector2 PAGE_BUTTON_FORWARD_POSITION = new Vector2(MAIN_SCALE.x / 2f - PAGE_BUTTON_SCALE.x / 2f, 0f);
    private static readonly Vector2 PAGE_BUTTON_BACK_POSITION = new Vector2(-MAIN_SCALE.x / 2f + PAGE_BUTTON_SCALE.x / 2f, 0f);
    
    private RectTransform pnBrowse;
    private LLButton btNewTag;
    private RectTransform pnPages;
    private TextMeshProUGUI lbPageNumber;
    private LLButton btPageBack;
    private LLButton btPageForward;

    private RectTransform pnTagList;
    private static readonly Vector2 TAG_LIST_SCALE = new Vector2(320f - PADDING*2f, 260f);
    private static readonly Vector2 TAG_LIST_POSITION = new Vector2(0f, 0f);
    private const int TAG_LIST_ROWS = 8;
    private static readonly Vector2 TAG_LIST_ENTRY_SCALE = new Vector2(TAG_LIST_SCALE.x, (TAG_LIST_SCALE.y - SPACING * (TAG_LIST_ROWS + 1)) / TAG_LIST_ROWS);
    private List<PlayerTag> loadedTags;
    private LLButton[] btSelectTags;
    private int maxPages;
    private int currentPage;

    private static readonly Vector2 CREATETAG_SCALE = new Vector2(280f - PADDING, 40f);
    private static readonly Vector2 CREATETAG_POSITION = new Vector2(-MAIN_SCALE.x / 2f + CREATETAG_SCALE.x / 2f + PADDING, MAIN_SCALE.y / 2f - CREATETAG_SCALE.y / 2f - PADDING);
    
    private static readonly Vector2 BACKSPACE_SCALE = new Vector2(40f - PADDING, 40f);
    private static readonly Vector2 BACKSPACE_POSITION = new Vector2(MAIN_SCALE.x / 2f - BACKSPACE_SCALE.x / 2f - PADDING, MAIN_SCALE.y / 2f - BACKSPACE_SCALE.y / 2f - PADDING);

    private static readonly Vector2 KEYPAD_SCALE = new Vector2(320f - PADDING * 2f, 280f - PADDING*2f - SPACING);
    private static readonly Vector2 KEYPAD_POSITION = new Vector2(0f, -MAIN_SCALE.y / 2f + KEYPAD_SCALE.y / 2f + PADDING);

    private const int KEYPAD_COLS = 3;
    private const int KEYPAD_ROWS = 4;

    private static readonly Vector2 KEYPAD_BUTTON_SCALE = new Vector2((KEYPAD_SCALE.x - SPACING * (KEYPAD_COLS - 1)) / KEYPAD_COLS, (KEYPAD_SCALE.y - SPACING * (KEYPAD_ROWS - 1)) / KEYPAD_ROWS);
    
    private RectTransform pnCreate;
    private TextMeshProUGUI lbCreateTag;
    private LLButton btBackspace;
    private LLButton btShift;
    private LLButton btEnter;
    private RectTransform pnKeypad;
    private List<CharsetButton> btCharsets;
    
    private static readonly Charset[] charsets =
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

    private const int TAG_MAX_LENGTH = 12;
    private const float TAG_REPEAT_TIME = 0.5f;
    private string tag;
    private bool upper;
    
    internal static PlayerTagMenu CreateMenu(Transform parent)
    {
        RectTransform panel = null;
        UIUtils.CreatePanel(ref panel, "Player Tag Menu", parent, MAIN_POSITION, MAIN_SCALE, Color.clear);
        PlayerTagMenu playerTagMenu = panel.gameObject.AddComponent<PlayerTagMenu>();
        playerTagMenu.rectTransform = panel;
        playerTagMenu.Init();
        playerTagMenu.gameObject.SetActive(false);
        return playerTagMenu;
    }

    private void Init()
    {
        InitBrowsePanel();
        InitCreatePanel();
        pnCreate.gameObject.SetActive(false);
    }

    private void InitBrowsePanel()
    {
        UIUtils.CreateBorderPanel(ref pnBrowse, "pnBrowse", rectTransform, new Vector2(0f, 0f), MAIN_SCALE, Color.black, Color.yellow, BORDER_SIZE);
        
        UIUtils.CreateButton(ref btNewTag, "btNewTag", pnBrowse, NEWTAG_POSITION, NEWTAG_SCALE, Color.yellow);
        btNewTag.textMesh.fontSize = CONTROL_FONT_SIZE;
        btNewTag.SetText("+ new tag");
        btNewTag.onClick = (int playerNr) => OpenCreate();
        
        UIUtils.CreatePanel(ref pnPages, "pnPages", pnBrowse, PAGES_POSITION, PAGES_SCALE, Color.yellow);
        
        UIUtils.CreateText(ref lbPageNumber, "lbPageNumber", pnPages, PAGE_NUMBER_POSITION, PAGE_NUMBER_SCALE);
        lbPageNumber.fontSize = CONTROL_FONT_SIZE;
        
        UIUtils.CreateButton(ref btPageForward, "btPageForward", pnPages, PAGE_BUTTON_FORWARD_POSITION, PAGE_BUTTON_SCALE, Color.clear);
        btPageForward.textMesh.fontSize = CONTROL_FONT_SIZE;
        btPageForward.SetText(">");
        btPageForward.onClick = playerNr => OnClickPageForward();
        
        UIUtils.CreateButton(ref btPageBack, "btPageBack", pnPages, PAGE_BUTTON_BACK_POSITION, PAGE_BUTTON_SCALE, Color.clear);
        btPageBack.textMesh.fontSize = CONTROL_FONT_SIZE;
        btPageBack.SetText("<");
        btPageBack.onClick = playerNr => OnClickPageBack();
        
        UIUtils.CreatePanel(ref pnTagList, "pnTagList", pnBrowse, TAG_LIST_POSITION, TAG_LIST_SCALE, Color.clear);
        Vector2 top = new Vector2(0f, TAG_LIST_SCALE.y / 2f - TAG_LIST_ENTRY_SCALE.y / 2f - SPACING);

        btSelectTags = new LLButton[TAG_LIST_ROWS];
        for (int i = 0; i < TAG_LIST_ROWS; i++)
        {
            LLButton btSelectTag = null;
            Vector2 pos = new Vector2(0f, top.y - (TAG_LIST_ENTRY_SCALE.y + SPACING) * i);
            UIUtils.CreateButton(ref btSelectTag, "btSelectTag" + i, pnTagList, pos, TAG_LIST_ENTRY_SCALE, Color.clear);
            btSelectTag.textMesh.fontSize = TAG_FONT_SIZE;
            btSelectTags[i] = btSelectTag;
        }
    }

    private void InitCreatePanel()
    {
        UIUtils.CreateBorderPanel(ref pnCreate, "pnCreate", rectTransform, new Vector2(0f, 0f), MAIN_SCALE, Color.black, Color.yellow, BORDER_SIZE);
        
        UIUtils.CreateText(ref lbCreateTag, "lbCreateTag", pnCreate, CREATETAG_POSITION, CREATETAG_SCALE);
        lbCreateTag.fontSize = TAG_FONT_SIZE;
        
        UIUtils.CreateButton(ref btBackspace, "btBackspace", pnCreate, BACKSPACE_POSITION, BACKSPACE_SCALE, Color.yellow);
        btBackspace.textMesh.fontSize = CONTROL_FONT_SIZE;
        btBackspace.SetText("<");
        btBackspace.onClick = playerNr => OnClickBackspace();
        
        UIUtils.CreatePanel(ref pnKeypad, "pnKeypad", pnCreate, KEYPAD_POSITION, KEYPAD_SCALE, Color.clear);
        
        UIUtils.CreateButton(ref btShift, "btShift", pnKeypad, GetKeypadPosition(0, 0), KEYPAD_BUTTON_SCALE, Color.yellow);
        btShift.textMesh.fontSize = CONTROL_FONT_SIZE;
        btShift.SetText("case");
        btShift.onClick = playerNr => OnClickShift();

        btCharsets = new List<CharsetButton>();
        for (int i = 0; i < charsets.Length; i++)
        {
            CharsetButton btCharset = null;
            int col = (i + 1) % KEYPAD_COLS;
            int row = (i + 1) / KEYPAD_COLS;
            UIUtils.CreateCharsetButton(ref btCharset, "btCharset" + i, pnKeypad, GetKeypadPosition(row, col), KEYPAD_BUTTON_SCALE, Color.gray);
            btCharset.textMesh.fontSize = CONTROL_FONT_SIZE;
            btCharset.SetCharset(charsets[i]);
            int btIndex = i;
            btCharset.onClick = playerNr => OnClickCharset(btCharset, btIndex);
            btCharsets.Add(btCharset);
        }
        
        UIUtils.CreateButton(ref btEnter, "btEnter", pnKeypad, GetKeypadPosition(3, 2), KEYPAD_BUTTON_SCALE, Color.yellow);
        btEnter.textMesh.fontSize = CONTROL_FONT_SIZE;
        btEnter.SetText("done");
        btEnter.onClick = playerNr => OnClickEnter();
    }

    private Vector2 GetKeypadPosition(int row, int col)
    {
        Vector2 topLeft = new Vector2(-KEYPAD_SCALE.x / 2f + KEYPAD_BUTTON_SCALE.x / 2f, KEYPAD_SCALE.y / 2f - KEYPAD_BUTTON_SCALE.y / 2f);
        return new Vector2(topLeft.x + (KEYPAD_BUTTON_SCALE.x + SPACING) * col, topLeft.y - (KEYPAD_BUTTON_SCALE.y + SPACING) * row);
    }

    internal void OpenBrowse()
    {
        pnBrowse.gameObject.SetActive(true);
        pnCreate.gameObject.SetActive(false);

        loadedTags = PlayerTagIO.PlayerTags.Values.OrderBy(pt => pt.GetName()).ToList();
        maxPages = 1 + (loadedTags.Count - 1) / TAG_LIST_ROWS;
        currentPage = 0;
        TextHandler.SetText(lbPageNumber, $"{currentPage+1}/{maxPages}");

        LoadPage();
        
        gameObject.SetActive(true);
    }

    internal void OnClickPageBack()
    {
        if (currentPage > 0) currentPage--;
        LoadPage();
        TextHandler.SetText(lbPageNumber, $"{currentPage+1}/{maxPages}");
    }
    
    internal void OnClickPageForward()
    {
        if (currentPage < maxPages - 1) currentPage++;
        LoadPage();
        TextHandler.SetText(lbPageNumber, $"{currentPage+1}/{maxPages}");
    }

    private void LoadPage()
    {
        foreach (LLButton btSelectTag in btSelectTags)
        {
            btSelectTag.SetText("");
            btSelectTag.onClick = null;
            btSelectTag.enabled = false;
        }

        for (int displayIndex = 0; displayIndex < TAG_LIST_ROWS; displayIndex++)
        {
            int tagIndex = currentPage * TAG_LIST_ROWS + displayIndex;
            if (tagIndex >= loadedTags.Count) break;
            
            PlayerTag displayTag = loadedTags[tagIndex];
            btSelectTags[displayIndex].SetText(displayTag.GetName());
        }
    }

    internal void OpenCreate()
    {
        pnBrowse.gameObject.SetActive(false);
        pnCreate.gameObject.SetActive(true);

        tag = "";
        TextHandler.SetText(lbCreateTag, tag);
        upper = false;
        btCharsets.ForEach(btCharset => btCharset.SetUpper(upper));
        
        gameObject.SetActive(true);
    }

    internal void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnClickBackspace()
    {
        if (tag.Length < 1) return;
        tag = tag.Remove(tag.Length - 1);
        TextHandler.SetText(lbCreateTag, tag);
        activeBtIndex = -1;
    }

    private void OnClickShift()
    {
        upper = !upper;
        btCharsets.ForEach(btCharset => btCharset.SetUpper(upper));
        activeBtIndex = -1;
    }

    private void OnClickEnter()
    {
        PlayerTagIO.SavePlayerTag(tag);
        OpenBrowse();
    }

    private int activeBtIndex = -1;
    private int count = 0;
    private float repeatTimer = 0f;
    private void OnClickCharset(CharsetButton btCharset, int btIndex)
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
        TextHandler.SetText(lbCreateTag, tag);
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
}