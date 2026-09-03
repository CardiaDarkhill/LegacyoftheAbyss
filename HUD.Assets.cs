#nullable disable
using System;
using LegacyoftheAbyss.Shade.Knight;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using LegacyoftheAbyss.Shade;

public partial class SimpleHUD
{
    private Sprite BuildMaskSprite()
    {
        var tex = new Texture2D(32, 32);
        for (int x = 0; x < 32; x++)
            for (int y = 0; y < 32; y++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// Resolves a row of bundle clips into their resting sprites, and whether each was packed
    /// turned.
    /// <para>
    /// <c>int.MaxValue</c> is "the last frame": <c>TryBuildSprite</c> clamps, and every clip set
    /// drawn this way is an animation whose resting pose is the frame it ends on. Returns false
    /// when nothing resolved, which usually means the bundle has not finished loading rather than
    /// that the art is missing - so a caller keeps the rotation flags and asks again.
    /// </para>
    /// </summary>
    private static bool TryResolveStageSprites(string[] clips, out Sprite[] sprites, out bool[] rotated)
    {
        sprites = new Sprite[clips.Length];
        rotated = new bool[clips.Length];
        bool any = false;

        for (int i = 0; i < clips.Length; i++)
        {
            sprites[i] = KnightAssets.TryBuildSprite(clips[i], int.MaxValue);
            rotated[i] = KnightAssets.IsSpriteRotated(clips[i], int.MaxValue);
            any |= sprites[i] != null;
        }

        return any;
    }

    /// <summary>
    /// The centred art child every HUD slot hangs its <see cref="Image"/> on.
    /// <para>
    /// Centred rather than filling the slot: a frame the atlas packed turned has to rotate about
    /// its own middle to land square, and these slots are laid out from a corner - turning about
    /// that corner would swing the art out of its own box.
    /// </para>
    /// </summary>
    private static Image CreateSlotArt(GameObject slot)
    {
        var art = new GameObject("Art");
        art.transform.SetParent(slot.transform, false);

        var artRect = art.AddComponent<RectTransform>();
        artRect.anchorMin = artRect.anchorMax = new Vector2(0.5f, 0.5f);
        artRect.pivot = new Vector2(0.5f, 0.5f);

        var image = art.AddComponent<Image>();
        image.preserveAspect = true;
        return image;
    }

    private void LoadSprites()
    {
        try
        {
            var maskPath = ModPaths.GetAssetPath("select_game_HUD_0001_health.png");
            var framePath = ModPaths.GetAssetPath("select_game_HUD_0002_health_frame.png");
            var slashPath = ModPaths.GetAssetPath("The Knight spells and items - atlas0 #00000309.png");
            var soulOrbPath = ModPaths.GetAssetPath("soul_orb_glow0000.png");
            var overcharmBackdropPath = ModPaths.GetAssetPath("overcharm_backboard.png");
            // Hollow Knight's own HUD art first, out of the Knight bundle. It is the companion's
            // HUD, so it should look like the companion's game - and the mask in particular is more
            // than twice the resolution of the still we were shipping (70x57 against 33x41).
            maskSprite = KnightAssets.TryBuildSprite(KnightHud.MaskClip, 0);
            maskSpriteRotated = maskSprite != null && KnightAssets.IsSpriteRotated(KnightHud.MaskClip, 0);
            maskBackboardSprite = KnightAssets.TryBuildSprite(KnightHud.MaskBackboardClip, 0);
            frameSprite = KnightAssets.TryBuildSprite(KnightHud.FrameClip, 0);

            // After the build, never before it. KnightAssets records a frame's packing while it is
            // cutting the sprite, so asking first is asking an empty set and always answers "not
            // turned" - which is what was happening to the frame, and it is packed turned.
            frameSpriteRotated = frameSprite != null && KnightAssets.IsSpriteRotated(KnightHud.FrameClip, 0);
            soulOrbSprite = KnightAssets.TryBuildSprite(KnightHud.SoulOrbClip, 0);
            soulOrbFillSprite = KnightAssets.TryBuildSpriteFromTexture(KnightHud.SoulOrbFillTexture);

            // The shipped stills stay as the fallback: the bundle is optional, and a player without
            // it should still get a HUD rather than a row of white boxes.
            if (maskSprite == null) maskSprite = LoadSprite(maskPath);
            if (maskSprite == null) maskSprite = FindSpriteInGame("select_game_HUD_0001_health");

            // Hiveblood's own masks, which the bundle turns out to carry. They were the plain mask
            // painted orange, and that stand-in could not survive the move to bundle art: the tint
            // copies pixels out of its source, and the source is now a region of an atlas texture
            // the CPU may not read back. The read fails, the tint falls back to a blank white box,
            // and the box is what was drawn - five orange rectangles where the masks should be.
            hivebloodMaskSprite = KnightAssets.TryBuildSprite(KnightHud.HiveMaskClip, 0);
            hivebloodMaskIsBundleArt = hivebloodMaskSprite != null;
            hivebloodMaskSpriteRotated = hivebloodMaskIsBundleArt
                && KnightAssets.IsSpriteRotated(KnightHud.HiveMaskClip, 0);

            if (hivebloodMaskSprite == null)
            {
                hivebloodMaskSprite = CreateTintedSprite(maskSprite, hivebloodMaskColor);
                hivebloodMaskSpriteRotated = maskSpriteRotated;
            }
            if (frameSprite == null) frameSprite = LoadSprite(framePath);
            if (frameSprite == null) frameSprite = FindSpriteInGame("select_game_HUD_0002_health_frame");
            slashFrames = LoadSpriteSheet(slashPath, 8, 8);
            if (soulOrbSprite == null) soulOrbSprite = LoadSprite(soulOrbPath);
            if (soulOrbFillSprite == null) soulOrbFillSprite = soulOrbSprite;
            overcharmBackdropSprite = LoadSprite(overcharmBackdropPath);
            if (overcharmBackdropSprite == null)
            {
                overcharmBackdropSprite = ShadeCharmIconLoader.TryLoadIcon("overcharm_backboard", "overcharm_backboard.png");
            }
            if (hivebloodMaskSprite == null)
            {
                var fallbackMask = BuildMaskSprite();
                hivebloodMaskSprite = CreateTintedSprite(fallbackMask, hivebloodMaskColor);
                hivebloodMaskSpriteRotated = false;
                if (fallbackMask != null)
                {
                    var fallbackTexture = fallbackMask.texture;
                    if (fallbackTexture != null)
                    {
                        UnityEngine.Object.Destroy(fallbackTexture);
                    }

                    UnityEngine.Object.Destroy(fallbackMask);
                }
            }
        }
        catch { }
    }

    private Sprite CreateTintedSprite(Sprite source, Color tint)
    {
        if (source == null)
        {
            return null;
        }

        Texture2D tintedTexture = null;
        try
        {
            var texture = source.texture;
            if (texture == null)
            {
                return null;
            }

            var rect = source.textureRect;
            int width = Mathf.Max(1, Mathf.RoundToInt(rect.width));
            int height = Mathf.Max(1, Mathf.RoundToInt(rect.height));
            int x = Mathf.RoundToInt(rect.x);
            int y = Mathf.RoundToInt(rect.y);

            Color[] pixels = texture.GetPixels(x, y, width, height);
            if (pixels == null || pixels.Length == 0)
            {
                return null;
            }

            var tinted = new Color[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                var px = pixels[i];
                tinted[i] = new Color(px.r * tint.r, px.g * tint.g, px.b * tint.b, px.a * tint.a);
            }

            tintedTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            tintedTexture.SetPixels(tinted);
            tintedTexture.Apply();
            tintedTexture.filterMode = FilterMode.Point;
            tintedTexture.wrapMode = TextureWrapMode.Clamp;

            var sprite = Sprite.Create(
                tintedTexture,
                new Rect(0f, 0f, width, height),
                new Vector2(0.5f, 0.5f),
                source.pixelsPerUnit);
            if (sprite != null)
            {
                return sprite;
            }
        }
        catch
        {
        }

        if (tintedTexture != null)
        {
            UnityEngine.Object.Destroy(tintedTexture);
        }

        return null;
    }

    private Sprite LoadSprite(string path)
    {
        if (!File.Exists(path)) return null;
        var bytes = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        TryLoadImage(tex, bytes);
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    private Sprite[] LoadSpriteSheet(string path, int cols, int rows)
    {
        if (!File.Exists(path)) return Array.Empty<Sprite>();
        var bytes = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        TryLoadImage(tex, bytes);
        tex.filterMode = FilterMode.Point;
        int w = tex.width / cols;
        int h = tex.height / rows;
        var sprites = new Sprite[cols * rows];
        int idx = 0;
        for (int y = rows - 1; y >= 0; y--)
            for (int x = 0; x < cols; x++)
                sprites[idx++] = Sprite.Create(tex, new Rect(x * w, y * h, w, h), new Vector2(0.5f, 0.5f));
        return sprites;
    }

    private Sprite BuildCircleSprite()
    {
        int size = 64;
        var tex = new Texture2D(size, size);
        Vector2 c = new Vector2(size / 2f, size / 2f);
        float r = size / 2f;
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                var col = Vector2.Distance(new Vector2(x, y), c) <= r ? Color.white : Color.clear;
                tex.SetPixel(x, y, col);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private Sprite FindSpriteInGame(string namePart)
    {
        if (string.IsNullOrEmpty(namePart)) return null;
        string key = Path.GetFileNameWithoutExtension(namePart);
        var all = Resources.FindObjectsOfTypeAll<Sprite>(); Sprite best = null; int bestScore = int.MinValue;
        foreach (var sp in all)
        {
            if (sp == null) continue;
            string n = sp.name ?? string.Empty; int score = 0;
            if (string.Equals(n, key, StringComparison.OrdinalIgnoreCase)) score += 1000;
            if (n.Contains(key, StringComparison.OrdinalIgnoreCase)) score += 100;
            score += (int)(sp.rect.width + sp.rect.height);
            if (score > bestScore) { bestScore = score; best = sp; }
        }
        return best;
    }

    private static bool TryLoadImage(Texture2D tex, byte[] bytes)
    {
        return ImageConversion.LoadImage(tex, bytes, false);
    }
}

#nullable restore
