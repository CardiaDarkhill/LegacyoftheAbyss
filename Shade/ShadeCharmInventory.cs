#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed class ShadeCharmInventory
    {
        private const int DefaultNotchCapacity = 3;
        private const int OvercharmAttemptsRequired = 3;

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

        public event Action? StateChanged;

        public ShadeCharmInventory()
        {
            _definitions = new List<ShadeCharmDefinition>();

            bool furyActive = false;
            bool elegyDamageBoostActive = false;
            float hivebloodTimer = 0f;
            float kingsoulTimer = 0f;

            // TODO: Surface the shade's map position while Wayward Compass is equipped.
            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.WaywardCompass),
                displayName: "Wayward Compass",
                description: "Whispers its location to the bearer whenever a map is open, allowing wanderers to pinpoint their current location.",
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
                description: "Bears the likeness of a strange bug known only as 'The Sprintmaster'. Increases the running speed of the bearer, allowing them to avoid danger or overtake rivals.",
                notchCost: 1,
                fallbackTint: new Color(0.92f, 0.58f, 0.36f),
                enumId: ShadeCharmId.Sprintmaster,
                iconName: "shade_charm_sprintmaster"));

            // TODO: Allow the shade to dash downward when Dashmaster is equipped.
            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Dashmaster),
                statModifiers: new ShadeCharmStatModifiers
                {
                    SprintDashCooldownMultiplier = 0.75f
                },
                displayName: "Dashmaster",
                description: "Bears the likeness of an eccentric bug known only as 'The Dashmaster'. The bearer will be able to dash more often as well as dash downwards. Perfect for those who want to move around as quickly as possible.",
                notchCost: 2,
                fallbackTint: new Color(0.35f, 0.70f, 0.78f),
                enumId: ShadeCharmId.Dashmaster,
                iconName: "shade_charm_dashmaster"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.ShamanStone),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplySpellDamage(1.3f),
                    OnRemoved = ctx => ctx.Controller?.MultiplySpellDamage(1f / 1.3f)
                },
                displayName: "Shaman Stone",
                description: "Said to contain the knowledge of past generations. Increases the power of Spells, dealing more damage to foes.",
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
                    QuakeSoulCostMultiplier = 0.85f
                },
                displayName: "Spell Twister",
                description: "Reflecting the desire of the Soul Sanctum for mastery over SOUL. Increases the bearer's mastery of Spells, reducing the SOUL cost of casting them.",
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
                description: "Born from imperfect, discarded Nails that have fused together. The Nails still long to feel proper use and will grant the bearer faster attacks.",
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
                description: "Contains the passion, skill and pride of the Moth Tribe. Increases the range of the bearer's nail, allowing them to strike foes from further away.",
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
                description: "A Nail forged long ago. Increases the range of the bearer's nail, allowing them to strike foes from further away.",
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
                description: "Used by shamans to draw more SOUL from the world around them. Increases the amount of SOUL gained when striking an enemy with the nail.",
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
                description: "Strengthens the bearer, allowing them to deal more damage to foes. If its bearer is killed, this charm will break.",
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
                description: "Forgotten shaman artefact, used to draw SOUL from still-living creatures. Greatly increases the amount of SOUL gained when striking an enemy with the nail.",
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
                description: "Contains the gratitude of freed Grubs. Gain SOUL when taking damage.",
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
                description: "Allows the bearer to focus SOUL at a much faster rate.",
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
                description: "Naturally formed within a crystal over a long period. Draws in SOUL from the surrounding air. The bearer will focus SOUL at a slower rate, but the healing effect will double.",
                notchCost: 4,
                fallbackTint: new Color(0.28f, 0.52f, 0.76f),
                enumId: ShadeCharmId.DeepFocus,
                iconName: "shade_charm_deep_focus"));

            // TODO: Morph the shade while focusing to better mirror Shape of Unn's form change.
            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.ShapeOfUnn),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetFocusMovementAllowed(true),
                    OnRemoved = ctx => ctx.Controller?.SetFocusMovementAllowed(false)
                },
                displayName: "Shape of Unn",
                description: "Reveals the form of Unn within the bearer's SOUL. While focusing SOUL, the bearer will take on a new shape and can move freely to avoid danger.",
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
                description: "Keeps its bearer from recoiling backwards when they strike an enemy with a nail.",
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
                description: "Builds resilience. When recovering from damage, the bearer will remain invulnerable for longer.",
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
                description: "Embodies the fury and heroism that comes upon those who are about to die. When close to death, the bearer's strength will increase.",
                notchCost: 2,
                fallbackTint: new Color(0.82f, 0.29f, 0.35f),
                enumId: ShadeCharmId.FuryOfTheFallen,
                iconName: "shade_charm_fury_of_the_fallen"));

            // TODO: Teach the companion to unleash Nail Arts faster when Nailmaster's Glory is equipped.
            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.NailmastersGlory),
                displayName: "Nailmaster's Glory",
                description: "Contains the passion of Nailmasters past. Increases the power of Nail Arts, allowing them to be unleashed much quicker.",
                notchCost: 3,
                fallbackTint: new Color(0.83f, 0.68f, 0.41f),
                enumId: ShadeCharmId.NailmastersGlory,
                iconName: "shade_charm_nailmasters_glory"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.CarefreeMelody),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetCarefreeMelodyChance(0.25f),
                    OnRemoved = ctx => ctx.Controller?.SetCarefreeMelodyChance(0f)
                },
                displayName: "Carefree Melody",
                description: "A soothing anthem that sometimes lets blows simply glance away from the shade.",
                notchCost: 3,
                fallbackTint: new Color(0.86f, 0.78f, 0.56f),
                enumId: ShadeCharmId.CarefreeMelody,
                iconName: "shade_charm_carefree_melody"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.FragileHeart),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddMaxHpBonus(2, true),
                    OnRemoved = ctx => ctx.Controller?.AddMaxHpBonus(-2, false)
                },
                displayName: "Fragile Heart",
                description: "Increases the health of the bearer, allowing them to take more damage. If its bearer is killed, this charm will break.",
                notchCost: 2,
                fallbackTint: new Color(0.94f, 0.56f, 0.60f),
                enumId: ShadeCharmId.FragileHeart,
                iconName: "shade_charm_fragile_heart"));

            // TODO: Empower the shade's teleport to damage foes and extend its reach for Sharp Shadow.
            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SharpShadow),
                displayName: "Sharp Shadow",
                description: "Contains a forbidden spell that transforms shadows into deadly weapons. When using Shadow Dash, the bearer's body will sharpen and damage enemies.",
                notchCost: 2,
                fallbackTint: new Color(0.28f, 0.24f, 0.42f),
                enumId: ShadeCharmId.SharpShadow,
                iconName: "shade_charm_sharp_shadow"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.GrubberflysElegy),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx =>
                    {
                        elegyDamageBoostActive = false;
                        ctx.Controller?.SetConditionalNailDamageMultiplier(nameof(ShadeCharmId.GrubberflysElegy), 1f);
                    },
                    OnRemoved = ctx =>
                    {
                        elegyDamageBoostActive = false;
                        ctx.Controller?.ClearConditionalNailDamageMultiplier(nameof(ShadeCharmId.GrubberflysElegy));
                    },
                    OnUpdate = (ctx, _) =>
                    {
                        var controller = ctx.Controller;
                        if (controller == null)
                        {
                            return;
                        }

                        bool atFull = controller.GetCurrentHP() >= controller.GetMaxHP();
                        if (atFull != elegyDamageBoostActive)
                        {
                            elegyDamageBoostActive = atFull;
                            controller.SetConditionalNailDamageMultiplier(nameof(ShadeCharmId.GrubberflysElegy), atFull ? 1.4f : 1f);
                        }
                    }
                },
                displayName: "Grubberfly's Elegy",
                description: "Calls upon the gratitude of every rescued grub. While at full health the shade's nail strikes with brilliant, searing force.",
                notchCost: 3,
                fallbackTint: new Color(0.50f, 0.68f, 0.94f),
                enumId: ShadeCharmId.GrubberflysElegy,
                iconName: "shade_charm_grubberflys_elegy"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.FragileGreed),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddSoulGainBonus(8),
                    OnRemoved = ctx => ctx.Controller?.AddSoulGainBonus(-8)
                },
                displayName: "Fragile Greed",
                description: "Fills the bearer with a desire to reap every scrap of SOUL. Increases SOUL gained from attacks, but will shatter if the shade is defeated.",
                notchCost: 2,
                fallbackTint: new Color(0.90f, 0.78f, 0.32f),
                enumId: ShadeCharmId.FragileGreed,
                iconName: "shade_charm_fragile_greed"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.HeavyBlow),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplyKnockbackForce(1.4f),
                    OnRemoved = ctx => ctx.Controller?.MultiplyKnockbackForce(1f / 1.4f)
                },
                displayName: "Heavy Blow",
                description: "Embues the shade's nail with tremendous force, sending foes staggering further with every strike.",
                notchCost: 2,
                fallbackTint: new Color(0.56f, 0.39f, 0.23f),
                enumId: ShadeCharmId.HeavyBlow,
                iconName: "shade_charm_heavy_blow"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.BaldurShell),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetFocusDamageShield(true),
                    OnRemoved = ctx => ctx.Controller?.SetFocusDamageShield(false)
                },
                displayName: "Baldur Shell",
                description: "A living protective shell that curls around the shade while focusing, helping it shrug off stray blows.",
                notchCost: 2,
                fallbackTint: new Color(0.58f, 0.74f, 0.80f),
                enumId: ShadeCharmId.BaldurShell,
                iconName: "shade_charm_baldur_shell"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.LifebloodHeart),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddMaxHpBonus(2, true),
                    OnRemoved = ctx => ctx.Controller?.AddMaxHpBonus(-2, false)
                },
                displayName: "Lifeblood Heart",
                description: "The shade grows new lifeblood nodes, granting extra vitality that must be renewed at benches.",
                notchCost: 2,
                fallbackTint: new Color(0.35f, 0.73f, 0.88f),
                enumId: ShadeCharmId.LifebloodHeart,
                iconName: "shade_charm_lifeblood_heart"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.LifebloodCore),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddMaxHpBonus(4, true),
                    OnRemoved = ctx => ctx.Controller?.AddMaxHpBonus(-4, false)
                },
                displayName: "Lifeblood Core",
                description: "A massive core of lifeblood that courses through the shade, dramatically increasing temporary vitality.",
                notchCost: 4,
                fallbackTint: new Color(0.26f, 0.62f, 0.84f),
                enumId: ShadeCharmId.LifebloodCore,
                iconName: "shade_charm_lifeblood_core"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.JonisBlessing),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx =>
                    {
                        ctx.Controller?.SetFocusHealingDisabled(true);
                        ctx.Controller?.AddMaxHpBonus(6, true);
                    },
                    OnRemoved = ctx =>
                    {
                        ctx.Controller?.SetFocusHealingDisabled(false);
                        ctx.Controller?.AddMaxHpBonus(-6, false);
                    }
                },
                displayName: "Joni's Blessing",
                description: "Blesses the shade with vast lifeblood reserves. Focus can no longer heal, but the companion's vitality surges.",
                notchCost: 4,
                fallbackTint: new Color(0.58f, 0.38f, 0.72f),
                enumId: ShadeCharmId.JonisBlessing,
                iconName: "shade_charm_jonis_blessing"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Hiveblood),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => hivebloodTimer = 0f,
                    OnRemoved = ctx => hivebloodTimer = 0f,
                    OnShadeDamaged = (ctx, evt) =>
                    {
                        if (evt.ActualDamage > 0 && !evt.WasPrevented)
                        {
                            hivebloodTimer = 0f;
                        }
                    },
                    OnUpdate = (ctx, delta) =>
                    {
                        var controller = ctx.Controller;
                        if (controller == null)
                        {
                            return;
                        }

                        if (controller.GetCurrentHP() >= controller.GetMaxHP())
                        {
                            hivebloodTimer = 0f;
                            return;
                        }

                        hivebloodTimer += Mathf.Max(0f, delta);
                        if (hivebloodTimer >= 10f)
                        {
                            hivebloodTimer = 0f;
                            controller.ReviveToAtLeast(controller.GetCurrentHP() + 1);
                        }
                    }
                },
                displayName: "Hiveblood",
                description: "Honeyed lifeblood seeps through the shade, knitting wounds back together if it can avoid harm for a short while.",
                notchCost: 4,
                fallbackTint: new Color(0.96f, 0.76f, 0.32f),
                enumId: ShadeCharmId.Hiveblood,
                iconName: "shade_charm_hiveblood"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Kingsoul),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => kingsoulTimer = 0f,
                    OnRemoved = ctx => kingsoulTimer = 0f,
                    OnUpdate = (ctx, delta) =>
                    {
                        var controller = ctx.Controller;
                        if (controller == null)
                        {
                            return;
                        }

                        kingsoulTimer += Mathf.Max(0f, delta);
                        if (kingsoulTimer >= 1.5f)
                        {
                            kingsoulTimer -= 1.5f;
                            if (controller.GetShadeSoul() < controller.GetShadeSoulMax())
                            {
                                controller.GainShadeSoul(4);
                            }
                        }
                    }
                },
                displayName: "Kingsoul",
                description: "A revered talisman of Hallownest. Gradually draws SOUL into the shade, empowering every spell and ability.",
                notchCost: 5,
                fallbackTint: new Color(0.92f, 0.91f, 0.75f),
                enumId: ShadeCharmId.Kingsoul,
                iconName: "shade_charm_kingsoul"));

            _definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.VoidHeart),
                statModifiers: new ShadeCharmStatModifiers
                {
                    SprintSpeedMultiplier = 1.05f,
                    FocusSoulCostMultiplier = 0.85f,
                    ShadeSoulCapacityFlatBonus = 15
                },
                displayName: "Void Heart",
                description: "The culmination of the Abyss. Harmonises Hornet and shade, reducing SOUL costs and granting unmatched stamina.",
                notchCost: 0,
                fallbackTint: new Color(0.32f, 0.32f, 0.42f),
                enumId: ShadeCharmId.VoidHeart,
                iconName: "shade_charm_void_heart"));

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
                int clamped = Mathf.Clamp(value, 0, 20);
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

        public IReadOnlyCollection<ShadeCharmId> GetOwnedCharms() => _owned.ToArray();

        public IReadOnlyCollection<ShadeCharmId> GetBrokenCharms() => _broken.ToArray();

        public IReadOnlyCollection<ShadeCharmId> GetNewlyDiscovered() => _newlyDiscovered.ToArray();

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

            if (id == ShadeCharmId.Kingsoul && _equipped.Contains(ShadeCharmId.VoidHeart))
            {
                message = "Void Heart refuses to share power with Kingsoul.";
                return false;
            }

            bool removedKingsoul = false;
            int removedKingsoulIndex = -1;
            if (id == ShadeCharmId.VoidHeart && _equipped.Contains(ShadeCharmId.Kingsoul))
            {
                removedKingsoulIndex = _equippedOrder.IndexOf(ShadeCharmId.Kingsoul);
                RemoveEquippedInternal(ShadeCharmId.Kingsoul);
                removedKingsoul = true;
            }

            int notchCost = definition.NotchCost;
            if (notchCost > 0 && _notchCapacity <= 0)
            {
                if (removedKingsoul)
                {
                    RestoreEquippedAtIndex(ShadeCharmId.Kingsoul, removedKingsoulIndex);
                }

                message = "Shade lacks any notches to equip this charm.";
                return false;
            }

            int prospectiveNotches = UsedNotches + notchCost;
            bool fits = notchCost <= 0 || prospectiveNotches <= _notchCapacity;

            if (!fits && _isOvercharmed)
            {
                if (removedKingsoul)
                {
                    RestoreEquippedAtIndex(ShadeCharmId.Kingsoul, removedKingsoulIndex);
                }

                message = "Shade is already overcharmed. Unequip a charm first.";
                return false;
            }

            if (!fits && !_isOvercharmed)
            {
                _overcharmAttemptCounter++;
                int remaining = Math.Max(0, OvercharmAttemptsRequired - _overcharmAttemptCounter);
                if (_overcharmAttemptCounter < OvercharmAttemptsRequired)
                {
                    if (removedKingsoul)
                    {
                        RestoreEquippedAtIndex(ShadeCharmId.Kingsoul, removedKingsoulIndex);
                    }

                    message = remaining > 0
                        ? $"Not enough notches. Force equip {remaining} more times to overcharm."
                        : "Not enough notches available.";
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
            bool overcharmChanged = RecalculateOvercharmed();

            if (_isOvercharmed && (!wasOvercharmed || overcharmChanged))
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

            if (removedKingsoul)
            {
                message += " Kingsoul withdrew to make room.";
            }

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
            if (_equipped.Count == 0 && _equippedOrder.Count == 0)
            {
                _overcharmAttemptCounter = 0;
                if (RecalculateOvercharmed())
                {
                    RaiseStateChanged();
                }
                return;
            }

            _equipped.Clear();
            _equippedOrder.Clear();
            _isOvercharmed = false;
            _overcharmAttemptCounter = 0;
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

        public bool BreakCharm(ShadeCharmId id)
        {
            if (!_definitionMap.ContainsKey(id))
            {
                return false;
            }

            bool newlyBroken = _broken.Add(id);
            bool removed = RemoveEquippedInternal(id);
            if (removed)
            {
                _overcharmAttemptCounter = 0;
                RecalculateOvercharmed();
            }
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
                foreach (var id in SanitizeIds(equipped))
                {
                    if (_owned.Contains(id) && !_broken.Contains(id))
                    {
                        AddEquippedInternal(id);
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
            _isOvercharmed = UsedNotches > _notchCapacity;
            TrimToCapacity();
            RecalculateOvercharmed();
            _overcharmAttemptCounter = 0;

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

        private bool AddEquippedInternal(ShadeCharmId id)
        {
            if (!_equipped.Add(id))
            {
                return false;
            }

            _equippedOrder.Add(id);
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

        private void RestoreEquippedAtIndex(ShadeCharmId id, int index)
        {
            if (_equipped.Contains(id) || _equippedOrder.Contains(id))
            {
                return;
            }

            if (!_definitionMap.ContainsKey(id))
            {
                return;
            }

            _equipped.Add(id);
            if (index >= 0 && index <= _equippedOrder.Count)
            {
                _equippedOrder.Insert(index, id);
            }
            else
            {
                _equippedOrder.Add(id);
            }
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
