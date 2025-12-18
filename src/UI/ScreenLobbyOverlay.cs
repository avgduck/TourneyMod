using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using UnityEngine;

namespace TourneyMod.UI;

public class ScreenLobbyOverlay
{
    internal static ScreenLobbyOverlay Instance { get; private set; }
    internal static bool IsActive => Instance != null;
    internal bool IsOpen { get; private set; }
    
    private ScreenPlayers screenPlayers;
    private TextMeshProUGUI lbGame;
    private TextMeshProUGUI lbSetCount;
    private LLButton btResetSetCount;

    private static readonly Vector2 GAME_POSITION = new Vector2(0f, 250f);
    private const int GAME_FONT_SIZE = 28;
    private static readonly Vector2 SETCOUNT_POSITION = new Vector2(0f, 204f);
    private const int SETCOUNT_FONT_SIZE = 52;
    private static readonly Vector2 RESET_SCALE = new Vector2(220f, 26f);
    private static readonly Vector2 RESET_POSITION = new Vector2(0f, 346f);
    private const int RESET_FONT_SIZE = 18;

    private bool[] resetVotes;

    internal static void Open()
    {
        //Plugin.LogGlobal.LogInfo("Opening lobby overlay screen");
        Instance = new ScreenLobbyOverlay();
    }

    internal static void Close()
    {
        //Plugin.LogGlobal.LogInfo("Closing lobby overlay screen");
        Instance = null;
    }

    internal void OnOpen(ScreenPlayers screenPlayers)
    {
        if (Plugin.Instance.ActiveTourneyMode == TourneyMode.NONE)
        {
            //Plugin.LogGlobal.LogInfo("Tourney mode not active! Hiding lobby overlay set count");
            return;
        }
        
        this.screenPlayers = screenPlayers;

        UI.CreateText(ref lbGame, "lbGame", screenPlayers.transform, GAME_POSITION);
        lbGame.fontSize = GAME_FONT_SIZE;
        
        UI.CreateText(ref lbSetCount, "lbSetCount", screenPlayers.transform, SETCOUNT_POSITION);
        lbSetCount.fontSize = SETCOUNT_FONT_SIZE;

        resetVotes = [false, false, false, false];
        UI.CreateButton(ref btResetSetCount, "btResetSetCount", screenPlayers.transform, RESET_POSITION, RESET_SCALE);
        UI.SetButtonBGVisibility(btResetSetCount, false);
        btResetSetCount.textMesh.fontSize = RESET_FONT_SIZE;
        btResetSetCount.SetText("Reset set count 0/0");
        btResetSetCount.onClick = (playerNumber) =>
        {
            OnResetClicked(playerNumber);
        };
        
        UpdateSetCount();
        IsOpen = true;
    }

    internal void UpdateSetCount()
    {
        int gameNumber = SetTracker.Instance.CurrentSet.GameNumber;
        int[] winCounts = SetTracker.Instance.CurrentSet.WinCounts;
        TextHandler.SetText(lbGame, $"Game {gameNumber}");
        TextHandler.SetText(lbSetCount, $"({winCounts[0]}-{winCounts[1]})");
        
        int sum = 0;
        foreach (bool vote in resetVotes)
        {
            if (vote) sum++;
        }
        btResetSetCount.SetText($"Reset set count {sum}/{SetTracker.Instance.NumPlayersInMatch}");
    }

    private void OnResetClicked(int playerNumber)
    {
        resetVotes[playerNumber] = true;
        
        int sum = 0;
        foreach (bool vote in resetVotes)
        {
            if (vote) sum++;
        }
        
        if (sum >= SetTracker.Instance.NumPlayersInMatch)
        {
            resetVotes = [false, false, false, false];
            SetTracker.Instance.Reset();
            
            if (StageStrikeTracker.Instance.IsTrackingStrikeInfo) StageStrikeTracker.Instance.Reset();
        }
        
        UpdateSetCount();
    }
}