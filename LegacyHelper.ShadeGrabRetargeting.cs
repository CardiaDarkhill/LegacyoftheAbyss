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
    /// The sibling of <see cref="EnemyAiRetargeting"/>: that one redirects where an enemy <i>goes</i>,
    /// this one decides who an attack <i>lands on</i>. Lace's cross slash is the shape to keep in mind
    /// - it marks a circular area, teleports the hero to the centre of it, multi-hits and slams, and
    /// every effect after the area check is aimed at <c>HeroController</c> by name regardless of who
    /// tripped it.
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
        /// Actions that ask <c>HeroController.CanBeGrabbed</c> - its only two callers anywhere. No
        /// boss observed so far routes a grab through them, but they share the refusal path with the
        /// damage gates and cost little to cover.
        /// </summary>
        private static readonly HashSet<string> GrabGateActionNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "CanHeroBeGrabbed",
            "CanHeroBeGrabbedV2"
        };

        private static MethodInfo s_finishMethod;

        // --- the rule ---------------------------------------------------------------------------

        /// <summary>
        /// Who is standing in an attack, asked separately for each of them. Nothing here is
        /// time-based: expressing "the Shade was hit" as a stretch of immunity for Hornet cannot
        /// represent both of them in the same circle, and spares her hits she should have taken.
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
            /// Whether Hornet's side of the reading means anything - a hurtbox was found, switched on,
            /// and testable against the attack. Without it "measured, and outside" and "not measurable
            /// at all" arrive as the same <c>false</c>, which turns a failed reading into grounds to
            /// take a hit away from her - the one direction this must never fail in.
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

            var colliders = attack.GetComponentsInChildren<Collider2D>(false);

            if (s_attackColliders.Count > 64)
            {
                s_attackColliders.Clear();
            }

            s_attackColliders[attack] = new KeyValuePair<int, Collider2D[]>(frame, colliders);
            return colliders;
        }

        /// <summary>
        /// Whether <paramref name="victim"/> is geometrically inside one of the attack's hitboxes.
        /// Must stay <c>Collider2D.Distance</c>, which is pure geometry: <c>IsTouching</c> consults
        /// the layer collision matrix, and the Shade's body sits on Default while these hitboxes do
        /// not, so it reports "not touching" wherever the Shade stands.
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

                if (collider.Distance(victim).isOverlapped)
                {
                    matched = collider.name;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Whether a collider is part of the attack rather than part of the boss noticing you. A boss
        /// carries arena-sized detection volumes - battle and alert ranges - as the same kind of
        /// trigger on the same layer as its hitboxes, and counting those reads Hornet as "inside the
        /// attack" from anywhere in the room. <c>TrackTriggerObjects</c> marks a detector and a hitbox
        /// is always a trigger, so the two tests separate the sets cleanly.
        /// </summary>
        private static bool IsAttackHitbox(Collider2D collider)
        {
            return collider != null
                && collider.enabled
                && collider.isTrigger
                && collider.GetComponent<TrackTriggerObjects>() == null;
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
        /// Whether the damage names Hornet herself, or something she hangs off, as its source. Settled
        /// here rather than measured, because occupancy cannot answer it: <see cref="IsInsideAttack"/>
        /// skips the victim's own collider, so when the attack is her own hierarchy she reads as
        /// outside it every time and a Shade stood on her takes the hit. Reached by debug-menu damage
        /// rather than by anything the game itself does.
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
        /// Measures occupancy of one attack. <paramref name="attack"/> must be the object actually
        /// delivering the effect and nothing above it: widening to the boss's <c>HealthManager</c>
        /// puts every hitbox the boss owns in one bucket, and every reading comes back <i>both
        /// inside</i>.
        /// </summary>
        private static Occupancy MeasureOccupancy(GameObject attack)
        {
            var hornetHurtbox = ResolveHornetHurtbox();

            // A hurtbox that is missing or switched off cannot place her either way. Off only while
            // she is dying (HeroBoxOff has no other caller), but it keeps a failed reading from being
            // spent on her behalf.
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
        /// Hands a hit to the Shade, when the Shade has no other way to receive it. Most attacks reach
        /// it on their own through the <see cref="DamageHero"/> on the collider it overlaps, and a
        /// second copy here would hurt it twice - so those are left alone.
        /// <para>
        /// The ones that need this carry no damage component on the hitbox at all, because the FSM
        /// calls <c>HeroController</c> directly. Lace's cross slash is one, and without this it cannot
        /// touch the Shade at all.
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
        /// A gate action's refusal branch: its public <c>FsmEvent</c> field named "cannot"
        /// something. Internal so <c>GameApiContract</c> asserts this lookup rather than a copy of
        /// it - a test that resolves the field its own way passes while the real one is dead.
        /// </summary>
        internal static FieldInfo FindRefusalEventField(FieldInfo[] fields)
        {
            return fields.FirstOrDefault(f =>
                f.FieldType == typeof(FsmEvent) &&
                f.Name.Contains("cannot", StringComparison.OrdinalIgnoreCase));
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

                var refuseField = FindRefusalEventField(fields);
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
        /// Refuses at the gate rather than suppressing the damage call: that call is one action in a
        /// state which also broadcasts <c>HERO DAMAGED</c> and <c>WOUND START</c>, so suppressing it
        /// alone costs Hornet no health but still plays the whole hit reaction.
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
                foreach (var type in FsmActionTypes())
                {
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
