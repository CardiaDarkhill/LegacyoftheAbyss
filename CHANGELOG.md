# Changelog

## 1.2.0

### Fixed: the Shade no longer draws on top of fog, snow and darkness

The Shade was being placed in the render order relative to "the first sprite renderer found
under Hornet". That is never Hornet herself — Silksong draws the hero as a sprite *mesh*, not a
sprite renderer — so what it actually found was one of her child effects, in practice one sitting
on a layer *above* every weather and lighting layer in the game. That is why the Shade floated
in front of fog banks, snowfall and darkness instead of being obscured by them like Hornet is.

The Shade now resolves Hornet's real renderer and sits on an explicit character sorting layer,
and it draws with a copy of Hornet's own sprite material, so scene darkness, area tinting and
appearance regions treat it as a character rather than as an unlit overlay pasted over the scene.

Both parts are adjustable in `BepInEx/config/LegacyoftheAbyss/config.json` if you would rather
have the old always-on-top look: `shadeUseHornetMaterial` turns the material change off, and
`shadeSortingOrderOffset` moves the Shade in front of or behind Hornet.

### New: shadow particles

The Shade is now surrounded by black wisps, in the style of the Shade from the first game. They
do not stream out of it: they appear scattered across and around the body, then drift lazily
upward with a slow side-to-side wander. They are clearly visible on an empty soul meter and
roughly twice as dense on a full one, so the Shade visibly darkens as it charges up.

`shadeShadowParticlesEnabled` turns them off, and `shadeShadowParticleIntensity` (0 to 2) scales
them, both in `config.json`.

### Fixed: skin previews are no longer a block of pixels

The skin selector draws the preview at up to 900 pixels tall from source art a fraction of that
size, with no filtering, so every source pixel became a visible square. The art itself is not
pixel art — its edges are already smooth — so previews are now properly resampled up to a larger
texture, which keeps those edges instead of chopping them into blocks.

There is a limit to this: the frames are 180 pixels square and the preview is around five times
that, so the result is smooth rather than sharp. Getting genuinely crisper previews would need
higher-resolution source art.

The same filtering can optionally be applied to the Shade in-game — `shadeSpriteSmoothing` in
`config.json`, off by default, because at gameplay size the crisp pixel look is a perfectly
reasonable preference. It is a separate setting from `shadeSkinPreviewSmoothing`, so you can
smooth the previews without touching how the Shade looks in-game.

### Note on the new settings

The six settings above live in `BepInEx/config/LegacyoftheAbyss/config.json` rather than in the
pause menu. They are read at startup and applied as the Shade spawns, so edit the file with the
game closed. Sensible defaults are already set — most players should never need to touch them.



