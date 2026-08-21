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
    /// Makes alerted enemies actually come after the Shade, by swapping the target out from under
    /// the enemy-AI actions that read it.
    /// <para>
    /// The obvious reading of this problem - 184 files reference <c>HeroController.instance</c>, so
    /// every enemy script needs rewriting - turns out to be the wrong place to look. Almost none of
    /// those references are enemies locating the player; they are things done <i>to</i> the hero
    /// (damage, cState, invulnerability, input blocking). Silksong's enemy AI is PlayMaker-driven,
    /// and the actions that move an enemy toward something are a single tagged set: PlayMaker's
    /// <c>ActionCategory("Enemy AI")</c>, 63 types, of which the ones that matter here share one
    /// shape - an <c>FsmOwnerDefault</c> for the enemy itself and an <c>FsmGameObject</c> for what it
    /// is chasing, facing, or firing at (<c>ChaseObject*</c>, <c>FaceObject*</c>, <c>DistanceFly*</c>,
    /// <c>FireAtTarget</c>, <c>GetAngleToTarget2D</c>, and so on).
    /// </para>
    /// <para>
    /// So the interception point is that one field. When an enemy-AI action is about to run and its
    /// target is Hornet, and <see cref="ShadeAggroTargeting"/> says this enemy should be going for
    /// the Shade instead, the field is pointed at the Shade for the duration of the call and put back
    /// afterwards. The enemy's own logic is untouched - it chases what it is told to chase.
    /// </para>
    /// <para>
    /// Borrow-and-restore rather than a permanent write, because an <c>FsmGameObject</c> field can be
    /// bound to a shared FSM variable; leaving the Shade in it would corrupt the enemy's own state and
    /// outlive the action. The pair is carried through Harmony's <c>__state</c>, so a restore is
    /// bound to the same invocation that borrowed.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Deliberately <b>not</b> a <c>[HarmonyPatch]</c> class, and so not picked up by
    /// <c>PatchAll()</c>. <c>PatchAll</c> is all-or-nothing: one patch class that throws while being
    /// applied aborts the whole call, and every patch class it had not reached yet silently never
    /// gets installed - which presents as unrelated parts of the mod breaking at once, with nothing
    /// in the log tying them together. This one resolves its own targets by reflection over the game
    /// assembly and patches around a hundred methods, so it is by far the likeliest to throw. It is
    /// applied separately by <see cref="Apply"/> instead, per method, after <c>PatchAll</c> has
    /// finished, so the worst it can cost is itself.
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
                var shadeObject = ShadeAggroTargeting.GetShadeTarget();
                if (shadeObject == null)
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
                        shouldRetarget = ShadeAggroTargeting.ShouldTargetShade(enemy, shadeObject);
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
