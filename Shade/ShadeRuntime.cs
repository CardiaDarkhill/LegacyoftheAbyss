#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    public enum ShadeUnlockNotificationType
    {
        Ability,
        Charm,
        Spell
    }


    /// <summary>
    /// Central coordination point for shade state that has to live longer than a single scene. The runtime
    /// exposes a simple API today but can later be extended to coordinate with save slots, file IO, or
    /// charm inventories without forcing the rest of the mod to understand those details.
    /// </summary>
    internal static class ShadeRuntime
    {
        internal const string BenchLockedMessage = "Rest at a bench to change the shade's charms.";
        private const string VoidHeartUnlockSceneName = "Song_Tower_Destroyed";

        private static readonly ShadePersistentState s_persistentState = new ShadePersistentState();
        private static readonly ShadeSaveSlotRepository s_saveSlots = new ShadeSaveSlotRepository();
        private static readonly ShadeCharmInventory s_charmInventory = CreateCharmInventory();
        private static readonly Queue<ShadeUnlockNotification> s_notificationQueue = new();
        private static readonly HashSet<string> s_seenNotificationKeys = new(StringComparer.OrdinalIgnoreCase);
        private static int s_activeSlot;
        private static bool s_hasActiveSlot;
        private static CharmInventorySnapshot? s_debugCharmSnapshot;
        private static bool s_debugUnlockAllCharmsActive;

        private static readonly (int Progress, string Key, string Message)[] s_spellMilestones = new[]
        {
            (1, "shade::spell_progress::1", "Shade learned a new needle technique."),
            (2, "shade::spell_progress::2", "Shade's needle technique grew stronger."),
            (3, "shade::spell_progress::3", "Shade awakened Descending Dark."),
            (4, "shade::spell_progress::4", "Shade's Descending Dark grew in power."),
            (5, "shade::spell_progress::5", "Shade unlocked the Shade Shriek."),
            (6, "shade::spell_progress::6", "Shade's Shade Shriek reached full potential."),
        };

        private static ShadeCharmInventory CreateCharmInventory()
        {
            var inventory = new ShadeCharmInventory();
            inventory.StateChanged += HandleCharmInventoryChanged;
            return inventory;
        }

        public static event Action? NotificationsChanged;

        public static ShadePersistentState PersistentState => s_persistentState;

        public static ShadeSaveSlotRepository SaveSlots => s_saveSlots;

        public static ShadeCharmInventory Charms => s_charmInventory;

        public static bool HasPendingNotifications
        {
            get
            {
                lock (s_notificationQueue)
                {
                    return s_notificationQueue.Count > 0;
                }
            }
        }

        public static bool TryGetPersistentState(out int currentHp, out int maxHp, out int lifebloodCurrent, out int lifebloodMax, out int soul, out bool canTakeDamage, out int baseMaxHp)
        {
            if (!s_persistentState.HasData)
            {
                currentHp = -1;
                maxHp = -1;
                lifebloodCurrent = -1;
                lifebloodMax = -1;
                soul = -1;
                canTakeDamage = true;
                baseMaxHp = -1;
                return false;
            }

            currentHp = s_persistentState.CurrentHP;
            maxHp = s_persistentState.MaxHP;
            lifebloodCurrent = s_persistentState.CurrentLifeblood;
            lifebloodMax = s_persistentState.LifebloodMax;
            soul = s_persistentState.Soul;
            canTakeDamage = s_persistentState.CanTakeDamage;
            baseMaxHp = s_persistentState.BaseMaxHP;
            return true;
        }

        public static void CaptureState(int currentHp, int maxHp, int lifebloodCurrent, int lifebloodMax, int soul, bool? canTakeDamage = null, int? baseMaxHp = null)
        {
            s_persistentState.Capture(currentHp, maxHp, lifebloodCurrent, lifebloodMax, soul, canTakeDamage, baseMaxHp);
        }

        public static void EnsureMinimumHealth(int minimum)
        {
            s_persistentState.ForceMinimumHealth(minimum);
        }

        internal static void HandleSceneEntered(string? sceneName)
        {
            EnsureActiveSlot();
            TryGrantVoidHeartForScene(sceneName);
        }

        public static void Clear()
        {
            DisableDebugUnlockIfActive();
            s_persistentState.Reset();
            s_charmInventory.ResetLoadout();
            ResetNotificationState();
        }

        public static void ResetNotificationState()
        {
            lock (s_notificationQueue)
            {
                s_notificationQueue.Clear();
                s_seenNotificationKeys.Clear();
            }

            RaiseNotificationsChanged();
        }

        public static bool TryDequeueNotification(out ShadeUnlockNotification notification)
        {
            lock (s_notificationQueue)
            {
                if (s_notificationQueue.Count > 0)
                {
                    notification = s_notificationQueue.Dequeue();
                    return true;
                }
            }

            notification = null!;
            return false;
        }

        public static bool EnqueueNotification(string? key, string message, ShadeUnlockNotificationType type, float duration = ShadeUnlockNotification.DefaultDuration, Sprite? icon = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            var notification = new ShadeUnlockNotification(key, message, type, duration, icon);
            return EnqueueNotification(notification);
        }

        public static bool EnqueueNotification(ShadeUnlockNotification notification)
        {
            if (notification == null || string.IsNullOrWhiteSpace(notification.Message))
            {
                return false;
            }

            bool queued = false;
            lock (s_notificationQueue)
            {
                if (s_seenNotificationKeys.Add(notification.Key))
                {
                    s_notificationQueue.Enqueue(notification);
                    queued = true;
                }
            }

            if (queued)
            {
                RaiseNotificationsChanged();
            }

            return queued;
        }

        public static void NotifyHornetSpellUnlocked()
        {
            int previous = s_persistentState.SpellProgress;
            s_persistentState.AdvanceSpellProgress();
            QueueSpellNotificationsBetween(previous, s_persistentState.SpellProgress);
        }

        public static void SyncSpellProgress(int progress)
        {
            int previous = s_persistentState.SpellProgress;
            s_persistentState.SetSpellProgress(progress);
        }

        private static void QueueSpellNotificationsBetween(int previous, int current)
        {
            if (current <= previous)
            {
                return;
            }

            foreach (var milestone in s_spellMilestones)
            {
                if (milestone.Progress > previous && milestone.Progress <= current)
                {
                    EnqueueNotification(milestone.Key, milestone.Message, ShadeUnlockNotificationType.Spell);
                }
            }
        }

        public static IReadOnlyCollection<int> GetDiscoveredCharms()
        {
            EnsureActiveSlot();
            return s_saveSlots.GetDiscoveredCharms(s_activeSlot);
        }

        public static bool IsCharmDiscovered(int charmId)
        {
            EnsureActiveSlot();
            return s_saveSlots.GetDiscoveredCharms(s_activeSlot).Contains(charmId);
        }

        public static bool UnlockCharm(int charmId)
        {
            EnsureActiveSlot();
            bool unlocked = s_saveSlots.UnlockCharm(s_activeSlot, charmId);
            if (unlocked && Enum.IsDefined(typeof(ShadeCharmId), charmId))
            {
                s_charmInventory.GrantCharm((ShadeCharmId)charmId);
            }

            return unlocked;
        }

        public static IReadOnlyCollection<int> GetEquippedCharms(int loadoutId)
        {
            EnsureActiveSlot();
            return s_saveSlots.GetEquippedCharms(s_activeSlot, loadoutId);
        }

        public static IReadOnlyDictionary<int, IReadOnlyCollection<int>> GetEquippedCharmLoadouts()
        {
            EnsureActiveSlot();
            return s_saveSlots.GetEquippedCharmLoadouts(s_activeSlot);
        }

        public static bool EquipCharm(int loadoutId, int charmId)
        {
            EnsureActiveSlot();
            bool result = s_saveSlots.EquipCharm(s_activeSlot, loadoutId, charmId);
            if (result)
            {
                SyncInventoryFromActiveSlot();
            }

            return result;
        }

        public static bool UnequipCharm(int loadoutId, int charmId)
        {
            EnsureActiveSlot();
            bool result = s_saveSlots.UnequipCharm(s_activeSlot, loadoutId, charmId);
            if (result)
            {
                SyncInventoryFromActiveSlot();
            }

            return result;
        }

        public static void ClearLoadout(int loadoutId)
        {
            EnsureActiveSlot();
            s_saveSlots.GetOrCreateSlot(s_activeSlot).ClearLoadout(loadoutId);
            SyncInventoryFromActiveSlot();
        }

        public static int GetNotchCapacity()
        {
            EnsureActiveSlot();
            return s_saveSlots.GetNotchCapacity(s_activeSlot);
        }

        public static bool SetNotchCapacity(int capacity)
        {
            EnsureActiveSlot();
            bool changed = s_saveSlots.SetNotchCapacity(s_activeSlot, capacity);
            if (changed)
            {
                SyncInventoryFromActiveSlot();
            }

            return changed;
        }

        public static bool TryCollectNotch(string notchId, int increment)
        {
            if (string.IsNullOrWhiteSpace(notchId))
            {
                return false;
            }

            EnsureActiveSlot();
            int sanitizedIncrement = Mathf.Max(1, increment);
            bool collected = s_saveSlots.TryCollectNotch(s_activeSlot, notchId, sanitizedIncrement);
            if (collected)
            {
                SyncInventoryFromActiveSlot();
            }

            return collected;
        }

        public static bool HasCollectedNotch(string notchId)
        {
            if (string.IsNullOrWhiteSpace(notchId))
            {
                return false;
            }

            EnsureActiveSlot();
            return s_saveSlots.HasCollectedNotch(s_activeSlot, notchId);
        }

        internal static void SyncActiveSlot(GameManager? gameManager)
        {
            if (gameManager == null)
            {
                EnsureActiveSlot();
                return;
            }

            int desired = TryGetProfileId(gameManager);
            SetActiveSlot(desired);
        }

        internal static void SetActiveSlot(int slot)
        {
            int clamped = s_saveSlots.MaxSlots > 0
                ? Mathf.Clamp(slot, 0, s_saveSlots.MaxSlots - 1)
                : 0;

            if (!s_hasActiveSlot)
            {
                s_activeSlot = clamped;
                s_hasActiveSlot = true;
                DisableDebugUnlockIfActive();
                s_saveSlots.GetOrCreateSlot(s_activeSlot);
                SyncInventoryFromActiveSlot();
                LegacyHelper.RequestShadeLoadoutRecompute();
                return;
            }

            if (s_activeSlot != clamped)
            {
                if (!s_debugUnlockAllCharmsActive)
                {
                    PersistInventoryToSlot(s_activeSlot);
                }

                DisableDebugUnlockIfActive();
                s_activeSlot = clamped;
                s_saveSlots.GetOrCreateSlot(s_activeSlot);
                SyncInventoryFromActiveSlot();
                LegacyHelper.RequestShadeLoadoutRecompute();
                return;
            }

            if (!s_debugUnlockAllCharmsActive)
            {
                PersistInventoryToSlot(s_activeSlot);
            }

            LegacyHelper.RequestShadeLoadoutRecompute();
        }

        private static void PersistInventoryToSlot(int slot)
        {
            if (slot < 0 || s_charmInventory == null)
            {
                return;
            }

            var owned = s_charmInventory.GetOwnedCharms();
            var broken = s_charmInventory.GetBrokenCharms();
            var equipped = s_charmInventory.GetEquipped().Select(id => (int)id);
            var newlyDiscovered = s_charmInventory.GetNewlyDiscovered();

            s_saveSlots.SetCollectedCharms(slot, owned);
            s_saveSlots.SetBrokenCharms(slot, broken);
            s_saveSlots.SetNotchCapacity(slot, s_charmInventory.NotchCapacity);
            s_saveSlots.SetEquippedCharms(slot, 0, equipped);
            s_saveSlots.SetNewlyDiscoveredCharms(slot, newlyDiscovered);
        }

        internal static bool ToggleDebugUnlockAllCharms()
        {
            EnsureActiveSlot();

            if (s_debugUnlockAllCharmsActive)
            {
                DisableDebugUnlockIfActive();
                return false;
            }

            var snapshot = CaptureCharmInventorySnapshot();
            var allOwned = s_charmInventory.AllCharms
                .Select(def => def.EnumId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToArray();

            s_debugUnlockAllCharmsActive = true;
            try
            {
                int slotNotchCapacity = Mathf.Clamp(s_saveSlots.GetNotchCapacity(s_activeSlot), 0, 20);
                int debugNotchCapacity = slotNotchCapacity > 0
                    ? slotNotchCapacity
                    : Mathf.Clamp(snapshot.NotchCapacity, 0, 20);

                s_charmInventory.LoadState(allOwned, snapshot.Equipped, snapshot.Broken, debugNotchCapacity, snapshot.NewlyDiscovered);
                s_debugCharmSnapshot = snapshot;
                s_saveSlots.SetDebugUnlockState(
                    s_activeSlot,
                    true,
                    new ShadeDebugCharmSnapshot(snapshot.Owned, snapshot.Equipped, snapshot.Broken, snapshot.NewlyDiscovered, debugNotchCapacity));
            }
            catch
            {
                s_debugUnlockAllCharmsActive = false;
                s_debugCharmSnapshot = null;
                s_saveSlots.SetDebugUnlockState(s_activeSlot, false, null);
                throw;
            }

            LegacyHelper.RequestShadeLoadoutRecompute();
            return true;
        }

        internal static bool IsDebugCharmModeActive()
        {
            EnsureActiveSlot();

            if (s_debugUnlockAllCharmsActive)
            {
                return true;
            }

            return s_saveSlots.IsDebugUnlockActive(s_activeSlot);
        }

        internal static IReadOnlyCollection<ShadeCharmId> GetCollectedCharms()
        {
            EnsureActiveSlot();
            return s_saveSlots.GetCollectedCharms(s_activeSlot);
        }

        internal static bool IsCharmCollected(ShadeCharmId charmId)
        {
            EnsureActiveSlot();
            return s_saveSlots.IsCharmCollected(s_activeSlot, charmId);
        }

        internal static bool TryCollectCharm(ShadeCharmId charmId)
        {
            EnsureActiveSlot();
            bool collected = s_saveSlots.MarkCharmCollected(s_activeSlot, charmId);
            if (collected)
            {
                s_saveSlots.UnlockCharm(s_activeSlot, (int)charmId);
                s_charmInventory.GrantCharm(charmId);
            }

            return collected;
        }

        internal static bool HandleShadeDeath()
        {
            if (s_charmInventory == null)
            {
                return false;
            }

            bool brokeCharm = false;
            brokeCharm |= TryBreakFragileCharm(ShadeCharmId.FragileStrength, string.Empty);
            brokeCharm |= TryBreakFragileCharm(ShadeCharmId.FragileHeart, string.Empty);
            brokeCharm |= TryBreakFragileCharm(ShadeCharmId.FragileGreed, string.Empty);
            return brokeCharm;
        }

        internal static bool HandleBenchRest()
        {
            if (s_charmInventory == null)
            {
                return false;
            }

            bool repaired = false;
            repaired |= TryRepairFragileCharm(ShadeCharmId.FragileStrength, string.Empty);
            repaired |= TryRepairFragileCharm(ShadeCharmId.FragileHeart, string.Empty);
            repaired |= TryRepairFragileCharm(ShadeCharmId.FragileGreed, string.Empty);
            return repaired;
        }

        internal static bool IsHornetRestingAtBench()
        {
            try
            {
                var gm = GameManager.instance;
                if (gm == null)
                {
                    return true;
                }

                var pd = gm.playerData;
                if (pd == null)
                {
                    return true;
                }

                return pd.atBench;
            }
            catch
            {
                return true;
            }
        }

        private static bool TryBreakFragileCharm(ShadeCharmId charmId, string message)
        {
            if (!s_charmInventory.IsEquipped(charmId) || s_charmInventory.IsBroken(charmId))
            {
                return false;
            }

            if (!s_charmInventory.BreakCharm(charmId))
            {
                return false;
            }

            ShadeCharmDefinition definition = s_charmInventory.GetDefinition(charmId);
            string displayName = definition.DisplayName;
            Sprite? icon = definition.BrokenIcon ?? definition.Icon;
            EnqueueNotification(
                $"shade::fragile_{charmId.ToString().ToLowerInvariant()}_broken::{Guid.NewGuid():N}",
                string.IsNullOrWhiteSpace(message) ? displayName + " broken!" : message,
                ShadeUnlockNotificationType.Charm,
                icon: icon);
            return true;
        }

        private static bool TryRepairFragileCharm(ShadeCharmId charmId, string message)
        {
            if (!s_charmInventory.IsBroken(charmId))
            {
                return false;
            }

            if (!s_charmInventory.RepairCharm(charmId))
            {
                return false;
            }

            ShadeCharmDefinition definition = s_charmInventory.GetDefinition(charmId);
            string displayName = definition.DisplayName;
            Sprite? icon = definition.Icon;
            EnqueueNotification(
                $"shade::fragile_{charmId.ToString().ToLowerInvariant()}_repaired::{Guid.NewGuid():N}",
                string.IsNullOrWhiteSpace(message) ? displayName + " repaired!" : message,
                ShadeUnlockNotificationType.Charm,
                icon: icon);
            return true;
        }

        private static void RaiseNotificationsChanged()
        {
            try
            {
                NotificationsChanged?.Invoke();
            }
            catch (Exception ex)
            {
                try
                {
                    Debug.LogException(ex);
                }
                catch
                {
                }
            }
        }

        private static void EnsureActiveSlot()
        {
            if (!s_hasActiveSlot)
            {
                SetActiveSlot(0);
            }
        }

        private static void TryGrantVoidHeartForScene(string? sceneName)
        {
            if (!string.Equals(sceneName, VoidHeartUnlockSceneName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (IsCharmCollected(ShadeCharmId.VoidHeart))
            {
                return;
            }

            bool granted = TryCollectCharm(ShadeCharmId.VoidHeart);
            if (!granted)
            {
                return;
            }

            QueueVoidHeartUnlockNotification();
        }

        private static void QueueVoidHeartUnlockNotification()
        {
            try
            {
                var definition = s_charmInventory?.GetDefinition(ShadeCharmId.VoidHeart);
                string displayName = definition?.DisplayName ?? "Void Heart";
                string message = FormattableString.Invariant($"{displayName} awakened.");
                string key = $"shade::void_heart_scene_unlock::{Guid.NewGuid():N}";
                EnqueueNotification(key, message, ShadeUnlockNotificationType.Charm);
            }
            catch
            {
            }
        }

        private static void HandleCharmInventoryChanged()
        {
            EnsureActiveSlot();

            if (!s_debugUnlockAllCharmsActive)
            {
                PersistInventoryToSlot(s_activeSlot);
            }

            LegacyHelper.RequestShadeLoadoutRecompute();
        }

        private static void SyncInventoryFromActiveSlot()
        {
            EnsureActiveSlot();

            s_saveSlots.GetOrCreateSlot(s_activeSlot);
            bool debugPersisted = s_saveSlots.IsDebugUnlockActive(s_activeSlot);
            DisableDebugUnlockIfActive(persist: !debugPersisted);

            var owned = s_saveSlots.GetCollectedCharms(s_activeSlot);
            var broken = s_saveSlots.GetBrokenCharms(s_activeSlot);
            var equipped = ConvertToCharmIds(s_saveSlots.GetEquippedCharms(s_activeSlot, 0));
            var newlyDiscovered = s_saveSlots.GetNewlyDiscoveredCharms(s_activeSlot);
            int notchCapacity = s_saveSlots.GetNotchCapacity(s_activeSlot);

            s_charmInventory.LoadState(owned, equipped, broken, notchCapacity, newlyDiscovered);
            ApplySavedDebugUnlockState();
        }

        private static void ApplySavedDebugUnlockState()
        {
            if (!s_saveSlots.IsDebugUnlockActive(s_activeSlot))
            {
                s_debugUnlockAllCharmsActive = false;
                s_debugCharmSnapshot = null;
                return;
            }

            var debugSnapshot = s_saveSlots.GetDebugUnlockSnapshot(s_activeSlot);
            if (!debugSnapshot.HasValue)
            {
                s_debugUnlockAllCharmsActive = false;
                s_debugCharmSnapshot = null;
                return;
            }

            var data = debugSnapshot.Value;
            s_debugUnlockAllCharmsActive = true;
            s_debugCharmSnapshot = new CharmInventorySnapshot(
                data.Owned,
                data.Equipped,
                data.Broken,
                data.NewlyDiscovered,
                data.NotchCapacity);

            var allOwned = s_charmInventory.AllCharms
                .Select(def => def.EnumId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToArray();

            int slotNotchCapacity = Mathf.Clamp(s_saveSlots.GetNotchCapacity(s_activeSlot), 0, 20);
            int debugNotchCapacity = slotNotchCapacity > 0
                ? slotNotchCapacity
                : Mathf.Clamp(s_debugCharmSnapshot.Value.NotchCapacity, 0, 20);
            s_charmInventory.LoadState(
                allOwned,
                s_debugCharmSnapshot.Value.Equipped,
                s_debugCharmSnapshot.Value.Broken,
                debugNotchCapacity,
                s_debugCharmSnapshot.Value.NewlyDiscovered);
        }

        private static IEnumerable<ShadeCharmId> ConvertToCharmIds(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                if (Enum.IsDefined(typeof(ShadeCharmId), value))
                {
                    yield return (ShadeCharmId)value;
                }
            }
        }

        private static CharmInventorySnapshot CaptureCharmInventorySnapshot()
        {
            return new CharmInventorySnapshot(
                s_charmInventory.GetOwnedCharms().ToArray(),
                s_charmInventory.GetEquipped().ToArray(),
                s_charmInventory.GetBrokenCharms().ToArray(),
                s_charmInventory.GetNewlyDiscovered().ToArray(),
                s_charmInventory.NotchCapacity);
        }

        private static void DisableDebugUnlockIfActive(bool persist = true)
        {
            if (!s_debugUnlockAllCharmsActive)
            {
                s_debugCharmSnapshot = null;
                return;
            }

            var snapshot = s_debugCharmSnapshot;
            s_debugUnlockAllCharmsActive = false;
            s_debugCharmSnapshot = null;

            if (persist)
            {
                ShadeDebugCharmSnapshot? persistenceSnapshot = snapshot.HasValue
                    ? new ShadeDebugCharmSnapshot(
                        snapshot.Value.Owned,
                        snapshot.Value.Equipped,
                        snapshot.Value.Broken,
                        snapshot.Value.NewlyDiscovered,
                        snapshot.Value.NotchCapacity)
                    : (ShadeDebugCharmSnapshot?)null;
                s_saveSlots.SetDebugUnlockState(s_activeSlot, false, persistenceSnapshot);
            }

            if (snapshot.HasValue)
            {
                s_charmInventory.LoadState(snapshot.Value.Owned, snapshot.Value.Equipped, snapshot.Value.Broken, snapshot.Value.NotchCapacity, snapshot.Value.NewlyDiscovered);
                LegacyHelper.RequestShadeLoadoutRecompute();
            }
        }

        private static int TryGetProfileId(GameManager? gameManager)
        {
            try
            {
                if (gameManager == null)
                {
                    return s_hasActiveSlot ? s_activeSlot : 0;
                }

                if (gameManager.profileID > 0)
                {
                    return NormalizeSlotIndex(gameManager.profileID);
                }

                if (gameManager.playerData != null && gameManager.playerData.profileID > 0)
                {
                    return NormalizeSlotIndex(gameManager.playerData.profileID);
                }

                const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                var type = gameManager.GetType();

                var field = type.GetField("profileID", flags)
                    ?? type.GetField("profileId", flags)
                    ?? type.GetField("ProfileID", flags)
                    ?? type.GetField("ProfileId", flags);

                if (field != null && field.FieldType == typeof(int))
                {
                    var value = field.GetValue(gameManager);
                    if (value is int intField)
                    {
                        return NormalizeSlotIndex(intField);
                    }
                }

                var property = type.GetProperty("profileID", flags)
                    ?? type.GetProperty("profileId", flags)
                    ?? type.GetProperty("ProfileID", flags)
                    ?? type.GetProperty("ProfileId", flags);

                if (property != null && property.PropertyType == typeof(int) && property.GetIndexParameters().Length == 0)
                {
                    var propertyValue = property.GetValue(gameManager);
                    if (propertyValue is int intProperty)
                    {
                        return NormalizeSlotIndex(intProperty);
                    }
                }

                MemberInfo? playerDataMember = (MemberInfo?)type.GetField("playerData", flags)
                    ?? (MemberInfo?)type.GetProperty("playerData", flags)
                    ?? (MemberInfo?)type.GetField("PlayerData", flags)
                    ?? (MemberInfo?)type.GetProperty("PlayerData", flags);

                object? playerData = null;
                switch (playerDataMember)
                {
                    case FieldInfo playerDataField:
                        playerData = playerDataField.GetValue(gameManager);
                        break;
                    case PropertyInfo playerDataProperty when playerDataProperty.GetIndexParameters().Length == 0:
                        playerData = playerDataProperty.GetValue(gameManager);
                        break;
                }

                if (playerData != null)
                {
                    var playerDataType = playerData.GetType();
                    var profileField = playerDataType.GetField("profileID", flags)
                        ?? playerDataType.GetField("profileId", flags)
                        ?? playerDataType.GetField("ProfileID", flags)
                        ?? playerDataType.GetField("ProfileId", flags);

                    if (profileField != null && profileField.FieldType == typeof(int))
                    {
                        var profileValue = profileField.GetValue(playerData);
                        if (profileValue is int playerDataFieldValue)
                        {
                            return NormalizeSlotIndex(playerDataFieldValue);
                        }
                    }

                    var profileProperty = playerDataType.GetProperty("profileID", flags)
                        ?? playerDataType.GetProperty("profileId", flags)
                        ?? playerDataType.GetProperty("ProfileID", flags)
                        ?? playerDataType.GetProperty("ProfileId", flags);

                    if (profileProperty != null
                        && profileProperty.PropertyType == typeof(int)
                        && profileProperty.GetIndexParameters().Length == 0)
                    {
                        var profilePropertyValue = profileProperty.GetValue(playerData);
                        if (profilePropertyValue is int playerDataPropertyValue)
                        {
                            return NormalizeSlotIndex(playerDataPropertyValue);
                        }
                    }
                }
            }
            catch
            {
                // Ignore reflection failures and fall back to default slot.
            }

            return 0;
        }

        private static int NormalizeSlotIndex(int slot)
        {
            if (slot <= 0)
            {
                return 0;
            }

            return slot - 1;
        }

        private readonly struct CharmInventorySnapshot
        {
            public CharmInventorySnapshot(
                ShadeCharmId[] owned,
                ShadeCharmId[] equipped,
                ShadeCharmId[] broken,
                ShadeCharmId[] newlyDiscovered,
                int notchCapacity)
            {
                Owned = owned ?? Array.Empty<ShadeCharmId>();
                Equipped = equipped ?? Array.Empty<ShadeCharmId>();
                Broken = broken ?? Array.Empty<ShadeCharmId>();
                NewlyDiscovered = newlyDiscovered ?? Array.Empty<ShadeCharmId>();
                NotchCapacity = Mathf.Clamp(notchCapacity, 0, 20);
            }

            public ShadeCharmId[] Owned { get; }

            public ShadeCharmId[] Equipped { get; }

            public ShadeCharmId[] Broken { get; }

            public ShadeCharmId[] NewlyDiscovered { get; }

            public int NotchCapacity { get; }
        }

        internal sealed class ShadeUnlockNotification
        {
            public const float DefaultDuration = 3.5f;

            public ShadeUnlockNotification(string? key, string message, ShadeUnlockNotificationType type, float duration, Sprite? icon = null)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentException("Notification message cannot be empty.", nameof(message));
                }

                Message = message.Trim();
                Type = type;
                Duration = SanitizeDuration(duration);
                Key = NormalizeKey(key, Message);
                Icon = icon;
            }

            public string Key { get; }

            public string Message { get; }

            public ShadeUnlockNotificationType Type { get; }

            public float Duration { get; }

            public Sprite? Icon { get; }

            private static string NormalizeKey(string? key, string message)
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    return key.Trim();
                }

                return $"shade::notification::{message.Trim()}";
            }

            private static float SanitizeDuration(float value)
            {
                if (float.IsNaN(value) || float.IsInfinity(value) || value <= 0f)
                {
                    return DefaultDuration;
                }

                return value;
            }
        }

    }
}
