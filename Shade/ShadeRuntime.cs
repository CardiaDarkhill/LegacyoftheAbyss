#nullable enable
using System.Collections.Generic;

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

        public static ShadePersistentState PersistentState => s_persistentState;

        public static ShadeSaveSlotRepository SaveSlots => s_saveSlots;

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
        }

        public static void NotifyHornetSpellUnlocked()
        {
            s_persistentState.AdvanceSpellProgress();
        }

        public static void SyncSpellProgress(int progress)
        {
            s_persistentState.SetSpellProgress(progress);
        }

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
        }
    }
}
