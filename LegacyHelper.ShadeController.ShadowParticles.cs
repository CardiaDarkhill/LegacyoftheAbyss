#nullable disable
using LegacyoftheAbyss.Shade;
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        /// <summary>Base tint of a wisp before the SOUL-driven alpha lift. Near-black with a faint violet cast.</summary>
        private static readonly Color ShadowWispColor = new Color(0.05f, 0.03f, 0.08f, 0.62f);

        private const float ShadowWispMinSize = 0.20f;
        private const float ShadowWispMaxSize = 0.46f;

        // Spawn box, in the Shade's local units (its transform runs at SpriteScale, 1.5). Sized a
        // little larger than the body collider (0.9 x 1.4) so wisps appear around the silhouette as
        // well as over it.
        private const float ShadowSpawnWidth = 1.15f;
        private const float ShadowSpawnHeight = 1.6f;

        /// <summary>
        /// The black wisps around the Shade, modelled on Hollow Knight 1's Shade smoke: they do not
        /// stream outward from a point, they appear scattered across the body and then drift lazily
        /// upward with a slow side-to-side wander. Density and opacity ride the Shade's current
        /// SOUL - clearly present on an empty meter, twice the emission rate on a full one (see
        /// <see cref="ShadeVisualTuning.EmissionRate"/>).
        /// </summary>
        private void EnsureShadowParticles()
        {
            if (shadowParticlePs)
            {
                return;
            }

            try
            {
                shadowParticleObject = new GameObject("ShadeShadowAura");
                shadowParticleObject.transform.SetParent(transform, false);
                shadowParticleObject.transform.localPosition = Vector3.zero;
                shadowParticlePs = shadowParticleObject.AddComponent<ParticleSystem>();

                var main = shadowParticlePs.main;
                main.loop = true;
                main.playOnAwake = false;
                // Long lives: a wisp needs time on screen for the drift to read as lazy rather than
                // as a puff of exhaust.
                main.startLifetime = new ParticleSystem.MinMaxCurve(0.9f, 1.9f);
                // Zero: all motion comes from velocityOverLifetime and noise below. Any start speed
                // at all points every particle away from its spawn point, which is exactly the
                // "flowing out of the Shade" look this is avoiding.
                main.startSpeed = 0f;
                main.startSize = new ParticleSystem.MinMaxCurve(ShadowWispMinSize, ShadowWispMaxSize);
                main.startColor = ShadowWispColor;
                main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
                // World space is what lets the wisps hang where they were shed while the Shade
                // floats on. Teleports clear it (see ClearShadowParticles) so the smoke cannot
                // streak across a room.
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                // The rise is a constant drift, not an acceleration - see velocityOverLifetime.
                main.gravityModifier = 0f;
                main.maxParticles = 220;

                var emission = shadowParticlePs.emission;
                emission.enabled = true;
                emission.rateOverTime = ShadeVisualTuning.BaseEmissionRate;

                // A box roughly the size of the Shade, so wisps materialise scattered across and
                // just outside the silhouette instead of all issuing from its centre. Local units:
                // the Shade's transform runs at SpriteScale (1.5), which scales this with it.
                var shape = shadowParticlePs.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = new Vector3(ShadowSpawnWidth, ShadowSpawnHeight, 0.01f);
                shape.position = Vector3.zero;
                shape.randomDirectionAmount = 0f;

                var velocity = shadowParticlePs.velocityOverLifetime;
                velocity.enabled = true;
                // World space so the drift rate stays absolute rather than picking up the Shade's
                // 1.5x transform scale.
                velocity.space = ParticleSystemSimulationSpace.World;
                velocity.x = 0f;
                velocity.y = new ParticleSystem.MinMaxCurve(0.25f, 0.6f);
                velocity.z = 0f;

                var color = shadowParticlePs.colorOverLifetime;
                color.enabled = true;
                var gradient = new Gradient();
                gradient.SetKeys(
                    new[]
                    {
                        new GradientColorKey(Color.white, 0f),
                        new GradientColorKey(Color.white, 1f)
                    },
                    new[]
                    {
                        new GradientAlphaKey(0f, 0f),
                        new GradientAlphaKey(1f, 0.25f),
                        new GradientAlphaKey(0.7f, 0.65f),
                        new GradientAlphaKey(0f, 1f)
                    });
                // colorOverLifetime multiplies startColor, so the white keys above leave the tint
                // alone and only shape the fade in/out. Fading in over the first quarter also means
                // wisps materialise rather than popping in at full opacity, which matters more now
                // that they appear across the body instead of at a single point.
                color.color = gradient;

                var size = shadowParticlePs.sizeOverLifetime;
                size.enabled = true;
                size.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                    new Keyframe(0f, 0.55f),
                    new Keyframe(0.35f, 1f),
                    new Keyframe(1f, 0.3f)));

                // The side-to-side wander. Separate axes so it is genuinely horizontal drift rather
                // than an all-directions jitter fighting the rise above: X gets several times the
                // strength of Y. Low frequency with damping off gives slow wide sweeps instead of a
                // fast shimmer.
                var noise = shadowParticlePs.noise;
                noise.enabled = true;
                noise.separateAxes = true;
                noise.strengthX = new ParticleSystem.MinMaxCurve(0.3f, 0.55f);
                noise.strengthY = new ParticleSystem.MinMaxCurve(0.04f, 0.12f);
                noise.strengthZ = 0f;
                noise.frequency = 0.3f;
                noise.scrollSpeed = 0.25f;
                noise.damping = false;

                shadowParticleRenderer = shadowParticlePs.GetComponent<ParticleSystemRenderer>();
                if (shadowParticleRenderer)
                {
                    shadowParticleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
                    shadowParticleRenderer.sharedMaterial = EnsureShadowWispMaterial();
                    shadowParticleRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    shadowParticleRenderer.receiveShadows = false;
                }

                SyncShadowParticleSorting();
                shadowParticlePs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            catch
            {
                if (shadowParticleObject)
                {
                    Destroy(shadowParticleObject);
                }

                shadowParticleObject = null;
                shadowParticlePs = null;
                shadowParticleRenderer = null;
            }
        }

        /// <summary>Soft round smoke puff, shared by every Shade instance for the lifetime of the process.</summary>
        private static Material EnsureShadowWispMaterial()
        {
            if (s_shadowWispMat != null)
            {
                return s_shadowWispMat;
            }

            if (s_shadowWispTex == null)
            {
                const int size = 32;
                var tex = new Texture2D(size, size, TextureFormat.ARGB32, false)
                {
                    name = "ShadeShadowWispTex",
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp,
                    hideFlags = HideFlags.HideAndDontSave
                };

                float center = (size - 1) * 0.5f;
                float radius = center;
                var pixels = new Color32[size * size];
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float dx = x - center;
                        float dy = y - center;
                        float distance = Mathf.Sqrt(dx * dx + dy * dy) / radius;
                        // Squared falloff keeps a dense core with a long soft skirt, which is what
                        // makes overlapping particles read as smoke rather than as a cluster of dots.
                        float alpha = Mathf.Clamp01(1f - distance);
                        alpha *= alpha;
                        pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                    }
                }

                tex.SetPixels32(pixels);
                tex.Apply();
                s_shadowWispTex = tex;
            }

            var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Transparent");
            s_shadowWispMat = new Material(shader)
            {
                name = "ShadeShadowWispMat",
                mainTexture = s_shadowWispTex,
                hideFlags = HideFlags.HideAndDontSave
            };

            return s_shadowWispMat;
        }

        private void SyncShadowParticleSorting()
        {
            if (!shadowParticleRenderer || !sr)
            {
                return;
            }

            try
            {
                shadowParticleRenderer.sortingLayerID = sr.sortingLayerID;
                shadowParticleRenderer.sortingOrder = sr.sortingOrder - 1;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Drops the live wisps without stopping the emitter. Called on teleports so the world-space
        /// trail does not stretch from wherever the Shade was to wherever it reappeared.
        /// </summary>
        private void ClearShadowParticles()
        {
            try
            {
                shadowParticlePs?.Clear(true);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Per-frame upkeep: creates the emitter on first use, keeps its density in step with the
        /// Shade's SOUL, and stops it when the feature is off or the Shade is dormant.
        /// </summary>
        private void UpdateShadowParticles()
        {
            var config = ModConfig.Instance;
            bool wanted = config.shadeShadowParticlesEnabled
                && config.shadeShadowParticleIntensity > 0f
                && !isInactive
                && !isDying;

            if (!wanted)
            {
                if (shadowParticlePs && shadowParticlePs.isEmitting)
                {
                    // StopEmitting rather than Clear: whatever is already in the air fades out
                    // naturally instead of vanishing mid-frame.
                    shadowParticlePs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }

                return;
            }

            EnsureShadowParticles();
            if (!shadowParticlePs)
            {
                return;
            }

            float target = ShadeVisualTuning.SoulFraction(shadeSoul, shadeSoulMax);
            shadowSoulFraction = Mathf.MoveTowards(
                shadowSoulFraction,
                target,
                ShadeVisualTuning.IntensitySlewPerSecond * Time.deltaTime);

            float intensity = config.shadeShadowParticleIntensity;
            if (!Mathf.Approximately(shadowSoulFraction, appliedShadowSoulFraction)
                || !Mathf.Approximately(intensity, appliedShadowIntensity))
            {
                appliedShadowSoulFraction = shadowSoulFraction;
                appliedShadowIntensity = intensity;

                try
                {
                    var emission = shadowParticlePs.emission;
                    emission.rateOverTime = ShadeVisualTuning.EmissionRate(shadowSoulFraction, intensity);

                    var main = shadowParticlePs.main;
                    var tint = ShadowWispColor;
                    tint.a = Mathf.Clamp01(tint.a * ShadeVisualTuning.AlphaScale(shadowSoulFraction, intensity));
                    main.startColor = tint;

                    float sizeScale = ShadeVisualTuning.SizeScale(shadowSoulFraction);
                    main.startSize = new ParticleSystem.MinMaxCurve(ShadowWispMinSize * sizeScale, ShadowWispMaxSize * sizeScale);
                }
                catch
                {
                }
            }

            if (!shadowParticlePs.isEmitting)
            {
                try
                {
                    shadowParticlePs.Play();
                }
                catch
                {
                }
            }
        }
    }
}
#nullable restore
