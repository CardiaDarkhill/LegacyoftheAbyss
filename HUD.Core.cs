#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using UnityEngine.UI;

public partial class SimpleHUD : MonoBehaviour
{
    private PlayerData playerData;

    // Health masks state
    private Image[] maskImages;
    private readonly HashSet<Image> animatingMaskImages = new HashSet<Image>();
    private Sprite maskSprite;
    private Sprite hivebloodMaskSprite;
    private readonly Color missingMaskColor = new Color(0.2f, 0.2f, 0.2f, 0.45f);
    private readonly Color overcharmMaskColor = Color.white;
    private readonly Color overcharmBackdropColor = new Color(0.85f, 0.25f, 0.25f, 0.1344f);
    private readonly Color overcharmBackdropSpriteColor = new Color(1f, 1f, 1f, 0.392f);
    private readonly Color lifebloodMaskColor = new Color(0.4f, 0.75f, 1f, 1f);
    private readonly Color hivebloodMaskColor = new Color(1f, 0.72f, 0.18f, 1f);
    private readonly Color lifebloodMissingColor = new Color(0.28f, 0.46f, 0.66f, 0.45f);

    // Soul orb state
    private Sprite soulOrbSprite;

    /// <summary>The orb's filled interior, drawn under the mask. Hollow Knight draws these separately.</summary>
    private Sprite soulOrbFillSprite;

    /// <summary>Set when the mask art came out of the atlas on its side. See KnightAssets.IsSpriteRotated.</summary>
    private bool maskSpriteRotated;

    /// <summary>The same, for Hiveblood's masks. The two are packed differently, so they are asked separately.</summary>
    private bool hivebloodMaskSpriteRotated;

    /// <summary>
    /// Whether the Hiveblood masks are the game's own art rather than the plain mask painted
    /// orange. The art carries its colour, so painting it again would only oversaturate it.
    /// </summary>
    private bool hivebloodMaskIsBundleArt;

    /// <summary>Whether the atlas stored the plate turned on its side, as tk2d often does.</summary>
    private bool frameSpriteRotated;

    /// <summary>The HUD plate behind the orb, or null when there is no frame art to draw.</summary>
    private Image hudFrameImage;
    private RectTransform soulOrbRoot;
    private RectTransform soulRevealMask;
    private Image soulImage;
    private Image soulBgImage;
    private Sprite overcharmBackdropSprite;

    // Fallback/legacy assets
    private Sprite frameSprite;

    // Shade health state
    private int shadeMax;
    private int shadeHealth;
    private int shadeLifebloodMax;
    private int shadeLifeblood;
    private int previousShadeTotalHealth;
    private bool hasExplicitShadeStats;
    private bool shadeOvercharmed;
    private bool shadeAssistModeActive;
    private bool suppressNextDamageSound;
    private bool pendingMaskRefresh;
    private Image hivebloodPreviewMask;
    private bool hivebloodEquipped;
    private ShadeCharmInventory subscribedCharmInventory;
    private bool charmInventoryDirty;

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

    // Rebindable via the Controls menu's debug rows, shown only when
    // ModConfig.Instance.debugKeysEnabled is on (see ShadeSettingsMenu.BuildControlsMenu).
    // Defaults live in ShadeInputConfig.ResetToDefaults.

    // Debug silk override (UI-only, does not write PlayerData)
    private bool debugUseCustomSilk;
    private float debugSilk;

    // Shade soul override (driven by ShadeController)
    private bool shadeSoulOverride;
    private float shadeSoul;
    private float shadeSoulMax;

    private const float MaskScale = 0.88f; // slightly shrink masks

    /// <summary>
    /// The height a mask is drawn at, in the pixels of the still this HUD was laid out against.
    /// <para>
    /// The layout used to be taken from whatever the mask sprite happened to measure, which was fine
    /// while that was always the same 33x41 still. It is not any more: the art now comes from the
    /// Knight bundle at 70x57, and sizing straight off the source would have made every mask half as
    /// big again and a different shape. Held to this height and the sprite's own aspect instead, so
    /// swapping the art changes how sharp the HUD is and not how big.
    /// </para>
    /// </summary>
    private const float MaskReferenceHeightPixels = 41f;

    /// <summary>The drawn size of one mask, at <see cref="MaskReferenceHeightPixels"/> and the source's aspect.</summary>
    private Vector2 MeasureMask(float uiScale)
    {
        Vector2 pixels = maskSprite != null
            ? new Vector2(maskSprite.rect.width, maskSprite.rect.height)
            : new Vector2(33f, 41f);

        // Displayed, not stored: a rotated frame occupies its atlas region turned on its side.
        if (maskSpriteRotated)
        {
            pixels = new Vector2(pixels.y, pixels.x);
        }

        float height = MaskReferenceHeightPixels * uiScale * MaskScale;
        float aspect = pixels.y > 0.001f ? pixels.x / pixels.y : 1f;
        return new Vector2(height * aspect, height);
    }
    private const float OvercharmBackdropScale = 3.4f;
    private const float OvercharmBackdropRotation = 180f;
    private const float OvercharmBackdropHorizontalOffsetFraction = 3.2f;
    private const float OvercharmBackdropVerticalOffsetFraction = 1.4f;
    private const int OvercharmBackdropReferenceMaskCount = 3;
    private Vector2 overcharmMaskSize = Vector2.zero;
    private float overcharmMaskSpacing;
    private const float HivebloodPreviewFirstStageSeconds = 3.5f;
    private const float HivebloodPreviewSecondStageSeconds = 7f;

    public void Init(PlayerData pd)
    {
        playerData = pd;
        LoadSprites();
        ComputeShadeFromPlayer();
        CreateUI();
        SubscribeToCharmInventory();
        charmInventoryDirty = true;
        previousShadeTotalHealth = shadeHealth + shadeLifeblood;
        RefreshHealth();
    }

    private float GetUIScale()
    {
        float hScale = Mathf.Max(0.1f, (float)Screen.height / 1080f);
        return 1f + (hScale - 1f) * 0.5f;
    }

    private void Update()
    {
        ApplyHudTuning();

        UpdatePauseFade();
        SubscribeToCharmInventory();

        if (charmInventoryDirty && playerData != null)
        {
            charmInventoryDirty = false;
            RefreshHealth();
        }

        if (playerData == null) return;

        HandleDebugKeys();
        SyncShadeFromPlayer();
        RefreshHealth();
        RefreshSoul();
    }

    /// <summary>
    /// Developer HP/soul adjustment keys. Gated so shipped builds neither poll these
    /// keys every frame nor expose the cheats.
    /// </summary>
    private void HandleDebugKeys()
    {
        if (!ModConfig.Instance.debugKeysEnabled) return;

        // Debug: Shade HP adjust. Applied to the companions, exactly as the soul keys below
        // are - writing the HUD field here made the damage cosmetic, so it came back on the
        // next scene and focus could not heal it. The masks follow from the push.
        if (ShadeInput.WasActionPressed(ShadeAction.DebugDamageShade))
        {
            foreach (var sc in LegacyHelper.ShadeController.ActiveInstances)
            {
                if (sc != null) sc.DebugAdjustHealth(-1);
            }

            if (ModConfig.Instance.logHud)
            {
                Debug.Log("[SimpleHUD] Debug: Shade HP -1");
            }
        }
        if (ShadeInput.WasActionPressed(ShadeAction.DebugHealShade))
        {
            foreach (var sc in LegacyHelper.ShadeController.ActiveInstances)
            {
                if (sc != null) sc.DebugAdjustHealth(1);
            }

            if (ModConfig.Instance.logHud)
            {
                Debug.Log("[SimpleHUD] Debug: Shade HP +1");
            }
        }

        // Debug soul controls (UI or Shade override)
        float sMax = shadeSoulOverride ? Mathf.Max(1f, shadeSoulMax) : Mathf.Max(1f, playerData.silkMax);
        float step = Mathf.Max(1f, sMax * 0.1f);
        if (ShadeInput.WasActionPressed(ShadeAction.DebugSoulIncrease))
        {
            if (shadeSoulOverride)
            {
                foreach (var sc in LegacyHelper.ShadeController.ActiveInstances)
                {
                    // Through the real gain, so the key fills the vessels once the meter is
                    // full rather than stopping at it - a debug key that does not exercise
                    // what is being debugged is worse than no key at all.
                    if (sc != null) sc.AddSoul(11);
                }
                shadeSoul = Mathf.Min(shadeSoul + 11f, Mathf.Max(1f, shadeSoulMax));
                if (ModConfig.Instance.logHud)
                {
                    Debug.Log("[SimpleHUD] Debug: Shade Soul +11");
                }
            }
            else
            {
                float baseVal = debugUseCustomSilk ? debugSilk : playerData.silk;
                debugUseCustomSilk = true;
                debugSilk = Mathf.Min(baseVal + step, sMax);
                if (ModConfig.Instance.logHud)
                {
                    Debug.Log("[SimpleHUD] Debug: Hornet Silk +step");
                }
            }
        }
        if (ShadeInput.WasActionPressed(ShadeAction.DebugSoulDecrease))
        {
            if (shadeSoulOverride)
            {
                foreach (var sc in LegacyHelper.ShadeController.ActiveInstances)
                {
                    if (sc != null) sc.DebugSpendSoul(11);
                }
                shadeSoul = Mathf.Max(shadeSoul - 11f, 0f);
                if (ModConfig.Instance.logHud)
                {
                    Debug.Log("[SimpleHUD] Debug: Shade Soul -11");
                }
            }
            else
            {
                float baseVal = debugUseCustomSilk ? debugSilk : playerData.silk;
                debugUseCustomSilk = true;
                debugSilk = Mathf.Max(baseVal - step, 0f);
                if (ModConfig.Instance.logHud)
                {
                    Debug.Log("[SimpleHUD] Debug: Hornet Silk -step");
                }
            }
        }
        if (ShadeInput.WasActionPressed(ShadeAction.DebugSoulReset))
        {
            if (shadeSoulOverride)
            {
                foreach (var sc in LegacyHelper.ShadeController.ActiveInstances)
                {
                    if (sc != null) sc.shadeSoul = 0;
                }
                shadeSoul = 0f;
                if (ModConfig.Instance.logHud)
                {
                    Debug.Log("[SimpleHUD] Debug: Shade Soul reset");
                }
            }
            else
            {
                debugUseCustomSilk = false;
                debugSilk = playerData.silk;
                if (ModConfig.Instance.logHud)
                {
                    Debug.Log("[SimpleHUD] Debug: Hornet Silk reset");
                }
            }
        }
    }

    private void UpdatePauseFade()
    {
        if (canvasGroup == null)
            return;

        bool menuActive = ShouldTreatAsMenu();

        // Fully hidden, not dimmed, whenever any menu surface is up (inventory, crests, charms, map,
        // pause) - the earlier dim-to-0.35 left the masks and soul orb sitting over the top of the
        // pane art. Same treatment while Hornet's controls are locked, which is the third consumer of
        // that flag alongside the Shade's movement state machine and its combat gate: in a
        // conversation, at a bench or in a cutscene the game takes its own HUD away and the Shade's
        // has no business staying up. That is not just the reasoning either - the game's own HUD
        // being gone is one of the things HornetControlsLocked reads to recognise those moments.
        bool controlsLocked = ShouldHideForLockedControls();

        // And whenever the game has put its own HUD away, whether or not it also took Hornet's
        // controls. The memory/dream sequences are the case that forced this: they hide the game's
        // HUD for atmosphere while leaving Hornet fully playable, so the control-lock test above
        // reads false and the Shade's masks and soul orb were the only UI left on screen.
        bool gameHudHidden = ShouldHideForHiddenGameHud();
        float target = (menuActive || controlsLocked || gameHudHidden) ? 0f : 1f;
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

    private bool ShouldHideForLockedControls()
    {
        try
        {
            return LegacyHelper.ShadeController.HornetControlsLocked();
        }
        catch
        {
            return false;
        }
    }

    private bool ShouldHideForHiddenGameHud()
    {
        try
        {
            return LegacyHelper.ShadeController.GameHudHidden();
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
    public void SetShadeSoul(int current, int max, int vesselSoul = 0, int vesselCount = 0)
    {
        shadeSoulOverride = true;
        shadeSoul = Mathf.Max(0, current);
        shadeSoulMax = Mathf.Max(1, max);
        SetShadeVessels(vesselSoul, vesselCount);
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
            pendingMaskRefresh = true;
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

    public void SetVisible(bool visible)
    {
        if (canvas != null) canvas.enabled = visible; else gameObject.SetActive(visible);
    }

    public void SetPlayerData(PlayerData pd)
    {
        if (pd == playerData) return;
        playerData = pd;
        int oldMax = shadeMax;
        SubscribeToCharmInventory();
        charmInventoryDirty = true;
        ComputeShadeFromPlayer();
        if (shadeMax != oldMax)
        {
            RebuildMasks();
            previousShadeTotalHealth = Mathf.Min(previousShadeTotalHealth, shadeMax + shadeLifebloodMax);
        }
        RefreshHealth();
        RefreshSoul();
    }

    private void OnDestroy()
    {
        UnsubscribeFromCharmInventory();
    }
}

#nullable restore
