using System.Collections.Generic;
using LLBML.States;
using LLGUI;
using LLHandlers;
using LLScreen;
using UnityEngine;

namespace TourneyMod.UI;

internal class ScreenMenuMain : LLScreen.ScreenMenuMain, ICustomScreen<LLScreen.ScreenMenuMain>
{
    private static readonly Vector3 OFFSET_RIGHTCOL = new Vector2(406.3f, 0f);
    //private static readonly Vector2 TOURNEY_POSITION = new Vector2(-166.1f, 259.2f);
    internal LLButton btTourney;
    
    public void Init(LLScreen.ScreenMenuMain screenVanilla)
    {
        screenType = screenVanilla.screenType;
        layer = screenVanilla.layer;
        isActive = screenVanilla.isActive;
        msgEsc = screenVanilla.msgEsc;
        msgMenu = screenVanilla.msgMenu;
        msgCancel = screenVanilla.msgCancel;

        btOnline = screenVanilla.btOnline;
        btVersus = screenVanilla.btVersus;
        btSingle = screenVanilla.btSingle;
        btOptions = screenVanilla.btOptions;
        btUnlocks = screenVanilla.btUnlocks;
        onlineGray = screenVanilla.onlineGray;
        onlineColDefault = screenVanilla.btOnline.colDefault;
        onlineColDisabled = screenVanilla.btOnline.colDisabled;
    }

    public override void OnOpen(ScreenType screenTypePrev)
    {
        base.OnOpen(screenTypePrev);
        
        btTourney = Instantiate(btOnline, transform);
        btTourney.name = "btTourney";
        btTourney.transform.localPosition += OFFSET_RIGHTCOL;
        btTourney.onClick = (playerNr) =>
        {
            Plugin.Instance.TourneyMenuOpen = true;
            GameStates.Send(Msg.SEL_VERSUS, playerNr, -1);
        };
        btTourney.SetText("tournament");
    }

    public override void GetControls(ref List<LLClickable> list, bool vert, LLClickable curFocus, LLCursor cursor)
    {
        base.GetControls(ref list, vert, curFocus, cursor);
        list.Add(btTourney);
    }
    
    public override bool DirectMove(Vector2 move, LLClickable curFocus, bool shouldMove)
    {
        if (!shouldMove) return false;
        bool vert = Mathf.Abs(move.y) > Mathf.Abs(move.x);
        float sign = vert ? Mathf.Sign(move.y) : Mathf.Sign(move.x);

        if (vert) return false;

        if (curFocus == btTourney)
        {
            UIScreen.SetFocus(btOnline.visible ? btOnline : btVersus);
            AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
            return true;
        }
        else
        {
            UIScreen.SetFocus(btTourney);
            AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
            return true;
        }
    }
}