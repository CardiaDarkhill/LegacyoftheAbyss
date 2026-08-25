#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LegacyoftheAbyss.Shade;
using LegacyoftheAbyss.Shade.Ai;
using UnityEngine;
using GlobalEnums;

public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        // Resolved once. InArenaFight is reached every frame through AdjustLeashForCamera.
        private static readonly FieldInfo s_battleSceneStartedField =
            typeof(BattleScene).GetField("started", BindingFlags.Instance | BindingFlags.NonPublic);

        private void Update()
        {
            // Ahead of every early return below and of every read: last frame's synthesised input
            // is cleared here and only republished if the AI driver actually runs this frame. A
            // Shade that is paused, dormant or gone therefore cannot leave a direction held down -
            // which matters because the pause-menu panes navigate on the same move actions.
            ShadeAiInput.Clear();

            if (pendingCharmLoadoutRecompute && baselineStatsInitialized)
            {
                pendingCharmLoadoutRecompute = false;
                try
                {
                    RecomputeCharmLoadout();
                }
                catch
                {
                    pendingCharmLoadoutRecompute = true;
                }
            }

            // Ahead of every early return below: the wisps should keep drifting while Hornet is
            // downed or her controls are locked, and the emitter has to be stopped when the Shade
            // goes dormant regardless of which branch this frame takes.
            UpdateShadowParticles();
            EnsureShadeLight();

            if (hornetTransform == null) return;

            bool pushedSoulThisFrame = false;
            if (!cachedHud)
            {
                var resolvedHud = UnityEngine.Object.FindFirstObjectByType<SimpleHUD>();
                if (resolvedHud)
                {
                    cachedHud = resolvedHud;
                    PushShadeStatsToHud(suppressDamageAudio: true);
                    PushSoulToHud();
                    pushedSoulThisFrame = true;
                }
            }

            if (cachedHud && !pushedSoulThisFrame)
            {
                PushSoulToHud();
            }

            if (GameIsPaused())
            {
                capturedMoveInput = Vector2.zero;
                capturedHorizontalInput = 0f;
                capturedSprintHeld = false;
                if (rb)
                    rb.linearVelocity = Vector2.zero;
                lastMoveDelta = Vector2.zero;
                isSprinting = false;
                sprintDashTimer = 0f;
                inHardLeash = false;
                hardLeashTimer = 0f;
                return;
            }

            if (HornetIsDowned())
            {
                CancelFocus();
                DestroyOtherSlashes(null);
                isCastingSpell = false;
                isChannelingTeleport = false;
                teleportChannelTimer = 0f;
                capturedMoveInput = Vector2.zero;
                capturedHorizontalInput = 0f;
                capturedSprintHeld = false;
                if (rb) rb.linearVelocity = Vector2.zero;
                lastMoveDelta = Vector2.zero;
                knockbackVelocity = Vector2.zero;
                knockbackTimer = 0f;
                isSprinting = false;
                sprintDashTimer = 0f;
                inHardLeash = false;
                hardLeashTimer = 0f;
                return;
            }

            if (hazardCooldown > 0f) hazardCooldown = Mathf.Max(0f, hazardCooldown - Time.deltaTime);
            if (hurtCooldown > 0f) hurtCooldown = Mathf.Max(0f, hurtCooldown - Time.deltaTime);
            if (damageStaggerTimer > 0f) damageStaggerTimer = Mathf.Max(0f, damageStaggerTimer - Time.deltaTime);
            // Assist mode and the AI switch used to be polled here as rebindable hotkeys. Both now
            // live only in the pause menu - assist mode on the Difficulty screen, the AI on its own
            // screen - so there is nothing to read every frame and no binding to mis-hit mid-fight.

            if (sceneProtectionActive)
            {
                if (sceneProtectionTimer > 0f)
                {
                    sceneProtectionTimer = Mathf.Max(0f, sceneProtectionTimer - Time.deltaTime);
                }

                if (canTakeDamage)
                {
                    canTakeDamage = false;
                    PushShadeStatsToHud(suppressDamageAudio: true);
                    PersistIfChanged();
                }

                if (sceneProtectionTimer <= 0f)
                {
                    if (SceneProtectionBlockedByOverlap())
                    {
                        sceneProtectionTimer = 0.1f;
                        hazardCooldown = Mathf.Max(hazardCooldown, 0.1f);
                        hurtCooldown = Mathf.Max(hurtCooldown, 0.1f);
                        TeleportToHornet();
                    }
                    else
                    {
                        sceneProtectionActive = false;
                        if (sceneProtectionSuppressingPersistence)
                        {
                            ExitPersistenceSuppression();
                            sceneProtectionSuppressingPersistence = false;
                        }

                        bool desiredCanTakeDamage = sceneProtectionDesiredDamageState;
                        if (canTakeDamage != desiredCanTakeDamage)
                        {
                            canTakeDamage = desiredCanTakeDamage;
                            PersistIfChanged();
                        }

                        PushShadeStatsToHud(suppressDamageAudio: true);
                    }
                }
            }
            ignoreRefreshTimer -= Time.deltaTime;
            if (ignoreRefreshTimer <= 0f)
            {
                RefreshCollisionIgnores();
                ignoreRefreshTimer = 1f;
            }

            hornetIgnoreRefreshTimer -= Time.deltaTime;
            if (hornetIgnoreRefreshTimer <= 0f)
            {
                EnsureIgnoreHornetCollisions();
                hornetIgnoreRefreshTimer = 0.5f;
            }

            // Track inactive flag
            isInactive = (!isDying && GetTotalCurrentHealth() <= 0);
            if (wasInactive && !isInactive)
            {
                hurtCooldown = Mathf.Max(hurtCooldown, ReviveIFrameSeconds);
                hazardCooldown = Mathf.Max(hazardCooldown, ReviveIFrameSeconds);
            }
            EnsureAggroProxyCollider();
            if (aggroProxyCollider)
            {
                bool proxyActive = !isInactive && isActiveAndEnabled && !assistModeEnabled;
                bool currentlyEnabled = aggroProxyCollider.enabled;
                if (currentlyEnabled != proxyActive)
                {
                    if (!proxyActive)
                    {
                        try { aggroProxyTracker?.ForceExitTrackedRemaskers(); } catch { }
                    }
                    aggroProxyCollider.enabled = proxyActive;

                    // "Enemies are ignoring the shade" has three possible causes and they are
                    // indistinguishable in-game, so name which one it is.
                    try
                    {
                        if (ModConfig.Instance.logShade)
                        {
                            LegacyHelper.LogInfo(proxyActive
                                ? "Shade aggro proxy enabled."
                                : FormattableString.Invariant($"Shade aggro proxy disabled (inactive={isInactive}, enabled={isActiveAndEnabled}, assistMode={assistModeEnabled})."));
                        }
                    }
                    catch
                    {
                    }
                }
            }
            wasInactive = isInactive;

            HandleTeleportChannel();

            CheckSprintUnlock();
            AdjustLeashForCamera();

            // Recomputed once a frame and read by FixedUpdate, the combat gate below, and SimpleHUD.
            bool wasControlsLocked = hornetControlsLocked;
            hornetControlsLocked = HornetControlsLocked(out bool shadeHidden);
            ApplyScriptedHoldVisibility(shadeHidden);
            if (hornetControlsLocked && !wasControlsLocked)
            {
                // Entering the locked state mid-action: drop whatever the Shade was doing rather than
                // leaving a focus or a channel running through a cutscene.
                CancelFocus();
                isCastingSpell = false;
                isChannelingTeleport = false;
                teleportChannelTimer = 0f;
            }

            // Before the driver, so an order given this frame is already in the plan it builds.
            UpdateShadeAiCommand();

            // Before CaptureMovementInput, because that is what reads the input this publishes.
            UpdateShadeAi();

            CaptureMovementInput();
            if (hornetControlsLocked)
            {
                capturedMoveInput = Vector2.zero;
                capturedHorizontalInput = 0f;
                capturedSprintHeld = false;
            }

            // Allow starting focus even when not casting other spells; focusing itself sets isCastingSpell
            if (!hornetControlsLocked && !inHardLeash && !isChannelingTeleport && !isInactive && damageStaggerTimer <= 0f)
            {
                HandleFocus();
                if (!isCastingSpell)
                    HandleFire();
                if (!isCastingSpell)
                {
                    HandleNailAttack();
                    HandleShriek();
                    HandleDescendingDark();
                }
            }

            CheckHazardOverlap();
            PersistIfChanged();
            CheckFocusReadySfx();
            UpdateSfxVolumes();
            HandleAnimation();

            if (charmUpdateCallbacks.Count > 0)
            {
                var context = new ShadeCharmContext(this, charmSnapshot);
                float delta = Time.deltaTime;
                foreach (var callback in charmUpdateCallbacks)
                {
                    try { callback(context, delta); }
                    catch { }
                }
            }
        }

        private void FixedUpdate()
        {
            if (hornetTransform == null) return;
            if (GameIsPaused())
            {
                if (rb)
                    rb.linearVelocity = Vector2.zero;
                lastMoveDelta = Vector2.zero;
                return;
            }
            if (HornetIsDowned())
            {
                if (rb)
                    rb.linearVelocity = Vector2.zero;
                lastMoveDelta = Vector2.zero;
                return;
            }
            HandleMovementAndFacing(Time.fixedDeltaTime);
        }

        public void ApplyBindHealFromHornet(Transform hornet)
        {
            try
            {
                var h = hornet != null ? hornet : hornetTransform;
                if (h == null) return;
                float dist = Vector2.Distance(h.position, transform.position);
                if (dist <= 6f)
                {
                    int before = shadeHP;
                    shadeHP = Mathf.Min(shadeHP + ModConfig.Instance.bindShadeHeal, shadeMaxHP);
                    if (shadeHP != before)
                    {
                        if (GetTotalCurrentHealth() > 0)
                        {
                            isInactive = false;
                            CancelDeathAnimation();
                        }
                        PushShadeStatsToHud(suppressDamageAudio: true);
                        PersistIfChanged();
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Turns assist mode on or off, with the spawn-protection deferral that has to happen either
        /// way.
        /// <para>
        /// Extracted from the hotkey handler when the AI briefly forced assist mode on. It no longer
        /// does - the AI fights on the same terms the player does - but the extraction is worth
        /// keeping: the deferral below is subtle and belongs in one place rather than inline in
        /// Update.
        /// </para>
        /// </summary>
        /// <summary>Whether assist mode is on right now. Read by the Difficulty menu's toggle.</summary>
        internal bool GetAssistModeEnabled() => assistModeEnabled;

        /// <summary>
        /// Public face of <see cref="SetAssistModeEnabled"/>, for the Difficulty menu. Assist mode
        /// used to be reachable only through a hotkey, which is why the setter was private.
        /// </summary>
        internal void SetAssistMode(bool enabled) => SetAssistModeEnabled(enabled);

        private void SetAssistModeEnabled(bool enabled)
        {
            if (assistModeEnabled == enabled)
            {
                return;
            }

            assistModeEnabled = enabled;
            bool desiredCanTakeDamage = !assistModeEnabled;

            if (sceneProtectionActive)
            {
                sceneProtectionDesiredDamageState = desiredCanTakeDamage;
                if (ModConfig.Instance.logShade)
                {
                    string assistState = assistModeEnabled ? "enabled" : "disabled";
                    string suffix = sceneProtectionTimer > 0f ? " (will apply after spawn protection)" : string.Empty;
                    try { UnityEngine.Debug.Log($"[ShadeDebug] Assist Mode {assistState}{suffix}"); } catch { }
                }
            }
            else
            {
                if (canTakeDamage != desiredCanTakeDamage)
                {
                    canTakeDamage = desiredCanTakeDamage;
                    PersistIfChanged();
                }
                if (ModConfig.Instance.logShade)
                {
                    string assistState = assistModeEnabled ? "enabled" : "disabled";
                    try { UnityEngine.Debug.Log($"[ShadeDebug] Assist Mode {assistState}"); } catch { }
                }
            }

            PushShadeStatsToHud(suppressDamageAudio: true);
        }

        private void CaptureMovementInput()
        {
            float left = ShadeInput.GetActionValue(ShadeAction.MoveLeft);
            float right = ShadeInput.GetActionValue(ShadeAction.MoveRight);
            float up = ShadeInput.GetActionValue(ShadeAction.MoveUp);
            float down = ShadeInput.GetActionValue(ShadeAction.MoveDown);
            Vector2 input = new Vector2(right - left, up - down);
            if (input.sqrMagnitude > 1f)
                input.Normalize();
            if (isChannelingTeleport)
                input = Vector2.zero;
            if (damageStaggerTimer > 0f)
                input = Vector2.zero;
            capturedMoveInput = input;
            capturedHorizontalInput = Mathf.Clamp(input.x, -1f, 1f);
            capturedSprintHeld = damageStaggerTimer <= 0f && sprintUnlocked && ShadeInput.IsActionHeld(ShadeAction.Sprint) && input.sqrMagnitude > 0f;
        }

        private static bool GameIsPaused()
        {
            try
            {
                var gm = GameManager.instance;
                return gm != null && gm.IsGamePaused();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The slice of Hornet's state that decides whether her controls are locked, lifted out of
        /// <see cref="HeroController"/> so the rule itself can be exercised without a Unity player
        /// loop. <see cref="CaptureHornetControlState"/> fills it in, <see cref="EvaluateControlsLocked"/>
        /// is the rule.
        /// </summary>
        internal struct HornetControlState
        {
            /// <summary>Sitting at a bench.</summary>
            public bool AtBench;
            /// <summary><c>HeroController.controlReqlinquished</c> - the game's own spelling.</summary>
            public bool ControlRelinquished;
            /// <summary><c>HeroController.acceptingInput</c>.</summary>
            public bool AcceptingInput;
            /// <summary><c>HeroController.IsPaused()</c> - paused or inventory open.</summary>
            public bool Paused;
            /// <summary>Mid scene change, in either direction.</summary>
            public bool Transitioning;
            /// <summary>Dead, dying on a hazard, or being put back on the last safe ground.</summary>
            public bool Downed;
            /// <summary>
            /// An interactable is holding Hornet - which is how conversations work, see
            /// <see cref="IsHeldByInteraction"/>.
            /// </summary>
            public bool HeldByInteraction;
            /// <summary>A cutscene, by any of the game's several names for one - see <see cref="IsInCutscene"/>.</summary>
            public bool InCutscene;
            /// <summary>
            /// The game has slid its own HUD off the screen. This is the tiebreaker for control losses
            /// nothing above accounts for - see <see cref="IsGameHudHidden"/>.
            /// </summary>
            public bool GameHudHidden;
            /// <summary>
            /// Hornet is part-way through a move of her own - see <see cref="IsInOwnMove"/>. The
            /// answer to "her controls are gone, but is anyone else driving?"
            /// </summary>
            public bool InOwnMove;
        }

        /// <summary>
        /// How long a control loss has to hold before it counts. Hornet's moves hand control back and
        /// forth over a couple of frames at their seams; without this, every one of those seams would
        /// strobe the Shade's HUD. Short enough that the start of a real cutscene still reads as
        /// instant - the HUD's own fade takes 0.2s on its own.
        /// </summary>
        private const float ControlLockGraceSeconds = 0.2f;

        /// <summary>
        /// How long an <i>inferred</i> hold has to last before the Shade is taken off screen.
        /// <para>
        /// Longer than <see cref="ControlLockGraceSeconds"/> because it guards a weaker inference and
        /// a louder failure. Docking the Shade a frame early is invisible; switching its renderer off
        /// and back on is a blink. Inside a memory the game's HUD is gone for the whole scene, so the
        /// pairing that identifies a scripted hold elsewhere - control gone, HUD gone - is true there
        /// for every move Hornet makes, and a mantle or an air dash read as a framed moment. Duration
        /// is what actually separates them: those moves are over in a third of a second and a framed
        /// moment is not.
        /// </para>
        /// <para>
        /// Only the inference waits. A cutscene the game names outright still hides the Shade on the
        /// frame it starts.
        /// </para>
        /// </summary>
        private const float InferredHoldGraceSeconds = 0.5f;

        /// <summary>When the current unbroken run of "her controls are gone" started, or -1 if none.</summary>
        private static float s_controlLockPendingSince = -1f;

        /// <summary>
        /// When the current unbroken run of an inferred hold started, or -1 if none. Its own clock:
        /// this is asked of a different question every frame than the one above, so they cannot share.
        /// </summary>
        private static float s_inferredHoldPendingSince = -1f;

        /// <summary>
        /// True while the game has taken Hornet away from the player: a conversation, a bench, or a
        /// cutscene. Those three are the whole intended list.
        /// <para>
        /// What this is emphatically NOT is "HeroController says it has her controls". Nearly
        /// everything the player asks Hornet to do that is not plain running and jumping is an FSM on
        /// the hero, and an FSM that wants to drive her takes her control away first - the Drifter's
        /// Cloak on an updraft, the air dash, the Needolin, every silk skill, several tools, the quick
        /// map. All of them set <c>controlReqlinquished</c> and clear <c>acceptingInput</c> for their
        /// whole duration, exactly as a cutscene does, so those two flags cannot tell the two apart
        /// and anything built on them alone docks the Shade mid-move. See
        /// <see cref="IsGameHudHidden"/> for what does tell them apart.
        /// </para>
        /// <para>
        /// The pause menu and the inventory are excluded too, and always have been: <c>acceptingInput</c>
        /// is false for those as well (<c>HeroController.PauseInput</c>), but the Shade stays under
        /// player control while a menu is open, which is how its own charm tab is reachable at all.
        /// <c>HeroController.IsPaused()</c> is exactly "paused or inventory open".
        /// </para>
        /// <para>
        /// Exposed statically for <c>SimpleHUD</c>, which hides the Shade's HUD off the same flag
        /// rather than deciding "is this a cutscene?" for itself.
        /// </para>
        /// </summary>
        internal static bool HornetControlsLocked() => HornetControlsLocked(out _);

        /// <summary>
        /// As <see cref="HornetControlsLocked()"/>, additionally reporting whether the Shade should
        /// be out of shot for it. One capture serves both - the two questions are asked together
        /// every frame and <see cref="CaptureHornetControlState"/> is not free.
        /// </summary>
        internal static bool HornetControlsLocked(out bool shadeHidden)
        {
            bool locked;
            bool hidden;
            bool hiddenIsInferred;
            shadeHidden = false;
            try
            {
                var state = CaptureHornetControlState();
                locked = EvaluateControlsLocked(state);
                hidden = EvaluateShadeHidden(state);
                hiddenIsInferred = EvaluateShadeHiddenInferred(state);
            }
            catch
            {
                return false;
            }

            // Both graces sit out here rather than inside the try above, and both for the same reason
            // the control-lock one always has: with no player loop the clock is an unjittable ecall
            // that throws on the call itself, before the callee's own try is ever entered. Nothing to
            // debounce against in that case, so the raw answers stand.
            try
            {
                shadeHidden = ApplyInferredHoldGrace(hidden, hiddenIsInferred);
            }
            catch
            {
                shadeHidden = hidden;
            }

            try
            {
                return ApplyControlLockGrace(locked);
            }
            catch
            {
                return locked;
            }
        }

        /// <summary>
        /// The rule itself, with nothing Unity-shaped left in it.
        /// </summary>
        internal static bool EvaluateControlsLocked(HornetControlState state)
        {
            // The three the feature is actually for, each of which the game names outright.
            if (state.AtBench || state.HeldByInteraction || state.InCutscene)
            {
                return true;
            }

            // Nothing has taken Hornet's input away: ordinary gameplay, or a menu - which is not this
            // flag's business, see the summary.
            if (!state.ControlRelinquished && (state.AcceptingInput || state.Paused))
            {
                return false;
            }

            if (state.Transitioning || state.Downed)
            {
                return true;
            }

            // Her control is gone and nothing above accounts for it. Default to trusting the game's
            // own HUD: it stays up for everything Hornet does, and slides away for the scripted holds
            // that do not identify themselves any other way.
            return state.GameHudHidden;
        }

        /// <summary>
        /// True while the Shade should not be on screen: the scripted, camera-framed holds.
        /// <para>
        /// Deliberately narrower than <see cref="EvaluateControlsLocked"/>. A bench and a
        /// conversation lock Hornet's controls too and the Shade stays visible for both - the bench
        /// is where its charms are changed, and docking beside her through dialogue is the intended
        /// look. Only the moments the camera has taken over want it gone; before this, the Shade sat
        /// visibly docked behind Hornet through every cutscene.
        /// </para>
        /// <para>
        /// Note what this deliberately is NOT: "we are in a memory scene". Those scenes run long
        /// playable parkour stretches where the Shade belongs on screen and under player control, so
        /// hiding it - or despawning it - for the whole scene is not an option. Only the framed
        /// moments inside them qualify, which is why <c>GameHudHidden</c> alone is not enough here:
        /// a memory keeps the game's HUD away for its playable stretches too, and hiding the Shade
        /// there would leave the player steering something they cannot see. Its HUD is a separate
        /// question, and <c>SimpleHUD</c> answers that one off the hidden game HUD directly.
        /// </para>
        /// </summary>
        internal static bool EvaluateShadeHidden(HornetControlState state)
            => EvaluateShadeHiddenNamed(state) || EvaluateShadeHiddenInferred(state);

        /// <summary>
        /// The moments where the game says outright that it has taken the scene over. Acted on the
        /// frame they start; there is nothing to be unsure about.
        /// </summary>
        internal static bool EvaluateShadeHiddenNamed(HornetControlState state)
            => !ShadeStaysVisibleRegardless(state) && state.InCutscene;

        /// <summary>
        /// A scripted hold nothing named, read from a control loss the game does not account for
        /// alongside its own HUD being gone.
        /// <para>
        /// The weak half of the rule, and the reason the two guards below it exist. Both halves of the
        /// pairing are true of a memory scene for its entire length: the HUD is gone there for
        /// atmosphere, and every move of Hornet's that is not plain running takes her control for its
        /// duration. With nothing else asked, this fired on a mantle, an air dash, a silk skill -
        /// which is what had the Shade blinking on and off through the playable stretches of a dream.
        /// </para>
        /// <para>
        /// It cannot simply be dropped: it is what hides the Shade for the framed moments inside those
        /// same scenes, the Needolin memories among them, and no single flag the game sets tells those
        /// apart from a mantle. So it is narrowed twice instead. <see cref="HornetControlState.InOwnMove"/>
        /// removes the moves that can be named, which is what covers the ones that run long enough to
        /// outlast a debounce - the Drifter's Cloak on an updraft above all. What is left is short by
        /// construction, and <see cref="InferredHoldGraceSeconds"/> waits it out.
        /// </para>
        /// </summary>
        internal static bool EvaluateShadeHiddenInferred(HornetControlState state)
            => !ShadeStaysVisibleRegardless(state)
                && !state.InCutscene
                && !state.InOwnMove
                && state.ControlRelinquished
                && state.GameHudHidden
                && !state.Paused;

        /// <summary>
        /// The four that keep the Shade on screen whatever else is true. A bench and a conversation
        /// lock Hornet's controls but are not framed moments - the bench is where the Shade's charms
        /// are changed, and docking beside her through dialogue is the intended look. A transition is
        /// already fading the screen, and on death the Shade's own death animation is the thing worth
        /// watching.
        /// </summary>
        private static bool ShadeStaysVisibleRegardless(HornetControlState state)
            => state.AtBench || state.HeldByInteraction || state.Downed || state.Transitioning;

        /// <summary>Exposed for <c>SimpleHUD</c>, which takes the game's own hidden HUD as its cue.</summary>
        internal static bool GameHudHidden() => IsGameHudHidden();

        private static HornetControlState CaptureHornetControlState()
        {
            // Both accessors are the "give me whatever is already there" variants on purpose.
            // GameManager.instance logs an error and HeroController.instance runs a scene scan
            // when nothing is registered - neither is wanted from a per-frame check, and both are
            // extern calls that make this untestable outside a Unity player loop.
            var gm = MenuStateUtility.TryGetGameManager();
            var pd = !ReferenceEquals(gm, null) ? gm.playerData : null;
            var state = new HornetControlState
            {
                AtBench = !ReferenceEquals(pd, null) && pd.atBench
            };

            if (state.AtBench)
            {
                return state;
            }

            var hc = HeroController.UnsafeInstance;
            if (ReferenceEquals(hc, null))
            {
                // No hero to have lost control, so report the shape of ordinary gameplay.
                state.AcceptingInput = true;
                return state;
            }

            state.ControlRelinquished = hc.controlReqlinquished;
            state.AcceptingInput = hc.acceptingInput;
            state.Paused = hc.IsPaused();

            var c = hc.cState;
            if (c != null)
            {
                state.Transitioning = c.transitioning || hc.transitionState != HeroTransitionState.WAITING_TO_TRANSITION;
                state.Downed = c.dead || c.hazardDeath || c.hazardRespawning;
                state.InCutscene = c.isInCutsceneMovement;
                state.InOwnMove = IsInOwnMove(c);
            }

            state.HeldByInteraction = IsHeldByInteraction();
            state.InCutscene = state.InCutscene || IsInCutscene(gm);
            state.GameHudHidden = IsGameHudHidden();

            return state;
        }

        /// <summary>
        /// Whether Hornet is part-way through a move the player asked for.
        /// <para>
        /// This exists because <c>controlReqlinquished</c> does not mean what its name suggests.
        /// Nearly everything Hornet does beyond running and jumping is an FSM on the hero, and an FSM
        /// that wants to drive her takes her control away first - so a mantle and a cutscene set the
        /// same flag, and outside a memory only the game's own HUD tells them apart. Inside one the
        /// HUD is gone either way and that tiebreaker is worth nothing, which is what this replaces.
        /// </para>
        /// <para>
        /// Named moves rather than a debounce, because the moves that matter most here are the ones
        /// no debounce can cover: the Drifter's Cloak rides an updraft for as long as the player holds
        /// it, and a bind and a focus both run about a second. Everything shorter is left to
        /// <see cref="InferredHoldGraceSeconds"/> rather than listed out.
        /// </para>
        /// <para>
        /// <c>needolinPlayingMemory</c> is deliberately absent. Playing the Needolin into a memory is
        /// the moment the Shade was hidden for in the first place, and naming it here would put it
        /// back on screen for exactly that.
        /// </para>
        /// </summary>
        private static bool IsInOwnMove(HeroControllerStates c)
        {
            try
            {
                return c.mantling
                    || c.mantleRecovery
                    || c.inUpdraft
                    || c.isBinding
                    || c.focusing
                    || c.isToolThrowing
                    // The game's own name for "an FSM she asked for is driving her": it gates whether
                    // a fresh move may interrupt this one, so a scripted hold never carries it.
                    || c.isInCancelableFSMMove;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Whether an interactable is currently holding Hornet, which is how every conversation in the
        /// game works: <c>InteractableBase.DisableInteraction</c> parks itself in
        /// <c>InteractManager.BlockingInteractable</c> and relinquishes control in the same breath, and
        /// every NPC derives from it (<c>NPCControlBase : InteractableBase</c>). It covers the rest of
        /// the family too - readable signs, boards, item handovers.
        /// </summary>
        private static bool IsHeldByInteraction()
        {
            try
            {
                if (InteractManager.BlockingInteractable != null)
                {
                    return true;
                }
            }
            catch
            {
            }

            try
            {
                // The full-screen prompt canvas, which is not an interactable but holds her the same way.
                if (GenericMessageCanvas.IsActive)
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
        /// The game has several unrelated notions of "cutscene" and no one of them covers the rest, so
        /// ask all of them: a played-back cinematic sets <c>GameState.CUTSCENE</c>, a dedicated
        /// cutscene scene answers <c>InGameCutsceneInfo</c>, camera-level cinematics set
        /// <c>GameCameras.IsInCinematic</c>, and <c>cState.isInCutsceneMovement</c> (read by the
        /// caller) marks Hornet being walked around inside an ordinary scene.
        /// </summary>
        private static bool IsInCutscene(GameManager gm)
        {
            try
            {
                if (!ReferenceEquals(gm, null) && gm.GameState == GameState.CUTSCENE)
                {
                    return true;
                }
            }
            catch
            {
            }

            try
            {
                if (InGameCutsceneInfo.IsInCutscene)
                {
                    return true;
                }
            }
            catch
            {
            }

            try
            {
                var cameras = TryGetGameCameras();
                if (!ReferenceEquals(cameras, null) && cameras.IsInCinematic)
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
        /// Whether the game has slid its own HUD off the screen.
        /// <para>
        /// This is the tiebreaker, and it works because it is the same question one layer up: the game
        /// takes its HUD away exactly when it is taking the moment away from the player.
        /// <c>DialogueBox</c> sends the HUD canvas "OUT" as a conversation opens and "IN" as it
        /// closes, and the cutscene, boss-door, relic-board and reward-message code all drive it
        /// through <c>GameCameras.HUDOut</c>/<c>HUDIn</c>. Nothing Hornet does at the player's request
        /// touches it - her HUD stays up through the Needolin, silk skills, tools, the quick map and
        /// every movement move - which is precisely why those stopped reading as scripted holds.
        /// </para>
        /// <para>
        /// A false negative here degrades to the Shade simply carrying on as normal, which is what it
        /// did before this feature existed. That is the right way round for the failure to fall.
        /// </para>
        /// </summary>
        private static bool IsGameHudHidden()
        {
            try
            {
                var cameras = TryGetGameCameras();
                if (ReferenceEquals(cameras, null))
                {
                    return false;
                }

                return !cameras.IsHudVisible;
            }
            catch
            {
                return false;
            }
        }

        private static FieldInfo s_gameCamerasInstanceField;
        private static bool s_gameCamerasInstanceFieldResolved;

        /// <summary>
        /// <c>GameCameras.instance</c> logs an error and <c>SilentInstance</c> falls back to a scene
        /// scan when nothing is registered, and this runs every frame - so read the backing field, the
        /// same bargain <c>MenuStateUtility.TryGetGameManager</c> strikes.
        /// </summary>
        private static GameCameras TryGetGameCameras()
        {
            if (!s_gameCamerasInstanceFieldResolved)
            {
                s_gameCamerasInstanceFieldResolved = true;
                try
                {
                    s_gameCamerasInstanceField = typeof(GameCameras).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
                }
                catch
                {
                }
            }

            try
            {
                return s_gameCamerasInstanceField?.GetValue(null) as GameCameras;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Holds off reporting a lock until it has lasted <see cref="ControlLockGraceSeconds"/>.
        /// Releasing is immediate; only the locking direction waits. Swallows the brief handoffs at the
        /// seams of Hornet's moves, without letting a real cutscene look late.
        /// </summary>
        private static bool ApplyControlLockGrace(bool locked)
        {
            if (!locked)
            {
                s_controlLockPendingSince = -1f;
                return false;
            }

            // Unscaled: a cutscene that stops time still ends, and the Shade should dock for it.
            float now = Time.unscaledTime;
            if (s_controlLockPendingSince < 0f || s_controlLockPendingSince > now)
            {
                s_controlLockPendingSince = now;
            }

            return now - s_controlLockPendingSince >= ControlLockGraceSeconds;
        }

        /// <summary>
        /// Holds off taking the Shade off screen until an inferred hold has lasted
        /// <see cref="InferredHoldGraceSeconds"/>. As with the lock above, only the hiding direction
        /// waits; showing it again is immediate.
        /// <para>
        /// The clock runs for a named cutscene as well as an inferred hold, even though a named one
        /// never waits on it. That is what keeps a cutscene which stops being named partway through -
        /// while still holding Hornet - from starting a fresh wait and blinking the Shade back into
        /// shot for half a second in the middle of it.
        /// </para>
        /// </summary>
        private static bool ApplyInferredHoldGrace(bool hidden, bool isInferred)
        {
            if (!hidden)
            {
                s_inferredHoldPendingSince = -1f;
                return false;
            }

            // Unscaled: a cutscene that stops time still ends, and the Shade should go for it.
            float now = Time.unscaledTime;
            if (s_inferredHoldPendingSince < 0f || s_inferredHoldPendingSince > now)
            {
                s_inferredHoldPendingSince = now;
            }

            if (!isInferred)
            {
                return true;
            }

            return now - s_inferredHoldPendingSince >= InferredHoldGraceSeconds;
        }

        /// <summary>
        /// Which way Hornet is facing, in the Shade's own facing convention (1 = right).
        /// </summary>
        private static int GetHornetFacing(int fallback)
        {
            try
            {
                var hc = HeroController.UnsafeInstance;
                if (hc != null)
                {
                    return hc.cState.facingRight ? 1 : -1;
                }
            }
            catch
            {
            }

            return fallback;
        }

        /// <summary>
        /// Parks the Shade beside Hornet and matches her facing, ignoring movement input entirely.
        /// Used while <see cref="HornetControlsLocked"/> is true so the Shade doesn't drift around
        /// mid-cutscene or wander off while Hornet is sat on a bench.
        /// </summary>
        private void HandleDockedMovement(float deltaTime)
        {
            isSprinting = false;
            sprintDashTimer = 0f;
            inHardLeash = false;
            hardLeashTimer = 0f;

            int hornetFacing = GetHornetFacing(facing);
            Vector3 hornetWorld = hornetTransform.position;

            // Dock behind Hornet rather than in front of her, so the Shade never covers whatever the
            // cutscene or bench interaction is actually showing.
            Vector2 target = new Vector2(hornetWorld.x - hornetFacing * dockOffsetX, hornetWorld.y + dockOffsetY);
            Vector2 current = rb ? rb.position : (Vector2)transform.position;
            Vector2 next = Vector2.MoveTowards(current, target, dockApproachSpeed * deltaTime);

            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.MovePosition(next);
            }
            else
            {
                transform.position = next;
            }

            lastMoveDelta = next - current;

            facing = hornetFacing;
            if (sr != null) sr.flipX = (facing == 1);
        }

        /// <summary>
        /// Takes the Shade off screen for a scripted hold and puts it back afterwards. It keeps
        /// updating and stays where it was, so a hold ending mid-parkour hands back a Shade exactly
        /// where the player left it.
        /// <para>
        /// Only the two visuals that are on unconditionally are touched; everything else the Shade
        /// draws is already cancelled when the control lock engages. The cloned hero light follows
        /// <c>sr</c> in <c>SyncShadeLight</c>, so it needs no line here.
        /// </para>
        /// </summary>
        private void ApplyScriptedHoldVisibility(bool hidden)
        {
            if (hidden == hiddenForScriptedHold)
            {
                return;
            }

            hiddenForScriptedHold = hidden;

            if (sr)
            {
                sr.enabled = !hidden;
            }

            if (shadowParticleRenderer)
            {
                shadowParticleRenderer.enabled = !hidden;
            }
        }

        private static bool HornetIsDowned()
        {
            try
            {
                var hc = HeroController.instance;
                if (hc != null)
                {
                    var state = hc.cState;
                    if (state.dead || state.hazardDeath || state.hazardRespawning)
                        return true;
                }
            }
            catch
            {
            }

            return false;
        }

        private DynamicLeashLimits GetDynamicLeashLimits(Vector3 hornetWorld)
        {
            var limits = new DynamicLeashLimits
            {
                X = new AxisLeashLimits
                {
                    NegativeSoft = softLeashRadius,
                    PositiveSoft = softLeashRadius,
                    NegativeHard = hardLeashRadius,
                    PositiveHard = hardLeashRadius,
                    NegativeSnap = snapLeashRadius,
                    PositiveSnap = snapLeashRadius
                },
                Y = new AxisLeashLimits
                {
                    NegativeSoft = softLeashRadius,
                    PositiveSoft = softLeashRadius,
                    NegativeHard = hardLeashRadius,
                    PositiveHard = hardLeashRadius,
                    NegativeSnap = snapLeashRadius,
                    PositiveSnap = snapLeashRadius
                }
            };

            try
            {
                var gm = GameManager.instance;
                var camCtrl = gm != null ? gm.cameraCtrl : null;
                var cam = camCtrl != null ? camCtrl.cam : null;
                if (cam != null)
                {
                    Vector3 viewport = cam.WorldToViewportPoint(hornetWorld);
                    float depth = viewport.z;
                    if (depth > 0f)
                    {
                        Vector3 leftWorld = cam.ViewportToWorldPoint(new Vector3(0f, viewport.y, depth));
                        Vector3 rightWorld = cam.ViewportToWorldPoint(new Vector3(1f, viewport.y, depth));
                        Vector3 bottomWorld = cam.ViewportToWorldPoint(new Vector3(viewport.x, 0f, depth));
                        Vector3 topWorld = cam.ViewportToWorldPoint(new Vector3(viewport.x, 1f, depth));

                        float roomMultiplier = GetLeashRoomMultiplier();
                        float roomFloor = GetLeashRoomFloor();
                        float leftRoom = Mathf.Max(roomFloor, Mathf.Max(0f, hornetWorld.x - leftWorld.x - LeashScreenPadding) * roomMultiplier);
                        float rightRoom = Mathf.Max(roomFloor, Mathf.Max(0f, rightWorld.x - hornetWorld.x - LeashScreenPadding) * roomMultiplier);
                        float downRoom = Mathf.Max(roomFloor, Mathf.Max(0f, hornetWorld.y - bottomWorld.y - LeashScreenPadding) * roomMultiplier);
                        float upRoom = Mathf.Max(roomFloor, Mathf.Max(0f, topWorld.y - hornetWorld.y - LeashScreenPadding) * roomMultiplier);

                        ApplyAxisLimit(ref limits.X.NegativeSoft, ref limits.X.NegativeHard, ref limits.X.NegativeSnap, leftRoom);
                        ApplyAxisLimit(ref limits.X.PositiveSoft, ref limits.X.PositiveHard, ref limits.X.PositiveSnap, rightRoom);
                        ApplyAxisLimit(ref limits.Y.NegativeSoft, ref limits.Y.NegativeHard, ref limits.Y.NegativeSnap, downRoom);
                        ApplyAxisLimit(ref limits.Y.PositiveSoft, ref limits.Y.PositiveHard, ref limits.Y.PositiveSnap, upRoom);
                    }
                }
            }
            catch { }

            return limits;
        }

        private float GetRadialHardLimit(DynamicLeashLimits limits)
        {
            float axisMax = Mathf.Max(
                Mathf.Max(limits.X.NegativeHard, limits.X.PositiveHard),
                Mathf.Max(limits.Y.NegativeHard, limits.Y.PositiveHard));
            return Mathf.Max(maxDistance, axisMax);
        }

        private float GetRadialSnapLimit(DynamicLeashLimits limits)
        {
            float axisMax = Mathf.Max(
                Mathf.Max(limits.X.NegativeSnap, limits.X.PositiveSnap),
                Mathf.Max(limits.Y.NegativeSnap, limits.Y.PositiveSnap));
            return Mathf.Max(snapLeashRadius, axisMax);
        }

        private static void ApplyAxisLimit(ref float soft, ref float hard, ref float snap, float available)
        {
            soft = Mathf.Max(0f, soft);
            hard = Mathf.Max(0f, hard);
            snap = Mathf.Max(0f, snap);

            if (available <= 0f)
            {
                soft = 0f;
                hard = Mathf.Min(hard, 0f);
                snap = Mathf.Max(hard, Mathf.Min(snap, SnapMinWhenNoRoom));
                return;
            }

            float clampedHard = Mathf.Max(0f, available);
            hard = clampedHard;
            float desiredSoft = Mathf.Clamp(clampedHard * SoftLimitRatio, 0f, clampedHard);
            soft = desiredSoft;
            float desiredSnap = Mathf.Max(clampedHard * SnapExtraMultiplier, clampedHard + SnapExtraMin);
            snap = Mathf.Max(clampedHard, Mathf.Max(snap, desiredSnap));
        }

        private static bool BeyondAxis(float value, float negativeLimit, float positiveLimit)
        {
            if (value > 0f)
                return positiveLimit >= 0f && value > positiveLimit;
            if (value < 0f)
                return negativeLimit >= 0f && -value > negativeLimit;
            return false;
        }

        private static bool BeyondSnap(float value, float negativeSnap, float positiveSnap)
        {
            if (value > 0f)
                return positiveSnap >= 0f && value > positiveSnap;
            if (value < 0f)
                return negativeSnap >= 0f && -value > negativeSnap;
            return false;
        }

        private static float ComputeAxisRatio(float value, float negativeSoft, float positiveSoft, float negativeHard, float positiveHard)
        {
            if (value > 0f)
            {
                float soft = Mathf.Max(0f, positiveSoft);
                if (value <= soft)
                    return 0f;
                float hard = Mathf.Max(soft, positiveHard);
                if (hard <= soft + Mathf.Epsilon)
                    return 1f;
                float clamped = Mathf.Min(value, hard);
                return (clamped - soft) / Mathf.Max(0.0001f, hard - soft);
            }
            if (value < 0f)
            {
                float abs = -value;
                float soft = Mathf.Max(0f, negativeSoft);
                if (abs <= soft)
                    return 0f;
                float hard = Mathf.Max(soft, negativeHard);
                if (hard <= soft + Mathf.Epsilon)
                    return 1f;
                float clamped = Mathf.Min(abs, hard);
                return (clamped - soft) / Mathf.Max(0.0001f, hard - soft);
            }
            return 0f;
        }

        private static float ClampAxis(float value, float negativeLimit, float positiveLimit)
        {
            float min = negativeLimit > 0f ? -negativeLimit : 0f;
            float max = positiveLimit > 0f ? positiveLimit : 0f;
            if (negativeLimit <= 0f && positiveLimit <= 0f)
                return 0f;
            return Mathf.Clamp(value, min, max);
        }

        private void HandleMovementAndFacing(float deltaTime)
        {
            // Hit-stun and knockback still win, so a hit landed just as a cutscene starts still reads
            // as a hit rather than the Shade sliding calmly into place.
            if (hornetControlsLocked && damageStaggerTimer <= 0f && knockbackTimer <= 0f)
            {
                HandleDockedMovement(deltaTime);
                return;
            }

            bool blockForFocus = isFocusing && !allowFocusMovement;
            bool blockForOtherSpells = isCastingSpell && !isFocusing;
            if (blockForFocus || blockForOtherSpells)
            {
                if (rb) rb.linearVelocity = Vector2.zero;
                lastMoveDelta = Vector2.zero;
                isSprinting = false;
                sprintDashTimer = 0f;
                inHardLeash = false;
                hardLeashTimer = 0f;
                return;
            }
            Vector2 input = capturedMoveInput;
            float h = capturedHorizontalInput;

            Vector3 hornetWorld = hornetTransform.position;
            Vector2 hornetPos2D = new Vector2(hornetWorld.x, hornetWorld.y);
            Vector2 currentPos = rb ? rb.position : (Vector2)transform.position;
            Vector2 offsetFromHornet = currentPos - hornetPos2D;
            Vector2 toHornet = -offsetFromHornet;
            float dist = toHornet.magnitude;

            var leash = GetDynamicLeashLimits(hornetWorld);
            float radialHardLimit = GetRadialHardLimit(leash);
            float radialSnapLimit = GetRadialSnapLimit(leash);

            if (BeyondSnap(offsetFromHornet.x, leash.X.NegativeSnap, leash.X.PositiveSnap) ||
                BeyondSnap(offsetFromHornet.y, leash.Y.NegativeSnap, leash.Y.PositiveSnap) ||
                dist > radialSnapLimit)
            {
                TeleportToHornet();
                inHardLeash = false; hardLeashTimer = 0f; EnableCollisions(true);
                return;
            }

            Vector2 moveDelta = Vector2.zero;

            if (BeyondAxis(offsetFromHornet.x, leash.X.NegativeHard, leash.X.PositiveHard) ||
                BeyondAxis(offsetFromHornet.y, leash.Y.NegativeHard, leash.Y.PositiveHard))
            {
                inHardLeash = true;
                hardLeashTimer += deltaTime;
                EnableCollisions(false);
                Vector2 dir = toHornet.sqrMagnitude > 0.0001f ? toHornet.normalized : Vector2.zero;
                moveDelta = dir * hardPullSpeed * deltaTime;
                if (hardLeashTimer >= hardLeashTimeout)
                {
                    TeleportToHornet();
                    inHardLeash = false; hardLeashTimer = 0f; EnableCollisions(true);
                    return;
                }
            }
            else
            {
                if (inHardLeash)
                {
                    inHardLeash = false;
                    hardLeashTimer = 0f;
                    EnableCollisions(true);
                }

                float ratioX = ComputeAxisRatio(offsetFromHornet.x, leash.X.NegativeSoft, leash.X.PositiveSoft, leash.X.NegativeHard, leash.X.PositiveHard);
                float ratioY = ComputeAxisRatio(offsetFromHornet.y, leash.Y.NegativeSoft, leash.Y.PositiveSoft, leash.Y.NegativeHard, leash.Y.PositiveHard);
                float pullStrength = Mathf.Max(ratioX, ratioY);
                if (pullStrength > 0f)
                {
                    Vector2 dir = toHornet.sqrMagnitude > 0.0001f ? toHornet.normalized : Vector2.zero;
                    moveDelta += dir * Mathf.Lerp(softPullSpeed, softPullSpeed * 1.5f, pullStrength) * deltaTime;
                }
            }

            if (!inHardLeash)
            {
                float speed = moveSpeed;
                bool sprinting = capturedSprintHeld && input.sqrMagnitude > 0f;
                bool startedSprint = sprinting && !isSprinting;
                if (startedSprint)
                {
                    SpawnSprintBurst(-input.normalized);
                    if (sprintDashCooldownTimer <= 0f)
                    {
                        sprintDashTimer = sprintDashDuration;
                        sprintDashCooldownTimer = sprintDashCooldown;
                        TryPlayDashSfx();
                    }
                }
                if (sprintDashTimer > 0f)
                {
                    speed *= sprintDashMultiplier;
                    sprintDashTimer -= deltaTime;
                    if (activeDashPs)
                    {
                        var emit = new ParticleSystem.EmitParams();
                        emit.velocity = activeDashDir * UnityEngine.Random.Range(4f, 8f);
                        emit.startSize = UnityEngine.Random.Range(0.15f, 0.25f);
                        activeDashPs.Emit(emit, 1);
                    }
                }
                else
                {
                    activeDashPs = null;
                    if (sprinting)
                    {
                        speed *= sprintMultiplier;
                    }
                }
                if (sprintDashCooldownTimer > 0f)
                    sprintDashCooldownTimer -= deltaTime;

                moveDelta += input * speed * deltaTime;
                isSprinting = sprinting;
            }
            else
            {
                isSprinting = false;
                sprintDashTimer = 0f;
            }

            bool dashNowActive = !inHardLeash && sprintDashTimer > 0f;
            bool sharpShadowShouldBeActive = dashNowActive && sharpShadowEquipped && IsVoidHeartEvading();
            if (sharpShadowShouldBeActive)
            {
                if (!sharpShadowDashActive)
                {
                    sharpShadowDashActive = true;
                    EnsureSharpShadowDashHitbox();
                }
                UpdateSharpShadowDashHitbox();
            }
            else if (sharpShadowDashActive)
            {
                sharpShadowDashActive = false;
                DestroySharpShadowDashHitbox();
            }

            if (knockbackTimer > 0f)
            {
                moveDelta += knockbackVelocity * deltaTime;
                knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, 10f * deltaTime);
                knockbackTimer -= deltaTime;
            }

            Vector2 proposed = currentPos + moveDelta;
            proposed = ClampAgainstTransitionGates(proposed);

            Vector2 proposedOffset = proposed - hornetPos2D;
            proposedOffset.x = ClampAxis(proposedOffset.x, leash.X.NegativeHard, leash.X.PositiveHard);
            proposedOffset.y = ClampAxis(proposedOffset.y, leash.Y.NegativeHard, leash.Y.PositiveHard);
            Vector2 clampedPos = hornetPos2D + proposedOffset;

            Vector2 finalToHornet = hornetPos2D - clampedPos;
            float finalDist = finalToHornet.magnitude;
            if (finalDist > radialHardLimit && finalDist > 0f)
            {
                clampedPos = hornetPos2D - finalToHornet.normalized * radialHardLimit;
                Vector2 clampedOffset = clampedPos - hornetPos2D;
                clampedOffset.x = ClampAxis(clampedOffset.x, leash.X.NegativeHard, leash.X.PositiveHard);
                clampedOffset.y = ClampAxis(clampedOffset.y, leash.Y.NegativeHard, leash.Y.PositiveHard);
                clampedPos = hornetPos2D + clampedOffset;
            }

            if (rb) rb.MovePosition(clampedPos);
            else transform.position = clampedPos;
            lastMoveDelta = clampedPos - currentPos;

            if (aiFacingOverride != 0)
            {
                facing = aiFacingOverride;
            }
            else if (h > 0.1f) facing = 1;
            else if (h < -0.1f) facing = -1;

            if (sr != null) sr.flipX = (facing == 1);
        }

        private void CheckSprintUnlock()
        {
            if (sprintUnlocked) return;
            try
            {
                var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
                if (pd != null && pd.hasDash)
                    sprintUnlocked = true;
            }
            catch { }
        }

        private bool InArenaFight()
        {
            if (BossSceneController.IsBossScene) return true;
            try
            {
                battleCheckTimer -= Time.deltaTime;
                if (battleCheckTimer <= 0f || cachedBattle == null)
                {
                    cachedBattle = UnityEngine.Object.FindFirstObjectByType<BattleScene>();
                    battleCheckTimer = 1f;
                }
                if (cachedBattle != null && s_battleSceneStartedField != null)
                {
                    if ((bool)s_battleSceneStartedField.GetValue(cachedBattle))
                        return true;
                }
            }
            catch { }
            return false;
        }

        /// <summary>Leash multiplier while an AI drives the Shade. See the note in the body.</summary>
        private const float ShadeAiLeashMultiplier = 2.5f;

        private void AdjustLeashForCamera()
        {
            try
            {
                var cam = GameManager.instance?.cameraCtrl;
                bool locked = cam != null && cam.mode == CameraController.CameraMode.LOCKED;
                bool arena = locked && InArenaFight();

                float multiplier = arena ? 3f : 1f;

                // An AI-driven Shade should be able to go anywhere on screen: the fixed radii were
                // tuned for a second player who keeps themselves near Hornet by choice, and they cut
                // the command reticle off well inside the visible screen. Raising them is safe
                // because it does not actually let the Shade off screen - GetDynamicLeashLimits
                // overwrites the per-axis limits with the real room to the screen edge either way, so
                // this only stops the radii being the tighter of the two constraints.
                if (aiEnabled)
                {
                    multiplier = Mathf.Max(multiplier, ShadeAiLeashMultiplier);
                }

                maxDistance = baseMaxDistance * multiplier;
                softLeashRadius = baseSoftLeashRadius * multiplier;
                hardLeashRadius = baseHardLeashRadius * multiplier;
                snapLeashRadius = baseSnapLeashRadius * multiplier;
            }
            catch { }
        }

        /// <summary>
        /// How much further than the visible screen the Shade may sit while it is holding a spot the
        /// player sent it to and Hornet is in the air.
        /// <para>
        /// The point of telling the Shade to wait is usually that it was in the way of a jump. Ending
        /// the wait at the screen edge undoes that exactly when it matters, because a platforming
        /// section is routinely wider than one screen - the Shade would be recalled to Hornet's side
        /// halfway through the thing the player moved it out of the way for. So while she is off the
        /// ground the room is widened, and it snaps back to normal the moment she lands.
        /// </para>
        /// </summary>
        private const float ShadeAiParkourRoomMultiplier = 2.2f;

        private float GetLeashRoomMultiplier()
        {
            if (!aiEnabled || !HasShadeAiCommand)
            {
                return 1f;
            }

            return aiHornetAirborne ? ShadeAiParkourRoomMultiplier : 1f;
        }

        /// <summary>
        /// The smallest leash room a standing order needs to be keepable.
        /// <para>
        /// The reticle reaches further than the screen does, so an order can be placed somewhere the
        /// screen-derived room would clamp the Shade short of - and a Shade that stops short of where
        /// it was sent looks broken for no visible reason. The order's own distance is therefore a
        /// floor under the room, for as long as it stands.
        /// </para>
        /// </summary>
        private float GetLeashRoomFloor()
        {
            return aiEnabled && HasShadeAiCommand ? aiCommandLeashFloor : 0f;
        }

        private void SpawnSprintBurst(Vector2 dir)
        {
            try
            {
                Vector2 ndir = dir.normalized;
                GameObject go = new GameObject("ShadeSprintBurst");
                go.transform.position = transform.position;
                var ps = go.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.startLifetime = 0.4f;
                main.startSpeed = 0f;
                main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.25f);
                main.startColor = Color.black;
                var psr = ps.GetComponent<ParticleSystemRenderer>();
                if (psr != null)
                {
                    if (s_sprintBurstMat == null)
                    {
                        s_sprintBurstMat = new Material(Shader.Find("Sprites/Default"));
                        s_sprintBurstMat.color = Color.black;
                    }
                    psr.sharedMaterial = s_sprintBurstMat;
                    psr.sharedMaterial.mainTexture = MakeDotSprite().texture;
                }
                var col = ps.colorOverLifetime;
                col.enabled = true;
                Gradient g = new Gradient();
                g.SetKeys(
                    new[] { new GradientColorKey(Color.black, 0f), new GradientColorKey(Color.black, 1f) },
                    new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
                col.color = g;
                var emission = ps.emission;
                emission.enabled = false;
                for (int i = 0; i < 12; i++)
                {
                    var emit = new ParticleSystem.EmitParams();
                    emit.velocity = ndir * UnityEngine.Random.Range(4f, 8f);
                    emit.startColor = Color.black;
                    emit.startSize = UnityEngine.Random.Range(0.15f, 0.25f);
                    ps.Emit(emit, 1);
                }
                ps.Play();
                activeDashPs = ps;
                activeDashDir = ndir;
                Destroy(go, 1f);
            }
            catch { }
        }

        internal void SetFuryModeActive(bool active)
        {
            try
            {
                if (active)
                {
                    EnsureFuryAura();
                    if (furyAuraPs)
                    {
                        if (!furyAuraPs.gameObject.activeSelf)
                        {
                            furyAuraPs.gameObject.SetActive(true);
                        }

                        var emission = furyAuraPs.emission;
                        emission.enabled = true;
                        if (!furyAuraPs.isPlaying)
                        {
                            furyAuraPs.Play();
                        }
                    }
                }
                else if (furyAuraPs)
                {
                    furyAuraPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
            catch { }
        }

    }
}
#nullable restore
