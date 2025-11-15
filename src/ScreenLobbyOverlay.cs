using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using UnityEngine;

namespace TourneyMod;

public class ScreenLobbyOverlay
{
    internal static ScreenLobbyOverlay Instance { get; private set; }
    internal static bool IsOpen => Instance != null;
    
    private ScreenPlayers screenPlayers;
    private TMP_Text lbGame;
    private TMP_Text lbSetCount;
    private LLButton btResetSetCount;

    //private static readonly Vector2 SETCOUNT_POSITION = new Vector2(0f, 280f);
    //private static readonly Vector2 SETCOUNT_POSITION = new Vector2(0f, 220f);
    private static readonly Vector2 GAME_POSITION = new Vector2(0f, 110f);
    private const int GAME_FONT_SIZE = 28;
    private static readonly Vector2 SETCOUNT_POSITION = new Vector2(0f, 64f);
    private const int SETCOUNT_FONT_SIZE = 52;
    private static readonly Vector2 RESET_SCALE = new Vector2(1f, 1f);
    private static readonly Vector2 RESET_POSITION = new Vector2(0f, 344f);
    private const int RESET_FONT_SIZE = 18;

    private bool[] resetVotes;

    internal static void Open()
    {
        Plugin.LogGlobal.LogInfo("Opening lobby overlay screen");
        Instance = new ScreenLobbyOverlay();
    }

    internal static void Close()
    {
        Plugin.LogGlobal.LogInfo("Closing lobby overlay screen");
        Instance = null;
    }
    
    private TMP_Text CreateNewText(string name, Transform parent)
    {
        TMP_Text text = Object.Instantiate(screenPlayers.lbStocks, parent);
        text.gameObject.name = name;
        text.SetText("");
        text.color = Color.white;
        text.fontSize = 32;
        text.transform.localScale = Vector3.one;
        text.transform.localPosition = Vector3.zero;
        text.alignment = TextAlignmentOptions.Center;
        return text;
    }

    private LLButton CreateNewButton(string name, Transform parent)
    {
        LLButton button = Object.Instantiate(screenPlayers.playerSelections[0].btPlayerName, parent);
        button.gameObject.name = name;
        button.SetText("");
        button.textMesh.color = Color.white;
        button.transform.localScale = Vector3.one;
        button.transform.localPosition = Vector3.zero;
        button.textMesh.transform.localPosition = Vector3.zero;
        button.textMesh.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        return button;
    }

    internal void OnOpen(ScreenPlayers screenPlayers)
    {
        this.screenPlayers = screenPlayers;

        lbGame = CreateNewText("lbGame", screenPlayers.transform);
        lbGame.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
        lbGame.rectTransform.localPosition = GAME_POSITION;
        lbGame.fontSize = GAME_FONT_SIZE;
        
        lbSetCount = CreateNewText("lbSetCount", screenPlayers.transform);
        lbSetCount.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
        lbSetCount.rectTransform.localPosition = SETCOUNT_POSITION;
        lbSetCount.fontSize = SETCOUNT_FONT_SIZE;

        resetVotes = [false, false];
        btResetSetCount = CreateNewButton("btResetSetCount", screenPlayers.transform);
        btResetSetCount.transform.localPosition = RESET_POSITION;
        btResetSetCount.transform.localScale = RESET_SCALE;
        btResetSetCount.textMesh.rectTransform.localScale = new Vector2(1f / RESET_SCALE.x, 1f / RESET_SCALE.y);
        btResetSetCount.textMesh.fontSize = RESET_FONT_SIZE;
        btResetSetCount.SetText("Reset set count 0/2");
        btResetSetCount.onClick = (playerNumber) =>
        {
            OnResetClicked(playerNumber);
        };
        
        UpdateSetCount();
    }

    private void UpdateSetCount()
    {
        int gameNumber = SetTracker.Instance.GetGameNumber();
        int[] winCounts = SetTracker.Instance.GetWinCounts();
        TextHandler.SetText(lbGame, $"Game {gameNumber}");
        TextHandler.SetText(lbSetCount, $"({winCounts[0]}-{winCounts[1]})");
        
        int sum = 0;
        foreach (bool vote in resetVotes)
        {
            if (vote) sum++;
        }
        btResetSetCount.SetText($"Reset set count {sum}/2");
    }

    private void OnResetClicked(int playerNumber)
    {
        resetVotes[playerNumber] = true;
        
        int sum = 0;
        foreach (bool vote in resetVotes)
        {
            if (vote) sum++;
        }
        
        if (sum >= 2)
        {
            resetVotes = [false, false];
            SetTracker.Instance.ResetSetCount();
        }
        
        UpdateSetCount();
    }
}