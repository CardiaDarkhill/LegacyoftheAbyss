using InControl;
using UnityEngine;
using Xunit;

[Collection(ModConfigCollection.Name)]
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
    public void SavesAndLoadsVisualSettings()
    {
        var cfg = ModConfig.Instance;
        string originalLayer = cfg.shadeSortingLayer;
        cfg.shadeSortingLayer = "Actors";
        cfg.shadeSortingOrderOffset = -2;
        cfg.shadeUseHornetMaterial = false;
        cfg.shadeShadowParticlesEnabled = false;
        cfg.shadeShadowParticleIntensity = 1.4f;
        cfg.shadeSkinPreviewSmoothing = false;
        cfg.shadeSpriteSmoothing = true;
        ModConfig.Save();

        var loaded = ModConfig.Load();
        Assert.Equal("Actors", loaded.shadeSortingLayer);
        Assert.Equal(-2, loaded.shadeSortingOrderOffset);
        Assert.False(loaded.shadeUseHornetMaterial);
        Assert.False(loaded.shadeShadowParticlesEnabled);
        Assert.Equal(1.4f, loaded.shadeShadowParticleIntensity, 3);
        Assert.False(loaded.shadeSkinPreviewSmoothing);
        Assert.True(loaded.shadeSpriteSmoothing);

        loaded.shadeSortingLayer = originalLayer;
        loaded.shadeSortingOrderOffset = 1;
        loaded.shadeUseHornetMaterial = true;
        loaded.shadeShadowParticlesEnabled = true;
        loaded.shadeShadowParticleIntensity = 1f;
        loaded.shadeSkinPreviewSmoothing = true;
        loaded.shadeSpriteSmoothing = false;
        ModConfig.Save();
    }

    [Fact]
    public void LoadRepairsOutOfRangeVisualSettings()
    {
        var cfg = ModConfig.Instance;
        // A hand-edited config.json, or one written by an older build, must not leave the Shade on
        // a blank sorting layer or drive the emitter past its tuned ceiling.
        cfg.shadeSortingLayer = "   ";
        cfg.shadeShadowParticleIntensity = 99f;
        ModConfig.Save();

        var loaded = ModConfig.Load();
        Assert.Equal(ModConfig.DefaultShadeSortingLayer, loaded.shadeSortingLayer);
        Assert.Equal(ModConfig.MaxShadowParticleIntensity, loaded.shadeShadowParticleIntensity, 3);

        loaded.shadeShadowParticleIntensity = 1f;
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
