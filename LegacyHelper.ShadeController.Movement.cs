#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using GlobalEnums;

public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        private void Update()
        {
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
            if (ShadeInput.WasActionPressed(ShadeAction.AssistMode))
            {
                assistModeEnabled = !assistModeEnabled;
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
                }
            }
            wasInactive = isInactive;

            HandleTeleportChannel();

            CheckSprintUnlock();
            AdjustLeashForCamera();

            CaptureMovementInput();
            // Allow starting focus even when not casting other spells; focusing itself sets isCastingSpell
            if (!inHardLeash && !isChannelingTeleport && !isInactive && damageStaggerTimer <= 0f)
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
            SyncShadeLight();
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

                        float leftRoom = Mathf.Max(0f, hornetWorld.x - leftWorld.x - LeashScreenPadding);
                        float rightRoom = Mathf.Max(0f, rightWorld.x - hornetWorld.x - LeashScreenPadding);
                        float downRoom = Mathf.Max(0f, hornetWorld.y - bottomWorld.y - LeashScreenPadding);
                        float upRoom = Mathf.Max(0f, topWorld.y - hornetWorld.y - LeashScreenPadding);

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

            if (h > 0.1f) facing = 1;
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
                if (cachedBattle != null)
                {
                    var f = typeof(BattleScene).GetField("started", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (f != null && (bool)f.GetValue(cachedBattle))
                        return true;
                }
            }
            catch { }
            return false;
        }

        private void AdjustLeashForCamera()
        {
            try
            {
                var cam = GameManager.instance?.cameraCtrl;
                bool locked = cam != null && cam.mode == CameraController.CameraMode.LOCKED;
                bool arena = locked && InArenaFight();
                if (arena)
                {
                    maxDistance = baseMaxDistance * 3f;
                    softLeashRadius = baseSoftLeashRadius * 3f;
                    hardLeashRadius = baseHardLeashRadius * 3f;
                    snapLeashRadius = baseSnapLeashRadius * 3f;
                }
                else
                {
                    maxDistance = baseMaxDistance;
                    softLeashRadius = baseSoftLeashRadius;
                    hardLeashRadius = baseHardLeashRadius;
                    snapLeashRadius = baseSnapLeashRadius;
                }
            }
            catch { }
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
