#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LegacyoftheAbyss.Shade
{
    internal static class ShadeCharmPlacementDatabase
    {
        private static readonly JsonSerializerOptions JsonOptions;
        private static readonly object ReloadLock = new();

        private static ShadeCharmPlacementDefinition[] s_allPlacements = Array.Empty<ShadeCharmPlacementDefinition>();
        private static Dictionary<string, ShadeCharmPlacementDefinition[]> s_exactPlacements = new(StringComparer.OrdinalIgnoreCase);
        private static ShadeCharmPlacementDefinition[] s_containsPlacements = Array.Empty<ShadeCharmPlacementDefinition>();
        private static string s_sourcePath = string.Empty;
        private static bool s_initialized;

        static ShadeCharmPlacementDatabase()
        {
            JsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            JsonOptions.Converters.Add(new JsonStringEnumIgnoreCaseConverter<ShadeCharmPlacementKind>());
            JsonOptions.Converters.Add(new JsonStringEnumIgnoreCaseConverter<ShadeCharmId>());
        }

        internal static void EnsureLoaded()
        {
            if (s_initialized)
            {
                return;
            }

            Reload();
        }

        internal static void Reload(string? overridePath = null)
        {
            lock (ReloadLock)
            {
                string path = overridePath ?? ModPaths.GetAssetPath("charm_placements.json");
                List<ShadeCharmPlacementDefinition> placements;
                try
                {
                    if (!File.Exists(path))
                    {
                        ShadeCharmPlacementService.LogWarning($"Charm placement data not found at '{path}'.");
                        placements = new List<ShadeCharmPlacementDefinition>();
                    }
                    else
                    {
                        string json = File.ReadAllText(path);
                        if (string.IsNullOrWhiteSpace(json))
                        {
                            placements = new List<ShadeCharmPlacementDefinition>();
                        }
                        else
                        {
                            var file = JsonSerializer.Deserialize<ShadeCharmPlacementFile>(json, JsonOptions);
                            placements = file?.Placements?.Where(p => p != null).ToList() ?? new List<ShadeCharmPlacementDefinition>();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShadeCharmPlacementService.LogWarning($"Failed to load charm placement data: {ex}");
                    placements = new List<ShadeCharmPlacementDefinition>();
                }

                var exact = new Dictionary<string, List<ShadeCharmPlacementDefinition>>(StringComparer.OrdinalIgnoreCase);
                var contains = new List<ShadeCharmPlacementDefinition>();
                foreach (var placement in placements)
                {
                    if (placement == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(placement.SceneName))
                    {
                        if (!exact.TryGetValue(placement.SceneName, out var list))
                        {
                            list = new List<ShadeCharmPlacementDefinition>();
                            exact[placement.SceneName] = list;
                        }

                        list.Add(placement);
                    }

                    if ((placement.SceneContainsAll != null && placement.SceneContainsAll.Length > 0) || string.IsNullOrWhiteSpace(placement.SceneName))
                    {
                        contains.Add(placement);
                    }
                }

                s_allPlacements = placements.ToArray();
                s_exactPlacements = exact.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray(), StringComparer.OrdinalIgnoreCase);
                s_containsPlacements = contains.ToArray();
                s_sourcePath = path;
                s_initialized = true;
            }
        }

        internal static IReadOnlyList<ShadeCharmPlacementDefinition> GetPlacementsForScene(string? sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return Array.Empty<ShadeCharmPlacementDefinition>();
            }

            EnsureLoaded();

            var exact = s_exactPlacements;
            var contains = s_containsPlacements;
            List<ShadeCharmPlacementDefinition>? combined = null;

            if (exact.TryGetValue(sceneName, out var exactMatches) && exactMatches.Length > 0)
            {
                combined = new List<ShadeCharmPlacementDefinition>(exactMatches);
            }

            if (contains.Length > 0)
            {
                foreach (var placement in contains)
                {
                    try
                    {
                        if (placement.MatchesScene(sceneName))
                        {
                            combined ??= new List<ShadeCharmPlacementDefinition>();
                            combined.Add(placement);
                        }
                    }
                    catch
                    {
                        // Ignore malformed entries
                    }
                }
            }

            return combined ?? (IReadOnlyList<ShadeCharmPlacementDefinition>)Array.Empty<ShadeCharmPlacementDefinition>();
        }

        internal static IReadOnlyList<ShadeCharmPlacementDefinition> GetAllPlacements()
        {
            EnsureLoaded();
            return s_allPlacements;
        }

        internal static string GetSourcePath()
        {
            EnsureLoaded();
            return s_sourcePath;
        }

        private sealed class ShadeCharmPlacementFile
        {
            [JsonPropertyName("placements")]
            public List<ShadeCharmPlacementDefinition>? Placements { get; init; }
        }

        private sealed class JsonStringEnumIgnoreCaseConverter<T> : JsonConverter<T> where T : struct, Enum
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string? value = reader.GetString();
                    if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, true, out T result))
                    {
                        return result;
                    }
                }

                throw new JsonException($"Unable to convert value to {typeof(T).Name}.");
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
