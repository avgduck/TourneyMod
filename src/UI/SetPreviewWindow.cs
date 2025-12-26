using TMPro;
using TourneyMod.SetTracking;
using UnityEngine;

namespace TourneyMod.UI;

internal class SetPreviewWindow
{
    private static readonly Vector2 SETPREVIEW_SCALE = new Vector2(380f, 58f);
    private const int HEADER_FONTSIZE = 22;
    private const int MAIN_FONTSIZE = 16;

    private static readonly Vector2 TOP = new Vector2(106f, -26f);
    private static readonly Vector2 HEADER_LINE_SPACING = new Vector2(0f, -HEADER_FONTSIZE);
    private static readonly Vector2 MAIN_LINE_SPACING = new Vector2(0f, -MAIN_FONTSIZE);

    private RectTransform rectTransform;

    private TextMeshProUGUI lbActiveSet;
    private TextMeshProUGUI lbRuleset;
    private TextMeshProUGUI lbScore;
    
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
        
        UIUtils.CreateText(ref lbScore, "lbScore", rectTransform, TOP + HEADER_LINE_SPACING + MAIN_LINE_SPACING);
        lbScore.fontSize = MAIN_FONTSIZE;
        lbScore.alignment = TextAlignmentOptions.TopLeft;
        
        UpdateText();
    }

    internal void UpdateText()
    {
        lbActiveSet.SetText($"Active set: <color=\"{(SetTracker.Instance.ActiveTourneyMode == TourneyMode.NONE ? "red" : "green")}\">{Plugin.GetModeName(SetTracker.Instance.ActiveTourneyMode)}</color>");
        lbRuleset.SetText($"Ruleset: <color=\"yellow\">{(SetTracker.Instance.IsTrackingSet ? SetTracker.Instance.CurrentSet.ActiveRuleset.name : "")}</color>");
        lbScore.SetText($"Score: <color=\"yellow\">{(SetTracker.Instance.IsTrackingSet ? $"Game {SetTracker.Instance.CurrentSet.GameNumber}, <color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_PLAYER[0])}>{SetTracker.Instance.CurrentSet.WinCounts[0]}</color>-<color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_PLAYER[1])}>{SetTracker.Instance.CurrentSet.WinCounts[1]}</color>" : "")}</color>");
    }
}