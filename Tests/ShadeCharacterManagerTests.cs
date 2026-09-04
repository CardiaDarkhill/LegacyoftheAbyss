#nullable enable

using System.Collections.Generic;
using LegacyoftheAbyss.Shade;
using Xunit;

/// <summary>
/// Character choice is persisted per companion slot, so these touch both the config singleton and
/// the companion registry.
/// </summary>
[Collection(ModConfigCollection.Name)]
public class ShadeCharacterManagerTests
{
    [Fact]
    public void SelectionRoundTripsThroughConfig()
    {
        var cfg = ModConfig.Instance;
        var original = cfg.companionCharacters;
        cfg.companionCharacters = new List<string> { "Shade" };

        try
        {
            Assert.True(ShadeCharacterManager.Select(0, ShadeCharacterId.Knight));
            Assert.Equal(ShadeCharacterId.Knight, ShadeCharacterManager.GetSelected(0).Id);

            var loaded = ModConfig.Load();
            Assert.Equal("Knight", loaded.companionCharacters[0]);
        }
        finally
        {
            ModConfig.Instance.companionCharacters = original;
            ModConfig.Save();
        }
    }

    /// <summary>
    /// Choosing the Knight switches the Shade AI off rather than parking it.
    /// <para>
    /// The regression this guards is not the AI driving a walking body - it never did, and a second
    /// guard on the driver stops that anyway. It is what happened on the way back: the setting
    /// stayed on while the Knight was worn, so the swap button handed the Shade straight to the AI
    /// instead of to the second player, and took Hornet's keyboard with it.
    /// </para>
    /// </summary>
    [Fact]
    public void ChoosingTheKnightSwitchesTheShadeAiOff()
    {
        var cfg = ModConfig.Instance;
        var original = cfg.companionCharacters;
        bool originalAi = cfg.shadeAiEnabled;
        cfg.companionCharacters = new List<string> { "Shade" };
        cfg.shadeAiEnabled = true;

        try
        {
            Assert.True(ShadeCharacterManager.Select(0, ShadeCharacterId.Knight));
            Assert.False(ModConfig.Instance.shadeAiEnabled);

            // And it stays off across the swap back. Turning it on again is the Shade AI screen's
            // job, which is the only place that can say the player meant it.
            Assert.True(ShadeCharacterManager.Select(0, ShadeCharacterId.Shade));
            Assert.False(ModConfig.Instance.shadeAiEnabled);
        }
        finally
        {
            ModConfig.Instance.companionCharacters = original;
            ModConfig.Instance.shadeAiEnabled = originalAi;
            ModConfig.Save();
        }
    }

    [Fact]
    public void ChoosingTheShadeLeavesTheAiSettingAlone()
    {
        var cfg = ModConfig.Instance;
        var original = cfg.companionCharacters;
        bool originalAi = cfg.shadeAiEnabled;
        cfg.companionCharacters = new List<string> { "Knight" };
        cfg.shadeAiEnabled = true;

        try
        {
            // Only a character the AI cannot drive clears it. Selecting the Shade is not an opinion
            // about whether the player wants an AI holding it.
            Assert.True(ShadeCharacterManager.Select(0, ShadeCharacterId.Shade));
            Assert.True(ModConfig.Instance.shadeAiEnabled);
        }
        finally
        {
            ModConfig.Instance.companionCharacters = original;
            ModConfig.Instance.shadeAiEnabled = originalAi;
            ModConfig.Save();
        }
    }

    [Fact]
    public void SelectingTheCurrentCharacterReportsNoChange()
    {
        var cfg = ModConfig.Instance;
        var original = cfg.companionCharacters;
        cfg.companionCharacters = new List<string> { "Knight" };

        try
        {
            Assert.False(ShadeCharacterManager.Select(0, ShadeCharacterId.Knight));
            Assert.True(ShadeCharacterManager.Select(0, ShadeCharacterId.Shade));
        }
        finally
        {
            ModConfig.Instance.companionCharacters = original;
            ModConfig.Save();
        }
    }

    [Fact]
    public void SlotsBeyondTheListDefaultToTheShade()
    {
        var cfg = ModConfig.Instance;
        var original = cfg.companionCharacters;
        cfg.companionCharacters = new List<string> { "Knight" };

        try
        {
            Assert.Equal(ShadeCharacterId.Knight, ShadeCharacterManager.GetSelected(0).Id);
            Assert.Equal(ShadeCharacterId.Shade, ShadeCharacterManager.GetSelected(3).Id);
        }
        finally
        {
            ModConfig.Instance.companionCharacters = original;
            ModConfig.Save();
        }
    }

    /// <summary>
    /// Writing a far slot must pad with the default rather than append, or every companion between
    /// silently inherits the wrong character.
    /// </summary>
    [Fact]
    public void WritingAFarSlotPadsRatherThanShifting()
    {
        var cfg = ModConfig.Instance;
        var original = cfg.companionCharacters;
        cfg.companionCharacters = new List<string> { "Shade" };

        try
        {
            ShadeCharacterManager.Select(2, ShadeCharacterId.Knight);

            var list = ModConfig.Instance.companionCharacters;
            Assert.Equal(3, list.Count);
            Assert.Equal("Shade", list[0]);
            Assert.Equal("Shade", list[1]);
            Assert.Equal("Knight", list[2]);
        }
        finally
        {
            ModConfig.Instance.companionCharacters = original;
            ModConfig.Save();
        }
    }

    [Fact]
    public void ApplyConfigToRegistryPushesEveryPersistedChoice()
    {
        var cfg = ModConfig.Instance;
        var original = cfg.companionCharacters;
        ShadeCompanionRegistry.Clear();
        var second = ShadeCompanionRegistry.CreateNext();
        cfg.companionCharacters = new List<string> { "Knight", "Shade" };

        try
        {
            ShadeCharacterManager.ApplyConfigToRegistry();

            Assert.Equal(ShadeCharacterId.Knight, ShadeCompanionRegistry.Primary.Character);
            Assert.Equal(ShadeCharacterId.Shade, second.Character);
        }
        finally
        {
            ModConfig.Instance.companionCharacters = original;
            ModConfig.Save();
            ShadeCompanionRegistry.Primary.Character = ShadeCharacterId.Shade;
            ShadeCompanionRegistry.Clear();
        }
    }
}
