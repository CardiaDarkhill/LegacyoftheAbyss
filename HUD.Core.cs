#nullable disable
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class SimpleHUD : MonoBehaviour
{
    private PlayerData playerData;

    // Health masks state
    private Image[] maskImages;
    private Sprite maskSprite;
    private readonly Color missingMaskColor = new Color(0.2f, 0.2f, 0.2f, 0.45f);
    private readonly Color overcharmMaskColor = new Color(1f, 0.5f, 0.5f, 1f);
    private readonly Color overcharmBackdropColor = new Color(0.85f, 0.25f, 0.25f, 0.28f);

    // Soul orb state
    private Sprite soulOrbSprite;
    private RectTransform soulOrbRoot;
    private RectTransform soulRevealMask;
    private Image soulImage;
    private Image soulBgImage;

    // Fallback/legacy assets
    private Sprite frameSprite;
    private Sprite[] slashFrames;

    // Shade health state
    private int shadeMax;
    private int shadeHealth;
    private int previousShadeHealth;
    private int prevHornetHealth;
    private int prevHornetMax;
    private bool hasExplicitShadeStats;
    private bool shadeOvercharmed;
    private bool suppressNextDamageSound;

    // UI containers
    private GameObject healthContainer;
    private Image overcharmBackdrop;
    private Canvas canvas;
    private CanvasScaler scaler;
    private CanvasGroup canvasGroup;
    private ShadeUnlockPopup unlockPopup;

    private Vector3 orbGameplayScale = Vector3.one;
    private Vector3 orbMenuScale = Vector3.one;
    private Vector3 healthGameplayScale = Vector3.one;
    private Vector3 healthMenuScale = Vector3.one;

    private const KeyCode DebugDamageKey = KeyCode.None; // disabled
    private const KeyCode DebugHealKey = KeyCode.None; // disabled
    private const KeyCode DebugSoulDecKey = KeyCode.None;   // disabled
    private const KeyCode DebugSoulIncKey = KeyCode.None;  // disabled
    private const KeyCode DebugSoulResetKey = KeyCode.Backslash;   // \

    // Debug silk override (UI-only, does not write PlayerData)
    private bool debugUseCustomSilk;
    private float debugSilk;

    // Shade soul override (driven by ShadeController)
    private bool shadeSoulOverride;
    private float shadeSoul;
    private float shadeSoulMax;

    private const float MaskScale = 0.88f; // slightly shrink masks
    private Vector2 overcharmMaskSize = Vector2.zero;
    private float overcharmMaskSpacing;

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

    private void Update()
    {
        UpdatePauseFade();

        if (playerData == null) return;

        // Debug: Shade HP adjust
        if (Input.GetKeyDown(DebugDamageKey))
        {
            shadeHealth = Mathf.Max(0, shadeHealth - 1);
            if (ModConfig.Instance.logHud)
            {
                try { Debug.Log("[SimpleHUD] Debug: Shade HP -1"); } catch { }
            }
        }
        if (Input.GetKeyDown(DebugHealKey))
        {
            shadeHealth = Mathf.Min(shadeMax, shadeHealth + 1);
            if (ModConfig.Instance.logHud)
            {
                try { Debug.Log("[SimpleHUD] Debug: Shade HP +1"); } catch { }
            }
        }

        // Debug soul controls (UI or Shade override)
        float sMax = shadeSoulOverride ? Mathf.Max(1f, shadeSoulMax) : Mathf.Max(1f, playerData.silkMax);
        float step = Mathf.Max(1f, sMax * 0.1f);
        if (Input.GetKeyDown(DebugSoulIncKey))
        {
            if (shadeSoulOverride)
            {
                try
                {
                    var sc = UnityEngine.Object.FindFirstObjectByType<LegacyHelper.ShadeController>();
                    if (sc != null) sc.shadeSoul = Mathf.Min(sc.shadeSoul + 11, sc.shadeSoulMax);
                }
                catch { }
                shadeSoul = Mathf.Min(shadeSoul + 11f, Mathf.Max(1f, shadeSoulMax));
                if (ModConfig.Instance.logHud)
                {
                    try { Debug.Log("[SimpleHUD] Debug: Shade Soul +11"); } catch { }
                }
            }
            else
            {
                float baseVal = debugUseCustomSilk ? debugSilk : playerData.silk;
                debugUseCustomSilk = true;
                debugSilk = Mathf.Min(baseVal + step, sMax);
                if (ModConfig.Instance.logHud)
                {
                    try { Debug.Log("[SimpleHUD] Debug: Hornet Silk +step"); } catch { }
                }
            }
        }
        if (Input.GetKeyDown(DebugSoulDecKey))
        {
            if (shadeSoulOverride)
            {
                try
                {
                    var sc = UnityEngine.Object.FindFirstObjectByType<LegacyHelper.ShadeController>();
                    if (sc != null) sc.shadeSoul = Mathf.Max(sc.shadeSoul - 11, 0);
                }
                catch { }
                shadeSoul = Mathf.Max(shadeSoul - 11f, 0f);
                if (ModConfig.Instance.logHud)
                {
                    try { Debug.Log("[SimpleHUD] Debug: Shade Soul -11"); } catch { }
                }
            }
            else
            {
                float baseVal = debugUseCustomSilk ? debugSilk : playerData.silk;
                debugUseCustomSilk = true;
                debugSilk = Mathf.Max(baseVal - step, 0f);
                if (ModConfig.Instance.logHud)
                {
                    try { Debug.Log("[SimpleHUD] Debug: Hornet Silk -step"); } catch { }
                }
            }
        }
        if (Input.GetKeyDown(DebugSoulResetKey))
        {
            if (shadeSoulOverride)
            {
                try
                {
                    var sc = UnityEngine.Object.FindFirstObjectByType<LegacyHelper.ShadeController>();
                    if (sc != null) sc.shadeSoul = 0;
                }
                catch { }
                shadeSoul = 0f;
                if (ModConfig.Instance.logHud)
                {
                    try { Debug.Log("[SimpleHUD] Debug: Shade Soul reset"); } catch { }
                }
            }
            else
            {
                debugUseCustomSilk = false;
                debugSilk = playerData.silk;
                if (ModConfig.Instance.logHud)
                {
                    try { Debug.Log("[SimpleHUD] Debug: Hornet Silk reset"); } catch { }
                }
            }
        }

        SyncShadeFromPlayer();
        RefreshHealth();
        RefreshSoul();
    }

    private void UpdatePauseFade()
    {
        if (canvasGroup == null)
            return;

        bool menuActive = ShouldTreatAsMenu();
        float target = menuActive ? 0.35f : 1f;
        float current = canvasGroup.alpha;
        if (!Mathf.Approximately(current, target))
        {
            float step = Mathf.Max(0.01f, Time.unscaledDeltaTime * 5f);
            canvasGroup.alpha = Mathf.MoveTowards(current, target, step);
        }

        UpdateMenuOrientation(menuActive);
    }

    private bool ShouldTreatAsMenu()
    {
        try
        {
            return MenuStateUtility.IsMenuActive();
        }
        catch
        {
            return false;
        }
    }

    private void UpdateMenuOrientation(bool menuActive)
    {
        var targetOrbScale = menuActive ? orbMenuScale : orbGameplayScale;
        if (soulOrbRoot != null && soulOrbRoot.localScale != targetOrbScale)
        {
            soulOrbRoot.localScale = targetOrbScale;
        }

        if (healthContainer != null)
        {
            var rect = healthContainer.GetComponent<RectTransform>();
            if (rect != null)
            {
                var targetScale = menuActive ? healthMenuScale : healthGameplayScale;
                if (rect.localScale != targetScale)
                {
                    rect.localScale = targetScale;
                }
            }
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
    public void SetShadeStats(int current, int max)
    {
        bool firstExplicit = !hasExplicitShadeStats;
        hasExplicitShadeStats = true;
        int newMax = Mathf.Max(1, max);
        int newCur = Mathf.Clamp(current, 0, newMax);
        bool maxChanged = (newMax != shadeMax);
        shadeMax = newMax;
        shadeHealth = newCur;
        if (firstExplicit)
        {
            previousShadeHealth = newCur;
            suppressNextDamageSound = false;
        }
        if (maxChanged) RebuildMasks();
        RefreshHealth();
    }

    public void SetShadeOvercharmed(bool overcharmed)
    {
        if (shadeOvercharmed == overcharmed)
            return;

        shadeOvercharmed = overcharmed;
        RefreshOvercharmBackdrop();
        RefreshHealth();
    }

    public void ClearShadeSoulOverride() => shadeSoulOverride = false;

    public void SetVisible(bool visible)
    {
        if (canvas != null) canvas.enabled = visible; else gameObject.SetActive(visible);
    }

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
}

#nullable restore
