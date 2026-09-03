#nullable enable

using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Hollow Knight's spell damage, used as it stands.
    /// <para>
    /// Flat, not scaled off Hornet's needle, and the difference matters enormously. Her silk skills
    /// scale with her needle because they have no upgrades of their own; the Knight's spells do —
    /// Vengeful Spirit becomes Shade Soul, Howling Wraiths becomes Abyss Shriek — so the growth is
    /// already in them. The Knight also has far stronger spell charms to stack on top than her
    /// equivalents: Shaman Stone, Spell Twister and the SOUL-gain charms have no counterpart on her
    /// side.
    /// </para>
    /// <para>
    /// Scaling as well compounds all three. An earlier pass here stated these as multiples of the
    /// needle — Abyss Shriek at four needles a burst, four bursts — which reads as reasonable at
    /// Hollow Knight's starting nail of five and becomes absurd at hers: enough to kill the final
    /// boss of the game in three casts.
    /// </para>
    /// <para>
    /// Several spells land in more than one piece, and the per-piece figure is much smaller than the
    /// spell. Both halves are named here, and <c>ShadeSpellDamageTests</c> checks each spell's
    /// pieces still add up to what the wiki says it does.
    /// </para>
    /// </summary>
    internal static class ShadeSpellDamage
    {
        /// <summary>Vengeful Spirit, one hit.</summary>
        internal const int VengefulSpirit = 15;

        /// <summary>Shade Soul, one hit. Twice its unupgraded form, as in Hollow Knight.</summary>
        internal const int ShadeSoul = 30;

        /// <summary>Howling Wraiths, three hits of this, 39 in all.</summary>
        internal const int HowlingWraithsPerHit = 13;

        internal const int HowlingWraithsHits = 3;

        /// <summary>Abyss Shriek, four hits of this, 80 in all.</summary>
        internal const int AbyssShriekPerHit = 20;

        internal const int AbyssShriekHits = 4;

        /// <summary>The dive itself, the same for both quake spells.</summary>
        internal const int QuakeDive = 15;

        /// <summary>Desolate Dive's shockwave. With the dive, Hollow Knight's 35.</summary>
        internal const int DesolateDiveShockwave = 20;

        /// <summary>
        /// Descending Dark's bursts. Hollow Knight splits them into an asymmetric first — 35 on one
        /// side, 30 on the other — and a second of 15 a side, so where an enemy stands decides
        /// whether it takes 45 or 50. Taken together as the midpoint rather than reproducing the
        /// sides, which puts the spell at 63 against Hollow Knight's 60 to 65.
        /// </summary>
        internal const int DescendingDarkBursts = 48;

        /// <summary>
        /// One piece of a spell, with the charm and difficulty scaling applied. Those two still
        /// scale — Shaman Stone is meant to raise a spell and the difficulty presets are meant to
        /// move every number — it is only the nail that does not enter into it.
        /// </summary>
        internal static int PerHit(int hollowKnightDamage, float charmMultiplier, float configMultiplier)
        {
            float scaled = hollowKnightDamage * charmMultiplier * configMultiplier;
            return Mathf.Max(1, Mathf.RoundToInt(scaled));
        }
    }
}
