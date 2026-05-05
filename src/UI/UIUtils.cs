using LLGUI;
using TMPro;
using TourneyMod.UI.PlayerTags;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI;

internal static class UIUtils
{
    private static Sprite panelBG;
    private static Sprite buttonBG;
    internal static Sprite spriteStageSelected;
    internal static Sprite spriteLock;
    
    internal static readonly Color[] COLOR_TEAM =
    [
        new Color(255/255f, 64/255f, 22/255f),
        new Color(13/255f, 136/255f, 255/255f),
        new Color(255/255f, 255/255f, 61/255f),
        new Color(90/255f, 244/255f, 90/255f)
    ];

    internal static void Init()
    {
        panelBG = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        buttonBG = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        spriteStageSelected = Sprite.Create(CreateBorderTexture(Color.yellow, 8, 500, 250), new Rect(0, 0, 500, 250), new Vector2(0.5f, 0.5f));
        spriteLock = ToSprite(Assets.LoadTexture("lock.png"));
    }

    internal static Texture2D CreateBorderTexture(Color color, int thickness, int width, int height)
    {
        Texture2D tex = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x <= thickness || x >= width - thickness - 1 || y <= thickness || y >= height - thickness - 1) tex.SetPixel(x, y, color);
                else tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        return tex;
    }

    internal static void CreateImage(ref Image image, Sprite sprite, string name, Transform parent, Vector2 position, Vector2 scale)
    {
        image = LLControl.CreateImage(parent, sprite);
        image.rectTransform.name = name;
        image.rectTransform.anchorMin = new Vector2(0f, 0f);
        image.rectTransform.anchorMax = new Vector2(1f, 1f);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);

        image.rectTransform.anchoredPosition = position;
    }

    internal static void CreateImageButton(ref LLButton button, Sprite sprite, string name, Transform parent, Vector2 position, Vector2 scale)
    {
        Image img = LLControl.CreateImage(parent, sprite);
        RectTransform panel = img.rectTransform;
        panel.name = name;
        panel.anchorMin = new Vector2(0f, 0f);
        panel.anchorMax = new Vector2(1f, 1f);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);
        panel.anchoredPosition = position;

        button = panel.gameObject.AddComponent<LLButton>();
        button.colDefault = Color.white;
        button.colHover = new Color(0.902f, 0.9529f, 0.051f);
        button.hoverColorToImage = true;
        button.hoverColorToOutline = false;
        button.Init();
    }

    internal static void CreatePanel(ref RectTransform panel, string name, Transform parent, Vector2 position, Vector2 scale)
    {
        CreatePanel(ref panel, name, parent, position, scale, Color.black);
    }

    internal static void CreatePanel(ref RectTransform panel, string name, Transform parent, Vector2 position, Vector2 scale, Color bgColor)
    {
        Image img = LLControl.CreateImage(parent, buttonBG);
        img.color = bgColor;
        panel = img.rectTransform;
        panel.name = name;
        panel.anchorMin = new Vector2(0f, 0f);
        panel.anchorMax = new Vector2(1f, 1f);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);

        panel.anchoredPosition = position;
    }

    internal static void CreateBorderPanel(ref RectTransform panel, string name, Transform parent, Vector2 position, Vector2 scale, Color bgColor, Color borderColor, int borderThickness)
    {
        CreatePanel(ref panel, name, parent, position, scale, bgColor);

        Texture2D texBorder = CreateBorderTexture(borderColor, borderThickness, (int)scale.x, (int)scale.y);
        Image imgBorder = LLControl.CreateImage(panel, Sprite.Create(texBorder, new Rect(0, 0, texBorder.width, texBorder.height), new Vector2(0.5f, 0.5f)));
    }

    internal static void CreateImageBorderPanel(ref Image panel, string name, Transform parent, Vector2 position, Vector2 scale, Color bgColor, Color borderColor, int borderThickness)
    {
        panel = LLControl.CreateImage(parent, buttonBG);
        panel.color = bgColor;
        panel.rectTransform.name = name;
        panel.rectTransform.anchorMin = new Vector2(0f, 0f);
        panel.rectTransform.anchorMax = new Vector2(1f, 1f);
        panel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        panel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);

        panel.rectTransform.anchoredPosition = position;
        
        Texture2D texBorder = CreateBorderTexture(borderColor, borderThickness, (int)scale.x, (int)scale.y);
        Image imgBorder = LLControl.CreateImage(panel.rectTransform, Sprite.Create(texBorder, new Rect(0, 0, texBorder.width, texBorder.height), new Vector2(0.5f, 0.5f)));
    }
    
    internal static void CreateText(ref TextMeshProUGUI text, string name, Transform parent)
    {
        CreateText(ref text, name, parent, Vector2.zero);
    }
    internal static void CreateText(ref TextMeshProUGUI text, string name, Transform parent, Vector2 position)
    {
        CreateText(ref text, name, parent, position, Vector2.zero);
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
        CreateButton(ref button, name, parent, position, scale, Color.black);
    }

    internal static void CreateButton(ref LLButton button, string name, Transform parent, Vector2 position, Vector2 scale, Color bgColor)
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
        button.soundClick = true;
        
        Image bg = LLControl.CreateImage(button.transform, buttonBG);
        bg.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        bg.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);
        bg.color = bgColor;
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
        button.colDisabled = new Color(0.5f, 0.5f, 0.5f);
        button.soundClick = true;
        
        Image bg = LLControl.CreateImage(button.transform, buttonBG);
        bg.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        bg.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);
        bg.color = Color.black;
        bg.raycastTarget = false;

        int borderThickness = 2;
        Image border = LLControl.CreateImage(button.transform, Sprite.Create(CreateBorderTexture(Color.white, borderThickness, (int)scale.x, (int)scale.y), new Rect(0, 0, (int)scale.x, (int)scale.y), new Vector2(0.5f, 0.5f)));
        border.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        border.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        border.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        border.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);
        border.color = Color.clear;
        border.raycastTarget = false;
        button.imgBorder = border;
        
        CreateText(ref button.textMesh, "Text", button.transform);
        button.textMesh.rectTransform.anchorMin = new Vector2(0f, 0f);
        button.textMesh.rectTransform.anchorMax = new Vector2(1f, 1f);
        button.textMesh.raycastTarget = false;
        button.SetText("");
        button.textMesh.color = Color.white;
        button.textMesh.alignment = TextAlignmentOptions.Center;
        button.Init();
    }

    internal static void CreateCharsetButton(ref CharsetButton button, string name, Transform parent, Vector2 position, Vector2 scale, Color bgColor)
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
        
        button = panel.gameObject.AddComponent<CharsetButton>();
        button.keepIconColor = true;
        button.colHover = new Color(0.902f, 0.9529f, 0.051f);
        button.colDisabled = new Color(0.5f, 0.5f, 0.5f);
        button.soundClick = true;
        
        Image bg = LLControl.CreateImage(button.transform, buttonBG);
        bg.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        bg.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale.x);
        bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale.y);
        bg.color = bgColor;
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
    
    // texture editing code from ColorSwap
    internal static void SetTextureCopy(ref Texture2D destination, Texture2D source)
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
    internal static void SetTextureColor(ref Texture2D texture, Color color)
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

    internal static Sprite ToSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}