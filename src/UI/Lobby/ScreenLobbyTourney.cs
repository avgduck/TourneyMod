using System.Linq;
using LLBML.Players;
using LLBML.Settings;
using LLScreen;
using TMPro;
using TourneyMod.SetTracking;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI.Lobby;

internal class ScreenLobbyTourney : ScreenPlayers, ICustomScreen<ScreenPlayers>
{
    private static readonly Vector2 GAME_POSITION = new Vector2(0f, 250f);
    private const int GAME_FONT_SIZE = 28;
    private static readonly Vector2 SCORE_POSITION = new Vector2(0f, 204f);
    private const int SCORE_FONT_SIZE = 52;
    private static readonly Vector2 SCORE_OFFSET = new Vector2(50f, 0f);
    private static readonly Vector2 RESET_SCALE = new Vector2(220f, 26f);
    private static readonly Vector2 RESET_POSITION = new Vector2(0f, -346f);
    private const int RESET_FONT_SIZE = 18;

    private static readonly Vector2 LOCK_SCALE = new Vector2(128f, 128f);
    private static readonly Vector2 LOCK_POSITION = new Vector2(0f, 60f);
    private static readonly Color LOCK_COLOR = Color.white * 0.95f;

    private static readonly Vector2 STOCKDISPLAY_POSITION = new Vector2(0f, 290f);
    
    private TextMeshProUGUI lbGame;
    private TextMeshProUGUI lbScoreRed;
    private TextMeshProUGUI lbScoreDash;
    private TextMeshProUGUI lbScoreBlue;
    
    private VoteButton btResetSetCount;

    private StockDisplay[] stockDisplays;
    private Image[] imgsCharacterLock;
    
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
        
        UIUtils.CreateText(ref lbGame, "lbGame", transform, GAME_POSITION);
        lbGame.fontSize = GAME_FONT_SIZE;
        
        UIUtils.CreateText(ref lbScoreDash, "lbScoreDash", transform, SCORE_POSITION);
        lbScoreDash.fontSize = SCORE_FONT_SIZE;
        lbScoreDash.SetText("-");
        UIUtils.CreateText(ref lbScoreRed, "lbScoreRed", transform, SCORE_POSITION - SCORE_OFFSET);
        lbScoreRed.fontSize = SCORE_FONT_SIZE;
        lbScoreRed.color = UIUtils.COLOR_TEAM[0];
        UIUtils.CreateText(ref lbScoreBlue, "lbScoreBlue", transform, SCORE_POSITION + SCORE_OFFSET);
        lbScoreBlue.fontSize = SCORE_FONT_SIZE;
        lbScoreBlue.color = UIUtils.COLOR_TEAM[1];

        UIUtils.CreateVoteButton(ref btResetSetCount, "btResetSetCount", transform, RESET_POSITION, RESET_SCALE);
        VoteButton.ActiveVoteButtons.Add(btResetSetCount);
        UIUtils.SetButtonBGVisibility(btResetSetCount, false);
        btResetSetCount.textMesh.fontSize = RESET_FONT_SIZE;
        btResetSetCount.label = "Reset set count";
        btResetSetCount.onVote = OnVoteReset;

        stockDisplays = new StockDisplay[4];
        imgsCharacterLock = new Image[4];
        for (int playerNr = 0; playerNr < 4; playerNr++)
        {
            StockDisplay.Create(ref stockDisplays[playerNr], GameSettings.current.stocks, "stockDisplay" + playerNr, transform, Vector2.zero, Vector2.one);
            stockDisplays[playerNr].rectTransform.gameObject.SetActive(false);
            
            UIUtils.CreateImage(ref imgsCharacterLock[playerNr], UIUtils.spriteLock, "imgLock", transform, Vector2.zero, LOCK_SCALE);
            imgsCharacterLock[playerNr].color = LOCK_COLOR;
            imgsCharacterLock[playerNr].gameObject.SetActive(false);
        }
        
        UpdateSetCount();
    }

    public override void OnClose(ScreenType screenTypeNext)
    {
        VoteButton.ActiveVoteButtons.Remove(btResetSetCount);
        
        base.OnClose(screenTypeNext);
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
        ShowCpuButtons(false);
        UpdateLockIcons();
        UpdateStockDisplays();
    }
    
    private void UpdateSetCount()
    {
        int gameNumber = SetTracker.Instance.CurrentSet.GameNumber;
        int[] winCounts = SetTracker.Instance.CurrentSet.WinCounts;
        
        lbGame.SetText($"Game {gameNumber}");
        
        lbScoreRed.SetText(winCounts[0].ToString());
        lbScoreBlue.SetText(winCounts[1].ToString());
    }

    private void UpdateStockDisplays()
    {
        Player.ForAll((Player player) =>
        {
            StockDisplay display = stockDisplays[player.nr];

            if (!player.IsInMatch || SetTracker.Instance.ActiveTourneyMode is not TourneyMode.LOCAL_CREW)
            {
                display.rectTransform.gameObject.SetActive(false);
                return;
            }
            
            display.rectTransform.anchoredPosition = playerSelections[player.nr].transform.localPosition + (Vector3)STOCKDISPLAY_POSITION;

            int maxStocks = GameSettings.current.stocks;
            display.SetMaxStocks(maxStocks);

            if (SetTracker.Instance.CurrentSet.IsGame1)
            {
                display.SetStocks(maxStocks);
            }
            else
            {
                int stocksRemaining = SetTracker.Instance.CurrentSet.CompletedMatches.Last().FinalScores[player.nr].Stocks;
                display.SetStocks(stocksRemaining > 0 ? stocksRemaining : maxStocks);
            }
            display.rectTransform.gameObject.SetActive(true);
        });
    }

    private void UpdateLockIcons()
    {
        Player.ForAll((Player player) =>
        {
            Image img = imgsCharacterLock[player.nr];

            if (!player.IsInMatch)
            {
                img.gameObject.SetActive(false);
                return;
            }
            
            img.rectTransform.anchoredPosition = playerSelections[player.nr].transform.localPosition + (Vector3)LOCK_POSITION;
            img.gameObject.SetActive(!SetTracker.Instance.CurrentSet.PlayerCharacterLock[player.nr].IsEmpty);
        });
    }
    
    private void OnVoteReset()
    {
        SetTracker.Instance.Reset();
        UpdateSetCount();
    }
}