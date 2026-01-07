using System.Collections.Generic;
using LLBML.Players;
using LLHandlers;
using TourneyMod.SetTracking;

namespace TourneyMod.Rulesets;

public class Ruleset(
    string name,
    List<Stage> stagesNeutral,
    List<Stage> stagesCounterpick,
    int[][] banAmounts,
    PlayerTeam game1FirstTeam,
    Ruleset.FirstTeam laterGamesFirstTeam,
    Ruleset.DsrMode dsrMode,
    Ruleset.RandomMode randomMode)
{
    public string Id { get; private set; }
    public readonly string name = name;
    public readonly List<Stage> stagesNeutral = stagesNeutral;
    public readonly List<Stage> stagesCounterpick = stagesCounterpick;

    public readonly int[][] banAmounts = banAmounts;
    public readonly PlayerTeam game1FirstTeam = game1FirstTeam;
    public readonly FirstTeam laterGamesFirstTeam = laterGamesFirstTeam;
    public readonly DsrMode dsrMode = dsrMode;
    public readonly RandomMode randomMode = randomMode;

    public enum FirstTeam
    {
        WINNER,
        LOSER
    }

    public enum DsrMode
    {
        OFF,
        FULL_SET,
        LAST_WIN
    }

    public enum RandomMode
    {
        OFF,
        ANY_3D,
        ANY_2D,
        BOTH,
        ANY,
        ANY_LEGAL
    }

    internal void InitId(string id)
    {
        if (Id != null) Plugin.LogGlobal.LogWarning($"Failed to set id '{id}' to ruleset with existing id '{Id}'");
        else Id = id;
    }

    internal static Team ConvertPlayerTeam(PlayerTeam team)
    {
        return team switch
        {
            PlayerTeam.RED => Team.RED,
            PlayerTeam.BLUE => Team.BLUE,
            PlayerTeam.YELLOW => Team.YELLOW,
            PlayerTeam.GREEN => Team.GREEN,
            _ => Team.NONE
        };
    }

    internal static PlayerTeam ConvertTeam(Team team)
    {
        if (team == Team.RED) return PlayerTeam.RED;
        if (team == Team.BLUE) return PlayerTeam.BLUE;
        if (team == Team.YELLOW) return PlayerTeam.YELLOW;
        if (team == Team.GREEN) return PlayerTeam.GREEN;
        return PlayerTeam.NONE;
    }
    
    public static readonly List<Stage> STAGES_3D = [Stage.OUTSKIRTS, Stage.SEWERS, Stage.JUNKTOWN, Stage.CONSTRUCTION, Stage.FACTORY, Stage.SUBWAY, Stage.STADIUM, Stage.STREETS, Stage.POOL, Stage.ROOM21];
    public static readonly List<Stage> STAGES_2D = [Stage.OUTSKIRTS_2D, Stage.SEWERS_2D, Stage.ROOM21_2D, Stage.STREETS_2D, Stage.SUBWAY_2D, Stage.FACTORY_2D];
}