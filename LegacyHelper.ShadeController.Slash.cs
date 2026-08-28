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
        private static readonly FieldInfo s_nailTravelInitialPosField = typeof(NailSlashTravel).GetField("initialLocalPos", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailTravelInitialScaleField = typeof(NailSlashTravel).GetField("initialLocalScale", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailTravelHasStartedField = typeof(NailSlashTravel).GetField("hasStarted", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailTravelIsSlashActiveField = typeof(NailSlashTravel).GetField("isSlashActive", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailTravelDistanceField = typeof(NailSlashTravel).GetField("travelDistance", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailSlashScaleField = typeof(NailAttackBase).GetField("scale", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailSlashLongScaleField = typeof(NailAttackBase).GetField("longNeedleScale", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailSlashHeroField = typeof(NailAttackBase).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailActivateOnSlashField = typeof(NailAttackBase).GetField("activateOnSlash", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo s_nailTravelHeroField = typeof(NailSlashTravel).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo s_travelOnHeroFlipped = typeof(NailSlashTravel).GetMethod("OnHeroFlipped", BindingFlags.Instance | BindingFlags.NonPublic);

        // The crest's slash prefabs are only reachable through these two arrays; nothing public
        // exposes a ConfigGroup by its Config.
        private static readonly FieldInfo[] s_heroConfigGroupFields = new[]
        {
            typeof(HeroController).GetField("configs", BindingFlags.Instance | BindingFlags.NonPublic),
            typeof(HeroController).GetField("specialConfigs", BindingFlags.Instance | BindingFlags.NonPublic)
        };

        private const BindingFlags DamageEnemiesFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // Resolved once: these were previously re-resolved by name on every slash.
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

        // includeStackTrace walks and formats the entire managed stack, far more expensive than the
        // log line itself. Pass true only from genuinely one-shot call sites.
        internal static void LogSlashState(string context, GameObject slash, ShadeController controller = null, bool includeStackTrace = false)
        {
            if (!ModConfig.Instance.logShade || !slash)
                return;

            // One boundary catch for the whole dump: reading a Transform, a bool or a cached
            // FieldInfo cannot throw, so per-statement guards here would only hide bugs.
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
            if (hc == null || !EnsureShamanSlashTemplates(hc))
            {
                PerformNailSlash(forcedV);
                return;
            }

            float v = forcedV;
            GameObject source;
            bool invertDown = false;
            if (v > 0.35f)
            {
                source = shamanUpSlashTemplate ?? shamanHorizontalSlashTemplate ?? shamanHorizontalAltSlashTemplate;
            }
            else if (v < -0.35f)
            {
                invertDown = true;
                source = shamanUpSlashTemplate ?? shamanDownSlashTemplate ?? shamanHorizontalSlashTemplate ?? shamanHorizontalAltSlashTemplate;
            }
            else
            {
                source = facing >= 0 && shamanHorizontalAltSlashTemplate != null
                    ? shamanHorizontalAltSlashTemplate
                    : shamanHorizontalSlashTemplate ?? shamanHorizontalAltSlashTemplate ?? shamanUpSlashTemplate ?? shamanDownSlashTemplate;
            }

            if (source == null)
            {
                PerformNailSlash(forcedV);
                return;
            }

            DestroyOtherSlashes(null);

            GameObject slash = SpawnSlash(hc, source);

            float orientationFacing = facing >= 0 ? 1f : -1f;
            if (invertDown && orientationFacing > 0f)
                orientationFacing = -orientationFacing;

            var marker = slash.AddComponent<ShadeSlashMarker>();
            marker.verticalInput = v;
            marker.invertDown = invertDown;
            marker.orientationFacing = orientationFacing;

            LogSlashState("Shaman slash spawn (pre-orient)", slash, this);

            ConfigureSpawnedSlash(hc, slash, source, v, invertDown, orientationFacing, "Shaman slash oriented", marker);

            // The clone stays parented to Hornet for a frame so it measures against the hierarchy it
            // came from; AdoptSlashAfterFrame moves it onto the Shade once it has.
            StartCoroutine(AdoptSlashAfterFrame(slash));

            DestroyOtherSlashes(slash);
        }

        private void PerformNailSlash(float forcedV = 0f)
        {
            var hc = HeroController.instance;
            if (hc == null) return;

            float v = forcedV;
            GameObject source = FindHeroSlashTemplate(hc, v);
            if (source == null) return;

            DestroyOtherSlashes(null);

            GameObject slash = SpawnSlash(hc, source);
            slash.transform.SetParent(transform, false);
            slash.AddComponent<ShadeSlashMarker>();
            slash.transform.position = transform.position;

            LogSlashState("Shade slash spawn (pre-orient)", slash, this);

            ConfigureSpawnedSlash(hc, slash, source, v, invertDown: false, orientationFacing: facing, "Shade slash oriented", marker: null);

            DestroyOtherSlashes(slash);
        }

        /// <summary>
        /// Picks the hero slash matching the input direction. Prefers the Wanderer crest's set when
        /// the scene has one, since that is the moveset the Shade mirrors.
        /// </summary>
        private static GameObject FindHeroSlashTemplate(HeroController hc, float v)
        {
            var all = hc.GetComponentsInChildren<NailSlash>(true);
            if (all == null || all.Length == 0)
                all = Resources.FindObjectsOfTypeAll<NailSlash>();
            if (all == null || all.Length == 0)
                return null;

            static bool IsWanderer(NailSlash ns) => ns && ns.transform.parent && ns.transform.parent.name == "Wanderer";
            static bool Mentions(NailSlash ns, string word) => ns
                && ((ns.name ?? string.Empty).Contains(word, StringComparison.OrdinalIgnoreCase)
                    || (ns.animName ?? string.Empty).Contains(word, StringComparison.OrdinalIgnoreCase));

            var wanderer = Array.FindAll(all, IsWanderer);
            var searchSet = wanderer.Length > 0 ? wanderer : all;

            NailSlash pick;
            if (v > 0.35f)
                pick = Array.Find(searchSet, s => Mentions(s, "up"));
            else if (v < -0.35f)
                pick = Array.Find(searchSet, s => Mentions(s, "down"));
            else
                pick = Array.Find(searchSet, s => !Mentions(s, "up") && !Mentions(s, "down"));

            pick ??= searchSet[0];
            return pick ? pick.gameObject : null;
        }

        /// <summary>
        /// Clones a hero slash under Hornet. The suppression flags keep the clone's own
        /// activateOnSlash chain from firing Hornet's other slashes while it is being built.
        /// </summary>
        private GameObject SpawnSlash(HeroController hc, GameObject source)
        {
            suppressActivateOnSlash = true;
            expectedSlashParent = hc.transform;
            try
            {
                return GameObject.Instantiate(source, hc.transform);
            }
            finally
            {
                expectedSlashParent = null;
                suppressActivateOnSlash = false;
            }
        }

        /// <summary>
        /// Turns a cloned hero slash into a Shade slash: strips what makes it Hornet's, points its
        /// damage at enemies only, and starts it. Shared by both slash paths, which differ only in
        /// where the clone is parented and which of Hornet's extras still apply.
        /// </summary>
        /// <param name="marker">
        /// The shaman path's marker, carrying orientation across the frame the clone spends parented
        /// to Hornet. Null on the nail path, which parents to the Shade immediately - and so also
        /// serves as "this is the nail path" for the two behaviours that only apply there.
        /// </param>
        private void ConfigureSpawnedSlash(
            HeroController hc,
            GameObject slash,
            GameObject source,
            float v,
            bool invertDown,
            float orientationFacing,
            string logContext,
            ShadeSlashMarker marker)
        {
            bool nailPath = marker == null;
            var nailSlash = slash.GetComponent<NailSlash>();
            var slashCols = slash.GetComponentsInChildren<Collider2D>(true);

            // The clone arrives carrying Hornet's own slash tag ("Nail Attack"), and must not keep
            // it. That tag is what every hero-only nail reaction keys off, so a tagged Shade slash
            // feeds Hornet's systems - silk gain on hit the clearest of them - from a second source
            // she never swung. Do not put it back to fix a single interaction.
            //
            // Known cost: FSMs that gate on the tag ignore the Shade, so nail-triggered breakables
            // such as moss fruits cannot be hit by it. Separating the two wants per-reaction
            // filtering rather than a tag.
            int desiredLayer = source.layer;
            foreach (var t in slash.GetComponentsInChildren<Transform>(true))
            {
                if (!t) continue;
                t.gameObject.layer = desiredLayer;
                t.gameObject.tag = "Untagged";
            }

            ApplyBaseSlashOrientation(slash, nailSlash, v, invertDown, orientationFacing);

            if (marker != null)
            {
                marker.storedLocalScale = slash.transform.localScale;
                marker.hasStoredScale = true;
            }

            DetachHeroFlipHandler(hc, slash);
            LogSlashState(logContext, slash, this);

            IgnoreCollisionsWithOwners(slashCols);

            var extraDamager = slash.transform.Find("Extra Damager");
            if (extraDamager) extraDamager.gameObject.SetActive(false);

            if (nailSlash == null)
                return;

            s_nailSlashHeroField?.SetValue(nailSlash, hc);

            var travel = slash.GetComponent<NailSlashTravel>();
            if (travel != null)
                s_nailTravelHeroField?.SetValue(travel, hc);

            ClearActivateOnSlash(nailSlash);
            StripHeroOnlyBehaviours(slash);

            float slashDir;
            Vector2 slashForward;
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
            else
            {
                slashDir = facing >= 0 ? 0f : 180f;
                slashForward = facing >= 0 ? Vector2.right : Vector2.left;
            }

            RetargetDamagers(slash, slashDir, slashForward, applyKnockbackCharm: nailPath);

            nailSlash.StartSlash();


            HookSlashDamage(slash, nailSlash, slashForward, applyRecoil: nailPath);

            // Failsafe against colliders outliving the animation.
            StartCoroutine(DisableSlashAfterWindow(slash, 0.3f));
        }

        /// <summary>
        /// Unsubscribes the clone's travel component from Hornet's sprite-flip event, which would
        /// otherwise re-orient a Shade slash every time she turns around.
        /// </summary>
        private static void DetachHeroFlipHandler(HeroController hc, GameObject slash)
        {
            var travel = slash.GetComponent<NailSlashTravel>();
            if (travel == null || s_travelOnHeroFlipped == null)
                return;

            hc.FlippedSprite -= (Action)Delegate.CreateDelegate(typeof(Action), travel, s_travelOnHeroFlipped);
        }

        /// <summary>Stops the slash's colliders from pushing Hornet or the Shade around.</summary>
        private void IgnoreCollisionsWithOwners(Collider2D[] slashCols)
        {
            if (hornetTransform != null)
            {
                foreach (var hornetCol in hornetTransform.GetComponentsInChildren<Collider2D>(true))
                    foreach (var sc in slashCols)
                        if (sc && hornetCol) Physics2D.IgnoreCollision(sc, hornetCol, true);
            }

            foreach (var shadeCol in GetComponentsInChildren<Collider2D>(true))
                foreach (var sc in slashCols)
                    if (sc && shadeCol) Physics2D.IgnoreCollision(sc, shadeCol, true);
        }

        /// <summary>
        /// Empties the clone's activateOnSlash chain, which would otherwise fire Hornet's own extra
        /// slashes whenever the Shade swings.
        /// </summary>
        private static void ClearActivateOnSlash(NailSlash nailSlash)
        {
            if (s_nailActivateOnSlashField?.GetValue(nailSlash) is not GameObject[] chain)
                return;

            foreach (var go in chain)
                if (go) go.SetActive(false);

            s_nailActivateOnSlashField.SetValue(nailSlash, Array.Empty<GameObject>());
        }

        /// <summary>
        /// Removes the components that only make sense on Hornet's own slash: recoil, terrain thunks,
        /// downspike bounce, and the extra-slash helpers that widen the hit window.
        /// </summary>
        private static void StripHeroOnlyBehaviours(GameObject slash)
        {
            DestroyAll(slash.GetComponentsInChildren<NailSlashRecoil>(true));
            DestroyAll(slash.GetComponentsInChildren<RecoilEnemiesToRadius>(true));
            DestroyAll(slash.GetComponentsInChildren<HeroExtraNailSlash>(true));
            DestroyAll(slash.GetComponentsInChildren<NailSlashTerrainThunk>(true));
            DestroyAll(slash.GetComponentsInChildren<HeroDownAttack>(true));
        }

        private static void DestroyAll<T>(T[] components) where T : Component
        {
            foreach (var component in components)
                if (component) Destroy(component);
        }

        /// <summary>
        /// Points the clone's damagers at enemies rather than at Hornet's systems, and fixes the
        /// damage to the Shade's own nail figure. Only the first damager stays enabled: the extras
        /// exist to multi-hit for Hornet and would multiply the Shade's damage.
        /// </summary>
        private void RetargetDamagers(GameObject slash, float slashDir, Vector2 slashForward, bool applyKnockbackCharm)
        {
            int nailDmg = GetShadeNailDamage();
            bool firstKept = false;

            foreach (var d in slash.GetComponentsInChildren<DamageEnemies>(true))
            {
                if (!d) continue;

                s_deSourceIsHero?.SetValue(d, false);
                s_deIsHeroDamage?.SetValue(d, false);
                s_deIsNailAttack?.SetValue(d, false);
                s_deAttackType?.SetValue(d, AttackTypes.Generic);
                s_deDirection?.SetValue(d, slashDir);
                s_deMoveDirection?.SetValue(d, false);
                s_deFlipDirectionIfBehind?.SetValue(d, false);
                s_deForwardVector?.SetValue(d, slashForward);
                s_deIgnoreNailPosition?.SetValue(d, true);
                s_deDoesNotGenerateSilk?.SetValue(d, true);
                s_deUseNailDamage?.SetValue(d, false);
                s_deDamageDealt?.SetValue(d, nailDmg);

                if (s_deSetOnlyDamageEnemies != null)
                    s_deSetOnlyDamageEnemies.Invoke(d, new object[] { false });
                else
                    s_deOnlyDamageEnemies?.SetValue(d, false);

                if (s_deSilkGeneration != null)
                    s_deSilkGeneration.SetValue(d, Enum.ToObject(s_deSilkGeneration.FieldType, 0));

                if (applyKnockbackCharm)
                    d.magnitudeMult = Mathf.Max(0.01f, d.magnitudeMult * charmNailKnockbackMultiplier);

                if (firstKept)
                    d.enabled = false;
                else
                    firstKept = true;
            }
        }

        /// <summary>
        /// Awards SOUL per enemy hit and tears the slash down when its damage window closes, so no
        /// collider outlives the swing.
        /// </summary>
        private void HookSlashDamage(GameObject slash, NailSlash nailSlash, Vector2 slashForward, bool applyRecoil)
        {
            var primaryDamager = nailSlash.EnemyDamager;
            if (primaryDamager == null)
                return;

            Vector2 recoilDirection = slashForward.sqrMagnitude > 0.001f
                ? slashForward.normalized
                : (facing >= 0 ? Vector2.right : Vector2.left);

            Action onDamaged = null;
            Action<bool> onEnded = null;

            onDamaged = () =>
            {
                int prevSoul = shadeSoul;
                shadeSoul = Mathf.Min(shadeSoulMax, shadeSoul + Mathf.Max(0, soulGainPerHit + charmSoulGainBonus));
                PushSoulToHud();
                CheckHazardOverlap();

                if (prevSoul < focusSoulCost && shadeSoul >= focusSoulCost)
                {
                    EnsureFocusSfx();
                    if (focusSfx != null && sfxFocusReady != null)
                        focusSfx.PlayOneShot(sfxFocusReady, Mathf.Clamp01(GetEffectiveSfxVolume()));
                }

                if (applyRecoil)
                    ApplyAttackRecoil(recoilDirection);
            };

            onEnded = _ =>
            {
                primaryDamager.DamagedEnemy -= onDamaged;
                nailSlash.EndedDamage -= onEnded;

                if (slash)
                {
                    DisableSlashHitboxes(slash);
                    slash.SetActive(false);
                    Destroy(slash);
                }
            };

            primaryDamager.DamagedEnemy += onDamaged;
            nailSlash.EndedDamage += onEnded;
        }

        private static void DisableSlashHitboxes(GameObject slash)
        {
            foreach (var de in slash.GetComponentsInChildren<DamageEnemies>(true))
                if (de) de.enabled = false;
            foreach (var col in slash.GetComponentsInChildren<Collider2D>(true))
                if (col) col.enabled = false;
        }

        /// <summary>
        /// Sizes and flips the clone to match the Shade rather than Hornet. The scale is written to
        /// NailAttackBase's cached copies as well, which is what the slash reads back mid-animation.
        /// </summary>
        private void ApplyBaseSlashOrientation(GameObject slash, NailSlash nailSlash, float verticalInput, bool invertDown, float facingForSlash)
        {
            if (!slash) return;

            var tr = slash.transform;
            var ls = tr.localScale;

            float usedFacing = facingForSlash != 0f ? facingForSlash : (facing >= 0 ? 1f : -1f);

            float scaleSign = -Mathf.Sign(usedFacing);
            if (verticalInput > 0.35f && usedFacing > 0f)
                scaleSign = 1f;

            ls.x = Mathf.Abs(ls.x) * scaleSign;
            ls *= charmNailScaleMultiplier / SpriteScale;

            if (invertDown)
            {
                ls.x = -ls.x;
                ls.y = -ls.y;
            }

            tr.localScale = ls;

            if (nailSlash != null)
            {
                s_nailSlashScaleField?.SetValue(nailSlash, ls);
                s_nailSlashLongScaleField?.SetValue(nailSlash, ls);
            }
        }

        private void DestroyOtherSlashes(GameObject keep)
        {
            foreach (var ns in transform.GetComponentsInChildren<NailSlash>(true))
            {
                if (!ns || (keep != null && ns.gameObject == keep)) continue;
                ns.gameObject.SetActive(false);
                Destroy(ns.gameObject);
            }
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
            if (slash) DisableSlashHitboxes(slash);
        }

        /// <summary>
        /// Moves a shaman slash off Hornet and onto the Shade, one frame after it was cloned so it
        /// measured its rects against the hierarchy it came from. The travel component's cached
        /// start pose is rewritten too: it was captured while the slash was still Hornet's.
        /// </summary>
        private IEnumerator AdoptSlashAfterFrame(GameObject slash)
        {
            yield return null;
            if (!slash) yield break;

            var tr = slash.transform;
            var marker = slash.GetComponent<ShadeSlashMarker>();

            float verticalInput = marker != null ? marker.verticalInput : 0f;
            bool invertDown = marker != null && marker.invertDown;
            float markerFacing = marker != null && marker.orientationFacing != 0f
                ? marker.orientationFacing
                : (facing >= 0 ? 1f : -1f);

            tr.SetParent(transform, false);
            tr.position = transform.position;
            tr.localPosition = Vector3.zero;

            var nailSlash = slash.GetComponent<NailSlash>();
            if (marker != null && marker.hasStoredScale)
            {
                tr.localScale = marker.storedLocalScale;
                if (nailSlash != null)
                {
                    s_nailSlashScaleField?.SetValue(nailSlash, marker.storedLocalScale);
                    s_nailSlashLongScaleField?.SetValue(nailSlash, marker.storedLocalScale);
                }
            }
            else
            {
                ApplyBaseSlashOrientation(slash, nailSlash, verticalInput, invertDown, markerFacing);
            }

            var travel = slash.GetComponent<NailSlashTravel>();
            if (travel != null)
            {
                s_nailTravelInitialPosField?.SetValue(travel, tr.localPosition);
                s_nailTravelInitialScaleField?.SetValue(travel, tr.localScale);

                if (invertDown && s_nailTravelDistanceField != null)
                {
                    var distance = (Vector2)s_nailTravelDistanceField.GetValue(travel);
                    distance.y = -distance.y;
                    s_nailTravelDistanceField.SetValue(travel, distance);
                }
            }

            LogSlashState("Shaman slash adopted", slash, this);
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

            var group = FindShamanConfigGroup(hc, config);
            if (group == null)
            {
                return false;
            }

            shamanHorizontalSlashTemplate = group.NormalSlashObject ?? group.AlternateSlashObject;
            shamanHorizontalAltSlashTemplate = group.AlternateSlashObject;
            shamanUpSlashTemplate = group.UpSlashObject ?? group.AltUpSlashObject ?? shamanHorizontalSlashTemplate ?? shamanHorizontalAltSlashTemplate;
            shamanDownSlashTemplate = group.DownSlashObject ?? group.AltDownSlashObject;

            shamanSlashConfigSource = config;
            return shamanHorizontalSlashTemplate != null || shamanHorizontalAltSlashTemplate != null || shamanUpSlashTemplate != null || shamanDownSlashTemplate != null;
        }

        private static HeroController.ConfigGroup FindShamanConfigGroup(HeroController hc, HeroControllerConfig config)
        {
            if (hc == null || config == null)
            {
                return null;
            }

            foreach (var field in s_heroConfigGroupFields)
            {
                if (field?.GetValue(hc) is not HeroController.ConfigGroup[] groups)
                {
                    continue;
                }

                foreach (var group in groups)
                {
                    if (group != null && group.Config == config)
                    {
                        return group;
                    }
                }
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
