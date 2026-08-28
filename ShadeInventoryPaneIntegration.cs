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
            foreach (var existing in destination.GetComponents<GridLayoutGroup>())
            {
                if (existing != null) UnityEngine.Object.Destroy(existing);
            }
        }

        if (!copyLayoutGroups)
        {
            foreach (var group in destination.GetComponents<LayoutGroup>())
            {
                // The grids were already dealt with above; do not destroy them twice.
                if (group == null || (!copyGridLayout && group is GridLayoutGroup))
                {
                    continue;
                }

                UnityEngine.Object.Destroy(group);
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

        DisplayNameField(pane) = new LocalisedString(string.Empty, label ?? string.Empty);
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

        var display = PaneListDisplayField != null ? PaneListDisplayField(paneList) : null;
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

    /// <summary>
    /// Keeps the Shade pane a <i>sibling</i> of the real panes rather than letting it end up nested
    /// inside one of them.
    /// <para>
    /// The parent comes from <c>ShadeInventoryPane.ResolveTemplateRootRectTransform</c>, which walks
    /// into a pane's children when the pane's own transform is not a <c>RectTransform</c> - so
    /// <c>templateRect.parent</c> can land <i>inside</i> the template pane rather than beside it. A
    /// Shade pane parented there inherits that pane's active state, and goes inactive with it when
    /// the inventory FSM deactivates the pane you came from, producing an <c>OnDisable</c> with no
    /// matching <c>PaneEnd</c> from that one source pane.
    /// </para>
    /// <para>
    /// Walking up to the first ancestor that is not itself a pane is a no-op when the resolved
    /// parent was already the panes' shared container.
    /// </para>
    /// </summary>
    private static Transform? ResolvePaneSiblingParent(Transform? candidate, InventoryPane templatePane, InventoryPane[] panes)
    {
        if (candidate == null || templatePane == null)
        {
            return candidate;
        }

        Transform? node = candidate;
        while (node != null && IsInsideAnyPane(node, panes))
        {
            node = node.parent;
        }

        if (node == null || node == candidate)
        {
            return candidate;
        }

        ShadeInventoryPane.LogMenuEvent(FormattableString.Invariant(
            $"Shade pane parent moved out of '{candidate.name}' to '{node.name}' to keep it a sibling of the real panes"));
        return node;
    }


    private static bool IsInsideAnyPane(Transform node, InventoryPane[] panes)
    {
        if (node == null || panes == null)
        {
            return false;
        }

        foreach (var pane in panes)
        {
            if (pane == null || pane is ShadeInventoryPane)
            {
                continue;
            }

            var paneTransform = pane.transform;
            if (paneTransform == null)
            {
                continue;
            }

            // node == paneTransform means "the panes' container" was resolved as the pane itself,
            // which is just as wrong as being one of its descendants.
            if (node == paneTransform || node.IsChildOf(paneTransform))
            {
                return true;
            }
        }

        return false;
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
                (goName.Contains("Charm", StringComparison.OrdinalIgnoreCase) ||
                 goName.Contains("Crest", StringComparison.OrdinalIgnoreCase));
            bool matchesType = !string.IsNullOrEmpty(typeName) &&
                (typeName.Contains("Charm", StringComparison.OrdinalIgnoreCase) ||
                 typeName.Contains("Crest", StringComparison.OrdinalIgnoreCase));
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

        parent = ResolvePaneSiblingParent(parent, templatePane, panes);

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
            icon = charms.AllCharms.Count > 0 ? charms.AllCharms[0].Icon : null;
        }
        AssignListIcon(shadePane, icon);

        var newList = panes.ToList();

        // Append, never insert. InventoryPaneList.panes is an ArrayForEnum array - index i *is*
        // PaneTypes value i, and GetPane, "Target Pane Index" and ListenForInventoryShortcut all
        // address panes by that number. Inserting mid-list shifts every later pane and silently
        // sends each shortcut to the wrong tab. Appending parks the Shade at an index the enum
        // never names; it stays reachable by cycling, which is the only route it ever had.
        newList.Add(shadePane);
        PanesField(paneList) = newList.ToArray();
        RefreshPaneListDisplay(paneList, newList);
        ShadeInventoryPane.LogMenuEvent($"Shade pane appended at index {newList.Count - 1}; total panes={newList.Count}");
        LogPaneLayout(newList);
    }

    private static string? s_loggedPaneLayout;

    /// <summary>
    /// Dumps the final pane order once per distinct layout, at Info level rather than behind
    /// <c>logMenu</c>: when a shortcut opens the wrong tab, this line says whether
    /// <c>InventoryPaneList.panes</c> still lines up with <c>PaneTypes</c>, which is the cause
    /// every time.
    /// </summary>
    private static void LogPaneLayout(List<InventoryPane> panes)
    {
        try
        {
            if (panes == null)
            {
                return;
            }

            var builder = new System.Text.StringBuilder("Inventory pane layout:");
            for (int i = 0; i < panes.Count; i++)
            {
                var pane = panes[i];
                string name = pane == null
                    ? "<null>"
                    : (pane.gameObject != null ? pane.gameObject.name : pane.name);
                string typeName = pane == null ? "-" : pane.GetType().Name;
                bool available = false;
                try { available = pane != null && pane.IsAvailable; } catch { available = false; }
                builder.Append(FormattableString.Invariant($" [{i}]{name}({typeName}){(available ? string.Empty : "*locked")}"));
            }

            string layout = builder.ToString();
            if (string.Equals(layout, s_loggedPaneLayout, StringComparison.Ordinal))
            {
                return;
            }

            s_loggedPaneLayout = layout;
            LegacyHelper.LogInfo(layout);
        }
        catch
        {
        }
    }

    /// <summary>Reads a bool field that may not have resolved against this build of the game.</summary>
    private static bool TryGetBool(FieldInfo? field, InventoryPaneInput input, bool defaultValue)
    {
        return field?.GetValue(input) is bool flag ? flag : defaultValue;
    }

    /// <summary>
    /// What an input looked like before the Shade pane borrowed it, so
    /// <see cref="RestoreSingleInput"/> can put it back exactly.
    /// </summary>
    private static InputBindingSnapshot CreateSnapshot(InventoryPaneInput input)
    {
        return new InputBindingSnapshot(
            PaneField?.GetValue(input) as InventoryPaneBase,
            PaneListField?.GetValue(input) as InventoryPaneList,
            TryGetBool(AllowHorizontalField, input, true),
            TryGetBool(AllowVerticalField, input, true),
            TryGetBool(AllowRepeatField, input, false),
            TryGetBool(AllowRepeatSubmitField, input, false),
            TryGetBool(AllowRightStickField, input, false),
            PaneControlField != null ? PaneControlField(input) : InventoryPaneList.PaneTypes.None,
            input.enabled);
    }

    private static void StoreOriginalBinding(InventoryPaneInput input)
    {
        if (input == null || OriginalInputBindings.ContainsKey(input))
        {
            return;
        }

        OriginalInputBindings[input] = CreateSnapshot(input);
    }

    /// <summary>
    /// Points <paramref name="input"/> at the Shade pane's input conventions.
    /// <para>
    /// <paramref name="isLocal"/> gates the one setting that must <b>not</b> be applied to a borrowed
    /// input: <c>paneControl</c>. <c>InventoryPaneInput.Update</c> switches on it to tell "the player
    /// pressed *this* pane's own shortcut, so close the inventory" from "the player pressed a
    /// *different* pane's shortcut, so switch to it", and its <c>PaneTypes.None</c> case treats
    /// <i>every</i> shortcut as this pane's own - an unconditional cancel. Write None onto the real
    /// panes' inputs and a single missed restore leaves all six inventory shortcuts closing the
    /// inventory instead of switching tabs, on every tab, for the rest of the session.
    /// </para>
    /// <para>
    /// Nothing needs that write: Submit/Direction routing to the Shade goes through the <c>pane</c>
    /// field, so a borrowed input keeps working with its own <c>paneControl</c> untouched - and a
    /// missed restore cannot break anything, because there is nothing to restore.
    /// </para>

    /// </summary>
    private static void ApplyShadeInputSettings(InventoryPaneInput input, bool isLocal)
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

        if (isLocal && PaneControlField != null)
        {
            // The Shade has no PaneTypes value of its own, so its own input component stays None.
            // TryJumpToShadeTab uses that as the "this is the Shade's own phantom input, not a real
            // pane's" marker, and TryHandleShadeTabPaneShortcut covers the shortcut handling that
            // None would otherwise turn into a cancel.
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
        if (input == null)
        {
            return;
        }

        if (OriginalInputBindings.TryGetValue(input, out var snapshot))
        {
            if (ModConfig.Instance.logMenu)
            {
                var live = PaneControlField != null ? PaneControlField(input) : InventoryPaneList.PaneTypes.None;
                string restoredPane = snapshot.Pane != null ? snapshot.Pane.name : "<null>";
                LegacyHelper.LogInfo(FormattableString.Invariant(
                    $"RestoreSingleInput {input.gameObject?.name}: paneControl {live} -> {snapshot.PaneControl}, pane -> {restoredPane}, enabled -> {snapshot.Enabled}"));
            }

            PaneField?.SetValue(input, snapshot.Pane);
            PaneListField?.SetValue(input, snapshot.PaneList);
            AllowHorizontalField?.SetValue(input, snapshot.AllowHorizontal);
            AllowVerticalField?.SetValue(input, snapshot.AllowVertical);
            AllowRepeatField?.SetValue(input, snapshot.AllowRepeat);
            AllowRepeatSubmitField?.SetValue(input, snapshot.AllowRepeatSubmit);
            AllowRightStickField?.SetValue(input, snapshot.AllowRightStick);

            if (PaneControlField != null)
            {
                PaneControlField(input) = snapshot.PaneControl;
            }

            input.enabled = snapshot.Enabled;
            OriginalInputBindings.Remove(input);
        }
        else
        {
            PaneField?.SetValue(input, null);
        }

        if (shadePane != null)
        {
            shadePane.UnregisterBoundInput(input);
        }
    }

    internal static void RestoreInputBindings(ShadeInventoryPane shadePane)
    {
        if (shadePane == null)
        {
            return;
        }

        if (CapturedInputs.TryGetValue(shadePane, out var inputs) && inputs.Count > 0)
        {
            foreach (var input in new List<InventoryPaneInput>(inputs))
            {
                RestoreSingleInput(shadePane, input);
            }
        }

        CapturedInputs.Remove(shadePane);
        shadePane.ClearBoundInputs();
        RestoreOrphanedInputs();
    }

    /// <summary>
    /// Puts back any input that still holds a snapshot but is no longer tracked against a live Shade
    /// pane.
    /// <para>
    /// <see cref="CapturedInputs"/> is per-pane bookkeeping that can be dropped without its matching
    /// restore running - a Shade pane destroyed out from under it takes its entry along. An input
    /// orphaned that way keeps the Shade's settings and points its <c>pane</c> at a pane that is no
    /// longer showing. <see cref="OriginalInputBindings"/> still holds the truth for each, so once no
    /// Shade pane holds a capture at all, whatever remains there is by definition a leak and can be
    /// restored unconditionally.
    /// </para>
    /// </summary>
    private static void RestoreOrphanedInputs()
    {
        if (OriginalInputBindings.Count == 0)
        {
            return;
        }

        foreach (var tracked in CapturedInputs.Values)
        {
            if (tracked != null && tracked.Count > 0)
            {
                // Another Shade pane still legitimately owns captures; leave everything alone.
                return;
            }
        }

        var orphaned = new List<InventoryPaneInput>(OriginalInputBindings.Keys);
        if (ModConfig.Instance.logMenu)
        {
            try
            {
                LegacyHelper.LogInfo(FormattableString.Invariant(
                    $"RestoreOrphanedInputs: {orphaned.Count} input(s) still held a snapshot after restore"));
            }
            catch
            {
            }
        }

        foreach (var input in orphaned)
        {
            RestoreSingleInput(null, input);
        }

        OriginalInputBindings.Clear();
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
            // Unwind the previous capture properly rather than just forgetting it. Re-capturing runs
            // routinely - PaneStart's SetActive(true) fires OnEnable, which binds, and then PaneStart
            // itself binds again - and dropping the tracked set without restoring left those inputs
            // holding Shade settings that nothing would ever put back. Restoring first also makes the
            // snapshot taken below a snapshot of the pane's *own* values, not of the Shade's.
            RestoreInputBindings(shadePane);
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
            ApplyShadeInputSettings(input, isLocal);
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

    /// <summary>
    /// Jumps from whichever real pane <paramref name="sourceInput"/> belongs to over to the
    /// appended Shade tab, by doing what <see cref="InventoryPaneInput.Update"/> does for a native
    /// shortcut: write the "Inventory Control" FSM's "Target Pane Index" and send it "MOVE PANE TO".
    /// <para>
    /// Do not call <see cref="InventoryPaneList.SetCurrentPane"/> instead. Its C# body only swaps
    /// pane <i>content</i>; the FSM's tracking variables ("Current Pane", "Current Pane Num", "Prev
    /// Pane") and the visual teardown of the outgoing pane both live in FSM states a direct call
    /// never enters, so the old pane stays superimposed and the next LB/RB press cycles from the
    /// wrong tab.
    /// </para>
    /// <para>
    /// Only works once the inventory is already open: the closed-state
    /// <c>ListenForInventoryShortcut</c> action switches on a named <c>PaneTypes</c> value and
    /// throws for anything it does not recognise, whereas "Target Pane Index" is a plain int that
    /// the Shade's out-of-enum index fits.
    /// </para>
    /// </summary>
    internal static bool TryJumpToShadeTab(InventoryPaneInput sourceInput)
    {
        if (sourceInput == null || PaneListField == null)
        {
            return false;
        }

        // Key 6 on the Shade tab closes the inventory, the same way pressing any real tab's own
        // shortcut a second time does. That symmetry matters more than usual here: the Shade's own
        // keys are otherwise all spoken for - TryHandleShadeTabPaneShortcut turns 1-5 into tab
        // switches - so without this a Shade-on-keyboard player has no key of their own that gets
        // them back out.
        if (ShadeInventoryPane.ActivePane != null)
        {
            return TryCloseInventory(sourceInput);
        }

        InventoryPaneList.PaneTypes paneControl;
        try
        {
            paneControl = PaneControlField(sourceInput);
        }
        catch
        {
            return false;
        }

        // PaneTypes.None marks the Shade's own always-active phantom input (see
        // ApplyShadeInputSettings) as well as any pane-list-level input that never owned a tab.
        // Excluding it means this only fires from the currently-displayed *real* pane's own input.
        if (paneControl == InventoryPaneList.PaneTypes.None)
        {
            return false;
        }

        // Borrowed inputs keep their own paneControl now, so paneControl alone no longer proves this
        // input isn't one the Shade has taken over.
        if (TryGetShadePane(sourceInput) != null)
        {
            return false;
        }

        InventoryPaneList? paneList;
        try
        {
            paneList = PaneListField.GetValue(sourceInput) as InventoryPaneList;
        }
        catch
        {
            paneList = null;
        }

        if (paneList == null)
        {
            return false;
        }

        var panes = PanesField(paneList);
        if (panes == null)
        {
            return false;
        }

        InventoryPane? shadePane = null;
        foreach (var candidate in panes)
        {
            if (candidate != null && candidate.TryGetComponent<ShadeInventoryPane>(out _))
            {
                shadePane = candidate;
                break;
            }
        }

        if (shadePane == null)
        {
            return false;
        }

        // Sanity check only - the FSM path below doesn't need currentPane itself, but bailing out
        // when we're already showing the Shade tab (or can't resolve the current pane at all) avoids
        // sending a no-op "MOVE PANE TO" and its transition sound for nothing.
        InventoryPane? currentPane;
        try
        {
            currentPane = paneList.GetPane(paneControl);
        }
        catch
        {
            currentPane = null;
        }

        if (currentPane == null || ReferenceEquals(currentPane, shadePane))
        {
            return false;
        }

        int shadeIndex = paneList.GetPaneIndex(shadePane);

        if (ModConfig.Instance.logMenu)
        {
            try
            {
                LegacyHelper.LogInfo(FormattableString.Invariant(
                    $"TryJumpToShadeTab: source={sourceInput.gameObject?.name} paneControl={paneControl} currentPane={currentPane.gameObject?.name} shadeIndex={shadeIndex}"));
            }
            catch
            {
            }
        }

        try
        {
            var fsm = PlayMakerFSM.FindFsmOnGameObject(paneList.gameObject, "Inventory Control");
            var targetPaneIndexVar = fsm?.FsmVariables.FindFsmInt("Target Pane Index");
            if (fsm == null || targetPaneIndexVar == null)
            {
                return false;
            }

            targetPaneIndexVar.Value = shadeIndex;
            fsm.SendEvent("MOVE PANE TO");
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Handles the five native inventory shortcuts for an input currently routed to the Shade pane,
    /// returning true when the caller should skip <c>InventoryPaneInput.Update</c> this frame.
    /// <para>
    /// The Shade has no <c>PaneTypes</c> value, so its <c>paneControl</c> stays <c>None</c> - which
    /// <c>InventoryPaneInput.Update</c> reads as "the player pressed this pane's own shortcut" and
    /// answers with <c>PressCancel()</c>, closing the inventory on all of keys 1-5. Switching here
    /// the way the native code does gives the Shade tab the shortcut behaviour every real tab has.
    /// Missing or locked panes fall through to the native path, which closes the inventory, exactly
    /// as the base game does for an unavailable pane.
    /// </para>
    /// </summary>
    internal static bool TryHandleShadeTabPaneShortcut(InventoryPaneInput input)
    {
        if (input == null || PaneListField == null)
        {
            return false;
        }

        try
        {
            if (InventoryPaneInput.IsInputBlocked || CheatManager.IsOpen)
            {
                return false;
            }

            var actions = HornetInput.FindHandler()?.inputActions;
            if (actions == null)
            {
                return false;
            }

            var pressed = InventoryPaneInput.GetInventoryInputPressed(actions);
            if (pressed == InventoryPaneList.PaneTypes.None)
            {
                return false;
            }

            var paneList = PaneListField.GetValue(input) as InventoryPaneList;
            if (paneList == null)
            {
                return false;
            }

            var target = paneList.GetPane(pressed);
            if (target == null || !target.IsAvailable)
            {
                return false;
            }

            var fsm = PlayMakerFSM.FindFsmOnGameObject(paneList.gameObject, "Inventory Control");
            var targetPaneIndexVar = fsm?.FsmVariables.FindFsmInt("Target Pane Index");
            if (fsm == null || targetPaneIndexVar == null)
            {
                return false;
            }

            if (ModConfig.Instance.logMenu)
            {
                try
                {
                    LegacyHelper.LogInfo(FormattableString.Invariant(
                        $"TryHandleShadeTabPaneShortcut: routing {pressed} away from the Shade tab's cancel path"));
                }
                catch
                {
                }
            }

            targetPaneIndexVar.Value = (int)pressed;
            fsm.SendEvent("MOVE PANE TO");
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static int lastInventoryCloseFrame = -1;

    /// <summary>
    /// Closes the inventory through the game's own route: the "Inventory Control" FSM's <c>Opened</c>
    /// state answers its <c>ListenForInventoryShortcut</c> with a <c>CLOSE</c> event, which is exactly
    /// what pressing the current pane's own shortcut produces natively.
    /// <para>
    /// Deliberately not routed through <c>InventoryPaneInput.PressCancel</c>: that sends "UI CANCEL"
    /// to FSMs on its <i>own</i> GameObject, and the Shade pane is a GameObject this mod created, with
    /// no FSM on it to receive anything.
    /// </para>
    /// <para>
    /// Frame-stamped because the Key 6 handler is a postfix on <c>InventoryPaneInput.Update</c> and
    /// more than one input component can run in the same frame; without it a single press would send
    /// <c>CLOSE</c> several times.
    /// </para>
    /// </summary>
    private static bool TryCloseInventory(InventoryPaneInput sourceInput)
    {
        if (sourceInput == null || PaneListField == null)
        {
            return false;
        }

        try
        {
            int frame = Time.frameCount;
            if (frame == lastInventoryCloseFrame)
            {
                return true;
            }

            var paneList = PaneListField.GetValue(sourceInput) as InventoryPaneList;
            if (paneList == null)
            {
                return false;
            }

            var fsm = PlayMakerFSM.FindFsmOnGameObject(paneList.gameObject, "Inventory Control");
            if (fsm == null)
            {
                return false;
            }

            lastInventoryCloseFrame = frame;
            if (ModConfig.Instance.logMenu)
            {
                try
                {
                    LegacyHelper.LogInfo("Key 6 pressed on the Shade tab: closing the inventory");
                }
                catch
                {
                }
            }

            fsm.SendEvent("CLOSE");
            return true;
        }
        catch
        {
            return false;
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


