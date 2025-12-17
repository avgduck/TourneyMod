using LLHandlers;

namespace TourneyMod.StageStriking;

internal class StageBan
{
    internal Stage stage;
    internal BanReason reason;
    internal int banPlayer;

    internal StageBan(Stage stage, BanReason reason, int banPlayer = -1)
    {
        this.stage = stage;
        this.reason = reason;
        this.banPlayer = banPlayer;
    }

    internal enum BanReason
    {
        COUNTERPICK,
        DSR,
        BAN
    }
}