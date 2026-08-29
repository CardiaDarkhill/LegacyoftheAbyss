#nullable enable

namespace LegacyoftheAbyss.Shade.Knight
{
    /// <summary>
    /// The slice of Hornet's save data that decides what the Knight can do, lifted out of
    /// <c>PlayerData</c> so the mapping can be exercised without a Unity player loop.
    /// Field names are Hornet's, not the Knight's.
    /// </summary>
    internal struct HornetProgressSnapshot
    {
        public bool HasDash;
        public bool HasWalljump;
        public bool HasDoubleJump;
        public bool HasHarpoonDash;
        public bool HasNeedleThrow;
        public bool HasSilkCharge;
        public bool HasParry;
        public bool HasSilkBossNeedle;
        public bool HasThreadSphere;
        public bool HasSilkBomb;
    }

    /// <summary>What the Knight has unlocked, derived from Hornet's progression.</summary>
    internal readonly struct KnightAbilities
    {
        internal KnightAbilities(
            bool mothwingCloak,
            bool mantisClaw,
            bool doubleJump,
            bool shadeCloak,
            int fireballLevel,
            int quakeLevel,
            int screamLevel)
        {
            MothwingCloak = mothwingCloak;
            MantisClaw = mantisClaw;
            DoubleJump = doubleJump;
            ShadeCloak = shadeCloak;
            FireballLevel = fireballLevel;
            QuakeLevel = quakeLevel;
            ScreamLevel = screamLevel;
        }

        internal bool MothwingCloak { get; }
        internal bool MantisClaw { get; }
        internal bool DoubleJump { get; }
        internal bool ShadeCloak { get; }
        internal int FireballLevel { get; }
        internal int QuakeLevel { get; }
        internal int ScreamLevel { get; }

        /// <summary>Shade Cloak is Mothwing plus intangibility, so a dash exists either way.</summary>
        internal bool CanDash => MothwingCloak || ShadeCloak;

        internal static KnightAbilities None => new KnightAbilities(
            false, false, false, false, 0, 0, 0);
    }

    /// <summary>
    /// Hornet's progression to the Knight's, following Knight in Silksong's own sync table so the
    /// two mods gate the same abilities at the same points.
    /// <para>
    /// Only what the Knight can actually do is mapped. KIS also syncs Crystal Heart, nail arts and
    /// the Dream Nail; none of those are implemented for a companion here, and carrying them as
    /// flags nothing reads is how a mapping ships dead.
    /// </para>
    /// </summary>
    internal static class KnightAbilityMap
    {
        internal static KnightAbilities FromHornet(in HornetProgressSnapshot hornet)
        {
            // Sprint is the Knight's Mothwing Cloak.
            bool mothwing = hornet.HasDash;

            // Harpoon Dash, not the start of Act 3: it keeps a dash upgrade behind a dash upgrade.
            bool shadeCloak = hornet.HasHarpoonDash;

            int fireball = 0;
            if (hornet.HasNeedleThrow) fireball = 1;
            if (hornet.HasSilkCharge) fireball = 2;

            int quake = 0;
            if (hornet.HasParry) quake = 1;
            if (hornet.HasSilkBossNeedle) quake = 2;

            int scream = 0;
            if (hornet.HasThreadSphere) scream = 1;
            if (hornet.HasSilkBomb) scream = 2;

            return new KnightAbilities(
                mothwingCloak: mothwing,
                mantisClaw: hornet.HasWalljump,
                doubleJump: hornet.HasDoubleJump,
                shadeCloak: shadeCloak,
                fireballLevel: fireball,
                quakeLevel: quake,
                screamLevel: scream);
        }
    }
}
