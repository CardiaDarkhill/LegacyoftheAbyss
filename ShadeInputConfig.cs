using System;
using System.Text;
using InControl;
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
    AssistMode
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
    public ShadeBinding assistMode = new();

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
        assistMode = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Alpha0), ShadeBindingOption.None());
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
        assistMode = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.RightStickButton), ShadeBindingOption.None());
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
        assistMode = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Keypad9), ShadeBindingOption.None());
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
        assistMode = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.RightStickButton), ShadeBindingOption.None());
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
               BindingUsesController(focus) || BindingUsesController(sprint) || BindingUsesController(assistMode);
    }

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
               BindingUsesControllerIndex(assistMode, fallbackIndex, index);
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
        ShadeAction.AssistMode => assistMode,
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
            case ShadeAction.AssistMode:
                assistMode = binding;
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
        clone.assistMode = CloneBinding(assistMode);
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
        assistMode = CloneBinding(other.assistMode);
    }
}

public static class ShadeInput
{
    private static readonly KeyCode[] AllKeyCodes = Enum.GetValues(typeof(KeyCode)) as KeyCode[] ?? Array.Empty<KeyCode>();
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

    public static float GetActionValue(ShadeAction action)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return 0f;
        return Mathf.Max(GetOptionValue(binding.primary), GetOptionValue(binding.secondary));
    }

    public static bool IsActionHeld(ShadeAction action)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return false;
        return IsOptionHeld(binding.primary) || IsOptionHeld(binding.secondary);
    }

    public static bool WasActionPressed(ShadeAction action)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return false;
        return WasOptionPressed(binding.primary) || WasOptionPressed(binding.secondary);
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
        try
        {
            return LegacyHelper.InputDeviceBlocker.ShouldSuppressShadeOption(option);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsOptionHeld(ShadeBindingOption option)
    {
        if (ShouldSuppressOption(option))
            return false;

        return option.type switch
        {
            ShadeBindingOptionType.Key => option.key != KeyCode.None && Input.GetKey(option.key),
            ShadeBindingOptionType.Controller => GetControl(option, out var control) && control.IsPressed,
            _ => false
        };
    }

    private static bool WasOptionPressed(ShadeBindingOption option)
    {
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
    {
        if (ShouldSuppressOption(option))
            return 0f;

        return option.type switch
        {
            ShadeBindingOptionType.Key => option.key != KeyCode.None && Input.GetKey(option.key) ? 1f : 0f,
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
            if (device == null)
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

    private static InputDevice? GetDeviceForOption(ShadeBindingOption option)
    {
        int index = GetEffectiveControllerIndex(option);
        if (index < 0)
            return null;
        var devices = InputManager.Devices;
        if (devices == null || devices.Count == 0)
            return null;
        if (index >= devices.Count)
            index = devices.Count - 1;
        return devices[index];
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
