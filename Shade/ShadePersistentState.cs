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
        public int BaseMaxHP { get; private set; } = -1;
        public int CurrentLifeblood { get; private set; } = -1;
        public int LifebloodMax { get; private set; } = -1;
        public int Soul { get; private set; } = -1;
        public bool CanTakeDamage { get; private set; } = true;
        public int SpellProgress { get; private set; }
            = 0;

        private readonly HashSet<int> _discoveredCharmIds = new();
        private readonly Dictionary<int, HashSet<int>> _equippedCharmLoadouts = new();

        public IReadOnlyCollection<int> DiscoveredCharmIds => _discoveredCharmIds;

        public int NotchCapacity { get; private set; }
            = 0;

        public bool HasData => MaxHP > 0 || LifebloodMax > 0;

        public ShadePersistentState Clone()
        {
            var clone = new ShadePersistentState
            {
                CurrentHP = CurrentHP,
                MaxHP = MaxHP,
                BaseMaxHP = BaseMaxHP,
                CurrentLifeblood = CurrentLifeblood,
                LifebloodMax = LifebloodMax,
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

        public void Capture(int currentHp, int maxHp, int lifebloodCurrent, int lifebloodMax, int soul, bool? canTakeDamage = null, int? baseMaxHp = null)
        {
            int previousMax = MaxHP;
            int previousHp = CurrentHP;
            int previousLifebloodMax = LifebloodMax;
            int previousLifeblood = CurrentLifeblood;
            int previousBaseMax = BaseMaxHP;

            int sanitizedMax = Mathf.Max(0, maxHp);
            int sanitizedBaseMax = Mathf.Max(0, baseMaxHp ?? sanitizedMax);
            int sanitizedLifebloodMax = Mathf.Max(0, lifebloodMax);
            int sanitizedHp = Mathf.Clamp(currentHp, 0, sanitizedMax);
            int sanitizedLifeblood = Mathf.Clamp(lifebloodCurrent, 0, sanitizedLifebloodMax);

            int previousTotalMax = Mathf.Max(0, previousMax) + Mathf.Max(0, previousLifebloodMax);
            int sanitizedTotalMax = Mathf.Max(0, sanitizedMax) + Mathf.Max(0, sanitizedLifebloodMax);
            int sanitizedTotalHp = Mathf.Clamp(sanitizedHp + sanitizedLifeblood, 0, sanitizedTotalMax);

            bool suspiciousDrop = previousTotalMax > 1 && sanitizedTotalMax <= 1 && sanitizedTotalHp <= 1;
            bool invulnerableDrop = false;
            if (!CanTakeDamage || (canTakeDamage.HasValue && !canTakeDamage.Value))
            {
                int previousTotalHp = Mathf.Max(0, previousHp) + Mathf.Max(0, previousLifeblood);
                invulnerableDrop = previousTotalHp >= 0 && sanitizedTotalHp < previousTotalHp;
            }

            bool baseChanged = baseMaxHp.HasValue && Mathf.Max(0, baseMaxHp.Value) != Mathf.Max(0, previousBaseMax);
            bool lifebloodCapChanged = Mathf.Max(0, lifebloodMax) != Mathf.Max(0, previousLifebloodMax);
            bool allowAuthoritativeDrop = baseChanged || lifebloodCapChanged;

            if ((suspiciousDrop || invulnerableDrop) && !allowAuthoritativeDrop)
            {
                sanitizedMax = previousMax;
                sanitizedHp = Mathf.Clamp(Mathf.Max(previousHp, currentHp), 0, Mathf.Max(1, sanitizedMax));
                sanitizedBaseMax = Mathf.Max(0, previousBaseMax);
                sanitizedLifebloodMax = previousLifebloodMax;
                sanitizedLifeblood = Mathf.Clamp(Mathf.Max(previousLifeblood, lifebloodCurrent), 0, sanitizedLifebloodMax);
                if (ModConfig.Instance.logShade)
                {
                    try
                    {
                        string reason = suspiciousDrop
                            ? "suspicious"
                            : "invulnerable";
                        string requestedBase = baseMaxHp.HasValue ? baseMaxHp.Value.ToString() : "null";
                        Debug.LogWarning($"[ShadePersistence] Ignored {reason} state capture (hp={currentHp}, max={maxHp}, lifeblood={lifebloodCurrent}, lifebloodMax={lifebloodMax}, baseMax={requestedBase}, prevHp={previousHp}, prevMax={previousMax}, prevLifeblood={previousLifeblood}, prevLifebloodMax={previousLifebloodMax}, prevBaseMax={previousBaseMax}, invulnerable={!CanTakeDamage}).");
                    }
                    catch
                    {
                    }
                }
            }

            if (sanitizedBaseMax <= 0 && sanitizedMax > 0)
            {
                sanitizedBaseMax = sanitizedMax;
            }

            if (sanitizedMax <= 0 && sanitizedLifebloodMax <= 0)
            {
                sanitizedMax = 1;
            }

            MaxHP = sanitizedMax;
            CurrentHP = Mathf.Clamp(sanitizedHp, 0, MaxHP > 0 ? MaxHP : sanitizedHp);
            BaseMaxHP = Mathf.Max(0, sanitizedBaseMax);
            LifebloodMax = sanitizedLifebloodMax;
            CurrentLifeblood = Mathf.Clamp(sanitizedLifeblood, 0, LifebloodMax);
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
            BaseMaxHP = -1;
            CurrentLifeblood = -1;
            LifebloodMax = -1;
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
                BaseMaxHP = BaseMaxHP,
                CurrentLifeblood = CurrentLifeblood,
                LifebloodMax = LifebloodMax,
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
            if (snapshot.MaxHP > 0 || snapshot.LifebloodMax > 0)
            {
                Capture(snapshot.CurrentHP, snapshot.MaxHP, snapshot.CurrentLifeblood, snapshot.LifebloodMax, snapshot.Soul, snapshot.CanTakeDamage, snapshot.BaseMaxHP);
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

            public int BaseMaxHP { get; set; }

            public int CurrentLifeblood { get; set; }

            public int LifebloodMax { get; set; }

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

        public void SetDiscoveredCharms(IEnumerable<int>? charmIds)
        {
            var sanitized = new HashSet<int>();

            if (charmIds != null)
            {
                foreach (var charmId in charmIds)
                {
                    if (charmId >= 0)
                    {
                        sanitized.Add(charmId);
                    }
                }
            }

            if (_equippedCharmLoadouts.Count > 0)
            {
                foreach (var loadout in _equippedCharmLoadouts.Values)
                {
                    if (loadout == null)
                    {
                        continue;
                    }

                    foreach (var charmId in loadout)
                    {
                        if (charmId >= 0)
                        {
                            sanitized.Add(charmId);
                        }
                    }
                }
            }

            if (!_discoveredCharmIds.SetEquals(sanitized))
            {
                _discoveredCharmIds.Clear();
                _discoveredCharmIds.UnionWith(sanitized);
            }

            if (_equippedCharmLoadouts.Count == 0 || sanitized.Count == 0)
            {
                return;
            }

            var loadoutIds = _equippedCharmLoadouts.Keys.ToArray();
            foreach (var loadoutId in loadoutIds)
            {
                if (!_equippedCharmLoadouts.TryGetValue(loadoutId, out var loadout) || loadout == null)
                {
                    continue;
                }

                loadout.RemoveWhere(id => !sanitized.Contains(id));
                if (loadout.Count == 0)
                {
                    _equippedCharmLoadouts.Remove(loadoutId);
                }
            }
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
