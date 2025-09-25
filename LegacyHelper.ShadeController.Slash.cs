#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using GlobalSettings;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        private enum ShamanSlashDirection
        {
            Horizontal,
            Up,
            Down,
        }

        private static readonly FieldInfo s_nailTravelInitialPosField = typeof(NailSlashTravel).GetField("initialLocalPos", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailTravelInitialScaleField = typeof(NailSlashTravel).GetField("initialLocalScale", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailSlashScaleField = typeof(NailAttackBase).GetField("scale", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailSlashLongScaleField = typeof(NailAttackBase).GetField("longNeedleScale", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailTravelDistanceField = typeof(NailSlashTravel).GetField("travelDistance", BindingFlags.Instance | BindingFlags.NonPublic);

        private void HandleNailAttack()
        {
            nailTimer -= Time.deltaTime;
            if (nailTimer > 0f) return;

            float forcedV = 0f;
            bool pressed = ShadeInput.WasActionPressed(ShadeAction.Nail);
            if (ShadeInput.WasActionPressed(ShadeAction.NailUp))
            {
                pressed = true;
                forcedV = 1f;
            }
            else if (ShadeInput.WasActionPressed(ShadeAction.NailDown))
            {
                pressed = true;
                forcedV = -1f;
            }
            if (pressed)
            {
                nailTimer = nailCooldown;
                if (shamanMovesetActive)
                    PerformShamanSlash(forcedV);
                else
                    PerformNailSlash(forcedV);
            }
        }

        private void PerformShamanSlash(float forcedV = 0f)
        {
            var hc = HeroController.instance;
            if (hc == null)
            {
                PerformNailSlash(forcedV);
                return;
            }

            if (!EnsureShamanSlashTemplates(hc))
            {
                PerformNailSlash(forcedV);
                return;
            }

            GameObject source = null;
            ShamanSlashDirection slashDirection = ShamanSlashDirection.Horizontal;
            float v = forcedV;
            if (v > 0.35f)
            {
                slashDirection = ShamanSlashDirection.Up;
                source = shamanUpSlashTemplate ?? shamanHorizontalSlashTemplate ?? shamanHorizontalAltSlashTemplate;
            }
            else if (v < -0.35f)
            {
                slashDirection = ShamanSlashDirection.Down;
                source = shamanUpSlashTemplate ?? shamanDownSlashTemplate ?? shamanHorizontalSlashTemplate ?? shamanHorizontalAltSlashTemplate;
            }
            else
            {
                slashDirection = ShamanSlashDirection.Horizontal;
                if (facing >= 0 && shamanHorizontalAltSlashTemplate != null)
                    source = shamanHorizontalAltSlashTemplate;
                else
                    source = shamanHorizontalSlashTemplate ?? shamanHorizontalAltSlashTemplate ?? shamanUpSlashTemplate ?? shamanDownSlashTemplate;
            }

            if (source == null)
            {
                PerformNailSlash(forcedV);
                return;
            }

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
            var marker = slash.AddComponent<ShadeSlashMarker>();
            if (marker != null)
            {
                marker.verticalInput = v;
                marker.invertDown = slashDirection == ShamanSlashDirection.Down;
            }

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
                ApplyBaseSlashOrientation(slash, nailSlash, v, marker != null && marker.invertDown);

                if (marker != null)
                {
                    marker.storedLocalScale = slash.transform.localScale;
                    marker.hasStoredScale = true;
                }

                var travel = slash.GetComponent<NailSlashTravel>();
                if (travel != null)
                {
                    var evt = typeof(HeroController).GetEvent("FlippedSprite");
                    var method = typeof(NailSlashTravel).GetMethod("OnHeroFlipped", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (evt != null && method != null)
                    {
                        var del = Delegate.CreateDelegate(evt.EventHandlerType, travel, method);
                        evt.RemoveEventHandler(hc, del);
                    }
                }

                if (ModConfig.Instance.logShade)
                {
                    var tr = slash.transform;
                    UnityEngine.Debug.Log($"[ShadeDebug] Shade slash spawned: {slash.name} scale={tr.localScale} parent={tr.parent?.name}\n{System.Environment.StackTrace}");
                }
            }
            catch { }

            StartCoroutine(AdoptSlashAfterFrame(slash));

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
                    nailDmg = Mathf.Max(1, Mathf.RoundToInt(nailDmg * ModConfig.Instance.shadeDamageMultiplier));
                    nailDmg = Mathf.Max(1, Mathf.RoundToInt(nailDmg * charmNailDamageMultiplier));
                    nailDmg = Mathf.Max(1, Mathf.RoundToInt(nailDmg * GetConditionalNailDamageMultiplier()));
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
                        if (!shamanMovesetActive && Mathf.Abs(v) < 0.35f && facing >= 0)
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
                                int soulGain = Mathf.Max(0, soulGainPerHit + charmSoulGainBonus);
                                shadeSoul = Mathf.Min(shadeSoulMax, shadeSoul + soulGain);
                                PushSoulToHud();
                                CheckHazardOverlap();
                                if (prevSoul < focusSoulCost && shadeSoul >= focusSoulCost)
                                {
                                    try { EnsureFocusSfx(); if (focusSfx != null && sfxFocusReady != null) focusSfx.PlayOneShot(sfxFocusReady, Mathf.Clamp01(GetEffectiveSfxVolume())); } catch { }
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

        private void PerformNailSlash(float forcedV = 0f)
        {
            var hc = HeroController.instance;
            if (hc == null) return;

            // Choose slash variant based on input: up / down / side
            GameObject source = null;
            float v = forcedV;

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
                ApplyBaseSlashOrientation(slash, nailSlash, v, invertDown: false);
                var tr = slash ? slash.transform : null;
                try
                {
                    var travel = slash.GetComponent<NailSlashTravel>();
                    if (travel != null)
                    {
                        var evt = typeof(HeroController).GetEvent("FlippedSprite");
                        var method = typeof(NailSlashTravel).GetMethod("OnHeroFlipped", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (evt != null && method != null)
                        {
                            var del = Delegate.CreateDelegate(evt.EventHandlerType, travel, method);
                            evt.RemoveEventHandler(hc, del);
                        }
                    }
                }
                catch { }
                if (ModConfig.Instance.logShade && tr != null)
                    UnityEngine.Debug.Log($"[ShadeDebug] Shade slash spawned: {slash.name} scale={tr.localScale} parent={tr.parent?.name}\n{System.Environment.StackTrace}");
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
                    nailDmg = Mathf.Max(1, Mathf.RoundToInt(nailDmg * ModConfig.Instance.shadeDamageMultiplier));
                    nailDmg = Mathf.Max(1, Mathf.RoundToInt(nailDmg * charmNailDamageMultiplier));
                    nailDmg = Mathf.Max(1, Mathf.RoundToInt(nailDmg * GetConditionalNailDamageMultiplier()));
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
                                int soulGain = Mathf.Max(0, soulGainPerHit + charmSoulGainBonus);
                                shadeSoul = Mathf.Min(shadeSoulMax, shadeSoul + soulGain);
                                PushSoulToHud();
                                CheckHazardOverlap();
                                if (prevSoul < focusSoulCost && shadeSoul >= focusSoulCost)
                                {
                                    try { EnsureFocusSfx(); if (focusSfx != null && sfxFocusReady != null) focusSfx.PlayOneShot(sfxFocusReady, Mathf.Clamp01(GetEffectiveSfxVolume())); } catch { }
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

        private void ApplyBaseSlashOrientation(GameObject slash, NailSlash nailSlash, float verticalInput, bool invertDown)
        {
            if (!slash) return;

            List<Transform> waveChildren = null;
            List<float> waveBaseSigns = null;
            List<SpriteRenderer> waveSprites = null;
            List<bool> waveBaseFlipY = null;

            if (invertDown)
            {
                try
                {
                    var sprites = slash.GetComponentsInChildren<SpriteRenderer>(true);
                    foreach (var sr in sprites)
                    {
                        if (!sr) continue;
                        var go = sr.gameObject;
                        if (!go) continue;

                        bool matches = false;
                        var goName = go.name ?? string.Empty;
                        if (!string.IsNullOrEmpty(goName) && goName.IndexOf("wave", StringComparison.OrdinalIgnoreCase) >= 0)
                            matches = true;
                        else
                        {
                            var spriteName = sr.sprite ? sr.sprite.name : null;
                            if (!string.IsNullOrEmpty(spriteName) && spriteName.IndexOf("wave", StringComparison.OrdinalIgnoreCase) >= 0)
                                matches = true;
                        }

                        if (!matches) continue;

                        var childTr = go.transform;
                        if (!childTr) continue;

                        waveChildren ??= new List<Transform>();
                        waveBaseSigns ??= new List<float>();

                        float lossy = childTr.lossyScale.y;
                        if (lossy == 0f) lossy = 1f;
                        waveChildren.Add(childTr);
                        waveBaseSigns.Add(Mathf.Sign(lossy));

                        waveSprites ??= new List<SpriteRenderer>();
                        waveBaseFlipY ??= new List<bool>();
                        waveSprites.Add(sr);
                        waveBaseFlipY.Add(sr.flipY);
                    }
                }
                catch { }
            }

            try
            {
                var tr = slash.transform;
                if (!tr) return;

                var ls = tr.localScale;

                float scaleSign = -facing;
                if (verticalInput > 0.35f && facing > 0f)
                    scaleSign = 1f;

                ls.x = Mathf.Abs(ls.x) * scaleSign;
                ls *= 1f / SpriteScale;
                ls *= charmNailScaleMultiplier;

                if (invertDown)
                {
                    ls.x = -ls.x;
                    ls.y = -ls.y;

                    // Ensure the down-slash stays vertically inverted regardless of any
                    // prefab quirks and keep the horizontal mirror consistent with the
                    // shade's facing so the wave art doesn't appear upside down when the
                    // shade attacks to the right.
                    ls.x = facing >= 0f ? Mathf.Abs(ls.x) : -Mathf.Abs(ls.x);
                    ls.y = -Mathf.Abs(ls.y);
                }

                tr.localScale = ls;

                if (nailSlash != null)
                {
                    try { s_nailSlashScaleField?.SetValue(nailSlash, ls); } catch { }
                    try { s_nailSlashLongScaleField?.SetValue(nailSlash, ls); } catch { }
                }

                if (invertDown && waveSprites != null)
                {
                    bool facingRight = facing > 0f;
                    try
                    {
                        for (int i = 0; i < waveSprites.Count; i++)
                        {
                            var sr = waveSprites[i];
                            if (!sr) continue;

                            bool baseFlip = waveBaseFlipY != null && i < waveBaseFlipY.Count ? waveBaseFlipY[i] : sr.flipY;
                            sr.flipY = facingRight ? !baseFlip : baseFlip;
                        }
                    }
                    catch { }
                }

                if (invertDown && facing > 0f && waveChildren != null)
                {
                    try
                    {
                        for (int i = 0; i < waveChildren.Count; i++)
                        {
                            var childTr = waveChildren[i];
                            if (!childTr) continue;

                            float targetSign = waveBaseSigns != null && i < waveBaseSigns.Count ? waveBaseSigns[i] : 1f;
                            if (targetSign == 0f) targetSign = 1f;
                            else targetSign = Mathf.Sign(targetSign);

                            float currentLossy = childTr.lossyScale.y;
                            float currentSign = currentLossy == 0f ? 1f : Mathf.Sign(currentLossy);

                            if (currentSign != targetSign)
                            {
                                var childScale = childTr.localScale;
                                if (Mathf.Abs(childScale.y) <= Mathf.Epsilon)
                                    childScale.y = targetSign;
                                else
                                    childScale.y = -childScale.y;

                                childTr.localScale = childScale;

                                currentLossy = childTr.lossyScale.y;
                                currentSign = currentLossy == 0f ? 1f : Mathf.Sign(currentLossy);
                                if (currentSign != targetSign)
                                {
                                    childScale = childTr.localScale;
                                    float magnitude = Mathf.Abs(childScale.y);
                                    if (magnitude <= Mathf.Epsilon) magnitude = 1f;
                                    childScale.y = magnitude * targetSign;
                                    childTr.localScale = childScale;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
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


        private class ShadeSlashMarker : MonoBehaviour
        {
            public float verticalInput;
            public bool invertDown;
            public Vector3 storedLocalScale;
            public bool hasStoredScale;
        }

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

        private IEnumerator AdoptSlashAfterFrame(GameObject slash)
        {
            yield return null;
            if (!slash) yield break;
            Transform tr = null;
            try { tr = slash.transform; }
            catch { }
            if (!tr) yield break;

            ShadeSlashMarker marker = null;
            try { marker = slash.GetComponent<ShadeSlashMarker>(); }
            catch { }

            float verticalInput = marker != null ? marker.verticalInput : 0f;
            bool invertDown = marker != null && marker.invertDown;

            try { tr.SetParent(transform, false); }
            catch { }
            try { tr.position = transform.position; }
            catch { }
            try { tr.localPosition = Vector3.zero; }
            catch { }

            var nailSlash = slash.GetComponent<NailSlash>();
            if (marker != null && marker.hasStoredScale)
            {
                try { tr.localScale = marker.storedLocalScale; } catch { }
                if (nailSlash != null)
                {
                    try { s_nailSlashScaleField?.SetValue(nailSlash, marker.storedLocalScale); } catch { }
                    try { s_nailSlashLongScaleField?.SetValue(nailSlash, marker.storedLocalScale); } catch { }
                }
            }
            else
            {
                ApplyBaseSlashOrientation(slash, nailSlash, verticalInput, invertDown);
            }

            var travel = slash.GetComponent<NailSlashTravel>();
            if (travel != null)
            {
                try { s_nailTravelInitialPosField?.SetValue(travel, tr.localPosition); } catch { }
                try { s_nailTravelInitialScaleField?.SetValue(travel, tr.localScale); } catch { }
                if (invertDown && s_nailTravelDistanceField != null)
                {
                    try
                    {
                        var distance = (Vector2)s_nailTravelDistanceField.GetValue(travel);
                        distance.y = -distance.y;
                        s_nailTravelDistanceField.SetValue(travel, distance);
                    }
                    catch { }
                }
            }
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

            float scale = SpriteScale * (IsProjectileUpgraded() ? 1.5f : 1f) * 1.6f;
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
            // Use spell progression for damage (2.5x upgraded, 30% less when unupgraded)
            int dmg = ComputeSpellDamageMultiplier(2.5f, IsProjectileUpgraded());
            sp.damage = Mathf.Max(1, dmg);
            sp.hornetRoot = hornetTransform;
            sp.destroyOnTerrain = !IsProjectileUpgraded();
            sp.maxRange = IsProjectileUpgraded() ? 22f : 0f;

            // SFX
            TryPlayFireballSfx();
        }

        private bool EnsureShamanSlashTemplates(HeroController hc)
        {
            var crest = Gameplay.SpellCrest;
            var config = crest ? crest.HeroConfig : null;
            if (hc == null || config == null)
            {
                shamanHorizontalSlashTemplate = null;
                shamanHorizontalAltSlashTemplate = null;
                shamanUpSlashTemplate = null;
                shamanDownSlashTemplate = null;
                shamanSlashConfigSource = null;
                shamanDownSlashType = HeroControllerConfig.DownSlashTypes.Slash;
                return false;
            }

            if (shamanSlashConfigSource == config && (shamanHorizontalSlashTemplate != null || shamanHorizontalAltSlashTemplate != null || shamanUpSlashTemplate != null))
            {
                return true;
            }

            shamanHorizontalSlashTemplate = null;
            shamanHorizontalAltSlashTemplate = null;
            shamanUpSlashTemplate = null;
            shamanDownSlashTemplate = null;
            shamanSlashConfigSource = null;
            shamanDownSlashType = config.DownSlashType;

            var group = FindShamanConfigGroup(hc, config);
            if (group == null)
            {
                return false;
            }

            shamanHorizontalSlashTemplate = group.NormalSlashObject ?? group.AlternateSlashObject;
            shamanHorizontalAltSlashTemplate = group.AlternateSlashObject;
            shamanUpSlashTemplate = group.UpSlashObject ?? group.AltUpSlashObject ?? shamanHorizontalSlashTemplate ?? shamanHorizontalAltSlashTemplate;
            shamanDownSlashType = config.DownSlashType;
            shamanDownSlashTemplate = group.DownSlashObject ?? group.AltDownSlashObject;

            shamanSlashConfigSource = config;
            return shamanHorizontalSlashTemplate != null || shamanHorizontalAltSlashTemplate != null || shamanUpSlashTemplate != null || shamanDownSlashTemplate != null;
        }

        private HeroController.ConfigGroup FindShamanConfigGroup(HeroController hc, HeroControllerConfig config)
        {
            if (hc == null || config == null)
            {
                return null;
            }

            try
            {
                var type = typeof(HeroController);
                foreach (var fieldName in new[] { "configs", "specialConfigs" })
                {
                    var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field == null)
                    {
                        continue;
                    }

                    if (field.GetValue(hc) is HeroController.ConfigGroup[] groups)
                    {
                        foreach (var group in groups)
                        {
                            if (group != null && group.Config == config)
                            {
                                return group;
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
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

#nullable restore
