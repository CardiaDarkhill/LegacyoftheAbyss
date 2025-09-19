#nullable enable

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
    }
}
