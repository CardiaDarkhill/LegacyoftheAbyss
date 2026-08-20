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
    internal void ConfigureFromTemplate(InventoryPane? template)
    {
        if (template == null)
        {
            return;
        }

        panelRectTemplate = null;
        contentRectTemplate = null;
        gridRectTemplate = null;
        detailRectTemplate = null;
        rootRectTemplate = null;
        gridLayoutTemplate = null;
        templateRootSize = null;
        rootLayoutTemplate = null;

        var arial = Resources.GetBuiltinResource<Font>("Arial.ttf");
        bodyFont = arial;
        headerFont = arial;
        var trajanFont = ResolveTrajanSourceFont();
        if (trajanFont != null)
        {
            bodyFont = trajanFont;
            headerFont = trajanFont;
        }
        bodyFontColor = Color.white;
        headerFontColor = Color.white;
        bodyTextStyle = null;
        headerTextStyle = null;
        bodyTmpTextStyle = null;
        headerTmpTextStyle = null;
        bool bodyFontAssigned = false;
        bool headerFontAssigned = false;
        bool bodyTmpAssigned = false;
        bool headerTmpAssigned = false;
        bool bodyTextAssigned = false;
        bool headerTextAssigned = false;

        try
        {
            var templateRect = ResolveTemplateRootRectTransform(template);
            if (templateRect != null)
            {
                Vector2 templateSize = templateRect.rect.size;
                if (!HasUsableTemplateRect(templateRect))
                {
                    float width = Mathf.Abs(templateSize.x);
                    float height = Mathf.Abs(templateSize.y);
                    float area = width * height;
                    LogMenuEvent(FormattableString.Invariant(
                        $"ConfigureFromTemplate: template root size {FormatVector2(templateSize)} unsuitable (minDimThreshold={MinTemplateCopyDimension}, minAreaThreshold={MinTemplateCopyArea}, area={area:0.##}); ignoring template layout"));
                }
            }

            var tmpTexts = template.GetComponentsInChildren<TMP_Text>(true);
            if (tmpTexts != null && tmpTexts.Length > 0)
            {
                var validTmp = tmpTexts
                    .Where(t => t != null && t.font != null)
                    .ToArray();

                if (validTmp.Length > 0)
                {
                    var orderedTmp = validTmp
                        .OrderBy(t => t.fontSize)
                        .ToArray();

                    TMP_Text? headerSampleTmp = null;
                    foreach (var candidate in validTmp)
                    {
                        string value = candidate.text ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(value) &&
                            string.Equals(value.Trim(), "Charms", StringComparison.OrdinalIgnoreCase))
                        {
                            headerSampleTmp = candidate;
                            break;
                        }
                    }

                    headerSampleTmp ??= orderedTmp.LastOrDefault();

                    TMP_Text? bodySampleTmp = orderedTmp.Length > 0
                        ? orderedTmp[Mathf.Clamp(orderedTmp.Length > 1 ? orderedTmp.Length / 2 : 0, 0, orderedTmp.Length - 1)]
                        : null;

                    if (bodySampleTmp == headerSampleTmp)
                    {
                        bodySampleTmp = orderedTmp.FirstOrDefault(t => t != headerSampleTmp) ?? bodySampleTmp;
                    }

                    if (bodySampleTmp != null)
                    {
                        bodyTmpTextStyle = CaptureTmpTextStyle(bodySampleTmp);
                        var converted = ConvertTmpToTextStyle(bodySampleTmp);
                        if (converted.HasValue)
                        {
                            bodyTextStyle = converted;
                            bodyTextAssigned = true;
                        }

                        if (bodySampleTmp.font != null)
                        {
                            var source = bodySampleTmp.font.sourceFontFile;
                            if (source != null)
                            {
                                bodyFont = source;
                                bodyFontAssigned = true;
                            }
                        }

                        if (bodySampleTmp.color.a > 0f)
                        {
                            bodyFontColor = bodySampleTmp.color;
                        }

                        bodyTmpAssigned = true;
                    }

                    if (headerSampleTmp != null)
                    {
                        headerTmpTextStyle = CaptureTmpTextStyle(headerSampleTmp);
                        var convertedHeader = ConvertTmpToTextStyle(headerSampleTmp);
                        if (convertedHeader.HasValue)
                        {
                            headerTextStyle = convertedHeader;
                            headerTextAssigned = true;
                        }

                        if (headerSampleTmp.font != null)
                        {
                            var source = headerSampleTmp.font.sourceFontFile;
                            if (source != null)
                            {
                                headerFont = source;
                                headerFontAssigned = true;
                            }
                        }

                        if (headerSampleTmp.color.a > 0f)
                        {
                            headerFontColor = headerSampleTmp.color;
                        }

                        headerTmpAssigned = true;
                    }
                }
            }

            var texts = template.GetComponentsInChildren<Text>(true);
            if (texts != null && texts.Length > 0)
            {
                var validTexts = texts
                    .Where(t => t != null && t.font != null)
                    .ToArray();

                if (validTexts.Length > 0)
                {
                    var ordered = validTexts
                        .OrderBy(t => t.fontSize)
                        .ToArray();

                    int bodyIndex = Mathf.Clamp(ordered.Length > 1 ? ordered.Length / 2 : 0, 0, ordered.Length - 1);
                    var bodySample = ordered[bodyIndex];
                    if (bodySample != null)
                    {
                        if (!bodyFontAssigned && bodySample.font != null)
                        {
                            bodyFont = bodySample.font;
                            bodyFontAssigned = true;
                        }

                        if (!bodyTextAssigned)
                        {
                            bodyTextStyle = CaptureTextStyle(bodySample);
                            bodyTextAssigned = true;
                        }

                        if (!bodyTmpAssigned && bodySample.color.a > 0f)
                        {
                            bodyFontColor = bodySample.color;
                        }
                    }

                    Text? headerSample = null;
                    foreach (var candidate in ordered)
                    {
                        string value = candidate.text ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(value) &&
                            string.Equals(value.Trim(), "Charms", StringComparison.OrdinalIgnoreCase))
                        {
                            headerSample = candidate;
                            break;
                        }
                    }

                    headerSample ??= ordered.LastOrDefault();

                    if (headerSample != null)
                    {
                        if (!headerFontAssigned && headerSample.font != null)
                        {
                            headerFont = headerSample.font;
                            headerFontAssigned = true;
                        }

                        if (!headerTextAssigned)
                        {
                            headerTextStyle = CaptureTextStyle(headerSample);
                            headerTextAssigned = true;
                        }

                        if (!headerTmpAssigned && headerSample.color.a > 0f)
                        {
                            headerFontColor = headerSample.color;
                        }
                    }
                }
            }

            var images = template.GetComponentsInChildren<Image>(true);
            if (images != null && images.Length > 0)
            {
                foreach (var img in images)
                {
                    if (img == null)
                    {
                        continue;
                    }

                    var sprite = img.sprite;
                    if (sprite == null)
                    {
                        continue;
                    }

                    string name = img.gameObject != null ? img.gameObject.name : string.Empty;
                    string lower = string.IsNullOrEmpty(name) ? string.Empty : name.ToLowerInvariant();

                    if (highlightSpriteTemplate == null && (lower.Contains("highlight") || lower.Contains("select")))
                    {
                        highlightSpriteTemplate = sprite;
                        if (img.color.a > 0f)
                        {
                            highlightColor = img.color;
                        }
                        continue;
                    }

                    if (cellFrameSprite == null && (lower.Contains("frame") || lower.Contains("slot") || lower.Contains("charm")))
                    {
                        cellFrameSprite = sprite;
                        if (img.color.a > 0f)
                        {
                            cellFrameColor = img.color;
                        }
                        continue;
                    }

                    if (panelBackgroundSprite == null && (lower.Contains("panel") || lower.Contains("background") || lower.Contains("back")))
                    {
                        panelBackgroundSprite = sprite;
                        if (img.color.a > 0f)
                        {
                            panelBackgroundColor = img.color;
                        }
                    }
                }
            }

        }
        catch
        {
        }

        EnsureTrajanFallbacks();

        if (isBuilt)
        {
            bool wasActive = isActive;
            RebuildUI();
            if (wasActive)
            {
                RefreshAll();
            }
            else
            {
                inventory ??= ShadeRuntime.Charms;
                int count = inventory != null ? inventory.AllCharms.Count : 0;
                EnsureEntryCount(count);
                RefreshEntryStates();
                UpdateNotchMeter();
                UpdateDetailPanel();
            }
        }
        else if (isActive)
        {
            RefreshAll();
        }
    }

    private void ResetOverlayReferences()
    {
        overlayCanvasObject = null;
        overlayRoot = null;
        overlayCanvas = null;
        overlayCanvasScaler = null;
        overlayRaycaster = null;
        canvasGroup = null!;
    }

    private void ResetBuiltUiState()
    {
        if (IsUnityObjectAlive(panelRoot))
        {
            try { Destroy(panelRoot.gameObject); }
            catch { }
        }

        panelRoot = null!;
        contentRoot = null!;
        gridRoot = null!;
        highlight = null;
        titleText = null;
        notchText = null;
        detailTitleText = null;
        descriptionText = null;
        statusText = null;
        hintText = null;
        detailCostLabel = null;
        detailCostLabelTMP = null;
        detailPreviewImage = null;
        detailPreviewRect = null;
        titleTextTMP = null;
        notchTextTMP = null;
        detailTitleTextTMP = null;
        descriptionTextTMP = null;
        statusTextTMP = null;
        statusTextAlignmentCaptured = false;
        statusTextDefaultAlignment = TextAnchor.UpperLeft;
        statusTextDefaultTmpAlignment = TextAlignmentOptions.TopLeft;
        hintTextTMP = null;
        leftContentRoot = null;
        notchIconContainer = null;
        detailCostRow = null;
        detailCostIconContainer = null;
        equippedIconsRoot = null;
        equippedOvercharmBackdrop = null;
        equippedIconsLayout = null;
        equippedOvercharmBackdrop = null;
        equippedIconsLayout = null;
        entries.Clear();
        entryGridPositions.Clear();
        entryCenterXs.Clear();
        notchMeterIcons.Clear();
        detailCostIcons.Clear();
        equippedIcons.Clear();
        detailPreviewTopOffset = 0f;
        detailDescriptionGap = 0f;
        detailDescriptionBottomPadding = 0f;
        detailHorizontalMargin = 0f;
        isBuilt = false;
    }

    private void EnsureBuilt()
    {
        if (isBuilt)
        {
            bool overlayValid = IsUnityObjectAlive(overlayRoot) && IsUnityObjectAlive(canvasGroup) &&
                (overlayCanvasObject == null || IsUnityObjectAlive(overlayCanvasObject));

            if (!overlayValid)
            {
                ResetOverlayReferences();
                ResetBuiltUiState();
            }
            else
            {
                bool hierarchyValid = IsUnityObjectAlive(panelRoot) &&
                    IsUnityObjectAlive(contentRoot) &&
                    IsUnityObjectAlive(gridRoot);

                if (hierarchyValid)
                {
                    try
                    {
                        if (panelRoot != null && overlayRoot != null && panelRoot.transform.parent != overlayRoot)
                        {
                            hierarchyValid = false;
                        }
                    }
                    catch
                    {
                        hierarchyValid = false;
                    }
                }

                if (!hierarchyValid)
                {
                    ResetBuiltUiState();
                }
            }
        }

        if (!isBuilt)
        {
            BuildUI();
        }
    }

    internal void ForceImmediateRefresh()
    {
        EnsureBuilt();
        RefreshAll();
        UpdateParentListLabel();
        labelPulseTimer = 0f;
        LogMenuEvent($"ForceImmediateRefresh: entries={entries.Count}, inventoryNull={inventory == null}");
    }

    private static Vector2 DetermineStandaloneFallbackSize(RectTransform root, out string source)
    {
        source = "default";
        if (root == null)
        {
            return DefaultStandaloneRootSize;
        }

        try
        {
            var canvas = root.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                var rect = canvas.pixelRect;
                if (rect.width >= MinRootSizeThreshold && rect.height >= MinRootSizeThreshold)
                {
                    source = "canvas.pixelRect";
                    return new Vector2(rect.width, rect.height);
                }
            }
        }
        catch
        {
        }

        try
        {
            float width = Screen.width;
            float height = Screen.height;
            if (width >= MinRootSizeThreshold && height >= MinRootSizeThreshold)
            {
                source = "screen";
                return new Vector2(width, height);
            }
        }
        catch
        {
        }

        try
        {
            var resolution = Screen.currentResolution;
            if (resolution.width >= MinRootSizeThreshold && resolution.height >= MinRootSizeThreshold)
            {
                source = "screen.currentResolution";
                return new Vector2(resolution.width, resolution.height);
            }
        }
        catch
        {
        }

        try
        {
            var display = Display.main;
            if (display != null && display.systemWidth >= MinRootSizeThreshold && display.systemHeight >= MinRootSizeThreshold)
            {
                source = "display";
                return new Vector2(display.systemWidth, display.systemHeight);
            }
        }
        catch
        {
        }

        source = "constant";
        return DefaultStandaloneRootSize;
    }

    private bool TryApplyStandaloneRootSizing(RectTransform root, Vector2? desiredSize = null)
    {
        if (root == null)
        {
            return false;
        }

        bool parentIsRect = root.parent is RectTransform;
        if (parentIsRect && HasSufficientRectSize(root))
        {
            return false;
        }

        Vector2? candidate = desiredSize ?? templateRootSize;
        string sizeSource = desiredSize.HasValue ? "override" : "template";
        if (!candidate.HasValue || Mathf.Abs(candidate.Value.x) < MinRootSizeThreshold || Mathf.Abs(candidate.Value.y) < MinRootSizeThreshold)
        {
            string fallbackSource;
            Vector2 fallback = DetermineStandaloneFallbackSize(root, out fallbackSource);
            if (Mathf.Abs(fallback.x) >= MinRootSizeThreshold && Mathf.Abs(fallback.y) >= MinRootSizeThreshold)
            {
                candidate = fallback;
                sizeSource = fallbackSource;
            }
        }

        if (!candidate.HasValue)
        {
            return false;
        }

        Vector2 size = new Vector2(Mathf.Abs(candidate.Value.x), Mathf.Abs(candidate.Value.y));
        if (size.x < MinRootSizeThreshold || size.y < MinRootSizeThreshold)
        {
            return false;
        }

        Vector2 anchorMin = rootRectTemplate?.AnchorMin ?? (parentIsRect ? root.anchorMin : new Vector2(0.5f, 0.5f));
        Vector2 anchorMax = rootRectTemplate?.AnchorMax ?? (parentIsRect ? root.anchorMax : new Vector2(0.5f, 0.5f));
        Vector2 pivot = rootRectTemplate?.Pivot ?? new Vector2(0.5f, 0.5f);
        Vector2 anchored = rootRectTemplate?.AnchoredPosition ?? Vector2.zero;
        Vector2 offsetMin = rootRectTemplate?.OffsetMin ?? (anchored - Vector2.Scale(size, pivot));
        Vector2 offsetMax = rootRectTemplate?.OffsetMax ?? (anchored + Vector2.Scale(size, Vector2.one - pivot));

        bool changed = false;

        if (!Approximately(root.anchorMin, anchorMin))
        {
            root.anchorMin = anchorMin;
            changed = true;
        }

        if (!Approximately(root.anchorMax, anchorMax))
        {
            root.anchorMax = anchorMax;
            changed = true;
        }

        if (!Approximately(root.pivot, pivot))
        {
            root.pivot = pivot;
            changed = true;
        }

        if (!Approximately(root.anchoredPosition, anchored))
        {
            root.anchoredPosition = anchored;
            changed = true;
        }

        if (!Approximately(root.offsetMin, offsetMin))
        {
            root.offsetMin = offsetMin;
            changed = true;
        }

        if (!Approximately(root.offsetMax, offsetMax))
        {
            root.offsetMax = offsetMax;
            changed = true;
        }

        if (!Approximately(root.sizeDelta, size))
        {
            root.sizeDelta = size;
            changed = true;
        }

        float beforeWidth = root.rect.width;
        root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        float afterWidth = root.rect.width;
        if (!Mathf.Approximately(beforeWidth, afterWidth))
        {
            changed = true;
        }

        float beforeHeight = root.rect.height;
        root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        float afterHeight = root.rect.height;
        if (!Mathf.Approximately(beforeHeight, afterHeight))
        {
            changed = true;
        }

        if (!changed)
        {
            return false;
        }

        templateRootSize = size;
        if (!rootRectTemplate.HasValue)
        {
            rootRectTemplate = RectSnapshot.From(root);
        }

        LogMenuEvent(FormattableString.Invariant(
            $"ApplyStandaloneRootSizing -> parent='{root.parent?.name ?? "<null>"}' size={FormatVector2(size)} anchorMin={FormatVector2(anchorMin)} anchorMax={FormatVector2(anchorMax)} pivot={FormatVector2(pivot)} anchored={FormatVector2(anchored)} source={sizeSource}"));
        return true;
    }

    private bool ApplyHardFallbackRootSizing(RectTransform root, Vector2 fallbackSize, string source)
    {
        if (root == null)
        {
            return false;
        }

        Vector2 size = new Vector2(Mathf.Abs(fallbackSize.x), Mathf.Abs(fallbackSize.y));
        if (size.x < MinRootSizeThreshold || size.y < MinRootSizeThreshold)
        {
            return false;
        }

        Vector2 anchorMin = rootRectTemplate?.AnchorMin ?? new Vector2(0.5f, 0.5f);
        Vector2 anchorMax = rootRectTemplate?.AnchorMax ?? anchorMin;
        Vector2 pivot = rootRectTemplate?.Pivot ?? new Vector2(0.5f, 0.5f);
        Vector2 anchored = rootRectTemplate?.AnchoredPosition ?? Vector2.zero;
        Vector2 offsetMin = rootRectTemplate?.OffsetMin ?? (anchored - Vector2.Scale(size, pivot));
        Vector2 offsetMax = rootRectTemplate?.OffsetMax ?? (anchored + Vector2.Scale(size, Vector2.one - pivot));

        bool changed = false;

        if (!Approximately(root.anchorMin, anchorMin))
        {
            root.anchorMin = anchorMin;
            changed = true;
        }

        if (!Approximately(root.anchorMax, anchorMax))
        {
            root.anchorMax = anchorMax;
            changed = true;
        }

        if (!Approximately(root.pivot, pivot))
        {
            root.pivot = pivot;
            changed = true;
        }

        if (!Approximately(root.anchoredPosition, anchored))
        {
            root.anchoredPosition = anchored;
            changed = true;
        }

        if (!Approximately(root.offsetMin, offsetMin))
        {
            root.offsetMin = offsetMin;
            changed = true;
        }

        if (!Approximately(root.offsetMax, offsetMax))
        {
            root.offsetMax = offsetMax;
            changed = true;
        }

        if (!Approximately(root.sizeDelta, size))
        {
            root.sizeDelta = size;
            changed = true;
        }

        float beforeWidth = root.rect.width;
        root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        float afterWidth = root.rect.width;
        if (!Mathf.Approximately(beforeWidth, afterWidth))
        {
            changed = true;
        }

        float beforeHeight = root.rect.height;
        root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        float afterHeight = root.rect.height;
        if (!Mathf.Approximately(beforeHeight, afterHeight))
        {
            changed = true;
        }

        var layoutElement = root.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = root.gameObject.AddComponent<LayoutElement>();
            changed = true;
        }

        if (layoutElement != null)
        {
            if (layoutElement.minWidth < size.x)
            {
                layoutElement.minWidth = size.x;
                changed = true;
            }

            if (layoutElement.preferredWidth < size.x)
            {
                layoutElement.preferredWidth = size.x;
                changed = true;
            }

            if (layoutElement.minHeight < size.y)
            {
                layoutElement.minHeight = size.y;
                changed = true;
            }

            if (layoutElement.preferredHeight < size.y)
            {
                layoutElement.preferredHeight = size.y;
                changed = true;
            }

            if (layoutElement.flexibleWidth < 0f)
            {
                layoutElement.flexibleWidth = 0f;
            }

            if (layoutElement.flexibleHeight < 0f)
            {
                layoutElement.flexibleHeight = 0f;
            }
        }

        templateRootSize = size;
        if (!rootRectTemplate.HasValue)
        {
            rootRectTemplate = RectSnapshot.From(root);
        }

        if (!rootLayoutTemplate.HasValue && layoutElement != null)
        {
            rootLayoutTemplate = LayoutElementSnapshot.From(layoutElement);
        }

        if (changed)
        {
            LogMenuEvent(FormattableString.Invariant(
                $"ApplyHardFallbackRootSizing -> parent='{root.parent?.name ?? "<null>"}' size={FormatVector2(size)} source={source}"));
        }

        return changed;
    }

    internal void EnsureRootSizing()
    {
        if (overlayRoot != null)
        {
            UpdateOverlayCanvasScaler();
            return;
        }

        var root = transform as RectTransform;
        if (root == null)
        {
            return;
        }

        TryApplyStandaloneRootSizing(root);

        if (HasSufficientRectSize(root))
        {
            return;
        }

        string fallbackSource;
        Vector2 fallback = DetermineStandaloneFallbackSize(root, out fallbackSource);
        if (Mathf.Abs(fallback.x) < MinRootSizeThreshold || Mathf.Abs(fallback.y) < MinRootSizeThreshold)
        {
            fallback = DefaultStandaloneRootSize;
            fallbackSource = "constant";
        }

        if (ApplyHardFallbackRootSizing(root, fallback, fallbackSource))
        {
            try
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(root);
            }
            catch
            {
            }
        }
    }

    private void ApplyTemplateRootLayoutFallback(RectTransform root)
    {
        if (root == null)
        {
            return;
        }

        Vector2? desiredSize = templateRootSize;
        bool adjustments = TryApplyStandaloneRootSizing(root, desiredSize);
        desiredSize = templateRootSize;
        if (desiredSize.HasValue)
        {
            var templateSize = desiredSize.Value;
            if (templateSize.x >= MinRootSizeThreshold)
            {
                float before = root.rect.width;
                root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, templateSize.x);
                if (!Mathf.Approximately(before, root.rect.width))
                {
                    adjustments = true;
                }
            }

            if (templateSize.y >= MinRootSizeThreshold)
            {
                float before = root.rect.height;
                root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, templateSize.y);
                if (!Mathf.Approximately(before, root.rect.height))
                {
                    adjustments = true;
                }
            }
        }

        var layoutElement = root.GetComponent<LayoutElement>();
        if (layoutElement != null)
        {
            bool layoutAdjusted = false;
            if (rootLayoutTemplate.HasValue)
            {
                rootLayoutTemplate.Value.Apply(layoutElement);
                layoutAdjusted = true;
            }

            if (desiredSize.HasValue)
            {
                var templateSize = desiredSize.Value;
                if (templateSize.x >= MinRootSizeThreshold)
                {
                    if (layoutElement.minWidth < templateSize.x)
                    {
                        layoutElement.minWidth = templateSize.x;
                        layoutAdjusted = true;
                    }
                    if (layoutElement.preferredWidth < templateSize.x)
                    {
                        layoutElement.preferredWidth = templateSize.x;
                        layoutAdjusted = true;
                    }
                }

                if (templateSize.y >= MinRootSizeThreshold)
                {
                    if (layoutElement.minHeight < templateSize.y)
                    {
                        layoutElement.minHeight = templateSize.y;
                        layoutAdjusted = true;
                    }
                    if (layoutElement.preferredHeight < templateSize.y)
                    {
                        layoutElement.preferredHeight = templateSize.y;
                        layoutAdjusted = true;
                    }
                }
            }

            if (layoutAdjusted)
            {
                layoutElement.flexibleWidth = Mathf.Max(0f, layoutElement.flexibleWidth);
                layoutElement.flexibleHeight = Mathf.Max(0f, layoutElement.flexibleHeight);
                adjustments = true;
            }
        }

        if (!adjustments)
        {
            return;
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);

        var parent = root.parent as RectTransform;
        int guard = 0;
        while (parent != null && guard < 3)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
            parent = parent.parent as RectTransform;
            guard++;
        }

        string templateSizeText = desiredSize.HasValue ? FormatVector2(desiredSize.Value) : "<null>";
        LogMenuEvent(FormattableString.Invariant(
            $"ForceLayoutRebuild applied template fallback -> root={FormatVector2(root.rect.size)} template={templateSizeText}"));
    }

    private static float ComputeNormalizedMargin(float dimension, float fraction)
    {
        if (dimension <= 0f)
        {
            return 0f;
        }

        float clampedFraction = Mathf.Clamp01(fraction);
        float margin = dimension * clampedFraction;
        float maxMargin = dimension * 0.45f;
        if (margin > maxMargin)
        {
            margin = maxMargin;
        }

        return Mathf.Max(0f, margin);
    }

    private static TMP_FontAsset? ResolveTrajanFontAsset()
    {
        if (cachedTrajanFont != null)
        {
            return cachedTrajanFont;
        }

        if (searchedTrajanFont)
        {
            return cachedTrajanFont;
        }

        searchedTrajanFont = true;

        try
        {
            cachedTrajanFont = Resources
                .FindObjectsOfTypeAll<TMP_FontAsset>()
                .FirstOrDefault(asset => asset != null &&
                    asset.name.IndexOf("Trajan", StringComparison.OrdinalIgnoreCase) >= 0);

            if (cachedTrajanFont == null)
            {
                if (TMP_Settings.instance != null)
                {
                    var defaultFontAsset = TMP_Settings.defaultFontAsset;
                    if (defaultFontAsset != null &&
                        defaultFontAsset.name.IndexOf("Trajan", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        cachedTrajanFont = defaultFontAsset;
                    }

                    if (cachedTrajanFont == null)
                    {
                        var fallbackAssets = TMP_Settings.fallbackFontAssets;
                        if (fallbackAssets != null)
                        {
                            cachedTrajanFont = fallbackAssets
                                .FirstOrDefault(asset => asset != null &&
                                    asset.name.IndexOf("Trajan", StringComparison.OrdinalIgnoreCase) >= 0);
                        }
                    }
                }
            }

            if (cachedTrajanFont == null)
            {
                string[] candidatePaths =
                {
                    "Fonts & Materials/TrajanPro-Regular SDF",
                    "Fonts & Materials/Trajan Pro-Regular SDF",
                    "Fonts & Materials/Trajan Pro SDF",
                    "Fonts & Materials/TrajanPro SDF",
                    "Fonts & Materials/Trajan SDF",
                    "TrajanPro-Regular SDF"
                };

                foreach (var path in candidatePaths)
                {
                    var loaded = Resources.Load<TMP_FontAsset>(path);
                    if (loaded != null)
                    {
                        cachedTrajanFont = loaded;
                        break;
                    }
                }
            }

            if (cachedTrajanFont == null)
            {
                foreach (var asset in Resources.LoadAll<TMP_FontAsset>("Fonts & Materials"))
                {
                    if (asset == null)
                    {
                        continue;
                    }

                    if (asset.name.IndexOf("Trajan", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        cachedTrajanFont = asset;
                        break;
                    }
                }
            }

            if (cachedTrajanFont != null && cachedTrajanFont.sourceFontFile != null)
            {
                cachedTrajanSourceFont = cachedTrajanFont.sourceFontFile;
            }
        }
        catch
        {
            cachedTrajanFont = null;
        }

        return cachedTrajanFont;
    }

    internal static Font? ResolveTrajanSourceFont()
    {
        if (cachedTrajanSourceFont != null)
        {
            return cachedTrajanSourceFont;
        }

        var asset = ResolveTrajanFontAsset();
        if (asset != null && asset.sourceFontFile != null)
        {
            cachedTrajanSourceFont = asset.sourceFontFile;
            return cachedTrajanSourceFont;
        }

        try
        {
            cachedTrajanSourceFont = Resources
                .FindObjectsOfTypeAll<Font>()
                .FirstOrDefault(font => font != null &&
                    font.name.IndexOf("Trajan", StringComparison.OrdinalIgnoreCase) >= 0);
        }
        catch
        {
            cachedTrajanSourceFont = null;
        }

        if (cachedTrajanSourceFont == null)
        {
            string[] candidatePaths =
            {
                "Fonts/TrajanPro-Regular",
                "Fonts/Trajan Pro-Regular",
                "Fonts/Trajan Pro",
                "Fonts/Trajan",
                "TrajanPro-Regular"
            };

            foreach (var path in candidatePaths)
            {
                try
                {
                    var loaded = Resources.Load<Font>(path);
                    if (loaded != null)
                    {
                        cachedTrajanSourceFont = loaded;
                        break;
                    }
                }
                catch
                {
                }
            }
        }

        if (cachedTrajanSourceFont == null)
        {
            try
            {
                foreach (var font in Resources.LoadAll<Font>("Fonts"))
                {
                    if (font == null)
                    {
                        continue;
                    }

                    if (font.name.IndexOf("Trajan", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        cachedTrajanSourceFont = font;
                        break;
                    }
                }
            }
            catch
            {
                cachedTrajanSourceFont = null;
            }
        }

        return cachedTrajanSourceFont;
    }

    private static bool FontMatchesTrajan(TMP_FontAsset? font)
    {
        return font != null && font.name.IndexOf("Trajan", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool FontMatchesTrajan(Font? font)
    {
        return font != null && font.name.IndexOf("Trajan", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void EnsureTrajanFallbacks()
    {
        var tmpFallback = ResolveTrajanFontAsset();
        if (tmpFallback != null)
        {
            if (bodyTmpTextStyle.HasValue)
            {
                var body = bodyTmpTextStyle.Value;
                if (!FontMatchesTrajan(body.Font))
                {
                    body.Font = tmpFallback;
                    bodyTmpTextStyle = body;
                }
            }

            if (headerTmpTextStyle.HasValue)
            {
                var header = headerTmpTextStyle.Value;
                if (!FontMatchesTrajan(header.Font))
                {
                    header.Font = tmpFallback;
                    headerTmpTextStyle = header;
                }
            }
        }

        var sourceFallback = ResolveTrajanSourceFont();
        if (sourceFallback != null)
        {
            if (!FontMatchesTrajan(bodyFont))
            {
                bodyFont = sourceFallback;
            }

            if (!FontMatchesTrajan(headerFont))
            {
                headerFont = sourceFallback;
            }
        }
    }

    private RectTransform? EnsureOverlayCanvas()
    {
        if (overlayRoot != null)
        {
            return overlayRoot;
        }

        GameObject? overlayObject = null;
        try
        {
            overlayObject = new GameObject("ShadeInventoryOverlay", typeof(RectTransform));
        }
        catch
        {
            overlayObject = null;
        }

        if (overlayObject == null)
        {
            return null;
        }

        overlayCanvasObject = overlayObject;
        overlayObject.layer = gameObject.layer;

        overlayRoot = overlayObject.GetComponent<RectTransform>();
        overlayRoot.SetParent(null, false);
        overlayRoot.anchorMin = Vector2.zero;
        overlayRoot.anchorMax = Vector2.one;
        overlayRoot.pivot = new Vector2(0.5f, 0.5f);
        overlayRoot.offsetMin = Vector2.zero;
        overlayRoot.offsetMax = Vector2.zero;
        overlayRoot.localScale = Vector3.one;
        overlayRoot.localPosition = Vector3.zero;

        overlayCanvas = overlayObject.AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayCanvas.sortingOrder = 1000;
        overlayCanvas.pixelPerfect = false;

        overlayCanvasScaler = overlayObject.AddComponent<CanvasScaler>();
        overlayCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        overlayCanvasScaler.referenceResolution = DefaultStandaloneRootSize;
        overlayCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        overlayCanvasScaler.matchWidthOrHeight = 0.5f;

        overlayRaycaster = overlayObject.AddComponent<GraphicRaycaster>();
        overlayRaycaster.ignoreReversedGraphics = false;
        overlayRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

        canvasGroup = overlayObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = overlayObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        UpdateOverlayCanvasScaler();

        bool shouldShow = isActive && gameObject.activeInHierarchy;
        ApplyOverlayVisibility(shouldShow);

        return overlayRoot;
    }

    private Vector2 DetermineOverlayCanvasSize(RectTransform root)
    {
        if (root == null)
        {
            return Vector2.zero;
        }

        if (overlayCanvas != null)
        {
            try
            {
                Rect pixelRect = overlayCanvas.pixelRect;
                if (pixelRect.width > MinRootSizeThreshold && pixelRect.height > MinRootSizeThreshold)
                {
                    return new Vector2(pixelRect.width, pixelRect.height);
                }
            }
            catch
            {
            }
        }

        string fallbackSource;
        Vector2 fallback = DetermineStandaloneFallbackSize(root, out fallbackSource);
        if (fallback.x >= MinRootSizeThreshold && fallback.y >= MinRootSizeThreshold)
        {
            return fallback;
        }

        return DefaultStandaloneRootSize;
    }

    private void UpdateOverlayCanvasScaler()
    {
        if (overlayCanvasScaler == null)
        {
            return;
        }

        if (overlayCanvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            overlayCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }

        if (overlayCanvasScaler.referenceResolution.sqrMagnitude <= 0f)
        {
            overlayCanvasScaler.referenceResolution = DefaultStandaloneRootSize;
        }

        overlayCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        overlayCanvasScaler.matchWidthOrHeight = 0.5f;
    }

    private void ApplyOverlayVisibility(bool visible)
    {
        if (overlayRoot == null || canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;

        if (overlayCanvas != null)
        {
            overlayCanvas.enabled = true;
        }
    }

    private void UpdateInventoryBinding(bool subscribe)
    {
        var current = ShadeRuntime.Charms;
        inventory = current;

        if (subscribe)
        {
            if (!ReferenceEquals(subscribedInventory, current))
            {
                if (subscribedInventory != null)
                {
                    subscribedInventory.StateChanged -= HandleInventoryChanged;
                }

                if (current != null)
                {
                    current.StateChanged += HandleInventoryChanged;
                }

                subscribedInventory = current;
            }
        }
        else if (subscribedInventory != null)
        {
            subscribedInventory.StateChanged -= HandleInventoryChanged;
            subscribedInventory = null;
        }
    }

    private float CalculateCharmIconSize()
    {
        float minDimension = Mathf.Min(Mathf.Abs(charmCellSize.x), Mathf.Abs(charmCellSize.y));
        if (minDimension < MinRootSizeThreshold)
        {
            minDimension = Mathf.Min(DefaultCharmCellSize.x, DefaultCharmCellSize.y);
        }

        return Mathf.Max(minDimension * CharmIconSizeMultiplier, 48f);
    }

    private float CalculateDetailPreviewSize()
    {
        float baseSize = currentCharmIconSize > 0f ? currentCharmIconSize : CalculateCharmIconSize();
        return Mathf.Max(baseSize * DetailPreviewScale, baseSize);
    }

    private void UpdateCharmIconSizeCache()
    {
        currentCharmIconSize = CalculateCharmIconSize();
        UpdateDetailPreviewSize();
    }

    private void UpdateDetailPreviewSize()
    {
        if (detailPreviewRect == null)
        {
            return;
        }

        float previewSize = CalculateDetailPreviewSize();
        detailPreviewRect.sizeDelta = new Vector2(previewSize, previewSize);
        detailPreviewRect.anchoredPosition = new Vector2(detailPreviewRect.anchoredPosition.x, -detailPreviewTopOffset);

        var descRect = ResolveRectTransform(descriptionText, descriptionTextTMP);
        if (descRect != null)
        {
            float descriptionTop = detailPreviewTopOffset + previewSize + detailDescriptionGap;
            descRect.offsetMin = new Vector2(detailHorizontalMargin, detailDescriptionBottomPadding);
            descRect.offsetMax = new Vector2(-detailHorizontalMargin, -descriptionTop);
        }
    }

    internal void AttachToPaneList(InventoryPaneList? paneList)
    {
        if (attachedPaneList == paneList)
        {
            return;
        }

        DetachPaneList();

        if (paneList == null)
        {
            return;
        }

        attachedPaneList = paneList;

        try { attachedPaneList.OpeningInventory += HandleInventoryOpened; }
        catch { }

        try { attachedPaneList.ClosingInventory += HandleInventoryClosed; }
        catch { }

        if (attachedPaneList != null)
        {
            ShadeInventoryPaneIntegration.BindInput(this, attachedPaneList, captureFocus: IsPaneActive);
        }
    }

    private void DetachPaneList()
    {
        if (attachedPaneList == null)
        {
            return;
        }

        try { attachedPaneList.OpeningInventory -= HandleInventoryOpened; }
        catch { }

        try { attachedPaneList.ClosingInventory -= HandleInventoryClosed; }
        catch { }

        attachedPaneList = null;
    }

    private void HandleInventoryOpened()
    {
        if (!IsPaneActive)
        {
            ApplyOverlayVisibility(false);
        }
    }

    private void HandleInventoryClosed()
    {
        ShadeInventoryPaneIntegration.RestoreInputBindings(this);
        ApplyOverlayVisibility(false);
        isActive = false;
        labelPulseTimer = 0f;
        ResetShadeInputState("InventoryClosed");
        UpdateInventoryBinding(false);
    }

    private bool ShouldUseNormalizedFallbackLayout(RectTransform? root, Vector2 rootSize)
    {
        if (root == null)
        {
            return false;
        }

        if (panelRectTemplate.HasValue || contentRectTemplate.HasValue || gridRectTemplate.HasValue ||
            detailRectTemplate.HasValue || gridLayoutTemplate.HasValue)
        {
            return false;
        }

        if (Mathf.Abs(rootSize.x) < MinRootSizeThreshold || Mathf.Abs(rootSize.y) < MinRootSizeThreshold)
        {
            return false;
        }

        float maxDimension = Mathf.Max(rootSize.x, rootSize.y);
        return maxDimension <= 64f;
    }

}
