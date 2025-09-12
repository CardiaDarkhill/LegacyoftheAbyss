
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

public class SimpleHUD : MonoBehaviour
{
    private PlayerData playerData;

    // Health masks
    private Image[] maskImages;
    private Sprite maskSprite;
    private Color missingMaskColor = new Color(0.2f, 0.2f, 0.2f, 0.45f);

    // Soul orb
    private Sprite soulOrbSprite;
    private RectTransform soulOrbRoot;
    private RectTransform soulRevealMask;
    private Image soulImage;
    private Image soulBgImage;
    private float lastLoggedFill = -1f;

    // Legacy assets kept for fallback/compat
    private Sprite frameSprite;
    private Sprite[] slashFrames;

    // State
    private int shadeMax;
    private int shadeHealth;
    private int previousShadeHealth;
    private int prevHornetHealth;
    private int prevHornetMax;

    // UI containers
    private GameObject healthContainer;
    private Canvas canvas;
    private CanvasScaler scaler;

    private const KeyCode DebugDamageKey = KeyCode.Minus;
    private const KeyCode DebugHealKey = KeyCode.Equals;
    private const KeyCode DebugSoulDecKey = KeyCode.LeftBracket;   // [
    private const KeyCode DebugSoulIncKey = KeyCode.RightBracket;  // ]
    private const KeyCode DebugSoulResetKey = KeyCode.Backslash;   // \

    // Debug silk override (UI-only, does not write PlayerData)
    private bool debugUseCustomSilk;
    private float debugSilk;

    // Shade soul override (driven by ShadeController)
    private bool shadeSoulOverride;
    private float shadeSoul;
    private float shadeSoulMax;

    // Slightly shrink masks
    private const float MaskScale = 0.88f;

    public void Init(PlayerData pd)
    {
        playerData = pd;
        LoadSprites();
        ComputeShadeFromPlayer();
        CreateUI();
        previousShadeHealth = shadeHealth;
    }

    private float GetUIScale()
    {
        float hScale = Mathf.Max(0.1f, (float)Screen.height / 1080f);
        return 1f + (hScale - 1f) * 0.5f;
    }

    private void CreateUI()
    {
        // Canvas
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f; // height bias
        gameObject.AddComponent<GraphicRaycaster>();

        // Soul orb root (replaces previous vessel frame)
        soulOrbRoot = new GameObject("SoulOrb").AddComponent<RectTransform>();
        soulOrbRoot.SetParent(canvas.transform, false);
        soulOrbRoot.anchorMin = soulOrbRoot.anchorMax = new Vector2(1, 1);
        soulOrbRoot.pivot = new Vector2(1, 1);
        Vector2 orbPixels = soulOrbSprite != null
            ? new Vector2(soulOrbSprite.rect.width, soulOrbSprite.rect.height)
            : new Vector2(96, 96);
        float uiScale = GetUIScale();
        soulOrbRoot.sizeDelta = orbPixels * uiScale * 0.85f;
        soulOrbRoot.anchoredPosition = new Vector2(-200f * uiScale, -20f * uiScale);
        soulOrbRoot.localScale = new Vector3(-1f, 1f, 1f); // mirror like previous frame

        // Background (always visible, dimmed)
        var soulBgGO = new GameObject("SoulBackground");
        soulBgGO.transform.SetParent(soulOrbRoot, false);
        soulBgImage = soulBgGO.AddComponent<Image>();
        soulBgImage.sprite = soulOrbSprite != null ? soulOrbSprite : BuildCircleSprite();
        soulBgImage.preserveAspect = true;
        var soulBgRect = soulBgImage.rectTransform;
        soulBgRect.anchorMin = new Vector2(0, 0);
        soulBgRect.anchorMax = new Vector2(1, 1);
        soulBgRect.pivot = new Vector2(0.5f, 0.5f);
        soulBgRect.anchoredPosition = Vector2.zero;
        soulBgRect.sizeDelta = Vector2.zero;
        // dark and see-through look when empty
        soulBgImage.color = new Color(0.6f, 0.6f, 0.6f, 0.35f);

        // Reveal mask (clips inner soul image from bottom to top)
        var maskGO = new GameObject("SoulRevealMask");
        maskGO.transform.SetParent(soulOrbRoot, false);
        soulRevealMask = maskGO.AddComponent<RectTransform>();
        soulRevealMask.anchorMin = new Vector2(0, 0);
        soulRevealMask.anchorMax = new Vector2(1, 0);
        soulRevealMask.pivot = new Vector2(0.5f, 0f); // bottom anchored
        soulRevealMask.anchoredPosition = Vector2.zero;
        soulRevealMask.sizeDelta = new Vector2(0f, 0f); // start empty (fully transparent)
        maskGO.AddComponent<RectMask2D>();

        // Inner orb image
        var soulImgGO = new GameObject("SoulImage");
        soulImgGO.transform.SetParent(soulOrbRoot, false);
        soulImage = soulImgGO.AddComponent<Image>();
        soulImage.sprite = soulOrbSprite != null ? soulOrbSprite : BuildCircleSprite();
        soulImage.preserveAspect = true;
        soulImage.maskable = true;
        soulImage.raycastTarget = false;
        soulImage.color = Color.white;
        // Use vertical fill on the image itself for robust filling behavior
        soulImage.type = Image.Type.Filled;
        soulImage.fillMethod = Image.FillMethod.Vertical;
        soulImage.fillOrigin = (int)Image.OriginVertical.Bottom;
        soulImage.fillAmount = 0f;
        var soulImgRect = soulImage.rectTransform;
        // Keep a constant size equal to the orb root, pinned to bottom-center of the mask.
        // This ensures the image is clipped by height without being squashed.
        soulImgRect.anchorMin = new Vector2(0.5f, 0f);
        soulImgRect.anchorMax = new Vector2(0.5f, 0f);
        soulImgRect.pivot = new Vector2(0.5f, 0f);
        soulImgRect.anchoredPosition = Vector2.zero;
        soulImgRect.sizeDelta = soulOrbRoot.sizeDelta;

        // Do not parent under RectMask; rely on Image vertical fill instead
        // (maskGO can remain unused without affecting visuals)

        try { Debug.Log($"[HelperMod] Soul orb sprite={(soulOrbSprite? soulOrbSprite.name: "<generated>")} size={orbPixels} uiScale={uiScale}"); } catch { }

        // Health masks container
        healthContainer = new GameObject("HealthContainer");
        healthContainer.transform.SetParent(canvas.transform, false);
        var hRect = healthContainer.AddComponent<RectTransform>();
        hRect.anchorMin = hRect.anchorMax = new Vector2(1, 1);
        hRect.pivot = new Vector2(1, 1);
        // Position masks row level with the vertical center of the orb
        Vector2 maskPixelsRef = maskSprite != null
            ? new Vector2(maskSprite.rect.width, maskSprite.rect.height)
            : new Vector2(33, 41);
        float maskHeight = maskPixelsRef.y * uiScale * MaskScale;
        float orbCenterY = soulOrbRoot.anchoredPosition.y - (soulOrbRoot.sizeDelta.y * 0.5f);
        // Align mask centers to orb center line
        hRect.anchoredPosition = new Vector2(
            soulOrbRoot.anchoredPosition.x,
            orbCenterY + (maskHeight * 0.5f)
        );

        BuildMasks(hRect, uiScale);
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
            var soulOrbPath = Path.Combine(dir, "soul_orb_glow0000.png");
            maskSprite = LoadSprite(maskPath);
            if (maskSprite == null) maskSprite = FindSpriteInGame("select_game_HUD_0001_health");
            frameSprite = LoadSprite(framePath);
            if (frameSprite == null) frameSprite = FindSpriteInGame("select_game_HUD_0002_health_frame");
            slashFrames = LoadSpriteSheet(slashPath, 8, 8);
            soulOrbSprite = LoadSprite(soulOrbPath);
        }
        catch { }
    }

    private Sprite LoadSprite(string path)
    {
        if (!File.Exists(path)) return null;
        var bytes = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        TryLoadImage(tex, bytes);
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    private Sprite[] LoadSpriteSheet(string path, int cols, int rows)
    {
        if (!File.Exists(path)) return new Sprite[0];
        var bytes = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        TryLoadImage(tex, bytes);
        tex.filterMode = FilterMode.Point;
        int w = tex.width / cols;
        int h = tex.height / rows;
        var sprites = new Sprite[cols * rows];
        int idx = 0;
        for (int y = rows - 1; y >= 0; y--)
            for (int x = 0; x < cols; x++)
                sprites[idx++] = Sprite.Create(tex, new Rect(x * w, y * h, w, h), new Vector2(0.5f, 0.5f));
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

        if (Input.GetKeyDown(DebugDamageKey)) shadeHealth = Mathf.Max(0, shadeHealth - 1);
        if (Input.GetKeyDown(DebugHealKey)) shadeHealth = Mathf.Min(shadeMax, shadeHealth + 1);

        // Debug soul controls (UI only)
        float sMax = Mathf.Max(1f, playerData.silkMax);
        float step = Mathf.Max(1f, sMax * 0.1f);
        if (Input.GetKeyDown(DebugSoulIncKey))
        {
            if (shadeSoulOverride)
            {
                // Drive Shade's soul directly to aid testing
                try
                {
                    var sc = UnityEngine.Object.FindFirstObjectByType<LegacyHelper.ShadeController>();
                    if (sc != null)
                    {
                        sc.shadeSoul = Mathf.Min(sc.shadeSoul + 11, sc.shadeSoulMax);
                    }
                }
                catch { }
                shadeSoul = Mathf.Min(shadeSoul + 11f, Mathf.Max(1f, shadeSoulMax));
            }
            else
            {
                float baseVal = debugUseCustomSilk ? debugSilk : playerData.silk;
                debugUseCustomSilk = true;
                debugSilk = Mathf.Min(baseVal + step, sMax);
            }
        }
        if (Input.GetKeyDown(DebugSoulDecKey))
        {
            if (shadeSoulOverride)
            {
                try
                {
                    var sc = UnityEngine.Object.FindFirstObjectByType<LegacyHelper.ShadeController>();
                    if (sc != null)
                    {
                        sc.shadeSoul = Mathf.Max(sc.shadeSoul - 11, 0);
                    }
                }
                catch { }
                shadeSoul = Mathf.Max(shadeSoul - 11f, 0f);
            }
            else
            {
                float baseVal = debugUseCustomSilk ? debugSilk : playerData.silk;
                debugUseCustomSilk = true;
                debugSilk = Mathf.Max(baseVal - step, 0f);
            }
        }
        if (Input.GetKeyDown(DebugSoulResetKey))
        {
            if (shadeSoulOverride)
            {
                try
                {
                    var sc = UnityEngine.Object.FindFirstObjectByType<LegacyHelper.ShadeController>();
                    if (sc != null)
                    {
                        sc.shadeSoul = 0;
                    }
                }
                catch { }
                shadeSoul = 0f;
            }
            else
            {
                debugUseCustomSilk = false;
                debugSilk = playerData.silk;
            }
        }

        SyncShadeFromPlayer();
        RefreshHealth();
        RefreshSoul();
    }

    private void RefreshHealth()
    {
        int cur = shadeHealth;
        if (cur < previousShadeHealth)
        {
            int lost = previousShadeHealth - cur;
            TryPlayPinnedHurtSfx(lost);
            for (int i = cur; i < previousShadeHealth && i < maskImages.Length; i++)
                StartCoroutine(LoseHealth(maskImages[i]));
        }
        for (int i = 0; i < cur && i < maskImages.Length; i++)
        {
            maskImages[i].sprite = maskSprite != null ? maskSprite : maskImages[i].sprite;
            maskImages[i].color = Color.white;
        }
        for (int i = cur; i < maskImages.Length; i++)
            if (i >= previousShadeHealth) maskImages[i].color = missingMaskColor;
        previousShadeHealth = cur;
    }

    private IEnumerator LoseHealth(Image img)
    {
        if (img == null) yield break;
        for (int i = 0; i < 2; i++)
        {
            img.color = Color.white; yield return new WaitForSeconds(0.05f);
            img.color = Color.red; yield return new WaitForSeconds(0.05f);
        }
        img.color = Color.white;
        var frames = GetSlashFrames();
        try { Debug.Log("[HelperMod] Slash frames: " + string.Join(", ", frames.Take(6).Select(f => f != null ? f.name : "<null>"))); } catch { }
        if (frames != null && frames.Length > 0)
        {            var slash = new GameObject("Slash");
            slash.transform.SetParent(img.transform, false);
            var sr = slash.AddComponent<Image>();
            sr.rectTransform.sizeDelta = img.rectTransform.sizeDelta * 2f;
            int count = Mathf.Min(6, frames.Length);
            for (int i = 0; i < count; i++)
            { sr.sprite = frames[i]; yield return new WaitForSeconds(0.03f); }
            GameObject.Destroy(slash);
        }
        float t = 0f; Color c = img.color;
        while (t < 0.3f)
        { t += Time.deltaTime; c.a = Mathf.Lerp(1f, 0f, t / 0.3f); img.color = c; yield return null; }
        img.color = missingMaskColor;
    }

    private void ComputeShadeFromPlayer()
    {
        if (playerData == null) { shadeMax = 0; shadeHealth = 0; return; }
        shadeMax = (playerData.maxHealth + 1) / 2;
        prevHornetMax = playerData.maxHealth;
        prevHornetHealth = playerData.health;
        if (previousShadeHealth == 0 && shadeHealth == 0)
            shadeHealth = (playerData.health + 1) / 2;
    }

    private void SyncShadeFromPlayer()
    {
        if (playerData == null) return;
        int newHornetMax = playerData.maxHealth;
        int newHornet = playerData.health;
        int newMax = (newHornetMax + 1) / 2;
        if (newMax != shadeMax)
        { shadeMax = newMax; RebuildMasks(); previousShadeHealth = Mathf.Min(previousShadeHealth, shadeMax); shadeHealth = Mathf.Min(shadeHealth, shadeMax); }
        // HUD is read-only: do not modify Shade health here.
        prevHornetHealth = newHornet; prevHornetMax = newHornetMax;
    }

    private void RebuildMasks()
    {
        if (healthContainer == null) return;
        foreach (var img in maskImages ?? Array.Empty<Image>()) if (img != null) Destroy(img.gameObject);
        BuildMasks(healthContainer.GetComponent<RectTransform>(), GetUIScale());
    }

    private void BuildMasks(RectTransform container, float uiScale)
    {
        maskImages = new Image[shadeMax];
        Vector2 maskPixels = maskSprite != null
            ? new Vector2(maskSprite.rect.width, maskSprite.rect.height)
            : new Vector2(33, 41);
        Vector2 maskSize = maskPixels * uiScale * MaskScale; // pixel-accurate with tuning

        float spacing = 6f * uiScale;
        float x = 0f;
        for (int i = 0; i < shadeMax; i++)
        {
            var m = new GameObject($"Mask{i}");
            m.transform.SetParent(container, false);
            var r = m.AddComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(1, 1);
            r.pivot = new Vector2(1, 1);
            r.sizeDelta = maskSize;
            r.anchoredPosition = new Vector2(-x, 0f);
            x += maskSize.x + spacing;

            var img = m.AddComponent<Image>();
            img.preserveAspect = true;
            img.sprite = maskSprite != null ? maskSprite : BuildMaskSprite();
            img.color = Color.white;
            maskImages[i] = img;
        }
    }

    private void RefreshSoul()
    {
        if (soulOrbRoot == null) return;
        // Prefer shade soul override when provided, otherwise use Hornet's silk
        float max = shadeSoulOverride ? Mathf.Max(1f, shadeSoulMax) : Mathf.Max(1f, playerData.silkMax);
        float silkVal = shadeSoulOverride ? Mathf.Clamp(shadeSoul, 0f, max) : (debugUseCustomSilk ? debugSilk : playerData.silk);
        float fill = Mathf.Clamp01(silkVal / max);
        // Drive image vertical fill
        if (soulImage != null) soulImage.fillAmount = fill;
        float fullH = soulOrbRoot.sizeDelta.y;
        var size = new Vector2(0f, fullH * fill);
        // Also drive the image's vertical fill to ensure visual update even if mask is unaffected
        if (soulImage != null) soulImage.fillAmount = fill;

        // Debug logging when value changes meaningfully
        if (Mathf.Abs(fill - lastLoggedFill) > 0.01f)
        {
            lastLoggedFill = fill;
            try { Debug.Log($"[HelperMod] Soul fill: silk={silkVal}/{max} fill={fill:F2} maskH={size.y:F1} rootH={fullH:F1}"); } catch { }
        }
    }

    // ShadeController drives this to show Shade's soul pool in the HUD
    public void SetShadeSoul(int current, int max)
    {
        shadeSoulOverride = true;
        shadeSoul = Mathf.Max(0, current);
        shadeSoulMax = Mathf.Max(1, max);
    }

    // Allow ShadeController to drive Shade HP and max
        // Allow ShadeController to drive Shade HP and max
    public void SetShadeStats(int current, int max)
    {
        int newMax = Mathf.Max(1, max);
        int newCur = Mathf.Clamp(current, 0, newMax);
        bool maxChanged = (newMax != shadeMax);
        shadeMax = newMax;
        shadeHealth = newCur;
        if (maxChanged) RebuildMasks();
        RefreshHealth();
    }


    // Allow turning off shade soul override (fallback to Hornet's silk)
    public void ClearShadeSoulOverride()
    {
        shadeSoulOverride = false;
    }

    public void SetVisible(bool visible)
    { if (canvas != null) canvas.enabled = visible; else gameObject.SetActive(visible); }

    public void SetPlayerData(PlayerData pd)
    {
        if (pd == playerData) return;
        playerData = pd;
        int oldMax = shadeMax;
        ComputeShadeFromPlayer();
        if (shadeMax != oldMax)
        {
            RebuildMasks();
            previousShadeHealth = Mathf.Min(previousShadeHealth, shadeMax);
        }
        RefreshHealth();
        RefreshSoul();
    }

    private AudioSource sfxSource;
    // Removed unused hornetDamageClip to silence compiler warning
    private List<AudioClip> shadeHurtCandidates;
    private int shadeHurtIdx;
    private AudioClip pinnedHurtSingle;
    private AudioClip pinnedHurtDouble;

    private void TryPlayPinnedHurtSfx(int lost)
    {
        try
        {
            if (sfxSource == null)
            {
                var go = new GameObject("ShadeHUD_SFX");
                go.transform.SetParent(this.transform, false);
                sfxSource = go.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false; sfxSource.spatialBlend = 0f; sfxSource.volume = 1f;
            }
            if (pinnedHurtSingle == null || pinnedHurtDouble == null)
            {
                AudioClip[] all = Resources.FindObjectsOfTypeAll<AudioClip>();
                if (all != null)
                {
                    foreach (var c in all)
                    {
                        if (c == null) continue; string n = c.name ?? string.Empty;
                        if (pinnedHurtSingle == null && string.Equals(n, "hero_damage", StringComparison.OrdinalIgnoreCase)) pinnedHurtSingle = c;
                        if (pinnedHurtDouble == null && string.Equals(n, "hero_double_damage", StringComparison.OrdinalIgnoreCase)) pinnedHurtDouble = c;
                        if (pinnedHurtSingle != null && pinnedHurtDouble != null) break;
                    }
                    // if exact not found, try contains
                    if (pinnedHurtSingle == null)
                    {
                        foreach (var c in all) { var n = c != null ? c.name : null; if (!string.IsNullOrEmpty(n) && n.IndexOf("hero_damage", StringComparison.OrdinalIgnoreCase) >= 0) { pinnedHurtSingle = c; break; } }
                    }
                    if (pinnedHurtDouble == null)
                    {
                        foreach (var c in all) { var n = c != null ? c.name : null; if (!string.IsNullOrEmpty(n) && n.IndexOf("hero_double_damage", StringComparison.OrdinalIgnoreCase) >= 0) { pinnedHurtDouble = c; break; } }
                    }
                }
            }
            AudioClip clip = (lost >= 2 && pinnedHurtDouble != null) ? pinnedHurtDouble : pinnedHurtSingle;
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
                Debug.Log("[HelperMod] Shade hurt SFX (pinned): " + clip.name + " (lost=" + lost + ")");
                return;
            }
        }
        catch { }
        // fallback to previous cycling behavior if pinned not found
        TryPlayDamageSfx();
    }

    private void TryPlayDamageSfx()
    {
        try
        {
            if (sfxSource == null)
            {
                var go = new GameObject("ShadeHUD_SFX");
                go.transform.SetParent(this.transform, false);
                sfxSource = go.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false; sfxSource.spatialBlend = 0f; sfxSource.volume = 1f;
            }
            if (shadeHurtCandidates == null || shadeHurtCandidates.Count == 0)
            { shadeHurtCandidates = BuildShadeHurtCandidates(); shadeHurtIdx = 0; try { Debug.Log("[HelperMod] Built shade hurt SFX list: " + string.Join(", ", shadeHurtCandidates.Select(c => c != null ? c.name : "<null>"))); } catch { } }
            if (shadeHurtCandidates != null && shadeHurtCandidates.Count > 0)
            {
                var clip = shadeHurtCandidates[shadeHurtIdx % shadeHurtCandidates.Count]; shadeHurtIdx++;
                if (clip != null) { sfxSource.PlayOneShot(clip); Debug.Log("[HelperMod] Shade hurt SFX: " + clip.name + " (" + shadeHurtIdx + "/" + shadeHurtCandidates.Count + ")"); }
            }
        }
        catch { }
    }

    private List<AudioClip> BuildShadeHurtCandidates()
    {
        var list = new List<AudioClip>(); var seen = new HashSet<AudioClip>();
        try { var fsmClips = FindHurtClipsFromHornetFSM(); if (fsmClips != null) foreach (var c in fsmClips) if (c != null && seen.Add(c)) list.Add(c); } catch { }
        try {
            var all = Resources.FindObjectsOfTypeAll<AudioClip>();
            if (all != null && all.Length > 0)
            {
                var ordered = all.Where(c => c != null).Select(c => new { clip = c, n = (c.name ?? string.Empty) })
                    .Where(x => x.n.IndexOf("hurt", StringComparison.OrdinalIgnoreCase) >= 0
                             || x.n.IndexOf("take_hit", StringComparison.OrdinalIgnoreCase) >= 0
                             || x.n.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0
                             || x.n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0
                             || x.n.IndexOf("hornet", StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderByDescending(x => (x.n.IndexOf("take", StringComparison.OrdinalIgnoreCase) >= 0 && x.n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0) ? 6 : 0)
                    .ThenByDescending(x => x.n.IndexOf("hurt", StringComparison.OrdinalIgnoreCase) >= 0 ? 1 : 0)
                    .ThenByDescending(x => x.n.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0 ? 1 : 0)
                    .ThenByDescending(x => x.n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0 ? 1 : 0)
                    .ThenByDescending(x => x.n.IndexOf("hornet", StringComparison.OrdinalIgnoreCase) >= 0 ? 1 : 0)
                    .ThenBy(x => x.n)
                    .Select(x => x.clip);
                foreach (var c in ordered)
                {
                    var n = c.name ?? string.Empty;
                    if (n.IndexOf("deal_damage", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                    if (n.IndexOf("attack", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                    if (n.IndexOf("tool", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                    if (n.IndexOf("slash", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                    if (n.IndexOf("charge", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                    if (seen.Add(c)) list.Add(c);
                }
            }
        } catch { }
        return list;
    }

    private List<AudioClip> FindHurtClipsFromHornetFSM()
    {
        var result = new List<AudioClip>();
        try
        {
            HeroController hc = (HeroController.instance != null) ? HeroController.instance : (GameManager.instance != null ? GameManager.instance.hero_ctrl : null);
            if (hc == null) return result;
            System.Type fsmType = System.Type.GetType("HutongGames.PlayMaker.PlayMakerFSM, PlayMaker");
            if (fsmType == null) fsmType = System.Type.GetType("PlayMakerFSM, PlayMaker");
            if (fsmType == null) return result;
            Component[] fsms = hc.GetComponentsInChildren(fsmType, true);
            if (fsms == null || fsms.Length == 0) return result;
            foreach (Component fsm in fsms)
            {
                if (fsm == null) continue;
                var fsmProp = fsm.GetType().GetProperty("Fsm", BindingFlags.Instance | BindingFlags.Public);
                var fsmObj = (fsmProp != null) ? fsmProp.GetValue(fsm, null) : null; if (fsmObj == null) continue;
                var fsmObjType = fsmObj.GetType();
                var nameProp = fsmObjType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
                string fsmName = (nameProp != null) ? (nameProp.GetValue(fsmObj, null) as string) : null;
                string fsmNameLower = (fsmName ?? string.Empty).ToLowerInvariant();
                if (!(fsmNameLower.Contains("damage") || fsmNameLower.Contains("hurt") || fsmNameLower.Contains("hit") || fsmNameLower.Contains("hero"))) continue;
                var statesProp = fsmObjType.GetProperty("States", BindingFlags.Instance | BindingFlags.Public);
                var states = (statesProp != null) ? (statesProp.GetValue(fsmObj, null) as Array) : null; if (states == null) continue;
                foreach (var state in states)
                {
                    if (state == null) continue;
                    var stType = state.GetType();
                    var stNameProp = stType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
                    string stateName = (stNameProp != null) ? (stNameProp.GetValue(state, null) as string) : null;
                    string stateNameLower = (stateName ?? string.Empty).ToLowerInvariant();
                    if (!(stateNameLower.Contains("damage") || stateNameLower.Contains("hurt") || stateNameLower.Contains("hit") || stateNameLower.Contains("impact"))) continue;
                    var actionsProp = stType.GetProperty("Actions", BindingFlags.Instance | BindingFlags.Public);
                    var actions = (actionsProp != null) ? (actionsProp.GetValue(state, null) as Array) : null; if (actions == null) continue;
                    foreach (var action in actions)
                    {
                        if (action == null) continue;
                        var at = action.GetType();
                        var fields = at.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var f in fields)
                        {
                            try
                            {
                                if (typeof(AudioClip).IsAssignableFrom(f.FieldType)) { var clip = f.GetValue(action) as AudioClip; if (clip != null && !result.Contains(clip)) result.Add(clip); }
                                if (typeof(AudioSource).IsAssignableFrom(f.FieldType)) { var src = f.GetValue(action) as AudioSource; if (src != null && src.clip != null && !result.Contains(src.clip)) result.Add(src.clip); }
                                var ft = f.FieldType;
                                if (ft != null && ft.FullName != null && ft.FullName.Contains("HutongGames.PlayMaker.FsmObject"))
                                {
                                    var pmObj = f.GetValue(action);
                                    if (pmObj != null)
                                    {
                                        var valProp = pmObj.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
                                        var val = (valProp != null) ? valProp.GetValue(pmObj, null) : null;
                                        var clip2 = val as AudioClip; if (clip2 != null && !result.Contains(clip2)) result.Add(clip2);
                                    }
                                }
                            }
                            catch { }
                        }
                        var props = at.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var p in props)
                        {
                            try
                            {
                                if (!p.CanRead) continue; var pt = p.PropertyType;
                                if (typeof(AudioClip).IsAssignableFrom(pt)) { var clip = p.GetValue(action, null) as AudioClip; if (clip != null && !result.Contains(clip)) result.Add(clip); }
                                if (typeof(AudioSource).IsAssignableFrom(pt)) { var src = p.GetValue(action, null) as AudioSource; if (src != null && src.clip != null && !result.Contains(src.clip)) result.Add(src.clip); }
                                if (pt != null && pt.FullName != null && pt.FullName.Contains("HutongGames.PlayMaker.FsmObject"))
                                { var pobj = p.GetValue(action, null); if (pobj != null) { var vprop = pobj.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public); var v = (vprop != null) ? vprop.GetValue(pobj, null) : null; var clip3 = v as AudioClip; if (clip3 != null && !result.Contains(clip3)) result.Add(clip3); } }
                            }
                            catch { }
                        }
                    }
                }
            }
        }
        catch { }
        return result;
    }

    private AudioClip FindHornetDamageClip()
    {
        try
        {
            // fallback only
            AudioClip[] all = Resources.FindObjectsOfTypeAll<AudioClip>();
            if (all != null && all.Length > 0)
            {
                AudioClip best = null; int bestScore = int.MinValue;
                foreach (AudioClip clip in all)
                {
                    if (clip == null) continue; string n = clip.name ?? string.Empty; int score = 0;
                    if (n.IndexOf("hurt", StringComparison.OrdinalIgnoreCase) >= 0) score += 5;
                    if (n.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0) score += 4;
                    if (n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0) score += 3;
                    if (n.IndexOf("hornet", StringComparison.OrdinalIgnoreCase) >= 0) score += 3;
                    if (n.IndexOf("hero", StringComparison.OrdinalIgnoreCase) >= 0 || n.IndexOf("player", StringComparison.OrdinalIgnoreCase) >= 0) score += 1;
                    if (n.IndexOf("deal_damage", StringComparison.OrdinalIgnoreCase) >= 0) score -= 5;
                    if (n.IndexOf("attack", StringComparison.OrdinalIgnoreCase) >= 0) score -= 4;
                    if (n.IndexOf("tool", StringComparison.OrdinalIgnoreCase) >= 0) score -= 3;
                    if (n.IndexOf("slash", StringComparison.OrdinalIgnoreCase) >= 0) score -= 2;
                    if (n.IndexOf("charge", StringComparison.OrdinalIgnoreCase) >= 0) score -= 2;
                    if (score > bestScore) { bestScore = score; best = clip; }
                }
                if (best != null) { Debug.Log("[HelperMod] Using damage SFX clip: " + best.name); return best; }
            }
        }
        catch { }
        return null;
    }

    private Sprite FindSpriteInGame(string namePart)
    {
        if (string.IsNullOrEmpty(namePart)) return null; string key = Path.GetFileNameWithoutExtension(namePart);
        var all = Resources.FindObjectsOfTypeAll<Sprite>(); Sprite best = null; int bestScore = int.MinValue;
        foreach (var sp in all)
        { if (sp == null) continue; string n = sp.name ?? string.Empty; int score = 0; if (string.Equals(n, key, StringComparison.OrdinalIgnoreCase)) score += 1000; if (n.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0) score += 100; score += (int)(sp.rect.width + sp.rect.height); if (score > bestScore) { bestScore = score; best = sp; } }
        return best;
    }

    private Sprite[] GetSlashFrames()
    {
        if (_slashFramesCache != null && _slashFramesCache.Length > 0) return _slashFramesCache;
        var all = Resources.FindObjectsOfTypeAll<Sprite>(); var list = new List<Sprite>();
        foreach (var sp in all)
        { if (sp == null) continue; string n = sp.name ?? string.Empty; if (n.IndexOf("charge_slash", StringComparison.OrdinalIgnoreCase) >= 0 || n.IndexOf("slash", StringComparison.OrdinalIgnoreCase) >= 0 || n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0 || n.IndexOf("impact", StringComparison.OrdinalIgnoreCase) >= 0) list.Add(sp); }
        if (list.Count > 0)
        { _slashFramesCache = list.OrderByDescending(s => s.name.IndexOf("charge_slash", StringComparison.OrdinalIgnoreCase) >= 0).ThenByDescending(s => s.name.IndexOf("_0002_", StringComparison.OrdinalIgnoreCase) >= 0).ThenBy(s => s.name).ToArray(); return _slashFramesCache; }
        if (slashFrames != null && slashFrames.Length > 0) { _slashFramesCache = slashFrames; return _slashFramesCache; }
        return Array.Empty<Sprite>();
    }

    private Sprite[] _slashFramesCache;

    private static bool TryLoadImage(Texture2D tex, byte[] bytes)
    {
        try
        {
            var t = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
            if (t != null)
            {
                var m = t.GetMethod("LoadImage", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(Texture2D), typeof(byte[]), typeof(bool) }, null);
                if (m != null) { m.Invoke(null, new object[] { tex, bytes, false }); return true; }
            }
        }
        catch { }
        return false;
    }
}



