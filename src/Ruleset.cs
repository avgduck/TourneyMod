using System.Collections.Generic;
using System.Linq;
using LLHandlers;

namespace TourneyMod;

public class Ruleset(
    string id,
    string name,
    Stage[] stagesNeutral,
    Stage[] stagesCounterpick,
    int[][] banAmounts,
    int firstBanPlayer,
    Ruleset.BanOrder banOrder,
    Ruleset.DsrMode dsrMode)
{
    public readonly string id = id;
    public readonly string name = name;
    public readonly Stage[] stagesNeutral = stagesNeutral;
    public readonly Stage[] stagesCounterpick = stagesCounterpick;

    public readonly int[][] banAmounts = banAmounts;
    public readonly int firstBanPlayer = firstBanPlayer;
    public readonly BanOrder banOrder = banOrder;
    public readonly DsrMode dsrMode = dsrMode;

    public enum BanOrder
    {
        WINNER_BANS,
        LOSER_BANS
    }

    public enum DsrMode
    {
        OFF,
        FULL_SET,
        LAST_WIN
    }

    private string PrintList<T>(List<T> list)
    {
        string s = "[";
        
        for (int i = 0; i < list.Count; i++)
        {
            if (i != 0) s += ", ";
            s += list[i];
        }

        s += "]";
        return s;
    }

    public override string ToString()
    {
        List<string> b = new List<string>();
        foreach (int[] banNums in banAmounts)
        {
            b.Add(PrintList<int>(banNums.ToList()));
        }
        
        return $"{{ id {id}, name '{name}', neutral {PrintList<Stage>(stagesNeutral.ToList())}, counterpick {PrintList<Stage>(stagesCounterpick.ToList())}, banAmounts {PrintList<string>(b)}, firstBanPlayer {firstBanPlayer}, banOrder {banOrder}, dsrMode {dsrMode} }}";
    }
}