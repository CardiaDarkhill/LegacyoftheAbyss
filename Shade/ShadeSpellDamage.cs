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
    /// pieces still add up to what the wiki says it does, with and without Shaman Stone.
    /// </para>
    /// </summary>
    internal static class ShadeSpellDamage
    {
        // Shaman Stone, which Hollow Knight sets per spell rather than once. The charm was a single
        // 1.3 here, which is nobody's figure - it was left over from when the base damage scaled off
        // the needle and the exact multiple mattered less than the direction.
        //
        // Vengeful Spirit and Shade Soul.
        private const float ShamanStoneProjectile = 1.33f;

        // Desolate Dive, whose 35 the wiki gives as 53 with the charm.
        private const float ShamanStoneDesolateDive = 1.51f;

        private const float ShamanStoneDescendingDark = 1.47f;

        // Howling Wraiths and Abyss Shriek, 39 to 60 and 80 to 120.
        private const float ShamanStoneScream = 1.5f;

        /// <summary>
        /// One piece of one spell: what it deals, and what Shaman Stone makes of that.
        /// <para>
        /// The two travel together because they are two statements about the same attack, and a
        /// caller that had to fetch them separately could pair the wrong ones - which is exactly
        /// what a single shared charm multiplier amounted to.
        /// </para>
        /// </summary>
        internal readonly struct SpellHit
        {
            internal SpellHit(int damage, float shamanStone)
            {
                Damage = damage;
                ShamanStone = shamanStone;
            }

            /// <summary>What Hollow Knight deals with no charms equipped.</summary>
            internal int Damage { get; }

            /// <summary>What Shaman Stone multiplies that by.</summary>
            internal float ShamanStone { get; }

            /// <summary>
            /// The figure to actually deal. The difficulty setting scales it as it scales
            /// everything else, and a spell never lands for nothing however far it is scaled down -
            /// an attack that does zero reads as broken rather than as weak.
            /// </summary>
            internal int Resolve(bool shamanStoneEquipped, float configMultiplier)
            {
                float scaled = Damage * (shamanStoneEquipped ? ShamanStone : 1f) * configMultiplier;
                return Mathf.Max(1, Mathf.RoundToInt(scaled));
            }
        }

        /// <summary>Vengeful Spirit, one hit. 15, or 20 with Shaman Stone.</summary>
        internal static readonly SpellHit VengefulSpirit = new SpellHit(15, ShamanStoneProjectile);

        /// <summary>Shade Soul, one hit. Twice its unupgraded form, as in Hollow Knight.</summary>
        internal static readonly SpellHit ShadeSoul = new SpellHit(30, ShamanStoneProjectile);

        /// <summary>Howling Wraiths, three hits of this: 39 in all, or 60 with Shaman Stone.</summary>
        internal static readonly SpellHit HowlingWraiths = new SpellHit(13, ShamanStoneScream);

        internal const int HowlingWraithsHits = 3;

        /// <summary>Abyss Shriek, four hits of this: 80 in all, or 120 with Shaman Stone.</summary>
        internal static readonly SpellHit AbyssShriek = new SpellHit(20, ShamanStoneScream);

        internal const int AbyssShriekHits = 4;

        /// <summary>Desolate Dive's own impact. With its shockwave, Hollow Knight's 35 - or 53.</summary>
        internal static readonly SpellHit DesolateDiveImpact = new SpellHit(15, ShamanStoneDesolateDive);

        internal static readonly SpellHit DesolateDiveShockwave = new SpellHit(20, ShamanStoneDesolateDive);

        /// <summary>The same dive, under the charm figure Hollow Knight gives the upgraded spell.</summary>
        internal static readonly SpellHit DescendingDarkImpact = new SpellHit(15, ShamanStoneDescendingDark);

        /// <summary>
        /// Descending Dark's bursts. Hollow Knight splits them into an asymmetric first — 35 on one
        /// side, 30 on the other — and a second of 15 a side, so where an enemy stands decides
        /// whether it takes 45 or 50. Taken together as the midpoint rather than reproducing the
        /// sides, which puts the spell at 63 against Hollow Knight's 60 to 65.
        /// </summary>
        internal static readonly SpellHit DescendingDarkBursts = new SpellHit(48, ShamanStoneDescendingDark);
    }
}
