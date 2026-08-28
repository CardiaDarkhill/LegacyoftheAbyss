#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>Which body a companion wears. Persisted per companion slot, not globally.</summary>
    internal enum ShadeCharacterId
    {
        Shade,
        Knight
    }

    /// <summary>
    /// How a character's frames reach the screen. The Shade animates flat sheets loaded by
    /// <c>LoadShadeSprites</c>; the Knight drives the animator inside KIS's asset bundle. The two
    /// paths share no code, so anything rendering a companion has to branch on this.
    /// </summary>
    internal enum ShadeRenderBackend
    {
        SpriteSheets,
        AssetBundle
    }

    /// <summary>
    /// One selectable companion character. Distinct from <see cref="ShadeSkinDefinition"/>: a skin
    /// re-textures the Shade, a character changes which abilities exist and how they animate.
    /// </summary>
    internal sealed class ShadeCharacterDefinition
    {
        internal ShadeCharacterDefinition(
            ShadeCharacterId id,
            string displayName,
            string description,
            ShadeRenderBackend renderBackend,
            bool supportsSkins)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            RenderBackend = renderBackend;
            SupportsSkins = supportsSkins;
        }

        internal ShadeCharacterId Id { get; }

        internal string DisplayName { get; }

        internal string Description { get; }

        internal ShadeRenderBackend RenderBackend { get; }

        /// <summary>Whether the Characters menu offers a skin list beneath this character.</summary>
        internal bool SupportsSkins { get; }

        /// <summary>Stable identifier persisted in config.</summary>
        internal string ConfigId => Id.ToString();

        internal bool Matches(string? id) => !string.IsNullOrWhiteSpace(id)
            && string.Equals(ConfigId, id, StringComparison.OrdinalIgnoreCase);
    }

    internal static class ShadeCharacterRegistry
    {
        private static readonly ShadeCharacterDefinition[] s_characters =
        {
            new ShadeCharacterDefinition(
                ShadeCharacterId.Shade,
                "Shade",
                "The vessel's shade. Fights with nail and soul, and wears any installed skin.",
                ShadeRenderBackend.SpriteSheets,
                supportsSkins: true),
            new ShadeCharacterDefinition(
                ShadeCharacterId.Knight,
                "Knight",
                "The Knight of Hallownest. Trades soul spells for Hollow Knight movement, and can pogo off Hornet.",
                ShadeRenderBackend.AssetBundle,
                supportsSkins: false),
        };

        internal static IReadOnlyList<ShadeCharacterDefinition> Characters => s_characters;

        internal static ShadeCharacterDefinition Default => s_characters[0];

        internal static ShadeCharacterDefinition Get(ShadeCharacterId id)
            => s_characters.FirstOrDefault(c => c.Id == id) ?? Default;

        /// <summary>Resolves a persisted config id, falling back to the Shade for unknown values.</summary>
        internal static ShadeCharacterDefinition Resolve(string? configId)
            => s_characters.FirstOrDefault(c => c.Matches(configId)) ?? Default;
    }
}
