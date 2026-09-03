using UnityEngine;
using Xunit;

/// <summary>
/// A Shade control must only ever answer to the Shade player's controller.
/// <para>
/// The bug these were written for: rebinding on a pad stored
/// <c>KeyCode.JoystickButtonN</c>, which is Unity's <em>device-agnostic</em> pad button -
/// <c>Input.GetKey</c> on one is true when that button is down on <em>any</em> attached controller,
/// and the key path never consults a device at all. Two rebound Shade controls therefore fired
/// whenever Hornet's player pressed the same buttons.
/// </para>
/// </summary>
public class ShadeBindingDeviceTests
{
    [Theory]
    [InlineData(KeyCode.JoystickButton0)]
    [InlineData(KeyCode.JoystickButton1)]
    [InlineData(KeyCode.JoystickButton5)]
    [InlineData(KeyCode.JoystickButton19)]
    [InlineData(KeyCode.Joystick1Button0)]
    [InlineData(KeyCode.Joystick8Button19)]
    public void ControllerButtonsAreNotKeys(KeyCode code)
    {
        Assert.True(ShadeInput.IsControllerKeyCode(code),
            $"{code} is a pad button and must go down the controller path, which records its device.");
    }

    /// <summary>
    /// The other half: ordinary keys must still be bindable, or the keyboard preset stops working.
    /// </summary>
    [Theory]
    [InlineData(KeyCode.A)]
    [InlineData(KeyCode.Space)]
    [InlineData(KeyCode.LeftShift)]
    [InlineData(KeyCode.Keypad0)]
    [InlineData(KeyCode.Mouse0)]
    [InlineData(KeyCode.Mouse2)]
    [InlineData(KeyCode.RightArrow)]
    public void RealKeysAndMouseButtonsStillCount(KeyCode code)
    {
        Assert.False(ShadeInput.IsControllerKeyCode(code),
            $"{code} is a key or mouse button and must stay bindable.");
    }

    /// <summary>
    /// A config written before the capture path was fixed heals itself: the offending bindings are
    /// cleared rather than left reading every pad, so the Controls screen shows them unbound and
    /// rebinding records the device properly.
    /// </summary>
    [Fact]
    public void ABindingCapturedFromAPadIsClearedOnLoad()
    {
        var config = new ShadeInputConfig();
        config.ApplyControllerLayout(deviceIndex: 1);

        // Exactly what was found in the wild: focus and sprint stored as joystick keys.
        config.focus.primary = ShadeBindingOption.FromKey(KeyCode.JoystickButton1);
        config.sprint.primary = ShadeBindingOption.FromKey(KeyCode.JoystickButton5);

        int cleared = config.ClearControllerKeyBindings();

        Assert.Equal(2, cleared);
        Assert.Equal(ShadeBindingOptionType.None, config.focus.primary.type);
        Assert.Equal(ShadeBindingOptionType.None, config.sprint.primary.type);
    }

    /// <summary>Nothing else is touched - a clean config must survive the sweep unchanged.</summary>
    [Fact]
    public void AGoodConfigIsLeftAlone()
    {
        var controllers = new ShadeInputConfig();
        controllers.ApplyControllerLayout(deviceIndex: 1);
        Assert.Equal(0, controllers.ClearControllerKeyBindings());

        var keyboard = new ShadeInputConfig();
        keyboard.ApplyKeyboardLayout();
        Assert.Equal(0, keyboard.ClearControllerKeyBindings());

        // The keyboard preset's mouse binds are the ones most at risk from a sloppy range check.
        Assert.Equal(ShadeBindingOptionType.Key, keyboard.commandShade.primary.type);
        Assert.Equal(KeyCode.Mouse2, keyboard.commandShade.primary.key);
    }
}
