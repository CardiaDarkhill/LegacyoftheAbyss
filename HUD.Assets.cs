#nullable disable
using System;
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

    private void LoadSprites()
    {
        try
        {
            var maskPath = ModPaths.GetAssetPath("select_game_HUD_0001_health.png");
            var framePath = ModPaths.GetAssetPath("select_game_HUD_0002_health_frame.png");
            var slashPath = ModPaths.GetAssetPath("The Knight spells and items - atlas0 #00000309.png");
            var soulOrbPath = ModPaths.GetAssetPath("soul_orb_glow0000.png");
            var overcharmBackdropPath = ModPaths.GetAssetPath("overcharm_backboard.png");
            maskSprite = LoadSprite(maskPath);
            if (maskSprite == null) maskSprite = FindSpriteInGame("select_game_HUD_0001_health");
            hivebloodMaskSprite = CreateTintedSprite(maskSprite, hivebloodMaskColor);
            frameSprite = LoadSprite(framePath);
            if (frameSprite == null) frameSprite = FindSpriteInGame("select_game_HUD_0002_health_frame");
            slashFrames = LoadSpriteSheet(slashPath, 8, 8);
            soulOrbSprite = LoadSprite(soulOrbPath);
            overcharmBackdropSprite = LoadSprite(overcharmBackdropPath);
            if (overcharmBackdropSprite == null)
            {
                overcharmBackdropSprite = ShadeCharmIconLoader.TryLoadIcon("overcharm_backboard", "overcharm_backboard.png");
            }
            if (hivebloodMaskSprite == null)
            {
                var fallbackMask = BuildMaskSprite();
                hivebloodMaskSprite = CreateTintedSprite(fallbackMask, hivebloodMaskColor);
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
