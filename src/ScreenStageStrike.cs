using System.Collections.Generic;
using LLBML.States;
using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod;

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
    private static readonly Vector2 STAGE_SIZE = new Vector2(500f, 250f) * STAGE_SCALE_FACTOR;
    private static readonly Vector2 STAGES_POSITION = new Vector2(0f, 0f);
    private static readonly Vector2 STAGES_SPACING = new Vector2(6f, 6f);
    private const float STAGE_CATEGORY_SPACING = 20f;

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

        CreateStageButtons();
        UpdateStageBans();
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
        int[] rowLengthsNeutral = GetRowLengths(SetTracker.Instance.StagesNeutral.Length);
        int[] rowLengthsCounterpick = GetRowLengths(SetTracker.Instance.StagesCounterpick.Length);
        int numRows = rowLengthsNeutral.Length + rowLengthsCounterpick.Length;
        bool bothCategories = rowLengthsNeutral.Length > 0 && rowLengthsCounterpick.Length > 0;
        
        float totalHeight = numRows * STAGE_SIZE.y + (numRows - 1) * STAGES_SPACING.y;
        if (bothCategories) totalHeight += STAGE_CATEGORY_SPACING;
        float startPositionY = STAGES_POSITION.y + totalHeight / 2f - STAGE_SIZE.y / 2f;

        screenStage.nButtons = SetTracker.Instance.StagesNeutral.Length + SetTracker.Instance.StagesCounterpick.Length;
        screenStage.btStages = new LLButton[screenStage.nButtons];

        int stageIndex = 0;
        int rowIndex = 0;
        int colIndex = 0;
        stageContainers = new List<StageContainer>();
        stageContainersNeutral = new List<StageContainer>();
        foreach (Stage stage in SetTracker.Instance.StagesNeutral)
        {
            float startPositionX = STAGES_POSITION.x - GetRowWidth(rowLengthsNeutral[rowIndex]) / 2f + STAGE_SIZE.x / 2f;
            
            StageContainer container = new StageContainer(stage);
            stageContainers.Add(container);
            stageContainersNeutral.Add(container);

            float posX = startPositionX + colIndex * (STAGE_SIZE.x + STAGES_SPACING.x);
            float posY = startPositionY - rowIndex * (STAGE_SIZE.y + STAGES_SPACING.y);
            RectTransform buttonRect = container.Button.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(posX, posY);
            buttonRect.localScale = Vector2.one * STAGE_SCALE_FACTOR;

            screenStage.btStages[stageIndex] = container.Button;

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
        foreach (Stage stage in SetTracker.Instance.StagesCounterpick)
        {
            float startPositionX = STAGES_POSITION.x - GetRowWidth(rowLengthsCounterpick[rowIndex]) / 2f+ STAGE_SIZE.x / 2f;
            
            StageContainer container = new StageContainer(stage);
            stageContainers.Add(container);
            stageContainersCounterpick.Add(container);

            float posX = startPositionX + colIndex * (STAGE_SIZE.x + STAGES_SPACING.x);
            float posY = startPositionY - (rowIndex + rowLengthsNeutral.Length) * (STAGE_SIZE.y + STAGES_SPACING.y);
            if (bothCategories) posY -= STAGE_CATEGORY_SPACING;
            RectTransform buttonRect = container.Button.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(posX, posY);
            buttonRect.localScale = Vector2.one * STAGE_SCALE_FACTOR;
            
            screenStage.btStages[stageIndex] = container.Button;

            stageIndex++;
            colIndex++;
            if (colIndex >= rowLengthsCounterpick[rowIndex])
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
            stageContainers.Find((container) => container.StoredStage == stageBan.stage).Button.SetBan(stageBan);
        }
    }

    private void OnClickStage(int playerNumber, Stage stage)
    {
        if (!SetTracker.Instance.CheckPlayerInteraction(stage, playerNumber)) return;
        
        screenStage.SelectStage(playerNumber, (int)stage);
        UIScreen.blockGlobalInput = false;
        //SetTracker.Instance.BanStage(stage, playerNumber);
        UpdateStageBans();
    }

    internal TMP_Text CreateNewText(string name, Transform parent)
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

    private class StageContainer
    {
        private TMP_Text lbStageName;
        private TMP_Text lbStageSize;

        internal StageButton Button { get; private set; }
        internal Stage StoredStage { get; private set; }
        private string StageName => StoredStage switch
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
            lbStageName.rectTransform.localPosition = new Vector2(0f, -110f);
            TextHandler.SetText(lbStageName, StageName);
            
            lbStageSize = ScreenStageStrike.Instance.CreateNewText("lbStageSize", Button.transform);
            lbStageSize.fontSize = 22;
            lbStageSize.rectTransform.localPosition = new Vector2(190f, 110f);
            TextHandler.SetText(lbStageSize, $"{StageSize.x}x{StageSize.y}");
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
            OnHoverOut(stageBan.banPlayer);
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