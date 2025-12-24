using System.Collections.Generic;
using LLBML.States;
using LLGUI;
using LLHandlers;
using LLScreen;
using TourneyMod.SetTracking;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI;

internal class ScreenMenuTourney : ScreenMenuVersus, ICustomScreen<ScreenMenuVersus>, IMenuTitle
{
    private static readonly Vector3 OFFSET_BUTTON_1DOWN = new Vector2(-16.3f, -88.8f);
    private static readonly Vector3 OFFSET_RIGHTCOL = new Vector2(406.3f, 0f);
    
    private static readonly Vector3 SETPREVIEW_OFFSET = new Vector2(60f, 0f);
    
    private LLButton btLocal1v1;
    private LLButton btLocalDoubles;
    private LLButton btLocalCrew;

    private LLButton btOnline1v1;

    private LLButton btEndSet;

    private SetPreviewWindow pnSetPreview;
    private RulesetPreviewWindow pnRulesetPreview;

    public string GetCustomTitle()
    {
        return "TOURNAMENT";
    }

    public void Init(ScreenMenuVersus screenVanilla)
    {
        screenType = screenVanilla.screenType;
        layer = screenVanilla.layer;
        isActive = screenVanilla.isActive;
        msgEsc = screenVanilla.msgEsc;
        msgMenu = screenVanilla.msgMenu;
        msgCancel = screenVanilla.msgCancel;
        
        btRoyale = screenVanilla.btRoyale;
        bt1v1 = screenVanilla.bt1v1;
        btTeams = screenVanilla.btTeams;
        btVolley = screenVanilla.btVolley;
        btStrikers = screenVanilla.btStrikers;
    }

    public override void OnOpen(ScreenType screenTypePrev)
    {
        base.OnOpen(screenTypePrev);
        
        btLocal1v1 = Instantiate(btRoyale, transform);
        btLocal1v1.name = "btLocal1v1";
        btLocal1v1.onClick = (playerNr) =>
        {
            SetTracker.Instance.SetTourneyMode(TourneyMode.LOCAL_1V1);
            GameStates.Send(Msg.SEL_1V1, playerNr, -1);
        };
        btLocal1v1.SetText("local 1v1");

        btLocalDoubles = Instantiate(bt1v1, transform);
        btLocalDoubles.name = "btLocalDoubles";
        btLocalDoubles.onClick = (playerNr) =>
        {
            SetTracker.Instance.SetTourneyMode(TourneyMode.LOCAL_DOUBLES);
            GameStates.Send(Msg.SEL_TEAMS, playerNr, -1);
        };
        btLocalDoubles.SetText("local doubles");
        
        btLocalCrew = Instantiate(btTeams, transform);
        btLocalCrew.name = "btLocalCrew";
        btLocalCrew.onClick = (playerNr) =>
        {
            SetTracker.Instance.SetTourneyMode(TourneyMode.LOCAL_CREW);
            GameStates.Send(Msg.SEL_1V1, playerNr, -1);
        };
        btLocalCrew.SetText("crew battle");
        
        btOnline1v1 = Instantiate(btRoyale, transform);
        btOnline1v1.transform.localPosition += OFFSET_RIGHTCOL;
        btOnline1v1.name = "btOnline1v1";
        btOnline1v1.onClick = (playerNr) =>
        {
            SetTracker.Instance.SetTourneyMode(TourneyMode.ONLINE_1V1);
            GameStates.Send(Msg.SEL_RANKED, playerNr, -1);
        };
        btOnline1v1.SetText("online 1v1");

        btEndSet = Instantiate(btTeams, transform);
        btEndSet.transform.localPosition += OFFSET_BUTTON_1DOWN * 2;
        btEndSet.name = "btEndSet";
        btEndSet.onClick = (playerNr) =>
        {
            SetTracker.Instance.SetTourneyMode(TourneyMode.NONE);
        };
        btEndSet.SetText("end set");
        
        btEndSet.SetActive(SetTracker.Instance.IsTrackingSet && SetTracker.Instance.ActiveTourneyMode != TourneyMode.NONE);
        
        btRoyale.visible = false;
        bt1v1.visible = false;
        btTeams.visible = false;
        btVolley.visible = false;
        btStrikers.visible = false;
        
        //btLocalCrew.SetActive(false);
        //btOnline1v1.SetActive(false);

        SetPreviewWindow.Create(ref pnSetPreview, transform, btEndSet.transform.localPosition + OFFSET_RIGHTCOL + SETPREVIEW_OFFSET);
        pnRulesetPreview = RulesetPreviewWindow.Create(transform, true);
    }

    public override void GetControls(ref List<LLClickable> list, bool vert, LLClickable curFocus, LLCursor cursor)
    {
        list.Add(btLocal1v1);
        list.Add(btLocalDoubles);
        list.Add(btLocalCrew);
        
        list.Add(btEndSet);
        
        list.Add(btOnline1v1);
    }

    public override LLClickable GetDefaultFocus(LLCursor cursor)
    {
        return btLocal1v1;
    }

    public override bool DirectMove(Vector2 move, LLClickable curFocus, bool shouldMove)
    {
        if (!shouldMove) return false;
        bool vert = Mathf.Abs(move.y) > Mathf.Abs(move.x);
        float sign = vert ? Mathf.Sign(move.y) : Mathf.Sign(move.x);

        if (vert) return false;

        if (curFocus == btOnline1v1)
        {
            UIScreen.SetFocus(btLocal1v1);
            AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
            return true;
        }
        else
        {
            UIScreen.SetFocus(btOnline1v1);
            AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
            return true;
        }
    }
}