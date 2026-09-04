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
    /// <summary>
    /// Which of the three sources supplied the last cell size. Recorded because they disagree by
    /// a long way, and a report about the grid being the wrong size cannot be read without it.
    /// </summary>
    private string lastCharmMetricsSource = "defaults";

    private void ResolveCharmLayoutMetrics(int entryCount)
    {
        Vector2 cell = DefaultCharmCellSize;
        Vector2 spacing = DefaultCharmSpacing;
        lastCharmMetricsSource = "defaults";

        if (gridLayoutTemplate.HasValue)
        {
            lastCharmMetricsSource = "template";
            var template = gridLayoutTemplate.Value;
            if (template.CellSize.x >= MinRootSizeThreshold && template.CellSize.y >= MinRootSizeThreshold)
            {
                cell = template.CellSize;
            }

            if (template.Spacing.x >= 0f || template.Spacing.y >= 0f)
            {
                spacing = new Vector2(Mathf.Max(0f, template.Spacing.x), Mathf.Max(0f, template.Spacing.y));
            }
        }
        else if (useNormalizedFallbackLayout)
        {
            lastCharmMetricsSource = "normalized";
            Vector2 rootSize = normalizedFallbackRootSize;
            float effectiveWidth = Mathf.Max(rootSize.x * 0.58f, MinRootSizeThreshold);
            float effectiveHeight = Mathf.Max(rootSize.y * 0.76f, MinRootSizeThreshold);

            float normalizedSpacingX = Mathf.Max(effectiveWidth * 0.025f, MinRootSizeThreshold * 0.5f);
            float normalizedSpacingY = Mathf.Max(effectiveHeight * 0.04f, MinRootSizeThreshold * 0.5f);

            int approxCount = entryCount;
            if (approxCount <= 0)
            {
                approxCount = ShadeRuntime.Charms?.AllCharms.Count ?? (DefaultCharmColumns * CharmRows);
            }

            int approxColumns = Mathf.Max(1, Mathf.CeilToInt(approxCount / (float)CharmRows));
            float totalSpacingX = normalizedSpacingX * Mathf.Max(approxColumns - 1, 0);
            float totalSpacingY = normalizedSpacingY * Mathf.Max(CharmRows - 1, 0);

            float cellWidth = Mathf.Max((effectiveWidth - totalSpacingX) / approxColumns, MinRootSizeThreshold);
            float cellHeight = Mathf.Max((effectiveHeight - totalSpacingY) / CharmRows, MinRootSizeThreshold);

            cell = new Vector2(cellWidth, cellHeight);
            spacing = new Vector2(normalizedSpacingX, normalizedSpacingY);
        }

        float width = Mathf.Abs(cell.x);
        if (width >= CharmCellShrinkWidthThreshold)
        {
            width = Mathf.Max(width * CharmCellShrinkScale, CharmCellMinWidth);
        }

        float height = Mathf.Abs(cell.y);
        if (height >= CharmCellShrinkHeightThreshold)
        {
            height = Mathf.Max(height * CharmCellShrinkScale, CharmCellMinHeight);
        }

        charmCellSize = new Vector2(
            Mathf.Max(width, MinRootSizeThreshold),
            Mathf.Max(height, MinRootSizeThreshold));

        float spacingX = Mathf.Max(spacing.x, 0f);
        float spacingY = Mathf.Max(spacing.y, 0f);
        if (spacingX > 0f)
        {
            spacingX = Mathf.Max(spacingX * CharmSpacingScale, CharmSpacingMin);
        }

        if (spacingY > 0f)
        {
            spacingY = Mathf.Max(spacingY * CharmSpacingScale, CharmSpacingMin);
        }

        charmSpacing = new Vector2(spacingX, spacingY);
        UpdateCharmIconSizeCache();
    }

    /// <summary>
    /// Scales the cells down until the whole grid fits the box left for it. The cloned grid template
    /// hands back a fixed cell size, which was big enough while the roster was 32 charms over eight
    /// columns; the layout positions rows but never shrinks them, so at 42 the grid ran off the
    /// column into the detail panel and up through the notch icons.
    /// <para>
    /// Both axes matter: width decides whether the description is overlapped, height whether the
    /// notch row is.
    /// </para>
    /// </summary>
    private void FitCharmCellsToBox(int columns, float availableWidth, float availableHeight)
    {
        if (columns <= 0 || availableWidth <= 0f || availableHeight <= 0f)
        {
            return;
        }

        float strideX = charmCellSize.x + charmSpacing.x;
        // Odd rows are offset by half a stride, so they are the widest.
        float requiredWidth = charmCellSize.x + Mathf.Max(0, columns - 1) * strideX + strideX * RowOffsetFactor;
        float requiredHeight = CharmRows * charmCellSize.y + Mathf.Max(0, CharmRows - 1) * charmSpacing.y;

        if (requiredWidth <= 0f || requiredHeight <= 0f)
        {
            return;
        }

        float scale = Mathf.Min(availableWidth / requiredWidth, availableHeight / requiredHeight);
        if (scale >= 1f)
        {
            return;
        }

        charmCellSize = new Vector2(
            Mathf.Max(charmCellSize.x * scale, MinRootSizeThreshold),
            Mathf.Max(charmCellSize.y * scale, MinRootSizeThreshold));
        charmSpacing = new Vector2(
            Mathf.Max(charmSpacing.x * scale, 0f),
            Mathf.Max(charmSpacing.y * scale, 0f));

        UpdateCharmIconSizeCache();
    }

    /// <summary>
    /// The scales between the grid and the screen, and the column's drawn width.
    /// <para>
    /// The layout fits the grid in the column's own units, so a scale anywhere up the parent chain
    /// makes the drawn result disagree with the arithmetic without anything looking wrong in the
    /// numbers themselves. Nothing else in the snapshot can tell those two cases apart.
    /// </para>
    /// </summary>
    private string DescribeDrawnScale()
    {
        try
        {
            Vector3 gridScale = gridRoot != null ? gridRoot.lossyScale : Vector3.one;
            Vector3 columnScale = leftContentRoot != null ? leftContentRoot.lossyScale : Vector3.one;
            float canvasScale = overlayCanvas != null ? overlayCanvas.scaleFactor : 1f;
            float derivedScale = overlayCanvas != null
                ? ResolveOverlayCanvasScaleFactor(overlayCanvas.pixelRect)
                : 1f;

            float drawnWidth = 0f;
            if (leftContentRoot != null)
            {
                var corners = new Vector3[4];
                leftContentRoot.GetWorldCorners(corners);
                drawnWidth = Mathf.Abs(corners[2].x - corners[0].x);
            }

            return FormattableString.Invariant(
                $"gridScale={gridScale.x:0.###} columnScale={columnScale.x:0.###} canvasScale={canvasScale:0.###} derivedScale={derivedScale:0.###} columnDrawnWidth={drawnWidth:0.#}");
        }
        catch
        {
            return "scale=unavailable";
        }
    }

    /// <summary>
    /// How far below the top of the left column the notch row actually ends, in that column's own
    /// units. Falls back to <see cref="NotchSectionBottom"/> when the row cannot be measured, and
    /// never reports less than it - the constant is the floor, not the answer.
    /// </summary>
    private float MeasureNotchSectionBottom(float sectionOffsetY)
    {
        float assumed = NotchSectionBottom + sectionOffsetY;
        if (leftContentRoot == null || notchIconContainer == null)
        {
            return assumed;
        }

        try
        {
            var corners = new Vector3[4];
            notchIconContainer.GetWorldCorners(corners);

            // corners[0] is the bottom-left. leftContentRoot's pivot is its top-left, so a point
            // below the top has a negative local y and its depth is the negation.
            float measured = -leftContentRoot.InverseTransformPoint(corners[0]).y;
            return measured > assumed ? measured : assumed;
        }
        catch
        {
            return assumed;
        }
    }

    private int DetermineCharmColumnCount(int entryCount)
    {
        if (gridLayoutTemplate.HasValue &&
            gridLayoutTemplate.Value.Constraint == GridLayoutGroup.Constraint.FixedColumnCount &&
            gridLayoutTemplate.Value.ConstraintCount > 0)
        {
            return gridLayoutTemplate.Value.ConstraintCount;
        }

        int approxCount = entryCount > 0 ? entryCount : ShadeRuntime.Charms?.AllCharms.Count ?? 0;
        if (approxCount <= 0)
        {
            approxCount = DefaultCharmColumns * CharmRows;
        }

        if (approxCount % CharmRows != 0)
        {
            approxCount += CharmRows - (approxCount % CharmRows);
        }

        int columns = Mathf.Max(1, approxCount / CharmRows);
        return columns;
    }

    /// <summary>
    /// Lays the charm grid out.
    /// <para>
    /// Wrapped because every caller is inside the game's own inventory flow: a throw here does not
    /// cost the Shade its charm grid, it costs the player the whole inventory screen. One failed
    /// layout should be a bad-looking pane and a logged reason, not an inventory that will not open.
    /// </para>
    /// </summary>
    private void LayoutCharmEntries()
    {
        try
        {
            LayoutCharmEntriesCore();
        }
        catch (Exception e)
        {
            // Recorded on the snapshot as well as logged: the log ring will not still be holding
            // this by the time anyone files a report about the inventory.
            LastLayoutFailure = e.ToString();
            LogMenuEvent($"Charm grid layout failed; the pane will be misdrawn but the inventory still opens: {e}");
        }
    }

    /// <summary>The last charm-grid layout exception, or null. Read by the bug reporter.</summary>
    internal static string? LastLayoutFailure { get; private set; }

    /// <summary>
    /// The measurements behind the last charm-grid layout. Read by the bug reporter, because "the
    /// grid is sometimes too spread out" cannot be told apart from "the column measured wrong"
    /// without them, and the two want different fixes.
    /// </summary>
    internal static string? LastCharmGridLayout { get; private set; }

    private void LayoutCharmEntriesCore() => LayoutCharmEntriesCore(allowCorrection: true);

    private void LayoutCharmEntriesCore(bool allowCorrection)
    {
        if (gridRoot == null || leftContentRoot == null)
        {
            return;
        }

        try
        {
            // Ahead of the rebuild below, and the reason the pane came out a different size the
            // second time it was opened in a scene: the canvas scaler applies on its own update, so
            // the very first layout after the pane is built measures rects that have not been
            // resolved against it yet. Flushing the canvas makes every layout measure the same
            // thing, rather than the first one measuring a canvas that no longer exists.
            Canvas.ForceUpdateCanvases();
        }
        catch
        {
        }

        try
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(leftContentRoot);
        }
        catch
        {
        }

        if (allowCorrection)
        {
            // Measured afresh on every top-level layout, so a correction cannot outlive the
            // situation that needed it.
            charmMeasuredCorrection = 1f;
            lastGridCorrection = 1f;
        }

        ResolveCharmLayoutMetrics(entries.Count);

        // Applied after the metrics, not folded into them: ResolveCharmLayoutMetrics goes back to
        // its source every time, so a correction written into charmCellSize would be discarded by
        // the very pass that is meant to apply it.
        if (charmMeasuredCorrection < 0.999f)
        {
            charmCellSize *= charmMeasuredCorrection;
            charmSpacing *= charmMeasuredCorrection;
            UpdateCharmIconSizeCache();
        }

        entryGridPositions.Clear();
        entryCenterXs.Clear();

        if (entries.Count == 0)
        {
            gridRoot.sizeDelta = Vector2.zero;
            gridRoot.anchoredPosition = Vector2.zero;
            UpdateDetailPreviewSize();
            return;
        }

        int columns = Mathf.Max(1, DetermineCharmColumnCount(entries.Count));
        int requiredColumns = Mathf.CeilToInt(entries.Count / (float)CharmRows);
        if (requiredColumns > columns)
        {
            columns = requiredColumns;
        }

        // Re-derived per layout rather than trusted from build time. The canvas scaler settles a
        // frame after the pane is built, so a value cached during the build describes a canvas that
        // no longer exists - and every margin below is measured against it.
        var refreshedRoot = overlayRoot != null ? DetermineOverlayCanvasSize(overlayRoot) : Vector2.zero;
        if (refreshedRoot.x > MinRootSizeThreshold && refreshedRoot.y > MinRootSizeThreshold)
        {
            normalizedFallbackRootSize = refreshedRoot;
        }

        Vector2 screenSize = normalizedFallbackRootSize.sqrMagnitude > 0f
            ? normalizedFallbackRootSize
            : DefaultStandaloneRootSize;

        Vector2 measuredParent = leftContentRoot.rect.size;
        Vector2 parentSize = measuredParent;
        if (parentSize.x < MinRootSizeThreshold || parentSize.y < MinRootSizeThreshold)
        {
            Vector2 fallbackParent = normalizedFallbackRootSize.sqrMagnitude > 0f ? normalizedFallbackRootSize : DefaultStandaloneRootSize;
            parentSize = new Vector2(fallbackParent.x * 0.58f, fallbackParent.y * 0.55f);
        }


        // Left edge lines up with the "Equipped" and "Notches" labels rather than sitting inside a
        // margin of its own, which left a column of dead space between them and the grid.
        float sectionOffsetX = ComputeNormalizedMargin(screenSize.x, SectionOffsetFraction * 0.5f);
        float sectionOffsetY = ComputeNormalizedMargin(screenSize.y, SectionOffsetFraction);
        float leftMargin = Mathf.Clamp(SectionLabelInset + sectionOffsetX, 0f, Mathf.Max(0f, parentSize.x - 1f));

        float desiredBottomMargin = ComputeNormalizedMargin(screenSize.y, CharmGridVerticalScreenFraction);
        float parentBottomMargin = ComputeNormalizedMargin(parentSize.y, CharmGridVerticalParentFraction);
        float bottomMargin = Mathf.Min(desiredBottomMargin, parentBottomMargin);

        // The equipped and notch rows own the top of this column, so the grid gets what is left
        // below them. Measured off the notch row itself rather than taken from the constant: that
        // constant is the container's declared height, and the icons inside it are sized from the
        // charm cell, so the row is routinely taller than the number that reserves space for it.
        float notchBottom = MeasureNotchSectionBottom(sectionOffsetY);
        float reservedTop = notchBottom + CharmGridTopGap;
        float availableWidth = Mathf.Max(1f, parentSize.x - leftMargin);
        float availableHeight = Mathf.Max(1f, parentSize.y - reservedTop - bottomMargin);
        reservedTopFraction = parentSize.y > MinRootSizeThreshold
            ? Mathf.Clamp01((reservedTop + bottomMargin) / parentSize.y)
            : 0f;

        FitCharmCellsToBox(columns, availableWidth, availableHeight);

        // After the fit, not folded into it: the fit decides the largest the grid can be without
        // running into anything, and this is the player saying they want it a different size.
        float gridScaleKnob = Mathf.Clamp(ModConfig.Instance?.shadeCharmGridScale ?? 1f, 0.25f, 2f);
        if (!Mathf.Approximately(gridScaleKnob, 1f))
        {
            charmCellSize *= gridScaleKnob;
            charmSpacing *= gridScaleKnob;
            UpdateCharmIconSizeCache();
        }

        // Recorded rather than logged: this is intermittent and the report is filed long after the
        // log ring has rolled, so the numbers behind a bad layout have to survive on the snapshot.
        LastCharmGridLayout = FormattableString.Invariant(
            $"columns={columns} entries={entries.Count} metrics={lastCharmMetricsSource} parent={measuredParent.x:0.#}x{measuredParent.y:0.#} used={parentSize.x:0.#}x{parentSize.y:0.#} screen={screenSize.x:0.#}x{screenSize.y:0.#} available={availableWidth:0.#}x{availableHeight:0.#} notchBottom={notchBottom:0.#} reservedTop={reservedTop:0.#} cell={charmCellSize.x:0.#}x{charmCellSize.y:0.#} spacing={charmSpacing.x:0.#}x{charmSpacing.y:0.#} icon={currentCharmIconSize:0.#} knob={gridScaleKnob:0.###} correction={lastGridCorrection:0.###} {DescribeDrawnScale()}");

        float strideX = charmCellSize.x + charmSpacing.x;
        float strideY = charmCellSize.y + charmSpacing.y;
        float halfStrideX = strideX * RowOffsetFactor;

        var rowCounts = new int[CharmRows];
        int remaining = entries.Count;
        for (int row = 0; row < CharmRows; row++)
        {
            int count = Mathf.Min(columns, remaining);
            rowCounts[row] = count;
            remaining -= count;
        }

        float baseRowWidth = charmCellSize.x + Mathf.Max(0, columns - 1) * strideX;
        float offsetRowWidth = baseRowWidth + halfStrideX;
        float maxWidth = Mathf.Max(baseRowWidth, offsetRowWidth);
        int usedRows = 0;

        for (int row = 0; row < CharmRows; row++)
        {
            int count = rowCounts[row];
            if (count <= 0)
            {
                continue;
            }

            usedRows = row + 1;
            float offset = (row & 1) == 1 ? halfStrideX : 0f;
            float rowWidth = offset + charmCellSize.x + Mathf.Max(0, count - 1) * strideX;
            if (rowWidth > maxWidth)
            {
                maxWidth = rowWidth;
            }
        }

        if (usedRows <= 0)
        {
            usedRows = Mathf.Min(CharmRows, Mathf.Max(1, Mathf.CeilToInt(entries.Count / (float)columns)));
        }

        float totalHeight = usedRows * charmCellSize.y + Mathf.Max(0, usedRows - 1) * charmSpacing.y;

        gridRoot.anchorMin = new Vector2(0f, 0f);
        gridRoot.anchorMax = new Vector2(0f, 0f);
        gridRoot.pivot = new Vector2(0f, 0f);
        gridRoot.sizeDelta = new Vector2(maxWidth, totalHeight);
        gridRoot.anchoredPosition = new Vector2(leftMargin, bottomMargin);

        for (int row = 0, index = 0; row < CharmRows && index < entries.Count; row++)
        {
            int count = rowCounts[row];
            if (count <= 0)
            {
                continue;
            }

            float offset = (row & 1) == 1 ? halfStrideX : 0f;
            float targetRowWidth = offset + charmCellSize.x + Mathf.Max(0, columns - 1) * strideX;
            float actualRowWidth = offset + charmCellSize.x + Mathf.Max(0, count - 1) * strideX;
            float horizontalPadding = Mathf.Max(0f, (targetRowWidth - actualRowWidth) * 0.5f);

            for (int column = 0; column < count && index < entries.Count; column++, index++)
            {
                var entry = entries[index];
                RectTransform? rect = entry.Root;
                float centerX = offset + horizontalPadding + column * strideX + charmCellSize.x * 0.5f;
                float centerY = totalHeight - (charmCellSize.y * 0.5f + row * strideY);

                if (rect != null)
                {
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 0f);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.sizeDelta = charmCellSize;
                    rect.anchoredPosition = new Vector2(centerX, centerY);
                }

                // The icon is re-sized here too. It used to be set once, when the cell was built,
                // from whatever the icon-size cache held at that moment - so the cells moved onto
                // the layout's stride while the art inside them kept an size from a different
                // pass. Drawn and computed have to come from the same numbers or the grid reads as
                // the wrong size however carefully it was fitted.
                var iconRect = entry.Icon != null ? entry.Icon.rectTransform : null;
                if (iconRect != null)
                {
                    iconRect.sizeDelta = new Vector2(currentCharmIconSize, currentCharmIconSize);
                }

                entryGridPositions.Add(new Vector2Int(row, column));
                entryCenterXs.Add(centerX);
            }
        }

        // Everything above is arithmetic against a rect. This asks the screen instead: if the row
        // that was just placed is drawn wider than the column it was fitted into, the numbers and
        // the result have disagreed, and the measurement is the one to believe. Three separate
        // theories about *why* they can disagree have been wrong, so this corrects the outcome
        // rather than the cause - and costs one extra placement pass in the case where it fires.
        // A knob above 1 is an explicit request for a grid larger than fits, so the corrective
        // pass stands down rather than immediately undoing it.
        float correction = allowCorrection && gridScaleKnob <= 1f ? MeasureGridOverrun() : 1f;
        if (correction < 0.995f)
        {
            charmMeasuredCorrection = Mathf.Clamp(correction, 0.15f, 1f);
            lastGridCorrection = charmMeasuredCorrection;
            LayoutCharmEntriesCore(allowCorrection: false);
            return;
        }

        UpdateDetailPreviewSize();
    }

    /// <summary>How much the last layout had to be shrunk by after measuring it. 1 means it fitted.</summary>
    private float lastGridCorrection = 1f;

    /// <summary>
    /// The shrink the measured pass asked for, held across the second placement. Reset at the start
    /// of every top-level layout so it is re-derived rather than accumulated.
    /// </summary>
    private float charmMeasuredCorrection = 1f;

    /// <summary>
    /// The factor the grid would have to be scaled by to sit inside its column, measured from what
    /// is actually drawn. Returns 1 when it already fits, or when nothing can be measured.
    /// </summary>
    private float MeasureGridOverrun()
    {
        if (gridRoot == null || leftContentRoot == null || entries.Count == 0)
        {
            return 1f;
        }

        try
        {
            var columnCorners = new Vector3[4];
            leftContentRoot.GetWorldCorners(columnCorners);
            float columnWidth = Mathf.Abs(columnCorners[2].x - columnCorners[0].x);
            float columnHeight = Mathf.Abs(columnCorners[1].y - columnCorners[0].y);
            if (columnWidth <= MinRootSizeThreshold || columnHeight <= MinRootSizeThreshold)
            {
                return 1f;
            }

            // The cells, not gridRoot. gridRoot's size is set from the same arithmetic being
            // checked, so measuring it only ever confirms itself - which is why the first version
            // of this reported that everything fitted while the grid was visibly overflowing.
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            var cellCorners = new Vector3[4];

            for (int i = 0; i < entries.Count; i++)
            {
                var rect = entries[i].Root;
                if (rect == null)
                {
                    continue;
                }

                rect.GetWorldCorners(cellCorners);
                for (int c = 0; c < 4; c++)
                {
                    minX = Mathf.Min(minX, cellCorners[c].x);
                    maxX = Mathf.Max(maxX, cellCorners[c].x);
                    minY = Mathf.Min(minY, cellCorners[c].y);
                    maxY = Mathf.Max(maxY, cellCorners[c].y);
                }
            }

            if (minX > maxX || minY > maxY)
            {
                return 1f;
            }

            float gridWidth = maxX - minX;
            float gridHeight = maxY - minY;
            if (gridWidth <= MinRootSizeThreshold || gridHeight <= MinRootSizeThreshold)
            {
                return 1f;
            }

            // The column has to hold the grid and the sections above it, so the grid gets what is
            // left below the notch row rather than the whole height.
            float usableHeight = columnHeight * Mathf.Clamp01(1f - (reservedTopFraction));
            float widthRatio = columnWidth / gridWidth;
            float heightRatio = usableHeight > MinRootSizeThreshold ? usableHeight / gridHeight : 1f;

            return Mathf.Clamp(Mathf.Min(widthRatio, heightRatio), 0.2f, 1f);
        }
        catch
        {
            return 1f;
        }
    }

    /// <summary>What proportion of the column the equipped and notch rows take, from the last fit.</summary>
    private float reservedTopFraction;

    private RectTransform? EnsureHighlightRect()
    {
        if (gridRoot == null)
        {
            return null;
        }

        if (highlight != null && highlight)
        {
            return highlight;
        }

        highlight = CreateHighlightRect(gridRoot);
        return highlight;
    }

    private Sprite? ResolveHighlightSprite()
    {
        if (highlightSpriteTemplate != null)
        {
            return highlightSpriteTemplate;
        }

        if (generatedHighlightSprite != null)
        {
            return generatedHighlightSprite;
        }

        const int size = 128;
        var tex = new Texture2D(size, size, TextureFormat.ARGB32, false)
        {
            name = "ShadeCharmHighlightGlowTex",
            hideFlags = HideFlags.HideAndDontSave,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = (size - 1) * 0.5f;
        Color inner = Color.white;
        Color outer = new Color(1f, 1f, 1f, 0f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float t = Mathf.Clamp01(distance / radius);
                float falloff = 1f - t;
                falloff = Mathf.Pow(falloff, 2.35f);
                tex.SetPixel(x, y, Color.Lerp(outer, inner, falloff));
            }
        }

        tex.Apply();

        generatedHighlightTexture = tex;
        generatedHighlightSprite = Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        generatedHighlightSprite.name = "ShadeCharmHighlightGlow";
        generatedHighlightSprite.hideFlags = HideFlags.HideAndDontSave;
        return generatedHighlightSprite;
    }

    private Color AdjustHighlightColor(Color color)
    {
        if (color.a < HighlightMinAlpha)
        {
            color.a = HighlightMinAlpha;
        }

        return color;
    }

    private RectTransform CreateHighlightRect(RectTransform parent)
    {
        var highlightRect = new GameObject("Highlight", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        highlightRect.gameObject.layer = parent.gameObject.layer;
        highlightRect.SetParent(parent, false);
        highlightRect.anchorMin = new Vector2(0.5f, 0.5f);
        highlightRect.anchorMax = new Vector2(0.5f, 0.5f);
        highlightRect.pivot = new Vector2(0.5f, 0.5f);
        highlightRect.localScale = Vector3.one;

        var highlightImage = highlightRect.GetComponent<Image>();
        var sprite = ResolveHighlightSprite();
        if (sprite != null)
        {
            highlightImage.sprite = sprite;
            highlightImage.type = Image.Type.Simple;
            highlightImage.preserveAspect = true;
            var color = AdjustHighlightColor(highlightColor);
            highlightColor = color;
            highlightImage.color = color;
        }
        else
        {
            var color = AdjustHighlightColor(highlightColor);
            highlightColor = color;
            highlightImage.color = color;
        }
        highlightImage.raycastTarget = false;
        highlightRect.gameObject.SetActive(false);
        return highlightRect;
    }

    private void PositionHighlight(RectTransform highlightRect, RectTransform entryRoot)
    {
        if (highlightRect == null || entryRoot == null)
        {
            return;
        }

        highlightRect.SetParent(entryRoot, false);
        highlightRect.SetAsFirstSibling();
        highlightRect.anchorMin = new Vector2(0.5f, 0.5f);
        highlightRect.anchorMax = new Vector2(0.5f, 0.5f);
        highlightRect.pivot = new Vector2(0.5f, 0.5f);
        highlightRect.anchoredPosition = Vector2.zero;
        highlightRect.localScale = Vector3.one;

        Vector2 baseSize = entryRoot.rect.size;
        if (baseSize.x <= 0f || baseSize.y <= 0f)
        {
            baseSize = charmCellSize;
        }

        float glowScale = HighlightScaleMultiplier;
        var newSize = new Vector2(baseSize.x * glowScale, baseSize.y * glowScale);
        highlightRect.sizeDelta = newSize;
    }

    public void ForceLayoutRebuild()
    {
        EnsureBuilt();

        EnsureRootSizing();

        RectTransform? rootRect = overlayRoot;
        if (!IsUnityObjectAlive(rootRect))
        {
            rootRect = EnsureOverlayCanvas();
        }

        if (!IsUnityObjectAlive(rootRect))
        {
            rootRect = transform as RectTransform;
        }

        if (!IsUnityObjectAlive(rootRect))
        {
            LogMenuEvent("ForceLayoutRebuild skipped: no root RectTransform available");
            return;
        }

        var rootRectNonNull = rootRect!;

        if (panelRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRoot);
        }

        if (contentRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);
        }

        if (gridRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridRoot);
        }

        if (equippedIconsRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(equippedIconsRoot);
        }

        if (notchIconContainer != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(notchIconContainer);
        }

        if (detailCostRow != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(detailCostRow);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rootRectNonNull);

        LayoutCharmEntries();

        Vector2 rootSize = rootRectNonNull.rect.size;
        string panelSizeText = panelRoot != null ? FormatVector2(panelRoot.rect.size) : "<null>";
        string contentSizeText = contentRoot != null ? FormatVector2(contentRoot.rect.size) : "<null>";
        string gridSizeText = gridRoot != null ? FormatVector2(gridRoot.rect.size) : "<null>";

        LogMenuEvent(FormattableString.Invariant(
            $"ForceLayoutRebuild -> root={FormatVector2(rootSize)}, panel={panelSizeText}, content={contentSizeText}, grid={gridSizeText}"));
    }

    private void RebuildUI()
    {
        bool wasActive = isActive;

        if (panelRoot != null)
        {
            Destroy(panelRoot.gameObject);
        }

        panelRoot = null!;
        contentRoot = null!;
        gridRoot = null!;
        highlight = null;
        titleText = null!;
        notchText = null!;
        detailTitleText = null!;
        descriptionText = null!;
        statusText = null!;
        hintText = null!;
        detailCostLabel = null;
        detailCostLabelTMP = null;
        entries.Clear();
        notchMeterIcons.Clear();
        detailCostIcons.Clear();
        equippedIcons.Clear();
        leftContentRoot = null;
        notchIconContainer = null;
        detailCostRow = null;
        detailCostIconContainer = null;
        equippedIconsRoot = null;
        isBuilt = false;

        BuildUI();
        if (wasActive)
        {
            ApplyOverlayVisibility(true);
        }
        RefreshAll();
        if (entries.Count > 0)
        {
            SelectIndex(Mathf.Clamp(selectedIndex, 0, entries.Count - 1));
        }
        UpdateNotchMeter();
        UpdateDetailPanel();
    }

    private void BuildUI()
    {
        if (isBuilt)
        {
            return;
        }

        var rootRect = EnsureOverlayCanvas();
        if (rootRect == null)
        {
            LogMenuEvent("BuildUI skipped: overlay canvas unavailable");
            return;
        }

        UpdateOverlayCanvasScaler();

        normalizedFallbackRootSize = DetermineOverlayCanvasSize(rootRect);
        if (normalizedFallbackRootSize.sqrMagnitude <= 0f)
        {
            normalizedFallbackRootSize = DefaultStandaloneRootSize;
        }
        useNormalizedFallbackLayout = ShouldUseNormalizedFallbackLayout(rootRect, normalizedFallbackRootSize);

        Vector2 screenSize = normalizedFallbackRootSize.sqrMagnitude > 0f
            ? normalizedFallbackRootSize
            : DefaultStandaloneRootSize;
        float sectionOffsetX = ComputeNormalizedMargin(screenSize.x, SectionOffsetFraction * 0.5f);
        float sectionOffsetY = ComputeNormalizedMargin(screenSize.y, SectionOffsetFraction);

        if (canvasGroup == null || canvasGroup.gameObject != rootRect.gameObject)
        {
            canvasGroup = rootRect.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = rootRect.gameObject.AddComponent<CanvasGroup>();
            }

            overlaySlide?.Bind(this, rootRect, canvasGroup);
        }

        ApplyOverlayVisibility(false);

        panelRoot = new GameObject("ShadePanel", typeof(RectTransform)).GetComponent<RectTransform>();
        panelRoot.gameObject.layer = rootRect.gameObject.layer;
        panelRoot.SetParent(rootRect, false);
        if (panelRectTemplate.HasValue)
        {
            panelRectTemplate.Value.Apply(panelRoot);
        }
        else if (useNormalizedFallbackLayout)
        {
            panelRoot.anchorMin = Vector2.zero;
            panelRoot.anchorMax = Vector2.one;
            panelRoot.offsetMin = Vector2.zero;
            panelRoot.offsetMax = Vector2.zero;
        }
        else
        {
            panelRoot.anchorMin = Vector2.zero;
            panelRoot.anchorMax = Vector2.one;
            panelRoot.offsetMin = new Vector2(28f, 28f);
            panelRoot.offsetMax = new Vector2(-28f, -32f);
        }

        var panelImage = panelRoot.gameObject.AddComponent<Image>();
        if (panelBackgroundSprite != null)
        {
            panelImage.sprite = panelBackgroundSprite;
            panelImage.type = Image.Type.Sliced;
            panelImage.color = panelBackgroundColor;
        }
        else
        {
            panelImage.enabled = false;
        }
        panelImage.raycastTarget = false;

        contentRoot = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
        contentRoot.gameObject.layer = panelRoot.gameObject.layer;
        contentRoot.SetParent(panelRoot, false);
        if (contentRectTemplate.HasValue)
        {
            contentRectTemplate.Value.Apply(contentRoot);
        }
        else if (useNormalizedFallbackLayout)
        {
            contentRoot.anchorMin = Vector2.zero;
            contentRoot.anchorMax = Vector2.one;
            float marginX = ComputeNormalizedMargin(normalizedFallbackRootSize.x, 0.04f);
            float marginY = ComputeNormalizedMargin(normalizedFallbackRootSize.y, 0.05f);
            contentRoot.offsetMin = new Vector2(marginX, marginY);
            contentRoot.offsetMax = new Vector2(-marginX, -marginY);
        }
        else
        {
            contentRoot.anchorMin = Vector2.zero;
            contentRoot.anchorMax = Vector2.one;
            contentRoot.offsetMin = new Vector2(32f, 36f);
            contentRoot.offsetMax = new Vector2(-32f, -36f);
        }

        titleText = null;
        titleTextTMP = null;

        leftContentRoot = new GameObject("LeftContent", typeof(RectTransform)).GetComponent<RectTransform>();
        leftContentRoot.gameObject.layer = contentRoot.gameObject.layer;
        leftContentRoot.SetParent(contentRoot, false);
        if (useNormalizedFallbackLayout)
        {
            leftContentRoot.anchorMin = new Vector2(0f, 0f);
            leftContentRoot.anchorMax = new Vector2(0.58f, 1f);
            leftContentRoot.pivot = new Vector2(0f, 1f);
            leftContentRoot.offsetMin = Vector2.zero;
            leftContentRoot.offsetMax = Vector2.zero;
        }
        else
        {
            leftContentRoot.anchorMin = new Vector2(0f, 0f);
            leftContentRoot.anchorMax = new Vector2(0.6f, 1f);
            leftContentRoot.pivot = new Vector2(0f, 1f);
            leftContentRoot.offsetMin = new Vector2(24f, 24f);
            leftContentRoot.offsetMax = new Vector2(-24f, -24f);
        }

        var equippedLabel = CreateText("EquippedLabel", leftContentRoot, FontStyle.Normal, 34, TextAnchor.UpperLeft, out var equippedLabelTMP);
        var equippedLabelRect = ResolveRectTransform(equippedLabel, equippedLabelTMP);
        if (equippedLabelRect != null)
        {
            equippedLabelRect.anchorMin = new Vector2(0f, 1f);
            equippedLabelRect.anchorMax = new Vector2(0f, 1f);
            equippedLabelRect.pivot = new Vector2(0f, 1f);
            equippedLabelRect.anchoredPosition = new Vector2(16f + sectionOffsetX, -(28f + sectionOffsetY));
            equippedLabelRect.sizeDelta = new Vector2(360f, 40f);
        }
        SetTextValue(equippedLabel, equippedLabelTMP, "Equipped");

        equippedIconsRoot = new GameObject("EquippedIcons", typeof(RectTransform)).GetComponent<RectTransform>();
        equippedIconsRoot.gameObject.layer = leftContentRoot.gameObject.layer;
        equippedIconsRoot.SetParent(leftContentRoot, false);
        equippedIconsRoot.anchorMin = new Vector2(0f, 1f);
        equippedIconsRoot.anchorMax = new Vector2(1f, 1f);
        equippedIconsRoot.pivot = new Vector2(0f, 1f);
        equippedIconsRoot.anchoredPosition = new Vector2(16f + sectionOffsetX, -(96f + sectionOffsetY));
        equippedIconsRoot.sizeDelta = new Vector2(0f, useNormalizedFallbackLayout ? 96f : 112f);
        var equippedLayout = equippedIconsRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
        equippedLayout.spacing = 12f;
        equippedLayout.childAlignment = TextAnchor.UpperLeft;
        equippedLayout.childControlWidth = false;
        equippedLayout.childControlHeight = false;
        equippedLayout.childForceExpandWidth = false;
        equippedLayout.childForceExpandHeight = false;
        equippedLayout.padding = new RectOffset();
        notchText = CreateText("Notches", leftContentRoot, FontStyle.Normal, 32, TextAnchor.UpperLeft, out notchTextTMP);
        var notchLabelRect = ResolveRectTransform(notchText, notchTextTMP);
        if (notchLabelRect != null)
        {
            notchLabelRect.anchorMin = new Vector2(0f, 1f);
            notchLabelRect.anchorMax = new Vector2(0f, 1f);
            notchLabelRect.pivot = new Vector2(0f, 1f);
            notchLabelRect.anchoredPosition = new Vector2(16f + sectionOffsetX, -(236f + sectionOffsetY));
            notchLabelRect.sizeDelta = new Vector2(360f, 36f);
        }
        SetTextValue(notchText, notchTextTMP, "Notches");
        if (notchTextTMP != null)
        {
            notchTextTMP.textWrappingMode = TextWrappingModes.NoWrap;
            notchTextTMP.overflowMode = TextOverflowModes.Overflow;
        }
        else if (notchText != null)
        {
            notchText.horizontalOverflow = HorizontalWrapMode.Overflow;
        }

        notchIconContainer = new GameObject("NotchIcons", typeof(RectTransform)).GetComponent<RectTransform>();
        notchIconContainer.gameObject.layer = leftContentRoot.gameObject.layer;
        notchIconContainer.SetParent(leftContentRoot, false);
        notchIconContainer.anchorMin = new Vector2(0f, 1f);
        notchIconContainer.anchorMax = new Vector2(1f, 1f);
        notchIconContainer.pivot = new Vector2(0f, 1f);
        notchIconContainer.anchoredPosition = new Vector2(16f + sectionOffsetX, -(284f + sectionOffsetY));
        notchIconContainer.sizeDelta = new Vector2(0f, useNormalizedFallbackLayout ? 52f : 56f);
        var notchLayout = notchIconContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
        notchLayout.spacing = 8f;
        notchLayout.childAlignment = TextAnchor.MiddleLeft;
        notchLayout.childControlWidth = false;
        notchLayout.childControlHeight = false;
        notchLayout.childForceExpandWidth = false;
        notchLayout.childForceExpandHeight = false;
        notchLayout.padding = new RectOffset();

        BuildIconPool(equippedIconsRoot, equippedIcons, MaxEquippedIcons, "EquippedCharm", new Vector2(96f, 96f));
        EnsureEquippedOvercharmBackdrop();
        ResetEquippedDisplayState();
        BuildIconPool(notchIconContainer, notchMeterIcons, MaxNotchIcons, "NotchIcon", new Vector2(32f, 32f));

        gridRoot = new GameObject("CharmGrid", typeof(RectTransform)).GetComponent<RectTransform>();
        gridRoot.gameObject.layer = leftContentRoot.gameObject.layer;
        gridRoot.SetParent(leftContentRoot, false);
        gridRoot.anchorMin = new Vector2(0f, 0f);
        gridRoot.anchorMax = new Vector2(0f, 0f);
        gridRoot.pivot = new Vector2(0f, 0f);
        gridRoot.sizeDelta = Vector2.zero;
        gridRoot.anchoredPosition = Vector2.zero;

        ResolveCharmLayoutMetrics(entries.Count);

        var highlightRect = EnsureHighlightRect();
        if (highlightRect != null)
        {
            highlightRect.SetParent(gridRoot, false);
            highlightRect.gameObject.SetActive(false);
        }

        var detailRoot = new GameObject("Details", typeof(RectTransform)).GetComponent<RectTransform>();
        detailRoot.gameObject.layer = contentRoot.gameObject.layer;
        detailRoot.SetParent(contentRoot, false);
        if (detailRectTemplate.HasValue)
        {
            detailRectTemplate.Value.Apply(detailRoot);
        }
        else if (useNormalizedFallbackLayout)
        {
            detailRoot.anchorMin = new Vector2(0.58f, 0f);
            detailRoot.anchorMax = new Vector2(1f, 1f);
            detailRoot.pivot = new Vector2(0f, 1f);
            float detailMarginX = ComputeNormalizedMargin(normalizedFallbackRootSize.x, 0.02f);
            float detailMarginY = ComputeNormalizedMargin(normalizedFallbackRootSize.y, 0.02f);
            detailRoot.offsetMin = new Vector2(detailMarginX, detailMarginY);
            detailRoot.offsetMax = new Vector2(-detailMarginX, -detailMarginY);
        }
        else
        {
            detailRoot.anchorMin = new Vector2(0.62f, 0f);
            detailRoot.anchorMax = new Vector2(1f, 1f);
            detailRoot.pivot = new Vector2(0f, 1f);
            detailRoot.offsetMin = new Vector2(24f, 16f);
            detailRoot.offsetMax = new Vector2(-16f, -104f);
        }

        detailTitleText = CreateText("CharmName", detailRoot, FontStyle.Normal, 38, TextAnchor.UpperCenter, out detailTitleTextTMP, useHeaderFont: true);
        var detailTitleRect = ResolveRectTransform(detailTitleText, detailTitleTextTMP);

        float detailTopPadding;
        float titleHeight;
        float costHeight;
        float rowGap;
        float previewGap;
        float descriptionGap;
        float bottomPadding;

        if (useNormalizedFallbackLayout)
        {
            detailHorizontalMargin = ComputeNormalizedMargin(normalizedFallbackRootSize.x, 0.05f);
            detailTopPadding = ComputeNormalizedMargin(normalizedFallbackRootSize.y, 0.05f);
            titleHeight = ComputeNormalizedMargin(normalizedFallbackRootSize.y, 0.085f);
            costHeight = ComputeNormalizedMargin(normalizedFallbackRootSize.y, 0.06f);
            rowGap = ComputeNormalizedMargin(normalizedFallbackRootSize.y, 0.005f);
            previewGap = ComputeNormalizedMargin(normalizedFallbackRootSize.y, 0.015f);
            descriptionGap = ComputeNormalizedMargin(normalizedFallbackRootSize.y, 0.02f);
            bottomPadding = ComputeNormalizedMargin(normalizedFallbackRootSize.y, 0.045f);
        }
        else
        {
            detailHorizontalMargin = 48f;
            detailTopPadding = 32f;
            titleHeight = 64f;
            costHeight = 44f;
            rowGap = 4f;
            previewGap = 18f;
            descriptionGap = 24f;
            bottomPadding = 48f;
        }

        detailDescriptionGap = descriptionGap;
        detailDescriptionBottomPadding = bottomPadding;
        detailPreviewTopOffset = detailTopPadding + titleHeight + rowGap + costHeight + previewGap;

        if (detailTitleRect != null)
        {
            detailTitleRect.anchorMin = new Vector2(0f, 1f);
            detailTitleRect.anchorMax = new Vector2(1f, 1f);
            detailTitleRect.pivot = new Vector2(0.5f, 1f);
            detailTitleRect.offsetMin = new Vector2(detailHorizontalMargin, -(detailTopPadding + titleHeight));
            detailTitleRect.offsetMax = new Vector2(-detailHorizontalMargin, -detailTopPadding);
        }
        SetTextValue(detailTitleText, detailTitleTextTMP, displayLabel);

        detailCostRow = new GameObject("CostRow", typeof(RectTransform)).GetComponent<RectTransform>();
        detailCostRow.gameObject.layer = detailRoot.gameObject.layer;
        detailCostRow.SetParent(detailRoot, false);
        detailCostRow.anchorMin = new Vector2(0f, 1f);
        detailCostRow.anchorMax = new Vector2(1f, 1f);
        detailCostRow.pivot = new Vector2(0.5f, 1f);
        float costTop = detailTopPadding + titleHeight + rowGap;
        detailCostRow.offsetMin = new Vector2(detailHorizontalMargin, -(costTop + costHeight));
        detailCostRow.offsetMax = new Vector2(-detailHorizontalMargin, -costTop);

        var costLayout = detailCostRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        costLayout.spacing = 12f;
        costLayout.childAlignment = TextAnchor.MiddleCenter;
        costLayout.childControlWidth = false;
        costLayout.childControlHeight = false;
        costLayout.childForceExpandWidth = false;
        costLayout.childForceExpandHeight = false;
        costLayout.padding = new RectOffset();

        detailCostLabel = CreateText("CostLabel", detailCostRow, FontStyle.Normal, 28, TextAnchor.MiddleCenter, out detailCostLabelTMP);
        var detailCostLabelRect = ResolveRectTransform(detailCostLabel, detailCostLabelTMP);
        if (detailCostLabelRect != null)
        {
            detailCostLabelRect.anchorMin = new Vector2(0.5f, 0.5f);
            detailCostLabelRect.anchorMax = new Vector2(0.5f, 0.5f);
            detailCostLabelRect.pivot = new Vector2(0.5f, 0.5f);
            detailCostLabelRect.sizeDelta = new Vector2(160f, costHeight);
        }
        SetTextValue(detailCostLabel, detailCostLabelTMP, "Cost");

        detailCostIconContainer = new GameObject("CostIcons", typeof(RectTransform)).GetComponent<RectTransform>();
        detailCostIconContainer.gameObject.layer = detailRoot.gameObject.layer;
        detailCostIconContainer.SetParent(detailCostRow, false);
        var costIconsLayout = detailCostIconContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
        costIconsLayout.spacing = 8f;
        costIconsLayout.childAlignment = TextAnchor.MiddleCenter;
        costIconsLayout.childControlWidth = false;
        costIconsLayout.childControlHeight = false;
        costIconsLayout.childForceExpandWidth = false;
        costIconsLayout.childForceExpandHeight = false;
        costIconsLayout.padding = new RectOffset();
        var costIconsElement = detailCostIconContainer.gameObject.AddComponent<LayoutElement>();
        costIconsElement.flexibleWidth = 1f;
        costIconsElement.minHeight = costHeight;
        BuildIconPool(detailCostIconContainer, detailCostIcons, MaxNotchIcons, "CostIcon", new Vector2(32f, 32f));

        detailPreviewRect = new GameObject("CharmPreview", typeof(RectTransform)).GetComponent<RectTransform>();
        detailPreviewRect.gameObject.layer = detailRoot.gameObject.layer;
        detailPreviewRect.SetParent(detailRoot, false);
        detailPreviewRect.anchorMin = new Vector2(0.5f, 1f);
        detailPreviewRect.anchorMax = new Vector2(0.5f, 1f);
        detailPreviewRect.pivot = new Vector2(0.5f, 1f);
        detailPreviewRect.anchoredPosition = new Vector2(0f, -detailPreviewTopOffset);

        detailPreviewImage = detailPreviewRect.gameObject.AddComponent<Image>();
        detailPreviewImage.raycastTarget = false;
        detailPreviewImage.preserveAspect = true;
        detailPreviewImage.enabled = false;
        UpdateDetailPreviewSize();

        descriptionText = CreateText("Description", detailRoot, FontStyle.Normal, 20, TextAnchor.UpperLeft, out descriptionTextTMP);
        var descRect = ResolveRectTransform(descriptionText, descriptionTextTMP);
        if (descRect != null)
        {
            descRect.anchorMin = new Vector2(0f, 0f);
            descRect.anchorMax = new Vector2(1f, 1f);
            descRect.pivot = new Vector2(0f, 1f);
            float previewSize = detailPreviewRect != null ? detailPreviewRect.sizeDelta.y : CalculateDetailPreviewSize();
            float descriptionTop = detailPreviewTopOffset + previewSize + detailDescriptionGap;
            descRect.offsetMin = new Vector2(detailHorizontalMargin, detailDescriptionBottomPadding);
            descRect.offsetMax = new Vector2(-detailHorizontalMargin, -descriptionTop);
        }
        if (descriptionText != null)
        {
            descriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
            descriptionText.verticalOverflow = VerticalWrapMode.Overflow;
            descriptionText.lineSpacing = 1.1f;
            descriptionText.fontSize = 20;
        }
        else if (descriptionTextTMP != null)
        {
            descriptionTextTMP.lineSpacing = 1.1f;
            descriptionTextTMP.textWrappingMode = TextWrappingModes.Normal;
            descriptionTextTMP.fontSize = 20f;
            descriptionTextTMP.margin = new Vector4(0f, 0f, 0f, 0f);
        }

        statusText = CreateText("Status", detailRoot, FontStyle.Italic, 28, TextAnchor.UpperLeft, out statusTextTMP);
        var statusRect = ResolveRectTransform(statusText, statusTextTMP);
        if (statusRect != null)
        {
            if (useNormalizedFallbackLayout)
            {
                statusRect.anchorMin = new Vector2(0f, 0.12f);
                statusRect.anchorMax = new Vector2(1f, 0.24f);
                statusRect.pivot = new Vector2(0f, 1f);
                statusRect.offsetMin = Vector2.zero;
                statusRect.offsetMax = Vector2.zero;
            }
            else
            {
                statusRect.anchorMin = new Vector2(0f, 0.12f);
                statusRect.anchorMax = new Vector2(1f, 0.24f);
                // Inset by the same margin the title and description use. Left flush against the
                // panel edges, both "This charm has not been discovered." and the bench message ran
                // out under the frame art on the right.
                statusRect.offsetMin = new Vector2(detailHorizontalMargin, 6f);
                statusRect.offsetMax = new Vector2(-detailHorizontalMargin, 0f);
            }
        }

        hintText = CreateText("Hint", detailRoot, FontStyle.Normal, 24, TextAnchor.UpperLeft, out hintTextTMP);
        var hintRect = ResolveRectTransform(hintText, hintTextTMP);
        if (hintRect != null)
        {
            if (useNormalizedFallbackLayout)
            {
                hintRect.anchorMin = new Vector2(0f, 0f);
                hintRect.anchorMax = new Vector2(1f, 0.12f);
                hintRect.pivot = new Vector2(0f, 0f);
                hintRect.offsetMin = Vector2.zero;
                hintRect.offsetMax = Vector2.zero;
            }
            else
            {
                hintRect.anchorMin = new Vector2(0f, 0f);
                hintRect.anchorMax = new Vector2(1f, 0.12f);
                hintRect.offsetMin = new Vector2(detailHorizontalMargin, 2f);
                hintRect.offsetMax = new Vector2(-detailHorizontalMargin, 0f);
            }
        }
        SetTextValue(hintText, hintTextTMP, string.Empty);
        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
        else if (hintTextTMP != null)
        {
            hintTextTMP.gameObject.SetActive(false);
        }

        isBuilt = true;
        if (contentRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);
        }
        LogMenuEvent("BuildUI complete");
    }

    private Text? CreateText(string name, RectTransform parent, FontStyle style, int size, TextAnchor anchor, out TMP_Text? tmpText, bool useHeaderFont = false)
    {
        tmpText = null;

        var go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        var tmpStyle = useHeaderFont ? headerTmpTextStyle : bodyTmpTextStyle;
        TMP_FontAsset? fallbackTmpFont = tmpStyle.HasValue && tmpStyle.Value.Font != null
            ? tmpStyle.Value.Font
            : ResolveTrajanFontAsset();

        if (tmpStyle.HasValue || fallbackTmpFont != null)
        {
            var tmpComponent = go.AddComponent<TextMeshProUGUI>();
            tmpText = tmpComponent;
            ApplyTmpTextStyle(tmpComponent, tmpStyle, fallbackTmpFont, useHeaderFont ? headerFontColor : bodyFontColor, ConvertFontStyle(style), size, ConvertAlignment(anchor));
            tmpComponent.raycastTarget = false;
            tmpComponent.text = string.Empty;

            if (fallbackTmpFont != null && !FontMatchesTrajan(tmpComponent.font))
            {
                tmpComponent.font = fallbackTmpFont;
            }

            if (fallbackTmpFont != null)
            {
                var sourceFont = fallbackTmpFont.sourceFontFile ?? ResolveTrajanSourceFont();
                if (useHeaderFont)
                {
                    if (headerFont == null && sourceFont != null)
                    {
                        headerFont = sourceFont;
                    }
                }
                else if (bodyFont == null && sourceFont != null)
                {
                    bodyFont = sourceFont;
                }
            }

            return null;
        }

        var text = go.AddComponent<Text>();
        var styleData = useHeaderFont ? headerTextStyle : bodyTextStyle;
        Font? fallbackFont = useHeaderFont ? headerFont : bodyFont;
        if (fallbackFont == null)
        {
            fallbackFont = ResolveTrajanSourceFont() ?? fallbackFont;
        }
        Color fallbackColor = useHeaderFont ? headerFontColor : bodyFontColor;
        ApplyTextStyle(text, styleData, fallbackFont, fallbackColor, style, size, anchor);
        if (useHeaderFont && headerFont == null)
        {
            headerFont = text.font;
        }
        else if (!useHeaderFont && bodyFont == null)
        {
            bodyFont = text.font;
        }

        text.raycastTarget = false;
        text.text = string.Empty;
        return text;
    }

    private void BuildIconPool(RectTransform container, List<Image> target, int count, string prefix, Vector2 size)
    {
        if (container == null)
        {
            return;
        }

        target.Clear();
        for (int i = 0; i < count; i++)
        {
            var icon = new GameObject(prefix + "_" + i, typeof(RectTransform)).GetComponent<RectTransform>();
            icon.gameObject.layer = container.gameObject.layer;
            icon.SetParent(container, false);
            icon.sizeDelta = size;
            icon.anchorMin = new Vector2(0f, 0.5f);
            icon.anchorMax = new Vector2(0f, 0.5f);
            icon.pivot = new Vector2(0.5f, 0.5f);

            var image = icon.gameObject.AddComponent<Image>();
            image.raycastTarget = false;
            image.preserveAspect = true;
            image.enabled = false;
            icon.gameObject.SetActive(false);
            target.Add(image);
        }
    }

    private static void EnsureNotchSprites()
    {
        if (notchSpritesSearched)
        {
            return;
        }

        notchLitSprite = ShadeCharmIconLoader.TryLoadIcon(NotchLitSpriteName, NotchLitSpriteName + ".png");
        notchUnlitSprite = ShadeCharmIconLoader.TryLoadIcon(LockedCharmSpriteName, LockedCharmSpriteName + ".png");

        if (notchLitSprite == null)
        {
            notchLitSprite = ResolveLockedCharmSprite();
        }

        if (notchUnlitSprite == null)
        {
            notchUnlitSprite = ResolveLockedCharmSprite() ?? notchLitSprite;
        }

        notchSpritesSearched = true;
    }

    private Image? EnsureEquippedOvercharmBackdrop()
    {
        if (equippedIconsRoot == null)
        {
            return null;
        }

        if (equippedOvercharmBackdrop != null)
        {
            return equippedOvercharmBackdrop;
        }

        var go = new GameObject("EquippedOvercharmBackdrop", typeof(RectTransform));
        go.layer = equippedIconsRoot.gameObject.layer;
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(equippedIconsRoot, false);
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.SetAsFirstSibling();

        var layoutElement = go.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;

        var image = go.AddComponent<Image>();
        image.raycastTarget = false;
        image.preserveAspect = true;

        var sprite = ResolveOvercharmBackdropSprite();
        if (sprite != null)
        {
            image.sprite = sprite;
            image.color = new Color(1f, 1f, 1f, 0.85f);
        }
        else
        {
            image.sprite = GetFallbackSprite();
            image.color = OvercharmedBackdropFallbackColor;
        }

        image.enabled = false;
        go.SetActive(false);
        equippedOvercharmBackdrop = image;
        return image;
    }

    private void UpdateEquippedOvercharmBackdrop(bool overcharmed, int equippedCount)
    {
        var image = EnsureEquippedOvercharmBackdrop();
        if (image == null)
        {
            return;
        }

        if (!overcharmed || equippedCount <= 0)
        {
            image.enabled = false;
            image.gameObject.SetActive(false);
            return;
        }

        if (equippedIconsRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(equippedIconsRoot);
        }

        var sprite = ResolveOvercharmBackdropSprite();
        if (sprite != null && image.sprite != sprite)
        {
            image.sprite = sprite;
        }

        image.color = sprite != null ? new Color(1f, 1f, 1f, 0.7f) : OvercharmedBackdropFallbackColor;

        if (!TryCalculateEquippedIconBounds(out var bounds))
        {
            // One nominal cell per equipped charm, centred on the row, for the frame before the
            // layout has run.
            bounds = new Rect(0f, -48f, 96f * Mathf.Max(1, equippedCount), 96f);
        }

        const float paddingRight = 24f;
        const float paddingY = 24f;

        float leftEdge = bounds.xMin;
        float rightEdge = bounds.xMax + paddingRight;
        float width = Mathf.Max(0f, rightEdge - leftEdge);
        float centerY = bounds.center.y;
        float height = bounds.height;

        var rect = image.rectTransform;
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(leftEdge, centerY);
        rect.sizeDelta = new Vector2(width, height + paddingY * 2f);

        image.gameObject.SetActive(true);
        image.enabled = true;
        image.transform.SetAsFirstSibling();
    }

    /// <summary>
    /// The box the equipped-charm icons currently occupy, in <c>equippedIconsRoot</c>'s local space.
    /// False when the row is empty or not built yet, in which case the caller supplies its own.
    /// </summary>
    private bool TryCalculateEquippedIconBounds(out Rect bounds)
    {
        bounds = default;

        if (equippedIconsRoot == null)
        {
            return false;
        }

        bool hasIcon = false;
        Vector3 minBounds = Vector3.zero;
        Vector3 maxBounds = Vector3.zero;

        foreach (var icon in equippedIcons)
        {
            if (icon == null || !icon.enabled || !icon.gameObject.activeSelf)
            {
                continue;
            }

            var rect = icon.rectTransform;
            if (rect == null)
            {
                continue;
            }

            var iconBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(equippedIconsRoot, rect);
            if (!hasIcon)
            {
                minBounds = iconBounds.min;
                maxBounds = iconBounds.max;
                hasIcon = true;
            }
            else
            {
                minBounds = Vector3.Min(minBounds, iconBounds.min);
                maxBounds = Vector3.Max(maxBounds, iconBounds.max);
            }
        }

        if (!hasIcon)
        {
            return false;
        }

        bounds = Rect.MinMaxRect(minBounds.x, minBounds.y, maxBounds.x, maxBounds.y);
        return true;
    }

    private void EnsureEquippedDisplayCapacity()
    {
        int target = equippedIcons.Count;
        if (equippedDisplayIds.Count != target)
        {
            equippedDisplayIds.Clear();
            for (int i = 0; i < target; i++)
            {
                equippedDisplayIds.Add(null);
            }
        }
    }

    private void ResetEquippedDisplayState()
    {
        previousEquippedOrder.Clear();
        EnsureEquippedDisplayCapacity();
        for (int i = 0; i < equippedDisplayIds.Count; i++)
        {
            equippedDisplayIds[i] = null;
        }
        ClearActiveCharmFlights();
        hasRenderedEquippedRow = false;
    }

    private void CaptureEquippedIconState()
    {
        EnsureEquippedDisplayCapacity();
        previousEquippedOrder.Clear();
        for (int i = 0; i < equippedDisplayIds.Count; i++)
        {
            var id = equippedDisplayIds[i];
            if (id.HasValue && !previousEquippedOrder.Contains(id.Value))
            {
                previousEquippedOrder.Add(id.Value);
            }
        }
    }

    private void AnimateEquippedChanges(
        IReadOnlyList<ShadeCharmId> previousOrder,
        IReadOnlyList<(ShadeCharmId Id, ShadeCharmDefinition Definition)> currentOrder,
        bool overcharmed)
    {
        if (previousOrder == null || currentOrder == null)
        {
            return;
        }

        foreach (var pair in currentOrder)
        {
            if (previousOrder.Contains(pair.Id))
            {
                continue;
            }

            int targetIndex = -1;
            for (int i = 0; i < currentOrder.Count; i++)
            {
                if (currentOrder[i].Id.Equals(pair.Id))
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex < 0 || targetIndex >= equippedIcons.Count)
            {
                continue;
            }

            var destination = equippedIcons[targetIndex];
            if (destination == null)
            {
                continue;
            }

            CharmEntry entry = default;
            bool found = false;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Id.Equals(pair.Id))
                {
                    entry = entries[i];
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                continue;
            }

            StartCharmFlightAnimation(entry, pair.Definition, destination, overcharmed);
        }
    }

    private void RefreshEquippedLayoutImmediate()
    {
        try
        {
            Canvas.ForceUpdateCanvases();
        }
        catch
        {
        }

        if (leftContentRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(leftContentRoot);
        }

        if (equippedIconsRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(equippedIconsRoot);
        }
    }

    private int DetermineNextEquippedSlotIndex()
    {
        EnsureEquippedDisplayCapacity();
        if (equippedIcons.Count == 0)
        {
            return -1;
        }

        int limit = Mathf.Min(equippedDisplayIds.Count, equippedIcons.Count);
        int filled = 0;
        for (int i = 0; i < limit; i++)
        {
            if (equippedDisplayIds[i].HasValue)
            {
                filled++;
            }
        }

        if (filled < equippedIcons.Count)
        {
            return filled;
        }

        return equippedIcons.Count - 1;
    }

}
