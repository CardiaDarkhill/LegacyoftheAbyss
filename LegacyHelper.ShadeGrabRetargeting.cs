#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using HutongGames.PlayMaker;
using UnityEngine;

public partial class LegacyHelper
{
    /// <summary>
    /// Makes an attack land on whoever is standing in it, Hornet and the Shade alike.
    /// <para>
    /// The sibling of <see cref="EnemyAiRetargeting"/>, and the second half of the same idea. That one
    /// redirects where an enemy <i>goes</i>; this one decides who an attack <i>lands on</i> once it
    /// gets there. Lace's cross slash is the case it was written for: it marks a circular area and, if
    /// anything is inside it, teleports the hero to the centre, multi-hits, then slams. The area check
    /// does not care who tripped it, and every effect after it is aimed at <c>HeroController</c> by
    /// name - so with the Shade in the circle, Hornet was dragged thirteen units across the room into
    /// an attack she was nowhere near.
    /// </para>
    /// <para>
    /// The question asked is occupancy, not preference: is Hornet in this attack, and is the Shade,
    /// each answered on its own against the attack's own colliders. Both of them in the same circle is
    /// an ordinary case rather than a tie to be broken.
    /// </para>
    /// <para>
    /// Interception happens at the game's own "may I hurt the hero?" gates rather than at the damage
    /// call, because the damage is only one action in the state that runs it - the rest broadcasts the
    /// hit and plays the recoil. Refusing the gate keeps the FSM out of that state entirely, which is
    /// a supported answer: these actions exist to be told no and every caller has a branch for it.
    /// </para>
    /// <para>
    /// Turn the whole behaviour off with <c>shadeBossAttackSharingEnabled</c>. It is separate from
    /// <c>shadeEnemyTargetingEnabled</c> because it reaches into hero damage, so it is the first thing
    /// worth switching off if a boss misbehaves; the Shade is still chased either way.
    /// </para>
    /// </summary>
    internal static class ShadeGrabRetargeting
    {
        /// <summary>
        /// Actions that ask whether the hero may be damaged, immediately before damaging them.
        /// Lace's <c>Multihitter</c> asks this one, which is what makes it the useful hook.
        /// </summary>
        private static readonly HashSet<string> DamageGateActionNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "CanHeroTakeDamage",
            "CanHeroTakeDamageIgnoreInvul"
        };

        /// <summary>
        /// Actions that ask <c>HeroController.CanBeGrabbed</c> - its only two callers anywhere.
        /// <para>
        /// Kept for grabs that route through it, though no boss observed so far actually does: Lace's
        /// cross slash goes through the damage gate above instead. Both sets are cheap to patch and
        /// share the same refusal, so covering both costs little.
        /// </para>
        /// </summary>
        private static readonly HashSet<string> GrabGateActionNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "CanHeroBeGrabbed",
            "CanHeroBeGrabbedV2"
        };

        private static MethodInfo s_finishMethod;

        // --- the rule ---------------------------------------------------------------------------

        /// <summary>
        /// Who is standing in an attack, asked separately for each of them.
        /// <para>
        /// An earlier version keyed off which of the two the boss was fighting and swapped one for the
        /// other, expressing "the Shade was hit" as a stretch of immunity for Hornet. That cannot
        /// represent both of them standing in the same circle, and spared her hits she should have
        /// taken. Nothing here is time-based; the only question is who is in the attack.
        /// </para>
        /// </summary>
        internal readonly struct Occupancy
        {
            internal Occupancy(bool hornetInside, bool shadeInside, bool hornetMeasurable = true)
            {
                HornetInside = hornetInside;
                ShadeInside = shadeInside;
                HornetMeasurable = hornetMeasurable;
            }

            internal bool HornetInside { get; }

            internal bool ShadeInside { get; }

            /// <summary>
            /// Whether Hornet's side of the reading means anything - that a hurtbox was found, was
            /// switched on, and could be tested against the attack.
            /// <para>
            /// Without this the two unlike answers "she was measured, and she is outside it" and
            /// "she could not be measured at all" arrive as the same <c>false</c>, and every caller
            /// below reads the second as the first. That reads a failure to measure as grounds to
            /// take a hit away from her, which is the one direction this must never fail in.
            /// </para>
            /// </summary>
            internal bool HornetMeasurable { get; }

            /// <summary>
            /// Hornet is known to be out of this attack, as opposed to merely not known to be in it.
            /// Everything that acts on her behalf asks this rather than <c>!HornetInside</c>.
            /// </summary>
            internal bool HornetOutside => HornetMeasurable && !HornetInside;

            internal string Describe()
            {
                if (!HornetMeasurable)
                {
                    return ShadeInside ? "Shade inside, Hornet unmeasurable" : "Hornet unmeasurable";
                }

                return HornetInside
                    ? (ShadeInside ? "both inside" : "Hornet inside")
                    : (ShadeInside ? "Shade inside" : "neither inside");
            }
        }

        /// <summary>
        /// Whether an effect that can only have one subject should take the Shade. It breaks the tie
        /// towards Hornet: being grabbed while standing in a grab is correct, and so is being grabbed
        /// when there was no reading of her to go on.
        /// </summary>
        internal static bool ShouldMoveShadeInstead(Occupancy occupancy)
            => occupancy.ShadeInside && occupancy.HornetOutside;

        /// <summary>
        /// Whether the Shade takes this hit. Independent of Hornet entirely - if the Shade is in the
        /// attack it is hit by it, whether or not she is standing there too.
        /// </summary>
        internal static bool ShouldShadeTakeHit(Occupancy occupancy) => occupancy.ShadeInside;

        /// <summary>
        /// Whether Hornet's own damage should be skipped. Only when she is known to be out of the
        /// attack - never as a consequence of the Shade having been hit, and never off a reading that
        /// could not be taken.
        /// </summary>
        internal static bool ShouldSpareHornet(Occupancy occupancy) => occupancy.HornetOutside;

        // --- measuring it -----------------------------------------------------------------------

        /// <summary>Colliders under one attack, recomputed at most once per frame.</summary>
        private static readonly Dictionary<GameObject, KeyValuePair<int, Collider2D[]>> s_attackColliders = new();

        /// <summary>
        /// Which hitbox produced the last non-empty reading, so a report can show whether the right
        /// collider was consulted rather than only the verdict.
        /// </summary>
        private static string s_lastHitboxes;

        /// <summary>
        /// The context handed from a gate to the damage call immediately after it.
        /// <c>TakeQuickDamage</c> names no source at all, so without this there is no way to know
        /// which attack a hit came from - but it is scoped to the frame the gate ran on rather than to
        /// a duration, so it cannot become a stretch of immunity.
        /// </summary>
        private static GameObject s_damageContextAttack;
        private static Occupancy s_damageContextOccupancy;
        private static int s_damageContextFrame = -1;

        private static Collider2D[] GetAttackColliders(GameObject attack)
        {
            if (attack == null)
            {
                return Array.Empty<Collider2D>();
            }

            int frame = Time.frameCount;
            if (s_attackColliders.TryGetValue(attack, out var cached) && cached.Key == frame)
            {
                return cached.Value;
            }

            Collider2D[] colliders;
            try { colliders = attack.GetComponentsInChildren<Collider2D>(false); }
            catch { colliders = Array.Empty<Collider2D>(); }

            if (s_attackColliders.Count > 64)
            {
                s_attackColliders.Clear();
            }

            s_attackColliders[attack] = new KeyValuePair<int, Collider2D[]>(frame, colliders);
            return colliders;
        }

        /// <summary>
        /// Whether <paramref name="victim"/> is geometrically inside one of the attack's hitboxes.
        /// <para>
        /// <c>Collider2D.Distance</c> rather than <c>IsTouching</c>, and the difference is a bug it
        /// replaced: <c>IsTouching</c> answers "are these in contact <i>as far as the physics system
        /// is concerned</i>", which consults the layer collision matrix. The Shade's body sits on
        /// Default and these hitboxes elsewhere, pairs that do not interact, so it reported "not
        /// touching" no matter where the Shade stood. <c>Distance</c> is pure geometry.
        /// </para>
        /// </summary>
        private static bool IsInsideAttack(GameObject attack, Collider2D victim, out string matched)
        {
            matched = null;

            if (attack == null || victim == null || !victim.enabled)
            {
                return false;
            }

            foreach (var collider in GetAttackColliders(attack))
            {
                if (!IsAttackHitbox(collider) || ReferenceEquals(collider, victim))
                {
                    continue;
                }

                try
                {
                    if (collider.Distance(victim).isOverlapped)
                    {
                        matched = collider.name;
                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// Whether a collider is part of the attack rather than part of the boss noticing you.
        /// <para>
        /// A boss carries arena-sized detection volumes - battle ranges, alert ranges - alongside its
        /// hitboxes, and they are the same kind of trigger on the same layer. Counting those made
        /// Hornet read as "inside the attack" from anywhere in the room. <c>TrackTriggerObjects</c>
        /// marks a volume as a detector, and a hitbox is always a trigger, so between them the two
        /// tests separate the sets cleanly.
        /// </para>
        /// </summary>
        private static bool IsAttackHitbox(Collider2D collider)
        {
            if (collider == null || !collider.enabled || !collider.isTrigger)
            {
                return false;
            }

            try
            {
                if (collider.GetComponent<TrackTriggerObjects>() != null)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static Collider2D ResolveHornetHurtbox()
        {
            try
            {
                var hero = HeroController.UnsafeInstance;
                var box = hero != null ? hero.heroBox : null;
                return box != null ? box.GetComponent<Collider2D>() : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Whether the damage names Hornet herself, or something she hangs off, as its source.
        /// <para>
        /// Occupancy cannot answer for an attack that is Hornet: <see cref="IsInsideAttack"/> skips
        /// the victim's own collider, and when the attack is her hierarchy the only hitbox that could
        /// have matched her <i>is</i> that collider - so she reads as outside her own attack every
        /// time, and any Shade stood on her takes the hit instead. Damage sourced at her is hers by
        /// definition, so it is settled here rather than measured.
        /// </para>
        /// <para>
        /// Nothing in the game damages Hornet this way; a debug menu's "hurt me" does, which is how it
        /// was found, and that is reason enough for the case to have an answer of its own.
        /// </para>
        /// </summary>
        private static bool IsHornetsOwn(GameObject source)
        {
            if (source == null)
            {
                return false;
            }

            try
            {
                var hero = HeroController.UnsafeInstance;
                var heroTransform = hero != null ? hero.transform : null;
                if (heroTransform == null)
                {
                    return false;
                }

                var sourceTransform = source.transform;
                return sourceTransform != null
                    && (sourceTransform.IsChildOf(heroTransform) || heroTransform.IsChildOf(sourceTransform));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Measures occupancy of one attack. <paramref name="attack"/> is the object actually
        /// delivering the effect and nothing above it - an earlier version widened this to the boss's
        /// <c>HealthManager</c>, which put every hitbox the boss owns into one bucket, so anyone in
        /// melee read as "inside the attack" and every reading came back <i>both inside</i>.
        /// </summary>
        private static Occupancy MeasureOccupancy(GameObject attack)
        {
            var hornetHurtbox = ResolveHornetHurtbox();

            // A hurtbox that is missing or switched off cannot place her either way. It is off only
            // while she is dying - HeroBoxOff has no other caller - so this is a guard rather than a
            // path anything ordinarily takes, but it is the guard that keeps a failed reading from
            // being spent on her behalf.
            bool hornetMeasurable = hornetHurtbox != null && hornetHurtbox.enabled;
            bool hornetInside = false;
            string hornetHitbox = null;
            if (hornetMeasurable)
            {
                hornetInside = IsInsideAttack(attack, hornetHurtbox, out hornetHitbox);
            }

            bool shadeInside = false;
            string shadeHitbox = null;
            try
            {
                if (TryGetShadeController(out var shade) && shade != null && !shade.IsInactive)
                {
                    shadeInside = IsInsideAttack(attack, shade.BodyCollider, out shadeHitbox);
                }
            }
            catch
            {
            }

            s_lastHitboxes = hornetInside || shadeInside
                ? FormattableString.Invariant($"hornet:{hornetHitbox ?? "-"} shade:{shadeHitbox ?? "-"}")
                : null;

            return new Occupancy(hornetInside, shadeInside, hornetMeasurable);
        }

        private static void NoteDamageContext(GameObject attack, Occupancy occupancy)
        {
            s_damageContextAttack = attack;
            s_damageContextOccupancy = occupancy;
            s_damageContextFrame = Time.frameCount;
        }

        // --- applying it ------------------------------------------------------------------------

        /// <summary>
        /// Hands a hit to the Shade, when the Shade has no other way to receive it.
        /// <para>
        /// Most attacks reach the Shade on their own: it finds the <see cref="DamageHero"/> on the
        /// collider it is overlapping and takes the hit through its ordinary damage path. Applying a
        /// second copy here would hurt it twice, so those are left alone.
        /// </para>
        /// <para>
        /// The ones that need this carry no damage component on the hitbox at all, because the FSM
        /// damages the hero by calling <c>HeroController</c> directly. Lace's cross slash is one, and
        /// the Shade had only ever been receiving it by accident - through a parent walk-up that
        /// attributed the boss's body contact to every child trigger. Correcting that attribution left
        /// the Shade completely immune to the attack until this was added.
        /// </para>
        /// </summary>
        private static string GiveTheShadeItsHit(GameObject attack, int knownAmount = 0)
        {
            try
            {
                if (ShadeController.CarriesItsOwnDamage(attack))
                {
                    return "the Shade takes this one through its own collision";
                }

                if (!TryGetShadeController(out var shade) || shade == null || shade.IsInactive)
                {
                    return "no Shade to give it to";
                }

                int amount = knownAmount > 0 ? knownAmount : ShadeController.ResolveAttackDamage(attack);
                shade.TakeAttackHit(amount, attack != null ? attack.name : "attack");

                return FormattableString.Invariant($"gave the Shade {amount}");
            }
            catch
            {
                return "failed to give the Shade its hit";
            }
        }

        /// <summary>
        /// Sends a gate action down its "cannot" branch and closes it out. Shared by both gate
        /// families, which have the same shape: an event target, a refusal event, and a <c>Finish</c>
        /// that has to be called because the action's own body never runs.
        /// </summary>
        private static bool TryRefuse(FsmStateAction action, out string refusedVia)
        {
            refusedVia = null;

            try
            {
                var fsm = action?.Fsm;
                if (fsm == null)
                {
                    return false;
                }

                var fields = action.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

                var refuseField = fields.FirstOrDefault(f =>
                    f.FieldType == typeof(FsmEvent) &&
                    f.Name.IndexOf("cannot", StringComparison.OrdinalIgnoreCase) >= 0);
                if (refuseField?.GetValue(action) is not FsmEvent refuseEvent || refuseEvent == null)
                {
                    return false;
                }

                var targetField = fields.FirstOrDefault(f => f.FieldType == typeof(FsmEventTarget));
                if (targetField?.GetValue(action) is FsmEventTarget target && target != null)
                {
                    fsm.Event(target, refuseEvent);
                }
                else
                {
                    fsm.Event(refuseEvent);
                }

                try { s_finishMethod?.Invoke(action, null); }
                catch { }

                refusedVia = refuseField.Name;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gives the Shade its share of an attack, and refuses the hero's when she is not in it.
        /// <para>
        /// Suppressing the damage call alone was not enough: it is one action in a state that also
        /// broadcasts <c>HERO DAMAGED</c> and <c>WOUND START</c>, so Hornet lost no health but still
        /// played the whole hit reaction. Refusing the gate keeps the FSM out of that state entirely.
        /// </para>
        /// </summary>
        private static bool GatePrefix(FsmStateAction __instance)
        {
            try
            {
                if (__instance == null || !ModConfig.Instance.shadeBossAttackSharingEnabled)
                {
                    return true;
                }

                var attack = ResolveAttackObject(__instance);
                if (IsHornetsOwn(attack))
                {
                    // Hers, and not a reading to be taken - see IsHornetsOwn.
                    return true;
                }

                var occupancy = MeasureOccupancy(attack);

                // Kept for TakeQuickDamage, which names no source of its own.
                NoteDamageContext(attack, occupancy);

                // The Shade's share is decided on its own, before anything about Hornet. Both of them
                // standing in the same attack is ordinary: she takes hers below, this is the Shade
                // taking its own.
                string shareNote = ShouldShadeTakeHit(occupancy)
                    ? GiveTheShadeItsHit(attack)
                    : "the Shade is not in this one";

                if (!ShouldSpareHornet(occupancy) || !ShouldShadeTakeHit(occupancy))
                {
                    // She is standing in it too, or neither of them is. Not ours to refuse.
                    return true;
                }

                if (!TryRefuse(__instance, out string refusedVia))
                {
                    // No branch to send it down; the damage suppression downstream is all we have.
                    return true;
                }

                LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
                    "hornet-spared",
                    attack != null ? attack.name : "attack",
                    FormattableString.Invariant(
                        $"{__instance.GetType().Name} refused via {refusedVia}; {occupancy.Describe()} [{s_lastHitboxes ?? "no hitbox matched"}]; no hit reaction on Hornet; {shareNote}"));

                return false;
            }
            catch
            {
            }

            return true;
        }

        /// <summary>
        /// Applies one hero-damage call to whoever is standing in the attack. Returns true only when
        /// Hornet is not in it, which is the one case where her own damage should be skipped.
        /// <para>
        /// The fallback rather than the main path: where a gate ran ahead of the damage it has usually
        /// refused already. This catches the calls that arrive with no gate in front of them.
        /// </para>
        /// </summary>
        internal static bool TryRedirectHeroDamage(GameObject source, int damageAmount, string entryPoint)
        {
            try
            {
                if (damageAmount <= 0 || !ModConfig.Instance.shadeBossAttackSharingEnabled)
                {
                    return false;
                }

                if (IsHornetsOwn(source))
                {
                    // Hers, and not a reading to be taken - see IsHornetsOwn.
                    return false;
                }

                GameObject attack;
                Occupancy occupancy;

                if (source != null)
                {
                    attack = source;
                    occupancy = MeasureOccupancy(attack);
                }
                else if (s_damageContextFrame == Time.frameCount && s_damageContextAttack != null)
                {
                    attack = s_damageContextAttack;
                    occupancy = s_damageContextOccupancy;
                }
                else
                {
                    // Anonymous damage with no gate ahead of it. Nothing to attribute it to, so it
                    // stays exactly as the game intended.
                    return false;
                }

                if (!ShouldSpareHornet(occupancy) || !ShouldShadeTakeHit(occupancy))
                {
                    return false;
                }

                string shareNote = GiveTheShadeItsHit(attack, damageAmount);

                LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
                    "hornet-spared",
                    attack != null ? attack.name : "attack",
                    FormattableString.Invariant(
                        $"{entryPoint} of {damageAmount}; {occupancy.Describe()} [{s_lastHitboxes ?? "no hitbox matched"}]; the Shade is in this one, Hornet is not; {shareNote}"));

                return true;
            }
            catch
            {
                return false;
            }
        }

        // --- installation -----------------------------------------------------------------------

        internal static void Apply(Harmony harmony)
        {
            if (harmony == null)
            {
                return;
            }

            try
            {
                if (!ModConfig.Instance.shadeEnemyTargetingEnabled || !ModConfig.Instance.shadeBossAttackSharingEnabled)
                {
                    LogInfo("Shade attack sharing: disabled by config, no methods patched");
                    return;
                }

                s_finishMethod ??= AccessTools.Method(typeof(FsmStateAction), "Finish");
                if (s_finishMethod == null)
                {
                    // A refusal that cannot close the action out would strand the FSM in that state,
                    // which is worse than not intercepting at all.
                    LogInfo("Shade attack sharing: FsmStateAction.Finish not found, no methods patched");
                    return;
                }

                var prefix = new HarmonyMethod(AccessTools.DeclaredMethod(typeof(ShadeGrabRetargeting), nameof(GatePrefix)));

                int patched = 0;
                int failed = 0;
                foreach (var method in GateMethods())
                {
                    try
                    {
                        harmony.Patch(method, prefix);
                        patched++;
                    }
                    catch
                    {
                        failed++;
                    }
                }

                LogInfo(FormattableString.Invariant(
                    $"Shade attack sharing: patched {patched} gate action(s), {failed} failed"));
            }
            catch (Exception ex)
            {
                try { LogInfo($"Shade attack sharing: could not be applied ({ex.GetType().Name}: {ex.Message})"); }
                catch { }
            }
        }

        /// <summary>
        /// The <c>OnEnter</c> of every gate action, damage and grab alike. Resolved by shape rather
        /// than named directly so an assembly we do not recognise leaves the feature switched off
        /// instead of throwing out of <c>PatchAll</c>.
        /// </summary>
        private static IEnumerable<MethodBase> GateMethods()
        {
            var methods = new List<MethodBase>();

            try
            {
                var assemblies = new HashSet<Assembly>
                {
                    typeof(FsmStateAction).Assembly,
                    typeof(HeroController).Assembly
                };

                foreach (var type in assemblies.SelectMany(SafeGetTypes))
                {
                    if (type == null || type.IsAbstract || !typeof(FsmStateAction).IsAssignableFrom(type))
                    {
                        continue;
                    }

                    if (!DamageGateActionNames.Contains(type.Name) && !GrabGateActionNames.Contains(type.Name))
                    {
                        continue;
                    }

                    var onEnter = AccessTools.DeclaredMethod(type, "OnEnter");
                    if (onEnter != null && !onEnter.IsAbstract)
                    {
                        methods.Add(onEnter);
                    }
                }
            }
            catch (Exception ex)
            {
                try { LogInfo($"Shade attack sharing: failed to enumerate gate actions ({ex.GetType().Name}: {ex.Message})"); }
                catch { }
            }

            return methods;
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }

        /// <summary>The object delivering the effect - the FSM's owner, and nothing above it.</summary>
        private static GameObject ResolveAttackObject(FsmStateAction action)
        {
            try
            {
                var owner = action.Fsm?.Owner;
                if (owner != null)
                {
                    return owner.gameObject;
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
#nullable restore
