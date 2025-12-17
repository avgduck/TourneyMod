using System.Collections.Generic;
using System.Linq;
using LLHandlers;

namespace TourneyMod.Rulesets;

public class Ruleset(
    string name,
    List<Stage> stagesNeutral,
    List<Stage> stagesCounterpick,
    int[][] banAmounts,
    int game1FirstPlayer,
    Ruleset.FirstPlayer laterGamesFirstPlayer,
    Ruleset.DsrMode dsrMode,
    Ruleset.RandomMode randomMode)
{
    public string Id { get; private set; }
    public readonly string name = name;
    public readonly List<Stage> stagesNeutral = stagesNeutral;
    public readonly List<Stage> stagesCounterpick = stagesCounterpick;

    public readonly int[][] banAmounts = banAmounts;
    public readonly int game1FirstPlayer = game1FirstPlayer;
    public readonly FirstPlayer laterGamesFirstPlayer = laterGamesFirstPlayer;
    public readonly DsrMode dsrMode = dsrMode;
    public readonly RandomMode randomMode = randomMode;

    public enum FirstPlayer
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
        ANY,
        ANY_3D,
        ANY_2D,
        ANY_LEGAL
    }

    internal void InitId(string id)
    {
        if (Id != null) Plugin.LogGlobal.LogWarning($"Failed to set id '{id}' to ruleset with existing id '{Id}'");
        else Id = id;
    }

    public static readonly List<Stage> STAGES_3D = [Stage.OUTSKIRTS, Stage.SEWERS, Stage.JUNKTOWN, Stage.CONSTRUCTION, Stage.FACTORY, Stage.SUBWAY, Stage.STADIUM, Stage.STREETS, Stage.POOL, Stage.ROOM21];
    public static readonly List<Stage> STAGES_2D = [Stage.OUTSKIRTS_2D, Stage.SEWERS_2D, Stage.ROOM21_2D, Stage.STREETS_2D, Stage.SUBWAY_2D, Stage.FACTORY_2D];
}