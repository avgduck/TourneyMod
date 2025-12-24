using TMPro;
using TourneyMod.SetTracking;
using UnityEngine;

namespace TourneyMod.UI;

internal class SetPreviewWindow
{
    private static readonly Vector2 SETPREVIEW_SCALE = new Vector2(400f, 200f);
    private const int HEADER_FONTSIZE = 24;
    private const int MAIN_FONTSIZE = 16;

    private const float TOP = 84f;
    private static readonly Vector2 LEFTCOL = new Vector2(-380f, TOP);
    private static readonly Vector2 RIGHTCOL = new Vector2(230f, TOP);
    private static readonly Vector2 HEADER_LINE_SPACING = new Vector2(0f, -HEADER_FONTSIZE);
    private static readonly Vector2 MAIN_LINE_SPACING = new Vector2(0f, -MAIN_FONTSIZE);
    
    private static readonly Vector2 ACTIVESET_POSITION_1 = new Vector2(-350f, TOP);
    private static readonly Vector2 ACTIVESET_POSITION_2 = new Vector2(260f, TOP);

    private RectTransform rectTransform;

    private TextMeshProUGUI lbActiveSet1;
    private TextMeshProUGUI lbActiveSet2;

    private TextMeshProUGUI lbPlayersRed1;
    private TextMeshProUGUI lbPlayersRed2;
    private TextMeshProUGUI lbPlayersBlue1;
    private TextMeshProUGUI lbPlayersBlue2;
    
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
        
        UIUtils.CreateText(ref lbPlayersRed1, "lbPlayersRed1", rectTransform, LEFTCOL + HEADER_LINE_SPACING);
        lbPlayersRed1.fontSize = MAIN_FONTSIZE;
        lbPlayersRed1.alignment = TextAlignmentOptions.Right;
        lbPlayersRed1.SetText("Red Players:");
        UIUtils.CreateText(ref lbPlayersRed2, "lbPlayersRed2", rectTransform, RIGHTCOL + HEADER_LINE_SPACING);
        lbPlayersRed2.fontSize = MAIN_FONTSIZE;
        lbPlayersRed2.alignment = TextAlignmentOptions.Left;
        lbPlayersRed2.color = Color.yellow;
        lbPlayersRed2.SetText("");
        
        UIUtils.CreateText(ref lbPlayersBlue1, "lbPlayersBlue1", rectTransform, LEFTCOL + HEADER_LINE_SPACING + MAIN_LINE_SPACING);
        lbPlayersBlue1.fontSize = MAIN_FONTSIZE;
        lbPlayersBlue1.alignment = TextAlignmentOptions.Right;
        lbPlayersBlue1.SetText("Blue Players:");
        UIUtils.CreateText(ref lbPlayersBlue2, "lbPlayersBlue2", rectTransform, RIGHTCOL + HEADER_LINE_SPACING + MAIN_LINE_SPACING);
        lbPlayersBlue2.fontSize = MAIN_FONTSIZE;
        lbPlayersBlue2.alignment = TextAlignmentOptions.Left;
        lbPlayersBlue2.color = Color.yellow;
        lbPlayersBlue2.SetText("");
        
        UpdateText();
    }

    private void UpdateText()
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
        
        lbPlayersRed2.SetText("N/A");
        lbPlayersBlue2.SetText("N/A");
    }
}