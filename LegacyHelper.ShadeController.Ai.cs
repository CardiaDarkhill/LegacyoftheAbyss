#nullable disable
using System;
using System.Collections.Generic;
using LegacyoftheAbyss.Shade.Ai;
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        /// <summary>
        /// Shortest gap between two <c>shade-ai</c> events that are not a cast. Sustained melee
        /// alternates between "swinging" and "waiting on the nail cooldown" several times a second,
        /// and a ring full of that alternation is a ring with no room for the decision that actually
        /// explains a report.
        /// </summary>
        private const float AiEventMinInterval = 0.5f;

        /// <summary>
        /// How far around itself the Shade looks for things that would hurt it. Wide enough to see an
        /// attack before standing in it, tight enough that one overlap query stays cheap.
        /// </summary>
        private const float AiThreatScanRadius = 9f;

        /// <summary>Bounds on the radius a single damaging volume is reduced to.</summary>
        private const float AiThreatMinRadius = 0.25f;

        private const float AiThreatMaxRadius = 8f;

        /// <summary>How long after landing Hornet still counts as airborne for escort placement.</summary>
        private const float AiAirborneHoldSeconds = 0.25f;

        /// <summary>
        /// How many times the navigator may turn round before a standing order is treated as
        /// unreachable. Each one costs it more than a second of trying, so three is several seconds
        /// of visibly getting nowhere - long enough not to fire on an enemy briefly in the way.
        /// </summary>
        private const int AiStuckStreakLimit = 3;

        /// <summary>
        /// Whether an AI is actually driving this companion. The Knight is excluded outright: the
        /// AI steers by synthesising input, which works for a Shade because it flies anywhere in a
        /// straight line, and a walking body needs jump planning the brain does not have. The
        /// player's setting is kept rather than cleared, so it returns when the Shade does.
        /// </summary>
        internal bool ShadeAiEnabled => aiEnabled && !UsesGroundedMovement;

        /// <summary>
        /// Whether an AI is driving the Shade in this scene. Falls back to config when no Shade is
        /// alive, so the answer does not flicker across a scene change.
        /// </summary>
        internal static bool ShadeAiDriving
        {
            get
            {
                try
                {
                    var shade = PrimaryInstance;
                    return shade != null ? shade.ShadeAiEnabled : ModConfig.Instance.shadeAiEnabled;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Reads the AI's starting state out of config. Called from <c>Start</c>, so a save loaded
        /// with the AI left on comes back with it on.
        /// </summary>
        private void InitializeShadeAi()
        {
            aiPlan = ShadeAiPlan.Idle(ShadeAiReason.Suspended);
            aiLastEventKey = int.MinValue;

            try
            {
                if (ModConfig.Instance.shadeAiEnabled)
                {
                    SetShadeAiEnabled(true, persist: false);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Switches the driver on or off. <paramref name="persist"/> is false when this is restoring
        /// a state config already holds, so startup does not rewrite the file it just read.
        /// <para>
        /// This deliberately does not touch assist mode. Forcing the Shade invincible while the AI
        /// drives it means the AI plays the game for the player - shredding everything, punishable
        /// for none of it. The AI fights on the same terms the player does, and anyone who wants an
        /// invincible Shade can turn assist mode on themselves.
        /// </para>
        /// </summary>
        internal void SetShadeAiEnabled(bool enabled, bool persist)
        {
            if (aiEnabled == enabled)
            {
                return;
            }

            aiEnabled = enabled;
            aiBrain.Reset();
            aiScanner.Reset();
            aiPlan = ShadeAiPlan.Idle(ShadeAiReason.Suspended);
            aiEngaged = false;
            aiTargetCount = 0;
            aiThreatCount = 0;
            aiFacingOverride = 0;
            aiLastNailTime = 0f;
            aiStuckStreak = 0;
            aiHornetAirborneHold = 0f;
            aiNavigator.Reset();
            ClearShadeAiCommand("ai toggled");
            ShadeAiInput.Clear();

            // With nobody holding the Shade's controls, the two-player device split has nothing to
            // serve, so Hornet goes back to answering every device at once - and back to the split
            // the moment the AI is switched off again.
            try { HornetInput.RefreshHornetDeviceBindings(); }
            catch { }

            if (persist)
            {
                try
                {
                    ModConfig.Instance.shadeAiEnabled = enabled;
                    ModConfig.Save();
                }
                catch
                {
                }
            }

            LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
                "shade-ai",
                enabled ? "AI enabled" : "AI disabled",
                FormattableString.Invariant($"assistMode={assistModeEnabled} canTakeDamage={canTakeDamage} persist={persist}"));

            if (ModConfig.Instance.logShade)
            {
                LegacyHelper.LogInfo(enabled ? "Shade AI enabled." : "Shade AI disabled.");
            }

            PushShadeStatsToHud(suppressDamageAudio: true);
        }

        /// <summary>
        /// One frame of the driver: assemble what the brain is allowed to see, ask it what to do,
        /// and publish the answer as input. Runs immediately before <c>CaptureMovementInput</c> so
        /// everything downstream - movement, facing, slashes, spells - consumes it without knowing
        /// it did not come from a player.
        /// </summary>
        private void UpdateShadeAi()
        {
            if (!ShadeAiEnabled)
            {
                if (aiEngaged)
                {
                    aiEngaged = false;
                    aiTargetCount = 0;
                    aiThreatCount = 0;
                    aiScanner.Reset();
                    aiBrain.Reset();
                }

                aiFacingOverride = 0;
                return;
            }

            ModConfig config;
            try
            {
                config = ModConfig.Instance;
            }
            catch
            {
                return;
            }

            bool canDrive = hornetTransform != null
                && !hornetControlsLocked
                && !inHardLeash
                && !isChannelingTeleport
                && !isInactive
                && !isDying
                && !isSpawning
                && damageStaggerTimer <= 0f
                && !ShadeAiUiIsOpen();

            if (!canDrive)
            {
                aiEngaged = false;
                aiFacingOverride = 0;
                aiPlan = ShadeAiPlan.Idle(ShadeAiReason.Suspended);
                RecordShadeAiDecision();
                return;
            }

            var tuning = ShadeAiTuning.FromConfig(config);
            Vector2 shadePosition = rb ? rb.position : (Vector2)transform.position;
            Vector2 hornetPosition = hornetTransform.position;
            float now = Time.time;

            var targets = aiScanner.Collect(
                shadePosition,
                tuning.EngageRadius * 2f,
                GetAiSpellWorthHealth(tuning),
                now,
                config.shadeAiScanIntervalSeconds,
                config.shadeAiSpellMinTargetHealth);
            aiTargetCount = targets.Count;

            var threats = CollectAiThreats(shadePosition, config);
            aiThreatCount = threats.Count;

            bool hornetAirborne = UpdateAiHornetAirborne();
            float nailInterval = GetAiNailInterval(config);
            int soulReserve = GetAiSoulReserve(tuning);
            aiSoulReserve = soulReserve;

            var snapshot = new ShadeAiSnapshot
            {
                Time = now,
                ShadePosition = shadePosition,
                HornetPosition = hornetPosition,
                Facing = facing,
                HornetFacing = GetHornetFacing(facing),
                HornetAirborne = hornetAirborne,
                SoftLeashRadius = softLeashRadius,
                MoveSpeed = moveSpeed,
                NailInterval = nailInterval,
                Soul = shadeSoul,
                SoulReserve = soulReserve,
                ProjectileSoulCost = projectileSoulCost,
                ShriekSoulCost = shriekSoulCost,
                QuakeSoulCost = quakeSoulCost,
                FocusSoulCost = focusSoulCost,
                ProjectileUnlocked = IsProjectileUnlocked(),
                ShriekUnlocked = IsShriekUnlocked(),
                DescendingDarkUnlocked = IsDescendingDarkUnlocked(),
                NailReady = IsAiNailReady(nailInterval, now),
                FireReady = fireTimer <= 0f,
                ShriekReady = shriekTimer <= 0f,
                QuakeReady = quakeTimer <= 0f,
                CanFocusHeal = CanAiFocusHeal(config),
                IsFocusing = isFocusing,
                FocusHealRange = focusHealRange,
                SelfHealthFraction = GetAiSelfHealthFraction(),
                HornetHealthFraction = GetHornetHealthFraction(),
                CanTakeDamage = canTakeDamage,
                HasCommand = TryGetShadeAiCommand(out Vector2 commandPoint),
                CommandPoint = commandPoint,
                Tuning = tuning,
                Targets = targets,
                Threats = threats
            };

            aiPlan = aiBrain.Decide(snapshot);
            aiEngaged = aiPlan.HasTarget;

            if (!aiNavigator.LastPathBlocked)
            {
                aiStuckStreak = 0;
            }

            // The brain has no idea where the walls are, so a destination it pushed clear of a hitbox
            // can land inside one. Correct it before steering rather than letting the Shade grind
            // against the wall in front of a place it cannot stand.
            float bodyRadius = GetAiBodyRadius();
            aiPlan.DesiredPosition = ShadeAiNavigator.PullBackToStandable(
                shadePosition,
                aiPlan.DesiredPosition,
                point => ShadeAiTerrain.PointBlocked(point, bodyRadius));

            ApplyShadeAiNavigation(shadePosition, now, config);
            CheckShadeAiStuck();

            // Mirrors the gate the player's own attack inputs already pass through in Update: a
            // press published during a cast would be swallowed anyway, and publishing it would only
            // make the event log claim an attack that never happened. Focus is the exception - it is
            // a channel that sets isCastingSpell itself and has to keep being held to continue.
            bool allowAttacks = !isCastingSpell || (isFocusing && aiPlan.Action == ShadeAiAction.Focus);
            ShadeAiInput.Publish(aiPlan, allowAttacks);

            if (allowAttacks && IsAiNailAction(aiPlan.Action))
            {
                aiLastNailTime = now;
            }

            // Facing normally follows horizontal movement, which is wrong for a driver: the strike
            // point sits between the Shade and the enemy, so the last half-unit of the approach is
            // spent moving *away* from the target, and a Shade that took its facing from that would
            // swing at nothing. Set here so the slash this frame already has it, and latched in
            // aiFacingOverride so HandleMovementAndFacing does not undo it in FixedUpdate.
            aiFacingOverride = aiPlan.FaceX;
            if (aiPlan.FaceX != 0)
            {
                facing = aiPlan.FaceX;
                try
                {
                    if (sr != null)
                    {
                        sr.flipX = facing == 1;
                    }
                }
                catch
                {
                }
            }

            RecordShadeAiDecision();
        }

        /// <summary>
        /// Whether Hornet counts as airborne for escort placement, with a short hold past the
        /// landing. See <see cref="aiHornetAirborneHold"/> for why the hold is there.
        /// </summary>
        private bool UpdateAiHornetAirborne()
        {
            // Resolved once a frame and cached: two callers want it now (the escort corner and the
            // leash), and decaying the hold twice per frame would halve it.
            int frame = Time.frameCount;
            if (aiHornetAirborneFrame == frame)
            {
                return aiHornetAirborne;
            }

            aiHornetAirborneFrame = frame;

            bool airborne = false;
            try
            {
                var hero = HeroController.instance;
                if (hero != null && hero.cState != null)
                {
                    airborne = !hero.cState.onGround;
                }
            }
            catch
            {
            }

            if (airborne)
            {
                aiHornetAirborneHold = AiAirborneHoldSeconds;
                aiHornetAirborne = true;
                return true;
            }

            if (aiHornetAirborneHold > 0f)
            {
                aiHornetAirborneHold = Mathf.Max(0f, aiHornetAirborneHold - Time.deltaTime);
                aiHornetAirborne = aiHornetAirborneHold > 0f;
                return aiHornetAirborne;
            }

            aiHornetAirborne = false;
            return false;
        }

        /// <summary>
        /// Replaces the plan's straight-line heading with one that goes around terrain.
        /// <para>
        /// The brain decides <i>where</i> and deliberately says nothing about how to get there; this
        /// is the seam that separation exists for. The Shade is a solid body on a layer that collides
        /// with the level, so a heading aimed through a wall does not glide past it - it grinds along
        /// it. Only the direction changes: the magnitude the brain chose still carries its own
        /// meaning, which is how close to the destination it is and therefore how gently to arrive.
        /// </para>
        /// </summary>
        private void ApplyShadeAiNavigation(Vector2 origin, float now, ModConfig config)
        {
            if (config == null || !config.shadeAiPathAroundTerrain)
            {
                return;
            }

            float magnitude = aiPlan.Move.magnitude;
            if (magnitude <= 0.0001f)
            {
                return;
            }

            Vector2 steer = aiNavigator.Steer(
                origin,
                aiPlan.DesiredPosition,
                GetAiBodyRadius(),
                now,
                aiThreats,
                ShadeAiTuning.FromConfig(config).ThreatStandoff);
            if (steer.sqrMagnitude > 0.0001f)
            {
                aiPlan.Move = steer.normalized * magnitude;
            }
        }

        /// <summary>
        /// Half the Shade's width, for sweeping a path it actually fits through. Falls back to the
        /// capsule the controller builds in <c>SetupPhysics</c> when the collider is unavailable.
        /// </summary>
        private float GetAiBodyRadius()
        {
            return bodyCol ? Mathf.Clamp(bodyCol.bounds.extents.x, 0.2f, 2f) : 0.45f;
        }


        /// <summary>
        /// Gives up on an order the Shade cannot honour.
        /// <para>
        /// Local steering can route around things but cannot know a destination is unreachable, so
        /// without this an order pointing somewhere sealed off - the far side of a boss arena wall
        /// being the case that turned up - leaves the Shade pressed against the obstacle indefinitely.
        /// Dropping the order hands it back to escorting Hornet, which is always reachable.
        /// </para>
        /// </summary>
        private void CheckShadeAiStuck()
        {
            if (!aiNavigator.LastPathStuck)
            {
                return;
            }

            aiStuckStreak++;
            if (aiStuckStreak < AiStuckStreakLimit)
            {
                return;
            }

            aiStuckStreak = 0;
            if (HasShadeAiCommand)
            {
                ClearShadeAiCommand("unreachable");
            }
        }

        private static bool IsAiNailAction(ShadeAiAction action)
        {
            return action == ShadeAiAction.SlashSide
                || action == ShadeAiAction.SlashUp
                || action == ShadeAiAction.SlashDown;
        }

        /// <summary>
        /// The gap the AI leaves between its own swings.
        /// <para>
        /// The Shade's nail cooldown is what the game permits, not what a person achieves: nobody
        /// lands a swing every 0.3s while also dodging, and an AI that does trivialises every fight
        /// it turns up to. <see cref="ModConfig.shadeAiAttackSpeedFraction"/> is the fraction of that
        /// theoretical maximum the AI is allowed, so the default of half means one swing per two
        /// cooldowns.
        /// </para>
        /// <para>
        /// Derived from the live cooldown rather than from a constant, so Quick Slash still speeds
        /// the AI up by exactly as much as it speeds a player up.
        /// </para>
        /// </summary>
        private float GetAiNailInterval(ModConfig config)
        {
            float fraction = Mathf.Clamp(config != null ? config.shadeAiAttackSpeedFraction : 0.5f, 0.05f, 1f);
            return Mathf.Max(0.01f, nailCooldown) / fraction;
        }

        private bool IsAiNailReady(float nailInterval, float now)
        {
            if (nailTimer > 0f || nailDurationTimer > 0f)
            {
                return false;
            }

            return aiLastNailTime <= 0f || now - aiLastNailTime >= nailInterval;
        }

        /// <summary>
        /// HP at or above which one enemy justifies a spell on its own, standing in for the boss flag
        /// the game does not expose. See <see cref="ShadeAiTuning.BossNailHits"/>.
        /// </summary>
        private int GetAiSpellWorthHealth(in ShadeAiTuning tuning)
        {
            int nailDamage = Mathf.Max(1, GetShadeNailDamage());
            return nailDamage * Mathf.Max(1, tuning.BossNailHits);
        }

        /// <summary>
        /// Everything that would damage the Shade if it stood there, as circles.
        /// <para>
        /// Deliberately routed through the same three filters the real damage intake uses -
        /// <c>ResolveDamager</c> for "is this actually a damage source rather than a child of one",
        /// <c>ShouldIgnoreDamageSource</c> for the alert/sight/detection volumes that are named like
        /// hitboxes but are not, and <c>CouldReachHornet</c> for the layer-matrix check. Avoidance
        /// built on a different notion of "dangerous" from the damage path would have the Shade
        /// dodging telegraphs that cannot hurt it while standing in things that can.
        /// </para>
        /// </summary>
        private IReadOnlyList<ShadeAiThreat> CollectAiThreats(Vector2 origin, ModConfig config)
        {
            aiThreats.Clear();

            // Nothing to avoid while invincible, and the query is the most expensive thing here.
            if (!canTakeDamage || config == null || !config.shadeAiAvoidAttacks)
            {
                return aiThreats;
            }

            try
            {
                if (!aiThreatFilterReady)
                {
                    aiThreatFilter = new ContactFilter2D
                    {
                        useTriggers = true,
                        useLayerMask = false,
                        useDepth = false
                    };
                    aiThreatFilterReady = true;
                }

                int count = Physics2D.OverlapCircle(origin, AiThreatScanRadius, aiThreatFilter, aiThreatBuffer);
                int limit = Mathf.Min(count, aiThreatBuffer.Length);
                for (int i = 0; i < limit; i++)
                {
                    var collider = aiThreatBuffer[i];
                    if (!collider)
                    {
                        continue;
                    }

                    if (collider.transform == transform || collider.transform.IsChildOf(transform))
                    {
                        continue;
                    }

                    if (hornetTransform != null)
                    {
                        var hornetRoot = hornetTransform.root;
                        if (collider.transform == hornetTransform
                            || collider.transform.IsChildOf(hornetTransform)
                            || (hornetRoot != null && collider.transform.root == hornetRoot))
                        {
                            continue;
                        }
                    }

                    var damager = ResolveDamager(collider);
                    if (damager == null || !damager.enabled || GetDamageAmount(damager) <= 0)
                    {
                        continue;
                    }

                    if (ShouldIgnoreDamageSource(collider) || ShouldIgnoreDamageSource(damager))
                    {
                        continue;
                    }

                    if (!CouldReachHornet(collider))
                    {
                        continue;
                    }

                    var bounds = collider.bounds;
                    float radius = Mathf.Clamp(
                        Mathf.Max(bounds.extents.x, bounds.extents.y),
                        AiThreatMinRadius,
                        AiThreatMaxRadius);
                    aiThreats.Add(new ShadeAiThreat(bounds.center, radius));
                }
            }
            catch
            {
            }

            return aiThreats;
        }

        /// <summary>
        /// SOUL the AI may not spend on offence, because a heal is going to need it.
        /// <para>
        /// Held back as soon as either the Shade or Hornet drops below its threshold, rather than
        /// once the heal is already wanted: SOUL spent on a cast a second earlier is SOUL the heal
        /// does not have, and the Shade refills it slowly enough that the difference decides fights.
        /// </para>
        /// </summary>
        private int GetAiSoulReserve(in ShadeAiTuning tuning)
        {
            if (focusHealingDisabled)
            {
                return 0;
            }

            bool selfLow = canTakeDamage && GetAiSelfHealthFraction() <= tuning.SelfHealHealthFraction;
            bool hornetLow = GetHornetHealthFraction() <= tuning.HornetHealHealthFraction;
            return selfLow || hornetLow ? focusSoulCost : 0;
        }

        /// <summary>
        /// Whether a Focus would actually do anything. <c>HandleFocus</c> refuses to channel at full
        /// health, so a Shade at full HP cannot heal Hornet either - healing her is a side effect of
        /// the Shade healing itself while she stands close, not something Focus can be aimed at her.
        /// </summary>
        private bool CanAiFocusHeal(ModConfig config)
        {
            if (config != null && !config.shadeAiHealWhenLow)
            {
                return false;
            }

            return !focusHealingDisabled && shadeHP < shadeMaxHP && GetFocusHealAmount() > 0;
        }

        private float GetAiSelfHealthFraction()
        {
            int max = GetMaxHP();
            return max > 0 ? Mathf.Clamp01((float)GetTotalCurrentHealth() / max) : 1f;
        }

        private static float GetHornetHealthFraction()
        {
            try
            {
                var playerData = GameManager.instance != null ? GameManager.instance.playerData : null;
                if (playerData != null && playerData.maxHealth > 0)
                {
                    return Mathf.Clamp01((float)playerData.health / playerData.maxHealth);
                }
            }
            catch
            {
            }

            return 1f;
        }

        /// <summary>
        /// Whether something that owns the Shade's controls is on screen.
        /// <para>
        /// The synthesised input bypasses <c>ShadeInput.ShouldSuppressOption</c> by construction -
        /// that gate is about physical bindings, and the AI has none - so the two things that gate
        /// keeps out have to be checked here instead. The bug-report overlay owns the keyboard while
        /// a report is being typed; the inventory panes and the settings menu navigate on the Shade's
        /// own move bindings, and a driver publishing movement would take those presses away from
        /// them.
        /// </para>
        /// </summary>
        internal static bool ShadeAiUiIsOpen()
        {
            try
            {
                if (LegacyoftheAbyss.Diagnostics.BugReportSystem.IsCapturingText)
                {
                    return true;
                }
            }
            catch
            {
            }

            try
            {
                if (ShadeSettingsMenu.IsShowing)
                {
                    return true;
                }
            }
            catch
            {
            }

            try
            {
                var playerData = MenuStateUtility.TryGetPlayerData();
                if (!ReferenceEquals(playerData, null) && playerData.isInventoryOpen)
                {
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Writes what the AI decided into the bug-report event ring, including the frames where it
        /// decided to do nothing - "no targets", "nothing in line of sight", "out of leash" and
        /// "waiting on a cooldown" are four completely different bugs that look identical in a
        /// screenshot, and a category that never appears cannot tell them apart.
        /// </summary>
        private void RecordShadeAiDecision()
        {
            bool isCast = aiPlan.Action == ShadeAiAction.Fireball
                || aiPlan.Action == ShadeAiAction.Shriek
                || aiPlan.Action == ShadeAiAction.DescendingDark;

            int key = ((int)aiPlan.Reason * 397) ^ ((int)aiPlan.Action << 8) ^ aiPlan.TargetId;
            if (key == aiLastEventKey)
            {
                return;
            }

            float now = Time.unscaledTime;
            if (!isCast && now - aiLastEventTime < AiEventMinInterval)
            {
                return;
            }

            aiLastEventKey = key;
            aiLastEventTime = now;

            try
            {
                LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
                    "shade-ai",
                    DescribeAiAction(aiPlan.Action),
                    FormattableString.Invariant(
                        $"reason={aiPlan.Reason} target={aiPlan.TargetId} wouldHit={aiPlan.ReasonCount} threats={aiThreatCount} soul={shadeSoul}/{shadeSoulMax} reserve={aiSoulReserve} hp={GetTotalCurrentHealth()}/{GetMaxHP()} rerouting={aiNavigator.LastPathBlocked} stuck={aiNavigator.LastPathStuck} {DescribeAiScan()}"));
            }
            catch
            {
            }
        }

        /// <summary>
        /// What the enemy scan kept and threw away, so a report saying the Shade sees nothing also
        /// says which step lost them.
        /// </summary>
        private string DescribeAiScan()
        {
            var scan = aiScanner.Stats;
            return FormattableString.Invariant(
                $"scan=found:{scan.Found}/alive:{scan.Tracked}/dormant:{scan.Dormant}/far:{scan.OutOfRange}/seen:{scan.Returned - scan.Blocked}/blocked:{scan.Blocked}/nospell:{scan.NotWorthASpell}/notdrawn:{scan.NotDrawn}");
        }

        private static string DescribeAiAction(ShadeAiAction action) => action switch
        {
            ShadeAiAction.SlashSide => "side slash",
            ShadeAiAction.SlashUp => "up slash",
            ShadeAiAction.SlashDown => "down slash",
            ShadeAiAction.Fireball => "cast projectile",
            ShadeAiAction.Shriek => "cast shriek",
            ShadeAiAction.DescendingDark => "cast descending dark",
            ShadeAiAction.Focus => "focus",
            _ => "no attack"
        };

        /// <summary>The AI's current state, as a flight-recorder/diagnostics fragment.</summary>
        private string DescribeAiState()
        {
            if (!ShadeAiEnabled)
            {
                return null;
            }

            return FormattableString.Invariant($"ai:{aiPlan.Reason}");
        }
    }
}
#nullable restore
