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
    private const int Columns = 7;
    private const float BackgroundAlpha = 0.82f;
    private const float MinRootSizeThreshold = 1f;

    private static readonly Color DefaultPanelColor = new Color(0.05f, 0.05f, 0.08f, 0.92f);
    private static readonly Color DefaultHighlightColor = new Color(0.78f, 0.86f, 1f, 0.35f);
    private static readonly Color DefaultCellColor = new Color(0.18f, 0.2f, 0.26f, BackgroundAlpha);
    private static readonly Color DefaultNewMarkerColor = new Color(1f, 0.85f, 0.25f, 0.95f);

    private struct RectSnapshot
    {
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Pivot;
        public Vector2 OffsetMin;
        public Vector2 OffsetMax;
        public Vector2 AnchoredPosition;
        public Vector2 SizeDelta;

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
                SizeDelta = rect.sizeDelta
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
            rect.sizeDelta = SizeDelta;
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

    private RectTransform panelRoot = null!;
    private RectTransform contentRoot = null!;
    private RectTransform gridRoot = null!;
    private RectTransform highlight = null!;
    private Text? titleText;
    private Text? notchText;
    private Text? detailTitleText;
    private Text? descriptionText;
    private Text? statusText;
    private Text? hintText;
    private TMP_Text? titleTextTMP;
    private TMP_Text? notchTextTMP;
    private TMP_Text? detailTitleTextTMP;
    private TMP_Text? descriptionTextTMP;
    private TMP_Text? statusTextTMP;
    private TMP_Text? hintTextTMP;
    private CanvasGroup canvasGroup = null!;

    private Font? bodyFont;
    private Font? headerFont;
    private Material? bodyFontMaterial;
    private Material? headerFontMaterial;
    private Color bodyFontColor = Color.white;
    private Color headerFontColor = Color.white;
    private TMP_FontAsset? bodyTmpFont;
    private TMP_FontAsset? headerTmpFont;
    private Material? bodyTmpFontMaterial;
    private Material? headerTmpFontMaterial;
    private Color bodyTmpFontColor = Color.white;
    private Color headerTmpFontColor = Color.white;
    private Sprite? panelBackgroundSprite;
    private Color panelBackgroundColor = DefaultPanelColor;
    private Sprite? highlightSpriteTemplate;
    private Color highlightColor = DefaultHighlightColor;
    private Sprite? cellFrameSprite;
    private Color cellFrameColor = DefaultCellColor;
    private Sprite? newMarkerSpriteTemplate;
    private Color newMarkerColor = DefaultNewMarkerColor;

    private ShadeCharmInventory? inventory;
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

    private struct CharmEntry
    {
        public ShadeCharmDefinition Definition;
        public ShadeCharmId Id;
        public RectTransform Root;
        public Image Icon;
        public Image Background;
        public GameObject NewMarker;
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

    private TMP_FontAsset? GetPreferredTmpFont(bool useHeaderFont)
    {
        return useHeaderFont ? (headerTmpFont ?? bodyTmpFont) : (bodyTmpFont ?? headerTmpFont);
    }

    private Material? GetPreferredTmpMaterial(bool useHeaderFont)
    {
        return useHeaderFont ? (headerTmpFontMaterial ?? bodyTmpFontMaterial) : (bodyTmpFontMaterial ?? headerTmpFontMaterial);
    }

    private Color GetPreferredTmpColor(bool useHeaderFont)
    {
        return useHeaderFont ? headerTmpFontColor : bodyTmpFontColor;
    }

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
        if (!template)
        {
            return null;
        }

        RectTransform? rect = template.transform as RectTransform;
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

        RectTransform? matchByComponent = null;
        RectTransform? matchByName = null;
        RectTransform? matchDirectChild = null;
        RectTransform? firstCandidate = null;

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

                if (candidate.transform == template.transform)
                {
                    rect = candidate;
                    break;
                }

                if (matchDirectChild == null && candidate.transform.parent == template.transform)
                {
                    matchDirectChild = candidate;
                }

                if (matchByComponent == null)
                {
                    try
                    {
                        var paneComponent = candidate.GetComponent<InventoryPane>();
                        if (paneComponent != null)
                        {
                            if (paneComponent == template)
                            {
                                rect = candidate;
                                break;
                            }

                            matchByComponent = candidate;
                        }
                    }
                    catch
                    {
                    }
                }

                if (matchByName == null)
                {
                    string? name = candidate.gameObject != null ? candidate.gameObject.name : null;
                    if (!string.IsNullOrEmpty(name))
                    {
                        string lower = name.ToLowerInvariant();
                        if (lower.Contains("pane") || lower.Contains("panel"))
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

            rect ??= matchByComponent;
            rect ??= matchByName;
            rect ??= matchDirectChild;
            rect ??= firstCandidate;
        }

        if (rect != null && rect.transform != template.transform)
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

    private static string FormatVector2(Vector2 value)
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
        canvasGroup ??= GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        ShadeCharmInventory? inv = ShadeRuntime.Charms;
        if (inv != null)
        {
            inv.StateChanged += HandleInventoryChanged;
        }
        inventory = inv;
        isActive = true;
        RefreshAll();
        UpdateParentListLabel();
        LogMenuEvent($"OnEnable: entries={entries.Count}, inventoryNull={inventory == null}");
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.StateChanged -= HandleInventoryChanged;
        }
        inventory = null;
        isActive = false;
        LogMenuEvent("OnDisable");
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnDestroy()
    {
        if (fallbackSprite != null)
        {
            if (fallbackSprite.texture != null)
            {
                Destroy(fallbackSprite.texture);
            }
            Destroy(fallbackSprite);
            fallbackSprite = null;
        }
    }

    public override void PaneStart()
    {
        base.PaneStart();
        EnsureBuilt();
        labelPulseTimer = 0f;
        canvasGroup ??= GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        inventory ??= ShadeRuntime.Charms;
        isActive = true;
        RefreshAll();
        UpdateParentListLabel();
        LogMenuEvent($"PaneStart: entries={entries.Count}, inventoryNull={inventory == null}");
    }

    public override void PaneEnd()
    {
        isActive = false;
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
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

        try
        {
            var templateRect = ResolveTemplateRootRectTransform(template);
            if (templateRect != null)
            {
                rootRectTemplate = RectSnapshot.From(templateRect);
                templateRootSize = templateRect.rect.size;
                var templateLayout = templateRect.GetComponent<LayoutElement>();
                if (templateLayout != null)
                {
                    rootLayoutTemplate = LayoutElementSnapshot.From(templateLayout);
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
                    bodyFont = bodySample.font;
                    bodyFontMaterial = bodySample.material;
                    if (bodySample.color.a > 0f)
                    {
                        bodyFontColor = bodySample.color;
                    }
                }

                var headerSample = ordered.LastOrDefault();
                if (headerSample != null)
                {
                    headerFont = headerSample.font;
                    headerFontMaterial = headerSample.material;
                    if (headerSample.color.a > 0f)
                    {
                        headerFontColor = headerSample.color;
                    }
                }
            }

            var tmpTexts = template.GetComponentsInChildren<TMP_Text>(true);
            if (tmpTexts != null && tmpTexts.Length > 0)
            {
                var orderedTmp = tmpTexts
                    .Where(t => t != null && t.font != null)
                    .OrderBy(t => t.fontSize)
                    .ToArray();

                if (orderedTmp.Length > 0)
                {
                    int bodyIndex = Mathf.Clamp(orderedTmp.Length > 1 ? orderedTmp.Length / 2 : 0, 0, orderedTmp.Length - 1);
                    var bodySample = orderedTmp[bodyIndex];
                    bodyTmpFont = bodySample.font;
                    bodyTmpFontMaterial = bodySample.fontMaterial;
                    if (bodySample.color.a > 0f)
                    {
                        bodyTmpFontColor = bodySample.color;
                    }
                }

                var headerSampleTmp = orderedTmp.LastOrDefault();
                if (headerSampleTmp != null)
                {
                    headerTmpFont = headerSampleTmp.font;
                    headerTmpFontMaterial = headerSampleTmp.fontMaterial;
                    if (headerSampleTmp.color.a > 0f)
                    {
                        headerTmpFontColor = headerSampleTmp.color;
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

                    if (newMarkerSpriteTemplate == null && lower.Contains("new"))
                    {
                        newMarkerSpriteTemplate = sprite;
                        if (img.color.a > 0f)
                        {
                            newMarkerColor = img.color;
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

            var rects = template.GetComponentsInChildren<RectTransform>(true);
            if (rects != null && rects.Length > 0)
            {
                foreach (var rect in rects)
                {
                    if (rect == null)
                    {
                        continue;
                    }

                    var grid = rect.GetComponent<GridLayoutGroup>();
                    if (grid != null && gridRectTemplate == null)
                    {
                        gridRectTemplate = RectSnapshot.From(rect);
                        gridLayoutTemplate = GridLayoutSnapshot.From(grid);
                        continue;
                    }

                    string lowerName = rect.gameObject != null ? rect.gameObject.name?.ToLowerInvariant() ?? string.Empty : string.Empty;
                    if (panelRectTemplate == null && !string.IsNullOrEmpty(lowerName) &&
                        (lowerName.Contains("panel") || lowerName.Contains("background")))
                    {
                        panelRectTemplate = RectSnapshot.From(rect);
                        continue;
                    }

                    if (contentRectTemplate == null && !string.IsNullOrEmpty(lowerName) && lowerName.Contains("content"))
                    {
                        contentRectTemplate = RectSnapshot.From(rect);
                        continue;
                    }

                    if (detailRectTemplate == null && !string.IsNullOrEmpty(lowerName) && lowerName.Contains("detail"))
                    {
                        detailRectTemplate = RectSnapshot.From(rect);
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

    private bool TryApplyStandaloneRootSizing(RectTransform root, Vector2? desiredSize = null)
    {
        if (root == null)
        {
            return false;
        }

        if (root.parent is RectTransform)
        {
            return false;
        }

        Vector2? candidate = desiredSize ?? templateRootSize;
        if (!candidate.HasValue)
        {
            return false;
        }

        Vector2 size = candidate.Value;
        if (size.x < MinRootSizeThreshold || size.y < MinRootSizeThreshold)
        {
            return false;
        }

        Vector2 anchor = new Vector2(0.5f, 0.5f);
        Vector2 pivot = rootRectTemplate?.Pivot ?? new Vector2(0.5f, 0.5f);
        Vector2 anchored = rootRectTemplate?.AnchoredPosition ?? Vector2.zero;

        bool changed = false;

        if (!Approximately(root.anchorMin, anchor) || !Approximately(root.anchorMax, anchor))
        {
            root.anchorMin = anchor;
            root.anchorMax = anchor;
            changed = true;
        }

        if (!Approximately(root.pivot, pivot))
        {
            root.pivot = pivot;
            changed = true;
        }

        if (!Approximately(root.sizeDelta, size))
        {
            root.sizeDelta = size;
            changed = true;
        }

        if (!Approximately(root.anchoredPosition, anchored))
        {
            root.anchoredPosition = anchored;
            changed = true;
        }

        Vector2 offsetMin = anchored - Vector2.Scale(size, pivot);
        Vector2 offsetMax = anchored + Vector2.Scale(size, Vector2.one - pivot);

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

        if (!changed)
        {
            return false;
        }

        LogMenuEvent(FormattableString.Invariant(
            $"ApplyStandaloneRootSizing -> parent='{root.parent?.name ?? "<null>"}' size={FormatVector2(size)} anchor={FormatVector2(anchor)} pivot={FormatVector2(pivot)} anchored={FormatVector2(anchored)}"));
        return true;
    }

    internal void EnsureRootSizing()
    {
        var root = transform as RectTransform;
        if (root == null)
        {
            return;
        }

        if (root.rect.width >= MinRootSizeThreshold && root.rect.height >= MinRootSizeThreshold && root.parent is RectTransform)
        {
            return;
        }

        TryApplyStandaloneRootSizing(root);
    }

    private void ApplyTemplateRootLayoutFallback(RectTransform root)
    {
        if (root == null)
        {
            return;
        }

        Vector2? desiredSize = templateRootSize;
        bool adjustments = TryApplyStandaloneRootSizing(root, desiredSize);
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

    public void ForceLayoutRebuild()
    {
        EnsureBuilt();

        var selfRect = transform as RectTransform;
        if (selfRect == null)
        {
            LogMenuEvent("ForceLayoutRebuild skipped: transform lacks RectTransform");
            return;
        }

        TryApplyStandaloneRootSizing(selfRect);

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

        LayoutRebuilder.ForceRebuildLayoutImmediate(selfRect);

        Vector2 rootSize = selfRect.rect.size;
        string panelSizeText = panelRoot != null ? FormatVector2(panelRoot.rect.size) : "<null>";
        string contentSizeText = contentRoot != null ? FormatVector2(contentRoot.rect.size) : "<null>";
        string gridSizeText = gridRoot != null ? FormatVector2(gridRoot.rect.size) : "<null>";

        bool rootTooSmall = rootSize.x < MinRootSizeThreshold || rootSize.y < MinRootSizeThreshold;
        if (rootTooSmall)
        {
            LogMenuEvent(FormattableString.Invariant(
                $"ForceLayoutRebuild -> root={FormatVector2(rootSize)}, panel={panelSizeText}, content={contentSizeText}, grid={gridSizeText} (threshold={MinRootSizeThreshold})"));
            LogRectTransformHierarchy(selfRect, $"ShadePaneRoot[{gameObject.name}].BeforeFallback");
            ApplyTemplateRootLayoutFallback(selfRect);

            rootSize = selfRect.rect.size;
            panelSizeText = panelRoot != null ? FormatVector2(panelRoot.rect.size) : "<null>";
            contentSizeText = contentRoot != null ? FormatVector2(contentRoot.rect.size) : "<null>";
            gridSizeText = gridRoot != null ? FormatVector2(gridRoot.rect.size) : "<null>";

            LogMenuEvent(FormattableString.Invariant(
                $"ForceLayoutRebuild post-fallback -> root={FormatVector2(rootSize)}, panel={panelSizeText}, content={contentSizeText}, grid={gridSizeText}"));
            LogRectTransformHierarchy(selfRect, $"ShadePaneRoot[{gameObject.name}].AfterFallback");
        }
        else
        {
            LogMenuEvent(FormattableString.Invariant(
                $"ForceLayoutRebuild -> root={FormatVector2(rootSize)}, panel={panelSizeText}, content={contentSizeText}, grid={gridSizeText}"));
        }
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
        highlight = null!;
        titleText = null!;
        notchText = null!;
        detailTitleText = null!;
        descriptionText = null!;
        statusText = null!;
        hintText = null!;
        entries.Clear();
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

        EnsureRootSizing();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        panelRoot = new GameObject("ShadePanel", typeof(RectTransform)).GetComponent<RectTransform>();
        panelRoot.SetParent(transform, false);
        if (panelRectTemplate.HasValue)
        {
            panelRectTemplate.Value.Apply(panelRoot);
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
            panelImage.color = panelBackgroundColor;
        }
        panelImage.raycastTarget = false;

        contentRoot = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
        contentRoot.SetParent(panelRoot, false);
        if (contentRectTemplate.HasValue)
        {
            contentRectTemplate.Value.Apply(contentRoot);
        }
        else
        {
            contentRoot.anchorMin = Vector2.zero;
            contentRoot.anchorMax = Vector2.one;
            contentRoot.offsetMin = new Vector2(32f, 36f);
            contentRoot.offsetMax = new Vector2(-32f, -36f);
        }

        titleText = CreateText("Title", contentRoot, FontStyle.Normal, 46, TextAnchor.UpperLeft, out titleTextTMP, useHeaderFont: true);
        var titleRect = ResolveRectTransform(titleText, titleTextTMP);
        if (titleRect != null)
        {
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0f, 1f);
            titleRect.offsetMin = new Vector2(0f, -60f);
            titleRect.offsetMax = new Vector2(0f, -8f);
        }
        SetTextValue(titleText, titleTextTMP, displayLabel);

        notchText = CreateText("Notches", contentRoot, FontStyle.Normal, 32, TextAnchor.UpperRight, out notchTextTMP);
        var notchRect = ResolveRectTransform(notchText, notchTextTMP);
        if (notchRect != null)
        {
            notchRect.anchorMin = new Vector2(0.45f, 1f);
            notchRect.anchorMax = new Vector2(1f, 1f);
            notchRect.pivot = new Vector2(1f, 1f);
            notchRect.offsetMin = new Vector2(-12f, -60f);
            notchRect.offsetMax = new Vector2(0f, -10f);
        }

        gridRoot = new GameObject("CharmGrid", typeof(RectTransform)).GetComponent<RectTransform>();
        gridRoot.SetParent(contentRoot, false);
        if (gridRectTemplate.HasValue)
        {
            gridRectTemplate.Value.Apply(gridRoot);
        }
        else
        {
            gridRoot.anchorMin = new Vector2(0f, 0f);
            gridRoot.anchorMax = new Vector2(0.58f, 1f);
            gridRoot.pivot = new Vector2(0f, 1f);
            gridRoot.offsetMin = new Vector2(0f, 16f);
            gridRoot.offsetMax = new Vector2(-32f, -104f);
            gridRoot.anchoredPosition = Vector2.zero;
        }

        var gridLayout = gridRoot.gameObject.AddComponent<GridLayoutGroup>();
        if (gridLayoutTemplate.HasValue)
        {
            gridLayoutTemplate.Value.Apply(gridLayout);
        }
        else
        {
            gridLayout.cellSize = new Vector2(104f, 112f);
            gridLayout.spacing = new Vector2(16f, 16f);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = Columns;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
            gridLayout.padding = new RectOffset(4, 4, 4, 4);
        }

        highlight = new GameObject("Highlight", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        highlight.SetParent(gridRoot, false);
        var highlightImage = highlight.GetComponent<Image>();
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
        highlight.gameObject.SetActive(false);

        var detailRoot = new GameObject("Details", typeof(RectTransform)).GetComponent<RectTransform>();
        detailRoot.SetParent(contentRoot, false);
        if (detailRectTemplate.HasValue)
        {
            detailRectTemplate.Value.Apply(detailRoot);
        }
        else
        {
            detailRoot.anchorMin = new Vector2(0.6f, 0f);
            detailRoot.anchorMax = new Vector2(1f, 1f);
            detailRoot.pivot = new Vector2(0f, 1f);
            detailRoot.offsetMin = new Vector2(24f, 16f);
            detailRoot.offsetMax = new Vector2(-8f, -104f);
        }

        detailTitleText = CreateText("CharmName", detailRoot, FontStyle.Normal, 38, TextAnchor.UpperLeft, out detailTitleTextTMP, useHeaderFont: true);
        var detailTitleRect = ResolveRectTransform(detailTitleText, detailTitleTextTMP);
        if (detailTitleRect != null)
        {
            detailTitleRect.anchorMin = new Vector2(0f, 0.74f);
            detailTitleRect.anchorMax = new Vector2(1f, 1f);
            detailTitleRect.pivot = new Vector2(0f, 1f);
            detailTitleRect.offsetMin = Vector2.zero;
            detailTitleRect.offsetMax = new Vector2(0f, -6f);
        }
        SetTextValue(detailTitleText, detailTitleTextTMP, displayLabel);

        descriptionText = CreateText("Description", detailRoot, FontStyle.Normal, 30, TextAnchor.UpperLeft, out descriptionTextTMP);
        var descRect = ResolveRectTransform(descriptionText, descriptionTextTMP);
        if (descRect != null)
        {
            descRect.anchorMin = new Vector2(0f, 0.32f);
            descRect.anchorMax = new Vector2(1f, 0.74f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = new Vector2(-6f, -4f);
        }
        if (descriptionText != null)
        {
            descriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
            descriptionText.verticalOverflow = VerticalWrapMode.Overflow;
            descriptionText.lineSpacing = 1.1f;
        }
        else if (descriptionTextTMP != null)
        {
            descriptionTextTMP.lineSpacing = 1.1f;
        }

        statusText = CreateText("Status", detailRoot, FontStyle.Italic, 28, TextAnchor.UpperLeft, out statusTextTMP);
        var statusRect = ResolveRectTransform(statusText, statusTextTMP);
        if (statusRect != null)
        {
            statusRect.anchorMin = new Vector2(0f, 0.18f);
            statusRect.anchorMax = new Vector2(1f, 0.34f);
            statusRect.offsetMin = new Vector2(0f, 4f);
            statusRect.offsetMax = new Vector2(-6f, 0f);
        }

        hintText = CreateText("Hint", detailRoot, FontStyle.Normal, 24, TextAnchor.UpperLeft, out hintTextTMP);
        var hintRect = ResolveRectTransform(hintText, hintTextTMP);
        if (hintRect != null)
        {
            hintRect.anchorMin = new Vector2(0f, 0f);
            hintRect.anchorMax = new Vector2(1f, 0.18f);
            hintRect.offsetMin = new Vector2(0f, 4f);
            hintRect.offsetMax = new Vector2(-6f, 0f);
        }
        SetTextValue(hintText, hintTextTMP, "Submit to equip or unequip. Ctrl + ` unlocks all charms (debug).");
        var hintColor = new Color(0.78f, 0.82f, 0.92f, 0.8f);
        if (hintText != null)
        {
            hintText.color = hintColor;
        }
        else if (hintTextTMP != null)
        {
            hintTextTMP.color = hintColor;
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
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);

        var preferredTmpFont = GetPreferredTmpFont(useHeaderFont);
        if (preferredTmpFont != null)
        {
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.font = preferredTmpFont;
            tmp.fontSize = size;
            tmp.fontStyle = ConvertFontStyle(style);
            tmp.alignment = ConvertAlignment(anchor);
            tmp.color = GetPreferredTmpColor(useHeaderFont);
            var tmpMaterial = GetPreferredTmpMaterial(useHeaderFont);
            if (tmpMaterial != null)
            {
                tmp.fontMaterial = tmpMaterial;
            }

            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.raycastTarget = false;
            tmp.text = string.Empty;
            tmpText = tmp;
            return null;
        }

        var text = go.AddComponent<Text>();

        Font? resolvedFont = useHeaderFont ? (headerFont ?? bodyFont) : (bodyFont ?? headerFont);
        if (resolvedFont != null)
        {
            text.font = resolvedFont;
        }
        else
        {
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        text.fontStyle = style;
        text.fontSize = size;
        text.alignment = anchor;

        var resolvedMaterial = useHeaderFont ? headerFontMaterial : bodyFontMaterial;
        if (resolvedMaterial != null)
        {
            text.material = resolvedMaterial;
        }

        text.color = useHeaderFont ? headerFontColor : bodyFontColor;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        text.text = string.Empty;
        return text;
    }

    private void MoveSelectionHorizontal(int direction)
    {
        EnsureBuilt();
        if (entries.Count == 0)
        {
            return;
        }

        int newIndex = selectedIndex + direction;
        int row = selectedIndex / Columns;
        int newRow = newIndex / Columns;
        if (newIndex < 0 || newIndex >= entries.Count || newRow != row)
        {
            return;
        }

        SelectIndex(newIndex);
    }

    private void MoveSelectionVertical(int direction)
    {
        EnsureBuilt();
        if (entries.Count == 0)
        {
            return;
        }

        int newIndex = selectedIndex + direction * Columns;
        if (newIndex < 0 || newIndex >= entries.Count)
        {
            return;
        }

        SelectIndex(newIndex);
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
        highlight.gameObject.SetActive(true);
        highlight.SetParent(entry.Root, false);
        highlight.SetAsFirstSibling();
        highlight.anchorMin = Vector2.zero;
        highlight.anchorMax = Vector2.one;
        highlight.offsetMin = Vector2.zero;
        highlight.offsetMax = Vector2.zero;

        var inv = inventory;
        if (inv != null)
        {
            inv.MarkCharmSeen(entry.Id);
        }

        UpdateDetailPanel();
        RefreshEntryStates();
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
            if (entry.Icon != null)
            {
                entry.Icon.sprite = sprite;
            }
            entries[i] = entry;
        }

        if (gridRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridRoot);
        }

        if (selectedIndex >= entries.Count)
        {
            selectedIndex = entries.Count > 0 ? entries.Count - 1 : 0;
        }

        UpdateNotchMeter();
        if (isActive && entries.Count > 0)
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
    }

    private CharmEntry CreateEntry(int index)
    {
        var cell = new GameObject($"CharmCell_{index}", typeof(RectTransform));
        var cellRect = cell.GetComponent<RectTransform>();
        cellRect.SetParent(gridRoot, false);
        cellRect.localScale = Vector3.one;

        var background = cell.AddComponent<Image>();
        if (cellFrameSprite != null)
        {
            background.sprite = cellFrameSprite;
            background.type = Image.Type.Sliced;
            background.color = cellFrameColor;
        }
        else
        {
            background.color = cellFrameColor;
        }
        background.raycastTarget = false;

        var iconGo = new GameObject("Icon", typeof(RectTransform));
        var iconRect = iconGo.GetComponent<RectTransform>();
        iconRect.SetParent(cellRect, false);
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.sizeDelta = new Vector2(96f, 96f);
        var icon = iconGo.AddComponent<Image>();
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        var newMarker = new GameObject("New", typeof(RectTransform), typeof(Image));
        var markerRect = newMarker.GetComponent<RectTransform>();
        markerRect.SetParent(cellRect, false);
        markerRect.anchorMin = new Vector2(1f, 1f);
        markerRect.anchorMax = new Vector2(1f, 1f);
        markerRect.pivot = new Vector2(1f, 1f);
        markerRect.anchoredPosition = new Vector2(-12f, -12f);
        markerRect.sizeDelta = new Vector2(18f, 18f);
        var markerImage = newMarker.GetComponent<Image>();
        if (newMarkerSpriteTemplate != null)
        {
            markerImage.sprite = newMarkerSpriteTemplate;
            markerImage.type = Image.Type.Sliced;
        }
        markerImage.color = newMarkerColor;
        markerImage.raycastTarget = false;
        newMarker.SetActive(false);

        return new CharmEntry
        {
            Root = cellRect,
            Background = background,
            Icon = icon,
            NewMarker = newMarker
        };
    }

    private void RefreshEntryStates()
    {
        EnsureBuilt();
        if (inventory == null)
        {
            foreach (var entry in entries)
            {
                entry.Icon.color = new Color(1f, 1f, 1f, 0.3f);
                entry.Background.color = new Color(0.12f, 0.12f, 0.12f, BackgroundAlpha);
                if (entry.NewMarker != null)
                {
                    entry.NewMarker.SetActive(false);
                }
            }
            return;
        }

        foreach (var entry in entries)
        {
            bool owned = inventory.IsOwned(entry.Id);
            bool equipped = inventory.IsEquipped(entry.Id);
            bool broken = inventory.IsBroken(entry.Id);
            bool isNew = inventory.IsNewlyDiscovered(entry.Id);

            entry.Icon.color = owned ? Color.white : new Color(1f, 1f, 1f, 0.25f);

            Color bgColor;
            if (!owned)
            {
                bgColor = new Color(0.1f, 0.1f, 0.1f, BackgroundAlpha);
            }
            else if (broken)
            {
                bgColor = new Color(0.55f, 0.22f, 0.22f, BackgroundAlpha);
            }
            else if (equipped)
            {
                bgColor = new Color(0.28f, 0.46f, 0.72f, BackgroundAlpha + 0.05f);
            }
            else
            {
                bgColor = new Color(0.18f, 0.2f, 0.26f, BackgroundAlpha);
            }
            entry.Background.color = bgColor;

            if (entry.NewMarker != null)
            {
                entry.NewMarker.SetActive(isNew);
            }
        }
    }

    private void UpdateNotchMeter()
    {
        EnsureBuilt();
        if (notchText == null && notchTextTMP == null)
        {
            return;
        }

        if (inventory == null)
        {
            SetTextValue(notchText, notchTextTMP, "Shade charm inventory unavailable.");
        }
        else
        {
            SetTextValue(notchText, notchTextTMP, $"Notches Used: {inventory.UsedNotches}/{inventory.NotchCapacity}");
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

    private void UpdateDetailPanel()
    {
        EnsureBuilt();
        if (entries.Count == 0 || inventory == null)
        {
            SetTextValue(detailTitleText, detailTitleTextTMP, displayLabel);
            SetTextValue(descriptionText, descriptionTextTMP, "Collect shade charms to unlock new abilities for your companion.");
            SetTextValue(statusText, statusTextTMP, string.Empty);
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
            status = "Equipped. Submit to unequip.";
        }
        else if (inventory.UsedNotches + notchCost > inventory.NotchCapacity)
        {
            status = "Not enough notches available.";
        }
        else
        {
            status = "Submit to equip this charm.";
        }
        string notchInfo;
        if (notchCost <= 0)
        {
            notchInfo = "No notch cost.";
        }
        else
        {
            var notchIcons = new string('●', Mathf.Clamp(notchCost, 1, 12));
            notchInfo = notchCost == 1
                ? $"Notch Cost: {notchIcons}"
                : $"Notch Cost: {notchIcons} ({notchCost})";
        }

        SetTextValue(statusText, statusTextTMP, string.IsNullOrEmpty(notchInfo) ? status : $"{notchInfo}\\n{status}");
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

    private static void CopyLayoutComponents(RectTransform? source, RectTransform destination)
    {
        if (source == null || destination == null)
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
        if (grid != null)
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
        if (layoutGroup != null)
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
            if (!p)
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
        }) ?? panes.FirstOrDefault(p => p != null) ?? panes[0];
        RectTransform? templateRect = template != null ? ShadeInventoryPane.ResolveTemplateRootRectTransform(template) : null;
        Transform? parent = null;
        if (templateRect != null)
        {
            parent = templateRect.parent;
        }
        else if (template != null)
        {
            parent = template.transform.parent;
        }
        if (existingShade != null)
        {
            var shadeRect = existingShade.transform as RectTransform;
            if (templateRect != null && shadeRect != null)
            {
                CopyRectTransform(templateRect, shadeRect, copySiblingIndex: false);
                CopyLayoutComponents(templateRect, shadeRect);
            }
            ShadeInventoryPane.LogRectTransformHierarchy(templateRect, "TemplatePaneLive");
            if (shadeRect != null)
            {
                ShadeInventoryPane.LogRectTransformHierarchy(shadeRect, "ShadePaneBeforeForce");
            }

            existingShade.ConfigureFromTemplate(template);
            existingShade.SetDisplayLabel("Charms");
            existingShade.EnsureRootSizing();
            existingShade.ForceImmediateRefresh();
            existingShade.ForceLayoutRebuild();
            if (shadeRect != null)
            {
                ShadeInventoryPane.LogRectTransformHierarchy(shadeRect, "ShadePaneAfterForce");
            }
            ShadeInventoryPane.LogMenuEvent(FormattableString.Invariant(
                $"EnsurePane refreshed existing shade pane from template (active={existingShade.isActiveAndEnabled})"));
            return;
        }

        parent ??= paneList.transform;
        ShadeInventoryPane.LogMenuEvent($"Injecting shade pane using template '{template?.GetType().Name ?? "<null>"}'");

        var go = new GameObject("ShadeInventoryPane", typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        if (templateRect != null)
        {
            CopyRectTransform(templateRect, rect);
            CopyLayoutComponents(templateRect, rect);
        }
        else
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }
        ShadeInventoryPane.LogRectTransformHierarchy(templateRect, "TemplatePaneLive");
        ShadeInventoryPane.LogRectTransformHierarchy(rect, "ShadePaneBeforeForce");

        var canvasGroup = go.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        go.AddComponent<SimpleCanvasNestedFadeGroup>();

        var shadePane = go.AddComponent<ShadeInventoryPane>();
        shadePane.RootPane = shadePane;
        shadePane.ConfigureFromTemplate(template);
        shadePane.SetDisplayLabel("Charms");
        shadePane.EnsureRootSizing();
        shadePane.ForceImmediateRefresh();
        shadePane.ForceLayoutRebuild();
        ShadeInventoryPane.LogRectTransformHierarchy(rect, "ShadePaneAfterForce");

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


