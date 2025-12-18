using GameplayEntities;
using LLGUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI;

internal static class UI
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

    internal static void SetButtonBGVisibility(LLButton button, bool visible)
    {
        Transform img = button.transform.Find("Image");
        if (img == null) return;
        img.gameObject.SetActive(visible);
    }
}