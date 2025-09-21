using System;
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

internal sealed class ShadeInventoryPane : InventoryPane
{
    private const int CharmRows = 4;
    private const int DefaultCharmColumns = 6;
    private static readonly Vector2 DefaultCharmCellSize = new Vector2(104f, 112f);
    private static readonly Vector2 DefaultCharmSpacing = new Vector2(16f, 16f);
    private const float RowOffsetFactor = 0.5f;
    private const float CharmIconSizeMultiplier = 0.69f;
    private const float CharmCellShrinkScale = 0.9f;
    private const float CharmCellShrinkWidthThreshold = 96f;
    private const float CharmCellShrinkHeightThreshold = 108f;
    private const float CharmCellMinWidth = 72f;
    private const float CharmCellMinHeight = 80f;
    private const float CharmSpacingScale = 0.4f;
    private const float CharmSpacingMin = 4f;
    private const float BackgroundAlpha = 0.82f;
    // Vanilla charm panes report RectTransform sizes of roughly 6.5 × 8 units even
    // though the UI fills the screen once the canvas scale factor is applied. Treat
    // anything above a minimal epsilon as "valid" so we can adopt those template
    // metrics instead of falling back to oversized screen-space defaults.
    internal const float MinRootSizeThreshold = 0.1f;
    internal const float MinTemplateCopyDimension = 48f;
    internal const float MinTemplateCopyArea = 4096f;

    private static readonly Color DefaultPanelColor = new Color(0.05f, 0.05f, 0.08f, 0.92f);
    private static readonly Color DefaultHighlightColor = new Color(0.78f, 0.86f, 1f, 0.35f);
    private static readonly Color DefaultCellColor = new Color(0.18f, 0.2f, 0.26f, BackgroundAlpha);
    private static readonly Vector2 DefaultStandaloneRootSize = new Vector2(1920f, 1080f);
    private const string LockedCharmSpriteName = "shade_charm_charmui0001charmcost02unlit";
    private const string NotchLitSpriteName = "shade_charm_charmui0000charmcost02lit";
    private const int MaxNotchIcons = 20;
    private const int MaxEquippedIcons = 20;

    private static readonly Color LockedIconColor = new Color(1f, 1f, 1f, 0.72f);
    private static readonly Color InactiveIconColor = new Color(1f, 1f, 1f, 0.3f);
    private static readonly Color EquippedIconColor = new Color(0.82f, 0.95f, 1f, 1f);
    private static readonly Color BrokenIconColor = new Color(1f, 0.64f, 0.64f, 1f);

    private static Sprite? lockedCharmSprite;
    private static bool lockedCharmSpriteSearched;

    private struct RectSnapshot
    {
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Pivot;
        public Vector2 OffsetMin;
        public Vector2 OffsetMax;
        public Vector2 AnchoredPosition;
        public float AnchoredPositionZ;
        public Vector2 SizeDelta;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;

        public static RectSnapshot From(RectTransform rect)
        {
            return new RectSnapshot
            {
                AnchorMin = rect.anchorMin,
                AnchorMax = rect.anchorMax,
                Pivot = rect.pivot,
                OffsetMin = rect.offsetMin,
                OffsetMax = rect.offsetMax,
                AnchoredPosition = rect.anchoredPosition,
                AnchoredPositionZ = rect.anchoredPosition3D.z,
                SizeDelta = rect.sizeDelta,
                LocalRotation = rect.localRotation,
                LocalScale = rect.localScale
            };
        }

        public void Apply(RectTransform rect)
        {
            rect.anchorMin = AnchorMin;
            rect.anchorMax = AnchorMax;
            rect.pivot = Pivot;
            rect.offsetMin = OffsetMin;
            rect.offsetMax = OffsetMax;
            rect.anchoredPosition = AnchoredPosition;
            rect.anchoredPosition3D = new Vector3(AnchoredPosition.x, AnchoredPosition.y, AnchoredPositionZ);
            rect.sizeDelta = SizeDelta;
            rect.localRotation = LocalRotation;
            rect.localScale = LocalScale;
        }
    }

    private struct LayoutElementSnapshot
    {
        public bool IgnoreLayout;
        public float MinWidth;
        public float PreferredWidth;
        public float FlexibleWidth;
        public float MinHeight;
        public float PreferredHeight;
        public float FlexibleHeight;
        public int LayoutPriority;

        public static LayoutElementSnapshot From(LayoutElement element)
        {
            return new LayoutElementSnapshot
            {
                IgnoreLayout = element.ignoreLayout,
                MinWidth = element.minWidth,
                PreferredWidth = element.preferredWidth,
                FlexibleWidth = element.flexibleWidth,
                MinHeight = element.minHeight,
                PreferredHeight = element.preferredHeight,
                FlexibleHeight = element.flexibleHeight,
                LayoutPriority = element.layoutPriority
            };
        }

        public void Apply(LayoutElement element)
        {
            element.ignoreLayout = IgnoreLayout;
            element.minWidth = MinWidth;
            element.preferredWidth = PreferredWidth;
            element.flexibleWidth = FlexibleWidth;
            element.minHeight = MinHeight;
            element.preferredHeight = PreferredHeight;
            element.flexibleHeight = FlexibleHeight;
            element.layoutPriority = LayoutPriority;
        }
    }

    private struct GridLayoutSnapshot
    {
        public Vector2 CellSize;
        public Vector2 Spacing;
        public GridLayoutGroup.Axis StartAxis;
        public GridLayoutGroup.Corner StartCorner;
        public GridLayoutGroup.Constraint Constraint;
        public int ConstraintCount;
        public RectOffset Padding;
        public TextAnchor ChildAlignment;

        public static GridLayoutSnapshot From(GridLayoutGroup grid)
        {
            return new GridLayoutSnapshot
            {
                CellSize = grid.cellSize,
                Spacing = grid.spacing,
                StartAxis = grid.startAxis,
                StartCorner = grid.startCorner,
                Constraint = grid.constraint,
                ConstraintCount = grid.constraintCount,
                Padding = new RectOffset(grid.padding.left, grid.padding.right, grid.padding.top, grid.padding.bottom),
                ChildAlignment = grid.childAlignment
            };
        }

        public void Apply(GridLayoutGroup grid)
        {
            grid.cellSize = CellSize;
            grid.spacing = Spacing;
            grid.startAxis = StartAxis;
            grid.startCorner = StartCorner;
            grid.constraint = Constraint;
            grid.constraintCount = ConstraintCount;
            grid.padding = new RectOffset(Padding.left, Padding.right, Padding.top, Padding.bottom);
            grid.childAlignment = ChildAlignment;
        }
    }

    private readonly List<CharmEntry> entries = new List<CharmEntry>();
    private readonly List<Vector2Int> entryGridPositions = new List<Vector2Int>();
    private readonly List<float> entryCenterXs = new List<float>();

    private RectTransform panelRoot = null!;
    private RectTransform contentRoot = null!;
    private RectTransform gridRoot = null!;
    private RectTransform? highlight;
    private Text? titleText;
    private Text? notchText;
    private Text? detailTitleText;
    private Text? descriptionText;
    private Text? statusText;
    private Text? hintText;
    private Text? detailCostLabel;
    private TMP_Text? titleTextTMP;
    private TMP_Text? notchTextTMP;
    private TMP_Text? detailTitleTextTMP;
    private TMP_Text? descriptionTextTMP;
    private TMP_Text? statusTextTMP;
    private TMP_Text? hintTextTMP;
    private CanvasGroup canvasGroup = null!;

    private GameObject? overlayCanvasObject;
    private RectTransform? overlayRoot;
    private Canvas? overlayCanvas;
    private CanvasScaler? overlayCanvasScaler;
    private GraphicRaycaster? overlayRaycaster;

    private Font? bodyFont;
    private Font? headerFont;
    private Color bodyFontColor = Color.white;
    private Color headerFontColor = Color.white;
    private Sprite? panelBackgroundSprite;
    private Color panelBackgroundColor = DefaultPanelColor;
    private Sprite? highlightSpriteTemplate;
    private Color highlightColor = DefaultHighlightColor;
    private Sprite? cellFrameSprite;
    private Color cellFrameColor = DefaultCellColor;

    private ShadeCharmInventory? inventory;
    private ShadeCharmInventory? subscribedInventory;
    private InventoryPaneList? attachedPaneList;
    private int selectedIndex;
    private bool isBuilt;
    private bool isActive;
    private float labelPulseTimer;
    private Sprite? fallbackSprite;
    private string displayLabel = "Charms";

    private RectSnapshot? panelRectTemplate;
    private RectSnapshot? contentRectTemplate;
    private RectSnapshot? gridRectTemplate;
    private RectSnapshot? detailRectTemplate;
    private RectSnapshot? rootRectTemplate;
    private GridLayoutSnapshot? gridLayoutTemplate;
    private Vector2? templateRootSize;
    private LayoutElementSnapshot? rootLayoutTemplate;
    private bool useNormalizedFallbackLayout;
    private Vector2 normalizedFallbackRootSize;
    private Vector2 charmCellSize = DefaultCharmCellSize;
    private Vector2 charmSpacing = DefaultCharmSpacing;
    private RectTransform? leftContentRoot;
    private RectTransform? notchIconContainer;
    private RectTransform? detailCostRow;
    private RectTransform? detailCostIconContainer;
    private RectTransform? equippedIconsRoot;
    private readonly List<Image> notchMeterIcons = new List<Image>(MaxNotchIcons);
    private readonly List<Image> detailCostIcons = new List<Image>(MaxNotchIcons);
    private readonly List<Image> equippedIcons = new List<Image>(MaxEquippedIcons);
    private static Sprite? notchLitSprite;
    private static Sprite? notchUnlitSprite;
    private static bool notchSpritesSearched;

    private struct CharmEntry
    {
        public ShadeCharmDefinition Definition;
        public ShadeCharmId Id;
        public RectTransform Root;
        public Image Icon;
        public Image Background;
        public GameObject? NewMarker;
        public Sprite? BaseSprite;
    }

    private struct NotchAssignment
    {
        public Sprite? Icon;
        public ShadeCharmDefinition? Definition;
        public ShadeCharmId? CharmId;
    }

    private static void SetTextValue(Text? text, TMP_Text? tmp, string value)
    {
        if (text != null)
        {
            text.text = value;
        }

        if (tmp != null)
        {
            tmp.text = value;
        }
    }

    private static string GetTextValue(Text? text, TMP_Text? tmp)
    {
        if (tmp != null)
        {
            return tmp.text ?? string.Empty;
        }

        if (text != null)
        {
            return text.text ?? string.Empty;
        }

        return string.Empty;
    }

    private static FontStyles ConvertFontStyle(FontStyle style) => style switch
    {
        FontStyle.Bold => FontStyles.Bold,
        FontStyle.Italic => FontStyles.Italic,
        FontStyle.BoldAndItalic => FontStyles.Bold | FontStyles.Italic,
        _ => FontStyles.Normal
    };

    private static TextAlignmentOptions ConvertAlignment(TextAnchor anchor) => anchor switch
    {
        TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
        TextAnchor.UpperCenter => TextAlignmentOptions.Top,
        TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
        TextAnchor.MiddleLeft => TextAlignmentOptions.Left,
        TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
        TextAnchor.MiddleRight => TextAlignmentOptions.Right,
        TextAnchor.LowerLeft => TextAlignmentOptions.BottomLeft,
        TextAnchor.LowerCenter => TextAlignmentOptions.Bottom,
        TextAnchor.LowerRight => TextAlignmentOptions.BottomRight,
        _ => TextAlignmentOptions.Center
    };

    private static RectTransform? ResolveRectTransform(Text? text, TMP_Text? tmp)
    {
        if (text != null)
        {
            return text.rectTransform;
        }

        return tmp != null ? tmp.rectTransform : null;
    }

    internal static RectTransform? ResolveTemplateRootRectTransform(InventoryPane? template)
    {
        if (template == null)
        {
            return null;
        }

        Transform templateTransform = template.transform;
        if (templateTransform == null)
        {
            return null;
        }

        RectTransform? rect = templateTransform as RectTransform;
        if (rect != null)
        {
            return rect;
        }

        try
        {
            rect = template.GetComponent<RectTransform>();
            if (rect != null)
            {
                return rect;
            }
        }
        catch
        {
        }

        RectTransform? matchByName = null;
        RectTransform? matchDirectChild = null;
        RectTransform? firstCandidate = null;
        RectTransform? scoredCandidate = null;
        int scoredCandidateValue = int.MinValue;

        RectTransform[]? rects = null;
        try
        {
            rects = template.GetComponentsInChildren<RectTransform>(true);
        }
        catch
        {
        }

        if (rects != null && rects.Length > 0)
        {
            foreach (var candidate in rects)
            {
                if (candidate == null)
                {
                    continue;
                }

                Transform candidateTransform = candidate.transform;
                if (candidateTransform == null)
                {
                    continue;
                }

                bool skipCandidate = false;
                try
                {
                    if (candidate != null)
                    {
                        // Skip any RectTransform that belongs to the shade pane we injected.
                        // Vanilla templates can reuse the "ShadeInventoryPane" object name, so we check
                        // for the actual component in the hierarchy instead of relying on string matches.
                        if (candidate.GetComponent<ShadeInventoryPane>() != null)
                        {
                            skipCandidate = true;
                        }
                        else
                        {
                            Transform? current = candidateTransform.parent;
                            while (current != null)
                            {
                                if (current.GetComponent<ShadeInventoryPane>() != null)
                                {
                                    skipCandidate = true;
                                    break;
                                }

                                current = current.parent;
                            }
                        }
                    }
                }
                catch
                {
                    skipCandidate = false;
                }

                if (skipCandidate)
                {
                    continue;
                }

                if (candidateTransform == templateTransform)
                {
                    rect = candidate;
                    break;
                }

                if (matchDirectChild == null && candidateTransform.parent == templateTransform)
                {
                    matchDirectChild = candidate;
                }

                try
                {
                    var paneComponent = candidate.GetComponent<InventoryPane>();
                    if (paneComponent != null && paneComponent == template)
                    {
                        rect = candidate;
                        break;
                    }
                }
                catch
                {
                }

                string? name = candidate.gameObject != null ? candidate.gameObject.name : null;
                if (name != null)
                {
                    string lower = name.ToLowerInvariant();

                    int candidateScore = ScoreTemplateRootCandidate(template, candidate, lower);
                    if (candidateScore > scoredCandidateValue)
                    {
                        scoredCandidate = candidate;
                        scoredCandidateValue = candidateScore;
                    }

                    if (matchByName == null)
                    {
                        if (lower.Contains("pane") || lower.Contains("panel"))
                        {
                            matchByName = candidate;
                        }
                    }
                    else if (matchByName != null)
                    {
                        // Prefer stronger string matches if available later in the iteration.
                        bool currentIsDescription = lower.Contains("description");
                        bool existingIsDescription = false;
                        string? existingName = matchByName.gameObject != null ? matchByName.gameObject.name : null;
                        if (!string.IsNullOrEmpty(existingName))
                        {
                            existingIsDescription = existingName.IndexOf("description", StringComparison.OrdinalIgnoreCase) >= 0;
                        }

                        if (!existingIsDescription && currentIsDescription)
                        {
                            // Keep the existing non-description match.
                        }
                        else if (existingIsDescription && !currentIsDescription)
                        {
                            matchByName = candidate;
                        }
                        else if (lower.Contains("shade") || lower.Contains("inventory"))
                        {
                            matchByName = candidate;
                        }
                    }
                }

                if (firstCandidate == null)
                {
                    firstCandidate = candidate;
                }
            }

        if (rect == null && scoredCandidate != null)
        {
            rect = scoredCandidate;
        }
        rect ??= matchByName;
        rect ??= matchDirectChild;
        rect ??= firstCandidate;
        }

        if (rect != null && rect.transform != templateTransform)
        {
            string rectName = rect.gameObject != null ? rect.gameObject.name : "<null>";
            string templateName = template.gameObject != null ? template.gameObject.name : template.name;
            LogMenuEvent(FormattableString.Invariant(
                $"Resolved template rect from child '{rectName}' for template '{templateName}'"));
        }

        if (rect == null)
        {
            string templateName = template.gameObject != null ? template.gameObject.name : template.name;
            LogMenuEvent(FormattableString.Invariant(
                $"ResolveTemplateRootRectTransform failed for template '{templateName}'"));
        }

        return rect;
    }

    private static int ScoreTemplateRootCandidate(InventoryPane template, RectTransform candidate, string lowerName)
    {
        if (candidate == null)
        {
            return int.MinValue;
        }

        int score = 0;

        try
        {
            if (candidate.transform.parent == template.transform)
            {
                score += 75;
            }
        }
        catch
        {
        }

        if (!string.IsNullOrEmpty(lowerName))
        {
            if (lowerName.Contains("shadeinventory"))
            {
                score += 500;
            }
            if (lowerName.Contains("inventorypane"))
            {
                score += 450;
            }
            else if (lowerName.Contains("inventory"))
            {
                score += 320;
            }

            if (lowerName.Contains("shade"))
            {
                score += 220;
            }

            if (lowerName.Contains("charm"))
            {
                score += 180;
            }

            if (lowerName.Contains("pane"))
            {
                score += 120;
            }

            if (lowerName.Contains("panel"))
            {
                score += 90;
            }

            if (lowerName.Contains("description"))
            {
                score -= 260;
            }

            if (lowerName.Contains("detail"))
            {
                score -= 200;
            }

            if (lowerName.Contains("grid"))
            {
                score -= 160;
            }

            if (lowerName.Contains("hint") || lowerName.Contains("status"))
            {
                score -= 120;
            }

            if (lowerName.Contains("button") || lowerName.Contains("prompt"))
            {
                score -= 200;
            }
        }

        int childCount = 0;
        try
        {
            childCount = candidate.childCount;
        }
        catch
        {
            childCount = 0;
        }

        if (childCount >= 6)
        {
            score += 110;
        }
        else if (childCount >= 3)
        {
            score += 70;
        }
        else if (childCount >= 1)
        {
            score += 30;
        }

        bool hasLayoutGroup = false;
        try
        {
            hasLayoutGroup = candidate.GetComponent<LayoutGroup>() != null;
        }
        catch
        {
            hasLayoutGroup = false;
        }

        if (hasLayoutGroup)
        {
            score += 60;
        }

        bool hasDirectGrid = false;
        try
        {
            hasDirectGrid = candidate.GetComponent<GridLayoutGroup>() != null;
        }
        catch
        {
            hasDirectGrid = false;
        }

        if (hasDirectGrid)
        {
            score -= 140;
        }

        if (HasGridLayoutDescendant(candidate, hasDirectGrid))
        {
            score += 240;
        }

        try
        {
            Vector2 size = candidate.rect.size;
            if (Mathf.Abs(size.x) >= 12f && Mathf.Abs(size.y) >= 12f)
            {
                score += 45;
            }
        }
        catch
        {
        }

        return score;
    }

    private static bool HasGridLayoutDescendant(RectTransform candidate, bool excludeSelf)
    {
        if (candidate == null)
        {
            return false;
        }

        GridLayoutGroup[]? grids = null;
        try
        {
            grids = candidate.GetComponentsInChildren<GridLayoutGroup>(true);
        }
        catch
        {
            grids = null;
        }

        if (grids == null || grids.Length == 0)
        {
            return false;
        }

        foreach (var grid in grids)
        {
            if (grid == null)
            {
                continue;
            }

            if (excludeSelf && grid.transform == candidate.transform)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    internal static string FormatVector2(Vector2 value)
    {
        return FormattableString.Invariant($"({value.x:0.##}, {value.y:0.##})");
    }

    private static string FormatRectOffset(RectOffset offset)
    {
        if (offset == null)
        {
            return "<null>";
        }

        return FormattableString.Invariant($"(l:{offset.left}, r:{offset.right}, t:{offset.top}, b:{offset.bottom})");
    }

    internal static bool HasSufficientRectSize(RectTransform? rect)
    {
        if (rect == null)
        {
            return false;
        }

        try
        {
            Vector2 size = rect.rect.size;
            return Mathf.Abs(size.x) >= MinRootSizeThreshold && Mathf.Abs(size.y) >= MinRootSizeThreshold;
        }
        catch
        {
            return false;
        }
    }

    internal static bool HasUsableTemplateRect(RectTransform? rect)
    {
        if (rect == null)
        {
            return false;
        }

        if (!HasSufficientRectSize(rect))
        {
            return false;
        }

        Vector2 size = rect.rect.size;
        float width = Mathf.Abs(size.x);
        float height = Mathf.Abs(size.y);
        float minDimension = Mathf.Min(width, height);
        float area = width * height;

        if (minDimension < MinTemplateCopyDimension && area < MinTemplateCopyArea)
        {
            return false;
        }

        return true;
    }

    private static bool Approximately(Vector2 a, Vector2 b, float tolerance = 0.001f)
    {
        return Mathf.Abs(a.x - b.x) <= tolerance && Mathf.Abs(a.y - b.y) <= tolerance;
    }

    private static string DescribeLayoutComponents(RectTransform rect)
    {
        var details = new List<string>();

        var layoutElement = rect.GetComponent<LayoutElement>();
        if (layoutElement != null)
        {
            var min = new Vector2(layoutElement.minWidth, layoutElement.minHeight);
            var preferred = new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight);
            var flexible = new Vector2(layoutElement.flexibleWidth, layoutElement.flexibleHeight);
            details.Add(FormattableString.Invariant(
                $"LayoutElement(min={FormatVector2(min)}, preferred={FormatVector2(preferred)}, flexible={FormatVector2(flexible)}, ignore={layoutElement.ignoreLayout}, priority={layoutElement.layoutPriority})"));
        }

        var grid = rect.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            details.Add(FormattableString.Invariant(
                $"GridLayoutGroup(cellSize={FormatVector2(grid.cellSize)}, spacing={FormatVector2(grid.spacing)}, startCorner={grid.startCorner}, startAxis={grid.startAxis}, constraint={grid.constraint}, count={grid.constraintCount}, alignment={grid.childAlignment}, padding={FormatRectOffset(grid.padding)})"));
        }
        else
        {
            var layoutGroup = rect.GetComponent<LayoutGroup>();
            if (layoutGroup != null)
            {
                string summary = FormattableString.Invariant(
                    $"LayoutGroup<{layoutGroup.GetType().Name}>(alignment={layoutGroup.childAlignment}, padding={FormatRectOffset(layoutGroup.padding)})");
                if (layoutGroup is HorizontalOrVerticalLayoutGroup hv)
                {
                    summary += FormattableString.Invariant(
                        $", spacing={hv.spacing}, childCtrl=({hv.childControlWidth},{hv.childControlHeight}), childForce=({hv.childForceExpandWidth},{hv.childForceExpandHeight})");
                }

                details.Add(summary);
            }
        }

        var fitter = rect.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            details.Add($"ContentSizeFitter(h={fitter.horizontalFit}, v={fitter.verticalFit})");
        }

        return details.Count > 0 ? string.Join("; ", details) : "<none>";
    }

    internal static void LogRectTransformHierarchy(RectTransform? start, string context)
    {
        if (start == null)
        {
            LogMenuEvent($"Layout diagnostics skipped for {context}: rect null");
            return;
        }

        Transform? current = start;
        int depth = 0;
        while (current != null)
        {
            if (current is RectTransform rect)
            {
                string details = DescribeLayoutComponents(rect);
                string name = rect.gameObject != null ? rect.gameObject.name : "<null>";
                bool active = rect.gameObject != null && rect.gameObject.activeInHierarchy;
                LogMenuEvent(FormattableString.Invariant(
                    $"LayoutDiag[{context}][{depth}] name='{name}' active={active} anchorMin={FormatVector2(rect.anchorMin)} anchorMax={FormatVector2(rect.anchorMax)} pivot={FormatVector2(rect.pivot)} offsetMin={FormatVector2(rect.offsetMin)} offsetMax={FormatVector2(rect.offsetMax)} anchoredPos={FormatVector2(rect.anchoredPosition)} size={FormatVector2(rect.rect.size)} layout={details}"));
            }
            else
            {
                string name = current.gameObject != null ? current.gameObject.name : "<null>";
                LogMenuEvent(FormattableString.Invariant($"LayoutDiag[{context}][{depth}] name='{name}' (no RectTransform)"));
            }

            current = current.parent;
            depth++;
        }
    }

    internal static void LogMenuEvent(string message)
    {
        if (!ModConfig.Instance.logMenu)
        {
            return;
        }

        try
        {
            Debug.Log("[ShadeInventory] " + message);
        }
        catch
        {
        }
    }

    public override void Awake()
    {
        base.Awake();
        SubscribeInput();
    }

    private void OnEnable()
    {
        EnsureBuilt();
        labelPulseTimer = 0f;

        if (IsPaneActive)
        {
            isActive = true;
            UpdateInventoryBinding(true);
            ApplyOverlayVisibility(true);
        }
        else
        {
            isActive = false;
            UpdateInventoryBinding(false);
            ApplyOverlayVisibility(false);
        }

        LogMenuEvent(FormattableString.Invariant(
            $"OnEnable: active={isActive} entries={entries.Count} inventoryNull={inventory == null}"));
    }

    private void OnDisable()
    {
        UpdateInventoryBinding(false);
        isActive = false;
        labelPulseTimer = 0f;
        ApplyOverlayVisibility(false);
        LogMenuEvent("OnDisable");
    }

    private void OnDestroy()
    {
        UpdateInventoryBinding(false);
        DetachPaneList();

        if (fallbackSprite != null)
        {
            if (fallbackSprite.texture != null)
            {
                Destroy(fallbackSprite.texture);
            }
            Destroy(fallbackSprite);
            fallbackSprite = null;
        }

        if (overlayCanvasObject != null)
        {
            try
            {
                Destroy(overlayCanvasObject);
            }
            catch
            {
            }

            overlayCanvasObject = null;
            overlayRoot = null;
            overlayCanvas = null;
            overlayCanvasScaler = null;
            overlayRaycaster = null;
            canvasGroup = null!;
        }
    }

    public override void PaneStart()
    {
        base.PaneStart();
        EnsureBuilt();
        labelPulseTimer = 0f;
        isActive = true;
        UpdateInventoryBinding(true);
        ApplyOverlayVisibility(true);
        RefreshAll();
        UpdateParentListLabel();
        ForceLayoutRebuild();
        LogMenuEvent($"PaneStart: entries={entries.Count}, inventoryNull={inventory == null}");
    }

    public override void PaneEnd()
    {
        UpdateInventoryBinding(false);
        isActive = false;
        labelPulseTimer = 0f;
        ApplyOverlayVisibility(false);
        LogMenuEvent("PaneEnd");
        base.PaneEnd();
    }

    internal void SetDisplayLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return;
        }

        displayLabel = label;
        ShadeInventoryPaneIntegration.SyncDisplayName(this, displayLabel);
        SetTextValue(titleText, titleTextTMP, label);
        UpdateParentListLabel();
    }

    internal string DisplayLabel => displayLabel;

    internal void HandleSubmit()
    {
        EnsureBuilt();
        if (!isActive || inventory == null || entries.Count == 0)
        {
            return;
        }

        var entry = entries[Mathf.Clamp(selectedIndex, 0, entries.Count - 1)];
        var id = entry.Id;
        if (!inventory.IsOwned(id))
        {
            SetTextValue(statusText, statusTextTMP, "This charm has not been unlocked yet.");
            return;
        }

        string message;
        bool success = inventory.IsEquipped(id)
            ? inventory.TryUnequip(id, out message)
            : inventory.TryEquip(id, out message);

        SetTextValue(statusText, statusTextTMP, message);
        if (success)
        {
            LegacyHelper.RequestShadeLoadoutRecompute();
            ShadeSettingsMenu.NotifyCharmLoadoutChanged();
        }

        RefreshEntryStates();
        UpdateNotchMeter();
        UpdateDetailPanel();
    }

    private void HandleInventoryChanged()
    {
        RefreshAll();
    }

    private void UpdateParentListLabel()
    {
        bool changed = false;
        try
        {
            var parentList = GetComponentInParent<InventoryPaneList>();
            if (parentList == null)
            {
                return;
            }

            string currentTitle = GetTextValue(titleText, titleTextTMP);
            if (!string.Equals(currentTitle, displayLabel, StringComparison.Ordinal))
            {
                SetTextValue(titleText, titleTextTMP, displayLabel);
                changed = true;
            }

            if (ShadeInventoryPaneIntegration.TrySetCurrentPaneLabel(parentList, displayLabel))
            {
                changed = true;
            }

            bool foundDisplayLabel = false;

            var texts = parentList.GetComponentsInChildren<Text>(true);
            foreach (var text in texts)
            {
                if (text == null)
                {
                    continue;
                }

                string current = text.text ?? string.Empty;
                if (string.Equals(current, displayLabel, StringComparison.OrdinalIgnoreCase))
                {
                    foundDisplayLabel = true;
                    break;
                }

                if (string.Equals(current, "??/??", StringComparison.OrdinalIgnoreCase) ||
                    string.IsNullOrWhiteSpace(current))
                {
                    text.text = displayLabel;
                    changed = true;
                    foundDisplayLabel = true;
                    break;
                }
            }

            if (!foundDisplayLabel)
            {
                var tmpTexts = parentList.GetComponentsInChildren<TMP_Text>(true);
                foreach (var tmp in tmpTexts)
                {
                    if (tmp == null)
                    {
                        continue;
                    }

                    string current = tmp.text ?? string.Empty;
                    if (string.Equals(current, displayLabel, StringComparison.OrdinalIgnoreCase))
                    {
                        foundDisplayLabel = true;
                        break;
                    }

                    if (string.Equals(current, "??/??", StringComparison.OrdinalIgnoreCase) ||
                        string.IsNullOrWhiteSpace(current))
                    {
                        tmp.text = displayLabel;
                        changed = true;
                        foundDisplayLabel = true;
                        break;
                    }
                }
            }
        }
        catch
        {
        }

        if (changed)
        {
            LogMenuEvent(FormattableString.Invariant($"UpdateParentListLabel -> '{displayLabel}'"));
        }

    }

    private void SubscribeInput()
    {
        OnInputLeft += () => MoveSelectionHorizontal(-1);
        OnInputRight += () => MoveSelectionHorizontal(1);
        OnInputUp += () => MoveSelectionVertical(-1);
        OnInputDown += () => MoveSelectionVertical(1);
    }

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
        bodyFontColor = Color.white;
        headerFontColor = Color.white;

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

            var texts = template.GetComponentsInChildren<Text>(true);
            if (texts != null && texts.Length > 0)
            {
                var ordered = texts
                    .Where(t => t != null && t.font != null)
                    .OrderBy(t => t.fontSize)
                    .ToArray();

                if (ordered.Length > 0)
                {
                    int bodyIndex = Mathf.Clamp(ordered.Length > 1 ? ordered.Length / 2 : 0, 0, ordered.Length - 1);
                    var bodySample = ordered[bodyIndex];
                    if (bodySample != null && bodySample.color.a > 0f)
                    {
                        bodyFontColor = bodySample.color;
                    }
                }

                var headerSample = ordered.LastOrDefault();
                if (headerSample != null && headerSample.color.a > 0f)
                {
                    headerFontColor = headerSample.color;
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

    private void EnsureBuilt()
    {
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
        ApplyOverlayVisibility(false);
        isActive = false;
        labelPulseTimer = 0f;
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

    private void ResolveCharmLayoutMetrics(int entryCount)
    {
        Vector2 cell = DefaultCharmCellSize;
        Vector2 spacing = DefaultCharmSpacing;

        if (gridLayoutTemplate.HasValue)
        {
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

    private void LayoutCharmEntries()
    {
        if (gridRoot == null || leftContentRoot == null)
        {
            return;
        }

        try
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(leftContentRoot);
        }
        catch
        {
        }

        ResolveCharmLayoutMetrics(entries.Count);

        entryGridPositions.Clear();
        entryCenterXs.Clear();

        if (entries.Count == 0)
        {
            gridRoot.sizeDelta = Vector2.zero;
            gridRoot.anchoredPosition = Vector2.zero;
            return;
        }

        int columns = Mathf.Max(1, DetermineCharmColumnCount(entries.Count));

        var rowAssignments = new int[entries.Count];
        var columnAssignments = new int[entries.Count];
        var rowCounts = new int[CharmRows];

        for (int i = 0; i < entries.Count; i++)
        {
            int row = Mathf.Min(i / columns, CharmRows - 1);
            int column = rowCounts[row];
            rowCounts[row] = column + 1;
            rowAssignments[i] = row;
            columnAssignments[i] = column;
        }

        int usedRows = 0;
        for (int row = 0; row < CharmRows; row++)
        {
            if (rowCounts[row] > 0)
            {
                usedRows = row + 1;
            }
        }

        if (usedRows <= 0)
        {
            usedRows = 1;
        }

        float strideX = charmCellSize.x + charmSpacing.x;
        float strideY = charmCellSize.y + charmSpacing.y;
        float halfStrideX = strideX * RowOffsetFactor;

        float maxWidth = 0f;
        for (int row = 0; row < usedRows; row++)
        {
            int countInRow = rowCounts[row];
            if (countInRow <= 0)
            {
                continue;
            }

            float rowWidth = (countInRow - 1) * strideX + charmCellSize.x;
            if ((row & 1) == 1)
            {
                rowWidth += halfStrideX;
            }

            if (rowWidth > maxWidth)
            {
                maxWidth = rowWidth;
            }
        }

        if (maxWidth <= 0f)
        {
            maxWidth = charmCellSize.x;
        }

        float totalHeight = usedRows * charmCellSize.y + Mathf.Max(usedRows - 1, 0) * charmSpacing.y;

        Vector2 parentSize = leftContentRoot.rect.size;
        if (parentSize.x < MinRootSizeThreshold || parentSize.y < MinRootSizeThreshold)
        {
            Vector2 fallback = normalizedFallbackRootSize.sqrMagnitude > 0f ? normalizedFallbackRootSize : DefaultStandaloneRootSize;
            parentSize = new Vector2(Mathf.Max(fallback.x * 0.6f, maxWidth), Mathf.Max(fallback.y * 0.6f, totalHeight));
        }

        float offsetX = Mathf.Max(0f, parentSize.x * 0.15f);
        float offsetY = Mathf.Max(0f, parentSize.y * 0.15f);
        float maxOffsetX = Mathf.Max(0f, parentSize.x - maxWidth);
        float maxOffsetY = Mathf.Max(0f, parentSize.y - totalHeight);
        offsetX = Mathf.Min(offsetX, maxOffsetX);
        offsetY = Mathf.Min(offsetY, maxOffsetY);

        gridRoot.sizeDelta = new Vector2(maxWidth, totalHeight);
        gridRoot.anchoredPosition = new Vector2(offsetX, offsetY);

        for (int i = 0; i < entries.Count; i++)
        {
            int row = rowAssignments[i];
            int column = columnAssignments[i];

            float x = (row % 2 == 1 ? halfStrideX : 0f) + column * strideX;
            float y = (usedRows - row - 1) * strideY;

            var entry = entries[i];
            RectTransform? rect = entry.Root;
            Vector2 anchored = new Vector2(x + charmCellSize.x * 0.5f, y + charmCellSize.y * 0.5f);
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = charmCellSize;
                rect.anchoredPosition = anchored;
            }

            entryGridPositions.Add(new Vector2Int(row, column));
            entryCenterXs.Add(anchored.x);
        }
    }

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

    private RectTransform CreateHighlightRect(RectTransform parent)
    {
        var highlightRect = new GameObject("Highlight", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        highlightRect.gameObject.layer = parent.gameObject.layer;
        highlightRect.SetParent(parent, false);
        var highlightImage = highlightRect.GetComponent<Image>();
        if (highlightSpriteTemplate != null)
        {
            highlightImage.sprite = highlightSpriteTemplate;
            highlightImage.type = Image.Type.Sliced;
            highlightImage.color = highlightColor;
        }
        else
        {
            highlightImage.color = highlightColor;
        }
        highlightImage.raycastTarget = false;
        highlightRect.gameObject.SetActive(false);
        return highlightRect;
    }

    public void ForceLayoutRebuild()
    {
        EnsureBuilt();

        EnsureRootSizing();

        var rootRect = overlayRoot ?? transform as RectTransform;
        if (rootRect == null)
        {
            LogMenuEvent("ForceLayoutRebuild skipped: no root RectTransform available");
            return;
        }

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

        LayoutRebuilder.ForceRebuildLayoutImmediate(rootRect);

        LayoutCharmEntries();

        Vector2 rootSize = rootRect.rect.size;
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
        if (wasActive && canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
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

        if (canvasGroup == null || canvasGroup.gameObject != rootRect.gameObject)
        {
            canvasGroup = rootRect.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = rootRect.gameObject.AddComponent<CanvasGroup>();
            }
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

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

        var equippedLabel = CreateText("EquippedLabel", leftContentRoot, FontStyle.Normal, 34, TextAnchor.UpperLeft, out _);
        var equippedLabelRect = ResolveRectTransform(equippedLabel, null);
        if (equippedLabelRect != null)
        {
            equippedLabelRect.anchorMin = new Vector2(0f, 1f);
            equippedLabelRect.anchorMax = new Vector2(0f, 1f);
            equippedLabelRect.pivot = new Vector2(0f, 1f);
            equippedLabelRect.anchoredPosition = new Vector2(16f, -28f);
            equippedLabelRect.sizeDelta = new Vector2(360f, 40f);
        }
        SetTextValue(equippedLabel, null, "Equipped");

        equippedIconsRoot = new GameObject("EquippedIcons", typeof(RectTransform)).GetComponent<RectTransform>();
        equippedIconsRoot.gameObject.layer = leftContentRoot.gameObject.layer;
        equippedIconsRoot.SetParent(leftContentRoot, false);
        equippedIconsRoot.anchorMin = new Vector2(0f, 1f);
        equippedIconsRoot.anchorMax = new Vector2(1f, 1f);
        equippedIconsRoot.pivot = new Vector2(0f, 1f);
        equippedIconsRoot.anchoredPosition = new Vector2(16f, -96f);
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
            notchLabelRect.anchoredPosition = new Vector2(16f, -236f);
            notchLabelRect.sizeDelta = new Vector2(220f, 36f);
        }
        SetTextValue(notchText, notchTextTMP, "Notches");

        notchIconContainer = new GameObject("NotchIcons", typeof(RectTransform)).GetComponent<RectTransform>();
        notchIconContainer.gameObject.layer = leftContentRoot.gameObject.layer;
        notchIconContainer.SetParent(leftContentRoot, false);
        notchIconContainer.anchorMin = new Vector2(0f, 1f);
        notchIconContainer.anchorMax = new Vector2(1f, 1f);
        notchIconContainer.pivot = new Vector2(0f, 1f);
        notchIconContainer.anchoredPosition = new Vector2(16f, -284f);
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
        if (detailTitleRect != null)
        {
            if (useNormalizedFallbackLayout)
            {
                detailTitleRect.anchorMin = new Vector2(0f, 0.84f);
                detailTitleRect.anchorMax = new Vector2(1f, 1f);
                detailTitleRect.pivot = new Vector2(0.5f, 1f);
                detailTitleRect.offsetMin = Vector2.zero;
                detailTitleRect.offsetMax = Vector2.zero;
            }
            else
            {
                detailTitleRect.anchorMin = new Vector2(0f, 0.84f);
                detailTitleRect.anchorMax = new Vector2(1f, 1f);
                detailTitleRect.pivot = new Vector2(0.5f, 1f);
                detailTitleRect.offsetMin = new Vector2(24f, 0f);
                detailTitleRect.offsetMax = new Vector2(-24f, -4f);
            }
        }
        SetTextValue(detailTitleText, detailTitleTextTMP, displayLabel);

        detailCostRow = new GameObject("CostRow", typeof(RectTransform)).GetComponent<RectTransform>();
        detailCostRow.gameObject.layer = detailRoot.gameObject.layer;
        detailCostRow.SetParent(detailRoot, false);
        if (useNormalizedFallbackLayout)
        {
            detailCostRow.anchorMin = new Vector2(0f, 0.76f);
            detailCostRow.anchorMax = new Vector2(1f, 0.84f);
            detailCostRow.pivot = new Vector2(0.5f, 1f);
            detailCostRow.offsetMin = Vector2.zero;
            detailCostRow.offsetMax = Vector2.zero;
        }
        else
        {
            detailCostRow.anchorMin = new Vector2(0f, 0.76f);
            detailCostRow.anchorMax = new Vector2(1f, 0.84f);
            detailCostRow.pivot = new Vector2(0.5f, 1f);
            detailCostRow.offsetMin = new Vector2(24f, 4f);
            detailCostRow.offsetMax = new Vector2(-24f, -2f);
        }

        var costLayout = detailCostRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        costLayout.spacing = 12f;
        costLayout.childAlignment = TextAnchor.MiddleCenter;
        costLayout.childControlWidth = false;
        costLayout.childControlHeight = false;
        costLayout.childForceExpandWidth = false;
        costLayout.childForceExpandHeight = false;
        costLayout.padding = new RectOffset();

        detailCostLabel = CreateText("CostLabel", detailCostRow, FontStyle.Normal, 28, TextAnchor.MiddleCenter, out _);
        var detailCostLabelRect = ResolveRectTransform(detailCostLabel, null);
        if (detailCostLabelRect != null)
        {
            detailCostLabelRect.anchorMin = new Vector2(0.5f, 0.5f);
            detailCostLabelRect.anchorMax = new Vector2(0.5f, 0.5f);
            detailCostLabelRect.pivot = new Vector2(0.5f, 0.5f);
            detailCostLabelRect.sizeDelta = new Vector2(120f, 32f);
        }
        SetTextValue(detailCostLabel, null, "Cost");

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
        costIconsElement.minHeight = 32f;
        BuildIconPool(detailCostIconContainer, detailCostIcons, MaxNotchIcons, "CostIcon", new Vector2(32f, 32f));

        descriptionText = CreateText("Description", detailRoot, FontStyle.Normal, 26, TextAnchor.UpperLeft, out descriptionTextTMP);
        var descRect = ResolveRectTransform(descriptionText, descriptionTextTMP);
        if (descRect != null)
        {
            if (useNormalizedFallbackLayout)
            {
                descRect.anchorMin = new Vector2(0f, 0.34f);
                descRect.anchorMax = new Vector2(1f, 0.74f);
                descRect.pivot = new Vector2(0f, 1f);
                float margin = ComputeNormalizedMargin(normalizedFallbackRootSize.x, 0.03f);
                descRect.offsetMin = new Vector2(margin, 0f);
                descRect.offsetMax = new Vector2(-margin, 0f);
            }
            else
            {
                descRect.anchorMin = new Vector2(0f, 0.32f);
                descRect.anchorMax = new Vector2(1f, 0.74f);
                descRect.offsetMin = new Vector2(18f, 0f);
                descRect.offsetMax = new Vector2(-24f, -4f);
            }
        }
        if (descriptionText != null)
        {
            descriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
            descriptionText.verticalOverflow = VerticalWrapMode.Overflow;
            descriptionText.lineSpacing = 1.1f;
            descriptionText.fontSize = 26;
        }
        else if (descriptionTextTMP != null)
        {
            descriptionTextTMP.lineSpacing = 1.1f;
            descriptionTextTMP.textWrappingMode = TextWrappingModes.Normal;
            descriptionTextTMP.fontSize = 26f;
        }

        statusText = CreateText("Status", detailRoot, FontStyle.Italic, 28, TextAnchor.UpperLeft, out statusTextTMP);
        var statusRect = ResolveRectTransform(statusText, statusTextTMP);
        if (statusRect != null)
        {
            if (useNormalizedFallbackLayout)
            {
                statusRect.anchorMin = new Vector2(0f, 0.18f);
                statusRect.anchorMax = new Vector2(1f, 0.34f);
                statusRect.pivot = new Vector2(0f, 1f);
                statusRect.offsetMin = Vector2.zero;
                statusRect.offsetMax = Vector2.zero;
            }
            else
            {
                statusRect.anchorMin = new Vector2(0f, 0.18f);
                statusRect.anchorMax = new Vector2(1f, 0.34f);
                statusRect.offsetMin = new Vector2(0f, 4f);
                statusRect.offsetMax = new Vector2(-6f, 0f);
            }
        }

        hintText = CreateText("Hint", detailRoot, FontStyle.Normal, 24, TextAnchor.UpperLeft, out hintTextTMP);
        var hintRect = ResolveRectTransform(hintText, hintTextTMP);
        if (hintRect != null)
        {
            if (useNormalizedFallbackLayout)
            {
                hintRect.anchorMin = new Vector2(0f, 0f);
                hintRect.anchorMax = new Vector2(1f, 0.18f);
                hintRect.pivot = new Vector2(0f, 0f);
                hintRect.offsetMin = Vector2.zero;
                hintRect.offsetMax = Vector2.zero;
            }
            else
            {
                hintRect.anchorMin = new Vector2(0f, 0f);
                hintRect.anchorMax = new Vector2(1f, 0.18f);
                hintRect.offsetMin = new Vector2(0f, 4f);
                hintRect.offsetMax = new Vector2(-6f, 0f);
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
        var text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontStyle = style;
        text.fontSize = size;
        text.alignment = anchor;
        text.color = useHeaderFont ? headerFontColor : bodyFontColor;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
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
        bool highlightEquippedSlots)
    {
        EnsureNotchSprites();

        Sprite lit = notchLitSprite ?? ResolveLockedCharmSprite() ?? GetFallbackSprite();
        Sprite empty = notchUnlitSprite ?? ResolveLockedCharmSprite() ?? lit;

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
                    image.color = Color.white;
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
                image.color = highlightEmpty ? Color.white : new Color(1f, 1f, 1f, 0.55f);
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
            return;
        }

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
            return;
        }

        var equippedDefs = inv.GetEquippedDefinitions()?.Where(def => def != null).ToList() ?? new List<ShadeCharmDefinition>();
        int count = Mathf.Clamp(equippedDefs.Count, 0, equippedIcons.Count);

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
                Sprite sprite = def?.Icon ?? GetFallbackSprite();
                if (sprite != null)
                {
                    image.sprite = sprite;
                    image.enabled = true;
                    image.color = Color.white;
                    image.gameObject.SetActive(true);
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

    private void SelectIndex(int index)
    {
        EnsureBuilt();
        if (entries.Count == 0)
        {
            return;
        }

        selectedIndex = Mathf.Clamp(index, 0, entries.Count - 1);
        var entry = entries[selectedIndex];
        var highlightRect = EnsureHighlightRect();
        RectTransform? entryRoot = entry.Root;
        if (highlightRect != null && entryRoot != null)
        {
            highlightRect.gameObject.SetActive(true);
            highlightRect.SetParent(entryRoot, false);
            highlightRect.SetAsFirstSibling();
            highlightRect.anchorMin = Vector2.zero;
            highlightRect.anchorMax = Vector2.one;
            highlightRect.offsetMin = Vector2.zero;
            highlightRect.offsetMax = Vector2.zero;
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
        for (int i = 0; i < definitions.Count; i++)
        {
            var entry = entries[i];
            entry.Definition = definitions[i];
            entry.Id = definitions[i].EnumId ?? ShadeCharmId.WaywardCompass;
            var sprite = definitions[i].Icon ?? GetFallbackSprite();
            entry.BaseSprite = sprite;
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

    private void HandleStateChanged()
    {
        RefreshEntryStates();
        UpdateNotchMeter();
        UpdateDetailPanel();
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
        float iconDimension = Mathf.Max(Mathf.Min(charmCellSize.x, charmCellSize.y) * CharmIconSizeMultiplier, 48f);
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
            BaseSprite = null
        };
    }

    private void RefreshEntryStates()
    {
        EnsureBuilt();
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
                if (!owned && lockedSprite != null)
                {
                    entry.Icon.sprite = lockedSprite;
                    entry.Icon.enabled = true;
                    entry.Icon.color = LockedIconColor;
                }
                else
                {
                    entry.Icon.sprite = entry.BaseSprite;
                    entry.Icon.enabled = entry.Icon.sprite != null;

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
        SetTextValue(notchText, notchTextTMP, "Notches");

        var inv = inventory ?? ShadeRuntime.Charms;
        if (inv == null)
        {
            RenderNotchMeter(notchMeterIcons, Array.Empty<NotchAssignment>(), 0, 0, null, null, false);
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
            highlightCost = Mathf.Max(selectedDefinition?.NotchCost ?? 0, 0);
            highlightEquippedSlots = inv.IsEquipped(selectedEntry.Id);
        }

        RenderNotchMeter(notchMeterIcons, assignments, capacity, highlightCost, selectedDefinition, selectedId, highlightEquippedSlots);
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

    private static string DescribeShadeSlashBinding()
    {
        try
        {
            string primary = ShadeInput.DescribeBindingOption(ShadeInput.GetBindingOption(ShadeAction.Nail, secondary: false));
            if (IsBindingLabelMeaningful(primary))
            {
                return primary;
            }

            string secondary = ShadeInput.DescribeBindingOption(ShadeInput.GetBindingOption(ShadeAction.Nail, secondary: true));
            if (IsBindingLabelMeaningful(secondary))
            {
                return secondary;
            }
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

    private void UpdateDetailPanel()
    {
        EnsureBuilt();
        if (entries.Count == 0 || inventory == null)
        {
            SetTextValue(detailTitleText, detailTitleTextTMP, displayLabel);
            SetTextValue(descriptionText, descriptionTextTMP, "Collect shade charms to unlock new abilities for your companion.");
            SetTextValue(statusText, statusTextTMP, string.Empty);
            if (detailCostRow != null)
            {
                detailCostRow.gameObject.SetActive(false);
            }
            RenderNotchStrip(detailCostIcons, 0, 0, false);
            return;
        }

        var entry = entries[Mathf.Clamp(selectedIndex, 0, entries.Count - 1)];
        var definition = entry.Definition;
        SetTextValue(detailTitleText, detailTitleTextTMP, definition?.DisplayName ?? displayLabel);
        SetTextValue(descriptionText, descriptionTextTMP, definition?.Description ?? string.Empty);

        bool owned = inventory.IsOwned(entry.Id);
        bool equipped = inventory.IsEquipped(entry.Id);
        bool broken = inventory.IsBroken(entry.Id);
        int notchCost = definition?.NotchCost ?? 0;
        string bindingLabel = DescribeShadeSlashBinding();
        string equipPrompt = BuildEquipPrompt(bindingLabel);
        string unequipPrompt = BuildUnequipPrompt(bindingLabel);
        string status;
        if (!owned)
        {
            status = "This charm has not been discovered.";
        }
        else if (broken)
        {
            status = "Charm is broken. Rest at a bench to repair it.";
        }
        else if (equipped)
        {
            status = FormattableString.Invariant($"Equipped. {unequipPrompt}");
        }
        else if (inventory.UsedNotches + notchCost > inventory.NotchCapacity)
        {
            status = "Not enough notches available.";
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
            status = $"No notch cost.\\n{status}";
        }

        SetTextValue(statusText, statusTextTMP, status);
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

internal sealed class SimpleCanvasNestedFadeGroup : NestedFadeGroupBase
{
    [SerializeField]
    private CanvasGroup canvasGroup = null!;

    protected override void GetMissingReferences()
    {
        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    protected override void OnAlphaChanged(float alpha)
    {
        if (!canvasGroup)
        {
            return;
        }

        canvasGroup.alpha = alpha;
        bool active = alpha > 0.001f;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }
}

internal static class ShadeInventoryPaneIntegration
{
    private static readonly AccessTools.FieldRef<InventoryPaneList, InventoryPane[]> PanesField =
        AccessTools.FieldRefAccess<InventoryPaneList, InventoryPane[]>("panes");

    private static readonly AccessTools.FieldRef<InventoryPane, Sprite> ListIconField =
        AccessTools.FieldRefAccess<InventoryPane, Sprite>("listIcon");

    private static readonly AccessTools.FieldRef<InventoryPane, PlayerDataTest> PlayerDataTestField =
        AccessTools.FieldRefAccess<InventoryPane, PlayerDataTest>("playerDataTest");

    private static readonly AccessTools.FieldRef<InventoryPane, string> HasNewPdField =
        AccessTools.FieldRefAccess<InventoryPane, string>("hasNewPDBool");

    private static readonly AccessTools.FieldRef<InventoryPane, LocalisedString> DisplayNameField =
        AccessTools.FieldRefAccess<InventoryPane, LocalisedString>("displayName");

    private static readonly AccessTools.FieldRef<InventoryPaneList, InventoryPaneListDisplay> PaneListDisplayField =
        AccessTools.FieldRefAccess<InventoryPaneList, InventoryPaneListDisplay>("paneListDisplay");

    private static readonly AccessTools.FieldRef<InventoryPaneList, string> NextPaneOpenField =
        AccessTools.FieldRefAccess<InventoryPaneList, string>("nextPaneOpen");

    private static readonly FieldInfo CurrentPaneTextFieldInfo = AccessTools.Field(typeof(InventoryPaneList), "currentPaneText");

    private static readonly PropertyInfo UnlockedPaneCountProperty =
        AccessTools.Property(typeof(InventoryPaneList), "UnlockedPaneCount");

    private static readonly MethodInfo GetPaneIndexMethod =
        AccessTools.Method(typeof(InventoryPaneList), "GetPaneIndex", new[] { typeof(string) });

    private static readonly AccessTools.FieldRef<InventoryPaneInput, InventoryPaneList.PaneTypes> PaneControlField =
        AccessTools.FieldRefAccess<InventoryPaneInput, InventoryPaneList.PaneTypes>("paneControl");

    private static readonly FieldInfo AllowHorizontalField = AccessTools.Field(typeof(InventoryPaneInput), "allowHorizontalSelection");
    private static readonly FieldInfo AllowVerticalField = AccessTools.Field(typeof(InventoryPaneInput), "allowVerticalSelection");
    private static readonly FieldInfo AllowRepeatField = AccessTools.Field(typeof(InventoryPaneInput), "allowRepeat");
    private static readonly FieldInfo AllowRepeatSubmitField = AccessTools.Field(typeof(InventoryPaneInput), "allowRepeatSubmit");
    private static readonly FieldInfo AllowRightStickField = AccessTools.Field(typeof(InventoryPaneInput), "allowRightStickSpeed");
    private static readonly FieldInfo PaneField = AccessTools.Field(typeof(InventoryPaneInput), "pane");
    private static readonly FieldInfo PaneListField = AccessTools.Field(typeof(InventoryPaneInput), "paneList");

    private const float ListIconScaleFactor = 0.5625f;

    private static Sprite? cachedListIcon;
    private static Sprite? cachedListIconSource;

    private static void AssignListIcon(ShadeInventoryPane shadePane, Sprite? icon)
    {
        if (shadePane == null || icon == null)
        {
            return;
        }

        Sprite? scaled = CreateScaledListIcon(icon);
        if (scaled != null)
        {
            ListIconField(shadePane) = scaled;
        }
        else
        {
            ListIconField(shadePane) = icon;
        }
    }

    private static Sprite? CreateScaledListIcon(Sprite icon)
    {
        if (icon == null)
        {
            return null;
        }

        if (Mathf.Approximately(ListIconScaleFactor, 1f))
        {
            return icon;
        }

        if (cachedListIconSource == icon && cachedListIcon != null)
        {
            return cachedListIcon;
        }

        if (cachedListIconSource != icon && cachedListIcon != null)
        {
            try { UnityEngine.Object.Destroy(cachedListIcon); } catch { }
            cachedListIcon = null;
            cachedListIconSource = null;
        }

        Texture2D texture = icon.texture;
        if (texture == null)
        {
            return icon;
        }

        Rect rect = icon.rect;
        if (rect.width <= 0f || rect.height <= 0f)
        {
            return icon;
        }

        float scale = Mathf.Clamp(ListIconScaleFactor, 0.01f, 10f);
        float pixelsPerUnit = icon.pixelsPerUnit;
        if (pixelsPerUnit <= 0f)
        {
            pixelsPerUnit = 100f;
        }
        float scaledPixelsPerUnit = pixelsPerUnit / scale;

        Vector2 pivot = new Vector2(icon.pivot.x / rect.width, icon.pivot.y / rect.height);
        Sprite scaledSprite = Sprite.Create(texture, rect, pivot, scaledPixelsPerUnit, 0, SpriteMeshType.FullRect, icon.border);
        scaledSprite.name = icon.name + "_ShadeScaled";
        scaledSprite.hideFlags = HideFlags.HideAndDontSave;

        cachedListIcon = scaledSprite;
        cachedListIconSource = icon;
        return scaledSprite;
    }

    private static void CopyRectTransform(RectTransform? source, RectTransform destination, bool copySiblingIndex = true)
    {
        if (source == null || destination == null)
        {
            return;
        }

        destination.anchorMin = source.anchorMin;
        destination.anchorMax = source.anchorMax;
        destination.pivot = source.pivot;
        destination.offsetMin = source.offsetMin;
        destination.offsetMax = source.offsetMax;
        destination.anchoredPosition = source.anchoredPosition;
        destination.anchoredPosition3D = source.anchoredPosition3D;
        destination.sizeDelta = source.sizeDelta;
        destination.localScale = source.localScale;
        destination.localRotation = source.localRotation;
        destination.localPosition = source.localPosition;
        if (copySiblingIndex)
        {
            destination.SetSiblingIndex(source.GetSiblingIndex());
        }
    }

    private static void CopyLayoutComponents(
        RectTransform? source,
        RectTransform destination,
        bool copyLayoutGroups = true,
        bool copyGridLayout = true)
    {
        if (destination == null)
        {
            return;
        }

        if (!copyGridLayout)
        {
            try
            {
                var existingGrids = destination.GetComponents<GridLayoutGroup>();
                if (existingGrids != null)
                {
                    foreach (var existing in existingGrids)
                    {
                        if (existing == null)
                        {
                            continue;
                        }

                        try { UnityEngine.Object.Destroy(existing); }
                        catch { }
                    }
                }
            }
            catch
            {
            }
        }

        if (!copyLayoutGroups)
        {
            try
            {
                var existingGroups = destination.GetComponents<LayoutGroup>();
                if (existingGroups != null)
                {
                    foreach (var group in existingGroups)
                    {
                        if (group == null)
                        {
                            continue;
                        }

                        if (!copyGridLayout && group is GridLayoutGroup)
                        {
                            continue;
                        }

                        try { UnityEngine.Object.Destroy(group); }
                        catch { }
                    }
                }
            }
            catch
            {
            }
        }

        if (source == null)
        {
            return;
        }

        var layoutElement = source.GetComponent<LayoutElement>();
        if (layoutElement != null)
        {
            var targetElement = destination.GetComponent<LayoutElement>() ?? destination.gameObject.AddComponent<LayoutElement>();
            targetElement.ignoreLayout = layoutElement.ignoreLayout;
            targetElement.minWidth = layoutElement.minWidth;
            targetElement.preferredWidth = layoutElement.preferredWidth;
            targetElement.flexibleWidth = layoutElement.flexibleWidth;
            targetElement.minHeight = layoutElement.minHeight;
            targetElement.preferredHeight = layoutElement.preferredHeight;
            targetElement.flexibleHeight = layoutElement.flexibleHeight;
            targetElement.layoutPriority = layoutElement.layoutPriority;
        }

        var fitter = source.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            var targetFitter = destination.GetComponent<ContentSizeFitter>() ?? destination.gameObject.AddComponent<ContentSizeFitter>();
            targetFitter.horizontalFit = fitter.horizontalFit;
            targetFitter.verticalFit = fitter.verticalFit;
        }

        var grid = source.GetComponent<GridLayoutGroup>();
        if (grid != null && copyGridLayout)
        {
            var targetGrid = destination.GetComponent<GridLayoutGroup>() ?? destination.gameObject.AddComponent<GridLayoutGroup>();
            targetGrid.cellSize = grid.cellSize;
            targetGrid.spacing = grid.spacing;
            targetGrid.startAxis = grid.startAxis;
            targetGrid.startCorner = grid.startCorner;
            targetGrid.constraint = grid.constraint;
            targetGrid.constraintCount = grid.constraintCount;
            targetGrid.childAlignment = grid.childAlignment;
            targetGrid.padding = new RectOffset(grid.padding.left, grid.padding.right, grid.padding.top, grid.padding.bottom);
            return;
        }

        var layoutGroup = source.GetComponent<LayoutGroup>();
        if (layoutGroup != null && copyLayoutGroups && (copyGridLayout || !(layoutGroup is GridLayoutGroup)))
        {
            var targetGroupComponent = destination.GetComponent(layoutGroup.GetType()) as LayoutGroup;
            if (targetGroupComponent == null)
            {
                targetGroupComponent = destination.gameObject.AddComponent(layoutGroup.GetType()) as LayoutGroup;
            }

            if (targetGroupComponent != null)
            {
                targetGroupComponent.padding = new RectOffset(layoutGroup.padding.left, layoutGroup.padding.right, layoutGroup.padding.top, layoutGroup.padding.bottom);
                targetGroupComponent.childAlignment = layoutGroup.childAlignment;

                if (layoutGroup is HorizontalOrVerticalLayoutGroup hv && targetGroupComponent is HorizontalOrVerticalLayoutGroup targetHv)
                {
                    targetHv.spacing = hv.spacing;
                    targetHv.childControlWidth = hv.childControlWidth;
                    targetHv.childControlHeight = hv.childControlHeight;
                    targetHv.childForceExpandWidth = hv.childForceExpandWidth;
                    targetHv.childForceExpandHeight = hv.childForceExpandHeight;
                }
            }
        }
    }

    internal static void SyncDisplayName(ShadeInventoryPane pane, string label)
    {
        if (pane == null)
        {
            return;
        }

        try
        {
            var value = new LocalisedString(string.Empty, string.IsNullOrEmpty(label) ? string.Empty : label);
            DisplayNameField(pane) = value;
        }
        catch
        {
        }
    }

    private static void ScheduleTemplateSync(InventoryPaneList paneList, InventoryPane? template, ShadeInventoryPane shadePane)
    {
    }

    private sealed class TemplateSyncHost : MonoBehaviour
    {
        private readonly List<SyncRequest> pending = new List<SyncRequest>();

        private void Awake()
        {
            hideFlags |= HideFlags.HideInInspector;
        }

        public void Schedule(InventoryPane template, ShadeInventoryPane shade)
        {
            if (!template || !shade)
            {
                return;
            }

            for (int i = pending.Count - 1; i >= 0; i--)
            {
                if (!pending[i].IsValid)
                {
                    pending.RemoveAt(i);
                }
            }

            for (int i = 0; i < pending.Count; i++)
            {
                if (pending[i].Matches(template, shade))
                {
                    return;
                }
            }

            pending.Add(new SyncRequest(template, shade));
            enabled = true;
        }

        private void LateUpdate()
        {
            bool hasPending = false;

            for (int i = pending.Count - 1; i >= 0; i--)
            {
                var request = pending[i];
                if (!request.IsValid)
                {
                    pending.RemoveAt(i);
                    continue;
                }

                if (request.TrySynchronize())
                {
                    pending.RemoveAt(i);
                }
                else
                {
                    hasPending = true;
                }
            }

            if (!hasPending)
            {
                enabled = false;
            }
        }

        private struct SyncRequest
        {
            public SyncRequest(InventoryPane template, ShadeInventoryPane shade)
            {
                Template = template;
                Shade = shade;
            }

            public InventoryPane Template { get; }

            public ShadeInventoryPane Shade { get; }

            public bool IsValid => Template && Shade;

            public bool Matches(InventoryPane template, ShadeInventoryPane shade) => Template == template && Shade == shade;

            public bool TrySynchronize()
            {
                if (!Template || !Shade)
                {
                    return true;
                }

                RectTransform? templateRect = ShadeInventoryPane.ResolveTemplateRootRectTransform(Template);
                if (templateRect == null)
                {
                    return false;
                }

                bool templateHasValidSize = ShadeInventoryPane.HasUsableTemplateRect(templateRect);

                var shadeRect = Shade.transform as RectTransform;
                if (shadeRect != null)
                {
                    if (templateRect == shadeRect)
                    {
                        return true;
                    }

                    try
                    {
                        if (templateRect.transform.IsChildOf(shadeRect))
                        {
                            return true;
                        }
                    }
                    catch
                    {
                    }

                    Transform? templateParent = templateRect.parent;
                    if (templateParent != null && shadeRect.parent != templateParent)
                    {
                        shadeRect.SetParent(templateParent, false);
                    }

                    if (templateHasValidSize)
                    {
                        CopyRectTransform(templateRect, shadeRect, copySiblingIndex: false);
                        CopyLayoutComponents(templateRect, shadeRect, copyLayoutGroups: false, copyGridLayout: false);

                        ShadeInventoryPane.LogRectTransformHierarchy(templateRect, "TemplatePaneSynced");
                        ShadeInventoryPane.LogRectTransformHierarchy(shadeRect, "ShadePaneBeforeSync");
                    }
                    else
                    {
                        Vector2 templateSize = templateRect.rect.size;
                        float width = Mathf.Abs(templateSize.x);
                        float height = Mathf.Abs(templateSize.y);
                        float area = width * height;
                        ShadeInventoryPane.LogMenuEvent(FormattableString.Invariant(
                            $"Template sync skipping layout copy: template size {ShadeInventoryPane.FormatVector2(templateRect.rect.size)} unsuitable (minDimThreshold={ShadeInventoryPane.MinTemplateCopyDimension}, minAreaThreshold={ShadeInventoryPane.MinTemplateCopyArea}, area={area:0.##})"));
                    }
                }

                Shade.ConfigureFromTemplate(Template);
                Shade.SetDisplayLabel(Shade.DisplayLabel);
                Shade.EnsureRootSizing();
                Shade.ForceImmediateRefresh();
                Shade.ForceLayoutRebuild();

                if (shadeRect != null)
                {
                    ShadeInventoryPane.LogRectTransformHierarchy(shadeRect, "ShadePaneAfterSync");
                }

                return true;
            }
        }
    }

    internal static bool TrySetCurrentPaneLabel(InventoryPaneList paneList, string label)
    {
        if (paneList == null || string.IsNullOrEmpty(label))
        {
            return false;
        }

        if (CurrentPaneTextFieldInfo == null)
        {
            return false;
        }

        try
        {
            var textObj = CurrentPaneTextFieldInfo.GetValue(paneList);
            if (textObj == null)
            {
                return false;
            }

            var textProp = textObj.GetType().GetProperty("text");
            if (textProp != null && textProp.CanWrite)
            {
                textProp.SetValue(textObj, label);
                return true;
            }
        }
        catch
        {
        }

        return false;
    }

    private static int GetUnlockedPaneCount(InventoryPaneList paneList, int fallback)
    {
        if (paneList == null)
        {
            return fallback;
        }

        if (UnlockedPaneCountProperty != null)
        {
            try
            {
                var value = UnlockedPaneCountProperty.GetValue(paneList, null);
                if (value is int unlocked)
                {
                    return unlocked;
                }
            }
            catch
            {
            }
        }

        return fallback;
    }

    private static int DetermineSelectedIndex(InventoryPaneList paneList, List<InventoryPane> panes)
    {
        if (paneList == null || panes == null || panes.Count == 0)
        {
            return 0;
        }

        int index = 0;
        try
        {
            string next = NextPaneOpenField != null ? NextPaneOpenField(paneList) : string.Empty;
            if (!string.IsNullOrEmpty(next) && GetPaneIndexMethod != null)
            {
                var result = GetPaneIndexMethod.Invoke(paneList, new object[] { next });
                if (result is int resolved && resolved >= 0)
                {
                    index = resolved;
                }
            }
        }
        catch
        {
        }

        return Mathf.Clamp(index, 0, panes.Count - 1);
    }

    private static void RefreshPaneListDisplay(InventoryPaneList paneList, List<InventoryPane> panes)
    {
        if (paneList == null || panes == null)
        {
            return;
        }

        InventoryPaneListDisplay? display = null;
        if (PaneListDisplayField != null)
        {
            try
            {
                display = PaneListDisplayField(paneList);
            }
            catch
            {
                display = null;
            }
        }
        if (display == null)
        {
            return;
        }

        try
        {
            display.PreInstantiate(panes.Count);
        }
        catch
        {
        }

        try
        {
            int selectedIndex = DetermineSelectedIndex(paneList, panes);
            int unlocked = GetUnlockedPaneCount(paneList, panes.Count);
            display.UpdateDisplay(selectedIndex, panes, unlocked);
        }
        catch
        {
        }
    }

    internal static void EnsurePane(InventoryPaneList paneList)
    {
        if (paneList == null)
        {
            ShadeInventoryPane.LogMenuEvent("EnsurePane skipped: paneList null");
            return;
        }

        var panes = PanesField(paneList);
        ShadeInventoryPane? existingShade = null;
        if (panes != null)
        {
            foreach (var pane in panes)
            {
                if (pane != null && pane.TryGetComponent<ShadeInventoryPane>(out var shade))
                {
                    existingShade = shade;
                    break;
                }
            }
        }

        if (panes == null || panes.Length == 0)
        {
            ShadeInventoryPane.LogMenuEvent("EnsurePane skipped: no template panes available");
            return;
        }

        InventoryPane? template = panes.FirstOrDefault(p =>
        {
            if (!p || p is ShadeInventoryPane)
            {
                return false;
            }

            string goName = p.gameObject != null ? p.gameObject.name : p.name;
            string typeName = p.GetType().Name;
            bool matchesName = !string.IsNullOrEmpty(goName) &&
                (goName.IndexOf("Charm", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 goName.IndexOf("Crest", StringComparison.OrdinalIgnoreCase) >= 0);
            bool matchesType = !string.IsNullOrEmpty(typeName) &&
                (typeName.IndexOf("Charm", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 typeName.IndexOf("Crest", StringComparison.OrdinalIgnoreCase) >= 0);
            return matchesName || matchesType;
        }) ?? panes.FirstOrDefault(p => p != null && !(p is ShadeInventoryPane));

        if (template == null)
        {
            ShadeInventoryPane.LogMenuEvent("EnsurePane skipped: no suitable template pane found");
            return;
        }

        InventoryPane templatePane = template;
        RectTransform? templateRect = ShadeInventoryPane.ResolveTemplateRootRectTransform(templatePane);
        Transform? parent = null;
        if (templateRect != null)
        {
            parent = templateRect.parent;
        }
        else
        {
            parent = templatePane.transform.parent;
        }

        if (existingShade != null)
        {
            existingShade.AttachToPaneList(paneList);
            existingShade.ConfigureFromTemplate(templatePane);
            existingShade.SetDisplayLabel("Charms");
            existingShade.ForceImmediateRefresh();
            existingShade.ForceLayoutRebuild();
            ShadeInventoryPane.LogMenuEvent(FormattableString.Invariant(
                $"EnsurePane refreshed existing shade overlay pane (active={existingShade.isActiveAndEnabled})"));
            return;
        }

        parent ??= paneList.transform;
        ShadeInventoryPane.LogMenuEvent($"Injecting shade overlay pane using template '{templatePane.GetType().Name}'");

        var go = new GameObject("ShadeInventoryPane", typeof(RectTransform));
        int templateLayer = templatePane.gameObject != null ? templatePane.gameObject.layer : paneList.gameObject.layer;
        go.layer = templateLayer;
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;

        var canvasGroup = go.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        go.AddComponent<SimpleCanvasNestedFadeGroup>();

        var shadePane = go.AddComponent<ShadeInventoryPane>();
        shadePane.RootPane = shadePane;
        shadePane.ConfigureFromTemplate(templatePane);
        shadePane.SetDisplayLabel("Charms");
        shadePane.ForceImmediateRefresh();
        shadePane.ForceLayoutRebuild();
        shadePane.AttachToPaneList(paneList);

        var input = go.AddComponent<InventoryPaneInput>();
        ConfigureInput(input, paneList, shadePane);

        PlayerDataTestField(shadePane) = new PlayerDataTest();
        HasNewPdField(shadePane) = string.Empty;

        var icon = ShadeCharmIconLoader.TryLoadIcon("shade_tab", "shade_charm_void_heart", "void_heart", "shade");
        var charms = ShadeRuntime.Charms;
        if (icon == null && charms != null)
        {
            icon = charms.AllCharms.FirstOrDefault()?.Icon;
        }
        AssignListIcon(shadePane, icon);

        var newList = panes.ToList();

        int insertIndex = -1;

        for (int i = 0; i < panes.Length; i++)
        {
            var existing = panes[i];
            if (!existing)
            {
                continue;
            }

            string typeName = existing.GetType().Name;
            if (!string.IsNullOrEmpty(typeName) &&
                typeName.IndexOf("Crest", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                insertIndex = i + 1;
                break;
            }
        }

        if (insertIndex < 0)
        {
            for (int i = 0; i < panes.Length; i++)
            {
                var existing = panes[i];
                if (!existing)
                {
                    continue;
                }

                string name = existing.gameObject != null ? existing.gameObject.name : existing.name;
                string typeName = existing.GetType().Name;
                bool matchesName = !string.IsNullOrEmpty(name) &&
                    (name.IndexOf("Charm", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("Tool", StringComparison.OrdinalIgnoreCase) >= 0);
                bool matchesType = !string.IsNullOrEmpty(typeName) &&
                    (typeName.IndexOf("Charm", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     typeName.IndexOf("Tool", StringComparison.OrdinalIgnoreCase) >= 0);

                if (matchesName || matchesType)
                {
                    insertIndex = i + 1;
                    break;
                }
            }
        }

        if (insertIndex < 0)
        {
            insertIndex = newList.Count;
        }

        insertIndex = Mathf.Clamp(insertIndex, 0, newList.Count);
        newList.Insert(insertIndex, shadePane);
        PanesField(paneList) = newList.ToArray();
        RefreshPaneListDisplay(paneList, newList);
        ShadeInventoryPane.LogMenuEvent($"Shade pane inserted at index {insertIndex}; total panes={newList.Count}");
    }

    private static void ConfigureInput(InventoryPaneInput input, InventoryPaneList paneList, ShadeInventoryPane shadePane)
    {
        if (input == null)
        {
            ShadeInventoryPane.LogMenuEvent("ConfigureInput skipped: input null");
            return;
        }

        PaneControlField(input) = InventoryPaneList.PaneTypes.None;
        AllowHorizontalField?.SetValue(input, true);
        AllowVerticalField?.SetValue(input, true);
        AllowRepeatField?.SetValue(input, true);
        AllowRepeatSubmitField?.SetValue(input, false);
        AllowRightStickField?.SetValue(input, false);

        if (shadePane != null && PaneField != null)
        {
            try
            {
                if (PaneField.GetValue(input) == null)
                {
                    PaneField.SetValue(input, shadePane);
                    ShadeInventoryPane.LogMenuEvent("Bound InventoryPaneInput to shade pane");
                }
            }
            catch { }
        }

        if (paneList != null && PaneListField != null)
        {
            try
            {
                PaneListField.SetValue(input, paneList);
            }
            catch { }
        }
    }

    internal static ShadeInventoryPane? TryGetShadePane(InventoryPaneInput input)
    {
        if (input == null || PaneField == null)
        {
            return null;
        }

        try
        {
            return PaneField.GetValue(input) as ShadeInventoryPane;
        }
        catch
        {
            return null;
        }
    }
}


