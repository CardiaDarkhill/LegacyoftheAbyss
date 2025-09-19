#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed class ShadeCharmInventory
    {
        private readonly List<ShadeCharmDefinition> _definitions;
        private readonly Dictionary<ShadeCharmId, ShadeCharmDefinition> _definitionMap;
        private readonly HashSet<ShadeCharmId> _owned;
        private readonly HashSet<ShadeCharmId> _equipped;
        private int _notchCapacity;

        public ShadeCharmInventory()
        {
            _definitions = new List<ShadeCharmDefinition>
            {
                new ShadeCharmDefinition(
                    nameof(ShadeCharmId.AbyssalCore),
                    displayName: "Abyssal Core",
                    description: "Shade focus draws more deeply from the void, reducing the soul required to mend wounds.",
                    notchCost: 2,
                    fallbackTint: new Color(0.58f, 0.29f, 0.76f),
                    enumId: ShadeCharmId.AbyssalCore),
                new ShadeCharmDefinition(
                    nameof(ShadeCharmId.PhantomStride),
                    displayName: "Phantom Stride",
                    description: "Improves the shade's sprinting burst, letting it keep pace when the fight drifts apart.",
                    notchCost: 1,
                    fallbackTint: new Color(0.29f, 0.58f, 0.76f),
                    enumId: ShadeCharmId.PhantomStride),
                new ShadeCharmDefinition(
                    nameof(ShadeCharmId.EchoOfBlades),
                    displayName: "Echo of Blades",
                    description: "Hones spellcasting instincts so that fireballs and nail flurries can be loosed in quicker succession.",
                    notchCost: 2,
                    fallbackTint: new Color(0.76f, 0.45f, 0.29f),
                    enumId: ShadeCharmId.EchoOfBlades),
                new ShadeCharmDefinition(
                    nameof(ShadeCharmId.DuskShroud),
                    displayName: "Dusk Shroud",
                    description: "Wraps the shade in a protective veil, easing the soul drain of its more taxing abilities.",
                    notchCost: 1,
                    fallbackTint: new Color(0.24f, 0.35f, 0.55f),
                    enumId: ShadeCharmId.DuskShroud),
                new ShadeCharmDefinition(
                    nameof(ShadeCharmId.TwinSoulBond),
                    displayName: "Twin Soul Bond",
                    description: "Strengthens the link between Hornet and shade, extending how far the two can stray before the tether snaps.",
                    notchCost: 3,
                    fallbackTint: new Color(0.45f, 0.70f, 0.36f),
                    enumId: ShadeCharmId.TwinSoulBond),
                new ShadeCharmDefinition(
                    nameof(ShadeCharmId.LuminousGuard),
                    displayName: "Luminous Guard",
                    description: "Fragments of light orbit the companion, stiffening its guard and making each blast slightly cheaper.",
                    notchCost: 2,
                    fallbackTint: new Color(0.85f, 0.72f, 0.32f),
                    enumId: ShadeCharmId.LuminousGuard)
            };

            _definitionMap = _definitions
                .Where(def => def.EnumId.HasValue)
                .ToDictionary(def => def.EnumId!.Value);
            _owned = new HashSet<ShadeCharmId>(_definitionMap.Keys);
            _equipped = new HashSet<ShadeCharmId>();
            _notchCapacity = 6;
        }

        public IReadOnlyList<ShadeCharmDefinition> AllCharms => _definitions;

        public int NotchCapacity
        {
            get => _notchCapacity;
            set
            {
                _notchCapacity = Mathf.Clamp(value, 0, 20);
                TrimToCapacity();
            }
        }

        public int UsedNotches
        {
            get
            {
                int total = 0;
                foreach (var id in _equipped)
                {
                    if (_definitionMap.TryGetValue(id, out var definition))
                    {
                        total += definition.NotchCost;
                    }
                }

                return total;
            }
        }

        public bool IsEquipped(ShadeCharmId id) => _equipped.Contains(id);

        public bool IsOwned(ShadeCharmId id) => _owned.Contains(id);

        public ShadeCharmDefinition GetDefinition(ShadeCharmId id)
        {
            if (_definitionMap.TryGetValue(id, out var definition))
            {
                return definition;
            }

            throw new KeyNotFoundException($"No charm definition registered for {id}.");
        }

        public IReadOnlyCollection<ShadeCharmId> GetEquipped()
        {
            return _equipped.ToArray();
        }

        public bool TryEquip(ShadeCharmId id, out string message)
        {
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

            if (_equipped.Contains(id))
            {
                message = $"{definition.DisplayName} is already equipped.";
                return false;
            }

            if (UsedNotches + definition.NotchCost > _notchCapacity)
            {
                message = "Not enough notches available.";
                return false;
            }

            _equipped.Add(id);
            message = $"{definition.DisplayName} equipped.";
            return true;
        }

        public bool TryUnequip(ShadeCharmId id, out string message)
        {
            if (!_equipped.Contains(id))
            {
                message = "Charm not currently equipped.";
                return false;
            }

            _equipped.Remove(id);
            if (_definitionMap.TryGetValue(id, out var definition))
            {
                message = $"{definition.DisplayName} removed.";
            }
            else
            {
                message = "Charm removed.";
            }

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
            _equipped.Clear();
        }

        public void GrantCharm(ShadeCharmId id)
        {
            _owned.Add(id);
        }

        public void GrantAllCharms()
        {
            foreach (var id in _definitionMap.Keys)
            {
                _owned.Add(id);
            }
        }

        private void TrimToCapacity()
        {
            if (UsedNotches <= _notchCapacity)
            {
                return;
            }

            var ordered = _equipped
                .Select(id => (_definitionMap.TryGetValue(id, out var def) ? def.NotchCost : 0, id))
                .OrderByDescending(tuple => tuple.Item1)
                .ToList();

            foreach (var (_, id) in ordered)
            {
                _equipped.Remove(id);
                if (UsedNotches <= _notchCapacity)
                {
                    break;
                }
            }
        }
    }
}
