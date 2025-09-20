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
    private const int Columns = 3;
    private const float BackgroundAlpha = 0.82f;

    private readonly List<CharmEntry> entries = new List<CharmEntry>();

    private RectTransform contentRoot;
    private RectTransform gridRoot;
    private RectTransform highlight;
    private Text notchText;
    private Text nameText;
    private Text descriptionText;
    private Text statusText;
    private Text hintText;
    private CanvasGroup canvasGroup;

    private ShadeCharmInventory inventory;
    private int selectedIndex;
    private bool isBuilt;
    private bool isActive;
    private Sprite fallbackSprite;
    private string displayLabel = "Shade";

    private struct CharmEntry
    {
        public ShadeCharmDefinition Definition;
        public ShadeCharmId Id;
        public RectTransform Root;
        public Image Icon;
        public Image Background;
        public GameObject NewMarker;
    }

    public override void Awake()
    {
        base.Awake();
        BuildUI();
        SubscribeInput();
    }

    private void OnEnable()
    {
        ShadeCharmInventory inv = ShadeRuntime.Charms;
        if (inv != null)
        {
            inv.StateChanged += HandleInventoryChanged;
        }
        inventory = inv;
        RefreshAll();
    }

    private void OnDisable()
    {
        ShadeCharmInventory inv = ShadeRuntime.Charms;
        if (inv != null)
        {
            inv.StateChanged -= HandleInventoryChanged;
        }
        isActive = false;
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
        canvasGroup ??= GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        inventory = ShadeRuntime.Charms;
        isActive = true;
        RefreshAll();
        SelectIndex(selectedIndex < entries.Count ? selectedIndex : 0);
    }

    public override void PaneEnd()
    {
        isActive = false;
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        base.PaneEnd();
    }

    internal void SetDisplayLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return;
        }

        displayLabel = label;
        if (nameText != null)
        {
            nameText.text = label;
        }
    }

    internal string DisplayLabel => displayLabel;

    internal void HandleSubmit()
    {
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

    private void SubscribeInput()
    {
        OnInputLeft += () => MoveSelectionHorizontal(-1);
        OnInputRight += () => MoveSelectionHorizontal(1);
        OnInputUp += () => MoveSelectionVertical(-1);
        OnInputDown += () => MoveSelectionVertical(1);
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

        contentRoot = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
        contentRoot.SetParent(transform, false);
        contentRoot.anchorMin = Vector2.zero;
        contentRoot.anchorMax = Vector2.one;
        contentRoot.offsetMin = new Vector2(80f, 60f);
        contentRoot.offsetMax = new Vector2(-80f, -80f);

        nameText = CreateText("Title", contentRoot, FontStyle.Bold, 40, TextAnchor.UpperLeft);
        var titleRect = nameText.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.offsetMin = new Vector2(0f, -44f);
        titleRect.offsetMax = Vector2.zero;
        nameText.text = displayLabel;

        notchText = CreateText("Notch", contentRoot, FontStyle.Normal, 30, TextAnchor.UpperLeft);
        var notchRect = notchText.rectTransform;
        notchRect.anchorMin = new Vector2(0f, 1f);
        notchRect.anchorMax = new Vector2(1f, 1f);
        notchRect.pivot = new Vector2(0f, 1f);
        notchRect.offsetMin = new Vector2(0f, -92f);
        notchRect.offsetMax = new Vector2(0f, -44f);

        gridRoot = new GameObject("CharmGrid", typeof(RectTransform)).GetComponent<RectTransform>();
        gridRoot.SetParent(contentRoot, false);
        gridRoot.anchorMin = new Vector2(0f, 0f);
        gridRoot.anchorMax = new Vector2(0.6f, 0.88f);
        gridRoot.pivot = new Vector2(0f, 1f);
        gridRoot.offsetMin = new Vector2(0f, 0f);
        gridRoot.offsetMax = new Vector2(-24f, -140f);
        gridRoot.anchoredPosition = new Vector2(0f, -120f);

        var gridLayout = gridRoot.gameObject.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(150f, 160f);
        gridLayout.spacing = new Vector2(32f, 28f);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = Columns;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;

        highlight = new GameObject("Highlight", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        highlight.SetParent(gridRoot, false);
        var highlightImage = highlight.GetComponent<Image>();
        highlightImage.color = new Color(0.8f, 0.9f, 1f, 0.28f);
        highlightImage.raycastTarget = false;
        highlight.gameObject.SetActive(false);

        var detailRoot = new GameObject("Details", typeof(RectTransform)).GetComponent<RectTransform>();
        detailRoot.SetParent(contentRoot, false);
        detailRoot.anchorMin = new Vector2(0.62f, 0f);
        detailRoot.anchorMax = new Vector2(1f, 0.88f);
        detailRoot.pivot = new Vector2(0f, 1f);
        detailRoot.offsetMin = new Vector2(24f, 0f);
        detailRoot.offsetMax = Vector2.zero;

        descriptionText = CreateText("Description", detailRoot, FontStyle.Normal, 28, TextAnchor.UpperLeft);
        var descRect = descriptionText.rectTransform;
        descRect.anchorMin = new Vector2(0f, 0.45f);
        descRect.anchorMax = new Vector2(1f, 1f);
        descRect.offsetMin = Vector2.zero;
        descRect.offsetMax = Vector2.zero;
        descriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        descriptionText.verticalOverflow = VerticalWrapMode.Overflow;
        descriptionText.lineSpacing = 1.15f;

        statusText = CreateText("Status", detailRoot, FontStyle.Italic, 26, TextAnchor.UpperLeft);
        var statusRect = statusText.rectTransform;
        statusRect.anchorMin = new Vector2(0f, 0.25f);
        statusRect.anchorMax = new Vector2(1f, 0.45f);
        statusRect.offsetMin = new Vector2(0f, 10f);
        statusRect.offsetMax = Vector2.zero;

        hintText = CreateText("Hint", detailRoot, FontStyle.Normal, 24, TextAnchor.UpperLeft);
        var hintRect = hintText.rectTransform;
        hintRect.anchorMin = new Vector2(0f, 0f);
        hintRect.anchorMax = new Vector2(1f, 0.25f);
        hintRect.offsetMin = new Vector2(0f, 10f);
        hintRect.offsetMax = Vector2.zero;
        hintText.text = "Ctrl + ` to unlock all charms (debug). Use submit to equip or unequip.";
        hintText.color = new Color(0.78f, 0.82f, 0.92f, 0.8f);

        isBuilt = true;
    }

    private Text CreateText(string name, RectTransform parent, FontStyle style, int size, TextAnchor anchor)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        var text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontStyle = style;
        text.fontSize = size;
        text.alignment = anchor;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private void MoveSelectionHorizontal(int direction)
    {
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
        ShadeCharmInventory inv = ShadeRuntime.Charms;
        inventory = inv;
        var definitions = inv != null ? inv.AllCharms : Array.Empty<ShadeCharmDefinition>();
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

        if (selectedIndex >= entries.Count)
        {
            selectedIndex = entries.Count > 0 ? entries.Count - 1 : 0;
        }

        UpdateNotchMeter();
        RefreshEntryStates();
        UpdateDetailPanel();
    }

    private void HandleStateChanged()
    {
        RefreshEntryStates();
        UpdateNotchMeter();
        UpdateDetailPanel();
    }

    private void EnsureEntryCount(int count)
    {
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
    }

    private CharmEntry CreateEntry(int index)
    {
        var cell = new GameObject($"CharmCell_{index}", typeof(RectTransform));
        var cellRect = cell.GetComponent<RectTransform>();
        cellRect.SetParent(gridRoot, false);
        cellRect.localScale = Vector3.one;

        var background = cell.AddComponent<Image>();
        background.color = new Color(0.18f, 0.2f, 0.26f, BackgroundAlpha);
        background.raycastTarget = false;

        var iconGo = new GameObject("Icon", typeof(RectTransform));
        var iconRect = iconGo.GetComponent<RectTransform>();
        iconRect.SetParent(cellRect, false);
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.sizeDelta = new Vector2(120f, 120f);
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
        markerImage.color = new Color(1f, 0.85f, 0.25f, 0.95f);
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

    private void UpdateDetailPanel()
    {
        if (entries.Count == 0 || inventory == null)
        {
            descriptionText.text = "Collect shade charms to unlock new abilities for your companion.";
            statusText.text = "";
            return;
        }

        var entry = entries[Mathf.Clamp(selectedIndex, 0, entries.Count - 1)];
        var definition = entry.Definition;
        nameText.text = definition?.DisplayName ?? displayLabel;
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
        statusText.text = status;
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
        return fallbackSprite;
    }
}

internal sealed class SimpleCanvasNestedFadeGroup : NestedFadeGroupBase
{
    [SerializeField]
    private CanvasGroup canvasGroup;

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

    internal static void EnsurePane(InventoryPaneList paneList)
    {
        if (paneList == null)
        {
            return;
        }

        var panes = PanesField(paneList);
        if (panes != null && panes.Any(p => p && p.TryGetComponent<ShadeInventoryPane>(out _)))
        {
            return;
        }

        if (panes == null || panes.Length == 0)
        {
            return;
        }

        var template = panes[0];
        var templateRect = template.GetComponent<RectTransform>();
        var parent = templateRect != null ? templateRect.parent : template.transform.parent;

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

        var input = go.AddComponent<InventoryPaneInput>();
        ConfigureInput(input);

        var shadePane = go.AddComponent<ShadeInventoryPane>();
        shadePane.RootPane = shadePane;
        shadePane.SetDisplayLabel("Shade Charms");

        PlayerDataTestField(shadePane) = new PlayerDataTest();
        HasNewPdField(shadePane) = string.Empty;

        var icon = ShadeCharmIconLoader.TryLoadIcon("shade_tab", "shade_charm_void_heart", "void_heart", "shade");
        if (icon == null && ShadeRuntime.Charms != null)
        {
            icon = ShadeRuntime.Charms.AllCharms.FirstOrDefault()?.Icon;
        }
        if (icon != null)
        {
            ListIconField(shadePane) = icon;
        }

        var newList = panes.ToList();

        int insertIndex = -1;
        for (int i = 0; i < panes.Length; i++)
        {
            var existing = panes[i];
            if (!existing)
            {
                continue;
            }

            string name = existing.gameObject != null ? existing.gameObject.name : existing.name;
            if (!string.IsNullOrEmpty(name))
            {
                if (name.IndexOf("Charm", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    name.IndexOf("Tool", StringComparison.OrdinalIgnoreCase) >= 0)
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
    }

    private static void ConfigureInput(InventoryPaneInput input)
    {
        if (input == null)
        {
            return;
        }

        PaneControlField(input) = InventoryPaneList.PaneTypes.None;
        AllowHorizontalField?.SetValue(input, true);
        AllowVerticalField?.SetValue(input, true);
        AllowRepeatField?.SetValue(input, true);
        AllowRepeatSubmitField?.SetValue(input, false);
        AllowRightStickField?.SetValue(input, false);
    }
}


