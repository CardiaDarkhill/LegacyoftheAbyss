#nullable enable

using System;
using System.Linq;
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
            new Placement(ShadeCharmId.StalwartShell, new Vector2(1.7f, -1.1f))
        };

        private static readonly Placement[] MossGrottoPlacements =
        {
            new Placement(ShadeCharmId.ShapeOfUnn, new Vector2(1.5f, 0.9f)),
            new Placement(ShadeCharmId.FuryOfTheFallen, new Vector2(-1.9f, 1.4f)),
            new Placement(ShadeCharmId.NailmastersGlory, new Vector2(2.4f, -0.6f)),
            new Placement(ShadeCharmId.FragileHeart, new Vector2(-2.3f, -0.9f)),
            new Placement(ShadeCharmId.SharpShadow, new Vector2(0.8f, 2.0f))
        };

        private static Sprite? s_defaultPickupSprite;

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
                var existing = UnityEngine.Object.FindObjectsOfType<ShadeUnlockPickup>(true);
                return existing.Any(p => p != null && p.charmId == charmId);
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
                var definition = ShadeRuntime.Charms.GetDefinition(placement.CharmId);
                var go = new GameObject($"ShadeCharmPickup_{placement.CharmId}");
                Vector3 basePos = heroTransform.position;
                go.transform.position = new Vector3(basePos.x + placement.Offset.x, basePos.y + placement.Offset.y, basePos.z);

                var spriteRenderer = go.AddComponent<SpriteRenderer>();
                var sprite = definition.Icon ?? GetDefaultPickupSprite();
                spriteRenderer.sprite = sprite;
                spriteRenderer.color = definition.Icon != null ? Color.white : definition.FallbackTint;
                spriteRenderer.sortingLayerID = TryGetHeroSortingLayer(heroTransform);
                spriteRenderer.sortingOrder = TryGetHeroSortingOrder(heroTransform) + 1;
                go.transform.localScale = Vector3.one * 0.85f;

                var collider = go.AddComponent<CircleCollider2D>();
                collider.isTrigger = true;
                collider.radius = 0.6f;

                var pickup = go.AddComponent<ShadeUnlockPickup>();
                pickup.grantCharm = true;
                pickup.charmId = placement.CharmId;
                pickup.useCharmNameForMessage = true;
                pickup.notificationKey = $"shade::charm_pickup::{placement.CharmId}";
                pickup.notificationType = ShadeUnlockNotificationType.Charm;
                pickup.requirePlayerTag = true;
                pickup.destroyOnPickup = true;
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

        private static Sprite GetDefaultPickupSprite()
        {
            if (s_defaultPickupSprite != null)
            {
                return s_defaultPickupSprite;
            }

            var tex = new Texture2D(16, 16, TextureFormat.ARGB32, false);
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    float dx = (x + 0.5f) / tex.width - 0.5f;
                    float dy = (y + 0.5f) / tex.height - 0.5f;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = dist <= 0.45f ? 1f : Mathf.Clamp01(0.55f - dist) / 0.1f;
                    tex.SetPixel(x, y, new Color(0.2f, 0.25f, 0.35f, alpha));
                }
            }
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            tex.hideFlags = HideFlags.HideAndDontSave;

            s_defaultPickupSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 32f);
            s_defaultPickupSprite.hideFlags = HideFlags.HideAndDontSave;
            return s_defaultPickupSprite;
        }
    }
}
