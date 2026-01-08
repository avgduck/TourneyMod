namespace TourneyMod.Rulesets;

public class GameOptions(
    int stocks,
    int time,
    int energy,
    HpFactor hpFactor,
    int minBallSpeed,
    BallType ballType,
    PowerupSelection powerupSelection)
{
    public readonly int stocks = stocks;
    public readonly int time = time;
    public readonly int energy = energy;
    public readonly HpFactor hpFactor = hpFactor;
    public readonly int minBallSpeed = minBallSpeed;
    public readonly BallType ballType = ballType;
    public readonly PowerupSelection powerupSelection = powerupSelection;
}