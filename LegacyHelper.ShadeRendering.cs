#nullable disable
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LegacyoftheAbyss.Shade;
using UnityEngine;

public partial class LegacyHelper
{
    /// <summary>
    /// Clone of Hornet's sprite material handed to the Shade, so the game's character shader
    /// (scene darkness, <c>_CharacterTintColor</c>, the <c>IS_CHARACTER</c> keyword
    /// <see cref="CharacterTint.CanAdd"/> gates on) applies to it as well. A clone rather than the
    /// shared instance: Hornet's own material carries per-frame flash/black-thread state that must
    /// not follow the Shade around.
    /// </summary>
    private static Material shadeSpriteMaterial;

    /// <summary>The Hornet material <see cref="shadeSpriteMaterial"/> was cloned from, to spot a swap.</summary>
    private static Material shadeSpriteMaterialSource;

    /// <summary>
    /// The material Unity gave the Shade's <see cref="SpriteRenderer"/> when it was created
    /// (Sprites/Default), kept so turning <c>shadeUseHornetMaterial</c> back off restores it.
    /// </summary>
    private static Material shadeDefaultSpriteMaterial;

    private static bool loggedShadeMaterialUnavailable;
    private static bool loggedHeroLightUnavailable;

    /// <summary>Set when the clone itself came out unusable. Unlike a missing hero light, that is
    /// not a timing problem, so it stops the per-frame retry rather than repeating forever.</summary>
    private static bool shadeLightCloneFailed;

    /// <summary>
    /// Hornet's body renderer. Silksong draws the hero as a sprite <i>mesh</i>, not a
    /// <see cref="SpriteRenderer"/>, so this must not fall back to a child search: a
    /// <c>GetComponentInChildren&lt;SpriteRenderer&gt;</c> here returns the hero vignette, which
    /// sits on the "Vignette" sorting layer above fog and every weather layer.
    /// </summary>
    internal static Renderer ResolveHornetBodyRenderer(HeroController hero)
    {
        if (hero == null)
        {
            return null;
        }

        var mesh = hero.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            return mesh;
        }

        return hero.GetComponent<SpriteRenderer>();
    }

    private static readonly FieldInfo HeroLightSpriteRendererField = AccessTools.Field(typeof(HeroLight), "spriteRenderer");

    /// <summary>
    /// Hornet's light sprite. Scene darkness is a shader cutout fed by a camera that renders this
    /// object, so it - not any sprite of our own - is what actually removes darkness.
    /// <para>
    /// It is <b>not</b> on the <see cref="HeroLight"/> component's own GameObject; the component
    /// points at it through a serialised field. `GameManager.heroLight` resolves it with a
    /// `GetComponent` on the component's object and therefore reads null on the shipped build,
    /// which is exactly how this returned null every frame for a whole session.
    /// </para>
    /// </summary>
    internal static SpriteRenderer ResolveHeroLight()
    {
        var hero = ResolveHeroController();
        var light = hero != null ? hero.heroLight : null;
        if (light == null)
        {
            return null;
        }

        if (HeroLightSpriteRendererField?.GetValue(light) is SpriteRenderer resolved && resolved != null)
        {
            return resolved;
        }

        return light.GetComponentInChildren<SpriteRenderer>(true);
    }

    /// <summary>
    /// The layers the darkness camera renders. Only objects on these feed <c>_DarknessCutout</c>,
    /// so only a copy of one of them can cut darkness anywhere new.
    /// </summary>
    internal static int ResolveDarknessMask()
    {
        var effect = UnityEngine.Object.FindFirstObjectByType<DarknessCameraEffect>();
        var camera = effect != null ? effect.GetComponent<Camera>() : null;
        return camera != null ? camera.cullingMask : 0;
    }

    private static HeroController ResolveHeroController()
    {
        var gm = GameManager.instance;
        if (gm != null && gm.hero_ctrl != null)
        {
            return gm.hero_ctrl;
        }

        return HeroController.instance;
    }

    private static bool SortingLayerExists(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        foreach (var layer in SortingLayer.layers)
        {
            if (string.Equals(layer.name, name, System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Puts <paramref name="sr"/> on the Shade's configured sorting layer and gives it Hornet's
    /// material. Safe to call repeatedly - it is re-run on every spawn and on every scene
    /// transition, which is what picks up a config edit made between launches.
    /// </summary>
    internal static void ApplyShadeSpriteRendering(SpriteRenderer sr)
    {
        if (sr == null)
        {
            return;
        }

        var config = ModConfig.Instance;
        shadeDefaultSpriteMaterial ??= sr.sharedMaterial;

        var hero = ResolveHeroController();
        var heroRenderer = ResolveHornetBodyRenderer(hero);

        string heroLayerName = heroRenderer != null ? SortingLayer.IDToName(heroRenderer.sortingLayerID) : null;
        int heroOrder = heroRenderer != null ? heroRenderer.sortingOrder : 0;

        string layerName = ShadeVisualTuning.ResolveSortingLayerName(config.shadeSortingLayer, SortingLayerExists, heroLayerName);
        bool sharesHeroLayer = string.Equals(layerName, heroLayerName, System.StringComparison.Ordinal);

        sr.sortingLayerName = layerName;
        sr.sortingOrder = ShadeVisualTuning.ResolveSortingOrder(heroOrder, config.shadeSortingOrderOffset, sharesHeroLayer);

        if (config.logShade)
        {
            string heroDescription = heroRenderer != null
                ? $"{heroRenderer.GetType().Name} on '{heroLayerName}' order {heroOrder}"
                : "unresolved";
            LogInfo($"Shade rendering: layer '{layerName}' order {sr.sortingOrder} (Hornet: {heroDescription}).");
        }

        ApplyShadeSpriteMaterial(sr, heroRenderer, config);
    }

    private static void ApplyShadeSpriteMaterial(SpriteRenderer sr, Renderer heroRenderer, ModConfig config)
    {
        Material resolved = config.shadeUseHornetMaterial ? ResolveShadeSpriteMaterial(heroRenderer) : null;
        sr.sharedMaterial = resolved ?? shadeDefaultSpriteMaterial;
    }

    private static Material ResolveShadeSpriteMaterial(Renderer heroRenderer)
    {
        Material source = heroRenderer != null ? heroRenderer.sharedMaterial : null;

        // A SpriteRenderer feeds its sprite's texture through _MainTex, so a material without that
        // property would render the Shade as a flat block. Anything failing this check is left on
        // Unity's default sprite material rather than risking an invisible companion.
        bool usable = source != null
            && source.shader != null
            && source.shader.name != "Hidden/InternalErrorShader"
            && source.HasProperty("_MainTex");

        if (!usable)
        {
            if (!loggedShadeMaterialUnavailable)
            {
                loggedShadeMaterialUnavailable = true;
                LogWarning("Could not resolve Hornet's sprite material for the Shade; falling back to the default sprite material.");
            }

            return null;
        }

        if (shadeSpriteMaterial != null && shadeSpriteMaterialSource == source)
        {
            return shadeSpriteMaterial;
        }

        var previous = shadeSpriteMaterial;
        shadeSpriteMaterial = new Material(source) { name = "ShadeSpriteMaterial" };
        shadeSpriteMaterialSource = source;
        if (previous != null)
        {
            UnityEngine.Object.Destroy(previous);
        }

        return shadeSpriteMaterial;
    }

    public partial class ShadeController
    {
        /// <summary>
        /// Re-derives the Shade's sorting layer/order and material, then re-hangs every child
        /// renderer off the result. The children are all positioned relative to the body sprite,
        /// so they only need re-sorting when the body itself moves layer.
        /// </summary>
        internal void ApplyRenderingSettings()
        {
            if (!sr)
            {
                sr = GetComponent<SpriteRenderer>();
            }

            if (!sr)
            {
                return;
            }

            LegacyHelper.ApplyShadeSpriteRendering(sr);

            // The Knight's rig sorts separately and had been given its order once, when it was
            // built - so a re-resolve here left the two of them a step apart for the rest of the
            // session.
            RefreshKnightSorting();

            SyncChildRendererSorting();
            EnsureShadeLight();
        }

        /// <summary>
        /// Everything that decides whether the companion draws in front of something Hornet is
        /// behind: both of their sorting layers, their orders, and their depths.
        /// <para>
        /// Written for the bug report because a screenshot cannot distinguish the three things that
        /// could cause it. A different sorting layer, a different order within a shared one, or - if
        /// those two agree, which they now deliberately do - a difference in z, which is all Unity
        /// has left to sort by. Two reports of the companion standing on the wrong side of the same
        /// clump of grass have each been answered with a guess; this is so the third is not.
        /// </para>
        /// </summary>
        internal string DescribeSorting()
        {
            try
            {
                var parts = new System.Collections.Generic.List<string>();

                var hero = ResolveHeroController();
                var heroRenderer = LegacyHelper.ResolveHornetBodyRenderer(hero);
                if (heroRenderer != null)
                {
                    parts.Add(FormattableString.Invariant(
                        $"hornet: {heroRenderer.GetType().Name} '{SortingLayer.IDToName(heroRenderer.sortingLayerID)}' order {heroRenderer.sortingOrder} z {heroRenderer.transform.position.z:0.####}"));
                }
                else
                {
                    parts.Add("hornet: unresolved");
                }

                if (sr != null)
                {
                    parts.Add(FormattableString.Invariant(
                        $"body: '{SortingLayer.IDToName(sr.sortingLayerID)}' order {sr.sortingOrder} z {transform.position.z:0.####} drawn {sr.enabled}"));
                }

                if (knightView != null)
                {
                    var rigRenderer = knightView.FirstRenderer;
                    if (rigRenderer != null)
                    {
                        parts.Add(FormattableString.Invariant(
                            $"knight rig: '{SortingLayer.IDToName(rigRenderer.sortingLayerID)}' order {rigRenderer.sortingOrder} z {rigRenderer.transform.position.z:0.####}"));
                    }
                }

                var config = ModConfig.Instance;
                if (config != null)
                {
                    parts.Add(FormattableString.Invariant(
                        $"config: layer '{config.shadeSortingLayer}' offset {config.shadeSortingOrderOffset}"));
                }

                return string.Join(" | ", parts.ToArray());
            }
            catch (Exception e)
            {
                return "unreadable: " + e.Message;
            }
        }

        private void SyncChildRendererSorting()
        {
            if (!sr)
            {
                return;
            }

            int layer = sr.sortingLayerID;
            int order = sr.sortingOrder;

            if (inactivePulseSr)
            {
                inactivePulseSr.sortingLayerID = layer;
                inactivePulseSr.sortingOrder = order - 1;
            }

            if (baldurShellRenderer)
            {
                baldurShellRenderer.sortingLayerID = layer;
                baldurShellRenderer.sortingOrder = order + 1;
            }

            if (furyAuraPs)
            {
                var furyRenderer = furyAuraPs.GetComponent<ParticleSystemRenderer>();
                if (furyRenderer)
                {
                    furyRenderer.sortingLayerID = layer;
                    furyRenderer.sortingOrder = order - 1;
                }
            }

            if (focusAuraRenderer)
            {
                focusAuraRenderer.sortingLayerID = layer;
                focusAuraRenderer.sortingOrder = order - 2;
            }

            // Deliberately not the shade light: its layer and sorting are Hornet's, cloned, and
            // that membership is what puts it in the darkness camera's pass.
            SyncShadowParticleSorting();
        }

        /// <summary>
        /// Creates the Shade's light if it does not have one yet and syncs it either way. Called
        /// every frame, because Hornet's light is not necessarily resolvable at the moment the
        /// Shade spawns and there is otherwise nothing to retry on. Turning
        /// <c>shadeLightEnabled</c> off destroys the clone here too.
        /// </summary>
        internal void EnsureShadeLight()
        {
            if (!ModConfig.Instance.shadeLightEnabled)
            {
                DestroyShadeLight();
                return;
            }

            if (shadeLightRenderers.Length == 0 && !shadeLightCloneFailed)
            {
                CreateShadeLight();
            }

            SyncShadeLight();
        }

        /// <summary>
        /// Copies Hornet's light onto the Shade.
        /// <para>
        /// Two separate things make her visible in a dark room and both are cloned: the glow around
        /// her (<c>HeroLight</c>, a blend-mode sprite whose donut half draws on the "Over" sorting
        /// layer, above the darkness vignette), and anything of hers on a layer the darkness camera
        /// renders, which is what feeds the <c>_DarknessCutout</c> texture the darkness shader reads.
        /// </para>
        /// </summary>
        private void CreateShadeLight()
        {
            var source = LegacyHelper.ResolveHeroLight();
            if (source == null)
            {
                if (!loggedHeroLightUnavailable)
                {
                    loggedHeroLightUnavailable = true;
                    LogWarning("Hornet's hero light could not be resolved; the Shade will not light dark rooms.");
                }

                return;
            }

            var roots = new List<Transform>();
            var baseScales = new List<Vector3>();
            var sourceRenderers = new List<SpriteRenderer>();
            var cloneRenderers = new List<SpriteRenderer>();

            CloneLightPart(source.gameObject, "ShadeHeroLight", roots, baseScales, sourceRenderers, cloneRenderers);

            // Whatever of Hornet's the darkness camera renders is the cutout source, and a copy of
            // it at the Shade is what cuts darkness there. Without this the Shade carries a glow
            // that draws over the darkness but never lifts it.
            int darknessMask = LegacyHelper.ResolveDarknessMask();
            var hero = LegacyHelper.ResolveHeroController();
            if (darknessMask != 0 && hero != null)
            {
                foreach (var candidate in hero.GetComponentsInChildren<Renderer>(true))
                {
                    if (candidate == null || (darknessMask & (1 << candidate.gameObject.layer)) == 0)
                    {
                        continue;
                    }

                    if (candidate.transform.IsChildOf(source.transform))
                    {
                        continue;
                    }

                    CloneLightPart(candidate.gameObject, "ShadeDarknessCutout", roots, baseScales, sourceRenderers, cloneRenderers);
                }
            }

            if (cloneRenderers.Count == 0)
            {
                shadeLightCloneFailed = true;
                LogWarning("Hero light clone produced no renderers; the Shade will not light dark rooms.");
                DestroyShadeLight();
                return;
            }

            shadeLightRoots = roots.ToArray();
            shadeLightRootBaseScales = baseScales.ToArray();
            shadeLightSourceRenderers = sourceRenderers.ToArray();
            shadeLightRenderers = cloneRenderers.ToArray();

            if (ModConfig.Instance.logShade)
            {
                LogInfo(FormattableString.Invariant(
                    $"Shade light cloned from '{source.name}': {shadeLightRoots.Length} part(s), {shadeLightRenderers.Length} renderer(s)."));
            }
        }

        /// <summary>
        /// Clones one object of Hornet's onto the Shade, stripped to its renderers and rescaled so
        /// it matches the original in world size before the radius multiplier is applied - the
        /// Shade carries its own sprite scale, which the clone would otherwise inherit on top.
        /// </summary>
        private void CloneLightPart(
            GameObject sourceObject,
            string name,
            List<Transform> roots,
            List<Vector3> baseScales,
            List<SpriteRenderer> sourceRenderers,
            List<SpriteRenderer> cloneRenderers)
        {
            var originals = sourceObject.GetComponentsInChildren<SpriteRenderer>(true);
            if (originals.Length == 0)
            {
                return;
            }

            var clone = UnityEngine.Object.Instantiate(sourceObject, transform);
            clone.name = name;

            var root = clone.transform;
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.identity;

            // Anything scripted riding along would be driving Hornet's light from the Shade.
            foreach (var behaviour in clone.GetComponentsInChildren<MonoBehaviour>(true))
            {
                UnityEngine.Object.Destroy(behaviour);
            }

            // Instantiate preserves hierarchy order, so index i in the clone is index i in the
            // source, whichever of them is the light body and whichever is the donut.
            var copies = clone.GetComponentsInChildren<SpriteRenderer>(true);
            if (copies.Length != originals.Length)
            {
                LogWarning(FormattableString.Invariant(
                    $"Hero light part '{name}' cloned {copies.Length} renderers from {originals.Length}; skipping it."));
                UnityEngine.Object.Destroy(clone);
                return;
            }

            // Hornet's light is masked to Hornet. A copy of it at the Shade sits outside that mask
            // and would be culled entirely, which looks exactly like a light that does nothing.
            foreach (var copy in copies)
            {
                copy.maskInteraction = SpriteMaskInteraction.None;
            }

            Vector3 sourceScale = sourceObject.transform.lossyScale;
            Vector3 shadeScale = transform.lossyScale;
            roots.Add(root);
            baseScales.Add(new Vector3(
                SafeScaleRatio(sourceScale.x, shadeScale.x),
                SafeScaleRatio(sourceScale.y, shadeScale.y),
                1f));

            sourceRenderers.AddRange(originals);
            cloneRenderers.AddRange(copies);
        }

        private static float SafeScaleRatio(float source, float parent)
        {
            float divisor = Mathf.Abs(parent);
            return divisor > 0.0001f ? Mathf.Abs(source) / divisor : Mathf.Abs(source);
        }

        /// <summary>
        /// Per-frame colour, size and visibility. Every renderer mirrors its opposite number on
        /// Hornet rather than caching anything: per-scene light colour, appearance-region fades and
        /// the parts she turns on and off are all applied there.
        /// </summary>
        private void SyncShadeLight()
        {
            if (shadeLightRenderers.Length == 0)
            {
                return;
            }

            var config = ModConfig.Instance;

            float measured = MeasureHeroLightRadius();
            if (measured > 0.01f)
            {
                heroLightRadius = measured;
            }

            float maxIntensity = Mathf.Max(0f, config.shadeLightIntensity);
            float maxRadius = Mathf.Max(0f, config.shadeLightRadiusScale);

            // The Knight lights more of the room than the Shade does - it is played, not followed.
            // Applied to the peaks so the distance fade below still governs it.
            if (UsesGroundedMovement)
            {
                maxIntensity *= Mathf.Max(0f, config.knightLightIntensityMultiplier);
                maxRadius *= Mathf.Max(0f, config.knightLightRadiusMultiplier);
            }

            // Both fade in with distance from Hornet, because the Shade's light is only wanted
            // where hers is not reaching: overlapping the two washes the pair out. Radius reaches
            // its maximum at the edge of her light; intensity ramps over the further distance at
            // which the two stop overlapping at all.
            // Measured off Hornet's light unless overridden. These sprites carry a wide soft
            // falloff, so the measurement can read larger than the light looks; the override exists
            // so the ramp can be tuned without a rebuild.
            float rampRadius = config.shadeLightFalloffRadius > 0.01f
                ? config.shadeLightFalloffRadius
                : heroLightRadius;

            var hero = LegacyHelper.ResolveHeroController();
            float separation = hero != null ? Vector2.Distance(transform.position, hero.transform.position) : 0f;
            float radiusT = rampRadius > 0.01f ? Mathf.Clamp01(separation / rampRadius) : 1f;
            float intensityT = rampRadius > 0.01f
                ? Mathf.Clamp01(separation / (rampRadius * (1f + maxRadius)))
                : 1f;

            float intensity = maxIntensity * intensityT;
            float radius = maxRadius * radiusT;

            // Follows whatever actually draws this companion, which is what
            // ApplyScriptedHoldVisibility turns off, so a scripted hold takes the light with it.
            // The Knight draws through its rig rather than the sheet renderer, and keying this on
            // the renderer alone left it carrying no light at all.
            bool visible = CompanionVisible;

            for (int i = 0; i < shadeLightRenderers.Length; i++)
            {
                var target = shadeLightRenderers[i];
                var origin = shadeLightSourceRenderers[i];
                if (!target || !origin)
                {
                    continue;
                }

                target.enabled = visible && origin.enabled;

                Color color = origin.color;
                color.a = Mathf.Clamp01(color.a * intensity);
                target.color = color;
            }

            for (int i = 0; i < shadeLightRoots.Length; i++)
            {
                if (shadeLightRoots[i])
                {
                    shadeLightRoots[i].localScale = shadeLightRootBaseScales[i] * radius;
                }
            }

            RecentreShadeLight();
        }

        /// <summary>
        /// Hornet's light radius in world units, measured off her own renderers so the distance
        /// ramp is expressed in terms of the thing it is fading against rather than a tuned number.
        /// </summary>
        private float MeasureHeroLightRadius()
        {
            float radius = 0f;
            foreach (var renderer in shadeLightSourceRenderers)
            {
                if (!renderer || !renderer.enabled)
                {
                    continue;
                }

                Vector3 extents = renderer.bounds.extents;
                radius = Mathf.Max(radius, Mathf.Max(extents.x, extents.y));
            }

            return radius;
        }

        /// <summary>
        /// Puts the light's visible centre on the Shade.
        /// <para>
        /// Hornet's rig hangs its glow well above her transform origin - her own light sprite sits
        /// about 5.7 units up - so cloning the rig and parenting it at the Shade's origin left the
        /// light floating roughly two body-heights over its head. Correcting by the rendered bounds
        /// rather than by a named part keeps the rig's internal arrangement and needs no assumption
        /// about which child is the glow.
        /// </para>
        /// </summary>
        private void RecentreShadeLight()
        {
            bool any = false;
            Bounds combined = default;

            foreach (var renderer in shadeLightRenderers)
            {
                if (!renderer || !renderer.enabled)
                {
                    continue;
                }

                if (any)
                {
                    combined.Encapsulate(renderer.bounds);
                }
                else
                {
                    combined = renderer.bounds;
                    any = true;
                }
            }

            if (!any)
            {
                return;
            }

            Vector3 delta = transform.position - combined.center;
            delta.z = 0f;

            foreach (var root in shadeLightRoots)
            {
                if (root)
                {
                    root.position += delta;
                }
            }
        }

        private void DestroyShadeLight()
        {
            foreach (var root in shadeLightRoots)
            {
                if (root)
                {
                    UnityEngine.Object.Destroy(root.gameObject);
                }
            }

            shadeLightRoots = Array.Empty<Transform>();
            shadeLightRootBaseScales = Array.Empty<Vector3>();
            shadeLightRenderers = Array.Empty<SpriteRenderer>();
            shadeLightSourceRenderers = Array.Empty<SpriteRenderer>();
        }
    }
}
#nullable restore
