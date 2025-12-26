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
    
    internal LLButton btLocal1v1;
    internal LLButton btLocalDoubles;
    internal LLButton btLocalCrew;

    internal LLButton btOnline1v1;

    internal LLButton btEndSet;
    internal LLButton btRulesets;

    internal SetPreviewWindow pnSetPreview;

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
            Plugin.Instance.RulesetsMenuOpen = false;
            SetTracker.Instance.ActiveTourneyMode = TourneyMode.LOCAL_1V1;
            GameStates.Send(Msg.SEL_1V1, playerNr, -1);
        };
        btLocal1v1.SetText(Plugin.GetModeName(TourneyMode.LOCAL_1V1));

        btLocalDoubles = Instantiate(bt1v1, transform);
        btLocalDoubles.name = "btLocalDoubles";
        btLocalDoubles.onClick = (playerNr) =>
        {
            Plugin.Instance.RulesetsMenuOpen = false;
            SetTracker.Instance.ActiveTourneyMode = TourneyMode.LOCAL_DOUBLES;
            GameStates.Send(Msg.SEL_TEAMS, playerNr, -1);
        };
        btLocalDoubles.SetText(Plugin.GetModeName(TourneyMode.LOCAL_DOUBLES));
        
        btLocalCrew = Instantiate(btTeams, transform);
        btLocalCrew.name = "btLocalCrew";
        btLocalCrew.onClick = (playerNr) =>
        {
            Plugin.Instance.RulesetsMenuOpen = false;
            SetTracker.Instance.ActiveTourneyMode = TourneyMode.LOCAL_CREW;
            GameStates.Send(Msg.SEL_1V1, playerNr, -1);
        };
        btLocalCrew.SetText(Plugin.GetModeName(TourneyMode.LOCAL_CREW));
        
        btOnline1v1 = Instantiate(btRoyale, transform);
        btOnline1v1.transform.localPosition += OFFSET_RIGHTCOL;
        btOnline1v1.name = "btOnline1v1";
        btOnline1v1.onClick = (playerNr) =>
        {
            Plugin.Instance.RulesetsMenuOpen = false;
            SetTracker.Instance.ActiveTourneyMode = TourneyMode.ONLINE_1V1;
            GameStates.Send(Msg.SEL_RANKED, playerNr, -1);
        };
        btOnline1v1.SetText(Plugin.GetModeName(TourneyMode.ONLINE_1V1));
        
        btRulesets = Instantiate(btTeams, transform);
        btRulesets.transform.localPosition += OFFSET_BUTTON_1DOWN * 2;
        btRulesets.name = "btRulesets";
        btRulesets.onClick = (playerNr) =>
        {
            Plugin.Instance.RulesetsMenuOpen = true;
            GameStates.Set(GameState.UNLOCKS);
            GameStates.Send(Msg.SEL_STAGES, playerNr, -1);
        };
        btRulesets.SetText("rulesets");

        btEndSet = Instantiate(btTeams, transform);
        btEndSet.transform.localPosition += OFFSET_BUTTON_1DOWN * 2 + OFFSET_RIGHTCOL;
        btEndSet.name = "btEndSet";
        btEndSet.onClick = (playerNr) =>
        {
            Plugin.Instance.RulesetsMenuOpen = false;
            SetTracker.Instance.ActiveTourneyMode = TourneyMode.NONE;
            SetTracker.Instance.End();
            pnSetPreview.UpdateText();
            UpdateButtons();
        };
        btEndSet.SetText("end set");
        
        SetPreviewWindow.Create(ref pnSetPreview, transform, btEndSet.transform.localPosition + OFFSET_BUTTON_1DOWN);
        
        UpdateButtons();
        
        btRoyale.visible = false;
        bt1v1.visible = false;
        btTeams.visible = false;
        btVolley.visible = false;
        btStrikers.visible = false;
    }

    public override void GetControls(ref List<LLClickable> list, bool vert, LLClickable curFocus, LLCursor cursor)
    {
        list.Add(btLocal1v1);
        list.Add(btLocalDoubles);
        list.Add(btLocalCrew);
        
        list.Add(btRulesets);
        
        list.Add(btOnline1v1);
        
        list.Add(btEndSet);
    }

    public override LLClickable GetDefaultFocus(LLCursor cursor)
    {
        if (Plugin.Instance.RulesetsMenuOpen) return btRulesets;
        if (btLocal1v1.isActive) return btLocal1v1;
        if (btLocalDoubles.isActive) return btLocalDoubles;
        if (btLocalCrew.isActive) return btLocalCrew;
        if (btOnline1v1.isActive) return btOnline1v1;
        return btRulesets;
    }

    public override bool DirectMove(Vector2 move, LLClickable curFocus, bool shouldMove)
    {
        if (!shouldMove) return false;
        bool vert = Mathf.Abs(move.y) > Mathf.Abs(move.x);
        float sign = vert ? Mathf.Sign(move.y) : Mathf.Sign(move.x);

        if (vert) return false;

        if (curFocus == btLocal1v1 || curFocus == btLocalDoubles || curFocus == btLocalCrew)
        {
            UIScreen.SetFocus(btOnline1v1);
            AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
            return true;
        }
        if (curFocus == btOnline1v1)
        {
            UIScreen.SetFocus(btLocal1v1);
            AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
            return true;
        }
        
        if (curFocus == btRulesets)
        {
            UIScreen.SetFocus(btEndSet);
            AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
            return true;
        }
        if (curFocus == btEndSet)
        {
            UIScreen.SetFocus(btRulesets);
            AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
            return true;
        }

        return false;
    }

    private void UpdateButtons()
    {
        btLocal1v1.SetActive(SetTracker.Instance.ActiveTourneyMode is TourneyMode.LOCAL_1V1 or TourneyMode.NONE);
        btLocalDoubles.SetActive(SetTracker.Instance.ActiveTourneyMode is TourneyMode.LOCAL_DOUBLES or TourneyMode.NONE);
        btLocalCrew.SetActive(SetTracker.Instance.ActiveTourneyMode is TourneyMode.LOCAL_CREW or TourneyMode.NONE);
        btOnline1v1.SetActive(SetTracker.Instance.ActiveTourneyMode is TourneyMode.ONLINE_1V1 or TourneyMode.NONE);
        btEndSet.SetActive(SetTracker.Instance.ActiveTourneyMode is not TourneyMode.NONE);
        
        UIScreen.SetFocus(GetDefaultFocus(UIInput.mainCursor));
        if (!btEndSet.isActive) btEndSet.OnHoverOut(-1);
        
        //btLocalCrew.SetActive(false);
        btOnline1v1.SetActive(false);
    }
}