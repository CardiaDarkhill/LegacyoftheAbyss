#nullable enable
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Represents the portions of the shade's runtime state that need to persist between scene loads.
    /// This class intentionally avoids any references to Unity components so that it can be tested in isolation.
    /// </summary>
    internal sealed class ShadePersistentState
    {
        private const int MaxSpellProgress = 6;

        public int CurrentHP { get; private set; } = -1;
        public int MaxHP { get; private set; } = -1;
        public int Soul { get; private set; } = -1;
        public bool CanTakeDamage { get; private set; } = true;
        public int SpellProgress { get; private set; }
            = 0;

        public bool HasData => MaxHP > 0;

        public ShadePersistentState Clone()
        {
            return new ShadePersistentState
            {
                CurrentHP = CurrentHP,
                MaxHP = MaxHP,
                Soul = Soul,
                CanTakeDamage = CanTakeDamage,
                SpellProgress = SpellProgress
            };
        }

        public void Capture(int currentHp, int maxHp, int soul, bool? canTakeDamage = null)
        {
            MaxHP = Mathf.Max(1, maxHp);
            CurrentHP = Mathf.Clamp(currentHp, 0, MaxHP);
            Soul = Mathf.Max(0, soul);
            if (canTakeDamage.HasValue)
            {
                CanTakeDamage = canTakeDamage.Value;
            }
        }

        public void Reset()
        {
            CurrentHP = -1;
            MaxHP = -1;
            Soul = -1;
            CanTakeDamage = true;
            SpellProgress = 0;
        }

        public void ForceMinimumHealth(int minimum)
        {
            if (!HasData)
            {
                return;
            }

            int required = Mathf.Max(0, minimum);
            CurrentHP = Mathf.Clamp(Mathf.Max(CurrentHP, required), 0, MaxHP);
        }

        public void AdvanceSpellProgress()
        {
            SpellProgress = Mathf.Clamp(SpellProgress + 1, 0, MaxSpellProgress);
        }

        public void SetSpellProgress(int progress)
        {
            SpellProgress = Mathf.Clamp(progress, 0, MaxSpellProgress);
        }
    }
}
