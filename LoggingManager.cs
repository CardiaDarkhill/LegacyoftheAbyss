using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal static class LoggingManager
{
    private class DamageEntry
    {
        public int success;
        public int blocked;
    }

    private static readonly Dictionary<string, DamageEntry> damage = new();
    private static string logFile;
    private static bool wroteHitHeader;
    private static bool wroteBlockedHeader;
    private static bool initialized;

    internal static void Initialize()
    {
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
        if (!ModConfig.Instance.logDamage) return;
        Initialize();
        if (!damage.TryGetValue(source, out var entry))
        {
            entry = new DamageEntry();
            damage[source] = entry;
            AppendHeader(succeeded);
            AppendLine($"- {source}");
        }
        if (succeeded) entry.success++; else entry.blocked++;
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
