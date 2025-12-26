using System.Collections.Generic;
using System.Linq;
using LLBML.Utils;
using LLGUI;
using LLHandlers;
using LLScreen;
using TMPro;
using TourneyMod.Rulesets;
using UnityEngine;

namespace TourneyMod.UI;

internal class ScreenMenuRulesets : ScreenUnlocksStages, ICustomScreen<ScreenUnlocksStages>, IMenuTitle
{
    internal List<LLButton> btRulesets;

    private const int FONTSIZE_RULESET = 32;
    private static readonly Vector2 SPACING_RULESET = new Vector2(0f, -FONTSIZE_RULESET);
    private const int FONTSIZE_HEADER = 24;
    private static readonly Vector2 SPACING_HEADER = new Vector2(0f, -FONTSIZE_HEADER);
    private const int FONTSIZE_MAIN = 16;
    private static readonly Vector2 SPACING_MAIN = new Vector2(0f, -FONTSIZE_MAIN);

    private static readonly Vector2 TOP = new Vector2(322f - 50f, 210f);
    private static readonly Vector2 LEFTCOL = new Vector2(TOP.x - 160f, TOP.y);
    private static readonly Vector2 RIGHTCOL = new Vector2(TOP.x + 160f, TOP.y);

    private TextMeshProUGUI lbRuleset;

    private TextMeshProUGUI lbStagesNeutral;
    private TextMeshProUGUI lbStagesCounterpick;

    private TextMeshProUGUI lbBanOrder;

    private TextMeshProUGUI lbDsrMode;
    private TextMeshProUGUI lbRandomMode;

    public string GetCustomTitle()
    {
        return "RULESETS";
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
        TextHandler.SetText(lbDescription, "");
        lbCurrency.gameObject.SetActive(false);

        btRulesets = new List<LLButton>();
        int index = 0;
        Plugin.Instance.SelectedRulesets.ToList().ForEach(entry =>
        {
            TourneyMode mode = entry.Key;
            Ruleset ruleset = entry.Value;

            GameObject goButton;
            if (index == 0)
            {
                goButton = btFirstButton.transform.parent.gameObject;
            }
            else
            {
                goButton = Instantiate(pfModeButton, base.transform, false);
                goButton.transform.localPosition = btFirstButton.transform.parent.localPosition + new Vector3(RowXOffset * index, RowYOffset * index, 0f);
            }

            LLButton btRuleset = goButton.GetComponentInChildren<LLButton>();
            btRulesets.Add(btRuleset);
            btRuleset.SetText(Plugin.GetModeName(mode, true));
            btRuleset.onHover = (playerNr) =>
            {
                AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
                if (UIInput.mainCursor.GetState() == CursorState.FOCUS)
                {
                    SetSelectedButton(mode);
                }
            };
            btRuleset.onClick = (playerNr) => {
                //if (UIInput.mainCursor.GetState() != CursorState.FOCUS) AudioHandler.PlayMenuSfx(Sfx.MENU_CONFIRM);
                SetSelectedButton(mode);
            };

            index++;
        });

        lbName.transform.localPosition = TOP - SPACING_RULESET;
        lbName.alignment = TextAlignmentOptions.Center;
        
        UIUtils.CreateText(ref lbRuleset, "lbRuleset", transform, TOP);
        lbRuleset.fontSize = FONTSIZE_RULESET;
        lbRuleset.richText = true;
        
        UIUtils.CreateText(ref lbStagesNeutral, "lbStagesNeutral", transform, LEFTCOL + SPACING_RULESET + SPACING_HEADER);
        lbStagesNeutral.fontSize = FONTSIZE_HEADER;
        lbStagesNeutral.alignment = TextAlignmentOptions.Top;
        lbStagesNeutral.richText = true;
        UIUtils.CreateText(ref lbStagesCounterpick, "lbStagesCounterpick", transform, RIGHTCOL + SPACING_RULESET + SPACING_HEADER);
        lbStagesCounterpick.fontSize = FONTSIZE_HEADER;
        lbStagesCounterpick.alignment = TextAlignmentOptions.Top;
        lbStagesCounterpick.richText = true;
        
        UIUtils.CreateText(ref lbBanOrder, "lbBanOrder", transform, TOP + SPACING_RULESET + SPACING_HEADER * 4);
        lbBanOrder.fontSize = FONTSIZE_HEADER;
        lbBanOrder.alignment = TextAlignmentOptions.Top;
        lbBanOrder.richText = true;
        
        UIUtils.CreateText(ref lbDsrMode, "lbDsrMode", transform, TOP + SPACING_RULESET + SPACING_HEADER * 6);
        lbDsrMode.fontSize = FONTSIZE_HEADER;
        lbDsrMode.alignment = TextAlignmentOptions.Top;
        lbDsrMode.richText = true;
        UIUtils.CreateText(ref lbRandomMode, "lbRandomMode", transform, TOP + SPACING_RULESET + SPACING_HEADER * 8);
        lbRandomMode.fontSize = FONTSIZE_HEADER;
        lbRandomMode.alignment = TextAlignmentOptions.Top;
        lbRandomMode.richText = true;
        
        btFirstButton.OnHover(-1);
        ((LLSelectionButton)btFirstButton).SetSelected(-1);
        btFirstButton.OnClickNoEffects(-1);
        btFirstButton.OnHoverOut(-1);
    }

    public override void UpdateText()
    { }

    public override void GetControls(ref List<LLClickable> list, bool vert, LLClickable curFocus, LLCursor cursor)
    {
        list.AddRange(btRulesets.Cast<LLClickable>());
    }
    
    private string GetBanOrder(Ruleset ruleset)
    {
        string s = "";

        if (ruleset.banAmounts.Length == 0)
        {
            s += $"<size={FONTSIZE_HEADER}pt>Free pick</size>";
        }
        else
        {
            s += $"<size={FONTSIZE_HEADER}pt>(Player {ruleset.game1FirstPlayer+1} starts for Game 1)</size>\n";
            int gameIndex = 0;
            foreach (int[] banNums in ruleset.banAmounts)
            {
                s += $"Game {gameIndex + 1}{(gameIndex == ruleset.banAmounts.Length - 1 ? "+" : "")}: ";
                for (int i = 0; i < banNums.Length; i++)
                {
                    if (banNums[i] == 0)
                    {
                        s += $"{(ruleset.laterGamesFirstPlayer == Ruleset.FirstPlayer.WINNER ? "W" : "L")} picks";
                        break;
                    }

                    if (i != 0) s += "-";
                    for (int j = 0; j < banNums[i]; j++)
                    {
                        s += (i % 2 == 0) ? (ruleset.laterGamesFirstPlayer == Ruleset.FirstPlayer.WINNER ? "W" : "L") : (ruleset.laterGamesFirstPlayer == Ruleset.FirstPlayer.WINNER ? "L" : "W");
                    }
                }

                s += "\n";
                gameIndex++;
            }
        }

        return s;
    }

    private void SetSelectedButton(TourneyMode tourneyMode)
    {
        Ruleset ruleset = Plugin.Instance.SelectedRulesets[tourneyMode];
        
        lbName.SetText(Plugin.GetModeName(tourneyMode, true));
        lbRuleset.SetText($"Ruleset: <color=\"yellow\">{ruleset.name}</color>");

        int linesNeutral = 0;
        string sNeutral = "";
        for (int i = 0; i < ruleset.stagesNeutral.Count; i++)
        {
            if (i != 0)
            {
                if (i % 2 == 0)
                {
                    sNeutral += "\n";
                    linesNeutral++;
                }
                else sNeutral += ", ";
            }
            sNeutral += StringUtils.GetStageReadableName(ruleset.stagesNeutral[i]);
        }
        lbStagesNeutral.SetText($"Neutral stages:\n<color=\"yellow\"><size={FONTSIZE_MAIN}pt>{sNeutral}</size></color>");

        int linesCounterpick = 0;
        string sCounterpick = "";
        for (int i = 0; i < ruleset.stagesCounterpick.Count; i++)
        {
            if (i != 0)
            {
                if (i % 2 == 0)
                {
                    sCounterpick += "\n";
                    linesCounterpick++;
                }
                else sCounterpick += ", ";
            }
            sCounterpick += StringUtils.GetStageReadableName(ruleset.stagesCounterpick[i]);
        }
        lbStagesCounterpick.SetText($"Counterpick stages:\n<color=\"yellow\"><size={FONTSIZE_MAIN}pt>{sCounterpick}</size></color>");
        int linesStages = Mathf.Max(linesNeutral, linesCounterpick);

        lbBanOrder.transform.localPosition = TOP + SPACING_RULESET + SPACING_HEADER * 4 + SPACING_MAIN * linesStages;
        lbBanOrder.SetText($"Ban order: <color=\"yellow\"><size={FONTSIZE_MAIN}pt>{GetBanOrder(ruleset)}</size></color>");
        int linesBans = ruleset.banAmounts.Length;

        lbDsrMode.transform.localPosition = TOP + SPACING_RULESET + SPACING_HEADER * 6 + SPACING_MAIN * linesStages + SPACING_MAIN * linesBans;
        lbDsrMode.SetText($"DSR: <color=\"yellow\">{ruleset.dsrMode switch {
            Ruleset.DsrMode.FULL_SET => $"ON\n<size={FONTSIZE_MAIN}pt>(includes all wins)</size>",
            Ruleset.DsrMode.LAST_WIN => $"ON\n<size={FONTSIZE_MAIN}pt>(only last win)</size>",
            _ => "OFF"
        }}</color>");
        int linesDsr = ruleset.dsrMode == Ruleset.DsrMode.OFF ? 0 : 1;

        lbRandomMode.transform.localPosition = TOP + SPACING_RULESET + SPACING_HEADER * 8 + SPACING_MAIN * linesStages + SPACING_MAIN * linesBans + SPACING_MAIN * linesDsr;
        lbRandomMode.SetText($"Random: <color=\"yellow\">{ruleset.randomMode switch {
            Ruleset.RandomMode.ANY_3D => $"ON\n<size={FONTSIZE_MAIN}pt>(any 3D stage)</size>",
            Ruleset.RandomMode.ANY_2D => $"ON\n<size={FONTSIZE_MAIN}pt>(any 2D stage)</size>",
            Ruleset.RandomMode.BOTH => $"ON\n<size={FONTSIZE_MAIN}pt>(both any 3D/any 2D stage options)</size>",
            Ruleset.RandomMode.ANY => $"ON\n<size={FONTSIZE_MAIN}pt>(any stage)</size>",
            Ruleset.RandomMode.ANY_LEGAL => $"ON\n<size={FONTSIZE_MAIN}pt>(any legal stage)</size>",
            _ => "OFF"
        }}</color>");
    }
}