#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LegacyoftheAbyss.Shade
{
    internal static class ShadeCharmPlacementHelpers
    {
        internal static Transform? FindTransform(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            try
            {
                var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length == 0)
                {
                    return null;
                }

                var scene = SceneManager.GetActiveScene();
                if (scene.IsValid())
                {
                    foreach (var root in scene.GetRootGameObjects())
                    {
                        if (root == null)
                        {
                            continue;
                        }

                        var result = FindRecursive(root.transform, segments, 0);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }

                var fallback = GameObject.Find(path);
                return fallback != null ? fallback.transform : null;
            }
            catch
            {
                return null;
            }
        }

        internal static Transform? FindTransformByName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            try
            {
                var scene = SceneManager.GetActiveScene();
                if (scene.IsValid())
                {
                    foreach (var root in scene.GetRootGameObjects())
                    {
                        var result = FindByNameRecursive(root.transform, name);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }

                var fallback = GameObject.Find(name);
                return fallback != null ? fallback.transform : null;
            }
            catch
            {
                return null;
            }
        }

        internal static string GetHierarchyPath(Transform? transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            var stack = new Stack<string>();
            var current = transform;
            while (current != null)
            {
                stack.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", stack);
        }

        internal static bool PickupAlreadyPresent(ShadeCharmPlacementDefinition placement)
        {
            if (placement == null)
            {
                return false;
            }

            try
            {
                var existing = UnityEngine.Object.FindObjectsByType<CollectableItemPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var pickup in existing)
                {
                    if (pickup?.Item == null)
                    {
                        continue;
                    }

                    switch (placement.ItemKind)
                    {
                        case ShadeCharmPlacementItemKind.Notch:
                            if (pickup.Item is ShadeNotchSavedItem notch && notch.TargetCapacity == Mathf.Max(0, placement.NotchTargetCapacity ?? 0))
                            {
                                return true;
                            }

                            break;
                        default:
                            if (pickup.Item is ShadeCharmSavedItem charm && charm.CharmId == placement.CharmId)
                            {
                                return true;
                            }

                            break;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        internal static void ApplyPickupVisuals(CollectableItemPickup pickup, Transform? heroTransform, ShadeCharmPlacementDefinition placement)
        {
            if (pickup == null || placement == null)
            {
                return;
            }

            switch (placement.ItemKind)
            {
                case ShadeCharmPlacementItemKind.Notch:
                    ApplyNotchPickupVisuals(pickup, heroTransform);
                    break;
                default:
                    ApplyCharmPickupVisuals(pickup, heroTransform, placement.CharmId);
                    break;
            }
        }

        private static void ApplyCharmPickupVisuals(CollectableItemPickup pickup, Transform? heroTransform, ShadeCharmId charmId)
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

                if (heroTransform != null)
                {
                    spriteRenderer.sortingLayerID = TryGetHeroSortingLayer(heroTransform);
                    spriteRenderer.sortingOrder = TryGetHeroSortingOrder(heroTransform) + 1;
                }
            }
            catch
            {
            }
        }

        private static void ApplyNotchPickupVisuals(CollectableItemPickup pickup, Transform? heroTransform)
        {
            try
            {
                var spriteRenderer = pickup.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    return;
                }

                var sprite = ShadeNotchSavedItem.ResolveSprite(out var tint);
                spriteRenderer.sprite = sprite;
                spriteRenderer.color = tint;

                if (heroTransform != null)
                {
                    spriteRenderer.sortingLayerID = TryGetHeroSortingLayer(heroTransform);
                    spriteRenderer.sortingOrder = TryGetHeroSortingOrder(heroTransform) + 1;
                }
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

        private static Transform? FindRecursive(Transform current, IReadOnlyList<string> segments, int index)
        {
            if (!NameEquals(current.name, segments[index]))
            {
                return null;
            }

            if (index == segments.Count - 1)
            {
                return current;
            }

            int nextIndex = index + 1;
            foreach (Transform child in current)
            {
                var match = FindRecursive(child, segments, nextIndex);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static Transform? FindByNameRecursive(Transform current, string name)
        {
            if (NameEquals(current.name, name))
            {
                return current;
            }

            foreach (Transform child in current)
            {
                var match = FindByNameRecursive(child, name);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static bool NameEquals(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }
    }
}
