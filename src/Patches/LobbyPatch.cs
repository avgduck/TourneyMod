using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;
using LLGUI;
using LLHandlers;
using LLScreen;
using TourneyMod.PlayerTags;
using TourneyMod.SetTracking;
using TourneyMod.UI.Lobby;
using TourneyMod.UI.PlayerTags;

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
        ScreenLobby screenLobby = __instance as ScreenLobby;
        if (screenLobby == null) return true;
        
        ScreenLobbyTourney screenLobbyTourney = __instance as ScreenLobbyTourney;
        if (screenLobbyTourney == null)
        {
            if (screenLobby.playerSelections == null || screenLobby.playerSelections.Length < 4) return false;

            for (int playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                bool isTagMenuEnabled = screenLobby.playerTagMenus != null && screenLobby.playerTagMenus[playerIndex].gameObject.activeSelf;
                bool isTeamMode = NCMFHODLNAJ.BABJPAPBMIP(GameSettings.current.gameMode);
                screenLobby.playerSelections[playerIndex].btTeam.SetActive(isTeamMode && !isTagMenuEnabled);
                screenLobby.playerSelections[playerIndex].btSkin.SetActive(!isTagMenuEnabled);
            }
            
            return false;
        }

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
        Player p = LGACHGEPNNH;
        //Plugin.LogGlobal.LogWarning($"AddPlayer P{p.nr} controller {p.controller}");
        
        if (SetTracker.Instance.ActiveTourneyMode is TourneyMode.NONE) return;
        if (SetTracker.Instance.CurrentSet.LastWinnerOverride != Team.NONE) return;
        
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
    }
    
    // bool GameStatesLobby::IsStartEnabled()
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.DJHHLDPLFMD))]
    // bool GameStatesLobbySingle::IsStartEnabled()
    [HarmonyPatch(typeof(HFAEJNGHDDM), nameof(HFAEJNGHDDM.DJHHLDPLFMD))]
    [HarmonyPrefix]
    private static bool GameStatesLobby_IsStartEnabled_Prefix(HPNLMFHPHFD __instance, ref bool __result)
    {
        ScreenPlayers screenPlayers = __instance.IMLMFFIEEAJ;
        ScreenLobby screenLobby = screenPlayers as ScreenLobby;
        if (screenLobby == null) return true;

        for (int playerIndex = 0; playerIndex < 4; playerIndex++)
        {
            bool isTagMenuEnabled = screenLobby.playerTagMenus != null && screenLobby.playerTagMenus[playerIndex].gameObject.activeSelf;
            if (isTagMenuEnabled)
            {
                __result = false;
                return false;
            }
        }

        return true;
    }

    // void GameStatesLobby::RemovePlayer(Player p)
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.GNBKBMENOMO))]
    [HarmonyPostfix]
    private static void GameStatesLobby_RemovePlayer_Postfix(HPNLMFHPHFD __instance, ALDOKEMAOMB LGACHGEPNNH)
    {
        ScreenPlayers screenPlayers = __instance.IMLMFFIEEAJ;
        ScreenLobby screenLobby = screenPlayers as ScreenLobby;
        if (screenLobby == null) return;
        if (screenLobby.playerTagMenus == null) return;

        Player p = LGACHGEPNNH;
        screenLobby.OnEject(p.nr);
    }

    // void GameStatesLobby::UpdatePlayer(Player p, bool play_selection_anim)
    [HarmonyPatch(typeof(HPNLMFHPHFD), nameof(HPNLMFHPHFD.BDMIDGAHNLA))]
    [HarmonyPostfix]
    private static void GameStatesLobby_UpdatePlayer_Postfix(HPNLMFHPHFD __instance, ALDOKEMAOMB LGACHGEPNNH)
    {
        ScreenPlayers screenPlayers = __instance.IMLMFFIEEAJ;
        ScreenLobby screenLobby = screenPlayers as ScreenLobby;
        if (screenLobby == null) return;
        if (screenLobby.playerTagMenus == null) return;
        
        Player p = LGACHGEPNNH;
        PlayerTag playerTag = Plugin.Instance.GetPlayerTag(p.nr);
        if (!p.IsInMatch && !p.IsSpectator) return;
        if (!p.isLocal) return;

        if (playerTag.IsDefault || p.IsAI)
        {
            screenLobby.SetPlayerName(p.nr, $"PLAYER{p.nr+1}");
            screenLobby.playerSelections[p.nr].btPlayerName.colDefault = PlayerTagMenu.COLOR_TAG_DEFAULT;
            screenLobby.playerSelections[p.nr].btPlayerName.textMesh.color = PlayerTagMenu.COLOR_TAG_DEFAULT;
        }
        else
        {
            screenLobby.SetPlayerName(p.nr, playerTag.GetName());
            screenLobby.playerSelections[p.nr].btPlayerName.colDefault = PlayerTagMenu.COLOR_TAG_CUSTOM;
            screenLobby.playerSelections[p.nr].btPlayerName.textMesh.color = PlayerTagMenu.COLOR_TAG_CUSTOM;
        }
    }
    
    // void GameHudPlayerInfo::SetPlayer(Player player, int playerNameSize)
    [HarmonyPatch(typeof(GameHudPlayerInfo), nameof(GameHudPlayerInfo.SetPlayer))]
    [HarmonyPostfix]
    private static void GameHudPlayerInfo_SetPlayer_Postfix(GameHudPlayerInfo __instance, ALDOKEMAOMB player)
    {
        Player p = player;
        if (GameSettings.IsOnline) return;
        PlayerTag playerTag = Plugin.Instance.GetPlayerTag(p.nr);
        if (playerTag.IsDefault) return;
        TextHandler.SetTextBestFont(__instance.lbName, playerTag.GetName());
    }
}