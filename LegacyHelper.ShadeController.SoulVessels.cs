#nullable disable
using UnityEngine;
using LegacyoftheAbyss.Shade;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        /// <summary>
        /// The companion's Soul Vessels. The arithmetic lives in <see cref="ShadeSoulVessels"/>,
        /// which has no Unity object in it and is where the rules are stated and tested; this is
        /// the pool itself, what fills it, and the clock that empties it back into the meter.
        /// </summary>
        internal const int SoulPerVessel = ShadeSoulVessels.SoulPerVessel;

        internal const int MaxSoulVessels = ShadeSoulVessels.MaxVessels;

        /// <summary>How many vessels this companion has earned, 0 to <see cref="MaxSoulVessels"/>.</summary>
        private int shadeSoulVessels;

        /// <summary>Soul held in the vessels, 0 to <see cref="GetShadeVesselCapacity"/>.</summary>
        private int shadeVesselSoul;

        /// <summary>Charm additions to the nail's vessel rate. Parallel to <c>charmSoulGainBonus</c>.</summary>
        private int charmVesselSoulGainBonus;

        /// <summary>How long the meter has had room. The drain waits out a delay before starting.</summary>
        private float vesselDrainIdleTimer;

        /// <summary>The part of a drained point not yet whole, so a fractional rate stays honest.</summary>
        private float vesselDrainRemainder;

        /// <summary>
        /// The meter as it stood at the last tick, so that spending from it can be seen.
        /// <para>
        /// Watched rather than reported. Every spender would otherwise have to remember to say so -
        /// there are five of them today and the focus drain spends a fraction of a mask at a time -
        /// and one that forgot would look exactly like this bug again. A fall in the meter is a
        /// spend whoever caused it, and the drain's own refills raise it, so they cannot be
        /// mistaken for one.
        /// </para>
        /// </summary>
        private int vesselDrainLastSoul = -1;

        public int GetShadeVesselSoul() => Mathf.Max(0, shadeVesselSoul);

        public int GetShadeVesselCount() => Mathf.Clamp(shadeSoulVessels, 0, MaxSoulVessels);

        public int GetShadeVesselCapacity() => ShadeSoulVessels.Capacity(shadeSoulVessels);

        /// <summary>Charm hook counterpart to <c>AddSoulGainBonus</c>, for the reduced vessel rate.</summary>
        internal void AddVesselSoulGainBonus(int amount)
        {
            charmVesselSoulGainBonus = Mathf.Clamp(charmVesselSoulGainBonus + amount, -99, 99);
        }

        /// <summary>Restores the reserve from a save. Clamped against whatever capacity is derived.</summary>
        internal void RestoreVesselSoul(int soul)
        {
            RefreshSoulVesselCount();
            shadeVesselSoul = Mathf.Clamp(soul, 0, GetShadeVesselCapacity());
            vesselDrainIdleTimer = 0f;
            vesselDrainRemainder = 0f;
            vesselDrainLastSoul = shadeSoul;
        }

        /// <summary>
        /// Reads the vessel count off Hornet's silk maximum.
        /// <para>
        /// Derived every frame rather than saved, so it cannot drift out of step with her save file
        /// and needs no migration for a run that predates the feature. Returns false when it did not
        /// change, which is the usual case.
        /// </para>
        /// </summary>
        private bool RefreshSoulVesselCount()
        {
            int earned = 0;
            try
            {
                var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
                if (pd != null)
                {
                    earned = ShadeSoulVessels.VesselsFromSilkMax(pd.silkMax);
                }
            }
            catch
            {
            }

            if (!ModConfig.Instance.shadeSoulVesselsEnabled)
            {
                earned = 0;
            }

            if (earned == shadeSoulVessels)
            {
                return false;
            }

            shadeSoulVessels = earned;

            // A capacity that shrank has to take the reserve down with it, or the HUD is asked to
            // draw more soul than it has vessels to draw it in.
            int capacity = GetShadeVesselCapacity();
            if (shadeVesselSoul > capacity)
            {
                shadeVesselSoul = capacity;
            }

            return true;
        }

        /// <summary>
        /// Adds soul the way every source of it should: the meter first, and whatever will not fit
        /// into the vessels.
        /// </summary>
        internal void AddSoul(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            RefreshSoulVesselCount();

            ShadeSoulVessels.Add(
                amount, shadeSoul, shadeSoulMax, shadeVesselSoul, GetShadeVesselCapacity(),
                out int newSoul, out int newReserve);

            if (newSoul == shadeSoul && newReserve == shadeVesselSoul)
            {
                return;
            }

            shadeSoul = newSoul;
            shadeVesselSoul = newReserve;
            PushSoulToHud();
            PersistIfChanged();
        }

        /// <summary>
        /// The debug key's counterpart to <see cref="AddSoul"/>: takes the meter down first and the
        /// reserve after it, so holding the key can empty both. Spending in play never touches the
        /// reserve - only this does, and only because otherwise there is no way to get back to an
        /// empty vessel to watch one fill.
        /// </summary>
        internal void DebugSpendSoul(int amount)
        {
            ShadeSoulVessels.Spend(amount, shadeSoul, shadeVesselSoul, out int newSoul, out int newReserve);
            if (newSoul == shadeSoul && newReserve == shadeVesselSoul)
            {
                return;
            }

            shadeSoul = newSoul;
            shadeVesselSoul = newReserve;
            PushSoulToHud();
            PersistIfChanged();
        }

        /// <summary>
        /// What one nail hit is worth. Hollow Knight pays a reduced rate once the meter is full and
        /// the soul would be going into the vessels - 6 rather than 11, with Soul Catcher and Soul
        /// Eater each worth less than they are to the meter. Every other source pays in full.
        /// </summary>
        private int NailSoulGain()
        {
            if (shadeSoul < shadeSoulMax)
            {
                return Mathf.Max(0, soulGainPerHit + charmSoulGainBonus);
            }

            return Mathf.Max(0, ShadeSoulVessels.NailVesselSoulGain + charmVesselSoulGainBonus);
        }

        /// <summary>
        /// Pours the reserve back into the meter once the meter has had room for a moment.
        /// <para>
        /// The delay is what makes the vessels read as a reserve rather than as a longer bar: spend
        /// the meter and it refills, but not so fast that it was never spent. Both it and the rate
        /// are config knobs, because a second and 33 a second are a starting guess about feel and
        /// the only way to judge feel is to play it.
        /// </para>
        /// </summary>
        private void UpdateSoulVessels(float deltaTime)
        {
            RefreshSoulVesselCount();

            // The delay is counted from the last time soul was spent, not from the last time the
            // meter had room. Casting mid-drain used to leave the reserve pouring straight back in
            // behind the spell, which reads as the spell being free - the vessels are a reserve to
            // be earned back, so every spend starts the wait again.
            int previousSoul = vesselDrainLastSoul;
            vesselDrainLastSoul = shadeSoul;
            if (ShadeSoulVessels.IsSpend(previousSoul, shadeSoul))
            {
                vesselDrainIdleTimer = 0f;
                vesselDrainRemainder = 0f;
            }

            if (shadeVesselSoul <= 0 || shadeSoul >= shadeSoulMax)
            {
                vesselDrainIdleTimer = 0f;
                vesselDrainRemainder = 0f;
                return;
            }

            if (deltaTime <= 0f)
            {
                return;
            }

            var config = ModConfig.Instance;
            vesselDrainIdleTimer += deltaTime;
            if (vesselDrainIdleTimer < Mathf.Max(0f, config.shadeSoulVesselDrainDelay))
            {
                return;
            }

            float rate = Mathf.Max(1f, config.shadeSoulVesselDrainRate);
            vesselDrainRemainder += rate * deltaTime;
            int whole = Mathf.FloorToInt(vesselDrainRemainder);
            if (whole <= 0)
            {
                return;
            }

            vesselDrainRemainder -= whole;

            int moved = Mathf.Min(whole, Mathf.Min(shadeVesselSoul, shadeSoulMax - shadeSoul));
            if (moved <= 0)
            {
                return;
            }

            shadeVesselSoul -= moved;
            shadeSoul += moved;
            PushSoulToHud();
            PersistIfChanged();
        }
    }
}
