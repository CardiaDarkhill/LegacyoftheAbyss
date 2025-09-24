#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed class ContainerPlacementHandler : IShadeCharmPlacementHandler
    {
        public void Populate(in ShadeCharmPlacementContext context, IReadOnlyList<ShadeCharmPlacementDefinition> placements)
        {
            if (placements == null || placements.Count == 0)
            {
                return;
            }

            foreach (var placement in placements)
            {
                TryPopulateContainer(context, placement);
            }
        }

        private static void TryPopulateContainer(in ShadeCharmPlacementContext context, ShadeCharmPlacementDefinition placement)
        {
            var data = placement.Container;
            string? targetPath = data?.TargetPath ?? placement.AnchorPath;

            Transform? target = null;
            if (!string.IsNullOrWhiteSpace(targetPath))
            {
                target = ShadeCharmPlacementHelpers.FindTransform(targetPath);
            }

            if (target == null && !string.IsNullOrWhiteSpace(data?.TargetName))
            {
                target = ShadeCharmPlacementHelpers.FindTransformByName(data.TargetName);
            }

            if (target == null)
            {
                ShadeCharmPlacementService.LogWarning($"Charm {placement.CharmId} container target not found (path='{targetPath}', name='{data?.TargetName}') in scene '{context.SceneName}'.");
                return;
            }

            var pickup = target.GetComponent<CollectableItemPickup>() ?? target.GetComponentInChildren<CollectableItemPickup>(true);
            if (pickup == null)
            {
                ShadeCharmPlacementService.LogWarning($"Charm {placement.CharmId} container '{target.name}' lacks a CollectableItemPickup component.");
                return;
            }

            if (pickup.Item is ShadeCharmSavedItem existing && existing.CharmId == placement.CharmId)
            {
                return;
            }

            try
            {
                pickup.SetItem(ShadeCharmSavedItem.Create(placement.CharmId), keepPersistence: true);

                if (!string.IsNullOrEmpty(placement.PlayerDataBool))
                {
                    pickup.SetPlayerDataBool(placement.PlayerDataBool);
                }

                pickup.SetFling(placement.FlingPickup ?? false);

                ShadeCharmPlacementHelpers.ApplyPickupVisuals(pickup, context.Hero, placement.CharmId);
            }
            catch (Exception ex)
            {
                ShadeCharmPlacementService.LogWarning($"Failed to assign container placement for charm {placement.CharmId}: {ex}");
            }
        }
    }
}
