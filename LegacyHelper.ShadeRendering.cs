#nullable disable
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

    /// <summary>
    /// Hornet's body renderer.
    /// <para>
    /// Silksong draws the hero as a sprite *mesh*, not a <see cref="SpriteRenderer"/> -
    /// <c>HeroController.SetupGameRefs</c> resolves it as
    /// <c>base.GetComponent&lt;MeshRenderer&gt;()</c>. The Shade's sorting used to be derived from
    /// <c>hero_ctrl.GetComponentInChildren&lt;SpriteRenderer&gt;()</c>, which never finds that
    /// renderer and instead returns whichever child effect happens to come first in the hierarchy
    /// (the hero vignette among them, which sits on the "Vignette" sorting layer - above fog, snow
    /// and every other weather layer). That is why the Shade drew on top of effects it should have
    /// been occluded by.
    /// </para>
    /// </summary>
    internal static Renderer ResolveHornetBodyRenderer(HeroController hero)
    {
        if (hero == null)
        {
            return null;
        }

        try
        {
            var mesh = hero.GetComponent<MeshRenderer>();
            if (mesh != null)
            {
                return mesh;
            }

            // Not expected on any shipped build, but a future hero rig could go back to sprites.
            var sprite = hero.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                return sprite;
            }
        }
        catch
        {
        }

        // Deliberately no child search: guessing at a child renderer is the bug this method exists
        // to fix. Returning null lets the caller fall back to the configured sorting layer instead.
        return null;
    }

    private static HeroController ResolveHeroController()
    {
        try
        {
            var gm = GameManager.instance;
            if (gm != null && gm.hero_ctrl != null)
            {
                return gm.hero_ctrl;
            }
        }
        catch
        {
        }

        try
        {
            return HeroController.instance;
        }
        catch
        {
            return null;
        }
    }

    private static bool SortingLayerExists(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        try
        {
            foreach (var layer in SortingLayer.layers)
            {
                if (string.Equals(layer.name, name, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }
        catch
        {
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

        try
        {
            if (shadeDefaultSpriteMaterial == null)
            {
                shadeDefaultSpriteMaterial = sr.sharedMaterial;
            }
        }
        catch
        {
        }

        var hero = ResolveHeroController();
        var heroRenderer = ResolveHornetBodyRenderer(hero);

        string heroLayerName = null;
        int heroOrder = 0;
        if (heroRenderer != null)
        {
            try
            {
                heroLayerName = SortingLayer.IDToName(heroRenderer.sortingLayerID);
                heroOrder = heroRenderer.sortingOrder;
            }
            catch
            {
            }
        }

        string layerName = ShadeVisualTuning.ResolveSortingLayerName(config.shadeSortingLayer, SortingLayerExists, heroLayerName);
        bool sharesHeroLayer = string.Equals(layerName, heroLayerName, System.StringComparison.Ordinal);

        int order = ShadeVisualTuning.ResolveSortingOrder(heroOrder, config.shadeSortingOrderOffset, sharesHeroLayer);
        try
        {
            sr.sortingLayerName = layerName;
            sr.sortingOrder = order;
        }
        catch
        {
        }

        // One line per resolution, gated on logShade: the live pass for this needs to confirm the
        // Shade landed on a character layer rather than on whatever the vignette was using.
        try
        {
            if (config.logShade)
            {
                string heroDescription = heroRenderer != null
                    ? $"{heroRenderer.GetType().Name} on '{heroLayerName}' order {heroOrder}"
                    : "unresolved";
                LogInfo($"Shade rendering: layer '{layerName}' order {order} (Hornet: {heroDescription}).");
            }
        }
        catch
        {
        }

        ApplyShadeSpriteMaterial(sr, heroRenderer, config);
    }

    private static void ApplyShadeSpriteMaterial(SpriteRenderer sr, Renderer heroRenderer, ModConfig config)
    {
        Material resolved = null;
        if (config.shadeUseHornetMaterial)
        {
            resolved = ResolveShadeSpriteMaterial(heroRenderer);
        }

        try
        {
            if (resolved != null)
            {
                sr.sharedMaterial = resolved;
            }
            else if (shadeDefaultSpriteMaterial != null)
            {
                sr.sharedMaterial = shadeDefaultSpriteMaterial;
            }
        }
        catch
        {
        }
    }

    private static Material ResolveShadeSpriteMaterial(Renderer heroRenderer)
    {
        Material source = null;
        try
        {
            source = heroRenderer != null ? heroRenderer.sharedMaterial : null;
        }
        catch
        {
        }

        // A SpriteRenderer feeds its sprite's texture through _MainTex, so a material without that
        // property would render the Shade as a flat block. Anything that fails this check is left
        // on Unity's default sprite material rather than risking an invisible companion.
        bool usable;
        try
        {
            usable = source != null
                && source.shader != null
                && source.shader.name != "Hidden/InternalErrorShader"
                && source.HasProperty("_MainTex");
        }
        catch
        {
            usable = false;
        }

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

        try
        {
            var previous = shadeSpriteMaterial;
            shadeSpriteMaterial = new Material(source) { name = "ShadeSpriteMaterial" };
            shadeSpriteMaterialSource = source;
            if (previous != null)
            {
                UnityEngine.Object.Destroy(previous);
            }
        }
        catch
        {
            shadeSpriteMaterial = null;
            shadeSpriteMaterialSource = null;
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
            SyncChildRendererSorting();
        }

        private void SyncChildRendererSorting()
        {
            if (!sr)
            {
                return;
            }

            int layer = sr.sortingLayerID;
            int order = sr.sortingOrder;

            try
            {
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

                SyncShadowParticleSorting();
            }
            catch
            {
            }

            // Covers shadeLightRenderers and the focus aura, which re-read the body sprite anyway.
            SyncShadeLight();
        }
    }
}
#nullable restore
