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
            float effectClipFps = 20f)
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
                LegacyoftheAbyss.Shade.Knight.KnightEffects.TrySpawnSorted(
                    effectPrefab, go.transform, sr, effectScale, sortingOffset: 1, alpha: effectAlpha);
            }
            else if (!string.IsNullOrEmpty(effectClip))
            {
                var drawn = LegacyoftheAbyss.Shade.Knight.KnightEffects.TrySpawnAnimatedClip(
                    effectClip, go.transform, sr, effectScale, effectAlpha);
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
                radius: 3.4f,
                damage: SporeShroomTickDamage,
                lifeSeconds: SporeShroomCloudSeconds,
                hitIntervalSeconds: SporeShroomTickSeconds,
                effectPrefab: LegacyoftheAbyss.Shade.Knight.KnightEffects.SporeCloud);
        }

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
