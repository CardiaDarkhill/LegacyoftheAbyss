#nullable disable
using System.Collections.Generic;
using System.Text;
using LegacyoftheAbyss.Diagnostics;
using LegacyoftheAbyss.Shade;
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        /// <summary>
        /// Fills the Shade columns of a flight-recorder row.
        /// <para>
        /// Lives on the controller rather than in the diagnostics code so it can read the private
        /// state fields directly. The recorder runs several times a second for the whole session, so
        /// the alternative - reflecting over ~50 private fields per sample - is not something worth
        /// paying for in a shipped build.
        /// </para>
        /// </summary>
        internal void CaptureFlightSample(ref BugReportFlightSample sample)
        {
            sample.ShadePresent = true;

            try
            {
                Vector3 position = transform.position;
                sample.ShadeX = position.x;
                sample.ShadeY = position.y;
            }
            catch
            {
            }

            try
            {
                if (rb != null)
                {
                    Vector2 velocity = rb.linearVelocity;
                    sample.ShadeVelX = velocity.x;
                    sample.ShadeVelY = velocity.y;
                }
            }
            catch
            {
            }

            sample.ShadeHp = GetCurrentHP();
            sample.ShadeMaxHp = GetMaxHP();
            sample.ShadeSoul = shadeSoul;
            sample.ShadeFlags = DescribeStateFlags();
        }

        /// <summary>Everything worth knowing about the Shade at the moment a report is filed.</summary>
        internal BugReportShadeState CaptureDiagnostics()
        {
            var state = new BugReportShadeState { Present = true };

            try
            {
                Vector3 position = transform.position;
                state.X = position.x;
                state.Y = position.y;
            }
            catch
            {
            }

            try
            {
                if (rb != null)
                {
                    Vector2 velocity = rb.linearVelocity;
                    state.VelocityX = velocity.x;
                    state.VelocityY = velocity.y;
                }
            }
            catch
            {
            }

            state.Hp = shadeHP;
            state.MaxHp = shadeMaxHP;
            state.BaseMaxHp = baseShadeMaxHP;
            state.Lifeblood = shadeLifeblood;
            state.LifebloodMax = shadeLifebloodMax;
            state.Soul = shadeSoul;
            state.SoulMax = shadeSoulMax;
            state.CanTakeDamage = canTakeDamage;
            state.AssistMode = assistModeEnabled;
            state.Facing = facing;
            state.Flags = DescribeStateFlags();
            state.HardLeashTimer = hardLeashTimer;
            state.HazardCooldown = hazardCooldown;
            state.SceneProtectionTimer = sceneProtectionTimer;
            state.TeleportCooldownTimer = teleportCooldownTimer;
            state.FireTimer = fireTimer;
            state.NailTimer = nailTimer;
            state.FocusTimer = focusTimer;
            state.MoveSpeed = moveSpeed;
            state.AiEnabled = aiEnabled;
            state.AiReason = aiPlan.Reason.ToString();
            state.AiAction = DescribeAiAction(aiPlan.Action);
            state.AiTargetId = aiPlan.TargetId;
            state.AiTargetsInRange = aiTargetCount;
            state.AiCommandState = aiCommandState.ToString();
            if (aiCommandState == ShadeAiCommandState.Holding)
            {
                state.AiCommandX = aiCommandPoint.x;
                state.AiCommandY = aiCommandPoint.y;
            }

            try
            {
                state.Skin = ShadeSkinManager.SelectedSkinId;
            }
            catch
            {
            }

            try
            {
                var charms = ShadeRuntime.Charms;
                if (charms != null)
                {
                    var equipped = new List<string>();
                    foreach (var definition in charms.GetEquippedDefinitions())
                    {
                        if (definition != null)
                        {
                            equipped.Add(definition.Id);
                        }
                    }

                    state.EquippedCharms = equipped.ToArray();
                    state.NotchesUsed = charms.UsedNotches;
                    state.NotchCapacity = charms.NotchCapacity;
                }
            }
            catch
            {
            }

            return state;
        }

        /// <summary>
        /// The Shade's mutually-overlapping mode flags as a compact <c>|</c>-joined list of whichever
        /// are currently set. Only the true ones appear, which keeps a flight-recorder cell short in
        /// the common case where the Shade is just following Hornet.
        /// </summary>
        private string DescribeStateFlags()
        {
            var builder = new StringBuilder();
            Append(builder, isInactive, "inactive");
            Append(builder, isSpawning, "spawning");
            Append(builder, isDying, "dying");
            Append(builder, isFocusing, "focusing");
            Append(builder, isCastingSpell, "casting");
            Append(builder, isChannelingTeleport, "teleporting");
            Append(builder, isSprinting, "sprinting");
            Append(builder, inHardLeash, "hardLeash");
            Append(builder, hornetControlsLocked, "hornetLocked");
            Append(builder, hiddenForScriptedHold, "hiddenForHold");
            Append(builder, assistModeEnabled, "assist");
            Append(builder, !canTakeDamage, "invuln");
            Append(builder, sceneProtectionActive, "sceneProtected");
            Append(builder, baldurShellActive, "baldurShell");
            Append(builder, sharpShadowDashActive, "sharpShadowDash");
            Append(builder, voidHeartEvadeActive, "voidHeartEvade");

            // Named rather than a bare flag: "the AI is on" and "the AI is on and has decided there
            // is nothing to fight" are different reports, and the flight recorder is where the
            // difference has to be visible.
            string aiState = DescribeAiState();
            if (aiState != null)
            {
                if (builder.Length > 0)
                {
                    builder.Append('|');
                }

                builder.Append(aiState);
            }

            return builder.Length == 0 ? "idle" : builder.ToString();
        }

        private static void Append(StringBuilder builder, bool condition, string name)
        {
            if (!condition)
            {
                return;
            }

            if (builder.Length > 0)
            {
                builder.Append('|');
            }

            builder.Append(name);
        }
    }
}
#nullable restore
