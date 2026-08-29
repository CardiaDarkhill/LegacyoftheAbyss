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
    /// Which set of movement rules a character plays by. Separate from the character itself so two
    /// characters could share one, and separate from the render backend, which is about art.
    /// </summary>
    internal enum ShadeMoveset
    {
        /// <summary>Floats on a leash near Hornet, with gravity off.</summary>
        Shade,

        /// <summary>Walks, jumps, clings to walls and pogos.</summary>
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
            ShadeMoveset moveset,
            bool supportsSkins,
            string? previewImagePath = null)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            RenderBackend = renderBackend;
            Moveset = moveset;
            SupportsSkins = supportsSkins;
            PreviewImagePath = previewImagePath;
        }

        internal ShadeCharacterId Id { get; }

        internal string DisplayName { get; }

        internal string Description { get; }

        internal ShadeRenderBackend RenderBackend { get; }

        internal ShadeMoveset Moveset { get; }

        /// <summary>Whether the Characters menu offers a skin list beneath this character.</summary>
        internal bool SupportsSkins { get; }

        /// <summary>
        /// Menu preview art, relative to the assets root. Null for a character whose preview comes
        /// from its skin sheets instead.
        /// </summary>
        internal string? PreviewImagePath { get; }

        /// <summary>How the Characters menu names this character's movement rules.</summary>
        internal string MovesetName => Moveset == ShadeMoveset.Knight ? "Knight Moveset" : "Shade Moveset";

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
                ShadeMoveset.Shade,
                supportsSkins: true),
            new ShadeCharacterDefinition(
                ShadeCharacterId.Knight,
                "Knight",
                "The Knight of Hallownest. Trades soul spells for Hollow Knight movement, and can pogo off Hornet.",
                ShadeRenderBackend.AssetBundle,
                ShadeMoveset.Knight,
                supportsSkins: false,
                // A still rather than a frame out of the bundle: the menu is built at launch, and
                // the bundle is ~54 MB that only a spawned Knight should pay for.
                previewImagePath: "Knight/knight_preview.png"),
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
