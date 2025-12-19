using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using UnityEngine;

namespace TourneyMod.UI;

internal class ScreenLobbyTourney : ScreenPlayers, ICustomScreen<ScreenPlayers>
{
    private static readonly Vector2 GAME_POSITION = new Vector2(0f, 250f);
    private const int GAME_FONT_SIZE = 28;
    private static readonly Vector2 SETCOUNT_POSITION = new Vector2(0f, 204f);
    private const int SETCOUNT_FONT_SIZE = 52;
    private static readonly Vector2 RESET_SCALE = new Vector2(220f, 26f);
    private static readonly Vector2 RESET_POSITION = new Vector2(0f, 346f);
    private const int RESET_FONT_SIZE = 18;
    
    private TextMeshProUGUI lbGame;
    private TextMeshProUGUI lbSetCount;
    private VoteButton btResetSetCount;
    
    public void Init(ScreenPlayers screenPlayers)
    {
        screenType = screenPlayers.screenType;
        layer = screenPlayers.layer;
        isActive = screenPlayers.isActive;
        msgEsc = screenPlayers.msgEsc;
        msgMenu = screenPlayers.msgMenu;
        msgCancel = screenPlayers.msgCancel;
        
        btBack = screenPlayers.btBack;
        btStart = screenPlayers.btStart;
        characterButtons = screenPlayers.characterButtons;
        playerSelections = screenPlayers.playerSelections;
        lbCharInfo = screenPlayers.lbCharInfo;
        lbCountdown = screenPlayers.lbCountdown;
        lbGameModeHeader = screenPlayers.lbGameModeHeader;
        lbGameMode = screenPlayers.lbGameMode;
        btGameMode = screenPlayers.btGameMode;
        btOptions = screenPlayers.btOptions;
        btInviteFriends = screenPlayers.btInviteFriends;
        lbStocksHeader = screenPlayers.lbStocksHeader;
        lbTimeHeader = screenPlayers.lbTimeHeader;
        lbSpeedHeader = screenPlayers.lbSpeedHeader;
        lbBallTypeHeader = screenPlayers.lbBallTypeHeader;
        lbEnergyHeader = screenPlayers.lbEnergyHeader;
        lbHpFactorHeader = screenPlayers.lbHpFactorHeader;
        lbPowerupSelectionHeader = screenPlayers.lbPowerupSelectionHeader;
        lbStocks = screenPlayers.lbStocks;
        lbTime = screenPlayers.lbTime;
        lbSpeed = screenPlayers.lbSpeed;
        lbBallType = screenPlayers.lbBallType;
        lbEnergy = screenPlayers.lbEnergy;
        lbHpFactor = screenPlayers.lbHpFactor;
        lbPowerupSelection = screenPlayers.lbPowerupSelection;
        obSettings = screenPlayers.obSettings;
        pfPlayerSelection = screenPlayers.pfPlayerSelection;
        pfCharacterButton = screenPlayers.pfCharacterButton;
        pnCharacterButtons = screenPlayers.pnCharacterButtons;
        pnPlayers = screenPlayers.pnPlayers;
        curCountDown = screenPlayers.curCountDown;
        kCountDown = screenPlayers.kCountDown;
        countDownState = screenPlayers.countDownState;
        nPlayersShown = screenPlayers.nPlayersShown;
    }

    public override void OnOpen(ScreenType screenTypePrev)
    {
        base.OnOpen(screenTypePrev);
        Plugin.LogGlobal.LogInfo("Custom tourney lobby OnOpen");
        
        UIUtils.CreateText(ref lbGame, "lbGame", transform, GAME_POSITION);
        lbGame.fontSize = GAME_FONT_SIZE;
        
        UIUtils.CreateText(ref lbSetCount, "lbSetCount", transform, SETCOUNT_POSITION);
        lbSetCount.fontSize = SETCOUNT_FONT_SIZE;

        UIUtils.CreateVoteButton(ref btResetSetCount, "btResetSetCount", transform, RESET_POSITION, RESET_SCALE);
        VoteButton.ActiveVoteButtons.Add(btResetSetCount);
        UIUtils.SetButtonBGVisibility(btResetSetCount, false);
        btResetSetCount.textMesh.fontSize = RESET_FONT_SIZE;
        btResetSetCount.label = "Reset set count";
        btResetSetCount.onVote = OnVoteReset;
        
        UpdateSetCount();
    }

    public override void OnClose(ScreenType screenTypeNext)
    {
        VoteButton.ActiveVoteButtons.Remove(btResetSetCount);
        
        base.OnClose(screenTypeNext);
        Plugin.LogGlobal.LogInfo("Custom tourney lobby OnClose");
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
        ShowCpuButtons(false);
    }
    
    private void UpdateSetCount()
    {
        int gameNumber = SetTracker.Instance.CurrentSet.GameNumber;
        int[] winCounts = SetTracker.Instance.CurrentSet.WinCounts;
        TextHandler.SetText(lbGame, $"Game {gameNumber}");
        TextHandler.SetText(lbSetCount, $"({winCounts[0]}-{winCounts[1]})");
    }
    
    private void OnVoteReset()
    {
        SetTracker.Instance.Reset();
        UpdateSetCount();
    }
}