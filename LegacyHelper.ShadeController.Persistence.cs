#nullable disable
using UnityEngine;
using LegacyoftheAbyss.Shade;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        public void RestorePersistentState(int hp, int max, int soul, bool canDamage = true)
        {
            shadeMaxHP = Mathf.Max(1, max);
            shadeHP = Mathf.Clamp(hp, 0, shadeMaxHP);
            shadeSoul = Mathf.Clamp(soul, 0, shadeSoulMax);
            canTakeDamage = canDamage;
            lastSavedCanTakeDamage = canTakeDamage;
        }

        public void FullHealFromBench()
        {
            shadeHP = Mathf.Max(shadeHP, shadeMaxHP);
            if (shadeHP > 0)
            {
                isInactive = false;
                CancelDeathAnimation();
            }
            ShadeRuntime.HandleBenchRest();
            PushShadeStatsToHud();
        }

        public void ReviveToAtLeast(int hp)
        {
            int target = Mathf.Max(1, hp);
            shadeHP = Mathf.Max(shadeHP, target);
            if (shadeHP > 0)
            {
                isInactive = false;
                CancelDeathAnimation();
            }
            PushShadeStatsToHud();
            PersistIfChanged();
        }

        public int GetCurrentHP() => shadeHP;
        public int GetMaxHP() => shadeMaxHP;
        public int GetShadeSoul() => shadeSoul;
        public bool GetCanTakeDamage() => canTakeDamage;

        public void Init(Transform hornet) { hornetTransform = hornet; }

        private void PersistIfChanged()
        {
            if (lastSavedHP != shadeHP || lastSavedMax != shadeMaxHP || lastSavedSoul != shadeSoul || lastSavedCanTakeDamage != canTakeDamage)
            {
                LegacyHelper.SaveShadeState(shadeHP, shadeMaxHP, shadeSoul, canTakeDamage);
                lastSavedHP = shadeHP; lastSavedMax = shadeMaxHP; lastSavedSoul = shadeSoul; lastSavedCanTakeDamage = canTakeDamage;
            }
        }

        private void PushSoulToHud()
        {
            if (cachedHud)
            {
                try { cachedHud.SetShadeSoul(shadeSoul, shadeSoulMax); } catch { }
            }
        }

        private void PushShadeStatsToHud()
        {
            if (cachedHud)
            {
                try { cachedHud.SetShadeStats(shadeHP, shadeMaxHP); } catch { }
            }
        }
    }
}
