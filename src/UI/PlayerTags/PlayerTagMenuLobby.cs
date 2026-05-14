using System.Collections.Generic;
using System.Linq;
using LLBML.Players;
using LLBML.States;
using LLGUI;
using LLHandlers;
using TMPro;
using TourneyMod.PlayerTags;
using UnityEngine;

namespace TourneyMod.UI.PlayerTags;

public class PlayerTagMenuLobby : MonoBehaviour
{
    private RectTransform rectTransform;
    private int playerNr;
    
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

    internal static readonly Color COLOR_TAG_DEFAULT = Color.red;
    internal static readonly Color COLOR_TAG_CUSTOM = Color.white;

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

    internal static PlayerTagMenuLobby CreateMenu(Transform parent, int playerNr)
    {
        RectTransform panel = null;
        UIUtils.CreatePanel(ref panel, "Player Tag Menu", parent, MAIN_POSITION, MAIN_SCALE, Color.clear);
        PlayerTagMenuLobby playerTagMenuLobby = panel.gameObject.AddComponent<PlayerTagMenuLobby>();
        playerTagMenuLobby.rectTransform = panel;
        playerTagMenuLobby.playerNr = playerNr;
        playerTagMenuLobby.Init();
        playerTagMenuLobby.gameObject.SetActive(false);
        return playerTagMenuLobby;
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
        btNewTag.onClick = playerNr =>
        {
            if (playerNr != -1 && this.playerNr != playerNr) return;
            OpenCreate();
        };
        
        UIUtils.CreatePanel(ref pnPages, "pnPages", pnBrowse, PAGES_POSITION, PAGES_SCALE, Color.yellow);
        
        UIUtils.CreateText(ref lbPageNumber, "lbPageNumber", pnPages, PAGE_NUMBER_POSITION, PAGE_NUMBER_SCALE);
        lbPageNumber.fontSize = CONTROL_FONT_SIZE;
        
        UIUtils.CreateButton(ref btPageForward, "btPageForward", pnPages, PAGE_BUTTON_FORWARD_POSITION, PAGE_BUTTON_SCALE, Color.clear);
        btPageForward.textMesh.fontSize = CONTROL_FONT_SIZE;
        btPageForward.SetText(">");
        btPageForward.onClick = OnClickPageForward;
        
        UIUtils.CreateButton(ref btPageBack, "btPageBack", pnPages, PAGE_BUTTON_BACK_POSITION, PAGE_BUTTON_SCALE, Color.clear);
        btPageBack.textMesh.fontSize = CONTROL_FONT_SIZE;
        btPageBack.SetText("<");
        btPageBack.onClick = OnClickPageBack;
        
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
        btBackspace.onClick = OnClickBackspace;
        
        UIUtils.CreatePanel(ref pnKeypad, "pnKeypad", pnCreate, KEYPAD_POSITION, KEYPAD_SCALE, Color.clear);
        
        UIUtils.CreateButton(ref btShift, "btShift", pnKeypad, GetKeypadPosition(3, 0), KEYPAD_BUTTON_SCALE, Color.yellow);
        btShift.textMesh.fontSize = CONTROL_FONT_SIZE;
        btShift.SetText("case");
        btShift.onClick = OnClickShift;
        
        UIUtils.CreateButton(ref btNumbers, "btNumbers", pnKeypad, GetKeypadPosition(3, 1), KEYPAD_BUTTON_SCALE, Color.yellow);
        btNumbers.textMesh.fontSize = CONTROL_FONT_SIZE;
        btNumbers.SetText("nums");
        btNumbers.onClick = OnClickNumbers;

        btCharsets = new List<CharsetButton>();
        btCharsetsAlpha = new List<CharsetButton>();
        for (int i = 0; i < charsetAlpha.Length; i++)
        {
            CharsetButton btCharset = null;
            int col = (i + 0) % KEYPAD_COLS;
            int row = (i + 0) / KEYPAD_COLS;
            UIUtils.CreateCharsetButton(ref btCharset, "btCharsetAlpha" + i, pnKeypad, GetKeypadPosition(row, col), KEYPAD_BUTTON_SCALE, Color.gray);
            btCharset.textMesh.fontSize = CONTROL_FONT_SIZE;
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
            UIUtils.CreateCharsetButton(ref btCharset, "btCharsetNumbers" + i, pnKeypad, GetKeypadPosition(row, col), KEYPAD_BUTTON_SCALE, Color.gray);
            btCharset.textMesh.fontSize = CONTROL_FONT_SIZE;
            btCharset.SetCharset(charsetNumbers[i]);
            int btIndex = i;
            btCharset.onClick = playerNr => OnClickCharset(playerNr, btCharset, btIndex);
            btCharsets.Add(btCharset);
            btCharsetsNumbers.Add(btCharset);
        }
        
        UIUtils.CreateButton(ref btEnter, "btEnter", pnKeypad, GetKeypadPosition(3, 2), KEYPAD_BUTTON_SCALE, Color.yellow);
        btEnter.textMesh.fontSize = CONTROL_FONT_SIZE;
        btEnter.SetText("done");
        btEnter.onClick = OnClickEnter;
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
        loadedTags.Insert(0, PlayerTag.DEFAULT);
        maxPages = 1 + (loadedTags.Count - 1) / TAG_LIST_ROWS;
        currentPage = 0;
        TextHandler.SetText(lbPageNumber, $"{currentPage+1}/{maxPages}");

        LoadPage();
        
        gameObject.SetActive(true);
    }

    private void OnClickPageBack(int playerNr)
    {
        if (playerNr != -1 && this.playerNr != playerNr) return;
        
        if (currentPage > 0) currentPage--;
        LoadPage();
        TextHandler.SetText(lbPageNumber, $"{currentPage+1}/{maxPages}");
    }
    
    private void OnClickPageForward(int playerNr)
    {
        if (playerNr != -1 && this.playerNr != playerNr) return;
        
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
            btSelectTag.SetActive(false);
        }

        for (int displayIndex = 0; displayIndex < TAG_LIST_ROWS; displayIndex++)
        {
            int tagIndex = currentPage * TAG_LIST_ROWS + displayIndex;
            if (tagIndex >= loadedTags.Count) break;
            
            PlayerTag displayTag = loadedTags[tagIndex];
            LLButton btSelectTag = btSelectTags[displayIndex];
            btSelectTag.SetText(displayTag.IsDefault ? $"PLAYER{playerNr+1}" : displayTag.GetName());
            btSelectTag.colDefault = displayTag.IsDefault ? COLOR_TAG_DEFAULT : COLOR_TAG_CUSTOM;
            btSelectTag.textMesh.color = displayTag.IsDefault ? COLOR_TAG_DEFAULT : COLOR_TAG_CUSTOM;
            btSelectTag.onClick = playerNr => OnClickSelectTag(playerNr, displayTag);
            btSelectTag.SetActive(true);
        }
    }

    private void OnClickSelectTag(int playerNr, PlayerTag playerTag)
    {
        if (playerNr != -1 && this.playerNr != playerNr) return;

        Plugin.Instance.SelectPlayerTag(this.playerNr, playerTag);
        Close();
        
        HPNLMFHPHFD gameStatesLobby = GameStates.GetCurrentGameStateObject() as HPNLMFHPHFD;
        if (gameStatesLobby == null) return;
        
        gameStatesLobby.BDMIDGAHNLA(Player.GetPlayer(this.playerNr));
    }

    private void OpenCreate()
    {
        pnBrowse.gameObject.SetActive(false);
        pnCreate.gameObject.SetActive(true);

        tag = "";
        TextHandler.SetText(lbCreateTag, tag);
        upper = false;
        numbers = false;
        btShift.SetText("case");
        btNumbers.SetText("nums");
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
        if (playerNr != -1 && this.playerNr != playerNr) return;
        
        if (tag.Length < 1) return;
        tag = tag.Remove(tag.Length - 1);
        TextHandler.SetText(lbCreateTag, tag);
        activeBtIndex = -1;
    }

    private void OnClickShift(int playerNr)
    {
        if (playerNr != -1 && this.playerNr != playerNr) return;
        
        upper = !upper;
        btCharsets.ForEach(btCharset => btCharset.SetUpper(upper));
        btShift.SetText(upper ? "CASE" : "case");
        activeBtIndex = -1;
    }
    
    private void OnClickNumbers(int playerNr)
    {
        if (playerNr != -1 && this.playerNr != playerNr) return;
        
        numbers = !numbers;
        btCharsetsAlpha.ForEach(btCharset => btCharset.gameObject.SetActive(!numbers));
        btCharsetsNumbers.ForEach(btCharset => btCharset.gameObject.SetActive(numbers));
        btNumbers.SetText(numbers ? "alpha" : "nums");
        activeBtIndex = -1;
    }

    private void OnClickEnter(int playerNr)
    {
        if (playerNr != -1 && this.playerNr != playerNr) return;

        PlayerTag createdTag = PlayerTagIO.CreatePlayerTag(tag);

        if (createdTag == null)
        {
            OpenBrowse();
        }
        else
        {
            OnClickSelectTag(this.playerNr, createdTag);
            Close();
        }
    }

    private int activeBtIndex = -1;
    private int count = 0;
    private float repeatTimer = 0f;
    private void OnClickCharset(int playerNr, CharsetButton btCharset, int btIndex)
    {
        if (playerNr != -1 && this.playerNr != playerNr) return;
        
        if (btIndex != activeBtIndex)
        {
            if (tag.Length >= TAG_MAX_LENGTH) return;
            
            activeBtIndex = btIndex;
            count = 0;
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