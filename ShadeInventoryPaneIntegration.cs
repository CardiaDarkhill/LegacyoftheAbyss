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

        // The shade pane MUST go on the end.
        //
        // InventoryPaneList.panes is an ArrayForEnum array: position i in the array *is*
        // InventoryPaneList.PaneTypes value i, and the base game relies on that identity all over
        // the place - GetPane(PaneTypes) indexes the array directly, InventoryPaneInput writes
        // (int)PaneTypes into the "Target Pane Index" FSM variable which SetCurrentInventoryPane
        // feeds straight to SetCurrentPane(index, ...), and ListenForInventoryShortcut compares a
        // PaneTypes value against the FSM's array-space "Current Pane Num".
        //
        // Inserting mid-list (this used to drop the shade pane in just after Tools/Crests) shifts
        // every later pane up one, so each of those numeric lookups silently resolves to the wrong
        // pane: "open journal" landed on Quests, "open map" landed on Journal, and so on. Appending
        // keeps indices 0..4 aligned with the enum and parks the shade at an index the enum never
        // names, so nothing base-game addresses it numerically. It stays reachable by cycling
        // left/right, which is the only way it was ever reachable anyway - no shortcut binds to it.
        newList.Add(shadePane);
        PanesField(paneList) = newList.ToArray();
        RefreshPaneListDisplay(paneList, newList);
        ShadeInventoryPane.LogMenuEvent($"Shade pane appended at index {newList.Count - 1}; total panes={newList.Count}");
        LogPaneLayout(newList);
    }

    private static string? s_loggedPaneLayout;

    /// <summary>
    /// Dumps the final pane order once per distinct layout, at Info level rather than behind the
    /// logMenu flag.
    /// <para>
    /// Position <c>i</c> in <c>InventoryPaneList.panes</c> is <c>PaneTypes</c> value <c>i</c>, and the
    /// base game addresses panes by that number from several places at once - the shortcut FSM's
    /// hardcoded indices (Tools 1, Quests 2, Journal 3, Map 4), <c>GetPane(PaneTypes)</c>, and the
    /// <c>Target Pane Index</c> variable <c>InventoryPaneInput</c> writes. If a shortcut opens the
    /// wrong tab, this line is the fastest way to see whether the array actually lines up with the
    /// enum, without guessing from in-game behaviour.
    /// </para>
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

    /// <summary>
    /// Jumps straight from whichever real pane <paramref name="sourceInput"/> belongs to over to the
    /// appended Shade tab.
    /// <para>
    /// Two earlier versions of this got the mechanism wrong. The first called
    /// <see cref="InventoryPaneList.SetCurrentPane"/> directly - correct for swapping *content*, but
    /// it bypasses the "Inventory Control" PlayMaker FSM's own idea of the current pane entirely.
    /// That FSM tracks three variables ("Current Pane", "Current Pane Num", "Prev Pane"), which
    /// <c>SetNextInventoryPane</c> (driving LB/RB pane cycling) reads directly - a bare
    /// <c>SetCurrentPane</c> call leaves them stale, so the next LB/RB press cycled from wherever the
    /// player was *before* jumping to the Shade tab, not from the Shade tab itself.
    /// </para>
    /// <para>
    /// The second version fixed that by hand-syncing those three variables to match
    /// <c>SetCurrentInventoryPane</c>'s own bookkeeping - which fixed LB/RB, but the pane the player
    /// left never actually disappeared, superimposed under the Shade pane's content on every tab, not
    /// just Map. The reason: <c>SetCurrentPane</c>'s C# body only handles the *content* swap
    /// (<c>PaneEnd()</c> / <c>PaneStart()</c>). The actual show/hide of the outgoing pane's own
    /// GameObject runs through a *separate* FSM sequence ("Fade Panes"), which only runs when the FSM
    /// is actually driven into it - which calling <c>SetCurrentPane</c> straight from C# never does,
    /// no matter how faithfully the tracking variables are kept in sync afterward.
    /// </para>
    /// <para>
    /// This version doesn't touch <c>SetCurrentPane</c> at all. It replicates exactly what
    /// <see cref="InventoryPaneInput.Update"/>'s own open-state shortcut handling does for every real
    /// shortcut (2-5) once the inventory is already open: set the FSM's "Target Pane Index" int
    /// variable and send it the "MOVE PANE TO" event, letting the FSM drive its own full sequence
    /// (content swap, tracking variables, *and* the Fade Panes visual teardown) exactly as a native
    /// shortcut press would. "Target Pane Index" is a plain int - unlike the closed-state
    /// <c>ListenForInventoryShortcut</c> action (which switches on a named <c>PaneTypes</c> enum and
    /// throws for anything it doesn't recognize, which is why this still only works once the
    /// inventory is already open), nothing here needs the Shade to have a named <c>PaneTypes</c>
    /// value of its own.
    /// </para>
    /// </summary>
    internal static bool TryJumpToShadeTab(InventoryPaneInput sourceInput)
    {
        if (sourceInput == null || PaneListField == null)
        {
            return false;
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

        // PaneTypes.None is what every shade-owned InventoryPaneInput is configured with (see
        // ApplyShadeInputSettings) - excluding it means this only fires from the currently-displayed
        // *real* pane's own input component, never from the Shade's own always-active phantom one.
        if (paneControl == InventoryPaneList.PaneTypes.None)
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


