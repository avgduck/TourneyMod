using System.Collections.Generic;
using LLGUI;
using LLHandlers;
using LLScreen;
using UnityEngine;

namespace TourneyMod.UI;

internal class ScreenMenuLocal : ScreenMenuVersus
{
    private LLButton btTourney;
    
    internal void Init(ScreenMenuVersus screenMenuVersus)
    {
        screenType = screenMenuVersus.screenType;
        layer = screenMenuVersus.layer;
        isActive = screenMenuVersus.isActive;
        msgEsc = screenMenuVersus.msgEsc;
        msgMenu = screenMenuVersus.msgMenu;
        msgCancel = screenMenuVersus.msgCancel;
        
        btRoyale = screenMenuVersus.btRoyale;
        bt1v1 = screenMenuVersus.bt1v1;
        btTeams = screenMenuVersus.btTeams;
        btVolley = screenMenuVersus.btVolley;
        btStrikers = screenMenuVersus.btStrikers;
    }
    
    public override void OnOpen(ScreenType screenTypePrev)
    {
        btTourney = Instantiate(btRoyale, transform);
        btTourney.name = "btTourney";
        btTourney.transform.localPosition = new Vector2(230f, 170.4f);
        btTourney.onClick = (playerNr) =>
        {
            Plugin.Instance.ActiveTourneyMode = TourneyMode.LOCAL;
            bt1v1.onClick(playerNr);
        };
        
        base.OnOpen(screenTypePrev);
    }

    public override void UpdateText()
    {
        base.UpdateText();

        int num = TextHandler.GetFontSize("MENU_BTS_VERSUS");
        btTourney.SetText("local tourney");
        btTourney.SetFontSize(num);
    }

    public override void GetControls(ref List<LLClickable> list, bool vert, LLClickable curFocus, LLCursor cursor)
    {
        list.Add(btTourney);
        
        base.GetControls(ref list, vert, curFocus, cursor);
    }

    public override bool DirectMove(Vector2 move, LLClickable curFocus, bool shouldMove)
    {
        if (!shouldMove) return false;
        bool vert = Mathf.Abs(move.y) > Mathf.Abs(move.x);
        float sign = vert ? Mathf.Sign(move.y) : Mathf.Sign(move.x);

        if (vert)
        {
            if (curFocus != btTourney) return false;
            if (curFocus == btTourney && sign > 0) return false;
            if (curFocus == btTourney && sign < 0)
            {
                UIScreen.SetFocus(bt1v1);
                return true;
            }
        }

        if (curFocus == btTourney)
        {
            UIScreen.SetFocus(btRoyale);
            return true;
        }
        else
        {
            UIScreen.SetFocus(btTourney);
            return true;
        }
    }
}