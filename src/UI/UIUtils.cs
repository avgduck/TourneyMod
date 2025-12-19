using LLBML.Players;
using LLGUI;
using TMPro;
using TourneyMod.StageStriking;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI;

internal static class UIUtils
{
    private static Sprite buttonBG
    {
        get
        {
            Sprite bg = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            return bg;
        }
    }
    
    internal static void CreateText(ref TextMeshProUGUI text, string name, Transform parent)
    {
        CreateText(ref text, name, parent, Vector2.zero);
    }
    internal static void CreateText(ref TextMeshProUGUI text, string name, Transform parent, Vector2 position)
    {
        CreateText(ref text, name, parent, position, new Vector2(200f, 50f));
    }

    internal static void CreateText(ref TextMeshProUGUI text, string name, Transform parent, Vector2 position, Vector2 scale)
    {
        RectTransform panel = LLControl.CreatePanel(parent, name);
        panel.anchorMin = new Vector2(0f, 0f);
        panel.anchorMax = new Vector2(1f, 1f);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);

        panel.anchoredPosition = position;
        
        text = panel.gameObject.AddComponent<TextMeshProUGUI>();
        text.raycastTarget = false;
        text.SetText("");
        text.color = Color.white;
        text.fontSize = 32;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
    }

    internal static void CreateButton(ref LLButton button, string name, Transform parent, Vector2 position, Vector2 scale)
    {
        Image img = LLControl.CreateImage(parent, buttonBG);
        img.color = new Color(1f, 1f, 0f, 0f);
        RectTransform panel = img.rectTransform;
        panel.name = name;
        panel.anchorMin = new Vector2(0f, 0f);
        panel.anchorMax = new Vector2(1f, 1f);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);

        panel.anchoredPosition = position;
        
        button = panel.gameObject.AddComponent<LLButton>();
        button.keepIconColor = true;
        button.colHover = new Color(0.902f, 0.9529f, 0.051f);
        
        Image bg = LLControl.CreateImage(button.transform, buttonBG);
        bg.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        bg.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);
        bg.color = Color.black;
        bg.raycastTarget = false;
        
        CreateText(ref button.textMesh, "Text", button.transform);
        button.textMesh.rectTransform.anchorMin = new Vector2(0f, 0f);
        button.textMesh.rectTransform.anchorMax = new Vector2(1f, 1f);
        button.textMesh.raycastTarget = false;
        button.SetText("");
        button.textMesh.color = Color.white;
        button.textMesh.alignment = TextAlignmentOptions.Center;
        button.Init();
    }

    internal static void CreateVoteButton(ref VoteButton button, string name, Transform parent, Vector2 position, Vector2 scale)
    {
        Image img = LLControl.CreateImage(parent, buttonBG);
        img.color = new Color(1f, 1f, 0f, 0f);
        RectTransform panel = img.rectTransform;
        panel.name = name;
        panel.anchorMin = new Vector2(0f, 0f);
        panel.anchorMax = new Vector2(1f, 1f);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);

        panel.anchoredPosition = position;
        
        button = panel.gameObject.AddComponent<VoteButton>();
        button.keepIconColor = true;
        button.colHover = new Color(0.902f, 0.9529f, 0.051f);
        
        Image bg = LLControl.CreateImage(button.transform, buttonBG);
        bg.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        bg.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);
        bg.color = Color.black;
        bg.raycastTarget = false;
        
        CreateText(ref button.textMesh, "Text", button.transform);
        button.textMesh.rectTransform.anchorMin = new Vector2(0f, 0f);
        button.textMesh.rectTransform.anchorMax = new Vector2(1f, 1f);
        button.textMesh.raycastTarget = false;
        button.SetText("");
        button.textMesh.color = Color.white;
        button.textMesh.alignment = TextAlignmentOptions.Center;
        button.Init();
    }

    internal static void SetButtonBGVisibility(LLButton button, bool visible)
    {
        Transform img = button.transform.Find("Image");
        if (img == null) return;
        img.gameObject.SetActive(visible);
    }
    
    private static readonly Color COLOR_CURSOR_ACTIVE = Color.white;
    private static readonly Color COLOR_CURSOR_INACTIVE = Color.white * 0.6f;
    
    // texture editing code from ColorSwap
    private static void SetTextureCopy(ref Texture2D destination, Texture2D source)
    {
        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        
        Graphics.Blit(source, temp);
        
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = temp;
        destination = new Texture2D(source.width, source.height, source.format, false);
        destination.ReadPixels(new Rect(0, 0, temp.width, temp.height), 0, 0);
        destination.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(temp);
    }
    private static void SetTextureColor(ref Texture2D texture, Color color)
    {
        Color[] pixels = texture.GetPixels();
        for (int pixelIndex = 0; pixelIndex < pixels.Length; pixelIndex++)
        {
            Color imgColor = pixels[pixelIndex];
            pixels[pixelIndex] = new Color(imgColor.r * color.r, imgColor.g * color.g, imgColor.b * color.b, imgColor.a * color.a);
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    private static readonly Texture2D[] cursorImagesActive = new Texture2D[4];
    private static readonly Texture2D[] cursorImagesInactive = new Texture2D[4];
    internal static void GenerateCursorImages(LLCursor cursor)
    {
        Texture2D source = cursor.texCursor;
        Texture2D cursorActive = new Texture2D(0, 0);
        Texture2D cursorInactive = new Texture2D(0, 0);
        SetTextureCopy(ref cursorActive, source);
        SetTextureCopy(ref cursorInactive, source);
        SetTextureColor(ref cursorInactive, COLOR_CURSOR_INACTIVE);
        Player player = cursor.player;
        cursorImagesActive[player.nr] = cursorActive;
        cursorImagesInactive[player.nr] = cursorInactive;
    }

    internal static void UpdateCursorColors(int controllingPlayer)
    {
        if (!StageStrikeTracker.Instance.IsTrackingStrikeInfo || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickMode || StageStrikeTracker.Instance.CurrentStrikeInfo.IsFreePickForced)
        {
            ResetCursorColors();
            return;
        }
        
        Player.ForAll(player =>
        {
            player.cursor.image.color = player.nr == controllingPlayer ? COLOR_CURSOR_ACTIVE : COLOR_CURSOR_INACTIVE;
            
            if (player.cursor.state != CursorState.POINTER_HW) return;
            Texture2D activeCursor = cursorImagesActive[player.nr];
            Texture2D inactiveCursor = cursorImagesInactive[player.nr];
            Cursor.SetCursor(player.nr == controllingPlayer ? activeCursor : inactiveCursor, new Vector2(0f, 0f), CursorMode.ForceSoftware);
        });
    }

    internal static void ResetCursorColors()
    {
        Player.ForAll(player =>
        {
            player.cursor.image.color = COLOR_CURSOR_ACTIVE;
            
            if (player.cursor.state != CursorState.POINTER_HW) return;
            Texture2D activeCursor = cursorImagesActive[player.nr];
            Cursor.SetCursor(activeCursor, new Vector2(0f, 0f), CursorMode.ForceSoftware);
        });
    }
}