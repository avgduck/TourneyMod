using LLBML.Players;
using LLHandlers;

namespace TourneyMod.SetTracking;

internal class Match
{
    internal Stage PlayedStage { get; private set; }
    internal PlayerCharacter[] PlayerCharacters { get; private set; }
    internal PlayerScore[] FinalScores { get; private set; }
    internal Team Winner;
    internal int GameNumber { get; private set; }
    internal bool IsTimeout { get; private set; }
    internal bool IsTiebreaker { get; private set; }

    internal void Start(Stage stage, PlayerCharacter[] playerCharacters)
    {
        PlayedStage = stage;
        PlayerCharacters = playerCharacters;
    }

    internal void End(PlayerScore[] scores, int gameNumber, bool isTimeout, bool isTiebreaker)
    {
        FinalScores = scores;
        Winner = GetWinner();
        GameNumber = gameNumber;
        IsTimeout = isTimeout;
        IsTiebreaker = isTiebreaker;
    }

    private Team GetWinner()
    {
        if (FinalScores == null) return Team.NONE;

        int[] totalPlayers = [0, 0, 0, 0];
        int[] deadPlayers = [0, 0, 0, 0];
            
        for (int playerNumber = 0; playerNumber < 4; playerNumber++)
        {
            PlayerScore score = FinalScores[playerNumber];
            if (score.Team == Team.NONE) continue;
            
            totalPlayers[(int)score.Team]++;
            if (score.Stocks == 0) deadPlayers[(int)score.Team]++;
        }

        int deadTeamCount = 0;
        int aliveTeamCount = 0;
        Team aliveTeam = Team.NONE;
        for (int teamNumber = 0; teamNumber < 4; teamNumber++)
        {
            if (totalPlayers[teamNumber] == 0) continue;

            if (deadPlayers[teamNumber] == totalPlayers[teamNumber])
            {
                deadTeamCount++;
            }
            else
            {
                aliveTeamCount++;
                aliveTeam = (Team)teamNumber;
            }
        }

        if (deadTeamCount == 0 || aliveTeamCount > 1) return Team.NONE;
        return aliveTeam;
    }

}