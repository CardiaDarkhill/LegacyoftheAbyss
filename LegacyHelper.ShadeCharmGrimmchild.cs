#nullable disable
using System.IO;
using System.Linq;
using LegacyoftheAbyss.Shade;
using UnityEngine;

public partial class LegacyHelper
{
    /// <summary>
    /// Grimmchild: a small flier that keeps station over its bearer's shoulder and throws fireballs
    /// at whatever comes near.
    /// <para>
    /// Deliberately not a second <see cref="ShadeController"/>. It behaves like the Shade under AI -
    /// it hovers at an offset, breaks station to line a target up, shoots, and drifts back - but a
    /// real Shade is a companion in <c>ShadeCompanionRegistry</c> with its own health, soul, charms,
    /// persistence and HUD row, none of which a charm should be creating. What is shared is the
    /// shape of the behaviour and the art, not the object.
    /// </para>
    /// <para>
    /// Its anchor is the bearer rather than Hornet, which is the whole point of the charm: it
    /// follows whichever companion is wearing it.
    /// </para>
    /// </summary>
    public class ShadeCharmGrimmchild : MonoBehaviour
    {
        /// <summary>The skin the art is taken from. Named by the developer; see the charm report.</summary>
        private const string SkinId = "Grimmchild Phase 3";

        /// <summary>How far above and behind the bearer it sits when nothing is worth shooting.</summary>
        private static readonly Vector2 ShoulderOffset = new Vector2(-1.15f, 1.35f);

        /// <summary>How far from its bearer it looks for something to shoot.</summary>
        private const float SeekRange = 13f;

        /// <summary>How far it will stray from the bearer to line a shot up.</summary>
        private const float LeashRange = 6.5f;

        private const float FollowSpeed = 11f;
        private const float FireCooldownSeconds = 1.6f;
        private const float CastAnticSeconds = 0.28f;
        private const float RetargetSeconds = 0.3f;

        /// <summary>Flat, and the charm's own figure: not a share of the bearer's nail or spells.</summary>
        private const int FireballDamage = 11;

        private const float FireballSpeed = 16f;
        private const float FireballLifeSeconds = 2.5f;
        private const float FireballRange = 20f;

        /// <summary>
        /// The radius the body is swept with. Terrain does not share a collision layer with what
        /// this flies on, so the sweep is explicit rather than left to a Rigidbody - the same
        /// reason the Knight resolves its own movement.
        /// </summary>
        private const float BodyRadius = 0.35f;

        private const float IdleFrameTime = 0.09f;
        private const float FloatFrameTime = 0.08f;
        private const float CastFrameTime = 0.07f;
        private const float BobAmplitude = 0.16f;
        private const float BobSpeed = 2.4f;

        internal Transform anchor;
        internal ShadeController owner;

        private SpriteRenderer sr;
        private Sprite[] currentFrames;
        private int frameIndex;
        private float frameTimer;
        private float frameTime = IdleFrameTime;

        private Transform target;
        private float retargetTimer;
        private float fireCooldown;
        private float castTimer;
        private Vector2 pendingShot;
        private float bobPhase;
        private int facing = 1;

        private static Sprite[] s_idleFrames;
        private static Sprite[] s_floatFrames;
        private static Sprite[] s_castFrames;
        private static Sprite[] s_fireballFrames;
        private static bool s_artResolved;

        /// <summary>
        /// Builds one and parents it to nothing: it follows its anchor by position rather than by
        /// hierarchy, so the bearer flipping does not mirror it.
        /// </summary>
        internal static GameObject Create(ShadeController controller)
        {
            if (controller == null)
            {
                return null;
            }

            EnsureArt();

            var go = new GameObject("ShadeCharmGrimmchild");
            go.transform.position = controller.transform.position + (Vector3)ShoulderOffset;

            var child = go.AddComponent<ShadeCharmGrimmchild>();
            child.owner = controller;
            child.anchor = controller.transform;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.color = Color.white;
            var ownerRenderer = controller.GetComponent<SpriteRenderer>();
            if (ownerRenderer != null)
            {
                renderer.sortingLayerID = ownerRenderer.sortingLayerID;
                renderer.sortingOrder = ownerRenderer.sortingOrder + 2;
            }

            child.sr = renderer;
            child.bobPhase = Random.Range(0f, Mathf.PI * 2f);
            child.SetFrames(s_idleFrames, IdleFrameTime);
            return go;
        }

        private void Update()
        {
            if (anchor == null || owner == null)
            {
                Destroy(gameObject);
                return;
            }

            float dt = Time.deltaTime;
            bobPhase += dt * BobSpeed;

            if (fireCooldown > 0f)
            {
                fireCooldown = Mathf.Max(0f, fireCooldown - dt);
            }

            retargetTimer -= dt;
            if (retargetTimer <= 0f)
            {
                retargetTimer = RetargetSeconds;
                target = ShadeCharmMinion.FindNearestEnemy(anchor.position, SeekRange);
            }

            // The antic runs to its end even if the target dies inside it, so a shot that has been
            // wound up is always thrown - the same way the Shade's own casts commit.
            if (castTimer > 0f)
            {
                castTimer -= dt;
                if (castTimer <= 0f)
                {
                    FireAt(pendingShot);
                }

                HoldStation(dt, engaging: true);
                Animate(dt);
                return;
            }

            bool engaging = target != null;
            if (engaging && fireCooldown <= 0f)
            {
                Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;
                if (toTarget.sqrMagnitude > 0.0001f)
                {
                    pendingShot = toTarget.normalized;
                    castTimer = CastAnticSeconds;
                    SetFrames(s_castFrames, CastFrameTime);
                }
            }

            HoldStation(dt, engaging);
            Animate(dt);
        }

        /// <summary>
        /// Moves toward where it wants to be: its bearer's shoulder normally, or a firing position
        /// beside the target while it has one. Kept inside <see cref="LeashRange"/> of the bearer,
        /// which is what stops it wandering off after something across the room.
        /// </summary>
        private void HoldStation(float dt, bool engaging)
        {
            Vector2 anchorPos = anchor.position;
            Vector2 desired;

            if (engaging && target != null)
            {
                // Stand off the target rather than sitting on it: the fireball is the weapon.
                Vector2 fromTarget = (Vector2)transform.position - (Vector2)target.position;
                if (fromTarget.sqrMagnitude < 0.0001f)
                {
                    fromTarget = Vector2.up;
                }

                desired = (Vector2)target.position + fromTarget.normalized * 3.2f;
            }
            else
            {
                float side = owner != null && owner.Facing < 0 ? 1f : -1f;
                desired = anchorPos + new Vector2(ShoulderOffset.x * -side, ShoulderOffset.y);
                desired.y += Mathf.Sin(bobPhase) * BobAmplitude;
            }

            Vector2 fromAnchor = desired - anchorPos;
            if (fromAnchor.magnitude > LeashRange)
            {
                desired = anchorPos + fromAnchor.normalized * LeashRange;
            }

            Vector2 step = Vector2.MoveTowards(transform.position, desired, FollowSpeed * dt);
            transform.position = ResolveTerrain(transform.position, step);

            Vector2 lookAt = engaging && target != null ? (Vector2)target.position : anchorPos;
            float dx = lookAt.x - transform.position.x;
            if (Mathf.Abs(dx) > 0.05f)
            {
                facing = dx >= 0f ? 1 : -1;
            }
        }

        /// <summary>
        /// Stops the body at terrain instead of drifting through it. Swept per axis so it slides
        /// along a wall rather than sticking to it, which is what lets it follow its bearer round a
        /// corner instead of pressing into the corner itself.
        /// </summary>
        private static Vector2 ResolveTerrain(Vector2 current, Vector2 target)
        {
            int mask = ShadeController.TerrainMask();
            Vector2 resolved = current;

            float dx = target.x - current.x;
            if (Mathf.Abs(dx) > 0.0001f)
            {
                var hit = Physics2D.CircleCast(resolved, BodyRadius, new Vector2(Mathf.Sign(dx), 0f), Mathf.Abs(dx), mask);
                resolved.x = hit.collider != null
                    ? resolved.x + Mathf.Sign(dx) * Mathf.Max(0f, hit.distance - 0.01f)
                    : target.x;
            }

            float dy = target.y - current.y;
            if (Mathf.Abs(dy) > 0.0001f)
            {
                var hit = Physics2D.CircleCast(resolved, BodyRadius, new Vector2(0f, Mathf.Sign(dy)), Mathf.Abs(dy), mask);
                resolved.y = hit.collider != null
                    ? resolved.y + Mathf.Sign(dy) * Mathf.Max(0f, hit.distance - 0.01f)
                    : target.y;
            }

            return resolved;
        }

        /// <summary>
        /// Throws one fireball along <paramref name="direction"/>.
        /// <para>
        /// Any angle, unlike the bearer's own Vengeful Spirit, which is always thrown along the
        /// ground. The sprite is rotated to match rather than merely flipped, because a bolt
        /// travelling up and to the left drawn flat reads as the wrong sprite rather than as aim.
        /// </para>
        /// </summary>
        private void FireAt(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            direction = direction.normalized;
            fireCooldown = FireCooldownSeconds;

            var proj = new GameObject("ShadeGrimmchildFireball");
            proj.transform.position = transform.position;
            proj.tag = "Hero Spell";

            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) proj.layer = spellLayer;
            else if (atkLayer >= 0) proj.layer = atkLayer;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            var psr = proj.AddComponent<SpriteRenderer>();
            if (s_fireballFrames != null && s_fireballFrames.Length > 0)
            {
                psr.sprite = s_fireballFrames[0];
            }

            if (sr != null)
            {
                psr.sortingLayerID = sr.sortingLayerID;
                psr.sortingOrder = sr.sortingOrder + 1;
            }

            var col = proj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.28f;

            var body = proj.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.linearVelocity = direction * FireballSpeed;

            IgnoreOwners(col);

            var shot = proj.AddComponent<ShadeProjectile>();
            shot.animFrames = s_fireballFrames;
            shot.damage = FireballDamage;
            shot.hornetRoot = owner != null ? owner.HornetRoot : null;
            shot.maxRange = FireballRange;
            shot.lifeSeconds = FireballLifeSeconds;

            // Bursts on terrain rather than sailing through it, as Vengeful Spirit does.
            shot.destroyOnTerrain = true;

            SetFrames(s_castFrames, CastFrameTime);
        }

        /// <summary>Neither the bearer nor Hornet is a target; the shot passes through both.</summary>
        private void IgnoreOwners(Collider2D projectileCollider)
        {
            if (projectileCollider == null)
            {
                return;
            }

            if (owner != null)
            {
                foreach (var ownerCol in owner.GetComponentsInChildren<Collider2D>(true))
                {
                    if (ownerCol) Physics2D.IgnoreCollision(projectileCollider, ownerCol, true);
                }

                var hornet = owner.HornetRoot;
                if (hornet != null)
                {
                    foreach (var heroCol in hornet.GetComponentsInChildren<Collider2D>(true))
                    {
                        if (heroCol) Physics2D.IgnoreCollision(projectileCollider, heroCol, true);
                    }
                }
            }
        }

        private void Animate(float dt)
        {
            if (sr == null)
            {
                return;
            }

            if (castTimer <= 0f)
            {
                bool moving = target != null;
                SetFrames(moving ? s_floatFrames : s_idleFrames, moving ? FloatFrameTime : IdleFrameTime);
            }

            if (currentFrames != null && currentFrames.Length > 0)
            {
                frameTimer += dt;
                if (frameTimer >= frameTime)
                {
                    frameTimer -= frameTime;
                    frameIndex = (frameIndex + 1) % currentFrames.Length;
                    sr.sprite = currentFrames[frameIndex];
                }
            }

            sr.flipX = facing < 0;
        }

        private void SetFrames(Sprite[] frames, float perFrame)
        {
            if (frames == null || frames.Length == 0 || ReferenceEquals(frames, currentFrames))
            {
                return;
            }

            currentFrames = frames;
            frameTime = Mathf.Max(0.01f, perFrame);
            frameIndex = 0;
            frameTimer = 0f;
            if (sr != null)
            {
                sr.sprite = frames[0];
            }
        }

        /// <summary>
        /// Loads the Grimmchild III sheets once for every Grimmchild. Cached including the failure,
        /// so a missing skin folder is probed once rather than on every spawn.
        /// </summary>
        private static void EnsureArt()
        {
            if (s_artResolved)
            {
                return;
            }

            s_artResolved = true;

            try
            {
                ShadeSkinManager.EnsureLoaded();
                var skin = ShadeSkinManager.Skins?.FirstOrDefault(s => s != null && s.Matches(SkinId));
                if (skin == null)
                {
                    LogWarning($"Grimmchild: the '{SkinId}' skin is not installed, so it will be invisible.");
                    return;
                }

                s_idleFrames = LoadStrip(ShadeSkinManager.ResolveSpritePath(skin, "Shade_Idle_Sheet.png"), 9);
                s_floatFrames = LoadStrip(ShadeSkinManager.ResolveSpritePath(skin, "Shade_Float_Sheet.png"), 6);
                s_castFrames = LoadStrip(ShadeSkinManager.ResolveSpritePath(skin, "Shade_Fireball_Cast_Sheet.png"), 4);
                s_fireballFrames = LoadStrip(ShadeSkinManager.ResolveSpritePath(skin, "Vengeful_Spirit_Sheet.png"), 2);
            }
            catch
            {
                s_idleFrames = null;
                s_floatFrames = null;
                s_castFrames = null;
                s_fireballFrames = null;
            }
        }

        /// <summary>
        /// A horizontal sprite strip cut into <paramref name="frames"/> equal frames. Point filtered
        /// and deliberately simpler than the Shade's own loader: these sheets are never re-cut for a
        /// skin change, because Grimmchild's art does not follow the bearer's skin.
        /// </summary>
        private static Sprite[] LoadStrip(string path, int frames)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    return null;
                }

                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                if (!texture.LoadImage(File.ReadAllBytes(path)))
                {
                    return null;
                }

                int count = Mathf.Max(1, frames);
                int frameWidth = Mathf.Max(1, texture.width / count);
                var sprites = new Sprite[count];
                for (int i = 0; i < count; i++)
                {
                    sprites[i] = Sprite.Create(
                        texture,
                        new Rect(i * frameWidth, 0f, frameWidth, texture.height),
                        new Vector2(0.5f, 0.5f),
                        100f);
                }

                return sprites;
            }
            catch
            {
                return null;
            }
        }
    }
}
#nullable restore
