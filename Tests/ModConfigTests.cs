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
        cfg.hornetSilkSkillDamageMultiplier = 0.7f;
        cfg.shadeDamageMultiplier = 0.8f;
        cfg.shadeSpellDamageMultiplier = 1.3f;
        cfg.bindHornetHeal = 4;
        cfg.bindShadeHeal = 1;
        cfg.focusHornetHeal = 2;
        cfg.focusShadeHeal = 3;
        cfg.shadeMaskFraction = 0.7f;
        cfg.shadeFocusAtFullMasks = true;
        cfg.logDamage = true;
        cfg.shadeEnabled = false;
        ModConfig.Save();
        var loaded = ModConfig.Load();
        Assert.Equal(1.5f, loaded.hornetDamageMultiplier, 3);
        Assert.Equal(0.7f, loaded.hornetSilkSkillDamageMultiplier, 3);
        Assert.Equal(0.8f, loaded.shadeDamageMultiplier, 3);
        Assert.Equal(1.3f, loaded.shadeSpellDamageMultiplier, 3);
        Assert.Equal(4, loaded.bindHornetHeal);
        Assert.Equal(1, loaded.bindShadeHeal);
        Assert.Equal(2, loaded.focusHornetHeal);
        Assert.Equal(3, loaded.focusShadeHeal);
        Assert.Equal(0.7f, loaded.shadeMaskFraction, 3);
        Assert.True(loaded.shadeFocusAtFullMasks);
        Assert.True(loaded.logDamage);
        Assert.False(loaded.shadeEnabled);
        loaded.shadeEnabled = true;
        loaded.shadeFocusAtFullMasks = false;
        loaded.shadeMaskFraction = ModConfig.DefaultShadeMaskFraction;
        ModConfig.Save();
    }

    /// <summary>
    /// A hand-edited or older config must not leave the mask setting between the menu's steps, or
    /// outside the range the menu can reach at all - either would strand the stepper on a value it
    /// cannot return to.
    /// </summary>
    [Theory]
    [InlineData(0f, ModConfig.MinShadeMaskFraction)]
    [InlineData(-3f, ModConfig.MinShadeMaskFraction)]
    [InlineData(0.55f, 0.6f)]
    [InlineData(0.54f, 0.5f)]
    [InlineData(9f, 1f)]
    public void LoadSnapsShadeMaskFractionToTheMenusSteps(float stored, float expected)
    {
        var cfg = ModConfig.Instance;
        cfg.shadeMaskFraction = stored;
        ModConfig.Save();

        var loaded = ModConfig.Load();
        Assert.Equal(expected, loaded.shadeMaskFraction, 3);

        loaded.shadeMaskFraction = ModConfig.DefaultShadeMaskFraction;
        ModConfig.Save();
    }

    /// <summary>
    /// The Shade's mask count rounds up and never reaches zero, and the lowest step means one mask
    /// rather than a literal tenth - see <see cref="ModConfig.shadeMaskFraction"/>.
    /// </summary>
    [Theory]
    [InlineData(0.5f, 10, 5)]
    [InlineData(0.5f, 9, 5)]
    [InlineData(0.4f, 10, 4)]
    [InlineData(0.4f, 7, 3)]
    [InlineData(1f, 10, 10)]
    [InlineData(0.1f, 10, 1)]
    [InlineData(0.1f, 40, 1)]
    [InlineData(0.2f, 3, 1)]
    public void ShadeMaskCountRoundsUpAndNeverReachesZero(float fraction, int hornetMasks, int expected)
    {
        var cfg = ModConfig.Instance;
        float original = cfg.shadeMaskFraction;
        cfg.shadeMaskFraction = fraction;
        try
        {
            Assert.Equal(expected, ModConfig.ComputeShadeMaskCount(hornetMasks));
        }
        finally
        {
            cfg.shadeMaskFraction = original;
        }
    }

    [Fact]
    public void ShadeMaskCountIsZeroWhenHornetHasNoMasks()
    {
        Assert.Equal(0, ModConfig.ComputeShadeMaskCount(0));
    }

    /// <summary>
    /// Applying a preset must land on values that preset then recognises as its own, or the
    /// Difficulty screen would show "Custom" the instant it was selected.
    /// </summary>
    [Fact]
    public void EveryDifficultyPresetIdentifiesItselfAfterBeingApplied()
    {
        var cfg = ModConfig.Instance;
        foreach (var preset in DifficultyPreset.All)
        {
            preset.ApplyTo(cfg);
            Assert.Equal(preset.Name, DifficultyPreset.IdentifyName(cfg));
            Assert.Equal(preset.Description, DifficultyPreset.IdentifyDescription(cfg));
        }

        DifficultyPreset.EasyPreset.ApplyTo(cfg);
        ModConfig.Save();
    }

    /// <summary>
    /// The presets have to be distinguishable from each other, or stepping through them would show
    /// the wrong name for values that are genuinely different.
    /// </summary>
    /// <summary>
    /// The rename promise: a run already being played at what used to be Abyss keeps those values
    /// and simply reads as Hard, rather than being quietly retuned to the new Abyss.
    /// </summary>
    [Fact]
    public void TheOldAbyssValuesNowReadAsHard()
    {
        var cfg = new ModConfig
        {
            hornetDamageMultiplier = 0.6f,
            hornetSilkSkillDamageMultiplier = 0.8f,
            shadeDamageMultiplier = 0.6f,
            shadeSpellDamageMultiplier = 0.8f,
            bindHornetHeal = 2,
            bindShadeHeal = 1,
            focusHornetHeal = 0,
            focusShadeHeal = 1,
            shadeMaskFraction = 0.4f,
            shadeFocusAtFullMasks = false
        };

        Assert.Equal(DifficultyPreset.Hard, DifficultyPreset.IdentifyName(cfg));
    }

    /// <summary>Abyss has to actually be the hard one, not merely the last one in the list.</summary>
    [Fact]
    public void AbyssIsHarderThanHard()
    {
        var hard = DifficultyPreset.HardPreset;
        var abyss = DifficultyPreset.AbyssPreset;

        Assert.True(abyss.HornetNeedleDamage < hard.HornetNeedleDamage);
        Assert.True(abyss.ShadeNailDamage < hard.ShadeNailDamage);
        Assert.True(abyss.BindHornetHeal < hard.BindHornetHeal);
        Assert.True(abyss.BindShadeHeal < hard.BindShadeHeal);
        Assert.True(abyss.FocusShadeHeal < hard.FocusShadeHeal);
        Assert.True(abyss.ShadeMaskFraction < hard.ShadeMaskFraction);

        // The lowest step, which ComputeShadeMaskCount reads as "always 1 mask" rather than as a
        // tenth of Hornet's - so this is the floor, not merely a small fraction.
        Assert.Equal(ModConfig.MinShadeMaskFraction, abyss.ShadeMaskFraction);
    }

    [Fact]
    public void DifficultyPresetsAreDistinct()
    {
        var cfg = ModConfig.Instance;
        foreach (var preset in DifficultyPreset.All)
        {
            preset.ApplyTo(cfg);
            foreach (var other in DifficultyPreset.All)
            {
                if (ReferenceEquals(other, preset))
                {
                    continue;
                }

                Assert.False(other.Matches(cfg), $"{other.Name} claims the values of {preset.Name}.");
            }
        }

        DifficultyPreset.EasyPreset.ApplyTo(cfg);
        ModConfig.Save();
    }

    [Fact]
    public void HandTunedValuesReadAsCustom()
    {
        var cfg = ModConfig.Instance;
        DifficultyPreset.NormalPreset.ApplyTo(cfg);
        cfg.shadeSpellDamageMultiplier += 0.1f;

        Assert.Null(DifficultyPreset.Identify(cfg));
        Assert.Equal(DifficultyPreset.Custom, DifficultyPreset.IdentifyName(cfg));
        Assert.Equal(DifficultyPreset.CustomDescription, DifficultyPreset.IdentifyDescription(cfg));

        DifficultyPreset.EasyPreset.ApplyTo(cfg);
        ModConfig.Save();
    }

    /// <summary>Easy is documented as "the defaults", so a fresh config has to already be on it.</summary>
    [Fact]
    public void EasyPresetMatchesTheShippedDefaults()
    {
        Assert.True(DifficultyPreset.EasyPreset.Matches(new ModConfig()));
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

    /// <summary>
    /// Mouse buttons are named the way players name them. Unity's enum counts from zero, so the
    /// generic enum formatter rendered the left button as "Mouse 0" - which is what the charm menu's
    /// equip prompt was telling people to press.
    /// </summary>
    [Theory]
    [InlineData(KeyCode.Mouse0, "LMB")]
    [InlineData(KeyCode.Mouse1, "RMB")]
    [InlineData(KeyCode.Mouse2, "MMB")]
    [InlineData(KeyCode.Mouse3, "Mouse 4")]
    [InlineData(KeyCode.Mouse4, "Mouse 5")]
    public void MouseButtonsAreDescribedTheWayPlayersNameThem(KeyCode key, string expected)
    {
        Assert.Equal(expected, ShadeInput.DescribeBindingOption(ShadeBindingOption.FromKey(key)));
    }

    [Fact]
    public void OrdinaryKeysKeepTheirSpacedOutName()
    {
        Assert.Equal("Left Shift", ShadeInput.DescribeBindingOption(ShadeBindingOption.FromKey(KeyCode.LeftShift)));
        Assert.Equal("J", ShadeInput.DescribeBindingOption(ShadeBindingOption.FromKey(KeyCode.J)));
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
