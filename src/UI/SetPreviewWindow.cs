using TMPro;
using TourneyMod.SetTracking;
using UnityEngine;

namespace TourneyMod.UI;

internal class SetPreviewWindow
{
    private static readonly Vector2 SETPREVIEW_SCALE = new Vector2(320f, 50f);
    private const int HEADER_FONTSIZE = 22;
    private const int MAIN_FONTSIZE = 16;

    private const float TOP = 10f;
    private static readonly Vector2 LEFTCOL = new Vector2(-320f, TOP);
    private static readonly Vector2 RIGHTCOL = new Vector2(210f, TOP);
    private static readonly Vector2 HEADER_LINE_SPACING = new Vector2(0f, -HEADER_FONTSIZE);
    private static readonly Vector2 MAIN_LINE_SPACING = new Vector2(0f, -MAIN_FONTSIZE);
    
    private static readonly Vector2 ACTIVESET_POSITION_1 = new Vector2(-280f, TOP);
    private static readonly Vector2 ACTIVESET_POSITION_2 = new Vector2(250f, TOP);

    private RectTransform rectTransform;

    private TextMeshProUGUI lbActiveSet1;
    private TextMeshProUGUI lbActiveSet2;

    private TextMeshProUGUI lbScore1;
    private TextMeshProUGUI lbScore2;
    
    internal static void Create(ref SetPreviewWindow pnSetPreview, Transform parent, Vector2 position)
    {
        pnSetPreview = new SetPreviewWindow();
        UIUtils.CreatePanel(ref pnSetPreview.rectTransform, "pnSetPreview", parent, position, SETPREVIEW_SCALE);
        pnSetPreview.Init();
    }

    private void Init()
    {
        UIUtils.CreateText(ref lbActiveSet1, "lbActiveSet1", rectTransform, ACTIVESET_POSITION_1);
        lbActiveSet1.fontSize = HEADER_FONTSIZE;
        lbActiveSet1.alignment = TextAlignmentOptions.Right;
        lbActiveSet1.SetText("Active set:");
        UIUtils.CreateText(ref lbActiveSet2, "lbActiveSet2", rectTransform, ACTIVESET_POSITION_2);
        lbActiveSet2.fontSize = HEADER_FONTSIZE;
        lbActiveSet2.alignment = TextAlignmentOptions.Left;
        lbActiveSet2.color = Color.red;
        lbActiveSet2.SetText("");
        
        UIUtils.CreateText(ref lbScore1, "lbScore1", rectTransform, LEFTCOL + HEADER_LINE_SPACING + MAIN_LINE_SPACING*0);
        lbScore1.fontSize = MAIN_FONTSIZE;
        lbScore1.alignment = TextAlignmentOptions.Right;
        lbScore1.SetText("Set count:");
        UIUtils.CreateText(ref lbScore2, "lbScore2", rectTransform, RIGHTCOL + HEADER_LINE_SPACING + MAIN_LINE_SPACING*0);
        lbScore2.fontSize = MAIN_FONTSIZE;
        lbScore2.alignment = TextAlignmentOptions.Left;
        lbScore2.color = Color.yellow;
        lbScore2.SetText("");
        
        UpdateText();
    }

    internal void UpdateText()
    {
        lbActiveSet2.color = SetTracker.Instance.ActiveTourneyMode == TourneyMode.NONE ? Color.red : Color.green;
        lbActiveSet2.SetText(SetTracker.Instance.ActiveTourneyMode switch
        {
            TourneyMode.LOCAL_1V1 => "local 1v1",
            TourneyMode.LOCAL_DOUBLES => "local doubles",
            TourneyMode.LOCAL_CREW => "crew battle",
            TourneyMode.ONLINE_1V1 => "online 1v1",
            _ => "none"
        });
        
        lbScore2.SetText(SetTracker.Instance.IsTrackingSet ? $"Game {SetTracker.Instance.CurrentSet.GameNumber}, {SetTracker.Instance.CurrentSet.WinCounts[0]}-{SetTracker.Instance.CurrentSet.WinCounts[1]}" : "");
    }
}