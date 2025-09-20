using System;

using System.Linq;
using LegacyoftheAbyss.Shade;
using Xunit;

public class ShadePersistentStateTests
{
    [Fact]
    public void CaptureClampsValues()
    {
        var state = new ShadePersistentState();
        state.Capture(-10, -5, -20, false);

        Assert.True(state.HasData);
        Assert.Equal(1, state.MaxHP);
        Assert.Equal(0, state.CurrentHP);
        Assert.Equal(0, state.Soul);
        Assert.False(state.CanTakeDamage);
    }

    [Fact]
    public void ForceMinimumHealthRespectsBounds()
    {
        var state = new ShadePersistentState();
        state.Capture(1, 3, 0, true);
        state.ForceMinimumHealth(4);

        Assert.Equal(3, state.CurrentHP); // clamped to max HP

        state.ForceMinimumHealth(2);
        Assert.Equal(3, state.CurrentHP); // remains at higher value
    }

    [Fact]
    public void SpellProgressClampsAndPersists()
    {
        var state = new ShadePersistentState();
        for (int i = 0; i < 10; i++)
        {
            state.AdvanceSpellProgress();
        }

        Assert.Equal(6, state.SpellProgress);

        state.SetSpellProgress(-5);
        Assert.Equal(0, state.SpellProgress);
    }

    [Fact]
    public void CharmStateDiscoveryEquippingCloneAndReset()
    {
        var state = new ShadePersistentState();

        Assert.False(state.HasDiscoveredCharm(7));
        Assert.True(state.UnlockCharm(7));
        Assert.True(state.HasDiscoveredCharm(7));
        Assert.False(state.UnlockCharm(7));

        Assert.False(state.EquipCharm(0, 5));
        Assert.True(state.EquipCharm(0, 7));
        Assert.Contains(7, state.GetEquippedCharms(0));
        Assert.Empty(state.GetEquippedCharms(1));

        var equippedSnapshot = state.GetEquippedCharms(0);
        if (equippedSnapshot is int[] equippedArray)
        {
            equippedArray[0] = 123;
        }

        Assert.Contains(7, state.GetEquippedCharms(0));

        Assert.True(state.SetNotchCapacity(5));
        Assert.Equal(5, state.NotchCapacity);
        Assert.True(state.SetNotchCapacity(-2));
        Assert.Equal(0, state.NotchCapacity);
        Assert.False(state.SetNotchCapacity(0));
        Assert.True(state.SetNotchCapacity(3));

        var clone = state.Clone();
        Assert.True(clone.HasDiscoveredCharm(7));
        Assert.Contains(7, clone.GetEquippedCharms(0));
        Assert.Equal(3, clone.NotchCapacity);

        clone.UnlockCharm(8);
        clone.EquipCharm(1, 8);
        clone.ClearLoadout(0);
        clone.SetNotchCapacity(6);

        Assert.True(state.HasDiscoveredCharm(7));
        Assert.False(state.HasDiscoveredCharm(8));
        Assert.Contains(7, state.GetEquippedCharms(0));
        Assert.Empty(state.GetEquippedCharms(1));
        Assert.Equal(3, state.NotchCapacity);

        state.Reset();
        Assert.Empty(state.DiscoveredCharmIds);
        Assert.Empty(state.GetEquippedCharms(0));
        Assert.Equal(0, state.NotchCapacity);
    }
}

public class ShadeSaveSlotRepositoryTests
{
    [Fact]
    public void UpdateSlotStoresClone()
    {
        var repository = new ShadeSaveSlotRepository();
        var original = new ShadePersistentState();
        original.Capture(2, 5, 10, false);

        repository.UpdateSlot(1, original);

        original.Capture(1, 3, 1, true);
        Assert.True(repository.TryGetSlot(1, out var stored));
        Assert.NotSame(original, stored);
        Assert.Equal(2, stored.CurrentHP);
        Assert.Equal(5, stored.MaxHP);
        Assert.Equal(10, stored.Soul);
        Assert.False(stored.CanTakeDamage);
        Assert.Equal(0, stored.NotchCapacity);

        original.UnlockCharm(7);
        original.EquipCharm(0, 7);
        original.SetNotchCapacity(4);

        repository.UpdateSlot(2, original);

        original.UnlockCharm(9);
        original.EquipCharm(1, 9);
        original.SetNotchCapacity(2);

        Assert.True(repository.TryGetSlot(2, out var storedWithCharms));
        Assert.NotSame(original, storedWithCharms);
        Assert.True(storedWithCharms.HasDiscoveredCharm(7));
        Assert.Contains(7, storedWithCharms.GetEquippedCharms(0));
        Assert.Equal(4, storedWithCharms.NotchCapacity);
    }

    [Fact]
    public void GetOrCreateSlotReturnsLiveReference()
    {
        var repository = new ShadeSaveSlotRepository();
        var slot = repository.GetOrCreateSlot(0);
        slot.Capture(3, 6, 12, true);

        var sameSlot = repository.GetOrCreateSlot(0);
        Assert.Same(slot, sameSlot);
        Assert.Equal(3, sameSlot.CurrentHP);
    }

    [Fact]
    public void ResetAndClearManageKnownSlots()
    {
        var repository = new ShadeSaveSlotRepository();
        repository.UpdateSlot(0, new ShadePersistentState());
        repository.UpdateSlot(2, new ShadePersistentState());

        Assert.True(repository.KnownSlots.SequenceEqual(new[] { 0, 2 }));

        repository.ClearSlot(2);
        Assert.True(repository.KnownSlots.SequenceEqual(new[] { 0 }));

        repository.ResetAll();
        Assert.Empty(repository.KnownSlots);
    }

    [Fact]
    public void CharmHelpersOperatePerSlot()
    {
        var repository = new ShadeSaveSlotRepository();

        Assert.True(repository.UnlockCharm(1, 12));
        Assert.Contains(12, repository.GetDiscoveredCharms(1));
        Assert.True(repository.SetNotchCapacity(1, 6));
        Assert.Equal(6, repository.GetNotchCapacity(1));

        Assert.True(repository.EquipCharm(1, 0, 12));
        Assert.Contains(12, repository.GetEquippedCharms(1, 0));
        Assert.Empty(repository.GetEquippedCharms(0, 0));

        Assert.False(repository.EquipCharm(1, 0, 99));
        Assert.False(repository.UnequipCharm(0, 0, 12));

        var loadouts = repository.GetEquippedCharmLoadouts(1);
        Assert.Single(loadouts);
        Assert.Equal(12, loadouts[0].Single());

        repository.ClearSlot(1);
        Assert.Empty(repository.GetDiscoveredCharms(1));
        Assert.Equal(0, repository.GetNotchCapacity(1));
        Assert.Empty(repository.GetEquippedCharmLoadouts(1));
    }

    [Fact]
    public void CharmCollectionStatePersistsPerSlot()
    {
        var repository = new ShadeSaveSlotRepository();

        Assert.False(repository.IsCharmCollected(0, ShadeCharmId.FragileStrength));
        Assert.True(repository.MarkCharmCollected(0, ShadeCharmId.FragileStrength));
        Assert.True(repository.IsCharmCollected(0, ShadeCharmId.FragileStrength));

        var snapshot = repository.GetCollectedCharms(0);
        Assert.Contains(ShadeCharmId.FragileStrength, snapshot);

        // Duplicated mark should report false and not add again.
        Assert.False(repository.MarkCharmCollected(0, ShadeCharmId.FragileStrength));

        var copy = repository.GetCollectedCharms(0);
        Assert.NotSame(snapshot, copy);
        Assert.True(snapshot.OrderBy(c => c).SequenceEqual(copy.OrderBy(c => c)));

        Assert.True(repository.ClearCharm(0, ShadeCharmId.FragileStrength));
        Assert.False(repository.IsCharmCollected(0, ShadeCharmId.FragileStrength));
    }

    [Fact]
    public void SetCollectedCharmsReplacesState()
    {
        var repository = new ShadeSaveSlotRepository();
        repository.MarkCharmCollected(0, ShadeCharmId.FragileStrength);

        repository.SetCollectedCharms(0, new[] { ShadeCharmId.MarkOfPride, ShadeCharmId.SoulCatcher });

        var charms = repository.GetCollectedCharms(0).OrderBy(c => c).ToArray();
        Assert.Equal(new[] { ShadeCharmId.MarkOfPride, ShadeCharmId.SoulCatcher }, charms);

        repository.SetCollectedCharms(0, Array.Empty<ShadeCharmId>());
        Assert.Empty(repository.GetCollectedCharms(0));
    }
}
