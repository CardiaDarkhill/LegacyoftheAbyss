#nullable enable

using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Central coordination point for shade state that has to live longer than a single scene. The runtime
    /// exposes a simple API today but can later be extended to coordinate with save slots, file IO, or
    /// charm inventories without forcing the rest of the mod to understand those details.
    /// </summary>
    internal static class ShadeRuntime
    {
        private static readonly ShadePersistentState s_persistentState = new ShadePersistentState();
        private static readonly ShadeSaveSlotRepository s_saveSlots = new ShadeSaveSlotRepository();
        private static readonly ShadeCharmInventory s_charmInventory = new ShadeCharmInventory();
        private static readonly Queue<ShadeUnlockNotification> s_notificationQueue = new();
        private static readonly HashSet<string> s_seenNotificationKeys = new(StringComparer.OrdinalIgnoreCase);

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

        public static bool TryGetPersistentState(out int currentHp, out int maxHp, out int soul, out bool canTakeDamage)
        {
            if (!s_persistentState.HasData)
            {
                currentHp = -1;
                maxHp = -1;
                soul = -1;
                canTakeDamage = true;
                return false;
            }

            currentHp = s_persistentState.CurrentHP;
            maxHp = s_persistentState.MaxHP;
            soul = s_persistentState.Soul;
            canTakeDamage = s_persistentState.CanTakeDamage;
            return true;
        }

        public static void CaptureState(int currentHp, int maxHp, int soul, bool? canTakeDamage = null)
        {
            s_persistentState.Capture(currentHp, maxHp, soul, canTakeDamage);
        }

        public static void EnsureMinimumHealth(int minimum)
        {
            s_persistentState.ForceMinimumHealth(minimum);
        }

        public static void Clear()
        {
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

        public static bool EnqueueNotification(string? key, string message, ShadeUnlockNotificationType type, float duration = ShadeUnlockNotification.DefaultDuration)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            var notification = new ShadeUnlockNotification(key, message, type, duration);
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

<<<<<<< ours
        public static IReadOnlyCollection<int> GetDiscoveredCharms()
        {
            return s_persistentState.GetDiscoveredCharmIdsSnapshot();
        }

        public static bool IsCharmDiscovered(int charmId)
        {
            return s_persistentState.HasDiscoveredCharm(charmId);
        }

        public static bool UnlockCharm(int charmId)
        {
            return s_persistentState.UnlockCharm(charmId);
        }

        public static IReadOnlyCollection<int> GetEquippedCharms(int loadoutId)
        {
            return s_persistentState.GetEquippedCharms(loadoutId);
        }

        public static IReadOnlyDictionary<int, IReadOnlyCollection<int>> GetEquippedCharmLoadouts()
        {
            return s_persistentState.GetEquippedCharmLoadouts();
        }

        public static bool EquipCharm(int loadoutId, int charmId)
        {
            return s_persistentState.EquipCharm(loadoutId, charmId);
        }

        public static bool UnequipCharm(int loadoutId, int charmId)
        {
            return s_persistentState.UnequipCharm(loadoutId, charmId);
        }

        public static void ClearLoadout(int loadoutId)
        {
            s_persistentState.ClearLoadout(loadoutId);
        }

        public static int GetNotchCapacity()
        {
            return s_persistentState.NotchCapacity;
        }

        public static bool SetNotchCapacity(int capacity)
        {
            return s_persistentState.SetNotchCapacity(capacity);
=======
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
            int clamped;
            if (s_saveSlots.MaxSlots > 0)
            {
                clamped = Mathf.Clamp(slot, 0, s_saveSlots.MaxSlots - 1);
            }
            else
            {
                clamped = 0;
            }

            s_activeSlot = clamped;
            s_hasActiveSlot = true;
            s_saveSlots.GetOrCreateSlot(s_activeSlot);
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
            return s_saveSlots.MarkCharmCollected(s_activeSlot, charmId);
        }

        private static void EnsureActiveSlot()
        {
            if (!s_hasActiveSlot)
            {
                SetActiveSlot(0);
            }
        }

        private static int TryGetProfileId(GameManager gameManager)
        {
            try
            {
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
                        return Math.Max(0, intField);
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
                        return Math.Max(0, intProperty);
                    }
                }
            }
            catch
            {
                // Ignore reflection failures and fall back to default slot.
            }

            return 0;
>>>>>>> theirs
        }
    }
}
