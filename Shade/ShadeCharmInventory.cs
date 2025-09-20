#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed class ShadeCharmInventory
    {
        private const int DefaultNotchCapacity = 6;

        private readonly List<ShadeCharmDefinition> _definitions;
        private readonly Dictionary<ShadeCharmId, ShadeCharmDefinition> _definitionMap;
        private readonly HashSet<ShadeCharmId> _owned;
        private readonly HashSet<ShadeCharmId> _equipped;
        private readonly HashSet<ShadeCharmId> _broken;
        private readonly HashSet<ShadeCharmId> _newlyDiscovered;
        private bool _suppressStateChanged;
        private int _notchCapacity;

        public event Action? StateChanged;

        public ShadeCharmInventory()
        {
            _definitions = new List<ShadeCharmDefinition>();

            bool furyActive = false;

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.WaywardCompass),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AdjustLeash(8f, 4f, 6f, 6f),
                    OnRemoved = ctx => ctx.Controller?.AdjustLeash(-8f, -4f, -6f, -6f)
                },
                displayName: "Wayward Compass",
                description: "The shade feels Hornet's call more keenly, extending their leash so the companion is easier to find in the fray.",
                notchCost: 1,
                fallbackTint: new Color(0.74f, 0.77f, 0.83f),
                enumId: ShadeCharmId.WaywardCompass,
                iconName: "shade_charm_wayward_compass"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Sprintmaster),
                statModifiers: new ShadeCharmStatModifiers
                {
                    MoveSpeedMultiplier = 1.1f,
                    SprintSpeedMultiplier = 1.15f,
                    SprintDashCooldownMultiplier = 0.85f
                },
                displayName: "Sprintmaster",
                description: "Fleet voidstuff guides the companion's steps, boosting both its pace and the responsiveness of its sprint.",
                notchCost: 1,
                fallbackTint: new Color(0.92f, 0.58f, 0.36f),
                enumId: ShadeCharmId.Sprintmaster,
                iconName: "shade_charm_sprintmaster"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Dashmaster),
                statModifiers: new ShadeCharmStatModifiers
                {
                    SprintDashCooldownMultiplier = 0.75f,
                    SprintDashDurationMultiplier = 1.3f,
                    SprintDashSpeedMultiplier = 1.1f
                },
                displayName: "Dashmaster",
                description: "Teaches the shade relentless motion, shortening the recovery after quick bursts and carrying momentum further.",
                notchCost: 2,
                fallbackTint: new Color(0.35f, 0.70f, 0.78f),
                enumId: ShadeCharmId.Dashmaster,
                iconName: "shade_charm_dashmaster"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.ShamanStone),
                statModifiers: new ShadeCharmStatModifiers
                {
                    ShriekCooldownMultiplier = 0.9f,
                    QuakeCooldownMultiplier = 0.9f
                },
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplySpellDamage(1.3f),
                    OnRemoved = ctx => ctx.Controller?.MultiplySpellDamage(1f / 1.3f)
                },
                displayName: "Shaman Stone",
                description: "Empowers the shade's spells, letting screams and quakes strike harder while returning to readiness more quickly.",
                notchCost: 3,
                fallbackTint: new Color(0.56f, 0.32f, 0.66f),
                enumId: ShadeCharmId.ShamanStone,
                iconName: "shade_charm_shaman_stone"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SpellTwister),
                statModifiers: new ShadeCharmStatModifiers
                {
                    ProjectileSoulCostMultiplier = 0.85f,
                    ShriekSoulCostMultiplier = 0.85f,
                    QuakeSoulCostMultiplier = 0.85f,
                    FocusSoulCostMultiplier = 0.9f
                },
                displayName: "Spell Twister",
                description: "Refines void channeling so spells and focus alike draw less soul from the shared reserve.",
                notchCost: 2,
                fallbackTint: new Color(0.40f, 0.48f, 0.86f),
                enumId: ShadeCharmId.SpellTwister,
                iconName: "shade_charm_spell_twister"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.QuickSlash),
                statModifiers: new ShadeCharmStatModifiers
                {
                    NailCooldownMultiplier = 0.6f
                },
                displayName: "Quick Slash",
                description: "The companion's blade darts with relentless rhythm, dramatically reducing the pause between nail strikes.",
                notchCost: 3,
                fallbackTint: new Color(0.86f, 0.32f, 0.32f),
                enumId: ShadeCharmId.QuickSlash,
                iconName: "shade_charm_quick_slash"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.MarkOfPride),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplyNailScale(1.35f),
                    OnRemoved = ctx => ctx.Controller?.MultiplyNailScale(1f / 1.35f)
                },
                displayName: "Mark of Pride",
                description: "Ancient sigils elongate the shade's nail, allowing its swings to cover a far wider span around Hornet.",
                notchCost: 3,
                fallbackTint: new Color(0.74f, 0.43f, 0.24f),
                enumId: ShadeCharmId.MarkOfPride,
                iconName: "shade_charm_mark_of_pride"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Longnail),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplyNailScale(1.2f),
                    OnRemoved = ctx => ctx.Controller?.MultiplyNailScale(1f / 1.2f)
                },
                displayName: "Longnail",
                description: "Woven strands stretch the companion's reach, slightly increasing the length of its nail arcs.",
                notchCost: 2,
                fallbackTint: new Color(0.58f, 0.66f, 0.44f),
                enumId: ShadeCharmId.Longnail,
                iconName: "shade_charm_longnail"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SoulCatcher),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddSoulGainBonus(6),
                    OnRemoved = ctx => ctx.Controller?.AddSoulGainBonus(-6)
                },
                displayName: "Soul Catcher",
                description: "The shade draws more soul from each successful strike, helping Hornet recover resources mid-fight.",
                notchCost: 2,
                fallbackTint: new Color(0.30f, 0.62f, 0.68f),
                enumId: ShadeCharmId.SoulCatcher,
                iconName: "shade_charm_soul_catcher"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.FragileStrength),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplyNailDamage(1.5f),
                    OnRemoved = ctx => ctx.Controller?.MultiplyNailDamage(1f / 1.5f)
                },
                displayName: "Fragile Strength",
                description: "Greatly boosts the companion's nail damage, but the charm will shatter if the shade is destroyed.",
                notchCost: 3,
                fallbackTint: new Color(0.82f, 0.52f, 0.18f),
                enumId: ShadeCharmId.FragileStrength,
                iconName: "shade_charm_fragile_strength"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SoulEater),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddSoulGainBonus(12),
                    OnRemoved = ctx => ctx.Controller?.AddSoulGainBonus(-12)
                },
                displayName: "Soul Eater",
                description: "Hunger for essence swells within the shade, drawing far more soul from each strike to fuel spells and focus.",
                notchCost: 4,
                fallbackTint: new Color(0.39f, 0.24f, 0.52f),
                enumId: ShadeCharmId.SoulEater,
                iconName: "shade_charm_soul_eater"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Grubsong),
                hooks: new ShadeCharmHooks
                {
                    OnShadeDamaged = (ctx, evt) =>
                    {
                        if (evt.WasPrevented || evt.ActualDamage <= 0)
                        {
                            return;
                        }

                        int reward = Mathf.Max(6, evt.ActualDamage * 6);
                        ctx.Controller?.GainShadeSoul(reward);
                    }
                },
                displayName: "Grubsong",
                description: "When the shade is struck it sings with collected kin, restoring a measure of soul to keep the fight alive.",
                notchCost: 1,
                fallbackTint: new Color(0.47f, 0.73f, 0.54f),
                enumId: ShadeCharmId.Grubsong,
                iconName: "shade_charm_grubsong"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.QuickFocus),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplyFocusTime(0.65f),
                    OnRemoved = ctx => ctx.Controller?.MultiplyFocusTime(1f / 0.65f)
                },
                displayName: "Quick Focus",
                description: "Channeling void becomes second nature, greatly reducing the time required for the shade to mend wounds.",
                notchCost: 3,
                fallbackTint: new Color(0.52f, 0.77f, 0.93f),
                enumId: ShadeCharmId.QuickFocus,
                iconName: "shade_charm_quick_focus"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.DeepFocus),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx =>
                    {
                        ctx.Controller?.MultiplyFocusTime(1.4f);
                        ctx.Controller?.AddFocusHealBonus(1);
                        ctx.Controller?.AddHornetFocusHealBonus(1);
                    },
                    OnRemoved = ctx =>
                    {
                        ctx.Controller?.MultiplyFocusTime(1f / 1.4f);
                        ctx.Controller?.AddFocusHealBonus(-1);
                        ctx.Controller?.AddHornetFocusHealBonus(-1);
                    }
                },
                displayName: "Deep Focus",
                description: "The shade's meditations flow slowly but mend extra wounds and share greater restorative energy with Hornet.",
                notchCost: 4,
                fallbackTint: new Color(0.28f, 0.52f, 0.76f),
                enumId: ShadeCharmId.DeepFocus,
                iconName: "shade_charm_deep_focus"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.ShapeOfUnn),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetFocusMovementAllowed(true),
                    OnRemoved = ctx => ctx.Controller?.SetFocusMovementAllowed(false)
                },
                displayName: "Shape of Unn",
                description: "Blessing of Unn lets the shade glide while focusing, keeping pace with Hornet without breaking concentration.",
                notchCost: 2,
                fallbackTint: new Color(0.32f, 0.68f, 0.40f),
                enumId: ShadeCharmId.ShapeOfUnn,
                iconName: "shade_charm_shape_of_unn"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SteadyBody),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.ModifyKnockbackSuppression(1),
                    OnRemoved = ctx => ctx.Controller?.ModifyKnockbackSuppression(-1)
                },
                displayName: "Steady Body",
                description: "The shade plants itself firmly, resisting enemy blows so its position beside Hornet barely shifts.",
                notchCost: 1,
                fallbackTint: new Color(0.78f, 0.74f, 0.48f),
                enumId: ShadeCharmId.SteadyBody,
                iconName: "shade_charm_steady_body"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.StalwartShell),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplyHurtInvulnerability(1.35f),
                    OnRemoved = ctx => ctx.Controller?.MultiplyHurtInvulnerability(1f / 1.35f)
                },
                displayName: "Stalwart Shell",
                description: "Layers of hardened void linger after each hit, extending the shade's invulnerability to survive brutal fights.",
                notchCost: 2,
                fallbackTint: new Color(0.64f, 0.58f, 0.44f),
                enumId: ShadeCharmId.StalwartShell,
                iconName: "shade_charm_stalwart_shell"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.FuryOfTheFallen),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx =>
                    {
                        furyActive = false;
                        ctx.Controller?.SetConditionalNailDamageMultiplier(nameof(ShadeCharmId.FuryOfTheFallen), 1f);
                    },
                    OnRemoved = ctx =>
                    {
                        furyActive = false;
                        ctx.Controller?.ClearConditionalNailDamageMultiplier(nameof(ShadeCharmId.FuryOfTheFallen));
                    },
                    OnUpdate = (ctx, _) =>
                    {
                        var controller = ctx.Controller;
                        if (controller == null)
                        {
                            return;
                        }

                        bool shouldBoost = controller.GetCurrentHP() <= 1;
                        if (shouldBoost != furyActive)
                        {
                            furyActive = shouldBoost;
                            controller.SetConditionalNailDamageMultiplier(nameof(ShadeCharmId.FuryOfTheFallen), shouldBoost ? 1.75f : 1f);
                        }
                    }
                },
                displayName: "Fury of the Fallen",
                description: "When only a sliver of vitality remains the shade fights with reckless ferocity, greatly amplifying nail damage.",
                notchCost: 2,
                fallbackTint: new Color(0.82f, 0.29f, 0.35f),
                enumId: ShadeCharmId.FuryOfTheFallen,
                iconName: "shade_charm_fury_of_the_fallen"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.NailmastersGlory),
                statModifiers: new ShadeCharmStatModifiers
                {
                    NailCooldownMultiplier = 0.7f
                },
                displayName: "Nailmaster's Glory",
                description: "Lessons from the Nailmasters echo through the shade, letting their blade recover swiftly between strikes.",
                notchCost: 3,
                fallbackTint: new Color(0.83f, 0.68f, 0.41f),
                enumId: ShadeCharmId.NailmastersGlory,
                iconName: "shade_charm_nailmasters_glory"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.FragileHeart),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddMaxHpBonus(2, true),
                    OnRemoved = ctx => ctx.Controller?.AddMaxHpBonus(-2, false)
                },
                displayName: "Fragile Heart",
                description: "Bolsters the shade with additional vitality, but the charm will shatter if the companion falls in battle.",
                notchCost: 2,
                fallbackTint: new Color(0.94f, 0.56f, 0.60f),
                enumId: ShadeCharmId.FragileHeart,
                iconName: "shade_charm_fragile_heart"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SharpShadow),
                statModifiers: new ShadeCharmStatModifiers
                {
                    TeleportCooldownMultiplier = 0.7f
                },
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplyTeleportChannelTime(0.85f),
                    OnRemoved = ctx => ctx.Controller?.MultiplyTeleportChannelTime(1f / 0.85f)
                },
                displayName: "Sharp Shadow",
                description: "Streamlined void lets the companion blink more readily, shortening both the charge-up and recovery of teleports.",
                notchCost: 2,
                fallbackTint: new Color(0.28f, 0.24f, 0.42f),
                enumId: ShadeCharmId.SharpShadow,
                iconName: "shade_charm_sharp_shadow"));

            _definitionMap = _definitions
                .Where(def => def.EnumId.HasValue)
                .ToDictionary(def => def.EnumId!.Value);
            _owned = new HashSet<ShadeCharmId>();
            _equipped = new HashSet<ShadeCharmId>();
            _broken = new HashSet<ShadeCharmId>();
            _newlyDiscovered = new HashSet<ShadeCharmId>();
            _notchCapacity = DefaultNotchCapacity;
        }

        public IReadOnlyList<ShadeCharmDefinition> AllCharms => _definitions;

        public int NotchCapacity
        {
            get => _notchCapacity;
            set
            {
                int clamped = Mathf.Clamp(value, 0, 20);
                if (_notchCapacity == clamped)
                {
                    return;
                }

                _notchCapacity = clamped;
                TrimToCapacity();
                RaiseStateChanged();
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

        public IReadOnlyCollection<ShadeCharmId> GetEquipped() => _equipped.ToArray();

        public IReadOnlyCollection<ShadeCharmDefinition> GetEquippedDefinitions()
        {
            return _equipped
                .Where(id => _definitionMap.TryGetValue(id, out _))
                .Select(id => _definitionMap[id])
                .ToArray();
        }

        public IReadOnlyCollection<ShadeCharmId> GetOwnedCharms() => _owned.ToArray();

        public IReadOnlyCollection<ShadeCharmId> GetBrokenCharms() => _broken.ToArray();

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

            if (UsedNotches + definition.NotchCost > _notchCapacity)
            {
                message = "Not enough notches available.";
                return false;
            }

            _equipped.Add(id);
            message = $"{definition.DisplayName} equipped.";
            RaiseStateChanged();
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
            if (_equipped.Count == 0)
            {
                return;
            }

            _equipped.Clear();
            RaiseStateChanged();
        }

        public void GrantCharm(ShadeCharmId id)
        {
            if (!_definitionMap.ContainsKey(id))
            {
                return;
            }

            bool added = _owned.Add(id);
            if (added)
            {
                _newlyDiscovered.Add(id);
                RaiseStateChanged();
            }
        }

        public void GrantAllCharms()
        {
            bool changed = false;
            foreach (var id in _definitionMap.Keys)
            {
                if (_owned.Add(id))
                {
                    _newlyDiscovered.Add(id);
                    changed = true;
                }
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

            if (_equipped.Count > 0)
            {
                _equipped.Clear();
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

        public bool BreakCharm(ShadeCharmId id)
        {
            if (!_definitionMap.ContainsKey(id))
            {
                return false;
            }

            bool newlyBroken = _broken.Add(id);
            bool removed = _equipped.Remove(id);
            if (newlyBroken || removed)
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

        public bool MarkCharmSeen(ShadeCharmId id)
        {
            return _newlyDiscovered.Remove(id);
        }

        public void LoadState(
            IEnumerable<ShadeCharmId>? owned,
            IEnumerable<ShadeCharmId>? equipped,
            IEnumerable<ShadeCharmId>? broken,
            int notchCapacity,
            IEnumerable<ShadeCharmId>? newlyDiscovered = null)
        {
            _suppressStateChanged = true;

            _owned.Clear();
            _equipped.Clear();
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
                foreach (var id in SanitizeIds(equipped))
                {
                    if (_owned.Contains(id) && !_broken.Contains(id))
                    {
                        _equipped.Add(id);
                    }
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

            _notchCapacity = Mathf.Clamp(notchCapacity > 0 ? notchCapacity : DefaultNotchCapacity, 0, 20);
            TrimToCapacity();

            _suppressStateChanged = false;
            RaiseStateChanged();
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

        private void RaiseStateChanged()
        {
            if (_suppressStateChanged)
            {
                return;
            }

            StateChanged?.Invoke();
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

            bool changed = false;
            foreach (var (_, id) in ordered)
            {
                if (_equipped.Remove(id))
                {
                    changed = true;
                }
                if (UsedNotches <= _notchCapacity)
                {
                    break;
                }
            }

            if (changed)
            {
                RaiseStateChanged();
            }
        }
    }
}
