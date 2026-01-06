using LLBML.Players;
using LLHandlers;

namespace TourneyMod.StageStriking;

internal class StageBan
{
    internal Stage stage;
    internal BanReason reason;
    internal Team banTeam;

    internal StageBan(Stage stage, BanReason reason, Team banTeam)
    {
        this.stage = stage;
        this.reason = reason;
        this.banTeam = banTeam;
    }

    internal enum BanReason
    {
        COUNTERPICK,
        DSR,
        BAN
    }
}