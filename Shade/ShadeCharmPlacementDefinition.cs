#nullable enable

using System;
using Newtonsoft.Json;
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
        [JsonProperty("charmId")]
        public ShadeCharmId CharmId { get; init; }

        [JsonProperty("sceneName")]
        public string? SceneName { get; init; }

        [JsonProperty("sceneContainsAll")]
        public string[]? SceneContainsAll { get; init; }

        [JsonProperty("sceneExcludesAny")]
        public string[]? SceneExcludesAny { get; init; }

        [JsonProperty("placementKind")]
        public ShadeCharmPlacementKind PlacementKind { get; init; }

        [JsonProperty("anchorPath")]
        public string? AnchorPath { get; init; }

        [JsonProperty("anchorOffset")]
        public ShadeCharmPlacementVector3? AnchorOffset { get; init; }

        [JsonProperty("worldPosition")]
        public ShadeCharmPlacementVector3? WorldPosition { get; init; }

        [JsonProperty("worldRotationEuler")]
        public ShadeCharmPlacementVector3? WorldRotationEuler { get; init; }

        [JsonProperty("playerDataBool")]
        public string? PlayerDataBool { get; init; }

        [JsonProperty("flingPickup")]
        public bool? FlingPickup { get; init; }

        [JsonProperty("container")]
        public ShadeCharmPlacementContainerData? Container { get; init; }

        [JsonProperty("shop")]
        public ShadeCharmPlacementShopData? Shop { get; init; }

        [JsonProperty("bossDrop")]
        public ShadeCharmPlacementBossDropData? BossDrop { get; init; }

        [JsonProperty("notes")]
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
        [JsonProperty("targetPath")]
        public string? TargetPath { get; init; }

        [JsonProperty("targetName")]
        public string? TargetName { get; init; }

        [JsonProperty("replaceExistingItem")]
        public bool? ReplaceExistingItem { get; init; }
    }

    internal sealed class ShadeCharmPlacementShopData
    {
        [JsonProperty("ownerPath")]
        public string? OwnerPath { get; init; }

        [JsonProperty("ownerName")]
        public string? OwnerName { get; init; }

        [JsonProperty("geoCost")]
        public int? GeoCost { get; init; }

        [JsonProperty("delayPurchase")]
        public bool? DelayPurchase { get; init; }

        [JsonProperty("closeOnPurchase")]
        public bool? CloseOnPurchase { get; init; }
    }

    internal sealed class ShadeCharmPlacementBossDropData
    {
        [JsonProperty("enemyName")]
        public string? EnemyName { get; init; }

        [JsonProperty("enemyNameContainsAll")]
        public string[]? EnemyNameContainsAll { get; init; }

        [JsonProperty("targetPath")]
        public string? TargetPath { get; init; }

        [JsonProperty("spawnPointPath")]
        public string? SpawnPointPath { get; init; }

        [JsonProperty("clearExistingDrops")]
        public bool? ClearExistingDrops { get; init; }

        [JsonProperty("probability")]
        public float? Probability { get; init; }
    }

    internal sealed class ShadeCharmPlacementVector3
    {
        [JsonProperty("x")]
        public float X { get; init; }

        [JsonProperty("y")]
        public float Y { get; init; }

        [JsonProperty("z")]
        public float Z { get; init; }

        internal Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
