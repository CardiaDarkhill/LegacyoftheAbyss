#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SimpleHUD : MonoBehaviour
{
    private PlayerData playerData;

    // Health masks state
    private Image[] maskImages;
    private readonly HashSet<Image> animatingMaskImages = new HashSet<Image>();
    private Sprite maskSprite;
    private readonly Color missingMaskColor = new Color(0.2f, 0.2f, 0.2f, 0.45f);
    private readonly Color overcharmMaskColor = Color.white;
    private readonly Color overcharmBackdropColor = new Color(0.85f, 0.25f, 0.25f, 0.1344f);
    private readonly Color overcharmBackdropSpriteColor = new Color(1f, 1f, 1f, 0.392f);
    private readonly Color lifebloodMaskColor = new Color(0.4f, 0.75f, 1f, 1f);
    private readonly Color lifebloodMissingColor = new Color(0.28f, 0.46f, 0.66f, 0.45f);

    // Soul orb state
    private Sprite soulOrbSprite;
    private RectTransform soulOrbRoot;
    private RectTransform soulRevealMask;
    private Image soulImage;
    private Image soulBgImage;
    private Sprite overcharmBackdropSprite;

    // Fallback/legacy assets
    private Sprite frameSprite;
    private Sprite[] slashFrames;

    // Shade health state
    private int shadeMax;
    private int shadeHealth;
    private int shadeLifebloodMax;
    private int shadeLifeblood;
    private int previousShadeTotalHealth;
    private int prevHornetHealth;
    private int prevHornetMax;
    private bool hasExplicitShadeStats;
    private bool shadeOvercharmed;
    private bool shadeAssistModeActive;
    private bool suppressNextDamageSound;
    private bool pendingMaskRefresh;

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
    private const float OvercharmBackdropScale = 3.4f;
    private const float OvercharmBackdropRotation = 180f;
    private const float OvercharmBackdropHorizontalOffsetFraction = 3.2f;
    private const float OvercharmBackdropVerticalOffsetFraction = 1.4f;
    private const int OvercharmBackdropReferenceMaskCount = 3;
    private Vector2 overcharmMaskSize = Vector2.zero;
    private float overcharmMaskSpacing;

    public void Init(PlayerData pd)
    {
        playerData = pd;
        LoadSprites();
        ComputeShadeFromPlayer();
        CreateUI();
        previousShadeTotalHealth = shadeHealth + shadeLifeblood;
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
    public void SetShadeStats(int currentNormal, int maxNormal, int lifebloodCurrent, int lifebloodMax)
    {
        bool firstExplicit = !hasExplicitShadeStats;
        hasExplicitShadeStats = true;

        int newMaxNormal = Mathf.Max(0, maxNormal);
        int newMaxLifeblood = Mathf.Max(0, lifebloodMax);
        int newCurNormal = Mathf.Clamp(currentNormal, 0, newMaxNormal);
        int newCurLifeblood = Mathf.Clamp(lifebloodCurrent, 0, newMaxLifeblood);

        bool maxChanged = (newMaxNormal != shadeMax) || (newMaxLifeblood != shadeLifebloodMax);

        shadeMax = newMaxNormal;
        shadeLifebloodMax = newMaxLifeblood;
        shadeHealth = newCurNormal;
        shadeLifeblood = newCurLifeblood;

        if (firstExplicit)
        {
            previousShadeTotalHealth = shadeHealth + shadeLifeblood;
            suppressNextDamageSound = false;
        }

        if (maxChanged)
        {
            if (shadeAssistModeActive || healthContainer == null || maskImages == null)
            {
                pendingMaskRefresh = true;
            }
            else
            {
                RebuildMasks();
                pendingMaskRefresh = false;
            }
        }

        HandleAssistVisibilityChange();
        RefreshHealth();
    }

    public void SetShadeAssistMode(bool assistActive)
    {
        if (shadeAssistModeActive == assistActive)
        {
            return;
        }

        shadeAssistModeActive = assistActive;
        pendingMaskRefresh |= !assistActive;
        previousShadeTotalHealth = shadeHealth + shadeLifeblood;
        HandleAssistVisibilityChange();
        RefreshHealth();
    }

    public void SuppressNextShadeDamageSfx()
    {
        suppressNextDamageSound = true;
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
            previousShadeTotalHealth = Mathf.Min(previousShadeTotalHealth, shadeMax + shadeLifebloodMax);
        }
        RefreshHealth();
        RefreshSoul();
    }
}

#nullable restore
