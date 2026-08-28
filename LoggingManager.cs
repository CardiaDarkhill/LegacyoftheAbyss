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
    private static StreamWriter writer;
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
            string logFile = Path.Combine(ModPaths.Logs, $"shade_damage_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            // Held open for the session. The previous implementation reopened and closed
            // the file for every single line, which meant a full open/flush/close per hit.
            // AutoFlush stays on so the log survives a crash -- flushing is the cheap part.
            writer = new StreamWriter(logFile, append: false) { AutoFlush = true };
            writer.WriteLine($"Shade damage log {DateTime.Now}");
        }
        catch
        {
            writer = null;
        }
    }

    internal static void LogShadeDamage(string source, bool succeeded)
    {
        // Ahead of the logDamage gate on purpose. This is the one line that names what hit the
        // Shade, and a filed bug report needs it whether or not damage logging happened to be on -
        // in practice it never is, so every damage report arrived without the answer in it.
        LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
            "shade-damage",
            succeeded ? "took damage" : "avoided damage",
            source);

        // Gate before the console write, so a disabled setting costs neither the spam nor the
        // message-string allocation.

        if (!ModConfig.Instance.logDamage) return;

        Initialize();

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
        if (writer == null) return;
        try { writer.WriteLine(line); }
        catch { }
    }

    internal static void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ModConfig.Instance.logDamage = !ModConfig.Instance.logDamage;
            if (ModConfig.Instance.logDamage)
            {
                Initialize();
            }
            AppendLine($"[Toggle] Damage logging {(ModConfig.Instance.logDamage ? "enabled" : "disabled")}");
        }
    }

    internal static void Flush()
    {
        if (!initialized || writer == null) return;
        try
        {
            AppendLine("== Totals ==");
            foreach (var kv in damage)
            {
                AppendLine($"{kv.Key}: {kv.Value.success} hits, {kv.Value.blocked} blocks");
            }
            writer.Flush();
            writer.Dispose();
        }
        catch { }
        finally
        {
            writer = null;
        }
    }
}
#nullable restore
