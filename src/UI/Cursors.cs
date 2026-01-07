using System;
using System.IO;
using System.Linq;
using LLBML.Players;
using LLGUI;
using TourneyMod.SetTracking;
using TourneyMod.StageStriking;
using UnityEngine;

namespace TourneyMod.UI;

internal static class Cursors
{
    private static readonly Color COLOR_CURSOR_ACTIVE = Color.white;
    private static readonly Color COLOR_CURSOR_INACTIVE = Color.white * 0.6f;
    private static readonly PlayerCursor[] playerCursors = [new(), new(), new(), new()];

    internal static void LoadCursorImages(DirectoryInfo directory)
    {
        foreach (FileInfo file in directory.GetFiles().OrderBy(f => f.Name))
        {
            if (!file.Name.Contains("cursor")) continue;
            string[] id = file.Name.Replace("cursor", "").Replace(".png", "").Split('_');
            int playerNumber = int.Parse(id[0]);
            PlayerTeam team = (PlayerTeam)Enum.Parse(typeof(PlayerTeam), id[1]);
            Texture2D texActive = UIUtils.LoadImageFile(file);
            
            Texture2D texInactive = new Texture2D(0, 0);
            UIUtils.SetTextureCopy(ref texInactive, texActive);
            UIUtils.SetTextureColor(ref texInactive, COLOR_CURSOR_INACTIVE);

            switch (team)
            {
                case PlayerTeam.RED:
                    playerCursors[playerNumber].texRedActive = texActive;
                    playerCursors[playerNumber].texRedInactive = texInactive;
                    playerCursors[playerNumber].spriteRedActive = UIUtils.ToSprite(texActive);
                    playerCursors[playerNumber].spriteRedInactive = UIUtils.ToSprite(texInactive);
                    break;
                case PlayerTeam.BLUE:
                    playerCursors[playerNumber].texBlueActive = texActive;
                    playerCursors[playerNumber].texBlueInactive = texInactive;
                    playerCursors[playerNumber].spriteBlueActive = UIUtils.ToSprite(texActive);
                    playerCursors[playerNumber].spriteBlueInactive = UIUtils.ToSprite(texInactive);
                    break;
                case PlayerTeam.YELLOW:
                    playerCursors[playerNumber].texYellowActive = texActive;
                    playerCursors[playerNumber].texYellowInactive = texInactive;
                    playerCursors[playerNumber].spriteYellowActive = UIUtils.ToSprite(texActive);
                    playerCursors[playerNumber].spriteYellowInactive = UIUtils.ToSprite(texInactive);
                    break;
                case PlayerTeam.GREEN:
                    playerCursors[playerNumber].texGreenActive = texActive;
                    playerCursors[playerNumber].texGreenInactive = texInactive;
                    playerCursors[playerNumber].spriteGreenActive = UIUtils.ToSprite(texActive);
                    playerCursors[playerNumber].spriteGreenInactive = UIUtils.ToSprite(texInactive);
                    break;
                default:
                    playerCursors[playerNumber].texNoneActive = texActive;
                    playerCursors[playerNumber].texNoneInactive = texInactive;
                    playerCursors[playerNumber].spriteNoneActive = UIUtils.ToSprite(texActive);
                    playerCursors[playerNumber].spriteNoneInactive = UIUtils.ToSprite(texInactive);
                    break;
            }
        }
    }

    internal static void UpdateCursorColors(Team controllingTeam)
    {
        if (!StageStrikeTracker.Instance.IsTrackingStrikeInfo || SetTracker.Instance.CurrentSet.IsFreePickMode || SetTracker.Instance.CurrentSet.IsFreePickForced)
        {
            ResetCursorColors();
            return;
        }
        
        Player.ForAll((Player player) =>
        {
            player.cursor.image.sprite = playerCursors[player.nr].GetSprite(player.Team, player.Team == controllingTeam);
            
            if (player.cursor.state != CursorState.POINTER_HW) return;
            Texture2D activeCursor = playerCursors[player.nr].GetTexture(player.Team, true);
            Texture2D inactiveCursor = playerCursors[player.nr].GetTexture(player.Team, false);
            Cursor.SetCursor(player.Team == controllingTeam ? activeCursor : inactiveCursor, new Vector2(0f, 0f), CursorMode.ForceSoftware);
        });
    }

    internal static void ResetCursorColors()
    {
        Player.ForAll((Player player) =>
        {
            player.cursor.image.sprite = playerCursors[player.nr].GetSprite(player.Team, true);
            
            if (player.cursor.state != CursorState.POINTER_HW) return;
            Texture2D activeCursor = playerCursors[player.nr].GetTexture(player.Team, true);
            Cursor.SetCursor(activeCursor, new Vector2(0f, 0f), CursorMode.ForceSoftware);
        });
    }
}