using LLHandlers;

namespace TourneyMod.UI;

internal interface IStageSelect
{
    public void OnClickStage(int playerNumber, Stage stage);
    public void OnStageSelected();
}