#nullable disable
using UnityEngine;
using LegacyoftheAbyss.Shade;
using LegacyoftheAbyss.Shade.Knight;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        // Tuned against Hollow Knight's own hero rather than Hornet's: the Knight falls slower and
        // jumps lower than she does, and matching her makes the borrowed animations read wrong.
        private const float KnightRunSpeed = 8.3f;
        private const float KnightGravity = 60f;
        private const float KnightMaxFallSpeed = 26f;
        private const float KnightJumpSpeed = 16.6f;
        private const float KnightJumpHoldSeconds = 0.28f;

        /// <summary>
        /// The air jump's share of a ground jump's height.
        /// <para>
        /// Applied as its square root, because height goes as the square of the launch speed: 0.6
        /// here is a jump 40% lower, where taking 40% off the speed outright would have cost nearly
        /// two thirds of the height. The ground jump is untouched.
        /// </para>
        /// </summary>
        private const float KnightDoubleJumpHeightScale = 0.6f;

        private static readonly float KnightDoubleJumpSpeed =
            KnightJumpSpeed * Mathf.Sqrt(KnightDoubleJumpHeightScale);

        /// <summary>
        /// What the held-jump thrust re-asserts, which is not always <see cref="KnightJumpSpeed"/>.
        /// Without this the hold would hand the air jump the ground jump's speed back a frame after
        /// launch and undo the whole difference.
        /// </summary>
        private float knightJumpThrustSpeed = KnightJumpSpeed;
        private const float KnightDashSpeed = 20f;
        private const float KnightDashSeconds = 0.25f;
        /// <summary>
        /// Hollow Knight's own Mothwing recharge, measured from the end of the dash.
        /// <para>
        /// This was zero, on the reasoning that the Knight needs to keep up with Hornet. It does,
        /// but a dash with no recharge is a dash that can be held down - and it left Dashmaster,
        /// whose whole job is shortening this, with nothing to shorten. Shade Cloak keeps its own
        /// longer recharge on top; the Shade's sprint dash is a separate ability and is untouched.
        /// </para>
        /// </summary>
        private const float KnightDashCooldownSeconds = 0.4f;


        /// <summary>
        /// Shade Cloak's own cooldown, separate from and longer than the plain dash's, as it is in
        /// Hollow Knight. The shadow particles are its readout: see ShadeCloakOnCooldown.
        /// </summary>
        private const float ShadeCloakCooldownSeconds = 1.5f;
        private const float KnightWallSlideSpeed = 6f;
        private const float KnightWallJumpSpeed = 13f;
        private const float KnightWallJumpLockSeconds = 0.18f;
        private const float KnightCoyoteSeconds = 0.09f;
        private const float KnightJumpBufferSeconds = 0.12f;

        /// <summary>
        /// The Knight jumps on the button the Shade uses for its down slash. It has no down-slash
        /// button of its own - the slash is aimed with the movement stick - so the slot is free,
        /// and reusing it avoids inventing a default that collides with Hornet's own controls.
        /// </summary>
        private const ShadeAction KnightJumpAction = ShadeAction.NailDown;

        /// <summary>Distance past the hard leash at which the Knight is put back beside Hornet.</summary>
        private const float KnightLeashSnapPadding = 6f;

        /// <summary>How much of its walk the Knight keeps while focusing under Shape of Unn.</summary>
        private const float KnightFocusMoveScale = 0.5f;

        private KnightView knightView;
        private KnightAbilities knightAbilities = KnightAbilities.None;
        private float knightAbilityRefreshTimer;

        private bool knightGrounded;
        private bool knightWasGrounded;

        /// <summary>Whether the Knight was holding a wall last frame, so the catch can be spotted.</summary>
        private bool knightWasClinging;

        /// <summary>
        /// Counts down to the moment after a room load when the Knight is put back beside Hornet.
        /// <para>
        /// Many rooms drop it out of the world outright: it is set down before the incoming room's
        /// geometry is where it will end up, so it starts inside a floor or in the gap under one and
        /// falls. The ordinary leash cannot catch that, because the leash is deliberately switched
        /// off during spawn protection - which is exactly this window - and by the time protection
        /// lifts the Knight is somewhere below the level.
        /// </para>
        /// <para>
        /// So the placement is simply redone once the room has settled, whether or not anything went
        /// wrong. A Knight that landed correctly is put back where it already is.
        /// </para>
        /// </summary>
        private float knightRoomEntrySettleTimer;

        /// <summary>
        /// How long after a room load that replacement happens. A quarter second, as asked: long
        /// enough for the incoming room's geometry to be where it will end up, short enough that
        /// the Knight is beside Hornet before a sprint through a doorway has carried her away from
        /// where it appeared.
        /// </summary>
        private const float KnightRoomEntrySettleSeconds = 0.25f;

        /// <summary>
        /// How long the Knight stands still, and cannot be hurt, after a hazard has put it back.
        /// <para>
        /// A hazard respawn drops the Knight beside Hornet mid-input, often mid-fall and often close
        /// to the thing that just killed it. Without a pause it kept whatever the player was holding
        /// and walked straight back into the spikes, which reads as the respawn not having worked.
        /// </para>
        /// </summary>
        private const float KnightHazardRespawnLockSeconds = 1f;

        private float knightHazardLockTimer;

        /// <summary>
        /// Holds the Knight still for a moment after a hazard respawn. The matching invulnerability
        /// is set alongside it in <c>OnShadeHitHazard</c>, so the pause cannot be spent being hit.
        /// </summary>
        internal void BeginKnightHazardRespawnLock()
        {
            if (!UsesGroundedMovement)
            {
                return;
            }

            knightHazardLockTimer = KnightHazardRespawnLockSeconds;
            knightBalloonRiseTimer = 0f;
            knightVerticalVelocity = 0f;
            knightJumpHoldTimer = 0f;
            knightDashTimer = 0f;
            knightJumpBufferTimer = 0f;
            knightJumpPressLatched = false;
            knightDashPressLatched = false;
            knockbackVelocity = Vector2.zero;
            knockbackTimer = 0f;
        }

        /// <summary>Starts the settle countdown. Called when a scene transition begins.</summary>
        internal void ScheduleKnightRoomEntrySettle()
        {
            knightRoomEntrySettleTimer = KnightRoomEntrySettleSeconds;
        }

        /// <summary>
        /// Puts the Knight back beside Hornet once the incoming room has settled. Ticked ahead of
        /// the control lock below, because a room entry <em>is</em> a control lock - leaving it
        /// after the early return meant it never ran on the transitions that need it.
        /// </summary>
        private void UpdateKnightRoomEntrySettle(float dt)
        {
            if (knightRoomEntrySettleTimer <= 0f)
            {
                return;
            }

            knightRoomEntrySettleTimer -= dt;
            if (knightRoomEntrySettleTimer > 0f || hornetTransform == null)
            {
                return;
            }

            knightRoomEntrySettleTimer = 0f;

            // Not while resting: the bench seat is a placement of its own and Hornet is sitting in
            // it, so moving the Knight onto her would undo the seating that just happened.
            if (knightBenchActive)
            {
                return;
            }

            TeleportToPosition(hornetTransform.position);
            knightVerticalVelocity = 0f;
            knightDashTimer = 0f;
            knightBalloonRiseTimer = 0f;
            knightAirJumpSpent = false;
            knightDashSpentInAir = false;
        }
        private int knightWallDirection;
        private float knightVerticalVelocity;
        private float knightJumpHoldTimer;
        private float knightCoyoteTimer;
        private float knightJumpBufferTimer;
        private float knightDashTimer;
        private float knightDashCooldownTimer;
        private int knightDashDirection;
        private bool knightDashIsShadeCloak;
        private bool knightAirJumpSpent;
        private bool knightDashSpentInAir;
        private float knightWallJumpLockTimer;
        private float knightLandTimer;

        // Latched in Update by CaptureMovementInput; see there for why.
        private bool knightJumpPressLatched;
        private bool knightDashPressLatched;
        private bool knightJumpHeld;

        /// <summary>Holds a scripted pose - the map, currently - against the movement animation.</summary>
        private bool knightMapOpen;

        internal void SetKnightMapOpen(bool open)
        {
            knightMapOpen = open;
        }

        /// <summary>
        /// Whether this companion walks rather than floats. The Shade flies on a leash with gravity
        /// off; the Knight is a platformer body, so the two share no movement code.
        /// </summary>
        private bool UsesGroundedMovement =>
            Companion != null && Companion.CharacterDefinition.Moveset == ShadeMoveset.Knight;

        /// <summary>
        /// Builds the Knight rig and switches the body to platformer physics. Returns false when the
        /// bundle is unavailable, leaving the companion as a Shade rather than an invisible body.
        /// </summary>
        private bool TryInitializeKnight()
        {
            knightView = KnightView.Attach(gameObject);
            if (knightView == null)
            {
                return false;
            }

            if (sr != null)
            {
                RefreshKnightSorting();
                sr.enabled = false;
            }

            if (rb != null)
            {
                // Velocity is integrated here rather than by the engine, so the body stays kinematic
                // in spirit: gravityScale off, and MovePosition drives it.
                rb.gravityScale = 0f;
            }

            ShrinkKnightBody();

            RefreshKnightAbilities(force: true);
            return true;
        }

        /// <summary>
        /// Brings the companion body down to the Knight's size. The colliders are sized for the
        /// Shade, and the rig is shrunk to stand beside Hornet at the right height - leaving the
        /// body alone would give the Knight a hurtbox a head taller than it looks, and a ground
        /// probe that finds the floor before its feet reach it. Everything else measures off these
        /// bounds, so the probes, the pogo reach and the depenetration all follow.
        /// </summary>
        private void ShrinkKnightBody()
        {
            float scale = Mathf.Clamp(ModConfig.Instance.knightScale, 0.01f, 1f);
            if (Mathf.Approximately(scale, 1f))
            {
                return;
            }

            foreach (var collider in GetComponents<Collider2D>())
            {
                if (collider is CapsuleCollider2D capsule)
                {
                    capsule.size *= scale;
                }
            }
        }

        private void RefreshKnightAbilities(bool force = false)
        {
            if (!force)
            {
                knightAbilityRefreshTimer -= Time.deltaTime;
                if (knightAbilityRefreshTimer > 0f)
                {
                    return;
                }
            }

            knightAbilityRefreshTimer = 0.5f;

            var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
            if (pd == null)
            {
                return;
            }

            var snapshot = new HornetProgressSnapshot
            {
                HasDash = pd.hasDash,
                HasWalljump = pd.hasWalljump,
                HasDoubleJump = pd.hasDoubleJump,
                HasHarpoonDash = pd.hasHarpoonDash,
                HasNeedleThrow = pd.hasNeedleThrow,
                HasSilkCharge = pd.hasSilkCharge,
                HasParry = pd.hasParry,
                HasSilkBossNeedle = pd.hasSilkBossNeedle,
                HasThreadSphere = pd.hasThreadSphere,
                HasSilkBomb = pd.hasSilkBomb,
            };

            knightAbilities = KnightAbilityMap.FromHornet(snapshot);
        }

        /// <summary>
        /// The Knight's whole movement step: probe the world, read input, integrate, then animate.
        /// Called from <c>HandleMovementAndFacing</c> in place of the Shade's leash flight.
        /// </summary>
        private void HandleKnightMovement(float dt)
        {
            if (dt <= 0f)
            {
                return;
            }

            RefreshKnightAbilities();
            UpdateKnightRoomEntrySettle(dt);

            if (knightHazardLockTimer > 0f)
            {
                knightHazardLockTimer = Mathf.Max(0f, knightHazardLockTimer - dt);
            }

            PushKnightOutOfTerrain();
            ProbeKnightSurroundings();

            if (knightCastFrozen && Time.time >= knightCastFreezeUntil)
            {
                EndKnightCastFreeze();
            }

            if (knightCastFrozen)
            {
                // Held rather than damped: at this scale a single frame of gravity is a visible
                // drop, and the point of the stall is that the Knight does not move at all.
                knightVerticalVelocity = 0f;
                knightJumpHoldTimer = 0f;
                UpdateKnightAnimation(0f);
                return;
            }

            // The launch owns the Knight outright while it runs, gravity included - so it has to
            // stand down for anything that owns it more: a hazard putting it back, a cutscene, a
            // bench, or the room-entry placement. Each of those puts the Knight somewhere
            // deliberate, and a rise still running would carry it straight back off.
            if (knightBalloonRiseTimer > 0f
                && (hornetControlsLocked || isInactive || knightBenchActive || knightHazardLockTimer > 0f))
            {
                knightBalloonRiseTimer = 0f;
            }

            if (UpdateKnightBalloonLaunch(dt))
            {
                ApplyKnightMotion(0f, dt, facingSpeed: 0f);

                // Still held on screen. Half a second at this speed is a nine unit rise, which can
                // clear the frame outright - and a Knight that leaves it only to be snapped back
                // when the launch ends reads as the launch having been cut off. The leash stays
                // out of it: that one measures distance to Hornet and would fight a rise she is
                // not making.
                ClampKnightToCameraView();
                UpdateKnightAnimation(0f);
                return;
            }

            // Consumed unconditionally, so a push landing on the same frame as a stagger or a
            // cutscene is spent rather than banked and released when the hold lifts.
            float knockbackSpeed = ConsumeKnightKnockback(dt);

            // Walking to a bench is the exception: the hold belongs to Hornet, and for that one the
            // Knight has somewhere to be. UpdateKnightBench owns the input for the whole approach.
            bool benchWalking = knightBenchActive && !knightBenchSeated;

            bool hazardLocked = knightHazardLockTimer > 0f;

            if (!benchWalking && (hornetControlsLocked || isInactive || hazardLocked || damageStaggerTimer > 0f))
            {
                if (knightBenchSeated)
                {
                    // A seated Knight is placed, not falling. It used to keep its gravity through
                    // the whole rest, so a seat over a gap dropped it out of the scene and the
                    // bench hold then held it there.
                    knightVerticalVelocity = 0f;
                    knightJumpHoldTimer = 0f;
                }
                else
                {
                    // Still fall, so a locked Knight does not hang in the air, but take no input.
                    // A damage stagger keeps its push - being knocked back is the whole point of
                    // it - while a cutscene or a dormant Knight is held still.
                    IntegrateKnightVertical(dt, allowInput: false);
                    ApplyKnightMotion(
                        hornetControlsLocked || isInactive || hazardLocked ? 0f : knockbackSpeed,
                        dt,
                        facingSpeed: 0f);
                }

                // The recovery runs on this path too. It used to sit past the return, so for as
                // long as the hold lasted - a room transition's entry walk, or a whole rest at a
                // bench - nothing could bring the Knight back: it fell away behind Hornet and
                // stayed there, which is why it never appeared at the entrance of the next room.
                //
                // Not during spawn protection: that is the window where the Knight has been set
                // down and Hornet may not be at her entry position yet, and a leash measured
                // against a position still settling would place the Knight against it.
                ClampKnightToCameraView();
                if (!sceneProtectionActive)
                {
                    EnforceKnightLeash();
                }
                UpdateKnightAnimation(0f);
                return;
            }

            float horizontal = capturedHorizontalInput;
            UpdateKnightTimers(dt);

            // Channelling roots the Knight the way it roots Hornet: no jump, no dash, and no walk.
            // Shape of Unn buys back the walk alone, at half speed. Nothing held the Knight before,
            // so it healed while jumping around at full speed.
            if (isFocusing)
            {
                knightJumpBufferTimer = 0f;
                knightDashPressLatched = false;
            }

            UpdateKnightDash(dt, horizontal);

            float runSpeed = KnightGroundSpeed;
            float facingDriver = float.NaN;

            if (isFocusing)
            {
                runSpeed *= allowFocusMovement ? KnightFocusMoveScale : 0f;
            }

            float speed;
            if (knightDashTimer > 0f)
            {
                speed = knightDashDirection * KnightDashSpeed;
                knightVerticalVelocity = 0f;
            }
            else
            {
                speed = horizontal * runSpeed;
                if (knightWallJumpLockTimer > 0f)
                {
                    // Hold the push off the wall briefly, or holding toward it cancels the jump.
                    speed = -knightWallDirection * runSpeed;
                }
                else
                {
                    // Kept out of the facing driver below: a recoil is a push, not a decision to
                    // turn round, and letting it steer spun the Knight away from what it just hit.
                    facingDriver = speed;
                    speed += knockbackSpeed;
                }

                IntegrateKnightVertical(dt, allowInput: true);
            }

            ApplyKnightMotion(speed, dt, float.IsNaN(facingDriver) ? (float?)null : facingDriver);
            UpdateSharpShadowDashState();
            ClampKnightToCameraView();
            EnforceKnightLeash();
            UpdateKnightAnimation(speed);
        }

        private void UpdateKnightTimers(float dt)
        {
            knightCoyoteTimer = knightGrounded ? KnightCoyoteSeconds : Mathf.Max(0f, knightCoyoteTimer - dt);
            knightWallJumpLockTimer = Mathf.Max(0f, knightWallJumpLockTimer - dt);
            knightDashCooldownTimer = Mathf.Max(0f, knightDashCooldownTimer - dt);
            knightLandTimer = Mathf.Max(0f, knightLandTimer - dt);

            if (knightJumpPressLatched)
            {
                knightJumpPressLatched = false;
                knightJumpBufferTimer = KnightJumpBufferSeconds;
            }
            else
            {
                knightJumpBufferTimer = Mathf.Max(0f, knightJumpBufferTimer - dt);
            }
        }

        private void IntegrateKnightVertical(float dt, bool allowInput)
        {
            // ProbeKnightSurroundings zeroes the rise on the frame a wall is caught, so this
            // engages immediately on contact and does not have to wait out a climb - which is what
            // the condition originally cost. It is still needed: without it a deliberate upward
            // impulse beside a wall, a pogo above all, was turned into a wall slide on the very
            // next frame, so a bounce off an enemy taken next to a wall gave no height at all.
            bool clingingToWall = knightWallDirection != 0
                && knightAbilities.MantisClaw
                && knightVerticalVelocity <= 0f;

            if (allowInput && TryKnightJump(clingingToWall))
            {
                return;
            }

            if (clingingToWall)
            {
                knightVerticalVelocity = -KnightWallSlideSpeed;
                return;
            }

            if (knightGrounded && knightVerticalVelocity <= 0f)
            {
                knightVerticalVelocity = 0f;
                return;
            }

            // Holding jump keeps thrust for a moment - Hollow Knight's variable jump height.
            if (allowInput && knightJumpHoldTimer > 0f && knightJumpHeld)
            {
                knightJumpHoldTimer -= dt;
                knightVerticalVelocity = knightJumpThrustSpeed;
                return;
            }

            knightJumpHoldTimer = 0f;
            knightVerticalVelocity = Mathf.Max(
                knightVerticalVelocity - KnightGravity * dt,
                -KnightMaxFallSpeed);
        }

        private bool TryKnightJump(bool clingingToWall)
        {
            if (knightJumpBufferTimer <= 0f)
            {
                return false;
            }

            if (clingingToWall)
            {
                knightJumpBufferTimer = 0f;
                knightVerticalVelocity = KnightWallJumpSpeed;
                knightJumpThrustSpeed = KnightJumpSpeed;
                knightJumpHoldTimer = KnightJumpHoldSeconds;
                knightWallJumpLockTimer = KnightWallJumpLockSeconds;
                facing = -knightWallDirection;
                // A wall jump restores the air jump, as it does in Hollow Knight.
                knightAirJumpSpent = false;
                return true;
            }

            if (knightGrounded || knightCoyoteTimer > 0f)
            {
                knightJumpBufferTimer = 0f;
                knightCoyoteTimer = 0f;
                knightVerticalVelocity = KnightJumpSpeed;
                knightJumpThrustSpeed = KnightJumpSpeed;
                knightJumpHoldTimer = KnightJumpHoldSeconds;
                return true;
            }

            if (knightAbilities.DoubleJump && !knightAirJumpSpent)
            {
                knightJumpBufferTimer = 0f;
                knightAirJumpSpent = true;
                knightVerticalVelocity = KnightDoubleJumpSpeed;
                knightJumpThrustSpeed = KnightDoubleJumpSpeed;
                knightJumpHoldTimer = KnightJumpHoldSeconds;
                knightView?.Play(KnightView.ClipDoubleJump, restart: true);
                knightView?.FlashMonarchWings();
                KnightAudio.PlayWings(EnsureKnightSfx(), GetEffectiveSfxVolume());
                return true;
            }

            return false;
        }

        /// <summary>
        /// How much of the dash recharge the equipped charms leave, taken from the Shade's own dash
        /// cooldown rather than from a flag of its own.
        /// <para>
        /// Dashmaster is a stat multiplier on <c>SprintDashCooldown</c>, so reading the ratio it has
        /// already produced means the charm shortens both characters dash recharge without knowing
        /// that either exists - and any future charm touching that stat does the same for free.
        /// </para>
        /// </summary>
        private float KnightDashCooldownScale
        {
            get
            {
                float baseline = s_defaultCharmStats.SprintDashCooldown;
                if (baseline <= 0.0001f)
                {
                    return 1f;
                }

                return Mathf.Clamp(sprintDashCooldown / baseline, 0.1f, 2f);
            }
        }

        private void UpdateKnightDash(float dt, float horizontal)
        {
            if (knightDashTimer > 0f)
            {
                knightDashTimer -= dt;
                return;
            }

            if (!knightDashPressLatched)
            {
                return;
            }

            knightDashPressLatched = false;

            if (!knightAbilities.CanDash || knightDashCooldownTimer > 0f)
            {
                return;
            }

            // Shade Cloak carries its own, longer cooldown; while it is recharging the Knight still
            // has the plain Mothwing dash, exactly as in Hollow Knight.
            bool cloakReady = knightAbilities.ShadeCloak && !ShadeCloakOnCooldown;

            // One dash per airtime until the ground or a wall gives it back.
            if (!knightGrounded && knightDashSpentInAir)
            {
                return;
            }

            knightDashDirection = Mathf.Abs(horizontal) > 0.1f ? (int)Mathf.Sign(horizontal) : facing;
            knightDashTimer = KnightDashSeconds;

            // Counted from the end of the dash rather than its start, so the charm shortens the wait
            // between dashes rather than part of the dash itself.
            knightDashCooldownTimer = KnightDashSeconds + (KnightDashCooldownSeconds * KnightDashCooldownScale);
            knightDashIsShadeCloak = cloakReady;
            knightVerticalVelocity = 0f;
            knightJumpHoldTimer = 0f;

            if (!knightGrounded)
            {
                knightDashSpentInAir = true;
            }

            if (knightDashIsShadeCloak)
            {
                BeginKnightDashIntangibility();
                BeginShadeCloakCooldown();
                knightView?.Play(KnightView.ClipShadeCloak, restart: true);
                KnightAudio.PlayShadeCloak(EnsureKnightSfx(), GetEffectiveSfxVolume());
            }
            else
            {
                knightView?.Play(KnightView.ClipDash, restart: true);
                KnightAudio.PlayDash(EnsureKnightSfx(), GetEffectiveSfxVolume());
            }
        }

        /// <summary>
        /// Shade Cloak's intangibility. Reuses the same damage gate spawn protection uses, so a
        /// dash through an attack is refused for the same reason and by the same code - and expires
        /// on its own, which is why there is nothing to switch back off at the end of the dash.
        /// </summary>
        private void BeginKnightDashIntangibility() => SuppressHazardDamage(KnightDashSeconds);

        /// <summary>
        /// Moves the Knight and turns it to face where it is going.
        /// <para>
        /// <paramref name="facingSpeed"/> is separate from the motion because they are not always
        /// the same question: a nail recoil is horizontal motion the Knight did not choose, and
        /// facing it turns the Knight to look at what it just hit away from. Pass the input-driven
        /// speed there and the total in <paramref name="horizontalSpeed"/>.
        /// </para>
        /// </summary>
        private void ApplyKnightMotion(float horizontalSpeed, float dt, float? facingSpeed = null)
        {
            float facingDriver = facingSpeed ?? horizontalSpeed;
            Vector2 delta = new Vector2(horizontalSpeed, knightVerticalVelocity) * dt;
            Vector2 current = transform.position;
            Vector2 target = current + delta;

            target = ResolveKnightCollision(current, target);

            if (rb != null)
            {
                rb.MovePosition(target);
            }
            else
            {
                // Depth carried through explicitly: assigning a Vector2 here fills z with zero
                // rather than leaving it alone, which walks the Knight off the playable plane and
                // in front of everything drawn on it.
                transform.position = new Vector3(target.x, target.y, transform.position.z);
            }

            lastMoveDelta = target - current;

            if (knightDashTimer <= 0f && knightWallJumpLockTimer <= 0f)
            {
                if (facingDriver > 0.1f) facing = 1;
                else if (facingDriver < -0.1f) facing = -1;
            }
            else if (knightDashTimer > 0f && knightDashDirection != 0)
            {
                facing = knightDashDirection;
            }
        }

        /// <summary>
        /// Holds the Knight inside the camera's view, so the second player can always see what they
        /// are doing.
        /// <para>
        /// This stands in for a split screen, which Silksong's camera will not support:
        /// <c>tk2dCamera</c> writes the projection matrix directly and exposes no viewport, the
        /// darkness pass feeds a full-screen cutout texture through global shader state, and
        /// <c>CameraController</c> owns the fades and lock areas a second view would have to
        /// duplicate. Confining the Knight is the cheap half of the same goal.
        /// </para>
        /// <para>
        /// Nothing here touches scene transitions: those stay Hornet's alone, and the Knight is
        /// carried along by the ordinary respawn path.
        /// </para>
        /// </summary>
        private void ClampKnightToCameraView()
        {
            if (!ModConfig.Instance.knightCameraLeashEnabled)
            {
                return;
            }

            var hero = HeroController.UnsafeInstance;
            if (hero == null)
            {
                return;
            }

            Vector3 position = transform.position;

            // Measured against everything the camera can still do - the furthest it will lean plus
            // the widest it will zoom - rather than against the frame as it stands. Clamping to the
            // live frame stopped the Knight at the very edge that would have made the camera lean
            // and zoom further, so the two deadlocked and the Knight had to fight the leash.
            Vector2 extents = bodyCol != null ? (Vector2)bodyCol.bounds.extents : new Vector2(0.5f, 0.75f);
            float margin = Mathf.Max(0f, ModConfig.Instance.knightCameraLeashMargin);
            if (!CompanionCameraBias.TryGetCompanionRoam(hero.transform.position, extents, margin, out var roam))
            {
                return;
            }

            float minX = roam.xMin;
            float maxX = roam.xMax;
            float minY = roam.yMin;
            float maxY = roam.yMax;

            // A box smaller than the Knight would invert these; centre it rather than fight.
            if (minX > maxX) minX = maxX = roam.center.x;
            if (minY > maxY) minY = maxY = roam.center.y;

            float clampedX = Mathf.Clamp(position.x, minX, maxX);
            float clampedY = Mathf.Clamp(position.y, minY, maxY);

            if (Mathf.Approximately(clampedX, position.x) && Mathf.Approximately(clampedY, position.y))
            {
                return;
            }

            // Kill the velocity being spent against an edge, or the Knight grinds along it and a
            // fall banks up speed it would release the moment the camera moved.
            if (clampedY > position.y && knightVerticalVelocity < 0f)
            {
                knightVerticalVelocity = 0f;
            }
            else if (clampedY < position.y && knightVerticalVelocity > 0f)
            {
                knightVerticalVelocity = 0f;
            }

            transform.position = new Vector3(clampedX, clampedY, position.z);
        }

        /// <summary>
        /// The Knight cannot fly back to Hornet, so once it is far enough behind it is placed beside
        /// her instead. This is the platformer stand-in for the Shade's snap leash.
        /// </summary>
        private void EnforceKnightLeash()
        {
            if (hornetTransform == null)
            {
                return;
            }

            float limit = snapLeashRadius + KnightLeashSnapPadding;
            Vector2 toHornet = (Vector2)hornetTransform.position - (Vector2)transform.position;
            if (toHornet.sqrMagnitude < limit * limit)
            {
                return;
            }

            TeleportToPosition(hornetTransform.position);
            knightVerticalVelocity = 0f;
            knightDashTimer = 0f;
            knightAirJumpSpent = false;
            knightDashSpentInAir = false;
        }

        /// <summary>
        /// The Knight's ground speed with the charm loadout's movement modifiers folded in.
        /// <para>
        /// Carried across as a <em>ratio</em> rather than by reading <c>moveSpeed</c> outright:
        /// that number is the Shade's flight speed, tuned against a leash, while the Knight's is
        /// tuned against its own gravity and jump arc. Taking the ratio keeps the Knight's feel and
        /// still lets every movement-speed charm reach it. Nothing carried it before, which is why
        /// Sprintmaster - and every other movement-stat charm - did nothing for the Knight.
        /// </para>
        /// </summary>
        private float KnightGroundSpeed
        {
            get
            {
                float baseline = s_defaultCharmStats.MoveSpeed;
                if (baseline <= 0.0001f)
                {
                    return KnightRunSpeed;
                }

                return KnightRunSpeed * Mathf.Clamp(moveSpeed / baseline, 0.25f, 3f);
            }
        }

        /// <summary>
        /// Spends one frame of <c>ApplyKnockback</c>'s push and reports it as a horizontal speed.
        /// <para>
        /// The Shade spends the same field in its leash step, which the Knight never reaches, so
        /// without this the recoil from its own nail hits was written and never read - the Knight
        /// stood perfectly still on every hit, which is what Steady Body is supposed to buy.
        /// Suppression still works, because <c>ApplyKnockback</c> declines to write anything.
        /// </para>
        /// <para>
        /// Horizontal only. The vertical component of a nail recoil would fight the down-slash
        /// pogo for the same frames, and an up-slash would drive the Knight into the floor.
        /// Both directions are already owned by <c>knightVerticalVelocity</c>.
        /// </para>
        /// </summary>
        private float ConsumeKnightKnockback(float dt)
        {
            if (knockbackTimer <= 0f)
            {
                knockbackVelocity = Vector2.zero;
                return 0f;
            }

            float horizontal = knockbackVelocity.x;
            knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, 10f * dt);
            knockbackTimer -= dt;
            if (knockbackTimer <= 0f)
            {
                knockbackVelocity = Vector2.zero;
            }

            return horizontal;
        }

        /// <summary>
        /// Puts the Knight's rig on the Shade's sorting layer, but at <em>Hornet's</em> own order
        /// rather than the Shade's.
        /// <para>
        /// The companion is deliberately drawn one order in front of her, which is right for the
        /// Shade - a translucent wisp that should read over her rather than disappear behind her.
        /// It is wrong for the Knight, which is an opaque character standing on the same ground:
        /// that single step was enough to put it in front of scenery Hornet is behind, reported as
        /// the two of them landing on opposite sides of the same clump of grass.
        /// </para>
        /// <para>
        /// Only when the two share a sorting layer. On any other layer the configured order is an
        /// absolute value rather than an offset from hers, so hers says nothing about it.
        /// </para>
        /// </summary>
        private void RefreshKnightSorting()
        {
            if (knightView == null || sr == null)
            {
                return;
            }

            int order = sr.sortingOrder;

            var heroRenderer = LegacyHelper.ResolveHornetBodyRenderer(HeroController.UnsafeInstance);
            if (heroRenderer != null && heroRenderer.sortingLayerID == sr.sortingLayerID)
            {
                order = heroRenderer.sortingOrder;
            }

            knightView.ApplySorting(sr.sortingLayerID, order);
        }

        /// <summary>The scripted spell pose currently overriding the movement clips, and its expiry.</summary>
        private string knightSpellClip;
        private float knightSpellClipUntil;

        /// <summary>
        /// The air-stall. Casting stops the Knight dead in the air, which in Hollow Knight is not a
        /// side effect of the animation but a movement tool in its own right - a spell is how you
        /// hang in place over a hazard or reset a fall.
        /// </summary>
        private bool knightCastFrozen;

        /// <summary>
        /// A backstop on that freeze. Every cast releases it explicitly, but a cast can also be
        /// dropped from outside - a room change, a cutscene, Hornet going down - and a freeze left
        /// standing would strand the Knight in mid-air for the rest of the session.
        /// </summary>
        private float knightCastFreezeUntil;

        private const float KnightCastFreezeMaxSeconds = 1.5f;

        /// <summary>
        /// Five frames at sixty, which is what the report asked for and what the up slash reads as
        /// in Hollow Knight: a swing that connects plants the Knight rather than carrying it along.
        /// </summary>
        private const float KnightUpSlashFreezeSeconds = 5f / 60f;

        /// <summary>
        /// Freezes the Knight when an up slash connects, and only then.
        /// <para>
        /// The freeze is the recoil off whatever was hit, so an up slash into open air must not
        /// have it - swung on the way up, an unconditional freeze stopped the jump dead every time.
        /// Nothing at the press knows whether the swing will land, and a probe for it would be
        /// guessing at the slash's reach when the slash itself measures it; so the spawned damager
        /// is subscribed to instead, and answers on the frame it responds to a hit.
        /// </para>
        /// <para>
        /// Every damager under the slash, because the swing is a parent with its hitboxes below it,
        /// and unsubscribed on the first of them to report - a multi-hitter would otherwise re-freeze
        /// the Knight for every enemy in the swing.
        /// </para>
        /// </summary>
        private void WatchForKnightUpSlashHit(GameObject slash)
        {
            if (!UsesGroundedMovement || slash == null)
            {
                return;
            }

            var damagers = slash.GetComponentsInChildren<DamageEnemies>(true);
            if (damagers == null || damagers.Length == 0)
            {
                return;
            }

            bool answered = false;
            System.Action<DamageEnemies.HitResponse> onHit = null;
            onHit = _ =>
            {
                if (answered)
                {
                    return;
                }

                answered = true;
                for (int i = 0; i < damagers.Length; i++)
                {
                    if (damagers[i] != null)
                    {
                        damagers[i].HitResponded -= onHit;
                    }
                }

                BeginKnightUpSlashFreeze();
            };

            for (int i = 0; i < damagers.Length; i++)
            {
                if (damagers[i] != null)
                {
                    damagers[i].HitResponded += onHit;
                }
            }
        }

        /// <summary>
        /// Plants the Knight for an up slash: momentum cancelled outright, position held briefly.
        /// <para>
        /// Reuses the cast freeze, which already stops the body without letting gravity accumulate
        /// underneath it - the whole point of that hold is that a frame of gravity at this scale is
        /// a visible drop.
        /// </para>
        /// </summary>
        internal void BeginKnightUpSlashFreeze()
        {
            if (!UsesGroundedMovement || knightBalloonRiseTimer > 0f)
            {
                return;
            }

            knightVerticalVelocity = 0f;
            knightJumpHoldTimer = 0f;
            knockbackVelocity = Vector2.zero;
            knockbackTimer = 0f;

            knightCastFrozen = true;
            knightCastFreezeUntil = Time.time + KnightUpSlashFreezeSeconds;
        }

        /// <summary>
        /// Holds the Knight still for a cast. A no-op for the Shade, which floats and has no
        /// momentum worth cancelling, so the spell routines can call it unguarded.
        /// </summary>
        private void BeginKnightCastFreeze()
        {
            // Not during a balloon launch. The freeze returns before the launch is ticked, so it
            // would hold the Knight mid-rise and then hand the whole rise back when it lifted -
            // and the launch is meant to own the Knight outright for its half second.
            if (!UsesGroundedMovement || knightBalloonRiseTimer > 0f)
            {
                return;
            }

            knightCastFrozen = true;
            knightCastFreezeUntil = Time.time + KnightCastFreezeMaxSeconds;
            knightVerticalVelocity = 0f;
            knightJumpHoldTimer = 0f;
        }

        private void EndKnightCastFreeze()
        {
            knightCastFrozen = false;
            knightCastFreezeUntil = 0f;
        }

        /// <summary>
        /// Holds one of the rig's spell poses for <paramref name="seconds"/>. A no-op for the Shade,
        /// which animates from its own sprite sheets, so the spell routines can call it unguarded.
        /// </summary>
        private void PlayKnightSpellAnimation(string clipName, float seconds)
        {
            if (knightView == null || string.IsNullOrEmpty(clipName))
            {
                return;
            }

            knightSpellClip = clipName;
            knightSpellClipUntil = Time.time + seconds;
            knightView.Play(clipName, restart: true);
        }

        private void UpdateKnightAnimation(float horizontalSpeed)
        {
            if (knightView == null)
            {
                return;
            }

            knightView.SetFacing(facing);

            // A scripted pose outranks the movement state, or the walk cycle stamps over it next
            // frame. Only while grounded: the Knight should not be reading a map mid-fall.
            if (knightMapOpen && knightGrounded)
            {
                knightView.Play(KnightView.ClipMap);
                return;
            }

            // Ahead of everything, including the dormant pose: sitting is what the Knight is doing
            // while Hornet rests, and the hold that would otherwise dock it is the same one.
            string benchClip = KnightBenchClip();
            if (benchClip != null)
            {
                knightView.Play(benchClip);
                return;
            }

            // Once the animator has a frame, sit the rig's feet on the collider's base.
            if (bodyCol != null)
            {
                knightView.AlignFeetTo(bodyCol.bounds.min.y);
            }

            // A spell pose outranks the movement state while the cast runs, for the same reason the
            // map pose does: this method runs every physics step and would otherwise stamp the
            // airborne or idle clip over it on the very next one. That is why Descending Dark went
            // off with all of its effects and none of the Knight's own animation.
            if (knightSpellClip != null && Time.time < knightSpellClipUntil)
            {
                knightView.Play(knightSpellClip);
                return;
            }

            if (isInactive)
            {
                knightView.Play(KnightView.ClipCollect);
                return;
            }

            // Above the movement states, which would otherwise stamp the run or idle clip over the
            // pose on the next physics step - the same reason the map and spell poses sit high.
            if (isFocusing)
            {
                if (allowFocusMovement && knightView.HasClip(KnightView.ClipSlugIdle))
                {
                    bool crawling = Mathf.Abs(horizontalSpeed) > 0.1f && knightView.HasClip(KnightView.ClipSlugWalk);
                    knightView.Play(crawling ? KnightView.ClipSlugWalk : KnightView.ClipSlugIdle);
                }
                else
                {
                    knightView.Play(KnightView.ClipFocus);
                }

                return;
            }

            if (knightDashTimer > 0f)
            {
                // Which dash matters: the per-frame animation step ran straight after the dash
                // started and stamped the plain Dash clip over the Shade Cloak one, so the cloak
                // never showed its own animation for a single frame.
                if (!knightDashIsShadeCloak)
                {
                    knightView.Play(KnightView.ClipDash);
                    return;
                }

                // Sharp Shadow has its own cloak animation - the blade the charm describes is in
                // the clip, so without it the charm reads as doing nothing even while it damages.
                string cloakClip = sharpShadowEquipped && knightView.HasClip(KnightView.ClipShadeCloakSharp)
                    ? KnightView.ClipShadeCloakSharp
                    : KnightView.ClipShadeCloak;
                knightView.Play(cloakClip);
                return;
            }

            if (knightWallDirection != 0 && !knightGrounded)
            {
                knightView.Play(KnightView.ClipWallSlide);
                return;
            }

            if (!knightGrounded)
            {
                knightView.Play(KnightView.ClipAirborne);
                return;
            }

            if (knightLandTimer > 0f)
            {
                knightView.Play(KnightView.ClipLand);
                return;
            }

            if (Mathf.Abs(horizontalSpeed) <= 0.1f)
            {
                knightView.Play(KnightView.ClipIdle);
                return;
            }

            // Sprintmaster brings its own walk cycle. Asked for rather than assumed: Play leaves
            // the previous clip running when it cannot find one, so a bundle without it would
            // freeze the Knight mid-stride rather than fall back to the ordinary run.
            bool sprintCycle = sprintmasterEquipped && knightView.HasClip(KnightView.ClipSprint);
            knightView.Play(sprintCycle ? KnightView.ClipSprint : KnightView.ClipRun);
        }
    }
}
