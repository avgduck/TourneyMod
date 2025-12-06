using System.Collections.Generic;
using LLBML.Players;
using LLBML.States;
using LLBML.Utils;
using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using TourneyMod.Rulesets;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI;

internal class ScreenStageStrike
{
    internal static ScreenStageStrike Instance { get; private set; }
    internal static bool IsOpen => Instance != null;
    private ScreenPlayersStage screenStage;
    private List<StageContainer> stageContainersNeutral;
    private List<StageContainer> stageContainersCounterpick;
    private List<StageContainer> stageContainers;
    private TMP_Text titleText;

    private static readonly Vector2 BG_SCALE = new Vector2(1f, 2f);
    private static readonly Vector2 BG_POSITION = new Vector2(0f, -20f);

    private static readonly Vector2 TITLE_SCALE = new Vector2(1f, 0.6f);
    private static readonly Vector2 TITLE_POSITION = new Vector2(0f, 328f);
    private const int TITLE_FONT_SIZE = 36;

    private static readonly Vector2 BACK_SCALE = new Vector2(0.6f, 0.5f);
    private static readonly Vector2 BACK_POSITION = new Vector2(-570f, -336f);
    private const int BACK_FONT_SIZE = 22;

    private static readonly float STAGE_SCALE_FACTOR = 0.62f;
    private static readonly float STAGE_SCALE_FACTOR_MINI = 0.48f;
    private static readonly float STAGE_SCALE_FACTOR_TINY = 0.41f;
    private static readonly Vector2 STAGE_SIZE = new Vector2(500f, 250f) * STAGE_SCALE_FACTOR;
    private static readonly Vector2 STAGE_SIZE_MINI = new Vector2(500f, 250f) * STAGE_SCALE_FACTOR_MINI;
    private static readonly Vector2 STAGE_SIZE_TINY = new Vector2(500f, 250f) * STAGE_SCALE_FACTOR_TINY;
    private static readonly Vector2 STAGES_POSITION = new Vector2(0f, 0f);
    private static readonly Vector2 STAGES_SPACING = new Vector2(6f, 6f);
    private const float STAGE_CATEGORY_SPACING = 20f;
    private const float STAGE_CATEGORY_SPACING_MINI = 10f;

    private static readonly Vector2 SETCOUNT_POSITION = new Vector2(0f, 270f);
    private const int SETCOUNT_FONT_SIZE = 32;
    private static readonly Vector2 BANSREMAINING_POSITION = new Vector2(0f, -276f);
    private const int BANSREMAINING_FONT_SIZE = 18;
    private static readonly Vector2 BANSTATUS_POSITION = new Vector2(0f, -310f);
    private const int BANSTATUS_FONT_SIZE = 42;
    
    private static readonly Vector2 FREEPICK_POSITION = new Vector2(506f, -336f);
    private static readonly Vector2 FREEPICK_SCALE = new Vector2(1.3f, 0.5f);
    private const int FREEPICK_FONT_SIZE = 18;
    
    private static readonly Vector2 RANDOM_POSITION = new Vector2(-378f, -336f);
    private static readonly Vector2 RANDOM_SCALE = new Vector2(1.42f, 0.5f);
    private const int RANDOM_FONT_SIZE = 18;
    
    private static readonly Color[] COLOR_PLAYER =
    [
        new Color(255/255f, 64/255f, 22/255f),
        new Color(13/255f, 136/255f, 255/255f),
        new Color(255/255f, 255/255f, 61/255f),
        new Color(90/255f, 244/255f, 90/255f)
    ];

    private static readonly Color COLOR_CURSOR_ACTIVE = Color.white;
    private static readonly Color COLOR_CURSOR_INACTIVE = Color.white * 0.6f;

    private TMP_Text lbSetCount;
    private TMP_Text lbBansRemaining;
    private TMP_Text lbBanStatus;

    private LLButton btFreePick;
    private bool[] freePickVotes = [false, false];
    
    private LLButton btRandom;
    private bool[] randomVotes = [false, false];

    internal static void Open()
    {
        Plugin.LogGlobal.LogInfo("Opening stage strike screen");
        Instance = new ScreenStageStrike();
        UIScreen.blockGlobalInput = true;
    }

    internal static void Close()
    {
        Plugin.LogGlobal.LogInfo("Closing stage strike screen");
        Instance = null;
        UIScreen.blockGlobalInput = false;
        SetTracker.Instance.ResetBans();
    }

    internal void OnOpen(ScreenPlayersStage screenStage)
    {
        this.screenStage = screenStage;
        screenStage.UpdateText();
        this.screenStage.msgMenu = Msg.NONE;

        this.screenStage = screenStage;

        RectTransform bar_top = screenStage.transform.Find("bar_top").GetComponent<RectTransform>();
        bar_top.anchoredPosition = TITLE_POSITION;
        bar_top.localScale = TITLE_SCALE;

        RectTransform lbTitle = bar_top.Find("lbTitle").GetComponent<RectTransform>();
        lbTitle.localScale = new Vector2(1f / TITLE_SCALE.x, 1f / TITLE_SCALE.y);
        titleText = lbTitle.GetComponent<TMP_Text>();
        titleText.fontSize = TITLE_FONT_SIZE;

        RectTransform bar_mid = screenStage.transform.Find("bar_mid").GetComponent<RectTransform>();
        bar_mid.anchoredPosition = BG_POSITION;
        bar_mid.localScale = BG_SCALE;

        screenStage.btLeft.visible = false;
        screenStage.btRight.visible = false;
        screenStage.btBack.onClick = (playerNumber) => { GameStates.Send(Msg.BACK, playerNumber, -1); };

        RectTransform btBack = screenStage.btBack.GetComponent<RectTransform>();
        btBack.anchoredPosition = BACK_POSITION;
        btBack.localScale = BACK_SCALE;
        RectTransform lbBack = btBack.Find("Text").GetComponent<RectTransform>();
        lbBack.localScale = new Vector2(1f / BACK_SCALE.x, 1f / BACK_SCALE.y);
        TMP_Text backText = lbBack.GetComponent<TMP_Text>();
        backText.fontSize = BACK_FONT_SIZE;
        
        lbSetCount = ScreenStageStrike.Instance.CreateNewText("lbSetCount", screenStage.transform);
        lbSetCount.fontSize = SETCOUNT_FONT_SIZE;
        lbSetCount.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000f);
        lbSetCount.rectTransform.localPosition = SETCOUNT_POSITION;
        TextHandler.SetText(lbSetCount, "");
        lbBansRemaining = ScreenStageStrike.Instance.CreateNewText("lbBansRemaining", screenStage.transform);
        lbBansRemaining.fontSize = BANSREMAINING_FONT_SIZE;
        lbBansRemaining.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000f);
        lbBansRemaining.rectTransform.localPosition = BANSREMAINING_POSITION;
        TextHandler.SetText(lbBansRemaining, "");
        lbBanStatus = ScreenStageStrike.Instance.CreateNewText("lbBanStatus", screenStage.transform);
        lbBanStatus.fontSize = BANSTATUS_FONT_SIZE;
        lbBanStatus.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000f);
        lbBanStatus.rectTransform.localPosition = BANSTATUS_POSITION;
        TextHandler.SetText(lbBanStatus, "");

        btFreePick = ScreenStageStrike.Instance.CreateNewButton("btFreePick", screenStage.transform);
        btFreePick.transform.localScale = FREEPICK_SCALE;
        btFreePick.transform.localPosition = FREEPICK_POSITION;
        btFreePick.SetText("Toggle free pick 0/2");
        btFreePick.textMesh.transform.localScale = new Vector2(1f / FREEPICK_SCALE.x, 1f / FREEPICK_SCALE.y);
        btFreePick.textMesh.fontSize = FREEPICK_FONT_SIZE;
        btFreePick.onClick = OnClickFreePick;
        
        btRandom = ScreenStageStrike.Instance.CreateNewButton("btRandom", screenStage.transform);
        btRandom.transform.localScale = RANDOM_SCALE;
        btRandom.transform.localPosition = RANDOM_POSITION;
        btRandom.SetText("Random (off) 0/2");
        btRandom.textMesh.transform.localScale = new Vector2(1f / RANDOM_SCALE.x, 1f / RANDOM_SCALE.y);
        btRandom.textMesh.fontSize = RANDOM_FONT_SIZE;
        btRandom.onClick = OnClickRandom;
        if (SetTracker.Instance.ruleset.randomMode == Ruleset.RandomMode.OFF) btRandom.gameObject.SetActive(false);

        CreateStageContainers();
        UpdateStageBans();
        UpdateSetInfo();
    }

    private float GetRowWidth(int rowLength, Vector2 stageSize)
    {
        return rowLength * stageSize.x + (rowLength - 1) * STAGES_SPACING.x;
    }

    private struct RowLengths(int[] rowLengthsNeutral, int[]  rowLengthsCounterpick, int maxRowLength, int numRowsTotal)
    {
        internal int[] rowLengthsNeutral = rowLengthsNeutral;
        internal int[] rowLengthsCounterpick = rowLengthsCounterpick;
        internal int maxRowLength = maxRowLength;
        internal int numRowsTotal = numRowsTotal;
    }
    private static RowLengths GetRowLengths(int numStagesNeutral, int numStagesCounterpick)
    {
        int[] rowLengthsNeutral;
        int[] rowLengthsCounterpick;

        switch (numStagesCounterpick)
        {
            case 0:
                rowLengthsCounterpick = [];
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    9 => [3, 3, 3],
                    10 => [3, 4, 3],
                    11 => [4, 4, 3],
                    12 => [4, 4, 4],
                    13 => [4, 5, 4],
                    14 => [5, 5, 4],
                    15 => [5, 5, 5],
                    16 => [4, 4, 4, 4],
                    17 => [5, 4, 4, 4],
                    _ => []
                };
                break;
            
            case >= 1 and <= 4:
                rowLengthsCounterpick = [numStagesCounterpick];
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    9 => [5, 4],
                    10 => [5, 5],
                    11 => [4, 4, 3],
                    12 => [4, 4, 4],
                    13 => [4, 5, 4],
                    14 => [5, 5, 4],
                    15 => [5, 5, 5],
                    16 => [5, 6, 5],
                    _ => []
                };
                break;
            
            case 5:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 10 => [3, 2],
                    >= 9 => [5],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    9 => [5, 4],
                    10 => [5, 5],
                    11 => [4, 4, 3],
                    12 => [4, 4, 4],
                    _ => []
                };
                break;
            
            case 6:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 10 => [3, 3],
                    11 => [6],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    9 => [5, 4],
                    10 => [5, 5],
                    11 => [4, 4, 3],
                    _ => []
                };
                break;
            
            case >= 7 and <= 8:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 10 => [4, numStagesCounterpick - 4],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    9 => [5, 4],
                    10 => [5, 5],
                    _ => []
                };
                break;
            
            case 9:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 4 => [3, 3, 3],
                    >= 5 and <= 8 => [5, 4],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    _ => []
                };
                break;
            
            case 10:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    0 => [3, 4, 3],
                    >= 1 and <= 7 => [5, 5],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    7 => [4, 3],
                    _ => []
                };
                break;
            
            case 11:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 6 => [4, 4, 3],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 6 => [numStagesNeutral],
                    _ => []
                };
                break;
            
            case 12:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 5 => [4, 4, 4],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 5 => [numStagesNeutral],
                    _ => []
                };
                break;
            
            case 13:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 4 => [4, 5, 4],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    _ => []
                };
                break;
            
            case 14:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 3 => [5, 5, 4],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 3 => [numStagesNeutral],
                    _ => []
                };
                break;
            
            case 15:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 2 => [5, 5, 5],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 2 => [numStagesNeutral],
                    _ => []
                };
                break;
            
            case 16:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    0 => [4, 4, 4, 4],
                    1 => [5, 6, 5],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    1 => [1],
                    _ => []
                };
                break;
            
            case 17:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    0 => [5, 4, 4, 4],
                    _ => []
                };
                rowLengthsNeutral = [];
                break;
            
            default:
                rowLengthsCounterpick = [];
                rowLengthsNeutral = [];
                break;
        }

        int maxRowLength = 0;
        foreach (int l in rowLengthsNeutral)
        {
            if (l > maxRowLength) maxRowLength = l;
        }
        foreach (int l in rowLengthsCounterpick)
        {
            if (l > maxRowLength) maxRowLength = l;
        }
        
        int numRowsTotal = rowLengthsNeutral.Length + rowLengthsCounterpick.Length;

        return new RowLengths(rowLengthsNeutral, rowLengthsCounterpick, maxRowLength, numRowsTotal);
    }

    private void CreateStageContainers()
    {
        RowLengths rowLengths = GetRowLengths(SetTracker.Instance.ruleset.stagesNeutral.Length, SetTracker.Instance.ruleset.stagesCounterpick.Length);
        
        bool useBothCategories = rowLengths.rowLengthsNeutral.Length > 0 && rowLengths.rowLengthsCounterpick.Length > 0;
        
        Vector2 stageSize = STAGE_SIZE;
        if (rowLengths.maxRowLength > 4 || rowLengths.numRowsTotal > 3) stageSize = STAGE_SIZE_MINI;
        if (rowLengths.maxRowLength > 5) stageSize = STAGE_SIZE_TINY;
        
        float stageScaleFactor = STAGE_SCALE_FACTOR;
        if (rowLengths.maxRowLength > 4 || rowLengths.numRowsTotal > 3) stageScaleFactor = STAGE_SCALE_FACTOR_MINI;
        if (rowLengths.maxRowLength > 5) stageScaleFactor = STAGE_SCALE_FACTOR_TINY;

        float stageCategorySpacing = STAGE_CATEGORY_SPACING;
        if (rowLengths.maxRowLength > 4 || rowLengths.numRowsTotal > 3) stageCategorySpacing = STAGE_CATEGORY_SPACING_MINI;
        
        float totalHeight = rowLengths.numRowsTotal * stageSize.y + (rowLengths.numRowsTotal - 1) * STAGES_SPACING.y;
        if (useBothCategories) totalHeight += stageCategorySpacing;
        float startPositionY = STAGES_POSITION.y + totalHeight / 2f - stageSize.y / 2f;

        screenStage.nButtons = SetTracker.Instance.ruleset.stagesNeutral.Length + SetTracker.Instance.ruleset.stagesCounterpick.Length;
        screenStage.btStages = new LLButton[screenStage.nButtons];

        int stageIndex = 0;
        int rowIndex = 0;
        int colIndex = 0;
        stageContainers = new List<StageContainer>();
        stageContainersNeutral = new List<StageContainer>();
        foreach (Stage stage in SetTracker.Instance.ruleset.stagesNeutral)
        {
            float startPositionX = STAGES_POSITION.x - GetRowWidth(rowLengths.rowLengthsNeutral[rowIndex], stageSize) / 2f + stageSize.x / 2f;
            
            StageContainer container = new StageContainer(stage);
            stageContainers.Add(container);
            stageContainersNeutral.Add(container);

            float posX = startPositionX + colIndex * (stageSize.x + STAGES_SPACING.x);
            float posY = startPositionY - rowIndex * (stageSize.y + STAGES_SPACING.y);
            RectTransform buttonRect = container.Button.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(posX, posY);
            buttonRect.localScale = Vector2.one * stageScaleFactor;

            screenStage.btStages[stageIndex] = container.Button;

            stageIndex++;
            colIndex++;
            if (colIndex >= rowLengths.rowLengthsNeutral[rowIndex])
            {
                rowIndex++;
                colIndex = 0;
            }
        }

        rowIndex = 0;
        colIndex = 0;
        stageContainersCounterpick = new List<StageContainer>();
        foreach (Stage stage in SetTracker.Instance.ruleset.stagesCounterpick)
        {
            float startPositionX = STAGES_POSITION.x - GetRowWidth(rowLengths.rowLengthsCounterpick[rowIndex], stageSize) / 2f+ stageSize.x / 2f;
            
            StageContainer container = new StageContainer(stage);
            stageContainers.Add(container);
            stageContainersCounterpick.Add(container);

            float posX = startPositionX + colIndex * (stageSize.x + STAGES_SPACING.x);
            float posY = startPositionY - (rowIndex + rowLengths.rowLengthsNeutral.Length) * (stageSize.y + STAGES_SPACING.y);
            if (useBothCategories) posY -= stageCategorySpacing;
            RectTransform buttonRect = container.Button.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(posX, posY);
            buttonRect.localScale = Vector2.one * stageScaleFactor;
            
            screenStage.btStages[stageIndex] = container.Button;

            stageIndex++;
            colIndex++;
            if (colIndex >= rowLengths.rowLengthsCounterpick[rowIndex])
            {
                rowIndex++;
                colIndex = 0;
            }
        }
    }

    private void UpdateStageBans()
    {
        List<SetTracker.StageBan> stageBans = SetTracker.Instance.GetStageBans();
        foreach (SetTracker.StageBan stageBan in stageBans)
        {
            stageContainers.Find((container) => container.StoredStage == stageBan.stage).Button.SetBan(SetTracker.Instance.IsFreePickMode ? null : stageBan);
        }
    }

    private void UpdateSetInfo()
    {
        int gameNumber = SetTracker.Instance.GetGameNumber();
        int[] winCounts = SetTracker.Instance.GetWinCounts();
        TextHandler.SetText(lbSetCount, $"Game {gameNumber} ({winCounts[0]}-{winCounts[1]})");

        if (SetTracker.Instance.IsFreePickMode)
        {
            TextHandler.SetText(lbBansRemaining, $"Free pick mode");

            lbBanStatus.color = Color.white;
            TextHandler.SetText(lbBanStatus, SetTracker.Instance.CurrentInteractMode == SetTracker.InteractMode.PICK ? "Picking..." : "Banning...");
        }
        else
        {
            TextHandler.SetText(lbBansRemaining, $"Bans remaining: P1 {SetTracker.Instance.TotalBansRemaining[0]}, P2 {SetTracker.Instance.TotalBansRemaining[1]}");

            int controllingPlayer = SetTracker.Instance.ControllingPlayer;
            lbBanStatus.color = COLOR_PLAYER[controllingPlayer];
            TextHandler.SetText(lbBanStatus, SetTracker.Instance.CurrentInteractMode == SetTracker.InteractMode.BAN
                ? $"P{controllingPlayer+1} banning {SetTracker.Instance.CurrentBansRemaining}..."
                : $"P{controllingPlayer+1} picking...");
        }
        
        int freePickSum = 0;
        foreach (bool vote in freePickVotes)
        {
            if (vote) freePickSum++;
        }
        btFreePick.SetText($"Toggle free pick {freePickSum}/2");
        
        int randomSum = 0;
        foreach (bool vote in randomVotes)
        {
            if (vote) randomSum++;
        }
        btRandom.SetText($"Random {SetTracker.Instance.ruleset.randomMode switch {
            Ruleset.RandomMode.OFF => "(off)",
            Ruleset.RandomMode.ANY => "(any 3D/2D)",
            Ruleset.RandomMode.ANY_3D => "(any 3D)",
            Ruleset.RandomMode.ANY_2D => "(any 2D)",
            Ruleset.RandomMode.ANY_LEGAL => "(any legal)",
        }} {randomSum}/2");
    }

    // texture editing code from ColorSwap
    private static void SetTextureCopy(ref Texture2D destination, Texture2D source)
    {
        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        
        Graphics.Blit(source, temp);
        
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = temp;
        destination = new Texture2D(source.width, source.height, source.format, false);
        destination.ReadPixels(new Rect(0, 0, temp.width, temp.height), 0, 0);
        destination.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(temp);
    }
    private static void SetTextureColor(ref Texture2D texture, Color color)
    {
        Color[] pixels = texture.GetPixels();
        for (int pixelIndex = 0; pixelIndex < pixels.Length; pixelIndex++)
        {
            Color imgColor = pixels[pixelIndex];
            pixels[pixelIndex] = new Color(imgColor.r * color.r, imgColor.g * color.g, imgColor.b * color.b, imgColor.a * color.a);
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    internal static readonly Texture2D[] cursorImagesActive = new Texture2D[4];
    internal static readonly Texture2D[] cursorImagesInactive = new Texture2D[4];
    internal static void GenerateCursorImages(LLCursor cursor)
    {
        Texture2D source = cursor.texCursor;
        Texture2D cursorActive = new Texture2D(0, 0);
        Texture2D cursorInactive = new Texture2D(0, 0);
        SetTextureCopy(ref cursorActive, source);
        SetTextureCopy(ref cursorInactive, source);
        SetTextureColor(ref cursorInactive, COLOR_CURSOR_INACTIVE);
        Player player = cursor.player;
        cursorImagesActive[player.nr] = cursorActive;
        cursorImagesInactive[player.nr] = cursorInactive;
    }

    internal static void UpdateCursorColors(int controllingPlayer)
    {
        if (!IsOpen || SetTracker.Instance.IsFreePickMode)
        {
            ResetCursorColors();
            return;
        }
        
        Player.ForAll((Player player) =>
        {
            player.cursor.image.color = player.nr == controllingPlayer ? COLOR_CURSOR_ACTIVE : COLOR_CURSOR_INACTIVE;
            
            if (player.cursor.state != CursorState.POINTER_HW) return;
            Texture2D activeCursor = cursorImagesActive[player.nr];
            Texture2D inactiveCursor = cursorImagesInactive[player.nr];
            Cursor.SetCursor(player.nr == controllingPlayer ? activeCursor : inactiveCursor, new Vector2(0f, 0f), CursorMode.ForceSoftware);
        });
    }

    internal static void ResetCursorColors()
    {
        Player.ForAll((Player player) =>
        {
            player.cursor.image.color = COLOR_CURSOR_ACTIVE;
            
            if (player.cursor.state != CursorState.POINTER_HW) return;
            Texture2D activeCursor = cursorImagesActive[player.nr];
            Cursor.SetCursor(activeCursor, new Vector2(0f, 0f), CursorMode.ForceSoftware);
        });
    }

    private void OnClickStage(int playerNumber, Stage stage)
    {
        if (!SetTracker.Instance.CheckPlayerInteraction(stage, playerNumber)) return;

        if (SetTracker.Instance.CurrentInteractMode == SetTracker.InteractMode.PICK)
        {
            screenStage.SelectStage(playerNumber, (int)stage);
        }
        else
        {
            SetTracker.Instance.BanStage(stage, playerNumber);
            UpdateStageBans();
        }
        
        UpdateSetInfo();
    }

    private void OnClickFreePick(int playerNumber)
    {
        freePickVotes[playerNumber] = true;

        int sum = 0;
        foreach (bool vote in freePickVotes)
        {
            if (vote) sum++;
        }

        if (sum >= 2)
        {
            freePickVotes = [false, false];
            SetTracker.Instance.ToggleFreePickMode();
            UpdateStageBans();
        }
        
        UpdateSetInfo();
    }

    private void OnClickRandom(int playerNumber)
    {
        randomVotes[playerNumber] = true;

        int sum = 0;
        foreach (bool vote in randomVotes)
        {
            if (vote) sum++;
        }

        if (sum >= 2)
        {
            randomVotes = [false, false];
            List<Stage> randomStagePool = new List<Stage>();
            switch (SetTracker.Instance.ruleset.randomMode)
            {
                case Ruleset.RandomMode.ANY:
                    randomStagePool.AddRange(Ruleset.STAGES_3D);
                    randomStagePool.AddRange(Ruleset.STAGES_2D);
                    break;
                
                case Ruleset.RandomMode.ANY_3D:
                    randomStagePool.AddRange(Ruleset.STAGES_3D);
                    break;
                
                case Ruleset.RandomMode.ANY_2D:
                    randomStagePool.AddRange(Ruleset.STAGES_2D);
                    break;
                
                case Ruleset.RandomMode.ANY_LEGAL:
                    randomStagePool.AddRange(SetTracker.Instance.ruleset.stagesNeutral);
                    randomStagePool.AddRange(SetTracker.Instance.ruleset.stagesCounterpick);
                    break;
                
                case Ruleset.RandomMode.OFF:
                default:
                    break;
            }
            if (SetTracker.Instance.ruleset.randomMode != Ruleset.RandomMode.OFF) screenStage.SelectStage(playerNumber, (int)randomStagePool[Random.RandomRangeInt(0, randomStagePool.Count)]);
        }
        
        UpdateSetInfo();
    }

    private TMP_Text CreateNewText(string name, Transform parent)
    {
        TMP_Text text = Object.Instantiate(titleText, parent);
        text.gameObject.name = name;
        text.SetText("");
        text.color = Color.white;
        text.fontSize = 32;
        text.transform.localScale = Vector3.one;
        text.transform.localPosition = Vector3.zero;
        return text;
    }

    private LLButton CreateNewButton(string name, Transform parent)
    {
        LLButton button = Object.Instantiate(screenStage.btBack, parent);
        button.gameObject.name = name;
        button.SetText("");
        button.textMesh.color = Color.white;
        button.transform.localScale = Vector3.one;
        button.transform.localPosition = Vector3.zero;
        return button;
    }

    private class StageContainer
    {
        private TMP_Text lbStageName;
        private TMP_Text lbStageSize;

        internal StageButton Button { get; private set; }
        internal Stage StoredStage { get; private set; }
        private string StageName => StringUtils.GetStageReadableName(StoredStage);

        private Vector2 StageSize => StoredStage switch
        {
            Stage.OUTSKIRTS => new Vector2(1240, 510),
            Stage.SEWERS => new Vector2(1240, 510), 
            Stage.JUNKTOWN => new Vector2(1130, 510),
            Stage.CONSTRUCTION => new Vector2(1492, 522),
            Stage.FACTORY => new Vector2(1400, 542),
            Stage.SUBWAY => new Vector2(1050, 510),
            Stage.STADIUM => new Vector2(1230, 540),
            Stage.STREETS => new Vector2(1320, 515),
            Stage.POOL => new Vector2(1210, 575),
            Stage.ROOM21 => new Vector2(1100, 550),
            _ => Vector2.zero
        };

        internal StageContainer(Stage stage)
        {
            StoredStage = stage;
            
            Button = StageButton.CreateStageButton(ScreenStageStrike.Instance.screenStage.stageButtonsContainer, stage);
            Button.SetActive(true);
            Button.onClick = (playerNumber) =>
            {
                Instance.OnClickStage(playerNumber, StoredStage);
                Button.UpdateDisplay();
            };

            lbStageName = ScreenStageStrike.Instance.CreateNewText("lbStageName", Button.transform);
            lbStageName.fontSize = 42;
            lbStageName.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
            lbStageName.rectTransform.localPosition = new Vector2(0f, -110f);
            TextHandler.SetText(lbStageName, StageName);
            
            lbStageSize = ScreenStageStrike.Instance.CreateNewText("lbStageSize", Button.transform);
            lbStageSize.fontSize = 22;
            lbStageSize.rectTransform.localPosition = new Vector2(190f, 110f);
            TextHandler.SetText(lbStageSize, (StageSize != Vector2.zero ? $"{StageSize.x}x{StageSize.y}" : ""));
        }
    }

    private class StageButton : LLButton
    {
        private static readonly Color COLOR_BANNED = Color.white * 0.25f;
        private static readonly Color COLOR_UNFOCUSED = Color.white * 0.6f;
        private static readonly Color COLOR_FOCUSED = Color.white;

        private static readonly Color COLOR_LOCK = Color.white;
        
        private static readonly Color[] COLOR_LOCK_PLAYER =
        [
            new Color(255/255f, 64/255f, 22/255f),
            new Color(13/255f, 136/255f, 255/255f),
            new Color(255/255f, 255/255f, 61/255f),
            new Color(90/255f, 244/255f, 90/255f)
        ];
        private static readonly Color[] COLOR_SOFTLOCK_PLAYER =
        [
            new Color(255/255f, 64/255f, 22/255f, 0.3f),
            new Color(13/255f, 136/255f, 255/255f, 0.3f),
            new Color(255/255f, 255/255f, 61/255f, 0.3f),
            new Color(90/255f, 244/255f, 90/255f, 0.3f)
        ];

        private bool[] playersHovering = [false, false, false, false];
        private SetTracker.StageBan stageBan;

        private bool IsBeingHovered =>
            playersHovering[0] || playersHovering[1] || playersHovering[2] || playersHovering[3];

        private Image stageImage;
        private Image lockedImage;
        private TMP_Text lbBanReason;

        internal static StageButton CreateStageButton(Transform parent, Stage stage)
        {
            RectTransform rect = LLControl.CreatePanel(parent, $"Button_{stage}");
            StageButton stageButton = rect.gameObject.AddComponent<StageButton>();
            Sprite stageSprite = JPLELOFJOOH.BNFIDCAPPDK($"_spritePreview{stage}"); // Assets.GetMenuSprite()
            stageButton.stageImage = LLControl.CreateImage(rect, stageSprite);
            Sprite lockedSprite = JPLELOFJOOH.BNFIDCAPPDK($"_spritePreviewLOCKED"); // Assets.GetMenuSprite()
            stageButton.lockedImage = LLControl.CreateImage(rect, lockedSprite);
            stageButton.lbBanReason = ScreenStageStrike.Instance.CreateNewText("lbBanReason", stageButton.transform);
            stageButton.lbBanReason.color = Color.white;
            stageButton.lbBanReason.fontSize = 22;
            stageButton.lbBanReason.rectTransform.localPosition = new Vector2(0f, 13f);
            TextHandler.SetText(stageButton.lbBanReason, "");
            stageButton.Init();
            return stageButton;
        }

        public override void InitNeeded()
        {
            OnHoverOut(-1);
        }

        public void SetBan(SetTracker.StageBan ban)
        {
            stageBan = ban;
            OnHoverOut(stageBan != null ? stageBan.banPlayer : -1);
            UpdateDisplay();
        }

        public override void OnHover(int playerNumber)
        {
            if (playerNumber == -1) return;
            playersHovering[playerNumber] = SetTracker.Instance.CheckPlayerInteraction(stageBan, playerNumber);
            UpdateDisplay();
        }

        public override void OnHoverOut(int playerNumber)
        {
            if (playerNumber == -1) playersHovering = [false, false, false, false];
            else playersHovering[playerNumber] = false;
            UpdateDisplay();
        }

        internal void UpdateDisplay()
        {
            if (IsBeingHovered)
            {
                stageImage.color = COLOR_FOCUSED;
            }
            else if (stageBan != null)
            {
                if (stageBan.reason == SetTracker.StageBan.BanReason.DSR && stageBan.banPlayer != -1)
                {
                    stageImage.color = COLOR_UNFOCUSED;
                }
                else
                {
                    stageImage.color = COLOR_BANNED;
                }
            }
            else
            {
                stageImage.color = COLOR_UNFOCUSED;
            }
            lockedImage.gameObject.SetActive(stageBan != null);
            if (stageBan == null)
            {
                TextHandler.SetText(lbBanReason, "");
                return;
            }

            switch (stageBan.reason)
            {
                case SetTracker.StageBan.BanReason.COUNTERPICK:
                    lockedImage.color = COLOR_LOCK;
                    lbBanReason.color = COLOR_LOCK;
                    TextHandler.SetText(lbBanReason, "Counterpick");
                    break;
                case SetTracker.StageBan.BanReason.BAN:
                    lockedImage.color = COLOR_LOCK_PLAYER[stageBan.banPlayer];
                    lbBanReason.color = COLOR_LOCK_PLAYER[stageBan.banPlayer];
                    TextHandler.SetText(lbBanReason, $"P{stageBan.banPlayer+1} Ban");
                    break;
                case SetTracker.StageBan.BanReason.DSR:
                    lockedImage.color = (stageBan.banPlayer == -1) ? COLOR_LOCK : COLOR_SOFTLOCK_PLAYER[stageBan.banPlayer];
                    lbBanReason.color = (stageBan.banPlayer == -1) ? COLOR_LOCK : COLOR_SOFTLOCK_PLAYER[stageBan.banPlayer];
                    TextHandler.SetText(lbBanReason, (stageBan.banPlayer == -1) ? "Both DSR" : $"P{stageBan.banPlayer+1} DSR");
                    break;
            }
        }
    }
}