#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using GlobalSettings;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (ShadeCharmPlacementHelpers.PickupAlreadyPresent(placement))
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
                position.z = context.Hero.position.z;
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

                pickup.gameObject.name = placement.ItemKind == ShadeCharmPlacementItemKind.Notch
                    ? BuildNotchPickupName(placement.NotchId)
                    : $"ShadeCharmPickup_{placement.CharmId}";
                pickup.transform.position = position;
                pickup.transform.rotation = rotation;
                try
                {
                    SceneManager.MoveGameObjectToScene(pickup.gameObject, SceneManager.GetActiveScene());
                }
                catch
                {
                }

                var savedItem = CreateSavedItem(placement);
                if (savedItem == null)
                {
                    UnityEngine.Object.Destroy(pickup.gameObject);
                    ShadeCharmPlacementService.LogWarning($"Failed to create saved item for {ShadeCharmPlacementService.DescribePlacement(placement)} in scene '{context.SceneName}'.");
                    return;
                }

                pickup.SetItem(savedItem);
                if (!string.IsNullOrEmpty(placement.PlayerDataBool))
                {
                    pickup.SetPlayerDataBool(placement.PlayerDataBool);
                }
                pickup.SetFling(placement.FlingPickup ?? false);

                ShadeCharmPlacementHelpers.ApplyPickupVisuals(pickup, context.Hero, placement);
                ShadeCharmPlacementService.LogInfo($"Spawned {ShadeCharmPlacementService.DescribePlacement(placement)} pickup at {position} in scene '{context.SceneName}'.");
            }
            catch (Exception ex)
            {
                ShadeCharmPlacementService.LogWarning($"Failed to spawn {ShadeCharmPlacementService.DescribePlacement(placement)} in scene '{context.SceneName}': {ex}");
            }
        }

        private static SavedItem? CreateSavedItem(ShadeCharmPlacementDefinition placement)
        {
            switch (placement.ItemKind)
            {
                case ShadeCharmPlacementItemKind.Notch:
                    string notchId = (placement.NotchId ?? string.Empty).Trim();
                    if (string.IsNullOrEmpty(notchId))
                    {
                        return null;
                    }

                    int increment = Mathf.Max(1, placement.NotchIncrement ?? 1);
                    return ShadeNotchSavedItem.Create(notchId, increment);
                default:
                    return ShadeCharmSavedItem.Create(placement.CharmId);
            }
        }

        private static string BuildNotchPickupName(string? notchId)
        {
            if (string.IsNullOrWhiteSpace(notchId))
            {
                return "ShadeNotchPickup";
            }

            var builder = new StringBuilder();
            foreach (char ch in notchId)
            {
                builder.Append(char.IsLetterOrDigit(ch) ? ch : '_');
            }

            string sanitized = builder.ToString().Trim('_');
            if (string.IsNullOrEmpty(sanitized))
            {
                sanitized = "Notch";
            }

            return $"ShadeNotchPickup_{sanitized}";
        }
    }
}
