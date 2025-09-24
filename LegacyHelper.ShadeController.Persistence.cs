#nullable disable
using UnityEngine;
using LegacyoftheAbyss.Shade;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        public void RestorePersistentState(int hp, int max, int baseMax, int lifeblood, int lifebloodMax, int soul, bool canDamage = true)
        {
            baseShadeMaxHP = Mathf.Max(0, baseMax);
            shadeMaxHP = Mathf.Max(0, max);
            shadeHP = Mathf.Clamp(hp, 0, shadeMaxHP);
            pendingRestoredLifebloodMax = Mathf.Max(0, lifebloodMax);
            pendingRestoredLifeblood = Mathf.Clamp(lifeblood, 0, pendingRestoredLifebloodMax);
            shadeLifebloodMax = pendingRestoredLifebloodMax;
            shadeLifeblood = pendingRestoredLifeblood;
            shadeSoul = Mathf.Clamp(soul, 0, shadeSoulMax);
            canTakeDamage = canDamage;
            lastSavedCanTakeDamage = canTakeDamage;

            if (baseShadeMaxHP <= 0)
            {
                baseShadeMaxHP = shadeMaxHP > 0 ? shadeMaxHP : 1;
            }
        }

        public void FullHealFromBench()
        {
            ApplyCharmHealthModifiers(refillLifeblood: true);
            shadeHP = shadeMaxHP;
            shadeLifeblood = shadeLifebloodMax;
            hivebloodPendingLifebloodRestore = false;
            if (GetTotalCurrentHealth() > 0)
            {
                isInactive = false;
                CancelDeathAnimation();
            }
            ShadeRuntime.HandleBenchRest();
            PushShadeStatsToHud(suppressDamageAudio: true);
        }

        public void ReviveToAtLeast(int hp, bool allowLifeblood = false)
        {
            int target = Mathf.Max(0, hp);
            shadeHP = Mathf.Clamp(Mathf.Max(shadeHP, target), 0, shadeMaxHP);

            if (allowLifeblood && shadeHP < target && shadeLifeblood < shadeLifebloodMax)
            {
                int deficit = Mathf.Max(0, target - shadeHP);
                int toRestore = Mathf.Min(deficit, shadeLifebloodMax - shadeLifeblood);
                shadeLifeblood += toRestore;
            }

            if (GetTotalCurrentHealth() > 0)
            {
                isInactive = false;
                CancelDeathAnimation();
            }
            PushShadeStatsToHud(suppressDamageAudio: true);
            PersistIfChanged();
        }

        public int GetCurrentHP() => Mathf.Max(0, shadeHP) + Mathf.Max(0, shadeLifeblood);
        public int GetCurrentNormalHP() => Mathf.Max(0, shadeHP);
        public int GetMaxHP() => Mathf.Max(0, shadeMaxHP) + Mathf.Max(0, shadeLifebloodMax);
        public int GetMaxNormalHP() => Mathf.Max(0, shadeMaxHP);
        public int GetBaseMaxHP() => Mathf.Max(0, baseShadeMaxHP);
        public int GetCurrentLifeblood() => Mathf.Max(0, shadeLifeblood);
        public int GetMaxLifeblood() => Mathf.Max(0, shadeLifebloodMax);
        public int GetShadeSoul() => shadeSoul;
        public int GetShadeSoulMax() => shadeSoulMax;
        public bool GetCanTakeDamage() => canTakeDamage;

        public void Init(Transform hornet) { hornetTransform = hornet; }

        private void PersistIfChanged()
        {
            if (persistenceSuppressionDepth > 0)
            {
                return;
            }

            if (lastSavedHP != shadeHP
                || lastSavedMax != shadeMaxHP
                || lastSavedLifeblood != shadeLifeblood
                || lastSavedLifebloodMax != shadeLifebloodMax
                || lastSavedSoul != shadeSoul
                || lastSavedCanTakeDamage != canTakeDamage)
            {
                LegacyHelper.SaveShadeState(shadeHP, shadeMaxHP, shadeLifeblood, shadeLifebloodMax, shadeSoul, canTakeDamage, baseShadeMaxHP);
                lastSavedHP = shadeHP;
                lastSavedMax = shadeMaxHP;
                lastSavedLifeblood = shadeLifeblood;
                lastSavedLifebloodMax = shadeLifebloodMax;
                lastSavedSoul = shadeSoul;
                lastSavedCanTakeDamage = canTakeDamage;
            }
        }

        private void PushSoulToHud()
        {
            if (cachedHud)
            {
                try { cachedHud.SetShadeSoul(shadeSoul, shadeSoulMax); } catch { }
            }
        }

        private void PushShadeStatsToHud(bool suppressDamageAudio = false)
        {
            if (cachedHud)
            {
                try
                {
                    if (suppressDamageAudio)
                    {
                        cachedHud.SuppressNextShadeDamageSfx();
                    }
                    cachedHud.SetShadeStats(shadeHP, shadeMaxHP, shadeLifeblood, shadeLifebloodMax);
                    cachedHud.SetShadeOvercharmed(ShadeRuntime.Charms?.IsOvercharmed ?? false);
                }
                catch
                {
                }
            }
        }

        private int GetTotalCurrentHealth()
        {
            return Mathf.Max(0, shadeHP) + Mathf.Max(0, shadeLifeblood);
        }
    }
}
