using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        private const int slashSoulGain = 11;

        private void HandleNailAttack()
        {
            nailTimer -= Time.deltaTime;
            if (nailTimer > 0f) return;
            if (isCastingSpell) return;
            bool pressed = Input.GetMouseButtonDown(0) || Input.GetKeyDown(NailKey);
            if (!pressed) return;
            nailTimer = nailCooldown;
            Vector2 dir = GetSlashDirection();
            StartCoroutine(SlashRoutine(dir));
        }

        private Vector2 GetSlashDirection()
        {
            float v = (Input.GetKey(KeyCode.S) ? -1f : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);
            if (v > 0.35f) return Vector2.up;
            if (v < -0.35f) return Vector2.down;
            return new Vector2(facing, 0f);
        }

        private IEnumerator SlashRoutine(Vector2 dir)
        {
            SpawnSlashVisual(dir);
            SpawnSlashDamage(dir);
            yield break;
        }

        private void SpawnSlashVisual(Vector2 dir)
        {
            try
            {
                var allSlashes = Resources.FindObjectsOfTypeAll<NailSlash>();
                if (allSlashes == null || allSlashes.Length == 0) return;
                allSlashes = Array.FindAll(allSlashes,
                    s => s && s.transform.parent &&
                         s.transform.parent.name.IndexOf("Wanderer",
                             StringComparison.OrdinalIgnoreCase) >= 0);
                if (allSlashes == null || allSlashes.Length == 0) return;

                bool MatchUp(NailSlash ns) => ns && (((ns.name ?? "").ToLowerInvariant().Contains("up")) || ((ns.animName ?? "").ToLowerInvariant().Contains("up")));
                bool MatchDown(NailSlash ns) => ns && (((ns.name ?? "").ToLowerInvariant().Contains("down")) || ((ns.animName ?? "").ToLowerInvariant().Contains("down")));
                bool MatchLeft(NailSlash ns)
                {
                    if (!ns) return false;
                    string n = (ns.name ?? "").ToLowerInvariant();
                    string a = (ns.animName ?? "").ToLowerInvariant();
                    return n.Contains("left") || a.Contains("left");
                }
                bool MatchRight(NailSlash ns)
                {
                    if (!ns) return false;
                    string n = (ns.name ?? "").ToLowerInvariant();
                    string a = (ns.animName ?? "").ToLowerInvariant();
                    if (n.Contains("alt") || n.Contains("right") || a.Contains("alt")) return true;
                    return a.Contains("right");
                }
                bool MatchNormal(NailSlash ns) => ns && !MatchUp(ns) && !MatchDown(ns);

                NailSlash pick = null;
                if (dir.y > 0.1f)
                    pick = Array.Find(allSlashes, s => MatchUp(s));
                else if (dir.y < -0.1f)
                    pick = Array.Find(allSlashes, s => MatchDown(s));
                else if ((dir.x != 0f ? dir.x : facing) >= 0f)
                    pick = Array.Find(allSlashes, s => MatchNormal(s) && MatchRight(s)) ?? Array.Find(allSlashes, s => MatchRight(s));
                else
                    pick = Array.Find(allSlashes, s => MatchNormal(s) && MatchLeft(s)) ?? Array.Find(allSlashes, s => MatchLeft(s));

                if (pick == null)
                    pick = Array.Find(allSlashes, s => MatchNormal(s));
                if (pick == null && allSlashes.Length > 0)
                    pick = allSlashes[0];

                var slash = Instantiate(pick.gameObject);
                slash.transform.position = transform.position;
                var horiz = dir.x != 0f ? Mathf.Sign(dir.x) : facing;
                var ls = slash.transform.localScale;
                ls.x = Mathf.Abs(ls.x) * horiz;
                ls.y = Mathf.Abs(ls.y) * (dir.y < -0.1f ? -1f : 1f);
                ls *= 1f / SpriteScale;
                slash.transform.localScale = ls;

                var ns = slash.GetComponent<NailSlash>();
                string animName = ns ? ns.animName : string.Empty;

                foreach (var col in slash.GetComponentsInChildren<Collider2D>(true)) Destroy(col);
                foreach (var mb in slash.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    string tn = mb.GetType().Name;
                    if (!tn.StartsWith("tk2d")) Destroy(mb);
                }
                foreach (var r in slash.GetComponentsInChildren<Renderer>(true)) r.enabled = true;

                var anim = slash.GetComponent("tk2dSpriteAnimator");
                if (anim != null && !string.IsNullOrEmpty(animName))
                {
                    var play = anim.GetType().GetMethod("Play", new Type[] { typeof(string) });
                    play?.Invoke(anim, new object[] { animName });
                }
                var audio = slash.GetComponent<AudioSource>();
                audio?.Play();

                slash.transform.SetParent(transform);
                Destroy(slash, 0.4f);
            }
            catch { }
        }

        private void SpawnSlashDamage(Vector2 dir)
        {
            var go = new GameObject("ShadeSlashDamager");
            go.transform.position = transform.position;
            go.tag = "Hero Spell";
            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) go.layer = spellLayer; else if (atkLayer >= 0) go.layer = atkLayer;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            float w = 1.4f, h = 0.6f;
            Vector2 offset = Vector2.zero;
            if (dir.y > 0.1f) { w = 0.6f; h = 1.4f; offset = Vector2.up * 0.8f; }
            else if (dir.y < -0.1f) { w = 0.6f; h = 1.4f; offset = Vector2.down * 0.8f; }
            else { offset = new Vector2(facing * 0.8f, 0f); }
            col.size = new Vector2(w, h);
            col.offset = offset;
            IgnoreHornetForCollider(col);
            try
            {
                var shadeCols = GetComponentsInChildren<Collider2D>(true);
                foreach (var sc in shadeCols)
                    if (sc) Physics2D.IgnoreCollision(col, sc, true);
            }
            catch { }

            var dmg = go.AddComponent<ShadeSlashDamage>();
            dmg.damage = Mathf.Max(1, GetHornetNailDamage());
            dmg.soulGain = slashSoulGain;
            dmg.owner = this;
            Destroy(go, 0.2f);
        }

        private void SpawnProjectile(Vector2 dir)
        {
            var proj = new GameObject("ShadeProjectile");
            proj.transform.position = transform.position + (Vector3)new Vector2(muzzleOffset.x * facing, muzzleOffset.y);
            proj.tag = "Hero Spell";
            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) proj.layer = spellLayer; else if (atkLayer >= 0) proj.layer = atkLayer;

            var psr = proj.AddComponent<SpriteRenderer>();
            Sprite[] frames = IsProjectileUpgraded() && shadeSoulAnimFrames.Length > 0 ? shadeSoulAnimFrames : vengefulAnimFrames;
            if (frames.Length > 0)
                psr.sprite = frames[0];
            else
                psr.sprite = MakeDotSprite();

            bool flip = dir.x < 0f;
            psr.flipX = flip;

            float scale = SpriteScale * (IsProjectileUpgraded() ? 1.5f : 1f);
            proj.transform.localScale = Vector3.one * scale;

            Collider2D[] projCols;
            if (frames.Length > 0)
            {
                var size = frames[0].bounds.size;
                float radius = size.y / 2f;
                float facingSign = flip ? -1f : 1f;

                var head = proj.AddComponent<CircleCollider2D>();
                head.isTrigger = true;
                head.radius = radius;
                head.offset = new Vector2(facingSign * (size.x / 2f - radius), 0f);

                var body = proj.AddComponent<BoxCollider2D>();
                body.isTrigger = true;
                body.size = new Vector2(Mathf.Max(0f, size.x - radius), size.y);
                body.offset = new Vector2(-facingSign * radius / 2f, 0f);

                projCols = new Collider2D[] { head, body };
            }
            else
            {
                var col = proj.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                projCols = new Collider2D[] { col };
            }

            var others = UnityEngine.Object.FindObjectsByType<ShadeProjectile>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var o in others)
                foreach (var oc in o.GetComponents<Collider2D>())
                    foreach (var pc in projCols)
                        if (oc && pc) Physics2D.IgnoreCollision(pc, oc, true);

            var rb = proj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.linearVelocity = dir.normalized * projectileSpeed;

            if (hornetTransform != null)
            {
                var hornetCols = hornetTransform.GetComponentsInChildren<Collider2D>(true);
                foreach (var hc in hornetCols)
                    foreach (var pc in projCols)
                        if (hc && pc) Physics2D.IgnoreCollision(pc, hc, true);
            }

            var sp = proj.AddComponent<ShadeProjectile>();
            sp.animFrames = frames;
            int dmg = ComputeSpellDamageMultiplier(2.5f, IsProjectileUpgraded());
            sp.damage = Mathf.Max(1, dmg);
            sp.hornetRoot = hornetTransform;
            sp.destroyOnTerrain = !IsProjectileUpgraded();
            sp.maxRange = IsProjectileUpgraded() ? 22f : 0f;

            TryPlayFireballSfx();
        }

        private Sprite MakeDotSprite()
        {
            var tex = new Texture2D(6, 6);
            for (int x = 0; x < tex.width; x++)
                for (int y = 0; y < tex.height; y++)
                    tex.SetPixel(x, y, Color.black);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 16f);
        }

        private void AddSoul(int amount)
        {
            int prev = shadeSoul;
            shadeSoul = Mathf.Min(shadeSoulMax, shadeSoul + amount);
            if (shadeSoul != prev)
                PushSoulToHud();
        }

        public class ShadeSlashDamage : MonoBehaviour
        {
            public int damage;
            public int soulGain;
            public ShadeController owner;
            private HashSet<Collider2D> hitSet = new HashSet<Collider2D>();

            private void OnTriggerEnter2D(Collider2D other)
            {
                if (other == null || owner == null) return;
                if (owner.hornetTransform != null && other.transform.IsChildOf(owner.hornetTransform)) return;
                if (hitSet.Contains(other)) return;
                if (!HitTaker.TryGetHealthManager(other.gameObject, out var hm)) return;
                hitSet.Add(other);

                float angle;
                if (other.transform.position.y > owner.transform.position.y && Mathf.Abs(owner.transform.position.x - other.transform.position.x) < 1f) angle = 90f;
                else if (other.transform.position.y < owner.transform.position.y && Mathf.Abs(owner.transform.position.x - other.transform.position.x) < 1f) angle = 270f;
                else angle = owner.facing >= 0 ? 0f : 180f;

                var hit = new HitInstance
                {
                    Source = gameObject,
                    AttackType = AttackTypes.Spell,
                    DamageDealt = damage,
                    Direction = angle,
                    MagnitudeMultiplier = 1f,
                    Multiplier = 1f,
                    IsHeroDamage = true,
                    IsFirstHit = true
                };

                HitTaker.Hit(other.gameObject, hit);
                hm.Hit(hit);
                owner.AddSoul(soulGain);
            }
        }
    }
}
