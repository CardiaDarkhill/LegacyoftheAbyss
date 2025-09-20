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
        private sealed class ShadeSaveSlotRecord
        {
            public ShadeSaveSlotRecord()
                : this(new ShadePersistentState(), new HashSet<ShadeCharmId>(), new HashSet<ShadeCharmId>())
            {
            }

            private ShadeSaveSlotRecord(ShadePersistentState state, HashSet<ShadeCharmId> collected, HashSet<ShadeCharmId> broken)
            {
                State = state;
                CollectedCharms = collected;
                BrokenCharms = broken;
            }

            public ShadePersistentState State { get; private set; }

            public HashSet<ShadeCharmId> CollectedCharms { get; }

            public HashSet<ShadeCharmId> BrokenCharms { get; }

            public ShadeSaveSlotRecord Clone()
            {
                return new ShadeSaveSlotRecord(State.Clone(), new HashSet<ShadeCharmId>(CollectedCharms), new HashSet<ShadeCharmId>(BrokenCharms));
            }

            public void ReplaceState(ShadePersistentState state)
            {
                State = state;
            }
        }

        private readonly Dictionary<int, ShadeSaveSlotRecord> _slots;
        private static readonly IReadOnlyCollection<int> s_emptyCharmList = Array.Empty<int>();
        private static readonly IReadOnlyDictionary<int, IReadOnlyCollection<int>> s_emptyLoadoutMap =
            new Dictionary<int, IReadOnlyCollection<int>>();

        public ShadeSaveSlotRepository(int maxSlots = 4)
        {
            if (maxSlots <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSlots), "Shade save repository must manage at least one slot.");
            }

            MaxSlots = maxSlots;
            _slots = new Dictionary<int, ShadeSaveSlotRecord>(MaxSlots);
        }

        public int MaxSlots { get; }

        public IEnumerable<int> KnownSlots => _slots.Keys.OrderBy(k => k);

        public ShadePersistentState GetOrCreateSlot(int slot)
        {
            if (!IsValidSlot(slot))
            {
                return new ShadePersistentState();
            }

            return GetOrCreateRecord(slot).State;
        }

        public bool TryGetSlot(int slot, out ShadePersistentState state)
        {
            if (!IsValidSlot(slot))
            {
                state = new ShadePersistentState();
                return false;
            }

            if (_slots.TryGetValue(slot, out var record))
            {
                state = record.State.Clone();
                return true;
            }

            state = new ShadePersistentState();
            return false;
        }

        public void UpdateSlot(int slot, ShadePersistentState data)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            var record = GetOrCreateRecord(slot);
            var clone = (data ?? new ShadePersistentState()).Clone();
            record.ReplaceState(clone);
        }

        public void ClearSlot(int slot)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            _slots.Remove(slot);
        }

        public void ResetAll()
        {
            _slots.Clear();
        }

        public IReadOnlyCollection<int> GetDiscoveredCharms(int slot)
        {
            if (_slots.TryGetValue(slot, out var record))
            {
                var discovered = record.State.GetDiscoveredCharmIdsSnapshot();
                if (record.CollectedCharms.Count == 0)
                {
                    return discovered;
                }

                var buffer = new HashSet<int>(discovered ?? Array.Empty<int>());
                foreach (var charm in record.CollectedCharms)
                {
                    buffer.Add((int)charm);
                }

                return buffer.ToArray();
            }

            return s_emptyCharmList;
        }

        public IReadOnlyCollection<int> GetEquippedCharms(int slot, int loadoutId)
        {
            if (_slots.TryGetValue(slot, out var record))
            {
                return record.State.GetEquippedCharms(loadoutId);
            }

            return s_emptyCharmList;
        }

        public IReadOnlyDictionary<int, IReadOnlyCollection<int>> GetEquippedCharmLoadouts(int slot)
        {
            if (_slots.TryGetValue(slot, out var record))
            {
                return record.State.GetEquippedCharmLoadouts();
            }

            return s_emptyLoadoutMap;
        }

        public bool UnlockCharm(int slot, int charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            return GetOrCreateRecord(slot).State.UnlockCharm(charmId);
        }

        public bool EquipCharm(int slot, int loadoutId, int charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            return GetOrCreateRecord(slot).State.EquipCharm(loadoutId, charmId);
        }

        public bool UnequipCharm(int slot, int loadoutId, int charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            if (_slots.TryGetValue(slot, out var record))
            {
                return record.State.UnequipCharm(loadoutId, charmId);
            }

            return false;
        }

        public int GetNotchCapacity(int slot)
        {
            if (_slots.TryGetValue(slot, out var record))
            {
                return record.State.NotchCapacity;
            }

            return 0;
        }

        public bool SetNotchCapacity(int slot, int capacity)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            return GetOrCreateRecord(slot).State.SetNotchCapacity(capacity);
        }

        public bool MarkCharmCollected(int slot, ShadeCharmId charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            return GetOrCreateRecord(slot).CollectedCharms.Add(charmId);
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

        public IReadOnlyCollection<ShadeCharmId> GetBrokenCharms(int slot)
        {
            if (!IsValidSlot(slot))
            {
                return Array.Empty<ShadeCharmId>();
            }

            if (_slots.TryGetValue(slot, out var record))
            {
                return record.BrokenCharms.ToArray();
            }

            return Array.Empty<ShadeCharmId>();
        }

        public bool ClearCharm(int slot, ShadeCharmId charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

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

        public void SetBrokenCharms(int slot, IEnumerable<ShadeCharmId> charms)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            var record = GetOrCreateRecord(slot);
            record.BrokenCharms.Clear();

            if (charms == null)
            {
                return;
            }

            foreach (var charm in charms)
            {
                record.BrokenCharms.Add(charm);
            }
        }

        public void SetEquippedCharms(int slot, int loadoutId, IEnumerable<int> charmIds)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            var record = GetOrCreateRecord(slot);
            record.State.ClearLoadout(loadoutId);

            if (charmIds == null)
            {
                return;
            }

            foreach (var charmId in charmIds)
            {
                record.State.EquipCharm(loadoutId, charmId);
            }
        }

        private ShadeSaveSlotRecord GetOrCreateRecord(int slot)
        {
            if (!IsValidSlot(slot))
            {
                throw new ArgumentOutOfRangeException(nameof(slot));
            }

            if (!_slots.TryGetValue(slot, out var record))
            {
                record = new ShadeSaveSlotRecord();
                _slots[slot] = record;
            }

            return record;
        }

        private bool IsValidSlot(int slot)
        {
            return slot >= 0 && slot < MaxSlots;
        }
    }
}
