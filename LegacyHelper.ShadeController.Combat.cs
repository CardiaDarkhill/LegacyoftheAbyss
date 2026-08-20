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
        private void SetupPhysics()
        {
            rb = GetComponent<Rigidbody2D>();
            if (!rb) rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            bodyCol = GetComponent<Collider2D>();
            if (!bodyCol)
            {
                var cap = gameObject.AddComponent<CapsuleCollider2D>();
                cap.direction = CapsuleDirection2D.Vertical;
                cap.size = new Vector2(0.9f, 1.4f);
                cap.isTrigger = false;
                bodyCol = cap;
            }
            else
            {
                bodyCol.isTrigger = false;
            }

            // Add an additional trigger collider to interact with Hornet's attack triggers (for pogo)
            try
            {
                var existingTriggers = GetComponents<Collider2D>();
                bool hasTrigger = false;
                foreach (var c in existingTriggers) if (c && c.isTrigger) { hasTrigger = true; break; }
                if (!hasTrigger)
                {
                    var trigger = gameObject.AddComponent<CapsuleCollider2D>();
                    trigger.direction = CapsuleDirection2D.Vertical;
                    trigger.size = new Vector2(0.95f, 1.5f);
                    trigger.isTrigger = true;
                }
            }
            catch { }

            try
            {
                var hc = HeroController.instance;
                if (hc)
                {
                    // Place shade on a non-hero layer to avoid triggering transitions and interactables
                    int heroLayer = hc.gameObject.layer; // typically 9
                    int desiredLayer = LayerMask.NameToLayer("Default");
                    if (desiredLayer < 0 || desiredLayer == heroLayer)
                    {
                        // Fallback to a safe built-in layer that is not the hero layer
                        int ignoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                        desiredLayer = (ignoreRaycast >= 0 && ignoreRaycast != heroLayer) ? ignoreRaycast : 0;
                    }

                    gameObject.layer = desiredLayer;
                    // Apply to immediate children we control
                    var allTransforms = GetComponentsInChildren<Transform>(true);
                    foreach (var t in allTransforms)
                    {
                        if (!t) continue;
                        t.gameObject.layer = desiredLayer;
                    }

                    // Still ignore collisions with Hornet (handled in a helper so we can call it later too)
                    EnsureIgnoreHornetCollisions();

                    // Initial enemy ignore pass
                    RefreshCollisionIgnores();
                }
            }
            catch { }
        }

        private void RefreshCollisionIgnores()
        {
            try
            {
                var myCols = GetComponentsInChildren<Collider2D>(true);
                if (myCols == null || myCols.Length == 0) return;

                // Ignore physical collisions with enemies (objects with HealthManager) but keep triggers for damage/hazards
                HealthManager[] enemies = null;
                try
                {
                    enemies = UnityEngine.Object.FindObjectsByType<HealthManager>(
                        FindObjectsInactive.Exclude,
                        FindObjectsSortMode.None);
                }
                catch { enemies = null; }
                if (enemies != null)
                {
                    foreach (var hm in enemies)
                    {
                        if (!hm) continue;
                        var cols = hm.GetComponentsInChildren<Collider2D>(true);
                        foreach (var ec in cols)
                        {
                            if (!ec || ec.isTrigger) continue; // don't ignore triggers to still receive hazard/damage events
                            foreach (var mc in myCols) if (mc) Physics2D.IgnoreCollision(mc, ec, true);
                        }
                    }
                }
            }
            catch { }
        }

        private void EnsureIgnoreHornetCollisions()
        {
            try
            {
                var hc = HeroController.instance;
                if (!hc) return;
                var myCols = GetComponentsInChildren<Collider2D>(true);
                var hornetCols = hc.transform.root.GetComponentsInChildren<Collider2D>(true);
                int heroAttackLayer = LayerMask.NameToLayer("Hero Attack");
                foreach (var mc in myCols)
                {
                    if (!mc) continue;
                    foreach (var hcCol in hornetCols)
                    {
                        if (!hcCol) continue;
                        if (mc.isTrigger || hcCol.isTrigger) continue;
                        if (hcCol.gameObject.layer == heroAttackLayer) continue; // allow hero attack contact
                        // Allow slashes (which may not be on Hero Attack layer) by checking their components
                        bool isSlash = false;
                        try { if (hcCol.GetComponentInParent<NailSlashTerrainThunk>()) isSlash = true; } catch { }
                        if (isSlash) continue;
                        Physics2D.IgnoreCollision(mc, hcCol, true);
                    }
                }
            }
            catch { }
        }

        private void EnableCollisions(bool enable)
        {
            try
            {
                if (bodyCol) bodyCol.enabled = enable;
                var extraCols = GetComponentsInChildren<Collider2D>(true);
                foreach (var c in extraCols) if (c && c != bodyCol) c.enabled = enable;
            }
            catch { }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryProcessDamageHero(collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryProcessDamageHero(other);
        }

        private bool ShouldIgnoreDamageSource(Component c)
        {
            if (!c) return false;
            try
            {
                string s = (c.name + " " + c.tag).ToLowerInvariant();
                foreach (var token in IgnoreDamageTokens)
                    if (s.Contains(token))
                        return true;
            }
            catch { }
            return false;
        }

        private void TryProcessDamageHero(Collider2D col)
        {
            if (!col) return;
            try
            {
                if (bodyCol && col && !bodyCol.IsTouching(col)) return;
                if (col.transform == transform || col.transform.IsChildOf(transform)) return;
                if (hornetTransform)
                {
                    var hornetRoot = hornetTransform.root;
                    if (col.transform == hornetTransform || col.transform.IsChildOf(hornetTransform) || col.transform.root == hornetRoot)
                        return;
                }
                var dh = col.GetComponentInParent<DamageHero>();
                if (dh != null)
                {
                    if (ShouldIgnoreDamageSource(col) || ShouldIgnoreDamageSource(dh)) { LogShadeDamage(dh, col, false); return; }
                    bool canDamage = false;
                    try { canDamage = dh.enabled && dh.CanCauseDamage; } catch { }
                    if (!canDamage) { LogShadeDamage(dh, col, false); return; }
                    int dmg = GetDamageAmount(dh);
                    var hz = GetHazardType(dh);
                    if (hz == GlobalEnums.HazardType.STEAM && dmg <= 0)
                    {
                        LogShadeDamage(dh, col, false);
                        return;
                    }
                    if (IsTerrainHazard(hz))
                    {
                        if (dmg <= 0)
                        {
                            LogShadeDamage(dh, col, false);
                            return;
                        }

                        LogShadeDamage(dh, col, canTakeDamage);
                        OnShadeHitHazard();
                        return;
                    }
                    if (dmg > 0)
                    {
                        bool preventedByVoidHeart = IsVoidHeartEvading();
                        LogShadeDamage(dh, col, canTakeDamage && !preventedByVoidHeart);
                        if (preventedByVoidHeart)
                        {
                            int attempted = ApplyOvercharmPenalty(dmg);
                            HandleVoidHeartEvadePreventedHit(attempted);
                            return;
                        }

                        OnShadeHitEnemy(dh);
                    }
                    else { LogShadeDamage(dh, col, false); }
                }
            }
            catch { }
        }

        private void LogShadeDamage(DamageHero dh, Collider2D src, bool succeeded)
        {
            try
            {
                string obj = dh ? dh.gameObject?.name ?? dh.name : "<null>";
                string colName = src ? src.name : "<null>";
                string source = $"{obj} via {colName}";
                LoggingManager.LogShadeDamage(source, succeeded);
            }
            catch { }
        }

        private bool IsVoidHeartEvading()
        {
            return voidHeartEvadeActive && sprintDashTimer > 0f;
        }

        private void EnsureSharpShadowDashHitbox()
        {
            if (!sharpShadowEquipped)
            {
                return;
            }

            if (!IsVoidHeartEvading())
            {
                return;
            }

            if (sharpShadowDashHitbox)
            {
                if (!sharpShadowDashAoE)
                {
                    try { sharpShadowDashAoE = sharpShadowDashHitbox.GetComponent<ShadeAoE>(); }
                    catch { }
                }
                return;
            }

            GameObject hitbox;
            try
            {
                hitbox = new GameObject("ShadeSharpShadowHitbox");
            }
            catch
            {
                return;
            }

            hitbox.transform.SetParent(transform, false);
            hitbox.transform.localPosition = Vector3.zero;
            hitbox.transform.localRotation = Quaternion.identity;
            hitbox.transform.localScale = Vector3.one;
            hitbox.tag = "Hero Spell";
            try
            {
                int spellLayer = LayerMask.NameToLayer("Hero Spell");
                int atkLayer = LayerMask.NameToLayer("Hero Attack");
                if (spellLayer >= 0) hitbox.layer = spellLayer;
                else if (atkLayer >= 0) hitbox.layer = atkLayer;
            }
            catch
            {
            }

            Collider2D collider = CloneSharpShadowDashCollider(hitbox);
            if (!collider)
            {
                try
                {
                    var cap = hitbox.AddComponent<CapsuleCollider2D>();
                    cap.direction = CapsuleDirection2D.Vertical;
                    cap.size = new Vector2(0.95f, 1.5f);
                    cap.offset = Vector2.zero;
                    cap.isTrigger = true;
                    collider = cap;
                }
                catch
                {
                }
            }

            ShadeAoE aoe = null;
            try
            {
                aoe = hitbox.AddComponent<ShadeAoE>();
                aoe.ConfigureDamage(GetShadeNailDamage(), applyDamageMultiplier: false);
                aoe.hornetRoot = hornetTransform;
                aoe.lifeSeconds = 0f;
                aoe.attackType = AttackTypes.Nail;
                aoe.direction = GetSharpShadowDashAngle();
                aoe.magnitudeMultiplier = Mathf.Max(0.01f, charmNailKnockbackMultiplier);
                aoe.multiplier = 1f;
                aoe.isHeroDamage = true;
                aoe.isFirstHit = true;
                aoe.sourceOverride = gameObject;
            }
            catch
            {
            }

            sharpShadowDashHitbox = hitbox;
            sharpShadowDashAoE = aoe;

            if (collider)
            {
                collider.isTrigger = true;
                IgnoreHornetForCollider(collider);
            }
        }

        private void UpdateSharpShadowDashHitbox()
        {
            if (!sharpShadowDashHitbox)
            {
                sharpShadowDashAoE = null;
                return;
            }

            if (!sharpShadowDashAoE)
            {
                try { sharpShadowDashAoE = sharpShadowDashHitbox.GetComponent<ShadeAoE>(); }
                catch { }
            }

            if (!sharpShadowDashAoE)
            {
                return;
            }

            try
            {
                sharpShadowDashHitbox.transform.localPosition = Vector3.zero;
                sharpShadowDashHitbox.transform.localRotation = Quaternion.identity;
            }
            catch
            {
            }

            sharpShadowDashAoE.direction = GetSharpShadowDashAngle();
            sharpShadowDashAoE.magnitudeMultiplier = Mathf.Max(0.01f, charmNailKnockbackMultiplier);
            sharpShadowDashAoE.sourceOverride = gameObject;
        }

        private void DestroySharpShadowDashHitbox()
        {
            if (sharpShadowDashHitbox)
            {
                try { Destroy(sharpShadowDashHitbox); }
                catch { }
            }

            sharpShadowDashHitbox = null;
            sharpShadowDashAoE = null;
        }

        private Collider2D CloneSharpShadowDashCollider(GameObject owner)
        {
            if (!owner)
            {
                return null;
            }

            if (!bodyCol)
            {
                return null;
            }

            try
            {
                if (bodyCol is CapsuleCollider2D cap)
                {
                    var clone = owner.AddComponent<CapsuleCollider2D>();
                    clone.direction = cap.direction;
                    clone.size = cap.size;
                    clone.offset = cap.offset;
                    clone.isTrigger = true;
                    return clone;
                }

                if (bodyCol is CircleCollider2D circle)
                {
                    var clone = owner.AddComponent<CircleCollider2D>();
                    clone.radius = circle.radius;
                    clone.offset = circle.offset;
                    clone.isTrigger = true;
                    return clone;
                }

                if (bodyCol is BoxCollider2D box)
                {
                    var clone = owner.AddComponent<BoxCollider2D>();
                    clone.size = box.size;
                    clone.offset = box.offset;
                    clone.isTrigger = true;
                    return clone;
                }
            }
            catch
            {
            }

            return null;
        }

        private float GetSharpShadowDashAngle()
        {
            Vector2 dir = lastMoveDelta;
            if (dir.sqrMagnitude < 0.0001f)
            {
                try
                {
                    if (rb && rb.linearVelocity.sqrMagnitude > 0.0001f)
                    {
                        dir = rb.linearVelocity;
                    }
                }
                catch
                {
                }

                if (dir.sqrMagnitude < 0.0001f)
                {
                    dir = new Vector2(facing >= 0 ? 1f : -1f, 0f);
                }
            }

            float angle;
            try
            {
                angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            }
            catch
            {
                angle = facing >= 0 ? 0f : 180f;
            }

            if (float.IsNaN(angle))
            {
                angle = facing >= 0 ? 0f : 180f;
            }

            return angle;
        }

        private void HandleVoidHeartEvadePreventedHit(int attemptedDamage)
        {
            int attempted = Mathf.Max(0, attemptedDamage);
            if (attempted > 0)
            {
                DispatchCharmDamageEvent(attempted, 0, wasHazard: false, prevented: true, lethal: false);
            }
        }

        private void TeleportToHornet()
        {
            if (!hornetTransform) return;
            bool hadSim = rb ? rb.simulated : false;
            if (rb) rb.simulated = false;
            transform.position = hornetTransform.position;
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = hadSim;
            }
        }

        public void TeleportToPosition(Vector3 position)
        {
            bool hadSim = rb ? rb.simulated : false;
            if (rb) rb.simulated = false;
            transform.position = position;
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = hadSim;
            }
        }

        public void SuppressHazardDamage(float duration)
        {
            if (duration <= 0f)
            {
                return;
            }

            hazardCooldown = Mathf.Max(hazardCooldown, duration);
            hurtCooldown = Mathf.Max(hurtCooldown, duration);
        }

        public void ApplySceneTransitionProtection(float duration)
        {
            if (duration <= 0f)
            {
                return;
            }

            sceneProtectionTimer = Mathf.Max(sceneProtectionTimer, duration);
            bool activating = !sceneProtectionActive;
            sceneProtectionActive = true;
            sceneProtectionDesiredDamageState = !assistModeEnabled;
            if (activating && !sceneProtectionSuppressingPersistence)
            {
                EnterPersistenceSuppression();
                sceneProtectionSuppressingPersistence = true;
            }

            hazardCooldown = Mathf.Max(hazardCooldown, duration);
            hurtCooldown = Mathf.Max(hurtCooldown, duration);

            if (canTakeDamage)
            {
                canTakeDamage = false;
                PushShadeStatsToHud(suppressDamageAudio: true);
                PersistIfChanged();
            }
        }

        private bool SceneProtectionBlockedByOverlap()
        {
            if (!bodyCol)
            {
                return false;
            }

            try
            {
                var filter = new ContactFilter2D();
                filter.useTriggers = true;
                int count = bodyCol.Overlap(filter, sceneProtectionOverlapBuffer);
                for (int i = 0; i < count; i++)
                {
                    var c = sceneProtectionOverlapBuffer[i];
                    if (!c)
                    {
                        continue;
                    }

                    if (c.transform == transform || c.transform.IsChildOf(transform))
                    {
                        continue;
                    }

                    if (hornetTransform && c.transform.root == hornetTransform.root)
                    {
                        continue;
                    }

                    var dh = c.GetComponentInParent<DamageHero>();
                    if (dh == null)
                    {
                        continue;
                    }

                    if (ShouldIgnoreDamageSource(c) || ShouldIgnoreDamageSource(dh))
                    {
                        continue;
                    }

                    bool canDamage = false;
                    try { canDamage = dh.enabled && dh.CanCauseDamage; }
                    catch { }
                    if (!canDamage)
                    {
                        continue;
                    }

                    int dmg = GetDamageAmount(dh);
                    var hz = GetHazardType(dh);
                    if (hz == GlobalEnums.HazardType.STEAM && dmg <= 0)
                    {
                        continue;
                    }

                    if (IsTerrainHazard(hz))
                    {
                        if (dmg <= 0)
                        {
                            continue;
                        }

                        return true;
                    }

                    if (dmg > 0)
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        private Vector2 ClampAgainstTransitionGates(Vector2 proposed)
        {
            try
            {
                if (!bodyCol) return proposed;
                // Approximate shade bounds at proposed position using current extents
                var ext = bodyCol.bounds.extents;
                Vector2 min = proposed - (Vector2)ext;
                Vector2 max = proposed + (Vector2)ext;

                var hits = Physics2D.OverlapAreaAll(min, max);
                if (hits == null || hits.Length == 0) return proposed;

                foreach (var h in hits)
                {
                    if (!h) continue;
                    var tp = h.GetComponentInParent<TransitionPoint>();
                    if (tp == null) continue;
                    bool isDoor = false;
                    try { isDoor = tp.isADoor; } catch { }
                    if (isDoor) continue; // block only edge-of-map gates

                    var gb = h.bounds;
                    var gatePos = tp.GetGatePosition();
                    switch (gatePos)
                    {
                        case GlobalEnums.GatePosition.right:
                            if (proposed.x > gb.min.x - ext.x)
                                proposed.x = gb.min.x - ext.x;
                            break;
                        case GlobalEnums.GatePosition.left:
                            if (proposed.x < gb.max.x + ext.x)
                                proposed.x = gb.max.x + ext.x;
                            break;
                        case GlobalEnums.GatePosition.top:
                            if (proposed.y > gb.min.y - ext.y)
                                proposed.y = gb.min.y - ext.y;
                            break;
                        case GlobalEnums.GatePosition.bottom:
                            if (proposed.y < gb.max.y + ext.y)
                                proposed.y = gb.max.y + ext.y;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch { }
            return proposed;
        }

        private void CheckHazardOverlap()
        {
            if (hazardCooldown > 0f) return;
            if (!bodyCol) return;
            var filter = new ContactFilter2D();
            filter.useTriggers = true;
            Collider2D[] results = new Collider2D[16];
            int count = bodyCol.Overlap(filter, results);
            for (int i = 0; i < count; i++)
            {
                var c = results[i];
                if (!c) continue;
                if (c.transform == transform || c.transform.IsChildOf(transform)) continue;
                if (hornetTransform && c.transform.root == hornetTransform.root) continue;
                var dh = c.GetComponentInParent<DamageHero>();
                if (dh != null)
                {
                    if (ShouldIgnoreDamageSource(c) || ShouldIgnoreDamageSource(dh)) { LogShadeDamage(dh, c, false); continue; }
                    bool canDamage = false;
                    try { canDamage = dh.enabled && dh.CanCauseDamage; } catch { }
                    if (!canDamage) { LogShadeDamage(dh, c, false); continue; }
                    int dmg = GetDamageAmount(dh);
                    var hz = GetHazardType(dh);
                    if (hz == GlobalEnums.HazardType.STEAM && dmg <= 0)
                    {
                        LogShadeDamage(dh, c, false);
                        continue;
                    }
                    if (IsTerrainHazard(hz))
                    {
                        if (dmg <= 0)
                        {
                            LogShadeDamage(dh, c, false);
                            continue;
                        }

                        LogShadeDamage(dh, c, canTakeDamage);
                        OnShadeHitHazard();
                        return;
                    }
                    if (dmg > 0) { LogShadeDamage(dh, c, canTakeDamage); OnShadeHitEnemy(dh); return; }
                    LogShadeDamage(dh, c, false);
                }
            }
        }

        private static int GetDamageAmount(DamageHero dh)
        {
            if (!dh)
            {
                return 0;
            }

            try { return dh.damageDealt; }
            catch { }

            return 0;
        }

        private static GlobalEnums.HazardType GetHazardType(DamageHero dh)
        {
            try
            {
                var tf = typeof(DamageHero).GetField("hazardType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (tf != null) return (GlobalEnums.HazardType)tf.GetValue(dh);
            }
            catch { }
            return GlobalEnums.HazardType.NON_HAZARD;
        }

        private void ApplyKnockback(Vector2 sourcePos, float forceMultiplier = 1f, bool fromDamage = false, float duration = 0.2f)
        {
            if (fromDamage)
            {
                float stagger = DamageStaggerBaseDuration * damageStaggerDurationMultiplier;
                damageStaggerTimer = Mathf.Max(damageStaggerTimer, stagger);
            }

            if (knockbackSuppressionCount > 0)
            {
                knockbackVelocity = Vector2.zero;
                knockbackTimer = 0f;
                return;
            }

            try
            {
                Vector2 dir = ((Vector2)transform.position - sourcePos).normalized;
                float scale = Mathf.Max(0f, forceMultiplier);
                knockbackVelocity = dir * hitKnockbackForce * scale;
                knockbackTimer = Mathf.Max(0f, duration);
            }
            catch { }
        }

        private void ApplyAttackRecoil(Vector2 attackDirection)
        {
            if (attackDirection.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Vector2 normalized = attackDirection.normalized;
            ApplyKnockback((Vector2)transform.position + normalized, 2.5f, false, 0.15f);
        }

        private void OnShadeHitHazard()
        {
            if (hazardCooldown > 0f) return;
            TeleportToHornet();
            hazardCooldown = 0.25f;
            int attempted = ApplyOvercharmPenalty(1);

            if (!canTakeDamage)
            {
                DispatchCharmDamageEvent(attempted, 0, true, true, false);
                return;
            }

            if (TryPreventFocusDamage(attempted, true))
            {
                return;
            }

            if (TryPreventCarefreeMelody(attempted, true))
            {
                CancelFocus();
                return;
            }

            int actual = ApplyDamageToPools(attempted);
            bool lethal = GetTotalCurrentHealth() <= 0;
            if (lethal) StartDeathAnimation();
            PushShadeStatsToHud();
            CancelFocus();
            PersistIfChanged();
            DispatchCharmDamageEvent(attempted, actual, true, actual <= 0, lethal);
        }

        private void OnShadeHitEnemy(DamageHero dh)
        {
            if (hurtCooldown > 0f) return;
            int dmg = GetDamageAmount(dh);
            dmg = ApplyOvercharmPenalty(dmg);
            if (dmg <= 0)
            {
                DispatchCharmDamageEvent(0, 0, false, true, false);
                return; // ignore non-damaging triggers
            }
            if (IsVoidHeartEvading())
            {
                HandleVoidHeartEvadePreventedHit(dmg);
                return;
            }
            Vector2 srcPos = dh ? (Vector2)dh.transform.position : (Vector2)transform.position;
            if (!canTakeDamage)
            {
                hurtCooldown = currentHurtIFrameDuration;
                DispatchCharmDamageEvent(dmg, 0, false, true, false);
                return;
            }

            if (TryPreventFocusDamage(dmg, false))
            {
                hurtCooldown = currentHurtIFrameDuration;
                return;
            }

            if (TryPreventCarefreeMelody(dmg, false))
            {
                hurtCooldown = currentHurtIFrameDuration;
                CancelFocus();
                return;
            }

            int actual = ApplyDamageToPools(dmg);
            bool lethal = GetTotalCurrentHealth() <= 0;
            if (!lethal)
            {
                bool tookDamage = actual > 0;
                float forceScale = tookDamage ? 0.5f : 1f;
                ApplyKnockback(srcPos, forceScale, tookDamage);
            }
            else
            {
                StartDeathAnimation();
            }
            PushShadeStatsToHud();
            hurtCooldown = currentHurtIFrameDuration;
            CancelFocus();
            PersistIfChanged();
            DispatchCharmDamageEvent(dmg, actual, false, actual <= 0, lethal);
        }

        private int ApplyDamageToPools(int damage)
        {
            int attempted = Mathf.Max(0, damage);
            if (attempted <= 0)
            {
                return 0;
            }

            int lostLifeblood = 0;
            if (shadeLifeblood > 0)
            {
                lostLifeblood = Mathf.Min(shadeLifeblood, attempted);
                shadeLifeblood -= lostLifeblood;
                attempted -= lostLifeblood;
            }

            int lostNormal = 0;
            if (attempted > 0)
            {
                int before = shadeHP;
                shadeHP = Mathf.Max(0, shadeHP - attempted);
                lostNormal = Mathf.Max(0, before - shadeHP);
            }

            if (lostLifeblood > 0)
            {
                if (jonisBlessingEquipped)
                {
                    hivebloodPendingLifebloodRestore = true;
                }
            }
            else if (shadeLifeblood <= 0)
            {
                hivebloodPendingLifebloodRestore = false;
            }

            return lostLifeblood + lostNormal;
        }

        private void DispatchCharmDamageEvent(int attemptedDamage, int actualDamage, bool wasHazard, bool prevented, bool lethal)
        {
            if (charmDamageCallbacks.Count == 0)
            {
                return;
            }

            var context = new ShadeCharmContext(this, charmSnapshot);
            var evt = new ShadeCharmDamageEvent(attemptedDamage, actualDamage, wasHazard, prevented, lethal);
            foreach (var callback in charmDamageCallbacks)
            {
                try { callback(context, evt); }
                catch { }
            }
        }

        private bool TryPreventFocusDamage(int attemptedDamage, bool wasHazard)
        {
            if (!focusDamageShieldEnabled || !isFocusing || focusDamageShieldAbsorbedThisChannel)
            {
                return false;
            }

            focusDamageShieldAbsorbedThisChannel = true;
            DispatchCharmDamageEvent(attemptedDamage, 0, wasHazard, true, false);
            return true;
        }

        private bool TryPreventCarefreeMelody(int attemptedDamage, bool wasHazard)
        {
            if (!carefreeMelodyEquipped)
            {
                return false;
            }

            float chance = Mathf.Clamp01(carefreeMelodyChance);
            if (chance <= 0f)
            {
                return false;
            }

            if (UnityEngine.Random.value > chance)
            {
                return false;
            }

            ResetCarefreeMelodyChance();
            PlayCarefreeMelodyBlockEffect();
            DispatchCharmDamageEvent(attemptedDamage, 0, wasHazard, true, false);
            return true;
        }

        private int ApplyOvercharmPenalty(int baseDamage)
        {
            if (baseDamage <= 0)
            {
                return 0;
            }

            var charms = ShadeRuntime.Charms;
            if (charms != null && charms.IsOvercharmed)
            {
                return Mathf.Max(1, Mathf.CeilToInt(baseDamage * 2f));
            }

            return baseDamage;
        }

        private void StartDeathAnimation()
        {
            if (isDying) return;
            CancelFocus();
            StopFocusChargeSfx();
            try { if (focusAuraRenderer) focusAuraRenderer.enabled = false; } catch { }
            DestroyOtherSlashes(null);
            isChannelingTeleport = false;
            teleportChannelTimer = 0f;
            bool brokeCharm = ShadeRuntime.HandleShadeDeath();
            if (brokeCharm)
            {
                LegacyHelper.RequestShadeLoadoutRecompute();
            }
            StopSpawnAnimation();
            if (deathRoutine != null) StopCoroutine(deathRoutine);
            isDying = true;
            deathRoutine = StartCoroutine(DeathAnimationRoutine());
        }

        private void CancelDeathAnimation()
        {
            if (!isDying)
            {
                if (deathRoutine != null) { StopCoroutine(deathRoutine); deathRoutine = null; }
                return;
            }
            if (deathRoutine != null) StopCoroutine(deathRoutine);
            deathRoutine = null;
            isDying = false;
            isCastingSpell = false;
            currentAnimFrames = null;
        }

        private IEnumerator DeathAnimationRoutine()
        {
            isDying = true;
            isCastingSpell = true;
            if (deathAnimFrames != null && deathAnimFrames.Length > 0)
            {
                currentAnimFrames = deathAnimFrames;
                float perFrame = 0.5f / deathAnimFrames.Length;
                for (int i = 0; i < deathAnimFrames.Length; i++)
                {
                    if (GetTotalCurrentHealth() > 0)
                    {
                        isCastingSpell = false;
                        isDying = false;
                        currentAnimFrames = null;
                        yield break;
                    }
                    if (sr) sr.sprite = deathAnimFrames[i];
                    yield return new WaitForSeconds(perFrame);
                }
            }
            else
            {
                float t = 0f;
                while (t < 0.5f)
                {
                    if (GetTotalCurrentHealth() > 0)
                    {
                        isCastingSpell = false;
                        isDying = false;
                        yield break;
                    }
                    t += Time.deltaTime;
                    yield return null;
                }
            }
            currentAnimFrames = null;
            isCastingSpell = false;
            isDying = false;
            deathRoutine = null;
        }

    }
}
#nullable restore
