#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Represents the portions of the shade's runtime state that need to persist between scene loads.
    /// This class intentionally avoids any references to Unity components so that it can be tested in isolation.
    /// </summary>
    internal sealed class ShadePersistentState
    {
        private const int MaxSpellProgress = 6;

        public int CurrentHP { get; private set; } = -1;
        public int MaxHP { get; private set; } = -1;
        public int Soul { get; private set; } = -1;
        public bool CanTakeDamage { get; private set; } = true;
        public int SpellProgress { get; private set; }
            = 0;

        private readonly HashSet<int> _discoveredCharmIds = new();
        private readonly Dictionary<int, HashSet<int>> _equippedCharmLoadouts = new();

        public IReadOnlyCollection<int> DiscoveredCharmIds => _discoveredCharmIds;

        public int NotchCapacity { get; private set; }
            = 0;

        public bool HasData => MaxHP > 0;

        public ShadePersistentState Clone()
        {
            var clone = new ShadePersistentState
            {
                CurrentHP = CurrentHP,
                MaxHP = MaxHP,
                Soul = Soul,
                CanTakeDamage = CanTakeDamage,
                SpellProgress = SpellProgress,
                NotchCapacity = NotchCapacity
            };

            clone._discoveredCharmIds.UnionWith(_discoveredCharmIds);
            foreach (var (loadoutId, charms) in _equippedCharmLoadouts)
            {
                clone._equippedCharmLoadouts[loadoutId] = new HashSet<int>(charms);
            }

            return clone;
        }

        public void Capture(int currentHp, int maxHp, int soul, bool? canTakeDamage = null)
        {
            int previousMax = MaxHP;
            int previousHp = CurrentHP;

            int sanitizedMax = Mathf.Max(1, maxHp);
            int sanitizedHp = Mathf.Clamp(currentHp, 0, sanitizedMax);

            bool suspiciousDrop = previousMax > 1 && sanitizedMax <= 1 && sanitizedHp <= 1;
            bool invulnerableDrop = false;
            if (!CanTakeDamage || (canTakeDamage.HasValue && !canTakeDamage.Value))
            {
                invulnerableDrop = previousHp >= 0 && sanitizedHp < previousHp;
            }

            if (suspiciousDrop || invulnerableDrop)
            {
                sanitizedMax = previousMax;
                sanitizedHp = Mathf.Clamp(Mathf.Max(previousHp, currentHp), 0, sanitizedMax);
                if (ModConfig.Instance.logShade)
                {
                    try
                    {
                        string reason = suspiciousDrop
                            ? "suspicious"
                            : "invulnerable";
                        Debug.LogWarning($"[ShadePersistence] Ignored {reason} state capture (hp={currentHp}, max={maxHp}, prevHp={previousHp}, prevMax={previousMax}, invulnerable={!CanTakeDamage}).");
                    }
                    catch
                    {
                    }
                }
            }

            MaxHP = sanitizedMax;
            CurrentHP = Mathf.Clamp(sanitizedHp, 0, MaxHP);
            Soul = Mathf.Max(0, soul);
            if (canTakeDamage.HasValue)
            {
                CanTakeDamage = canTakeDamage.Value;
            }

        }

        public void Reset()
        {
            CurrentHP = -1;
            MaxHP = -1;
            Soul = -1;
            CanTakeDamage = true;
            SpellProgress = 0;
            NotchCapacity = 0;
            _discoveredCharmIds.Clear();
            _equippedCharmLoadouts.Clear();
        }

        internal ShadePersistentStateData ToData()
        {
            var equipped = new Dictionary<int, int[]>();
            foreach (var (loadoutId, charms) in _equippedCharmLoadouts)
            {
                equipped[loadoutId] = charms.ToArray();
            }

            return new ShadePersistentStateData
            {
                CurrentHP = CurrentHP,
                MaxHP = MaxHP,
                Soul = Soul,
                CanTakeDamage = CanTakeDamage,
                SpellProgress = SpellProgress,
                NotchCapacity = NotchCapacity,
                DiscoveredCharmIds = _discoveredCharmIds.ToArray(),
                EquippedCharmLoadouts = equipped
            };
        }

        internal void LoadFromData(ShadePersistentStateData? data)
        {
            Reset();
            if (!data.HasValue)
            {
                return;
            }

            var snapshot = data.Value;
            if (snapshot.MaxHP > 0)
            {
                Capture(snapshot.CurrentHP, snapshot.MaxHP, snapshot.Soul, snapshot.CanTakeDamage);
            }
            else
            {
                Soul = Mathf.Max(0, snapshot.Soul);
                CanTakeDamage = snapshot.CanTakeDamage;
            }

            SetSpellProgress(snapshot.SpellProgress);
            SetNotchCapacity(snapshot.NotchCapacity);

            if (snapshot.DiscoveredCharmIds != null)
            {
                foreach (var charmId in snapshot.DiscoveredCharmIds)
                {
                    if (charmId >= 0)
                    {
                        UnlockCharm(charmId);
                    }
                }
            }

            if (snapshot.EquippedCharmLoadouts != null)
            {
                foreach (var kvp in snapshot.EquippedCharmLoadouts)
                {
                    int loadoutId = kvp.Key;
                    var charms = kvp.Value;
                    if (charms == null)
                    {
                        continue;
                    }

                    foreach (var charmId in charms)
                    {
                        if (charmId < 0)
                        {
                            continue;
                        }

                        UnlockCharm(charmId);
                        EquipCharm(loadoutId, charmId);
                    }
                }
            }
        }

        internal struct ShadePersistentStateData
        {
            public int CurrentHP { get; set; }

            public int MaxHP { get; set; }

            public int Soul { get; set; }

            public bool CanTakeDamage { get; set; }

            public int SpellProgress { get; set; }

            public int NotchCapacity { get; set; }

            public int[]? DiscoveredCharmIds { get; set; }

            public Dictionary<int, int[]>? EquippedCharmLoadouts { get; set; }
        }

        public void ForceMinimumHealth(int minimum)
        {
            if (!HasData)
            {
                return;
            }

            int required = Mathf.Max(0, minimum);
            CurrentHP = Mathf.Clamp(Mathf.Max(CurrentHP, required), 0, MaxHP);
        }

        public void AdvanceSpellProgress()
        {
            SpellProgress = Mathf.Clamp(SpellProgress + 1, 0, MaxSpellProgress);
        }

        public void SetSpellProgress(int progress)
        {
            SpellProgress = Mathf.Clamp(progress, 0, MaxSpellProgress);
        }

        public bool HasDiscoveredCharm(int charmId)
        {
            return _discoveredCharmIds.Contains(charmId);
        }

        public bool UnlockCharm(int charmId)
        {
            if (charmId < 0)
            {
                return false;
            }

            return _discoveredCharmIds.Add(charmId);
        }

        public bool EquipCharm(int loadoutId, int charmId)
        {
            if (!_discoveredCharmIds.Contains(charmId))
            {
                return false;
            }

            if (!_equippedCharmLoadouts.TryGetValue(loadoutId, out var loadout))
            {
                loadout = new HashSet<int>();
                _equippedCharmLoadouts[loadoutId] = loadout;
            }

            return loadout.Add(charmId);
        }

        public bool UnequipCharm(int loadoutId, int charmId)
        {
            if (!_equippedCharmLoadouts.TryGetValue(loadoutId, out var loadout))
            {
                return false;
            }

            bool removed = loadout.Remove(charmId);
            if (removed && loadout.Count == 0)
            {
                _equippedCharmLoadouts.Remove(loadoutId);
            }

            return removed;
        }

        public IReadOnlyCollection<int> GetDiscoveredCharmIdsSnapshot()
        {
            if (_discoveredCharmIds.Count == 0)
            {
                return Array.Empty<int>();
            }

            return _discoveredCharmIds.ToArray();
        }

        public IReadOnlyCollection<int> GetEquippedCharms(int loadoutId)
        {
            if (_equippedCharmLoadouts.TryGetValue(loadoutId, out var loadout))
            {
                return loadout.ToArray();
            }

            return Array.Empty<int>();
        }

        public IReadOnlyDictionary<int, IReadOnlyCollection<int>> GetEquippedCharmLoadouts()
        {
            return _equippedCharmLoadouts.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyCollection<int>)kvp.Value.ToArray());
        }

        public void ClearLoadout(int loadoutId)
        {
            _equippedCharmLoadouts.Remove(loadoutId);
        }

        public bool SetNotchCapacity(int capacity)
        {
            int sanitized = Mathf.Max(0, capacity);
            if (sanitized == NotchCapacity)
            {
                return false;
            }

            NotchCapacity = sanitized;
            return true;
        }
    }
}
