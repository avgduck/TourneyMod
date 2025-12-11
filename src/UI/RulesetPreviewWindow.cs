using System.Collections.Generic;
using LLGUI;
using LLHandlers;
using TMPro;
using TourneyMod.Rulesets;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI;

internal class RulesetPreviewWindow : MonoBehaviour
{
    private static readonly Vector2 POSITION = new Vector2(530f, 200f);
    private const float FONT_SIZE = 12f;
    private const float FONT_SIZE_2 = 10f;
    private const float SPACING = 20f;

    private const float LEFTCOL = -143f;
    private const float RIGHTCOL = 63f;
    private const float LEFT = 6f;

    internal static void Create(Transform tfParent)
    {
        RectTransform tf = LLControl.CreatePanel(tfParent, "rulesetPreviewUI", POSITION.x, POSITION.y);
        RulesetPreviewWindow window = tf.gameObject.AddComponent<RulesetPreviewWindow>();
        window.rectTransform = tf;
        window.Init();
    }

    private RectTransform rectTransform;
    private RectTransform tfContainer;
    private Image imgBg;

    private TextMeshProUGUI lbSelected;
    private TextMeshProUGUI lbRuleset;

    private List<TextMeshProUGUI> rulesetInfo;

    private TextMeshProUGUI lbName1;
    private TextMeshProUGUI lbName2;

    private TextMeshProUGUI lbNeutralStages1;
    private TextMeshProUGUI lbNeutralStages2;
    
    private TextMeshProUGUI lbCounterpickStages1;
    private TextMeshProUGUI lbCounterpickStages2;

    private TextMeshProUGUI lbDsrMode1;
    private TextMeshProUGUI lbDsrMode2;

    private TextMeshProUGUI lbRandomMode1;
    private TextMeshProUGUI lbRandomMode2;

    private TextMeshProUGUI lbBanOrder1;
    private TextMeshProUGUI lbBanOrder2;

    private void Init()
    {
        tfContainer = LLControl.CreatePanel(rectTransform, "container", 0f, 0f);
        imgBg = LLControl.CreateImage(tfContainer, Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f)));
        imgBg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200f);
        imgBg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 440f);
        imgBg.rectTransform.ForceUpdateRectTransforms();
        imgBg.rectTransform.pivot = new Vector2(0.5f, 1f);
        imgBg.rectTransform.localPosition = new Vector2(0f, 30f);
        imgBg.color = Color.black;
        
        
        lbSelected = LLControl.CreatePanel(tfContainer, "lbSelected", LEFTCOL, 0f).gameObject.AddComponent<TextMeshProUGUI>();
        lbSelected.fontSize = FONT_SIZE;
        lbSelected.enableWordWrapping = false;
        lbSelected.alignment = TextAlignmentOptions.TopRight;
        lbSelected.color = Color.white;
        lbSelected.SetText("Ruleset:");
        
        lbRuleset = LLControl.CreatePanel(tfContainer, "lbRuleset", RIGHTCOL, 0f).gameObject.AddComponent<TextMeshProUGUI>();
        lbRuleset.fontSize = FONT_SIZE;
        lbRuleset.enableWordWrapping = false;
        lbRuleset.alignment = TextAlignmentOptions.TopLeft;
        lbRuleset.color = Color.red;
        lbRuleset.SetText("NULL");

        rulesetInfo = new List<TextMeshProUGUI>();
        
        lbName1 = LLControl.CreatePanel(tfContainer, "lbName1", LEFT, -SPACING * (1)).gameObject.AddComponent<TextMeshProUGUI>();
        lbName1.fontSize = FONT_SIZE;
        lbName1.enableWordWrapping = false;
        lbName1.alignment = TextAlignmentOptions.TopLeft;
        lbName1.color = Color.white;
        lbName1.SetText("Name:");
        rulesetInfo.Add(lbName1);
        
        lbName2 = LLControl.CreatePanel(tfContainer, "lbName2", LEFT, -SPACING * (1) - FONT_SIZE).gameObject.AddComponent<TextMeshProUGUI>();
        lbName2.fontSize = FONT_SIZE_2;
        lbName2.enableWordWrapping = false;
        lbName2.alignment = TextAlignmentOptions.TopLeft;
        lbName2.color = Color.yellow;
        lbName2.SetText("");
        rulesetInfo.Add(lbName2);
        
        lbNeutralStages1 = LLControl.CreatePanel(tfContainer, "lbNeutralStages", LEFT, -SPACING * (2) - FONT_SIZE_2 * (1)).gameObject.AddComponent<TextMeshProUGUI>();
        lbNeutralStages1.fontSize = FONT_SIZE;
        lbNeutralStages1.enableWordWrapping = false;
        lbNeutralStages1.alignment = TextAlignmentOptions.TopLeft;
        lbNeutralStages1.color = Color.white;
        lbNeutralStages1.SetText("Neutral stages:");
        rulesetInfo.Add(lbNeutralStages1);
        
        lbNeutralStages2 = LLControl.CreatePanel(tfContainer, "lbNeutralStageCount", LEFT, -SPACING * (2) - FONT_SIZE_2 * (1) - FONT_SIZE).gameObject.AddComponent<TextMeshProUGUI>();
        lbNeutralStages2.fontSize = FONT_SIZE_2;
        lbNeutralStages2.enableWordWrapping = false;
        lbNeutralStages2.alignment = TextAlignmentOptions.TopLeft;
        lbNeutralStages2.color = Color.yellow;
        lbNeutralStages2.SetText("");
        rulesetInfo.Add(lbNeutralStages2);
        
        lbCounterpickStages1 = LLControl.CreatePanel(tfContainer, "lbCounterpickStages", LEFT, -SPACING * (3) - FONT_SIZE_2 * (1+10)).gameObject.AddComponent<TextMeshProUGUI>();
        lbCounterpickStages1.fontSize = FONT_SIZE;
        lbCounterpickStages1.enableWordWrapping = false;
        lbCounterpickStages1.alignment = TextAlignmentOptions.TopLeft;
        lbCounterpickStages1.color = Color.white;
        lbCounterpickStages1.SetText("Counterpick stages:");
        rulesetInfo.Add(lbCounterpickStages1);

        lbCounterpickStages2 = LLControl.CreatePanel(tfContainer, "lbCounterpickStageCount", LEFT, -SPACING * (3) - FONT_SIZE_2 * (1+10) - FONT_SIZE).gameObject.AddComponent<TextMeshProUGUI>();
        lbCounterpickStages2.fontSize = FONT_SIZE_2;
        lbCounterpickStages2.enableWordWrapping = false;
        lbCounterpickStages2.alignment = TextAlignmentOptions.TopLeft;
        lbCounterpickStages2.color = Color.yellow;
        lbCounterpickStages2.SetText("");
        rulesetInfo.Add(lbCounterpickStages2);
        
        lbDsrMode1 = LLControl.CreatePanel(tfContainer, "lbDsrMode1", LEFT, -SPACING * (4) - FONT_SIZE_2 * (1+20)).gameObject.AddComponent<TextMeshProUGUI>();
        lbDsrMode1.fontSize = FONT_SIZE;
        lbDsrMode1.enableWordWrapping = false;
        lbDsrMode1.alignment = TextAlignmentOptions.TopLeft;
        lbDsrMode1.color = Color.white;
        lbDsrMode1.SetText("DSR:");
        rulesetInfo.Add(lbDsrMode1);

        lbDsrMode2 = LLControl.CreatePanel(tfContainer, "lbDsrMode2", LEFT, -SPACING * (4) - FONT_SIZE_2 * (1+20) - FONT_SIZE).gameObject.AddComponent<TextMeshProUGUI>();
        lbDsrMode2.fontSize = FONT_SIZE_2;
        lbDsrMode2.enableWordWrapping = false;
        lbDsrMode2.alignment = TextAlignmentOptions.TopLeft;
        lbDsrMode2.color = Color.yellow;
        lbDsrMode2.SetText("");
        rulesetInfo.Add(lbDsrMode2);
        
        lbRandomMode1 = LLControl.CreatePanel(tfContainer, "lbRandomMode1", LEFT, -SPACING * (5) - FONT_SIZE_2 * (2+20)).gameObject.AddComponent<TextMeshProUGUI>();
        lbRandomMode1.fontSize = FONT_SIZE;
        lbRandomMode1.enableWordWrapping = false;
        lbRandomMode1.alignment = TextAlignmentOptions.TopLeft;
        lbRandomMode1.color = Color.white;
        lbRandomMode1.SetText("Random:");
        rulesetInfo.Add(lbRandomMode1);

        lbRandomMode2 = LLControl.CreatePanel(tfContainer, "lbRandomMode2", LEFT, -SPACING * (5) - FONT_SIZE_2 * (2+20) - FONT_SIZE).gameObject.AddComponent<TextMeshProUGUI>();
        lbRandomMode2.fontSize = FONT_SIZE_2;
        lbRandomMode2.enableWordWrapping = false;
        lbRandomMode2.alignment = TextAlignmentOptions.TopLeft;
        lbRandomMode2.color = Color.yellow;
        lbRandomMode2.SetText("");
        rulesetInfo.Add(lbRandomMode2);
        
        
        lbBanOrder1 = LLControl.CreatePanel(tfContainer, "lbBanOrder1", LEFT, -SPACING * (6) - FONT_SIZE_2 * (3+20)).gameObject.AddComponent<TextMeshProUGUI>();
        lbBanOrder1.fontSize = FONT_SIZE;
        lbBanOrder1.enableWordWrapping = false;
        lbBanOrder1.alignment = TextAlignmentOptions.TopLeft;
        lbBanOrder1.color = Color.white;
        lbBanOrder1.SetText("Ban order:");
        rulesetInfo.Add(lbBanOrder1);
        
        lbBanOrder2 = LLControl.CreatePanel(tfContainer, "lbBanOrder2", LEFT, -SPACING * (6) - FONT_SIZE_2 * (3+20) - FONT_SIZE).gameObject.AddComponent<TextMeshProUGUI>();
        lbBanOrder2.fontSize = FONT_SIZE_2;
        lbBanOrder2.enableWordWrapping = false;
        lbBanOrder2.alignment = TextAlignmentOptions.TopLeft;
        lbBanOrder2.color = Color.yellow;
        lbBanOrder2.SetText("");
        rulesetInfo.Add(lbBanOrder2);
        
        tfContainer.gameObject.SetActive(false);
    }

    private string GetStageList(Stage[] stages)
    {
        string s = "";

        for (int stageIndex = 0; stageIndex < stages.Length; stageIndex++)
        {
            if (stageIndex != 0) s += $",{(stageIndex % 2 == 0 ? "\n" : " ")}";
            s += LLBML.Utils.StringUtils.GetStageReadableName(stages[stageIndex]);
        }
        
        return s;
    }

    private string GetBanOrder(Ruleset ruleset)
    {
        string s = "";

        if (ruleset.banAmounts.Length == 0)
        {
            s += "Free pick";
        }
        else
        {
            s += $"(Player {ruleset.game1FirstPlayer+1} starts for G1)\n";
            int gameIndex = 0;
            foreach (int[] banNums in ruleset.banAmounts)
            {
                s += $"G{gameIndex + 1}{(gameIndex == ruleset.banAmounts.Length - 1 ? "+" : "")} ";
                bool bansIncluded = true;
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
    
    private void Update()
    {
        if (!Patches.RulesetPreviewPatch.isInModMenu)
        {
            tfContainer.gameObject.SetActive(false);
            return;
        }

        Ruleset ruleset = Plugin.Instance.selectedRuleset;
        tfContainer.gameObject.SetActive(true);
        lbRuleset.color = ruleset == null ? Color.red : Color.green;
        lbRuleset.SetText(ruleset == null ? "NULL" : ruleset.Id);

        if (ruleset == null)
        {
            rulesetInfo.ForEach(lb => lb.gameObject.SetActive(false));
            return;
        }
        rulesetInfo.ForEach(lb => lb.gameObject.SetActive(true));
        
        lbName2.SetText(ruleset.name);
        lbNeutralStages2.SetText(GetStageList(ruleset.stagesNeutral));
        lbCounterpickStages2.SetText(GetStageList(ruleset.stagesCounterpick));
        lbDsrMode2.SetText(ruleset.dsrMode switch
        {
            Ruleset.DsrMode.OFF => "OFF",
            Ruleset.DsrMode.FULL_SET => "ON, includes all wins",
            Ruleset.DsrMode.LAST_WIN => "ON, only last win"
        });
        lbRandomMode2.SetText(ruleset.randomMode switch
        {
            Ruleset.RandomMode.OFF => "OFF",
            Ruleset.RandomMode.ANY => "ON, any stage (3D/2D)",
            Ruleset.RandomMode.ANY_3D => "ON, any 3D stage (vanilla)",
            Ruleset.RandomMode.ANY_2D => "ON, any 2D stage",
            Ruleset.RandomMode.ANY_LEGAL => "ON, any legal stage"
        });
       lbBanOrder2.SetText(GetBanOrder(ruleset));
    }
}