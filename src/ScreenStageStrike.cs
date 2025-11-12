using LLScreen;
using UnityEngine;

namespace TourneyMod;

public class ScreenStageStrike
{
    internal static ScreenStageStrike Instance { get; private set; }
    internal bool IsOpen => Instance != null;

    private ScreenPlayersStage screenStage;

    internal static void Open(ScreenPlayersStage screenStage)
    {
        Plugin.LogGlobal.LogInfo("Opening stage strike screen");
        Instance = new ScreenStageStrike(screenStage);
    }

    internal static void Close()
    {
        Plugin.LogGlobal.LogInfo("Closing stage strike screen");
        Instance = null;
    }
    
    private ScreenStageStrike(ScreenPlayersStage screenStage)
    {
        this.screenStage = screenStage;
    }
}