using System.Collections.Generic;
using System.Linq;
using LLHandlers;

namespace TourneyMod.Rulesets;

public class Ruleset(
    string id,
    string name,
    Stage[] stagesNeutral,
    Stage[] stagesCounterpick,
    int[][] banAmounts,
    int game1FirstPlayer,
    Ruleset.FirstPlayer laterGamesFirstPlayer,
    Ruleset.DsrMode dsrMode)
{
    public readonly string id = id;
    public readonly string name = name;
    public readonly Stage[] stagesNeutral = stagesNeutral;
    public readonly Stage[] stagesCounterpick = stagesCounterpick;

    public readonly int[][] banAmounts = banAmounts;
    public readonly int game1FirstPlayer = game1FirstPlayer;
    public readonly FirstPlayer laterGamesFirstPlayer = laterGamesFirstPlayer;
    public readonly DsrMode dsrMode = dsrMode;

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

    private string PrintList<T>(List<T> list, bool includeBrackets = true)
    {
        string s = "";
        if (includeBrackets) s += "[";

        for (int i = 0; i < list.Count; i++)
        {
            if (i != 0) s += ", ";
            s += list[i];
        }

        if (includeBrackets) s += "]";
        return s;
    }

    private string PrintBanOrder()
    {
        string s = "";
        if (banAmounts.Length == 0)
        {
            s += $"{(laterGamesFirstPlayer == FirstPlayer.WINNER ? "W" : "L")} picks";
        }
        else
        {
            int gameIndex = 0;
            foreach (int[] banNums in banAmounts)
            {
                if (gameIndex != 0) s += " | ";
                if (banAmounts.Length > 1) s += $"G{gameIndex + 1}{(gameIndex == banAmounts.Length - 1 ? "+" : "")} ";
                bool bansIncluded = true;
                for (int i = 0; i < banNums.Length; i++)
                {
                    if (banNums[i] == 0)
                    {
                        s += $"{(laterGamesFirstPlayer == FirstPlayer.WINNER ? "W" : "L")} picks";
                        break;
                    }

                    if (i != 0) s += "-";
                    for (int j = 0; j < banNums[i]; j++)
                    {
                        s += (i % 2 == 0) ? (laterGamesFirstPlayer == FirstPlayer.WINNER ? "W" : "L") : (laterGamesFirstPlayer == FirstPlayer.WINNER ? "L" : "W");
                    }
                }
                
                gameIndex++;
            }
        }
        
        return s;
    }

    public override string ToString()
    {
        List<string> b = new List<string>();
        foreach (int[] banNums in banAmounts)
        {
            b.Add(PrintList<int>(banNums.ToList()));
        }

        return
            $"{{ id {id}, name '{name}', neutral {PrintList<Stage>(stagesNeutral.ToList())}, counterpick {PrintList<Stage>(stagesCounterpick.ToList())}, banAmounts {PrintList<string>(b)}, game1FirstPlayer {game1FirstPlayer}, laterGamesFirstPlayer {laterGamesFirstPlayer}, dsrMode {dsrMode} }}";
    }

    internal List<string> GetDescription()
    {
        List<string> text = new List<string>();
        
        text.Add("");
        text.Add($"- <b>{name} [{id}]</b>:");
        text.Add($"<i>Neutral Stages</i>: {PrintList<Stage>(stagesNeutral.ToList(), false)}");
        text.Add($"<i>Counterpick Stages</i>: {PrintList<Stage>(stagesCounterpick.ToList(), false)}");
        text.Add($"<i>Ban Order (P{game1FirstPlayer+1} starts for G1)</i>: {PrintBanOrder()}");
        text.Add($"<i>DSR</i>: {dsrMode switch {
            DsrMode.OFF => "OFF",
            DsrMode.FULL_SET => "ON, includes all wins",
            DsrMode.LAST_WIN => "ON, only last win"
        }}");
        
        return text;
    }
}