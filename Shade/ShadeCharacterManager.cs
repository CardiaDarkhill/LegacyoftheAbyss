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
        internal static ShadeCharacterDefinition GetSelected(int companionId)
            => ShadeCharacterRegistry.Resolve(ReadConfigId(companionId));

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
