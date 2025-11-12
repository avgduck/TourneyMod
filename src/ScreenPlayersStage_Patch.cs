using HarmonyLib;
using LLGUI;
using LLHandlers;
using LLScreen;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TourneyMod
{
    [HarmonyPatch]
    internal class ScreenPlayersStage_Patch
    {

        static List<LLButton> stageButtons3D = new List<LLButton>(10);
        static List<LLButton> stageButtons2D = new List<LLButton>(7);
        static LLButton[] stageButtonsRND = new LLButton[2];

        static TMP_Text lbStageName;
        static TMP_Text lbStageSize;

        const float SCALE_FACTOR = 0.415f;
        const float SPACING = 5f;
        const float SIZE_X = 500 * SCALE_FACTOR;
        const float SIZE_Y = 250 * SCALE_FACTOR;
        const float RND_START_POS_X = -536 + SPACING;
        const float START_POS_X = RND_START_POS_X + SIZE_X + SPACING;
        const float START_POS_Y = 89f;


        static string GetStageSize(Stage stage)
        {
            switch (stage)
            {
                case Stage.OUTSKIRTS:
                case Stage.SEWERS:
                    return "1240, 510";
                case Stage.JUNKTOWN:
                    return "1130, 510";
                case Stage.CONSTRUCTION:
                    return "1492, 522";
                case Stage.FACTORY:
                    return "1400, 542";
                case Stage.SUBWAY:
                    return "1050, 510";
                case Stage.STADIUM:
                    return "1230, 540";
                case Stage.STREETS:
                    return "1320, 515";
                case Stage.POOL:
                    return "1210, 575";
                case Stage.ROOM21:
                    return "1100, 550";
                default:
                    return "";
            }
        }

        static string UnlockStageText(Stage stage)
        {
            int matchesNeeded = 0;
            int levelsNeeded = 0;

            switch (stage)
            {
                case Stage.STREETS_2D:
                    matchesNeeded = 500;
                    levelsNeeded = 70;
                    break;
                case Stage.FACTORY_2D:
                    matchesNeeded = 670;
                    levelsNeeded = 90;
                    break;
                case Stage.SEWERS_2D:
                    matchesNeeded = 840;
                    levelsNeeded = 110;
                    break;
                case Stage.ROOM21_2D:
                    matchesNeeded = 1010;
                    levelsNeeded = 130;
                    break;
                case Stage.SUBWAY_2D:
                    matchesNeeded = 1180;
                    levelsNeeded = 150;
                    break;
                case Stage.POOL_2D:
                    matchesNeeded = 1350;
                    levelsNeeded = 170;
                    break;
                case Stage.OUTSKIRTS_2D:
                    matchesNeeded = 1520;
                    levelsNeeded = 190;
                    break;
                default:
                    return TextHandler.Get($"STAGE_{stage}");
            }

            return TextHandler.Get("MAIN_TXT_MATCHES", new string[] { matchesNeeded.ToString() })
                + " || " + TextHandler.Get("MAIN_TXT_LEVEL", new string[] { levelsNeeded.ToString() });
        }


        [HarmonyPatch(typeof(ScreenPlayersStage), nameof(ScreenPlayersStage.OnOpen))]
        [HarmonyPostfix]
        static void OnOpen_Postfix(ScreenPlayersStage __instance)
        {
            for (int i = 0; i < __instance.stageButtonsContainer.childCount; i++)
            {
                GameObject.Destroy(__instance.stageButtonsContainer.GetChild(i).gameObject);
            }

            ReorganiseStageButtons(__instance);

            __instance.btRight.visible = false;
            __instance.btLeft.visible = false;

            stageHasBeenSelected = false;
        }

        static TMP_Text MakeNewTextFromlbTitle(TMP_Text copyThis, Transform parentTf, Vector3 position)
        {
            TMP_Text newLabel = GameObject.Instantiate(copyThis, parentTf);
            newLabel.SetText("");
            newLabel.color = Color.white;
            newLabel.fontSize = 32;
            newLabel.transform.localPosition = position;

            return newLabel;
        }


        static void ReorganiseStageButtons(ScreenPlayersStage instance)
        {
            stageButtons3D.Clear();
            stageButtons2D.Clear();

            float posX = RND_START_POS_X;
            float posY = START_POS_Y;

            lbStageName = MakeNewTextFromlbTitle(instance.lbTitle, instance.lbTitle.transform.parent, new Vector3(0, -360));
            lbStageName.name = "lbStageName";
            lbStageSize = MakeNewTextFromlbTitle(instance.lbTitle, instance.lbTitle.transform.parent, new Vector3(0, -355));
            lbStageSize.name = "lbStageSize";
            lbStageSize.alignment = TextAlignmentOptions.Right;
            lbStageSize.fontSize = 24;
            RectTransform rectTf = lbStageSize.gameObject.GetComponent<RectTransform>();
            rectTf.sizeDelta = Vector2.one;
            rectTf.anchorMax = new Vector2(0.99f, 1);

            List<Stage> stageList = new List<Stage>();
            stageList.AddRange(StageHandler.stagesAll);
            instance.nButtons = stageList.Count + 2;
            instance.btStages = new LLButton[instance.nButtons];


            Sprite sprite = JPLELOFJOOH.BNFIDCAPPDK("_spritePreviewRandom");
            LLButton llButton = LLButton.CreateImageButton(instance.stageButtonsContainer, sprite, new Color(0.6f, 0.6f, 0.6f, 1f), Color.white);
            llButton.SetActive(true);
            llButton.name = llButton.name + "_" + (-1).ToString();
            llButton.onClick = delegate (int playerNr)
            {
                instance.curIndex = 0;
                instance.SelectStage(playerNr, -1);
            };

            llButton.onHover = delegate (int playerNr)
            {
                if (stageHasBeenSelected == true)
                {
                    return;
                }

                foreach (var stageButton in stageButtons3D)
                {
                    stageButton.gameObject.SetActive(true);
                };

                foreach (var stageButton in stageButtons2D)
                {
                    stageButton.gameObject.SetActive(false);
                };
            };

            RectTransform btnRectTransform3D = llButton.GetComponent<RectTransform>();
            btnRectTransform3D.anchoredPosition = new Vector2(posX, posY);
            llButton.posDefault = btnRectTransform3D.anchoredPosition;
            btnRectTransform3D.localScale = SCALE_FACTOR * Vector3.one;
            posY -= SIZE_Y + SPACING;

            instance.btStages[0] = llButton;
            stageButtonsRND[0] = llButton;

            sprite = Resources.Load<Sprite>("Textures/_spritePreviewRandom_2d");
            llButton = LLButton.CreateImageButton(instance.stageButtonsContainer, sprite, new Color(0.6f, 0.6f, 0.6f, 1f), Color.white);
            llButton.SetActive(true);
            llButton.name = llButton.name + "_" + (-2).ToString();
            llButton.onClick = delegate (int playerNr)
            {
                instance.curIndex = 1;
                instance.SelectStage(playerNr, -2);
            };

            llButton.onHover = delegate (int playerNr)
            {
                if (stageHasBeenSelected == true)
                {
                    return;
                }

                foreach (var stageButton in stageButtons3D)
                {
                    stageButton.gameObject.SetActive(false);
                };

                foreach (var stageButton in stageButtons2D)
                {
                    stageButton.gameObject.SetActive(true);
                };
            };

            RectTransform btnRectTransform = llButton.GetComponent<RectTransform>();
            btnRectTransform.anchoredPosition = new Vector2(posX, posY);
            llButton.posDefault = btnRectTransform.anchoredPosition;
            btnRectTransform.localScale = SCALE_FACTOR * Vector3.one;

            instance.btStages[1] = llButton;
            stageButtonsRND[1] = llButton;

            posX = START_POS_X;
            posY = START_POS_Y;
            int count = 0;

            for (int i = 0; i < stageList.Count; i++)
            {
                count++;
                int offset = i + 2;

                sprite = JPLELOFJOOH.BNFIDCAPPDK("_spritePreview" + stageList[i]);
                LLButton llbutton = LLButton.CreateImageButton(instance.stageButtonsContainer, sprite, new Color(0.6f, 0.6f, 0.6f, 1f), Color.white);
                llbutton.SetActive(true);
                if (EPCDKLCABNC.KFFJOEAJLEH(stageList[i]))
                {
                    Stage stage = stageList[i];
                    int stage_selection = (int)stage;
                    string stageName = stageList[i] != Stage.ROOM21_2D ? stageList[i] != Stage.SUBWAY_2D ? stageList[i].ToString() : "TRAIN_2D" : "MENTAL_2D";
                    LLButton llbutton2 = llbutton;
                    llbutton2.name = llbutton2.name + "_" + stage_selection.ToString();
                    llbutton.onClick = delegate (int playerNr)
                    {
                        instance.curIndex = offset;
                        instance.SelectStage(playerNr, stage_selection);
                    };

                    llbutton.onHover = delegate (int playerNr)
                    {
                        TextHandler.SetTextCode(lbStageName, $"STAGE_{stageName}");
                        TextHandler.SetText(lbStageSize, GetStageSize(stage));
                    };

                    llbutton.onHoverOut = delegate (int playerNr)
                    {
                        if (stageHasBeenSelected == true) return;

                        TextHandler.SetText(lbStageName, "");
                        TextHandler.SetText(lbStageSize, "");
                    };

                }
                else
                {
                    Stage stage_selection = stageList[i];
                    sprite = JPLELOFJOOH.BNFIDCAPPDK("_spritePreviewLOCKED");
                    LLButton llbutton3 = LLButton.CreateImageButton(llbutton.transform, sprite, new Color(0.6f, 0.6f, 0.6f, 1f), Color.white);
                    llbutton3.SetActive(false);

                    llbutton.onHover = delegate (int playerNr)
                    {
                        TextHandler.SetText(lbStageName, UnlockStageText(stage_selection));
                    };

                    llbutton.onHoverOut = delegate (int playerNr)
                    {
                        TextHandler.SetText(lbStageName, "");
                    };
                }

                RectTransform comp = llbutton.GetComponent<RectTransform>();
                comp.anchoredPosition = new Vector2(posX, posY);
                llbutton.posDefault = comp.anchoredPosition;
                comp.localScale = SCALE_FACTOR * Vector3.one;
                if (count >= 5)
                {
                    if (stageList[i] == Stage.ROOM21)
                    {
                        posY = START_POS_Y;
                    }
                    else
                    {
                        posY -= SIZE_Y + SPACING;
                    }
                    posX = START_POS_X;
                    count = 0;
                }
                else
                {
                    posX += SIZE_X + SPACING;
                }

                if (i <= 9)
                {
                    stageButtons3D.Add(llbutton);
                }
                else
                {
                    stageButtons2D.Add(llbutton);
                    llbutton.gameObject.SetActive(false);
                }
                instance.btStages[offset] = llbutton;
            }

            instance.curIndex = 0;
        }

        static void Move(int d, ScreenPlayersStage inst, bool vert = false)
        {
            bool wrapToEnd = (inst.curIndex >= 14 && inst.curIndex <= 16);
            bool selecting3DStage = (inst.curIndex >= 2 && inst.curIndex <= 11);

            int currentIndex = inst.curIndex;
            currentIndex += d;

            int clampedRange = selecting3DStage ? Mathf.Clamp(currentIndex, 2, 11) : Mathf.Clamp(currentIndex, 12, 18);

            if (selecting3DStage)
            {
                if (vert == true)
                {
                    if (currentIndex > clampedRange)
                    {
                        currentIndex -= 10;
                    }
                    else if (currentIndex < clampedRange)
                    {
                        currentIndex += 10;
                    }
                }
            }
            else
            {
                if (vert == true)
                {
                    if (wrapToEnd)
                    {
                        currentIndex = 18;
                    }
                    else if (currentIndex > clampedRange)
                    {
                        currentIndex -= 10;
                    }
                    else if (currentIndex < clampedRange)
                    {
                        currentIndex += 10;
                        if (currentIndex > 18)
                        {
                            currentIndex = inst.nButtons - 1;
                        }
                    }
                }
            }

            inst.curIndex = currentIndex;
            UIScreen.SetFocus(inst.GetButton(inst.curIndex));

        }

        [HarmonyPatch(typeof(ScreenPlayersStage), nameof(ScreenPlayersStage.GetControls))]
        [HarmonyPrefix]
        static bool GetControls(ref List<LLClickable> list, bool vert, LLClickable curFocus, LLCursor cursor, ScreenPlayersStage __instance)
        {
            if (stageHasBeenSelected == true)
            {
                return false;
            }

            if (curFocus == null)
            {
                int num = __instance.curIndex < 12 ? 0 : 1;
                list.Add(stageButtonsRND[num]);
                __instance.curIndex = num;
            }

            if (vert)
            {
                if (curFocus != stageButtonsRND[0])
                {
                    list.Add(stageButtonsRND[0]);
                    __instance.curIndex = 0;
                }
                else
                {
                    list.Add(stageButtonsRND[1]);
                    __instance.curIndex = 1;
                }
            }
            else
            {
                if (curFocus != stageButtonsRND[0] && (__instance.curIndex == 2 || __instance.curIndex == 6) || (__instance.curIndex == 12 || __instance.curIndex == 16))
                {
                    list.Add(stageButtonsRND[0]);
                    __instance.curIndex = 0;
                }
                else
                {
                    list.Add(stageButtonsRND[1]);
                    __instance.curIndex = 1;
                }
            }
            return false;
        }

        [HarmonyPatch(typeof(ScreenPlayersStage), nameof(ScreenPlayersStage.DirectMove))]
        [HarmonyPrefix]
        static bool DirectMove(ref Vector2 move, LLClickable curFocus, bool shouldMove, ScreenPlayersStage __instance, ref bool __result)
        {
            if (stageHasBeenSelected == true)
            {
                __result = true;
                return false;
            }

            bool notActive = curFocus == null;
            bool onRandom = (curFocus == stageButtonsRND[0] || curFocus == stageButtonsRND[1]);
            bool goToRandom3D = __instance.curIndex == 2 && move.x < 0 || __instance.curIndex == 6 && move.x > 0 || __instance.curIndex == 12 && move.x < 0 || __instance.curIndex == 16 && move.x > 0;
            bool goToRandom2D = __instance.curIndex == 7 && move.x < 0 || __instance.curIndex == 11 && move.x > 0 || __instance.curIndex == 17 && move.x < 0 || __instance.curIndex == 18 && move.x > 0;

            if (!shouldMove || goToRandom3D || goToRandom2D || onRandom && move.y != 0 || notActive)
            {
                __result = false;
                return false;
            }


            if (curFocus == stageButtonsRND[0] && move.y == 0)
            {
                __instance.curIndex = move.x > 0 ? 2 : 6;
                Move(0, __instance);
                __result = true;
                return false;
            }

            if (curFocus == stageButtonsRND[1] && move.y == 0)
            {
                __instance.curIndex = move.x > 0 ? 17 : 18;
                Move(0, __instance);
                __result = true;
                return false;
            }


            if (move.x < 0f)
            {
                Move(-1, __instance);
            }
            else if (move.x > 0f)
            {
                Move(1, __instance);
            }
            else if (move.y < 0f)
            {
                Move(5, __instance, true);
            }
            else
            {
                Move(-5, __instance, true);
            }

            __result = true;
            return false;
        }

        static bool stageHasBeenSelected = false;

        [HarmonyPatch(typeof(ScreenPlayersStage), nameof(ScreenPlayersStage.SelectionDone))]
        [HarmonyPostfix]
        static void Method(ScreenPlayersStage __instance)
        {
            stageHasBeenSelected = true;
            lbStageSize.SetText("");
            __instance.StartCoroutine(LerpButtonToCenter(__instance, __instance.curIndex));
        }

        static IEnumerator LerpButtonToCenter(ScreenPlayersStage instance, int buttonIndex)
        {
            float dur = 0.1f;
            Vector2 finalPos = new Vector2(2.5f, 12);
            for (float f = 0f; f <= 1f;)
            {
                f += Time.deltaTime / dur;

                instance.Place(buttonIndex,
                    Vector2.Lerp(instance.btStages[buttonIndex].posDefault, finalPos, f),
                    Vector3.Lerp(SCALE_FACTOR * Vector3.one, instance.scaleBig, f));
                yield return null;
            }

            yield break;
        }

    }
}
