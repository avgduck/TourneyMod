using HarmonyLib;
using LLHandlers;
using LLScreen;
using TMPro;
using TourneyMod.SetTracking;
using TourneyMod.UI;
using UnityEngine;

namespace TourneyMod.Patches;

public class GameHudPatch
{
    private static readonly Vector2 SCORE_POSITION = new Vector2(0f, 22f);
    private const int SCORE_FONT_SIZE = 18;
    private static readonly Vector2 SCORE_OFFSET = new Vector2(41f, 0f);
    
    [HarmonyPatch(typeof(ScreenGameHud), nameof(ScreenGameHud.OnOpen))]
    [HarmonyPostfix]
    private static void ScreenGameHud_OnOpen_Postfix(ScreenGameHud __instance)
    {
        if (!SetTracker.Instance.IsTrackingSet) return;
        if (SetTracker.Instance.ActiveTourneyMode is TourneyMode.NONE) return;
        
        TextMeshProUGUI lbScoreRed = null;
        UIUtils.CreateText(ref lbScoreRed, "lbScoreRed", __instance.rtBoomBox, SCORE_POSITION - SCORE_OFFSET);
        lbScoreRed.fontSize = SCORE_FONT_SIZE;
        TextMeshProUGUI lbScoreBlue = null;
        UIUtils.CreateText(ref lbScoreBlue, "lbScoreBlue", __instance.rtBoomBox, SCORE_POSITION + SCORE_OFFSET);
        lbScoreBlue.fontSize = SCORE_FONT_SIZE;

        int[] winCounts = SetTracker.Instance.CurrentSet.WinCounts;
        TextHandler.SetText(lbScoreRed, winCounts[0].ToString());
        TextHandler.SetText(lbScoreBlue, winCounts[1].ToString());
    }
}