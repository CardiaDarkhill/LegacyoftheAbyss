#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Tracks a small collection of shade save slots so that future systems can attach shade-specific
    /// progress to the game's existing save files. The repository is intentionally simple today but
    /// provides cloning semantics to avoid accidental sharing of mutable state.
    /// </summary>
    internal sealed class ShadeSaveSlotRepository
    {
        private readonly Dictionary<int, ShadePersistentState> _slots;

        public ShadeSaveSlotRepository(int maxSlots = 4)
        {
            if (maxSlots <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSlots), "Shade save repository must manage at least one slot.");
            }

            MaxSlots = maxSlots;
            _slots = new Dictionary<int, ShadePersistentState>(MaxSlots);
        }

        public int MaxSlots { get; }

        public IEnumerable<int> KnownSlots => _slots.Keys.OrderBy(k => k);

        public ShadePersistentState GetOrCreateSlot(int slot)
        {
            if (!_slots.TryGetValue(slot, out var existing))
            {
                existing = new ShadePersistentState();
                _slots[slot] = existing;
            }

            return existing;
        }

        public bool TryGetSlot(int slot, out ShadePersistentState state)
        {
            if (_slots.TryGetValue(slot, out var existing))
            {
                state = existing.Clone();
                return true;
            }

            state = new ShadePersistentState();
            return false;
        }

        public void UpdateSlot(int slot, ShadePersistentState data)
        {
            if (slot < 0 || slot >= MaxSlots)
            {
                return;
            }

            _slots[slot] = (data ?? new ShadePersistentState()).Clone();
        }

        public void ClearSlot(int slot)
        {
            _slots.Remove(slot);
        }

        public void ResetAll()
        {
            _slots.Clear();
        }
    }
}
