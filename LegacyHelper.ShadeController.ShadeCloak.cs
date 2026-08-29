#nullable disable
using UnityEngine;
using LegacyoftheAbyss.Shade;
using LegacyoftheAbyss.Shade.Knight;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        private float shadeCloakCooldownTimer;

        /// <summary>Seconds of the inward "absorb" flourish left to play once the cooldown ends.</summary>
        private float shadeCloakAbsorbTimer;

        private const float ShadeCloakAbsorbSeconds = 0.35f;

        /// <summary>
        /// Whether this companion has Shade Cloak at all. Gated on Hornet's Harpoon Dash for both
        /// characters, so a Shade that has reached that point wears the same cooldown tell as the
        /// Knight rather than the two behaving differently.
        /// </summary>
        internal bool ShadeCloakUnlocked
        {
            get
            {
                if (UsesGroundedMovement)
                {
                    return knightAbilities.ShadeCloak;
                }

                var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
                return pd != null && pd.hasHarpoonDash;
            }
        }

        internal bool ShadeCloakOnCooldown => shadeCloakCooldownTimer > 0f;

        internal void BeginShadeCloakCooldown()
        {
            shadeCloakCooldownTimer = ShadeCloakCooldownSeconds;
            shadeCloakAbsorbTimer = 0f;
        }

        /// <summary>
        /// Runs the cooldown and hands the shadow particles their cue.
        /// <para>
        /// Once Shade Cloak is unlocked the wisps stop reporting SOUL and start reporting the
        /// cooldown, the way Hollow Knight's do: full while it recharges, then drawn back into the
        /// body at the moment it is ready. That inward pull is the tell - a fade would read as the
        /// effect merely ending rather than as the ability returning.
        /// </para>
        /// </summary>
        private void UpdateShadeCloakCooldown(float deltaTime)
        {
            if (shadeCloakCooldownTimer <= 0f)
            {
                if (shadeCloakAbsorbTimer > 0f)
                {
                    shadeCloakAbsorbTimer = Mathf.Max(0f, shadeCloakAbsorbTimer - deltaTime);
                }

                return;
            }

            shadeCloakCooldownTimer = Mathf.Max(0f, shadeCloakCooldownTimer - deltaTime);
            if (shadeCloakCooldownTimer <= 0f)
            {
                shadeCloakAbsorbTimer = ShadeCloakAbsorbSeconds;
                PlayShadeCloakReadyFlourish();

                // The bundle ships a "Shadow Recharge" clip for exactly this moment.
                knightView?.Play(KnightView.ClipShadeCloakReady, restart: true);
            }
        }

        /// <summary>
        /// Whether the shadow wisps should be showing at all, and at what strength.
        /// <para>
        /// Before Shade Cloak they track SOUL, as they always have. After it they are the cooldown
        /// readout instead, and run at full strength rather than scaling - a half-strength "not
        /// ready" reads as ambiguous.
        /// </para>
        /// </summary>
        private bool TryGetShadeCloakParticleDrive(out float fraction)
        {
            fraction = 0f;
            if (!ShadeCloakUnlocked)
            {
                return false;
            }

            if (shadeCloakCooldownTimer > 0f)
            {
                fraction = 1f;
                return true;
            }

            // Held through the absorb so the wisps have something to be pulled from.
            if (shadeCloakAbsorbTimer > 0f)
            {
                fraction = 1f;
                return true;
            }

            // Ready: no wisps at all. Their presence is the whole signal, so a resting level would
            // make "recharging" and "ready" look the same.
            return true;
        }

        /// <summary>Pulls the wisps already in the air back inward and stops making more.</summary>
        private void PlayShadeCloakReadyFlourish()
        {
            if (shadowParticlePs == null)
            {
                return;
            }

            var emission = shadowParticlePs.emission;
            emission.rateOverTime = 0f;

            // The wisps normally drift upward off the body. Pulling them radially inward and
            // reversing that drift drags what is already in the air back in, which is what makes
            // this read as the ability returning rather than the effect merely stopping.
            var velocity = shadowParticlePs.velocityOverLifetime;
            velocity.enabled = true;
            velocity.radial = new ParticleSystem.MinMaxCurve(-ShadeCloakAbsorbPull, -ShadeCloakAbsorbPull * 0.6f);
            velocity.y = new ParticleSystem.MinMaxCurve(-ShadeCloakAbsorbPull * 0.25f, 0f);
        }

        private const float ShadeCloakAbsorbPull = 6f;
    }
}
