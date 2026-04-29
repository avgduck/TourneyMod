using LLBML.States;
using LLGUI;
using LLScreen;
using TourneyMod.UI.Lobby.PlayerTags;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI.Lobby;

public class ScreenLobby : ScreenPlayers, ICustomScreen<ScreenPlayers>
{
    private LLButton[] settingsButtons;
    internal PlayerTagMenu[] playerTagMenus;
    
    public void Init(ScreenPlayers screenPlayers)
    {
        screenType = screenPlayers.screenType;
        layer = screenPlayers.layer;
        isActive = screenPlayers.isActive;
        msgEsc = screenPlayers.msgEsc;
        msgMenu = screenPlayers.msgMenu;
        msgCancel = screenPlayers.msgCancel;
        
        btBack = screenPlayers.btBack;
        btStart = screenPlayers.btStart;
        characterButtons = screenPlayers.characterButtons;
        playerSelections = screenPlayers.playerSelections;
        lbCharInfo = screenPlayers.lbCharInfo;
        lbCountdown = screenPlayers.lbCountdown;
        lbGameModeHeader = screenPlayers.lbGameModeHeader;
        lbGameMode = screenPlayers.lbGameMode;
        btGameMode = screenPlayers.btGameMode;
        btOptions = screenPlayers.btOptions;
        btInviteFriends = screenPlayers.btInviteFriends;
        lbStocksHeader = screenPlayers.lbStocksHeader;
        lbTimeHeader = screenPlayers.lbTimeHeader;
        lbSpeedHeader = screenPlayers.lbSpeedHeader;
        lbBallTypeHeader = screenPlayers.lbBallTypeHeader;
        lbEnergyHeader = screenPlayers.lbEnergyHeader;
        lbHpFactorHeader = screenPlayers.lbHpFactorHeader;
        lbPowerupSelectionHeader = screenPlayers.lbPowerupSelectionHeader;
        lbStocks = screenPlayers.lbStocks;
        lbTime = screenPlayers.lbTime;
        lbSpeed = screenPlayers.lbSpeed;
        lbBallType = screenPlayers.lbBallType;
        lbEnergy = screenPlayers.lbEnergy;
        lbHpFactor = screenPlayers.lbHpFactor;
        lbPowerupSelection = screenPlayers.lbPowerupSelection;
        obSettings = screenPlayers.obSettings;
        pfPlayerSelection = screenPlayers.pfPlayerSelection;
        pfCharacterButton = screenPlayers.pfCharacterButton;
        pnCharacterButtons = screenPlayers.pnCharacterButtons;
        pnPlayers = screenPlayers.pnPlayers;
        curCountDown = screenPlayers.curCountDown;
        kCountDown = screenPlayers.kCountDown;
        countDownState = screenPlayers.countDownState;
        nPlayersShown = screenPlayers.nPlayersShown;
    }

    public override void OnOpen(ScreenType screenTypePrev)
    {
        base.OnOpen(screenTypePrev);

        Texture2D texGear = Assets.LoadTexture("gear.png");
        Sprite spriteGear = UIUtils.ToSprite(texGear);

        settingsButtons = new LLButton[4];
        playerTagMenus = new PlayerTagMenu[4];
        for (int playerIndex = 0; playerIndex < 4; playerIndex++)
        {
            PlayersSelection playerSelection = playerSelections[playerIndex];

            LLButton btSettings = null;
            UIUtils.CreateImageButton(ref btSettings, spriteGear, "btSettings", playerSelection.btPlayerName.transform.parent, playerSelection.btPlayerName.transform.localPosition + new Vector3(0f, 2f, 0f), new Vector2(20f, 20f));
            int i = playerIndex;
            btSettings.onClick = playerNr => OnClickSettings(i, playerNr);
            settingsButtons[playerIndex] = btSettings;
            
            playerSelection.btPlayerName.transform.localPosition += new Vector3(15f, 0f, 0f);

            playerTagMenus[playerIndex] = PlayerTagMenu.CreateMenu(playerSelection.transform, playerIndex);
        }
    }

    internal void OnEject(int playerNr)
    {
        PlayerTagMenu tagMenu = playerTagMenus[playerNr];
        tagMenu.Close();
        UpdateTeamButtons();
    }

    private void OnClickSettings(int playerIndex, int playerNr)
    {
        if (playerNr != -1 && playerIndex != playerNr) return;
        
        PlayerTagMenu tagMenu = playerTagMenus[playerIndex];
        if (tagMenu.gameObject.activeSelf) tagMenu.Close();
        else tagMenu.OpenBrowse();
        UpdateTeamButtons();
    }
}