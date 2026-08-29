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

        internal static void NoteListenerRan(HeroActions? actions)
        {
            listenerRuns++;
            lastListenerRun = Time.realtimeSinceStartup;

            if (actions == null)
            {
                return;
            }

            var pressed = InventoryPaneInput.GetInventoryInputPressed(actions);
            if (pressed == InventoryPaneList.PaneTypes.None)
            {
                return;
            }

            Push(sightings, string.Format(
                CultureInfo.InvariantCulture,
                "t={0:F1} {1}",
                lastListenerRun,
                pressed));
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
                "listener ran {0}x (last t={1}) | saw: {2} | CanOpenInventory asked: {3}",
                listenerRuns,
                lastListenerRun < 0f ? "never" : lastListenerRun.ToString("F1", CultureInfo.InvariantCulture),
                sightings.Count == 0 ? "nothing" : string.Join(" ; ", sightings),
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
