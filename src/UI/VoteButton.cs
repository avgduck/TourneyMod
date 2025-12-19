using System;
using System.Linq;
using LLGUI;
using TourneyMod.SetTracking;

namespace TourneyMod.UI;

internal class VoteButton : LLButton
{
    private bool[] votes = new bool[4];
    internal Action onVote;
    internal bool enableVoting = true;
    internal string label = "";

    public override void OnClick(int playerNr)
    {
        base.OnClick(playerNr);

        if (playerNr == -1) SetAllVotes(true);
        else votes[playerNr] = true;
    }

    private void SetAllVotes(bool vote)
    {
        for (int i = 0; i < 4; i++)
        {
            votes[i] = vote;
        }
    }

    private void LateUpdate()
    {
        SetText($"{label}{(enableVoting ? $" {votes.Count(vote => vote)}/{SetTracker.Instance.NumPlayersInMatch}" : "")}");
        UpdateVoteStatus();
    }

    private void UpdateVoteStatus()
    {
        int requiredVotes = enableVoting ? SetTracker.Instance.NumPlayersInMatch : 1;
        if (votes.Count(vote => vote) >= requiredVotes) OnVote();
    }

    private void OnVote()
    {
        SetAllVotes(false);
        onVote();
    }
}