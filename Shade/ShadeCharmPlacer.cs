#nullable enable

using System;
using System.Linq;
using GlobalSettings;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal static class ShadeCharmPlacer
    {
        private sealed record Placement(ShadeCharmId CharmId, Vector2 Offset);

        private static readonly Placement[] BoneBottomPlacements =
        {
            new Placement(ShadeCharmId.SoulEater, new Vector2(2.2f, 0.6f)),
            new Placement(ShadeCharmId.Grubsong, new Vector2(-1.6f, 0.8f)),
            new Placement(ShadeCharmId.QuickFocus, new Vector2(3.0f, -0.4f)),
            new Placement(ShadeCharmId.DeepFocus, new Vector2(0.3f, 2.2f)),
            new Placement(ShadeCharmId.SteadyBody, new Vector2(-2.6f, -0.7f)),
            new Placement(ShadeCharmId.StalwartShell, new Vector2(1.7f, -1.1f)),
            new Placement(ShadeCharmId.GrubberflysElegy, new Vector2(-0.4f, 1.8f)),
            new Placement(ShadeCharmId.FragileGreed, new Vector2(2.9f, 1.4f)),
            new Placement(ShadeCharmId.HeavyBlow, new Vector2(-3.0f, -1.2f)),
            new Placement(ShadeCharmId.BaldurShell, new Vector2(3.2f, 0.9f)),
            new Placement(ShadeCharmId.LifebloodHeart, new Vector2(-2.8f, 0.1f))
        };

        private static readonly Placement[] MossGrottoPlacements =
        {
            new Placement(ShadeCharmId.ShapeOfUnn, new Vector2(1.5f, 0.9f)),
            new Placement(ShadeCharmId.FuryOfTheFallen, new Vector2(-1.9f, 1.4f)),
            new Placement(ShadeCharmId.NailmastersGlory, new Vector2(2.4f, -0.6f)),
            new Placement(ShadeCharmId.FragileHeart, new Vector2(-2.3f, -0.9f)),
            new Placement(ShadeCharmId.SharpShadow, new Vector2(0.8f, 2.0f)),
            new Placement(ShadeCharmId.LifebloodCore, new Vector2(-3.2f, 1.8f)),
            new Placement(ShadeCharmId.JonisBlessing, new Vector2(3.0f, -1.5f)),
            new Placement(ShadeCharmId.CarefreeMelody, new Vector2(-1.9f, -2.4f)),
            new Placement(ShadeCharmId.Hiveblood, new Vector2(-1.2f, 2.5f)),
            new Placement(ShadeCharmId.Kingsoul, new Vector2(1.5f, -2.0f)),
            new Placement(ShadeCharmId.VoidHeart, new Vector2(0.2f, 2.7f))
        };

        public static void PopulateScene(string? sceneName, Transform? heroTransform)
        {
            if (string.IsNullOrWhiteSpace(sceneName) || heroTransform == null)
            {
                return;
            }

            string lower = sceneName.ToLowerInvariant();
            if (lower.Contains("bone") && lower.Contains("bottom"))
            {
                SpawnPlacements(BoneBottomPlacements, heroTransform);
            }
            else if (lower.Contains("moss") && lower.Contains("grotto"))
            {
                SpawnPlacements(MossGrottoPlacements, heroTransform);
            }
        }

        private static void SpawnPlacements(Placement[] placements, Transform heroTransform)
        {
            foreach (var placement in placements)
            {
                if (ShadeRuntime.IsCharmCollected(placement.CharmId))
                {
                    continue;
                }

                if (PickupAlreadyPresent(placement.CharmId))
                {
                    continue;
                }

                TrySpawnPickup(heroTransform, placement);
            }
        }

        private static bool PickupAlreadyPresent(ShadeCharmId charmId)
        {
            try
            {
                var existing = UnityEngine.Object.FindObjectsByType<CollectableItemPickup>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
                return existing.Any(p => p != null && p.Item is ShadeCharmSavedItem item && item.CharmId == charmId);
            }
            catch
            {
                return false;
            }
        }

        private static void TrySpawnPickup(Transform heroTransform, Placement placement)
        {
            try
            {
                var prefab = Gameplay.CollectableItemPickupPrefab;
                if (prefab == null)
                {
                    return;
                }

                var pickup = UnityEngine.Object.Instantiate(prefab);
                if (pickup == null)
                {
                    return;
                }

                pickup.gameObject.name = $"ShadeCharmPickup_{placement.CharmId}";
                Vector3 basePos = heroTransform.position;
                pickup.transform.position = new Vector3(basePos.x + placement.Offset.x, basePos.y + placement.Offset.y, basePos.z);
                pickup.transform.rotation = Quaternion.identity;

                pickup.SetItem(ShadeCharmSavedItem.Create(placement.CharmId));
                pickup.SetPlayerDataBool(string.Empty);
                pickup.SetFling(false);

                ApplyPickupVisuals(pickup, heroTransform, placement.CharmId);
            }
            catch
            {
            }
        }

        private static int TryGetHeroSortingLayer(Transform heroTransform)
        {
            try
            {
                var renderer = heroTransform.GetComponentInChildren<SpriteRenderer>();
                if (renderer != null)
                {
                    return renderer.sortingLayerID;
                }
            }
            catch
            {
            }

            return 0;
        }

        private static int TryGetHeroSortingOrder(Transform heroTransform)
        {
            try
            {
                var renderer = heroTransform.GetComponentInChildren<SpriteRenderer>();
                if (renderer != null)
                {
                    return renderer.sortingOrder;
                }
            }
            catch
            {
            }

            return 0;
        }

        private static void ApplyPickupVisuals(CollectableItemPickup pickup, Transform heroTransform, ShadeCharmId charmId)
        {
            try
            {
                var spriteRenderer = pickup.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    return;
                }

                var sprite = ShadeCharmSavedItem.ResolveCharmSprite(charmId, out var tint);
                spriteRenderer.sprite = sprite;
                spriteRenderer.color = tint;
                spriteRenderer.sortingLayerID = TryGetHeroSortingLayer(heroTransform);
                spriteRenderer.sortingOrder = TryGetHeroSortingOrder(heroTransform) + 1;
            }
            catch
            {
            }
        }
    }
}
