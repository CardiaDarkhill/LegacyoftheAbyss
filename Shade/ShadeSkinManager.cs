#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Discovers the Shade skins on disk and resolves sprite-sheet lookups through the
    /// selected one. Skin folders live under <c>Assets/Knight_Shade_Sprites/Skins</c>; each
    /// only needs the sheets it overrides, so <see cref="ResolveSpritePath"/> falls back to
    /// the built-in set for anything a skin omits.
    /// </summary>
    internal static class ShadeSkinManager
    {
        internal const string DefaultSkinId = "Default";
        internal const string SkinsFolderName = "Skins";
        private const string ManifestFileName = "skins.json";
        private const string PreviewSheetName = "Shade_Idle_Sheet.png";

        private static readonly object SyncRoot = new();
        private static readonly Dictionary<string, Sprite?> PreviewCache = new(StringComparer.OrdinalIgnoreCase);

        private static ShadeSkinDefinition[] s_skins = Array.Empty<ShadeSkinDefinition>();
        private static string? s_spritesRootOverride;
        private static bool s_initialized;

        internal static IReadOnlyList<ShadeSkinDefinition> Skins
        {
            get
            {
                EnsureLoaded();
                return s_skins;
            }
        }

        internal static ShadeSkinDefinition SelectedSkin
        {
            get
            {
                EnsureLoaded();
                string id = ModConfig.Instance.shadeSkin;
                foreach (var skin in s_skins)
                {
                    if (skin.Matches(id))
                    {
                        return skin;
                    }
                }

                return s_skins.Length > 0 ? s_skins[0] : CreateDefaultSkin();
            }
        }

        internal static string SelectedSkinId => SelectedSkin.Id;

        internal static void EnsureLoaded()
        {
            if (s_initialized)
            {
                return;
            }

            Reload();
        }

        /// <param name="spritesRootOverride">
        /// Stands in for <c>Assets/Knight_Shade_Sprites</c>. Only used by tests; production
        /// callers pass null so lookups keep going through <see cref="ModPaths"/>.
        /// </param>
        internal static void Reload(string? spritesRootOverride = null)
        {
            lock (SyncRoot)
            {
                s_spritesRootOverride = spritesRootOverride;
                s_skins = Discover().ToArray();
                s_initialized = true;
            }
        }

        /// <summary>
        /// Full path to <paramref name="fileName"/> for the currently selected skin, falling
        /// back to the built-in sheet when the skin does not override it.
        /// </summary>
        internal static string ResolveSpritePath(string fileName) => ResolveSpritePath(SelectedSkin, fileName);

        internal static string ResolveSpritePath(ShadeSkinDefinition? skin, string fileName)
        {
            string defaultPath = s_spritesRootOverride != null
                ? Path.Combine(s_spritesRootOverride, fileName)
                : ModPaths.GetAssetPath("Knight_Shade_Sprites", fileName);
            if (skin == null || skin.IsDefault || string.IsNullOrWhiteSpace(fileName))
            {
                return defaultPath;
            }

            try
            {
                string candidate = Path.Combine(skin.Directory!, fileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
            catch
            {
            }

            return defaultPath;
        }

        /// <summary>
        /// Persists <paramref name="skinId"/> as the selection. Returns true when it actually
        /// changed — callers refresh the live Shade via <see cref="LegacyHelper.SetShadeSkin"/>
        /// rather than calling this directly.
        /// </summary>
        internal static bool SelectSkin(string? skinId)
        {
            EnsureLoaded();
            var target = s_skins.FirstOrDefault(s => s.Matches(skinId));
            if (target == null)
            {
                return false;
            }

            if (string.Equals(ModConfig.Instance.shadeSkin, target.Id, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            ModConfig.Instance.shadeSkin = target.Id;
            ModConfig.Save();
            return true;
        }

        /// <summary>
        /// A single idle frame for <paramref name="skin"/>, cropped out of its idle sheet, for menu previews.
        /// </summary>
        internal static Sprite? GetPreviewSprite(ShadeSkinDefinition? skin)
        {
            if (skin == null)
            {
                return null;
            }

            lock (PreviewCache)
            {
                if (PreviewCache.TryGetValue(skin.Id, out var cached) && cached != null)
                {
                    return cached;
                }
            }

            var sprite = BuildPreviewSprite(skin);
            lock (PreviewCache)
            {
                PreviewCache[skin.Id] = sprite;
            }

            return sprite;
        }

        private static Sprite? BuildPreviewSprite(ShadeSkinDefinition skin)
        {
            Texture2D? sheet = null;
            try
            {
                string path = ResolveSpritePath(skin, PreviewSheetName);
                if (!File.Exists(path))
                {
                    return null;
                }

                sheet = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                if (!TryLoadImage(sheet, File.ReadAllBytes(path)))
                {
                    return null;
                }

                // The idle sheet is a single horizontal strip of square frames.
                int frameSize = Mathf.Max(1, sheet.height);
                if (sheet.width < frameSize)
                {
                    return null;
                }

                var pixels = CropFrame(sheet, frameSize);
                int width = frameSize;
                int height = frameSize;
                bool smoothing = ModConfig.Instance.shadeSkinPreviewSmoothing;
                if (smoothing)
                {
                    // The selector draws this at ~900px, where point filtering turns every source
                    // pixel into a visible block. Resampling up to a larger texture keeps the
                    // anti-aliasing the source art already has and leaves the GPU only a small
                    // bilinear stretch - see ShadeSpriteSmoothing for why that beats upscaling and
                    // blurring.
                    int target = ShadeSpriteSmoothing.ChoosePreviewSize(frameSize);
                    pixels = ShadeSpriteSmoothing.Antialias(pixels, frameSize, frameSize, target, out width, out height);
                }

                var preview = new Texture2D(width, height, TextureFormat.ARGB32, false)
                {
                    name = "ShadeSkinPreview_" + skin.Id,
                    filterMode = smoothing ? FilterMode.Bilinear : FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };
                preview.SetPixels32(pixels);
                preview.Apply(false, true);
                return Sprite.Create(preview, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f));
            }
            catch
            {
                return null;
            }
            finally
            {
                if (sheet != null)
                {
                    UnityEngine.Object.Destroy(sheet);
                }
            }
        }

        /// <summary>
        /// The first (leftmost) frame of a horizontal idle strip, as a bottom-up
        /// <see cref="Color32"/> block matching Unity's <c>GetPixels32</c> layout.
        /// </summary>
        private static Color32[] CropFrame(Texture2D sheet, int frameSize)
        {
            var all = sheet.GetPixels32();
            var frame = new Color32[frameSize * frameSize];
            for (int y = 0; y < frameSize; y++)
            {
                Array.Copy(all, y * sheet.width, frame, y * frameSize, frameSize);
            }

            return frame;
        }

        private static IEnumerable<ShadeSkinDefinition> Discover()
        {
            yield return CreateDefaultSkin();

            string skinsRoot;
            try
            {
                skinsRoot = s_spritesRootOverride != null
                    ? Path.Combine(s_spritesRootOverride, SkinsFolderName)
                    : ModPaths.GetAssetDirectory("Knight_Shade_Sprites", SkinsFolderName);
            }
            catch
            {
                yield break;
            }

            string[] folders;
            try
            {
                if (!Directory.Exists(skinsRoot))
                {
                    yield break;
                }

                folders = Directory.GetDirectories(skinsRoot);
            }
            catch
            {
                yield break;
            }

            var byName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var folder in folders)
            {
                string name = new DirectoryInfo(folder).Name;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    byName[name] = folder;
                }
            }

            // Manifest entries come first and in file order; anything else is appended
            // alphabetically so a hand-dropped skin folder shows up with no config edits.
            var emitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in ReadManifest(skinsRoot))
            {
                if (entry.Id == null || !byName.TryGetValue(entry.Id, out var folder) || !emitted.Add(entry.Id))
                {
                    continue;
                }

                yield return new ShadeSkinDefinition(entry.Id, entry.DisplayName ?? entry.Id, folder);
            }

            foreach (var name in byName.Keys.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToArray())
            {
                if (emitted.Add(name))
                {
                    yield return new ShadeSkinDefinition(name, name, byName[name]);
                }
            }
        }

        private static ShadeSkinDefinition CreateDefaultSkin() => new(DefaultSkinId, "Shade", null);

        private static IEnumerable<ManifestEntry> ReadManifest(string skinsRoot)
        {
            ManifestFile? manifest = null;
            try
            {
                string path = Path.Combine(skinsRoot, ManifestFileName);
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        manifest = JsonConvert.DeserializeObject<ManifestFile>(json);
                    }
                }
            }
            catch
            {
            }

            return manifest?.Skins?.Where(s => s != null && !string.IsNullOrWhiteSpace(s.Id))
                ?? Enumerable.Empty<ManifestEntry>();
        }

        private static bool TryLoadImage(Texture2D texture, byte[] bytes)
        {
            try
            {
                var type = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
                var method = type?.GetMethod(
                    "LoadImage",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(Texture2D), typeof(byte[]), typeof(bool) },
                    null);
                if (method != null)
                {
                    method.Invoke(null, new object[] { texture, bytes, false });
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        private sealed class ManifestFile
        {
            [JsonProperty("skins")]
            public List<ManifestEntry>? Skins { get; set; }
        }

        private sealed class ManifestEntry
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("displayName")]
            public string? DisplayName { get; set; }
        }
    }
}
