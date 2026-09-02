#nullable disable
using UnityEngine;
using LegacyoftheAbyss.Shade;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        private bool flukenestEquipped;
        private bool sporeShroomEquipped;
        private bool gatheringSwarmEquipped;

        /// <summary>Whether Vengeful Spirit is thrown as a cluster of flukes instead of one bolt.</summary>
        internal bool FlukenestActive => flukenestEquipped;

        internal void SetFlukenestEnabled(bool enabled) => flukenestEquipped = enabled;

        internal void SetSporeShroomEnabled(bool enabled)
        {
            sporeShroomEquipped = enabled;
            if (!enabled)
            {
                sporeShroomCooldown = 0f;
            }
        }

        /// <summary>Read by the rosary magnet patch; see CurrencyObjectBase_MagnetToolIsEquipped_GatheringSwarm.</summary>
        internal bool HasGatheringSwarm => gatheringSwarmEquipped;

        internal void SetGatheringSwarmEnabled(bool enabled)
        {
            gatheringSwarmEquipped = enabled;
        }

        /// <summary>
        /// A short-lived damage volume centred on this companion. Shared by Defender's Crest's
        /// cloud, Thorns of Agony's retaliation and Spore Shroom's focus burst, which differ only
        /// in radius, damage and what triggers them.
        /// </summary>
        internal void SpawnCharmDamageBurst(
            float radius,
            int damage,
            float lifeSeconds,
            float hitIntervalSeconds = 0f,
            bool applyDamageMultiplier = true,
            string effectPrefab = null,
            float effectScale = 1f,
            float effectAlpha = 1f,
            string effectClip = null,
            bool clipReplacesBody = false)
        {
            if (damage <= 0 || radius <= 0f)
            {
                return;
            }

            var go = new GameObject("ShadeCharmBurst");
            go.transform.position = transform.position;

            // The cloud or burst this charm is supposed to look like. Left behind on the volume
            // rather than parented to the companion, because the volume is what stands still while
            // the bearer walks out of it - which is the whole behaviour of a dropped cloud.
            if (!string.IsNullOrEmpty(effectPrefab))
            {
                var drawnCloud = LegacyoftheAbyss.Shade.Knight.KnightEffects.TrySpawnSorted(
                    effectPrefab, go.transform, sr, effectScale, sortingOffset: 1, alpha: effectAlpha);

                // Drawn at the size it damages. The two were independent, and Spore Shroom's cloud
                // covered several times the circle below it - so an enemy plainly inside the cloud
                // took nothing, which is the only honest reading of that picture.
                LegacyoftheAbyss.Shade.Knight.KnightEffects.ScaleCloudToRadius(drawnCloud, effectPrefab, radius);
            }
            else if (!string.IsNullOrEmpty(effectClip))
            {
                // A clip that *is* the companion's body is matched to it and takes its place for
                // as long as it plays; anything else is drawn where the volume is and left alone.
                Bounds? body = null;
                if (clipReplacesBody && TryGetCompanionRenderedBounds(out var measured))
                {
                    body = measured;
                }

                var drawn = LegacyoftheAbyss.Shade.Knight.KnightEffects.TrySpawnAnimatedClip(
                    effectClip, go.transform, sr, effectScale, effectAlpha, fitToBody: body);

                if (drawn != null && clipReplacesBody)
                {
                    HideCompanionForClip(lifeSeconds);
                }
                if (drawn == null)
                {
                    LegacyHelper.LogWarning($"Charm burst wanted the '{effectClip}' animation and got nothing; the effect will be invisible.");
                }
            }

            var circle = go.AddComponent<CircleCollider2D>();
            circle.isTrigger = true;
            circle.radius = radius;

            var aoe = go.AddComponent<ShadeAoE>();
            aoe.hornetRoot = hornetTransform;
            aoe.lifeSeconds = lifeSeconds;
            aoe.sourceOverride = gameObject;
            aoe.hitIntervalSeconds = hitIntervalSeconds;
            aoe.ConfigureDamage(damage, applyDamageMultiplier);
        }

        /// <summary>
        /// The companion's drawn extent - the Knight's rig where it has one, its sprite otherwise.
        /// Bounds rather than the transform, because neither character's origin is where it draws.
        /// </summary>
        private bool TryGetCompanionRenderedBounds(out Bounds bounds)
        {
            bounds = default;

            if (UsesGroundedMovement && knightView != null && knightView.TryGetRenderedBounds(out bounds))
            {
                return bounds.size.y > 0.0001f;
            }

            if (sr != null)
            {
                bounds = sr.bounds;
                return bounds.size.y > 0.0001f;
            }

            return false;
        }

        /// <summary>Until when the companion's own body is stood down for a clip that replaces it.</summary>
        private float bodyHiddenForClipUntil;

        /// <summary>
        /// Stands the companion's own body down while a clip that contains it is playing.
        /// <para>
        /// Thorns of Agony is the Knight's body bursting into thorns, so drawing it over the real
        /// one puts two Knights on screen. The stagger from the hit that triggered it already holds
        /// the controls and the invulnerability, so this only has to take care of the drawing - and
        /// is extended to cover the clip in case the stagger is the shorter of the two.
        /// </para>
        /// </summary>
        private void HideCompanionForClip(float seconds)
        {
            bodyHiddenForClipUntil = Mathf.Max(bodyHiddenForClipUntil, Time.time + seconds);
            damageStaggerTimer = Mathf.Max(damageStaggerTimer, seconds);
        }

        /// <summary>Whether a clip is currently standing in for the companion's own body.</summary>
        private bool BodyHiddenForClip => Time.time < bodyHiddenForClipUntil;

        /// <summary>Exactly one nail slash of damage, for the charms specified in those terms.</summary>
        internal int NailSlashDamage => GetShadeNailDamage();

        /// <summary>Seconds of Spore Shroom cooldown left; a fresh cloud waits for this to run out.</summary>
        private float sporeShroomCooldown;

        /// <summary>
        /// Spore Shroom's cloud, released when a focus channel completes.
        /// <para>
        /// A lingering cloud rather than one burst: ~26 damage spread over its 4.1s, which is what
        /// it should have been doing all along - a single volume hits each foe once however long it
        /// stands, so the whole cloud landed as one hit the instant it appeared.
        /// </para>
        /// </summary>
        internal void OnFocusCompletedCharmEffects()
        {
            if (!sporeShroomEquipped || sporeShroomCooldown > 0f)
            {
                return;
            }

            sporeShroomCooldown = SporeShroomCooldownSeconds;
            SpawnCharmDamageBurst(
                radius: SporeShroomRadius,
                damage: SporeShroomTickDamage,
                lifeSeconds: SporeShroomCloudSeconds,
                hitIntervalSeconds: SporeShroomTickSeconds,
                effectPrefab: LegacyoftheAbyss.Shade.Knight.KnightEffects.SporeCloud);
        }

        /// <summary>
        /// Hollow Knight's own cloud radius, read from the prefab rather than chosen: the
        /// <c>Knight Spore Cloud</c> prefab carries a <c>CircleCollider2D</c> of 6.06 at a prefab
        /// scale of 1.35, so 8.18 units. The drawing is scaled to whatever this says, so taking the
        /// figure from the same place the art comes from leaves the two identical and the cloud at
        /// its authored size.
        /// </summary>
        private static float SporeShroomRadius
        {
            get
            {
                float authored = LegacyoftheAbyss.Shade.Knight.KnightEffects.TryGetPrefabRadius(
                    LegacyoftheAbyss.Shade.Knight.KnightEffects.SporeCloud);

                // Only if the bundle is there to be asked. Without it the cloud is invisible anyway,
                // and the old figure is a better guess than nothing.
                return authored > 0.01f ? authored : SporeShroomFallbackRadius;
            }
        }

        /// <summary>What the cloud measured before the prefab was asked, for a run without the bundle.</summary>
        private const float SporeShroomFallbackRadius = 3.4f;

        /// <summary>The cloud stands this long, ticking every <see cref="SporeShroomTickSeconds"/>.</summary>
        private const float SporeShroomCloudSeconds = 4.1f;

        private const float SporeShroomTickSeconds = 0.3f;

        /// <summary>13 ticks across the cloud's life, for the ~26 total the charm is specified at.</summary>
        private const int SporeShroomTickDamage = 2;

        private const float SporeShroomCooldownSeconds = 4.25f;

        /// <summary>
        /// Runs the Spore Shroom cooldown down. Taking a hit clears it outright, which is the
        /// charm's own rule: the cloud is available again either after the wait or after a hit.
        /// </summary>
        private void UpdateSporeShroomCooldown(float delta)
        {
            if (sporeShroomCooldown > 0f)
            {
                sporeShroomCooldown = Mathf.Max(0f, sporeShroomCooldown - delta);
            }
        }

        internal void ClearSporeShroomCooldown()
        {
            sporeShroomCooldown = 0f;
        }

    }
}
