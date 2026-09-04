#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GlobalEnums;

public partial class LegacyHelper
{
    /// <summary>
    /// Keeps the Shade out of SlideSurface's trigger volumes.
    /// <para>
    /// SlideSurface assumes the only thing that can enter it is Hornet. <c>OnTriggerEnter2D</c>
    /// unconditionally flips <c>isHeroInside</c> and bumps the static <c>_heroInsideCount</c>, then
    /// assigns <c>hc = collision.GetComponent&lt;HeroController&gt;()</c> and bails when that is
    /// null - so a non-Hornet entrant overwrites the cached hero reference with null while leaving
    /// the surface's "hero is here" bookkeeping switched on. The next follow tick calls
    /// <c>UpdateFacing()</c>, which dereferences <c>this.hc.cState</c>, and the frame dies with a
    /// NullReferenceException. <c>OnTriggerExit2D</c> has the mirror problem: the Shade leaving
    /// clears <c>isHeroInside</c> even though Hornet is still standing on the slide.
    /// </para>
    /// <para>
    /// The Shade floats with gravityScale 0 and has no business sliding, and the base game already
    /// intends to ignore non-hero entrants - it just does the ignoring too late. Dropping the
    /// callbacks before they touch any state is the whole fix.
    /// </para>
    /// </summary>
    [HarmonyPatch(typeof(SlideSurface))]
    private class SlideSurface_Triggers_IgnoreShade
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (string name in new[] { "OnTriggerEnter2D", "OnTriggerStay2D", "OnTriggerExit2D" })
            {
                var method = AccessTools.Method(typeof(SlideSurface), name, new[] { typeof(Collider2D) });
                if (method != null)
                {
                    yield return method;
                }
            }
        }

        private static bool Prefix(Collider2D collision)
        {
            try
            {
                if (collision != null && collision.GetComponentInParent<ShadeController>() != null)
                {
                    return false;
                }
            }
            catch
            {
            }

            return true;
        }
    }

    /// <summary>
    /// Keeps the Shade from being counted as an occupant of regions that ask "is the hero in here?".
    /// <para>
    /// The Shade carries a child "ShadeAggroProxy" that deliberately copies Hornet's layer and tag so
    /// enemies notice it, and <see cref="TrackTriggerObjects"/> filters entrants on exactly those two
    /// things - so the proxy is indistinguishable from Hornet to every region derived from it.
    /// </para>
    /// <para>
    /// The hook is <c>IsCounted</c> rather than the trigger callbacks on purpose. Updraft lift is not
    /// driven by <c>WindRegion</c> at all; it is an FSM that polls <c>CheckTrackTriggerCount</c> -&gt;
    /// <c>TrackTriggerObjects.InsideCount</c> and calls <c>HeroController.EnterUpdraft</c> /
    /// <c>ExitUpdraft</c>. <c>InsideCount</c> is the only thing that filters through <c>IsCounted</c>,
    /// so excluding the Shade here makes the FSM fire EXIT the moment Hornet leaves, wherever the
    /// Shade happens to be. The same getter backs <c>IsInside</c>, which the rest of the game uses for
    /// bench work ranges, breakable ranges, pickup triggers, camera shake, music, frost and driftfly
    /// dispersal - all of them "is Hornet here?" questions the Shade should not be answering.
    /// </para>
    /// <para>
    /// Crucially this leaves <c>insideGameObjects</c> and the <c>OnTrackTriggerEntered</c> callback
    /// untouched, so the aggro proxy still registers with every range. Do not filter at
    /// <c>OnTriggerEnter2D</c> instead: dropping Shade colliders there stops enemies noticing the
    /// Shade, and misses the updraft anyway, since the region types such a filter can see are not the
    /// ones the updraft uses.
    /// </para>
    /// </summary>
    [HarmonyPatch]
    internal static class TrackTriggerObjects_IsCounted_IgnoreShade
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            // IsCounted is virtual and TrackTriggerObjectsLineOfSight overrides it, so patching the
            // base alone would miss every line-of-sight range.
            foreach (var type in new[] { typeof(TrackTriggerObjects), typeof(TrackTriggerObjectsLineOfSight) })
            {
                var method = AccessTools.DeclaredMethod(type, "IsCounted", new[] { typeof(GameObject) });
                if (method != null)
                {
                    yield return method;
                }
            }
        }

        private static void Postfix(TrackTriggerObjects __instance, GameObject obj, ref bool __result)
        {
            try
            {
                if (!__result || __instance == null || obj == null)
                {
                    return;
                }

                if (CountsTheShade(__instance.GetType()))
                {
                    return;
                }

                if (obj.GetComponentInParent<ShadeController>() == null)
                {
                    return;
                }

                __result = false;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Ranges that must keep counting the Shade. Enemy aggro is the entire reason the proxy
        /// exists, and some enemy FSMs read their alert range through <c>CheckTrackTriggerCount</c>,
        /// so excluding the Shade there would make it invisible to them.
        /// </summary>
        internal static bool CountsTheShade(Type regionType)
            => regionType != null && typeof(AlertRange).IsAssignableFrom(regionType);
    }

    /// <summary>
    /// Lets particle hazards hit the Shade.
    /// <para>
    /// <c>ParticleDamageHero.Start</c> clears the particle system's trigger collider list and adds
    /// exactly one collider - Hornet's hero box - then damages her from <c>OnParticleTrigger</c>
    /// whenever any particle enters it. Nothing in that reaches the Shade: the "projectiles" are
    /// particles, so there is no collider overlap for <c>TryProcessDamageHero</c> to find and no
    /// <c>DamageHero</c> component to walk up to. The Shade simply stood in the acid unharmed.
    /// </para>
    /// <para>
    /// The relay below registers the Shade's body collider alongside Hornet's. That forces the
    /// callback to be replaced rather than extended: the stock one damages Hornet on <i>any</i>
    /// enter particle, so with two colliders registered a spray that only touched the Shade would
    /// hurt Hornet instead. The replacement asks which collider each particle actually hit, which
    /// needs <c>colliderQueryMode</c> raised from <c>One</c> to <c>All</c>.
    /// </para>
    /// </summary>
    [HarmonyPatch(typeof(ParticleDamageHero), "Start")]
    private static class ParticleDamageHero_Start_AddShadeRelay
    {
        private static void Postfix(ParticleDamageHero __instance)
        {
            try
            {
                if (__instance == null || __instance.GetComponent<ParticleDamageHeroShadeRelay>() != null)
                {
                    return;
                }

                __instance.gameObject.AddComponent<ParticleDamageHeroShadeRelay>();
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(ParticleDamageHero), "OnParticleTrigger")]
    private static class ParticleDamageHero_OnParticleTrigger_Shade
    {
        private static bool Prefix(ParticleDamageHero __instance)
        {
            try
            {
                var relay = __instance != null ? __instance.GetComponent<ParticleDamageHeroShadeRelay>() : null;
                // The relay declines whenever the Shade is not registered, leaving the stock
                // hero-only path to run exactly as it always did.
                return relay == null || !relay.HandleParticleTrigger();
            }
            catch
            {
                return true;
            }
        }
    }

    private sealed class ParticleDamageHeroShadeRelay : MonoBehaviour
    {
        private static readonly List<ParticleSystem.Particle> ParticleBuffer = new List<ParticleSystem.Particle>();

        private ParticleSystem system;
        private ShadeController cachedController;
        private Collider2D registeredShadeCollider;

        private void Awake()
        {
            system = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            SyncShadeCollider();
        }

        /// <summary>
        /// Re-checked every frame rather than captured once, because the Shade is destroyed and
        /// rebuilt on every scene load and whenever it is toggled off. A collider registered once in
        /// Start would go stale on the first transition and quietly stop the Shade being hit again
        /// for the rest of the session.
        /// </summary>
        private void SyncShadeCollider()
        {
            if (system == null)
            {
                return;
            }

            Collider2D desired = ResolveShadeCollider();
            if (ReferenceEquals(desired, registeredShadeCollider))
            {
                return;
            }

            try
            {
                var trigger = system.trigger;
                if (registeredShadeCollider != null)
                {
                    for (int i = trigger.colliderCount - 1; i >= 0; i--)
                    {
                        if (ReferenceEquals(trigger.GetCollider(i), registeredShadeCollider))
                        {
                            trigger.RemoveCollider(i);
                        }
                    }
                }

                if (desired != null)
                {
                    trigger.AddCollider(desired);
                    trigger.colliderQueryMode = ParticleSystemColliderQueryMode.All;
                }

                registeredShadeCollider = desired;
            }
            catch
            {
                registeredShadeCollider = null;
            }
        }

        private Collider2D ResolveShadeCollider()
        {
            try
            {
                if (!ModConfig.Instance.shadeEnabled)
                {
                    return null;
                }

                if (cachedController == null && !TryGetShadeController(out cachedController))
                {
                    return null;
                }

                return cachedController != null ? cachedController.BodyCollider : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns false to hand the frame back to the stock handler; true once it has dealt with
        /// the entering particles itself.
        /// </summary>
        internal bool HandleParticleTrigger()
        {
            if (system == null || registeredShadeCollider == null)
            {
                return false;
            }

            if (system.GetSafeTriggerParticlesSize(ParticleSystemTriggerEventType.Enter) <= 0)
            {
                return true;
            }

            bool hitHornet = false;
            bool hitShade = false;

            int count = system.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, ParticleBuffer, out var colliderData);
            for (int i = 0; i < count; i++)
            {
                int colliders = colliderData.GetColliderCount(i);
                for (int j = 0; j < colliders; j++)
                {
                    var hit = colliderData.GetCollider(i, j);
                    if (hit == null)
                    {
                        continue;
                    }

                    if (ReferenceEquals(hit, registeredShadeCollider))
                    {
                        hitShade = true;
                    }
                    else
                    {
                        hitHornet = true;
                    }
                }
            }

            if (hitHornet)
            {
                DamageHornet();
            }

            if (hitShade)
            {
                DamageShade();
            }

            return true;
        }

        // Mirrors the body of the method this replaces, HeroBox.Inactive guard included.
        private void DamageHornet()
        {
            try
            {
                var hero = HeroController.instance;
                var heroBox = hero != null ? hero.heroBox : null;
                if (heroBox != null && !HeroBox.Inactive)
                {
                    heroBox.CheckForDamage(gameObject);
                }
            }
            catch
            {
            }
        }

        private void DamageShade()
        {
            try
            {
                // Unity's null check, not C#'s: the Shade can be destroyed between the frame that
                // registered its collider and the callback that reports a hit on it.
                var controller = cachedController;
                if (controller != null)
                {
                    controller.NotifyParticleDamage(gameObject);
                }
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Records what damages Hornet, for the bug report event ring.
    /// <para>
    /// <c>HeroBox.CheckForDamage</c> is the single choke point for every hero damage the game
    /// delivers - it handles both shapes, an FSM named <c>damages_hero</c> on the source and a plain
    /// <see cref="DamageHero"/> component - so one hook here names the culprit whatever form it took.
    /// </para>
    /// <para>
    /// This exists because of the Lace report the Shade damage recorder could not close out. That
    /// one established, to the frame, that the Shade was hit by <c>Lace Boss1/Cross Slash/hero
    /// damager</c> and that Hornet took a hit as the attack ended - and could not say what hit
    /// Hornet, because nothing recorded the hero's side at all. Whether the Shade's presence caused
    /// her hit or the attack simply reached her too is exactly the question this answers.
    /// </para>
    /// </summary>
    [HarmonyPatch(typeof(HeroBox), nameof(HeroBox.CheckForDamage))]
    private static class HeroBox_CheckForDamage_Record
    {
        private static void Prefix(GameObject otherGameObject)
        {
            try
            {
                if (otherGameObject == null)
                {
                    return;
                }

                int damage = 0;
                var hazard = GlobalEnums.HazardType.NON_HAZARD;
                var damager = otherGameObject.GetComponent<DamageHero>();
                if (damager != null)
                {
                    damage = damager.enabled ? damager.damageDealt : 0;
                    hazard = damager.hazardType;
                }

                LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
                    "hero-damage",
                    DescribeHierarchy(otherGameObject.transform, 3),
                    FormattableString.Invariant(
                        $"damage={damage} hazard={hazard} layer={LayerMask.LayerToName(otherGameObject.layer)} tag={otherGameObject.tag} hasDamageHero={damager != null}"));
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// An object's name with a few ancestors, which is what makes a recorded event identifiable:
    /// "hero damager" on its own names nothing, "Lace Boss1/Cross Slash/hero damager" names the
    /// attack.
    /// </summary>
    internal static string DescribeHierarchy(Transform target, int ancestors)
    {
        if (target == null)
        {
            return "<null>";
        }

        var builder = new System.Text.StringBuilder(target.name);
        var current = target.parent;
        int depth = 0;
        while (current != null && depth < ancestors)
        {
            builder.Insert(0, current.name + "/");
            current = current.parent;
            depth++;
        }

        return builder.ToString();
    }

    /// <summary>
    /// Sends the hits of a redirected grab to the Shade, since the Shade is the one in the attack.
    /// <para>
    /// <c>ShadeGrabRetargeting</c> moves the grab's <i>teleport</i> onto the Shade; everything after
    /// that is a separate call aimed at <c>HeroController</c> by name, so without this pair the Shade
    /// is dragged into the attack and Hornet still takes every hit of it from where she stood.
    /// <c>TakeDamage</c> names the damaging object and can be attributed properly;
    /// <c>TakeQuickDamage</c> - what the multi-hit part of Lace's cross slash calls - names nothing,
    /// which is why the redirect window exists to attribute an anonymous hit to the grab that opened it.
    /// </para>
    /// <para>
    /// Both are resolved by parameter shape rather than named through the attribute: naming
    /// <c>TakeQuickDamage</c> throws <c>AmbiguousMatchException</c>, because the shipped assembly
    /// carries an overload the decompiled reference does not - and that throw escapes
    /// <c>PatchAll</c>, costing the mod every one of its patches rather than just this one.
    /// </para>
    /// </summary>
    [HarmonyPatch]
    private static class HeroController_TakeQuickDamage_RedirectGrab
    {
        private static IEnumerable<MethodBase> TargetMethods()
            => FindHeroDamageMethods("TakeQuickDamage", requireSource: false);

        private static bool Prefix(int damageAmount)
        {
            return !ShadeGrabRetargeting.TryRedirectHeroDamage(null, damageAmount, "TakeQuickDamage");
        }
    }

    [HarmonyPatch]
    private static class HeroController_TakeDamage_RedirectGrab
    {
        private static IEnumerable<MethodBase> TargetMethods()
            => FindHeroDamageMethods("TakeDamage", requireSource: true);

        private static bool Prefix(GameObject go, int damageAmount)
        {
            return !ShadeGrabRetargeting.TryRedirectHeroDamage(go, damageAmount, "TakeDamage");
        }
    }

    /// <summary>
    /// Every <see cref="HeroController"/> method of this name whose parameters the prefixes above can
    /// actually bind to - an <c>int damageAmount</c>, plus a <c>GameObject go</c> when the caller
    /// needs to know the source. Harmony binds prefix parameters by name, so an overload missing one
    /// of those would fail at patch time; filtering here is what keeps that from happening.
    /// </summary>
    internal static IEnumerable<MethodBase> FindHeroDamageMethods(string name, bool requireSource)
    {
        MethodInfo[] candidates;
        try
        {
            candidates = typeof(HeroController).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        catch
        {
            yield break;
        }

        foreach (var candidate in candidates)
        {
            if (candidate == null || !string.Equals(candidate.Name, name, StringComparison.Ordinal))
            {
                continue;
            }

            ParameterInfo[] parameters;
            try { parameters = candidate.GetParameters(); }
            catch { continue; }

            bool hasAmount = false;
            bool hasSource = false;
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == typeof(int) && string.Equals(parameter.Name, "damageAmount", StringComparison.Ordinal))
                {
                    hasAmount = true;
                }
                else if (parameter.ParameterType == typeof(GameObject) && string.Equals(parameter.Name, "go", StringComparison.Ordinal))
                {
                    hasSource = true;
                }
            }

            if (hasAmount && (!requireSource || hasSource))
            {
                yield return candidate;
            }
        }
    }

    [HarmonyPatch(typeof(GameManager), "BeginScene")]
    private class GameManager_BeginScene_Patch
    {
        private static void Postfix(GameManager __instance)
        {
            if (__instance != null)
            {
                ShadeRuntime.SyncActiveSlot(__instance);
            }

            DisableStartup(__instance);
            bool gameplay = __instance.IsGameplayScene();
            if (hud != null)
            {
                hud.SetVisible(gameplay && ModConfig.Instance.shadeEnabled);
            }
            if (!gameplay)
            {
                DestroyShadeInstance();
                return;
            }

            if (!ModConfig.Instance.shadeEnabled)
            {
                DestroyShadeInstance();
                return;
            }

            if (!registeredEnterSceneHandler)
            {
                __instance.OnFinishedEnteringScene += HandleFinishedEnteringScene;
                registeredEnterSceneHandler = true;
            }

            if (hud == null)
            {
                var hudGO = new UnityEngine.GameObject("SimpleHUD");
                UnityEngine.Object.DontDestroyOnLoad(hudGO);
                hud = hudGO.AddComponent<SimpleHUD>();
                hud.Init(__instance.playerData);
            }
            else
            {
                hud.SetPlayerData(__instance.playerData);
            }
        }
    }

    [HarmonyPatch(typeof(TrackTriggerObjects), "OnTriggerEnter2D")]
    private static class TrackTriggerObjects_OnTriggerEnter2D_Patch
    {
        private static bool Prefix(TrackTriggerObjects __instance, Collider2D collision)
        {
            // A camera lock area is Hornet's to trip; the Shade's aggro proxy must not move the camera.
            if (__instance is CameraLockArea
                && collision
                && collision.GetComponent<ShadeController.AggroProxyTracker>() != null)
            {
                return false;
            }

            return true;
        }
    }

    // Enter and exit need the same suppression, so one patch covers both rather than
    // two byte-identical classes.
    [HarmonyPatch]
    private static class Remasker_ShadeProxy_Patch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Remasker), "OnTriggerEnter2D");
            yield return AccessTools.Method(typeof(Remasker), "OnTriggerExit2D");
        }

        private static bool Prefix(Remasker __instance, Collider2D collision)
        {
            if (collision == null)
            {
                return true;
            }

            var tracker = collision.GetComponent<ShadeController.AggroProxyTracker>();
            if (tracker == null)
            {
                return true;
            }

            try
            {
                tracker.NotifyRemaskerIgnored(__instance);
            }
            catch
            {
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(AlertRange), "FixedUpdate")]
    internal static class AlertRange_FixedUpdate_Patch
    {
        private static readonly AccessTools.FieldRef<AlertRange, bool> HaveLineOfSightRef = AccessTools.FieldRefAccess<AlertRange, bool>("haveLineOfSight");
        private static readonly AccessTools.FieldRef<AlertRange, bool> IsHeroInRangeRef = AccessTools.FieldRefAccess<AlertRange, bool>("isHeroInRange");
        private static readonly AccessTools.FieldRef<AlertRange, AlertRange.LineOfSightChecks> LineOfSightModeRef = AccessTools.FieldRefAccess<AlertRange, AlertRange.LineOfSightChecks>("lineOfSight");
        private static readonly AccessTools.FieldRef<AlertRange, Transform> InitialParentRef = AccessTools.FieldRefAccess<AlertRange, Transform>("initialParent");

        private sealed class LogState
        {
            public bool Logged;
        }

        private static readonly List<ShadeAggroTracker.Target> TargetBuffer = new List<ShadeAggroTracker.Target>();
        private static readonly ConditionalWeakTable<AlertRange, LogState> LoggedStates = new ConditionalWeakTable<AlertRange, LogState>();

        private static void Postfix(AlertRange __instance)
        {
            try
            {
                if (__instance == null)
                {
                    return;
                }

                if (!ShadeAggroTracker.TryGetTargets(__instance, TargetBuffer) || TargetBuffer.Count == 0)
                {
                    return;
                }

                bool hadLineOfSight = HaveLineOfSightRef(__instance);
                bool heroInRange = IsHeroInRangeRef(__instance);
                bool proxiesGrantLineOfSight = false;

                var mode = LineOfSightModeRef(__instance);
                if (mode <= AlertRange.LineOfSightChecks.None)
                {
                    proxiesGrantLineOfSight = true;
                }
                else
                {
                    Transform originTransform = null;
                    switch (mode)
                    {
                        case AlertRange.LineOfSightChecks.Self:
                            originTransform = __instance.transform;
                            break;
                        case AlertRange.LineOfSightChecks.Parent:
                            originTransform = __instance.transform.parent ?? InitialParentRef(__instance);
                            break;
                    }

                    Vector2 origin = originTransform ? (Vector2)originTransform.position : (Vector2)__instance.transform.position;
                    foreach (var target in TargetBuffer)
                    {
                        if (target.Shade == null || !target.Shade.IsAggroEligible)
                        {
                            continue;
                        }

                        if (!global::Helper.LineCast2DHit(origin, target.Position, 256, out _))
                        {
                            proxiesGrantLineOfSight = true;
                            if (!hadLineOfSight)
                            {
                                var state = LoggedStates.GetOrCreateValue(__instance);
                                if (!state.Logged && ModConfig.Instance.logShade)
                                {
                                    try
                                    {
                                        string owner = target.Shade != null ? target.Shade.gameObject.name : "Shade";
                                        string rangeOwner = __instance.transform != null ? __instance.transform.root?.name ?? __instance.transform.name : __instance.name;
                                        LegacyHelper.Instance?.Logger?.LogInfo($"Shade aggro granted line of sight for '{__instance.name}' on '{rangeOwner}' via shade '{owner}'.");
                                    }
                                    catch
                                    {
                                    }
                                    state.Logged = true;
                                }
                            }
                            break;
                        }
                    }
                }

                if (!heroInRange)
                {
                    IsHeroInRangeRef(__instance) = true;
                }

                if (proxiesGrantLineOfSight)
                {
                    HaveLineOfSightRef(__instance) = true;
                }
                else
                {
                    var state = LoggedStates.GetOrCreateValue(__instance);
                    state.Logged = false;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    LegacyHelper.Instance?.Logger?.LogWarning($"Shade aggro patch failed for '{__instance?.name}': {ex}");
                }
                catch
                {
                }
            }
            finally
            {
                TargetBuffer.Clear();
            }
        }

        internal static void ResetLog(AlertRange range)
        {
            if (range == null)
            {
                return;
            }

            if (LoggedStates.TryGetValue(range, out var logState))
            {
                logState.Logged = false;
            }
        }
    }

    // Refill the shade when Hornet dies, matching the MaxHealth she gets on respawn
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.PlayerDead))]
    private class GameManager_PlayerDead_Patch
    {
        private static void Postfix()
        {
            try
            {
                if (!ModConfig.Instance.shadeEnabled)
                    return;

                bool healedAny = false;
                foreach (var companion in ShadeCompanionRegistry.All)
                {
                    var sc = companion.Controller;
                    if (sc == null)
                        continue;

                    sc.FullHealOnRespawn();
                    SaveShadeState(companion, sc.GetCurrentNormalHP(), sc.GetMaxNormalHP(), sc.GetCurrentLifeblood(), sc.GetMaxLifeblood(), sc.GetShadeSoul(), sc.GetCanTakeDamage(), sc.GetBaseMaxHP(), sc.GetShadeVesselSoul());
                    healedAny = true;
                }

                if (healedAny)
                    return;
                // Fallback: refill the saved state so the next spawn comes back whole
                if (ShadeRuntime.PersistentState.HasData)
                {
                    ShadeRuntime.RestoreFullHealth();
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Puts the new-game questions in front of a new game.
    /// <para>
    /// Patched here rather than on one of the <c>UIStartNewGame</c> wrappers because there are
    /// several of those and they all land on this: the save slot button, the play mode screen, and
    /// the overscan and brightness prompts that call back into it once they have been answered.
    /// <see cref="ShadeSettingsMenu.InterceptNewGame"/> decides which of those passes is the one
    /// that is actually about to start a game.
    /// </para>
    /// </summary>
    [HarmonyPatch(typeof(UIManager), nameof(UIManager.StartNewGame))]
    private class UIManager_StartNewGame_Patch
    {
        private static bool Prefix(UIManager __instance, bool permaDeath, bool bossRush)
        {
            try
            {
                if (ShadeSettingsMenu.InterceptNewGame(__instance, permaDeath, bossRush))
                    return false;
            }
            catch (Exception e)
            {
                // Never at the cost of the new game itself: a mod screen that cannot be shown is a
                // missing screen, not a save file the player cannot start.
                LogWarning($"New game options threw; starting the game as normal: {e}");
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(UIManager), nameof(UIManager.TogglePauseGame))]
    private class UIManager_TogglePauseGame_Patch
    {
        private static bool Prefix(UIManager __instance)
        {
            try
            {
                if (ShadeSettingsMenu.HandlePauseToggle(__instance))
                    return false;
            }
            catch { }
            return true;
        }
    }

    // Both pause entry points need identical handling, so one patch class targets both
    // rather than two byte-identical copies. Stacking two [HarmonyPatch] attributes would
    // merge into a single target, so the multi-target form goes through TargetMethods.
    [HarmonyPatch]
    private class GameManager_PauseGameToggle_Patch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(GameManager), nameof(GameManager.PauseGameToggle));
            yield return AccessTools.Method(typeof(GameManager), nameof(GameManager.PauseGameToggleByMenu));
        }

        private static bool Prefix(GameManager __instance, ref IEnumerator __result)
        {
            try
            {
                var ui = UIManager.instance;
                if (ui == null && __instance != null)
                    ui = __instance.ui;
                if (ui != null && ShadeSettingsMenu.HandlePauseToggle(ui))
                {
                    __result = Skip();
                    return false;
                }
            }
            catch { }
            return true;
        }

        private static IEnumerator Skip()
        {
            yield break;
        }
    }

    [HarmonyPatch(typeof(HealthManager), "SpawnCurrency")]
    private class HealthManager_SpawnCurrency_Patch
    {
        private static void Prefix(ref int smallGeoCount, ref int mediumGeoCount, ref int largeGeoCount, ref int largeSmoothGeoCount)
        {
            if (!FragileGreedActive)
            {
                return;
            }

            smallGeoCount = ApplyMultiplier(smallGeoCount);
            mediumGeoCount = ApplyMultiplier(mediumGeoCount);
            largeGeoCount = ApplyMultiplier(largeGeoCount);
            largeSmoothGeoCount = ApplyMultiplier(largeSmoothGeoCount);
        }

        private static int ApplyMultiplier(int value)
        {
            if (value <= 0)
            {
                return value;
            }

            int scaled = Mathf.CeilToInt(value * 1.5f);
            return Mathf.Max(1, scaled);
        }
    }

    [HarmonyPatch(typeof(UIManager), nameof(UIManager.ShowMenu))]
    private class UIManager_ShowMenu_Patch
    {
        private static bool Prefix(UIManager __instance, MenuScreen menu, ref IEnumerator __result)
        {
            try
            {
                if (!ShadeSettingsMenu.IsShowing || menu == null || __instance == null)
                    return true;

                if (menu == __instance.pauseMenuScreen || menu == __instance.optionsMenuScreen || menu == __instance.gameOptionsMenuScreen)
                {
                    __result = EmptyEnumerator();
                    return false;
                }
            }
            catch { }
            return true;
        }

        private static IEnumerator EmptyEnumerator()
        {
            yield break;
        }
    }

    [HarmonyPatch(typeof(GameManager), "Awake")]
    private class GameManager_Awake_Patch
    {
        private static void Postfix(GameManager __instance)
        {
            // New GameManager instance; ensure we re-register scene-enter handler next time.
            registeredEnterSceneHandler = false;
            DisableStartup(__instance);
        }
    }

    [HarmonyPatch(typeof(GameManager), "Start")]
    private class GameManager_Start_Patch
    {
        private static void Postfix(GameManager __instance) => DisableStartup(__instance);
    }

    [HarmonyPatch]
    private class InventoryPaneList_EnsureShadePane_Patch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var type = typeof(InventoryPaneList);

            string[] candidates = { "Awake", "Start", "OnEnable" };
            foreach (string name in candidates)
            {
                var method = AccessTools.Method(type, name);
                if (method != null)
                {
                    yield return method;
                }
            }
        }

        private static void Postfix(InventoryPaneList __instance)
        {
            if (__instance == null)
            {
                return;
            }

            LegacyoftheAbyss.Diagnostics.InventoryOpenProbe.Attach(__instance);

            try
            {
                ShadeInventoryPaneIntegration.EnsurePane(__instance);
            }
            catch (Exception ex)
            {
                if (ModConfig.Instance.logMenu)
                {
                    Debug.LogWarning($"[ShadeInventory] Failed to ensure shade pane: {ex}");
                }
            }
        }
    }

    /// <summary>
    /// Traces every numeric pane request while <c>logMenu</c> is on.
    /// <para>
    /// Pane switching runs through three separate numeric paths - the shortcut FSM's hardcoded
    /// indices (Tools 1, Quests 2, Journal 3, Map 4), the <c>Target Pane Index</c> variable
    /// <c>InventoryPaneInput</c> writes while the inventory is open, and <c>-1</c> meaning "reopen
    /// whatever was last shown". A wrong tab looks identical from the player's side no matter which
    /// path produced it, so log the index that came in and the pane that came out.
    /// </para>
    /// </summary>
    [HarmonyPatch(typeof(InventoryPaneList), nameof(InventoryPaneList.SetCurrentPane))]
    private class InventoryPaneList_SetCurrentPane_Trace
    {
        private static void Postfix(InventoryPaneList __instance, int index, InventoryPane __result)
        {
            try
            {
                if (!ModConfig.Instance.logMenu)
                {
                    return;
                }

                string resolved = __result == null
                    ? "<null>"
                    : (__result.gameObject != null ? __result.gameObject.name : __result.name);
                int resolvedIndex = __instance != null && __result != null ? __instance.GetPaneIndex(__result) : -1;


                LegacyHelper.LogInfo(FormattableString.Invariant(
                    $"SetCurrentPane(requested={index}) -> [{resolvedIndex}] {resolved}"));
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Key 6 - the slot immediately after the K&amp;M inventory shortcuts (1=Inv, 2=Tools, 3=Quests,
    /// 4=Journal, 5=Map; see HornetInput.cs) - jumps straight to the Shade's charm tab from any other
    /// open pane. There is no base-game <c>PaneTypes</c> value for the Shade pane to hang a "real"
    /// shortcut off of, so this runs entirely outside the FSM-driven shortcut system; see
    /// <see cref="ShadeInventoryPaneIntegration.TryJumpToShadeTab"/> for why it only works once the
    /// inventory is already open.
    /// </summary>
    [HarmonyPatch(typeof(InventoryPaneInput), "Update")]
    private class InventoryPaneInput_Update_ShadeTabShortcut
    {
        /// <summary>
        /// Handles the two ways an <c>InventoryPaneInput</c> routed to the Shade pane misbehaves.
        /// <para>
        /// If the Shade tab is <i>not</i> currently showing, the input is stale and is made inert.
        /// The Shade's input component carries <c>paneControl = None</c>, having no <c>PaneTypes</c>
        /// value of its own, and <c>None</c> is the case <c>InventoryPaneInput.Update</c> reads as
        /// "the player pressed this pane's own shortcut" - answered with <c>PressCancel()</c>. Left
        /// running after its tab is closed, that component makes every inventory shortcut close the
        /// whole inventory, on every tab, for the rest of the session. Refusing input for a pane that
        /// is not on screen fixes it without depending on why the GameObject stayed active: an input
        /// belonging to a hidden pane has nothing legitimate to do either way.
        /// </para>

        /// <para>
        /// If the Shade tab <i>is</i> showing, keys 1-5 switch tabs instead of closing the inventory -
        /// see <see cref="ShadeInventoryPaneIntegration.TryHandleShadeTabPaneShortcut"/>.
        /// </para>
        /// </summary>
        private static bool Prefix(InventoryPaneInput __instance)
        {
            try
            {
                if (__instance == null)
                {
                    return true;
                }

                var shadePane = ShadeInventoryPaneIntegration.TryGetShadePane(__instance);
                if (shadePane == null)
                {
                    return true;
                }

                if (!shadePane.IsPaneActive || !ReferenceEquals(ShadeInventoryPane.ActivePane, shadePane))
                {
                    if (ModConfig.Instance.logMenu)
                    {
                        try
                        {
                            LegacyHelper.LogInfo(FormattableString.Invariant(
                                $"Ignoring input on '{__instance.gameObject?.name}': routed to the Shade pane but its tab is not showing"));
                        }
                        catch
                        {
                        }
                    }

                    return false;
                }

                return !ShadeInventoryPaneIntegration.TryHandleShadeTabPaneShortcut(__instance);
            }
            catch
            {
            }

            return true;
        }

        private static void Postfix(InventoryPaneInput __instance)
        {
            try
            {
                if (__instance == null || !Input.GetKeyDown(KeyCode.Alpha6))
                {
                    return;
                }

                ShadeInventoryPaneIntegration.TryJumpToShadeTab(__instance);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Names whatever closes the inventory. <c>PressCancel</c> is the single path by which an
    /// inventory shortcut ends up closing the whole inventory instead of switching tabs (bug 4b), and
    /// the component it fires on is the only thing that identifies the culprit - the resulting
    /// "UI CANCEL" event is anonymous by the time anything else sees it.
    /// </summary>
    [HarmonyPatch(typeof(InventoryPaneInput), "PressCancel")]
    private class InventoryPaneInput_PressCancel_Trace
    {
        private static readonly AccessTools.FieldRef<InventoryPaneInput, InventoryPaneList.PaneTypes> PaneControlField =
            AccessTools.FieldRefAccess<InventoryPaneInput, InventoryPaneList.PaneTypes>("paneControl");

        private static void Prefix(InventoryPaneInput __instance)
        {
            try
            {
                if (__instance == null || !ModConfig.Instance.logMenu)
                {
                    return;
                }

                var paneControl = PaneControlField(__instance);
                bool boundToShade = ShadeInventoryPaneIntegration.TryGetShadePane(__instance) != null;
                LegacyHelper.LogInfo(FormattableString.Invariant(
                    $"PressCancel from '{__instance.gameObject?.name}' (paneControl={paneControl}, boundToShade={boundToShade}, shadeTabShowing={ShadeInventoryPane.ActivePane != null})"));
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InventoryPaneInput), "PressSubmit")]
    private class InventoryPaneInput_PressSubmit_Shade
    {
        private static bool Prefix(InventoryPaneInput __instance)
        {
            try
            {
                var shadePane = ShadeInventoryPaneIntegration.TryGetShadePane(__instance) ?? ShadeInventoryPane.ActivePane;
                if (shadePane != null)
                {
                    shadePane.HandleSubmit();
                    return false;
                }
            }
            catch { }

            return true;
        }
    }

    [HarmonyPatch(typeof(InventoryPaneInput), "PressDirection")]
    private class InventoryPaneInput_PressDirection_Shade
    {
        private static void Postfix(InventoryPaneInput __instance, InventoryPaneBase.InputEventType direction)
        {
            try
            {
                var shadePane = ShadeInventoryPaneIntegration.TryGetShadePane(__instance) ?? ShadeInventoryPane.ActivePane;
                shadePane?.HandleDirectionalInput(direction);
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InventoryPaneList), nameof(InventoryPaneList.BeginPane))]
    private class InventoryPaneList_BeginPane_BindShadeInput
    {
        private static void Postfix(InventoryPaneList __instance, InventoryPane pane)
        {
            try
            {
                if (__instance == null || pane == null)
                {
                    return;
                }

                var shadePane = pane as ShadeInventoryPane;
                if (shadePane == null)
                {
                    shadePane = pane.RootPane as ShadeInventoryPane;
                }

                if (shadePane != null)
                {
                    ShadeInventoryPaneIntegration.BindInput(shadePane, __instance, captureFocus: true);
                }
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(StartManager), "Start")]
    private class StartManager_Start_Enumerator_Patch
    {
        private static void Prefix(StartManager __instance)
        {
            if (__instance.startManagerAnimator != null)
                __instance.startManagerAnimator.SetBool("WillShowQuote", false);
        }

        private static void Postfix(StartManager __instance, ref IEnumerator __result)
        {
            if (__result == null) return;
            var fields = __result.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(bool) && f.Name.Contains("showIntroSequence"))
                {
                    f.SetValue(__result, false);
                    if (__instance.startManagerAnimator != null)
                        __instance.startManagerAnimator.Play("LoadingIcon", 0, 1f);
                    break;
                }
            }
        }
    }

    [HarmonyPatch(typeof(RestBenchHelper), "SetOnBench")]
    private class RestBenchHelper_SetOnBench_Patch
    {
        private static void Postfix(bool onBench)
        {
            if (!onBench) return;
            try
            {
                if (!ModConfig.Instance.shadeEnabled)
                    return;

                foreach (var companion in ShadeCompanionRegistry.All)
                {
                    var sc = companion.Controller;
                    if (sc == null)
                        continue;

                    sc.FullHealFromBench();
                    SaveShadeState(companion, sc.GetCurrentNormalHP(), sc.GetMaxNormalHP(), sc.GetCurrentLifeblood(), sc.GetMaxLifeblood(), sc.GetShadeSoul(), sc.GetCanTakeDamage(), sc.GetBaseMaxHP(), sc.GetShadeVesselSoul());
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Scales Hornet's damage, needle and silk skills separately, at the moment of the hit.
    /// <para>
    /// This used to run once when a damage object was created, and scale <c>damageDealt</c>. That
    /// worked for silk skills and did <em>nothing at all</em> for the needle, because a damager with
    /// <c>useNailDamage</c> set never reads <c>damageDealt</c>: <c>DoDamage</c> starts the damage
    /// stack over from <c>PlayerData.nailDamage</c> and applies <c>nailDamageMultiplier</c> to it,
    /// discarding whatever was in the field. The Needle slider therefore moved a number nothing
    /// consumed. Scaling at spawn was also at the mercy of anything that writes either field later -
    /// pooled damagers, projectiles that cache and restore their own amounts.
    /// </para>
    /// <para>
    /// So it happens here instead: the multiplier is applied to whichever of the two the hit is
    /// actually going to read, immediately before the hit resolves, and put back immediately after.
    /// The Shade's own slash is not caught by this - it clears <c>sourceIsHero</c> and
    /// <c>isHeroDamage</c> on its cloned damager precisely so it is not mistaken for hers.
    /// </para>
    /// </summary>
    [HarmonyPatch(typeof(DamageEnemies), nameof(DamageEnemies.DoDamage), new[] { typeof(GameObject), typeof(bool) })]
    private class DamageEnemies_DoDamage_HornetScaling
    {
        // Private on DamageEnemies, so resolved once here rather than per hit. Asserted in
        // Tests/GameApiContract.cs: if one stops resolving, Hornet's damage sliders silently stop
        // telling needle from silk skill.

        private static readonly FieldInfo SourceIsHeroField =
            typeof(DamageEnemies).GetField("sourceIsHero", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo IsHeroDamageField =
            typeof(DamageEnemies).GetField("isHeroDamage", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo IsNailAttackField =
            typeof(DamageEnemies).GetField("isNailAttack", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// What the prefix changed, so the postfix can put it back exactly. Built complete and
        /// assigned in one go - Harmony's analyser reads any member assignment through a patch
        /// parameter, an object initializer included, as a write that will not survive the call.
        /// </summary>
        internal readonly struct Scaled
        {
            internal Scaled(int damageDealt, float nailMultiplier)
            {
                Applied = true;
                DamageDealt = damageDealt;
                NailMultiplier = nailMultiplier;
            }

            public bool Applied { get; }
            public int DamageDealt { get; }
            public float NailMultiplier { get; }
        }

        private static bool ReadPrivateBool(FieldInfo field, DamageEnemies instance)
        {
            return field?.GetValue(instance) is bool value && value;
        }

        private static void Prefix(DamageEnemies __instance, ref Scaled __state)
        {
            __state = default;

            try
            {
                bool src = ReadPrivateBool(SourceIsHeroField, __instance);
                bool hero = ReadPrivateBool(IsHeroDamageField, __instance);
                if (!src && !hero)
                {
                    return;
                }

                // The three conditions here are the same ones DamageEnemies itself uses to decide
                // whether a hit counts as a nail hit (see its DoDamage).
                bool isNeedle = __instance.attackType == AttackTypes.Nail
                    || __instance.attackType == AttackTypes.NailBeam
                    || ReadPrivateBool(IsNailAttackField, __instance);
                float multiplier = isNeedle
                    ? ModConfig.Instance.hornetDamageMultiplier
                    : ModConfig.Instance.hornetSilkSkillDamageMultiplier;

                if (Mathf.Approximately(multiplier, 1f))
                {
                    return;
                }

                __state = new Scaled(__instance.damageDealt, __instance.nailDamageMultiplier);

                if (__instance.useNailDamage)
                {
                    // Floored so a low setting weakens a hit rather than erasing it, which is what
                    // the Max(1, ...) on the other branch has always done for silk skills.
                    var playerData = PlayerData.instance;
                    int nailDamage = playerData != null ? playerData.nailDamage : 0;
                    float floor = nailDamage > 0 ? 1f / nailDamage : 0f;
                    __instance.nailDamageMultiplier = Mathf.Max(__instance.nailDamageMultiplier * multiplier, floor);
                }
                else
                {
                    __instance.damageDealt = Mathf.Max(1, Mathf.RoundToInt(__instance.damageDealt * multiplier));
                }
            }
            catch
            {
            }
        }

        private static void Postfix(DamageEnemies __instance, Scaled __state)
        {
            // Copied out whole before anything is read off it. Harmony analyser reads any member
            // access through a patch parameter as a write it is about to lose, reads included.
            var scaled = __state;
            if (!scaled.Applied)
            {
                return;
            }

            try
            {
                __instance.damageDealt = scaled.DamageDealt;
                __instance.nailDamageMultiplier = scaled.NailMultiplier;
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Rewrites the bind's own heal to <c>bindHornetHeal</c>, at the call that actually performs it.
    /// <para>
    /// The obvious hook - watch Hornet's health across <c>BindCompleted</c> and correct the
    /// difference - heals her twice. <c>HeroController.BindCompleted</c> touches no health at all; it
    /// sets Warrior and Reaper crest state and nothing else. The heal is a <c>CallMethodProper</c> on
    /// <c>AddHealth</c> in the bind FSM's Bind Burst state, outside that window entirely, so the
    /// difference reads zero and the correction lands on top of the game's three instead of replacing it.
    /// </para>
    /// <para>
    /// Two conditions identify the burst: Hornet is binding, and the amount is the bind's own. Every
    /// other heal that can land mid-bind moves one mask at a time - regen, lifeblood - so the amount
    /// separates them on its own, and the state keeps an unrelated three-mask heal from being caught
    /// if one ever exists.
    /// </para>
    /// <para>
    /// Should the game ever change that amount, the override stops matching and the heal falls back
    /// to whatever the game does: a customised value silently stops applying, which is the right way
    /// round for this to fail. Logged once per bind so a report can show whether it fired.
    /// </para>
    /// </summary>
    [HarmonyPatch]
    private static class HeroController_AddHealth_BindOverride
    {
        /// <summary>The bind FSM's hardcoded heal - see Bind Burst in the FSM dump.</summary>
        private const int VanillaBindHeal = 3;

        private static IEnumerable<MethodBase> TargetMethods()
            => FindHeroAddHealthMethods();

        private static void Prefix(HeroController __instance, ref int amount)
        {
            try
            {
                if (amount != VanillaBindHeal)
                {
                    return;
                }

                var cState = __instance != null ? __instance.cState : null;
                if (cState == null || !cState.isBinding)
                {
                    return;
                }

                int desired = Mathf.Max(0, ModConfig.Instance.bindHornetHeal);
                if (desired == amount)
                {
                    return;
                }

                LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
                    "bind-heal",
                    "Hornet",
                    FormattableString.Invariant($"bind heal {amount} -> {desired}"));

                amount = desired;
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// <c>HeroController.AddHealth</c> overloads whose parameters the prefix above can bind to.
    /// Resolved by shape rather than named through the attribute for the same reason
    /// <see cref="FindHeroDamageMethods"/> is: an unrecognised overload set leaves the override off
    /// instead of throwing <c>AmbiguousMatchException</c> out of <c>PatchAll</c> and costing the mod
    /// every one of its patches.
    /// </summary>
    internal static IEnumerable<MethodBase> FindHeroAddHealthMethods()
    {
        MethodInfo[] candidates;
        try
        {
            candidates = typeof(HeroController).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        catch
        {
            yield break;
        }

        foreach (var candidate in candidates)
        {
            if (candidate == null || candidate.Name != "AddHealth")
            {
                continue;
            }

            var parameters = candidate.GetParameters();
            if (parameters.Length != 1)
            {
                continue;
            }

            if (parameters[0].Name != "amount" || parameters[0].ParameterType != typeof(int))
            {
                continue;
            }

            yield return candidate;
        }
    }

    // Trigger shade heal on explicit Bind completion event
    [HarmonyPatch(typeof(HeroController), "BindCompleted")]
    private class HeroController_BindCompleted_Patch
    {
        private static void Postfix(HeroController __instance)
        {
            try
            {
                if (ModConfig.Instance.shadeEnabled)
                {
                    var heroTransform = __instance != null ? __instance.transform : null;
                    foreach (var sc in ActiveShadeControllers())
                    {
                        sc.ApplyBindHealFromHornet(heroTransform);
                    }
                }
            }
            catch { }
        }
    }

    // When a SpellGetOrb completes collection (appears during spell acquisition sequences),
    // advance shade spell progression.
    [HarmonyPatch(typeof(SpellGetOrb), "Collect")]
    private class SpellGetOrb_Collect_Patch
    {
        private static void Postfix()
        {
            try { NotifyHornetSpellUnlocked(); } catch { }
        }
    }

    [HarmonyPatch(typeof(NailSlash), "Awake")]
    private class NailSlash_Awake_Log
    {
        private static bool Prefix(NailSlash __instance)
        {
            if (ShadeController.suppressActivateOnSlash)
            {
                Transform parent = __instance.transform.parent;
                if (parent != ShadeController.expectedSlashParent)
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                    return false;
                }
            }
            return true;
        }

        private static void Postfix(NailSlash __instance)
        {
            try
            {
                LegacyHelper.ShadeController.LogSlashState("NailSlash.Awake", __instance != null ? __instance.gameObject : null, includeStackTrace: true);
            }
            catch (System.Exception ex)
            {
                if (ModConfig.Instance.logShade)
                    UnityEngine.Debug.Log($"[ShadeDebug] NailSlash log error: {ex}");
            }
        }
    }

    [HarmonyPatch(typeof(NailSlash), nameof(NailSlash.StartSlash))]
    private class NailSlash_StartSlash_Log
    {
        private static void Postfix(NailSlash __instance)
        {
            try
            {
                LegacyHelper.ShadeController.LogSlashState("NailSlash.StartSlash", __instance != null ? __instance.gameObject : null, includeStackTrace: false);
            }
            catch { }
        }
    }

    // Prevent shade slashes from triggering Hornet pogo/bounce logic
    [HarmonyPatch(typeof(NailSlash), "DoDownspikeBounce")]
    private class NailSlash_DoDownspikeBounce_Block
    {
        private static bool Prefix(NailSlash __instance)
        {
            return __instance.transform.GetComponentInParent<ShadeController>() == null;
        }
    }

    [HarmonyPatch(typeof(NailSlash), "DownBounce")]
    private class NailSlash_DownBounce_Block
    {
        private static bool Prefix(NailSlash __instance)
        {
            return __instance.transform.GetComponentInParent<ShadeController>() == null;
        }
    }

    [HarmonyPatch(typeof(NailSlash), nameof(NailSlash.QueueBounce))]
    private class NailSlash_QueueBounce_Block
    {
        private static bool Prefix(NailSlash __instance)
        {
            return __instance.transform.GetComponentInParent<ShadeController>() == null;
        }
    }

    [HarmonyPatch(typeof(UIManager), nameof(UIManager.ShowMenu))]
    private class UIManager_ShowMenu_AddShadeButton
    {
        private static IEnumerator Postfix(IEnumerator __result, UIManager __instance, MenuScreen menu)
        {
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }
            try
            {
                if (menu == __instance.pauseMenuScreen)
                {
                    ShadeSettingsMenu.Inject(__instance);
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(PauseMenuButton), "OnSubmit")]
    private class PauseMenuButton_OnSubmit_Shade
    {
        private static bool Prefix(PauseMenuButton __instance, BaseEventData eventData)
        {
            if (__instance != null && __instance.gameObject.name == "ShadeSettingsButton")
            {
                try
                {
                    var ui = UnityEngine.Object.FindFirstObjectByType<UIManager>();
                    if (ui != null)
                        ui.StartCoroutine(ShadeSettingsMenu.Show(ui));
                }
                catch { }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UIManager), nameof(UIManager.UIGoToPauseMenu))]
    private class UIManager_UIGoToPauseMenu_HideShadeMenu
    {
        private static void Prefix(UIManager __instance)
        {
            if (ShadeSettingsMenu.IsShowing)
                ShadeSettingsMenu.HideImmediate(__instance);
        }
    }

    [HarmonyPatch(typeof(UIManager), nameof(UIManager.UIClosePauseMenu))]
    private class UIManager_UIClosePauseMenu_ClearShadeMenu
    {
        private static void Prefix(UIManager __instance)
        {
            if (ShadeSettingsMenu.IsShowing)
                ShadeSettingsMenu.HideImmediate(__instance);
            ShadeSettingsMenu.Clear();
        }
    }
}
#nullable restore
