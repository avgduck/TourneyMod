using System.Collections.Generic;
using GameplayEntities;
using LLBML.Players;
using LLBML.States;
using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI;

internal class ScreenStageStrikeRanked : ScreenPlayersStageComp, ICustomScreen<ScreenPlayersStageComp>, IStageSelect
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
    private static readonly Vector2 RANDOM_BOTH_OFFSET = new Vector2(60f, 0f);
    private static readonly Vector2 RANDOM_BOTH_SCALE = new Vector2(100f, 34f);
    private const int RANDOM_BOTH_FONT_SIZE = 16;

    private static readonly Vector2 USER_INFO_POSITION = new Vector2(-580f, -250f);
    private static readonly Vector2 OPPONENT_INFO_POSITION = new Vector2(580f, -250f);
    private static readonly Vector2 INFO_SCALE = Vector2.one * 0.65f;
    
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
    private VoteButton btRandomMain;
    private VoteButton btRandomBoth3D;
    private VoteButton btRandomBoth2D;

    private Stage selectedStage = Stage.NONE;
    private Ruleset.RandomMode selectedRandom = Ruleset.RandomMode.OFF;
    
    public void Init(ScreenPlayersStageComp screenPlayersStageComp)
    {
        screenType = screenPlayersStageComp.screenType;
        layer = screenPlayersStageComp.layer;
        isActive = screenPlayersStageComp.isActive;
        msgEsc = screenPlayersStageComp.msgEsc;
        msgMenu = screenPlayersStageComp.msgMenu;
        msgCancel = screenPlayersStageComp.msgCancel;
        
        btBack = screenPlayersStageComp.btBack;
        btLeft = screenPlayersStageComp.btLeft;
        btRight = screenPlayersStageComp.btRight;
        lbTitle = screenPlayersStageComp.lbTitle;
        lbPlayersSelectingStage = screenPlayersStageComp.lbPlayersSelectingStage;
        obSpectator = screenPlayersStageComp.obSpectator;
        obNotSpectator = screenPlayersStageComp.obNotSpectator;
        stageButtonsContainer = screenPlayersStageComp.stageButtonsContainer;
        btStages = screenPlayersStageComp.btStages;
        posMid = screenPlayersStageComp.posMid;
        posLeft = screenPlayersStageComp.posLeft;
        posLeft2 = screenPlayersStageComp.posLeft2;
        posRight = screenPlayersStageComp.posRight;
        posRight2 = screenPlayersStageComp.posRight2;
        scaleSmall = screenPlayersStageComp.scaleSmall;
        scaleBig = screenPlayersStageComp.scaleBig;
        nButtons = screenPlayersStageComp.nButtons;
        curIndex = screenPlayersStageComp.curIndex;
        isMoving = screenPlayersStageComp.isMoving;
        queuedMove = screenPlayersStageComp.queuedMove;

        userInfo = screenPlayersStageComp.userInfo;
        opponentInfo = screenPlayersStageComp.opponentInfo;
        userNameLabel = screenPlayersStageComp.userNameLabel;
        userPlayerRender = screenPlayersStageComp.userPlayerRender;
        userCharacterIcon = screenPlayersStageComp.userCharacterIcon;
        opponentNameLabel = screenPlayersStageComp.opponentNameLabel;
        opponentPlayerRender = screenPlayersStageComp.opponentPlayerRender;
        opponentCharacterIcon = screenPlayersStageComp.opponentCharacterIcon;
    }

    public override void OnOpen(ScreenType screenTypePrev)
    {
        Plugin.LogGlobal.LogInfo("Custom stage select ranked OnOpen");
        StageStrikeTracker.Instance.Start();
        UIScreen.blockGlobalInput = true;
        Plugin.Instance.RecolorCursors = true;
        
        // manually do ScreenBase::OnOpen to avoid going through ScreenPlayersStage::OnOpen
        if (TextHandler.isInited)
        {
            UpdateText();
        }
        // manually do ScreenPlayersStageComp::OnOpen for the same reason
        RankedOnOpen();

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

        Ruleset.RandomMode randomMode = StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.randomMode;
        
        UIUtils.CreateVoteButton(ref btRandomMain, "btRandomMain", transform, RANDOM_POSITION, RANDOM_SCALE);
        VoteButton.ActiveVoteButtons.Add(btRandomMain);
        btRandomMain.label = $"Random {randomMode switch {
            Ruleset.RandomMode.ANY => "(any 3D/2D)",
            Ruleset.RandomMode.ANY_3D => "(any 3D)",
            Ruleset.RandomMode.ANY_2D => "(any 2D)",
            Ruleset.RandomMode.ANY_LEGAL => "(any legal)",
            _ => ""
        }}";
        btRandomMain.textMesh.fontSize = RANDOM_FONT_SIZE;
        btRandomMain.onVote = () => OnVoteRandom(randomMode);
        btRandomMain.enableVoting = !(StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced);
        if (randomMode == Ruleset.RandomMode.OFF || randomMode == Ruleset.RandomMode.BOTH) btRandomMain.gameObject.SetActive(false);
        
        UIUtils.CreateVoteButton(ref btRandomBoth3D, "btRandomBoth3D", transform, RANDOM_POSITION - RANDOM_BOTH_OFFSET, RANDOM_BOTH_SCALE);
        VoteButton.ActiveVoteButtons.Add(btRandomBoth3D);
        btRandomBoth3D.label = $"Random\n(3D)";
        btRandomBoth3D.textMesh.fontSize = RANDOM_BOTH_FONT_SIZE;
        btRandomBoth3D.onVote = () => OnVoteRandom(Ruleset.RandomMode.ANY_3D);
        btRandomBoth3D.enableVoting = !(StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced);
        if (randomMode != Ruleset.RandomMode.BOTH) btRandomBoth3D.gameObject.SetActive(false);
        
        UIUtils.CreateVoteButton(ref btRandomBoth2D, "btRandomBoth2D", transform, RANDOM_POSITION + RANDOM_BOTH_OFFSET, RANDOM_BOTH_SCALE);
        VoteButton.ActiveVoteButtons.Add(btRandomBoth2D);
        btRandomBoth2D.label = $"Random\n(2D)";
        btRandomBoth2D.textMesh.fontSize = RANDOM_BOTH_FONT_SIZE;
        btRandomBoth2D.onVote = () => OnVoteRandom(Ruleset.RandomMode.ANY_2D);
        btRandomBoth2D.enableVoting = !(StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced);
        if (randomMode != Ruleset.RandomMode.BOTH) btRandomBoth2D.gameObject.SetActive(false);

        CreateStageContainers();
        UpdateStageBans();
        UpdateSetInfo();
    }

    private void RankedOnOpen()
    {
        Plugin.LogGlobal.LogInfo("Custom stage select ranked RankedOnOpen");
        ScreenPlayers screenPlayers = GameObject.FindObjectOfType<ScreenPlayers>();
        if (screenPlayers == null) Plugin.LogGlobal.LogFatal("Could not find lobby screen ScreenPlayers");

        Character userCharacter = Player.GetPlayer(0).CharacterSelected;
        userNameLabel = userInfo.Find("lbName").GetComponent<TextMeshProUGUI>();
        userNameLabel.text = screenPlayers.playerSelections[0].btPlayerName.GetText();
        userPlayerRender = userInfo.GetComponentInChildren<RawImage>();
        userPlayerRender.texture = screenPlayers.playerSelections[0].modelPreview.renderTex;
        userPlayerRender.rectTransform.sizeDelta = screenPlayers.playerSelections[0].modelPreview.image.rectTransform.sizeDelta;
        userPlayerRender.rectTransform.anchoredPosition = GetCharacterRenderCustomPosition(userCharacter, out Vector3 userScale, Side.LEFT);
        userPlayerRender.rectTransform.localScale = userScale;
        userCharacterIcon = userInfo.Find("characterIcon").GetComponent<Image>();
        if (userCharacter != Character.RANDOM)
        {
            int symbol = JPLELOFJOOH.LPCPPJOIIEF(userCharacter);
            userCharacterIcon.gameObject.SetActive(true);
            userCharacterIcon.sprite = JPLELOFJOOH.BNFIDCAPPDK("_spriteCharacterSymbols", symbol);
        }
        else
        {
            userCharacterIcon.gameObject.SetActive(false);
        }

        Character opponentCharacter = Player.GetPlayer(1).CharacterSelected;
        opponentNameLabel = opponentInfo.Find("lbName").GetComponent<TextMeshProUGUI>();
        opponentNameLabel.text = screenPlayers.playerSelections[1].btPlayerName.GetText();
        opponentPlayerRender = opponentInfo.GetComponentInChildren<RawImage>();
        opponentPlayerRender.texture = screenPlayers.playerSelections[1].modelPreview.renderTex;
        opponentPlayerRender.rectTransform.sizeDelta = screenPlayers.playerSelections[1].modelPreview.image.rectTransform.sizeDelta;
        opponentPlayerRender.rectTransform.anchoredPosition = GetCharacterRenderCustomPosition(opponentCharacter, out Vector3 opponentScale, Side.RIGHT);
        opponentPlayerRender.rectTransform.localScale = opponentScale;
        opponentCharacterIcon = opponentInfo.Find("characterIcon").GetComponent<Image>();
        if (opponentCharacter != Character.RANDOM)
        {
            int symbol = JPLELOFJOOH.LPCPPJOIIEF(opponentCharacter);
            opponentCharacterIcon.gameObject.SetActive(true);
            opponentCharacterIcon.sprite = JPLELOFJOOH.BNFIDCAPPDK("_spriteCharacterSymbols", symbol);
        }
        else
        {
            opponentCharacterIcon.gameObject.SetActive(false);
        }

        stageButtonsContainer.localPosition = Vector2.zero;
        Transform lbVersus = transform.Find("lbVersus");
        lbVersus.gameObject.SetActive(false);

        userInfo.localPosition = USER_INFO_POSITION;
        userInfo.localScale = INFO_SCALE;
        opponentInfo.localPosition = OPPONENT_INFO_POSITION;
        opponentInfo.localScale = INFO_SCALE;
    }

    public override void OnClose(ScreenType screenTypeNext)
    {
        Plugin.LogGlobal.LogInfo("Custom stage select OnClose");
        StageStrikeTracker.Instance.End();
        UIScreen.blockGlobalInput = false;
        Plugin.Instance.RecolorCursors = false;
        
        VoteButton.ActiveVoteButtons.Remove(btFreePick);
        VoteButton.ActiveVoteButtons.Remove(btRandomMain);
        VoteButton.ActiveVoteButtons.Remove(btRandomBoth3D);
        VoteButton.ActiveVoteButtons.Remove(btRandomBoth2D);
        
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
            
            StageContainer container = new StageContainer(this, stageButtonsContainer, stage);
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
            
            StageContainer container = new StageContainer(this, stageButtonsContainer, stage);
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

    public void OnClickStage(int playerNumber, Stage stage)
    {
        if (!StageStrikeTracker.Instance.CurrentStrikeInfo.CheckPlayerInteraction(stage, playerNumber)) return;

        if (StageStrikeTracker.Instance.CurrentStrikeInfo.CurrentInteractMode == StrikeInfo.InteractMode.PICK)
        {
            UIScreen.blockGlobalInput = false;
            AudioHandler.PlaySfx(Sfx.LOBBY_START_GAME);
            selectedStage = StageStrikeTracker.Instance.CurrentStrikeInfo.PickStage(stage, playerNumber);
        }
        else
        {
            StageStrikeTracker.Instance.CurrentStrikeInfo.BanStage(stage, playerNumber);
            UpdateStageBans();
        }
        
        UpdateSetInfo();
    }

    public void OnStageSelected()
    {
        if (selectedStage == Stage.NONE) return;
        
        TextHandler.SetTextCode(lbTitle, "PLAYER_STAGE_VOTED");
        
        stageContainers.ForEach(container =>
        {
            container.Button.OnHoverOut(-1);
            container.Button.SetActive(false);
            container.Button.UpdateDisplay();
        });
        
        btRandomMain.SetActive(false);
        btRandomBoth3D.SetActive(false);
        btRandomBoth2D.SetActive(false);
        btFreePick.SetActive(false);

        if (selectedRandom == Ruleset.RandomMode.OFF || selectedRandom == Ruleset.RandomMode.BOTH)
        {
            StageContainer container = stageContainers.Find(container => container.StoredStage == selectedStage);
            Plugin.LogGlobal.LogInfo(container);
            if (container == null) return;
            container.Button.Select(true);
            container.Button.UpdateDisplay();
        }
        else
        {
            Ruleset.RandomMode randomMode = StageStrikeTracker.Instance.CurrentStrikeInfo.ActiveRuleset.randomMode;
            if (randomMode == Ruleset.RandomMode.BOTH)
            {
                if (selectedRandom == Ruleset.RandomMode.ANY_3D)
                {
                    btRandomBoth3D.imgBorder.color = Color.white;
                    btRandomBoth3D.colDisabled = btRandomBoth3D.colHover;
                    btRandomBoth3D.UpdateColor();
                }
                else if (selectedRandom == Ruleset.RandomMode.ANY_2D)
                {
                    btRandomBoth2D.imgBorder.color = Color.white;
                    btRandomBoth2D.colDisabled = btRandomBoth2D.colHover;
                    btRandomBoth2D.UpdateColor();
                }
            }
            else
            {
                btRandomMain.imgBorder.color = Color.white;
                btRandomMain.colDisabled = btRandomMain.colHover;
                btRandomMain.UpdateColor();
            }
        }
        
        Plugin.LogGlobal.LogWarning("OnStageSelected");
    }

    private void OnVoteFreePick()
    {
        StageStrikeTracker.Instance.CurrentStrikeInfo.ToggleFreePickMode();
        btRandomMain.enableVoting = !(StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced);
        btRandomBoth3D.enableVoting = !(StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced);
        btRandomBoth2D.enableVoting = !(StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced);
        UpdateStageBans();
        UpdateSetInfo();
    }

    private void OnVoteRandom(Ruleset.RandomMode randomMode)
    {
        if (randomMode == Ruleset.RandomMode.OFF || randomMode == Ruleset.RandomMode.BOTH) return;
        UIScreen.blockGlobalInput = false;
        AudioHandler.PlaySfx(Sfx.LOBBY_START_GAME);
        selectedStage = StageStrikeTracker.Instance.CurrentStrikeInfo.PickRandomStage(randomMode);
        selectedRandom = randomMode;
    }
}