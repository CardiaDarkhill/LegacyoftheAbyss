using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TeamCherry.NestedFadeGroup;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using LegacyoftheAbyss.Shade;

internal sealed class ShadeInventoryPane : InventoryPane
{
    private const int Columns = 7;
    private const float BackgroundAlpha = 0.82f;

    private static readonly Color DefaultPanelColor = new Color(0.05f, 0.05f, 0.08f, 0.92f);
    private static readonly Color DefaultHighlightColor = new Color(0.78f, 0.86f, 1f, 0.35f);
    private static readonly Color DefaultCellColor = new Color(0.18f, 0.2f, 0.26f, BackgroundAlpha);
    private static readonly Color DefaultNewMarkerColor = new Color(1f, 0.85f, 0.25f, 0.95f);

    private readonly List<CharmEntry> entries = new List<CharmEntry>();

    private RectTransform panelRoot = null!;
    private RectTransform contentRoot = null!;
    private RectTransform gridRoot = null!;
    private RectTransform highlight = null!;
    private Text titleText = null!;
    private Text notchText = null!;
    private Text detailTitleText = null!;
    private Text descriptionText = null!;
    private Text statusText = null!;
    private Text hintText = null!;
    private CanvasGroup canvasGroup = null!;

    private Font? bodyFont;
    private Font? headerFont;
    private Material? bodyFontMaterial;
    private Material? headerFontMaterial;
    private Color bodyFontColor = Color.white;
    private Color headerFontColor = Color.white;
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

    private struct CharmEntry
    {
        public ShadeCharmDefinition Definition;
        public ShadeCharmId Id;
        public RectTransform Root;
        public Image Icon;
        public Image Background;
        public GameObject NewMarker;
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
        if (titleText != null)
        {
            titleText.text = label;
        }
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
            statusText.text = "This charm has not been unlocked yet.";
            return;
        }

        string message;
        bool success = inventory.IsEquipped(id)
            ? inventory.TryUnequip(id, out message)
            : inventory.TryEquip(id, out message);

        statusText.text = message;
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

            if (titleText != null && !string.Equals(titleText.text, displayLabel, StringComparison.Ordinal))
            {
                titleText.text = displayLabel;
                changed = true;
            }

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
                    return;
                }

                if (string.Equals(current, "??/??", StringComparison.OrdinalIgnoreCase) ||
                    string.IsNullOrWhiteSpace(current))
                {
                    text.text = displayLabel;
                    changed = true;
                    break;
                }
            }
        }
        catch
        {
        }

        if (changed)
        {
            LogMenuEvent($"Label updated to '{displayLabel}'");
        }
    }

    private void SubscribeInput()
    {
        OnInputLeft += () => MoveSelectionHorizontal(-1);
        OnInputRight += () => MoveSelectionHorizontal(1);
        OnInputUp += () => MoveSelectionVertical(-1);
        OnInputDown += () => MoveSelectionVertical(1);
    }

    internal void ConfigureFromTemplate(InventoryPane template)
    {
        if (template == null)
        {
            return;
        }

        try
        {
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
        panelRoot.anchorMin = Vector2.zero;
        panelRoot.anchorMax = Vector2.one;
        panelRoot.offsetMin = new Vector2(36f, 24f);
        panelRoot.offsetMax = new Vector2(-36f, -36f);

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
        contentRoot.anchorMin = Vector2.zero;
        contentRoot.anchorMax = Vector2.one;
        contentRoot.offsetMin = new Vector2(30f, 30f);
        contentRoot.offsetMax = new Vector2(-30f, -30f);

        titleText = CreateText("Title", contentRoot, FontStyle.Normal, 46, TextAnchor.UpperLeft, useHeaderFont: true);
        var titleRect = titleText.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.offsetMin = new Vector2(0f, -52f);
        titleRect.offsetMax = new Vector2(0f, -4f);
        titleText.text = displayLabel;

        notchText = CreateText("Notches", contentRoot, FontStyle.Normal, 32, TextAnchor.UpperRight);
        var notchRect = notchText.rectTransform;
        notchRect.anchorMin = new Vector2(0.45f, 1f);
        notchRect.anchorMax = new Vector2(1f, 1f);
        notchRect.pivot = new Vector2(1f, 1f);
        notchRect.offsetMin = new Vector2(-12f, -52f);
        notchRect.offsetMax = new Vector2(0f, -6f);

        gridRoot = new GameObject("CharmGrid", typeof(RectTransform)).GetComponent<RectTransform>();
        gridRoot.SetParent(contentRoot, false);
        gridRoot.anchorMin = new Vector2(0f, 0f);
        gridRoot.anchorMax = new Vector2(0.58f, 0.9f);
        gridRoot.pivot = new Vector2(0f, 1f);
        gridRoot.offsetMin = new Vector2(0f, 0f);
        gridRoot.offsetMax = new Vector2(-24f, -96f);
        gridRoot.anchoredPosition = new Vector2(0f, -72f);

        var gridLayout = gridRoot.gameObject.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(104f, 112f);
        gridLayout.spacing = new Vector2(16f, 16f);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = Columns;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;

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
        detailRoot.anchorMin = new Vector2(0.6f, 0f);
        detailRoot.anchorMax = new Vector2(1f, 0.9f);
        detailRoot.pivot = new Vector2(0f, 1f);
        detailRoot.offsetMin = new Vector2(20f, 0f);
        detailRoot.offsetMax = new Vector2(0f, -64f);

        detailTitleText = CreateText("CharmName", detailRoot, FontStyle.Normal, 38, TextAnchor.UpperLeft, useHeaderFont: true);
        var detailTitleRect = detailTitleText.rectTransform;
        detailTitleRect.anchorMin = new Vector2(0f, 0.72f);
        detailTitleRect.anchorMax = new Vector2(1f, 0.98f);
        detailTitleRect.pivot = new Vector2(0f, 1f);
        detailTitleRect.offsetMin = Vector2.zero;
        detailTitleRect.offsetMax = new Vector2(0f, -2f);
        detailTitleText.text = displayLabel;

        descriptionText = CreateText("Description", detailRoot, FontStyle.Normal, 30, TextAnchor.UpperLeft);
        var descRect = descriptionText.rectTransform;
        descRect.anchorMin = new Vector2(0f, 0.32f);
        descRect.anchorMax = new Vector2(1f, 0.74f);
        descRect.offsetMin = Vector2.zero;
        descRect.offsetMax = Vector2.zero;
        descriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        descriptionText.verticalOverflow = VerticalWrapMode.Overflow;
        descriptionText.lineSpacing = 1.1f;

        statusText = CreateText("Status", detailRoot, FontStyle.Italic, 28, TextAnchor.UpperLeft);
        var statusRect = statusText.rectTransform;
        statusRect.anchorMin = new Vector2(0f, 0.16f);
        statusRect.anchorMax = new Vector2(1f, 0.32f);
        statusRect.offsetMin = new Vector2(0f, 6f);
        statusRect.offsetMax = Vector2.zero;

        hintText = CreateText("Hint", detailRoot, FontStyle.Normal, 24, TextAnchor.UpperLeft);
        var hintRect = hintText.rectTransform;
        hintRect.anchorMin = new Vector2(0f, 0f);
        hintRect.anchorMax = new Vector2(1f, 0.16f);
        hintRect.offsetMin = new Vector2(0f, 6f);
        hintRect.offsetMax = Vector2.zero;
        hintText.text = "Submit to equip or unequip. Ctrl + ` unlocks all charms (debug).";
        hintText.color = new Color(0.78f, 0.82f, 0.92f, 0.8f);

        isBuilt = true;
        LogMenuEvent("BuildUI complete");
    }

    private Text CreateText(string name, RectTransform parent, FontStyle style, int size, TextAnchor anchor, bool useHeaderFont = false)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
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
        if (notchText == null)
        {
            return;
        }

        if (inventory == null)
        {
            notchText.text = "Shade charm inventory unavailable.";
        }
        else
        {
            notchText.text = $"Notches Used: {inventory.UsedNotches}/{inventory.NotchCapacity}";
        }
    }

    private void LateUpdate()
    {
        if (!isActive)
        {
            return;
        }

        if (titleText != null && !string.Equals(titleText.text, displayLabel, StringComparison.Ordinal))
        {
            titleText.text = displayLabel;
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
            detailTitleText.text = displayLabel;
            descriptionText.text = "Collect shade charms to unlock new abilities for your companion.";
            statusText.text = string.Empty;
            return;
        }

        var entry = entries[Mathf.Clamp(selectedIndex, 0, entries.Count - 1)];
        var definition = entry.Definition;
        detailTitleText.text = definition?.DisplayName ?? displayLabel;
        descriptionText.text = definition?.Description ?? string.Empty;

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

        statusText.text = string.IsNullOrEmpty(notchInfo) ? status : $"{notchInfo}\\n{status}";
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

    internal static void EnsurePane(InventoryPaneList paneList)
    {
        if (paneList == null)
        {
            ShadeInventoryPane.LogMenuEvent("EnsurePane skipped: paneList null");
            return;
        }

        var panes = PanesField(paneList);
        if (panes != null && panes.Any(p => p && p.TryGetComponent<ShadeInventoryPane>(out _)))
        {
            ShadeInventoryPane.LogMenuEvent("EnsurePane skipped: shade pane already present");
            return;
        }

        if (panes == null || panes.Length == 0)
        {
            ShadeInventoryPane.LogMenuEvent("EnsurePane skipped: no template panes available");
            return;
        }

        var template = panes[0];
        var templateRect = template.GetComponent<RectTransform>();
        var parent = templateRect != null ? templateRect.parent : template.transform.parent;
        ShadeInventoryPane.LogMenuEvent($"Injecting shade pane using template '{template?.GetType().Name ?? "<null>"}'");

        var go = new GameObject("ShadeInventoryPane", typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        if (templateRect != null)
        {
            rect.anchorMin = templateRect.anchorMin;
            rect.anchorMax = templateRect.anchorMax;
            rect.pivot = templateRect.pivot;
            rect.sizeDelta = templateRect.sizeDelta;
            rect.localScale = templateRect.localScale;
            rect.localPosition = templateRect.localPosition;
        }
        else
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
        }

        var canvasGroup = go.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        go.AddComponent<SimpleCanvasNestedFadeGroup>();

        var shadePane = go.AddComponent<ShadeInventoryPane>();
        shadePane.RootPane = shadePane;
        shadePane.ConfigureFromTemplate(template);
        shadePane.SetDisplayLabel("Charms");
        shadePane.ForceImmediateRefresh();

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


