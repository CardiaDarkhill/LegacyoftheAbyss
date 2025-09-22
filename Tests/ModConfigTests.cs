using InControl;
using UnityEngine;
using Xunit;

public class ModConfigTests
{
    [Fact]
    public void SavesAndLoadsDifficulty()
    {
        var cfg = ModConfig.Instance;
        cfg.hornetDamageMultiplier = 1.5f;
        cfg.shadeDamageMultiplier = 0.8f;
        cfg.bindHornetHeal = 4;
        cfg.bindShadeHeal = 1;
        cfg.focusHornetHeal = 2;
        cfg.focusShadeHeal = 3;
        cfg.logDamage = true;
        cfg.shadeEnabled = false;
        ModConfig.Save();
        var loaded = ModConfig.Load();
        Assert.Equal(1.5f, loaded.hornetDamageMultiplier, 3);
        Assert.Equal(0.8f, loaded.shadeDamageMultiplier, 3);
        Assert.Equal(4, loaded.bindHornetHeal);
        Assert.Equal(1, loaded.bindShadeHeal);
        Assert.Equal(2, loaded.focusHornetHeal);
        Assert.Equal(3, loaded.focusShadeHeal);
        Assert.True(loaded.logDamage);
        Assert.False(loaded.shadeEnabled);
        loaded.shadeEnabled = true;
        ModConfig.Save();
    }

    [Fact]
    public void ShadeBindingRebindPersists()
    {
        var cfg = ModConfig.Instance;
        cfg.shadeInput.ResetToDefaults();
        cfg.shadeInput.SetBindingOption(ShadeAction.Nail, false, ShadeBindingOption.FromKey(KeyCode.P));
        cfg.shadeInput.SetBindingOption(ShadeAction.Nail, true, ShadeBindingOption.None());
        ModConfig.Save();
        var loaded = ModConfig.Load();
        var binding = loaded.shadeInput.GetBinding(ShadeAction.Nail);
        Assert.Equal(ShadeBindingOptionType.Key, binding.primary.type);
        Assert.Equal(KeyCode.P, binding.primary.key);
    }

    [Fact]
    public void ShadeControllerBindingPersists()
    {
        var cfg = ModConfig.Instance;
        cfg.shadeInput.ResetToDefaults();
        cfg.shadeInput.controllerDeviceIndex = 2;
        cfg.shadeInput.SetBindingOption(ShadeAction.MoveLeft, false, ShadeBindingOption.FromControl(InputControlType.LeftStickLeft, 1));
        cfg.shadeInput.SetBindingOption(ShadeAction.Focus, true, ShadeBindingOption.FromControl(InputControlType.RightTrigger));
        ModConfig.Save();
        var loaded = ModConfig.Load();

        Assert.Equal(2, loaded.shadeInput.controllerDeviceIndex);

        var moveLeft = loaded.shadeInput.GetBinding(ShadeAction.MoveLeft);
        Assert.NotNull(moveLeft);
        Assert.Equal(ShadeBindingOptionType.Controller, moveLeft.primary.type);
        Assert.Equal(InputControlType.LeftStickLeft, moveLeft.primary.control);
        Assert.Equal(1, moveLeft.primary.controllerDevice);

        var focus = loaded.shadeInput.GetBinding(ShadeAction.Focus);
        Assert.NotNull(focus);
        Assert.Equal(ShadeBindingOptionType.Controller, focus.secondary.type);
        Assert.Equal(InputControlType.RightTrigger, focus.secondary.control);
        Assert.Equal(-1, focus.secondary.controllerDevice);
    }
}
