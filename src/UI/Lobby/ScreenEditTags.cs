using LLScreen;
using UnityEngine;

namespace TourneyMod.UI.Lobby;

public class ScreenEditTags : ScreenPlayersSettings, ICustomScreen<ScreenPlayersSettings>
{
    internal ScreenLobby screenLobby;
    
    public void Init(ScreenPlayersSettings screenPlayersSettings)
    {
        screenType = screenPlayersSettings.screenType;
        layer = screenPlayersSettings.layer;
        isActive = screenPlayersSettings.isActive;
        msgEsc = screenPlayersSettings.msgEsc;
        msgMenu = screenPlayersSettings.msgMenu;
        msgCancel = screenPlayersSettings.msgCancel;

        btStocks = screenPlayersSettings.btStocks;
        btTime = screenPlayersSettings.btTime;
        btTag = screenPlayersSettings.btTag;
        btSpeed = screenPlayersSettings.btSpeed;
        btBallType = screenPlayersSettings.btBallType;
        btEnergy = screenPlayersSettings.btEnergy;
        btHpFactor = screenPlayersSettings.btHpFactor;
        btPowerupSelection = screenPlayersSettings.btPowerupSelection;
        btReset = screenPlayersSettings.btReset;
        btBack = screenPlayersSettings.btBack;

        screenLobby = GameObject.FindObjectOfType<ScreenLobby>();
    }
    
    public override void OnOpen(ScreenType screenTypePrev)
    {
        base.OnOpen(screenTypePrev);
        
        btStocks.visible = false;
        btTime.visible = false;
        btTag.visible = false;
        btSpeed.visible = false;
        btBallType.visible = false;
        btEnergy.visible = false;
        btHpFactor.visible = false;
        btPowerupSelection.visible = false;
        btReset.visible = false;
        btBack.visible = false;
        transform.Find("Panel").gameObject.SetActive(false);
    }
}