using LLBML.Players;

namespace TourneyMod.SetTracking;

internal struct PlayerScore()
{
    internal int Stocks = -1;
    internal Team Team = Team.NONE;

    public override string ToString()
    {
        return $"({Team}, {Stocks})";
    }
}