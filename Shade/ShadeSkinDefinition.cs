#nullable enable

using System;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// One selectable Shade appearance. The built-in skin reads straight from
    /// <c>Assets/Knight_Shade_Sprites</c>; every other skin lives in a folder under
    /// <c>Assets/Knight_Shade_Sprites/Skins</c> and only needs to supply the sheets it
    /// actually changes — anything missing falls back to the built-in set.
    /// </summary>
    internal sealed class ShadeSkinDefinition
    {
        internal ShadeSkinDefinition(string id, string displayName, string? directory)
        {
            Id = id;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? id : displayName;
            Directory = directory;
        }

        /// <summary>Stable identifier persisted in config. Matches the folder name for disk skins.</summary>
        internal string Id { get; }

        /// <summary>Name shown in the skins menu.</summary>
        internal string DisplayName { get; }

        /// <summary>Absolute folder holding this skin's overrides, or null for the built-in skin.</summary>
        internal string? Directory { get; }

        internal bool IsDefault => Directory == null;

        internal bool Matches(string? id) => !string.IsNullOrWhiteSpace(id)
            && string.Equals(Id, id, StringComparison.OrdinalIgnoreCase);
    }
}
