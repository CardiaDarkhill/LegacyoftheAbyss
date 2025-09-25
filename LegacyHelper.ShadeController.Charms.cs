#nullable disable
using LegacyoftheAbyss.Shade;
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        public void RecomputeCharmLoadout()
        {
            if (!baselineStatsInitialized)
            {
                pendingCharmLoadoutRecompute = true;
                return;
            }

            EnterPersistenceSuppression();
            try
            {
                pendingCharmLoadoutRecompute = false;

                maxDistance = baseMaxDistance;
                softLeashRadius = baseSoftLeashRadius;
                hardLeashRadius = baseHardLeashRadius;
                snapLeashRadius = baseSnapLeashRadius;
                sprintMultiplier = baseSprintMultiplier;
                fireCooldown = baseFireCooldown;
                nailCooldown = baseNailCooldown;
                focusSoulCost = baseFocusSoulCost;
                projectileSoulCost = baseProjectileSoulCost;
                shriekSoulCost = baseShriekSoulCost;
                quakeSoulCost = baseQuakeSoulCost;
                soulGainPerHit = baseSoulGainPerHit;
                focusChannelTime = baseFocusChannelTime;
                focusHealRange = baseFocusHealRange;
                teleportChannelTime = baseTeleportChannelTime;
                hitKnockbackForce = baseHitKnockbackForce;
                shadeMaxHP = baseShadeMaxHP;
                ResetCharmDerivedStats();

                var inventory = ShadeRuntime.Charms;
                var loadout = inventory?.GetEquippedDefinitions();
                ApplyCharmLoadout(loadout);

                maxDistance = Mathf.Max(6f, maxDistance);
                softLeashRadius = Mathf.Max(4f, softLeashRadius);
                hardLeashRadius = Mathf.Max(softLeashRadius, hardLeashRadius);
                snapLeashRadius = Mathf.Max(hardLeashRadius, snapLeashRadius);
            }
            finally
            {
                ExitPersistenceSuppression();
            }

            PushSoulToHud();
            PushShadeStatsToHud(suppressDamageAudio: true);
            PersistIfChanged();
        }

        internal void MultiplyNailDamage(float factor)
        {
            if (factor <= 0f)
                return;

            charmNailDamageMultiplier = Mathf.Clamp(charmNailDamageMultiplier * factor, 0.1f, 10f);
        }

        internal void MultiplySpellDamage(float factor)
        {
            if (factor <= 0f)
                return;

            charmSpellDamageMultiplier = Mathf.Clamp(charmSpellDamageMultiplier * factor, 0.1f, 10f);
        }

        internal void MultiplyNailScale(float factor)
        {
            if (factor <= 0f)
                return;

            charmNailScaleMultiplier = Mathf.Clamp(charmNailScaleMultiplier * factor, 0.5f, 3f);
        }

        internal void AddSoulGainBonus(int amount)
        {
            charmSoulGainBonus = Mathf.Clamp(charmSoulGainBonus + amount, -99, 99);
        }

        internal void AdjustLeash(float maxDelta, float softDelta, float hardDelta, float snapDelta)
        {
            maxDistance += maxDelta;
            softLeashRadius += softDelta;
            hardLeashRadius += hardDelta;
            snapLeashRadius += snapDelta;
        }

        internal void ResetCharmDerivedStats()
        {
            charmNailDamageMultiplier = 1f;
            charmSpellDamageMultiplier = 1f;
            charmNailScaleMultiplier = 1f;
            charmSoulGainBonus = 0;
            charmFocusHealBonus = 0;
            charmHornetFocusHealBonus = 0;
            charmFocusTimeMultiplier = 1f;
            charmTeleportChannelMultiplier = 1f;
            charmHurtIFrameMultiplier = 1f;
            charmMaxHpBonus = 0;
            charmLifebloodBonus = 0;
            jonisBlessingEquipped = false;
            hivebloodPendingLifebloodRestore = false;
            allowFocusMovement = false;
            knockbackSuppressionCount = 0;
            focusDamageShieldEnabled = false;
            focusDamageShieldAbsorbedThisChannel = false;
            focusHealingDisabled = false;
            carefreeMelodyChance = 0f;
            conditionalNailDamageMultipliers.Clear();
            conditionalNailDamageProduct = 1f;
            UpdateFocusDerivedValues();
            UpdateTeleportChannelTime();
            UpdateHurtIFrameDuration();
            ApplyCharmHealthModifiers(deferHudAndPersistence: true);
        }

        internal void GainShadeSoul(int amount)
        {
            if (amount <= 0)
                return;

            int before = shadeSoul;
            shadeSoul = Mathf.Clamp(shadeSoul + amount, 0, shadeSoulMax);
            if (shadeSoul != before)
            {
                PushSoulToHud();
                PersistIfChanged();
            }
        }

        internal void AddFocusHealBonus(int amount)
        {
            charmFocusHealBonus = Mathf.Clamp(charmFocusHealBonus + amount, -12, 12);
        }

        internal void AddHornetFocusHealBonus(int amount)
        {
            charmHornetFocusHealBonus = Mathf.Clamp(charmHornetFocusHealBonus + amount, -12, 12);
        }

        internal void MultiplyFocusTime(float factor)
        {
            if (factor <= 0f)
                return;

            charmFocusTimeMultiplier = Mathf.Clamp(charmFocusTimeMultiplier * factor, 0.2f, 5f);
            UpdateFocusDerivedValues();
        }

        internal void MultiplyTeleportChannelTime(float factor)
        {
            if (factor <= 0f)
                return;

            charmTeleportChannelMultiplier = Mathf.Clamp(charmTeleportChannelMultiplier * factor, 0.25f, 4f);
            UpdateTeleportChannelTime();
        }

        internal void MultiplyHurtInvulnerability(float factor)
        {
            if (factor <= 0f)
                return;

            charmHurtIFrameMultiplier = Mathf.Clamp(charmHurtIFrameMultiplier * factor, 0.5f, 5f);
            UpdateHurtIFrameDuration();
        }

        internal void ModifyKnockbackSuppression(int delta)
        {
            knockbackSuppressionCount = Mathf.Clamp(knockbackSuppressionCount + delta, 0, 10);
        }

        internal void MultiplyKnockbackForce(float factor)
        {
            if (factor <= 0f)
            {
                return;
            }

            hitKnockbackForce = Mathf.Clamp(hitKnockbackForce * factor, 0.1f, Mathf.Max(0.1f, baseHitKnockbackForce * 5f));
        }

        internal void SetFocusMovementAllowed(bool allowed)
        {
            allowFocusMovement = allowed;
        }

        internal void SetFocusDamageShield(bool enabled)
        {
            focusDamageShieldEnabled = enabled;
            if (!enabled)
            {
                focusDamageShieldAbsorbedThisChannel = false;
            }
        }

        internal void SetFocusHealingDisabled(bool disabled)
        {
            if (focusHealingDisabled == disabled)
            {
                return;
            }

            focusHealingDisabled = disabled;
            if (disabled)
            {
                CancelFocus();
            }
        }

        internal void SetCarefreeMelodyChance(float chance)
        {
            carefreeMelodyChance = Mathf.Clamp01(chance);
        }

        internal void AddMaxHpBonus(int amount, bool fillNew)
        {
            int prevNormalMax = Mathf.Max(0, shadeMaxHP);
            int prevNormalHp = Mathf.Clamp(shadeHP, 0, prevNormalMax);
            int prevLifebloodMax = Mathf.Max(0, shadeLifebloodMax);
            int prevLifeblood = Mathf.Clamp(shadeLifeblood, 0, prevLifebloodMax);
            bool wasJonis = jonisBlessingEquipped;

            int previousLoadoutMax = Mathf.Max(0, baseShadeMaxHP + charmMaxHpBonus);
            charmMaxHpBonus = Mathf.Clamp(charmMaxHpBonus + amount, -20, 40);
            int newLoadoutMax = Mathf.Max(0, baseShadeMaxHP + charmMaxHpBonus);
            int fill = fillNew ? Mathf.Max(0, newLoadoutMax - previousLoadoutMax) : 0;
            ApplyCharmHealthModifiers(
                fillAmount: fill,
                refillLifeblood: false,
                deferHudAndPersistence: persistenceSuppressionDepth > 0,
                previousNormalHpOverride: prevNormalHp,
                previousNormalMaxOverride: prevNormalMax,
                previousLifebloodOverride: prevLifeblood,
                previousLifebloodMaxOverride: prevLifebloodMax,
                previousJonisOverride: wasJonis);
        }

        internal void AddLifebloodBonus(int amount)
        {
            int prevNormalMax = Mathf.Max(0, shadeMaxHP);
            int prevNormalHp = Mathf.Clamp(shadeHP, 0, prevNormalMax);
            int prevLifebloodMax = Mathf.Max(0, shadeLifebloodMax);
            int prevLifeblood = Mathf.Clamp(shadeLifeblood, 0, prevLifebloodMax);
            bool wasJonis = jonisBlessingEquipped;

            charmLifebloodBonus = Mathf.Clamp(charmLifebloodBonus + amount, 0, 99);
            bool refill = amount > 0 && ShouldRefillLifebloodImmediately();
            ApplyCharmHealthModifiers(
                fillAmount: 0,
                refillLifeblood: refill,
                deferHudAndPersistence: persistenceSuppressionDepth > 0,
                previousNormalHpOverride: prevNormalHp,
                previousNormalMaxOverride: prevNormalMax,
                previousLifebloodOverride: prevLifeblood,
                previousLifebloodMaxOverride: prevLifebloodMax,
                previousJonisOverride: wasJonis);
        }

        internal void SetJonisBlessingActive(bool active)
        {
            bool wasJonis = jonisBlessingEquipped;
            int prevNormalMax = Mathf.Max(0, shadeMaxHP);
            int prevNormalHp = Mathf.Clamp(shadeHP, 0, prevNormalMax);
            int prevLifebloodMax = Mathf.Max(0, shadeLifebloodMax);
            int prevLifeblood = Mathf.Clamp(shadeLifeblood, 0, prevLifebloodMax);

            if (jonisBlessingEquipped == active)
            {
                return;
            }

            jonisBlessingEquipped = active;
            if (!jonisBlessingEquipped)
            {
                hivebloodPendingLifebloodRestore = false;
            }

            bool refill = jonisBlessingEquipped && ShouldRefillLifebloodImmediately();
            ApplyCharmHealthModifiers(
                fillAmount: 0,
                refillLifeblood: refill,
                deferHudAndPersistence: persistenceSuppressionDepth > 0,
                previousNormalHpOverride: prevNormalHp,
                previousNormalMaxOverride: prevNormalMax,
                previousLifebloodOverride: prevLifeblood,
                previousLifebloodMaxOverride: prevLifebloodMax,
                previousJonisOverride: wasJonis);
        }

        internal bool IsJonisBlessingActive() => jonisBlessingEquipped;

        internal bool ShouldHivebloodRestoreLifeblood()
        {
            return jonisBlessingEquipped && hivebloodPendingLifebloodRestore && shadeLifeblood < shadeLifebloodMax;
        }

        internal bool TryRestoreLifeblood(int amount)
        {
            if (amount <= 0 || shadeLifeblood >= shadeLifebloodMax)
            {
                return false;
            }

            int restored = Mathf.Min(amount, shadeLifebloodMax - shadeLifeblood);
            if (restored <= 0)
            {
                return false;
            }

            shadeLifeblood += restored;
            hivebloodPendingLifebloodRestore = jonisBlessingEquipped && shadeLifeblood < shadeLifebloodMax && hivebloodPendingLifebloodRestore;
            PushShadeStatsToHud(suppressDamageAudio: true);
            PersistIfChanged();
            return true;
        }

        internal void ResetHivebloodLifebloodRequest()
        {
            hivebloodPendingLifebloodRestore = false;
        }

        internal void SetConditionalNailDamageMultiplier(string key, float multiplier)
        {
            if (string.IsNullOrEmpty(key))
                return;

            conditionalNailDamageMultipliers[key] = Mathf.Max(0.01f, multiplier);
            UpdateConditionalNailDamageProduct();
        }

        internal void ClearConditionalNailDamageMultiplier(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (conditionalNailDamageMultipliers.Remove(key))
            {
                UpdateConditionalNailDamageProduct();
            }
        }

        private void UpdateConditionalNailDamageProduct()
        {
            float product = 1f;
            foreach (var value in conditionalNailDamageMultipliers.Values)
            {
                product *= Mathf.Clamp(value, 0.01f, 10f);
            }

            conditionalNailDamageProduct = Mathf.Clamp(product, 0.1f, 10f);
        }

        private float GetConditionalNailDamageMultiplier()
        {
            return conditionalNailDamageProduct;
        }

        private void UpdateFocusDerivedValues()
        {
            focusChannelTime = Mathf.Max(0.05f, baseFocusChannelTime * charmFocusTimeMultiplier);
        }

        private void UpdateTeleportChannelTime()
        {
            teleportChannelTime = Mathf.Max(0.05f, baseTeleportChannelTime * charmTeleportChannelMultiplier);
        }

        private void UpdateHurtIFrameDuration()
        {
            currentHurtIFrameDuration = Mathf.Max(0.05f, HurtIFrameSeconds * charmHurtIFrameMultiplier);
        }

        private int GetFocusHealAmount()
        {
            if (focusHealingDisabled)
            {
                return 0;
            }

            int baseAmount = ModConfig.Instance.focusShadeHeal + charmFocusHealBonus;
            return Mathf.Clamp(baseAmount, 0, 12);
        }

        private int GetHornetFocusHealAmount()
        {
            if (focusHealingDisabled)
            {
                return 0;
            }

            int baseAmount = ModConfig.Instance.focusHornetHeal + charmHornetFocusHealBonus;
            return Mathf.Clamp(baseAmount, 0, 12);
        }

        private void ApplyCharmHealthModifiers(
            int fillAmount = 0,
            bool refillLifeblood = false,
            bool deferHudAndPersistence = false,
            int? previousNormalHpOverride = null,
            int? previousNormalMaxOverride = null,
            int? previousLifebloodOverride = null,
            int? previousLifebloodMaxOverride = null,
            bool? previousJonisOverride = null)
        {
            int prevNormalMax = Mathf.Max(0, previousNormalMaxOverride ?? shadeMaxHP);
            int prevNormalHp = Mathf.Clamp(previousNormalHpOverride ?? shadeHP, 0, prevNormalMax);
            int prevLifebloodMax = Mathf.Max(0, previousLifebloodMaxOverride ?? shadeLifebloodMax);
            int prevLifeblood = Mathf.Clamp(previousLifebloodOverride ?? shadeLifeblood, 0, prevLifebloodMax);
            bool wasJonis = previousJonisOverride ?? jonisBlessingEquipped;

            int combinedPrevious = Mathf.Clamp(prevNormalHp, 0, prevNormalMax)
                + Mathf.Clamp(prevLifeblood, 0, prevLifebloodMax);
            int positiveFill = Mathf.Max(0, fillAmount);
            int adjustedCombined = combinedPrevious + positiveFill;

            int baseNormalMax = Mathf.Max(0, baseShadeMaxHP + charmMaxHpBonus);
            int lifebloodCapacity = Mathf.Clamp(charmLifebloodBonus, 0, 99);
            bool jonisActive = jonisBlessingEquipped;

            if (jonisActive)
            {
                int jonisBase = Mathf.Max(1, baseNormalMax);
                lifebloodCapacity += Mathf.CeilToInt(jonisBase * 1.4f);
            }

            int targetNormalMax = jonisActive ? 0 : Mathf.Max(1, baseNormalMax);
            int targetLifebloodMax = jonisActive
                ? Mathf.Clamp(lifebloodCapacity, 0, 99)
                : lifebloodCapacity;

            int newNormalHp;
            int newLifeblood;

            if (jonisActive)
            {
                newNormalHp = 0;
                newLifeblood = refillLifeblood
                    ? targetLifebloodMax
                    : Mathf.Clamp(adjustedCombined, 0, targetLifebloodMax);
            }
            else
            {
                newNormalHp = Mathf.Clamp(adjustedCombined, 0, targetNormalMax);
                if (refillLifeblood)
                {
                    newLifeblood = targetLifebloodMax;
                }
                else
                {
                    int leftover = Mathf.Max(0, adjustedCombined - newNormalHp);
                    newLifeblood = Mathf.Clamp(leftover, 0, targetLifebloodMax);
                }
            }

            bool statsChanged =
                targetNormalMax != prevNormalMax
                || targetLifebloodMax != prevLifebloodMax
                || newNormalHp != prevNormalHp
                || newLifeblood != prevLifeblood
                || jonisActive != wasJonis;

            shadeMaxHP = targetNormalMax;
            shadeHP = targetNormalMax > 0
                ? Mathf.Clamp(newNormalHp, 0, targetNormalMax)
                : 0;
            shadeLifebloodMax = Mathf.Clamp(targetLifebloodMax, 0, 99);
            shadeLifeblood = Mathf.Clamp(newLifeblood, 0, shadeLifebloodMax);

            hivebloodPendingLifebloodRestore = jonisActive
                && hivebloodPendingLifebloodRestore
                && shadeLifeblood < shadeLifebloodMax;

            if (statsChanged)
            {
                if (deferHudAndPersistence)
                {
                    pendingDeferredHealthSync = true;
                    pendingDeferredHealthSuppressDamage = true;
                    PushShadeStatsToHud(suppressDamageAudio: true);
                }
                else
                {
                    PushShadeStatsToHud(suppressDamageAudio: true);
                    PersistIfChanged();
                }
            }
        }

        private bool ShouldRefillLifebloodImmediately()
        {
            try
            {
                var gm = GameManager.instance;
                if (gm == null)
                {
                    return false;
                }

                var pd = gm.playerData;
                if (pd == null)
                {
                    return false;
                }

                return pd.atBench;
            }
            catch
            {
                return false;
            }
        }
    }
}
