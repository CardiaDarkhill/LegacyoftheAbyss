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

            if (s_previewCache.TryGetValue(character.Id, out var cached)
                && (ReferenceEquals(cached, null) || cached))
            {
                // ReferenceEquals rather than ==, because the two nulls need opposite treatment and
                // Unity's operator reports both. A real null is a cached *absence* and must stay
                // cached, or a character with no art re-reads the file and re-warns every time its
                // row is focused. A destroyed sprite is a cache that has gone stale - the Knight's
                // preview is built from knight.bundle, and KnightAssets.Unload tears that down with
                // unloadAllLoadedObjects, which destroys the texture behind this sprite while
                // leaving this dictionary (in another class) holding it. A plain hit would then
                // hand back a dead sprite for the rest of the session and the row would simply
                // draw nothing.
                return cached;
            }

            // An actual frame of the character's own animation when we can get one; the shipped
            // still is only there for when the bundle is missing.
            var sprite = character.Id == ShadeCharacterId.Knight
                ? Knight.KnightAssets.TryBuildIdlePreview()
                : null;

            // Unity's null, not ??=, for the same reason.
            if (!sprite)
            {
                sprite = LoadPreview(character.PreviewImagePath);
            }

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
            DisableAiForCharactersItCannotDrive(definition);
            ModConfig.Save();

            if (ShadeCompanionRegistry.TryGet(companionId, out var companion))
            {
                companion.Character = definition.Id;
            }

            return true;
        }

        /// <summary>
        /// Switches the Shade AI off when a character it cannot drive is chosen, rather than leaving
        /// it set and unused.
        /// <para>
        /// The AI steers by synthesising the inputs a second player would give, which works for a
        /// body that flies anywhere in a straight line and not for one that has to plan a jump - so
        /// it has always stood down for the Knight. Standing down is not the same as being off,
        /// though: the setting stayed on, and the instant the player swapped back to the Shade the
        /// AI took it out of the second player's hands. It also took Hornet's keyboard with it,
        /// because an AI-held Shade means there is no two-player device split left to serve.
        /// </para>
        /// <para>
        /// Turning it back on is a deliberate act on the Shade AI screen, whose row already refuses
        /// while the Knight is worn.
        /// </para>
        /// </summary>
        private static void DisableAiForCharactersItCannotDrive(ShadeCharacterDefinition definition)
        {
            if (definition.Moveset != ShadeMoveset.Knight)
            {
                return;
            }

            var config = ModConfig.Instance;
            if (config == null || !config.shadeAiEnabled)
            {
                return;
            }

            config.shadeAiEnabled = false;

            foreach (var shade in LegacyHelper.ShadeController.ActiveInstances)
            {
                // persist:false - the caller saves once, straight after this.
                shade?.SetShadeAiEnabled(false, persist: false);
            }
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
