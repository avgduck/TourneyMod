using LLBML.Players;
using LLBML.Settings;
using LLBML.States;
using LLBML.Utils;
using LLHandlers;
using LLScreen;
using TMPro;
using TourneyMod.SetTracking;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI.Lobby;

internal class ScreenLobbyTourney : ScreenLobby
{
    private static readonly Vector2 GAME_POSITION = new Vector2(0f, 250f);
    private const int GAME_FONT_SIZE = 28;
    private static readonly Vector2 SCORE_POSITION = new Vector2(0f, 204f);
    private const int SCORE_FONT_SIZE = 52;
    private static readonly Vector2 SCORE_OFFSET = new Vector2(50f, 0f);
    private static readonly Vector2 BT_EDITSCORES_SCALE = new Vector2(220f, 26f);
    private static readonly Vector2 BT_EDITSCORES_POSITION = new Vector2(0f, -346f);
    private const int BT_EDITSCORES_FONT_SIZE = 18;

    private static readonly Vector2 LOCK_SCALE = new Vector2(128f, 128f);
    private static readonly Vector2 LOCK_POSITION = new Vector2(0f, 60f);
    private static readonly Color LOCK_COLOR = Color.white * 0.95f;

    private static readonly Vector2 STOCKDISPLAY_POSITION = new Vector2(0f, 290f);

    private static readonly Vector2 TIEBREAKER_POSITION = new Vector2(0f, 140f);
    private const int TIEBREAKER_FONT_SIZE = 28;
    
    private TextMeshProUGUI lbGame;
    private TextMeshProUGUI lbScoreRed;
    private TextMeshProUGUI lbScoreDash;
    private TextMeshProUGUI lbScoreBlue;
    
    private VoteButton btEditScores;

    private StockDisplay[] stockDisplays;
    private Image[] imgsCharacterLock;

    private TextMeshProUGUI lbTiebreaker;

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
        
        UIUtils.CreateVoteButton(ref btEditScores, "btEditScores", transform, BT_EDITSCORES_POSITION, BT_EDITSCORES_SCALE);
        VoteButton.ActiveVoteButtons.Add(btEditScores);
        UIUtils.SetButtonBGVisibility(btEditScores, false);
        btEditScores.textMesh.fontSize = BT_EDITSCORES_FONT_SIZE;
        btEditScores.label = "Edit scores";
        btEditScores.onVote = OnVoteEditScores;

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
        
        UIUtils.CreateText(ref lbTiebreaker, "lbTiebreaker", transform, TIEBREAKER_POSITION);
        lbTiebreaker.fontSize = TIEBREAKER_FONT_SIZE;
        lbTiebreaker.richText = true;
        lbTiebreaker.alignment = TextAlignmentOptions.Top;
        
        UpdateSetCount();
    }

    public override void OnClose(ScreenType screenTypeNext)
    {
        VoteButton.ActiveVoteButtons.Remove(btEditScores);
        
        base.OnClose(screenTypeNext);
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
        ShowCpuButtons(false);
        UpdateLockIcons();
        UpdateStockDisplays();
    }
    
    internal void UpdateSetCount()
    {
        int gameNumber = SetTracker.Instance.CurrentSet.GameNumber;
        int[] winCounts = SetTracker.Instance.CurrentSet.WinCounts;
        
        lbGame.SetText($"Game {gameNumber}");
        
        lbScoreRed.SetText(winCounts[0].ToString());
        lbScoreBlue.SetText(winCounts[1].ToString());
        
        lbTiebreaker.SetText(SetTracker.Instance.CurrentSet.IsTiebreaker && SetTracker.Instance.CurrentSet.LastWinnerOverride == Team.NONE ? "Tiebreaker!" + (SetTracker.Instance.CurrentSet.StageLock != Stage.NONE ? $"\n<color=\"yellow\">{StringUtils.GetStageReadableName(SetTracker.Instance.CurrentSet.StageLock)}</color>" : "") : "");
    }

    internal void UpdateStockDisplays()
    {
        Player.ForAll((Player player) =>
        {
            StockDisplay display = stockDisplays[player.nr];

            if (!player.IsInMatch)
            {
                display.rectTransform.gameObject.SetActive(false);
                return;
            }
            
            display.rectTransform.anchoredPosition = playerSelections[player.nr].transform.localPosition + (Vector3)STOCKDISPLAY_POSITION;
            display.rectTransform.gameObject.SetActive(true);

            int maxStocks = GameSettings.current.stocks;
            display.SetMaxStocks(maxStocks);

            if (SetTracker.Instance.CurrentSet.IsGame1 && !SetTracker.Instance.CurrentSet.IsTiebreaker || SetTracker.Instance.CurrentSet.LastWinnerOverride != Team.NONE)
            {
                if (SetTracker.Instance.ActiveTourneyMode is TourneyMode.LOCAL_CREW) display.SetStocks(maxStocks);
                else display.rectTransform.gameObject.SetActive(false);
            }
            else
            {
                int stocksRemaining = SetTracker.Instance.CurrentSet.PlayerStockLock[player.nr];
                if (SetTracker.Instance.ActiveTourneyMode is not TourneyMode.LOCAL_CREW && stocksRemaining < 1) display.rectTransform.gameObject.SetActive(false);
                display.SetStocks(stocksRemaining > 0 ? stocksRemaining : maxStocks);
            }
        });
    }

    internal void UpdateLockIcons()
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
            img.gameObject.SetActive(!SetTracker.Instance.CurrentSet.PlayerCharacterLock[player.nr].IsEmpty && SetTracker.Instance.CurrentSet.LastWinnerOverride == Team.NONE);
        });
    }
    
    private void OnVoteEditScores()
    {
        Plugin.Instance.ScoreEditMenuOpen = true;
        GameStates.Send(Msg.SEL_OPTIONS, -1, -1);
    }
}