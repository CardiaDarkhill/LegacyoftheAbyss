using System;
using System.Globalization;
using System.Text;
using InControl;
using LegacyoftheAbyss.Shade.Ai;
using UnityEngine;

public enum ShadeAction
{
    MoveLeft,
    MoveRight,
    MoveUp,
    MoveDown,
    Fire,
    Nail,
    NailUp,
    NailDown,
    Teleport,
    Focus,
    Sprint,
    /// <summary>
    /// Opens the targeting reticle that tells an AI-driven Shade where to stand. Bound on Hornet's
    /// side of the controls rather than the Shade's, because in AI mode nobody is holding the
    /// Shade's own inputs - see LegacyHelper.ShadeController.AiCommand.cs.
    /// </summary>
    CommandShade,
    // Developer-only utility actions. Only ever surfaced in the Controls menu when
    // ModConfig.Instance.debugKeysEnabled is on (see BuildControlsMenu), and only ever
    // read when the same flag is on (see SimpleHUD.HandleDebugKeys), so an ordinary
    // player never sees or triggers these regardless of what they're bound to.
    DebugDamageShade,
    DebugHealShade,
    DebugSoulIncrease,
    DebugSoulDecrease,
    DebugSoulReset
}

public enum ShadeBindingOptionType
{
    None,
    Key,
    Controller
}

[Serializable]
public struct ShadeBindingOption
{
    public ShadeBindingOptionType type;
    public KeyCode key;
    public InputControlType control;
    public int controllerDevice;

    public static ShadeBindingOption None() => new()
    {
        type = ShadeBindingOptionType.None,
        key = KeyCode.None,
        control = InputControlType.None,
        controllerDevice = -1
    };

    public static ShadeBindingOption FromKey(KeyCode keyCode) => new()
    {
        type = ShadeBindingOptionType.Key,
        key = keyCode,
        control = InputControlType.None,
        controllerDevice = -1
    };

    public static ShadeBindingOption FromControl(InputControlType controlType, int controllerIndex = -1) => new()
    {
        type = ShadeBindingOptionType.Controller,
        key = KeyCode.None,
        control = controlType,
        controllerDevice = controllerIndex
    };

    public ShadeBindingOption WithControllerIndex(int index)
    {
        controllerDevice = index;
        return this;
    }
}

[Serializable]
public class ShadeBinding
{
    public ShadeBindingOption primary = ShadeBindingOption.None();
    public ShadeBindingOption secondary = ShadeBindingOption.None();

    public ShadeBinding()
    {
    }

    public ShadeBinding(ShadeBindingOption first, ShadeBindingOption second)
    {
        primary = first;
        secondary = second;
    }

    public ShadeBinding Clone() => new ShadeBinding(primary, secondary);
}

[Serializable]
public class ShadeInputConfig
{
    public int controllerDeviceIndex = 1;
    public float controllerDeadzone = 0.25f;

    public ShadeBinding moveLeft = new();
    public ShadeBinding moveRight = new();
    public ShadeBinding moveUp = new();
    public ShadeBinding moveDown = new();
    public ShadeBinding fire = new();
    public ShadeBinding nail = new();
    public ShadeBinding nailUp = new();
    public ShadeBinding nailDown = new();
    public ShadeBinding teleport = new();
    public ShadeBinding focus = new();
    public ShadeBinding sprint = new();
    public ShadeBinding commandShade = new();
    public ShadeBinding debugDamageShade = new();
    public ShadeBinding debugHealShade = new();
    public ShadeBinding debugSoulIncrease = new();
    public ShadeBinding debugSoulDecrease = new();
    public ShadeBinding debugSoulReset = new();

    public ShadeInputConfig()
    {
        ResetToDefaults();
    }

    public static ShadeInputConfig CreateDefault() => new ShadeInputConfig();

    public void ResetToDefaults()
    {
        controllerDeviceIndex = 1;
        controllerDeadzone = Mathf.Clamp(controllerDeadzone <= 0f ? 0.25f : controllerDeadzone, 0.01f, 1f);

        moveLeft = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.A), ShadeBindingOption.None());
        moveRight = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.D), ShadeBindingOption.None());
        moveUp = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.W), ShadeBindingOption.None());
        moveDown = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.S), ShadeBindingOption.None());
        fire = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Space), ShadeBindingOption.None());
        nail = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.J), ShadeBindingOption.FromKey(KeyCode.Mouse0));
        nailUp = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.E), ShadeBindingOption.None());
        nailDown = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Q), ShadeBindingOption.None());
        teleport = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.K), ShadeBindingOption.None());
        focus = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.H), ShadeBindingOption.None());
        sprint = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.LeftShift), ShadeBindingOption.None());
        // Middle mouse and the left stick of the *first* pad: this is Hornet's control, not the
        // Shade player's, so it is pinned to device 0 rather than following controllerDeviceIndex.
        commandShade = new ShadeBinding(
            ShadeBindingOption.FromKey(KeyCode.Mouse2),
            ShadeBindingOption.FromControl(InputControlType.LeftStickButton, 0));

        // Matches the defaults these carried as hardcoded, unrebindable KeyCode constants
        // in SimpleHUD before they moved into the normal binding system -- unbound except
        // for soul reset, so existing behaviour doesn't change until someone rebinds them.
        debugDamageShade = new ShadeBinding(ShadeBindingOption.None(), ShadeBindingOption.None());
        debugHealShade = new ShadeBinding(ShadeBindingOption.None(), ShadeBindingOption.None());
        debugSoulIncrease = new ShadeBinding(ShadeBindingOption.None(), ShadeBindingOption.None());
        debugSoulDecrease = new ShadeBinding(ShadeBindingOption.None(), ShadeBindingOption.None());
        debugSoulReset = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Backslash), ShadeBindingOption.None());
    }

    public void ApplyDualControllerPreset()
    {
        controllerDeviceIndex = 1;
        controllerDeadzone = Mathf.Clamp(controllerDeadzone <= 0f ? 0.25f : controllerDeadzone, 0.01f, 1f);

        moveLeft = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickLeft), ShadeBindingOption.None());
        moveRight = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickRight), ShadeBindingOption.None());
        moveUp = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickUp), ShadeBindingOption.None());
        moveDown = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickDown), ShadeBindingOption.None());
        fire = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.RightBumper), ShadeBindingOption.None());
        nail = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action3), ShadeBindingOption.None());
        nailUp = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action4), ShadeBindingOption.None());
        nailDown = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action1), ShadeBindingOption.None());
        teleport = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickButton), ShadeBindingOption.None());
        focus = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action2), ShadeBindingOption.None());
        sprint = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.RightTrigger), ShadeBindingOption.None());
        // Every pad button worth having is spoken for, so the AI toggle keeps its keyboard binding
        // rather than displacing one. A preset that left this holding the previous preset's value
        // would be worse than leaving it plainly on the keyboard.
        commandShade = new ShadeBinding(
            ShadeBindingOption.FromKey(KeyCode.Mouse2),
            ShadeBindingOption.FromControl(InputControlType.LeftStickButton, 0));
    }

    public void ApplyKeyboardOnlyPreset()
    {
        controllerDeviceIndex = -1;
        controllerDeadzone = Mathf.Clamp(controllerDeadzone <= 0f ? 0.25f : controllerDeadzone, 0.01f, 1f);

        moveLeft = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.LeftArrow), ShadeBindingOption.None());
        moveRight = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.RightArrow), ShadeBindingOption.None());
        moveUp = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.UpArrow), ShadeBindingOption.None());
        moveDown = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.DownArrow), ShadeBindingOption.None());
        fire = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Keypad1), ShadeBindingOption.None());
        nail = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Keypad2), ShadeBindingOption.FromKey(KeyCode.RightShift));
        nailUp = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Keypad8), ShadeBindingOption.None());
        nailDown = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Keypad5), ShadeBindingOption.None());
        teleport = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Keypad3), ShadeBindingOption.None());
        focus = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.KeypadEnter), ShadeBindingOption.None());
        sprint = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Keypad0), ShadeBindingOption.None());
        commandShade = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Mouse2), ShadeBindingOption.None());
    }

    public void ApplySharedKeyboardPreset()
    {
        ApplyKeyboardOnlyPreset();
    }

    public void ApplyShadeControllerPreset()
    {
        controllerDeviceIndex = 0;
        controllerDeadzone = Mathf.Clamp(controllerDeadzone <= 0f ? 0.25f : controllerDeadzone, 0.01f, 1f);

        moveLeft = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickLeft), ShadeBindingOption.None());
        moveRight = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickRight), ShadeBindingOption.None());
        moveUp = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickUp), ShadeBindingOption.None());
        moveDown = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickDown), ShadeBindingOption.None());
        fire = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.RightBumper), ShadeBindingOption.None());
        nail = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action3), ShadeBindingOption.None());
        nailUp = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action4), ShadeBindingOption.None());
        nailDown = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action1), ShadeBindingOption.None());
        teleport = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickButton), ShadeBindingOption.None());
        focus = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action2), ShadeBindingOption.None());
        sprint = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.RightTrigger), ShadeBindingOption.None());
        // The Shade owns pad 0 under this preset, so its left stick click is already Teleport.
        // Hornet is on the keyboard here, which leaves middle mouse as the whole binding.
        commandShade = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Mouse2), ShadeBindingOption.None());
    }

    private static bool BindingUsesController(ShadeBinding binding)
    {
        if (binding == null)
            return false;
        return binding.primary.type == ShadeBindingOptionType.Controller || binding.secondary.type == ShadeBindingOptionType.Controller;
    }

    private static bool OptionUsesControllerIndex(ShadeBindingOption option, int fallbackIndex, int targetIndex)
    {
        if (option.type != ShadeBindingOptionType.Controller)
            return false;
        int actualIndex = option.controllerDevice >= 0 ? option.controllerDevice : fallbackIndex;
        return actualIndex == targetIndex;
    }

    private static bool BindingUsesControllerIndex(ShadeBinding binding, int fallbackIndex, int targetIndex)
    {
        if (binding == null)
            return false;
        return OptionUsesControllerIndex(binding.primary, fallbackIndex, targetIndex) || OptionUsesControllerIndex(binding.secondary, fallbackIndex, targetIndex);
    }

    public bool UsesControllerBindings()
    {
        return BindingUsesController(moveLeft) || BindingUsesController(moveRight) || BindingUsesController(moveUp) ||
               BindingUsesController(moveDown) || BindingUsesController(fire) || BindingUsesController(nail) ||
               BindingUsesController(nailUp) || BindingUsesController(nailDown) || BindingUsesController(teleport) ||
               BindingUsesController(focus) || BindingUsesController(sprint) || BindingUsesController(commandShade);
    }

    /// <summary>
    /// Whether a controller is <em>the Shade player's</em>, and so has to be kept away from Hornet.
    /// <para>
    /// Not the same question as <see cref="IsControllerIndexInUse"/>, and the difference is the
    /// whole point of this method. <see cref="commandShade"/> is bound to the first pad on purpose -
    /// ordering the Shade about is Hornet's player's control, not the Shade player's - so a literal
    /// "is any shade binding on this pad" reads Hornet's own controller as the Shade's. With exactly
    /// two pads that made the Shade look like it had claimed both, which tripped the guard that
    /// stops Hornet being left with no controller at all, and neither pad ended up reserved: Hornet
    /// answered to both, and the Shade player's stick drove both characters at once.
    /// </para>
    /// </summary>
    public bool ReservesControllerIndex(int index)
    {
        if (index < 0)
            return false;
        int fallbackIndex = Mathf.Max(-1, controllerDeviceIndex);
        return BindingUsesControllerIndex(moveLeft, fallbackIndex, index) ||
               BindingUsesControllerIndex(moveRight, fallbackIndex, index) ||
               BindingUsesControllerIndex(moveUp, fallbackIndex, index) ||
               BindingUsesControllerIndex(moveDown, fallbackIndex, index) ||
               BindingUsesControllerIndex(fire, fallbackIndex, index) ||
               BindingUsesControllerIndex(nail, fallbackIndex, index) ||
               BindingUsesControllerIndex(nailUp, fallbackIndex, index) ||
               BindingUsesControllerIndex(nailDown, fallbackIndex, index) ||
               BindingUsesControllerIndex(teleport, fallbackIndex, index) ||
               BindingUsesControllerIndex(focus, fallbackIndex, index) ||
               BindingUsesControllerIndex(sprint, fallbackIndex, index);
    }

    /// <summary>Whether any pad at all is the Shade player's. See <see cref="ReservesControllerIndex"/>.</summary>
    public bool ReservesAnyController()
    {
        return BindingUsesController(moveLeft) || BindingUsesController(moveRight) || BindingUsesController(moveUp) ||
               BindingUsesController(moveDown) || BindingUsesController(fire) || BindingUsesController(nail) ||
               BindingUsesController(nailUp) || BindingUsesController(nailDown) || BindingUsesController(teleport) ||
               BindingUsesController(focus) || BindingUsesController(sprint);
    }

    /// <summary>
    /// Whether any shade binding at all sits on this controller, the command binding included.
    /// For reserving a pad away from Hornet use <see cref="ReservesControllerIndex"/> instead.
    /// </summary>
    public bool IsControllerIndexInUse(int index)
    {
        if (index < 0)
            return false;
        int fallbackIndex = Mathf.Max(-1, controllerDeviceIndex);
        return BindingUsesControllerIndex(moveLeft, fallbackIndex, index) ||
               BindingUsesControllerIndex(moveRight, fallbackIndex, index) ||
               BindingUsesControllerIndex(moveUp, fallbackIndex, index) ||
               BindingUsesControllerIndex(moveDown, fallbackIndex, index) ||
               BindingUsesControllerIndex(fire, fallbackIndex, index) ||
               BindingUsesControllerIndex(nail, fallbackIndex, index) ||
               BindingUsesControllerIndex(nailUp, fallbackIndex, index) ||
               BindingUsesControllerIndex(nailDown, fallbackIndex, index) ||
               BindingUsesControllerIndex(teleport, fallbackIndex, index) ||
               BindingUsesControllerIndex(focus, fallbackIndex, index) ||
               BindingUsesControllerIndex(sprint, fallbackIndex, index) ||
               BindingUsesControllerIndex(commandShade, fallbackIndex, index);
    }

    /// <summary>Every action a binding can be held for, so a sweep cannot miss one.</summary>
    internal static readonly ShadeAction[] AllActions =
        (ShadeAction[])Enum.GetValues(typeof(ShadeAction));

    /// <summary>
    /// Clears any binding that was captured as a device-agnostic joystick key, and reports how many
    /// it cleared.
    /// <para>
    /// Rebinding on a pad used to store <c>KeyCode.JoystickButtonN</c>, which fires on every
    /// attached controller regardless of the device the binding names. Those are now ignored when
    /// read, but an ignored binding is an invisible one - the Controls screen would still show the
    /// button while nothing happened. Clearing them instead leaves the row plainly unbound, and
    /// rebinding it now records the device properly.
    /// </para>
    /// </summary>
    public int ClearControllerKeyBindings()
    {
        int cleared = 0;

        foreach (var action in AllActions)
        {
            var binding = GetBinding(action);
            if (binding == null)
                continue;

            if (binding.primary.type == ShadeBindingOptionType.Key && ShadeInput.IsControllerKeyCode(binding.primary.key))
            {
                binding.primary = ShadeBindingOption.None();
                cleared++;
            }

            if (binding.secondary.type == ShadeBindingOptionType.Key && ShadeInput.IsControllerKeyCode(binding.secondary.key))
            {
                binding.secondary = ShadeBindingOption.None();
                cleared++;
            }
        }

        return cleared;
    }

    public ShadeBinding GetBinding(ShadeAction action) => action switch
    {
        ShadeAction.MoveLeft => moveLeft,
        ShadeAction.MoveRight => moveRight,
        ShadeAction.MoveUp => moveUp,
        ShadeAction.MoveDown => moveDown,
        ShadeAction.Fire => fire,
        ShadeAction.Nail => nail,
        ShadeAction.NailUp => nailUp,
        ShadeAction.NailDown => nailDown,
        ShadeAction.Teleport => teleport,
        ShadeAction.Focus => focus,
        ShadeAction.Sprint => sprint,
        ShadeAction.CommandShade => commandShade,
        ShadeAction.DebugDamageShade => debugDamageShade,
        ShadeAction.DebugHealShade => debugHealShade,
        ShadeAction.DebugSoulIncrease => debugSoulIncrease,
        ShadeAction.DebugSoulDecrease => debugSoulDecrease,
        ShadeAction.DebugSoulReset => debugSoulReset,
        _ => moveLeft
    };

    public void SetBinding(ShadeAction action, ShadeBinding binding)
    {
        switch (action)
        {
            case ShadeAction.MoveLeft:
                moveLeft = binding;
                break;
            case ShadeAction.MoveRight:
                moveRight = binding;
                break;
            case ShadeAction.MoveUp:
                moveUp = binding;
                break;
            case ShadeAction.MoveDown:
                moveDown = binding;
                break;
            case ShadeAction.Fire:
                fire = binding;
                break;
            case ShadeAction.Nail:
                nail = binding;
                break;
            case ShadeAction.NailUp:
                nailUp = binding;
                break;
            case ShadeAction.NailDown:
                nailDown = binding;
                break;
            case ShadeAction.Teleport:
                teleport = binding;
                break;
            case ShadeAction.Focus:
                focus = binding;
                break;
            case ShadeAction.Sprint:
                sprint = binding;
                break;
            case ShadeAction.CommandShade:
                commandShade = binding;
                break;
            case ShadeAction.DebugDamageShade:
                debugDamageShade = binding;
                break;
            case ShadeAction.DebugHealShade:
                debugHealShade = binding;
                break;
            case ShadeAction.DebugSoulIncrease:
                debugSoulIncrease = binding;
                break;
            case ShadeAction.DebugSoulDecrease:
                debugSoulDecrease = binding;
                break;
            case ShadeAction.DebugSoulReset:
                debugSoulReset = binding;
                break;
        }
    }

    public void SetBindingOption(ShadeAction action, bool secondary, ShadeBindingOption option)
    {
        var binding = GetBinding(action);
        if (binding == null)
        {
            binding = new ShadeBinding();
            SetBinding(action, binding);
        }
        if (secondary)
            binding.secondary = option;
        else
            binding.primary = option;
    }

    private static ShadeBinding CloneBinding(ShadeBinding binding)
    {
        return binding != null ? binding.Clone() : new ShadeBinding();
    }

    public ShadeInputConfig Clone()
    {
        var clone = new ShadeInputConfig();
        clone.controllerDeviceIndex = controllerDeviceIndex;
        clone.controllerDeadzone = controllerDeadzone;
        clone.moveLeft = CloneBinding(moveLeft);
        clone.moveRight = CloneBinding(moveRight);
        clone.moveUp = CloneBinding(moveUp);
        clone.moveDown = CloneBinding(moveDown);
        clone.fire = CloneBinding(fire);
        clone.nail = CloneBinding(nail);
        clone.nailUp = CloneBinding(nailUp);
        clone.nailDown = CloneBinding(nailDown);
        clone.teleport = CloneBinding(teleport);
        clone.focus = CloneBinding(focus);
        clone.sprint = CloneBinding(sprint);
        clone.commandShade = CloneBinding(commandShade);
        clone.debugDamageShade = CloneBinding(debugDamageShade);
        clone.debugHealShade = CloneBinding(debugHealShade);
        clone.debugSoulIncrease = CloneBinding(debugSoulIncrease);
        clone.debugSoulDecrease = CloneBinding(debugSoulDecrease);
        clone.debugSoulReset = CloneBinding(debugSoulReset);
        return clone;
    }

    public void CopyBindingsFrom(ShadeInputConfig other)
    {
        if (other == null)
            return;

        controllerDeviceIndex = other.controllerDeviceIndex;
        controllerDeadzone = other.controllerDeadzone;
        moveLeft = CloneBinding(other.moveLeft);
        moveRight = CloneBinding(other.moveRight);
        moveUp = CloneBinding(other.moveUp);
        moveDown = CloneBinding(other.moveDown);
        fire = CloneBinding(other.fire);
        nail = CloneBinding(other.nail);
        nailUp = CloneBinding(other.nailUp);
        nailDown = CloneBinding(other.nailDown);
        teleport = CloneBinding(other.teleport);
        focus = CloneBinding(other.focus);
        sprint = CloneBinding(other.sprint);
        commandShade = CloneBinding(other.commandShade);
        debugDamageShade = CloneBinding(other.debugDamageShade);
        debugHealShade = CloneBinding(other.debugHealShade);
        debugSoulIncrease = CloneBinding(other.debugSoulIncrease);
        debugSoulDecrease = CloneBinding(other.debugSoulDecrease);
        debugSoulReset = CloneBinding(other.debugSoulReset);
    }
}

public static class ShadeInput
{
    private static readonly KeyCode[] AllKeyCodes = Enum.GetValues(typeof(KeyCode)) as KeyCode[] ?? Array.Empty<KeyCode>();

    /// <summary>
    /// Whether a <see cref="KeyCode"/> is a controller button rather than a key.
    /// <para>
    /// These have no place in a Shade binding, and the reason is the whole point of this method.
    /// <c>KeyCode.JoystickButton0</c> and its siblings are Unity's <em>device-agnostic</em> pad
    /// buttons: <c>Input.GetKey(JoystickButton1)</c> is true when button 1 is down on <em>any</em>
    /// attached pad. A Shade control captured as one of those fires when Hornet's player presses the
    /// same button, whatever device the binding claims to be on - the key path never consults a
    /// device at all. The numbered <c>Joystick1Button0</c> forms are per-device, but they are keyed
    /// to Unity's joystick numbering rather than to InControl's device list, which is what the rest
    /// of this file reserves and reads by, so they are no use here either.
    /// </para>
    /// <para>
    /// Controller presses belong on the controller path, which records which device they came from.
    /// See <see cref="TryCaptureKey"/> and <see cref="TryCaptureControl"/>.
    /// </para>
    /// </summary>
    internal static bool IsControllerKeyCode(KeyCode code)
    {
        return code >= KeyCode.JoystickButton0;
    }
    private static readonly InputControlType[] CaptureControls =
    {
        InputControlType.Action1,
        InputControlType.Action2,
        InputControlType.Action3,
        InputControlType.Action4,
        InputControlType.Action5,
        InputControlType.Action6,
        InputControlType.LeftTrigger,
        InputControlType.RightTrigger,
        InputControlType.LeftBumper,
        InputControlType.RightBumper,
        InputControlType.LeftStickButton,
        InputControlType.RightStickButton,
        InputControlType.LeftStickUp,
        InputControlType.LeftStickDown,
        InputControlType.LeftStickLeft,
        InputControlType.LeftStickRight,
        InputControlType.DPadUp,
        InputControlType.DPadDown,
        InputControlType.DPadLeft,
        InputControlType.DPadRight,
        InputControlType.Start,
        InputControlType.Back,
        InputControlType.Select,
        InputControlType.Options,
        InputControlType.Command
    };

    private static ShadeInputConfig ConfigInstance
    {
        get
        {
            var cfg = ModConfig.Instance;
            if (cfg.shadeInput == null)
            {
                cfg.shadeInput = ShadeInputConfig.CreateDefault();
            }
            return cfg.shadeInput;
        }
    }

    public static ShadeInputConfig Config => ConfigInstance;

    public static ShadeBindingOption GetBindingOption(ShadeAction action, bool secondary)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return ShadeBindingOption.None();
        return secondary ? binding.secondary : binding.primary;
    }

    public static void SetBindingOption(ShadeAction action, bool secondary, ShadeBindingOption option)
    {
        if (option.type != ShadeBindingOptionType.Controller)
        {
            option.controllerDevice = -1;
        }
        ConfigInstance.SetBindingOption(action, secondary, option);
    }

    public static float GetActionValue(ShadeAction action) => GetActionValue(action, null);

    // The three reads below are the only place the Shade AI exists as far as the rest of the mod is
    // concerned: it publishes a frame of synthesised input and every handler that polls an action
    // gets it without knowing. A caller that names a requiredType is asking about physical hardware
    // (the Hornet-input bridge in LegacyHelper.Patches.cs, and the bindings UI), so those read past
    // it. See LegacyoftheAbyss.Shade.Ai.ShadeAiInput.
    internal static float GetActionValue(ShadeAction action, ShadeBindingOptionType? requiredType)
    {
        if (requiredType == null)
        {
            if (ShadeAiInput.TryGetValue(action, out float driven))
                return driven;
            if (ShadeAiInput.Suppressed(action))
                return 0f;
        }

        return GetActionValueRaw(action, requiredType);
    }

    internal static float GetActionValueRaw(ShadeAction action, ShadeBindingOptionType? requiredType = null)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return 0f;
        return Mathf.Max(GetOptionValue(binding.primary, requiredType), GetOptionValue(binding.secondary, requiredType));
    }

    public static bool IsActionHeld(ShadeAction action) => IsActionHeld(action, null);

    internal static bool IsActionHeld(ShadeAction action, ShadeBindingOptionType? requiredType)
    {
        if (requiredType == null)
        {
            if (ShadeAiInput.TryGetHeld(action, out bool driven))
                return driven;
            if (ShadeAiInput.Suppressed(action))
                return false;
        }

        return IsActionHeldRaw(action, requiredType);
    }

    internal static bool IsActionHeldRaw(ShadeAction action, ShadeBindingOptionType? requiredType = null)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return false;
        return IsOptionHeld(binding.primary, requiredType) || IsOptionHeld(binding.secondary, requiredType);
    }

    public static bool WasActionPressed(ShadeAction action) => WasActionPressed(action, null);

    internal static bool WasActionPressed(ShadeAction action, ShadeBindingOptionType? requiredType)
    {
        if (requiredType == null)
        {
            if (ShadeAiInput.TryGetPressed(action, out bool driven))
                return driven;
            if (ShadeAiInput.Suppressed(action))
                return false;
        }

        return WasActionPressedRaw(action, requiredType);
    }

    internal static bool WasActionPressedRaw(ShadeAction action, ShadeBindingOptionType? requiredType = null)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return false;
        return WasOptionPressed(binding.primary, requiredType) || WasOptionPressed(binding.secondary, requiredType);
    }

    /// <summary>
    /// A stick on whichever device an action's controller binding points at, or zero when it has
    /// none. Used to steer the Shade command reticle with the same pad that opened it, rather than
    /// guessing which device the player is holding.
    /// </summary>
    /// <param name="rightStick">
    /// True for the right stick. The reticle uses it because the left stick is Hornet's movement and
    /// aiming must not cost the player the ability to walk.
    /// </param>
    public static Vector2 GetActionStick(ShadeAction action, bool rightStick)
    {
        try
        {
            var binding = ConfigInstance.GetBinding(action);
            if (binding == null)
                return Vector2.zero;

            Vector2 primary = ReadStick(binding.primary, rightStick);
            return primary.sqrMagnitude > 0f ? primary : ReadStick(binding.secondary, rightStick);
        }
        catch
        {
            return Vector2.zero;
        }
    }

    private static Vector2 ReadStick(ShadeBindingOption option, bool rightStick)
    {
        if (option.type != ShadeBindingOptionType.Controller)
            return Vector2.zero;

        var device = GetDeviceForOption(option);
        if (device == null || device == InputDevice.Null)
            return Vector2.zero;

        var stick = rightStick
            ? new Vector2(device.RightStickX.Value, device.RightStickY.Value)
            : new Vector2(device.LeftStickX.Value, device.LeftStickY.Value);
        float deadzone = Mathf.Clamp(ConfigInstance.controllerDeadzone, 0.01f, 1f);
        return stick.sqrMagnitude >= deadzone * deadzone ? stick : Vector2.zero;
    }

    public static string DescribeBindingOption(ShadeBindingOption option)
    {
        return option.type switch
        {
            ShadeBindingOptionType.Key => DescribeKey(option.key),
            ShadeBindingOptionType.Controller => DescribeControl(option.control, GetEffectiveControllerIndex(option)),
            _ => "Unbound"
        };
    }

    public static void EnsureControllerIndex(int index)
    {
        ConfigInstance.controllerDeviceIndex = index;
    }

    private static bool ShouldSuppressOption(ShadeBindingOption option)
    {
        // Every Shade input read funnels through here, which makes it the one place that can keep
        // the Shade from acting on a bug report being typed into the overlay. Blocking at the
        // binding level rather than per-action means a new action is covered for free.
        if (LegacyoftheAbyss.Diagnostics.BugReportSystem.IsCapturingText)
        {
            return true;
        }

        try
        {
            return LegacyHelper.InputDeviceBlocker.ShouldSuppressShadeOption(option);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsOptionHeld(ShadeBindingOption option, ShadeBindingOptionType? requiredType = null)
    {
        if (requiredType.HasValue && option.type != requiredType.Value)
            return false;
        if (ShouldSuppressOption(option))
            return false;

        return option.type switch
        {
            ShadeBindingOptionType.Key => option.key != KeyCode.None && Input.GetKey(option.key),
            ShadeBindingOptionType.Controller => GetControl(option, out var control) && control.IsPressed,
            _ => false
        };
    }

    private static bool WasOptionPressed(ShadeBindingOption option, ShadeBindingOptionType? requiredType = null)
    {
        if (requiredType.HasValue && option.type != requiredType.Value)
            return false;
        if (ShouldSuppressOption(option))
            return false;

        return option.type switch
        {
            ShadeBindingOptionType.Key => option.key != KeyCode.None && Input.GetKeyDown(option.key),
            ShadeBindingOptionType.Controller => GetControl(option, out var control) && control.WasPressed,
            _ => false
        };
    }

    private static float GetOptionValue(ShadeBindingOption option)
        => GetOptionValue(option, null);

    private static float GetOptionValue(ShadeBindingOption option, ShadeBindingOptionType? requiredType)
    {
        if (requiredType.HasValue && option.type != requiredType.Value)
            return 0f;
        if (ShouldSuppressOption(option))
            return 0f;

        return option.type switch
        {
            // The joystick guard matters for configs written before capture was fixed: such a
            // binding reads every attached pad, so it is ignored rather than left cross-firing.
            ShadeBindingOptionType.Key => option.key != KeyCode.None && !IsControllerKeyCode(option.key) && Input.GetKey(option.key) ? 1f : 0f,
            ShadeBindingOptionType.Controller => GetControl(option, out var control) ? Mathf.Clamp01(Mathf.Abs(control.Value)) : 0f,
            _ => 0f
        };
    }

    private static bool GetControl(ShadeBindingOption option, out InputControl control)
    {
        control = InputControl.Null;
        if (option.type != ShadeBindingOptionType.Controller)
            return false;
        try
        {
            var device = GetDeviceForOption(option);
            if (device == null || device == InputDevice.Null)
                return false;
            control = device.GetControl(option.control);
            return control != null && control != InputControl.Null;
        }
        catch
        {
            control = InputControl.Null;
            return false;
        }
    }

    /// <summary>
    /// The one device a Shade controller binding is allowed to read, or null.
    /// <para>
    /// Null rather than a stand-in, deliberately. This used to fall back to
    /// <c>InputManager.ActiveDevice</c> when the binding named no device and to the last attached
    /// pad when it named one that is not plugged in - both of which hand the Shade whichever
    /// controller the other player happens to be holding. A Shade control whose device is absent
    /// should do nothing, not something on someone else's pad.
    /// </para>
    /// </summary>
    private static InputDevice? GetDeviceForOption(ShadeBindingOption option)
    {
        int index = GetEffectiveControllerIndex(option);
        if (index < 0)
            return null;

        var devices = InputManager.Devices;
        if (devices == null || index >= devices.Count)
            return null;

        var selected = devices[index];
        return selected ?? InputDevice.Null;
    }

    private static int GetEffectiveControllerIndex(ShadeBindingOption option)
    {
        if (option.controllerDevice >= 0)
            return option.controllerDevice;
        return Mathf.Max(-1, ConfigInstance.controllerDeviceIndex);
    }

    private static string DescribeKey(KeyCode key)
    {
        if (key == KeyCode.None)
            return "Unbound";

        // Unity numbers the mouse from zero, so the enum name renders as "Mouse 0" - which is
        // nobody's name for the left button. The first three get what players call them; the rest
        // keep a number, shifted up one so it agrees with how mice are labelled.
        switch (key)
        {
            case KeyCode.Mouse0:
                return "LMB";
            case KeyCode.Mouse1:
                return "RMB";
            case KeyCode.Mouse2:
                return "MMB";
            case KeyCode.Mouse3:
            case KeyCode.Mouse4:
            case KeyCode.Mouse5:
            case KeyCode.Mouse6:
                return "Mouse " + ((int)key - (int)KeyCode.Mouse0 + 1).ToString(CultureInfo.InvariantCulture);
        }

        return FormatEnumName(key.ToString());
    }

    private static string DescribeControl(InputControlType control, int deviceIndex)
    {
        string controlName = FormatEnumName(control.ToString());
        if (deviceIndex < 0)
            return controlName;
        return $"Controller {deviceIndex + 1} {controlName}";
    }

    private static string FormatEnumName(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return raw;
        var sb = new StringBuilder(raw.Length * 2);
        char previous = '\0';
        foreach (char c in raw)
        {
            if (c == '_')
            {
                if (sb.Length > 0 && sb[sb.Length - 1] != ' ')
                    sb.Append(' ');
                previous = c;
                continue;
            }
            if (char.IsUpper(c) && previous != '\0' && !char.IsUpper(previous) && previous != ' ')
                sb.Append(' ');
            else if (char.IsDigit(c) && previous != '\0' && !char.IsDigit(previous) && previous != ' ')
                sb.Append(' ');
            sb.Append(c);
            previous = c;
        }
        return sb.ToString();
    }

    public static bool TryCaptureKey(out KeyCode key)
    {
        foreach (var code in AllKeyCodes)
        {
            if (code == KeyCode.None)
                continue;

            // Skipped so the press falls through to TryCaptureControl, which records the device it
            // came from. This check ran first and matched everything, so every rebind made on a pad
            // was stored as a device-agnostic joystick key and fired on both players' controllers.
            if (IsControllerKeyCode(code))
                continue;

            if (Input.GetKeyDown(code))
            {
                key = code;
                return true;
            }
        }
        key = KeyCode.None;
        return false;
    }

    public static bool TryCaptureControl(out InputControlType controlType, out int deviceIndex)
    {
        controlType = InputControlType.None;
        deviceIndex = -1;
        var devices = InputManager.Devices;
        if (devices == null || devices.Count == 0)
            return false;
        for (int i = 0; i < devices.Count; i++)
        {
            var device = devices[i];
            foreach (var controlCandidate in CaptureControls)
            {
                var control = device.GetControl(controlCandidate);
                if (control == null || control == InputControl.Null)
                    continue;
                if (control.WasPressed)
                {
                    controlType = controlCandidate;
                    deviceIndex = i;
                    return true;
                }
            }
        }
        return false;
    }
}
