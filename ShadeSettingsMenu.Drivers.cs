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
        ShadeAi,
        /// <summary>Back out of the new-game questions to the save slots, starting nothing.</summary>
        ShadeNewGame
    }

    private class CancelRouter : MonoBehaviour, ICancelHandler
    {
        public CancelTarget target;

        public void OnCancel(BaseEventData eventData)
        {
            eventData?.Use();
            if (target == CancelTarget.ShadeNewGame)
            {
                var manager = newGameBuiltFor ?? UIManager.instance;
                if (manager != null)
                {
                    manager.StartCoroutine(CancelNewGame(manager));
                }
            }
            else if (target == CancelTarget.ShadeAi)
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

        public void HandleEquipPressed() => HandleEquipChange(equip: true);

        public void HandleUnequipPressed() => HandleEquipChange(equip: false);

        /// <summary>
        /// The three things that stop a charm change, and then the change. Equipping and unequipping
        /// differed only in which inventory call was made and one word of the message, and were two
        /// copies of the same four guards.
        /// </summary>
        private void HandleEquipChange(bool equip)
        {
            string verb = equip ? "equip" : "unequip";

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
                pendingStatusMessage = $"Select a charm to {verb}.";
                RefreshAll();
                return;
            }

            string message;
            bool changed = equip
                ? inventory.TryEquip(selectedCharm.Value, out message)
                : inventory.TryUnequip(selectedCharm.Value, out message);

            if (changed)
            {
                pendingStatusMessage = message;
                LegacyHelper.RequestShadeLoadoutRecompute();
                return;
            }

            pendingStatusMessage = string.IsNullOrEmpty(message) ? $"Unable to {verb} charm." : message;
            RefreshAll();
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
        private bool capturing;

        public void Initialize(MenuButton menuButton, ShadeAction targetAction, bool isSecondary, string label)
        {
            button = menuButton;
            action = targetAction;
            secondary = isSecondary;
            labelPrefix = label;

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

        private void SetButtonText(string value) => SetSelectableLabelText(button.gameObject, value);

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
    /// Hands the two pads out by asking each player to press a button on theirs.
    /// <para>
    /// The mod stores which controller belongs to the companion as an index into
    /// <c>InputManager.Devices</c>, and that index is not something a player can see, work out or
    /// reliably guess - two identical pads are indistinguishable in any list, and the order they
    /// appear in is the order the machine happened to enumerate them. So neither player is asked
    /// which number they are holding. They are asked to press a button, and the device the press
    /// arrives on is the answer.
    /// </para>
    /// <para>
    /// Both are asked rather than only the companion, even though only the companion's index is
    /// stored. Hornet's press is what proves the two players are on different pads: pressing the
    /// same one twice is the mistake this is meant to catch, and catching it costs one extra step.
    /// </para>
    /// </summary>
    internal class ControllerAssignmentDriver : MonoBehaviour
    {
        private MenuButton button;
        private bool capturing;

        public void Initialize(MenuButton menuButton)
        {
            button = menuButton;
            button.OnSubmitPressed.RemoveAllListeners();
            button.OnSubmitPressed.AddListener(BeginCapture);
            UpdateLabel();
        }

        private void SetButtonText(string value) => SetSelectableLabelText(button.gameObject, value);

        public void UpdateLabel()
        {
            SetButtonText("Assign Controllers: " + Describe());
        }

        /// <summary>The split as it stands, named by device rather than by index.</summary>
        private static string Describe()
        {
            try
            {
                var devices = InControl.InputManager.Devices;
                int count = devices?.Count ?? 0;
                if (count == 0)
                {
                    return "no controllers attached";
                }

                int companion = ModConfig.Instance.shadeInput != null
                    ? ModConfig.Instance.shadeInput.controllerDeviceIndex
                    : -1;

                if (companion < 0 || companion >= count)
                {
                    return "companion is not on a controller";
                }

                string companionName = DeviceName(devices[companion], companion);

                // Hornet's is whichever else is attached. Named rather than numbered for the same
                // reason the companion's is, and left vague when there is more than one candidate -
                // the mod reserves one pad and leaves her the rest, so there may be no single answer.
                string hornetName = "the keyboard";
                int others = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i == companion)
                    {
                        continue;
                    }

                    others++;
                    if (others == 1)
                    {
                        hornetName = DeviceName(devices[i], i);
                    }
                }

                if (others > 1)
                {
                    hornetName = "any other controller";
                }

                return $"Hornet {hornetName}, companion {companionName}";
            }
            catch
            {
                return "unavailable";
            }
        }

        private static string DeviceName(InControl.InputDevice device, int index)
        {
            string name = device != null ? device.Name : null;
            if (string.IsNullOrEmpty(name))
            {
                return "controller " + (index + 1).ToString(CultureInfo.InvariantCulture);
            }

            return name;
        }

        private void BeginCapture()
        {
            if (!capturing)
            {
                StartCoroutine(CaptureRoutine());
            }
        }

        private IEnumerator CaptureRoutine()
        {
            capturing = true;

            int hornetIndex = -1;
            int companionIndex = -1;

            while (true)
            {
                SetButtonText("Press a button on HORNET's controller... (Esc cancels)");
                yield return WaitForPad(result => hornetIndex = result);
                if (hornetIndex < 0)
                {
                    break;
                }

                SetButtonText("Now press a button on the COMPANION's controller... (Esc cancels)");
                yield return WaitForPad(result => companionIndex = result);
                if (companionIndex < 0)
                {
                    break;
                }

                if (companionIndex == hornetIndex)
                {
                    // The whole point of asking twice. Say so and start again rather than storing a
                    // split that would leave one pad driving both characters - which is the state
                    // this screen exists to get out of.
                    SetButtonText("That is the same controller - try again");
                    yield return new WaitForSecondsRealtime(1.5f);
                    hornetIndex = -1;
                    companionIndex = -1;
                    continue;
                }

                // The whole assignment, not just the config index: a control rebound on a pad
                // remembers that pad itself, and those remembered devices outrank the index.
                var shadeConfig = ModConfig.Instance.shadeInput;
                int moved = shadeConfig != null
                    ? shadeConfig.ApplyControllerAssignment(hornetIndex, companionIndex)
                    : 0;

                // Hornet needs her controller switched back on, or the pad she just pressed is
                // reserved from her by a setting she did not touch.
                ModConfig.Instance.hornetControllerEnabled = true;
                ModConfig.Save();

                if (moved > 0 && ModConfig.Instance.logMenu)
                {
                    try
                    {
                        LegacyHelper.LogInfo(FormattableString.Invariant(
                            $"Controller assignment moved {moved} binding(s) onto the newly named devices."));
                    }
                    catch
                    {
                    }
                }

                try { HornetInput.RefreshHornetDeviceBindings(); }
                catch { }

                NotifyBindingChanged();
                break;
            }

            capturing = false;
            UpdateLabel();
        }

        /// <summary>
        /// Waits for a button on any pad and reports which device it came from, or -1 if the player
        /// backed out. Uses the same capture the rebinding rows use, so a press is attributed to the
        /// device it was made on rather than to whichever pad was last active.
        /// </summary>
        private static IEnumerator WaitForPad(Action<int> report)
        {
            while (true)
            {
                yield return null;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    report(-1);
                    yield break;
                }

                // WasPressed rather than held, so one press cannot answer for both players: the
                // edge is gone by the frame the next prompt starts listening.
                if (ShadeInput.TryCaptureControl(out _, out int deviceIndex) && deviceIndex >= 0)
                {
                    report(deviceIndex);
                    yield break;
                }
            }
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

        /// <summary>
        /// Re-asked every time the row is shown, so a setting that another screen can make
        /// meaningless reads as unavailable rather than as a stale On/Off. Null means always
        /// available.
        /// </summary>
        private System.Func<bool> isUnavailable;

        public void Initialize(MenuButton menuButton, string labelText, bool initial, System.Action<bool> changed, System.Func<bool> unavailable = null)
        {
            button = menuButton;
            label = labelText;
            value = initial;
            onChange = changed;
            isUnavailable = unavailable;
            button.OnSubmitPressed.RemoveAllListeners();
            button.OnSubmitPressed.AddListener(Toggle);
            UpdateLabel();
        }

        private bool Unavailable => isUnavailable != null && isUnavailable();

        private void OnEnable()
        {
            UpdateLabel();
        }

        private void Toggle()
        {
            if (Unavailable)
            {
                UpdateLabel();
                return;
            }

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

            SetSelectableLabelText(button.gameObject, label + ": " + (Unavailable ? "Unavailable" : (value ? "On" : "Off")));
        }
    }

    /// <summary>
    /// A row whose value is chosen from a short list rather than dragged on a slider: the difficulty
    /// preset, assist mode, the Shade's mask share. Submit advances by one, wrapping.
    /// <para>
    /// Rendered as a menu button reading <c>"Label: Value"</c>, the same shape
    /// <see cref="LabeledToggleDriver"/> uses, because these three sit in one row across the top of
    /// the Difficulty screen where a slider would not fit and would read as three different kinds of
    /// control for three settings of the same weight.
    /// </para>
    /// <para>
    /// Deliberately not an <c>IMoveHandler</c>, unlike <see cref="SliderMenuDriver"/>. These three
    /// sit side by side, so Left/Right has to move between them - and Unity delivers a move event to
    /// every handler on the object, <c>Selectable.OnMove</c> included, with no way for one to
    /// suppress the other. A stepper that also read Left/Right would change its value and jump to
    /// the next cell on the same press.
    /// </para>
    /// </summary>
    private sealed class LabeledStepperDriver : MonoBehaviour
    {
        private MenuButton button;
        private string label;
        private Func<string> describeValue;
        private Action<int> step;

        /// <summary>
        /// The screen this row belongs to, set when it is registered. Every stepper used to refresh
        /// the Difficulty screen by name, which is only the right screen when the row is on it.
        /// </summary>
        public DifficultyMenuController Owner { get; set; }

        public void Initialize(MenuButton menuButton, string labelText, Func<string> valueText, Action<int> stepBy)
        {
            button = menuButton;
            label = labelText;
            describeValue = valueText;
            step = stepBy;
            button.OnSubmitPressed.RemoveAllListeners();
            button.OnSubmitPressed.AddListener(() => Step(1));
            UpdateLabel();
        }

        private void OnEnable()
        {
            UpdateLabel();
        }

        private void Step(int direction)
        {
            try
            {
                step?.Invoke(direction);
            }
            catch (Exception e)
            {
                LogMenuWarning($"Stepper '{label}' threw: {e}");
            }

            // Every stepper on the Difficulty screen can change what another one reads - applying a
            // preset rewrites the sliders, and editing a slider turns the preset into Custom - so
            // the whole screen refreshes rather than just this row.
            (Owner ?? difficultyController)?.RefreshAll();
        }

        public void UpdateLabel()
        {
            if (button == null || label == null)
                return;

            string value;
            try
            {
                value = describeValue?.Invoke() ?? string.Empty;
            }
            catch (Exception e)
            {
                LogMenuWarning($"Stepper '{label}' could not describe its value: {e}");
                return;
            }

            SetSelectableLabelText(button.gameObject, label + ": " + value);
        }
    }

    /// <summary>
    /// Keeps the Difficulty screen agreeing with itself.
    /// <para>
    /// Its rows are not independent: choosing a preset rewrites every slider on the screen, and
    /// nudging any one of those sliders turns the preset row into "Custom". Nothing here owns state -
    /// every row reads <see cref="ModConfig"/> when asked to refresh - so a refresh is always
    /// correct regardless of what changed it, including a change made from outside this screen.
    /// </para>
    /// </summary>
    private sealed class DifficultyMenuController : MonoBehaviour
    {
        private readonly List<LabeledStepperDriver> steppers = new();
        private readonly List<Action> sliderRefreshers = new();

        /// <summary>
        /// Guards the loop a refresh would otherwise re-enter: pushing a preset's value into a slider
        /// fires its onValueChanged, whose handler asks for a refresh so the preset row can go to
        /// Custom if the value was edited by hand. Without this the outer pass would restart once per
        /// remaining stale row. It terminates either way - each pass settles at least one row - but
        /// there is no reason to do the work.
        /// </summary>
        private bool refreshing;

        public void RegisterStepper(LabeledStepperDriver stepper)
        {
            if (stepper != null && !steppers.Contains(stepper))
            {
                steppers.Add(stepper);
                stepper.Owner = this;
            }
        }

        public void RegisterSliderRefresh(Action refresh)
        {
            if (refresh != null)
                sliderRefreshers.Add(refresh);
        }

        public void HandleScreenShown() => RefreshAll();

        public void RefreshAll()
        {
            if (refreshing)
            {
                return;
            }

            refreshing = true;
            try
            {
                for (int i = sliderRefreshers.Count - 1; i >= 0; i--)
                {
                    try { sliderRefreshers[i]?.Invoke(); }
                    catch (Exception e) { LogMenuWarning($"Difficulty slider refresh threw: {e}"); }
                }

                for (int i = steppers.Count - 1; i >= 0; i--)
                {
                    var stepper = steppers[i];
                    if (stepper == null)
                    {
                        steppers.RemoveAt(i);
                        continue;
                    }

                    stepper.UpdateLabel();
                }
            }
            finally
            {
                refreshing = false;
            }
        }
    }

    /// <summary>
    /// Makes hovering a row select it.
    /// <para>
    /// Unity selects on click, not on hover: <c>Selectable.OnPointerEnter</c> only sets the
    /// highlighted *tint*, leaving <c>currentSelectedGameObject</c> where it was. Everything this
    /// menu shows about the current row hangs off the selection - the fleurs either side of it, the
    /// explanation at the bottom of the screen, which column the shoulder prompts belong to - so
    /// with the mouse all of it stayed pinned to whichever row the screen opened on while the cursor
    /// moved over everything else.
    /// </para>
    /// <para>
    /// Selecting on hover is also how the game's own menus behave, so this is the behaviour a player
    /// arrives expecting.
    /// </para>
    /// </summary>
    private sealed class PointerSelectDriver : MonoBehaviour, IPointerEnterHandler
    {
        public Selectable target;

        public void OnPointerEnter(PointerEventData eventData)
        {
            var selectable = target != null ? target : GetComponent<Selectable>();
            if (selectable == null || !selectable.IsInteractable())
            {
                return;
            }

            var eventSystem = EventSystem.current;
            if (eventSystem == null || eventSystem.currentSelectedGameObject == selectable.gameObject)
            {
                return;
            }

            eventSystem.SetSelectedGameObject(selectable.gameObject);
            UIManager.HighlightSelectableNoSound(selectable);
        }
    }

    /// <summary>
    /// Keeps a shoulder-button prompt showing the device the player is actually on.
    /// <para>
    /// Resolving this once at build time is the trap: the menu is built seconds after launch, before
    /// a pad has necessarily been seen, and whatever it picks is then kept for the session - showing
    /// a controller player keyboard key caps that nothing will change. The device to ask about was
    /// never the problem; asking only once was.
    /// </para>
    /// <para>
    /// The game raises <c>InputHandler.RefreshActiveControllerEvent</c> whenever the active device
    /// changes and redraws its own prompts on it. This does the same, and redraws on becoming
    /// visible as well, for the changes that happened while the screen was closed.
    /// </para>
    /// </summary>
    private sealed class PanePromptGlyphDriver : MonoBehaviour
    {
        private HeroActionButton action;
        private Image glyph;
        private Text symbol;
        private LayoutElement slot;
        private float size;
        private InputHandler subscribed;

        public void Initialize(HeroActionButton button, Image image, Text symbolText, LayoutElement layout, float glyphSize)
        {
            action = button;
            glyph = image;
            symbol = symbolText;
            slot = layout;
            size = glyphSize;
            Refresh();
        }

        private void OnEnable()
        {
            Subscribe();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            try
            {
                var handler = HornetInput.FindHandler();
                if (handler == null || handler == subscribed)
                {
                    return;
                }

                Unsubscribe();
                handler.RefreshActiveControllerEvent += Refresh;
                subscribed = handler;
            }
            catch (Exception e)
            {
                LogMenuWarning($"Could not follow the active controller for the {action} prompt: {e}");
            }
        }

        private void Unsubscribe()
        {
            if (subscribed == null)
            {
                return;
            }

            try
            {
                subscribed.RefreshActiveControllerEvent -= Refresh;
            }
            catch (Exception e)
            {
                LogMenuWarning($"Could not stop following the active controller for the {action} prompt: {e}");
            }

            subscribed = null;
        }

        private void Refresh()
        {
            var skin = ResolvePaneButtonSkin(action);

            // A ButtonSkin is two halves and both are drawn: on a keyboard the sprite is a blank key
            // cap with the letter in symbol, to go on top of it; on a pad the sprite is the whole
            // glyph and there is no symbol. Drawing only the first is what produced empty boxes.
            bool hasSprite = skin != null && skin.sprite != null;
            bool hasSymbol = skin != null && !string.IsNullOrWhiteSpace(skin.symbol);

            if (glyph != null)
            {
                glyph.sprite = hasSprite ? skin.sprite : null;
                glyph.enabled = hasSprite;
            }

            if (symbol != null)
            {
                symbol.text = hasSymbol ? skin.symbol : string.Empty;
                symbol.enabled = hasSymbol;
            }

            if (slot != null)
            {
                // A wide key cap is drawn wide; a pad glyph and a bare letter are square. The same
                // three cases the game sizes its own prompt containers by.
                float width = skin != null && skin.skinType == ButtonSkinType.WIDE ? size * 1.9f : size;
                slot.preferredWidth = width;
                slot.minWidth = width;
            }
        }
    }

    /// <summary>
    /// Moves the highlight between two columns of rows on a shoulder-button press.
    /// <para>
    /// The Difficulty screen's rows are almost all sliders, and a slider row eats Left and Right to
    /// step its own value - so there was no way to reach the Healing column from the Damage one at
    /// all. The inventory already solves this exact problem with its pane buttons, so this borrows
    /// them: the same two actions, and the same glyphs drawn beside each column heading.
    /// </para>
    /// <para>
    /// Polls <c>PaneLeft</c>/<c>PaneRight</c> off the game's own <c>InputHandler</c> rather than
    /// listening for a UI event, because these arrive as PlayerActions and nothing routes them into
    /// a MenuScreen. Held state is tracked here so one press moves one column.
    /// </para>
    /// </summary>
    private sealed class PaneSwitchDriver : MonoBehaviour
    {
        public List<MenuSelectable> leftColumn;
        public List<MenuSelectable> rightColumn;

        /// <summary>
        /// Shown only while the highlight is in the column it would move you out of - a prompt for a
        /// button that would do nothing is worse than no prompt.
        /// </summary>
        public CanvasGroup leftColumnPrompt;
        public CanvasGroup rightColumnPrompt;

        private bool leftWasPressed;
        private bool rightWasPressed;

        private void OnDisable()
        {
            leftWasPressed = false;
            rightWasPressed = false;
        }

        private void OnEnable()
        {
            UpdatePromptVisibility();
        }

        private void UpdatePromptVisibility()
        {
            var eventSystem = EventSystem.current;
            var current = eventSystem != null ? eventSystem.currentSelectedGameObject : null;

            SetVisible(leftColumnPrompt, current != null && rightColumn != null && rightColumn.Count > 0 && Contains(leftColumn, current));
            SetVisible(rightColumnPrompt, current != null && leftColumn != null && leftColumn.Count > 0 && Contains(rightColumn, current));
        }

        private static void SetVisible(CanvasGroup group, bool visible)
        {
            if (group == null)
            {
                return;
            }

            float target = visible ? 1f : 0f;
            if (!Mathf.Approximately(group.alpha, target))
            {
                group.alpha = target;
            }
        }

        private static bool Contains(List<MenuSelectable> column, GameObject candidate)
        {
            if (column == null)
            {
                return false;
            }

            for (int i = 0; i < column.Count; i++)
            {
                if (column[i] != null && column[i].gameObject == candidate)
                {
                    return true;
                }
            }

            return false;
        }

        private void Update()
        {
            HeroActions actions;
            try
            {
                var handler = HornetInput.FindHandler();
                actions = handler != null ? handler.inputActions : null;
            }
            catch
            {
                return;
            }

            if (actions == null)
            {
                return;
            }

            bool leftHeld = actions.PaneLeft.IsPressed;
            bool rightHeld = actions.PaneRight.IsPressed;

            if (rightHeld && !rightWasPressed)
            {
                MoveTo(rightColumn, leftColumn);
            }
            else if (leftHeld && !leftWasPressed)
            {
                MoveTo(leftColumn, rightColumn);
            }

            leftWasPressed = leftHeld;
            rightWasPressed = rightHeld;

            UpdatePromptVisibility();
        }

        /// <summary>
        /// Highlights <paramref name="target"/>'s row at the same index as the highlighted row in
        /// <paramref name="source"/>, clamped when the columns are different lengths. Does nothing
        /// when the highlight is not in the source column, so pressing the button that points at the
        /// column you are already in is a no-op rather than a jump to its first row.
        /// </summary>
        private void MoveTo(List<MenuSelectable> target, List<MenuSelectable> source)
        {
            if (target == null || target.Count == 0 || source == null)
            {
                return;
            }

            var eventSystem = EventSystem.current;
            var current = eventSystem != null ? eventSystem.currentSelectedGameObject : null;
            if (current == null)
            {
                return;
            }

            int index = -1;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] != null && source[i].gameObject == current)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
            {
                return;
            }

            var destination = target[Mathf.Min(index, target.Count - 1)];
            if (destination == null)
            {
                return;
            }

            var interactable = destination.GetFirstInteractable();
            if (interactable == null)
            {
                return;
            }

            eventSystem.SetSelectedGameObject(interactable.gameObject);
            UIManager.HighlightSelectableNoSound(interactable);
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

        /// <summary>
        /// Rows whose explanation depends on their own current value - the Difficulty screen's
        /// preset row, which describes whichever preset is selected. Looked up every frame the row
        /// is highlighted rather than cached, because stepping the preset does not change the
        /// selection and so would not otherwise re-run the lookup.
        /// </summary>
        private readonly Dictionary<GameObject, Func<string>> liveDescriptions = new Dictionary<GameObject, Func<string>>();

        private GameObject lastSelected;
        private string lastText;

        public void Register(MenuSelectable selectable, string description)
        {
            if (selectable == null || string.IsNullOrEmpty(description))
            {
                return;
            }

            descriptions[selectable.gameObject] = description;
        }

        public void RegisterLive(MenuSelectable selectable, Func<string> description)
        {
            if (selectable == null || description == null)
            {
                return;
            }

            liveDescriptions[selectable.gameObject] = description;
        }

        private void OnEnable()
        {
            // Force a refresh: the screen may be reopened with the same row highlighted.
            lastSelected = null;
            lastText = null;
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }

            var eventSystem = EventSystem.current;
            var selected = eventSystem != null ? eventSystem.currentSelectedGameObject : null;
            bool isLive = selected != null && liveDescriptions.ContainsKey(selected);
            if (selected == lastSelected && !isLive)
            {
                return;
            }

            lastSelected = selected;

            string text = string.Empty;
            if (selected != null)
            {
                if (liveDescriptions.TryGetValue(selected, out var live))
                {
                    try { text = live() ?? string.Empty; }
                    catch (Exception e) { LogMenuWarning($"Live description threw: {e}"); }
                }
                else if (descriptions.TryGetValue(selected, out var description))
                {
                    text = description;
                }
            }

            if (!string.Equals(text, lastText, StringComparison.Ordinal))
            {
                lastText = text;
                target.text = text;
            }
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
    /// Re-runs a navigation-wiring action once, after Unity's own deferred <c>Start()</c> on this
    /// GameObject has had its chance.
    /// <para>
    /// <c>MenuButtonList.Start()</c> calls <c>SetupActive()</c>, which overwrites
    /// <c>selectOnUp</c>/<c>selectOnDown</c> on any selectable left in <c>Navigation.Mode.Explicit</c>
    /// with the previous/next entry of its flat array. <c>SetupButtonList</c> calls
    /// <c>SetupActive()</c> while every selectable is still Automatic, so that first call is a no-op
    /// for Up/Down and Unity's later <c>Start()</c> is the one that clobbers an explicit 2D grid.
    /// <c>LateUpdate</c> is guaranteed to run after every <c>Start()</c> that fired this frame, so
    /// reapplying there wins.
    /// </para>
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

    /// <summary>
    /// Keeps the keyboard/controller-focused element inside <see cref="content"/> scrolled into view
    /// within <see cref="viewport"/> - the Controls screen's binding list can be taller than the
    /// screen, and navigating below the fold otherwise moves focus with no visual feedback. Scrolls
    /// through the <c>ScrollRect</c>'s own API rather than poking <c>content.anchoredPosition</c>, so
    /// the visible scrollbar handle stays in sync with pad navigation and not just mouse dragging.
    /// </summary>
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
