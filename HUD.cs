using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Reflection;

public class SimpleHUD : MonoBehaviour
{
    private PlayerData playerData;
    private Image[] maskImages;
    private Image soulImage;
    private Sprite maskSprite;
    private Sprite frameSprite;
    private Sprite[] slashFrames;
    private int previousHealth;

    private const KeyCode DebugDamageKey = KeyCode.Minus;
    private const KeyCode DebugHealKey = KeyCode.Equals;

    public void Init(PlayerData pd)
    {
        playerData = pd;
        LoadSprites();
        CreateUI();
        previousHealth = playerData.health;
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

        GameObject frame = null;
        if (frameSprite != null)
        {
            frame = new GameObject("SoulFrame");
            frame.transform.SetParent(health.transform, false);
            var fr = frame.AddComponent<RectTransform>();
            fr.sizeDelta = new Vector2(frameSprite.rect.width, frameSprite.rect.height);
            var fi = frame.AddComponent<Image>();
            fi.sprite = frameSprite;
            frame.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        maskImages = new Image[playerData.maxHealth];
        for (int i = 0; i < maskImages.Length; i++)
        {
            var m = new GameObject("Mask" + i);
            m.transform.SetParent(health.transform, false);
            var r = m.AddComponent<RectTransform>();
            if (maskSprite != null)
                r.sizeDelta = new Vector2(maskSprite.rect.width, maskSprite.rect.height);
            else
                r.sizeDelta = new Vector2(32, 32);
            var img = m.AddComponent<Image>();
            img.sprite = maskSprite != null ? maskSprite : BuildMaskSprite();
            img.color = Color.white;
            maskImages[i] = img;
        }

        if (frame != null)
        {
            var soul = new GameObject("SoulVessel");
            soul.transform.SetParent(frame.transform, false);
            var sRect = soul.AddComponent<RectTransform>();
            sRect.anchorMin = sRect.anchorMax = new Vector2(0.5f, 0.5f);
            sRect.pivot = new Vector2(0.5f, 0.5f);
            sRect.anchoredPosition = Vector2.zero;
            sRect.sizeDelta = new Vector2(64, 64);
            soulImage = soul.AddComponent<Image>();
            soulImage.sprite = BuildCircleSprite();
            soulImage.type = Image.Type.Filled;
            soulImage.fillMethod = Image.FillMethod.Radial360;
            soulImage.fillAmount = 0f;
            soulImage.color = Color.white;
        }
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

    private void LoadSprites()
    {
        try
        {
            var dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
            var maskPath = Path.Combine(dir, "select_game_HUD_0001_health.png");
            var framePath = Path.Combine(dir, "select_game_HUD_0002_health_frame.png");
            var slashPath = Path.Combine(dir, "The Knight spells and items - atlas0 #00000309.png");
            maskSprite = LoadSprite(maskPath);
            frameSprite = LoadSprite(framePath);
            slashFrames = LoadSpriteSheet(slashPath, 8, 8);
        }
        catch { }
    }

    private Sprite LoadSprite(string path)
    {
        if (!File.Exists(path)) return null;
        var bytes = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        ImageConversion.LoadImage(tex, bytes);
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    private Sprite[] LoadSpriteSheet(string path, int cols, int rows)
    {
        if (!File.Exists(path)) return new Sprite[0];
        var bytes = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        ImageConversion.LoadImage(tex, bytes);
        tex.filterMode = FilterMode.Point;
        int w = tex.width / cols;
        int h = tex.height / rows;
        var sprites = new Sprite[cols * rows];
        int idx = 0;
        for (int y = rows - 1; y >= 0; y--)
        {
            for (int x = 0; x < cols; x++)
            {
                var rect = new Rect(x * w, y * h, w, h);
                sprites[idx++] = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
            }
        }
        return sprites;
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

        if (Input.GetKeyDown(DebugDamageKey))
            playerData.health = Mathf.Max(0, playerData.health - 1);
        if (Input.GetKeyDown(DebugHealKey))
            playerData.health = Mathf.Min(playerData.maxHealth, playerData.health + 1);

        RefreshHealth();
        RefreshSoul();
    }

    private void RefreshHealth()
    {
        int cur = playerData.health;
        // animate health loss
        if (cur < previousHealth)
        {
            for (int i = cur; i < previousHealth && i < maskImages.Length; i++)
            {
                StartCoroutine(LoseHealth(maskImages[i]));
            }
        }

        // refresh remaining health
        for (int i = 0; i < cur && i < maskImages.Length; i++)
        {
            maskImages[i].sprite = maskSprite != null ? maskSprite : maskImages[i].sprite;
            maskImages[i].color = Color.white;
        }

        // ensure unused masks are hidden
        for (int i = cur; i < maskImages.Length; i++)
        {
            if (i >= previousHealth)
                maskImages[i].color = Color.clear;
        }

        previousHealth = cur;
    }

    private IEnumerator LoseHealth(Image img)
    {
        if (img == null) yield break;

        // flash mask
        for (int i = 0; i < 2; i++)
        {
            img.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            img.color = Color.red;
            yield return new WaitForSeconds(0.05f);
        }
        img.color = Color.white;

        // play slash animation over mask
        if (slashFrames != null && slashFrames.Length > 0)
        {
            var slash = new GameObject("Slash");
            slash.transform.SetParent(img.transform, false);
            var sr = slash.AddComponent<Image>();
            sr.rectTransform.sizeDelta = img.rectTransform.sizeDelta * 2f;
            int count = Mathf.Min(6, slashFrames.Length);
            for (int i = 0; i < count; i++)
            {
                sr.sprite = slashFrames[i];
                yield return new WaitForSeconds(0.03f);
            }
            GameObject.Destroy(slash);
        }

        // fade mask out
        float t = 0f;
        Color c = img.color;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / 0.3f);
            img.color = c;
            yield return null;
        }
        img.color = Color.clear;
    }

    private void RefreshSoul()
    {
        if (soulImage == null) return;
        float max = playerData.silkMax;
        soulImage.fillAmount = max > 0 ? (float)playerData.silk / max : 0f;
    }
}
