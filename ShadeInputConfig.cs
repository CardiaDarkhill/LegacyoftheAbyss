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
    Teleport,
    Focus,
    Sprint,
    DamageToggle
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
    public ShadeBinding teleport = new();
    public ShadeBinding focus = new();
    public ShadeBinding sprint = new();
    public ShadeBinding damageToggle = new();

    public ShadeInputConfig()
    {
        ResetToDefaults();
    }

    public static ShadeInputConfig CreateDefault() => new ShadeInputConfig();

    public void ResetToDefaults()
    {
        controllerDeviceIndex = 1;
        controllerDeadzone = Mathf.Clamp(controllerDeadzone <= 0f ? 0.25f : controllerDeadzone, 0.01f, 1f);

        moveLeft = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.A), ShadeBindingOption.FromControl(InputControlType.LeftStickLeft));
        moveRight = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.D), ShadeBindingOption.FromControl(InputControlType.LeftStickRight));
        moveUp = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.W), ShadeBindingOption.FromControl(InputControlType.LeftStickUp));
        moveDown = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.S), ShadeBindingOption.FromControl(InputControlType.LeftStickDown));
        fire = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Space), ShadeBindingOption.FromControl(InputControlType.RightTrigger));
        nail = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.J), ShadeBindingOption.FromControl(InputControlType.Action1));
        teleport = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.K), ShadeBindingOption.FromControl(InputControlType.Action3));
        focus = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.H), ShadeBindingOption.FromControl(InputControlType.LeftTrigger));
        sprint = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.LeftShift), ShadeBindingOption.FromControl(InputControlType.LeftBumper));
        damageToggle = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Alpha0), ShadeBindingOption.None());
    }

    public void ApplySharedKeyboardPreset()
    {
        controllerDeviceIndex = -1;
        moveLeft = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.LeftArrow), ShadeBindingOption.None());
        moveRight = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.RightArrow), ShadeBindingOption.None());
        moveUp = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.UpArrow), ShadeBindingOption.None());
        moveDown = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.DownArrow), ShadeBindingOption.None());
        fire = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Keypad1), ShadeBindingOption.None());
        nail = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Keypad2), ShadeBindingOption.None());
        teleport = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Keypad3), ShadeBindingOption.None());
        focus = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.KeypadEnter), ShadeBindingOption.None());
        sprint = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.RightControl), ShadeBindingOption.FromKey(KeyCode.Keypad0));
        damageToggle = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.KeypadPeriod), ShadeBindingOption.None());
    }

    public ShadeBinding GetBinding(ShadeAction action) => action switch
    {
        ShadeAction.MoveLeft => moveLeft,
        ShadeAction.MoveRight => moveRight,
        ShadeAction.MoveUp => moveUp,
        ShadeAction.MoveDown => moveDown,
        ShadeAction.Fire => fire,
        ShadeAction.Nail => nail,
        ShadeAction.Teleport => teleport,
        ShadeAction.Focus => focus,
        ShadeAction.Sprint => sprint,
        ShadeAction.DamageToggle => damageToggle,
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
            case ShadeAction.Teleport:
                teleport = binding;
                break;
            case ShadeAction.Focus:
                focus = binding;
                break;
            case ShadeAction.Sprint:
                sprint = binding;
                break;
            case ShadeAction.DamageToggle:
                damageToggle = binding;
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

    private static bool IsOptionHeld(ShadeBindingOption option)
    {
        return option.type switch
        {
            ShadeBindingOptionType.Key => option.key != KeyCode.None && Input.GetKey(option.key),
            ShadeBindingOptionType.Controller => GetControl(option, out var control) && control.IsPressed,
            _ => false
        };
    }

    private static bool WasOptionPressed(ShadeBindingOption option)
    {
        return option.type switch
        {
            ShadeBindingOptionType.Key => option.key != KeyCode.None && Input.GetKeyDown(option.key),
            ShadeBindingOptionType.Controller => GetControl(option, out var control) && control.WasPressed,
            _ => false
        };
    }

    private static float GetOptionValue(ShadeBindingOption option)
    {
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

    private static InputDevice GetDeviceForOption(ShadeBindingOption option)
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
                if (sb.Length > 0 && sb[^1] != ' ')
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
