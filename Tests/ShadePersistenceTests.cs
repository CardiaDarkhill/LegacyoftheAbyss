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
}
