using System.Collections.Generic;
using LLBML.States;
using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using UnityEngine;

namespace TourneyMod;

public class ScreenStageStrike
{
    internal static ScreenStageStrike Instance { get; private set; }
    internal static bool IsOpen => Instance != null;
    private ScreenPlayersStage screenStage;
    private List<StageContainer> stageContainersNeutral;
    private List<StageContainer> stageContainersCounterpick;

    private static readonly Vector2 BG_SCALE = new Vector2(1f, 2f);
    private static readonly Vector2 BG_POSITION = new Vector2(0f, -20f);

    private static readonly Vector2 TITLE_SCALE = new Vector2(1f, 0.6f);
    private static readonly Vector2 TITLE_POSITION = new Vector2(0f, 328f);
    private const int TITLE_FONT_SIZE = 36;

    private static readonly Vector2 BACK_SCALE = new Vector2(0.6f, 0.5f);
    private static readonly Vector2 BACK_POSITION = new Vector2(-570f, -336f);
    private const int BACK_FONT_SIZE = 22;

    private static readonly float STAGE_SCALE_FACTOR = 0.62f;
    private static readonly Vector2 STAGE_SIZE = new Vector2(500f, 250f) * STAGE_SCALE_FACTOR;
    private static readonly Vector2 STAGES_POSITION = new Vector2(0f, 0f);
    private static readonly Vector2 STAGES_SPACING = new Vector2(6f, 6f);
    private const float STAGE_CATEGORY_SPACING = 20f;

    internal static void Open()
    {
        Plugin.LogGlobal.LogInfo("Opening stage strike screen");
        Instance = new ScreenStageStrike();
    }

    internal static void Close()
    {
        Plugin.LogGlobal.LogInfo("Closing stage strike screen");
        Instance = null;
    }

    internal void OnOpen(ScreenPlayersStage screenStage)
    {
        this.screenStage = screenStage;
        screenStage.UpdateText();
        this.screenStage.msgMenu = Msg.NONE;

        Plugin.LogGlobal.LogInfo("ScreenStageStrike OnOpen successful");
        this.screenStage = screenStage;

        RectTransform bar_top = screenStage.transform.Find("bar_top").GetComponent<RectTransform>();
        bar_top.anchoredPosition = TITLE_POSITION;
        bar_top.localScale = TITLE_SCALE;

        RectTransform lbTitle = bar_top.Find("lbTitle").GetComponent<RectTransform>();
        lbTitle.localScale = new Vector2(1f / TITLE_SCALE.x, 1f / TITLE_SCALE.y);
        TMP_Text titleText = lbTitle.GetComponent<TMP_Text>();
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

        CreateStageButtons();
    }

    private float GetRowWidth(int rowLength)
    {
        return rowLength * STAGE_SIZE.x + (rowLength - 1) * STAGES_SPACING.x;
    }

    private static int[] GetRowLengths(int numStages)
    {
        return numStages switch {
            >= 1 and <= 4 => [numStages],
            >= 5 and <= 6 => [3, numStages - 3],
            >= 7 and <= 8 => [4, numStages - 4],
            9 => [3, 3, 3],
            _ => []
        };
    }

    private void CreateStageButtons()
    {
        int[] rowLengthsNeutral = GetRowLengths(Plugin.StagesNeutral.Length);
        int[] rowLengthsCounterpick = GetRowLengths(Plugin.StagesCounterpick.Length);
        int numRows = rowLengthsNeutral.Length + rowLengthsCounterpick.Length;
        bool bothCategories = rowLengthsNeutral.Length > 0 && rowLengthsCounterpick.Length > 0;
        
        float totalHeight = numRows * STAGE_SIZE.y + (numRows - 1) * STAGES_SPACING.y;
        if (bothCategories) totalHeight += STAGE_CATEGORY_SPACING;
        float startPositionY = STAGES_POSITION.y + totalHeight / 2f - STAGE_SIZE.y / 2f;

        screenStage.nButtons = Plugin.StagesNeutral.Length + Plugin.StagesCounterpick.Length;
        screenStage.btStages = new LLButton[screenStage.nButtons];

        int stageIndex = 0;
        int rowIndex = 0;
        int colIndex = 0;
        stageContainersNeutral = new List<StageContainer>();
        foreach (Stage stage in Plugin.StagesNeutral)
        {
            float startPositionX = STAGES_POSITION.x - GetRowWidth(rowLengthsNeutral[rowIndex]) / 2f + STAGE_SIZE.x / 2f;
            
            StageContainer container = new StageContainer(stage);
            stageContainersNeutral.Add(container);

            float posX = startPositionX + colIndex * (STAGE_SIZE.x + STAGES_SPACING.x);
            float posY = startPositionY - rowIndex * (STAGE_SIZE.y + STAGES_SPACING.y);
            RectTransform buttonRect = container.StageButton.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(posX, posY);
            buttonRect.localScale = Vector2.one * STAGE_SCALE_FACTOR;

            screenStage.btStages[stageIndex] = container.StageButton;

            stageIndex++;
            colIndex++;
            if (colIndex >= rowLengthsNeutral[rowIndex])
            {
                rowIndex++;
                colIndex = 0;
            }
        }

        rowIndex = 0;
        colIndex = 0;
        stageContainersCounterpick = new List<StageContainer>();
        foreach (Stage stage in Plugin.StagesCounterpick)
        {
            float startPositionX = STAGES_POSITION.x - GetRowWidth(rowLengthsCounterpick[rowIndex]) / 2f+ STAGE_SIZE.x / 2f;
            
            StageContainer container = new StageContainer(stage);
            stageContainersCounterpick.Add(container);

            float posX = startPositionX + colIndex * (STAGE_SIZE.x + STAGES_SPACING.x);
            float posY = startPositionY - (rowIndex + rowLengthsNeutral.Length) * (STAGE_SIZE.y + STAGES_SPACING.y);
            if (bothCategories) posY -= STAGE_CATEGORY_SPACING;
            RectTransform buttonRect = container.StageButton.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(posX, posY);
            buttonRect.localScale = Vector2.one * STAGE_SCALE_FACTOR;
            
            screenStage.btStages[stageIndex] = container.StageButton;

            stageIndex++;
            colIndex++;
            if (colIndex >= rowLengthsCounterpick[rowIndex])
            {
                rowIndex++;
                colIndex = 0;
            }
        }
    }

    private void OnClickStage(int playerNumber, Stage stage)
    {
        screenStage.SelectStage(playerNumber, (int)stage);
    }

    private class StageContainer
    {
        private Sprite stageSprite;
        private TMP_Text lbStageName;
        private TMP_Text lbStageSize;

        internal LLButton StageButton { get; private set; }
        internal Stage StoredStage { get; private set; }
        internal string StageName => StoredStage switch
        {
            Stage.OUTSKIRTS => "Outskirts",
            Stage.SEWERS => "Sewers", 
            Stage.JUNKTOWN => "Desert",
            Stage.CONSTRUCTION => "Elevator",
            Stage.FACTORY => "Factory",
            Stage.SUBWAY => "Subway",
            Stage.STADIUM => "Stadium",
            Stage.STREETS => "Streets",
            Stage.POOL => "Pool",
            Stage.ROOM21 => "Room 21",
            _ => ""
        };

        internal Vector2 StageSize => StoredStage switch
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
        
        internal static Color COLOR_UNFOCUSED = Color.white * 0.6f;
        internal static readonly Color COLOR_FOCUSED = Color.white;

        internal StageContainer(Stage stage)
        {
            StoredStage = stage;

            stageSprite = JPLELOFJOOH.BNFIDCAPPDK($"_spritePreview{stage}"); // Assets.GetMenuSprite()
            StageButton = LLButton.CreateImageButton(ScreenStageStrike.Instance.screenStage.stageButtonsContainer, stageSprite, COLOR_UNFOCUSED, COLOR_FOCUSED);
            StageButton.SetActive(true);
            StageButton.onClick = (playerNumber) =>
                Instance.OnClickStage(playerNumber, StoredStage);

            //lbStageName = new GameObject("lbStageName").AddComponent<TMP_Text>();
            //lbStageSize = new GameObject("lbStageName").AddComponent<TMP_Text>();
        }
    }
}