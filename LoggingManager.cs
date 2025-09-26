#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using UnityEngine;

internal static class LoggingManager
{
    private class DamageEntry
    {
        public int success;
        public int blocked;
    }

    private static readonly Dictionary<string, DamageEntry> damage = new();
    private static ManualLogSource consoleLogger;
    private static string logFile;
    private static bool wroteHitHeader;
    private static bool wroteBlockedHeader;
    private static bool initialized;

    internal static void Initialize(ManualLogSource logger = null)
    {
        if (logger != null)
        {
            consoleLogger = logger;
        }
        if (initialized) return;
        initialized = true;
        try
        {
            Directory.CreateDirectory(ModPaths.Logs);
            logFile = Path.Combine(ModPaths.Logs, $"shade_damage_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            File.WriteAllText(logFile, $"Shade damage log {DateTime.Now}\n");
        }
        catch { }
    }

    internal static void LogShadeDamage(string source, bool succeeded)
    {
        try
        {
            if (consoleLogger != null)
            {
                string message = succeeded
                    ? $"Shade took damage from {source}."
                    : $"Shade avoided damage from {source}.";
                if (succeeded)
                {
                    consoleLogger.LogWarning(message);
                }
                else
                {
                    consoleLogger.LogInfo(message);
                }
            }
        }
        catch
        {
        }

        if (!ModConfig.Instance.logDamage) return;
        Initialize();
        if (!damage.TryGetValue(source, out var entry))
        {
            entry = new DamageEntry();
            damage[source] = entry;
        }
        if (succeeded)
        {
            if (entry.success == 0)
            {
                AppendHeader(true);
                AppendLine($"- {source}");
            }
            entry.success++;
        }
        else
        {
            if (entry.blocked == 0)
            {
                AppendHeader(false);
                AppendLine($"- {source}");
            }
            entry.blocked++;
        }
    }

    private static void AppendHeader(bool succeeded)
    {
        if (succeeded && !wroteHitHeader)
        {
            AppendLine("== Damage sources ==");
            wroteHitHeader = true;
        }
        if (!succeeded && !wroteBlockedHeader)
        {
            AppendLine("== Blocked sources ==");
            wroteBlockedHeader = true;
        }
    }

    private static void AppendLine(string line)
    {
        try { File.AppendAllText(logFile, line + Environment.NewLine); } catch { }
    }

    internal static void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ModConfig.Instance.logDamage = !ModConfig.Instance.logDamage;
            AppendLine($"[Toggle] Damage logging {(ModConfig.Instance.logDamage ? "enabled" : "disabled")}");
        }
    }

    internal static void Flush()
    {
        if (!initialized) return;
        AppendLine("== Totals ==");
        foreach (var kv in damage)
        {
            AppendLine($"{kv.Key}: {kv.Value.success} hits, {kv.Value.blocked} blocks");
        }
    }
}
#nullable restore
