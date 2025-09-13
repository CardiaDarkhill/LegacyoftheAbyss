using System.Collections;
using System.Reflection;
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        private void HandleNailAttack()
        {
            nailTimer -= Time.deltaTime;
            if (nailTimer > 0f) return;
            bool pressed = Input.GetMouseButtonDown(0) || Input.GetKeyDown(NailKey);
            if (pressed)
            {
                nailTimer = nailCooldown;
                PerformNailSlash();
            }
        }

        private void PerformNailSlash()
        {
            var hc = HeroController.instance;
            if (hc == null) return;

            // Choose slash variant based on input: up / down / side
            GameObject source = null;
            float v = (Input.GetKey(KeyCode.S) ? -1f : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);

            try
            {
                var allHeroSlashes = hc.GetComponentsInChildren<NailSlash>(true);
                if (allHeroSlashes == null || allHeroSlashes.Length == 0)
                    allHeroSlashes = Resources.FindObjectsOfTypeAll<NailSlash>();
                if (allHeroSlashes != null && allHeroSlashes.Length > 0)
                {
                    bool IsWanderer(NailSlash ns) => ns && ns.transform.parent && ns.transform.parent.name == "Wanderer";
                    var wanderer = System.Array.FindAll(allHeroSlashes, s => IsWanderer(s));
                    var searchSet = (wanderer != null && wanderer.Length > 0) ? wanderer : allHeroSlashes;

                    bool MatchUp(NailSlash ns) { return ns && (((ns.name ?? "").ToLowerInvariant().Contains("up")) || ((ns.animName ?? "").ToLowerInvariant().Contains("up"))); }
                    bool MatchDown(NailSlash ns) { return ns && (((ns.name ?? "").ToLowerInvariant().Contains("down")) || ((ns.animName ?? "").ToLowerInvariant().Contains("down"))); }
                    bool MatchNormal(NailSlash ns) { return ns && !MatchUp(ns) && !MatchDown(ns); }

                    NailSlash pick = null;
                    if (v > 0.35f) pick = System.Array.Find(searchSet, s => MatchUp(s));
                    else if (v < -0.35f) pick = System.Array.Find(searchSet, s => MatchDown(s));
                    else pick = System.Array.Find(searchSet, s => MatchNormal(s));
                    if (pick == null) pick = searchSet[0];
                    source = pick ? pick.gameObject : null;
                }
            }
            catch { }
            if (source == null) return;

            // remove lingering slashes from prior attacks
            DestroyOtherSlashes(null);

            // Spawn the slash while suppressing any activateOnSlash side effects
            GameObject slash = null;
            suppressActivateOnSlash = true;
            expectedSlashParent = hc.transform;
            try
            {
                slash = GameObject.Instantiate(source, hc.transform);
            }
            finally
            {
                expectedSlashParent = null;
                suppressActivateOnSlash = false;
            }
            slash.transform.SetParent(transform, false);
            slash.AddComponent<ShadeSlashMarker>();
            slash.transform.position = transform.position;

            var nailSlash = slash.GetComponent<NailSlash>();

            var tempCols = slash.GetComponentsInChildren<Collider2D>(true);

            try
            {
                int desiredLayer = source.layer;
                foreach (var t in slash.GetComponentsInChildren<Transform>(true))
                {
                    if (!t) continue;
                    t.gameObject.layer = desiredLayer;
                    t.gameObject.tag = "Untagged";
                }
            }
            catch { }

            try
            {
                var tr = slash.transform;
                var ls = tr.localScale;
                // Wanderer slashes are authored facing left. To mirror them so the
                // shade attacks to the right, we use a negative X scale when facing right.
                // NailAttackBase.OnSlashStarting later resets transform.localScale from its
                // private "scale" field, so we update that field (and longNeedleScale) too.
                ls.x = Mathf.Abs(ls.x) * -facing;
                // Previously we inverted the Y scale for right-facing up-slashes, which
                // caused the animation to play downward. Keeping the Y scale positive
                // ensures upward slashes display correctly regardless of facing.
                ls.y = Mathf.Abs(ls.y);
                ls *= 1f / SpriteScale;
                tr.localScale = ls;
                if (nailSlash != null)
                {
                    var scaleField = typeof(NailAttackBase).GetField("scale", BindingFlags.Instance | BindingFlags.NonPublic);
                    var longNeedleField = typeof(NailAttackBase).GetField("longNeedleScale", BindingFlags.Instance | BindingFlags.NonPublic);
                    try { scaleField?.SetValue(nailSlash, ls); } catch { }
                    try { longNeedleField?.SetValue(nailSlash, ls); } catch { }
                }
                UnityEngine.Debug.Log($"[ShadeDebug] Shade slash spawned: {slash.name} scale={ls} parent={tr.parent?.name}\n{System.Environment.StackTrace}");
            }
            catch { }

            // Proactively ignore collisions with Hornet before re-enabling colliders
            try
            {
                if (hornetTransform != null)
                {
                    var hornetCols = hornetTransform.GetComponentsInChildren<Collider2D>(true);
                    foreach (var sc in tempCols)
                        foreach (var hc2 in hornetCols)
                            if (sc && hc2) Physics2D.IgnoreCollision(sc, hc2, true);
                }
                var shadeCols = GetComponentsInChildren<Collider2D>(true);
                foreach (var sc in tempCols)
                    foreach (var sh in shadeCols)
                        if (sc && sh) Physics2D.IgnoreCollision(sc, sh, true);
            }
            catch { }

            // Disable known extra damager object if present
            try
            {
                var extra = slash.transform.Find("Extra Damager");
                if (extra && extra.gameObject) extra.gameObject.SetActive(false);
            }
            catch { }

            if (nailSlash != null)
            {
                var f = typeof(NailAttackBase).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic);
                f?.SetValue(nailSlash, hc);

                try
                {
                    var travel = slash.GetComponent<NailSlashTravel>();
                    if (travel != null)
                    {
                        var tf = typeof(NailSlashTravel).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic);
                        tf?.SetValue(travel, hc);
                    }
                }
                catch { }

                // Prevent StartSlash from activating any additional slashes from Hornet
                try
                {
                    var actField = typeof(NailAttackBase).GetField("activateOnSlash", BindingFlags.Instance | BindingFlags.NonPublic);
                    var arr = actField?.GetValue(nailSlash) as GameObject[];
                    if (arr != null)
                    {
                        foreach (var go in arr)
                            if (go)
                                go.SetActive(false);
                        actField.SetValue(nailSlash, new GameObject[0]);
                    }
                }
                catch { }

                try
                {
                    var recoils = slash.GetComponentsInChildren<NailSlashRecoil>(true);
                    foreach (var r in recoils) if (r) Destroy(r);
                    // Remove any other behaviours with 'Recoil' in their type name (belt-and-braces)
                    var allBehaviours = slash.GetComponentsInChildren<MonoBehaviour>(true);
                    foreach (var mb in allBehaviours)
                    {
                        if (!mb) continue;
                        var tn = mb.GetType().Name;
                        if (!string.IsNullOrEmpty(tn) && tn.IndexOf("Recoil", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            try { Destroy(mb); } catch { }
                        }
                    }
                }
                catch { }
                // Remove helpers that can extend hit windows
                try { var extra = slash.GetComponentsInChildren<HeroExtraNailSlash>(true); foreach (var x in extra) if (x) Destroy(x); } catch { }
                try { var thunks = slash.GetComponentsInChildren<NailSlashTerrainThunk>(true); foreach (var t in thunks) if (t) Destroy(t); } catch { }
                try { var downAttacks = slash.GetComponentsInChildren<HeroDownAttack>(true); foreach (var d in downAttacks) if (d) Destroy(d); } catch { }

                try
                {
                    var damagers = slash.GetComponentsInChildren<DamageEnemies>(true);
                    var srcField = typeof(DamageEnemies).GetField("sourceIsHero", BindingFlags.Instance | BindingFlags.NonPublic);
                    var ihField  = typeof(DamageEnemies).GetField("isHeroDamage", BindingFlags.Instance | BindingFlags.NonPublic);
                    var dirField = typeof(DamageEnemies).GetField("direction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var moveDirField = typeof(DamageEnemies).GetField("moveDirection", BindingFlags.Instance | BindingFlags.NonPublic);
                    var flipBehindField = typeof(DamageEnemies).GetField("flipDirectionIfBehind", BindingFlags.Instance | BindingFlags.NonPublic);
                    var fwdVecField = typeof(DamageEnemies).GetField("forwardVector", BindingFlags.Instance | BindingFlags.NonPublic);
                    var isNailAttackField = typeof(DamageEnemies).GetField("isNailAttack", BindingFlags.Instance | BindingFlags.NonPublic);
                    var onlyEnemiesField = typeof(DamageEnemies).GetField("onlyDamageEnemies", BindingFlags.Instance | BindingFlags.NonPublic);
                    var setOnlyEnemies = typeof(DamageEnemies).GetMethod("setOnlyDamageEnemies", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var ignoreNailPosField = typeof(DamageEnemies).GetField("ignoreNailPosition", BindingFlags.Instance | BindingFlags.NonPublic);
                    var silkGenField = typeof(DamageEnemies).GetField("silkGeneration", BindingFlags.Instance | BindingFlags.NonPublic);
                    var doesNotGenSilkField = typeof(DamageEnemies).GetField("doesNotGenerateSilk", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    var attackTypeField = typeof(DamageEnemies).GetField("attackType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var useNailDmgField = typeof(DamageEnemies).GetField("useNailDamage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var damageDealtField = typeof(DamageEnemies).GetField("damageDealt", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    float dir = 0f;
                    Vector2 fwd = Vector2.zero;
                    if (v > 0.35f)
                    {
                        dir = 90f;
                        fwd = Vector2.up;
                    }
                    else if (v < -0.35f)
                    {
                        dir = 270f;
                        fwd = Vector2.down;
                    }
                    else
                    {
                        dir = (facing >= 0 ? 0f : 180f);
                        fwd = (facing >= 0 ? Vector2.right : Vector2.left);
                    }

                    int nailDmg = Mathf.Max(1, GetHornetNailDamage());
                    foreach (var d in damagers)
                    {
                        if (!d) continue;
                        try { srcField?.SetValue(d, false); } catch { }
                        try { ihField?.SetValue(d, false); } catch { }
                        try { isNailAttackField?.SetValue(d, false); } catch { }
                        try { attackTypeField?.SetValue(d, AttackTypes.Generic); } catch { }
                        try { dirField?.SetValue(d, dir); } catch { }
                        try { moveDirField?.SetValue(d, false); } catch { }
                        try { flipBehindField?.SetValue(d, false); } catch { }
                        try { fwdVecField?.SetValue(d, fwd); } catch { }
                        try { if (setOnlyEnemies != null) setOnlyEnemies.Invoke(d, new object[] { false }); else onlyEnemiesField?.SetValue(d, false); } catch { }
                        try { ignoreNailPosField?.SetValue(d, true); } catch { }
                        try { if (silkGenField != null) { var enumType = silkGenField.FieldType; var noneVal = System.Enum.ToObject(enumType, 0); silkGenField.SetValue(d, noneVal);} } catch { }
                        try { doesNotGenSilkField?.SetValue(d, true); } catch { }
                        try { useNailDmgField?.SetValue(d, false); } catch { }
                        try { damageDealtField?.SetValue(d, nailDmg); } catch { }
                    }

                }
                catch { }

                    // Disable extra damagers beyond the first
                    try
                    {
                        var allDamagers = slash.GetComponentsInChildren<DamageEnemies>(true);
                        bool firstKept = false;
                        foreach (var d in allDamagers)
                        {
                            if (!d) continue;
                            if (!firstKept) { firstKept = true; continue; }
                            d.enabled = false;
                        }
                    }
                    catch { }

                    // Prefer Alternate anim clip for right-facing side slash when available
                    try
                    {
                        if (Mathf.Abs(v) < 0.35f && facing >= 0)
                        {
                            var altSlashProp = hc.GetType().GetProperty("AlternateSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(hc, null) as NailSlash;
                            if (altSlashProp != null && !string.IsNullOrEmpty(altSlashProp.animName))
                            {
                                nailSlash.animName = altSlashProp.animName;
                            }
                        }
                    }
                    catch { }

                    // Start the slash once we've patched it
                    nailSlash.StartSlash();

                    // Ensure we fully end the hitboxes when damage ends to avoid lingering hits
                    try
                    {
                        var primaryDamager = nailSlash.EnemyDamager;
                        if (primaryDamager != null)
                        {
                            System.Action onDamaged = null; System.Action<bool> onEnded = null;
                            onDamaged = () =>
                            {
                                int prevSoul = shadeSoul;
                                shadeSoul = Mathf.Min(shadeSoulMax, shadeSoul + soulGainPerHit);
                                PushSoulToHud();
                                CheckHazardOverlap();
                                if (prevSoul < focusSoulCost && shadeSoul >= focusSoulCost)
                                {
                                    try { EnsureFocusSfx(); if (focusSfx != null && sfxFocusReady != null) focusSfx.PlayOneShot(sfxFocusReady); } catch { }
                                }
                            };
                            primaryDamager.DamagedEnemy += onDamaged;

                            onEnded = (didHit) =>
                            {
                                try { primaryDamager.DamagedEnemy -= onDamaged; } catch { }
                                try { nailSlash.EndedDamage -= onEnded; } catch { }
                                try {
                                    var damagersAll = slash.GetComponentsInChildren<DamageEnemies>(true);
                                    foreach (var de in damagersAll) if (de) de.enabled = false;
                                    var colsAll = slash.GetComponentsInChildren<Collider2D>(true);
                                    foreach (var c2 in colsAll) if (c2) c2.enabled = false;
                                } catch { }
                                try { slash.SetActive(false); } catch { }
                                try { Destroy(slash); } catch { }
                            };
                            nailSlash.EndedDamage += onEnded;
                        }
                    }
                    catch { }
                    // Failsafe to ensure no lingering colliders/hitboxes
                    StartCoroutine(DisableSlashAfterWindow(slash, 0.3f));
                }
            else
            {
                // No NailSlash component found
            }

            DestroyOtherSlashes(slash);

        }

        private void DestroyOtherSlashes(GameObject keep)
        {
            try
            {
                var slashes = transform.GetComponentsInChildren<NailSlash>(true);
                foreach (var ns in slashes)
                {
                    if (!ns) continue;
                    if (keep != null && ns.gameObject == keep) continue;
                    ns.gameObject.SetActive(false);
                    Destroy(ns.gameObject);
                }
            }
            catch { }
        }


        private class ShadeSlashMarker : MonoBehaviour { }

        private IEnumerator DisableSlashAfterWindow(GameObject slash, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (!slash) yield break;
            try
            {
                var damagersAll = slash.GetComponentsInChildren<DamageEnemies>(true);
                foreach (var de in damagersAll) if (de) de.enabled = false;
                var colsAll = slash.GetComponentsInChildren<Collider2D>(true);
                foreach (var c2 in colsAll) if (c2) c2.enabled = false;
            }
            catch { }
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

            var others = Object.FindObjectsByType<ShadeProjectile>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
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
            // Use spell progression for damage (2.5x upgraded, 30% less when unupgraded)
            int dmg = ComputeSpellDamageMultiplier(2.5f, IsProjectileUpgraded());
            sp.damage = Mathf.Max(1, dmg);
            sp.hornetRoot = hornetTransform;
            sp.destroyOnTerrain = !IsProjectileUpgraded();
            sp.maxRange = IsProjectileUpgraded() ? 22f : 0f;

            // SFX
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

    }
}

