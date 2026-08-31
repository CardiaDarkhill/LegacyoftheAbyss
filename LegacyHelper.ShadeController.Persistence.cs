#nullable disable
using System.Collections;
using UnityEngine;
using LegacyoftheAbyss.Shade;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        public void RestorePersistentState(int hp, int max, int baseMax, int lifeblood, int lifebloodMax, int soul, bool canDamage = true, int vesselSoul = 0)
        {
            baseShadeMaxHP = Mathf.Max(0, baseMax);
            shadeMaxHP = Mathf.Max(0, max);
            shadeHP = Mathf.Clamp(hp, 0, shadeMaxHP);
            pendingRestoredLifebloodMax = Mathf.Max(0, lifebloodMax);
            pendingRestoredLifeblood = Mathf.Clamp(lifeblood, 0, pendingRestoredLifebloodMax);
            shadeLifebloodMax = pendingRestoredLifebloodMax;
            shadeLifeblood = pendingRestoredLifeblood;
            shadeSoul = Mathf.Clamp(soul, 0, shadeSoulMax);
            RestoreVesselSoul(vesselSoul);
            canTakeDamage = canDamage;
            assistModeEnabled = !canTakeDamage;
            sceneProtectionDesiredDamageState = canTakeDamage;
            sceneProtectionActive = false;
            sceneProtectionTimer = 0f;
            sceneProtectionSuppressingPersistence = false;
            lastSavedCanTakeDamage = canTakeDamage;

            if (baseShadeMaxHP <= 0)
            {
                baseShadeMaxHP = shadeMaxHP > 0 ? shadeMaxHP : 1;
            }
        }

        public void FullHealFromBench()
        {
            RefillHealthPools();
            ShadeRuntime.HandleBenchRest();
            PushShadeStatsToHud(suppressDamageAudio: true);
        }

        /// <summary>
        /// Matches Hornet's own death recovery. <c>HeroController.Respawn</c> calls <c>MaxHealth</c>
        /// whenever she was dead, whatever marker she respawns at, so the Shade refills too - gating
        /// the refill on the marker being a bench leaves it on a single mask everywhere else.
        /// <para>
        /// Unlike <see cref="FullHealFromBench"/> this deliberately skips the bench rest: a death is
        /// what breaks the fragile charms, so it must not also repair them.
        /// </para>
        /// </summary>
        public void FullHealOnRespawn()
        {
            RefillHealthPools();
            PushShadeStatsToHud(suppressDamageAudio: true);
        }

        private void RefillHealthPools()
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
        public bool GetCanTakeDamage()
        {
            return sceneProtectionActive ? sceneProtectionDesiredDamageState : canTakeDamage;
        }

        public void Init(Transform hornet) { hornetTransform = hornet; }

        /// <summary>
        /// Writes this Shade's health and soul to its own companion. Falls back to the primary's
        /// state only for a controller spawned outside the registry, which the tests do.
        /// </summary>
        private void SaveOwnState(bool damageState)
        {
            if (Companion != null)
            {
                LegacyHelper.SaveShadeState(Companion, shadeHP, shadeMaxHP, shadeLifeblood, shadeLifebloodMax, shadeSoul, damageState, baseShadeMaxHP, shadeVesselSoul);
                return;
            }

            LegacyHelper.SaveShadeState(shadeHP, shadeMaxHP, shadeLifeblood, shadeLifebloodMax, shadeSoul, damageState, baseShadeMaxHP, shadeVesselSoul);
        }

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
                || lastSavedVesselSoul != shadeVesselSoul
                || lastSavedCanTakeDamage != canTakeDamage)
            {
                SaveOwnState(canTakeDamage);
                lastSavedHP = shadeHP;
                lastSavedMax = shadeMaxHP;
                lastSavedLifeblood = shadeLifeblood;
                lastSavedLifebloodMax = shadeLifebloodMax;
                lastSavedSoul = shadeSoul;
                lastSavedVesselSoul = shadeVesselSoul;
                lastSavedCanTakeDamage = canTakeDamage;
            }
        }

        private SimpleHUD ResolveHud()
        {
            if (!cachedHud)
            {
                try
                {
                    cachedHud = Object.FindFirstObjectByType<SimpleHUD>();
                }
                catch
                {
                    cachedHud = null;
                }
            }

            return cachedHud;
        }

        private void PushSoulToHud()
        {
            pendingHudSoulSync = true;
            TryFlushHudSync();
            EnsureHudSyncScheduled();
        }

        private void PushShadeStatsToHud(bool suppressDamageAudio = false)
        {
            pendingHudStatsSync = true;
            pendingHudAssistSync = true;
            pendingHudOvercharmSync = true;
            if (suppressDamageAudio)
            {
                pendingHudSuppressDamageSfx = true;
            }

            TryFlushHudSync();
            EnsureHudSyncScheduled();
        }

        /// <summary>
        /// Whether this Shade drives the shared HUD. Only one can: the readouts hold a single
        /// Shade's masks and soul, so a second one pushing to them would overwrite the first every
        /// frame. Secondary companions keep their state and wait for a HUD of their own.
        /// </summary>
        private bool OwnsHud => Companion == null || Companion.IsPrimary;

        private void TryFlushHudSync()
        {
            var hud = ResolveHud();
            if (!hud || !OwnsHud)
            {
                return;
            }

            if (pendingHudStatsSync)
            {
                if (pendingHudSuppressDamageSfx)
                {
                    hud.SuppressNextShadeDamageSfx();
                }

                hud.SetShadeStats(shadeHP, shadeMaxHP, shadeLifeblood, shadeLifebloodMax);
                pendingHudStatsSync = false;
                pendingHudSuppressDamageSfx = false;
            }

            if (pendingHudAssistSync)
            {
                hud.SetShadeAssistMode(assistModeEnabled);
                pendingHudAssistSync = false;
            }

            if (pendingHudOvercharmSync)
            {
                // This Shade's own charms, not the primary's - they equip independently.
                var charms = OwnCharms;
                hud.SetShadeOvercharmed(charms?.IsOvercharmed ?? false);
                pendingHudOvercharmSync = false;
            }

            if (pendingHudSoulSync)
            {
                hud.SetShadeSoul(shadeSoul, shadeSoulMax, shadeVesselSoul, GetShadeVesselCount());
                pendingHudSoulSync = false;
            }
        }

        private bool IsHudSyncPending()
        {
            return pendingHudStatsSync
                || pendingHudAssistSync
                || pendingHudOvercharmSync
                || pendingHudSoulSync;
        }

        private void EnsureHudSyncScheduled()
        {
            if (!IsHudSyncPending())
            {
                if (hudSyncRoutine != null)
                {
                    try { StopCoroutine(hudSyncRoutine); }
                    catch { }
                    hudSyncRoutine = null;
                }

                return;
            }

            if (hudSyncRoutine == null)
            {
                try { hudSyncRoutine = StartCoroutine(FlushHudSyncDeferred()); }
                catch { hudSyncRoutine = null; }
            }
        }

        private IEnumerator FlushHudSyncDeferred()
        {
            while (IsHudSyncPending())
            {
                yield return null;
                TryFlushHudSync();
            }

            hudSyncRoutine = null;
        }

        private int GetTotalCurrentHealth()
        {
            return Mathf.Max(0, shadeHP) + Mathf.Max(0, shadeLifeblood);
        }
    }
}
