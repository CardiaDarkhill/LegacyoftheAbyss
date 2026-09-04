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

    /// <summary>
    /// The shader keyword Silksong uses to mean "this renderer is a character". Scene darkness, the
    /// light cutout and appearance-region tinting all key off it - <see cref="CharacterTint.CanAdd"/>
    /// refuses any renderer whose material has it switched off. Asserted against the shipped
    /// assembly in <c>Tests/GameApiContract.cs</c>, because a renamed keyword would silently put the
    /// companion back to being lit as scenery.
    /// </summary>
    internal const string CharacterShaderKeyword = "IS_CHARACTER";

    /// <summary>The tint <see cref="CharacterTint"/> drives, seeded to white so enabling the keyword
    /// cannot leave the companion drawing through an unset colour.</summary>
    internal const string CharacterTintColorProperty = "_CharacterTintColor";

    private static bool loggedShadeMaterialUnavailable;
    private static bool loggedHeroLightUnavailable;
    private static bool loggedHeroNotACharacter;

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
    /// Decodes a PNG or JPEG into <paramref name="texture"/>.
    /// <para>
    /// <paramref name="markNonReadable"/> defaults to true: nothing reads these pixels back once
    /// <c>Sprite.Create</c> has them, and keeping them readable holds a second full copy of every
    /// sheet in managed memory for the life of the texture. The one caller that does read them back
    /// is the sprite-smoothing path, which passes false.
    /// </para>
    /// <para>
    /// A direct call, not reflection. Two of the three copies this replaced looked
    /// <c>UnityEngine.ImageConversion</c> up by name at runtime, which cannot fail any way the
    /// compiler would not have caught first - and if it somehow did, it returned false and the art
    /// simply never appeared.
    /// </para>
    /// </summary>
    internal static bool TryLoadImage(Texture2D texture, byte[] bytes, bool markNonReadable = true)
    {
        if (texture == null || bytes == null || bytes.Length == 0)
        {
            return false;
        }

        return ImageConversion.LoadImage(texture, bytes, markNonReadable);
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
        if (!shadeDefaultSpriteMaterial)
        {
            shadeDefaultSpriteMaterial = sr.sharedMaterial;
        }

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
        if (!resolved)
        {
            resolved = shadeDefaultSpriteMaterial;
        }

        // Unity's null, not ??: a destroyed material is not null to the operator, and assigning one
        // - or assigning nothing, when the cached default never resolved - draws the companion as a
        // magenta block. Leaving the renderer on what it already has is always the better failure.
        if (resolved)
        {
            sr.sharedMaterial = resolved;
        }
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
            ApplyKnightCharacterShading();

            SyncChildRendererSorting();
            EnsureShadeLight();
        }

        /// <summary>
        /// What the light ramp last decided, for the bug report. The ramp runs on separation from
        /// Hornet and both of the things it produces - a glow drawn over the companion, and a cutout
        /// that lifts darkness around it - can be mistaken for the companion's own opacity changing
        /// with distance, which is exactly the complaint being diagnosed. The numbers say which.
        /// </summary>
        private float lastLightSeparation;
        private float lastLightIntensity;
        private float lastLightGlowIntensity;
        private float lastLightRadius;

        /// <summary>
        /// Marks the Knight's rig as a character to Silksong's shader, which is what the Shade gets
        /// for free by drawing through a clone of Hornet's own material.
        /// <para>
        /// The rig cannot take that clone: it draws Hollow Knight's atlas through tk2d, so the
        /// material has to stay its own. What it ships with is the Hollow Knight material, and that
        /// material carries <b>no shader keywords at all</b> - so on Silksong's shader the Knight is
        /// lit as scenery rather than as a character. In a dark room that is not a subtle difference:
        /// scenery fades into the dark, and the reports of a "see-through" Knight are the background
        /// showing through a companion the shader is fading out, while Hornet beside it stays solid.
        /// </para>
        /// <para>
        /// Hornet's own material is asked first rather than the keyword being enabled on faith. If
        /// hers does not carry it then the guess about what the keyword means is wrong, and turning
        /// on a keyword nothing else uses is more likely to break the Knight's shading than fix it -
        /// so that case logs and changes nothing.
        /// </para>
        /// <para>
        /// The rig body draws every one of its 891 frames from a single material, so this is a
        /// one-time edit to a shared asset rather than something tk2d can swap out from under it;
        /// re-running it per scene is idempotent.
        /// </para>
        /// </summary>
        private void ApplyKnightCharacterShading()
        {
            if (knightView == null || !ModConfig.Instance.shadeUseHornetMaterial)
            {
                return;
            }

            var rigRenderer = knightView.FirstRenderer;
            var material = rigRenderer != null ? rigRenderer.sharedMaterial : null;
            if (!material)
            {
                return;
            }

            var heroMaterial = LegacyHelper.ResolveHornetBodyRenderer(ResolveHeroController())?.sharedMaterial;
            if (!heroMaterial || !heroMaterial.IsKeywordEnabled(LegacyHelper.CharacterShaderKeyword))
            {
                if (!loggedHeroNotACharacter)
                {
                    loggedHeroNotACharacter = true;
                    LogWarning(
                        $"Hornet's material does not carry '{LegacyHelper.CharacterShaderKeyword}', so the Knight is left on the shading it shipped with. "
                        + "Scene darkness will treat it as scenery.");
                }

                return;
            }

            if (!material.IsKeywordEnabled(LegacyHelper.CharacterShaderKeyword))
            {
                material.EnableKeyword(LegacyHelper.CharacterShaderKeyword);
            }

            // Only seeded, never re-asserted against a live value: once the keyword is on, the game's
            // own CharacterTint drives this through a property block, and a property block wins over
            // the material anyway.
            if (material.HasProperty(LegacyHelper.CharacterTintColorProperty))
            {
                material.SetColor(LegacyHelper.CharacterTintColorProperty, Color.white);
            }
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

        /// <summary>
        /// The opacity the companion is actually drawn at, and where it comes from.
        /// <para>
        /// Written because "the Knight looks see-through" cannot be answered from a screenshot. A
        /// washed-out companion is either our own alpha - the Shade sits at 0.9 by design, and the
        /// focus and teleport channels drop it further - or a mist prop the room draws in front of
        /// it, and those look identical in a frame. Reading the alpha back settles it in one line:
        /// at 1.00 the wash is the scene's and nothing here caused it.
        /// </para>
        /// <para>
        /// Both halves of the Knight are reported because they are separate values that both scale
        /// what reaches the screen - the material's own <c>_Color</c>, and the vertex colour tk2d
        /// multiplies into it - and the rig keeps Hollow Knight's materials rather than the clone of
        /// Hornet's that the Shade's body gets, so its shader is named too.
        /// </para>
        /// </summary>
        internal string DescribeRendering()
        {
            try
            {
                var parts = new System.Collections.Generic.List<string>();

                var heroRenderer = LegacyHelper.ResolveHornetBodyRenderer(ResolveHeroController());
                parts.Add(heroRenderer != null
                    ? "hornet: " + DescribeMaterial(heroRenderer.sharedMaterial) + DescribeRendererOverrides(heroRenderer)
                    : "hornet: unresolved");

                if (sr != null)
                {
                    parts.Add(FormattableString.Invariant(
                        $"body: drawn {sr.enabled} colour a={sr.color.a:0.00} {DescribeMaterial(sr.sharedMaterial)}{DescribeRendererOverrides(sr)}"));
                }

                if (knightView != null)
                {
                    var rigRenderer = knightView.FirstRenderer;
                    if (rigRenderer != null)
                    {
                        var sprite = rigRenderer.GetComponent<tk2dBaseSprite>();
                        string vertex = sprite != null
                            ? FormattableString.Invariant($"sprite a={sprite.color.a:0.00}")
                            : "sprite absent";

                        parts.Add(FormattableString.Invariant(
                            $"knight rig: drawn {rigRenderer.enabled} {vertex} {DescribeMaterial(rigRenderer.sharedMaterial)}{DescribeRendererOverrides(rigRenderer)}"));
                    }
                    else
                    {
                        parts.Add("knight rig: no renderer");
                    }
                }

                parts.Add(DescribeLightParts());

                parts.Add(shadeLightRenderers.Length == 0
                    ? (ModConfig.Instance.shadeLightEnabled
                        ? (shadeLightCloneFailed ? "light: clone failed" : "light: none yet")
                        : "light: switched off")
                    : FormattableString.Invariant(
                        $"light: {shadeLightRenderers.Length} renderer(s) at {lastLightSeparation:0.0}u from Hornet, cutout {lastLightIntensity:0.00} glow {lastLightGlowIntensity:0.00} radius {lastLightRadius:0.00}"));

                return string.Join(" | ", parts.ToArray());
            }
            catch (Exception e)
            {
                return "unreadable: " + e.Message;
            }
        }

        /// <summary>
        /// Puts the decorative half of the cloned light behind the companion instead of level with it.
        /// <para>
        /// Four of the five glow parts - <c>HeroLight</c>, <c>Imbued Hero Light</c>, <c>Dust</c> and
        /// <c>Dust BG</c> - are soft 18-to-24 unit discs that clone onto sorting layer "Default" at
        /// order 0, which is <em>exactly</em> the companion's own layer and order. Unity has no
        /// defined winner for that tie, and for the companion it broke the wrong way: the discs
        /// landed on the body rather than around it and the companion read as see-through, worse
        /// with distance because the ramp brightens and grows them with separation. Three bug
        /// reports went to it, and two guesses at the cause before the report was made to say which.
        /// </para>
        /// <para>
        /// Explicitly ordering them behind the companion removes the tie rather than resolving it in
        /// our favour by luck. Only within the companion's own layer: <c>white_light_donut</c> sits
        /// on "Over", above the darkness vignette, which is the only place it can be seen from - and
        /// a donut has a hole where the body is, so it was never the half doing this. The cutout
        /// parts are untouched for the same reason as the shadow particles: their cloned membership
        /// is what puts them in the darkness camera's pass, and that pass is what lights the room.
        /// </para>
        /// </summary>
        private void SyncGlowSorting(int layer, int order)
        {
            for (int i = 0; i < shadeLightRenderers.Length; i++)
            {
                if (i >= shadeLightIsGlow.Length || !shadeLightIsGlow[i])
                {
                    continue;
                }

                var renderer = shadeLightRenderers[i];
                if (!renderer || renderer.sortingLayerID != layer)
                {
                    continue;
                }

                renderer.sortingOrder = order + GlowSortingOffset;
            }
        }

        /// <summary>
        /// How far behind the companion its glow sits. Below the focus aura at -2, so the aura still
        /// reads in front of the glow it is drawn against.
        /// </summary>
        private const int GlowSortingOffset = -3;

        /// <summary>
        /// Hornet's light arrangement, part by part, beside the copy of it the companion carries.
        /// <para>
        /// The halo draws over the companion where hers does not draw over her, and nothing in the
        /// game's code says why: there is no <c>SpriteMask</c> anywhere in <c>HeroLight</c>, so
        /// whatever spares her is a scene arrangement that can only be read at runtime. Three
        /// candidates, and they want opposite fixes - a mask the clone drops
        /// (<see cref="CloneLightPart"/> sets <c>maskInteraction</c> to None so the copy renders at
        /// all), a sorting order that puts her body above her glow, or a donut sprite whose hole
        /// sits over her and which the radius ramp closes on the companion.
        /// </para>
        /// <para>
        /// So each part reports the three together, source against clone. Guessing between them
        /// costs a build, a restart and a repro; this costs one line in a report.
        /// </para>
        /// </summary>
        private string DescribeLightParts()
        {
            if (shadeLightRenderers.Length == 0)
            {
                return "light parts: none";
            }

            var parts = new System.Collections.Generic.List<string>();
            for (int i = 0; i < shadeLightRenderers.Length; i++)
            {
                var clone = shadeLightRenderers[i];
                var origin = i < shadeLightSourceRenderers.Length ? shadeLightSourceRenderers[i] : null;
                if (!clone || !origin)
                {
                    continue;
                }

                string sprite = origin.sprite != null ? origin.sprite.name : "no sprite";
                string kind = i < shadeLightIsGlow.Length && shadeLightIsGlow[i] ? "glow" : "cutout";

                string mask = FormattableString.Invariant(
                    $"mask {origin.maskInteraction}->{clone.maskInteraction}");
                string sorting = FormattableString.Invariant(
                    $"layer '{SortingLayer.IDToName(origin.sortingLayerID)}'{origin.sortingOrder}->'{SortingLayer.IDToName(clone.sortingLayerID)}'{clone.sortingOrder}");
                string size = FormattableString.Invariant(
                    $"size {origin.bounds.size.x:0.00}->{clone.bounds.size.x:0.00}");

                parts.Add(FormattableString.Invariant($"{kind} '{origin.name}'<{sprite}> {mask} {sorting} {size}"));
            }

            // The masks themselves, which is the half a renderer cannot report: whether one exists
            // at all decides whether the clone can be masked the way she is.
            var hero = LegacyHelper.ResolveHeroController();
            if (hero != null)
            {
                var masks = hero.GetComponentsInChildren<SpriteMask>(true);
                parts.Add(masks.Length == 0
                    ? "hornet masks: none"
                    : "hornet masks: " + string.Join(", ", System.Array.ConvertAll(masks, m => FormattableString.Invariant(
                        $"'{m.name}' enabled={m.enabled} range={(m.isCustomRangeActive ? SortingLayer.IDToName(m.backSortingLayerID) + m.backSortingOrder + ".." + SortingLayer.IDToName(m.frontSortingLayerID) + m.frontSortingOrder : "all")}"))));
            }

            return "light parts: " + string.Join(" | ", parts.ToArray());
        }

        /// <summary>
        /// Anything the renderer is overriding its material with.
        /// <para>
        /// Reading the material alone is not reading what is drawn. A
        /// <see cref="MaterialPropertyBlock"/> set on the renderer wins over every value in the
        /// material and cannot be seen from it, so a companion faded through a block reports
        /// <c>a=1.00</c> and looks like nothing is fading it - which is the dead end two rounds of
        /// this were spent in. <see cref="CharacterTint"/> drives its tint through one, so there is
        /// certainly at least one block in play on anything the game treats as a character.
        /// </para>
        /// </summary>
        private static string DescribeRendererOverrides(Renderer renderer)
        {
            if (!renderer || !renderer.HasPropertyBlock())
            {
                return " no block";
            }

            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);

            var parts = new System.Collections.Generic.List<string>();
            foreach (var property in new[] { "_Color", LegacyHelper.CharacterTintColorProperty })
            {
                // An unset colour reads (0,0,0,0), which is not distinguishable from one set to
                // nothing on purpose - so the whole colour is reported rather than just its alpha.
                var value = block.GetColor(property);
                parts.Add(FormattableString.Invariant(
                    $"{property}=({value.r:0.00},{value.g:0.00},{value.b:0.00},a={value.a:0.00})"));
            }

            return " block[" + string.Join(" ", parts.ToArray()) + "]";
        }

        /// <summary>A material by name, shader and tint, or why there is nothing to read.</summary>
        private static string DescribeMaterial(Material material)
        {
            if (!material)
            {
                return "no material";
            }

            string shader = material.shader != null ? material.shader.name : "no shader";
            string tint = material.HasProperty("_Color")
                ? FormattableString.Invariant($" _Color a={material.GetColor("_Color").a:0.00}")
                : string.Empty;

            string character = material.IsKeywordEnabled(LegacyHelper.CharacterShaderKeyword)
                ? " character"
                : " NOT a character";

            return FormattableString.Invariant($"mat '{material.name}' shader '{shader}'{tint}{character}");
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

            SyncGlowSorting(layer, order);

            // Deliberately not the *cutout* half of the shade light: its layer and sorting are
            // Hornet's, cloned, and that membership is what puts it in the darkness camera's pass.
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
            var glowFlags = new List<bool>();

            CloneLightPart(source.gameObject, "ShadeHeroLight", true, roots, baseScales, sourceRenderers, cloneRenderers, glowFlags);

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

                    CloneLightPart(candidate.gameObject, "ShadeDarknessCutout", false, roots, baseScales, sourceRenderers, cloneRenderers, glowFlags);
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
            shadeLightIsGlow = glowFlags.ToArray();

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
            bool isGlow,
            List<Transform> roots,
            List<Vector3> baseScales,
            List<SpriteRenderer> sourceRenderers,
            List<SpriteRenderer> cloneRenderers,
            List<bool> glowFlags)
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

            // Hornet carries no SpriteMask and none of her light renderers interact with one - both
            // read out of a live game into the bug report's "light parts" row. So this is a no-op
            // held only because a clone that ever did inherit a mask would be culled outside it,
            // which looks exactly like a light that does nothing.
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
            for (int i = 0; i < copies.Length; i++)
            {
                glowFlags.Add(isGlow);
            }
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

            // The glow is held to its own ceiling, because the two halves of this clone want
            // different things from the same number. The cutout is a mask fed to the darkness
            // shader, and driving it hard is how a companion lights a dark room away from Hornet.
            // The glow is a soft sprite drawn on "Over", above everything including the companion,
            // so it lands on the body rather than around it and the companion reads as see-through
            // - blended with what is behind it, and worse with separation, because the halo grows
            // and brightens with exactly that. Hornet is spared it by a SpriteMask the clone cannot
            // keep (see CloneLightPart), so the ceiling now defaults to zero and the halo is off
            // until the clone can be masked the way hers is.
            float glowIntensity = Mathf.Min(intensity, Mathf.Max(0f, config.shadeLightGlowIntensityCap));

            lastLightSeparation = separation;
            lastLightIntensity = intensity;
            lastLightGlowIntensity = glowIntensity;
            lastLightRadius = radius;

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

                bool isGlow = i < shadeLightIsGlow.Length && shadeLightIsGlow[i];

                Color color = origin.color;
                color.a = Mathf.Clamp01(color.a * (isGlow ? glowIntensity : intensity));
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
            shadeLightIsGlow = Array.Empty<bool>();
        }
    }
}
#nullable restore
