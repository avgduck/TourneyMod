using LLGUI;
using LLHandlers;
using TMPro;
using TourneyMod.StageStriking;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI;

internal class StageButton : LLButton
{
    private static readonly Color COLOR_BANNED = Color.white * 0.25f;
    private static readonly Color COLOR_UNFOCUSED = Color.white * 0.6f;
    private static readonly Color COLOR_FOCUSED = Color.white;

    private static readonly Color COLOR_LOCK = Color.white;
    
    private static readonly Color[] COLOR_LOCK_PLAYER =
    [
        new Color(255/255f, 64/255f, 22/255f),
        new Color(13/255f, 136/255f, 255/255f),
        new Color(255/255f, 255/255f, 61/255f),
        new Color(90/255f, 244/255f, 90/255f)
    ];
    private static readonly Color[] COLOR_SOFTLOCK_PLAYER =
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

    private Image stageImage;
    private Image lockedImage;
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
        OnHoverOut(stageBan != null ? stageBan.banPlayer : -1);
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

    internal void UpdateDisplay()
    {
        if (IsBeingHovered)
        {
            stageImage.color = COLOR_FOCUSED;
        }
        else if (stageBan != null)
        {
            if (stageBan.reason == StageBan.BanReason.DSR && stageBan.banPlayer != -1)
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
                lockedImage.color = COLOR_LOCK_PLAYER[stageBan.banPlayer];
                lbBanReason.color = COLOR_LOCK_PLAYER[stageBan.banPlayer];
                TextHandler.SetText(lbBanReason, $"P{stageBan.banPlayer+1} Ban");
                break;
            case StageBan.BanReason.DSR:
                lockedImage.color = (stageBan.banPlayer == -1) ? COLOR_LOCK : COLOR_SOFTLOCK_PLAYER[stageBan.banPlayer];
                lbBanReason.color = (stageBan.banPlayer == -1) ? COLOR_LOCK : COLOR_SOFTLOCK_PLAYER[stageBan.banPlayer];
                TextHandler.SetText(lbBanReason, (stageBan.banPlayer == -1) ? "Both DSR" : $"P{stageBan.banPlayer+1} DSR");
                break;
        }
    }
}