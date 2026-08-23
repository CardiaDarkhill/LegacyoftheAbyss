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
        // Resolved once. GetHazardType runs inside CheckHazardOverlap's per-frame collider loop,
        // where a name-keyed Type.GetField lookup per call is pure waste.
        private static readonly FieldInfo s_damageHeroHazardTypeField =
            typeof(DamageHero).GetField("hazardType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // Reusable buffers for the per-frame overlap test; Overlap fills the array in place.
        // Per-instance rather than static so concurrent shades can't share a buffer.
        private readonly Collider2D[] overlapResults = new Collider2D[16];
        private ContactFilter2D overlapFilter = new ContactFilter2D { useTriggers = true };

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

        // Physics2D.IgnoreCollision state is persistent, so a pair only needs setting once.
        // Without this the two refresh passes re-applied every pair on a 0.5s/1s timer,
        // which in a busy arena is hundreds of interop calls per second that change nothing.
        private readonly HashSet<long> ignoredColliderPairs = new HashSet<long>();
        private Collider2D[] cachedOwnColliders;
        private static int s_heroAttackLayer = -1;
        private readonly List<Collider2D> colliderScratch = new List<Collider2D>();

        private static long PairKey(Collider2D a, Collider2D b)
        {
            int x = a.GetInstanceID();
            int y = b.GetInstanceID();
            if (x > y) (x, y) = (y, x);
            return ((long)x << 32) | (uint)y;
        }

        private bool TryIgnorePair(Collider2D a, Collider2D b)
        {
            long key = PairKey(a, b);
            if (!ignoredColliderPairs.Add(key)) return false;
            Physics2D.IgnoreCollision(a, b, true);
            return true;
        }

        /// <summary>
        /// Colliders on the shade itself. Re-resolved only when the cache is empty or a
        /// cached entry has been destroyed.
        /// </summary>
        private Collider2D[] GetOwnColliders()
        {
            if (cachedOwnColliders != null && cachedOwnColliders.Length > 0)
            {
                bool stale = false;
                foreach (var c in cachedOwnColliders)
                {
                    if (!c) { stale = true; break; }
                }
                if (!stale) return cachedOwnColliders;
            }

            cachedOwnColliders = GetComponentsInChildren<Collider2D>(true);
            return cachedOwnColliders;
        }

        /// <summary>
        /// Clears the memo when the world changes underneath us -- collider instances from
        /// the previous scene are gone and their ignore state went with them.
        /// </summary>
        private void ResetCollisionIgnoreMemo()
        {
            ignoredColliderPairs.Clear();
            cachedOwnColliders = null;
        }

        private void RefreshCollisionIgnores()
        {
            try
            {
                var myCols = GetOwnColliders();
                if (myCols == null || myCols.Length == 0) return;

                // Ignore physical collisions with enemies (objects with HealthManager) but keep triggers for damage/hazards
                HealthManager[] enemies;
                try
                {
                    enemies = UnityEngine.Object.FindObjectsByType<HealthManager>(
                        FindObjectsInactive.Exclude,
                        FindObjectsSortMode.None);
                }
                catch { return; }
                if (enemies == null) return;

                foreach (var hm in enemies)
                {
                    if (!hm) continue;
                    colliderScratch.Clear();
                    hm.GetComponentsInChildren(true, colliderScratch);
                    foreach (var ec in colliderScratch)
                    {
                        if (!ec || ec.isTrigger) continue; // don't ignore triggers to still receive hazard/damage events
                        foreach (var mc in myCols)
                        {
                            if (mc) TryIgnorePair(mc, ec);
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

                var myCols = GetOwnColliders();
                if (myCols == null || myCols.Length == 0) return;

                colliderScratch.Clear();
                hc.transform.root.GetComponentsInChildren(true, colliderScratch);

                if (s_heroAttackLayer < 0) s_heroAttackLayer = LayerMask.NameToLayer("Hero Attack");

                foreach (var mc in myCols)
                {
                    if (!mc || mc.isTrigger) continue;
                    foreach (var hcCol in colliderScratch)
                    {
                        if (!hcCol || hcCol.isTrigger) continue;
                        if (hcCol.gameObject.layer == s_heroAttackLayer) continue; // allow hero attack contact

                        long key = PairKey(mc, hcCol);
                        if (ignoredColliderPairs.Contains(key)) continue;

                        // Allow slashes (which may not be on Hero Attack layer) by checking their components
                        if (hcCol.GetComponentInParent<NailSlashTerrainThunk>()) continue;

                        ignoredColliderPairs.Add(key);
                        Physics2D.IgnoreCollision(mc, hcCol, true);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// The Shade's body collider, for the systems that have to register it explicitly the way
        /// the game registers Hornet's hero box - see <see cref="NotifyParticleDamage"/>.
        /// </summary>
        internal Collider2D BodyCollider => bodyCol;

        /// <summary>Shade is down and cannot be given anything to do or take.</summary>
        internal bool IsInactive => isInactive;

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
                var dh = ResolveDamager(col);
                if (dh != null)
                {
                    ApplyDamageHero(dh, col);
                }
            }
            catch { }
        }

        /// <summary>
        /// Damage the game delivers through a particle system rather than a collider. Those go via
        /// <c>ParticleDamageHero</c>, which registers Hornet's hero box as the particle trigger's
        /// only collider, so no overlap and no <see cref="DamageHero"/> component ever reaches the
        /// Shade through the ordinary paths above - it simply stands in the acid unharmed. See
        /// <c>ParticleDamageHero_ShadeRelay</c> for the collider registration that calls this.
        /// </summary>
        internal void NotifyParticleDamage(GameObject source)
        {
            if (!source)
            {
                return;
            }

            try
            {
                var dh = source.GetComponent<DamageHero>() ?? source.GetComponentInParent<DamageHero>();
                if (dh != null)
                {
                    ApplyDamageHero(dh, null);
                }
            }
            catch { }
        }

        /// <summary>
        /// A hit the Shade cannot receive any other way.
        /// <para>
        /// Most damage reaches the Shade through its own overlap scan, by finding a
        /// <see cref="DamageHero"/> on the collider it is touching. Some attacks carry none: Lace's
        /// cross slash damages Hornet by having its FSM call <c>HeroController.TakeQuickDamage</c>
        /// directly, and its hitbox is a bare trigger with no damage component on it at all. Nothing
        /// the Shade can see makes that a hit, so when such an attack is taken off Hornet because she
        /// is not standing in it, this is what puts it on the Shade instead.
        /// </para>
        /// <para>
        /// Runs the ordinary pools, charms, i-frames and death handling, because it is an ordinary
        /// hit - only the "what touched us" question was answered somewhere else.
        /// </para>
        /// </summary>
        internal void TakeAttackHit(int amount, string sourceName)
        {
            if (amount <= 0 || isInactive || hurtCooldown > 0f)
            {
                return;
            }

            try
            {
                int attempted = ApplyOvercharmPenalty(amount);
                LoggingManager.LogShadeDamage(sourceName ?? "attack", canTakeDamage);

                if (!canTakeDamage)
                {
                    hurtCooldown = currentHurtIFrameDuration;
                    DispatchCharmDamageEvent(attempted, 0, false, true, false);
                    return;
                }

                if (TryPreventFocusDamage(attempted, false))
                {
                    hurtCooldown = currentHurtIFrameDuration;
                    return;
                }

                int actual = ApplyDamageToPools(attempted);
                bool lethal = GetTotalCurrentHealth() <= 0;
                if (lethal)
                {
                    StartDeathAnimation();
                }

                PushShadeStatsToHud();
                hurtCooldown = currentHurtIFrameDuration;
                CancelFocus();
                PersistIfChanged();
                DispatchCharmDamageEvent(attempted, actual, false, actual <= 0, lethal);
            }
            catch { }
        }

        /// <summary>
        /// Whether the Shade's own overlap scan would find this object as a damage source. If it
        /// would, a hit taken off Hornet must not also be applied by hand - the Shade would take it
        /// twice.
        /// </summary>
        internal static bool CarriesItsOwnDamage(GameObject candidate)
        {
            if (!candidate)
            {
                return false;
            }

            try
            {
                var dh = candidate.GetComponent<DamageHero>();
                return dh != null && dh.enabled && dh.damageDealt > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The damage an attack deals, for the hits that name no amount at the point they are
        /// intercepted. Falls back to one mask, which is what every one of these has been.
        /// </summary>
        internal static int ResolveAttackDamage(GameObject attack)
        {
            try
            {
                if (attack)
                {
                    foreach (var dh in attack.GetComponentsInChildren<DamageHero>(false))
                    {
                        if (dh != null && dh.enabled && dh.damageDealt > 0)
                        {
                            return dh.damageDealt;
                        }
                    }
                }
            }
            catch
            {
            }

            return 1;
        }

        /// <summary>
        /// The damage rules themselves, with the "did something touch us?" question already
        /// answered. <paramref name="source"/> is the collider that carried the hit where there was
        /// one, and is used only for the ignore-token check and the log line.
        /// </summary>
        private void ApplyDamageHero(DamageHero dh, Collider2D source)
        {
            // Not logged: the ignore tokens name detection volumes - alert ranges, sight ranges,
            // bounce colliders - so this is "that was never a damage source", not a damage decision
            // anyone wants a record of. It was reported per frame per overlap, and one boss fight
            // put 800 identical "avoided damage from Lace Boss1 via Battle Range" lines into the
            // log ring, crowding out everything a report was filed to capture.
            if (ShouldIgnoreDamageSource(source) || ShouldIgnoreDamageSource(dh)) { return; }

            // Likewise unlogged: a collider on a layer that cannot touch Hornet was never a hit,
            // it was only something the Shade's layer-blind overlap scan happened to find.
            if (!CouldReachHornet(source)) { return; }
            bool canDamage = false;
            try { canDamage = dh.enabled && dh.CanCauseDamage; } catch { }
            if (!canDamage) { LogShadeDamage(dh, source, false); return; }
            int dmg = GetDamageAmount(dh);
            var hz = GetHazardType(dh);
            if (hz == GlobalEnums.HazardType.STEAM && dmg <= 0)
            {
                LogShadeDamage(dh, source, false);
                return;
            }
            if (IsTerrainHazard(hz))
            {
                if (dmg <= 0)
                {
                    LogShadeDamage(dh, source, false);
                    return;
                }

                LogShadeDamage(dh, source, canTakeDamage);
                OnShadeHitHazard();
                return;
            }
            if (dmg > 0)
            {
                bool preventedByVoidHeart = IsVoidHeartEvading();
                LogShadeDamage(dh, source, canTakeDamage && !preventedByVoidHeart);
                if (preventedByVoidHeart)
                {
                    int attempted = ApplyOvercharmPenalty(dmg);
                    HandleVoidHeartEvadePreventedHit(attempted);
                    return;
                }

                OnShadeHitEnemy(dh);
            }
            else { LogShadeDamage(dh, source, false); }
        }

        /// <summary>
        /// The damager a collider actually represents, resolved exactly as <see cref="HeroBox"/> does
        /// it: <c>GetComponent</c> on the collider's own object, never a walk up the hierarchy.
        /// <para>
        /// Walking up is what made the Shade take damage from an attack's telegraph. A boss carries a
        /// <see cref="DamageHero"/> on its root for body contact, and its attacks are child triggers
        /// that mostly carry none of their own - so <c>GetComponentInParent</c> attributed the boss's
        /// contact damage to every child trigger the Shade touched, and the Shade was hurt by marker
        /// volumes that damage nobody. Hornet is never hit that way, because
        /// <c>HeroBox.CheckForDamage</c> reads only the object it actually touched.
        /// </para>
        /// <para>
        /// The consequence is deliberate: a collider whose object has no <c>DamageHero</c> deals the
        /// Shade no damage, which is precisely what it would deal Hornet.
        /// </para>
        /// </summary>
        private static DamageHero ResolveDamager(Collider2D collider)
        {
            if (!collider)
            {
                return null;
            }

            try { return collider.GetComponent<DamageHero>(); }
            catch { return null; }
        }

        private static int s_heroBoxLayer = -1;

        /// <summary>
        /// Whether a damager could actually have reached Hornet, judged by the physics layer matrix.
        /// <para>
        /// The Shade finds its hazards with <c>Collider2D.Overlap</c> and no layer mask, which returns
        /// everything geometrically overlapping it whether or not those layers interact. Hornet's own
        /// damage arrives the opposite way: something has to physically touch her <c>HeroBox</c>, so
        /// the layer matrix has already filtered it. That asymmetry let the Shade be hit by colliders
        /// that cannot touch Hornet at all - an ability's telegraph being the case that showed it,
        /// where the marked circle exists to be looked at and the hitbox that follows is a different
        /// object on a different layer.
        /// </para>
        /// <para>
        /// Asking the matrix restores the symmetry without hardcoding which layers those are: the
        /// Shade is hit by exactly the things that could have hit Hornet where she standing there.
        /// </para>
        /// </summary>
        private static bool CouldReachHornet(Collider2D damager)
        {
            if (!damager)
            {
                return true;
            }

            try
            {
                if (s_heroBoxLayer < 0)
                {
                    var hero = HeroController.instance;
                    var box = hero ? hero.heroBox : null;
                    if (!box)
                    {
                        return true;
                    }

                    s_heroBoxLayer = box.gameObject.layer;
                }

                return !Physics2D.GetIgnoreLayerCollision(damager.gameObject.layer, s_heroBoxLayer);
            }
            catch
            {
                return true;
            }
        }

        private void LogShadeDamage(DamageHero dh, Collider2D src, bool succeeded)
        {
            try
            {
                string obj = dh ? dh.gameObject?.name ?? dh.name : "<null>";
                string colName = src ? src.name : "<null>";
                string layer = src ? LayerMask.LayerToName(src.gameObject.layer) : "-";
                string source = $"{obj} via {colName} [{layer}]";
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
                    sharpShadowDashAoE = sharpShadowDashHitbox.GetComponent<ShadeAoE>();
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
                sharpShadowDashAoE = sharpShadowDashHitbox.GetComponent<ShadeAoE>();
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
            // The shadow wisps simulate in world space, so without this the trail would stretch
            // from the old position to the new one across the whole room.
            ClearShadowParticles();
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
            if (activating)
            {
                // New scene: the previous scene's colliders are gone along with their
                // ignore state, so the memo must not carry over.
                ResetCollisionIgnoreMemo();
            }
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

                    var dh = ResolveDamager(c);
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
            int count = bodyCol.Overlap(overlapFilter, overlapResults);
            for (int i = 0; i < count; i++)
            {
                var c = overlapResults[i];
                if (!c) continue;
                if (c.transform == transform || c.transform.IsChildOf(transform)) continue;
                if (hornetTransform && c.transform.root == hornetTransform.root) continue;
                var dh = ResolveDamager(c);
                if (dh != null)
                {
                    if (ShouldIgnoreDamageSource(c) || ShouldIgnoreDamageSource(dh)) { continue; }
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
            if (s_damageHeroHazardTypeField == null) return GlobalEnums.HazardType.NON_HAZARD;
            try
            {
                return (GlobalEnums.HazardType)s_damageHeroHazardTypeField.GetValue(dh);
            }
            catch
            {
                return GlobalEnums.HazardType.NON_HAZARD;
            }
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
