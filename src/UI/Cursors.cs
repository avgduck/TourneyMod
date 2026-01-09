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
    private static PlayerCursor[] playerCursors;

    internal static void LoadCursorImages()
    {
        playerCursors = new PlayerCursor[4];

        for (int playerNr = 0; playerNr < 4; playerNr++)
        {
            PlayerCursor cursor = new PlayerCursor();

            for (int team = 0; team < 5; team++)
            {
                Texture2D texActive = Assets.LoadTexture($"cursors/cursor{playerNr}_{(Team)team}.png");
                Texture2D texInactive = new Texture2D(1, 1);
                UIUtils.SetTextureCopy(ref texInactive, texActive);
                UIUtils.SetTextureColor(ref texInactive, COLOR_CURSOR_INACTIVE);
                
                cursor.textures[team, 0] = texActive;
                cursor.textures[team, 1] = texInactive;
                cursor.sprites[team, 0] = UIUtils.ToSprite(texActive);
                cursor.sprites[team, 1] = UIUtils.ToSprite(texInactive);
            }
            
            playerCursors[playerNr] = cursor;
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