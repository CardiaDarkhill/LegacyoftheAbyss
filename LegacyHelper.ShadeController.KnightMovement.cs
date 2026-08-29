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
        private const float KnightDashSpeed = 20f;
        private const float KnightDashSeconds = 0.25f;
        private const float KnightDashCooldownSeconds = 0.6f;
        private const float KnightWallSlideSpeed = 6f;
        private const float KnightWallJumpSpeed = 13f;
        private const float KnightWallJumpLockSeconds = 0.18f;
        private const float KnightPogoSpeed = 16f;
        private const float KnightGroundProbe = 0.12f;
        private const float KnightCoyoteSeconds = 0.09f;
        private const float KnightJumpBufferSeconds = 0.12f;

        /// <summary>Distance past the hard leash at which the Knight is put back beside Hornet.</summary>
        private const float KnightLeashSnapPadding = 6f;

        private KnightView knightView;
        private KnightAbilities knightAbilities = KnightAbilities.None;
        private float knightAbilityRefreshTimer;

        private bool knightGrounded;
        private bool knightWasGrounded;
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
                // The sheet renderer is the Shade's; the Knight draws through its own rig, but the
                // sorting it resolved is still the right sorting for this companion.
                knightView.ApplySorting(sr.sortingLayerID, sr.sortingOrder);
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
            PushKnightOutOfTerrain();
            ProbeKnightSurroundings();

            if (hornetControlsLocked || isInactive || damageStaggerTimer > 0f)
            {
                // Still fall, so a locked Knight does not hang in the air, but take no input.
                IntegrateKnightVertical(dt, allowInput: false);
                ApplyKnightMotion(0f, dt);
                UpdateKnightAnimation(0f);
                return;
            }

            float horizontal = capturedHorizontalInput;
            UpdateKnightTimers(dt, horizontal);
            UpdateKnightDash(dt, horizontal);

            float speed;
            if (knightDashTimer > 0f)
            {
                speed = knightDashDirection * KnightDashSpeed;
                knightVerticalVelocity = 0f;
            }
            else
            {
                speed = horizontal * KnightRunSpeed;
                if (knightWallJumpLockTimer > 0f)
                {
                    // Hold the push off the wall briefly, or holding toward it cancels the jump.
                    speed = -knightWallDirection * KnightRunSpeed;
                }

                IntegrateKnightVertical(dt, allowInput: true);
            }

            ApplyKnightMotion(speed, dt);
            ClampKnightToCameraView();
            EnforceKnightLeash();
            UpdateKnightAnimation(speed);
        }

        /// <summary>
        /// Lifts the body clear when it is already inside terrain. The swept collision below only
        /// stops motion *into* geometry; a Knight spawned or teleported into the floor starts
        /// overlapping and would otherwise stay buried, because every sweep out of it begins in
        /// contact. Uses <c>Collider2D.Distance</c>, which is pure geometry rather than filtered by
        /// the layer collision matrix the Knight and terrain do not share.
        /// </summary>
        private void PushKnightOutOfTerrain()
        {
            if (bodyCol == null)
            {
                return;
            }

            var bounds = bodyCol.bounds;
            var overlaps = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f, KnightTerrainMask());

            for (int i = 0; i < overlaps.Length; i++)
            {
                var other = overlaps[i];
                if (other == null || other == bodyCol || other.transform.IsChildOf(transform))
                {
                    continue;
                }

                var distance = bodyCol.Distance(other);
                if (!distance.isValid || !distance.isOverlapped)
                {
                    continue;
                }

                // The normal runs from this collider toward the other, so separating means moving
                // back along it by the penetration depth.
                float depth = Mathf.Abs(distance.distance);
                transform.position -= (Vector3)(distance.normal * depth);

                if (knightVerticalVelocity < 0f)
                {
                    knightVerticalVelocity = 0f;
                }
            }
        }

        private void ProbeKnightSurroundings()
        {
            knightWasGrounded = knightGrounded;

            var bounds = bodyCol != null ? bodyCol.bounds : new Bounds(transform.position, Vector3.one);
            int mask = KnightTerrainMask();

            Vector2 footCentre = new Vector2(bounds.center.x, bounds.min.y);
            knightGrounded = knightVerticalVelocity <= 0.01f
                && Physics2D.OverlapBox(footCentre, new Vector2(bounds.size.x * 0.85f, KnightGroundProbe), 0f, mask) != null;

            knightWallDirection = 0;
            if (!knightGrounded && knightAbilities.MantisClaw)
            {
                Vector2 side = new Vector2(bounds.size.x * 0.5f + 0.06f, 0f);
                Vector2 centre = bounds.center;
                var probe = new Vector2(0.08f, bounds.size.y * 0.7f);

                if (Physics2D.OverlapBox(centre + side, probe, 0f, mask) != null)
                    knightWallDirection = 1;
                else if (Physics2D.OverlapBox(centre - side, probe, 0f, mask) != null)
                    knightWallDirection = -1;
            }

            if (knightGrounded)
            {
                knightAirJumpSpent = false;
                knightDashSpentInAir = false;
                if (!knightWasGrounded)
                {
                    knightLandTimer = 0.12f;
                }
            }
        }

        /// <summary>
        /// Terrain the Knight stands on. Hornet's own collision mask is the authority; falling back
        /// to a named layer keeps the Knight solid if the hero is momentarily absent.
        /// </summary>
        private static int KnightTerrainMask()
        {
            int terrain = LayerMask.NameToLayer("Terrain");
            int mask = terrain >= 0 ? 1 << terrain : 0;

            int soft = LayerMask.NameToLayer("Soft Terrain");
            if (soft >= 0)
            {
                mask |= 1 << soft;
            }

            return mask != 0 ? mask : Physics2D.AllLayers;
        }

        private void UpdateKnightTimers(float dt, float horizontal)
        {
            knightCoyoteTimer = knightGrounded ? KnightCoyoteSeconds : Mathf.Max(0f, knightCoyoteTimer - dt);
            knightWallJumpLockTimer = Mathf.Max(0f, knightWallJumpLockTimer - dt);
            knightDashCooldownTimer = Mathf.Max(0f, knightDashCooldownTimer - dt);
            knightLandTimer = Mathf.Max(0f, knightLandTimer - dt);

            if (ShadeInput.WasActionPressed(ShadeAction.Jump))
            {
                knightJumpBufferTimer = KnightJumpBufferSeconds;
            }
            else
            {
                knightJumpBufferTimer = Mathf.Max(0f, knightJumpBufferTimer - dt);
            }
        }

        private void IntegrateKnightVertical(float dt, bool allowInput)
        {
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
            if (allowInput && knightJumpHoldTimer > 0f && ShadeInput.IsActionHeld(ShadeAction.Jump))
            {
                knightJumpHoldTimer -= dt;
                knightVerticalVelocity = KnightJumpSpeed;
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
                knightJumpHoldTimer = KnightJumpHoldSeconds;
                return true;
            }

            if (knightAbilities.DoubleJump && !knightAirJumpSpent)
            {
                knightJumpBufferTimer = 0f;
                knightAirJumpSpent = true;
                knightVerticalVelocity = KnightJumpSpeed;
                knightJumpHoldTimer = KnightJumpHoldSeconds;
                knightView?.Play(KnightView.ClipDoubleJump, restart: true);
                return true;
            }

            return false;
        }

        private void UpdateKnightDash(float dt, float horizontal)
        {
            if (knightDashTimer > 0f)
            {
                knightDashTimer -= dt;
                if (knightDashTimer <= 0f && knightDashIsShadeCloak)
                {
                    SetKnightIntangible(false);
                }

                return;
            }

            if (!knightAbilities.CanDash
                || knightDashCooldownTimer > 0f
                || !ShadeInput.WasActionPressed(ShadeAction.Sprint))
            {
                return;
            }

            // One dash per airtime until the ground or a wall gives it back.
            if (!knightGrounded && knightDashSpentInAir)
            {
                return;
            }

            knightDashDirection = Mathf.Abs(horizontal) > 0.1f ? (int)Mathf.Sign(horizontal) : facing;
            knightDashTimer = KnightDashSeconds;
            knightDashCooldownTimer = KnightDashCooldownSeconds;
            knightDashIsShadeCloak = knightAbilities.ShadeCloak;
            knightVerticalVelocity = 0f;
            knightJumpHoldTimer = 0f;

            if (!knightGrounded)
            {
                knightDashSpentInAir = true;
            }

            if (knightDashIsShadeCloak)
            {
                SetKnightIntangible(true);
            }

            knightView?.Play(KnightView.ClipDash, restart: true);
        }

        /// <summary>
        /// Shade Cloak's intangibility. Reuses the same damage gate spawn protection uses, so a
        /// dash through an attack is refused for the same reason and by the same code.
        /// </summary>
        private void SetKnightIntangible(bool intangible)
        {
            if (intangible)
            {
                SuppressHazardDamage(KnightDashSeconds);
            }
        }

        private void ApplyKnightMotion(float horizontalSpeed, float dt)
        {
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
                transform.position = target;
            }

            lastMoveDelta = target - current;

            if (knightDashTimer <= 0f && knightWallJumpLockTimer <= 0f)
            {
                if (horizontalSpeed > 0.1f) facing = 1;
                else if (horizontalSpeed < -0.1f) facing = -1;
            }
            else if (knightDashTimer > 0f && knightDashDirection != 0)
            {
                facing = knightDashDirection;
            }
        }

        /// <summary>
        /// Stops the body at terrain rather than letting MovePosition push it through. Each axis is
        /// swept separately so sliding along a wall or a ceiling still works.
        /// </summary>
        private Vector2 ResolveKnightCollision(Vector2 current, Vector2 target)
        {
            if (bodyCol == null)
            {
                return target;
            }

            int mask = KnightTerrainMask();
            var size = bodyCol.bounds.size;
            var boxSize = new Vector2(size.x * 0.9f, size.y * 0.9f);

            Vector2 resolved = current;

            Vector2 horizontalStep = new Vector2(target.x - current.x, 0f);
            if (Mathf.Abs(horizontalStep.x) > 0.0001f)
            {
                var hit = Physics2D.BoxCast(resolved, boxSize, 0f, new Vector2(Mathf.Sign(horizontalStep.x), 0f), Mathf.Abs(horizontalStep.x), mask);
                resolved.x = hit.collider != null
                    ? resolved.x + Mathf.Sign(horizontalStep.x) * Mathf.Max(0f, hit.distance - 0.01f)
                    : target.x;
            }

            Vector2 verticalStep = new Vector2(0f, target.y - current.y);
            if (Mathf.Abs(verticalStep.y) > 0.0001f)
            {
                var hit = Physics2D.BoxCast(resolved, boxSize, 0f, new Vector2(0f, Mathf.Sign(verticalStep.y)), Mathf.Abs(verticalStep.y), mask);
                if (hit.collider != null)
                {
                    resolved.y += Mathf.Sign(verticalStep.y) * Mathf.Max(0f, hit.distance - 0.01f);
                    // Landing or hitting a ceiling both kill vertical speed.
                    knightVerticalVelocity = 0f;
                }
                else
                {
                    resolved.y = target.y;
                }
            }

            return resolved;
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

            Vector3 position = transform.position;
            if (!TryGetCameraViewBounds(position, out var view))
            {
                return;
            }

            // Hold back by the margin plus the Knight's own half-size, so it stops fully inside the
            // frame rather than with half of itself past the edge.
            Vector2 extents = bodyCol != null ? (Vector2)bodyCol.bounds.extents : new Vector2(0.5f, 0.75f);
            float margin = Mathf.Max(0f, ModConfig.Instance.knightCameraLeashMargin);

            float minX = view.xMin + extents.x + margin;
            float maxX = view.xMax - extents.x - margin;
            float minY = view.yMin + extents.y + margin;
            float maxY = view.yMax - extents.y - margin;

            // A view smaller than the Knight would invert these; centre it rather than fight.
            if (minX > maxX) minX = maxX = view.center.x;
            if (minY > maxY) minY = maxY = view.center.y;

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
        }

        /// <summary>
        /// Bounces the Knight upward off whatever it just down-slashed, and gives back the air jump
        /// and air dash exactly as landing would.
        /// </summary>
        internal void ApplyKnightPogoBounce()
        {
            knightVerticalVelocity = KnightPogoSpeed;
            knightJumpHoldTimer = 0f;
            knightAirJumpSpent = false;
            knightDashSpentInAir = false;
        }

        /// <summary>How far below the Knight a down slash looks for something to bounce off.</summary>
        private const float KnightPogoProbeDepth = 0.9f;

        /// <summary>
        /// A down slash looking for a surface to bounce off. Hornet counts, which is the point:
        /// without her as a platform most of the game's verticality is closed to the Knight.
        /// Called after the slash is thrown, so the bounce and the swing stay in step.
        /// </summary>
        private bool TryKnightPogo()
        {
            if (!UsesGroundedMovement || knightGrounded)
            {
                return false;
            }

            var bounds = bodyCol != null ? bodyCol.bounds : new Bounds(transform.position, Vector3.one);
            var probeCentre = new Vector2(bounds.center.x, bounds.min.y - KnightPogoProbeDepth * 0.5f);
            var probeSize = new Vector2(bounds.size.x * 1.1f, KnightPogoProbeDepth);

            // An explicit mask keeps this a geometry query rather than one filtered by the layer
            // collision matrix, which the Knight and Hornet do not share.
            var hits = Physics2D.OverlapBoxAll(probeCentre, probeSize, 0f, Physics2D.AllLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit == null || hit.transform.IsChildOf(transform))
                {
                    continue;
                }

                if (IsKnightPogoSurface(hit))
                {
                    ApplyKnightPogoBounce();
                    return true;
                }
            }

            return false;
        }

        private static bool IsKnightPogoSurface(Collider2D collider)
        {
            var hero = HeroController.UnsafeInstance;
            if (hero != null && hero.transform != null && collider.transform.IsChildOf(hero.transform))
            {
                return true;
            }

            return collider.GetComponentInParent<HealthManager>() != null;
        }

        private void UpdateKnightAnimation(float horizontalSpeed)
        {
            if (knightView == null)
            {
                return;
            }

            knightView.SetFacing(facing);

            // Once the animator has a frame, sit the rig's feet on the collider's base.
            if (bodyCol != null)
            {
                knightView.AlignFeetTo(bodyCol.bounds.min.y);
            }

            if (isInactive)
            {
                knightView.Play(KnightView.ClipCollect);
                return;
            }

            if (knightDashTimer > 0f)
            {
                knightView.Play(KnightView.ClipDash);
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

            knightView.Play(Mathf.Abs(horizontalSpeed) > 0.1f ? KnightView.ClipRun : KnightView.ClipIdle);
        }
    }
}
