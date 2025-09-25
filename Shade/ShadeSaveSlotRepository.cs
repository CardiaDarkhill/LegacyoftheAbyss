#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Tracks a small collection of shade save slots so that future systems can attach shade-specific
    /// progress to the game's existing save files. The repository is intentionally simple today but
    /// now persists each slot to disk so the shade's progression follows the player's save files.
    /// </summary>
    internal sealed class ShadeSaveSlotRepository
    {
        private sealed class ShadeSaveSlotRecord
        {
            public ShadeSaveSlotRecord()
                : this(new ShadePersistentState(), new HashSet<ShadeCharmId>(), new HashSet<ShadeCharmId>(), new HashSet<ShadeCharmId>())
            {
            }

            private ShadeSaveSlotRecord(
                ShadePersistentState state,
                HashSet<ShadeCharmId> collected,
                HashSet<ShadeCharmId> broken,
                HashSet<ShadeCharmId> newlyDiscovered)
            {
                State = state;
                CollectedCharms = collected;
                BrokenCharms = broken;
                NewlyDiscovered = newlyDiscovered;
                DebugOwned = Array.Empty<ShadeCharmId>();
                DebugEquipped = Array.Empty<ShadeCharmId>();
                DebugBroken = Array.Empty<ShadeCharmId>();
                DebugNewlyDiscovered = Array.Empty<ShadeCharmId>();
            }

            public ShadePersistentState State { get; private set; }

            public HashSet<ShadeCharmId> CollectedCharms { get; }

            public HashSet<ShadeCharmId> BrokenCharms { get; }

            public HashSet<ShadeCharmId> NewlyDiscovered { get; }

            public bool DebugUnlockActive { get; set; }

            public ShadeCharmId[] DebugOwned { get; set; }

            public ShadeCharmId[] DebugEquipped { get; set; }

            public ShadeCharmId[] DebugBroken { get; set; }

            public ShadeCharmId[] DebugNewlyDiscovered { get; set; }

            public int DebugNotchCapacity { get; set; }

            public ShadeSaveSlotRecord Clone()
            {
                return new ShadeSaveSlotRecord(
                        State.Clone(),
                        new HashSet<ShadeCharmId>(CollectedCharms),
                        new HashSet<ShadeCharmId>(BrokenCharms),
                        new HashSet<ShadeCharmId>(NewlyDiscovered))
                {
                    DebugUnlockActive = DebugUnlockActive,
                    DebugOwned = DebugOwned?.ToArray() ?? Array.Empty<ShadeCharmId>(),
                    DebugEquipped = DebugEquipped?.ToArray() ?? Array.Empty<ShadeCharmId>(),
                    DebugBroken = DebugBroken?.ToArray() ?? Array.Empty<ShadeCharmId>(),
                    DebugNewlyDiscovered = DebugNewlyDiscovered?.ToArray() ?? Array.Empty<ShadeCharmId>(),
                    DebugNotchCapacity = DebugNotchCapacity
                };
            }

            public void ReplaceState(ShadePersistentState state)
            {
                State = state;
            }
        }

        private sealed class ShadeSaveSlotData
        {
            public ShadePersistentState.ShadePersistentStateData? State { get; set; }

            public ShadeCharmId[]? Collected { get; set; }

            public ShadeCharmId[]? Broken { get; set; }

            public ShadeCharmId[]? NewlyDiscovered { get; set; }

            public bool DebugUnlockActive { get; set; }

            public ShadeCharmId[]? DebugOwned { get; set; }

            public ShadeCharmId[]? DebugEquipped { get; set; }

            public ShadeCharmId[]? DebugBroken { get; set; }

            public ShadeCharmId[]? DebugNewlyDiscovered { get; set; }

            public int DebugNotchCapacity { get; set; }
        }

        private readonly Dictionary<int, ShadeSaveSlotRecord> _slots;
        private readonly string _storageRoot;
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore
        };
        private bool _suppressPersistence;

        private const string SlotFilePrefix = "shade_slot_";
        private const string SlotFileExtension = ".json";

        private static readonly IReadOnlyCollection<int> s_emptyCharmList = Array.Empty<int>();
        private static readonly IReadOnlyDictionary<int, IReadOnlyCollection<int>> s_emptyLoadoutMap =
            new Dictionary<int, IReadOnlyCollection<int>>();

        public ShadeSaveSlotRepository(int maxSlots = 4, string? storageRoot = null)
        {
            if (maxSlots <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSlots), "Shade save repository must manage at least one slot.");
            }

            MaxSlots = maxSlots;
            _slots = new Dictionary<int, ShadeSaveSlotRecord>(MaxSlots);
            _storageRoot = DetermineStorageRoot(storageRoot);

            try
            {
                Directory.CreateDirectory(_storageRoot);
            }
            catch
            {
            }

            LoadAllSlots();
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
            PersistSlot(slot);
        }

        public void ClearSlot(int slot)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            if (_slots.Remove(slot))
            {
                DeleteSlotFile(slot);
            }
        }

        public void ResetAll()
        {
            _slots.Clear();
            for (int i = 0; i < MaxSlots; i++)
            {
                DeleteSlotFile(i);
            }
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

            bool changed = GetOrCreateRecord(slot).State.UnlockCharm(charmId);
            if (changed)
            {
                PersistSlot(slot);
            }

            return changed;
        }

        public bool EquipCharm(int slot, int loadoutId, int charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            bool changed = GetOrCreateRecord(slot).State.EquipCharm(loadoutId, charmId);
            if (changed)
            {
                PersistSlot(slot);
            }

            return changed;
        }

        public bool UnequipCharm(int slot, int loadoutId, int charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            if (_slots.TryGetValue(slot, out var record))
            {
                bool removed = record.State.UnequipCharm(loadoutId, charmId);
                if (removed)
                {
                    PersistSlot(slot);
                }

                return removed;
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

            var record = GetOrCreateRecord(slot);
            bool changed = record.State.SetNotchCapacity(capacity);
            if (changed)
            {
                PersistSlot(slot);
            }

            return changed;
        }

        public bool MarkCharmCollected(int slot, ShadeCharmId charmId)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            var record = GetOrCreateRecord(slot);
            bool added = record.CollectedCharms.Add(charmId);
            if (added)
            {
                PersistSlot(slot);
            }

            return added;
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

        public IReadOnlyCollection<ShadeCharmId> GetNewlyDiscoveredCharms(int slot)
        {
            if (!IsValidSlot(slot))
            {
                return Array.Empty<ShadeCharmId>();
            }

            if (_slots.TryGetValue(slot, out var record))
            {
                return record.NewlyDiscovered.ToArray();
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
                bool removed = record.CollectedCharms.Remove(charmId);
                if (removed)
                {
                    PersistSlot(slot);
                }

                return removed;
            }

            return false;
        }

        public void SetCollectedCharms(int slot, IEnumerable<ShadeCharmId>? charms)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            var record = GetOrCreateRecord(slot);
            record.CollectedCharms.Clear();

            if (charms != null)
            {
                foreach (var charm in charms)
                {
                    record.CollectedCharms.Add(charm);
                }
            }

            var discoveredIds = new HashSet<int>(record.CollectedCharms
                .Select(charm => (int)charm));

            var loadouts = record.State.GetEquippedCharmLoadouts();
            foreach (var loadout in loadouts.Values)
            {
                if (loadout == null)
                {
                    continue;
                }

                foreach (var charmId in loadout)
                {
                    if (charmId >= 0)
                    {
                        discoveredIds.Add(charmId);
                    }
                }
            }

            record.State.SetDiscoveredCharms(discoveredIds);

            PersistSlot(slot);
        }

        public void SetBrokenCharms(int slot, IEnumerable<ShadeCharmId>? charms)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            var record = GetOrCreateRecord(slot);
            record.BrokenCharms.Clear();

            if (charms != null)
            {
                foreach (var charm in charms)
                {
                    record.BrokenCharms.Add(charm);
                }
            }

            PersistSlot(slot);
        }

        public void SetNewlyDiscoveredCharms(int slot, IEnumerable<ShadeCharmId>? charms)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            var record = GetOrCreateRecord(slot);
            record.NewlyDiscovered.Clear();

            if (charms != null)
            {
                foreach (var charm in charms)
                {
                    record.NewlyDiscovered.Add(charm);
                }
            }

            PersistSlot(slot);
        }

        public void SetEquippedCharms(int slot, int loadoutId, IEnumerable<int>? charmIds)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            var record = GetOrCreateRecord(slot);
            record.State.ClearLoadout(loadoutId);

            if (charmIds != null)
            {
                foreach (var charmId in charmIds)
                {
                    if (charmId < 0)
                    {
                        continue;
                    }

                    record.State.UnlockCharm(charmId);
                    record.State.EquipCharm(loadoutId, charmId);
                }
            }

            PersistSlot(slot);
        }

        public void SetDebugUnlockState(int slot, bool active, ShadeDebugCharmSnapshot? snapshot)
        {
            if (!IsValidSlot(slot))
            {
                return;
            }

            var record = GetOrCreateRecord(slot);
            record.DebugUnlockActive = active;

            if (snapshot.HasValue)
            {
                var data = snapshot.Value;
                record.DebugOwned = data.Owned.ToArray();
                record.DebugEquipped = data.Equipped.ToArray();
                record.DebugBroken = data.Broken.ToArray();
                record.DebugNewlyDiscovered = data.NewlyDiscovered.ToArray();
                record.DebugNotchCapacity = data.NotchCapacity;
            }
            else
            {
                record.DebugOwned = Array.Empty<ShadeCharmId>();
                record.DebugEquipped = Array.Empty<ShadeCharmId>();
                record.DebugBroken = Array.Empty<ShadeCharmId>();
                record.DebugNewlyDiscovered = Array.Empty<ShadeCharmId>();
                record.DebugNotchCapacity = 0;
            }

            PersistSlot(slot);
        }

        public bool IsDebugUnlockActive(int slot)
        {
            return _slots.TryGetValue(slot, out var record) && record.DebugUnlockActive;
        }

        public ShadeDebugCharmSnapshot? GetDebugUnlockSnapshot(int slot)
        {
            if (_slots.TryGetValue(slot, out var record) && record.DebugUnlockActive)
            {
                return new ShadeDebugCharmSnapshot(
                    record.DebugOwned ?? Array.Empty<ShadeCharmId>(),
                    record.DebugEquipped ?? Array.Empty<ShadeCharmId>(),
                    record.DebugBroken ?? Array.Empty<ShadeCharmId>(),
                    record.DebugNewlyDiscovered ?? Array.Empty<ShadeCharmId>(),
                    record.DebugNotchCapacity);
            }

            return null;
        }

        private static string DetermineStorageRoot(string? storageRoot)
        {
            if (string.IsNullOrWhiteSpace(storageRoot))
            {
                return ModPaths.Assets;
            }

            try
            {
                return Path.GetFullPath(storageRoot);
            }
            catch
            {
                return ModPaths.Assets;
            }
        }

        private void LoadAllSlots()
        {
            for (int slot = 0; slot < MaxSlots; slot++)
            {
                LoadSlot(slot);
            }
        }

        private void LoadSlot(int slot)
        {
            string path = GetSlotFilePath(slot);
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return;
                }

                var data = JsonConvert.DeserializeObject<ShadeSaveSlotData>(json);
                if (data == null)
                {
                    return;
                }

                var record = GetOrCreateRecord(slot);
                _suppressPersistence = true;
                try
                {
                    var state = new ShadePersistentState();
                    state.LoadFromData(data.State);
                    record.ReplaceState(state);

                    record.CollectedCharms.Clear();
                    if (data.Collected != null)
                    {
                        foreach (var charm in data.Collected)
                        {
                            record.CollectedCharms.Add(charm);
                        }
                    }

                    record.BrokenCharms.Clear();
                    if (data.Broken != null)
                    {
                        foreach (var charm in data.Broken)
                        {
                            record.BrokenCharms.Add(charm);
                        }
                    }

                    record.NewlyDiscovered.Clear();
                    if (data.NewlyDiscovered != null)
                    {
                        foreach (var charm in data.NewlyDiscovered)
                        {
                            record.NewlyDiscovered.Add(charm);
                        }
                    }

                    record.DebugUnlockActive = data.DebugUnlockActive;
                    record.DebugOwned = data.DebugOwned ?? Array.Empty<ShadeCharmId>();
                    record.DebugEquipped = data.DebugEquipped ?? Array.Empty<ShadeCharmId>();
                    record.DebugBroken = data.DebugBroken ?? Array.Empty<ShadeCharmId>();
                    record.DebugNewlyDiscovered = data.DebugNewlyDiscovered ?? Array.Empty<ShadeCharmId>();
                    record.DebugNotchCapacity = data.DebugNotchCapacity;
                }
                finally
                {
                    _suppressPersistence = false;
                }
            }
            catch
            {
            }
        }

        private void PersistSlot(int slot)
        {
            if (_suppressPersistence || !IsValidSlot(slot))
            {
                return;
            }

            if (!_slots.TryGetValue(slot, out var record))
            {
                DeleteSlotFile(slot);
                return;
            }

            if (IsRecordEmpty(record))
            {
                DeleteSlotFile(slot);
                return;
            }

            var data = new ShadeSaveSlotData
            {
                State = record.State.ToData(),
                Collected = record.CollectedCharms.ToArray(),
                Broken = record.BrokenCharms.ToArray(),
                NewlyDiscovered = record.NewlyDiscovered.ToArray(),
                DebugUnlockActive = record.DebugUnlockActive,
                DebugOwned = record.DebugOwned?.ToArray(),
                DebugEquipped = record.DebugEquipped?.ToArray(),
                DebugBroken = record.DebugBroken?.ToArray(),
                DebugNewlyDiscovered = record.DebugNewlyDiscovered?.ToArray(),
                DebugNotchCapacity = record.DebugNotchCapacity
            };

            string path = GetSlotFilePath(slot);
            try
            {
                Directory.CreateDirectory(_storageRoot);
                string json = JsonConvert.SerializeObject(data, _jsonSettings);
                File.WriteAllText(path, json);
            }
            catch
            {
            }
        }

        private bool IsRecordEmpty(ShadeSaveSlotRecord record)
        {
            if (record == null)
            {
                return true;
            }

            bool stateEmpty = !record.State.HasData
                && record.State.NotchCapacity <= 0
                && (record.State.GetDiscoveredCharmIdsSnapshot()?.Count ?? 0) == 0
                && record.State.GetEquippedCharmLoadouts().Count == 0;

            bool collectionsEmpty = record.CollectedCharms.Count == 0
                && record.BrokenCharms.Count == 0
                && record.NewlyDiscovered.Count == 0;

            bool debugEmpty = !record.DebugUnlockActive
                && (record.DebugOwned?.Length ?? 0) == 0
                && (record.DebugEquipped?.Length ?? 0) == 0
                && (record.DebugBroken?.Length ?? 0) == 0
                && (record.DebugNewlyDiscovered?.Length ?? 0) == 0
                && record.DebugNotchCapacity == 0;

            return stateEmpty && collectionsEmpty && debugEmpty;
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

        private string GetSlotFilePath(int slot)
        {
            string fileName = $"{SlotFilePrefix}{slot + 1}{SlotFileExtension}";
            return Path.Combine(_storageRoot, fileName);
        }

        private void DeleteSlotFile(int slot)
        {
            string path = GetSlotFilePath(slot);
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
            }
        }
    }

    internal readonly struct ShadeDebugCharmSnapshot
    {
        public ShadeDebugCharmSnapshot(
            IEnumerable<ShadeCharmId>? owned,
            IEnumerable<ShadeCharmId>? equipped,
            IEnumerable<ShadeCharmId>? broken,
            IEnumerable<ShadeCharmId>? newlyDiscovered,
            int notchCapacity)
        {
            Owned = owned?.ToArray() ?? Array.Empty<ShadeCharmId>();
            Equipped = equipped?.ToArray() ?? Array.Empty<ShadeCharmId>();
            Broken = broken?.ToArray() ?? Array.Empty<ShadeCharmId>();
            NewlyDiscovered = newlyDiscovered?.ToArray() ?? Array.Empty<ShadeCharmId>();
            NotchCapacity = notchCapacity;
        }

        public ShadeCharmId[] Owned { get; }

        public ShadeCharmId[] Equipped { get; }

        public ShadeCharmId[] Broken { get; }

        public ShadeCharmId[] NewlyDiscovered { get; }

        public int NotchCapacity { get; }
    }
}
