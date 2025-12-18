using LLBML.Utils;
using LLHandlers;
using TMPro;
using UnityEngine;

namespace TourneyMod.UI;

internal class StageContainer
{
    private TMP_Text lbStageName;
    private TMP_Text lbStageSize;

    internal StageButton Button { get; private set; }
    internal Stage StoredStage { get; private set; }
    private string StageName => StringUtils.GetStageReadableName(StoredStage);

    private Vector2 StageSize => StoredStage switch
    {
        Stage.OUTSKIRTS => new Vector2(1240, 510),
        Stage.SEWERS => new Vector2(1240, 510), 
        Stage.JUNKTOWN => new Vector2(1130, 510),
        Stage.CONSTRUCTION => new Vector2(1492, 522),
        Stage.FACTORY => new Vector2(1400, 542),
        Stage.SUBWAY => new Vector2(1050, 510),
        Stage.STADIUM => new Vector2(1230, 540),
        Stage.STREETS => new Vector2(1320, 515),
        Stage.POOL => new Vector2(1210, 575),
        Stage.ROOM21 => new Vector2(1100, 550),
        
        Stage.OUTSKIRTS_2D => new Vector2(1230, 493),
        Stage.POOL_2D => new Vector2(1160, 518),
        Stage.SEWERS_2D => new Vector2(1162, 512),
        Stage.ROOM21_2D => new Vector2(975, 484),
        Stage.STREETS_2D => new Vector2(1105, 480),
        Stage.SUBWAY_2D => new Vector2(1100, 483),
        Stage.FACTORY_2D => new Vector2(1256, 407),
        
        _ => Vector2.zero
    };

    internal StageContainer(Stage stage)
    {
        StoredStage = stage;
        
        Button = StageButton.CreateStageButton(ScreenStageStrike.Instance.screenStage.stageButtonsContainer, stage);
        Button.SetActive(true);
        Button.onClick = (playerNumber) =>
        {
            ScreenStageStrike.Instance.OnClickStage(playerNumber, StoredStage);
            Button.UpdateDisplay();
        };

        lbStageName = ScreenStageStrike.Instance.CreateNewText("lbStageName", Button.transform);
        lbStageName.fontSize = 42;
        lbStageName.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
        lbStageName.rectTransform.localPosition = new Vector2(0f, -110f);
        TextHandler.SetText(lbStageName, StageName);
        
        lbStageSize = ScreenStageStrike.Instance.CreateNewText("lbStageSize", Button.transform);
        lbStageSize.fontSize = 22;
        lbStageSize.rectTransform.localPosition = new Vector2(190f, 110f);
        TextHandler.SetText(lbStageSize, (StageSize != Vector2.zero ? $"{StageSize.x}x{StageSize.y}" : ""));
    }
}