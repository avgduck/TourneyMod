using System.Collections.Generic;
using System.Linq;
using LLBML.Math;
using LLBML.Players;
using LLBML.Utils;
using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using TourneyMod.Rulesets;
using TourneyMod.SetTracking;
using UnityEngine;
using UnityEngine.Assertions;

namespace TourneyMod.UI.Menu;

internal class ScreenMenuSetPreview : ScreenUnlocksStages, ICustomScreen<ScreenUnlocksStages>, IMenuTitle
{
    internal List<LLButton> btGames;
    private List<Match> reversed;

    private const int FONTSIZE_TITLE = 32;
    private static readonly Vector2 SPACING_TITLE = new Vector2(0f, -FONTSIZE_TITLE);
    private const int FONTSIZE_HEADER = 22;
    private static readonly Vector2 SPACING_HEADER = new Vector2(0f, -FONTSIZE_HEADER);
    private const int FONTSIZE_MAIN = 14;
    private static readonly Vector2 SPACING_MAIN = new Vector2(0f, -FONTSIZE_MAIN);

    private static readonly Vector2 TOP = new Vector2(322f - 50f, 220f);
    private static readonly Vector2 LEFTCOL = new Vector2(TOP.x - 160f, TOP.y);
    private static readonly Vector2 RIGHTCOL = new Vector2(TOP.x + 160f, TOP.y);

    private TextMeshProUGUI lbInfo;

    private enum ButtonType
    {
        OVERVIEW,
        COMPLETED
    }

    public string GetCustomTitle()
    {
        return "CURRENT SET";
    }

    public void Init(ScreenUnlocksStages screenUnlocksStages)
    {
        screenType = screenUnlocksStages.screenType;
        layer = screenUnlocksStages.layer;
        isActive = screenUnlocksStages.isActive;
        msgEsc = screenUnlocksStages.msgEsc;
        msgMenu = screenUnlocksStages.msgMenu;
        msgCancel = screenUnlocksStages.msgCancel;

        btFirstButton = screenUnlocksStages.btFirstButton;
        pfModeButton = screenUnlocksStages.pfModeButton;
        pfPriceButton = screenUnlocksStages.pfPriceButton;
        pfBuyButton = screenUnlocksStages.pfBuyButton;
        characterBackdrop = screenUnlocksStages.characterBackdrop;
        lbCurrency = screenUnlocksStages.lbCurrency;
        lbName = screenUnlocksStages.lbName;
        lbDescription = screenUnlocksStages.lbDescription;
        imPreview = screenUnlocksStages.imPreview;
        pnBuy = screenUnlocksStages.pnBuy;
        RowYOffset = screenUnlocksStages.RowYOffset;
        RowXOffset = screenUnlocksStages.RowXOffset;
        priceButtons = screenUnlocksStages.priceButtons;
        btStageButtons = screenUnlocksStages.btStageButtons;
    }

    public override void OnOpen(ScreenType screenTypePrev)
    {
        characterBackdrop.transform.SetParent(OGKPCMDOMPF.screenMenu.tfBackgroundOverlay, true);
        characterBackdrop.transform.SetAsFirstSibling();

        LLButton btBuy = pfBuyButton.GetComponentInChildren<LLButton>();
        btBuy.visible = false;
        btStageButtons = [];
        lbDescription.SetText("");
        pnBuy.gameObject.SetActive(false);

        btGames = new List<LLButton>();
        
        LLButton btSetOverview = btFirstButton.transform.parent.gameObject.GetComponentInChildren<LLButton>();
        btGames.Add(btSetOverview);
        btSetOverview.SetText("Set Overview");
        btSetOverview.onHover = (playerNr) =>
        {
            AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
            if (UIInput.mainCursor.GetState() == CursorState.FOCUS)
            {
                SetSelectedButton(ButtonType.OVERVIEW, -1);
            }
        };
        btSetOverview.onClick = (playerNr) => {
            //if (UIInput.mainCursor.GetState() != CursorState.FOCUS) AudioHandler.PlayMenuSfx(Sfx.MENU_CONFIRM);
            SetSelectedButton(ButtonType.OVERVIEW, -1);
        };

        reversed = new List<Match>(SetTracker.Instance.CurrentSet.CompletedMatches);
        reversed.Reverse();
        int index = 0;
        reversed.ForEach(match =>
        {
            GameObject goButton;
            goButton = Instantiate(pfModeButton, base.transform, false);
            goButton.transform.localPosition = btFirstButton.transform.parent.localPosition + new Vector3(RowXOffset * (index+1), RowYOffset * (index+1), 0f);

            LLButton btGame = goButton.GetComponentInChildren<LLButton>();
            btGames.Add(btGame);
            btGame.SetText($"Game {match.GameNumber}" + (match.IsTiebreaker ? " Tiebreaker" : ""));
            int mi = index;
            btGame.onHover = (playerNr) =>
            {
                AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
                if (UIInput.mainCursor.GetState() == CursorState.FOCUS)
                {
                    SetSelectedButton(ButtonType.COMPLETED, mi);
                }
            };
            btGame.onClick = (playerNr) => {
                //if (UIInput.mainCursor.GetState() != CursorState.FOCUS) AudioHandler.PlayMenuSfx(Sfx.MENU_CONFIRM);
                SetSelectedButton(ButtonType.COMPLETED, mi);
            };

            index++;
        });

        lbName.transform.localPosition = TOP - SPACING_TITLE;
        lbName.alignment = TextAlignmentOptions.Center;

        UIUtils.CreateText(ref lbInfo, "lbInfo", transform, TOP + SPACING_TITLE);
        lbInfo.alignment = TextAlignmentOptions.Top;
        lbInfo.richText = true;
        lbInfo.fontSize = FONTSIZE_HEADER;
        
        btFirstButton.OnHover(-1);
        ((LLSelectionButton)btFirstButton).SetSelected(-1);
        btFirstButton.OnClickNoEffects(-1);
        btFirstButton.OnHoverOut(-1);
    }

    public override void UpdateText()
    { }

    public override void GetControls(ref List<LLClickable> list, bool vert, LLClickable curFocus, LLCursor cursor)
    {
        list.AddRange(btGames.Cast<LLClickable>());
    }

    private void SetSelectedButton(ButtonType buttonType, int matchIndex)
    {
        if (buttonType == ButtonType.OVERVIEW)
        {
            Set set = SetTracker.Instance.CurrentSet;
            lbName.SetText("Set Overview");

            string[] characterLock = Player.EPlayers().Where(p => !set.PlayerCharacterLock[((Player)p).nr].IsEmpty).Select(p => $"<color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[(int)SetTracker.Instance.GetPlayerTeam(((Player)p).nr)])}>{StringUtils.GetCharacterSafeName(set.PlayerCharacterLock[((Player)p).nr].character)}</color>").ToArray();
            string[] stockLock = Player.EPlayers().Where(p => set.PlayerStockLock[((Player)p).nr] != 0).Select(p => $"<color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[(int)SetTracker.Instance.GetPlayerTeam(((Player)p).nr)])}>{set.PlayerStockLock[((Player)p).nr]}</color>").ToArray();
            Stage stageLock = SetTracker.Instance.CurrentSet.StageLock;
            
            lbInfo.SetText(
                $"Mode: <color=\"yellow\">{Plugin.GetModeName(SetTracker.Instance.ActiveTourneyMode, true)}</color>"
                + $"\nRuleset: <color=\"yellow\">{set.ActiveRuleset.name}</color>"
                + $"\n\n<color=\"yellow\">Game {set.GameNumber}" + (set.IsTiebreaker ? " Tiebreaker" : "") + "</color>"
                + $"\nScore: <color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[0])}>{set.WinCounts[0]}</color><color=\"yellow\"> - </color><color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[1])}>{set.WinCounts[1]}</color>"
                + $"\n\nCharacter lock: <color=\"yellow\">{(characterLock.Length > 0 ? Plugin.PrintArray(characterLock, false) : "<color=\"yellow\">none</color>")}</color>"
                + $"\nStock lock: <color=\"yellow\">{(stockLock.Length > 0 ? Plugin.PrintArray(stockLock, false) : "<color=\"yellow\">none</color>")}</color>"
                + $"\nStage lock: <color=\"yellow\">{(stageLock != Stage.NONE ? StringUtils.GetStageReadableName(stageLock) : "none")}</color>"
            );
        }
        else if (buttonType == ButtonType.COMPLETED)
        {
            Match match = reversed[matchIndex];
            lbName.SetText($"Game {match.GameNumber}" + (match.IsTiebreaker ? " Tiebreaker" : ""));

            string[] characters = match.PlayerCharacters.Where(pc => !pc.IsEmpty).Select(pc => $"<color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[(int)pc.team])}>{StringUtils.GetCharacterSafeName(pc.character)}</color>").ToArray();
            string[] stocks = match.FinalScores.Where(ps => ps.Team != Team.NONE).Select(ps => $"<color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[(int)ps.Team])}>{ps.Stocks}</color>").ToArray();
            string[] hp = match.FinalScores.Where(ps => ps.Team != Team.NONE).Select(ps => $"<color=#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[(int)ps.Team])}>{Mathf.RoundToInt(Floatf.ToFloat(ps.Hp) * 100f)}%</color>").ToArray();
            
            lbInfo.SetText(
                $"Stage: <color=\"yellow\">{StringUtils.GetStageReadableName(match.PlayedStage)}</color>"
                + $"\nCharacters: <color=\"yellow\">{Plugin.PrintArray(characters, false)}</color>"
                + $"\n\nStocks: <color=\"yellow\">{Plugin.PrintArray(stocks, false)}</color>"
                + $"\nHP: <color=\"yellow\">{Plugin.PrintArray(hp, false)}</color>"
                + $"\nTimeout: <color=\"yellow\">{(match.IsTimeout ? "yes" : "no")}</color>"
                + $"\n\nWinner: <color={(match.Winner == Team.NONE ? "\"yellow\"" : $"#{ColorUtility.ToHtmlStringRGB(UIUtils.COLOR_TEAM[(int)match.Winner])}")}>{match.Winner}</color>"
            );
        }
    }
}