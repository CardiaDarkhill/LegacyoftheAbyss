using System;
using System.Collections.Generic;
using System.IO;
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

public class ModConfig
{
    public bool logDamage = true;
    public bool logGeneral = true;
    public bool logMenu = true;
    public bool logShade = true;
    public bool logHud = true;
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

    private static ModConfig instance;

    public static ModConfig Instance => instance ??= Load();

    public static ModConfig Load()
    {
        try
        {
            if (File.Exists(ModPaths.Config))
            {
                string json = File.ReadAllText(ModPaths.Config);
                instance = UnityEngine.JsonUtility.FromJson<ModConfig>(json) ?? new ModConfig();
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
            Directory.CreateDirectory(Path.GetDirectoryName(ModPaths.Config));
            string json = UnityEngine.JsonUtility.ToJson(Instance, true);
            File.WriteAllText(ModPaths.Config, json);
        }
        catch { }
    }
}
