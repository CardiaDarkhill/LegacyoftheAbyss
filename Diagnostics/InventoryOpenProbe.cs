#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using InControl;
using UnityEngine;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>
    /// Watches the game's own inventory-open path so "the inventory will not open" resolves to one
    /// of its three possible causes rather than to a guess.
    /// <para>
    /// The press travels: <c>HeroActions.OpenInventory.WasPressed</c> → the <c>Inventory Control</c>
    /// FSM's <c>Closed</c> state → <c>Can Open Inventory?</c> → <c>HeroController.CanOpenInventory()</c>
    /// → <c>Open</c>. From outside, a press that never arrives, a press the FSM never sees, and a
    /// press <c>CanOpenInventory</c> refuses all look identical. This records which of those
    /// happened, with the gate values as they stood at that instant, because by the time a report is
    /// filed the moment is long gone.
    /// </para>
    /// </summary>
    internal static class InventoryOpenProbe
    {
        private const int HistoryLength = 6;

        private static InventoryPaneList? paneList;
        private static PlayMakerFSM? controlFsm;
        private static string lastFsmState = string.Empty;
        private static bool lastInventoryOpen;
        private static float lastOpenedRealtime = -1f;

        private static readonly List<string> presses = new(HistoryLength);
        private static readonly List<string> fsmStates = new(HistoryLength);

        private static int heldFrames;
        private static int tickedFrames;
        private static float lastHeldRealtime = -1f;

        /// <summary>Handed the pane list by the patch that injects the Shade's tab, so nothing has to search for it.</summary>
        internal static void Attach(InventoryPaneList list)
        {
            paneList = list;
            controlFsm = null;
        }

        internal static void Tick(float realtime)
        {
            var handler = InputHandler.UnsafeInstance;
            var actions = handler != null ? handler.inputActions : null;
            if (actions == null)
            {
                return;
            }

            TrackFsmState(realtime);

            // Counted as well as edge-detected. A binding stuck on reads as "no press ever seen",
            // because WasPressed only fires on the transition - and that looks identical to the key
            // not being bound at all unless the held state is recorded next to it.
            tickedFrames++;
            if (actions.OpenInventory != null && actions.OpenInventory.IsPressed)
            {
                heldFrames++;
                lastHeldRealtime = realtime;
            }

            string? pressed = FirstPressedName(actions);
            if (pressed != null)
            {
                Push(presses, string.Format(
                    CultureInfo.InvariantCulture,
                    "t={0:F1} {1} fsm={2} {3}",
                    realtime,
                    pressed,
                    lastFsmState.Length == 0 ? "?" : lastFsmState,
                    DescribeGates(handler)));
            }

            bool open = IsInventoryOpen();
            if (open && !lastInventoryOpen)
            {
                lastOpenedRealtime = realtime;
            }

            lastInventoryOpen = open;
        }

        private static void TrackFsmState(float realtime)
        {
            if (controlFsm == null && paneList != null)
            {
                controlFsm = PlayMakerFSM.FindFsmOnGameObject(paneList.gameObject, "Inventory Control");
            }

            if (controlFsm == null)
            {
                return;
            }

            string state = controlFsm.ActiveStateName ?? string.Empty;
            if (state == lastFsmState)
            {
                return;
            }

            lastFsmState = state;
            Push(fsmStates, string.Format(CultureInfo.InvariantCulture, "t={0:F1} {1}", realtime, state));
        }

        /// <summary>
        /// The first inventory action pressed this frame, or null. Named rather than boolean because
        /// "the inventory key does nothing but the map key works" and "no key does anything" point at
        /// different halves of the path.
        /// </summary>
        private static string? FirstPressedName(HeroActions actions)
        {
            if (actions.OpenInventory != null && actions.OpenInventory.WasPressed) return "openInventory";
            if (actions.OpenInventoryMap != null && actions.OpenInventoryMap.WasPressed) return "openInventoryMap";
            if (actions.OpenInventoryJournal != null && actions.OpenInventoryJournal.WasPressed) return "openInventoryJournal";
            if (actions.OpenInventoryTools != null && actions.OpenInventoryTools.WasPressed) return "openInventoryTools";
            if (actions.OpenInventoryQuests != null && actions.OpenInventoryQuests.WasPressed) return "openInventoryQuests";
            return null;
        }

        /// <summary>
        /// Every gate between a press and an open inventory, as a short flag list. Only the ones
        /// standing in the way are named, so a healthy press reads "canOpen".
        /// </summary>
        private static string DescribeGates(InputHandler? handler)
        {
            var builder = new StringBuilder();
            var hero = HeroController.UnsafeInstance;
            var gm = GameManager.instance;
            var pd = PlayerData.HasInstance ? PlayerData.instance : null;

            if (hero == null)
            {
                return "no hero";
            }

            InventoryOpenTrace.SuppressVerdicts = true;
            try
            {
                builder.Append(hero.CanOpenInventory() ? "canOpen" : "BLOCKED");
            }
            finally
            {
                InventoryOpenTrace.SuppressVerdicts = false;
            }

            Flag(builder, handler != null && !handler.acceptingInput, "!handler.acceptingInput");
            Flag(builder, !InputManager.Enabled, "!InputManager.Enabled");
            Flag(builder, gm != null && gm.isPaused, "paused");
            Flag(builder, gm != null && gm.RespawningHero, "respawning");
            Flag(builder, hero.IsInputBlocked(), "inputBlocked");
            Flag(builder, CheatManager.IsOpen, "cheatManagerOpen");
            Flag(builder, hero.controlReqlinquished, "controlRelinquished");
            Flag(builder, pd != null && pd.disableInventory, "disableInventory");
            Flag(builder, pd != null && pd.disablePause, "disablePause");
            Flag(builder, pd != null && pd.isInventoryOpen, "isInventoryOpen");
            Flag(builder, InteractManager.BlockingInteractable != null, "blockingInteractable");
            Flag(builder, GenericMessageCanvas.IsActive, "messageCanvas");
            Flag(builder, gm != null && !gm.IsGameplayScene(), "!gameplayScene");

            // CanInput broken out rather than flagged. Hornet keeps moving with any of these wrong -
            // LookForInput does not consult them - so "she plays fine, the inventory does not open"
            // is exactly the shape they produce, and the whole question is which one it is.
            if (!hero.CanInput())
            {
                builder.Append(" !canInput(");
                builder.Append("hero.acceptingInput=").Append(hero.acceptingInput);
                builder.Append(" paused=").Append(hero.IsPaused());
                builder.Append(" transition=").Append(hero.transitionState);
                builder.Append(" state=").Append(gm != null ? gm.GameState.ToString() : "?");
                builder.Append(')');
            }

            return builder.ToString();
        }

        private static void Flag(StringBuilder builder, bool condition, string name)
        {
            if (condition)
            {
                builder.Append(' ').Append(name);
            }
        }

        private static bool IsInventoryOpen()
        {
            var pd = PlayerData.HasInstance ? PlayerData.instance : null;
            return pd != null && pd.isInventoryOpen;
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
            var builder = new StringBuilder();

            builder.Append("fsm=");
            builder.Append(controlFsm == null ? "not found" : (lastFsmState.Length == 0 ? "?" : lastFsmState));

            builder.Append(" | last opened=");
            builder.Append(lastOpenedRealtime < 0f
                ? "never this session"
                : lastOpenedRealtime.ToString("F1", CultureInfo.InvariantCulture));

            builder.Append(" | openInventory held ");
            builder.Append(heldFrames.ToString(CultureInfo.InvariantCulture));
            builder.Append('/');
            builder.Append(tickedFrames.ToString(CultureInfo.InvariantCulture));
            builder.Append(" frames, last t=");
            builder.Append(lastHeldRealtime < 0f
                ? "never"
                : lastHeldRealtime.ToString("F1", CultureInfo.InvariantCulture));

            builder.Append(" | presses: ");
            builder.Append(presses.Count == 0 ? "none seen" : string.Join(" ; ", presses));

            builder.Append(" | fsm states: ");
            builder.Append(fsmStates.Count == 0 ? "none seen" : string.Join(" -> ", fsmStates));

            // Sampled once a frame, so a state the FSM enters and leaves inside its own update never
            // shows up here. InventoryOpenTrace watches from inside for that reason.
            builder.Append(" | host active=");
            builder.Append(paneList != null && paneList.gameObject.activeInHierarchy);
            builder.Append(" fsm enabled=");
            builder.Append(controlFsm != null && controlFsm.enabled);

            builder.Append(" | ").Append(InventoryOpenTrace.Describe());

            return builder.ToString();
        }

        internal static void Reset()
        {
            paneList = null;
            controlFsm = null;
            lastFsmState = string.Empty;
            lastInventoryOpen = false;
            lastOpenedRealtime = -1f;
            presses.Clear();
            fsmStates.Clear();
            heldFrames = 0;
            tickedFrames = 0;
            lastHeldRealtime = -1f;
        }
    }
}
