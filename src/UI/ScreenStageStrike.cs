using System.Collections.Generic;
using LLBML.Players;
using LLBML.Settings;
using LLBML.States;
using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using UnityEngine;

namespace TourneyMod.UI;

internal class ScreenStageStrike
{
    internal static ScreenStageStrike Instance { get; private set; }
    internal static bool IsOpen => Instance != null;
    internal ScreenPlayersStage screenStage;
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

    private static readonly Vector2 SETCOUNT_POSITION = new Vector2(0f, 270f);
    private const int SETCOUNT_FONT_SIZE = 32;
    private static readonly Vector2 BANSREMAINING_POSITION = new Vector2(0f, -276f);
    private const int BANSREMAINING_FONT_SIZE = 18;
    private static readonly Vector2 BANSTATUS_POSITION = new Vector2(0f, -310f);
    private const int BANSTATUS_FONT_SIZE = 42;
    
    private static readonly Vector2 FREEPICK_POSITION = new Vector2(506f, -336f);
    private static readonly Vector2 FREEPICK_SCALE = new Vector2(234f, 27.5f);
    private const int FREEPICK_FONT_SIZE = 18;
    
    private static readonly Vector2 RANDOM_POSITION = new Vector2(-378f, -336f);
    private static readonly Vector2 RANDOM_SCALE = new Vector2(255.6f, 27.5f);
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

    private TextMeshProUGUI lbSetCount;
    private TextMeshProUGUI lbBansRemaining;
    private TextMeshProUGUI lbBanStatus;

    private LLButton btFreePick;
    private bool[] freePickVotes = [false, false, false, false];
    
    private LLButton btRandom;
    private bool[] randomVotes = [false, false, false, false];

    internal static void Open()
    {
        //Plugin.LogGlobal.LogInfo("Opening stage strike screen");
        Instance = new ScreenStageStrike();
        UIScreen.blockGlobalInput = true;
    }

    internal static void Close()
    {
        //Plugin.LogGlobal.LogInfo("Closing stage strike screen");
        Instance = null;
        UIScreen.blockGlobalInput = false;
    }

    internal void OnOpen(ScreenPlayersStage screenStage)
    {
        this.screenStage = screenStage;
        screenStage.UpdateText();
        screenStage.msgMenu = Msg.NONE;

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
        if (backText != null) backText.fontSize = BACK_FONT_SIZE;
        
        UI.CreateText(ref lbSetCount, "lbSetCount", screenStage.transform, SETCOUNT_POSITION);
        lbSetCount.fontSize = SETCOUNT_FONT_SIZE;
        TextHandler.SetText(lbSetCount, "");
        if (GameSettings.current.gameMode != GameMode._1v1 || (GameSettings.IsOnline && GameSettings.OnlineMode == OnlineMode.RANKED))
        {
            //Plugin.LogGlobal.LogInfo("Game mode is not local 1v1! Hiding stage select set count");
            lbSetCount.gameObject.SetActive(false);
        }
        
        UI.CreateText(ref lbBansRemaining, "lbBansRemaining", screenStage.transform, BANSREMAINING_POSITION);
        lbBansRemaining.fontSize = BANSREMAINING_FONT_SIZE;
        TextHandler.SetText(lbBansRemaining, "");
        
        UI.CreateText(ref lbBanStatus, "lbBanStatus", screenStage.transform, BANSTATUS_POSITION);
        lbBanStatus.fontSize = BANSTATUS_FONT_SIZE;
        TextHandler.SetText(lbBanStatus, "");

        UI.CreateButton(ref btFreePick, "btFreePick", screenStage.transform, FREEPICK_POSITION, FREEPICK_SCALE);
        btFreePick.SetText("Toggle free pick 0/0");
        btFreePick.textMesh.fontSize = FREEPICK_FONT_SIZE;
        btFreePick.onClick = OnClickFreePick;
        if (StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced) btFreePick.gameObject.SetActive(false);
        
        UI.CreateButton(ref btRandom, "btRandom", screenStage.transform, RANDOM_POSITION, RANDOM_SCALE);
        btRandom.SetText("Random (off) 0/0");
        btRandom.textMesh.fontSize = RANDOM_FONT_SIZE;
        btRandom.onClick = OnClickRandom;
        if (StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.randomMode == Ruleset.RandomMode.OFF) btRandom.gameObject.SetActive(false);

        CreateStageContainers();
        UpdateStageBans();
        UpdateSetInfo();
    }

    private void CreateStageContainers()
    {
        StageLayout layout = StageLayout.Create(StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesNeutral.Count, StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesCounterpick.Count);
        
        float startPositionY = layout.position.y + layout.totalHeight / 2f - layout.stageSize.y / 2f;

        screenStage.nButtons = StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesNeutral.Count + StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesCounterpick.Count;
        screenStage.btStages = new LLButton[screenStage.nButtons];

        int stageIndex = 0;
        int rowIndex = 0;
        int colIndex = 0;
        stageContainers = new List<StageContainer>();
        stageContainersNeutral = new List<StageContainer>();
        foreach (Stage stage in StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesNeutral)
        {
            float startPositionX = layout.position.x - layout.GetRowWidth(layout.rowLengthsNeutral[rowIndex]) / 2f + layout.stageSize.x / 2f;
            
            StageContainer container = new StageContainer(stage);
            stageContainers.Add(container);
            stageContainersNeutral.Add(container);

            float posX = startPositionX + colIndex * (layout.stageSize.x + layout.spacing.x);
            float posY = startPositionY - rowIndex * (layout.stageSize.y + layout.spacing.y);
            RectTransform buttonRect = container.Button.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(posX, posY);
            buttonRect.localScale = Vector2.one * layout.stageScaleFactor;

            screenStage.btStages[stageIndex] = container.Button;

            stageIndex++;
            colIndex++;
            if (colIndex >= layout.rowLengthsNeutral[rowIndex])
            {
                rowIndex++;
                colIndex = 0;
            }
        }

        rowIndex = 0;
        colIndex = 0;
        stageContainersCounterpick = new List<StageContainer>();
        foreach (Stage stage in StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesCounterpick)
        {
            float startPositionX = layout.position.x - layout.GetRowWidth(layout.rowLengthsCounterpick[rowIndex]) / 2f+ layout.stageSize.x / 2f;
            
            StageContainer container = new StageContainer(stage);
            stageContainers.Add(container);
            stageContainersCounterpick.Add(container);

            float posX = startPositionX + colIndex * (layout.stageSize.x + layout.spacing.x);
            float posY = startPositionY - (rowIndex + layout.rowLengthsNeutral.Length) * (layout.stageSize.y + layout.spacing.y);
            if (layout.useBothCategories) posY -= layout.stageCategorySpacing;
            RectTransform buttonRect = container.Button.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(posX, posY);
            buttonRect.localScale = Vector2.one * layout.stageScaleFactor;
            
            screenStage.btStages[stageIndex] = container.Button;

            stageIndex++;
            colIndex++;
            if (colIndex >= layout.rowLengthsCounterpick[rowIndex])
            {
                rowIndex++;
                colIndex = 0;
            }
        }
    }

    private void UpdateStageBans()
    {
        List<StageBan> stageBans = StageStrikeTracker.Instance.CurrentStrikeInfo.StageBans;
        stageBans.ForEach(ban =>
        {
            StageContainer stageContainer = stageContainers.Find(container => container.StoredStage == ban.stage);
            if (stageContainer == null) return;
            stageContainer.Button.SetBan(StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced ? null : ban);
        });
    }

    private void UpdateSetInfo()
    {
        int gameNumber = 0;
        int[] winCounts = [0, 0, 0, 0];
        if (SetTracker.Instance.IsTrackingSet)
        {
            gameNumber = SetTracker.Instance.CurrentSet.GameNumber;
            winCounts = SetTracker.Instance.CurrentSet.WinCounts;
        }
        
        TextHandler.SetText(lbSetCount, $"Game {gameNumber} ({winCounts[0]}-{winCounts[1]})");

        if (StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced)
        {
            TextHandler.SetText(lbBansRemaining, $"Free pick mode");

            lbBanStatus.color = Color.white;
            TextHandler.SetText(lbBanStatus, StageStrikeTracker.Instance.CurrentStrikeInfo.CurrentInteractMode == StrikeInfo.InteractMode.PICK ? "Picking..." : "Banning...");
        }
        else
        {
            TextHandler.SetText(lbBansRemaining, $"Bans remaining: P1 {StageStrikeTracker.Instance.CurrentStrikeInfo.TotalBansRemaining[0]}, P2 {StageStrikeTracker.Instance.CurrentStrikeInfo.TotalBansRemaining[1]}");

            int controllingPlayer = StageStrikeTracker.Instance.CurrentStrikeInfo.ControllingPlayer;
            lbBanStatus.color = COLOR_PLAYER[controllingPlayer];
            TextHandler.SetText(lbBanStatus, StageStrikeTracker.Instance.CurrentStrikeInfo.CurrentInteractMode == StrikeInfo.InteractMode.BAN
                ? $"P{controllingPlayer+1} banning {StageStrikeTracker.Instance.CurrentStrikeInfo.CurrentBansRemaining}..."
                : $"P{controllingPlayer+1} picking...");
        }
        
        int freePickSum = 0;
        foreach (bool vote in freePickVotes)
        {
            if (vote) freePickSum++;
        }
        btFreePick.SetText($"Toggle free pick {freePickSum}/{SetTracker.Instance.NumPlayersInMatch}");
        
        int randomSum = 0;
        foreach (bool vote in randomVotes)
        {
            if (vote) randomSum++;
        }
        btRandom.SetText($"Random {StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.randomMode switch {
            Ruleset.RandomMode.OFF => "(off)",
            Ruleset.RandomMode.ANY => "(any 3D/2D)",
            Ruleset.RandomMode.ANY_3D => "(any 3D)",
            Ruleset.RandomMode.ANY_2D => "(any 2D)",
            Ruleset.RandomMode.ANY_LEGAL => "(any legal)",
        }}" + (StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced ? "" : $" {randomSum}/{SetTracker.Instance.NumPlayersInMatch}"));
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
        if (!IsOpen || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced)
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

    internal void OnClickStage(int playerNumber, Stage stage)
    {
        if (!StageStrikeTracker.Instance.CurrentStrikeInfo.CheckPlayerInteraction(stage, playerNumber)) return;

        if (StageStrikeTracker.Instance.CurrentStrikeInfo.CurrentInteractMode == StrikeInfo.InteractMode.PICK)
        {
            UIScreen.blockGlobalInput = false;
            StageStrikeTracker.Instance.CurrentStrikeInfo.PickStage(screenStage, stage, playerNumber);
        }
        else
        {
            StageStrikeTracker.Instance.CurrentStrikeInfo.BanStage(stage, playerNumber);
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

        if (sum >= SetTracker.Instance.NumPlayersInMatch)
        {
            freePickVotes = [false, false, false, false];
            StageStrikeTracker.Instance.CurrentStrikeInfo.ToggleFreePickMode();
            UpdateStageBans();
        }
        
        UpdateSetInfo();
    }

    private void OnClickRandom(int playerNumber)
    {
        if (playerNumber != -1) randomVotes[playerNumber] = true;

        int sum = 0;
        foreach (bool vote in randomVotes)
        {
            if (vote) sum++;
        }

        if (sum >= SetTracker.Instance.NumPlayersInMatch || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced)
        {
            randomVotes = [false, false, false, false];

            if (StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.randomMode != Ruleset.RandomMode.OFF)
            {
                UIScreen.blockGlobalInput = false;
                StageStrikeTracker.Instance.CurrentStrikeInfo.PickRandomStage(screenStage, playerNumber);
            }
        }
        
        UpdateSetInfo();
    }
}