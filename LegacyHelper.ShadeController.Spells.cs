#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LegacyoftheAbyss.Shade;
using LegacyoftheAbyss.Shade.Knight;
using UnityEngine;
using GlobalEnums;

public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        private void EnsureFuryAura()
        {
            if (furyAuraPs)
                return;

            try
            {
                furyAuraObject = new GameObject("ShadeFuryAura");
                furyAuraObject.transform.SetParent(transform, false);
                furyAuraPs = furyAuraObject.AddComponent<ParticleSystem>();

                var main = furyAuraPs.main;
                main.loop = true;
                main.playOnAwake = false;
                main.startLifetime = new ParticleSystem.MinMaxCurve(0.66f, 1.14f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(0.45f, 1.35f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.22f, 0.38f);
                main.startColor = new Color(0.82f, 0.08f, 0.12f, 0.95f);
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
                main.maxParticles = 240;
                main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);

                var emission = furyAuraPs.emission;
                emission.enabled = true;
                emission.rateOverTime = new ParticleSystem.MinMaxCurve(56f, 72f);

                var shape = furyAuraPs.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = 0.15f;
                shape.radiusThickness = 0f;
                shape.arcMode = ParticleSystemShapeMultiModeValue.Random;
                shape.arc = 360f;
                shape.randomDirectionAmount = 1f;
                shape.alignToDirection = true;

                var velocity = furyAuraPs.velocityOverLifetime;
                velocity.enabled = true;
                velocity.radial = new ParticleSystem.MinMaxCurve(1.1f, 1.8f);
                velocity.orbitalZ = new ParticleSystem.MinMaxCurve(-0.35f, 0.35f);

                var color = furyAuraPs.colorOverLifetime;
                color.enabled = true;
                Gradient grad = new Gradient();
                grad.SetKeys(
                    new[]
                    {
                        new GradientColorKey(new Color(0.96f, 0.18f, 0.22f, 1f), 0f),
                        new GradientColorKey(new Color(0.35f, 0f, 0f, 1f), 1f)
                    },
                    new[]
                    {
                        new GradientAlphaKey(0.95f, 0f),
                        new GradientAlphaKey(0f, 1f)
                    });
                color.color = grad;

                var size = furyAuraPs.sizeOverLifetime;
                size.enabled = true;
                size.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                    new Keyframe(0f, 0.75f),
                    new Keyframe(0.5f, 1.05f),
                    new Keyframe(1f, 1.25f)));

                var renderer = furyAuraPs.GetComponent<ParticleSystemRenderer>();
                if (renderer)
                {
                    renderer.renderMode = ParticleSystemRenderMode.Billboard;
                    if (s_furyAuraMat == null)
                    {
                        var shader = Shader.Find("Particles/Additive");
                        if (!shader)
                        {
                            shader = Shader.Find("Sprites/Default");
                        }

                        s_furyAuraMat = shader != null
                            ? new Material(shader)
                            : new Material(Shader.Find("Sprites/Default"));
                        s_furyAuraMat.color = Color.white;
                    }
                    renderer.sharedMaterial = s_furyAuraMat;
                    renderer.sharedMaterial.mainTexture = MakeDotSprite().texture;
                    if (sr)
                    {
                        renderer.sortingLayerID = sr.sortingLayerID;
                        renderer.sortingOrder = sr.sortingOrder - 1;
                    }
                }

                furyAuraPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            catch
            {
                furyAuraPs = null;
            }
        }

        private void TryPlayDashSfx()
        {
            try
            {
                var hc = HeroController.instance;
                if (hc != null && hc.AudioCtrl != null)
                    hc.AudioCtrl.PlaySound(HeroSounds.DASH);
            }
            catch { }
        }

        /// <summary>The wind-up currently running, so a room change can stop it releasing.</summary>
        private Coroutine activeSpellCast;

        private void TrackSpellCast(Coroutine routine)
        {
            activeSpellCast = routine;
        }

        /// <summary>
        /// Drops a spell that is still winding up. Called when the Shade is moved between rooms:
        /// the controller is reused rather than rebuilt, so without this the coroutine picks up
        /// again in the new room and fires from wherever the body was set down.
        /// </summary>
        internal void CancelSpellCasts()
        {
            if (activeSpellCast != null)
            {
                StopCoroutine(activeSpellCast);
                activeSpellCast = null;
            }

            if (isCastingSpell)
            {
                isCastingSpell = false;
                currentAnimFrames = null;
            }

            EndKnightCastFreeze();
        }

        private void HandleFire()
        {
            fireTimer -= Time.deltaTime;
            if (!ShadeInput.IsActionHeld(ShadeAction.Fire) || fireTimer > 0f) return;
            // If aiming a spell with up/down, don't fire projectile
            if (ShadeInput.IsActionHeld(ShadeAction.MoveUp) || ShadeInput.IsActionHeld(ShadeAction.MoveDown)) return;
            if (!IsProjectileUnlocked()) return;
            if (shadeSoul < projectileSoulCost) return;
            fireTimer = fireCooldown;
            shadeSoul = Mathf.Max(0, shadeSoul - projectileSoulCost);
            PushSoulToHud();
            CheckHazardOverlap();

            // Recorded because "a Shade Soul fires by itself on room entry" has no other witness:
            // by the time it is noticed the input that caused it is a frame old. The fire value is
            // what decides this, so it is the first thing the event says.
            LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
                "shade-spell",
                "shade soul cast",
                FormattableString.Invariant(
                    $"fire={ShadeInput.GetActionValue(ShadeAction.Fire):F2} ai={LegacyoftheAbyss.Shade.Ai.ShadeAiInput.Active} at=({transform.position.x:F2}, {transform.position.y:F2}) facing={facing} sceneProtected={sceneProtectionActive} controlsLocked={hornetControlsLocked}"));

            TrackSpellCast(StartCoroutine(FireballCastRoutine()));
        }

        private void HandleShriek()
        {
            shriekTimer -= Time.deltaTime;
            // Trigger on Fire + Up
            if (!ShadeInput.WasActionPressed(ShadeAction.Fire) || !ShadeInput.IsActionHeld(ShadeAction.MoveUp)) return;
            if (!IsShriekUnlocked()) return; // locked until 3rd unlock
            if (shriekTimer > 0f) return;
            if (shadeSoul < shriekSoulCost) return;
            shriekTimer = shriekCooldown;
            shadeSoul = Mathf.Max(0, shadeSoul - shriekSoulCost);
            PushSoulToHud();
            CheckHazardOverlap();
            TrackSpellCast(StartCoroutine(ShriekCastRoutine()));
        }

        private IEnumerator FireballCastRoutine()
        {
            isCastingSpell = true;
            BeginKnightCastFreeze();
            PlayKnightSpellAnimation(
                IsProjectileUpgraded() ? KnightView.ClipFireballUpgraded : KnightView.ClipFireball,
                0.35f);
            if (fireballCastAnimFrames != null && fireballCastAnimFrames.Length > 0)
            {
                currentAnimFrames = fireballCastAnimFrames;
                animFrameIndex = 0;
                animTimer = 0f;
                float perFrame = 0.25f / fireballCastAnimFrames.Length;
                for (int i = 0; i < fireballCastAnimFrames.Length; i++)
                {
                    if (sr) sr.sprite = fireballCastAnimFrames[i];
                    yield return new WaitForSeconds(perFrame);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }

            Vector2 dir = new Vector2(facing, 0f);
            if (FlukenestActive)
            {
                SpawnFlukeCluster(dir);
            }
            else
            {
                SpawnProjectile(dir);
            }

            currentAnimFrames = null;
            isCastingSpell = false;
            EndKnightCastFreeze();
        }

        /// <summary>How many flukes the spell throws, by whether it has been upgraded.</summary>
        private const int FlukeCountVengefulSpirit = 9;

        private const int FlukeCountShadeSoul = 16;

        /// <summary>
        /// The middle of the range a fluke covers, in world units - about half the distance from a
        /// centred caster to the edge of the screen.
        /// </summary>
        private const float FlukeAverageRange = 7f;

        /// <summary>
        /// Gravity a fluke falls under, as a multiple of the scene's. They are lobbed rather than
        /// fired, which is what makes the cluster spread out over the ground in front of the caster
        /// instead of arriving as one line.
        /// </summary>
        private const float FlukeGravityScale = 2.5f;

        /// <summary>The arc the cluster is thrown through, measured from horizontal.</summary>
        private const float FlukeMinAngle = 8f;

        private const float FlukeMaxAngle = 52f;

        /// <summary>
        /// How far above the muzzle a fluke starts. The caster is usually standing on the floor and
        /// the muzzle sits near its feet, so a fluke launched from there met the ground on its first
        /// step and burst before it had travelled at all.
        /// </summary>
        private const float FlukeMuzzleLift = 0.55f;

        /// <summary>A hitbox the size of a fluke rather than of the bolt it replaces.</summary>
        private const float FlukeColliderRadius = 0.16f;

        /// <summary>
        /// Flukenest's cluster: a handful of flukes lobbed in an arc ahead of the caster.
        /// <para>
        /// Each carries a flat 4, or 5 under Shaman Stone - the charm's own numbers, which is why
        /// they follow neither the nail nor the spell progression. The count does follow the
        /// upgrade, and the launch speed is randomised so the cluster lands spread across roughly
        /// half to one and a half times its average range rather than all in one place.
        /// </para>
        /// </summary>
        private void SpawnFlukeCluster(Vector2 dir)
        {
            int flukeCount = IsProjectileUpgraded() ? FlukeCountShadeSoul : FlukeCountVengefulSpirit;
            int flukeDamage = shamanStoneEquipped ? 5 : 4;
            float facingSign = dir.x < 0f ? -1f : 1f;

            // Range goes as the square of the launch speed, so a +/-22% spread in speed is the
            // +/-50% spread in distance the cluster is meant to cover.
            float baseSpeed = Mathf.Sqrt(FlukeAverageRange * Mathf.Abs(Physics2D.gravity.y) * FlukeGravityScale);

            for (int i = 0; i < flukeCount; i++)
            {
                float t = flukeCount > 1 ? i / (float)(flukeCount - 1) : 0.5f;
                float angle = Mathf.Lerp(FlukeMinAngle, FlukeMaxAngle, t);
                float radians = angle * Mathf.Deg2Rad;
                Vector2 launch = new Vector2(Mathf.Cos(radians) * facingSign, Mathf.Sin(radians));

                SpawnProjectile(
                    launch,
                    fixedDamage: flukeDamage,
                    effectPrefab: LegacyoftheAbyss.Shade.Knight.KnightEffects.SpellFluke,
                    effectScale: 0.45f,
                    speedOverride: baseSpeed * UnityEngine.Random.Range(0.78f, 1.22f),
                    gravityScale: FlukeGravityScale,
                    destroyOnTerrain: true,
                    faceVelocity: true,
                    colliderRadius: FlukeColliderRadius,
                    muzzleLift: FlukeMuzzleLift);
            }
        }

        private IEnumerator ShriekCastRoutine()
        {
            isCastingSpell = true;
            BeginKnightCastFreeze();
            bool upgraded = IsShriekUpgraded();
            PlayKnightSpellAnimation(
                upgraded ? KnightView.ClipScreamUpgraded : KnightView.ClipScream,
                0.5f);
            int hits = upgraded ? ShadeSpellDamage.AbyssShriekHits : ShadeSpellDamage.HowlingWraithsHits;
            int dmg = SpellDamage(upgraded ? ShadeSpellDamage.AbyssShriek : ShadeSpellDamage.HowlingWraiths);
            LoggingManager.LogShadeSpellDamage(
                CharacterLogName,
                upgraded ? "Abyss Shriek" : "Howling Wraiths",
                FormattableString.Invariant($"{dmg} x {hits} hits"),
                dmg * hits);
            TryPlayShriekSfx(upgraded);

            // Standing long enough to land every burst rather than one: Hollow Knight's wraiths are
            // three hits in quick succession and the shriek is four, and a volume that hits once is
            // a third of the spell.
            Vector2 localOffset = new Vector2(0f, 0.8f);
            // A whole interval per hit, so the volume outlives the last of them by one tick and
            // cannot be destroyed a physics step before it lands - and dies before a further one
            // would be due, so the count is exact.
            float life = ShriekBurstInterval * hits;
            SpawnShriekCone(12f, 95f, dmg, life, localOffset, ShriekBurstInterval);
            SpawnShriekFx(upgraded);

            if (shriekCastAnimFrames != null && shriekCastAnimFrames.Length > 0)
            {
                currentAnimFrames = shriekCastAnimFrames;
                animFrameIndex = 0;
                animTimer = 0f;
                float perFrame = 0.25f / shriekCastAnimFrames.Length;
                float elapsed = 0f;
                while (elapsed < 0.25f)
                {
                    if (sr) sr.sprite = shriekCastAnimFrames[(int)(elapsed / perFrame) % shriekCastAnimFrames.Length];
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }
            currentAnimFrames = null;
            isCastingSpell = false;
        }

        private void HandleDescendingDark()
        {
            quakeTimer -= Time.deltaTime;
            // Trigger on Fire + Down
            if (!ShadeInput.WasActionPressed(ShadeAction.Fire) || !ShadeInput.IsActionHeld(ShadeAction.MoveDown)) return;
            if (!IsDescendingDarkUnlocked()) return; // locked until 2nd unlock
            if (quakeTimer > 0f) return;
            if (shadeSoul < quakeSoulCost) return;
            quakeTimer = quakeCooldown;
            shadeSoul = Mathf.Max(0, shadeSoul - quakeSoulCost);
            PushSoulToHud();
            CheckHazardOverlap();

            bool upgraded = IsDescendingDarkUpgraded();
            TryPlayQuakePrepareSfx();
            // Two volumes with two figures, as Hollow Knight has it: the dive itself and the
            // shockwave it throws out along the ground.
            int diveDamage = SpellDamage(upgraded ? ShadeSpellDamage.DescendingDarkImpact : ShadeSpellDamage.DesolateDiveImpact);
            int shockwaveDamage = SpellDamage(upgraded ? ShadeSpellDamage.DescendingDarkBursts : ShadeSpellDamage.DesolateDiveShockwave);
            LoggingManager.LogShadeSpellDamage(
                CharacterLogName,
                upgraded ? "Descending Dark" : "Desolate Dive",
                FormattableString.Invariant($"{diveDamage} dive + {shockwaveDamage} shockwave"),
                diveDamage + shockwaveDamage);
            TrackSpellCast(StartCoroutine(DescendingDarkRoutine(diveDamage, shockwaveDamage, upgraded)));
        }

        // Spell progression helpers.
        // The Shade counts Hornet's six spell flags into one 0-6 track. The Knight instead follows
        // Knight in Silksong's per-spell mapping, so each of its three spells has its own two gates
        // - see KnightAbilityMap.
        private int ShadeSpellProgress
        {
            get
            {
                try
                {
                    var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
                    if (pd != null)
                    {
                        int c = 0;
                        if (pd.hasNeedleThrow) c++;
                        if (pd.hasThreadSphere) c++;
                        if (pd.hasSilkCharge) c++;
                        if (pd.hasParry) c++;
                        if (pd.hasSilkBomb) c++;
                        if (pd.hasSilkBossNeedle) c++;
                        c = Mathf.Clamp(c, 0, 6);
                        ShadeRuntime.SyncSpellProgress(c);
                        return c;
                    }
                }
                catch
                {
                }

                return ShadeRuntime.PersistentState.SpellProgress;
            }
        }
        private bool IsProjectileUnlocked()
        {
            if (abilityOverrides.EnableProjectile.HasValue)
                return abilityOverrides.EnableProjectile.Value;
            if (UsesGroundedMovement)
                return knightAbilities.FireballLevel >= 1;
            return ShadeSpellProgress >= 1;
        }

        private bool IsDescendingDarkUnlocked()
        {
            if (abilityOverrides.EnableDescendingDark.HasValue)
                return abilityOverrides.EnableDescendingDark.Value;
            if (UsesGroundedMovement)
                return knightAbilities.QuakeLevel >= 1;
            return ShadeSpellProgress >= 2;
        }

        private bool IsShriekUnlocked()
        {
            if (abilityOverrides.EnableShriek.HasValue)
                return abilityOverrides.EnableShriek.Value;
            if (UsesGroundedMovement)
                return knightAbilities.ScreamLevel >= 1;
            return ShadeSpellProgress >= 3;
        }

        private bool IsProjectileUpgraded()
        {
            if (abilityOverrides.UpgradeProjectile.HasValue)
                return abilityOverrides.UpgradeProjectile.Value;
            if (UsesGroundedMovement)
                return knightAbilities.FireballLevel >= 2;
            return ShadeSpellProgress >= 4;
        }

        private bool IsDescendingDarkUpgraded()
        {
            if (abilityOverrides.UpgradeDescendingDark.HasValue)
                return abilityOverrides.UpgradeDescendingDark.Value;
            if (UsesGroundedMovement)
                return knightAbilities.QuakeLevel >= 2;
            return ShadeSpellProgress >= 5;
        }

        private bool IsShriekUpgraded()
        {
            if (abilityOverrides.UpgradeShriek.HasValue)
                return abilityOverrides.UpgradeShriek.Value;
            if (UsesGroundedMovement)
                return knightAbilities.ScreamLevel >= 2;
            return ShadeSpellProgress >= 6;
        }
        /// <summary>
        /// One piece of a spell, in damage. The figures are Hollow Knight's own and live in
        /// <see cref="ShadeSpellDamage"/>, which also says why they are used flat rather than
        /// scaled off Hornet's needle, and carries Shaman Stone's per-spell increase with each.
        /// </summary>
        private int SpellDamage(ShadeSpellDamage.SpellHit hit)
            => hit.Resolve(shamanStoneEquipped, ModConfig.Instance.shadeSpellDamageMultiplier);

        private void IgnoreHornetForCollider(Collider2D col)
        {
            try
            {
                if (!col || !hornetTransform) return;
                var hornetCols = hornetTransform.root.GetComponentsInChildren<Collider2D>(true);
                foreach (var hc in hornetCols)
                    if (hc) Physics2D.IgnoreCollision(col, hc, true);
            }
            catch { }
        }

        /// <summary>
        /// How close together the bursts of a shriek land. Long enough to outlast a physics step, so
        /// each one is a separate tick of the volume rather than the same frame counted twice.
        /// </summary>
        private const float ShriekBurstInterval = 0.07f;

        private void SpawnShriekCone(float height, float degrees, int damage, float lifeSeconds, Vector2 localOffset, float hitIntervalSeconds)
        {
            var go = new GameObject("ShadeShriekCone");
            go.transform.position = transform.position + (Vector3)localOffset;
            go.tag = "Hero Spell";
            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) go.layer = spellLayer; else if (atkLayer >= 0) go.layer = atkLayer;

            var poly = go.AddComponent<PolygonCollider2D>();
            poly.isTrigger = true;
            // Build wedge polygon with apex at (0,0) and arc up
            int segments = 8;
            float half = degrees * 0.5f;
            List<Vector2> pts = new List<Vector2>();
            pts.Add(Vector2.zero);
            for (int i = 0; i <= segments; i++)
            {
                float a = Mathf.Lerp(-half, half, i / (float)segments);
                float ang = (90f + a) * Mathf.Deg2Rad; // around up axis
                Vector2 p = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * height;
                pts.Add(p);
            }
            poly.SetPath(0, pts.ToArray());

            var aoe = go.AddComponent<ShadeAoE>();
            aoe.ConfigureDamage(damage, applyDamageMultiplier: false);
            aoe.hornetRoot = hornetTransform;
            aoe.lifeSeconds = lifeSeconds;
            aoe.hitIntervalSeconds = hitIntervalSeconds;

            IgnoreHornetForCollider(poly);
        }

        private void SpawnShriekFx(bool upgraded)
        {
            var frames = upgraded ? abyssShriekAnimFrames : howlingWraithsAnimFrames;
            if (frames == null || frames.Length == 0 || sr == null) return;
            var go = new GameObject(upgraded ? "AbyssShriekFx" : "HowlingWraithsFx");
            go.transform.position = transform.position;
            go.layer = gameObject.layer;
            var fxSr = go.AddComponent<SpriteRenderer>();
            fxSr.sortingLayerID = sr.sortingLayerID;
            fxSr.sortingOrder = sr.sortingOrder - 1;
            float fxScale = SpriteScale * 3f;
            go.transform.localScale = Vector3.one * fxScale;
            StartCoroutine(PlayShriekFx(fxSr, frames, fxScale));
        }

        private IEnumerator PlayShriekFx(SpriteRenderer fxSr, Sprite[] frames, float fxScale)
        {
            if (fxSr == null || frames == null || frames.Length == 0) yield break;
            float shadeBottom = transform.position.y;
            if (sr && sr.sprite)
                shadeBottom -= sr.sprite.bounds.extents.y * SpriteScale;
            float fxExt = frames[0].bounds.extents.y * fxScale;
            var pos = fxSr.transform.position;
            pos.y = shadeBottom + fxExt;
            fxSr.transform.position = pos;

            float duration = 0.4f;
            float per = duration / frames.Length;
            float t = 0f;
            fxSr.sprite = frames[0];
            while (t < duration)
            {
                int idx = Mathf.Min((int)(t / per), frames.Length - 1);
                fxSr.sprite = frames[idx];
                t += Time.deltaTime;
                yield return null;
            }
            Destroy(fxSr.gameObject);
        }

        private IEnumerator DescendingDarkRoutine(int diveDamage, int shockwaveDamage, bool upgraded)
        {
            isCastingSpell = true;

            // Released again the moment the dive itself starts, below. This spell carries its own
            // movement, so freezing it for the whole cast would fight the descent rather than set
            // it up.
            BeginKnightCastFreeze();
            PlayKnightSpellAnimation(KnightView.ClipQuakeAntic, 0.35f);
            float prevVelY = rb ? rb.linearVelocity.y : 0f;
            if (rb) rb.linearVelocity = Vector2.zero;
            if (quakeCastAnimFrames != null && quakeCastAnimFrames.Length > 0)
            {
                currentAnimFrames = quakeCastAnimFrames;
                animFrameIndex = 0;
                animTimer = 0f;
                float perFrame = 0.25f / quakeCastAnimFrames.Length;
                for (int i = 0; i < quakeCastAnimFrames.Length; i++)
                {
                    if (sr) sr.sprite = quakeCastAnimFrames[i];
                    yield return new WaitForSeconds(perFrame);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }

            // Enemy i-frames during descent (not hazards)
            hurtCooldown = Mathf.Max(hurtCooldown, 0.6f);

            if (descendAnimFrames != null && descendAnimFrames.Length > 0 && sr)
            {
                currentAnimFrames = descendAnimFrames;
                animFrameIndex = 0;
                animTimer = 0f;
                sr.sprite = descendAnimFrames[0];
            }

            PlayKnightSpellAnimation(
                upgraded ? KnightView.ClipQuakeFallUpgraded : KnightView.ClipQuakeFall,
                1f);
            EndKnightCastFreeze();
            var aura = SpawnDescendAura();

            // Find ground below: ignore Hornet/enemy/hazard hitboxes so we only stop on terrain
            Vector2 start = transform.position;
            float maxDist = 60f;
            var hits = Physics2D.RaycastAll(start, Vector2.down, maxDist);
            RaycastHit2D? pick = null;
            Transform hornetRoot = hornetTransform ? hornetTransform.root : null;
            foreach (var h in hits)
            {
                if (!h.collider) continue;
                if (h.collider.isTrigger) continue;
                var ht = h.collider.transform;
                // ignore self (any part of the shade hierarchy)
                if (ht == transform || ht.IsChildOf(transform) || transform.IsChildOf(ht)) continue;
                // ignore hornet using hierarchy checks
                if (hornetTransform)
                {
                    if (ht == hornetTransform || ht.IsChildOf(hornetTransform) || (hornetRoot && ht.root == hornetRoot))
                        continue;
                }
                // also skip any collider attached to Hornet via HeroController component
                try
                {
                    if (h.collider.GetComponentInParent<HeroController>() != null)
                        continue;
                }
                catch { }
                if (h.collider.name == "Hero Physics Pusher")
                    continue;
                if (h.collider.CompareTag("Player"))
                    continue;
                // ignore enemies/hazards (anything that damages hero)
                try { if (h.collider.GetComponentInParent<DamageHero>() != null) continue; } catch { }
                // otherwise this is acceptable ground
                pick = h;
                if (ModConfig.Instance.logShade)
                    UnityEngine.Debug.Log($"[ShadeDebug] Descending Dark ground hit {h.collider.name} tag={h.collider.tag} layer={h.collider.gameObject.layer}");
                break;
            }

            Vector3 targetPos = transform.position + Vector3.down * 8f; // fallback
            float groundY = targetPos.y;
            float extY = bodyCol ? bodyCol.bounds.extents.y : 0.7f;
            if (pick.HasValue)
            {
                groundY = pick.Value.point.y;
                targetPos = new Vector3(transform.position.x, groundY + extY + 0.02f, transform.position.z);
            }

            // Quick drop over 0.12s
            float dropTime = 0.12f;
            Vector3 from = transform.position;
            float elapsed = 0f;
            float descTimer = 0f;
            float descFrame = 0.05f;
            while (elapsed < dropTime)
            {
                elapsed += Time.deltaTime;
                float u = Mathf.Clamp01(elapsed / dropTime);
                Vector3 p = Vector3.Lerp(from, targetPos, u*u); // ease in
                TeleportToPosition(p);
                if (descendAnimFrames != null && descendAnimFrames.Length > 0 && sr)
                {
                    descTimer += Time.deltaTime;
                    if (descTimer >= descFrame)
                    {
                        descTimer -= descFrame;
                        animFrameIndex = (animFrameIndex + 1) % descendAnimFrames.Length;
                        sr.sprite = descendAnimFrames[animFrameIndex];
                    }
                }
                yield return null;
            }
            TeleportToPosition(targetPos);
            if (aura) Destroy(aura);

            // If landing area is a hazard, skip the impact
            if (IsHazardAtPosition(new Vector2(targetPos.x, groundY + 0.2f), 0.8f))
            {
                currentAnimFrames = null;
                isCastingSpell = false;
                yield break;
            }

            PlayKnightSpellAnimation(
                upgraded ? KnightView.ClipQuakeLandUpgraded : KnightView.ClipQuakeLand,
                0.45f);

            // Spawn two hitboxes: ground strip (10 units wide), and teardrop (6x8) above
            TryPlayQuakeImpactSfx(upgraded);

            // The strip along the ground is the shockwave; the column above the landing is the dive.
            SpawnQuakeImpact(groundY, shockwaveDamage);
            SpawnQuakeTeardrop(groundY, diveDamage);
            if (upgraded)
            {
                SpawnGroundSlamFx(dDarkSlamAnimFrames, groundY, DarkSlamArtFraction);
                SpawnDarkBurstFx(groundY);
            }
            else
            {
                SpawnGroundSlamFx(dDiveSlamAnimFrames, groundY, DiveSlamArtFraction);
            }

            // Small delay to keep i-frames briefly after impact
            yield return new WaitForSeconds(0.1f);
            if (rb) rb.linearVelocity = new Vector2(rb.linearVelocity.x, prevVelY);
            currentAnimFrames = null;
            isCastingSpell = false;
        }

        private bool IsHazardAtPosition(Vector2 pos, float radius)
        {
            try
            {
                var hits = Physics2D.OverlapCircleAll(pos, radius, ~0, -Mathf.Infinity, Mathf.Infinity);
                Transform hornetRoot = hornetTransform ? hornetTransform.root : null;
                foreach (var c in hits)
                {
                    if (!c) continue;
                    if (c.transform == transform || c.transform.IsChildOf(transform)) continue;
                    if (hornetRoot && c.transform.root == hornetRoot) continue;
                    var dh = c.GetComponentInParent<DamageHero>();
                    if (dh != null)
                    {
                        var hz = GetHazardType(dh);
                        if (IsTerrainHazard(hz)) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private void SpawnQuakeImpact(float groundY, int damage)
        {
            var go = new GameObject("ShadeQuakeStrip");
            go.transform.position = new Vector3(transform.position.x, groundY + 0.5f, transform.position.z);
            go.tag = "Hero Spell";
            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) go.layer = spellLayer; else if (atkLayer >= 0) go.layer = atkLayer;

            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(QuakeImpactWidth, 1.0f);

            var aoe = go.AddComponent<ShadeAoE>();
            aoe.ConfigureDamage(damage, applyDamageMultiplier: false);
            aoe.hornetRoot = hornetTransform;
            aoe.lifeSeconds = 0.25f;

            IgnoreHornetForCollider(box);
        }

        private void SpawnQuakeTeardrop(float groundY, int damage)
        {
            var go = new GameObject("ShadeQuakeTear");
            // Place center so bottom is at ground contact near the shade's position.
            float height = 8f; float width = 6f;
            float centerY = groundY + (height * 0.5f);
            go.transform.position = new Vector3(transform.position.x, centerY, transform.position.z);
            go.tag = "Hero Spell";
            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) go.layer = spellLayer; else if (atkLayer >= 0) go.layer = atkLayer;

            var cap = go.AddComponent<CapsuleCollider2D>();
            cap.isTrigger = true;
            cap.direction = CapsuleDirection2D.Vertical;
            cap.size = new Vector2(width, height);

            var aoe = go.AddComponent<ShadeAoE>();
            aoe.ConfigureDamage(damage, applyDamageMultiplier: false);
            aoe.hornetRoot = hornetTransform;
            aoe.lifeSeconds = 0.25f;

            IgnoreHornetForCollider(cap);
        }

        private GameObject SpawnDescendAura()
        {
            if (descendAuraAnimFrames == null || descendAuraAnimFrames.Length == 0 || sr == null) return null;
            var go = new GameObject("ShadeDescendAura");
            go.transform.SetParent(transform, false);
            var auraSr = go.AddComponent<SpriteRenderer>();
            auraSr.sortingLayerID = sr.sortingLayerID;
            auraSr.sortingOrder = sr.sortingOrder - 1;
            StartCoroutine(PlayDescendAura(auraSr, descendAuraAnimFrames));
            return go;
        }

        private IEnumerator PlayDescendAura(SpriteRenderer auraSr, Sprite[] frames)
        {
            if (auraSr == null || frames == null || frames.Length == 0) yield break;
            int idx = 0; float timer = 0f; float frameTime = 0.05f;
            while (auraSr)
            {
                timer += Time.deltaTime;
                if (timer >= frameTime)
                {
                    timer -= frameTime;
                    idx = (idx + 1) % frames.Length;
                }
                var frame = frames[idx];
                auraSr.sprite = frame;
                if (sr) auraSr.flipX = sr.flipX;
                float shadeHeight = sr && sr.sprite ? sr.sprite.bounds.size.y * SpriteScale : 0f;
                float auraHeight = frame.bounds.size.y * SpriteScale;
                float auraBottom = -shadeHeight * 0.5f - auraHeight * 0.1f;
                auraSr.transform.localScale = Vector3.one * SpriteScale;
                auraSr.transform.localPosition = new Vector3(0f, auraBottom + auraHeight * 0.5f, 0f);
                yield return null;
            }
        }

        /// <summary>How wide the impact is, as damage and as drawing. One number, so they agree.</summary>
        private const float QuakeImpactWidth = 10f;

        /// <summary>
        /// How long the slam is drawn for, however many frames its sheet holds.
        /// <para>
        /// Per frame rather than in total is what the two sheets had before, and they are not the
        /// same length: Descending Dark's six frames ran for 0.3s while Desolate Dive's two ran for
        /// 0.1, which on a dark background is barely a flicker.
        /// </para>
        /// </summary>
        private const float SlamFxSeconds = 0.3f;

        /// <summary>
        /// What share of its sprite cell each slam sheet's art actually fills, measured off the
        /// PNGs. Needed because the textures are marked non-readable at runtime, so it cannot be
        /// worked out there - and because the two differ enormously: Desolate Dive's burst covers
        /// 153 pixels of a 520-wide cell, where Descending Dark's covers 334 of 510.
        /// </summary>
        private const float DiveSlamArtFraction = 153f / 520f;

        private const float DarkSlamArtFraction = 334f / 510f;

        /// <summary>
        /// The slam's burst, drawn to span the strip it damages.
        /// <para>
        /// Sized by the art rather than by the sprite cell, which is not the same thing and is why
        /// the two spells did not look alike. A single cell width drew Desolate Dive's burst about
        /// four units across against Descending Dark's ten - small, dark, on a dark background, for
        /// a tenth of a second - which is why the Dive shockwave was reported as never appearing.
        /// </para>
        /// </summary>
        private void SpawnGroundSlamFx(Sprite[] frames, float groundY, float artFraction)
        {
            if (frames == null || frames.Length == 0 || sr == null) return;
            var go = new GameObject("ShadeQuakeSlamFx");
            var fxSr = go.AddComponent<SpriteRenderer>();
            fxSr.sortingLayerID = sr.sortingLayerID;
            fxSr.sortingOrder = sr.sortingOrder - 1;

            float cellWidth = frames[0].bounds.size.x;
            float artWidth = cellWidth * Mathf.Clamp(artFraction, 0.05f, 1f);
            if (artWidth <= 0.0001f) return;

            float scale = QuakeImpactWidth / artWidth;
            go.transform.localScale = new Vector3(scale, scale, 1f);
            float height = frames[0].bounds.size.y * scale;
            go.transform.position = new Vector3(transform.position.x, groundY + height / 2f, transform.position.z);
            StartCoroutine(PlayAndDestroy(fxSr, frames, SlamFxSeconds / frames.Length));
        }

        private void SpawnDarkBurstFx(float groundY)
        {
            if (dDarkBurstAnimFrames == null || dDarkBurstAnimFrames.Length == 0 || sr == null) return;
            var go = new GameObject("ShadeDDarkBurstFx");
            var fxSr = go.AddComponent<SpriteRenderer>();
            fxSr.sortingLayerID = sr.sortingLayerID;
            fxSr.sortingOrder = sr.sortingOrder - 1;
            float desiredHeight = 12f;
            float spriteHeight = dDarkBurstAnimFrames[0].bounds.size.y;
            float scale = desiredHeight / spriteHeight;
            go.transform.localScale = new Vector3(scale, scale, 1f);
            float height = spriteHeight * scale;
            go.transform.position = new Vector3(transform.position.x, groundY + height / 2f, transform.position.z);
            StartCoroutine(PlayAndDestroy(fxSr, dDarkBurstAnimFrames, 0.05f));
        }

        private IEnumerator PlayAndDestroy(SpriteRenderer rend, Sprite[] frames, float perFrame)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                if (rend) rend.sprite = frames[i];
                yield return new WaitForSeconds(perFrame);
            }
            if (rend) Destroy(rend.gameObject);
        }

    }
}
#nullable restore
