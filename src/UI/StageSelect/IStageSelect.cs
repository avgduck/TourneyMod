using LLHandlers;

namespace TourneyMod.UI.StageSelect;

internal interface IStageSelect
{
    public void OnClickStage(int playerNumber, Stage stage);
    public void OnStageSelected();
}