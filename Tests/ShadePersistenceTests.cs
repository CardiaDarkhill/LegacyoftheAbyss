using System;
using System.Collections.Generic;
using System.IO;
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
    private static string CreateTempStorageRoot()
    {
        string root = Path.Combine(Path.GetTempPath(), "LegacyAbyssRepoTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch
        {
        }
    }

    private static void WithRepository(Action<ShadeSaveSlotRepository> action)
    {
        string root = CreateTempStorageRoot();
        try
        {
            var repository = new ShadeSaveSlotRepository(storageRoot: root);
            try
            {
                action(repository);
            }
            finally
            {
                repository.ResetAll();
            }
        }
        finally
        {
            TryDeleteDirectory(root);
        }
    }

    [Fact]
    public void UpdateSlotStoresClone()
    {
        WithRepository(repository =>
        {
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
        });
    }

    [Fact]
    public void GetOrCreateSlotReturnsLiveReference()
    {
        WithRepository(repository =>
        {
            var slot = repository.GetOrCreateSlot(0);
            slot.Capture(3, 6, 12, true);

            var sameSlot = repository.GetOrCreateSlot(0);
            Assert.Same(slot, sameSlot);
            Assert.Equal(3, sameSlot.CurrentHP);
        });
    }

    [Fact]
    public void ResetAndClearManageKnownSlots()
    {
        WithRepository(repository =>
        {
            repository.UpdateSlot(0, new ShadePersistentState());
            repository.UpdateSlot(2, new ShadePersistentState());

            Assert.True(repository.KnownSlots.SequenceEqual(new[] { 0, 2 }));

            repository.ClearSlot(2);
            Assert.True(repository.KnownSlots.SequenceEqual(new[] { 0 }));

            repository.ResetAll();
            Assert.Empty(repository.KnownSlots);
        });
    }

    [Fact]
    public void CharmHelpersOperatePerSlot()
    {
        WithRepository(repository =>
        {
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
        });
    }

    [Fact]
    public void CharmCollectionStatePersistsPerSlot()
    {
        WithRepository(repository =>
        {
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
        });
    }

    [Fact]
    public void SetCollectedCharmsReplacesState()
    {
        WithRepository(repository =>
        {
            repository.MarkCharmCollected(0, ShadeCharmId.FragileStrength);

            repository.SetCollectedCharms(0, new[] { ShadeCharmId.MarkOfPride, ShadeCharmId.SoulCatcher });

            var charms = repository.GetCollectedCharms(0).OrderBy(c => c).ToArray();
            Assert.Equal(new[] { ShadeCharmId.MarkOfPride, ShadeCharmId.SoulCatcher }, charms);

            repository.SetCollectedCharms(0, Array.Empty<ShadeCharmId>());
            Assert.Empty(repository.GetCollectedCharms(0));
        });
    }

    [Fact]
    public void RepositoryPersistsDataAcrossInstances()
    {
        string root = CreateTempStorageRoot();
        try
        {
            var state = new ShadePersistentState();
            state.Capture(4, 8, 20, true);
            state.SetNotchCapacity(5);
            state.UnlockCharm((int)ShadeCharmId.ShamanStone);
            state.EquipCharm(0, (int)ShadeCharmId.ShamanStone);

            var snapshot = new ShadeDebugCharmSnapshot(
                new[] { ShadeCharmId.WaywardCompass, ShadeCharmId.ShamanStone },
                new[] { ShadeCharmId.WaywardCompass },
                new[] { ShadeCharmId.FragileHeart },
                new[] { ShadeCharmId.QuickSlash },
                12);

            var first = new ShadeSaveSlotRepository(storageRoot: root);
            first.UpdateSlot(1, state);
            first.SetCollectedCharms(1, new[] { ShadeCharmId.WaywardCompass, ShadeCharmId.ShamanStone });
            first.SetBrokenCharms(1, new[] { ShadeCharmId.FragileHeart });
            first.SetNewlyDiscoveredCharms(1, new[] { ShadeCharmId.QuickSlash });
            first.SetDebugUnlockState(1, true, snapshot);

            var second = new ShadeSaveSlotRepository(storageRoot: root);
            try
            {
                Assert.True(second.TryGetSlot(1, out var loaded));
                Assert.Equal(4, loaded.CurrentHP);
                Assert.Equal(8, loaded.MaxHP);
                Assert.Equal(20, loaded.Soul);
                Assert.True(loaded.CanTakeDamage);
                Assert.True(loaded.HasDiscoveredCharm((int)ShadeCharmId.ShamanStone));
                Assert.Contains((int)ShadeCharmId.ShamanStone, loaded.GetEquippedCharms(0));
                Assert.Equal(5, loaded.NotchCapacity);

                Assert.True(second.IsCharmCollected(1, ShadeCharmId.WaywardCompass));
                Assert.Contains(ShadeCharmId.FragileHeart, second.GetBrokenCharms(1));
                Assert.Contains(ShadeCharmId.QuickSlash, second.GetNewlyDiscoveredCharms(1));

                Assert.True(second.IsDebugUnlockActive(1));
                var persistedSnapshot = second.GetDebugUnlockSnapshot(1);
                Assert.True(persistedSnapshot.HasValue);
                Assert.Contains(ShadeCharmId.WaywardCompass, persistedSnapshot.Value.Owned);
                Assert.Contains(ShadeCharmId.WaywardCompass, persistedSnapshot.Value.Equipped);
                Assert.Contains(ShadeCharmId.FragileHeart, persistedSnapshot.Value.Broken);
                Assert.Contains(ShadeCharmId.QuickSlash, persistedSnapshot.Value.NewlyDiscovered);
                Assert.Equal(12, persistedSnapshot.Value.NotchCapacity);
            }
            finally
            {
                second.ResetAll();
            }
        }
        finally
        {
            TryDeleteDirectory(root);
        }
    }
}

public class ShadeRuntimeDebugTests
{
    [Fact]
    public void ToggleDebugUnlockAllCharmsTemporarilyOverridesState()
    {
        ShadeRuntime.SaveSlots.ResetAll();
        ShadeRuntime.Clear();

        try
        {
            var inventory = ShadeRuntime.Charms;
            inventory.RevokeAllCharms();
            inventory.NotchCapacity = 6;

            inventory.GrantCharm(ShadeCharmId.WaywardCompass);
            inventory.GrantCharm(ShadeCharmId.Sprintmaster);
            Assert.True(inventory.TryEquip(ShadeCharmId.WaywardCompass, out _));

            var baselineOwned = inventory.GetOwnedCharms().ToHashSet();
            var baselineEquipped = inventory.GetEquipped().ToHashSet();
            var baselineBroken = inventory.GetBrokenCharms().ToHashSet();
            var baselineNew = inventory.GetNewlyDiscovered().ToHashSet();
            int baselineCapacity = inventory.NotchCapacity;

            var slotOwned = ShadeRuntime.SaveSlots.GetCollectedCharms(0).ToHashSet();
            var slotEquipped = ShadeRuntime.SaveSlots.GetEquippedCharms(0, 0).ToHashSet();

            Assert.True(ShadeRuntime.ToggleDebugUnlockAllCharms());
            Assert.Equal(20, inventory.NotchCapacity);
            Assert.True(inventory.IsOwned(ShadeCharmId.QuickSlash));
            Assert.True(inventory.IsEquipped(ShadeCharmId.WaywardCompass));

            Assert.True(slotOwned.SetEquals(ShadeRuntime.SaveSlots.GetCollectedCharms(0).ToHashSet()));
            Assert.True(slotEquipped.SetEquals(ShadeRuntime.SaveSlots.GetEquippedCharms(0, 0).ToHashSet()));

            Assert.True(inventory.TryEquip(ShadeCharmId.QuickSlash, out _));
            Assert.True(inventory.IsEquipped(ShadeCharmId.QuickSlash));
            Assert.True(slotEquipped.SetEquals(ShadeRuntime.SaveSlots.GetEquippedCharms(0, 0).ToHashSet()));

            Assert.True(ShadeRuntime.SaveSlots.IsDebugUnlockActive(0));
            Assert.True(ShadeRuntime.SaveSlots.GetDebugUnlockSnapshot(0).HasValue);

            Assert.False(ShadeRuntime.ToggleDebugUnlockAllCharms());

            Assert.Equal(baselineCapacity, inventory.NotchCapacity);
            Assert.True(baselineOwned.SetEquals(inventory.GetOwnedCharms()));
            Assert.True(baselineEquipped.SetEquals(inventory.GetEquipped()));
            Assert.True(baselineBroken.SetEquals(inventory.GetBrokenCharms()));
            Assert.True(baselineNew.SetEquals(inventory.GetNewlyDiscovered()));
            Assert.False(inventory.IsOwned(ShadeCharmId.QuickSlash));
            Assert.False(inventory.IsEquipped(ShadeCharmId.QuickSlash));

            Assert.True(slotOwned.SetEquals(ShadeRuntime.SaveSlots.GetCollectedCharms(0).ToHashSet()));
            Assert.True(slotEquipped.SetEquals(ShadeRuntime.SaveSlots.GetEquippedCharms(0, 0).ToHashSet()));
            Assert.False(ShadeRuntime.SaveSlots.IsDebugUnlockActive(0));
            Assert.False(ShadeRuntime.SaveSlots.GetDebugUnlockSnapshot(0).HasValue);
        }
        finally
        {
            ShadeRuntime.SaveSlots.ResetAll();
            ShadeRuntime.Clear();
        }
    }
}
