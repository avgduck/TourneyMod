using LLHandlers;

namespace TourneyMod.SetTracking;

internal class Match
{
    internal Stage PlayedStage { get; private set; }
    internal Character[] SelectedCharacters { get; private set; }
    internal Character[] PlayedCharacters { get; private set; }
    internal int[] FinalScores { get; private set; }
    internal int Winner {
        get {
            if (FinalScores == null) return -1;
            int deadPlayer = -1;
            int alivePlayer = -1;
            for (int playerNumber = 0; playerNumber < 4; playerNumber++)
            {
                if (FinalScores[playerNumber] == 0) deadPlayer = playerNumber;
                else if (FinalScores[playerNumber] > 0) alivePlayer = playerNumber;
            }

            if (deadPlayer == -1 || alivePlayer == -1) return -1;
            return alivePlayer;
        }
    }

    internal void Start(Stage stage, Character[] selectedCharacters, Character[] playedCharacters)
    {
        PlayedStage = stage;
        SelectedCharacters = selectedCharacters;
        PlayedCharacters = playedCharacters;
    }

    internal void End(int[] scores)
    {
        FinalScores = scores;
    }

}