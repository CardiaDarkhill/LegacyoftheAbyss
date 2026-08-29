#nullable enable

using System.Collections.Generic;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Reads and writes the per-companion character choice in <see cref="ModConfig"/>. The registry
    /// holds the live value; this is the disk side of it.
    /// </summary>
    internal static class ShadeCharacterManager
    {
        private static readonly Dictionary<ShadeCharacterId, UnityEngine.Sprite?> s_previewCache = new();

        internal static ShadeCharacterDefinition GetSelected(int companionId)
            => ShadeCharacterRegistry.Resolve(ReadConfigId(companionId));

        /// <summary>
        /// Menu preview art for a character, or null when it has none of its own - the Shade's
        /// preview comes from its selected skin's idle sheet instead. Cached, since the menu asks
        /// again every time a row is focused.
        /// </summary>
        internal static UnityEngine.Sprite? GetPreviewSprite(ShadeCharacterDefinition? character)
        {
            if (character?.PreviewImagePath == null)
            {
                return null;
            }

            if (s_previewCache.TryGetValue(character.Id, out var cached))
            {
                return cached;
            }

            // An actual frame of the character's own animation when we can get one; the shipped
            // still is only there for when the bundle is missing.
            var sprite = character.Id == ShadeCharacterId.Knight
                ? Knight.KnightAssets.TryBuildIdlePreview()
                : null;

            sprite ??= LoadPreview(character.PreviewImagePath);
            s_previewCache[character.Id] = sprite;
            return sprite;
        }

        private static UnityEngine.Sprite? LoadPreview(string relativePath)
        {
            string path = System.IO.Path.Combine(ModPaths.Assets, relativePath);
            if (!System.IO.File.Exists(path))
            {
                LegacyHelper.LogWarning($"Character preview image missing at {path}; that row will show no art.");
                return null;
            }

            var texture = new UnityEngine.Texture2D(2, 2, UnityEngine.TextureFormat.ARGB32, false)
            {
                hideFlags = UnityEngine.HideFlags.HideAndDontSave,
            };

            if (!UnityEngine.ImageConversion.LoadImage(texture, System.IO.File.ReadAllBytes(path), markNonReadable: false))
            {
                UnityEngine.Object.Destroy(texture);
                LegacyHelper.LogWarning($"Character preview image at {path} could not be decoded.");
                return null;
            }

            texture.filterMode = UnityEngine.FilterMode.Bilinear;
            texture.wrapMode = UnityEngine.TextureWrapMode.Clamp;

            var sprite = UnityEngine.Sprite.Create(
                texture,
                new UnityEngine.Rect(0f, 0f, texture.width, texture.height),
                new UnityEngine.Vector2(0.5f, 0.5f));
            sprite.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
            return sprite;
        }

        /// <summary>
        /// Persists <paramref name="character"/> for one companion and applies it to the live
        /// companion. Returns true when the choice actually changed.
        /// </summary>
        internal static bool Select(int companionId, ShadeCharacterId character)
        {
            var definition = ShadeCharacterRegistry.Get(character);
            if (GetSelected(companionId).Id == definition.Id)
            {
                return false;
            }

            WriteConfigId(companionId, definition.ConfigId);
            ModConfig.Save();

            if (ShadeCompanionRegistry.TryGet(companionId, out var companion))
            {
                companion.Character = definition.Id;
            }

            return true;
        }

        /// <summary>Pushes every persisted choice onto the registry. Call after config load.</summary>
        internal static void ApplyConfigToRegistry()
        {
            foreach (var companion in ShadeCompanionRegistry.All)
            {
                companion.Character = GetSelected(companion.Id).Id;
            }
        }

        private static string? ReadConfigId(int companionId)
        {
            var list = ModConfig.Instance?.companionCharacters;
            if (list == null || companionId < 0 || companionId >= list.Count)
            {
                return null;
            }

            return list[companionId];
        }

        private static void WriteConfigId(int companionId, string configId)
        {
            var config = ModConfig.Instance;
            if (config == null || companionId < 0)
            {
                return;
            }

            config.companionCharacters ??= new List<string>();

            // Slots between the end of the list and this one default to the Shade rather than
            // shifting every later entry onto the wrong companion.
            while (config.companionCharacters.Count <= companionId)
            {
                config.companionCharacters.Add(ShadeCharacterRegistry.Default.ConfigId);
            }

            config.companionCharacters[companionId] = configId;
        }
    }
}
