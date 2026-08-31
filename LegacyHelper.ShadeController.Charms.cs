#nullable disable
using System;
using System.Globalization;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using GlobalEnums;

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

                var inventory = OwnCharms;
                // Active, not equipped: a broken fragile charm keeps its slot but not its effect.
                var loadout = inventory?.GetActiveDefinitions();
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

        internal void MultiplyNailKnockback(float factor)
        {
            if (factor <= 0f)
                return;

            charmNailKnockbackMultiplier = Mathf.Clamp(charmNailKnockbackMultiplier * factor, 0.1f, 5f);
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
            charmNailKnockbackMultiplier = 1f;
            charmSoulGainBonus = 0;
            charmVesselSoulGainBonus = 0;
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
            damageStaggerDurationMultiplier = 1f;
            focusDamageShieldEnabled = false;
            focusDamageShieldAbsorbedThisChannel = false;
            focusHealingDisabled = false;
            carefreeMelodyEquipped = false;

            carefreeMelodyChance = 0f;
            voidHeartEvadeActive = false;
            DisableCarefreeMelodyEffect();
            LegacyHelper.SetFragileGreedActive(false);
            sharpShadowEquipped = false;
            sprintmasterEquipped = false;
            grubberflyElegyEquipped = false;
            shamanStoneEquipped = false;
            furyModeActive = false;
            sharpShadowDashActive = false;
            DestroySharpShadowDashHitbox();
            conditionalNailDamageMultipliers.Clear();
            conditionalNailDamageProduct = 1f;
            UpdateFocusDerivedValues();
            UpdateTeleportChannelTime();
            UpdateHurtIFrameDuration();
            ApplyCharmHealthModifiers(deferHudAndPersistence: true);
            RefreshBaldurShellFocusState(immediate: true);
        }

        /// <summary>
        /// Soul from a charm - Grubsong and its like. One of the "all other sources" that fill the
        /// vessels at their normal rate, so it goes through the same overflow as everything else.
        /// </summary>
        internal void GainShadeSoul(int amount)
        {
            AddSoul(amount);
        }

        internal void AddFocusHealBonus(int amount)
        {
            charmFocusHealBonus = Mathf.Clamp(charmFocusHealBonus + amount, -12, 12);
        }

        internal void AddHornetFocusHealBonus(int amount)
        {
            charmHornetFocusHealBonus = Mathf.Clamp(charmHornetFocusHealBonus + amount, -12, 12);
        }

        internal void SetVoidHeartEvadeActive(bool active)
        {
            voidHeartEvadeActive = active;
            if (!active)
            {
                sharpShadowDashActive = false;
                DestroySharpShadowDashHitbox();
            }
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

        internal void MultiplyDamageStaggerDuration(float factor)
        {
            if (factor <= 0f)
            {
                return;
            }

            damageStaggerDurationMultiplier = Mathf.Clamp(damageStaggerDurationMultiplier * factor, 0.1f, 5f);
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

        /// <summary>Scratch for <see cref="PushBuffsToHud"/>, so a per-frame push allocates nothing.</summary>
        private readonly System.Collections.Generic.List<SimpleHUD.BuffIcon> buffIconScratch = new System.Collections.Generic.List<SimpleHUD.BuffIcon>();

        /// <summary>
        /// Gathers this companion's status icons and hands them to the HUD.
        /// <para>
        /// Adding a buff is one more Append call here; the bar sizes itself to whatever the list
        /// holds and does not know what any of it means.
        /// </para>
        /// </summary>
        private void PushBuffsToHud()
        {
            // The HUD is shared, and a second companion pushing its own set would simply
            // overwrite the first's every frame. The primary owns the bar, as it owns the mask row.
            if (!cachedHud || Companion == null || !Companion.IsPrimary)
            {
                return;
            }

            buffIconScratch.Clear();
            AppendBaldurShellBuff(buffIconScratch);

            cachedHud.SetBuffIcons(buffIconScratch);
        }

        /// <summary>
        /// Baldur Shell's shell, shown only while the charm is worn - including when it is spent,
        /// which is the whole point of the readout: it breaks quietly and this is the only sign.
        /// </summary>
        private void AppendBaldurShellBuff(System.Collections.Generic.List<SimpleHUD.BuffIcon> icons)
        {
            if (!focusDamageShieldEnabled)
            {
                return;
            }

            var charms = OwnCharms;
            if (charms == null)
            {
                return;
            }

            icons.Add(SimpleHUD.BuildBaldurShellIcon(charms.BaldurShellCharges, ShadeCharmInventory.BaldurShellMaxCharges));
        }

        internal void SetShamanStoneEquipped(bool equipped)
        {
            shamanStoneEquipped = equipped;
        }

        internal void SetGrubberflyElegyEquipped(bool equipped)
        {
            grubberflyElegyEquipped = equipped;
        }

        internal void SetSprintmasterEquipped(bool equipped)
        {
            sprintmasterEquipped = equipped;
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

            RefreshBaldurShellFocusState(immediate: !enabled);
        }

        internal void SetSharpShadowEnabled(bool enabled)
        {
            if (sharpShadowEquipped == enabled)
            {
                return;
            }

            sharpShadowEquipped = enabled;
            if (enabled)
            {
                EnsureSharpShadowShadeView();
            }
            else
            {
                sharpShadowDashActive = false;
                DestroySharpShadowDashHitbox();
                DiscardSharpShadowShadeView();
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

        internal void SetCarefreeMelodyEquipped(bool equipped)
        {
            carefreeMelodyEquipped = equipped;
            if (!equipped)
            {
                carefreeMelodyChance = 0f;
                DisableCarefreeMelodyEffect();
            }
            else
            {
                carefreeMelodyChance = 0f;
            }
        }

        internal void IncrementCarefreeMelodyChance()
        {
            if (!carefreeMelodyEquipped)
            {
                return;
            }

            carefreeMelodyChance = Mathf.Clamp01(carefreeMelodyChance + CarefreeMelodyChanceStep);
        }

        private void ResetCarefreeMelodyChance()
        {
            carefreeMelodyChance = 0f;
        }

        private void DisableCarefreeMelodyEffect()
        {
            try
            {
                if (carefreeMelodyShieldEffect)
                {
                    carefreeMelodyShieldEffect.SetActive(false);
                }
            }
            catch
            {
            }
        }

        private void PlayCarefreeMelodyBlockEffect()
        {
            var effect = EnsureCarefreeMelodyEffect();
            if (!effect)
            {
                return;
            }

            effect.SetActive(false);
            effect.SetActive(true);
        }

        private GameObject EnsureCarefreeMelodyEffect()
        {
            if (carefreeMelodyShieldEffect)
            {
                return carefreeMelodyShieldEffect;
            }

            try
            {
                var hc = HeroController.instance;
                if (hc != null && hc.luckyDiceShieldEffectPrefab != null)
                {
                    carefreeMelodyShieldEffect = Instantiate(hc.luckyDiceShieldEffectPrefab, transform);
                    carefreeMelodyShieldEffect.transform.localPosition = Vector3.zero;
                    carefreeMelodyShieldEffect.SetActive(false);
                }
            }
            catch
            {
            }

            return carefreeMelodyShieldEffect;
        }

        internal void SetFragileGreedActive(bool active)
        {
            LegacyHelper.SetFragileGreedActive(active);
        }

        /// <summary>
        /// How much health a resize should put back before clamping to the new maximum. Zero while
        /// unpaused, so ordinary max-health changes never heal. While paused it is whatever the
        /// Shade has lost since the pause, which is what stops the mask fraction option ratcheting
        /// it down: the option's list wraps through "Always 1", so a player cycling back to the
        /// setting they started on would otherwise be left on 1 health permanently.
        /// </summary>
        /// <param name="pausedBaseline">Health at the last pause, or negative while unpaused.</param>
        internal static int ResolveResizeRefill(int currentHealth, int pausedBaseline)
        {
            return pausedBaseline < 0 ? 0 : Mathf.Max(0, pausedBaseline - currentHealth);
        }

        /// <summary>
        /// Re-derives the Shade's mask count from Hornet's max health and
        /// <see cref="ModConfig.shadeMaskFraction"/>, and resizes it in place. Cheap and idempotent
        /// when neither input has moved, so it is safe to call every frame.
        /// <para>
        /// Awake's own derivation only ever raises the maximum - it has to, or loading a save whose
        /// Shade out-levels the current playerData would shrink it. That makes it useless both for a
        /// setting the player can turn *down* in the pause menu and for Hornet's max health falling,
        /// which is why this is a separate, unconditional path rather than a call back into Awake.
        /// </para>
        /// </summary>
        internal void RefreshDerivedMaskCount()
        {
            var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
            if (pd == null)
            {
                return;
            }

            int derived = ModConfig.ComputeShadeMaskCount(pd.maxHealth);
            if (derived <= 0 || derived == baseShadeMaxHP)
            {
                return;
            }

            baseShadeMaxHP = derived;

            // Everything downstream of the base max - the charm bonus, Joni's lifeblood conversion,
            // clamping current health into the new ceiling, the HUD and persistence - is already
            // handled here, so this is the one call needed rather than a second copy of that maths.
            ApplyCharmHealthModifiers(fillAmount: ResolveResizeRefill(GetTotalCurrentHealth(), pausedHealthBaseline));
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

            int fill = 0;
            if (fillNew)
            {
                if (jonisBlessingEquipped)
                {
                    int lifebloodCapacity = Mathf.Clamp(charmLifebloodBonus, 0, 99);
                    int jonisBase = Mathf.Max(1, newLoadoutMax);
                    lifebloodCapacity += Mathf.CeilToInt(jonisBase * 1.4f);
                    fill = Mathf.Max(0, lifebloodCapacity - prevLifeblood);
                }
                else
                {
                    fill = Mathf.Max(0, newLoadoutMax - prevNormalHp);
                }
            }

            LogCharmHealthEvent($"AddMaxHpBonus amount={amount} fillNew={fillNew} previousLoadoutMax={previousLoadoutMax} newLoadoutMax={newLoadoutMax} prevNormal={prevNormalHp}/{prevNormalMax} prevLifeblood={prevLifeblood}/{prevLifebloodMax} wasJonis={wasJonis}");
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
            LogCharmHealthEvent($"AddLifebloodBonus amount={amount} refill={refill} prevNormal={prevNormalHp}/{prevNormalMax} prevLifeblood={prevLifeblood}/{prevLifebloodMax} wasJonis={wasJonis}");
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
            LogCharmHealthEvent($"SetJonisBlessingActive active={active} refill={refill} prevNormal={prevNormalHp}/{prevNormalMax} prevLifeblood={prevLifeblood}/{prevLifebloodMax} wasJonis={wasJonis}");
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

        /// <summary>Which character this companion is, for a log line. Nothing else reads it.</summary>
        private string CharacterLogName => UsesGroundedMovement ? "Knight" : "Shade";

        private int GetShadeNailDamage()
        {
            int nailDmg = Mathf.Max(1, GetHornetNailDamage());
            nailDmg = Mathf.Max(1, Mathf.RoundToInt(nailDmg * ModConfig.Instance.shadeDamageMultiplier));
            nailDmg = Mathf.Max(1, Mathf.RoundToInt(nailDmg * charmNailDamageMultiplier));
            nailDmg = Mathf.Max(1, Mathf.RoundToInt(nailDmg * GetConditionalNailDamageMultiplier()));
            return nailDmg;
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

            LogCharmHealthEvent($"ApplyCharmHealthModifiers start fill={fillAmount} refillLifeblood={refillLifeblood} defer={deferHudAndPersistence} prevNormal={prevNormalHp}/{prevNormalMax} prevLifeblood={prevLifeblood}/{prevLifebloodMax} wasJonis={wasJonis} combinedPrevious={combinedPrevious} baseNormalMax={baseNormalMax} lifebloodCapacity={lifebloodCapacity} jonisActive={jonisActive}");

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
                LogCharmHealthEvent($"ApplyCharmHealthModifiers result statsChanged=True normal={shadeHP}/{shadeMaxHP} lifeblood={shadeLifeblood}/{shadeLifebloodMax} jonisActive={jonisActive} adjustedCombined={adjustedCombined} defer={deferHudAndPersistence}");
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
            else
            {
                LogCharmHealthEvent($"ApplyCharmHealthModifiers result statsChanged=False normal={shadeHP}/{shadeMaxHP} lifeblood={shadeLifeblood}/{shadeLifebloodMax} jonisActive={jonisActive} adjustedCombined={adjustedCombined}");
            }
        }

        private static void LogCharmHealthEvent(FormattableString message)
        {
            if (!ModConfig.Instance.logShade)
            {
                return;
            }

            LogCharmHealthEvent(message.ToString(CultureInfo.InvariantCulture));
        }

        private static void LogCharmHealthEvent(string message)
        {
            if (!ModConfig.Instance.logShade)
            {
                return;
            }

            UnityEngine.Debug.Log("[ShadeCharmHealth] " + message);
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

        private int GetHornetNailDamage()
        {
            try
            {
                var gm = GameManager.instance;
                var pd = gm != null ? gm.playerData : null;
                if (pd == null) return 5;
                int baseDmg = Mathf.Max(1, pd.nailDamage);
                bool bound = false;
                try { bound = BossSequenceController.BoundNail; } catch { bound = false; }
                if (bound)
                {
                    int boundVal = 0;
                    try { boundVal = BossSequenceController.BoundNailDamage; } catch { boundVal = baseDmg; }
                    return Mathf.Min(baseDmg, Mathf.Max(1, boundVal));
                }
                return baseDmg;
            }
            catch { return 5; }
        }
    }
}
