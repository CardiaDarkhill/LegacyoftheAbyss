#nullable enable
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// One save slot's difficulty, lifted out of <see cref="ModConfig"/> so it can be stored per
    /// profile rather than once for the whole install.
    /// <para>
    /// The live values still live on <see cref="ModConfig"/>, because every read site in the mod
    /// reads them from there and routing all of those through a slot lookup would be a large change
    /// for no gain. Instead this is the disk side of them: the active slot's settings are pushed
    /// onto the config when a slot is entered, and pulled back off it whenever the Difficulty screen
    /// changes something - the same shape as <c>ShadeCharmInventory</c> and its slot record.
    /// </para>
    /// <para>
    /// A slot with nothing stored adopts whatever is in <c>config.json</c>, so a save that predates
    /// this keeps the difficulty it was being played at instead of snapping to a default.
    /// </para>
    /// </summary>
    internal sealed class ShadeDifficultySettings
    {
        public float HornetNeedleDamage { get; set; } = 1f;

        public float HornetSilkSkillDamage { get; set; } = 1f;

        public float ShadeNailDamage { get; set; } = 1f;

        public float ShadeSpellDamage { get; set; } = 1f;

        public int BindHornetHeal { get; set; } = 3;

        public int BindShadeHeal { get; set; } = 2;

        public int FocusHornetHeal { get; set; } = 1;

        public int FocusShadeHeal { get; set; } = 1;

        public float ShadeMaskFraction { get; set; } = ModConfig.DefaultShadeMaskFraction;

        public bool ShadeFocusAtFullMasks { get; set; }

        /// <summary>Reads the live difficulty off the config.</summary>
        public static ShadeDifficultySettings CaptureFrom(ModConfig? config)
        {
            if (config == null)
            {
                return new ShadeDifficultySettings();
            }

            return new ShadeDifficultySettings
            {
                HornetNeedleDamage = config.hornetDamageMultiplier,
                HornetSilkSkillDamage = config.hornetSilkSkillDamageMultiplier,
                ShadeNailDamage = config.shadeDamageMultiplier,
                ShadeSpellDamage = config.shadeSpellDamageMultiplier,
                BindHornetHeal = config.bindHornetHeal,
                BindShadeHeal = config.bindShadeHeal,
                FocusHornetHeal = config.focusHornetHeal,
                FocusShadeHeal = config.focusShadeHeal,
                ShadeMaskFraction = config.shadeMaskFraction,
                ShadeFocusAtFullMasks = config.shadeFocusAtFullMasks,
            };
        }

        /// <summary>Writes these settings back onto the live config.</summary>
        public void ApplyTo(ModConfig? config)
        {
            if (config == null)
            {
                return;
            }

            config.hornetDamageMultiplier = HornetNeedleDamage;
            config.hornetSilkSkillDamageMultiplier = HornetSilkSkillDamage;
            config.shadeDamageMultiplier = ShadeNailDamage;
            config.shadeSpellDamageMultiplier = ShadeSpellDamage;
            config.bindHornetHeal = BindHornetHeal;
            config.bindShadeHeal = BindShadeHeal;
            config.focusHornetHeal = FocusHornetHeal;
            config.focusShadeHeal = FocusShadeHeal;
            config.shadeMaskFraction = ShadeMaskFraction;
            config.shadeFocusAtFullMasks = ShadeFocusAtFullMasks;
        }

        public ShadeDifficultySettings Clone()
        {
            return (ShadeDifficultySettings)MemberwiseClone();
        }

        /// <summary>
        /// Whether two sets are the same difficulty. Used to keep a slot from being rewritten - and
        /// its file rewritten - on every menu refresh that changed nothing.
        /// </summary>
        public bool Matches(ShadeDifficultySettings? other)
        {
            if (other == null)
            {
                return false;
            }

            return Mathf.Approximately(HornetNeedleDamage, other.HornetNeedleDamage)
                && Mathf.Approximately(HornetSilkSkillDamage, other.HornetSilkSkillDamage)
                && Mathf.Approximately(ShadeNailDamage, other.ShadeNailDamage)
                && Mathf.Approximately(ShadeSpellDamage, other.ShadeSpellDamage)
                && BindHornetHeal == other.BindHornetHeal
                && BindShadeHeal == other.BindShadeHeal
                && FocusHornetHeal == other.FocusHornetHeal
                && FocusShadeHeal == other.FocusShadeHeal
                && Mathf.Approximately(ShadeMaskFraction, other.ShadeMaskFraction)
                && ShadeFocusAtFullMasks == other.ShadeFocusAtFullMasks;
        }
    }
}
