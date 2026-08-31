#nullable enable
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// The arithmetic behind the Soul Vessels, with no Unity object in sight so it can be tested.
    /// <para>
    /// The vessels are a reserve behind the soul meter, not extra room in it. Nothing is ever spent
    /// out of them: spells and Focus take from the meter alone, and the vessels pour back into it a
    /// moment later. That is the whole feel of the mechanic - three casts in a row rather than one
    /// long bar - and it is why the companion carries a second pool rather than a larger maximum.
    /// </para>
    /// <para>
    /// The reserve is one number rather than three. "No soul may ever be in a higher vessel if the
    /// lower one is not yet full" is then not a rule to enforce but the only thing that can be
    /// represented: vessel <c>i</c> holds <c>clamp(reserve - 33i, 0, 33)</c>, so filling from the
    /// bottom and draining from the top are the same arithmetic read in two directions.
    /// </para>
    /// </summary>
    internal static class ShadeSoulVessels
    {
        internal const int SoulPerVessel = 33;

        /// <summary>The most vessels obtainable, doubling the carried soul at 99 + 3x33.</summary>
        internal const int MaxVessels = 3;

        /// <summary>
        /// Hornet's silk maximum at the start of a run, from <c>PlayerData</c>. Her spool upgrades
        /// are counted from here.
        /// </summary>
        internal const int BaseHornetSilkMax = 9;

        /// <summary>How many points of Hornet's silk maximum buy one vessel.</summary>
        internal const int SilkMaxPerVessel = 2;

        /// <summary>Soul the nail is worth once the meter is full and it is filling the vessels.</summary>
        internal const int NailVesselSoulGain = 6;

        internal static int Capacity(int vesselCount) => Mathf.Clamp(vesselCount, 0, MaxVessels) * SoulPerVessel;

        /// <summary>
        /// How many vessels Hornet's own progression has earned: one for every two points her silk
        /// maximum has risen above where it starts.
        /// </summary>
        internal static int VesselsFromSilkMax(int silkMax)
        {
            int gained = silkMax - BaseHornetSilkMax;
            if (gained <= 0)
            {
                return 0;
            }

            return Mathf.Clamp(gained / SilkMaxPerVessel, 0, MaxVessels);
        }

        /// <summary>What the vessel at <paramref name="index"/> holds, counting from the bottom.</summary>
        internal static int HeldInVessel(int reserve, int index)
        {
            if (index < 0)
            {
                return 0;
            }

            return Mathf.Clamp(reserve - (index * SoulPerVessel), 0, SoulPerVessel);
        }

        /// <summary>
        /// Which of the five drawn states a vessel holding <paramref name="held"/> shows.
        /// <para>
        /// Full is exactly full and empty is exactly empty; anything in between shows at least a
        /// quarter, so a single point of soul is visibly there rather than rounding away.
        /// </para>
        /// </summary>
        internal static int StageFor(int held)
        {
            if (held <= 0)
            {
                return 0;
            }

            if (held >= SoulPerVessel)
            {
                return 4;
            }

            return Mathf.Clamp(Mathf.Max(1, held * 4 / SoulPerVessel), 1, 3);
        }

        /// <summary>
        /// Adds soul the way every source of it should: the meter first, and whatever will not fit
        /// into the vessels. Anything past both is lost, as it is in Hollow Knight.
        /// </summary>
        internal static void Add(int amount, int soul, int soulMax, int reserve, int capacity, out int newSoul, out int newReserve)
        {
            newSoul = soul;
            newReserve = reserve;
            if (amount <= 0)
            {
                return;
            }

            int intoMeter = Mathf.Min(amount, Mathf.Max(0, soulMax - soul));
            newSoul = soul + intoMeter;

            int overflow = amount - intoMeter;
            if (overflow > 0)
            {
                newReserve = Mathf.Min(reserve + overflow, Mathf.Max(0, capacity));
            }
        }

        /// <summary>
        /// Takes the meter down first and the reserve after it. Only the debug key spends this way -
        /// spending in play never reaches the vessels, which is the point of them.
        /// </summary>
        internal static void Spend(int amount, int soul, int reserve, out int newSoul, out int newReserve)
        {
            newSoul = soul;
            newReserve = reserve;
            if (amount <= 0)
            {
                return;
            }

            int fromMeter = Mathf.Min(amount, Mathf.Max(0, soul));
            newSoul = soul - fromMeter;

            int remaining = amount - fromMeter;
            if (remaining > 0)
            {
                newReserve = Mathf.Max(0, reserve - remaining);
            }
        }
    }
}
