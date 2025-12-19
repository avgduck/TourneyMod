using System.Collections.Generic;
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

internal class ScreenStageStrike : ScreenPlayersStage, ICustomScreen<ScreenPlayersStage>
{
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
    
    private List<StageContainer> stageContainersNeutral;
    private List<StageContainer> stageContainersCounterpick;
    private List<StageContainer> stageContainers;
    
    private TextMeshProUGUI lbSetCount;
    private TextMeshProUGUI lbBansRemaining;
    private TextMeshProUGUI lbBanStatus;

    private VoteButton btFreePick;
    private VoteButton btRandom;
    
    public void Init(ScreenPlayersStage screenPlayersStage)
    {
        screenType = screenPlayersStage.screenType;
        layer = screenPlayersStage.layer;
        isActive = screenPlayersStage.isActive;
        msgEsc = screenPlayersStage.msgEsc;
        msgMenu = screenPlayersStage.msgMenu;
        msgCancel = screenPlayersStage.msgCancel;
        
        btBack = screenPlayersStage.btBack;
        btLeft = screenPlayersStage.btLeft;
        btRight = screenPlayersStage.btRight;
        lbTitle = screenPlayersStage.lbTitle;
        lbPlayersSelectingStage = screenPlayersStage.lbPlayersSelectingStage;
        obSpectator = screenPlayersStage.obSpectator;
        obNotSpectator = screenPlayersStage.obNotSpectator;
        stageButtonsContainer = screenPlayersStage.stageButtonsContainer;
        btStages = screenPlayersStage.btStages;
        posMid = screenPlayersStage.posMid;
        posLeft = screenPlayersStage.posLeft;
        posLeft2 = screenPlayersStage.posLeft2;
        posRight = screenPlayersStage.posRight;
        posRight2 = screenPlayersStage.posRight2;
        scaleSmall = screenPlayersStage.scaleSmall;
        scaleBig = screenPlayersStage.scaleBig;
        nButtons = screenPlayersStage.nButtons;
        curIndex = screenPlayersStage.curIndex;
        isMoving = screenPlayersStage.isMoving;
        queuedMove = screenPlayersStage.queuedMove;
    }

    public override void OnOpen(ScreenType screenTypePrev)
    {
        Plugin.LogGlobal.LogInfo("Custom stage select OnOpen");
        StageStrikeTracker.Instance.Start();
        UIScreen.blockGlobalInput = true;
        Plugin.Instance.RecolorCursors = true;
        
        // manually do ScreenBase::OnOpen to avoid going through ScreenPlayersStage::OnOpen
        if (TextHandler.isInited)
        {
            UpdateText();
        }

        msgMenu = Msg.NONE;
        
        RectTransform bar_top = transform.Find("bar_top").GetComponent<RectTransform>();
        bar_top.anchoredPosition = TITLE_POSITION;
        bar_top.localScale = TITLE_SCALE;
        
        lbTitle.rectTransform.localScale = new Vector2(1f / TITLE_SCALE.x, 1f / TITLE_SCALE.y);
        lbTitle.fontSize = TITLE_FONT_SIZE;

        RectTransform bar_mid = transform.Find("bar_mid").GetComponent<RectTransform>();
        bar_mid.anchoredPosition = BG_POSITION;
        bar_mid.localScale = BG_SCALE;

        btLeft.visible = false;
        btRight.visible = false;
        btBack.onClick = (playerNumber) => { GameStates.Send(Msg.BACK, playerNumber, -1); };

        btBack.transform.localPosition = BACK_POSITION;
        btBack.transform.localScale = BACK_SCALE;
        btBack.textMesh.rectTransform.localScale = new Vector2(1f / BACK_SCALE.x, 1f / BACK_SCALE.y);
        if (btBack.textMesh != null) btBack.textMesh.fontSize = BACK_FONT_SIZE;
        
        UIUtils.CreateText(ref lbSetCount, "lbSetCount", transform, SETCOUNT_POSITION);
        lbSetCount.fontSize = SETCOUNT_FONT_SIZE;
        TextHandler.SetText(lbSetCount, "");
        if (Plugin.Instance.ActiveTourneyMode == TourneyMode.NONE)
        {
            //Plugin.LogGlobal.LogInfo("Tourney mode not active! Hiding stage select set count");
            lbSetCount.gameObject.SetActive(false);
        }
        
        UIUtils.CreateText(ref lbBansRemaining, "lbBansRemaining", transform, BANSREMAINING_POSITION);
        lbBansRemaining.fontSize = BANSREMAINING_FONT_SIZE;
        TextHandler.SetText(lbBansRemaining, "");
        
        UIUtils.CreateText(ref lbBanStatus, "lbBanStatus", transform, BANSTATUS_POSITION);
        lbBanStatus.fontSize = BANSTATUS_FONT_SIZE;
        TextHandler.SetText(lbBanStatus, "");

        UIUtils.CreateVoteButton(ref btFreePick, "btFreePick", transform, FREEPICK_POSITION, FREEPICK_SCALE);
        VoteButton.ActiveVoteButtons.Add(btFreePick);
        btFreePick.label = "Toggle free pick";
        btFreePick.textMesh.fontSize = FREEPICK_FONT_SIZE;
        btFreePick.onVote = OnVoteFreePick;
        if (StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced) btFreePick.gameObject.SetActive(false);
        
        UIUtils.CreateVoteButton(ref btRandom, "btRandom", transform, RANDOM_POSITION, RANDOM_SCALE);
        VoteButton.ActiveVoteButtons.Add(btRandom);
        btRandom.label = $"Random {StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.randomMode switch {
            Ruleset.RandomMode.OFF => "(off)",
            Ruleset.RandomMode.ANY => "(any 3D/2D)",
            Ruleset.RandomMode.ANY_3D => "(any 3D)",
            Ruleset.RandomMode.ANY_2D => "(any 2D)",
            Ruleset.RandomMode.ANY_LEGAL => "(any legal)",
        }}";
        btRandom.textMesh.fontSize = RANDOM_FONT_SIZE;
        btRandom.onVote = OnVoteRandom;
        btRandom.enableVoting = !(StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced);
        if (StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.randomMode == Ruleset.RandomMode.OFF) btRandom.gameObject.SetActive(false);

        CreateStageContainers();
        UpdateStageBans();
        UpdateSetInfo();
    }

    public override void OnClose(ScreenType screenTypeNext)
    {
        Plugin.LogGlobal.LogInfo("Custom stage select OnClose");
        StageStrikeTracker.Instance.End();
        UIScreen.blockGlobalInput = false;
        Plugin.Instance.RecolorCursors = false;
        
        VoteButton.ActiveVoteButtons.Remove(btFreePick);
        VoteButton.ActiveVoteButtons.Remove(btRandom);
        
        base.OnClose(screenTypeNext);
    }
    
    private void CreateStageContainers()
    {
        StageLayout layout = StageLayout.Create(StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesNeutral.Count, StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesCounterpick.Count);
        
        float startPositionY = layout.position.y + layout.totalHeight / 2f - layout.stageSize.y / 2f;

        nButtons = StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesNeutral.Count + StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesCounterpick.Count;
        btStages = new LLButton[nButtons];

        int stageIndex = 0;
        int rowIndex = 0;
        int colIndex = 0;
        stageContainers = new List<StageContainer>();
        stageContainersNeutral = new List<StageContainer>();
        foreach (Stage stage in StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.stagesNeutral)
        {
            float startPositionX = layout.position.x - layout.GetRowWidth(layout.rowLengthsNeutral[rowIndex]) / 2f + layout.stageSize.x / 2f;
            
            StageContainer container = new StageContainer(this, stage);
            stageContainers.Add(container);
            stageContainersNeutral.Add(container);

            float posX = startPositionX + colIndex * (layout.stageSize.x + layout.spacing.x);
            float posY = startPositionY - rowIndex * (layout.stageSize.y + layout.spacing.y);
            RectTransform buttonRect = container.Button.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(posX, posY);
            buttonRect.localScale = Vector2.one * layout.stageScaleFactor;

            btStages[stageIndex] = container.Button;

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
            
            StageContainer container = new StageContainer(this, stage);
            stageContainers.Add(container);
            stageContainersCounterpick.Add(container);

            float posX = startPositionX + colIndex * (layout.stageSize.x + layout.spacing.x);
            float posY = startPositionY - (rowIndex + layout.rowLengthsNeutral.Length) * (layout.stageSize.y + layout.spacing.y);
            if (layout.useBothCategories) posY -= layout.stageCategorySpacing;
            RectTransform buttonRect = container.Button.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(posX, posY);
            buttonRect.localScale = Vector2.one * layout.stageScaleFactor;
            
            btStages[stageIndex] = container.Button;

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
    }

    internal void OnClickStage(int playerNumber, Stage stage)
    {
        if (!StageStrikeTracker.Instance.CurrentStrikeInfo.CheckPlayerInteraction(stage, playerNumber)) return;

        if (StageStrikeTracker.Instance.CurrentStrikeInfo.CurrentInteractMode == StrikeInfo.InteractMode.PICK)
        {
            UIScreen.blockGlobalInput = false;
            StageStrikeTracker.Instance.CurrentStrikeInfo.PickStage(this, stage, playerNumber);
        }
        else
        {
            StageStrikeTracker.Instance.CurrentStrikeInfo.BanStage(stage, playerNumber);
            UpdateStageBans();
        }
        
        UpdateSetInfo();
    }

    private void OnVoteFreePick()
    {
        StageStrikeTracker.Instance.CurrentStrikeInfo.ToggleFreePickMode();
        btRandom.enableVoting = !(StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced);
        UpdateStageBans();
        UpdateSetInfo();
    }

    private void OnVoteRandom()
    {
        if (StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.randomMode == Ruleset.RandomMode.OFF) return;
        UIScreen.blockGlobalInput = false;
        StageStrikeTracker.Instance.CurrentStrikeInfo.PickRandomStage(this);
    }
}