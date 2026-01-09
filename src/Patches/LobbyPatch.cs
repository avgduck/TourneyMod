using HarmonyLib;
using LLBML.Players;
using LLScreen;
using TourneyMod.SetTracking;
using TourneyMod.UI.Lobby;

namespace TourneyMod.Patches;

internal static class LobbyPatch
{
    [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.ShowCpuButtons))]
    [HarmonyPrefix]
    private static void ScreenPlayers_ShowCpuButtons_Prefix(ScreenPlayers __instance, ref bool visible)
    {
        ScreenLobbyTourney screenLobbyTourney = __instance as ScreenLobbyTourney;
        if (screenLobbyTourney == null) return;
        visible = false;
    }
    
    [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.UpdateTeamButtons))]
    [HarmonyPrefix]
    private static bool ScreenPlayers_UpdateTeamButtons_Prefix(ScreenPlayers __instance)
    {
        ScreenLobbyTourney screenLobbyTourney = __instance as ScreenLobbyTourney;
        if (screenLobbyTourney == null) return true;

        foreach (PlayersSelection playerSelection in __instance.playerSelections)
        {
            playerSelection.btTeam.SetActive(false);
        }

        return false;
    }

    // void Player::ResetTeam(GameMode gameMode, bool changeAnyway)
    [HarmonyPatch(typeof(ALDOKEMAOMB), nameof(ALDOKEMAOMB.MLFHMGNCMNA))]
    [HarmonyPrefix]
    private static bool Player_ResetTeam_Prefix(ALDOKEMAOMB __instance)
    {
        if (SetTracker.Instance.ActiveTourneyMode == TourneyMode.NONE) return true;

        Player player = __instance;
        if (SetTracker.Instance.IsMode1v1) player.Team = player.nr == 0 ? Team.RED : Team.BLUE;
        else if (SetTracker.Instance.IsModeDoubles) player.Team = player.nr <= 1 ? Team.RED : Team.BLUE;
        
        return false;
    }
    
    // bool GameStatesLobby::IsStartEnabled()
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.DJHHLDPLFMD))]
    [HarmonyPrefix]
    private static bool GameStatesLobby_IsStartEnabled_Prefix(ref bool __result)
    {
        if (!SetTracker.Instance.IsModeDoubles) return true;

        int nReady = 0;
        Player.ForAllInMatch(player =>
        {
            if (player.selected) nReady++;
        });
        __result = nReady == 4;
        
        return false;
    }

    [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.ShowOptionsButton))]
    [HarmonyPrefix]
    private static void ScreenPlayers_ShowOptionsButton_Prefix(ref bool enabled)
    {
        if (!SetTracker.Instance.IsTrackingSet) return;
        if (SetTracker.Instance.CurrentSet.ActiveRuleset.HasGameOptions) enabled = false;
    }
}