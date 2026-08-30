#nullable disable
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

// Keeps the companion out of the world's interaction ranges. See InteractableBase_AddInside_IgnoreCompanion.
public partial class LegacyHelper
{
    /// <summary>
    /// The companion may be noticed, but it may not interact.
    /// <para>
    /// The Shade carries a proxy collider that copies Hornet's layer and tag on purpose - that is
    /// the whole reason enemies see it. <c>InteractableBase</c> admits a collider to its range on
    /// one test, <c>layer != 9</c>, so the proxy walks straight in: parking the Knight on a bench
    /// left Hornet able to sit at it from across the room, teleporting her to it.
    /// </para>
    /// <para>
    /// Patched at <c>AddInside</c>/<c>LocalAddInside</c> because that pair is where every route in
    /// converges - the interactable's own trigger messages and a child detector's forwarded ones -
    /// and because <c>InteractableBase</c> is the game's own mark for "the player can act on this".
    /// Benches, levers, doors and NPCs all carry one; nothing that merely wants to notice the hero
    /// does, so enemy detection is untouched.
    /// </para>
    /// <para>
    /// An earlier attempt patched PlayMaker's <c>Fsm</c> trigger callbacks instead, on the strength
    /// of the bench's FSM showing up in the proxy's trigger log. It bound correctly and did nothing:
    /// the bench's range is C#, and the FSM only ever hears about it afterwards.
    /// </para>
    /// </summary>
    [HarmonyPatch]
    internal static class InteractableBase_AddInside_IgnoreCompanion
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            string[] names = { "AddInside", "LocalAddInside" };
            int found = 0;

            foreach (string name in names)
            {
                var method = AccessTools.DeclaredMethod(typeof(InteractableBase), name, new[] { typeof(Collider2D) });
                if (method != null)
                {
                    found++;
                    yield return method;
                }
            }

            if (found != names.Length)
            {
                LogWarning($"Companion interaction blocker: only {found} of {names.Length} InteractableBase range methods resolved; the companion can still stand in for Hornet at benches and levers.");
            }
        }

        private static bool Prefix(Collider2D col)
        {
            return col == null || col.GetComponentInParent<ShadeController>() == null;
        }
    }

    /// <summary>
    /// The same rule at the other door.
    /// <para>
    /// <c>TransitionPoint</c> is an <c>InteractableBase</c>, but it does not go through the range
    /// bookkeeping above - its own trigger callbacks test <c>layer == 9</c> and call
    /// <c>TryDoTransition</c> straight away. The companion is on layer 9 on purpose, so it was
    /// walking Hornet back out of rooms she had just entered: she arrives at the doorway, the
    /// companion is set down beside her inside the same trigger, and the trigger fires.
    /// </para>
    /// <para>
    /// Patched at <c>TryDoTransition</c> rather than at the two callbacks, because that is where
    /// both of them meet and it is the only thing either of them does with the collider.
    /// </para>
    /// </summary>
    [HarmonyPatch]
    internal static class TransitionPoint_TryDoTransition_IgnoreCompanion
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var method = AccessTools.DeclaredMethod(typeof(TransitionPoint), "TryDoTransition", new[] { typeof(Collider2D) });
            if (method == null)
            {
                LogWarning("Companion interaction blocker: TransitionPoint.TryDoTransition not found; the companion can still walk Hornet through doors.");
                yield break;
            }

            yield return method;
        }

        private static bool Prefix(Collider2D heroCollider)
        {
            return heroCollider == null || heroCollider.GetComponentInParent<ShadeController>() == null;
        }
    }

    /// <summary>
    /// Gathering Swarm, run through the game's own rosary magnet rather than a second one.
    /// <para>
    /// Every <c>CurrencyObjectBase</c> already knows how to fly to the hero; it asks
    /// <c>MagnetToolIsEquipped</c> once, from <c>OnEnable</c>, and starts its <c>Getter</c>
    /// coroutine if the answer is yes. Answering yes for the charm gets the real behaviour -
    /// the start delay, the little lift, the effect object, the attraction curve - none of which
    /// a hand-written pull reproduced. The companion is deliberately not the destination: the
    /// game's magnet drags to <c>HeroController.instance</c>, and rosaries gathered to the
    /// companion would be gathered to something that cannot spend them.
    /// </para>
    /// <para>
    /// Pooled pickups are re-enabled on every drop, so anything dropped after the charm goes on
    /// picks this up by itself. <see cref="LegacyoftheAbyss.Shade.ShadeCharmSummons"/> is not
    /// involved: there is nothing to summon.
    /// </para>
    /// </summary>
    [HarmonyPatch]
    internal static class CurrencyObjectBase_MagnetToolIsEquipped_GatheringSwarm
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var method = AccessTools.DeclaredMethod(typeof(CurrencyObjectBase), "MagnetToolIsEquipped", System.Type.EmptyTypes);
            if (method == null)
            {
                LogWarning("Gathering Swarm: CurrencyObjectBase.MagnetToolIsEquipped did not resolve; the charm will not draw rosaries in.");
                yield break;
            }

            yield return method;
        }

        private static void Postfix(ref bool __result)
        {
            if (__result)
            {
                return;
            }

            __result = GatheringSwarmActive;
        }
    }

    /// <summary>
    /// Whether any companion is wearing Gathering Swarm. Asked per pickup rather than pushed, so
    /// a companion appearing or changing its loadout needs no bookkeeping here.
    /// </summary>
    internal static bool GatheringSwarmActive
    {
        get
        {
            try
            {
                var instances = ShadeController.ActiveInstances;
                if (instances == null)
                {
                    return false;
                }

                foreach (var instance in instances)
                {
                    if (instance != null && instance.HasGatheringSwarm)
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }
    }
}
