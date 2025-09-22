using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

internal static class ModPaths
{
    internal static readonly string Root = Path.GetDirectoryName(typeof(ModPaths).Assembly.Location) ?? Directory.GetCurrentDirectory();
    internal static readonly string Assets = Path.Combine(Root, "Assets");
    internal static readonly string Logs = Path.Combine(Assets, "logs");
    internal static readonly string Config = Path.Combine(Assets, "config.json");
    private static readonly string CleanupRoot = Path.GetFullPath(Path.Combine(Root, "..", "LegacyCleanup"));

    private static IEnumerable<string> GetAssetSearchRoots()
    {
        var order = new[]
        {
            Assets,
            Path.Combine(Root, "..", "Assets"),
            Path.Combine(CleanupRoot, "LegacyoftheAbyss", "Assets"),
            Path.Combine(CleanupRoot, "Assets"),
            Path.Combine(CleanupRoot, "LegacyoftheAbyss"),
            CleanupRoot
        };

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var candidate in order)
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                continue;
            }

            string full = Path.GetFullPath(candidate);
            if (seen.Add(full))
            {
                yield return full;
            }
        }
    }

    internal static bool TryGetAssetPath(out string fullPath, params string[] parts)
    {
        fullPath = string.Empty;
        if (parts == null || parts.Length == 0)
        {
            return false;
        }

        string relative = Path.Combine(parts);
        foreach (var root in GetAssetSearchRoots())
        {
            string candidate = Path.Combine(root, relative);
            if (File.Exists(candidate))
            {
                fullPath = candidate;
                return true;
            }
        }

        fullPath = Path.Combine(Assets, relative);
        return File.Exists(fullPath);
    }

    internal static string GetAssetPath(params string[] parts)
    {
        return TryGetAssetPath(out var resolved, parts)
            ? resolved
            : Path.Combine(Assets, Path.Combine(parts));
    }

    internal static string GetAssetDirectory(params string[] parts)
    {
        string relative = Path.Combine(parts);
        foreach (var root in GetAssetSearchRoots())
        {
            string candidate = Path.Combine(root, relative);
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        return Path.Combine(Assets, relative);
    }
}

[Serializable]
public class ModConfig
{
    public bool logDamage = true;
    public bool logGeneral = true;
    public bool logMenu = true;
    public bool logShade = true;
    public bool logHud = true;
    public bool shadeUnlockPopupsMuted = false;
    public float shadeUnlockPopupDelaySeconds = 0.75f;
    public float shadeUnlockPopupDurationSeconds = 3.5f;
    public bool shadeEnabled = true;
    public bool hornetKeyboardEnabled = false;
    public bool hornetControllerEnabled = true;
    public float hornetDamageMultiplier = 1f;
    public float shadeDamageMultiplier = 1f;
    public int bindHornetHeal = 3;
    public int bindShadeHeal = 2;
    public int focusHornetHeal = 1;
    public int focusShadeHeal = 1;
    public ShadeInputConfig shadeInput = ShadeInputConfig.CreateDefault();

    private static ModConfig? instance;
    private static readonly JsonSerializerSettings FallbackJsonSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.None,
        NullValueHandling = NullValueHandling.Ignore
    };

    public static ModConfig Instance => instance ??= Load();

    public static ModConfig Load()
    {
        try
        {
            if (File.Exists(ModPaths.Config))
            {
                string json = File.ReadAllText(ModPaths.Config);
                instance = Deserialize(json) ?? new ModConfig();
            }
            else
            {
                instance = new ModConfig();
                Save();
            }

            instance.shadeInput ??= ShadeInputConfig.CreateDefault();
        }
        catch
        {
            instance = new ModConfig();
        }

        return instance;
    }

    public static void Save()
    {
        try
        {
            var directory = Path.GetDirectoryName(ModPaths.Config);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (LegacyHelper.InputDeviceBlocker.CreateSaveScope())
            {
                string json = Serialize(Instance);
                File.WriteAllText(ModPaths.Config, json);
            }
        }
        catch
        {
        }
    }

    private static string Serialize(ModConfig config)
    {
        try
        {
            string json = JsonConvert.SerializeObject(config, FallbackJsonSettings);
            if (!string.IsNullOrWhiteSpace(json))
            {
                return json;
            }
        }
        catch
        {
        }

        if (TrySerializeWithUnity(config, out var unityJson))
        {
            return unityJson;
        }

        return string.Empty;
    }

    private static bool TrySerializeWithUnity(ModConfig config, out string json)
    {
        try
        {
            json = JsonUtility.ToJson(config, true);
            if (!string.IsNullOrWhiteSpace(json))
            {
                return true;
            }
        }
        catch
        {
        }

        json = string.Empty;
        return false;
    }

    private static ModConfig? Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var config = JsonConvert.DeserializeObject<ModConfig>(json, FallbackJsonSettings);
            if (config != null)
            {
                return config;
            }
        }
        catch
        {
        }

        if (TryDeserializeWithUnity(json, out var unityConfig))
        {
            return unityConfig;
        }

        return null;
    }

    private static bool TryDeserializeWithUnity(string json, out ModConfig? config)
    {
        try
        {
            config = JsonUtility.FromJson<ModConfig>(json);
            if (config != null)
            {
                return true;
            }
        }
        catch
        {
        }

        config = null;
        return false;
    }
}
