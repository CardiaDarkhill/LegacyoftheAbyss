#nullable enable

using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed class GroundPlacementHandler : IShadeCharmPlacementHandler
    {
        public void Populate(in ShadeCharmPlacementContext context, IReadOnlyList<ShadeCharmPlacementDefinition> placements)
        {
            if (placements == null || placements.Count == 0)
            {
                return;
            }

            foreach (var placement in placements)
            {
                TrySpawnPickup(context, placement);
            }
        }

        private static void TrySpawnPickup(in ShadeCharmPlacementContext context, ShadeCharmPlacementDefinition placement)
        {
            if (ShadeCharmPlacementHelpers.PickupAlreadyPresent(placement.CharmId))
            {
                return;
            }

            var prefab = Gameplay.CollectableItemPickupPrefab;
            if (prefab == null)
            {
                ShadeCharmPlacementService.LogWarning("CollectableItemPickupPrefab is unavailable; cannot create ground charm placement.");
                return;
            }

            Transform? anchor = null;
            if (!string.IsNullOrWhiteSpace(placement.AnchorPath))
            {
                anchor = ShadeCharmPlacementHelpers.FindTransform(placement.AnchorPath);
                if (anchor == null)
                {
                    ShadeCharmPlacementService.LogWarning($"Charm {placement.CharmId} anchor '{placement.AnchorPath}' not found in scene '{context.SceneName}'.");
                }
            }

            var baseTransform = anchor ?? context.Hero;
            Vector3 position;
            if (placement.WorldPosition is { } worldPosition)
            {
                position = worldPosition.ToVector3();
            }
            else
            {
                var offset = placement.AnchorOffset?.ToVector3() ?? Vector3.zero;
                position = anchor != null ? anchor.TransformPoint(offset) : baseTransform.position + offset;
            }

            if (anchor == null && placement.WorldPosition == null)
            {
                position.z = context.Hero.position.z;
            }

            Quaternion rotation = Quaternion.identity;
            if (placement.WorldRotationEuler is { } euler)
            {
                rotation = Quaternion.Euler(euler.ToVector3());
            }

            try
            {
                var pickup = UnityEngine.Object.Instantiate(prefab);
                if (pickup == null)
                {
                    return;
                }

                pickup.gameObject.name = $"ShadeCharmPickup_{placement.CharmId}";
                pickup.transform.position = position;
                pickup.transform.rotation = rotation;

                pickup.SetItem(ShadeCharmSavedItem.Create(placement.CharmId));
                pickup.SetPlayerDataBool(placement.PlayerDataBool ?? string.Empty);
                pickup.SetFling(placement.FlingPickup ?? false);

                ShadeCharmPlacementHelpers.ApplyPickupVisuals(pickup, context.Hero, placement.CharmId);
            }
            catch (Exception ex)
            {
                ShadeCharmPlacementService.LogWarning($"Failed to spawn charm {placement.CharmId} in scene '{context.SceneName}': {ex}");
            }
        }
    }
}
