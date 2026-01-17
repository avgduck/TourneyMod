using LLBML.Players;
using LLBML.States;
using LLGUI;
using LLScreen;
using TMPro;
using TourneyMod.SetTracking;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI.Lobby;

internal class ScreenEditScores : ScreenPlayersSettings, ICustomScreen<ScreenPlayersSettings>
{
    private static readonly Vector2 POSITION = new Vector2(0f, 40f);
    private static readonly Vector2 SCALE = new Vector2(400f, 190f);

    private static readonly Vector2 TOP = new Vector2(0f, SCALE.y/2f);
    
    private static readonly Vector2 CLOSE_POSITION = new Vector2(SCALE.x/2f - 10f, SCALE.y/2f - 10f);
    private static readonly Vector2 CLOSE_SCALE = new Vector2(26f, 26f);
    private const int CLOSE_FONTSIZE = 18;

    private static readonly Vector2 SCORE_HEADER_POSITION = new Vector2(TOP.x, TOP.y - 20f);
    private static readonly Vector2 WINNER_HEADER_POSITION = new Vector2(TOP.x, TOP.y - 100f);
    private const int HEADER_FONT_SIZE = 24;
    
    private static readonly Vector2 SCORE_POSITION = new Vector2(TOP.x, TOP.y - 60f);
    private const int SCORE_FONT_SIZE = 52;
    private static readonly Vector2 SCORE_OFFSET = new Vector2(50f, 0f);

    private static readonly Vector2 BTSCORE_SCALE = new Vector2(32f, 32f);
    private const int BTSCORE_FONT_SIZE = 28;
    private static readonly Vector2 BTSCORE_OFFSET0 = new Vector2(0f, 4f);
    private static readonly Vector2 BTSCORE_OFFSET1 = new Vector2(100f, 0f);
    private static readonly Vector2 BTSCORE_OFFSET2 = new Vector2(140f, 0f);

    private static readonly Vector2 WINNER_POSITION = new Vector2(TOP.x, TOP.y - 130f);
    private static readonly Vector2 WINNER_SCALE = new Vector2(80f, 40f);
    private static readonly Vector2 WINNER_OFFSET = new Vector2(WINNER_SCALE.x / 2f, 0f);
    private const int WINNER_FONT_SIZE = 22;
    private static readonly Color WINNER_COLOR_INACTIVE = new Color(0.5f, 0.5f, 0.5f);

    private static readonly Vector2 RESET_POSITION = new Vector2(TOP.x, TOP.y - 170f);
    private static readonly Vector2 RESET_SCALE = new Vector2(150f, 24f);
    private const int RESET_FONT_SIZE = 18;

    internal ScreenLobbyTourney screenLobbyTourney;
    
    private RectTransform pnEditScores;
    private LLButton btClose;

    private TextMeshProUGUI lbScoreHeader;
    
    private TextMeshProUGUI lbScoreOverrideRed;
    private TextMeshProUGUI lbScoreOverrideDash;
    private TextMeshProUGUI lbScoreOverrideBlue;
    
    private LLButton btScoreOverrideUpRed;
    private LLButton btScoreOverrideDownRed;
    private LLButton btScoreOverrideUpBlue;
    private LLButton btScoreOverrideDownBlue;

    private TextMeshProUGUI lbWinnerHeader;
    private Image imgLastWinnerRed;
    private LLButton btLastWinnerRed;
    private Image imgLastWinnerBlue;
    private LLButton btLastWinnerBlue;
    
    private LLButton btResetSetCount;
    
    public void Init(ScreenPlayersSettings screenPlayersSettings)
    {
        screenType = screenPlayersSettings.screenType;
        layer = screenPlayersSettings.layer;
        isActive = screenPlayersSettings.isActive;
        msgEsc = screenPlayersSettings.msgEsc;
        msgMenu = screenPlayersSettings.msgMenu;
        msgCancel = screenPlayersSettings.msgCancel;

        btStocks = screenPlayersSettings.btStocks;
        btTime = screenPlayersSettings.btTime;
        btTag = screenPlayersSettings.btTag;
        btSpeed = screenPlayersSettings.btSpeed;
        btBallType = screenPlayersSettings.btBallType;
        btEnergy = screenPlayersSettings.btEnergy;
        btHpFactor = screenPlayersSettings.btHpFactor;
        btPowerupSelection = screenPlayersSettings.btPowerupSelection;
        btReset = screenPlayersSettings.btReset;
        btBack = screenPlayersSettings.btBack;

        screenLobbyTourney = GameObject.FindObjectOfType<ScreenLobbyTourney>();
    }

    public override void OnOpen(ScreenType screenTypePrev)
    {
        base.OnOpen(screenTypePrev);

        btStocks.visible = false;
        btTime.visible = false;
        btTag.visible = false;
        btSpeed.visible = false;
        btBallType.visible = false;
        btEnergy.visible = false;
        btHpFactor.visible = false;
        btPowerupSelection.visible = false;
        btReset.visible = false;
        btBack.visible = false;
        transform.Find("Panel").gameObject.SetActive(false);

        UIUtils.CreateBorderPanel(ref pnEditScores, "pnEditScores", transform, POSITION, SCALE, Color.yellow, Color.black, 4);

        UIUtils.CreateButton(ref btClose, "btClose", pnEditScores, CLOSE_POSITION, CLOSE_SCALE);
        btClose.textMesh.fontSize = CLOSE_FONTSIZE;
        btClose.SetText("X");
        btClose.onClick = (playerNr) => GameStates.Send(Msg.BACK, playerNr, -1);

        UIUtils.CreateText(ref lbScoreHeader, "lbScoreHeader", pnEditScores, SCORE_HEADER_POSITION);
        lbScoreHeader.fontSize = HEADER_FONT_SIZE;
        lbScoreHeader.SetText("Score override");

        UIUtils.CreateText(ref lbScoreOverrideDash, "lbScoreOverrideDash", pnEditScores, SCORE_POSITION);
        lbScoreOverrideDash.fontSize = SCORE_FONT_SIZE;
        lbScoreOverrideDash.SetText("-");
        UIUtils.CreateText(ref lbScoreOverrideRed, "lbScoreOverrideRed", pnEditScores, SCORE_POSITION - SCORE_OFFSET);
        lbScoreOverrideRed.fontSize = SCORE_FONT_SIZE;
        lbScoreOverrideRed.color = UIUtils.COLOR_TEAM[0];
        UIUtils.CreateText(ref lbScoreOverrideBlue, "lbScoreOverrideBlue", pnEditScores, SCORE_POSITION + SCORE_OFFSET);
        lbScoreOverrideBlue.fontSize = SCORE_FONT_SIZE;
        lbScoreOverrideBlue.color = UIUtils.COLOR_TEAM[1];

        UIUtils.CreateButton(ref btScoreOverrideUpRed, "btScoreOverrideUpRed", pnEditScores, SCORE_POSITION + BTSCORE_OFFSET0 - BTSCORE_OFFSET1, BTSCORE_SCALE);
        btScoreOverrideUpRed.textMesh.fontSize = BTSCORE_FONT_SIZE;
        btScoreOverrideUpRed.SetText("+");
        UIUtils.CreateButton(ref btScoreOverrideDownRed, "btScoreOverrideDownRed", pnEditScores, SCORE_POSITION + BTSCORE_OFFSET0 - BTSCORE_OFFSET2, BTSCORE_SCALE);
        btScoreOverrideDownRed.textMesh.fontSize = BTSCORE_FONT_SIZE;
        btScoreOverrideDownRed.SetText("-");
        UIUtils.CreateButton(ref btScoreOverrideUpBlue, "btScoreOverrideUpBlue", pnEditScores, SCORE_POSITION + BTSCORE_OFFSET0 + BTSCORE_OFFSET1, BTSCORE_SCALE);
        btScoreOverrideUpBlue.textMesh.fontSize = BTSCORE_FONT_SIZE;
        btScoreOverrideUpBlue.SetText("+");
        UIUtils.CreateButton(ref btScoreOverrideDownBlue, "btScoreOverrideDownBlue", pnEditScores, SCORE_POSITION + BTSCORE_OFFSET0 + BTSCORE_OFFSET2, BTSCORE_SCALE);
        btScoreOverrideDownBlue.textMesh.fontSize = BTSCORE_FONT_SIZE;
        btScoreOverrideDownBlue.SetText("-");

        btScoreOverrideUpRed.onClick = (playerNr) =>
        {
            SetTracker.Instance.CurrentSet.AdjustWinCountOverride(Team.RED, 1);
            UpdateScores();
        };
        btScoreOverrideDownRed.onClick = (playerNr) =>
        {
            SetTracker.Instance.CurrentSet.AdjustWinCountOverride(Team.RED, -1);
            UpdateScores();
        };
        btScoreOverrideUpBlue.onClick = (playerNr) =>
        {
            SetTracker.Instance.CurrentSet.AdjustWinCountOverride(Team.BLUE, 1);
            UpdateScores();
        };
        btScoreOverrideDownBlue.onClick = (playerNr) =>
        {
            SetTracker.Instance.CurrentSet.AdjustWinCountOverride(Team.BLUE, -1);
            UpdateScores();
        };
        
        UIUtils.CreateText(ref lbWinnerHeader, "lbWinnerHeader", pnEditScores, WINNER_HEADER_POSITION);
        lbWinnerHeader.fontSize = HEADER_FONT_SIZE;
        lbWinnerHeader.SetText("Last winner override");
        
        UIUtils.CreateImageBorderPanel(ref imgLastWinnerRed, "imgLastWinnerRed", pnEditScores, WINNER_POSITION - WINNER_OFFSET, WINNER_SCALE, WINNER_COLOR_INACTIVE, Color.black, 2);
        UIUtils.CreateButton(ref btLastWinnerRed, "btLastWinnerRed", pnEditScores, WINNER_POSITION - WINNER_OFFSET, WINNER_SCALE, Color.clear);
        btLastWinnerRed.textMesh.fontSize = WINNER_FONT_SIZE;
        btLastWinnerRed.SetText("RED");
        btLastWinnerRed.colDisabled = WINNER_COLOR_INACTIVE;
        
        UIUtils.CreateImageBorderPanel(ref imgLastWinnerBlue, "imgLastWinnerBlue", pnEditScores, WINNER_POSITION + WINNER_OFFSET, WINNER_SCALE, WINNER_COLOR_INACTIVE, Color.black, 2);
        UIUtils.CreateButton(ref btLastWinnerBlue, "btLastWinnerBlue", pnEditScores, WINNER_POSITION + WINNER_OFFSET, WINNER_SCALE, Color.clear);
        btLastWinnerBlue.textMesh.fontSize = WINNER_FONT_SIZE;
        btLastWinnerBlue.SetText("BLUE");
        btLastWinnerBlue.colDisabled = WINNER_COLOR_INACTIVE;

        btLastWinnerRed.onClick = (playerNr) =>
        {
            SetTracker.Instance.CurrentSet.SetLastWinnerOverride(Team.RED);
            UpdateScores();
        };
        btLastWinnerBlue.onClick = (playerNr) =>
        {
            SetTracker.Instance.CurrentSet.SetLastWinnerOverride(Team.BLUE);
            UpdateScores();
        };
        
        UIUtils.CreateButton(ref btResetSetCount, "btResetSetCount", pnEditScores, RESET_POSITION, RESET_SCALE);
        btResetSetCount.textMesh.fontSize = RESET_FONT_SIZE;
        btResetSetCount.SetText("Reset scores");
        btResetSetCount.onClick = (playerNr) =>
        {
            SetTracker.Instance.Reset();
            UpdateScores();
        };
        
        UpdateScores();
    }

    private void UpdateScores()
    {
        screenLobbyTourney.UpdateSetCount();
        screenLobbyTourney.UpdateLockIcons();
        screenLobbyTourney.UpdateStockDisplays();
        
        lbScoreOverrideRed.SetText(SetTracker.Instance.CurrentSet.WinCountOverride[0].ToString());
        lbScoreOverrideBlue.SetText(SetTracker.Instance.CurrentSet.WinCountOverride[1].ToString());

        imgLastWinnerRed.color = SetTracker.Instance.CurrentSet.LastWinnerOverride == Team.RED
            ? UIUtils.COLOR_TEAM[0]
            : WINNER_COLOR_INACTIVE;
        imgLastWinnerBlue.color = SetTracker.Instance.CurrentSet.LastWinnerOverride == Team.BLUE
            ? UIUtils.COLOR_TEAM[1]
            : WINNER_COLOR_INACTIVE;

        int[] winCounts = SetTracker.Instance.CurrentSet.WinCounts;
        btLastWinnerRed.SetActive(winCounts[0] > 0);
        btLastWinnerBlue.SetActive(winCounts[1] > 0);
    }
}