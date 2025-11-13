using LLHandlers;

namespace TourneyMod;

internal class Ruleset
{
    internal static readonly Ruleset RULES_STANDARD_ONLINE = new Ruleset([
        Stage.JUNKTOWN,
        Stage.ROOM21,
        (Plugin.USE_SEWERS ? Stage.SEWERS : Stage.OUTSKIRTS),
        Stage.STADIUM,
        Stage.STREETS,
        Stage.POOL,
        Stage.SUBWAY,
        Stage.FACTORY,
        Stage.CONSTRUCTION,
    ], [
    ], [
        [3],
    ], 0, BanOrder.WINNER_BANS, DsrMode.OFF);
    
    internal static readonly Ruleset RULES_UK = new Ruleset([
        Stage.JUNKTOWN,
        Stage.ROOM21,
        (Plugin.USE_SEWERS ? Stage.SEWERS : Stage.OUTSKIRTS),
        Stage.STADIUM,
        Stage.STREETS,
    ], [
        Stage.POOL,
        Stage.SUBWAY,
        Stage.FACTORY,
        Stage.CONSTRUCTION,
    ], [
        [1, 2, 1],
        [2]
    ], 1, BanOrder.WINNER_BANS, DsrMode.FULL_SET);
    
    internal readonly Stage[] stagesNeutral;
    internal readonly Stage[] stagesCounterpick;

    internal readonly int[][] banAmounts;
    internal readonly int firstBanPlayer;
    internal readonly BanOrder banOrder;
    internal readonly DsrMode dsrMode;

    internal Ruleset(Stage[] stagesNeutral, Stage[] stagesCounterpick, int[][] banAmounts, int firstBanPlayer, BanOrder banOrder, DsrMode dsrMode)
    {
        this.stagesNeutral = stagesNeutral;
        this.stagesCounterpick = stagesCounterpick;
        this.banAmounts = banAmounts;
        this.firstBanPlayer = firstBanPlayer;
        this.banOrder = banOrder;
        this.dsrMode = dsrMode;
    }

    internal enum BanOrder
    {
        WINNER_BANS,
        LOSER_BANS
    }

    internal enum DsrMode
    {
        OFF,
        FULL_SET,
        LAST_MATCH
    }
}