#nullable enable

using System;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// One companion's own state: its character, its health/soul, and its charms. Everything here
    /// used to be a static field on <see cref="ShadeRuntime"/>, which is what made a second shade
    /// impossible. <see cref="ShadeRuntime"/> now delegates its API to the primary companion, so
    /// existing call sites keep the old behaviour while the model underneath holds many.
    /// </summary>
    internal sealed class ShadeCompanion
    {
        internal ShadeCompanion(int id, ShadeCharacterId character)
        {
            Id = id;
            _character = character;
            State = new ShadePersistentState();
            Charms = new ShadeCharmInventory();
            Charms.StateChanged += () => CharmsChanged?.Invoke(this);
        }

        private ShadeCharacterId _character;
        private string? _skinId;

        /// <summary>Companion slot index. <see cref="ShadeCompanionRegistry.PrimaryId"/> is the original shade.</summary>
        internal int Id { get; }

        internal bool IsPrimary => Id == ShadeCompanionRegistry.PrimaryId;

        internal ShadePersistentState State { get; }

        internal ShadeCharmInventory Charms { get; }

        /// <summary>Raised when this companion's character or skin changes, so its view can rebuild.</summary>
        internal event Action<ShadeCompanion>? AppearanceChanged;

        /// <summary>This companion's charm inventory changed. Tagged so persistence writes the right slot.</summary>
        internal event Action<ShadeCompanion>? CharmsChanged;

        internal ShadeCharacterId Character
        {
            get => _character;
            set
            {
                if (_character == value)
                {
                    return;
                }

                _character = value;
                AppearanceChanged?.Invoke(this);
            }
        }

        internal ShadeCharacterDefinition CharacterDefinition => ShadeCharacterRegistry.Get(_character);

        /// <summary>
        /// Selected skin id, or null for this character's default. Only meaningful when the
        /// character's <see cref="ShadeCharacterDefinition.SupportsSkins"/> is set.
        /// </summary>
        internal string? SkinId
        {
            get => _skinId;
            set
            {
                if (string.Equals(_skinId, value, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                _skinId = value;
                AppearanceChanged?.Invoke(this);
            }
        }

        /// <summary>The spawned GameObject, or null while despawned. Set before the controller exists.</summary>
        internal UnityEngine.GameObject? Body { get; set; }

        /// <summary>The live controller driving this companion, or null while it is despawned.</summary>
        internal LegacyHelper.ShadeController? Controller { get; set; }

        internal void Reset()
        {
            State.Reset();
            Charms.ResetLoadout();
        }
    }
}
