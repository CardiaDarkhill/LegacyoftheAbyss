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
        private float gatheringSwarmTimer;

        /// <summary>Whether Vengeful Spirit is thrown as a cluster of flukes instead of one bolt.</summary>
        internal bool FlukenestActive => flukenestEquipped;

        internal void SetFlukenestEnabled(bool enabled) => flukenestEquipped = enabled;

        internal void SetSporeShroomEnabled(bool enabled) => sporeShroomEquipped = enabled;

        internal void SetGatheringSwarmEnabled(bool enabled)
        {
            gatheringSwarmEquipped = enabled;
            if (!enabled)
            {
                gatheringSwarmTimer = 0f;
            }
        }

        /// <summary>
        /// A short-lived damage volume centred on this companion. Shared by Defender's Crest's
        /// cloud, Thorns of Agony's retaliation and Spore Shroom's focus burst, which differ only
        /// in radius, damage and what triggers them.
        /// </summary>
        internal void SpawnCharmDamageBurst(float radius, int damage, float lifeSeconds)
        {
            if (damage <= 0 || radius <= 0f)
            {
                return;
            }

            var go = new GameObject("ShadeCharmBurst");
            go.transform.position = transform.position;

            var circle = go.AddComponent<CircleCollider2D>();
            circle.isTrigger = true;
            circle.radius = radius;

            var aoe = go.AddComponent<ShadeAoE>();
            aoe.hornetRoot = hornetTransform;
            aoe.lifeSeconds = lifeSeconds;
            aoe.sourceOverride = gameObject;
            aoe.ConfigureDamage(damage, applyDamageMultiplier: true);
        }

        /// <summary>Spore Shroom's cloud, released when a focus channel completes.</summary>
        internal void OnFocusCompletedCharmEffects()
        {
            if (sporeShroomEquipped)
            {
                SpawnCharmDamageBurst(radius: 3.4f, damage: 8, lifeSeconds: 2.5f);
            }
        }

        /// <summary>
        /// Gathering Swarm: pulls loose currency toward the bearer. Pickups are found by component
        /// rather than by name, and only ones already loose in the world are moved - this drags
        /// what was dropped within reach, it does not reach into containers.
        /// </summary>
        private void UpdateGatheringSwarm(float delta)
        {
            if (!gatheringSwarmEquipped)
            {
                return;
            }

            gatheringSwarmTimer -= delta;
            if (gatheringSwarmTimer > 0f)
            {
                return;
            }

            gatheringSwarmTimer = 0.1f;

            const float pullRadius = 7f;
            const float pullSpeed = 9f;

            var hits = Physics2D.OverlapCircleAll(transform.position, pullRadius, Physics2D.AllLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                var collider = hits[i];
                if (collider == null)
                {
                    continue;
                }

                var pickup = collider.GetComponentInParent<CurrencyObjectBase>();
                if (pickup == null)
                {
                    continue;
                }

                var pickupTransform = pickup.transform;
                pickupTransform.position = Vector3.MoveTowards(
                    pickupTransform.position,
                    transform.position,
                    pullSpeed * 0.1f);
            }
        }
    }
}
