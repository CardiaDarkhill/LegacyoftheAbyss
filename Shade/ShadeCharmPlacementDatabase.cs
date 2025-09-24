#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LegacyoftheAbyss.Shade
{
    internal static class ShadeCharmPlacementDatabase
    {
        private static readonly JsonSerializerSettings JsonSettings;
        private static readonly object ReloadLock = new();

        private static ShadeCharmPlacementDefinition[] s_allPlacements = Array.Empty<ShadeCharmPlacementDefinition>();
        private static Dictionary<string, ShadeCharmPlacementDefinition[]> s_exactPlacements = new(StringComparer.OrdinalIgnoreCase);
        private static ShadeCharmPlacementDefinition[] s_containsPlacements = Array.Empty<ShadeCharmPlacementDefinition>();
        private static string s_sourcePath = string.Empty;
        private static bool s_initialized;

        static ShadeCharmPlacementDatabase()
        {
            JsonSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            JsonSettings.Converters.Add(new StringEnumIgnoreCaseConverter<ShadeCharmPlacementKind>());
            JsonSettings.Converters.Add(new StringEnumIgnoreCaseConverter<ShadeCharmId>());
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
                            var file = Deserialize(json);
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

        private static ShadeCharmPlacementFile? Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                var serializer = JsonSerializer.Create(JsonSettings);
                using var reader = new JsonTextReader(new StringReader(json))
                {
                    DateParseHandling = DateParseHandling.None,
                    FloatParseHandling = FloatParseHandling.Double,
                    CloseInput = true
                };
                reader.CommentHandling = CommentHandling.Ignore;
                var file = serializer.Deserialize<ShadeCharmPlacementFile>(reader);
                return file;
            }
            catch (JsonException)
            {
                var token = JToken.Parse(json, new JsonLoadSettings
                {
                    CommentHandling = CommentHandling.Ignore
                });
                return token.ToObject<ShadeCharmPlacementFile>(JsonSerializer.Create(JsonSettings));
            }
        }

        private sealed class ShadeCharmPlacementFile
        {
            [JsonProperty("placements")]
            public List<ShadeCharmPlacementDefinition>? Placements { get; init; }
        }

        private sealed class StringEnumIgnoreCaseConverter<TEnum> : JsonConverter where TEnum : struct, Enum
        {
            public override bool CanConvert(Type objectType)
            {
                Type targetType = Nullable.GetUnderlyingType(objectType) ?? objectType;
                return targetType == typeof(TEnum);
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                {
                    return Nullable.GetUnderlyingType(objectType) != null ? null : default(TEnum);
                }

                if (reader.TokenType == JsonToken.String)
                {
                    string? value = reader.Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, true, out TEnum result))
                    {
                        return result;
                    }
                }

                throw new JsonSerializationException($"Unable to convert value to {typeof(TEnum).Name}.");
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                if (value is TEnum enumValue)
                {
                    writer.WriteValue(enumValue.ToString());
                }
                else
                {
                    writer.WriteNull();
                }
            }
        }
    }
}
