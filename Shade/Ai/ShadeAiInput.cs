#nullable enable

using System;
using UnityEngine;

namespace LegacyoftheAbyss.Shade.Ai
{
    /// <summary>
    /// The AI drives the Shade by synthesising the same inputs a second player would give it, which
    /// is why Stage 1 needed an AI driver and no new movement, combat or physics code: every
    /// existing handler - <c>CaptureMovementInput</c>, <c>HandleNailAttack</c>, <c>HandleFire</c>,
    /// <c>HandleShriek</c>, <c>HandleDescendingDark</c> - reads <see cref="ShadeInput"/> and cannot
    /// tell the difference.
    /// <para>
    /// Only the actions listed in <see cref="Driven"/> are intercepted. Assist mode, the AI toggle
    /// itself, teleport and the developer keys always read real hardware, so the player can never be
    /// locked out of the controls that turn this off.
    /// </para>
    /// <para>
    /// Every published value carries the frame it was published on and is ignored on any other
    /// frame. A Shade that is destroyed, disabled or sitting behind an early return in <c>Update</c>
    /// therefore cannot leave a key stuck down - which matters most for the movement actions, since
    /// the pause-menu panes navigate on the same ones.
    /// </para>
    /// </summary>
    internal static class ShadeAiInput
    {
        private static readonly int ActionCount = Enum.GetValues(typeof(ShadeAction)).Length;

        /// <summary>Actions the AI supplies. Anything else falls through to the player's own bindings.</summary>
        private static readonly ShadeAction[] Driven =
        {
            ShadeAction.MoveLeft,
            ShadeAction.MoveRight,
            ShadeAction.MoveUp,
            ShadeAction.MoveDown,
            ShadeAction.Fire,
            ShadeAction.Nail,
            ShadeAction.NailUp,
            ShadeAction.NailDown,
            ShadeAction.Sprint,
            ShadeAction.Focus
        };

        /// <summary>
        /// Actions the AI drives that it does not itself supply. Teleport is the whole list: the AI
        /// never asks for one, and a player pressing it would drag the Shade out from under the plan
        /// in flight.
        /// </summary>
        private static readonly ShadeAction[] LockedButNotDriven = { ShadeAction.Teleport };

        /// <summary>
        /// The Shade's own gameplay controls, which do nothing at all while an AI is driving it -
        /// everything the AI supplies, plus <see cref="LockedButNotDriven"/>. Derived rather than
        /// written out, because two hand-written lists of the same actions is how the two came to
        /// disagree about which ones the AI owns.
        /// <para>
        /// This replaced a "player takes over for a moment" rule, which stopped making sense the
        /// moment Hornet was allowed both devices again: the Shade's bindings then share a pad with
        /// hers, so simply walking would suspend the AI and drag the Shade along with her. Locking
        /// them out is also what was asked for - there is no second player to take over.
        /// </para>
        /// <para>
        /// Deliberately not locked: the command reticle, which is how the player steers the AI
        /// rather than the Shade, and the developer keys. Charm-menu navigation reuses the movement
        /// actions, so the lockout stands down whenever a Shade menu is open - see
        /// <see cref="Suppressed"/>.
        /// </para>
        /// </summary>
        private static readonly bool[] LockedWhileDriving = BuildLockedTable();

        private static readonly bool[] DrivenTable = BuildTable(Driven);

        private static readonly float[] Values = new float[ActionCount];
        private static readonly bool[] Pressed = new bool[ActionCount];
        private static int publishedFrame = -1;

        private static bool[] BuildTable(ShadeAction[] actions)
        {
            var table = new bool[ActionCount];
            foreach (var action in actions)
            {
                table[(int)action] = true;
            }

            return table;
        }

        private static bool[] BuildLockedTable()
        {
            var table = BuildTable(Driven);
            foreach (var action in LockedButNotDriven)
            {
                table[(int)action] = true;
            }

            return table;
        }

        /// <summary>True while a plan published this frame is standing in for the player.</summary>
        internal static bool Active => publishedFrame == Time.frameCount;

        internal static void Clear()
        {
            publishedFrame = -1;
            Array.Clear(Values, 0, Values.Length);
            Array.Clear(Pressed, 0, Pressed.Length);
        }

        /// <summary>
        /// Turns a plan into one frame of held/pressed input. Movement always publishes, so the
        /// Shade keeps steering even on a frame it has no attack to make.
        /// </summary>
        internal static void Publish(in ShadeAiPlan plan, bool allowAttacks)
        {
            Array.Clear(Values, 0, Values.Length);
            Array.Clear(Pressed, 0, Pressed.Length);
            publishedFrame = Time.frameCount;

            Vector2 move = plan.Move;
            Set(ShadeAction.MoveLeft, Mathf.Clamp01(-move.x));
            Set(ShadeAction.MoveRight, Mathf.Clamp01(move.x));
            Set(ShadeAction.MoveDown, Mathf.Clamp01(-move.y));
            Set(ShadeAction.MoveUp, Mathf.Clamp01(move.y));
            Set(ShadeAction.Sprint, plan.Sprint ? 1f : 0f);

            if (!allowAttacks)
            {
                return;
            }

            switch (plan.Action)
            {
                case ShadeAiAction.SlashSide:
                    Press(ShadeAction.Nail);
                    break;
                case ShadeAiAction.SlashUp:
                    Press(ShadeAction.NailUp);
                    break;
                case ShadeAiAction.SlashDown:
                    Press(ShadeAction.NailDown);
                    break;
                case ShadeAiAction.Fireball:
                    // HandleFire refuses to fire while either vertical direction is held, because
                    // that combination is how a player aims a spell. Drop the vertical steering for
                    // this frame; the Shade is about to stop moving for the cast anyway.
                    Set(ShadeAction.MoveUp, 0f);
                    Set(ShadeAction.MoveDown, 0f);
                    Press(ShadeAction.Fire);
                    break;
                case ShadeAiAction.Shriek:
                    Set(ShadeAction.MoveDown, 0f);
                    Set(ShadeAction.MoveUp, 1f);
                    Press(ShadeAction.Fire);
                    break;
                case ShadeAiAction.DescendingDark:
                    Set(ShadeAction.MoveUp, 0f);
                    Set(ShadeAction.MoveDown, 1f);
                    Press(ShadeAction.Fire);
                    break;
                case ShadeAiAction.Focus:
                    // A channel, not a press: HandleFocus cancels the moment the action stops being
                    // held, so this has to be republished every frame until the heal completes.
                    Set(ShadeAction.MoveLeft, 0f);
                    Set(ShadeAction.MoveRight, 0f);
                    Set(ShadeAction.MoveUp, 0f);
                    Set(ShadeAction.MoveDown, 0f);
                    Set(ShadeAction.Sprint, 0f);
                    Set(ShadeAction.Focus, 1f);
                    break;
            }
        }

        internal static bool TryGetValue(ShadeAction action, out float value)
        {
            value = 0f;
            if (!Active || !IsDriven(action))
            {
                return false;
            }

            value = Values[(int)action];
            return true;
        }

        internal static bool TryGetHeld(ShadeAction action, out bool held)
        {
            held = false;
            if (!TryGetValue(action, out float value))
            {
                return false;
            }

            held = value > 0.5f;
            return true;
        }

        internal static bool TryGetPressed(ShadeAction action, out bool pressed)
        {
            pressed = false;
            if (!Active || !IsDriven(action))
            {
                return false;
            }

            pressed = Pressed[(int)action];
            return true;
        }

        /// <summary>
        /// Whether a physical press of this action should be ignored because an AI is driving.
        /// <para>
        /// Answered here rather than per-binding in <c>ShadeInput.ShouldSuppressOption</c> because it
        /// is a question about the action, not about which key is on it.
        /// </para>
        /// </summary>
        internal static bool Suppressed(ShadeAction action)
        {
            if (!IsLockedWhileDriving(action))
            {
                return false;
            }

            try
            {
                if (!LegacyHelper.ShadeController.ShadeAiDriving)
                {
                    return false;
                }

                // The charm menu and the settings screens navigate on these same actions, so the
                // lockout has to stand down while one of them owns the input.
                return !LegacyHelper.ShadeController.ShadeAiUiIsOpen();
            }
            catch
            {
                return false;
            }
        }

        private static bool IsLockedWhileDriving(ShadeAction action)
        {
            int index = (int)action;
            return index >= 0 && index < LockedWhileDriving.Length && LockedWhileDriving[index];
        }

        private static bool IsDriven(ShadeAction action)
        {
            int index = (int)action;
            return index >= 0 && index < DrivenTable.Length && DrivenTable[index];
        }

        private static void Set(ShadeAction action, float value)
        {
            Values[(int)action] = value;
        }

        private static void Press(ShadeAction action)
        {
            Values[(int)action] = 1f;
            Pressed[(int)action] = true;
        }
    }
}
