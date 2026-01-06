using LLBML.Players;
using LLHandlers;

namespace TourneyMod.SetTracking;

internal class Match
{
    internal Stage PlayedStage { get; private set; }
    internal Character[] SelectedCharacters { get; private set; }
    internal Character[] PlayedCharacters { get; private set; }
    internal PlayerScore[] FinalScores { get; private set; }
    internal Team Winner { get; private set; }

    internal void Start(Stage stage, Character[] selectedCharacters, Character[] playedCharacters)
    {
        PlayedStage = stage;
        SelectedCharacters = selectedCharacters;
        PlayedCharacters = playedCharacters;
    }

    internal void End(PlayerScore[] scores)
    {
        FinalScores = scores;
        Winner = GetWinner();
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