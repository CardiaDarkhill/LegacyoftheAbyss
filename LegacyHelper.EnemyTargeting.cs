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
    /// Makes an alert range the Shade is standing in actually <i>report</i> the Shade, so an enemy
    /// can notice it at all. The half below <see cref="EnemyAiRetargeting"/>, and the one that has to
    /// work first: redirecting an enemy's target means nothing while it never leaves idle.
    /// <para>
    /// <c>TrackTriggerObjects</c> admits an object into <c>insideGameObjects</c> only if it passes
    /// the range's <c>ignoreLayers</c>/<c>tagIncludeList</c>/<c>tagExcludeList</c>, which the Shade's
    /// aggro proxy does not - so <c>InsideCount</c> reads 0, <c>IsInside</c> false, and
    /// <c>GetClosestInside</c> null however close it stands. <c>AlertRange_FixedUpdate_Patch</c>
    /// covers <c>AlertRange.IsHeroInRange()</c>; these cover the FSMs that count a range's contents
    /// or ask for the closest thing in it instead.
    /// </para>
    /// <para>
    /// Scoped to <c>AlertRange</c> rather than every <c>TrackTriggerObjects</c>: the base class backs
    /// plenty that has nothing to do with noticing the player, and inflating those counts would be a
    /// change with no upside. <c>AlertRange</c> inherits these members rather than overriding them,
    /// so patching the base and filtering here reaches exactly the intended set.
    /// </para>
    /// </summary>
    [HarmonyPatch(typeof(TrackTriggerObjects), nameof(TrackTriggerObjects.InsideCount), MethodType.Getter)]
    private static class TrackTriggerObjects_InsideCount_CountShade
    {
        private static void Postfix(TrackTriggerObjects __instance, ref int __result)
        {
            try
            {
                if (__instance is not AlertRange range || !ModConfig.Instance.shadeEnemyTargetingEnabled)
                {
                    return;
                }

                // A fact, not a preference: the Shade is either in the range or it isn't. Which of
                // the two an alerted enemy then goes after is EnemyAiRetargeting's decision.
                __result += ShadeAggroTracker.CountTargets(range);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Lets the Shade win "closest thing in this range" when it genuinely is closest.
    /// See <see cref="TrackTriggerObjects_InsideCount_CountShade"/> for why the range cannot see it
    /// on its own. Purely a distance comparison against whatever the base game picked - no
    /// preference, no hysteresis: this answers "what is nearest", not "who should I chase".
    /// </summary>
    [HarmonyPatch(typeof(TrackTriggerObjects), nameof(TrackTriggerObjects.GetClosestInside))]
    private static class TrackTriggerObjects_GetClosestInside_ConsiderShade
    {
        private static void Postfix(TrackTriggerObjects __instance, Vector2 toPos, List<GameObject> excludeObjects, ref GameObject __result)
        {
            try
            {
                if (__instance is not AlertRange range || !ModConfig.Instance.shadeEnemyTargetingEnabled)
                {
                    return;
                }

                if (!ShadeAggroTracker.TryGetClosestTarget(range, toPos, out var shadeObject, out _, out float shadeSqrDistance))
                {
                    return;
                }

                if (excludeObjects != null && excludeObjects.Contains(shadeObject))
                {
                    return;
                }

                if (__result != null && (((Vector2)__result.transform.position) - toPos).sqrMagnitude <= shadeSqrDistance)
                {
                    return;
                }

                __result = shadeObject;
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// The line-of-sight variant. Only the three-argument overload is patched - the two-argument one
    /// delegates to it, so patching both would run this twice for one call.
    /// </summary>
    [HarmonyPatch(typeof(TrackTriggerObjects), nameof(TrackTriggerObjects.GetClosestInsideLineOfSight),
        new[] { typeof(Vector2), typeof(HashSet<GameObject>), typeof(int) })]
    private static class TrackTriggerObjects_GetClosestInsideLineOfSight_ConsiderShade
    {
        private static void Postfix(TrackTriggerObjects __instance, Vector2 originPos, HashSet<GameObject> excludeObjects, int obstacleLayerMask, ref GameObject __result)
        {
            try
            {
                if (__instance is not AlertRange range || !ModConfig.Instance.shadeEnemyTargetingEnabled)
                {
                    return;
                }

                if (!ShadeAggroTracker.TryGetClosestTarget(range, originPos, out var shadeObject, out var shadePosition, out float shadeSqrDistance))
                {
                    return;
                }

                if (excludeObjects != null && excludeObjects.Contains(shadeObject))
                {
                    return;
                }

                if (__result != null && (((Vector2)__result.transform.position) - originPos).sqrMagnitude <= shadeSqrDistance)
                {
                    return;
                }

                // Same obstacle test the base method applies to its own candidates, against the
                // proxy's target point rather than the Shade's transform origin.
                Vector2 toShade = shadePosition - originPos;
                if (toShade.sqrMagnitude > 0.0001f)
                {
                    var hit = Physics2D.Raycast(originPos, toShade.normalized, toShade.magnitude, obstacleLayerMask);
                    if (hit.collider != null && hit.collider.gameObject != shadeObject)
                    {
                        return;
                    }
                }

                __result = shadeObject;
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Makes alerted enemies come after the Shade, by swapping the target out from under the
    /// enemy-AI actions that read it.
    /// <para>
    /// The interception point is one field. Silksong's enemy AI is PlayMaker-driven, and the actions
    /// that move an enemy toward something share a shape: an <c>FsmOwnerDefault</c> for the enemy and
    /// an <c>FsmGameObject</c> for what it chases, faces or fires at (<c>ChaseObject*</c>,
    /// <c>FaceObject*</c>, <c>DistanceFly*</c>, <c>FireAtTarget</c>, and so on). When such an action
    /// is about to run against Hornet and <see cref="ShadeAggroTargeting"/> says this enemy should
    /// prefer the Shade, that field is pointed at the Shade for the call and put back after. The
    /// enemy's own logic is untouched - it chases what it is told to chase.
    /// </para>
    /// <para>
    /// Borrow-and-restore rather than a permanent write: an <c>FsmGameObject</c> field can be bound
    /// to a shared FSM variable, so leaving the Shade in it would corrupt the enemy's own state and
    /// outlive the action. The pair rides Harmony's <c>__state</c>, binding each restore to the
    /// invocation that borrowed.
    /// </para>
    /// <para>
    /// Note the hero-side references are a red herring: most of the 184 files naming
    /// <c>HeroController.instance</c> do things <i>to</i> the hero - damage, cState, invulnerability,
    /// input blocking - rather than locating her.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Deliberately <b>not</b> a <c>[HarmonyPatch]</c> class, so <c>PatchAll()</c> does not pick it
    /// up. <c>PatchAll</c> is all-or-nothing: one class that throws while being applied aborts the
    /// call and every class it had not yet reached goes uninstalled, which presents as unrelated
    /// parts of the mod breaking at once with nothing in the log tying them together. This one
    /// resolves its targets reflectively and patches around a hundred methods, so it is the likeliest
    /// to throw; <see cref="Apply"/> installs it per method after <c>PatchAll</c> has finished, so
    /// the worst it can cost is itself.
    /// </remarks>

    internal static class EnemyAiRetargeting
    {
        /// <summary>
        /// Installs the redirect. Called after <c>PatchAll()</c>; failures are contained per method
        /// and reported rather than thrown.
        /// </summary>
        internal static void Apply(Harmony harmony)
        {
            if (harmony == null)
            {
                return;
            }

            try
            {
                if (!ModConfig.Instance.shadeEnemyTargetingEnabled)
                {
                    LogInfo("Enemy AI retargeting: disabled by config, no methods patched");
                    return;
                }

                var prefix = new HarmonyMethod(AccessTools.DeclaredMethod(typeof(EnemyAiRetargeting), nameof(Prefix)));
                var postfix = new HarmonyMethod(AccessTools.DeclaredMethod(typeof(EnemyAiRetargeting), nameof(Postfix)));

                int patched = 0;
                int failed = 0;
                foreach (var method in TargetMethods())
                {
                    try
                    {
                        harmony.Patch(method, prefix, postfix);
                        patched++;
                    }
                    catch
                    {
                        failed++;
                    }
                }

                LogInfo(FormattableString.Invariant(
                    $"Enemy AI retargeting: patched {patched} method(s) across {TargetFields.Count} action type(s), {failed} failed"));
            }
            catch (Exception ex)
            {
                try { LogInfo($"Enemy AI retargeting: could not be applied ({ex.GetType().Name}: {ex.Message})"); }
                catch { }
            }
        }

        /// <summary>
        /// Per action type, the <c>FsmGameObject</c> fields worth examining. Resolved once at patch
        /// time - these actions run every frame, and reflecting over fields per call would not be
        /// affordable.
        /// </summary>
        private static readonly Dictionary<Type, FieldInfo[]> TargetFields = new();

        private static readonly string[] LifecycleMethods = { "OnEnter", "OnUpdate", "OnFixedUpdate", "OnLateUpdate" };

        private static IEnumerable<MethodBase> TargetMethods()
        {
            var methods = new List<MethodBase>();

            // Both assemblies, because they are not the same one: FsmStateAction itself lives in
            // PlayMaker.dll, but every action this cares about is Silksong's own and lives in
            // Assembly-CSharp. Scanning only the base type's assembly finds nothing.
            var assemblies = new HashSet<Assembly>
            {
                typeof(FsmStateAction).Assembly,
                typeof(HeroController).Assembly
            };

            try
            {
                foreach (var type in assemblies.SelectMany(SafeGetTypes))
                {
                    if (type == null || type.IsAbstract || !typeof(FsmStateAction).IsAssignableFrom(type))
                    {
                        continue;
                    }

                    if (!IsEnemyAiAction(type))
                    {
                        continue;
                    }

                    var fields = ResolveTargetFields(type);
                    if (fields.Length == 0)
                    {
                        continue;
                    }

                    bool patchedAny = false;
                    foreach (var name in LifecycleMethods)
                    {
                        var method = AccessTools.DeclaredMethod(type, name);
                        if (method == null || method.IsAbstract)
                        {
                            continue;
                        }

                        methods.Add(method);
                        patchedAny = true;
                    }

                    if (patchedAny)
                    {
                        TargetFields[type] = fields;
                    }
                }
            }
            catch (Exception ex)
            {
                try { LogInfo($"Enemy AI retargeting: failed to enumerate actions ({ex.GetType().Name}: {ex.Message})"); }
                catch { }
            }

            return methods;
        }

        /// <summary>
        /// <c>Assembly.GetTypes()</c> throws outright if any single type fails to load; the partially
        /// populated <c>Types</c> array on the exception is still usable and is all this needs.
        /// </summary>
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

        /// <summary>
        /// Reads PlayMaker's <c>ActionCategoryAttribute</c> without compiling against it. The
        /// attribute exposes its category through a string member whose name has changed between
        /// PlayMaker versions, so this takes the first string it finds rather than naming one.
        /// </summary>
        private static bool IsEnemyAiAction(Type type)
        {
            try
            {
                foreach (var attribute in type.GetCustomAttributes(false))
                {
                    if (attribute == null || attribute.GetType().Name != "ActionCategoryAttribute")
                    {
                        continue;
                    }

                    var attributeType = attribute.GetType();
                    foreach (var property in attributeType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (property.PropertyType == typeof(string) &&
                            string.Equals(property.GetValue(attribute, null) as string, "Enemy AI", StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }

                    foreach (var field in attributeType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (field.FieldType == typeof(string) &&
                            string.Equals(field.GetValue(attribute) as string, "Enemy AI", StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        private static FieldInfo[] ResolveTargetFields(Type type)
        {
            try
            {
                return type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => f.FieldType == typeof(FsmGameObject) &&
                                // Output parameters, not targets - swapping one would be writing to
                                // something the action is about to overwrite anyway.
                                !f.Name.StartsWith("store", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }
            catch
            {
                return Array.Empty<FieldInfo>();
            }
        }

        private static void Prefix(FsmStateAction __instance, out List<KeyValuePair<FsmGameObject, GameObject>> __state)
        {
            __state = null;

            try
            {
                if (!ShadeAggroTargeting.HasEligibleShade())
                {
                    return;
                }

                if (__instance == null || !TargetFields.TryGetValue(__instance.GetType(), out var fields))
                {
                    return;
                }

                var hero = HeroController.UnsafeInstance;
                if (hero == null)
                {
                    return;
                }

                var heroTransform = hero.transform;
                GameObject enemy = null;
                GameObject shadeObject = null;
                bool resolvedEnemy = false;
                bool shouldRetarget = false;

                foreach (var field in fields)
                {
                    if (field.GetValue(__instance) is not FsmGameObject slot)
                    {
                        continue;
                    }

                    var current = slot.Value;
                    if (current == null || !IsHero(current, heroTransform))
                    {
                        continue;
                    }

                    // Only worth asking once per invocation, and only once we know at least one field
                    // actually points at Hornet.
                    if (!resolvedEnemy)
                    {
                        resolvedEnemy = true;
                        enemy = ResolveEnemy(__instance);
                        // Picked per enemy, not once per frame: the nearest Shade differs between them.
                        shadeObject = ShadeAggroTargeting.GetShadeTargetFor(enemy);
                        shouldRetarget = shadeObject != null
                            && ShadeAggroTargeting.ShouldTargetShade(enemy, shadeObject);
                    }

                    if (!shouldRetarget)
                    {
                        return;
                    }

                    __state ??= new List<KeyValuePair<FsmGameObject, GameObject>>(1);
                    __state.Add(new KeyValuePair<FsmGameObject, GameObject>(slot, current));
                    slot.Value = shadeObject;
                }
            }
            catch
            {
                __state = null;
            }
        }

        private static void Postfix(List<KeyValuePair<FsmGameObject, GameObject>> __state)
        {
            if (__state == null)
            {
                return;
            }

            for (int i = 0; i < __state.Count; i++)
            {
                try { __state[i].Key.Value = __state[i].Value; }
                catch { }
            }
        }

        /// <summary>
        /// Matches the hero's own GameObject and anything parented under it, since an FSM target can
        /// legitimately be a child marker on Hornet rather than Hornet herself.
        /// </summary>
        private static bool IsHero(GameObject candidate, Transform heroTransform)
        {
            if (candidate == null || heroTransform == null)
            {
                return false;
            }

            if (ReferenceEquals(candidate, heroTransform.gameObject))
            {
                return true;
            }

            try { return candidate.transform.IsChildOf(heroTransform); }
            catch { return false; }
        }

        private static GameObject ResolveEnemy(FsmStateAction action)
        {
            try
            {
                // Fsm.Owner is the PlayMakerFSM component, not the object it sits on.
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
