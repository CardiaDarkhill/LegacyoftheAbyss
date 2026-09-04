#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed partial class ShadeCharmInventory
    {
        private const int DefaultNotchCapacity = 3;
        private const int OvercharmAttemptsRequired = 4;

        private readonly List<ShadeCharmDefinition> _definitions;
        private readonly Dictionary<ShadeCharmId, ShadeCharmDefinition> _definitionMap;
        private readonly HashSet<ShadeCharmId> _owned;
        private readonly HashSet<ShadeCharmId> _equipped;
        private readonly List<ShadeCharmId> _equippedOrder;
        private readonly HashSet<ShadeCharmId> _broken;
        private readonly HashSet<ShadeCharmId> _newlyDiscovered;
        private bool _suppressStateChanged;
        private int _notchCapacity;
        private bool _isOvercharmed;
        private int _overcharmAttemptCounter;
        private float _hivebloodTimer;
        private bool _hivebloodPendingMaskRestore;

        private const float HivebloodRegenDurationSeconds = 10f;

        /// <summary>Blows Baldur Shell absorbs before the shell breaks.</summary>
        public const int BaldurShellMaxCharges = 4;

        /// <summary>
        /// Shell durability. Lives here rather than on the controller because the controller is
        /// rebuilt with the scene and the shell is not supposed to mend itself in a doorway - the
        /// companion, and so its inventory, outlives the room.
        /// </summary>
        private int _baldurShellCharges = BaldurShellMaxCharges;

        public event Action? StateChanged;

        public bool HivebloodMaskRegenerating => _hivebloodPendingMaskRestore;

        public float HivebloodRegenTimer => _hivebloodTimer;

        public float HivebloodRegenDuration => HivebloodRegenDurationSeconds;

        public ShadeCharmInventory()
        {
            _definitions = BuildDefinitions();

            _definitionMap = _definitions
                .Where(def => def.EnumId.HasValue)
                .ToDictionary(def => def.EnumId!.Value);
            _owned = new HashSet<ShadeCharmId>();
            _equipped = new HashSet<ShadeCharmId>();
            _equippedOrder = new List<ShadeCharmId>();
            _broken = new HashSet<ShadeCharmId>();
            _newlyDiscovered = new HashSet<ShadeCharmId>();
            _notchCapacity = DefaultNotchCapacity;
            _isOvercharmed = false;
            _overcharmAttemptCounter = 0;
        }

        public IReadOnlyList<ShadeCharmDefinition> AllCharms => _definitions;

        public int NotchCapacity
        {
            get => _notchCapacity;
            set
            {
                int clamped = Mathf.Clamp(value, 0, ShadePersistentState.MaxNotchCapacity);
                if (_notchCapacity == clamped)
                {
                    return;
                }

                _notchCapacity = clamped;
                TrimToCapacity();
                RecalculateOvercharmed();
                RaiseStateChanged();
            }
        }

        public int UsedNotches
        {
            get
            {
                int total = 0;
                foreach (var id in _equippedOrder)
                {
                    if (_definitionMap.TryGetValue(id, out var definition))
                    {
                        total += definition.NotchCost;
                    }
                }

                return total;
            }
        }

        /// <summary>Blows the shell can still absorb. Zero is broken until the next bench.</summary>
        public int BaldurShellCharges => _baldurShellCharges;

        /// <summary>
        /// Spends one blow. False once the shell is gone, which is how it "just stops working"
        /// - there is no broken-charm notification for it, by design.
        /// </summary>
        public bool TryConsumeBaldurShellCharge()
        {
            if (_baldurShellCharges <= 0)
            {
                return false;
            }

            _baldurShellCharges--;
            RaiseStateChanged();
            return true;
        }

        /// <summary>Mends the shell. Called from the bench, alongside the fragile-charm repairs.</summary>
        public bool RefillBaldurShellCharges()
        {
            if (_baldurShellCharges >= BaldurShellMaxCharges)
            {
                return false;
            }

            _baldurShellCharges = BaldurShellMaxCharges;
            RaiseStateChanged();
            return true;
        }

        public bool IsEquipped(ShadeCharmId id) => _equipped.Contains(id);

        public bool IsOwned(ShadeCharmId id) => _owned.Contains(id);

        public bool IsBroken(ShadeCharmId id) => _broken.Contains(id);

        public bool IsNewlyDiscovered(ShadeCharmId id) => _newlyDiscovered.Contains(id);

        public ShadeCharmDefinition GetDefinition(ShadeCharmId id)
        {
            if (_definitionMap.TryGetValue(id, out var definition))
            {
                return definition;
            }

            throw new KeyNotFoundException($"No charm definition registered for {id}.");
        }

        public bool IsOvercharmed => _isOvercharmed;

        public int OvercharmAttemptThreshold => OvercharmAttemptsRequired;

        public int RemainingOvercharmAttempts => _isOvercharmed
            ? 0
            : Math.Max(0, OvercharmAttemptsRequired - _overcharmAttemptCounter);

        public IReadOnlyCollection<ShadeCharmId> GetEquipped() => _equippedOrder.ToArray();

        public IReadOnlyCollection<ShadeCharmDefinition> GetEquippedDefinitions()
        {
            return _equippedOrder
                .Where(id => _definitionMap.TryGetValue(id, out _))
                .Select(id => _definitionMap[id])
                .ToArray();
        }

        /// <summary>
        /// The charms whose effects are actually running: equipped, minus the broken ones.
        /// <para>
        /// A broken fragile charm stays in the loadout so that repairing it at a bench - the same
        /// place the loadout is edited - restores it rather than making the player rebuild the
        /// set. It keeps its notches for the same reason: freeing them on the break could leave a
        /// repair overcharming a loadout that was legal when it was assembled. So "equipped" is no
        /// longer the same question as "in effect", and anything applying effects wants this one.
        /// </para>
        /// </summary>
        public IReadOnlyCollection<ShadeCharmDefinition> GetActiveDefinitions()
        {
            return _equippedOrder
                .Where(id => !_broken.Contains(id) && _definitionMap.ContainsKey(id))
                .Select(id => _definitionMap[id])
                .ToArray();
        }

        /// <summary>Equipped, unbroken, and therefore actually doing something.</summary>
        public bool IsActive(ShadeCharmId id) => _equipped.Contains(id) && !_broken.Contains(id);

        public IReadOnlyCollection<ShadeCharmId> GetOwnedCharms() => _owned.ToArray();

        public IReadOnlyCollection<ShadeCharmId> GetBrokenCharms() => _broken.ToArray();

        public IReadOnlyCollection<ShadeCharmId> GetNewlyDiscovered() => _newlyDiscovered.ToArray();

        public bool TryEquip(ShadeCharmId id, out string message)
        {
            if (!ShadeRuntime.IsHornetRestingAtBench())
            {
                message = ShadeRuntime.BenchLockedMessage;
                return false;
            }

            if (!_definitionMap.TryGetValue(id, out var definition))
            {
                message = "Charm data missing.";
                return false;
            }

            if (!_owned.Contains(id))
            {
                message = $"{definition.DisplayName} has not been discovered yet.";
                return false;
            }

            if (_broken.Contains(id))
            {
                message = $"{definition.DisplayName} is broken and must be repaired at a bench.";
                return false;
            }

            if (_equipped.Contains(id))
            {
                message = $"{definition.DisplayName} is already equipped.";
                return false;
            }

            int notchCost = definition.NotchCost;
            if (notchCost > 0 && _notchCapacity <= 0)
            {
                message = "Shade lacks any notches to equip this charm.";
                return false;
            }

            int prospectiveNotches = UsedNotches + notchCost;
            bool fits = notchCost <= 0 || prospectiveNotches <= _notchCapacity;

            if (!fits && _isOvercharmed)
            {
                message = "Shade is already overcharmed. Unequip a charm first.";
                return false;
            }

            if (!fits && !_isOvercharmed)
            {
                _overcharmAttemptCounter++;
                if (_overcharmAttemptCounter < OvercharmAttemptsRequired)
                {
                    message = "Not enough notches available.";
                    return false;
                }

                _overcharmAttemptCounter = 0;
            }
            else
            {
                _overcharmAttemptCounter = 0;
            }

            bool wasOvercharmed = _isOvercharmed;
            AddEquippedInternal(id);
            RecalculateOvercharmed();

            if (_isOvercharmed && !wasOvercharmed)
            {
                message = $"{definition.DisplayName} equipped. Shade is overcharmed.";
            }
            else if (_isOvercharmed)
            {
                message = $"{definition.DisplayName} equipped. Shade remains overcharmed.";
            }
            else
            {
                message = $"{definition.DisplayName} equipped.";
            }

            EnsureVoidHeartEquipped();
            RaiseStateChanged();
            return true;
        }

        public bool TryUnequip(ShadeCharmId id, out string message)
        {
            if (!ShadeRuntime.IsHornetRestingAtBench())
            {
                message = ShadeRuntime.BenchLockedMessage;
                return false;
            }

            if (id == ShadeCharmId.VoidHeart && ShouldForceVoidHeartEquipped())
            {
                message = "Void Heart refuses to be unequipped.";
                return false;
            }

            if (!_equipped.Contains(id))
            {
                message = "Charm not currently equipped.";
                return false;
            }

            RemoveEquippedInternal(id);
            if (_definitionMap.TryGetValue(id, out var definition))
            {
                message = $"{definition.DisplayName} removed.";
            }
            else
            {
                message = "Charm removed.";
            }

            _overcharmAttemptCounter = 0;
            RecalculateOvercharmed();
            EnsureVoidHeartEquipped();
            RaiseStateChanged();
            return true;
        }

        public bool TryToggle(ShadeCharmId id, out string message)
        {
            if (IsEquipped(id))
            {
                return TryUnequip(id, out message);
            }

            return TryEquip(id, out message);
        }

        public void ResetLoadout()
        {
            _hivebloodTimer = 0f;
            _hivebloodPendingMaskRestore = false;

            bool changed;

            if (_equipped.Count == 0 && _equippedOrder.Count == 0)
            {
                _overcharmAttemptCounter = 0;
                changed = RecalculateOvercharmed();
            }
            else
            {
                _equipped.Clear();
                _equippedOrder.Clear();
                _isOvercharmed = false;
                _overcharmAttemptCounter = 0;
                changed = true;
            }

            if (EnsureVoidHeartEquipped())
            {
                changed = true;
            }

            if (changed)
            {
                RaiseStateChanged();
            }
        }

        public void GrantCharm(ShadeCharmId id)
        {
            if (!_definitionMap.ContainsKey(id))
            {
                return;
            }

            bool added = _owned.Add(id);
            bool changed = false;

            if (added)
            {
                _newlyDiscovered.Add(id);
                changed = true;
            }

            if (EnsureVoidHeartEquipped())
            {
                changed = true;
            }

            if (changed)
            {
                RaiseStateChanged();
            }
        }

        public void RevokeAllCharms(bool resetNotchCapacity = true)
        {
            bool changed = false;

            if (_owned.Count > 0)
            {
                _owned.Clear();
                changed = true;
            }

            if (_equipped.Count > 0 || _equippedOrder.Count > 0)
            {
                _equipped.Clear();
                _equippedOrder.Clear();
                changed = true;
            }

            if (_broken.Count > 0)
            {
                _broken.Clear();
                changed = true;
            }

            if (_newlyDiscovered.Count > 0)
            {
                _newlyDiscovered.Clear();
                changed = true;
            }

            if (_isOvercharmed)
            {
                _isOvercharmed = false;
                changed = true;
            }

            _overcharmAttemptCounter = 0;

            if (resetNotchCapacity && _notchCapacity != DefaultNotchCapacity)
            {
                _notchCapacity = DefaultNotchCapacity;
                changed = true;
            }

            if (changed)
            {
                RaiseStateChanged();
            }
        }

        /// <summary>
        /// Marks a charm broken without unequipping it. See <see cref="GetActiveDefinitions"/> for
        /// why it stays in the loadout.
        /// </summary>
        public bool BreakCharm(ShadeCharmId id)
        {
            if (!_definitionMap.ContainsKey(id))
            {
                return false;
            }

            bool newlyBroken = _broken.Add(id);
            if (newlyBroken)
            {
                RaiseStateChanged();
            }

            return newlyBroken;
        }

        public bool RepairCharm(ShadeCharmId id)
        {
            if (_broken.Remove(id))
            {
                RaiseStateChanged();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears a charm's "new" marker. Raises the change like every other mutator here: without
        /// it the marker was only written to the slot if the player happened to also equip
        /// something, so it came back on the next launch. Fires at most once per charm.
        /// </summary>
        public bool MarkCharmSeen(ShadeCharmId id)
        {
            if (!_newlyDiscovered.Remove(id))
            {
                return false;
            }

            RaiseStateChanged();
            return true;
        }

        public void LoadState(
            IEnumerable<ShadeCharmId>? owned,
            IEnumerable<ShadeCharmId>? equipped,
            IEnumerable<ShadeCharmId>? broken,
            int notchCapacity,
            IEnumerable<ShadeCharmId>? newlyDiscovered = null)
        {
            _suppressStateChanged = true;
            try
            {
                LoadStateCore(owned, equipped, broken, notchCapacity, newlyDiscovered);
            }
            finally
            {
                _suppressStateChanged = false;
            }

            RaiseStateChanged();
        }

        private void LoadStateCore(
            IEnumerable<ShadeCharmId>? owned,
            IEnumerable<ShadeCharmId>? equipped,
            IEnumerable<ShadeCharmId>? broken,
            int notchCapacity,
            IEnumerable<ShadeCharmId>? newlyDiscovered)
        {
            _owned.Clear();
            _equipped.Clear();
            _equippedOrder.Clear();
            _broken.Clear();
            _newlyDiscovered.Clear();

            if (owned != null)
            {
                foreach (var id in SanitizeIds(owned))
                {
                    _owned.Add(id);
                }
            }

            if (broken != null)
            {
                foreach (var id in SanitizeIds(broken))
                {
                    if (_owned.Contains(id))
                    {
                        _broken.Add(id);
                    }
                }
            }

            if (equipped != null)
            {
                // Broken charms are equipped like any other now, so a save written after one broke
                // restores the loadout the player left rather than the gaps in it.
                foreach (var id in SanitizeIds(equipped))
                {
                    _owned.Add(id);
                    AddEquippedInternal(id);
                }
            }

            if (newlyDiscovered != null)
            {
                foreach (var id in SanitizeIds(newlyDiscovered))
                {
                    if (_owned.Contains(id))
                    {
                        _newlyDiscovered.Add(id);
                    }
                }
            }

            _notchCapacity = Mathf.Clamp(notchCapacity > 0 ? notchCapacity : DefaultNotchCapacity, 0, ShadePersistentState.MaxNotchCapacity);

            // Set before the trim, not merely recomputed after it: TrimToCapacity stands down when
            // this is already true, which is what lets a loadout that was saved overcharmed be
            // restored as it was rather than pruned back to the notch count.
            _isOvercharmed = UsedNotches > _notchCapacity;
            TrimToCapacity();
            RecalculateOvercharmed();
            _overcharmAttemptCounter = 0;

            EnsureVoidHeartEquipped();
        }

        private IEnumerable<ShadeCharmId> SanitizeIds(IEnumerable<ShadeCharmId> source)
        {
            foreach (var id in source)
            {
                if (_definitionMap.ContainsKey(id))
                {
                    yield return id;
                }
            }
        }

        private bool ShouldForceVoidHeartEquipped()
        {
            if (!_definitionMap.ContainsKey(ShadeCharmId.VoidHeart))
            {
                return false;
            }

            if (!_owned.Contains(ShadeCharmId.VoidHeart))
            {
                return false;
            }

            if (_broken.Contains(ShadeCharmId.VoidHeart))
            {
                return false;
            }

            return !ShadeRuntime.IsDebugCharmModeActive();
        }

        private bool EnsureVoidHeartEquipped()
        {
            if (!ShouldForceVoidHeartEquipped())
            {
                return false;
            }

            bool changed = false;

            if (!_equipped.Contains(ShadeCharmId.VoidHeart))
            {
                AddEquippedInternal(ShadeCharmId.VoidHeart);
                changed = true;
            }
            else if (_equippedOrder.Count == 0 || _equippedOrder[0] != ShadeCharmId.VoidHeart)
            {
                _equippedOrder.Remove(ShadeCharmId.VoidHeart);
                _equippedOrder.Insert(0, ShadeCharmId.VoidHeart);
                changed = true;
            }

            if (changed)
            {
                _overcharmAttemptCounter = 0;
                RecalculateOvercharmed();
            }

            return changed;
        }

        private bool AddEquippedInternal(ShadeCharmId id)
        {
            if (!_equipped.Add(id))
            {
                return false;
            }

            if (id == ShadeCharmId.VoidHeart)
            {
                _equippedOrder.Insert(0, id);
            }
            else
            {
                _equippedOrder.Add(id);
            }
            return true;
        }

        private bool RemoveEquippedInternal(ShadeCharmId id)
        {
            bool removed = _equipped.Remove(id);
            if (_equippedOrder.Remove(id))
            {
                removed = true;
            }

            return removed;
        }

        private bool RecalculateOvercharmed()
        {
            bool newValue = UsedNotches > _notchCapacity;
            if (_isOvercharmed != newValue)
            {
                _isOvercharmed = newValue;
                return true;
            }

            return false;
        }

        private void RaiseStateChanged()
        {
            if (_suppressStateChanged)
            {
                return;
            }

            RecalculateOvercharmed();
            StateChanged?.Invoke();
        }

        private void TrimToCapacity()
        {
            if (_isOvercharmed)
            {
                RecalculateOvercharmed();
                return;
            }

            if (UsedNotches <= _notchCapacity)
            {
                RecalculateOvercharmed();
                return;
            }

            var ordered = _equippedOrder
                .Select(id => (_definitionMap.TryGetValue(id, out var def) ? def.NotchCost : 0, id))
                .OrderByDescending(tuple => tuple.Item1)
                .ThenBy(tuple => tuple.id)
                .ToList();

            foreach (var (_, id) in ordered)
            {
                if (RemoveEquippedInternal(id) && UsedNotches <= _notchCapacity)
                {
                    break;
                }
            }

            RecalculateOvercharmed();
        }
    }
}
