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
    private static ShadeInventoryInputDriver? inputDriver;

    private static void EnsureInputDriver()
    {
        if (inputDriver != null)
        {
            return;
        }

        try
        {
            var host = new GameObject("ShadeInventoryInputDriver");
            host.hideFlags = HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(host);
            inputDriver = host.AddComponent<ShadeInventoryInputDriver>();
            ShadeInventoryPane.LogMenuEvent("Created shade inventory input driver");
        }
        catch (Exception ex)
        {
            ShadeInventoryPane.LogMenuEvent(FormattableString.Invariant(
                $"Failed to create shade inventory input driver: {ex.GetType().Name} {ex.Message}"));
        }
    }

    private sealed class InputBindingSnapshot
    {
        public InputBindingSnapshot(
            InventoryPaneBase? pane,
            InventoryPaneList? paneList,
            bool allowHorizontal,
            bool allowVertical,
            bool allowRepeat,
            bool allowRepeatSubmit,
            bool allowRightStick,
            InventoryPaneList.PaneTypes paneControl,
            bool enabled)
        {
            Pane = pane;
            PaneList = paneList;
            AllowHorizontal = allowHorizontal;
            AllowVertical = allowVertical;
            AllowRepeat = allowRepeat;
            AllowRepeatSubmit = allowRepeatSubmit;
            AllowRightStick = allowRightStick;
            PaneControl = paneControl;
            Enabled = enabled;
        }

        public InventoryPaneBase? Pane { get; }

        public InventoryPaneList? PaneList { get; }

        public bool AllowHorizontal { get; }

        public bool AllowVertical { get; }

        public bool AllowRepeat { get; }

        public bool AllowRepeatSubmit { get; }

        public bool AllowRightStick { get; }

        public InventoryPaneList.PaneTypes PaneControl { get; }

        public bool Enabled { get; }
    }

    private static readonly Dictionary<InventoryPaneInput, InputBindingSnapshot> OriginalInputBindings =
        new Dictionary<InventoryPaneInput, InputBindingSnapshot>();

    private static readonly Dictionary<ShadeInventoryPane, HashSet<InventoryPaneInput>> CapturedInputs =
        new Dictionary<ShadeInventoryPane, HashSet<InventoryPaneInput>>();

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

        EnsureInputDriver();

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
            BindInput(existingShade, paneList, captureFocus: existingShade.IsPaneActive);
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
        ConfigureInput(input, paneList, shadePane, captureFocus: false, isLocal: true);
        BindInput(shadePane, paneList, captureFocus: false);

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

    private static bool TryGetBool(FieldInfo? field, InventoryPaneInput input, bool defaultValue)
    {
        if (field == null)
        {
            return defaultValue;
        }

        try
        {
            object? value = field.GetValue(input);
            if (value is bool flag)
            {
                return flag;
            }
        }
        catch
        {
        }

        return defaultValue;
    }

    private static InputBindingSnapshot CreateSnapshot(InventoryPaneInput input)
    {
        InventoryPaneBase? pane = null;
        if (PaneField != null)
        {
            try { pane = PaneField.GetValue(input) as InventoryPaneBase; }
            catch { pane = null; }
        }

        InventoryPaneList? paneList = null;
        if (PaneListField != null)
        {
            try { paneList = PaneListField.GetValue(input) as InventoryPaneList; }
            catch { paneList = null; }
        }

        bool allowHorizontal = TryGetBool(AllowHorizontalField, input, true);
        bool allowVertical = TryGetBool(AllowVerticalField, input, true);
        bool allowRepeat = TryGetBool(AllowRepeatField, input, false);
        bool allowRepeatSubmit = TryGetBool(AllowRepeatSubmitField, input, false);
        bool allowRightStick = TryGetBool(AllowRightStickField, input, false);

        InventoryPaneList.PaneTypes paneControl = InventoryPaneList.PaneTypes.None;
        if (PaneControlField != null)
        {
            try { paneControl = PaneControlField(input); }
            catch { paneControl = InventoryPaneList.PaneTypes.None; }
        }

        bool enabled = false;
        try { enabled = input.enabled; }
        catch { enabled = false; }

        return new InputBindingSnapshot(
            pane,
            paneList,
            allowHorizontal,
            allowVertical,
            allowRepeat,
            allowRepeatSubmit,
            allowRightStick,
            paneControl,
            enabled);
    }

    private static void StoreOriginalBinding(InventoryPaneInput input)
    {
        if (input == null || OriginalInputBindings.ContainsKey(input))
        {
            return;
        }

        try
        {
            OriginalInputBindings[input] = CreateSnapshot(input);
        }
        catch
        {
        }
    }

    private static void ApplyShadeInputSettings(InventoryPaneInput input)
    {
        if (input == null)
        {
            return;
        }

        try
        {
            if (!input.enabled)
            {
                input.enabled = true;
            }
        }
        catch
        {
        }

        if (PaneControlField != null)
        {
            try { PaneControlField(input) = InventoryPaneList.PaneTypes.None; }
            catch { }
        }

        AllowHorizontalField?.SetValue(input, true);
        AllowVerticalField?.SetValue(input, true);
        AllowRepeatField?.SetValue(input, true);
        AllowRepeatSubmitField?.SetValue(input, false);
        AllowRightStickField?.SetValue(input, false);
    }

    private static void TrackCapturedInput(ShadeInventoryPane? shadePane, InventoryPaneInput? input)
    {
        if (shadePane == null || input == null)
        {
            return;
        }

        if (!CapturedInputs.TryGetValue(shadePane, out var inputs))
        {
            inputs = new HashSet<InventoryPaneInput>();
            CapturedInputs[shadePane] = inputs;
        }

        inputs.RemoveWhere(candidate => candidate == null);
        if (inputs.Add(input))
        {
            shadePane.RegisterBoundInput(input);
        }
    }

    private static void RestoreSingleInput(ShadeInventoryPane? shadePane, InventoryPaneInput? input)
    {
        if (shadePane == null)
        {
            return;
        }

        if (input == null)
        {
            return;
        }

        try
        {
            if (OriginalInputBindings.TryGetValue(input, out var snapshot))
            {
                if (PaneField != null)
                {
                    try { PaneField.SetValue(input, snapshot.Pane); }
                    catch { }
                }

                if (PaneListField != null)
                {
                    try { PaneListField.SetValue(input, snapshot.PaneList); }
                    catch { }
                }

                AllowHorizontalField?.SetValue(input, snapshot.AllowHorizontal);
                AllowVerticalField?.SetValue(input, snapshot.AllowVertical);
                AllowRepeatField?.SetValue(input, snapshot.AllowRepeat);
                AllowRepeatSubmitField?.SetValue(input, snapshot.AllowRepeatSubmit);
                AllowRightStickField?.SetValue(input, snapshot.AllowRightStick);

                if (PaneControlField != null)
                {
                    try { PaneControlField(input) = snapshot.PaneControl; }
                    catch { }
                }

                try { input.enabled = snapshot.Enabled; }
                catch { }

                OriginalInputBindings.Remove(input);
            }
            else if (PaneField != null)
            {
                try { PaneField.SetValue(input, null); }
                catch { }
            }
        }
        catch
        {
        }

        shadePane.UnregisterBoundInput(input);
    }

    internal static void RestoreInputBindings(ShadeInventoryPane shadePane)
    {
        if (shadePane == null)
        {
            return;
        }

        if (!CapturedInputs.TryGetValue(shadePane, out var inputs) || inputs.Count == 0)
        {
            shadePane.ClearBoundInputs();
            return;
        }

        var toRestore = new List<InventoryPaneInput>(inputs);
        foreach (var input in toRestore)
        {
            RestoreSingleInput(shadePane, input);
            inputs.Remove(input);
        }

        if (inputs.Count == 0)
        {
            CapturedInputs.Remove(shadePane);
        }

        shadePane.ClearBoundInputs();
    }

    internal static void BindInput(ShadeInventoryPane shadePane, InventoryPaneList paneList, bool captureFocus)
    {
        if (shadePane == null || paneList == null)
        {
            return;
        }

        EnsureInputDriver();

        if (captureFocus)
        {
            CapturedInputs.Remove(shadePane);
            shadePane.ClearBoundInputs();
        }

        try
        {
            var inputs = shadePane.GetComponents<InventoryPaneInput>();
            if (inputs != null)
            {
                foreach (var input in inputs)
                {
                    if (input == null)
                    {
                        continue;
                    }

                    ConfigureInput(input, paneList, shadePane, captureFocus, isLocal: true);
                }
            }
        }
        catch
        {
        }

        try
        {
            var sharedInputs = paneList.GetComponentsInChildren<InventoryPaneInput>(true);
            if (sharedInputs == null)
            {
                return;
            }

            foreach (var input in sharedInputs)
            {
                if (input == null)
                {
                    continue;
                }

                InventoryPaneBase? currentPane = null;
                if (PaneField != null)
                {
                    try
                    {
                        currentPane = PaneField.GetValue(input) as InventoryPaneBase;
                    }
                    catch
                    {
                        currentPane = null;
                    }
                }

                bool currentlyShade = ReferenceEquals(currentPane, shadePane);
                if (!captureFocus)
                {
                    if (currentlyShade)
                    {
                        ConfigureInput(input, paneList, shadePane, captureFocus: false, isLocal: false);
                    }
                    continue;
                }

                if (currentPane is ShadeInventoryPane existingShade && !ReferenceEquals(existingShade, shadePane))
                {
                    continue;
                }

                ConfigureInput(input, paneList, shadePane, captureFocus: true, isLocal: false);
            }
        }
        catch
        {
        }
    }

    private static void ConfigureInput(
        InventoryPaneInput input,
        InventoryPaneList paneList,
        ShadeInventoryPane shadePane,
        bool captureFocus,
        bool isLocal)
    {
        if (input == null)
        {
            ShadeInventoryPane.LogMenuEvent("ConfigureInput skipped: input null");
            return;
        }

        InventoryPaneBase? currentPane = null;
        if (PaneField != null)
        {
            try
            {
                currentPane = PaneField.GetValue(input) as InventoryPaneBase;
            }
            catch
            {
                currentPane = null;
            }
        }

        bool alreadyShade = ReferenceEquals(currentPane, shadePane);
        if (!isLocal && !captureFocus && !alreadyShade)
        {
            return;
        }

        if (captureFocus)
        {
            StoreOriginalBinding(input);
        }

        if (captureFocus || isLocal)
        {
            ApplyShadeInputSettings(input);
        }

        if (shadePane != null && PaneField != null)
        {
            try
            {
                if (!ReferenceEquals(currentPane, shadePane))
                {
                    PaneField.SetValue(input, shadePane);
                    ShadeInventoryPane.LogMenuEvent("Bound InventoryPaneInput to shade pane");
                }
            }
            catch
            {
            }
        }

        if (paneList != null && PaneListField != null)
        {
            try
            {
                PaneListField.SetValue(input, paneList);
            }
            catch
            {
            }
        }

        if (captureFocus)
        {
            TrackCapturedInput(shadePane, input);
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

    private sealed class ShadeInventoryInputDriver : MonoBehaviour
    {
        private void Awake()
        {
            hideFlags |= HideFlags.HideAndDontSave;
        }

        private void Update()
        {
            var pane = ShadeInventoryPane.ActivePane;
            if (pane == null)
            {
                return;
            }

            try
            {
                pane.ProcessShadeInputTick();
            }
            catch (Exception ex)
            {
                ShadeInventoryPane.LogMenuEvent(FormattableString.Invariant(
                    $"ShadeInventoryInputDriver exception {ex.GetType().Name}: {ex.Message}"));
            }
        }

        private void OnDestroy()
        {
            ShadeInventoryPaneIntegration.inputDriver = null;
        }
    }
}


