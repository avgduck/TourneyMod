using LLBML.Players;
using LLGUI;
using LLHandlers;
using TMPro;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI.StageSelect;

internal class StageButton : LLButton
{
    private static readonly Color COLOR_BANNED = Color.white * 0.25f;
    private static readonly Color COLOR_UNFOCUSED = Color.white * 0.6f;
    private static readonly Color COLOR_FOCUSED = Color.white;

    private static readonly Color COLOR_LOCK = Color.white;
    
    private static readonly Color[] COLOR_LOCK_TEAM =
    [
        new Color(255/255f, 64/255f, 22/255f),
        new Color(13/255f, 136/255f, 255/255f),
        new Color(255/255f, 255/255f, 61/255f),
        new Color(90/255f, 244/255f, 90/255f)
    ];
    private static readonly Color[] COLOR_SOFTLOCK_TEAM =
    [
        new Color(255/255f, 64/255f, 22/255f, 0.3f),
        new Color(13/255f, 136/255f, 255/255f, 0.3f),
        new Color(255/255f, 255/255f, 61/255f, 0.3f),
        new Color(90/255f, 244/255f, 90/255f, 0.3f)
    ];

    private bool[] playersHovering = [false, false, false, false];
    private StageBan stageBan;

    private bool IsBeingHovered =>
        playersHovering[0] || playersHovering[1] || playersHovering[2] || playersHovering[3];

    private bool selected;
    private Image stageImage;
    private Image lockedImage;
    private Image selectedImage;
    private TextMeshProUGUI lbBanReason;

    internal static StageButton CreateStageButton(Transform tfParent, Stage stage)
    {
        RectTransform rect = LLControl.CreatePanel(tfParent, $"Button_{stage}");
        StageButton stageButton = rect.gameObject.AddComponent<StageButton>();

        stageButton.soundClick = true;
        stageButton.soundHover = true;
        
        Sprite stageSprite = JPLELOFJOOH.BNFIDCAPPDK($"_spritePreview{stage}"); // Assets.GetMenuSprite()
        stageButton.stageImage = LLControl.CreateImage(rect, stageSprite);
        
        Sprite lockedSprite = JPLELOFJOOH.BNFIDCAPPDK($"_spritePreviewLOCKED"); // Assets.GetMenuSprite()
        stageButton.lockedImage = LLControl.CreateImage(rect, lockedSprite);
        stageButton.lockedImage.raycastTarget = false;

        stageButton.selectedImage = LLControl.CreateImage(rect, UIUtils.spriteStageSelected);
        stageButton.selectedImage.raycastTarget = false;
        
        UIUtils.CreateText(ref stageButton.lbBanReason, "lbBanReason", stageButton.transform, new Vector2(0f, 13f));
        stageButton.lbBanReason.fontSize = 22;
        TextHandler.SetText(stageButton.lbBanReason, "");
        stageButton.Init();
        return stageButton;
    }

    public override void InitNeeded()
    {
        OnHoverOut(-1);
    }

    public void SetBan(StageBan ban)
    {
        stageBan = ban;

        if (stageBan != null)
        {
            if (stageBan.banTeam == Team.NONE) OnHoverOut(-1);
            else
            {
                for (int playerNumber = 0; playerNumber < 4; playerNumber++)
                {
                    if (SetTracker.Instance.GetPlayerTeam(playerNumber) == stageBan.banTeam) OnHoverOut(playerNumber);
                }
            }
        }

        UpdateDisplay();
    }

    public override void OnHover(int playerNumber)
    {
        if (playerNumber == -1)
        {
            playersHovering = [true, true, true, true];
            if (soundHover) AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
        }
        else
        {
            bool doHover = StageStrikeTracker.Instance.CurrentStrikeInfo.CheckPlayerInteraction(stageBan, playerNumber);
            playersHovering[playerNumber] = doHover;
            if (doHover && soundHover) AudioHandler.PlayMenuSfx(Sfx.MENU_SCROLL);
        }
        
        UpdateDisplay();
    }

    public override void OnHoverOut(int playerNumber)
    {
        if (playerNumber == -1) playersHovering = [false, false, false, false];
        else playersHovering[playerNumber] = false;
        UpdateDisplay();
    }

    internal void Select(bool selected)
    {
        this.selected = selected;
    }

    internal void UpdateDisplay()
    {
        selectedImage.color = Color.clear;
        
        if (selected)
        {
            stageImage.color = COLOR_FOCUSED;
            selectedImage.color = Color.white;
        }
        else if (!isActive)
        {
            stageImage.color = COLOR_BANNED;
        }
        else if (IsBeingHovered)
        {
            stageImage.color = COLOR_FOCUSED;
        }
        else if (stageBan != null)
        {
            if (stageBan.reason == StageBan.BanReason.DSR && stageBan.banTeam != Team.NONE)
            {
                stageImage.color = COLOR_UNFOCUSED;
            }
            else
            {
                stageImage.color = COLOR_BANNED;
            }
        }
        else
        {
            stageImage.color = COLOR_UNFOCUSED;
        }
        lockedImage.gameObject.SetActive(stageBan != null);
        if (stageBan == null)
        {
            TextHandler.SetText(lbBanReason, "");
            return;
        }

        switch (stageBan.reason)
        {
            case StageBan.BanReason.COUNTERPICK:
                lockedImage.color = COLOR_LOCK;
                lbBanReason.color = COLOR_LOCK;
                TextHandler.SetText(lbBanReason, "Counterpick");
                break;
            case StageBan.BanReason.BAN:
                lockedImage.color = COLOR_LOCK_TEAM[(int)stageBan.banTeam];
                lbBanReason.color = COLOR_LOCK_TEAM[(int)stageBan.banTeam];
                TextHandler.SetText(lbBanReason, $"{stageBan.banTeam} Ban");
                break;
            case StageBan.BanReason.DSR:
                lockedImage.color = (stageBan.banTeam == Team.NONE) ? COLOR_LOCK : COLOR_SOFTLOCK_TEAM[(int)stageBan.banTeam];
                lbBanReason.color = (stageBan.banTeam == Team.NONE) ? COLOR_LOCK : COLOR_SOFTLOCK_TEAM[(int)stageBan.banTeam];
                TextHandler.SetText(lbBanReason, (stageBan.banTeam == Team.NONE) ? "Both DSR" : $"{stageBan.banTeam} DSR");
                break;
        }
    }
}