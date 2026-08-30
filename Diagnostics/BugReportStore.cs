using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>Everything one filed report is made of, before it hits disk.</summary>
    internal sealed class BugReportPayload
    {
        internal BugReportState State = new BugReportState();
        internal string? LogText;

        /// <summary>The excerpt quoted inline in report.md. See <c>BugReportLogRing.RenderTail</c>.</summary>
        internal string? LogTail;
        internal string? FlightCsv;
        internal string? EventCsv;

        /// <summary>The excerpt quoted inline in report.md. See <c>BugReportEventRing.RenderTail</c>.</summary>
        internal string? EventTail;
        internal byte[]? ScreenshotPng;
    }

    /// <summary>Where a written report ended up, for the confirmation toast and the log line.</summary>
    internal readonly struct BugReportWriteResult
    {
        internal BugReportWriteResult(bool success, string reportId, string folder, string? error)
        {
            Success = success;
            ReportId = reportId;
            Folder = folder;
            Error = error;
        }

        internal bool Success { get; }

        internal string ReportId { get; }

        internal string Folder { get; }

        internal string? Error { get; }
    }

    /// <summary>
    /// Turns a captured report into a folder of files, and maintains the index that lists them.
    /// <para>
    /// Reports go under <see cref="ModPaths.UserData"/> - i.e. <c>BepInEx/config/LegacyoftheAbyss/</c>
    /// - for the same reason save data does: a mod manager deletes and re-extracts the whole package
    /// folder on update, and a bug report that vanishes when you update the mod you were reporting a
    /// bug in is worse than useless.
    /// </para>
    /// </summary>
    internal static class BugReportStore
    {
        internal const string IndexFileName = "index.md";
        internal const string ReportFileName = "report.md";
        internal const string StateFileName = "state.json";
        internal const string LogFileName = "log.txt";
        internal const string FlightFileName = "flight.csv";
        internal const string EventFileName = "events.csv";
        internal const string ScreenshotFileName = "screenshot.png";

        /// <summary>Log entries quoted inline in report.md. The full ring still goes to log.txt.</summary>
        internal const int InlineLogTailEntries = 60;

        /// <summary>Ceiling on that excerpt, so one enormous entry cannot crowd out the summary.</summary>
        internal const int InlineLogTailCharacters = 8000;

        private const int MaxSlugLength = 48;

        /// <summary>
        /// No byte-order mark. <see cref="Encoding.UTF8"/> emits one, which puts an invisible U+FEFF at
        /// the head of every file we write - harmless in a text editor, but it becomes part of the first
        /// CSV header cell (so a strict parser sees a column named "﻿t_rel" rather than "t_rel")
        /// and shows up as a stray glyph before the leading "#" of the markdown.
        /// </summary>
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private static readonly JsonSerializerSettings StateJsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None
        };

        internal static string Root => Path.Combine(ModPaths.UserData, "bug_reports");

        /// <summary>
        /// A filesystem- and URL-safe form of the report title. Lowercase, ASCII, single hyphens.
        /// Returns <c>report</c> for input with nothing usable in it, because a nameless folder is
        /// still better than a failed capture.
        /// </summary>
        internal static string Slugify(string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return "report";
            }

            var builder = new StringBuilder(MaxSlugLength);
            bool pendingHyphen = false;
            foreach (char character in title!)
            {
                // Overshoot by one word rather than stopping dead at the cap, so the trim below always
                // has a hyphen to cut back to.
                if (builder.Length > MaxSlugLength)
                {
                    break;
                }

                if ((character >= 'a' && character <= 'z') || (character >= '0' && character <= '9'))
                {
                    if (pendingHyphen && builder.Length > 0)
                    {
                        builder.Append('-');
                    }

                    pendingHyphen = false;
                    builder.Append(character);
                }
                else if (character >= 'A' && character <= 'Z')
                {
                    if (pendingHyphen && builder.Length > 0)
                    {
                        builder.Append('-');
                    }

                    pendingHyphen = false;
                    builder.Append(char.ToLowerInvariant(character));
                }
                else
                {
                    pendingHyphen = true;
                }
            }

            return builder.Length == 0 ? "report" : TrimToWordBoundary(builder.ToString());
        }

        /// <summary>
        /// Cuts an over-long slug back to the last whole word. A hard cut leaves folder names ending in
        /// half a word ("...-bug-report-featur"), which reads as a corrupted name rather than a
        /// shortened one. Falls back to the hard cut when the first word alone is already too long,
        /// since there is no boundary to find.
        /// </summary>
        private static string TrimToWordBoundary(string slug)
        {
            if (slug.Length <= MaxSlugLength)
            {
                return slug.TrimEnd('-');
            }

            int lastHyphen = slug.LastIndexOf('-', MaxSlugLength - 1);
            string trimmed = lastHyphen > 0 ? slug.Substring(0, lastHyphen) : slug.Substring(0, MaxSlugLength);
            return trimmed.TrimEnd('-');
        }

        /// <summary>
        /// Report ids lead with a sortable timestamp so the folder listing is chronological, and carry
        /// the slug so a directory of them is readable without opening any.
        /// </summary>
        internal static string BuildReportId(DateTime localTime, string? title)
        {
            return localTime.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture) + "-" + Slugify(title);
        }

        /// <summary>First line of the message is the title; the rest is the body.</summary>
        internal static void SplitMessage(string? message, out string title, out string body)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                title = "Untitled report";
                body = string.Empty;
                return;
            }

            string normalised = message!.Replace("\r\n", "\n").Replace('\r', '\n');
            int breakIndex = normalised.IndexOf('\n');
            if (breakIndex < 0)
            {
                title = normalised.Trim();
                body = string.Empty;
            }
            else
            {
                title = normalised.Substring(0, breakIndex).Trim();
                body = normalised.Substring(breakIndex + 1).Trim();
            }

            if (title.Length == 0)
            {
                title = "Untitled report";
            }
        }

        internal static BugReportWriteResult Write(BugReportPayload payload)
        {
            if (payload == null)
            {
                return new BugReportWriteResult(false, string.Empty, string.Empty, "No payload.");
            }

            string reportId = payload.State.ReportId ?? BuildReportId(DateTime.Now, payload.State.Title);
            payload.State.ReportId = reportId;
            string folder = Path.Combine(Root, reportId);

            try
            {
                Directory.CreateDirectory(folder);

                bool hasFlight = !string.IsNullOrEmpty(payload.FlightCsv);
                bool hasEvents = !string.IsNullOrEmpty(payload.EventCsv);
                bool hasScreenshot = payload.ScreenshotPng != null && payload.ScreenshotPng.Length > 0;
                bool hasLog = !string.IsNullOrEmpty(payload.LogText);

                File.WriteAllText(
                    Path.Combine(folder, ReportFileName),
                    RenderMarkdown(payload.State, payload.LogTail, payload.EventTail, hasLog, hasFlight, hasEvents, hasScreenshot),
                    Utf8NoBom);

                File.WriteAllText(Path.Combine(folder, StateFileName), SerializeState(payload.State), Utf8NoBom);

                if (hasLog)
                {
                    File.WriteAllText(Path.Combine(folder, LogFileName), payload.LogText, Utf8NoBom);
                }

                if (hasFlight)
                {
                    File.WriteAllText(Path.Combine(folder, FlightFileName), payload.FlightCsv, Utf8NoBom);
                }

                if (hasEvents)
                {
                    File.WriteAllText(Path.Combine(folder, EventFileName), payload.EventCsv, Utf8NoBom);
                }

                if (hasScreenshot)
                {
                    File.WriteAllBytes(Path.Combine(folder, ScreenshotFileName), payload.ScreenshotPng);
                }

                AppendToIndex(payload.State);
                return new BugReportWriteResult(true, reportId, folder, null);
            }
            catch (Exception ex)
            {
                return new BugReportWriteResult(false, reportId, folder, ex.Message);
            }
        }

        internal static string SerializeState(BugReportState state)
        {
            try
            {
                return JsonConvert.SerializeObject(state, StateJsonSettings);
            }
            catch (Exception ex)
            {
                // A serialiser failure must not cost the whole report - the markdown and the log are
                // the parts a human reads first, and they are already written by this point.
                return "{\"serializationError\":\"" + ex.Message.Replace("\"", "'") + "\"}";
            }
        }

        private static void AppendToIndex(BugReportState state)
        {
            try
            {
                string indexPath = Path.Combine(Root, IndexFileName);
                if (!File.Exists(indexPath))
                {
                    File.WriteAllText(
                        indexPath,
                        "# Legacy of the Abyss - bug reports\n\n" +
                        "Newest last. `[ ]` is open, `[x]` is fixed. Each id is a folder next to this file.\n\n",
                        Utf8NoBom);
                }

                File.AppendAllText(indexPath, FormatIndexLine(state) + "\n", Utf8NoBom);
            }
            catch
            {
            }
        }

        internal static string FormatIndexLine(BugReportState state)
        {
            string scene = string.IsNullOrWhiteSpace(state.Scene) ? "?" : state.Scene!;
            string title = string.IsNullOrWhiteSpace(state.Title) ? "Untitled report" : state.Title!;
            string trigger = string.IsNullOrWhiteSpace(state.Trigger) ? "manual" : state.Trigger!;
            return string.Format(
                CultureInfo.InvariantCulture,
                "- [ ] `{0}` - {1} _(scene: {2}, mod {3}, {4})_",
                state.ReportId,
                title.Replace("\n", " "),
                scene,
                string.IsNullOrWhiteSpace(state.ModVersion) ? "?" : state.ModVersion!,
                trigger);
        }

        /// <summary>
        /// The human-and-agent-facing summary. Ordered so the first screenful answers what happened,
        /// where, and to whom - the log tail is quoted last because it is the part you scroll to only
        /// once the summary has told you what to look for.
        /// </summary>
        internal static string RenderMarkdown(BugReportState state, string? logTail, string? eventTail, bool hasLog, bool hasFlight, bool hasEvents, bool hasScreenshot)
        {
            var builder = new StringBuilder();
            builder.Append("# ").AppendLine(string.IsNullOrWhiteSpace(state.Title) ? "Untitled report" : state.Title);
            builder.AppendLine();
            builder.Append("- **Id:** `").Append(state.ReportId).AppendLine("`");
            builder.Append("- **Filed:** ").Append(state.CapturedLocal).Append(" (local) / ").Append(state.CapturedUtc).AppendLine();
            builder.Append("- **Trigger:** ").AppendLine(state.Trigger);
            builder.Append("- **Mod version:** ").AppendLine(string.IsNullOrWhiteSpace(state.ModVersion) ? "?" : state.ModVersion);
            builder.AppendLine("- **Status:** open");
            builder.AppendLine();

            if (!string.IsNullOrWhiteSpace(state.Message))
            {
                builder.AppendLine("## What I saw");
                builder.AppendLine();
                builder.AppendLine(state.Message!.Trim());
                builder.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(state.ExceptionMessage))
            {
                builder.AppendLine("## Exception");
                builder.AppendLine();
                builder.AppendLine("```");
                builder.AppendLine(state.ExceptionMessage!.Trim());
                if (!string.IsNullOrWhiteSpace(state.ExceptionStackTrace))
                {
                    builder.AppendLine(state.ExceptionStackTrace!.TrimEnd());
                }

                builder.AppendLine("```");
                builder.AppendLine();
            }

            builder.AppendLine("## Where");
            builder.AppendLine();
            builder.AppendLine("| Field | Value |");
            builder.AppendLine("| --- | --- |");
            Row(builder, "Scene", state.Scene);
            Row(builder, "Game state", state.GameState);
            Row(builder, "Paused", state.Paused ? "yes" : "no");
            Row(builder, "Time scale", state.TimeScale.ToString("0.###", CultureInfo.InvariantCulture));
            Row(builder, "Realtime", state.Realtime.ToString("0.##", CultureInfo.InvariantCulture) + "s");
            Row(builder, "Game / Unity", state.GameVersion + " / " + state.UnityVersion);
            Row(builder, "Display", state.Resolution + (state.Fullscreen ? " fullscreen" : " windowed"));
            if (state.SceneHistory != null && state.SceneHistory.Length > 0)
            {
                Row(builder, "Recent scenes", string.Join(" -> ", state.SceneHistory));
            }

            if (!string.IsNullOrEmpty(state.MenuSliderTemplate))
            {
                Row(builder, "Menu slider template", state.MenuSliderTemplate);
            }

            if (!string.IsNullOrEmpty(state.CoopCamera))
            {
                Row(builder, "Co-op camera", state.CoopCamera);
            }

            if (!string.IsNullOrEmpty(state.KnightBundle))
            {
                Row(builder, "Knight bundle", state.KnightBundle);
            }

            if (!string.IsNullOrEmpty(state.KnightAudio))
            {
                Row(builder, "Knight audio", state.KnightAudio);
            }

            if (!string.IsNullOrEmpty(state.ShadePaneLayoutFailure))
            {
                Row(builder, "Shade pane layout failure", state.ShadePaneLayoutFailure);
            }

            if (!string.IsNullOrEmpty(state.ShadeCharmGridLayout))
            {
                Row(builder, "Shade charm grid layout", state.ShadeCharmGridLayout);
            }

            if (!string.IsNullOrEmpty(state.HeroMenuBindings))
            {
                Row(builder, "Hero menu bindings", state.HeroMenuBindings);
            }

            if (!string.IsNullOrEmpty(state.InventoryProbe))
            {
                Row(builder, "Inventory probe", state.InventoryProbe);
            }

            builder.AppendLine();

            var hero = state.Hero;
            if (hero != null && hero.Present)
            {
                builder.AppendLine("## Hornet");
                builder.AppendLine();
                builder.AppendLine("| Field | Value |");
                builder.AppendLine("| --- | --- |");
                Row(builder, "Position", Coordinate(hero.X, hero.Y));
                Row(builder, "Actor state", hero.ActorState);
                Row(builder, "Flags", hero.Flags);
                Row(builder, "Health", hero.Health + " / " + hero.MaxHealth);
                Row(builder, "Silk", hero.Silk + " / " + hero.SilkMax);
                Row(builder, "Control relinquished", hero.ControlRelinquished ? "yes" : "no");
                Row(builder, "Save slot", hero.ProfileId.ToString(CultureInfo.InvariantCulture));
                builder.AppendLine();
            }

            var shade = state.Shade;
            builder.AppendLine("## Shade");
            builder.AppendLine();
            if (shade == null || !shade.Present)
            {
                builder.AppendLine("No Shade instance was alive when this was captured.");
                builder.AppendLine();
            }
            else
            {
                builder.AppendLine("| Field | Value |");
                builder.AppendLine("| --- | --- |");
                Row(builder, "Position", Coordinate(shade.X, shade.Y));
                Row(builder, "Velocity", Coordinate(shade.VelocityX, shade.VelocityY));
                Row(builder, "Flags", shade.Flags);
                Row(builder, "HP", shade.Hp + " / " + shade.MaxHp + " (base max " + shade.BaseMaxHp + ")");
                Row(builder, "Lifeblood", shade.Lifeblood + " / " + shade.LifebloodMax);
                Row(builder, "Soul", shade.Soul + " / " + shade.SoulMax);
                Row(builder, "Can take damage", shade.CanTakeDamage ? "yes" : "no");
                Row(builder, "Assist mode", shade.AssistMode ? "on" : "off");
                // "The Shade did nothing" is the shape of most Shade reports, and whether an AI was
                // driving it at the time is the first thing that changes what to look at next.
                Row(builder, "AI order", string.IsNullOrEmpty(shade.AiCommandState) || shade.AiCommandState == "Inactive"
                    ? "none"
                    : shade.AiCommandState + " " + Coordinate(shade.AiCommandX, shade.AiCommandY));
                Row(builder, "AI", shade.AiEnabled
                    ? shade.AiAction + " (" + shade.AiReason + "), target " + shade.AiTargetId + ", " + shade.AiTargetsInRange + " enemies in range"
                    : "off");
                Row(builder, "Skin", shade.Skin);
                Row(builder, "Notches", shade.NotchesUsed + " / " + shade.NotchCapacity);
                Row(builder, "Charms", shade.EquippedCharms == null || shade.EquippedCharms.Length == 0
                    ? "none"
                    : string.Join(", ", shade.EquippedCharms));
                builder.AppendLine();
            }

            builder.AppendLine("## Files");
            builder.AppendLine();
            builder.Append("- `").Append(StateFileName).AppendLine("` - full state snapshot, including the mod config and every loaded plugin.");
            if (hasLog)
            {
                builder.Append("- `").Append(LogFileName).AppendLine("` - the whole captured log ring, all sources.");
            }

            if (hasFlight)
            {
                builder.Append("- `").Append(FlightFileName).AppendLine("` - rolling state samples leading up to the capture. `t_rel` is seconds relative to it, so the last rows are the moment reported.");
            }

            if (hasEvents)
            {
                builder.Append("- `").Append(EventFileName).AppendLine("` - discrete events leading up to the capture: hero repositions, what the Shade's aggro proxy entered, every damage decision. Shares the `realtime` column with `flight.csv`, so the two line up row for row.");
            }

            if (hasScreenshot)
            {
                builder.Append("- `").Append(ScreenshotFileName).AppendLine("` - the frame as it looked when the hotkey was pressed, before the report overlay drew.");
            }

            builder.AppendLine();

            // Ahead of the log tail on purpose: these lines are the ones chosen for being about the
            // Shade, so they are far likelier to name the cause than the last 60 lines of whatever
            // every loaded plugin happened to be saying.
            if (!string.IsNullOrEmpty(eventTail))
            {
                builder.Append("## Event tail (last ").Append(BugReportEventRing.InlineTailEntries.ToString(CultureInfo.InvariantCulture)).AppendLine(" events)");
                builder.AppendLine();
                builder.AppendLine("```");
                builder.AppendLine(eventTail!.TrimEnd('\n', '\r'));
                builder.AppendLine("```");
                builder.AppendLine();
            }

            if (!string.IsNullOrEmpty(logTail))
            {
                builder.Append("## Log tail (last ").Append(InlineLogTailEntries.ToString(CultureInfo.InvariantCulture)).AppendLine(" entries)");
                builder.AppendLine();
                builder.AppendLine("```");
                builder.AppendLine(logTail!.TrimEnd('\n', '\r'));
                builder.AppendLine("```");
            }

            return builder.ToString();
        }

        private static string Coordinate(float x, float y)
        {
            return "X=" + x.ToString("0.###", CultureInfo.InvariantCulture) + ", Y=" + y.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static void Row(StringBuilder builder, string label, string? value)
        {
            builder.Append("| ").Append(label).Append(" | ")
                .Append(string.IsNullOrWhiteSpace(value) ? "-" : value!.Replace("|", "\\|"))
                .AppendLine(" |");
        }
    }
}
