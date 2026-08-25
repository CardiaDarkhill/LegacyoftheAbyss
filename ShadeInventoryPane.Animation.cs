using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TeamCherry.NestedFadeGroup;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TeamCherry.Localization;
using LegacyoftheAbyss.Shade;

internal sealed partial class ShadeInventoryPane : InventoryPane
{
    private void StartOvercharmAttemptAnimation(CharmEntry entry, ShadeCharmDefinition definition, int attemptIndex, int attemptThreshold)
    {
        var root = EnsureOverlayCanvas();
        if (root == null || entry.Icon == null)
        {
            return;
        }

        if (isActive)
        {
            ApplyOverlayVisibility(true);
        }

        var sourceRect = entry.Icon.rectTransform;
        if (sourceRect == null)
        {
            return;
        }

        StopActiveOvercharmAttempt();

        if (!TryGetOverlayPosition(sourceRect, out var start))
        {
            return;
        }

        Vector2 end;
        if (!TryGetOvercharmAttemptTarget(out end))
        {
            end = start + new Vector2(220f, 0f);
        }

        Sprite? sprite = definition?.Icon ?? entry.Icon.sprite ?? GetFallbackSprite();
        if (sprite == null)
        {
            return;
        }

        Color tint = entry.Icon.color;

        var flight = new GameObject($"CharmOvercharm_{entry.Id}", typeof(RectTransform));
        flight.layer = root.gameObject.layer;
        var rect = flight.GetComponent<RectTransform>();
        rect.SetParent(root, false);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Vector2 size = sourceRect.rect.size;
        if (size.sqrMagnitude <= 0.01f)
        {
            size = new Vector2(96f, 96f);
        }
        rect.sizeDelta = size;
        rect.anchoredPosition = start;
        rect.localScale = Vector3.one;

        var image = flight.AddComponent<Image>();
        image.raycastTarget = false;
        image.preserveAspect = true;
        image.sprite = sprite;
        image.color = tint;

        activeCharmFlights.Add(flight);
        activeOvercharmFlight = flight;

        var sourceIcon = entry.Icon;
        if (sourceIcon != null && animatingSourceIcons.Add(sourceIcon))
        {
            sourceIcon.enabled = false;
        }

        bool cleanupInvoked = false;
        void Cleanup()
        {
            if (cleanupInvoked)
            {
                return;
            }

            cleanupInvoked = true;

            if (sourceIcon != null)
            {
                animatingSourceIcons.Remove(sourceIcon);
                sourceIcon.enabled = sourceIcon.sprite != null;
            }

            if (ReferenceEquals(activeOvercharmFlight, flight))
            {
                activeOvercharmFlight = null;
            }

            if (flight != null)
            {
                activeCharmFlights.Remove(flight);
                Destroy(flight);
            }
        }

        void StartReturnFlight()
        {
            OverlayAnimation? inbound = null;
            inbound = RegisterOverlayAnimation(
                rect,
                end,
                start,
                0.28f,
                0.85f,
                1.05f,
                OverlayAnimationEase.EaseIn,
                () =>
                {
                    overcharmAnimations.Remove(inbound!);
                    Cleanup();
                },
                () =>
                {
                    overcharmAnimations.Remove(inbound!);
                    Cleanup();
                });

            if (inbound != null)
            {
                overcharmAnimations.Add(inbound);
            }
        }

        OverlayAnimation? outbound = null;
        outbound = RegisterOverlayAnimation(
            rect,
            start,
            end,
            0.35f,
            1f,
            0.85f,
            OverlayAnimationEase.EaseOut,
            () =>
            {
                overcharmAnimations.Remove(outbound!);

                float normalized = attemptThreshold > 0 ? Mathf.Clamp01((float)attemptIndex / attemptThreshold) : 1f;
                float amplitude = ComputeShakeAmplitude(attemptIndex, attemptThreshold);
                float duration = Mathf.Lerp(0.25f, 0.5f, normalized);

                if (amplitude > 0f && duration > 0f)
                {
                    StartShakeAnimation(amplitude, duration, StartReturnFlight);
                }
                else
                {
                    StartReturnFlight();
                }
            },
            () =>
            {
                overcharmAnimations.Remove(outbound!);
                Cleanup();
            });

        if (outbound != null)
        {
            overcharmAnimations.Add(outbound);
        }
    }

    private void CaptureShakeTargets()
    {
        shakeBasePositions.Clear();
        foreach (var rect in EnumerateCharmIconRects())
        {
            if (rect != null && !shakeBasePositions.ContainsKey(rect))
            {
                shakeBasePositions[rect] = rect.anchoredPosition;
            }
        }
    }

    private IEnumerable<RectTransform> EnumerateCharmIconRects()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            var icon = entries[i].Icon;
            if (icon != null && icon.rectTransform != null)
            {
                yield return icon.rectTransform;
            }
        }

        foreach (var icon in equippedIcons)
        {
            if (icon != null && icon.rectTransform != null)
            {
                yield return icon.rectTransform;
            }
        }
    }

    private static float ComputeShakeAmplitude(int attemptIndex, int attemptThreshold)
    {
        if (attemptIndex <= 0)
        {
            return 0f;
        }

        float normalized = attemptThreshold > 0 ? Mathf.Clamp01((float)attemptIndex / attemptThreshold) : 1f;
        return Mathf.Lerp(10f, 28f, normalized);
    }

    private bool TryGetOvercharmAttemptTarget(out Vector2 overlayPoint)
    {
        overlayPoint = Vector2.zero;
        if (equippedIconsRoot == null)
        {
            return false;
        }

        RefreshEquippedLayoutImmediate();

        int slotIndex = DetermineNextEquippedSlotIndex();
        if (slotIndex >= 0 && slotIndex < equippedIcons.Count)
        {
            var targetIcon = equippedIcons[slotIndex];
            var rect = targetIcon != null ? targetIcon.rectTransform : null;
            if (rect != null)
            {
                bool activatedPlaceholder = false;
                bool previousEnabled = targetIcon != null && targetIcon.enabled;

                try
                {
                    if (!rect.gameObject.activeSelf)
                    {
                        rect.gameObject.SetActive(true);
                        activatedPlaceholder = true;
                        if (targetIcon != null)
                        {
                            previousEnabled = targetIcon.enabled;
                            targetIcon.enabled = false;
                        }

                        RefreshEquippedLayoutImmediate();
                    }

                    if (TryGetOverlayPosition(rect, out overlayPoint))
                    {
                        return true;
                    }
                }
                finally
                {
                    if (activatedPlaceholder)
                    {
                        if (targetIcon != null)
                        {
                            targetIcon.enabled = previousEnabled;
                        }

                        rect.gameObject.SetActive(false);
                        RefreshEquippedLayoutImmediate();
                    }
                }
            }
        }

        Vector2 center;
        Vector2 size;
        Vector2 min;
        Vector2 max;
        if (TryCalculateEquippedIconBounds(out center, out size, out min, out max))
        {
            if (TryConvertEquippedLocalToOverlay(center, out overlayPoint))
            {
                return true;
            }
        }

        var rectRoot = equippedIconsRoot.rect;
        Vector2 fallback = rectRoot.center;
        return TryConvertEquippedLocalToOverlay(fallback, out overlayPoint);
    }

    private bool TryConvertEquippedLocalToOverlay(Vector2 localPoint, out Vector2 overlayPoint)
    {
        overlayPoint = Vector2.zero;
        if (equippedIconsRoot == null)
        {
            return false;
        }

        var root = overlayRoot ?? EnsureOverlayCanvas();
        if (root == null)
        {
            return false;
        }

        Camera? camera = overlayCanvas != null && overlayCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? overlayCanvas.worldCamera
            : null;

        Vector3 world = equippedIconsRoot.TransformPoint(localPoint);
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, world);
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screenPoint, camera, out overlayPoint);
    }

    private void StartCharmFlightAnimation(CharmEntry entry, ShadeCharmDefinition definition, Image destinationIcon, bool overcharmed)
    {
        var root = EnsureOverlayCanvas();
        if (root == null)
        {
            return;
        }

        if (isActive)
        {
            ApplyOverlayVisibility(true);
        }

        if (entry.Icon == null || destinationIcon == null)
        {
            return;
        }

        var sourceRect = entry.Icon.rectTransform;
        var destRect = destinationIcon.rectTransform;
        if (sourceRect == null || destRect == null)
        {
            return;
        }

        if (!TryGetOverlayPosition(sourceRect, out var start))
        {
            return;
        }

        RefreshEquippedLayoutImmediate();

        if (!TryGetOverlayPosition(destRect, out var end))
        {
            return;
        }

        Sprite? sprite = definition?.Icon ?? entry.Icon.sprite ?? destinationIcon.sprite ?? GetFallbackSprite();
        if (sprite == null)
        {
            return;
        }

        BeginEquippedIconAnimation(destinationIcon);

        var flight = new GameObject($"CharmFlight_{entry.Id}", typeof(RectTransform));
        flight.layer = root.gameObject.layer;
        var rect = flight.GetComponent<RectTransform>();
        rect.SetParent(root, false);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        Vector2 size = destinationIcon.rectTransform != null ? destinationIcon.rectTransform.rect.size : Vector2.zero;
        if (size.sqrMagnitude <= 0.01f)
        {
            size = sourceRect.rect.size.sqrMagnitude > 0.01f ? sourceRect.rect.size : new Vector2(96f, 96f);
        }
        rect.sizeDelta = size;
        rect.anchoredPosition = start;
        rect.localScale = Vector3.one;

        var image = flight.AddComponent<Image>();
        image.raycastTarget = false;
        image.preserveAspect = true;
        image.sprite = sprite;
        image.color = overcharmed ? OvercharmedEquippedIconColor : Color.white;

        activeCharmFlights.Add(flight);

        bool cleanupInvoked = false;
        void CleanupFlight()
        {
            if (cleanupInvoked)
            {
                return;
            }

            cleanupInvoked = true;
            CompleteEquippedIconAnimation(destinationIcon);

            if (flight != null)
            {
                activeCharmFlights.Remove(flight);
                Destroy(flight);
            }
        }

        RegisterOverlayAnimation(
            rect,
            start,
            end,
            0.35f,
            1f,
            0.8f,
            OverlayAnimationEase.EaseOut,
            () =>
            {
                CleanupFlight();
            },
            () =>
            {
                CleanupFlight();
            });
    }

    private OverlayAnimation RegisterOverlayAnimation(
        RectTransform rect,
        Vector2 start,
        Vector2 end,
        float duration,
        float startScale,
        float endScale,
        OverlayAnimationEase ease,
        Action? onCompleted,
        Action? onCancelled)
    {
        var animation = new OverlayAnimation
        {
            Rect = rect,
            Start = start,
            End = end,
            StartScale = startScale,
            EndScale = endScale,
            Duration = Mathf.Max(0f, duration),
            Ease = ease,
            OnCompleted = onCompleted,
            OnCancelled = onCancelled,
            Elapsed = 0f
        };

        overlayAnimations.Add(animation);
        overlayAnimationTimeInitialized = false;
        return animation;
    }

    private void CancelOverlayAnimation(OverlayAnimation animation)
    {
        if (animation == null)
        {
            return;
        }

        animation.Cancel();
        overlayAnimations.Remove(animation);
        overcharmAnimations.Remove(animation);
    }

    private void CancelOvercharmAnimations()
    {
        if (overcharmAnimations.Count == 0)
        {
            return;
        }

        foreach (var animation in overcharmAnimations.ToArray())
        {
            if (animation != null)
            {
                CancelOverlayAnimation(animation);
            }
        }

        overcharmAnimations.Clear();
    }

    private void CancelActiveShake()
    {
        if (activeShakeAnimation == null)
        {
            return;
        }

        activeShakeAnimation.Cancel(this);
        activeShakeAnimation = null;
    }

    private void StartShakeAnimation(float amplitude, float duration, Action? onCompleted)
    {
        CancelActiveShake();

        if (amplitude <= 0f || duration <= 0f)
        {
            onCompleted?.Invoke();
            return;
        }

        CaptureShakeTargets();

        activeShakeAnimation = new ActiveShakeAnimation
        {
            Amplitude = amplitude,
            Duration = Mathf.Max(0f, duration),
            Elapsed = 0f,
            OnCompleted = () =>
            {
                activeShakeAnimation = null;
                onCompleted?.Invoke();
            },
            OnCancelled = () =>
            {
                activeShakeAnimation = null;
            }
        };

        overlayAnimationTimeInitialized = false;
    }

    private void StopActiveOvercharmAttempt()
    {
        CancelOvercharmAnimations();
        CancelActiveShake();

        if (activeOvercharmFlight != null)
        {
            activeCharmFlights.Remove(activeOvercharmFlight);
            Destroy(activeOvercharmFlight);
            activeOvercharmFlight = null;
        }

        foreach (var icon in animatingSourceIcons.ToArray())
        {
            if (icon != null)
            {
                icon.enabled = icon.sprite != null;
            }
        }

        animatingSourceIcons.Clear();
        lastOverlayAnimationFrame = -1;
    }

    private void ClearActiveCharmFlights()
    {
        ResetAnimatingEquippedIcons();
        CancelOvercharmAnimations();
        CancelActiveShake();

        for (int i = overlayAnimations.Count - 1; i >= 0; i--)
        {
            var animation = overlayAnimations[i];
            if (animation != null)
            {
                animation.Cancel();
            }
        }
        overlayAnimations.Clear();
        overcharmAnimations.Clear();

        for (int i = activeCharmFlights.Count - 1; i >= 0; i--)
        {
            var flight = activeCharmFlights[i];
            if (flight != null)
            {
                Destroy(flight);
            }
        }
        activeCharmFlights.Clear();

        foreach (var icon in animatingSourceIcons.ToArray())
        {
            if (icon != null)
            {
                icon.enabled = icon.sprite != null;
            }
        }

        animatingSourceIcons.Clear();
        activeOvercharmFlight = null;
        overlayAnimationTimeInitialized = false;
        lastOverlayAnimationFrame = -1;
        RestoreShakeTargets();
    }

    private void BeginEquippedIconAnimation(Image destinationIcon)
    {
        if (destinationIcon == null)
        {
            return;
        }

        if (animatingEquippedIcons.Add(destinationIcon))
        {
            destinationIcon.enabled = false;
        }
    }

    private void CompleteEquippedIconAnimation(Image destinationIcon)
    {
        if (destinationIcon == null)
        {
            return;
        }

        if (animatingEquippedIcons.Remove(destinationIcon))
        {
            destinationIcon.enabled = true;
        }
    }

    private void ResetAnimatingEquippedIcons()
    {
        if (animatingEquippedIcons.Count == 0)
        {
            return;
        }

        foreach (var icon in animatingEquippedIcons.ToArray())
        {
            if (icon != null)
            {
                icon.enabled = true;
            }
        }

        animatingEquippedIcons.Clear();
    }

    private void RestoreShakeTargets()
    {
        if (shakeBasePositions.Count == 0)
        {
            return;
        }

        foreach (var kvp in shakeBasePositions)
        {
            if (kvp.Key != null)
            {
                kvp.Key.anchoredPosition = kvp.Value;
            }
        }

        shakeBasePositions.Clear();
    }

    private bool TryGetOverlayRelativePoint(RectTransform root, RectTransform rect, out Vector2 overlayPoint)
    {
        overlayPoint = Vector2.zero;
        if (root == null || rect == null)
        {
            return false;
        }

        Vector3 center = RectTransformUtility.CalculateRelativeRectTransformBounds(root, rect).center;
        if (float.IsNaN(center.x) || float.IsNaN(center.y) ||
            float.IsInfinity(center.x) || float.IsInfinity(center.y))
        {
            return false;
        }

        overlayPoint = new Vector2(center.x, center.y);
        return true;
    }

    /// <summary>
    /// Projects a world point into overlay-local space through the camera the point is drawn by.
    /// Returns false rather than approximating - the overlay is its own screen-space canvas, so a
    /// point that will not project has no position on it, and the callers all have a better
    /// strategy left to try.
    /// </summary>
    private bool TryProjectWorldPointToOverlay(
        RectTransform root,
        Vector3 worldPoint,
        Camera? sourceCamera,
        Camera? overlayCamera,
        out Vector2 overlayPoint)
    {
        overlayPoint = Vector2.zero;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(sourceCamera, worldPoint);
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screenPoint, overlayCamera, out var localPoint))
        {
            return false;
        }

        if (float.IsNaN(localPoint.x) || float.IsNaN(localPoint.y) ||
            float.IsInfinity(localPoint.x) || float.IsInfinity(localPoint.y))
        {
            return false;
        }

        overlayPoint = localPoint;
        return true;
    }

    private bool TryGetOverlayPosition(RectTransform rect, out Vector2 overlayPoint)
    {
        overlayPoint = Vector2.zero;
        if (rect == null)
        {
            return false;
        }

        var root = overlayRoot ?? EnsureOverlayCanvas();
        if (root == null)
        {
            return false;
        }

        RectTransform? relativeRoot = null;
        if (IsUnityObjectAlive(panelRoot) && rect.transform.IsChildOf(panelRoot))
        {
            relativeRoot = panelRoot;
        }

        Camera? rectCamera = ResolveCanvasCamera(rect.GetComponentInParent<Canvas>());
        Camera? overlayCamera = ResolveCanvasCamera(overlayCanvas);

        if (relativeRoot != null && TryGetOverlayRelativePoint(relativeRoot, rect, out overlayPoint))
        {
            if (relativeRoot == root)
            {
                return true;
            }

            Vector3 worldPoint = relativeRoot.TransformPoint(new Vector3(overlayPoint.x, overlayPoint.y, 0f));

            Vector3 overlayLocal = root.InverseTransformPoint(worldPoint);
            if (!float.IsNaN(overlayLocal.x) && !float.IsNaN(overlayLocal.y) &&
                !float.IsInfinity(overlayLocal.x) && !float.IsInfinity(overlayLocal.y))
            {
                overlayPoint = new Vector2(overlayLocal.x, overlayLocal.y);
                return true;
            }

            if (TryProjectWorldPointToOverlay(root, worldPoint, rectCamera, overlayCamera, out var converted))
            {
                overlayPoint = converted;
                return true;
            }
        }

        if (rect.transform.IsChildOf(root) && TryGetOverlayRelativePoint(root, rect, out overlayPoint))
        {
            return true;
        }

        rect.GetWorldCorners(overlayWorldCorners);
        Vector3 worldCenter = (overlayWorldCorners[0] + overlayWorldCorners[2]) * 0.5f;

        if (TryProjectWorldPointToOverlay(root, worldCenter, rectCamera, overlayCamera, out var projected))
        {
            overlayPoint = projected;
            return true;
        }

        overlayPoint = Vector2.zero;
        return false;
    }

    private static float EaseInCubic(float t)
    {
        float clamped = Mathf.Clamp01(t);
        return clamped * clamped * clamped;
    }

    private static float EaseOutCubic(float t)
    {
        float clamped = Mathf.Clamp01(t);
        float inv = 1f - clamped;
        return 1f - inv * inv * inv;
    }

    private void ApplyNotchLabelColor(bool overcharmed)
    {
        if (!notchLabelDefaultsCaptured)
        {
            if (notchText != null)
            {
                notchLabelDefaultColor = notchText.color;
            }

            if (notchTextTMP != null)
            {
                notchLabelDefaultTmpColor = notchTextTMP.color;
            }

            notchLabelDefaultsCaptured = true;
        }

        if (notchText != null)
        {
            notchText.color = overcharmed ? OvercharmedTextColor : notchLabelDefaultColor;
        }

        if (notchTextTMP != null)
        {
            notchTextTMP.color = overcharmed ? OvercharmedTextColor : notchLabelDefaultTmpColor;
        }
    }

    private void RenderNotchStrip(List<Image> icons, int litCount, int totalCount, bool showEmpty)
    {
        EnsureNotchSprites();

        Sprite lit = notchLitSprite ?? ResolveLockedCharmSprite() ?? GetFallbackSprite();
        Sprite empty = notchUnlitSprite ?? ResolveLockedCharmSprite() ?? lit;

        for (int i = 0; i < icons.Count; i++)
        {
            var image = icons[i];
            if (image == null)
            {
                continue;
            }

            var rect = image.rectTransform;
            if (rect != null)
            {
                rect.localScale = Vector3.one;
            }

            if (i < totalCount)
            {
                bool filled = i < litCount;
                image.gameObject.SetActive(true);
                image.sprite = filled ? lit : (showEmpty ? empty : null);
                image.enabled = image.sprite != null;
                image.color = filled ? Color.white : (showEmpty ? new Color(1f, 1f, 1f, 0.45f) : Color.clear);

                if (!showEmpty && !filled)
                {
                    image.gameObject.SetActive(false);
                }
            }
            else
            {
                image.sprite = null;
                image.enabled = false;
                image.gameObject.SetActive(false);
            }
        }
    }

    private void RenderNotchMeter(
        List<Image> icons,
        IReadOnlyList<NotchAssignment> assignments,
        int capacity,
        int highlightCost,
        ShadeCharmDefinition? selectedDefinition,
        ShadeCharmId? selectedId,
        bool highlightEquippedSlots,
        bool overcharmed)
    {
        EnsureNotchSprites();

        Sprite lit = notchLitSprite ?? ResolveLockedCharmSprite() ?? GetFallbackSprite();
        Sprite empty = notchUnlitSprite ?? ResolveLockedCharmSprite() ?? lit;

        Color filledColor = overcharmed ? OvercharmedNotchFillColor : Color.white;
        Color highlightedFilledColor = overcharmed ? OvercharmedNotchHighlightColor : Color.white;
        Color emptyColor = overcharmed ? OvercharmedNotchEmptyColor : new Color(1f, 1f, 1f, 0.55f);
        Color highlightEmptyColor = overcharmed ? OvercharmedNotchHighlightColor : Color.white;

        int usedCount = Mathf.Clamp(assignments?.Count ?? 0, 0, capacity);
        int highlightStart = usedCount;
        int highlightLength = Mathf.Clamp(highlightCost, 0, capacity);
        int highlightEnd = Mathf.Clamp(highlightStart + highlightLength, 0, capacity);

        for (int i = 0; i < icons.Count; i++)
        {
            var image = icons[i];
            if (image == null)
            {
                continue;
            }

            var rect = image.rectTransform;
            if (rect != null)
            {
                rect.localScale = Vector3.one;
            }

            if (i >= capacity)
            {
                image.sprite = null;
                image.enabled = false;
                image.gameObject.SetActive(false);
                continue;
            }

            if (assignments != null && i < usedCount)
            {
                var assignment = assignments[i];
                Sprite? sprite = assignment.Icon;
                if (sprite != null)
                {
                    image.sprite = sprite;
                    image.enabled = true;
                    image.color = filledColor;
                    image.gameObject.SetActive(true);

                    bool highlight = false;
                    if (highlightEquippedSlots && highlightCost > 0)
                    {
                        if (assignment.Definition != null && selectedDefinition != null)
                        {
                            if (ReferenceEquals(assignment.Definition, selectedDefinition))
                            {
                                highlight = true;
                            }
                            else if (assignment.Definition.EnumId.HasValue && selectedDefinition.EnumId.HasValue &&
                                     assignment.Definition.EnumId.Value == selectedDefinition.EnumId.Value)
                            {
                                highlight = true;
                            }
                        }

                        if (!highlight && assignment.CharmId.HasValue && selectedId.HasValue &&
                            assignment.CharmId.Value.Equals(selectedId.Value))
                        {
                            highlight = true;
                        }
                    }

                    if (highlight && rect != null)
                    {
                        rect.localScale = new Vector3(1.1f, 1.1f, 1f);
                        image.color = highlightedFilledColor;
                    }
                }
                else
                {
                    image.sprite = null;
                    image.enabled = false;
                    image.gameObject.SetActive(false);
                }

                continue;
            }

            bool highlightEmpty = !highlightEquippedSlots && highlightCost > 0 && i >= highlightStart && i < highlightEnd;
            Sprite slotSprite = highlightEmpty ? lit : empty;
            if (slotSprite != null)
            {
                image.sprite = slotSprite;
                image.enabled = true;
                image.color = highlightEmpty ? highlightEmptyColor : emptyColor;
                image.gameObject.SetActive(true);
            }
            else
            {
                image.sprite = null;
                image.enabled = false;
                image.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateEquippedRow()
    {
        EnsureBuilt();
        if (equippedIcons.Count == 0)
        {
            UpdateEquippedOvercharmBackdrop(false, 0);
            return;
        }

        EnsureEquippedDisplayCapacity();
        CaptureEquippedIconState();

        var inv = inventory ?? ShadeRuntime.Charms;
        if (inv == null)
        {
            foreach (var image in equippedIcons)
            {
                if (image == null)
                {
                    continue;
                }

                image.sprite = null;
                image.enabled = false;
                image.gameObject.SetActive(false);
            }

            for (int i = 0; i < equippedDisplayIds.Count; i++)
            {
                equippedDisplayIds[i] = null;
            }

            previousEquippedOrder.Clear();
            hasRenderedEquippedRow = false;
            UpdateEquippedOvercharmBackdrop(false, 0);
            return;
        }

        var equippedDefs = inv.GetEquippedDefinitions()?.Where(def => def != null).ToList() ?? new List<ShadeCharmDefinition>();
        int count = Mathf.Clamp(equippedDefs.Count, 0, equippedIcons.Count);
        var orderedPairs = new List<(ShadeCharmId Id, ShadeCharmDefinition Definition)>();
        bool overcharmed = inv.IsOvercharmed;
        Color equippedTint = overcharmed ? OvercharmedEquippedIconColor : Color.white;

        for (int i = 0; i < equippedIcons.Count; i++)
        {
            var image = equippedIcons[i];
            if (image == null)
            {
                continue;
            }

            if (i < count)
            {
                var def = equippedDefs[i];
                if (def?.EnumId.HasValue == true)
                {
                    orderedPairs.Add((def.EnumId.Value, def));
                }

                Sprite sprite = def?.Icon ?? GetFallbackSprite();
                if (sprite != null)
                {
                    bool animating = animatingEquippedIcons.Contains(image);
                    image.sprite = sprite;
                    image.color = equippedTint;
                    image.gameObject.SetActive(true);
                    image.enabled = !animating;
                }
                else
                {
                    image.sprite = null;
                    image.enabled = false;
                    image.gameObject.SetActive(false);
                }
            }
            else
            {
                image.sprite = null;
                image.enabled = false;
                image.gameObject.SetActive(false);
            }
        }

        UpdateEquippedOvercharmBackdrop(overcharmed, count);

        if (hasRenderedEquippedRow)
        {
            AnimateEquippedChanges(previousEquippedOrder, orderedPairs, overcharmed);
        }

        hasRenderedEquippedRow = true;

        EnsureEquippedDisplayCapacity();
        for (int i = 0; i < equippedDisplayIds.Count; i++)
        {
            if (i < orderedPairs.Count)
            {
                equippedDisplayIds[i] = orderedPairs[i].Id;
            }
            else
            {
                equippedDisplayIds[i] = null;
            }
        }
    }

    private void MoveSelectionHorizontal(int direction)
    {
        EnsureBuilt();
        if (entries.Count == 0 || direction == 0)
        {
            UpdateEquippedRow();
            return;
        }

        if (entryGridPositions.Count != entries.Count)
        {
            LayoutCharmEntries();
        }

        if (selectedIndex < 0 || selectedIndex >= entries.Count)
        {
            SelectIndex(Mathf.Clamp(selectedIndex, 0, entries.Count - 1));
            return;
        }

        Vector2Int current = selectedIndex < entryGridPositions.Count
            ? entryGridPositions[selectedIndex]
            : new Vector2Int(0, 0);
        int targetColumn = current.y + direction;
        int row = current.x;
        int candidate = -1;

        for (int i = 0; i < entryGridPositions.Count; i++)
        {
            var pos = entryGridPositions[i];
            if (pos.x == row && pos.y == targetColumn)
            {
                candidate = i;
                break;
            }
        }

        if (candidate >= 0)
        {
            SelectIndex(candidate);
        }
    }

    private void MoveSelectionVertical(int direction)
    {
        EnsureBuilt();
        if (entries.Count == 0 || direction == 0)
        {
            return;
        }

        if (entryGridPositions.Count != entries.Count)
        {
            LayoutCharmEntries();
        }

        if (selectedIndex < 0 || selectedIndex >= entries.Count)
        {
            SelectIndex(Mathf.Clamp(selectedIndex, 0, entries.Count - 1));
            return;
        }

        Vector2Int current = selectedIndex < entryGridPositions.Count
            ? entryGridPositions[selectedIndex]
            : new Vector2Int(0, 0);
        int targetRow = current.x + direction;
        if (targetRow < 0 || targetRow >= CharmRows)
        {
            return;
        }

        float currentCenterX = entryCenterXs.Count > selectedIndex ? entryCenterXs[selectedIndex] : 0f;
        int candidate = -1;
        float candidateDistance = float.MaxValue;

        for (int i = 0; i < entryGridPositions.Count; i++)
        {
            var pos = entryGridPositions[i];
            if (pos.x != targetRow)
            {
                continue;
            }

            float center = entryCenterXs.Count > i ? entryCenterXs[i] : 0f;
            float distance = Mathf.Abs(center - currentCenterX);
            if (distance < candidateDistance - 0.01f ||
                (Mathf.Abs(distance - candidateDistance) <= 0.01f && (candidate < 0 || i < candidate)))
            {
                candidateDistance = distance;
                candidate = i;
            }
        }

        if (candidate >= 0)
        {
            SelectIndex(candidate);
        }
    }

    internal void HandleDirectionalInput(InventoryPaneBase.InputEventType direction, bool fromInputComponent = true)
    {
        EnsureBuilt();
        bool skipDuplicate = fromInputComponent && inputHandlersRegistered && lastPaneInputCameFromEvent &&
            lastPaneInputFrame == Time.frameCount && lastPaneInputDirection == direction;
        if (skipDuplicate)
        {
            lastPaneInputCameFromEvent = false;
            return;
        }

        if (fromInputComponent)
        {
            lastPaneInputFrame = Time.frameCount;
            lastPaneInputDirection = direction;
            lastPaneInputCameFromEvent = false;
        }

        switch (direction)
        {
            case InventoryPaneBase.InputEventType.Left:
                MoveSelectionHorizontal(-1);
                break;
            case InventoryPaneBase.InputEventType.Right:
                MoveSelectionHorizontal(1);
                break;
            case InventoryPaneBase.InputEventType.Up:
                MoveSelectionVertical(-1);
                break;
            case InventoryPaneBase.InputEventType.Down:
                MoveSelectionVertical(1);
                break;
        }
    }

    private void SelectIndex(int index)
    {
        EnsureBuilt();
        if (entries.Count == 0)
        {
            return;
        }

        int previousIndex = Mathf.Clamp(selectedIndex, 0, entries.Count - 1);
        selectedIndex = Mathf.Clamp(index, 0, entries.Count - 1);
        var entry = entries[selectedIndex];
        string entryName = entry.Root != null && entry.Root.gameObject != null
            ? entry.Root.gameObject.name
            : "<null>";
        var highlightRect = EnsureHighlightRect();
        RectTransform? entryRoot = entry.Root;
        if (highlightRect != null && entryRoot != null)
        {
            highlightRect.gameObject.SetActive(true);
            PositionHighlight(highlightRect, entryRoot);
        }
        else if (highlightRect != null)
        {
            highlightRect.gameObject.SetActive(false);
        }

        var inv = inventory;
        if (isActive && inv != null)
        {
            inv.MarkCharmSeen(entry.Id);
        }

        UpdateDetailPanel();
        RefreshEntryStates();
        UpdateNotchMeter();
    }

    private void RefreshAll()
    {
        EnsureBuilt();
        ShadeCharmInventory? inv = ShadeRuntime.Charms;
        inventory = inv;
        var definitions = inv != null ? inv.AllCharms : Array.Empty<ShadeCharmDefinition>();
        LogMenuEvent($"RefreshAll: definitions={definitions.Count}, inventoryNull={inv == null}");
        EnsureEntryCount(definitions.Count);
        ResetEquippedDisplayState();
        for (int i = 0; i < definitions.Count; i++)
        {
            var entry = entries[i];
            entry.Definition = definitions[i];
            entry.Id = definitions[i].EnumId ?? ShadeCharmId.WaywardCompass;
            var sprite = definitions[i].Icon ?? GetFallbackSprite();
            entry.BaseSprite = sprite;
            entry.BrokenSprite = definitions[i].BrokenIcon ?? sprite;
            if (entry.Icon != null)
            {
                entry.Icon.sprite = sprite;
                entry.Icon.enabled = sprite != null;
            }
            if (entry.Background != null)
            {
                entry.Background.enabled = false;
                entry.Background.color = Color.clear;
            }
            entries[i] = entry;
        }

        if (gridRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridRoot);
        }

        LayoutCharmEntries();

        if (selectedIndex >= entries.Count)
        {
            selectedIndex = entries.Count > 0 ? entries.Count - 1 : 0;
        }

        UpdateNotchMeter();
        if (entries.Count > 0)
        {
            SelectIndex(Mathf.Clamp(selectedIndex, 0, entries.Count - 1));
        }
        else
        {
            RefreshEntryStates();
            UpdateDetailPanel();
        }
    }

    private void EnsureEntryCount(int count)
    {
        EnsureBuilt();
        int previous = entries.Count;
        while (entries.Count < count)
        {
            entries.Add(CreateEntry(entries.Count));
        }

        for (int i = entries.Count - 1; i >= count; i--)
        {
            if (entries[i].Root != null)
            {
                Destroy(entries[i].Root.gameObject);
            }
            entries.RemoveAt(i);
        }
        if (entries.Count != previous)
        {
            LogMenuEvent($"EnsureEntryCount -> entries={entries.Count}");
        }

        LayoutCharmEntries();
    }

    private CharmEntry CreateEntry(int index)
    {
        var cell = new GameObject($"CharmCell_{index}", typeof(RectTransform));
        cell.layer = gridRoot.gameObject.layer;
        var cellRect = cell.GetComponent<RectTransform>();
        cellRect.SetParent(gridRoot, false);
        cellRect.localScale = Vector3.one;
        cellRect.anchorMin = new Vector2(0f, 0f);
        cellRect.anchorMax = new Vector2(0f, 0f);
        cellRect.pivot = new Vector2(0.5f, 0.5f);
        cellRect.sizeDelta = charmCellSize;
        cellRect.anchoredPosition = Vector2.zero;

        var background = cell.AddComponent<Image>();
        background.enabled = false;
        background.color = Color.clear;
        background.raycastTarget = false;

        var iconGo = new GameObject("Icon", typeof(RectTransform));
        iconGo.layer = cell.layer;
        var iconRect = iconGo.GetComponent<RectTransform>();
        iconRect.SetParent(cellRect, false);
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        float iconDimension = currentCharmIconSize > 0f ? currentCharmIconSize : CalculateCharmIconSize();
        iconRect.sizeDelta = new Vector2(iconDimension, iconDimension);
        var icon = iconGo.AddComponent<Image>();
        icon.preserveAspect = true;
        icon.raycastTarget = false;
        icon.enabled = false;

        GameObject? newMarker = null;

        return new CharmEntry
        {
            Root = cellRect,
            Background = background,
            Icon = icon,
            NewMarker = newMarker,
            BaseSprite = null,
            BrokenSprite = null
        };
    }

    private void RefreshEntryStates()
    {
        EnsureBuilt();
        EnsureEquippedDisplayCapacity();
        var inv = inventory ?? ShadeRuntime.Charms;
        inventory = inv;

        if (entries.Count == 0)
        {
            return;
        }

        if (inv == null)
        {
            foreach (var entry in entries)
            {
                if (entry.Icon != null)
                {
                    entry.Icon.sprite = entry.BaseSprite;
                    entry.Icon.enabled = entry.Icon.sprite != null;
                    entry.Icon.color = InactiveIconColor;
                }

                if (entry.Background != null)
                {
                    entry.Background.enabled = false;
                    entry.Background.color = Color.clear;
                }

                if (entry.NewMarker != null)
                {
                    entry.NewMarker.SetActive(false);
                }
            }
            ResetEquippedDisplayState();
            UpdateEquippedRow();
            return;
        }

        var lockedSprite = ResolveLockedCharmSprite();
        foreach (var entry in entries)
        {
            bool owned = inv.IsOwned(entry.Id);
            bool equipped = inv.IsEquipped(entry.Id);
            bool broken = inv.IsBroken(entry.Id);
            bool isNew = owned && inv.IsNewlyDiscovered(entry.Id);

            if (entry.Icon != null)
            {
                // The locked placeholder is a notch sprite standing in for a charm, and at the grid's
                // full icon size it dwarfs the real art around it. Scaled rather than resized so the
                // cell itself, and the grid spacing that depends on it, are untouched.
                entry.Icon.rectTransform.localScale = owned ? Vector3.one : LockedIconScale;

                if (!owned && lockedSprite != null)
                {
                    entry.Icon.sprite = lockedSprite;
                    entry.Icon.enabled = !animatingSourceIcons.Contains(entry.Icon);
                    entry.Icon.color = LockedIconColor;
                }
                else
                {
                    entry.Icon.sprite = broken ? entry.BrokenSprite : entry.BaseSprite;
                    entry.Icon.enabled = entry.Icon.sprite != null && !animatingSourceIcons.Contains(entry.Icon);

                    if (!owned)
                    {
                        entry.Icon.color = InactiveIconColor;
                    }
                    else if (broken)
                    {
                        entry.Icon.color = BrokenIconColor;
                    }
                    else if (equipped)
                    {
                        entry.Icon.color = EquippedIconColor;
                    }
                    else
                    {
                        entry.Icon.color = Color.white;
                    }
                }
            }

            if (entry.Background != null)
            {
                entry.Background.enabled = false;
                entry.Background.color = Color.clear;
            }

            if (entry.NewMarker != null)
            {
                entry.NewMarker.SetActive(isNew);
            }
        }

        UpdateEquippedRow();
    }

    private void UpdateNotchMeter()
    {
        EnsureBuilt();
        var inv = inventory ?? ShadeRuntime.Charms;
        bool overcharmed = inv != null && inv.IsOvercharmed;
        SetTextValue(notchText, notchTextTMP, overcharmed ? "Notches - Overcharmed" : "Notches");
        ApplyNotchLabelColor(overcharmed);
        if (inv == null)
        {
            RenderNotchMeter(notchMeterIcons, Array.Empty<NotchAssignment>(), 0, 0, null, null, false, false);
            return;
        }

        int capacity = Mathf.Clamp(inv.NotchCapacity, 0, MaxNotchIcons);
        var assignments = new List<NotchAssignment>(capacity);
        var equippedDefs = inv.GetEquippedDefinitions()?.Where(def => def != null).ToList();
        if (equippedDefs != null)
        {
            foreach (var def in equippedDefs)
            {
                if (def == null)
                {
                    continue;
                }

                int cost = Mathf.Max(def.NotchCost, 0);
                if (cost <= 0)
                {
                    continue;
                }

                Sprite sprite = def.Icon ?? GetFallbackSprite();
                ShadeCharmId? charmId = def.EnumId;
                for (int i = 0; i < cost && assignments.Count < capacity; i++)
                {
                    assignments.Add(new NotchAssignment
                    {
                        Icon = sprite,
                        Definition = def,
                        CharmId = charmId
                    });
                }
            }
        }

        ShadeCharmDefinition? selectedDefinition = null;
        ShadeCharmId? selectedId = null;
        int highlightCost = 0;
        bool highlightEquippedSlots = false;
        if (selectedIndex >= 0 && selectedIndex < entries.Count)
        {
            var selectedEntry = entries[selectedIndex];
            selectedDefinition = selectedEntry.Definition;
            selectedId = selectedEntry.Id;
            // Only a charm you actually have can preview its cost. Hovering an undiscovered one used
            // to light up notches on the Shade's meter for a charm that cannot be equipped, which
            // reads as "this is what it would cost" for something you are not supposed to know yet.
            highlightCost = inv.IsOwned(selectedEntry.Id)
                ? Mathf.Max(selectedDefinition?.NotchCost ?? 0, 0)
                : 0;
            highlightEquippedSlots = inv.IsEquipped(selectedEntry.Id);
        }

        RenderNotchMeter(
            notchMeterIcons,
            assignments,
            capacity,
            highlightCost,
            selectedDefinition,
            selectedId,
            highlightEquippedSlots,
            overcharmed);
    }

    /// <summary>
    /// The camera a canvas is drawn through, or null for a screen-space-overlay canvas (which is
    /// what <see cref="RectTransformUtility"/> wants for one).
    /// </summary>
    private static Camera? ResolveCanvasCamera(Canvas? canvas)
    {
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }

        return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
    }

    /// <summary>
    /// Pane travel either side of centre, in the inventory's own units - the "Inventory Control"
    /// FSM's "Next Pane Start X". Read live in <see cref="RefreshPaneSlideUnits"/>; this is the
    /// value the shipped FSM uses.
    /// </summary>
    private const float DefaultPaneSlideUnits = 31f;

    private float paneSlideUnits = DefaultPaneSlideUnits;

    internal void RefreshPaneSlideUnits()
    {
        if (attachedPaneList == null)
        {
            return;
        }

        var fsm = PlayMakerFSM.FindFsmOnGameObject(attachedPaneList.gameObject, "Inventory Control");
        var startX = fsm != null ? fsm.FsmVariables.FindFsmFloat("Next Pane Start X") : null;
        if (startX != null && Mathf.Abs(startX.Value) > 0.001f)
        {
            paneSlideUnits = Mathf.Abs(startX.Value);
        }
    }

    /// <summary>
    /// How far the pane GameObject currently sits from its resting place, as an overlay offset.
    /// <para>
    /// The Shade's content is drawn on its own screen-space canvas rather than under the pane
    /// object, so the FSM tweens an empty transform. Reading that transform back and offsetting the
    /// overlay by the same proportion reproduces the game's slide exactly, without re-timing it.
    /// </para>
    /// <para>
    /// Converted by ratio, not by projecting through a camera. The inventory is not a screen-space
    /// canvas and does not expose one we can resolve, so <c>WorldToScreenPoint</c> with the camera
    /// we could find treated the pane's world position as if it were already in screen pixels and
    /// returned a 15px "slide" on a 4K display - a real animation nobody could see. The pane rests
    /// at local zero (the FSM's "Pane Final Pos") and travels <see cref="paneSlideUnits"/> either
    /// side of it, which is one screen width.
    /// </para>
    /// </summary>
    internal bool TryGetPaneSlideOffset(out Vector2 offset)
    {
        offset = Vector2.zero;

        var root = overlayRoot;
        if (root == null || paneSlideUnits <= 0.001f)
        {
            return false;
        }

        float fraction = transform.localPosition.x / paneSlideUnits;
        offset = new Vector2(fraction * root.rect.width, 0f);
        return true;
    }
}

/// <summary>
/// Drives the Shade pane overlay's fade and slide. It lives on the overlay canvas object - a scene
/// root - rather than on the pane, because the FSM deactivates the pane you just left while its
/// slide-out is still running, and a component on an inactive object stops receiving LateUpdate.
/// </summary>
internal sealed class ShadeInventoryPaneSlide : MonoBehaviour
{
    /// <summary>Slide length, from the "Tween Panes" state's iTweenMoveTo.</summary>
    private const float SlideSeconds = 0.35f;

    /// <summary>Fade length, from the FadeNestedFadeGroupV3 in the same state.</summary>
    private const float FadeSeconds = 0.2f;

    private ShadeInventoryPane? pane;
    private RectTransform? overlayRoot;
    private CanvasGroup? canvasGroup;

    private float alpha;
    private float fadeFrom;
    private float fadeTo;
    private float fadeRemaining;
    private float slideRemaining;

    internal bool IsTransitioning => fadeRemaining > 0f || slideRemaining > 0f;

    internal void Bind(ShadeInventoryPane owner, RectTransform root, CanvasGroup group)
    {
        pane = owner;
        overlayRoot = root;
        canvasGroup = group;
    }

    internal void SetVisible(bool visible, bool animate)
    {
        if (canvasGroup == null || overlayRoot == null)
        {
            return;
        }

        fadeFrom = alpha;
        fadeTo = visible ? 1f : 0f;
        fadeRemaining = animate ? FadeSeconds : 0f;
        slideRemaining = animate ? SlideSeconds : 0f;

        if (!animate)
        {
            alpha = fadeTo;
            overlayRoot.anchoredPosition = Vector2.zero;
        }

        Apply();
    }

    private void LateUpdate()
    {
        if (canvasGroup == null || overlayRoot == null)
        {
            return;
        }

        // Unscaled: the inventory runs with the game paused, and the FSM's own tweens are realtime.
        float delta = Time.unscaledDeltaTime;

        if (fadeRemaining > 0f)
        {
            fadeRemaining = Mathf.Max(0f, fadeRemaining - delta);
            alpha = Mathf.Lerp(fadeTo, fadeFrom, fadeRemaining / FadeSeconds);
        }
        else
        {
            alpha = fadeTo;
        }

        if (slideRemaining > 0f)
        {
            slideRemaining = Mathf.Max(0f, slideRemaining - delta);
            if (pane != null && pane.TryGetPaneSlideOffset(out var offset))
            {
                overlayRoot.anchoredPosition = offset;
            }
        }
        else
        {
            overlayRoot.anchoredPosition = Vector2.zero;
        }

        Apply();
    }

    private void Apply()
    {
        canvasGroup!.alpha = alpha;
        bool interactive = fadeTo > 0.5f;
        canvasGroup.interactable = interactive;
        canvasGroup.blocksRaycasts = interactive;
    }
}
