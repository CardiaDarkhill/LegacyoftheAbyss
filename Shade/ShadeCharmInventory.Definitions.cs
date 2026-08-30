#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// The shade's charm catalogue: data, not logic. It stays an instance method rather than a
    /// static table because several charms (Fury of the Fallen, Kingsoul, Hiveblood) close over
    /// mutable per-inventory state, which a shared table would leak between instances.
    /// </summary>
    internal sealed partial class ShadeCharmInventory
    {
        private List<ShadeCharmDefinition> BuildDefinitions()
        {
            var definitions = new List<ShadeCharmDefinition>();

            bool furyActive = false;
            float kingsoulTimer = 0f;

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.WaywardCompass),
                displayName: "Wayward Compass",
                description: "Whispers of relics still lost to the dark. Marks the rooms holding treasures of the abyss the bearer has yet to claim, on any map they carry.",
                notchCost: 1,
                fallbackTint: new Color(0.74f, 0.77f, 0.83f),
                enumId: ShadeCharmId.WaywardCompass,
                iconName: "shade_charm_wayward_compass"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Sprintmaster),
                statModifiers: new ShadeCharmStatModifiers
                {
                    MoveSpeedMultiplier = 1.2f,
                    SprintSpeedMultiplier = 1.2f
                },
                hooks: new ShadeCharmHooks
                {
                    // Speed comes from the modifiers above; this is only the walk cycle the Knight
                    // swaps to, which has no stat to hang off.
                    OnApplied = ctx => ctx.Controller?.SetSprintmasterEquipped(true),
                    OnRemoved = ctx => ctx.Controller?.SetSprintmasterEquipped(false)
                },
                displayName: "Sprintmaster",
                description: "Bears the likeness of a strange bug known only as 'The Sprintmaster'. Increases the running speed of the bearer, allowing them to avoid danger or overtake rivals.",
                notchCost: 1,
                fallbackTint: new Color(0.92f, 0.58f, 0.36f),
                enumId: ShadeCharmId.Sprintmaster,
                iconName: "shade_charm_sprintmaster"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Dashmaster),
                statModifiers: new ShadeCharmStatModifiers
                {
                    SprintDashCooldownMultiplier = 0.66f
                },
                displayName: "Dashmaster",
                description: "Bears the likeness of an eccentric bug known only as 'The Dashmaster'. The bearer will be able to dash more often. Perfect for those who want to move around as quickly as possible.",
                notchCost: 2,
                fallbackTint: new Color(0.35f, 0.70f, 0.78f),
                enumId: ShadeCharmId.Dashmaster,
                iconName: "shade_charm_dashmaster"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.ShamanStone),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx =>
                    {
                        ctx.Controller?.MultiplySpellDamage(1.3f);
                        ctx.Controller?.SetShamanStoneEquipped(true);
                    },
                    OnRemoved = ctx =>
                    {
                        ctx.Controller?.MultiplySpellDamage(1f / 1.3f);
                        ctx.Controller?.SetShamanStoneEquipped(false);
                    }
                },
                displayName: "Shaman Stone",
                description: "Said to contain the knowledge of past generations. Increases the power of Spells, dealing more damage to foes.",
                notchCost: 3,
                fallbackTint: new Color(0.56f, 0.32f, 0.66f),
                enumId: ShadeCharmId.ShamanStone,
                iconName: "shade_charm_shaman_stone"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SpellTwister),
                statModifiers: new ShadeCharmStatModifiers
                {
                    ProjectileSoulCostMultiplier = 0.73f,
                    ShriekSoulCostMultiplier = 0.73f,
                    QuakeSoulCostMultiplier = 0.73f
                },
                displayName: "Spell Twister",
                description: "Reflecting the desire of the Soul Sanctum for mastery over SOUL. Increases the bearer's mastery of Spells, reducing the SOUL cost of casting them.",
                notchCost: 2,
                fallbackTint: new Color(0.40f, 0.48f, 0.86f),
                enumId: ShadeCharmId.SpellTwister,
                iconName: "shade_charm_spell_twister"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.QuickSlash),
                statModifiers: new ShadeCharmStatModifiers
                {
                    // 0.41s -> 0.25s and 0.35s -> 0.28s, which are Hollow Knight's numbers.
                    NailCooldownMultiplier = 0.25f / 0.41f,
                    NailDurationMultiplier = 0.28f / 0.35f
                },
                displayName: "Quick Slash",
                description: "Born from imperfect, discarded Nails that have fused together. The Nails still long to feel proper use and will grant the bearer faster attacks.",
                notchCost: 3,
                fallbackTint: new Color(0.86f, 0.32f, 0.32f),
                enumId: ShadeCharmId.QuickSlash,
                iconName: "shade_charm_quick_slash"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.MarkOfPride),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplyNailScale(1.25f),
                    OnRemoved = ctx => ctx.Controller?.MultiplyNailScale(1f / 1.25f)
                },
                displayName: "Mark of Pride",
                description: "Contains the passion, skill and pride of the Moth Tribe. Increases the range of the bearer's nail, allowing them to strike foes from further away.",
                notchCost: 3,
                fallbackTint: new Color(0.74f, 0.43f, 0.24f),
                enumId: ShadeCharmId.MarkOfPride,
                iconName: "shade_charm_mark_of_pride"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Longnail),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplyNailScale(1.15f),
                    OnRemoved = ctx => ctx.Controller?.MultiplyNailScale(1f / 1.15f)
                },
                displayName: "Longnail",
                description: "A Nail forged long ago. Increases the range of the bearer's nail, allowing them to strike foes from further away.",
                notchCost: 2,
                fallbackTint: new Color(0.58f, 0.66f, 0.44f),
                enumId: ShadeCharmId.Longnail,
                iconName: "shade_charm_longnail"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SoulCatcher),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddSoulGainBonus(3),
                    OnRemoved = ctx => ctx.Controller?.AddSoulGainBonus(-3)
                },
                displayName: "Soul Catcher",
                description: "Used by shamans to draw more SOUL from the world around them. Increases the amount of SOUL gained when striking an enemy with the nail.",
                notchCost: 2,
                fallbackTint: new Color(0.30f, 0.62f, 0.68f),
                enumId: ShadeCharmId.SoulCatcher,
                iconName: "shade_charm_soul_catcher"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SoulEater),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddSoulGainBonus(8),
                    OnRemoved = ctx => ctx.Controller?.AddSoulGainBonus(-8)
                },
                displayName: "Soul Eater",
                description: "Forgotten shaman artefact, used to draw SOUL from still-living creatures. Greatly increases the amount of SOUL gained when striking an enemy with the nail.",
                notchCost: 4,
                fallbackTint: new Color(0.39f, 0.24f, 0.52f),
                enumId: ShadeCharmId.SoulEater,
                iconName: "shade_charm_soul_eater"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Grubsong),
                hooks: new ShadeCharmHooks
                {
                    OnShadeDamaged = (ctx, evt) =>
                    {
                        if (evt.WasPrevented || evt.ActualDamage <= 0)
                        {
                            return;
                        }

                        int reward = Mathf.Max(15, evt.ActualDamage * 15);
                        ctx.Controller?.GainShadeSoul(reward);
                    }
                },
                displayName: "Grubsong",
                description: "Contains the gratitude of freed Grubs. Gain SOUL when taking damage.",
                notchCost: 1,
                fallbackTint: new Color(0.47f, 0.73f, 0.54f),
                enumId: ShadeCharmId.Grubsong,
                iconName: "shade_charm_grubsong"));

            definitions.Add(new ShadeCharmDefinition(
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

            definitions.Add(new ShadeCharmDefinition(
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
            definitions.Add(new ShadeCharmDefinition(
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

            definitions.Add(new ShadeCharmDefinition(
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

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.StalwartShell),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx =>
                    {
                        ctx.Controller?.MultiplyHurtInvulnerability(1.35f);
                        ctx.Controller?.MultiplyDamageStaggerDuration(0.4f);
                    },
                    OnRemoved = ctx =>
                    {
                        ctx.Controller?.MultiplyHurtInvulnerability(1f / 1.35f);
                        ctx.Controller?.MultiplyDamageStaggerDuration(1f / 0.4f);
                    }
                },
                displayName: "Stalwart Shell",
                description: "Builds resilience. When recovering from damage, the bearer will remain invulnerable for longer.",
                notchCost: 2,
                fallbackTint: new Color(0.64f, 0.58f, 0.44f),
                enumId: ShadeCharmId.StalwartShell,
                iconName: "shade_charm_stalwart_shell"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.FuryOfTheFallen),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx =>
                    {
                        furyActive = false;
                        ctx.Controller?.SetConditionalNailDamageMultiplier(nameof(ShadeCharmId.FuryOfTheFallen), 1f);
                        ctx.Controller?.SetFuryModeActive(false);
                    },
                    OnRemoved = ctx =>
                    {
                        furyActive = false;
                        ctx.Controller?.ClearConditionalNailDamageMultiplier(nameof(ShadeCharmId.FuryOfTheFallen));
                        ctx.Controller?.SetFuryModeActive(false);
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
                            controller.SetFuryModeActive(shouldBoost);
                        }
                    }
                },
                displayName: "Fury of the Fallen",
                description: "Embodies the fury and heroism that comes upon those who are about to die. When close to death, the bearer's strength will increase.",
                notchCost: 2,
                fallbackTint: new Color(0.82f, 0.29f, 0.35f),
                enumId: ShadeCharmId.FuryOfTheFallen,
                iconName: "shade_charm_fury_of_the_fallen"));

            // Nail Arts do not exist for the companion yet, so this charm is inert and its
            // description says so. Drop the trailing sentence when the mechanic lands.
            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.NailmastersGlory),
                displayName: "Nailmaster's Glory",
                description: "Contains the passion of Nailmasters past. Increases the power of Nail Arts, allowing them to be unleashed much quicker.\n\nThis charm will be implemented at a later date, when the mechanics related to it are added to the mod.",
                notchCost: 3,
                fallbackTint: new Color(0.83f, 0.68f, 0.41f),
                enumId: ShadeCharmId.NailmastersGlory,
                iconName: "shade_charm_nailmasters_glory"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.CarefreeMelody),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetCarefreeMelodyEquipped(true),
                    OnRemoved = ctx => ctx.Controller?.SetCarefreeMelodyEquipped(false),
                    OnShadeDamaged = (ctx, evt) =>
                    {
                        if (evt.ActualDamage > 0)
                        {
                            ctx.Controller?.IncrementCarefreeMelodyChance();
                        }
                    }
                },
                displayName: "Carefree Melody",
                description: "A soothing anthem that sometimes lets blows simply glance away from the shade.",
                notchCost: 3,
                fallbackTint: new Color(0.86f, 0.78f, 0.56f),
                enumId: ShadeCharmId.CarefreeMelody,
                iconName: "shade_charm_carefree_melody"));

            definitions.Add(new ShadeCharmDefinition(
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
                iconName: "shade_charm_fragile_heart",
                brokenIconName: "shade_charm_fragileheartbroken0002charmglasshealbroken.png"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.FragileGreed),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetFragileGreedActive(true),
                    OnRemoved = ctx => ctx.Controller?.SetFragileGreedActive(false)
                },
                displayName: "Fragile Greed",
                description: "Fills the bearer with a desire to reap every scrap of SOUL. Increases SOUL gained from attacks, but will shatter if the shade is defeated.",
                notchCost: 2,
                fallbackTint: new Color(0.90f, 0.78f, 0.32f),
                enumId: ShadeCharmId.FragileGreed,
                iconName: "shade_charm_fragile_greed",
                brokenIconName: "shade_charm_fragilegreedbroken0003charmglassgeobroken.png"));

            definitions.Add(new ShadeCharmDefinition(
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
                iconName: "shade_charm_fragile_strength",
                brokenIconName: "shade_charm_fragilestrengthbroken0002charmglassattackupbroken.png"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SharpShadow),
                statModifiers: new ShadeCharmStatModifiers
                {
                    SprintDashSpeedMultiplier = 1.4f
                },
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetSharpShadowEnabled(true),
                    OnRemoved = ctx => ctx.Controller?.SetSharpShadowEnabled(false)
                },
                displayName: "Sharp Shadow",
                description: "Contains a forbidden spell that transforms shadows into deadly weapons. When using Shadow Dash, the bearer's body will sharpen, slice through foes, and surge forward faster.",
                notchCost: 2,
                fallbackTint: new Color(0.28f, 0.24f, 0.42f),
                enumId: ShadeCharmId.SharpShadow,
                iconName: "shade_charm_sharp_shadow"));

            // Purely additive: the nail swings as it always does and a beam goes with it. The
            // health condition lives with the beam, in ShadeController.Slash, so the charm is just
            // a switch - an update hook re-deciding it every frame is what used to swap the whole
            // moveset out from under the swing.
            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.GrubberflysElegy),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetGrubberflyElegyEquipped(true),
                    OnRemoved = ctx => ctx.Controller?.SetGrubberflyElegyEquipped(false)
                },
                displayName: "Grubberfly's Elegy",
                description: "Calls upon the gratitude of every rescued grub. While at full health, the bearer's nail sends out a wave of energy with every strike.",
                notchCost: 3,
                fallbackTint: new Color(0.50f, 0.68f, 0.94f),
                enumId: ShadeCharmId.GrubberflysElegy,
                iconName: "shade_charm_grubberflys_elegy"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.HeavyBlow),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.MultiplyNailKnockback(1.4f),
                    OnRemoved = ctx => ctx.Controller?.MultiplyNailKnockback(1f / 1.4f)
                },
                displayName: "Heavy Blow",
                description: "Embues the shade's nail with tremendous force, sending foes staggering further with every strike.",
                notchCost: 2,
                fallbackTint: new Color(0.56f, 0.39f, 0.23f),
                enumId: ShadeCharmId.HeavyBlow,
                iconName: "shade_charm_heavy_blow"));

            definitions.Add(new ShadeCharmDefinition(
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

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.LifebloodHeart),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddLifebloodBonus(2),
                    OnRemoved = ctx => ctx.Controller?.AddLifebloodBonus(-2)
                },
                displayName: "Lifeblood Heart",
                description: "The shade grows new lifeblood nodes, granting extra vitality that must be renewed at benches.",
                notchCost: 2,
                fallbackTint: new Color(0.35f, 0.73f, 0.88f),
                enumId: ShadeCharmId.LifebloodHeart,
                iconName: "shade_charm_lifeblood_heart"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.LifebloodCore),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.AddLifebloodBonus(4),
                    OnRemoved = ctx => ctx.Controller?.AddLifebloodBonus(-4)
                },
                displayName: "Lifeblood Core",
                description: "A massive core of lifeblood that courses through the shade, dramatically increasing temporary vitality.",
                notchCost: 4,
                fallbackTint: new Color(0.26f, 0.62f, 0.84f),
                enumId: ShadeCharmId.LifebloodCore,
                iconName: "shade_charm_lifeblood_core"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.JonisBlessing),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx =>
                    {
                        ctx.Controller?.SetFocusHealingDisabled(true);
                        ctx.Controller?.SetJonisBlessingActive(true);
                    },
                    OnRemoved = ctx =>
                    {
                        ctx.Controller?.SetFocusHealingDisabled(false);
                        ctx.Controller?.SetJonisBlessingActive(false);
                    }
                },
                displayName: "Joni's Blessing",
                description: "Blesses the shade with vast lifeblood reserves. Focus can no longer heal, but the companion's vitality surges.",
                notchCost: 4,
                fallbackTint: new Color(0.58f, 0.38f, 0.72f),
                enumId: ShadeCharmId.JonisBlessing,
                iconName: "shade_charm_jonis_blessing"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Hiveblood),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx =>
                    {
                        _hivebloodTimer = 0f;
                        _hivebloodPendingMaskRestore = false;
                    },
                    OnRemoved = ctx =>
                    {
                        _hivebloodTimer = 0f;
                        _hivebloodPendingMaskRestore = false;
                        ctx.Controller?.ResetHivebloodLifebloodRequest();
                    },
                    OnShadeDamaged = (ctx, evt) =>
                    {
                        if (evt.ActualDamage > 0 && !evt.WasPrevented)
                        {
                            _hivebloodTimer = 0f;
                            var controller = ctx.Controller;
                            if (controller != null)
                            {
                                _hivebloodPendingMaskRestore = controller.GetCurrentNormalHP() < controller.GetMaxNormalHP();
                            }
                            else
                            {
                                _hivebloodPendingMaskRestore = false;
                            }
                        }
                    },
                    OnUpdate = (ctx, delta) =>
                    {
                        var controller = ctx.Controller;
                        if (controller == null)
                        {
                            return;
                        }

                        int currentNormal = controller.GetCurrentNormalHP();
                        int maxNormal = controller.GetMaxNormalHP();
                        bool missingNormal = currentNormal < maxNormal;

                        if (!missingNormal)
                        {
                            _hivebloodPendingMaskRestore = false;
                        }

                        bool shouldRestoreNormal = _hivebloodPendingMaskRestore && missingNormal;
                        bool pendingLifeblood = controller.ShouldHivebloodRestoreLifeblood();

                        if (!shouldRestoreNormal && !pendingLifeblood)
                        {
                            _hivebloodTimer = 0f;
                            return;
                        }

                        _hivebloodTimer = Mathf.Min(
                            HivebloodRegenDurationSeconds,
                            _hivebloodTimer + Mathf.Max(0f, delta));

                        if (_hivebloodTimer >= HivebloodRegenDurationSeconds)
                        {
                            _hivebloodTimer = 0f;
                            if (shouldRestoreNormal)
                            {
                                int before = currentNormal;
                                controller.ReviveToAtLeast(before + 1);
                                if (controller.GetCurrentNormalHP() > before)
                                {
                                    _hivebloodPendingMaskRestore = false;
                                }
                            }
                            else if (pendingLifeblood && controller.TryRestoreLifeblood(1))
                            {
                                controller.ResetHivebloodLifebloodRequest();
                            }
                        }
                    }
                },
                displayName: "Hiveblood",
                description: "Honeyed lifeblood seeps through the shade, knitting wounds back together if it can avoid harm for a short while.",
                notchCost: 4,
                fallbackTint: new Color(0.96f, 0.76f, 0.32f),
                enumId: ShadeCharmId.Hiveblood,
                iconName: "shade_charm_hiveblood"));

            definitions.Add(new ShadeCharmDefinition(
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
                description: "Holy charm symbolising a union between higher beings. The bearer will slowly absorb the limitless SOUL contained within.",
                notchCost: 5,
                fallbackTint: new Color(0.92f, 0.91f, 0.75f),
                enumId: ShadeCharmId.Kingsoul,
                iconName: "shade_charm_kingsoul"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.VoidHeart),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetVoidHeartEvadeActive(true),
                    OnRemoved = ctx => ctx.Controller?.SetVoidHeartEvadeActive(false)
                },
                displayName: "Void Heart",
                description: "The Abyss calls to its lord, but once more, for the sake of an idea instilled, a Vessel defies its nature. The suffusion of abyss allows the Shade to avoid damage while evading.",
                notchCost: 0,
                fallbackTint: new Color(0.32f, 0.32f, 0.42f),
                enumId: ShadeCharmId.VoidHeart,
                iconName: "shade_charm_void_heart"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Weaversong),
                hooks: new ShadeCharmHooks
                {
                    // Exactly 3, always 3: the weaverlings carry a fixed number rather than a
                    // share of the nail, so the difficulty multiplier is deliberately not applied.
                    OnApplied = ctx => ShadeCharmSummons.Spawn(
                        ctx.Controller, ShadeCharmId.Weaversong,
                        count: 3, damage: 3, orbitRadius: 1.9f, seekRange: 9f,
                        scaleWithDamageMultiplier: false, groundBound: true, wanders: true),
                    OnRemoved = ctx => ShadeCharmSummons.Dismiss(ctx.Controller, ShadeCharmId.Weaversong)
                },
                displayName: "Weaversong",
                description: "Contains the lingering souls of a departed tribe of weavers. Summons weaverlings that scurry along at the bearer's heel and set upon nearby foes.",
                notchCost: 2,
                fallbackTint: new Color(0.45f, 0.36f, 0.62f),
                enumId: ShadeCharmId.Weaversong,
                iconName: "shade_charm_weaversongcharmgrimmsilkweaver"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.DefendersCrest),
                hooks: new ShadeCharmHooks
                {
                    OnUpdate = (ctx, delta) =>
                    {
                        var controller = ctx.Controller;
                        if (controller == null)
                            return;

                        // A cloud every 0.75s that stands for 1.1s and bites every 0.3s, so two
                        // overlap briefly - which is what makes it a cloud rather than a pulse.
                        if (ShadeCharmSummons.TickSpawnTimer(controller, ShadeCharmId.DefendersCrest, delta, 0.75f))
                            // Half the radius it had and a fifth of the opacity: at full strength
                            // the borrowed cloud filled the screen and buried everything behind it.
                            controller.SpawnCharmDamageBurst(
                                radius: 2.1f, damage: 3, lifeSeconds: 1.1f, hitIntervalSeconds: 0.3f,
                                effectPrefab: LegacyoftheAbyss.Shade.Knight.KnightEffects.DungCloud,
                                effectScale: 0.65f,
                                effectAlpha: 0.2f);
                    }
                },
                displayName: "Defender's Crest",
                description: "Crest of a proud knight. The bearer is wreathed in a cloud of noxious spores that damages foes that draw near. The smell is quite terrible.",
                notchCost: 1,
                fallbackTint: new Color(0.52f, 0.58f, 0.28f),
                enumId: ShadeCharmId.DefendersCrest,
                iconName: "shade_charm_defenderscrestcharmdungdef"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Flukenest),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetFlukenestEnabled(true),
                    OnRemoved = ctx => ctx.Controller?.SetFlukenestEnabled(false)
                },
                displayName: "Flukenest",
                description: "Contains the young of a parasitic creature. Changes the Vengeful Spirit spell, causing the bearer to launch a cluster of volatile flukes instead.",
                notchCost: 3,
                fallbackTint: new Color(0.72f, 0.38f, 0.44f),
                enumId: ShadeCharmId.Flukenest,
                iconName: "shade_charm_flukenestcharmfluke"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.SporeShroom),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetSporeShroomEnabled(true),
                    OnRemoved = ctx => ctx.Controller?.SetSporeShroomEnabled(false)
                },
                displayName: "Spore Shroom",
                description: "Formed from the flesh of a fungal creature. When the bearer focuses SOUL, a cloud of corrosive spores bursts forth to harm any foe that lingers.",
                notchCost: 1,
                fallbackTint: new Color(0.63f, 0.66f, 0.42f),
                enumId: ShadeCharmId.SporeShroom,
                iconName: "shade_charm_sporeshroomcharmfungus"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.ThornsOfAgony),
                hooks: new ShadeCharmHooks
                {
                    OnShadeDamaged = (ctx, evt) =>
                    {
                        // Only a hit that actually landed retaliates - a shielded or evaded hit
                        // never hurt the bearer, so there is nothing to answer for.
                        if (evt.WasPrevented || evt.ActualDamage <= 0)
                            return;

                        var controller = ctx.Controller;
                        if (controller == null)
                            return;

                        // Exactly one nail slash. Taken from the same figure the nail uses, which
                        // already carries the difficulty multiplier - hence not applying it twice.
                        // The vines are 6 frames at 20fps, so the volume stands for as long as
                        // they are drawn rather than the other way round.
                        controller.SpawnCharmDamageBurst(
                            radius: 3.2f,
                            damage: controller.NailSlashDamage,
                            lifeSeconds: 0.3f,
                            applyDamageMultiplier: false,
                            effectClip: LegacyoftheAbyss.Shade.Knight.KnightEffects.ThornAttackClip,
                            effectClipFps: LegacyoftheAbyss.Shade.Knight.KnightEffects.ThornAttackFps);
                    }
                },
                displayName: "Thorns of Agony",
                description: "Bramble-shaped charm containing the memory of pain. When the bearer suffers damage, thorny vines burst out and lash the foes surrounding them.",
                notchCost: 1,
                fallbackTint: new Color(0.44f, 0.60f, 0.34f),
                enumId: ShadeCharmId.ThornsOfAgony,
                iconName: "shade_charm_thornsofagony0000charmthorncounter"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.GlowingWomb),
                hooks: new ShadeCharmHooks
                {
                    OnUpdate = (ctx, delta) =>
                    {
                        var controller = ctx.Controller;
                        if (controller == null)
                            return;

                        // Costs SOUL to birth one, as it costs the Knight in Hallownest.
                        if (!ShadeCharmSummons.TickSpawnTimer(controller, ShadeCharmId.GlowingWomb, delta, 4f))
                            return;

                        if (controller.GetShadeSoul() < 8)
                            return;

                        controller.GainShadeSoul(-8);
                        ShadeCharmSummons.AddOne(
                            controller, ShadeCharmId.GlowingWomb,
                            maxAlive: 4, damage: 9, seekRange: 12f,
                            lifeSeconds: 12f, expiresOnHit: true,
                            scaleWithDamageMultiplier: false);
                    },
                    OnRemoved = ctx => ShadeCharmSummons.Dismiss(ctx.Controller, ShadeCharmId.GlowingWomb)
                },
                displayName: "Glowing Womb",
                description: "Forms a bond between the SOUL of the bearer and the void within. Consumes SOUL to birth fragile hatchlings that seek out foes and burst upon them.",
                notchCost: 2,
                fallbackTint: new Color(0.78f, 0.80f, 0.55f),
                enumId: ShadeCharmId.GlowingWomb,
                iconName: "shade_charm_glowingwomb0009charmhatchling"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.GatheringSwarm),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ctx.Controller?.SetGatheringSwarmEnabled(true),
                    OnRemoved = ctx => ctx.Controller?.SetGatheringSwarmEnabled(false)
                },
                displayName: "Gathering Swarm",
                description: "A swarm of tiny creatures that follow the bearer, gathering up loose rosaries that would otherwise be left behind.",
                notchCost: 1,
                fallbackTint: new Color(0.86f, 0.78f, 0.44f),
                enumId: ShadeCharmId.GatheringSwarm,
                iconName: "shade_charm_gatheringswarmcharmsprite02"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Grimmchild),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ShadeCharmSummons.SpawnGrimmchild(ctx.Controller),
                    OnRemoved = ctx => ShadeCharmSummons.Dismiss(ctx.Controller, ShadeCharmId.Grimmchild)
                },
                displayName: "Grimmchild",
                description: "A child of the Nightmare's Heart, held close. It rides at the bearer's shoulder and spits fire at any foe that draws near, hungry for the flames within them.",
                notchCost: 2,
                fallbackTint: new Color(0.76f, 0.26f, 0.30f),
                enumId: ShadeCharmId.Grimmchild,
                iconName: "shade_charm_grimmchildcharmgrimmkin04"));

            // Inert on purpose: the companion has no Dream Nail, so the SOUL bonuses this used to
            // grant were a stand-in for a mechanic that does not exist and did not match the
            // charm. Equippable and collectable, but it does nothing until the Dream Nail lands.
            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.DreamWielder),
                displayName: "Dream Wielder",
                description: "Transient charm created for those who wield the Dream Nail and collect Essence. Allows the bearer to charge the Dream Nail faster and collect more SOUL when striking foes.\n\nThis charm will be implemented at a later date, when the mechanics related to it are added to the mod.",
                notchCost: 1,
                fallbackTint: new Color(0.60f, 0.72f, 0.88f),
                enumId: ShadeCharmId.DreamWielder,
                iconName: "shade_charm_dreamwielder"));

            definitions.Add(new ShadeCharmDefinition(
                nameof(ShadeCharmId.Dreamshield),
                hooks: new ShadeCharmHooks
                {
                    OnApplied = ctx => ShadeCharmSummons.Spawn(
                        ctx.Controller, ShadeCharmId.Dreamshield,
                        count: 1, damage: 10, orbitRadius: 0.6f, seekRange: 0f,
                        faceOutward: true, orbitVerticalScale: 1f,
                        orbitSpeed: 60f, visualScale: 0.6f),
                    OnRemoved = ctx => ShadeCharmSummons.Dismiss(ctx.Controller, ShadeCharmId.Dreamshield)
                },
                displayName: "Dreamshield",
                description: "A shield of dream-stuff that circles the bearer, striking foes it passes through. It keeps its slow orbit rather than seeking anything out.",
                notchCost: 3,
                fallbackTint: new Color(0.70f, 0.84f, 0.90f),
                enumId: ShadeCharmId.Dreamshield,
                iconName: "shade_charm_dreamshieldcharmgrimmmarkothshield"));

            return definitions;
        }
    }
}
