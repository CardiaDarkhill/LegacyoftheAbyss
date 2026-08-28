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

        rect = template.GetComponent<RectTransform>();
        if (rect != null)
        {
            return rect;
        }

        RectTransform? matchByName = null;
        RectTransform? matchDirectChild = null;
        RectTransform? firstCandidate = null;
        RectTransform? scoredCandidate = null;
        int scoredCandidateValue = int.MinValue;

        var rects = template.GetComponentsInChildren<RectTransform>(true);
        if (rects.Length > 0)
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

                if (candidate != null && candidate.GetComponent<InventoryPane>() == template)
                {
                    rect = candidate;
                    break;
                }


                string name = candidate != null ? candidate.gameObject.name : string.Empty;
                if (!string.IsNullOrEmpty(name))
                {
                    string lower = name.ToLowerInvariant();

                    if (candidate != null)
                    {
                        int candidateScore = ScoreTemplateRootCandidate(template, candidate, lower);
                        if (candidateScore > scoredCandidateValue)
                        {
                            scoredCandidate = candidate;
                            scoredCandidateValue = candidateScore;
                        }
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
                        string existingName = matchByName.gameObject.name;
                        if (!string.IsNullOrEmpty(existingName))
                        {
                            existingIsDescription = existingName.Contains("description", StringComparison.OrdinalIgnoreCase);
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
            string rectName = rect.gameObject.name;
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

        Debug.Log("[ShadeInventory] " + message);
    }

    public override void Awake()
    {
        base.Awake();
        RegisterInputHandlers();
    }

    private void OnEnable()
    {
        RegisterInputHandlers();
        EnsureBuilt();
        labelPulseTimer = 0f;
        loggedInactiveHierarchyProcessing = false;

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

        if (attachedPaneList != null)
        {
            ShadeInventoryPaneIntegration.BindInput(this, attachedPaneList, captureFocus: IsPaneActive);
        }

        LogMenuEvent(FormattableString.Invariant(
            $"OnEnable: active={isActive} entries={entries.Count} inventoryNull={inventory == null}"));
    }

    private void OnDisable()
    {
        // The FSM only ever deactivates the pane you just left, and PaneEnd runs first - so this
        // firing while still the current pane means an ancestor went inactive and took the pane
        // with it. Name the ancestor rather than only recording that it happened.
        if (IsPaneActive)
        {
            LogUnexpectedDeactivation();
        }

        ShadeInventoryPaneIntegration.RestoreInputBindings(this);
        UnregisterInputHandlers();
        UpdateInventoryBinding(false);
        isActive = false;
        labelPulseTimer = 0f;
        ResetShadeInputState("OnDisable");
        StopAllCoroutines();
        ClearActiveCharmFlights();

        // Not while a slide-out is running: the FSM deactivates the pane you just left partway
        // through its own tween, so cutting the overlay here makes the Charms tab vanish instead of
        // sliding away.
        if (overlaySlide == null || !overlaySlide.IsTransitioning)
        {
            ApplyOverlayVisibility(false);
        }
        if (ReferenceEquals(activePane, this))
        {
            activePane = null;
        }
        loggedInactiveHierarchyProcessing = false;
        LogMenuEvent("OnDisable");
    }

    /// <summary>
    /// Dumps this pane's ancestor chain with each link's own <c>activeSelf</c>. The first ancestor
    /// reading <c>False</c> is what actually deactivated this object; if every ancestor reads
    /// <c>True</c>, something called <c>SetActive(false)</c> on the pane itself instead.
    /// </summary>
    private void LogUnexpectedDeactivation()
    {
        var builder = new System.Text.StringBuilder("Shade pane deactivated while still the current pane. Ancestors:");
        Transform node = transform;
        while (node != null)
        {
            builder.Append(FormattableString.Invariant($" {node.name}(activeSelf={node.gameObject.activeSelf})"));
            node = node.parent;
            if (node != null)
            {
                builder.Append(" <-");
            }
        }

        LegacyHelper.LogInfo(builder.ToString());
    }

    private void OnDestroy()
    {
        ShadeInventoryPaneIntegration.RestoreInputBindings(this);
        UnregisterInputHandlers();
        UpdateInventoryBinding(false);
        DetachPaneList();
        if (ReferenceEquals(activePane, this))
        {
            activePane = null;
        }

        StopAllCoroutines();
        ClearActiveCharmFlights();

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

        if (generatedHighlightSprite != null)
        {
            Destroy(generatedHighlightSprite);
            generatedHighlightSprite = null;
        }

        if (generatedHighlightTexture != null)
        {
            Destroy(generatedHighlightTexture);
            generatedHighlightTexture = null;
        }
    }

    private void RegisterInputHandlers()
    {
        if (inputHandlersRegistered)
        {
            return;
        }

        OnInputLeft += HandleInputLeft;
        OnInputRight += HandleInputRight;
        OnInputUp += HandleInputUp;
        OnInputDown += HandleInputDown;
        inputHandlersRegistered = true;
        LogMenuEvent("Registered directional input handlers");
    }

    private void UnregisterInputHandlers()
    {
        if (!inputHandlersRegistered)
        {
            return;
        }

        OnInputLeft -= HandleInputLeft;
        OnInputRight -= HandleInputRight;
        OnInputUp -= HandleInputUp;
        OnInputDown -= HandleInputDown;
        inputHandlersRegistered = false;
        LogMenuEvent("Unregistered directional input handlers");
    }

    private void UpdateCapturedInputFocus()
    {
        boundInputs.RemoveWhere(input => input == null);
        bool focused = boundInputs.Count > 0;
        if (focused != hasCapturedInputFocus)
        {
            hasCapturedInputFocus = focused;
            LogMenuEvent(FormattableString.Invariant(
                $"Shade input focus {(focused ? "acquired" : "lost")} -> count={boundInputs.Count}"));
        }
    }

    private bool HasBoundInputs
    {
        get
        {
            UpdateCapturedInputFocus();
            return hasCapturedInputFocus;
        }
    }

    internal void RegisterBoundInput(InventoryPaneInput input)
    {
        if (input == null)
        {
            return;
        }

        boundInputs.RemoveWhere(candidate => candidate == null);
        bool added = boundInputs.Add(input);
        UpdateCapturedInputFocus();
        if (added)
        {
            LogMenuEvent(FormattableString.Invariant(
                $"RegisterBoundInput -> count={boundInputs.Count}"));
        }
    }

    internal void UnregisterBoundInput(InventoryPaneInput input)
    {
        if (input == null)
        {
            return;
        }

        boundInputs.RemoveWhere(candidate => candidate == null);
        bool removed = boundInputs.Remove(input);
        UpdateCapturedInputFocus();
        if (removed)
        {
            LogMenuEvent(FormattableString.Invariant(
                $"UnregisterBoundInput -> count={boundInputs.Count}"));
        }
    }

    internal void ClearBoundInputs()
    {
        bool hadInputs = boundInputs.Count > 0;
        boundInputs.Clear();
        if (hadInputs)
        {
            LogMenuEvent("ClearBoundInputs");
        }
        UpdateCapturedInputFocus();
    }

    private void HandleInputLeft()
    {
        lastPaneInputFrame = Time.frameCount;
        lastPaneInputDirection = InventoryPaneBase.InputEventType.Left;
        lastPaneInputCameFromEvent = true;
        HandleDirectionalInput(InventoryPaneBase.InputEventType.Left, fromInputComponent: false);
    }

    private void HandleInputRight()
    {
        lastPaneInputFrame = Time.frameCount;
        lastPaneInputDirection = InventoryPaneBase.InputEventType.Right;
        lastPaneInputCameFromEvent = true;
        HandleDirectionalInput(InventoryPaneBase.InputEventType.Right, fromInputComponent: false);
    }

    private void HandleInputUp()
    {
        lastPaneInputFrame = Time.frameCount;
        lastPaneInputDirection = InventoryPaneBase.InputEventType.Up;
        lastPaneInputCameFromEvent = true;
        HandleDirectionalInput(InventoryPaneBase.InputEventType.Up, fromInputComponent: false);
    }

    private void HandleInputDown()
    {
        lastPaneInputFrame = Time.frameCount;
        lastPaneInputDirection = InventoryPaneBase.InputEventType.Down;
        lastPaneInputCameFromEvent = true;
        HandleDirectionalInput(InventoryPaneBase.InputEventType.Down, fromInputComponent: false);
    }

    public override void PaneStart()
    {
        base.PaneStart();
        EnsureBuilt();
        activePane = this;
        labelPulseTimer = 0f;
        isActive = true;
        ResetShadeInputState("PaneStart");
        UpdateInventoryBinding(true);
        if (attachedPaneList != null)
        {
            ShadeInventoryPaneIntegration.BindInput(this, attachedPaneList, captureFocus: true);
        }
        SetOverlayVisibility(true, animate: true);
        RefreshAll();
        UpdateParentListLabel();
        ForceLayoutRebuild();
        LogMenuEvent($"PaneStart: entries={entries.Count}, inventoryNull={inventory == null}");
    }

    public override void PaneEnd()
    {
        ShadeInventoryPaneIntegration.RestoreInputBindings(this);
        UpdateInventoryBinding(false);
        isActive = false;
        labelPulseTimer = 0f;
        ResetShadeInputState("PaneEnd");

        // The FSM calls PaneEnd both for a pane swap and on the way out of the inventory, and only
        // the swap is animated. PlayerData.isInventoryOpen is cleared by SetIsInventoryOpen(false)
        // before the close path reaches PaneEnd, which tells the two apart.
        var playerData = PlayerData.instance;
        SetOverlayVisibility(false, animate: playerData != null && playerData.isInventoryOpen);
        if (ReferenceEquals(activePane, this))
        {
            activePane = null;
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

    internal static ShadeInventoryPane? ActivePane => activePane;

    internal void HandleSubmit()
    {
        EnsureBuilt();
        LogMenuEvent(FormattableString.Invariant(
            $"HandleSubmit invoked: active={isActive} inventoryNull={inventory == null} entryCount={entries.Count} selectedIndex={selectedIndex}"));
        if (!isActive || inventory == null || entries.Count == 0)
        {
            LogMenuEvent("HandleSubmit aborted: pane inactive or missing data");
            return;
        }

        var entry = entries[Mathf.Clamp(selectedIndex, 0, entries.Count - 1)];
        var id = entry.Id;
        if (!ShadeRuntime.IsHornetRestingAtBench())
        {
            LogMenuEvent("HandleSubmit blocked: hornet not at bench");
            SetTextValue(statusText, statusTextTMP, ShadeRuntime.BenchLockedMessage);
            return;
        }

        if (!inventory.IsOwned(id))
        {
            LogMenuEvent(FormattableString.Invariant(
                $"HandleSubmit blocked: charm '{id}' not owned"));
            SetTextValue(statusText, statusTextTMP, "This charm has not been unlocked yet.");
            return;
        }

        string message;
        bool currentlyEquipped = inventory.IsEquipped(id);
        bool success = currentlyEquipped
            ? inventory.TryUnequip(id, out message)
            : inventory.TryEquip(id, out message);
        LogMenuEvent(FormattableString.Invariant(
            $"HandleSubmit {(currentlyEquipped ? "unequip" : "equip")} -> success={success} message='{message}'"));

        bool triggeredOvercharmAttempt = false;
        int attemptIndex = 0;
        int attemptThreshold = inventory.OvercharmAttemptThreshold;
        var definition = entry.Definition;
        if (!success && !currentlyEquipped && definition != null)
        {
            int notchCost = definition.NotchCost;
            if (notchCost > 0 && inventory.UsedNotches + notchCost > inventory.NotchCapacity && !inventory.IsOvercharmed)
            {
                attemptIndex = Mathf.Clamp(attemptThreshold - inventory.RemainingOvercharmAttempts, 0, attemptThreshold);
                triggeredOvercharmAttempt = attemptIndex > 0;
            }
        }

        SetTextValue(statusText, statusTextTMP, message);
        if (triggeredOvercharmAttempt && definition != null)
        {
            StartOvercharmAttemptAnimation(entry, definition, attemptIndex, attemptThreshold);
        }

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
        RefreshEntryStates();
        UpdateNotchMeter();
        UpdateDetailPanel();
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

}
