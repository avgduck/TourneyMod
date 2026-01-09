using System.Linq;
using LLBML.Utils;
using TMPro;
using TourneyMod.SetTracking;
using UnityEngine;

namespace TourneyMod.UI.Menu;

internal class SetPreviewWindow
{
    private static readonly Vector2 SETPREVIEW_SCALE = new Vector2(380f, 160f);
    private const int HEADER_FONTSIZE = 22;
    private const int MAIN_FONTSIZE = 16;

    private static readonly Vector2 TOP = new Vector2(106f, -26f);
    private static readonly Vector2 HEADER_LINE_SPACING = new Vector2(0f, -HEADER_FONTSIZE);
    private static readonly Vector2 MAIN_LINE_SPACING = new Vector2(0f, -MAIN_FONTSIZE);

    private RectTransform rectTransform;

    private TextMeshProUGUI lbActiveSet;
    private TextMeshProUGUI lbRuleset;
    private TextMeshProUGUI lbScore;

    private TextMeshProUGUI lbLastMatchHeader;
    private TextMeshProUGUI lbWinner;
    private TextMeshProUGUI lbCharacterLock;
    private TextMeshProUGUI lbStage;
    private TextMeshProUGUI lbStocksRemaining;

    private Match lastMatch;
    
    internal static void Create(ref SetPreviewWindow pnSetPreview, Transform parent, Vector2 position)
    {
        pnSetPreview = new SetPreviewWindow();
        UIUtils.CreatePanel(ref pnSetPreview.rectTransform, "pnSetPreview", parent, position, SETPREVIEW_SCALE);
        pnSetPreview.Init();
    }

    private void Init()
    {
        UIUtils.CreateText(ref lbActiveSet, "lbActiveSet", rectTransform, TOP);
        lbActiveSet.fontSize = HEADER_FONTSIZE;
        lbActiveSet.alignment = TextAlignmentOptions.TopLeft;
        lbActiveSet.richText = true;
        
        UIUtils.CreateText(ref lbRuleset, "lbRuleset", rectTransform, TOP + HEADER_LINE_SPACING);
        lbRuleset.fontSize = MAIN_FONTSIZE;
        lbRuleset.alignment = TextAlignmentOptions.TopLeft;
        lbRuleset.richText = true;
        
        UIUtils.CreateText(ref lbScore, "lbScore", rectTransform, TOP + HEADER_LINE_SPACING + MAIN_LINE_SPACING);
        lbScore.fontSize = MAIN_FONTSIZE;
        lbScore.alignment = TextAlignmentOptions.TopLeft;
        lbScore.richText = true;
        
        UIUtils.CreateText(ref lbLastMatchHeader, "lbLastMatchHeader", rectTransform, TOP + HEADER_LINE_SPACING + MAIN_LINE_SPACING * 3f);
        lbLastMatchHeader.fontSize = HEADER_FONTSIZE;
        lbLastMatchHeader.alignment = TextAlignmentOptions.TopLeft;
        lbLastMatchHeader.richText = true;
        
        UIUtils.CreateText(ref lbWinner, "lbWinner", rectTransform, TOP + HEADER_LINE_SPACING * 2f + MAIN_LINE_SPACING * 3f);
        lbWinner.fontSize = MAIN_FONTSIZE;
        lbWinner.alignment = TextAlignmentOptions.TopLeft;
        lbWinner.richText = true;
        
        UIUtils.CreateText(ref lbCharacterLock, "lbCharacterLock", rectTransform, TOP + HEADER_LINE_SPACING * 2f + MAIN_LINE_SPACING * 4f);
        lbCharacterLock.fontSize = MAIN_FONTSIZE;
        lbCharacterLock.alignment = TextAlignmentOptions.TopLeft;
        lbCharacterLock.richText = true;
        
        UIUtils.CreateText(ref lbStage, "lbStage", rectTransform, TOP + HEADER_LINE_SPACING * 2f + MAIN_LINE_SPACING * 5f);
        lbStage.fontSize = MAIN_FONTSIZE;
        lbStage.alignment = TextAlignmentOptions.TopLeft;
        lbStage.richText = true;
        
        UIUtils.CreateText(ref lbStocksRemaining, "lbStocksRemaining", rectTransform, TOP + HEADER_LINE_SPACING * 2f + MAIN_LINE_SPACING * 6f);
        lbStocksRemaining.fontSize = MAIN_FONTSIZE;
        lbStocksRemaining.alignment = TextAlignmentOptions.TopLeft;
        lbStocksRemaining.richText = true;
        
        UpdateText();
    }

    internal void UpdateText()
    {
        lbActiveSet.SetText($"Active set: <color=\"{(SetTracker.Instance.ActiveTourneyMode == TourneyMode.NONE ? "red" : "green")}\">{Plugin.GetModeName(SetTracker.Instance.ActiveTourneyMode)}</color>");
        
        lbRuleset.SetText($"Ruleset: <color=\"yellow\">{(SetTracker.Instance.IsTrackingSet ? SetTracker.Instance.CurrentSet.ActiveRuleset.name : "")}</color>");
        
        lbScore.SetText($"Score: <color=\"yellow\">{(SetTracker.Instance.IsTrackingSet ? $"Game {SetTracker.Instance.CurrentSet.GameNumber}, <color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[0])}>{SetTracker.Instance.CurrentSet.WinCounts[0]}</color>-<color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[1])}>{SetTracker.Instance.CurrentSet.WinCounts[1]}</color>" : "")}</color>");

        lbLastMatchHeader.SetText("Last completed match");

        if (SetTracker.Instance.IsTrackingSet && !SetTracker.Instance.CurrentSet.IsGame1) lastMatch = SetTracker.Instance.CurrentSet.CompletedMatches.Last();
        else lastMatch = null;
        
        lbWinner.SetText("Winner: " + (lastMatch != null
            ? $"<color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[(int)lastMatch.Winner])}>{lastMatch.Winner}</color>"
            : ""));
        
        lbCharacterLock.SetText("Character: <color=\"yellow\">" + (lastMatch != null
            ? Plugin.PrintArray(SetTracker.Instance.CurrentSet.PlayerCharacterLock.ToList().Where(pc => !pc.IsEmpty).Select(pc => StringUtils.GetCharacterSafeName(pc.character)).ToArray(), false)
            : "") + "</color>");
        
        lbStage.SetText("Stage: <color=\"yellow\">" + (lastMatch != null
            ? StringUtils.GetStageReadableName(lastMatch.PlayedStage)
            : "") + "</color>");
        
        lbStocksRemaining.SetText("Stocks: <color=\"yellow\">" + (lastMatch != null
            ? Plugin.PrintArray(lastMatch.FinalScores.Where(ps => ps.Team == lastMatch.Winner).Select(ps => ps.Stocks).ToArray(), false)
            : "") + "</color>");
    }
}