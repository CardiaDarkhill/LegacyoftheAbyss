#nullable enable

using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal enum ShadeCharmPlacementKind
    {
        GroundAnchor,
        Ground,
        Container,
        ShopListing,
        BossDrop
    }

    internal sealed class ShadeCharmPlacementDefinition
    {
        [JsonPropertyName("charmId")]
        public ShadeCharmId CharmId { get; init; }

        [JsonPropertyName("sceneName")]
        public string? SceneName { get; init; }

        [JsonPropertyName("sceneContainsAll")]
        public string[]? SceneContainsAll { get; init; }

        [JsonPropertyName("sceneExcludesAny")]
        public string[]? SceneExcludesAny { get; init; }

        [JsonPropertyName("placementKind")]
        public ShadeCharmPlacementKind PlacementKind { get; init; }

        [JsonPropertyName("anchorPath")]
        public string? AnchorPath { get; init; }

        [JsonPropertyName("anchorOffset")]
        public ShadeCharmPlacementVector3? AnchorOffset { get; init; }

        [JsonPropertyName("worldPosition")]
        public ShadeCharmPlacementVector3? WorldPosition { get; init; }

        [JsonPropertyName("worldRotationEuler")]
        public ShadeCharmPlacementVector3? WorldRotationEuler { get; init; }

        [JsonPropertyName("playerDataBool")]
        public string? PlayerDataBool { get; init; }

        [JsonPropertyName("flingPickup")]
        public bool? FlingPickup { get; init; }

        [JsonPropertyName("container")]
        public ShadeCharmPlacementContainerData? Container { get; init; }

        [JsonPropertyName("shop")]
        public ShadeCharmPlacementShopData? Shop { get; init; }

        [JsonPropertyName("bossDrop")]
        public ShadeCharmPlacementBossDropData? BossDrop { get; init; }

        [JsonPropertyName("notes")]
        public string? Notes { get; init; }

        internal bool MatchesScene(string? sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(SceneName) &&
                string.Equals(SceneName, sceneName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (SceneContainsAll != null && SceneContainsAll.Length > 0)
            {
                bool containsAll = true;
                foreach (string token in SceneContainsAll)
                {
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        continue;
                    }

                    if (sceneName.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        containsAll = false;
                        break;
                    }
                }

                if (containsAll)
                {
                    if (SceneExcludesAny != null && SceneExcludesAny.Length > 0)
                    {
                        foreach (string exclude in SceneExcludesAny)
                        {
                            if (string.IsNullOrWhiteSpace(exclude))
                            {
                                continue;
                            }

                            if (sceneName.IndexOf(exclude, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class ShadeCharmPlacementContainerData
    {
        [JsonPropertyName("targetPath")]
        public string? TargetPath { get; init; }

        [JsonPropertyName("targetName")]
        public string? TargetName { get; init; }

        [JsonPropertyName("replaceExistingItem")]
        public bool? ReplaceExistingItem { get; init; }
    }

    internal sealed class ShadeCharmPlacementShopData
    {
        [JsonPropertyName("ownerPath")]
        public string? OwnerPath { get; init; }

        [JsonPropertyName("ownerName")]
        public string? OwnerName { get; init; }

        [JsonPropertyName("geoCost")]
        public int? GeoCost { get; init; }

        [JsonPropertyName("delayPurchase")]
        public bool? DelayPurchase { get; init; }

        [JsonPropertyName("closeOnPurchase")]
        public bool? CloseOnPurchase { get; init; }
    }

    internal sealed class ShadeCharmPlacementBossDropData
    {
        [JsonPropertyName("enemyName")]
        public string? EnemyName { get; init; }

        [JsonPropertyName("enemyNameContainsAll")]
        public string[]? EnemyNameContainsAll { get; init; }

        [JsonPropertyName("targetPath")]
        public string? TargetPath { get; init; }

        [JsonPropertyName("spawnPointPath")]
        public string? SpawnPointPath { get; init; }

        [JsonPropertyName("clearExistingDrops")]
        public bool? ClearExistingDrops { get; init; }

        [JsonPropertyName("probability")]
        public float? Probability { get; init; }
    }

    internal sealed class ShadeCharmPlacementVector3
    {
        [JsonPropertyName("x")]
        public float X { get; init; }

        [JsonPropertyName("y")]
        public float Y { get; init; }

        [JsonPropertyName("z")]
        public float Z { get; init; }

        internal Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
