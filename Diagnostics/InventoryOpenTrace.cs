#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>
    /// The two points a press has to pass through between the button and an open inventory, watched
    /// from the inside.
    /// <para>
    /// <see cref="InventoryOpenProbe"/> established that the press arrives, that
    /// <c>CanOpenInventory()</c> answers yes when sampled, and that the <c>Inventory Control</c> FSM
    /// sits in <c>Closed</c> - which is the state that is supposed to be listening. That leaves
    /// three possibilities the probe cannot separate, because it samples once a frame from outside
    /// and the FSM does its whole round trip inside its own update: the listening action never runs,
    /// it runs but does not see the press, or it fires and the gate refuses it in that same frame.
    /// </para>
    /// <para>
    /// So this counts <c>ListenForInventoryShortcut.OnUpdate</c> actually running, records what it
    /// saw when it saw anything, and records every answer <c>CanOpenInventory</c> gives to whoever
    /// asks. One of those three lines will be missing, and that names the fault.
    /// </para>
    /// </summary>
    internal static class InventoryOpenTrace
    {
        private const int HistoryLength = 5;

        private static long listenerRuns;
        private static float lastListenerRun = -1f;
        private static readonly List<string> sightings = new(HistoryLength);
        private static readonly List<string> verdicts = new(HistoryLength);
        private static readonly List<string> outcomes = new(HistoryLength);

        /// <summary>
        /// Every FSM running the listener, by owner. More than one means the press is being fired
        /// into a duplicate inventory while the live one never hears it - which the counters alone
        /// cannot show, because a duplicate polls just as busily as the real thing.
        /// </summary>
        private static readonly HashSet<string> listenerOwners = new(StringComparer.Ordinal);

        /// <summary>True between prefix and postfix when the listener saw a press this call.</summary>
        private static bool sawPressThisCall;

        internal static void NoteListenerRan(HeroActions? actions)
        {
            listenerRuns++;
            lastListenerRun = Time.realtimeSinceStartup;
            sawPressThisCall = false;

            if (actions == null)
            {
                return;
            }

            var pressed = InventoryPaneInput.GetInventoryInputPressed(actions);
            if (pressed == InventoryPaneList.PaneTypes.None)
            {
                return;
            }

            sawPressThisCall = true;
            Push(sightings, string.Format(
                CultureInfo.InvariantCulture,
                "t={0:F1} {1}",
                lastListenerRun,
                pressed));
        }

        /// <summary>
        /// Where the FSM ended up on the same call that saw the press. PlayMaker switches state
        /// inside <c>Fsm.Event</c>, so a state still reading <c>Closed</c> here means the event went
        /// nowhere - the transition is absent, or the event itself is null.
        /// </summary>
        internal static void NoteListenerFinished(string ownerPath, string fsmName, string stateAfter, string? eventName)
        {
            listenerOwners.Add(fsmName + " on " + ownerPath);

            if (!sawPressThisCall)
            {
                return;
            }

            sawPressThisCall = false;
            Push(outcomes, string.Format(
                CultureInfo.InvariantCulture,
                "t={0:F1} fired '{1}' -> {2}",
                Time.realtimeSinceStartup,
                eventName ?? "<null event>",
                string.IsNullOrEmpty(stateAfter) ? "?" : stateAfter));
        }

        /// <summary>
        /// Set while the probe asks <c>CanOpenInventory</c> itself. Without it the probe's own call
        /// lands in the history beside the FSM's, and "the FSM never asked" - the single most useful
        /// thing this can say - becomes unreadable.
        /// </summary>
        internal static bool SuppressVerdicts;

        internal static void NoteOpenVerdict(bool canOpen)
        {
            if (SuppressVerdicts)
            {
                return;
            }

            Push(verdicts, string.Format(
                CultureInfo.InvariantCulture,
                "t={0:F1} {1}",
                Time.realtimeSinceStartup,
                canOpen ? "yes" : "NO"));
        }

        private static void Push(List<string> history, string entry)
        {
            history.Add(entry);
            if (history.Count > HistoryLength)
            {
                history.RemoveAt(0);
            }
        }

        internal static string Describe()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "listener ran {0}x (last t={1}) on [{2}] | saw: {3} | after firing: {4} | CanOpenInventory asked: {5}",
                listenerRuns,
                lastListenerRun < 0f ? "never" : lastListenerRun.ToString("F1", CultureInfo.InvariantCulture),
                listenerOwners.Count == 0 ? "nothing" : string.Join(" + ", listenerOwners),
                sightings.Count == 0 ? "nothing" : string.Join(" ; ", sightings),
                outcomes.Count == 0 ? "n/a" : string.Join(" ; ", outcomes),
                verdicts.Count == 0 ? "never" : string.Join(" ; ", verdicts));
        }
    }

    /// <summary>
    /// The PlayMaker action the <c>Closed</c> state polls with. If this never runs, the FSM is not
    /// updating and no amount of correct input will reach it.
    /// </summary>
    [HarmonyPatch]
    internal static class ListenForInventoryShortcut_OnUpdate_Trace
    {
        private static readonly FieldInfo? HandlerField =
            AccessTools.Field(typeof(ListenForInventoryShortcut), "inputHandler");

        private static IEnumerable<MethodBase> TargetMethods()
        {
            var method = AccessTools.DeclaredMethod(typeof(ListenForInventoryShortcut), "OnUpdate");
            if (method == null)
            {
                LegacyHelper.LogWarning("Inventory open trace disabled: ListenForInventoryShortcut.OnUpdate not found.");
                yield break;
            }

            yield return method;
        }

        private static void Prefix(ListenForInventoryShortcut __instance)
        {
            // Read through the action's own handler field rather than InputHandler.UnsafeInstance:
            // the whole question is whether what it is looking at agrees with what everything else
            // sees, and substituting a different instance here would hide exactly that.
            var handler = HandlerField?.GetValue(__instance) as InputHandler;
            InventoryOpenTrace.NoteListenerRan(handler != null ? handler.inputActions : null);
        }

        private static void Postfix(ListenForInventoryShortcut __instance)
        {
            var fsm = __instance.Fsm;
            var owner = fsm?.GameObject;
            InventoryOpenTrace.NoteListenerFinished(
                owner != null ? DescribePath(owner.transform) : "<no owner>",
                fsm?.Name ?? "?",
                fsm?.ActiveStateName ?? "?",
                __instance.WasPressed?.Name);
        }

        private static string DescribePath(Transform node)
        {
            var builder = new System.Text.StringBuilder(node.name);
            for (var parent = node.parent; parent != null; parent = parent.parent)
            {
                builder.Insert(0, '/').Insert(0, parent.name);
            }

            return builder.ToString();
        }
    }

    [HarmonyPatch]
    internal static class HeroController_CanOpenInventory_Trace
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var method = AccessTools.DeclaredMethod(typeof(HeroController), "CanOpenInventory");
            if (method == null)
            {
                LegacyHelper.LogWarning("Inventory open trace disabled: HeroController.CanOpenInventory not found.");
                yield break;
            }

            yield return method;
        }

        private static void Postfix(bool __result) => InventoryOpenTrace.NoteOpenVerdict(__result);
    }
}
