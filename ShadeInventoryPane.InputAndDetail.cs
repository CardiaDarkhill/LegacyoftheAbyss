using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TeamCherry.NestedFadeGroup;
using HarmonyLib;
using InControl;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TeamCherry.Localization;
using LegacyoftheAbyss.Shade;

internal sealed partial class ShadeInventoryPane : InventoryPane
{
    internal void ProcessShadeInputTick()
    {
        AdvanceOverlayAnimations();

        if (!CanProcessShadeInput())
        {
            ResetShadeInputState("CannotProcessShadeInput");
            return;
        }

        if (lastShadeInputFrame == Time.frameCount)
        {
            return;
        }

        lastShadeInputFrame = Time.frameCount;
        ProcessShadeDirectionalInput();
        ProcessShadeSubmitInput();
    }

    private bool CanProcessShadeInput()
    {
        if (!isActive)
        {
            return false;
        }

        if (!HasBoundInputs)
        {
            return false;
        }

        if (CheatManager.IsOpen)
        {
            return false;
        }

        if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
        {
            if (!loggedInactiveHierarchyProcessing)
            {
                LogMenuEvent(FormattableString.Invariant(
                    $"CanProcessShadeInput proceeding despite inactive hierarchy: enabled={isActiveAndEnabled} inHierarchy={gameObject.activeInHierarchy}"));
                loggedInactiveHierarchyProcessing = true;
            }
        }
        else if (loggedInactiveHierarchyProcessing)
        {
            loggedInactiveHierarchyProcessing = false;
        }

        return true;
    }

    private void ProcessShadeDirectionalInput()
    {
        var pressed = TryGetShadeDirectionalPress();
        if (pressed.HasValue)
        {
            shadeHeldDirection = pressed;
            shadeDirectionRepeatTimer = ShadeInputInitialRepeatDelay;
            HandleDirectionalInput(pressed.Value, fromInputComponent: false);
            return;
        }

        if (!shadeHeldDirection.HasValue)
        {
            return;
        }

        var direction = shadeHeldDirection.Value;
        if (!IsShadeDirectionHeld(direction))
        {
            ResetShadeInputState("DirectionReleased");
            return;
        }

        shadeDirectionRepeatTimer -= Time.unscaledDeltaTime;
        if (shadeDirectionRepeatTimer > 0f)
        {
            return;
        }

        shadeDirectionRepeatTimer = ShadeInputRepeatInterval;
        HandleDirectionalInput(direction, fromInputComponent: false);
    }

    /// <summary>
    /// Reads the Shade's own bindings only. Hornet's directional input reaches the pane through
    /// <c>InventoryPaneInput.PressDirection</c> - once as the <c>OnInput*</c> event this pane
    /// subscribes to, and once as the Harmony postfix that catches the case where those handlers
    /// are not registered, with <see cref="HandleDirectionalInput"/> deduplicating the pair.
    /// Polling <c>HeroActions</c> here as well made every press move the selection twice and ran a
    /// second repeat timer alongside the game's own.
    /// </summary>
    private InventoryPaneBase.InputEventType? TryGetShadeDirectionalPress()
    {
        if (ShadeInput.WasActionPressed(ShadeAction.MoveLeft))
        {
            return InventoryPaneBase.InputEventType.Left;
        }

        if (ShadeInput.WasActionPressed(ShadeAction.MoveRight))
        {
            return InventoryPaneBase.InputEventType.Right;
        }

        if (ShadeInput.WasActionPressed(ShadeAction.MoveUp))
        {
            return InventoryPaneBase.InputEventType.Up;
        }

        if (ShadeInput.WasActionPressed(ShadeAction.MoveDown))
        {
            return InventoryPaneBase.InputEventType.Down;
        }

        return null;
    }

    private bool IsShadeDirectionHeld(InventoryPaneBase.InputEventType direction)
    {
        return direction switch
        {
            InventoryPaneBase.InputEventType.Left => ShadeInput.IsActionHeld(ShadeAction.MoveLeft),
            InventoryPaneBase.InputEventType.Right => ShadeInput.IsActionHeld(ShadeAction.MoveRight),
            InventoryPaneBase.InputEventType.Up => ShadeInput.IsActionHeld(ShadeAction.MoveUp),
            InventoryPaneBase.InputEventType.Down => ShadeInput.IsActionHeld(ShadeAction.MoveDown),
            _ => false
        };
    }

    /// <summary>
    /// The Shade's own slash binding only. Hornet's confirm arrives through the
    /// <c>InventoryPaneInput.PressSubmit</c> patch; also polling <c>Attack</c>/<c>Jump</c> here
    /// meant one controller press called <see cref="HandleSubmit"/> twice - and since that toggles,
    /// a charm was equipped and immediately unequipped, playing the animation but changing nothing.
    /// </summary>
    private void ProcessShadeSubmitInput()
    {
        if (ShadeInput.WasActionPressed(ShadeAction.Nail))
        {
            HandleSubmit();
        }
    }

    private void ResetShadeInputState(string? reason = null)
    {
        shadeHeldDirection = null;
        shadeDirectionRepeatTimer = 0f;
        lastShadeInputFrame = -1;
    }

    private void Update()
    {
        AdvanceOverlayAnimations();
    }

    private void AdvanceOverlayAnimations()
    {
        int frame = Time.frameCount;
        if (lastOverlayAnimationFrame == frame)
        {
            return;
        }

        lastOverlayAnimationFrame = frame;

        bool hasAnimations = overlayAnimations.Count > 0 || activeShakeAnimation != null;
        if (!hasAnimations)
        {
            overlayAnimationTimeInitialized = false;
            return;
        }

        float currentTime = Time.realtimeSinceStartup;
        float deltaTime;

        if (!overlayAnimationTimeInitialized)
        {
            overlayAnimationTimeInitialized = true;
            lastOverlayAnimationTime = currentTime;
            deltaTime = Time.unscaledDeltaTime;
            if (deltaTime <= 0f)
            {
                deltaTime = 0.016f;
            }
        }
        else
        {
            deltaTime = Mathf.Max(0f, currentTime - lastOverlayAnimationTime);
            lastOverlayAnimationTime = currentTime;
            if (deltaTime <= 0f)
            {
                float fallback = Time.unscaledDeltaTime;
                deltaTime = fallback > 0f ? fallback : 0.016f;
            }
        }

        if (overlayAnimations.Count > 0)
        {
            for (int i = overlayAnimations.Count - 1; i >= 0; i--)
            {
                var animation = overlayAnimations[i];
                if (animation == null)
                {
                    overlayAnimations.RemoveAt(i);
                    continue;
                }

                if (animation.Update(deltaTime))
                {
                    overlayAnimations.RemoveAt(i);
                    overcharmAnimations.Remove(animation);
                }
            }
        }

        if (activeShakeAnimation != null && activeShakeAnimation.Update(deltaTime, this))
        {
            activeShakeAnimation = null;
        }
    }

    private void LateUpdate()
    {
        if (!isActive)
        {
            return;
        }

        string currentTitle = GetTextValue(titleText, titleTextTMP);
        if (!string.Equals(currentTitle, displayLabel, StringComparison.Ordinal))
        {
            SetTextValue(titleText, titleTextTMP, displayLabel);
        }

        labelPulseTimer -= Time.unscaledDeltaTime;
        if (labelPulseTimer <= 0f)
        {
            labelPulseTimer = 0.5f;
            UpdateParentListLabel();
        }
    }

    private static bool IsBindingLabelMeaningful(string? label)
    {
        return !string.IsNullOrWhiteSpace(label) &&
               !string.Equals(label, "Unbound", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Every way the charm under the cursor can be equipped right now, as one label - "J / (A)".
    /// <para>
    /// This used to name a single key: the Shade's slash binding, first option only, whatever it
    /// happened to be. That is wrong here for a reason particular to this pane - both players can
    /// work it. The Shade equips with its own slash binding, and Hornet equips through the game's
    /// Submit, which arrives via the <c>InventoryPaneInput.PressSubmit</c> patch. Naming one of them
    /// tells the other player nothing.
    /// </para>
    /// <para>
    /// Nor can this follow the last-used device the way a single-player prompt would, for the same
    /// reason: with two people on the pane, the device that moved the cursor last is not necessarily
    /// the device about to press equip. So every option that is actually available is listed, and a
    /// controller binding is only listed while a controller is attached.
    /// </para>
    /// </summary>
    private static string DescribeShadeEquipBindings()
    {
        var labels = new List<string>(3);

        void Add(string? label)
        {
            if (!IsBindingLabelMeaningful(label))
            {
                return;
            }

            foreach (var existing in labels)
            {
                if (string.Equals(existing, label, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            labels.Add(label!);
        }

        bool controllerAttached = IsAnyControllerAttached();

        try
        {
            foreach (bool secondary in new[] { false, true })
            {
                var option = ShadeInput.GetBindingOption(ShadeAction.Nail, secondary);
                if (option.type == ShadeBindingOptionType.Controller && !controllerAttached)
                {
                    continue;
                }

                Add(ShadeInput.DescribeBindingOption(option));
            }
        }
        catch
        {
        }

        // Hornet's own confirm. Only worth naming on a pad: on the keyboard the Shade's binding
        // above is already a key, and two keys in the prompt is noise rather than information.
        if (controllerAttached)
        {
            Add(DescribeHornetSubmitButton());
        }

        return labels.Count > 0 ? string.Join(" / ", labels) : string.Empty;
    }

    private static bool IsAnyControllerAttached()
    {
        try
        {
            var devices = InputManager.Devices;
            if (devices == null)
            {
                return false;
            }

            foreach (var device in devices)
            {
                if (device != null && device != InputDevice.Null && device.IsAttached)
                {
                    return true;
                }
            }
        }
        catch
        {
        }

        return false;
    }

    /// <summary>
    /// The face-button label for Hornet's confirm, as the attached pad names it - "(A)" on an Xbox
    /// controller, "(Cross)" on a PlayStation one. Taken from the device's own control handle rather
    /// than from the InControl enum name, which would render as "Action 1".
    /// </summary>
    private static string DescribeHornetSubmitButton()
    {
        try
        {
            var device = InputManager.ActiveDevice;
            if (device == null || device == InputDevice.Null || !device.IsAttached)
            {
                foreach (var candidate in InputManager.Devices)
                {
                    if (candidate != null && candidate != InputDevice.Null && candidate.IsAttached)
                    {
                        device = candidate;
                        break;
                    }
                }
            }

            if (device == null || device == InputDevice.Null)
            {
                return string.Empty;
            }

            var control = device.GetControl(InputControlType.Action1);
            string? handle = control?.Handle;
            if (string.IsNullOrWhiteSpace(handle))
            {
                return string.Empty;
            }

            return "(" + handle!.Trim() + ")";
        }
        catch
        {
        }

        return string.Empty;
    }

    private static string BuildEquipPrompt(string bindingLabel)
    {
        return string.IsNullOrEmpty(bindingLabel)
            ? "Equip"
            : FormattableString.Invariant($"Equip {bindingLabel}");
    }

    private static string BuildUnequipPrompt(string bindingLabel)
    {
        if (string.IsNullOrEmpty(bindingLabel))
        {
            return "Submit to unequip.";
        }

        return FormattableString.Invariant($"Press {bindingLabel} to unequip.");
    }

    private void UpdateDetailPreview(ShadeCharmDefinition? definition, bool owned, bool equipped, bool broken)
    {
        if (detailPreviewImage == null)
        {
            return;
        }

        if (definition == null && !owned)
        {
            detailPreviewImage.sprite = null;
            detailPreviewImage.enabled = false;
            detailPreviewImage.gameObject.SetActive(false);
            return;
        }

        Sprite? sprite = null;
        if (!owned)
        {
            sprite = ResolveLockedCharmSprite() ?? definition?.Icon ?? GetFallbackSprite();
        }
        else if (broken)
        {
            sprite = definition?.BrokenIcon ?? definition?.Icon ?? GetFallbackSprite();
        }
        else
        {
            sprite = definition?.Icon ?? GetFallbackSprite();
        }

        if (sprite != null)
        {
            detailPreviewImage.sprite = sprite;
            detailPreviewImage.enabled = true;
            detailPreviewImage.preserveAspect = true;
            detailPreviewImage.gameObject.SetActive(true);

            Color color;
            if (!owned)
            {
                color = InactiveIconColor;
            }
            else if (broken)
            {
                color = BrokenIconColor;
            }
            else if (equipped)
            {
                color = EquippedIconColor;
            }
            else
            {
                color = Color.white;
            }

            detailPreviewImage.color = color;
            UpdateDetailPreviewSize();
            // Same reasoning as the grid: an undiscovered charm shows a notch sprite standing in for
            // charm art, and blown up to preview size it is the largest thing on the panel.
            detailPreviewImage.rectTransform.localScale = owned ? Vector3.one : LockedIconScale;
        }
        else
        {
            detailPreviewImage.sprite = null;
            detailPreviewImage.enabled = false;
            detailPreviewImage.gameObject.SetActive(false);
        }
    }

    private void UpdateDetailPanel()
    {
        EnsureBuilt();
        if (entries.Count == 0 || inventory == null)
        {
            SetTextValue(detailTitleText, detailTitleTextTMP, displayLabel);
            SetTextValue(descriptionText, descriptionTextTMP, "Collect shade charms to unlock new abilities for your companion.");
            SetTextValue(statusText, statusTextTMP, string.Empty);
            ApplyStatusTextAlignment(null, null);
            SetHintMessage(string.Empty);
            if (detailCostRow != null)
            {
                detailCostRow.gameObject.SetActive(false);
            }
            RenderNotchStrip(detailCostIcons, 0, 0, false);
            UpdateDetailPreview(null, false, false, false);
            return;
        }

        var entry = entries[Mathf.Clamp(selectedIndex, 0, entries.Count - 1)];
        var definition = entry.Definition;

        EnsureStatusTextAlignmentCaptured();

        bool owned = inventory.IsOwned(entry.Id);
        bool equipped = owned && inventory.IsEquipped(entry.Id);
        bool broken = owned && inventory.IsBroken(entry.Id);

        if (!owned)
        {
            SetTextValue(detailTitleText, detailTitleTextTMP, string.Empty);
            SetTextValue(descriptionText, descriptionTextTMP, string.Empty);
            UpdateDetailPreview(null, false, false, false);
            if (detailCostRow != null)
            {
                detailCostRow.gameObject.SetActive(false);
            }
            RenderNotchStrip(detailCostIcons, 0, 0, false);
            ApplyStatusTextAlignment(TextAnchor.MiddleCenter, TextAlignmentOptions.Midline);
            SetTextValue(statusText, statusTextTMP, "This charm has not been discovered.");
            SetHintMessage(string.Empty);
            return;
        }

        ApplyStatusTextAlignment(null, null);
        SetTextValue(detailTitleText, detailTitleTextTMP, definition?.DisplayName ?? displayLabel);
        SetTextValue(descriptionText, descriptionTextTMP, definition?.Description ?? string.Empty);
        UpdateDetailPreview(definition, owned, equipped, broken);

        int notchCost = definition?.NotchCost ?? 0;
        string bindingLabel = DescribeShadeEquipBindings();
        string equipPrompt = BuildEquipPrompt(bindingLabel);
        string unequipPrompt = BuildUnequipPrompt(bindingLabel);
        string status;
        bool overcharmed = inventory.IsOvercharmed;
        bool wouldOvercharm = inventory.UsedNotches + notchCost > inventory.NotchCapacity;
        bool debugCharmMode = ShadeRuntime.IsDebugCharmModeActive();
        bool isVoidHeart = entry.Id == ShadeCharmId.VoidHeart;
        if (broken)
        {
            status = "Charm is broken. Rest at a bench to repair it.";
        }
        else if (equipped)
        {
            // No "Equipped." prefix. The charm is already sitting in the Equipped row above and lit
            // in the notch strip beside it; the only thing this line has to add is how to take it
            // off again.
            status = unequipPrompt;
            if (overcharmed)
            {
                status += " Shade is overcharmed.";
            }
        }
        else if (wouldOvercharm && overcharmed)
        {
            status = "Shade is overcharmed. Unequip a charm first.";
        }
        else if (isVoidHeart && !debugCharmMode)
        {
            status = "Void Heart is bound to the Shade.";
        }
        else
        {
            status = equipPrompt;
        }

        int displayCost = Mathf.Clamp(notchCost, 0, MaxNotchIcons);
        if (detailCostRow != null)
        {
            detailCostRow.gameObject.SetActive(displayCost > 0);
        }

        if (displayCost > 0)
        {
            RenderNotchStrip(detailCostIcons, displayCost, displayCost, false);
        }
        else
        {
            RenderNotchStrip(detailCostIcons, 0, 0, false);
            if (!isVoidHeart)
            {
                status = $"No notch cost.\n{status}";
            }
        }

        if (!ShadeRuntime.IsHornetRestingAtBench())
        {
            status = ShadeRuntime.BenchLockedMessage;
        }

        SetTextValue(statusText, statusTextTMP, status);
        SetHintMessage(string.Empty);
    }
    private static Sprite? ResolveLockedCharmSprite()
    {
        if (lockedCharmSpriteSearched)
        {
            return lockedCharmSprite;
        }

        lockedCharmSprite = ShadeCharmIconLoader.TryLoadIcon(LockedCharmSpriteName, LockedCharmSpriteName + ".png");
        if (lockedCharmSprite == null)
        {
            try
            {
                lockedCharmSprite = Resources
                    .FindObjectsOfTypeAll<Sprite>()
                    .FirstOrDefault(sprite => sprite != null &&
                        string.Equals(sprite.name, LockedCharmSpriteName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                lockedCharmSprite = null;
            }
        }

        lockedCharmSpriteSearched = true;
        return lockedCharmSprite;
    }

    private static Sprite? ResolveOvercharmBackdropSprite()
    {
        if (overcharmBackdropSpriteSearched)
        {
            return overcharmBackdropSprite;
        }

        overcharmBackdropSprite = ShadeCharmIconLoader.TryLoadIcon("overcharm_backboard", "overcharm_backboard.png");
        overcharmBackdropSpriteSearched = true;
        return overcharmBackdropSprite;
    }

    private Sprite GetFallbackSprite()
    {
        if (fallbackSprite != null)
        {
            return fallbackSprite;
        }

        var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
        {
            name = "ShadeCharmFallbackTex",
            hideFlags = HideFlags.HideAndDontSave
        };
        tex.SetPixel(0, 0, new Color(0.45f, 0.48f, 0.55f, 1f));
        tex.Apply();

        fallbackSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        fallbackSprite.name = "ShadeCharmFallbackSprite";
        fallbackSprite.hideFlags = HideFlags.HideAndDontSave;
        return fallbackSprite!;
    }
}
