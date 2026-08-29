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
                // Flukenest trades the single bolt for a spread: more total damage, but only if
                // enough of the cluster connects.
                const int flukeCount = 5;
                for (int i = 0; i < flukeCount; i++)
                {
                    float spread = Mathf.Lerp(-22f, 22f, flukeCount > 1 ? i / (float)(flukeCount - 1) : 0.5f);
                    SpawnProjectile(Quaternion.Euler(0f, 0f, spread) * dir, damageScale: 0.45f);
                }
            }
            else
            {
                SpawnProjectile(dir);
            }

            currentAnimFrames = null;
            isCastingSpell = false;
        }

        private IEnumerator ShriekCastRoutine()
        {
            isCastingSpell = true;
            bool upgraded = IsShriekUpgraded();
            int dmg = ComputeSpellDamageMultiplier(4f, upgraded);
            TryPlayShriekSfx(upgraded);
            float life = 0.18f;
            Vector2 localOffset = new Vector2(0f, 0.8f);
            SpawnShriekCone(12f, 95f, dmg, life, localOffset);
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
            int dmg = ComputeSpellDamageMultiplier(3f, upgraded); // Descending Dark base 3x
            TrackSpellCast(StartCoroutine(DescendingDarkRoutine(dmg, upgraded)));
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
        private int ComputeSpellDamageMultiplier(float baseMult, bool upgraded)
        {
            int nail = Mathf.Max(1, GetHornetNailDamage());
            float mult = upgraded ? baseMult : baseMult * 0.7f; // Soul variant = 30% less
            mult *= charmSpellDamageMultiplier;
            int dmg = Mathf.RoundToInt(nail * mult * ModConfig.Instance.shadeSpellDamageMultiplier);
            return Mathf.Max(1, dmg);
        }

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

        private void SpawnShriekCone(float height, float degrees, int damage, float lifeSeconds, Vector2 localOffset)
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

        private IEnumerator DescendingDarkRoutine(int totalDamage, bool upgraded)
        {
            isCastingSpell = true;
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

            // Spawn two hitboxes: ground strip (10 units wide), and teardrop (6x8) above
            TryPlayQuakeImpactSfx(upgraded);
            int half = Mathf.Max(1, Mathf.RoundToInt(totalDamage * 0.5f));
            SpawnQuakeImpact(groundY, half);
            SpawnQuakeTeardrop(groundY, half);
            if (upgraded)
            {
                SpawnGroundSlamFx(dDarkSlamAnimFrames, groundY);
                SpawnDarkBurstFx(groundY);
            }
            else
            {
                SpawnGroundSlamFx(dDiveSlamAnimFrames, groundY);
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
            box.size = new Vector2(10f, 1.0f);

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

        private void SpawnGroundSlamFx(Sprite[] frames, float groundY)
        {
            if (frames == null || frames.Length == 0 || sr == null) return;
            var go = new GameObject("ShadeQuakeSlamFx");
            var fxSr = go.AddComponent<SpriteRenderer>();
            fxSr.sortingLayerID = sr.sortingLayerID;
            fxSr.sortingOrder = sr.sortingOrder - 1;
            float desiredWidth = 15f;
            float spriteWidth = frames[0].bounds.size.x;
            float scale = desiredWidth / spriteWidth;
            go.transform.localScale = new Vector3(scale, scale, 1f);
            float height = frames[0].bounds.size.y * scale;
            go.transform.position = new Vector3(transform.position.x, groundY + height / 2f, transform.position.z);
            StartCoroutine(PlayAndDestroy(fxSr, frames, 0.05f));
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
