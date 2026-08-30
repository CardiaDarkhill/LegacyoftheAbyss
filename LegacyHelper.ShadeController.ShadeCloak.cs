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

                // On its own effect object, not the body: the rig shares one clip library across
                // every animator in it, so playing an effect clip on the body would draw the burst
                // as the Knight rather than beside it.
                knightView?.FlashShadeCloakReady();
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

        /// <summary>The borrowed Knight rig the Shade wears for a Sharp Shadow dash, or null.</summary>
        private KnightView sharpShadowShadeView;

        /// <summary>Set once the bundle has been asked for and refused, so it is not asked again.</summary>
        private bool sharpShadowShadeViewUnavailable;

        private bool sharpShadowFormActive;

        /// <summary>
        /// Draws the Shade as the sharpened cloak form for the length of a Sharp Shadow dash.
        /// <para>
        /// The clip is the Knight's own body animation, so wearing it means swapping the Shade's
        /// sprite sheets for the bundled rig and swapping back afterwards. That reads correctly
        /// because the animation is itself a transformation - the body sharpens into the dash and
        /// returns - rather than a pose the two characters would have to share.
        /// </para>
        /// <para>
        /// Shade only. The Knight already draws this clip through the rig it is.
        /// </para>
        /// </summary>
        private void UpdateSharpShadowShadeForm()
        {
            if (UsesGroundedMovement)
            {
                return;
            }

            bool wanted = IsSharpShadowDashing() && sharpShadowShadeView != null;

            if (wanted)
            {
                if (!sharpShadowFormActive)
                {
                    sharpShadowFormActive = true;
                    if (sr != null)
                    {
                        sr.enabled = false;
                    }

                    sharpShadowShadeView.SetVisible(true);
                    sharpShadowShadeView.Play(KnightView.ClipShadeCloakSharp, restart: true);
                }

                sharpShadowShadeView.SetFacing(facing);
                if (bodyCol != null)
                {
                    sharpShadowShadeView.AlignFeetTo(bodyCol.bounds.min.y);
                }

                return;
            }

            if (sharpShadowFormActive)
            {
                sharpShadowFormActive = false;
                sharpShadowShadeView?.SetVisible(false);
                if (sr != null)
                {
                    sr.enabled = true;
                }
            }
        }

        /// <summary>
        /// Builds the borrowed rig, hidden, when a Shade equips Sharp Shadow.
        /// <para>
        /// Done on equip rather than on the first dash because the Knight bundle is about 54 MB and
        /// loading it is not something to do in the frame a dash starts. Equipping happens at a
        /// bench, which is the right place to pay for it.
        /// </para>
        /// </summary>
        private void EnsureSharpShadowShadeView()
        {
            if (UsesGroundedMovement || sharpShadowShadeView != null || sharpShadowShadeViewUnavailable)
            {
                return;
            }

            var view = KnightView.Attach(gameObject);
            if (view == null)
            {
                sharpShadowShadeViewUnavailable = true;
                LegacyHelper.LogWarning("Sharp Shadow: the Knight bundle did not load, so the Shade keeps its ordinary cloak animation.");
                return;
            }

            if (sr != null)
            {
                view.ApplySorting(sr.sortingLayerID, sr.sortingOrder);
            }

            view.SetVisible(false);
            sharpShadowShadeView = view;
        }

        /// <summary>Puts the Shade back in its own sheets and drops the borrowed rig.</summary>
        private void DiscardSharpShadowShadeView()
        {
            if (sharpShadowFormActive)
            {
                sharpShadowFormActive = false;
                if (sr != null)
                {
                    sr.enabled = true;
                }
            }

            if (sharpShadowShadeView != null)
            {
                Destroy(sharpShadowShadeView);
                sharpShadowShadeView = null;
            }

            sharpShadowShadeViewUnavailable = false;
        }
    }
}
