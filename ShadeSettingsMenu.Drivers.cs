#nullable disable
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using BepInEx.Logging;
using GlobalEnums;
using LegacyoftheAbyss.Shade;

public static partial class ShadeSettingsMenu
{
    private enum CancelTarget
    {
        PauseMenu,
        ShadeMain,
        /// <summary>Back out to the Shade AI screen - for anything nested under it.</summary>
        ShadeAi
    }

    private class CancelRouter : MonoBehaviour, ICancelHandler
    {
        public CancelTarget target;

        public void OnCancel(BaseEventData eventData)
        {
            eventData?.Use();
            if (target == CancelTarget.ShadeAi)
            {
                ShowShadeAiMenu();
            }
            else if (target == CancelTarget.ShadeMain)
            {
                ShowMainMenu();
            }
            else
            {
                var ui = builtFor ?? UIManager.instance;
                if (ui != null)
                {
                    bool consumeToggle = activeScreen != null && activeScreen != mainScreen;
                    HideImmediate(ui, consumeToggle);
                }
            }
        }
    }

    internal static bool IsShowing => activeScreen != null && activeScreen.gameObject != null && activeScreen.gameObject.activeSelf;

    private sealed class SliderMenuDriver : MonoBehaviour, IMoveHandler, ISubmitHandler
    {
        public Slider slider;
        public bool wholeNumbers;

        public void Initialize(Slider s, bool whole)
        {
            slider = s;
            wholeNumbers = whole;
        }

        private void Step(float direction)
        {
            if (slider == null)
                return;
            float delta = wholeNumbers ? 1f : FractionalSliderStep;
            float target = slider.value + delta * direction;
            float snapped = SnapSliderValue(target, slider.minValue, slider.maxValue, wholeNumbers);
            if (!Mathf.Approximately(snapped, slider.value))
            {
                slider.value = snapped;
            }
        }

        public void OnMove(AxisEventData eventData)
        {
            if (slider == null || eventData == null)
                return;
            if (eventData.moveDir == MoveDirection.Left)
            {
                Step(-1f);
                eventData.Use();
            }
            else if (eventData.moveDir == MoveDirection.Right)
            {
                Step(1f);
                eventData.Use();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (slider == null)
                return;
            Step(1f);
            eventData?.Use();
        }
    }

    private sealed class CharmButtonDriver : MonoBehaviour, ISelectHandler, ISubmitHandler
    {
        private CharmMenuController controller;
        private ShadeCharmDefinition definition;
        private MenuButton menuButton;
        private Image iconImage;
        private Text nameLabel;
        private Text notchLabel;
        private Text statusLabel;
        private Sprite fallbackIcon;

        public ShadeCharmId? CharmId => definition?.EnumId;

        public void Initialize(CharmMenuController owner, ShadeCharmDefinition def, MenuButton button, Image icon, Text name, Text notch, Text status, Sprite fallback)
        {
            controller = owner;
            definition = def;
            menuButton = button;
            iconImage = icon;
            nameLabel = name;
            notchLabel = notch;
            statusLabel = status;
            fallbackIcon = fallback;
            controller?.RegisterCharmButton(this);
            UpdateStaticContent();
            Refresh();
        }

        private void UpdateStaticContent()
        {
            if (definition == null)
                return;

            if (nameLabel != null)
                nameLabel.text = definition.DisplayName;

            if (notchLabel != null)
            {
                int cost = Mathf.Max(0, definition.NotchCost);
                notchLabel.text = cost == 1 ? "1 Notch" : $"{cost} Notches";
            }

            if (iconImage != null)
            {
                iconImage.sprite = definition.Icon ?? fallbackIcon;
            }
        }

        public void Refresh()
        {
            if (definition == null)
                return;

            var inventory = ShadeRuntime.Charms;
            var enumId = definition.EnumId;
            bool owned = enumId.HasValue && (inventory?.IsOwned(enumId.Value) ?? false);
            bool equipped = enumId.HasValue && (inventory?.IsEquipped(enumId.Value) ?? false);
            bool broken = enumId.HasValue && (inventory?.IsBroken(enumId.Value) ?? false);
            bool isNew = enumId.HasValue && (inventory?.IsNewlyDiscovered(enumId.Value) ?? false);

            if (menuButton != null)
                menuButton.interactable = owned && !broken;

            if (iconImage != null)
            {
                iconImage.sprite = definition.Icon ?? fallbackIcon;
            }

            if (iconImage != null)
            {
                if (!owned)
                {
                    iconImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
                }
                else if (broken)
                {
                    iconImage.color = new Color(0.55f, 0.25f, 0.25f, 1f);
                }
                else
                {
                    iconImage.color = definition.Icon != null ? Color.white : definition.FallbackTint;
                }
            }

            if (statusLabel != null)
            {
                if (broken)
                {
                    statusLabel.text = "Broken";
                    statusLabel.color = new Color(0.83f, 0.35f, 0.35f, 1f);
                }
                else if (equipped)
                {
                    statusLabel.text = "Equipped";
                    statusLabel.color = new Color(0.92f, 0.86f, 0.55f, 1f);
                }
                else if (!owned)
                {
                    statusLabel.text = "Locked";
                    statusLabel.color = new Color(0.7f, 0.32f, 0.32f, 1f);
                }
                else if (isNew)
                {
                    statusLabel.text = "New";
                    statusLabel.color = new Color(0.55f, 0.78f, 0.92f, 1f);
                }
                else
                {
                    statusLabel.text = string.Empty;
                    statusLabel.color = Color.white;
                }
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (definition == null)
                return;
            if (definition.EnumId.HasValue)
            {
                controller?.HandleCharmSelected(definition.EnumId.Value);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (definition == null)
                return;
            if (definition.EnumId.HasValue)
            {
                controller?.HandleCharmSubmit(definition.EnumId.Value);
            }
            eventData?.Use();
        }

        private void OnDestroy()
        {
            controller?.UnregisterCharmButton(this);
        }
    }

    private sealed class CharmMenuController : MonoBehaviour
    {
        private readonly List<CharmButtonDriver> charmButtons = new();
        private Text notchMeter;
        private Text statusText;
        private Text detailTitleText;
        private Text detailDescriptionText;
        private Text navigationHintText;
        private MenuButton equipButton;
        private MenuButton unequipButton;
        private ShadeCharmId? selectedCharm;
        private string pendingStatusMessage = string.Empty;

        public void Initialize(Text notch, Text status, Text title, Text description, Text navigation, MenuButton equip, MenuButton unequip)
        {
            notchMeter = notch;
            statusText = status;
            detailTitleText = title;
            detailDescriptionText = description;
            navigationHintText = navigation;
            equipButton = equip;
            unequipButton = unequip;

            if (navigationHintText != null)
            {
                navigationHintText.text = "Use arrow keys or the left stick to move between charms. Press Enter/A to equip, Backspace/X to unequip, and Esc/B to return to the pause menu. QA: Equip a charm, confirm the notch meter updates, then back out cleanly.";
            }

            if (equipButton != null)
            {
                equipButton.OnSubmitPressed.RemoveAllListeners();
                equipButton.OnSubmitPressed.AddListener(HandleEquipPressed);
            }

            if (unequipButton != null)
            {
                unequipButton.OnSubmitPressed.RemoveAllListeners();
                unequipButton.OnSubmitPressed.AddListener(HandleUnequipPressed);
            }

            RefreshAll();
        }

        public void RegisterCharmButton(CharmButtonDriver driver)
        {
            if (driver != null && !charmButtons.Contains(driver))
                charmButtons.Add(driver);
        }

        public void UnregisterCharmButton(CharmButtonDriver driver)
        {
            if (driver == null)
                return;
            charmButtons.Remove(driver);
        }

        public void HandleScreenShown()
        {
            RefreshAll();
        }

        public void RefreshAll()
        {
            for (int i = charmButtons.Count - 1; i >= 0; i--)
            {
                if (charmButtons[i] == null)
                {
                    charmButtons.RemoveAt(i);
                    continue;
                }
                charmButtons[i].Refresh();
            }

            UpdateNotchMeter();
            UpdateActionState();
            UpdateDetailPanel();
        }

        public void HandleCharmSelected(ShadeCharmId id)
        {
            selectedCharm = id;
            var inventory = ShadeRuntime.Charms;
            if (inventory != null && inventory.MarkCharmSeen(id))
            {
                foreach (var driver in charmButtons)
                {
                    driver?.Refresh();
                }
            }
            UpdateActionState();
            UpdateDetailPanel();
        }

        public void HandleCharmSubmit(ShadeCharmId id)
        {
            var inventory = ShadeRuntime.Charms;
            if (inventory == null)
            {
                pendingStatusMessage = "Charm inventory not ready.";
                UpdateStatusText(pendingStatusMessage);
                return;
            }

            if (!ShadeRuntime.IsHornetRestingAtBench())
            {
                pendingStatusMessage = ShadeRuntime.BenchLockedMessage;
                RefreshAll();
                return;
            }

            if (inventory.TryToggle(id, out var message))
            {
                pendingStatusMessage = message;
                selectedCharm = id;
                LegacyHelper.RequestShadeLoadoutRecompute();
            }
            else
            {
                pendingStatusMessage = string.IsNullOrEmpty(message) ? "Unable to change charm." : message;
                RefreshAll();
            }
        }

        public void HandleEquipPressed()
        {
            var inventory = ShadeRuntime.Charms;
            if (inventory == null)
            {
                pendingStatusMessage = "Charm inventory not ready.";
                RefreshAll();
                return;
            }

            if (!ShadeRuntime.IsHornetRestingAtBench())
            {
                pendingStatusMessage = ShadeRuntime.BenchLockedMessage;
                RefreshAll();
                return;
            }

            if (!selectedCharm.HasValue)
            {
                pendingStatusMessage = "Select a charm to equip.";
                RefreshAll();
                return;
            }

            if (inventory.TryEquip(selectedCharm.Value, out var message))
            {
                pendingStatusMessage = message;
                LegacyHelper.RequestShadeLoadoutRecompute();
            }
            else
            {
                pendingStatusMessage = string.IsNullOrEmpty(message) ? "Unable to equip charm." : message;
                RefreshAll();
            }
        }

        public void HandleUnequipPressed()
        {
            var inventory = ShadeRuntime.Charms;
            if (inventory == null)
            {
                pendingStatusMessage = "Charm inventory not ready.";
                RefreshAll();
                return;
            }

            if (!ShadeRuntime.IsHornetRestingAtBench())
            {
                pendingStatusMessage = ShadeRuntime.BenchLockedMessage;
                RefreshAll();
                return;
            }

            if (!selectedCharm.HasValue)
            {
                pendingStatusMessage = "Select a charm to unequip.";
                RefreshAll();
                return;
            }

            if (inventory.TryUnequip(selectedCharm.Value, out var message))
            {
                pendingStatusMessage = message;
                LegacyHelper.RequestShadeLoadoutRecompute();
            }
            else
            {
                pendingStatusMessage = string.IsNullOrEmpty(message) ? "Unable to unequip charm." : message;
                RefreshAll();
            }
        }

        private void UpdateNotchMeter()
        {
            if (notchMeter == null)
                return;
            var inventory = ShadeRuntime.Charms;
            if (inventory == null)
            {
                notchMeter.text = "Charm inventory unavailable.";
                return;
            }

            string status = $"Notches Used: {inventory.UsedNotches}/{inventory.NotchCapacity}";
            if (inventory.IsOvercharmed)
            {
                status = $"Overcharmed! {status}";
            }

            notchMeter.text = status;
        }

        private void UpdateDetailPanel()
        {
            var inventory = ShadeRuntime.Charms;
            if (inventory == null)
            {
                SetDetailTexts("Charms", "Charm inventory data not yet ready.");
                UpdateStatusText("Charm data unavailable.");
                return;
            }

            if (!selectedCharm.HasValue)
            {
                foreach (var driver in charmButtons)
                {
                    if (driver != null)
                    {
                        selectedCharm = driver.CharmId;
                        break;
                    }
                }
            }

            string fallbackStatus = "Select a charm to view details.";

            if (selectedCharm.HasValue)
            {
                var def = inventory.GetDefinition(selectedCharm.Value);
                SetDetailTexts(def.DisplayName, def.Description);
                if (!inventory.IsOwned(selectedCharm.Value))
                    fallbackStatus = "This charm has not been unlocked yet.";
                else if (inventory.IsBroken(selectedCharm.Value))
                    fallbackStatus = "This charm is broken. Rest at a bench to repair it before equipping.";
                else if (inventory.IsEquipped(selectedCharm.Value))
                    fallbackStatus = "Charm equipped. Unequip to free notches for other charms.";
                else if (def.NotchCost > 0 && inventory.UsedNotches + def.NotchCost > inventory.NotchCapacity)
                {
                    fallbackStatus = inventory.IsOvercharmed
                        ? "Shade is overcharmed. Unequip a charm first."
                        : "Equip to add this charm to your shade's loadout.";
                }
                else
                    fallbackStatus = "Equip to add this charm to your shade's loadout.";
            }
            else
            {
                SetDetailTexts("Charms", "Select a charm to view its description and equip requirements.");
            }

            if (inventory.IsOvercharmed && (fallbackStatus == null || fallbackStatus.IndexOf("overcharm", StringComparison.OrdinalIgnoreCase) < 0))
            {
                fallbackStatus = "Shade is overcharmed. " + fallbackStatus;
            }

            if (!ShadeRuntime.IsHornetRestingAtBench())
            {
                fallbackStatus = ShadeRuntime.BenchLockedMessage;
            }

            UpdateStatusText(fallbackStatus);
        }

        private void SetDetailTexts(string title, string description)
        {
            if (detailTitleText != null)
                detailTitleText.text = title;
            if (detailDescriptionText != null)
                detailDescriptionText.text = description;
        }

        private void UpdateActionState()
        {
            var inventory = ShadeRuntime.Charms;
            bool canEquip = false;
            bool canUnequip = false;
            bool atBench = ShadeRuntime.IsHornetRestingAtBench();

            if (inventory != null && selectedCharm.HasValue)
            {
                var def = inventory.GetDefinition(selectedCharm.Value);
                bool owned = inventory.IsOwned(selectedCharm.Value);
                bool equipped = inventory.IsEquipped(selectedCharm.Value);
                bool broken = inventory.IsBroken(selectedCharm.Value);
                if (atBench)
                {
                    canUnequip = equipped;
                    canEquip = owned && !equipped && !broken;
                }
            }

            if (equipButton != null)
                equipButton.interactable = canEquip && atBench;
            if (unequipButton != null)
                unequipButton.interactable = canUnequip && atBench;
        }

        private void UpdateStatusText(string fallback)
        {
            if (statusText == null)
                return;

            if (!string.IsNullOrEmpty(pendingStatusMessage))
            {
                statusText.text = pendingStatusMessage;
                pendingStatusMessage = string.Empty;
            }
            else
            {
                statusText.text = fallback;
            }
        }
    }

    private sealed class BindingMenuDriver : MonoBehaviour
    {
        private MenuButton button;
        private ShadeAction action;
        private bool secondary;
        private string labelPrefix;
        private Text uiText;
        private Component tmpTextComponent;
        private PropertyInfo tmpTextProperty;
        private bool capturing;

        public void Initialize(MenuButton menuButton, ShadeAction targetAction, bool isSecondary, string label)
        {
            button = menuButton;
            action = targetAction;
            secondary = isSecondary;
            labelPrefix = label;
            uiText = button.GetComponentInChildren<Text>(true);
            var tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
            if (tmpType != null)
            {
                tmpTextComponent = button.GetComponentInChildren(tmpType, true);
                tmpTextProperty = tmpType.GetProperty("text");
            }

            button.OnSubmitPressed.RemoveAllListeners();
            button.OnSubmitPressed.AddListener(BeginCapture);
            RegisterBindingDriver(this);
            UpdateLabel();
        }

        public void UpdateLabel()
        {
            string bindingText = ShadeInput.DescribeBindingOption(ShadeInput.GetBindingOption(action, secondary));
            SetButtonText($"{labelPrefix}: {bindingText}");
        }

        private void SetButtonText(string value)
        {
            if (uiText != null)
            {
                uiText.text = value;
                return;
            }
            if (tmpTextComponent != null && tmpTextProperty != null)
            {
                tmpTextProperty.SetValue(tmpTextComponent, value);
            }
        }

        private void BeginCapture()
        {
            if (!capturing)
                StartCoroutine(CaptureRoutine());
        }

        private System.Collections.IEnumerator CaptureRoutine()
        {
            capturing = true;
            SetButtonText($"{labelPrefix}: Press a binding... (Esc cancels, Backspace clears)");
            while (true)
            {
                yield return null;
                if (Input.GetKeyDown(KeyCode.Escape))
                    break;
                if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
                {
                    ShadeInput.SetBindingOption(action, secondary, ShadeBindingOption.None());
                    ModConfig.Save();
                    NotifyBindingChanged();
                    break;
                }
                if (ShadeInput.TryCaptureKey(out var key))
                {
                    ShadeInput.SetBindingOption(action, secondary, ShadeBindingOption.FromKey(key));
                    ModConfig.Save();
                    NotifyBindingChanged();
                    break;
                }
                if (ShadeInput.TryCaptureControl(out var control, out int deviceIndex))
                {
                    ShadeInput.SetBindingOption(action, secondary, ShadeBindingOption.FromControl(control, deviceIndex));
                    ShadeInput.EnsureControllerIndex(deviceIndex);
                    ModConfig.Save();
                    NotifyBindingChanged();
                    break;
                }
            }
            capturing = false;
            UpdateLabel();
        }

        private void OnDestroy()
        {
            UnregisterBindingDriver(this);
        }
    }

    /// <summary>
    /// Writes a label onto a cloned menu row, whichever text component the game's prefab happens to
    /// use. Shared by every row that renders its own state into its label.
    /// </summary>
    private static void SetSelectableLabelText(GameObject root, string value)
    {
        if (root == null)
        {
            return;
        }

        var uiText = root.GetComponentInChildren<Text>(true);
        if (uiText != null)
        {
            uiText.text = value;
            return;
        }

        var tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
        if (tmpType == null)
        {
            return;
        }

        var tmp = root.GetComponentInChildren(tmpType, true);
        if (tmp != null)
        {
            tmpType.GetProperty("text")?.SetValue(tmp, value);
        }
    }

    /// <summary>
    /// A yes/no option rendered the way the Shade Enabled row always was: the label carries the
    /// state after a colon, and submitting flips it.
    /// <para>
    /// This is what every toggle in these menus is now. The checkbox squares it replaced were a
    /// second visual language for the same idea, and being cloned from a Toggle prefab rather than a
    /// MenuButton they also had no selection fleurs of their own.
    /// </para>
    /// </summary>
    private sealed class LabeledToggleDriver : MonoBehaviour
    {
        private MenuButton button;
        private string label;
        private bool value;
        private System.Action<bool> onChange;

        public void Initialize(MenuButton menuButton, string labelText, bool initial, System.Action<bool> changed)
        {
            button = menuButton;
            label = labelText;
            value = initial;
            onChange = changed;
            button.OnSubmitPressed.RemoveAllListeners();
            button.OnSubmitPressed.AddListener(Toggle);
            UpdateLabel();
        }

        private void OnEnable()
        {
            UpdateLabel();
        }

        private void Toggle()
        {
            value = !value;
            try
            {
                onChange?.Invoke(value);
            }
            catch (Exception e)
            {
                LogMenuWarning($"Toggle '{label}' threw: {e}");
            }

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (button == null || label == null)
            {
                return;
            }

            SetSelectableLabelText(button.gameObject, label + ": " + (value ? "On" : "Off"));
        }
    }

    /// <summary>
    /// Shows a line of explanation for whichever row is highlighted.
    /// <para>
    /// Polls the EventSystem for the same reason <see cref="ScrollIntoViewDriver"/> does: rows are
    /// cloned game prefabs and there is no selection event to subscribe to that covers all of them.
    /// </para>
    /// </summary>
    private sealed class MenuDescriptionDriver : MonoBehaviour
    {
        public Text target;

        private readonly Dictionary<GameObject, string> descriptions = new Dictionary<GameObject, string>();
        private GameObject lastSelected;

        public void Register(MenuSelectable selectable, string description)
        {
            if (selectable == null || string.IsNullOrEmpty(description))
            {
                return;
            }

            descriptions[selectable.gameObject] = description;
        }

        private void OnEnable()
        {
            // Force a refresh: the screen may be reopened with the same row highlighted.
            lastSelected = null;
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }

            var eventSystem = EventSystem.current;
            var selected = eventSystem != null ? eventSystem.currentSelectedGameObject : null;
            if (selected == lastSelected)
            {
                return;
            }

            lastSelected = selected;
            target.text = selected != null && descriptions.TryGetValue(selected, out var description)
                ? description
                : string.Empty;
        }
    }

    private sealed class ShadeToggleDriver : MonoBehaviour
    {
        private MenuButton button;

        public void Initialize(MenuButton menuButton)
        {
            button = menuButton;
            button.OnSubmitPressed.RemoveAllListeners();
            button.OnSubmitPressed.AddListener(ToggleShade);
            shadeToggleDriver = this;
            UpdateLabel();
        }

        private void OnEnable()
        {
            if (shadeToggleDriver == null)
                shadeToggleDriver = this;
            UpdateLabel();
        }

        public void UpdateLabel()
        {
            SetButtonText(GetShadeToggleLabel());
        }

        private void ToggleShade()
        {
            LegacyHelper.SetShadeEnabled(!ModConfig.Instance.shadeEnabled);
        }

        private void SetButtonText(string value)
        {
            SetSelectableLabelText(button != null ? button.gameObject : null, value);
        }

        private void OnDestroy()
        {
            if (shadeToggleDriver == this)
                shadeToggleDriver = null;
        }
    }

    private static void RegisterBindingDriver(BindingMenuDriver driver)
    {
        if (driver != null && !bindingDrivers.Contains(driver))
            bindingDrivers.Add(driver);
    }

    private static void UnregisterBindingDriver(BindingMenuDriver driver)
    {
        if (driver == null)
            return;
        bindingDrivers.Remove(driver);
    }

    internal static void NotifyBindingChanged()
    {
        for (int i = bindingDrivers.Count - 1; i >= 0; i--)
        {
            var driver = bindingDrivers[i];
            if (driver == null)
            {
                bindingDrivers.RemoveAt(i);
                continue;
            }
            driver.UpdateLabel();
        }
    }

    internal static void NotifyShadeToggleChanged()
    {
        shadeToggleDriver?.UpdateLabel();
    }

    internal static void NotifyCharmLoadoutChanged()
    {
        charmsController?.RefreshAll();
    }

    private static void ApplyDefaultPreset()
    {
        ShadeInput.Config.ResetToDefaults();
        HornetInput.ApplyControllerDefaults();
        ModConfig.Save();
        NotifyBindingChanged();
    }

    private static void ApplyDualControllerPresetOption()
    {
        ShadeInput.Config.ApplyDualControllerPreset();
        HornetInput.ApplyControllerDefaults();
        ModConfig.Save();
        NotifyBindingChanged();
    }

    private static void ApplyKeyboardOnlyPresetOption()
    {
        ShadeInput.Config.ApplyKeyboardOnlyPreset();
        HornetInput.ApplyKeyboardDefaults(true);
        ModConfig.Save();
        NotifyBindingChanged();
    }

    private static void ApplyShadeControllerPresetOption()
    {
        ShadeInput.Config.ApplyShadeControllerPreset();
        HornetInput.ApplyKeyboardDefaults(true);
        ModConfig.Save();
        NotifyBindingChanged();
    }

    private sealed class RowHighlightDriver : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject highlight;
        private readonly List<Animator> animators = new();
        private static readonly int ShowTrigger = Animator.StringToHash("show");
        private static readonly int HideTrigger = Animator.StringToHash("hide");

        public void Initialize(GameObject highlightGo, IEnumerable<Animator> highlightAnimators)
        {
            highlight = highlightGo;
            animators.Clear();
            if (highlightAnimators != null)
            {
                foreach (var animator in highlightAnimators)
                {
                    if (animator != null)
                        animators.Add(animator);
                }
            }
            SetActive(false, true);
        }

        private void SetActive(bool active, bool instant = false)
        {
            bool useFallback = highlight != null && animators.Count == 0;
            if (useFallback && highlight.activeSelf != active)
                highlight.SetActive(active);

            if (animators.Count == 0)
                return;

            foreach (var animator in animators)
            {
                if (animator == null)
                    continue;
                try
                {
                    if (active)
                    {
                        animator.ResetTrigger(HideTrigger);
                        animator.SetTrigger(ShowTrigger);
                    }
                    else
                    {
                        animator.ResetTrigger(ShowTrigger);
                        animator.SetTrigger(HideTrigger);
                    }
                    if (instant)
                        animator.Update(0f);
                }
                catch
                {
                }
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != gameObject)
                EventSystem.current.SetSelectedGameObject(gameObject);
            SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                return;
            SetActive(false);
        }

        private void OnDisable()
        {
            SetActive(false, true);
        }
    }

    private sealed class MenuFocusDriver : MonoBehaviour
    {
        public MenuScreen screen;

        private void Update()
        {
            if (screen == null || !screen.gameObject.activeInHierarchy)
                return;

            var eventSystem = EventSystem.current;
            if (eventSystem == null)
                return;

            var current = eventSystem.currentSelectedGameObject;
            if (current != null && current.transform.IsChildOf(screen.transform))
                return;

            var highlight = GetPreferredHighlight(screen);
            if (highlight == null)
                return;

            var selectable = highlight.GetFirstInteractable();
            if (selectable == null)
                return;

            eventSystem.SetSelectedGameObject(selectable.gameObject);
            UIManager.HighlightSelectableNoSound(selectable);
        }
    }

    /// <summary>
    /// Keeps whatever is currently keyboard/controller-focused inside <see cref="content"/>
    /// scrolled into view within <see cref="viewport"/>. Needed because the Controls screen's
    /// binding list can be taller than the screen (see BuildControlsMenu) -- without this,
    /// navigating to a row below the fold would move focus there with no visual feedback.
    /// Scrolls through the ScrollRect's own API (not by poking content.anchoredPosition
    /// directly) so the visible scrollbar handle stays in sync with keyboard/controller nav,
    /// not just mouse dragging.
    /// </summary>
    /// <summary>
    /// Re-runs a navigation-wiring action once, after Unity's own deferred Start() call on
    /// this GameObject has had a chance to run.
    ///
    /// MenuButtonList.Start() unconditionally calls its own SetupActive(), which -- for any
    /// selectable left in Navigation.Mode.Explicit -- overwrites selectOnUp/selectOnDown
    /// with "the previous/next entry in its flat entries array", regardless of what those
    /// were set to beforehand. It never touches selectOnLeft/selectOnRight. Since
    /// BuildControlsMenu's own SetupButtonList call to SetupActive() runs while every
    /// selectable is still in Automatic mode (so that first call is a no-op for Up/Down),
    /// the *next* call -- Unity's own deferred Start(), which fires later, after this
    /// GameObject/screen has actually gone active -- is the one that clobbers whatever
    /// explicit 2D grid was configured. LateUpdate is guaranteed by Unity to run after every
    /// Start()/Update() that fired this frame, so reapplying there reliably wins.
    /// </summary>
    private sealed class DeferredNavigationReapplyDriver : MonoBehaviour
    {
        public System.Action Reapply;
        private bool applied;

        private void LateUpdate()
        {
            if (applied)
                return;
            applied = true;
            try
            {
                Reapply?.Invoke();
            }
            finally
            {
                Destroy(this);
            }
        }
    }

    private sealed class ScrollIntoViewDriver : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public RectTransform viewport;
        public RectTransform content;

        private void Update()
        {
            if (scrollRect == null || viewport == null || content == null)
                return;

            var eventSystem = EventSystem.current;
            var selected = eventSystem != null ? eventSystem.currentSelectedGameObject : null;
            if (selected == null)
                return;

            var selectedRect = selected.transform as RectTransform;
            if (selectedRect == null || !selectedRect.IsChildOf(content))
                return;

            float maxScroll = Mathf.Max(0f, content.rect.height - viewport.rect.height);
            if (maxScroll <= 0f)
                return;

            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, selectedRect);
            var viewportRect = viewport.rect;

            // Positive when the focused row currently sticks out past that edge of the
            // viewport -- i.e. how far we'd need to shift content to just clear it.
            float topOverflow = bounds.max.y - viewportRect.yMax;
            float bottomOverflow = viewportRect.yMin - bounds.min.y;
            if (topOverflow <= 0f && bottomOverflow <= 0f)
                return;

            float targetY = content.anchoredPosition.y;
            if (topOverflow > 0f)
                targetY -= topOverflow;
            else
                targetY += bottomOverflow;

            targetY = Mathf.Clamp(targetY, 0f, maxScroll);
            // verticalNormalizedPosition: 1 = scrolled to top (anchoredPosition.y == 0),
            // 0 = scrolled to bottom (anchoredPosition.y == maxScroll).
            scrollRect.verticalNormalizedPosition = 1f - (targetY / maxScroll);
        }
    }

}
#nullable restore
