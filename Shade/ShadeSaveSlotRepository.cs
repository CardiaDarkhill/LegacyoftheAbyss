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

        private static readonly IReadOnlyCollection<int> s_emptyCharmList = Array.Empty<int>();

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

<<<<<<< ours
        public IReadOnlyCollection<int> GetDiscoveredCharms(int slot)
        {
            if (_slots.TryGetValue(slot, out var state))
            {
                return state.GetDiscoveredCharmIdsSnapshot();
            }

            return s_emptyCharmList;
        }

        public IReadOnlyCollection<int> GetEquippedCharms(int slot, int loadoutId)
        {
            if (_slots.TryGetValue(slot, out var state))
            {
                return state.GetEquippedCharms(loadoutId);
            }

            return s_emptyCharmList;
        }

        public IReadOnlyDictionary<int, IReadOnlyCollection<int>> GetEquippedCharmLoadouts(int slot)
        {
            if (_slots.TryGetValue(slot, out var state))
            {
                return state.GetEquippedCharmLoadouts();
            }

            return new Dictionary<int, IReadOnlyCollection<int>>();
        }

        public bool UnlockCharm(int slot, int charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            return GetOrCreateSlot(slot).UnlockCharm(charmId);
        }

        public bool EquipCharm(int slot, int loadoutId, int charmId)
=======
        public bool MarkCharmCollected(int slot, ShadeCharmId charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            var record = GetOrCreateRecord(slot);
            return record.CollectedCharms.Add(charmId);
        }

        public bool IsCharmCollected(int slot, ShadeCharmId charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            return _slots.TryGetValue(slot, out var record) && record.CollectedCharms.Contains(charmId);
        }

        public IReadOnlyCollection<ShadeCharmId> GetCollectedCharms(int slot)
        {
            if (!IsValidSlot(slot))
            {
                return Array.Empty<ShadeCharmId>();
            }

            if (_slots.TryGetValue(slot, out var record))
            {
                return record.CollectedCharms.ToArray();
            }

            return Array.Empty<ShadeCharmId>();
        }

        public bool ClearCharm(int slot, ShadeCharmId charmId)
>>>>>>> theirs
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

<<<<<<< ours
            return GetOrCreateSlot(slot).EquipCharm(loadoutId, charmId);
        }

        public bool UnequipCharm(int slot, int loadoutId, int charmId)
        {
            if (!_slots.TryGetValue(slot, out var state))
            {
                return false;
            }

            return state.UnequipCharm(loadoutId, charmId);
        }

        public int GetNotchCapacity(int slot)
        {
            if (_slots.TryGetValue(slot, out var state))
            {
                return state.NotchCapacity;
            }

            return 0;
        }

        public bool SetNotchCapacity(int slot, int capacity)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            return GetOrCreateSlot(slot).SetNotchCapacity(capacity);
        }

        private bool IsValidSlot(int slot)
        {
            return slot >= 0 && slot < MaxSlots;
        }
=======
            if (_slots.TryGetValue(slot, out var record))
            {
                return record.CollectedCharms.Remove(charmId);
            }

            return false;
        }

        public void SetCollectedCharms(int slot, IEnumerable<ShadeCharmId> charms)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            var record = GetOrCreateRecord(slot);
            record.CollectedCharms.Clear();

            if (charms == null)
            {
                return;
            }

            foreach (var charm in charms)
            {
                record.CollectedCharms.Add(charm);
            }
        }

        private ShadeSaveSlotRecord GetOrCreateRecord(int slot)
        {
            if (!_slots.TryGetValue(slot, out var record))
            {
                record = new ShadeSaveSlotRecord();
                _slots[slot] = record;
            }

            return record;
        }

        private bool IsValidSlot(int slot) => slot >= 0 && slot < MaxSlots;
>>>>>>> theirs
    }
}
