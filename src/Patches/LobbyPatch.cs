using HarmonyLib;
using LLBML.Players;
using LLGUI;
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

    // void GameStatesLobby::AddPlayer(Player p)
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.GNCDBOBHOHN))]
    [HarmonyPrefix]
    private static void GameStatesLobby_AddPlayer_Prefix(ref ALDOKEMAOMB LGACHGEPNNH)
    {
        if (SetTracker.Instance.ActiveTourneyMode is TourneyMode.NONE) return;
        if (SetTracker.Instance.CurrentSet.LastWinnerOverride != Team.NONE) return;

        Player p = LGACHGEPNNH;
        PlayerCharacter characterLock = SetTracker.Instance.CurrentSet.PlayerCharacterLock[p.nr];
        if (characterLock.IsEmpty) return;
        
        p.variant = characterLock.variant;
    }
    
    // void GameStatesLobby::UpdatePlayer(Player p, bool play_selection_anim)
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.BDMIDGAHNLA))]
    [HarmonyPrefix]
    private static void GameStatesLobby_UpdatePlayer_Prefix(ref ALDOKEMAOMB LGACHGEPNNH)
    {
        if (SetTracker.Instance.ActiveTourneyMode is TourneyMode.NONE) return;
        if (SetTracker.Instance.CurrentSet.LastWinnerOverride != Team.NONE) return;
        
        Player p = LGACHGEPNNH;
        PlayerCharacter characterLock = SetTracker.Instance.CurrentSet.PlayerCharacterLock[p.nr];
        if (characterLock.IsEmpty) return;
        
        p.CharacterSelected = characterLock.character;
        p.selected = true;
    }

    [HarmonyPatch(typeof(ScreenPlayers), nameof(ScreenPlayers.AddCharacters))]
    [HarmonyPostfix]
    private static void ScreenPlayers_AddCharacters_Postfix(ScreenPlayers __instance)
    {
        foreach (PlayersCharacterButton characterButton in __instance.characterButtons)
        {
            LLClickable.ControlDelegate onClick = characterButton.btCharacter.onClick;
            characterButton.btCharacter.onClick = (playerNr) =>
            {
                if (SetTracker.Instance.ActiveTourneyMode is not TourneyMode.NONE && playerNr != -1 && !SetTracker.Instance.CurrentSet.PlayerCharacterLock[playerNr].IsEmpty && SetTracker.Instance.CurrentSet.LastWinnerOverride == Team.NONE) return;
                onClick(playerNr);
            };
        }
    }
    
    // void GameStatesLobby::CloseOptions()
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.BFKBMEJONDL))]
    [HarmonyPostfix]
    private static void CloseOptions_Postfix()
    {
        Plugin.Instance.ScoreEditMenuOpen = false;
        Plugin.Instance.TagEditMenuOpen = false;
    }
}