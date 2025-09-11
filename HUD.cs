using UnityEngine;
using UnityEngine.UI;

public class SimpleHUD : MonoBehaviour
{
    private PlayerData playerData;
    private Image[] maskImages;
    private Image soulImage;

    public void Init(PlayerData pd)
    {
        playerData = pd;
        CreateUI();
    }

    private void CreateUI()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        gameObject.AddComponent<GraphicRaycaster>();

        var health = new GameObject("HealthContainer");
        health.transform.SetParent(canvas.transform, false);
        var hRect = health.AddComponent<RectTransform>();
        hRect.anchorMin = hRect.anchorMax = new Vector2(1, 1);
        hRect.pivot = new Vector2(1, 1);
        hRect.anchoredPosition = new Vector2(-10, -10);
        var layout = health.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 2f;
        layout.childAlignment = TextAnchor.UpperRight;
        layout.reverseArrangement = true;

        maskImages = new Image[playerData.maxHealth];
        for (int i = 0; i < maskImages.Length; i++)
        {
            var m = new GameObject("Mask" + i);
            m.transform.SetParent(health.transform, false);
            var r = m.AddComponent<RectTransform>();
            r.sizeDelta = new Vector2(32, 32);
            var img = m.AddComponent<Image>();
            img.sprite = BuildMaskSprite();
            img.color = Color.black;
            maskImages[i] = img;
        }

        var soul = new GameObject("SoulVessel");
        soul.transform.SetParent(canvas.transform, false);
        var sRect = soul.AddComponent<RectTransform>();
        sRect.anchorMin = sRect.anchorMax = new Vector2(1, 1);
        sRect.pivot = new Vector2(1, 1);
        sRect.anchoredPosition = new Vector2(-150, -50);
        sRect.sizeDelta = new Vector2(64, 64);
        soulImage = soul.AddComponent<Image>();
        soulImage.sprite = BuildCircleSprite();
        soulImage.type = Image.Type.Filled;
        soulImage.fillMethod = Image.FillMethod.Radial360;
        soulImage.fillAmount = 0f;
        soulImage.color = Color.white;
    }

    private Sprite BuildMaskSprite()
    {
        var tex = new Texture2D(32, 32);
        for (int x = 0; x < 32; x++)
            for (int y = 0; y < 32; y++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }

    private Sprite BuildCircleSprite()
    {
        int size = 64;
        var tex = new Texture2D(size, size);
        Vector2 c = new Vector2(size / 2f, size / 2f);
        float r = size / 2f;
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                var col = Vector2.Distance(new Vector2(x, y), c) <= r ? Color.white : Color.clear;
                tex.SetPixel(x, y, col);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        if (playerData == null)
            return;
        RefreshHealth();
        RefreshSoul();
    }

    private void RefreshHealth()
    {
        int cur = playerData.health;
        for (int i = 0; i < maskImages.Length; i++)
            maskImages[i].color = i < cur ? Color.white : Color.black;
    }

    private void RefreshSoul()
    {
        if (soulImage == null) return;
        float max = playerData.silkMax;
        soulImage.fillAmount = max > 0 ? (float)playerData.silk / max : 0f;
    }
}
