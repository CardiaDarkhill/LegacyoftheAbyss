#nullable enable

using System;
using System.Linq;
using LegacyoftheAbyss.Shade;
using Xunit;

/// <summary>
/// The ten charms brought across from Knight in Silksong. A charm with no definition, no icon or
/// no placement is invisible in play rather than broken, which is exactly the failure these catch.
/// </summary>
[Collection(ShadeRuntimeCollection.Name)]
public class ShadeNewCharmTests
{
    private static readonly ShadeCharmId[] NewCharms =
    {
        ShadeCharmId.Weaversong,
        ShadeCharmId.DefendersCrest,
        ShadeCharmId.Flukenest,
        ShadeCharmId.SporeShroom,
        ShadeCharmId.ThornsOfAgony,
        ShadeCharmId.GlowingWomb,
        ShadeCharmId.GatheringSwarm,
        ShadeCharmId.Grimmchild,
        ShadeCharmId.DreamWielder,
        ShadeCharmId.Dreamshield,
    };

    [Fact]
    public void EveryCharmIdHasADefinition()
    {
        var inventory = new ShadeCharmInventory();

        foreach (ShadeCharmId id in Enum.GetValues(typeof(ShadeCharmId)))
        {
            var definition = inventory.GetDefinition(id);
            Assert.True(definition != null, $"{id} has no definition.");
        }
    }

    [Fact]
    public void TheNewCharmsAreAllPresent()
    {
        var inventory = new ShadeCharmInventory();

        foreach (var id in NewCharms)
        {
            var definition = inventory.GetDefinition(id);
            Assert.NotNull(definition);
            Assert.Equal(id, definition!.EnumId);
            Assert.False(string.IsNullOrWhiteSpace(definition.DisplayName), $"{id} has no display name.");
            Assert.False(string.IsNullOrWhiteSpace(definition.Description), $"{id} has no description.");
        }
    }

    /// <summary>Notch costs follow Hollow Knight's, which is what the descriptions promise.</summary>
    [Theory]
    [InlineData(ShadeCharmId.Weaversong, 2)]
    [InlineData(ShadeCharmId.DefendersCrest, 1)]
    [InlineData(ShadeCharmId.Flukenest, 3)]
    [InlineData(ShadeCharmId.SporeShroom, 1)]
    [InlineData(ShadeCharmId.ThornsOfAgony, 1)]
    [InlineData(ShadeCharmId.GlowingWomb, 2)]
    [InlineData(ShadeCharmId.GatheringSwarm, 1)]
    [InlineData(ShadeCharmId.Grimmchild, 2)]
    [InlineData(ShadeCharmId.DreamWielder, 1)]
    [InlineData(ShadeCharmId.Dreamshield, 3)]
    public void NotchCostsMatchHollowKnight(ShadeCharmId id, int expected)
    {
        var inventory = new ShadeCharmInventory();
        Assert.Equal(expected, inventory.GetDefinition(id)!.NotchCost);
    }

    [Fact]
    public void RosterIsFortyTwoCharms()
    {
        var inventory = new ShadeCharmInventory();
        Assert.Equal(42, inventory.AllCharms.Count);
        Assert.Equal(42, Enum.GetValues(typeof(ShadeCharmId)).Length);
    }

    /// <summary>
    /// Charm ids are persisted as integers in save slots, so the pre-existing charms must keep the
    /// ordinals they already have on disk. The new ones are appended after Void Heart.
    /// </summary>
    [Fact]
    public void ExistingCharmOrdinalsAreUnchanged()
    {
        Assert.Equal(0, (int)ShadeCharmId.WaywardCompass);
        Assert.Equal(9, (int)ShadeCharmId.FragileStrength);
        Assert.Equal(30, (int)ShadeCharmId.Kingsoul);
        Assert.Equal(31, (int)ShadeCharmId.VoidHeart);
        Assert.Equal(32, (int)ShadeCharmId.Weaversong);
        Assert.Equal(41, (int)ShadeCharmId.Dreamshield);
    }

    [Fact]
    public void NewCharmsCanBeGrantedAndEquipped()
    {
        var inventory = new ShadeCharmInventory();
        inventory.NotchCapacity = 20;

        foreach (var id in NewCharms)
        {
            inventory.GrantCharm(id);
            Assert.Contains(id, inventory.GetOwnedCharms());
        }

        // One notch-light charm equips without argument.
        Assert.True(inventory.TryEquip(ShadeCharmId.GatheringSwarm, out _));
        Assert.Contains(ShadeCharmId.GatheringSwarm, inventory.GetEquipped());
    }

    [Fact]
    public void EveryNewCharmIsObtainableSomewhere()
    {
        ShadeCharmPlacementDatabase.EnsureLoaded();
        var placed = ShadeCharmPlacementDatabase.GetAllPlacements()
            .Select(p => p.CharmId)
            .ToHashSet();

        foreach (var id in NewCharms)
        {
            Assert.True(placed.Contains(id), $"{id} has no placement, so it can never be collected.");
        }
    }
}
