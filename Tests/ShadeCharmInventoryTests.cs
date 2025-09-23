#nullable enable

using System;
using LegacyoftheAbyss.Shade;
using Xunit;

public class ShadeCharmInventoryTests
{
    [Fact]
    public void TryEquipRequiresOvercharmAttemptsBeforeExceedingCapacity()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.ShamanStone);
        inventory.GrantCharm(ShadeCharmId.SoulCatcher);

        Assert.True(inventory.TryEquip(ShadeCharmId.ShamanStone, out _));
        Assert.Equal(3, inventory.UsedNotches);
        Assert.False(inventory.IsOvercharmed);
        Assert.Equal(inventory.OvercharmAttemptThreshold, inventory.RemainingOvercharmAttempts);

        Assert.False(inventory.TryEquip(ShadeCharmId.SoulCatcher, out var firstAttempt));
        Assert.Contains("Not enough notches", firstAttempt);
        Assert.Equal(inventory.OvercharmAttemptThreshold - 1, inventory.RemainingOvercharmAttempts);

        Assert.False(inventory.TryEquip(ShadeCharmId.SoulCatcher, out _));
        Assert.Equal(1, inventory.RemainingOvercharmAttempts);

        Assert.True(inventory.TryEquip(ShadeCharmId.SoulCatcher, out var successMessage));
        Assert.True(inventory.IsOvercharmed);
        Assert.Equal(0, inventory.RemainingOvercharmAttempts);
        Assert.Equal(5, inventory.UsedNotches);
        Assert.Contains("overcharm", successMessage, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UnequipDropsOvercharmStatusWhenCapacityRespected()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.ShamanStone);
        inventory.GrantCharm(ShadeCharmId.SoulCatcher);

        inventory.TryEquip(ShadeCharmId.ShamanStone, out _);
        inventory.TryEquip(ShadeCharmId.SoulCatcher, out _);
        inventory.TryEquip(ShadeCharmId.SoulCatcher, out _);
        inventory.TryEquip(ShadeCharmId.SoulCatcher, out _);

        Assert.True(inventory.IsOvercharmed);
        Assert.True(inventory.TryUnequip(ShadeCharmId.SoulCatcher, out _));
        Assert.False(inventory.IsOvercharmed);
        Assert.Equal(inventory.OvercharmAttemptThreshold, inventory.RemainingOvercharmAttempts);
    }

    [Fact]
    public void CannotEquipWhenNoNotchesRemain()
    {
        var inventory = new ShadeCharmInventory();
        inventory.NotchCapacity = 0;
        inventory.GrantCharm(ShadeCharmId.WaywardCompass);

        Assert.False(inventory.TryEquip(ShadeCharmId.WaywardCompass, out var message));
        Assert.Contains("notch", message, StringComparison.OrdinalIgnoreCase);
        Assert.False(inventory.IsEquipped(ShadeCharmId.WaywardCompass));
        Assert.Equal(0, inventory.UsedNotches);
    }
}
