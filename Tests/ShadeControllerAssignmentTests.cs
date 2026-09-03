using InControl;
using UnityEngine;
using Xunit;

/// <summary>
/// Assigning a controller to each player.
/// <para>
/// The mod stores the companion's pad as an index into <c>InputManager.Devices</c>, but a control
/// rebound on a pad also remembers that pad in the binding itself - and the remembered device wins.
/// So the assignment has to move both or it appears not to work for anyone who has rebound
/// anything.
/// </para>
/// </summary>
public class ShadeControllerAssignmentTests
{
    private static ShadeInputConfig Fresh() => ShadeInputConfig.CreateDefault();

    [Fact]
    public void AssigningControllersMovesTheBindingsThatRememberADevice()
    {
        // The bug this exists for: a control rebound on a pad stores that pad in the binding, and a
        // stored device outranks the config-level index - so moving the companion to another
        // controller left every rebound control still answering to the old one.
        var config = Fresh();
        config.SetBinding(
            ShadeAction.Nail,
            new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action1, 0), ShadeBindingOption.None()));
        config.SetBinding(
            ShadeAction.Focus,
            new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action2, 0), ShadeBindingOption.None()));

        int moved = config.ApplyControllerAssignment(hornetIndex: 0, companionIndex: 1);

        Assert.Equal(1, config.controllerDeviceIndex);
        Assert.Equal(1, config.GetBinding(ShadeAction.Nail).primary.controllerDevice);
        Assert.Equal(1, config.GetBinding(ShadeAction.Focus).primary.controllerDevice);
        Assert.True(moved >= 2);
    }

    [Fact]
    public void CommandShadeStaysOnHornetsPad()
    {
        // It is the button Hornet's player presses to send the companion somewhere, which is why it
        // is the one action excluded from the reservation. Moving it with the rest would put it on
        // the pad it must never be on.
        var config = Fresh();
        config.SetBinding(
            ShadeAction.CommandShade,
            new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickButton, 1), ShadeBindingOption.None()));

        config.ApplyControllerAssignment(hornetIndex: 0, companionIndex: 1);

        Assert.Equal(0, config.GetBinding(ShadeAction.CommandShade).primary.controllerDevice);
    }

    [Fact]
    public void AssigningControllersLeavesKeyBindingsAlone()
    {
        var config = Fresh();
        config.SetBinding(
            ShadeAction.Nail,
            new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.J), ShadeBindingOption.None()));

        config.ApplyControllerAssignment(hornetIndex: 0, companionIndex: 1);

        var nail = config.GetBinding(ShadeAction.Nail).primary;
        Assert.Equal(ShadeBindingOptionType.Key, nail.type);
        Assert.Equal(KeyCode.J, nail.key);
    }

}
