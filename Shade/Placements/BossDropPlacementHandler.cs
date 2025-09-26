#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed class BossDropPlacementHandler : IShadeCharmPlacementHandler
    {
        internal static BossDropPlacementHandler? Instance { get; private set; }

        private readonly List<ShadeCharmPlacementDefinition> _activePlacements = new();
        private string _sceneName = string.Empty;

        internal BossDropPlacementHandler()
        {
            Instance = this;
        }

        public void Populate(in ShadeCharmPlacementContext context, IReadOnlyList<ShadeCharmPlacementDefinition> placements)
        {
            _sceneName = context.SceneName;
            _activePlacements.Clear();

            if (placements == null || placements.Count == 0)
            {
                return;
            }

            foreach (var placement in placements)
            {
                if (placement.BossDrop == null)
                {
                    continue;
                }

                if (placement.ItemKind != ShadeCharmPlacementItemKind.Charm)
                {
                    ShadeCharmPlacementService.LogWarning($"Boss drop placements do not support {ShadeCharmPlacementService.DescribePlacement(placement)}; skipping.");
                    continue;
                }

                _activePlacements.Add(placement);
            }
        }

        internal void TryRegisterDrop(HealthManager manager)
        {
            if (manager == null || _activePlacements.Count == 0)
            {
                return;
            }

            foreach (var placement in _activePlacements)
            {
                if (!Matches(manager, placement))
                {
                    continue;
                }

                InjectDrop(manager, placement);
            }
        }

        private bool Matches(HealthManager manager, ShadeCharmPlacementDefinition placement)
        {
            var data = placement.BossDrop;
            if (data == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(data.TargetPath))
            {
                string currentPath = ShadeCharmPlacementHelpers.GetHierarchyPath(manager.transform);
                if (!string.Equals(currentPath, data.TargetPath, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            string enemyName = manager.gameObject?.name ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(data.EnemyName))
            {
                if (!string.Equals(enemyName, data.EnemyName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (data.EnemyNameContainsAll != null && data.EnemyNameContainsAll.Length > 0)
            {
                foreach (var token in data.EnemyNameContainsAll)
                {
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        continue;
                    }

                    if (enemyName.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        return false;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(data.EnemyName) && (data.EnemyNameContainsAll == null || data.EnemyNameContainsAll.Length == 0) && string.IsNullOrWhiteSpace(data.TargetPath))
            {
                ShadeCharmPlacementService.LogWarning($"Boss drop placement for charm {placement.CharmId} in scene '{_sceneName}' does not specify enemyName, enemyNameContainsAll, or targetPath; skipping.");
                return false;
            }

            return true;
        }

        private void InjectDrop(HealthManager manager, ShadeCharmPlacementDefinition placement)
        {
            if (ShadeCharmPlacementService.IsCharmAlreadyCollected(placement.CharmId))
            {
                return;
            }

            try
            {
                foreach (var item in manager.EnumerateItemDrops())
                {
                    if (item is ShadeCharmSavedItem existing && existing.CharmId == placement.CharmId)
                    {
                        return;
                    }
                }
            }
            catch
            {
            }

            try
            {
                if (TryInjectDropInternal(manager, placement))
                {
                    ShadeCharmPlacementService.LogInfo($"Registered boss drop for charm {placement.CharmId} on '{manager.gameObject.name}' in scene '{_sceneName}'.");
                }
            }
            catch (Exception ex)
            {
                ShadeCharmPlacementService.LogWarning($"Failed to register boss drop for charm {placement.CharmId}: {ex}");
            }
        }

        private bool TryInjectDropInternal(HealthManager manager, ShadeCharmPlacementDefinition placement)
        {
            if (!EnsureReflection())
            {
                return false;
            }

            var data = placement.BossDrop!;

            var groupsObj = s_itemDropGroupsField!.GetValue(manager) as IList;
            if (groupsObj == null)
            {
                var listType = typeof(List<>).MakeGenericType(s_itemDropGroupType!);
                groupsObj = (IList?)Activator.CreateInstance(listType);
                if (groupsObj == null)
                {
                    return false;
                }

                s_itemDropGroupsField.SetValue(manager, groupsObj);
            }

            if (data.ClearExistingDrops == true)
            {
                groupsObj.Clear();
            }

            var drop = Activator.CreateInstance(s_itemDropProbabilityType!);
            if (drop == null)
            {
                return false;
            }

            s_itemDropProbabilityItemField!.SetValue(drop, ShadeCharmSavedItem.Create(placement.CharmId));
            s_itemDropProbabilityChanceField!.SetValue(drop, Math.Max(0.0001f, data.Probability ?? 1f));

            var dropsListType = typeof(List<>).MakeGenericType(s_itemDropProbabilityType!);
            var dropsList = (IList?)Activator.CreateInstance(dropsListType);
            if (dropsList == null)
            {
                return false;
            }

            dropsList.Add(drop);

            var group = Activator.CreateInstance(s_itemDropGroupType!);
            if (group == null)
            {
                return false;
            }

            s_itemDropGroupDropsField!.SetValue(group, dropsList);
            s_itemDropGroupTotalProbabilityField!.SetValue(group, 1f);

            groupsObj.Add(group);
            return true;
        }

        private static bool EnsureReflection()
        {
            if (s_reflectionInitialized)
            {
                return s_itemDropProbabilityType != null && s_itemDropGroupType != null && s_itemDropGroupsField != null && s_itemDropProbabilityItemField != null && s_itemDropProbabilityChanceField != null && s_itemDropGroupDropsField != null && s_itemDropGroupTotalProbabilityField != null;
            }

            s_reflectionInitialized = true;

            try
            {
                s_itemDropProbabilityType = AccessTools.Inner(typeof(HealthManager), "ItemDropProbability");
                s_itemDropGroupType = AccessTools.Inner(typeof(HealthManager), "ItemDropGroup");
                s_itemDropGroupsField = AccessTools.Field(typeof(HealthManager), "itemDropGroups");

                if (s_itemDropProbabilityType != null)
                {
                    s_itemDropProbabilityItemField = AccessTools.Field(s_itemDropProbabilityType, "item");
                    s_itemDropProbabilityChanceField = AccessTools.Field(s_itemDropProbabilityType, "Probability");
                }

                if (s_itemDropGroupType != null)
                {
                    s_itemDropGroupDropsField = AccessTools.Field(s_itemDropGroupType, "Drops");
                    s_itemDropGroupTotalProbabilityField = AccessTools.Field(s_itemDropGroupType, "TotalProbability");
                }
            }
            catch (Exception ex)
            {
                ShadeCharmPlacementService.LogWarning($"Failed to initialize boss drop reflection: {ex}");
            }

            return s_itemDropProbabilityType != null && s_itemDropGroupType != null && s_itemDropGroupsField != null && s_itemDropProbabilityItemField != null && s_itemDropProbabilityChanceField != null && s_itemDropGroupDropsField != null && s_itemDropGroupTotalProbabilityField != null;
        }

        private static Type? s_itemDropProbabilityType;
        private static Type? s_itemDropGroupType;
        private static FieldInfo? s_itemDropGroupsField;
        private static FieldInfo? s_itemDropProbabilityItemField;
        private static FieldInfo? s_itemDropProbabilityChanceField;
        private static FieldInfo? s_itemDropGroupDropsField;
        private static FieldInfo? s_itemDropGroupTotalProbabilityField;
        private static bool s_reflectionInitialized;

        [HarmonyPatch(typeof(HealthManager), nameof(HealthManager.OnAwake))]
        private static class HealthManager_OnAwake_Patch
        {
            private static void Postfix(HealthManager __instance)
            {
                var handler = Instance;
                if (handler == null || __instance == null)
                {
                    return;
                }

                handler.TryRegisterDrop(__instance);
            }
        }
    }
}
