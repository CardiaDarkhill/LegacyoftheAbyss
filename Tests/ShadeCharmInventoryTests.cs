#nullable enable

using System;
using System.Linq;
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
        Assert.Contains("resists", firstAttempt, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(inventory.OvercharmAttemptThreshold - 1, inventory.RemainingOvercharmAttempts);

        Assert.False(inventory.TryEquip(ShadeCharmId.SoulCatcher, out _));
        Assert.Equal(inventory.OvercharmAttemptThreshold - 2, inventory.RemainingOvercharmAttempts);

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

    [Fact]
    public void VoidHeartAutomaticallyEquippedOutsideDebug()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.VoidHeart);

        var equipped = inventory.GetEquipped().ToArray();
        Assert.Single(equipped);
        Assert.Equal(ShadeCharmId.VoidHeart, equipped[0]);
    }

    [Fact]
    public void VoidHeartCannotBeUnequippedOutsideDebug()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.VoidHeart);

        Assert.False(inventory.TryUnequip(ShadeCharmId.VoidHeart, out var message));
        Assert.Contains("Void Heart", message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(ShadeCharmId.VoidHeart, inventory.GetEquipped());
    }

    [Fact]
    public void VoidHeartCanCoexistWithKingsoul()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.Kingsoul);
        inventory.GrantCharm(ShadeCharmId.VoidHeart);

        inventory.NotchCapacity = 10;
        Assert.True(inventory.TryEquip(ShadeCharmId.Kingsoul, out _));

        var equipped = inventory.GetEquipped().ToArray();
        Assert.Contains(ShadeCharmId.Kingsoul, equipped);
        Assert.Equal(ShadeCharmId.VoidHeart, equipped[0]);
    }

    [Fact]
    public void VoidHeartCanBeUnequippedDuringDebugMode()
    {
        ShadeRuntime.Clear();
        var inventory = ShadeRuntime.Charms;
        inventory.ResetLoadout();
        inventory.GrantCharm(ShadeCharmId.VoidHeart);

        bool debugEnabled = ShadeRuntime.ToggleDebugUnlockAllCharms();

        try
        {
            Assert.True(debugEnabled);
            Assert.True(inventory.TryUnequip(ShadeCharmId.VoidHeart, out _));
        }
        finally
        {
            if (ShadeRuntime.IsDebugCharmModeActive())
            {
                ShadeRuntime.ToggleDebugUnlockAllCharms();
            }

            ShadeRuntime.Clear();
        }
    }
}
