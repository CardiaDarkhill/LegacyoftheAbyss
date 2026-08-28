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

            // A trigger of its own, so Hornet's attack triggers can find the Shade to pogo off.
            bool hasTrigger = false;
            foreach (var c in GetComponents<Collider2D>())
            {
                if (c && c.isTrigger) { hasTrigger = true; break; }
            }

            if (!hasTrigger)
            {
                var trigger = gameObject.AddComponent<CapsuleCollider2D>();
                trigger.direction = CapsuleDirection2D.Vertical;
                trigger.size = new Vector2(0.95f, 1.5f);
                trigger.isTrigger = true;
            }

            var hero = HeroController.instance;
            if (!hero) return;

            // Anything but the hero layer, or the Shade trips scene transitions and interactables.
            int heroLayer = hero.gameObject.layer;
            int desiredLayer = LayerMask.NameToLayer("Default");
            if (desiredLayer < 0 || desiredLayer == heroLayer)
            {
                int ignoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                desiredLayer = (ignoreRaycast >= 0 && ignoreRaycast != heroLayer) ? ignoreRaycast : 0;
            }

            gameObject.layer = desiredLayer;
            foreach (var t in GetComponentsInChildren<Transform>(true))
            {
                if (t) t.gameObject.layer = desiredLayer;
            }

            EnsureIgnoreHornetCollisions();
            RefreshCollisionIgnores();
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
            var myCols = GetOwnColliders();
            if (myCols.Length == 0) return;

            var enemies = UnityEngine.Object.FindObjectsByType<HealthManager>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);

            foreach (var hm in enemies)
            {
                if (!hm) continue;
                colliderScratch.Clear();
                hm.GetComponentsInChildren(true, colliderScratch);

                foreach (var ec in colliderScratch)
                {
                    // Triggers are left alone, so hazard and damage events still reach the Shade.
                    if (!ec || ec.isTrigger) continue;
                    foreach (var mc in myCols)
                    {
                        if (mc) TryIgnorePair(mc, ec);
                    }
                }
            }
        }

        private void EnsureIgnoreHornetCollisions()
        {
            var hero = HeroController.instance;
            if (!hero) return;

            var myCols = GetOwnColliders();
            if (myCols.Length == 0) return;

            colliderScratch.Clear();
            hero.transform.root.GetComponentsInChildren(true, colliderScratch);

            if (s_heroAttackLayer < 0) s_heroAttackLayer = LayerMask.NameToLayer("Hero Attack");

            foreach (var mc in myCols)
            {
                if (!mc || mc.isTrigger) continue;
                foreach (var heroCol in colliderScratch)
                {
                    if (!heroCol || heroCol.isTrigger) continue;

                    // Hornet's attacks have to keep reaching the Shade, so it can be pogoed off.
                    // Slashes are not reliably on the Hero Attack layer, hence the second test.
                    if (heroCol.gameObject.layer == s_heroAttackLayer) continue;
                    if (heroCol.GetComponentInParent<NailSlashTerrainThunk>()) continue;

                    long key = PairKey(mc, heroCol);
                    if (!ignoredColliderPairs.Add(key)) continue;

                    Physics2D.IgnoreCollision(mc, heroCol, true);
                }
            }
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
            if (bodyCol) bodyCol.enabled = enable;
            foreach (var c in GetComponentsInChildren<Collider2D>(true))
            {
                if (c && c != bodyCol) c.enabled = enable;
            }
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryProcessDamageHero(collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryProcessDamageHero(other);
        }

        private static bool ShouldIgnoreDamageSource(Component c)
        {
            if (!c) return false;

            string s = (c.name + " " + c.tag).ToLowerInvariant();
            foreach (var token in IgnoreDamageTokens)
                if (s.Contains(token))
                    return true;

            return false;
        }

        private void TryProcessDamageHero(Collider2D col)
        {
            if (!col) return;
            if (bodyCol && !bodyCol.IsTouching(col)) return;
            if (col.transform == transform || col.transform.IsChildOf(transform)) return;

            // Anything of Hornet's is hers to be hit by, not the Shade's.
            if (hornetTransform
                && (col.transform == hornetTransform
                    || col.transform.IsChildOf(hornetTransform)
                    || col.transform.root == hornetTransform.root))
            {
                return;
            }

            var dh = ResolveDamager(col);
            if (dh != null)
            {
                ApplyDamageHero(dh, col);
            }
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

            var dh = source.GetComponent<DamageHero>() ?? source.GetComponentInParent<DamageHero>();
            if (dh != null)
            {
                ApplyDamageHero(dh, null);
            }
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

            var dh = candidate.GetComponent<DamageHero>();
            return dh != null && dh.enabled && dh.damageDealt > 0;
        }

        /// <summary>
        /// The damage an attack deals, for the hits that name no amount at the point they are
        /// intercepted. Falls back to one mask, which is what every one of these has been.
        /// </summary>
        internal static int ResolveAttackDamage(GameObject attack)
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

            return 1;
        }

        /// <summary>
        /// The damage rules themselves, with the "did something touch us?" question already
        /// answered. <paramref name="source"/> is the collider that carried the hit where there was
        /// one, and is used only for the ignore-token check and the log line.
        /// </summary>
        private void ApplyDamageHero(DamageHero dh, Collider2D source)
        {
            // Both of these return unlogged on purpose. The ignore tokens name detection volumes -
            // alert ranges, sight ranges, bounce colliders - and a collider on a layer that cannot
            // touch Hornet was never a hit either; both are "never a damage source" rather than a
            // damage decision worth a record. Logged, they fire per frame per overlap and one boss
            // fight buries the log ring in identical lines.
            if (ShouldIgnoreDamageSource(source) || ShouldIgnoreDamageSource(dh)) { return; }
            if (!CouldReachHornet(source)) { return; }

            if (!dh.enabled || !dh.CanCauseDamage) { LogShadeDamage(dh, source, false); return; }
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
        /// A boss carries a <see cref="DamageHero"/> on its root for body contact and hangs its
        /// attacks off child triggers that carry none, so <c>GetComponentInParent</c> would charge
        /// body-contact damage for touching any child trigger - including telegraphs that damage
        /// nobody. <c>HeroBox.CheckForDamage</c> reads only the object it actually touched.
        /// </para>
        /// <para>
        /// The consequence is deliberate: a collider whose object has no <c>DamageHero</c> deals the
        /// Shade no damage, which is precisely what it would deal Hornet.
        /// </para>
        /// </summary>
        private static DamageHero ResolveDamager(Collider2D collider)
        {
            return collider ? collider.GetComponent<DamageHero>() : null;
        }

        private static int s_heroBoxLayer = -1;

        /// <summary>
        /// Whether a damager could actually have reached Hornet, judged by the physics layer matrix.
        /// <para>
        /// The Shade finds hazards with a maskless <c>Collider2D.Overlap</c>, which returns everything
        /// geometrically overlapping it whether or not those layers interact; Hornet's damage arrives
        /// the opposite way, by something physically touching her <c>HeroBox</c>, already filtered by
        /// the matrix. Without this the Shade is hit by colliders that cannot touch Hornet at all -
        /// a telegraph circle being the clearest case, drawn on a different layer to the hitbox that
        /// follows it.
        /// </para>
        /// <para>
        /// Fails open: an unanswerable question means the hit stands, which is the direction damage
        /// should err in.
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

        private static void LogShadeDamage(DamageHero dh, Collider2D src, bool succeeded)
        {
            string obj = dh ? dh.gameObject.name : "<null>";
            string colName = src ? src.name : "<null>";
            string layer = src ? LayerMask.LayerToName(src.gameObject.layer) : "-";
            LoggingManager.LogShadeDamage($"{obj} via {colName} [{layer}]", succeeded);
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

            var hitbox = new GameObject("ShadeSharpShadowHitbox");
            hitbox.transform.SetParent(transform, false);
            hitbox.transform.localPosition = Vector3.zero;
            hitbox.transform.localRotation = Quaternion.identity;
            hitbox.transform.localScale = Vector3.one;
            hitbox.tag = "Hero Spell";

            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) hitbox.layer = spellLayer;
            else if (atkLayer >= 0) hitbox.layer = atkLayer;

            // Mirrors the Shade's own body where it can, so the dash hits what the Shade covers.
            Collider2D collider = CloneSharpShadowDashCollider(hitbox);
            if (!collider)
            {
                var cap = hitbox.AddComponent<CapsuleCollider2D>();
                cap.direction = CapsuleDirection2D.Vertical;
                cap.size = new Vector2(0.95f, 1.5f);
                cap.offset = Vector2.zero;
                collider = cap;
            }

            var aoe = hitbox.AddComponent<ShadeAoE>();
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

            sharpShadowDashHitbox.transform.localPosition = Vector3.zero;
            sharpShadowDashHitbox.transform.localRotation = Quaternion.identity;

            sharpShadowDashAoE.direction = GetSharpShadowDashAngle();
            sharpShadowDashAoE.magnitudeMultiplier = Mathf.Max(0.01f, charmNailKnockbackMultiplier);
            sharpShadowDashAoE.sourceOverride = gameObject;
        }

        private void DestroySharpShadowDashHitbox()
        {
            if (sharpShadowDashHitbox)
            {
                Destroy(sharpShadowDashHitbox);
            }

            sharpShadowDashHitbox = null;
            sharpShadowDashAoE = null;
        }

        /// <summary>The Shade's own body shape, as a trigger, or null when it is not one we can copy.</summary>
        private Collider2D CloneSharpShadowDashCollider(GameObject owner)
        {
            if (!owner || !bodyCol)
            {
                return null;
            }

            Collider2D clone = null;
            switch (bodyCol)
            {
                case CapsuleCollider2D cap:
                    var capClone = owner.AddComponent<CapsuleCollider2D>();
                    capClone.direction = cap.direction;
                    capClone.size = cap.size;
                    capClone.offset = cap.offset;
                    clone = capClone;
                    break;

                case CircleCollider2D circle:
                    var circleClone = owner.AddComponent<CircleCollider2D>();
                    circleClone.radius = circle.radius;
                    circleClone.offset = circle.offset;
                    clone = circleClone;
                    break;

                case BoxCollider2D box:
                    var boxClone = owner.AddComponent<BoxCollider2D>();
                    boxClone.size = box.size;
                    boxClone.offset = box.offset;
                    clone = boxClone;
                    break;
            }

            if (clone)
            {
                clone.isTrigger = true;
            }

            return clone;
        }

        /// <summary>
        /// Which way the dash hitbox knocks things. Falls back through velocity to plain facing, so
        /// a dash that has not moved yet still points somewhere.
        /// </summary>
        private float GetSharpShadowDashAngle()
        {
            Vector2 dir = lastMoveDelta;

            if (dir.sqrMagnitude < 0.0001f && rb && rb.linearVelocity.sqrMagnitude > 0.0001f)
            {
                dir = rb.linearVelocity;
            }

            // NaN as well as zero: a physics blow-up upstream must not steer the hitbox.
            float angle = dir.sqrMagnitude >= 0.0001f
                ? Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg
                : float.NaN;

            return float.IsNaN(angle) ? (facing >= 0 ? 0f : 180f) : angle;
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

                    if (!dh.enabled || !dh.CanCauseDamage)
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
            if (!bodyCol) return proposed;

            // The Shade's bounds at the proposed spot, approximated from its current extents.
            var ext = bodyCol.bounds.extents;
            Vector2 min = proposed - (Vector2)ext;
            Vector2 max = proposed + (Vector2)ext;

            foreach (var h in Physics2D.OverlapAreaAll(min, max))
            {
                if (!h) continue;
                var tp = h.GetComponentInParent<TransitionPoint>();
                if (tp == null || tp.isADoor) continue; // block only edge-of-map gates

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

            var charms = OwnCharms;
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


        private static bool IsTerrainHazard(GlobalEnums.HazardType hz)
        {
            switch (hz)
            {
                case GlobalEnums.HazardType.SPIKES:
                case GlobalEnums.HazardType.ACID:
                case GlobalEnums.HazardType.LAVA:
                case GlobalEnums.HazardType.PIT:
                case GlobalEnums.HazardType.COAL:
                case GlobalEnums.HazardType.ZAP:
                case GlobalEnums.HazardType.SINK:
                case GlobalEnums.HazardType.STEAM:
                case GlobalEnums.HazardType.COAL_SPIKES:
                case GlobalEnums.HazardType.RESPAWN_PIT:
                    return true;
                default:
                    return false;
            }
        }
    }
}
#nullable restore
