#nullable disable
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
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
        private static readonly FieldInfo s_nailTravelHasStartedField = typeof(NailSlashTravel).GetField("hasStarted", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailTravelIsSlashActiveField = typeof(NailSlashTravel).GetField("isSlashActive", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailTravelDistanceField = typeof(NailSlashTravel).GetField("travelDistance", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailSlashScaleField = typeof(NailAttackBase).GetField("scale", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailSlashLongScaleField = typeof(NailAttackBase).GetField("longNeedleScale", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailSlashHeroField = typeof(NailAttackBase).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic);

        private const BindingFlags DamageEnemiesFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // These fifteen handles were previously re-resolved by name inside both
        // PerformShamanSlash and PerformNailSlash -- i.e. on every single slash.
        private static readonly FieldInfo s_deSourceIsHero = typeof(DamageEnemies).GetField("sourceIsHero", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_deIsHeroDamage = typeof(DamageEnemies).GetField("isHeroDamage", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_deDirection = typeof(DamageEnemies).GetField("direction", DamageEnemiesFlags);
        private static readonly FieldInfo s_deMoveDirection = typeof(DamageEnemies).GetField("moveDirection", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_deFlipDirectionIfBehind = typeof(DamageEnemies).GetField("flipDirectionIfBehind", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_deForwardVector = typeof(DamageEnemies).GetField("forwardVector", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_deIsNailAttack = typeof(DamageEnemies).GetField("isNailAttack", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_deOnlyDamageEnemies = typeof(DamageEnemies).GetField("onlyDamageEnemies", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo s_deSetOnlyDamageEnemies = typeof(DamageEnemies).GetMethod("setOnlyDamageEnemies", DamageEnemiesFlags);
        private static readonly FieldInfo s_deIgnoreNailPosition = typeof(DamageEnemies).GetField("ignoreNailPosition", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_deSilkGeneration = typeof(DamageEnemies).GetField("silkGeneration", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_deDoesNotGenerateSilk = typeof(DamageEnemies).GetField("doesNotGenerateSilk", DamageEnemiesFlags);
        private static readonly FieldInfo s_deAttackType = typeof(DamageEnemies).GetField("attackType", DamageEnemiesFlags);
        private static readonly FieldInfo s_deUseNailDamage = typeof(DamageEnemies).GetField("useNailDamage", DamageEnemiesFlags);
        private static readonly FieldInfo s_deDamageDealt = typeof(DamageEnemies).GetField("damageDealt", DamageEnemiesFlags);

        // includeStackTrace defaults to false: Environment.StackTrace walks and formats the
        // entire managed stack, which is far more expensive than the log line itself. Pass
        // true only from genuinely one-shot call sites.
        internal static void LogSlashState(string context, GameObject slash, ShadeController controller = null, bool includeStackTrace = false)
        {
            if (!ModConfig.Instance.logShade || !slash)
                return;

            // One boundary catch. Reading a Transform, a bool or a cached FieldInfo cannot
            // throw, so the ~35 individual try/catch wrappers this replaced only hid bugs.
            try
            {
                Transform tr = slash.transform;
                controller ??= tr.GetComponentInParent<ShadeController>();

                var sb = new StringBuilder();
                sb.Append("[ShadeDebug] ").Append(context ?? "Slash")
                  .Append(": ").Append(slash.name ?? "(unnamed)")
                  .Append(" frame=").Append(Time.frameCount)
                  .Append(" active=").Append(slash.activeSelf).Append('/').Append(slash.activeInHierarchy);

                Transform parent = tr.parent;
                sb.Append(" parent=").Append(parent ? parent.name : "(null)")
                  .Append(" localPos=").Append(tr.localPosition)
                  .Append(" worldPos=").Append(tr.position)
                  .Append(" localEuler=").Append(tr.localEulerAngles)
                  .Append(" localScale=").Append(tr.localScale)
                  .Append(" lossyScale=").Append(tr.lossyScale);

                if (controller != null)
                {
                    sb.Append(" shadeFacing=").Append(controller.facing)
                      .Append(" shadeLocalScale=").Append(controller.transform.localScale)
                      .Append(" shamanActive=").Append(controller.shamanMovesetActive);

                    var marker = slash.GetComponent<ShadeSlashMarker>();
                    if (marker != null)
                    {
                        sb.Append(" markerVertical=").Append(marker.verticalInput.ToString("0.###", CultureInfo.InvariantCulture))
                          .Append(" markerInvertDown=").Append(marker.invertDown)
                          .Append(" markerHasStoredScale=").Append(marker.hasStoredScale);
                        if (marker.hasStoredScale)
                            sb.Append(" markerStoredScale=").Append(marker.storedLocalScale);
                    }
                }

                var nailSlash = slash.GetComponent<NailSlash>();
                if (nailSlash != null)
                {
                    sb.Append(" anim=").Append(nailSlash.animName ?? "(null)")
                      .Append(" isStarting=").Append(nailSlash.IsStartingSlash)
                      .Append(" isOut=").Append(nailSlash.IsSlashOut);
                }

                NailAttackBase nailAttack = nailSlash != null ? nailSlash : slash.GetComponent<NailAttackBase>();
                if (nailAttack != null)
                {
                    AppendField(sb, " baseScale=", s_nailSlashScaleField, nailAttack);
                    AppendField(sb, " longScale=", s_nailSlashLongScaleField, nailAttack);

                    if (s_nailSlashHeroField?.GetValue(nailAttack) is HeroController hero && hero)
                    {
                        sb.Append(" heroScale=").Append(hero.transform.localScale)
                          .Append(" heroPos=").Append(hero.transform.position);
                    }
                }

                var travel = slash.GetComponent<NailSlashTravel>();
                if (travel != null)
                {
                    sb.Append(" travelLocalPos=").Append(travel.transform.localPosition);
                    AppendField(sb, " travelInitialPos=", s_nailTravelInitialPosField, travel);
                    AppendField(sb, " travelInitialScale=", s_nailTravelInitialScaleField, travel);
                    AppendField(sb, " travelDistance=", s_nailTravelDistanceField, travel);
                    AppendField(sb, " travelHasStarted=", s_nailTravelHasStartedField, travel);
                    AppendField(sb, " travelIsActive=", s_nailTravelIsSlashActiveField, travel);
                }

                if (includeStackTrace)
                {
                    sb.Append('\n').Append(System.Environment.StackTrace);
                }

                UnityEngine.Debug.Log(sb.ToString());
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.Log($"[ShadeDebug] LogSlashState error: {ex}");
            }
        }

        /// <summary>
        /// Appends a reflected field value, skipping the entry entirely if the handle did
        /// not resolve against this build of the game.
        /// </summary>
        private static void AppendField(StringBuilder sb, string label, FieldInfo field, object target)
        {
            if (field == null) return;
            sb.Append(label).Append(field.GetValue(target));
        }

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
            float orientationFacing = facing >= 0 ? 1f : -1f;
            if (slashDirection == ShamanSlashDirection.Down && orientationFacing > 0f)
                orientationFacing = -orientationFacing;

            if (marker != null)
            {
                marker.verticalInput = v;
                marker.invertDown = slashDirection == ShamanSlashDirection.Down;
                marker.orientationFacing = orientationFacing;
            }

            LogSlashState("Shaman slash spawn (pre-orient)", slash, this);

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
                ApplyBaseSlashOrientation(slash, nailSlash, v, marker != null && marker.invertDown, orientationFacing);

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

                LogSlashState("Shaman slash oriented", slash, this, includeStackTrace: false);
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
                        actField.SetValue(nailSlash, Array.Empty<GameObject>());
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
                        if (!string.IsNullOrEmpty(tn) && tn.Contains("Recoil", System.StringComparison.OrdinalIgnoreCase))
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

                    int nailDmg = GetShadeNailDamage();
                    foreach (var d in damagers)
                    {
                        if (!d) continue;
                        try { s_deSourceIsHero?.SetValue(d, false); } catch { }
                        try { s_deIsHeroDamage?.SetValue(d, false); } catch { }
                        try { s_deIsNailAttack?.SetValue(d, false); } catch { }
                        try { s_deAttackType?.SetValue(d, AttackTypes.Generic); } catch { }
                        try { s_deDirection?.SetValue(d, dir); } catch { }
                        try { s_deMoveDirection?.SetValue(d, false); } catch { }
                        try { s_deFlipDirectionIfBehind?.SetValue(d, false); } catch { }
                        try { s_deForwardVector?.SetValue(d, fwd); } catch { }
                        try { if (s_deSetOnlyDamageEnemies != null) s_deSetOnlyDamageEnemies.Invoke(d, new object[] { false }); else s_deOnlyDamageEnemies?.SetValue(d, false); } catch { }
                        try { s_deIgnoreNailPosition?.SetValue(d, true); } catch { }
                        try { if (s_deSilkGeneration != null) { var enumType = s_deSilkGeneration.FieldType; var noneVal = System.Enum.ToObject(enumType, 0); s_deSilkGeneration.SetValue(d, noneVal);} } catch { }
                        try { s_deDoesNotGenerateSilk?.SetValue(d, true); } catch { }
                        try { s_deUseNailDamage?.SetValue(d, false); } catch { }
                        try { s_deDamageDealt?.SetValue(d, nailDmg); } catch { }
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

            LogSlashState("Shade slash spawn (pre-orient)", slash, this);

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
                ApplyBaseSlashOrientation(slash, nailSlash, v, invertDown: false, facing);
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
                LogSlashState("Shade slash oriented", slash, this, includeStackTrace: false);
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
                        actField.SetValue(nailSlash, Array.Empty<GameObject>());
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
                        if (!string.IsNullOrEmpty(tn) && tn.Contains("Recoil", System.StringComparison.OrdinalIgnoreCase))
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

                Vector2 slashForward = (facing >= 0 ? Vector2.right : Vector2.left);
                float slashDir = (facing >= 0 ? 0f : 180f);
                if (v > 0.35f)
                {
                    slashDir = 90f;
                    slashForward = Vector2.up;
                }
                else if (v < -0.35f)
                {
                    slashDir = 270f;
                    slashForward = Vector2.down;
                }

                try
                {
                    var damagers = slash.GetComponentsInChildren<DamageEnemies>(true);

                    int nailDmg = GetShadeNailDamage();
                    foreach (var d in damagers)
                    {
                        if (!d) continue;
                        try { s_deSourceIsHero?.SetValue(d, false); } catch { }
                        try { s_deIsHeroDamage?.SetValue(d, false); } catch { }
                        try { s_deIsNailAttack?.SetValue(d, false); } catch { }
                        try { s_deAttackType?.SetValue(d, AttackTypes.Generic); } catch { }
                        try { s_deDirection?.SetValue(d, slashDir); } catch { }
                        try { s_deMoveDirection?.SetValue(d, false); } catch { }
                        try { s_deFlipDirectionIfBehind?.SetValue(d, false); } catch { }
                        try { s_deForwardVector?.SetValue(d, slashForward); } catch { }
                        try { if (s_deSetOnlyDamageEnemies != null) s_deSetOnlyDamageEnemies.Invoke(d, new object[] { false }); else s_deOnlyDamageEnemies?.SetValue(d, false); } catch { }
                        try { s_deIgnoreNailPosition?.SetValue(d, true); } catch { }
                        try { if (s_deSilkGeneration != null) { var enumType = s_deSilkGeneration.FieldType; var noneVal = System.Enum.ToObject(enumType, 0); s_deSilkGeneration.SetValue(d, noneVal);} } catch { }
                        try { s_deDoesNotGenerateSilk?.SetValue(d, true); } catch { }
                        try { s_deUseNailDamage?.SetValue(d, false); } catch { }
                        try { s_deDamageDealt?.SetValue(d, nailDmg); } catch { }
                        try { d.magnitudeMult = Mathf.Max(0.01f, d.magnitudeMult * charmNailKnockbackMultiplier); } catch { }
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
                            Vector2 recoilDirection = slashForward.sqrMagnitude > 0.001f
                                ? slashForward.normalized
                                : (facing >= 0 ? Vector2.right : Vector2.left);
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
                                ApplyAttackRecoil(recoilDirection);
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

        private void ApplyBaseSlashOrientation(GameObject slash, NailSlash nailSlash, float verticalInput, bool invertDown, float facingForSlash)
        {
            if (!slash) return;

            try
            {
                var tr = slash.transform;
                if (!tr) return;

                var ls = tr.localScale;

                float usedFacing = facingForSlash;
                if (usedFacing == 0f)
                    usedFacing = facing >= 0 ? 1f : -1f;

                float scaleSign = -Mathf.Sign(usedFacing);
                if (verticalInput > 0.35f && usedFacing > 0f)
                    scaleSign = 1f;

                ls.x = Mathf.Abs(ls.x) * scaleSign;
                ls *= 1f / SpriteScale;
                ls *= charmNailScaleMultiplier;

                if (invertDown)
                {
                    ls.x = -ls.x;
                    ls.y = -ls.y;
                }

                tr.localScale = ls;

                if (nailSlash != null)
                {
                    try { s_nailSlashScaleField?.SetValue(nailSlash, ls); } catch { }
                    try { s_nailSlashLongScaleField?.SetValue(nailSlash, ls); } catch { }
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
            public float orientationFacing;
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
            marker = slash.GetComponent<ShadeSlashMarker>();

            float verticalInput = marker != null ? marker.verticalInput : 0f;
            bool invertDown = marker != null && marker.invertDown;
            float markerFacing = marker != null && marker.orientationFacing != 0f ? marker.orientationFacing : (facing >= 0 ? 1f : -1f);

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
                ApplyBaseSlashOrientation(slash, nailSlash, verticalInput, invertDown, markerFacing);
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

            LogSlashState("Shaman slash adopted", slash, this, includeStackTrace: false);
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

            const float shadeSoulScaleMultiplier = 1.6f * 0.7f; // reduce projectile size by 30%
            float scale = SpriteScale * (IsProjectileUpgraded() ? 1.5f : 1f) * shadeSoulScaleMultiplier;
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
            const int size = 8;
            var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = center.x;
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    Vector2 pos = new Vector2(x, y);
                    float dist = Vector2.Distance(pos, center);
                    if (dist <= radius)
                    {
                        float t = Mathf.Clamp01(1f - dist / radius);
                        float alpha = t * t;
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                    }
                    else
                    {
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0f));
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 16f);
        }

    }
}

#nullable restore
