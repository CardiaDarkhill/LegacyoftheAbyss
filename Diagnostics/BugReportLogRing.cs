using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BepInEx.Logging;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>
    /// One captured log line, flattened at capture time.
    /// <para>
    /// Everything here is a value or an already-materialised string on purpose. <see cref="LogEventArgs"/>
    /// hands out a <c>Data</c> object whose <c>ToString</c> is frequently a closure over live game
    /// state, so holding the event itself would both keep that state alive for the length of the ring
    /// and render a *later* value than the one that was logged.
    /// </para>
    /// </summary>
    internal readonly struct BugReportLogEntry
    {
        internal BugReportLogEntry(DateTime timestampUtc, float realtime, string level, string source, string message)
        {
            TimestampUtc = timestampUtc;
            Realtime = realtime;
            Level = string.IsNullOrEmpty(level) ? "Info" : level;
            Source = string.IsNullOrEmpty(source) ? "?" : source;
            Message = message ?? string.Empty;
        }

        internal DateTime TimestampUtc { get; }

        /// <summary>Seconds since game start, so a line can be lined up against a flight-recorder row.</summary>
        internal float Realtime { get; }

        internal string Level { get; }

        internal string Source { get; }

        internal string Message { get; }

        internal string Format()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "[{0:HH:mm:ss.fff}] [t={1,8:F2}] [{2,-7}] [{3}] {4}",
                TimestampUtc,
                Realtime,
                Level,
                Source,
                Message);
        }
    }

    /// <summary>
    /// Fixed-capacity, oldest-evicting buffer of recent log lines.
    /// <para>
    /// The point of the cap is that this runs for an entire play session: an unbounded list would grow
    /// with every log line the game and every other plugin emits, and a bug report only ever wants the
    /// tail anyway. Writes are locked because Unity's threaded log callback and BepInEx's listener
    /// chain do not both run on the main thread.
    /// </para>
    /// </summary>
    internal sealed class BugReportLogRing
    {
        internal const int MinimumCapacity = 16;
        internal const int MaximumCapacity = 20000;

        private readonly BugReportLogEntry[] _entries;
        private readonly object _sync = new object();
        private int _next;
        private int _count;
        private long _dropped;

        internal BugReportLogRing(int capacity)
        {
            _entries = new BugReportLogEntry[ClampCapacity(capacity)];
        }

        internal static int ClampCapacity(int capacity)
        {
            if (capacity < MinimumCapacity)
            {
                return MinimumCapacity;
            }

            return capacity > MaximumCapacity ? MaximumCapacity : capacity;
        }

        internal int Capacity => _entries.Length;

        internal int Count
        {
            get
            {
                lock (_sync)
                {
                    return _count;
                }
            }
        }

        /// <summary>How many lines fell out of the back of the ring, i.e. how much older context is gone.</summary>
        internal long DroppedCount
        {
            get
            {
                lock (_sync)
                {
                    return _dropped;
                }
            }
        }

        internal void Add(in BugReportLogEntry entry)
        {
            lock (_sync)
            {
                if (_count == _entries.Length)
                {
                    _dropped++;
                }

                _entries[_next] = entry;
                _next = (_next + 1) % _entries.Length;
                if (_count < _entries.Length)
                {
                    _count++;
                }
            }
        }

        /// <summary>Oldest entry first.</summary>
        internal BugReportLogEntry[] Snapshot()
        {
            lock (_sync)
            {
                var result = new BugReportLogEntry[_count];
                int start = (_next - _count + _entries.Length) % _entries.Length;
                for (int i = 0; i < _count; i++)
                {
                    result[i] = _entries[(start + i) % _entries.Length];
                }

                return result;
            }
        }

        internal string Render()
        {
            var entries = Snapshot();
            var builder = new StringBuilder();
            long dropped = DroppedCount;
            if (dropped > 0)
            {
                builder.Append("... ").Append(dropped.ToString(CultureInfo.InvariantCulture))
                    .AppendLine(" earlier line(s) dropped from the ring buffer.");
            }

            foreach (var entry in entries)
            {
                builder.AppendLine(entry.Format());
            }

            return builder.ToString();
        }

        /// <summary>
        /// The most recent <paramref name="entryCount"/> entries, for quoting inline in a report.
        /// <para>
        /// Counted in entries rather than in rendered lines, which is not the same thing and matters
        /// more than it sounds: a single Unity exception entry carries its whole stack trace as
        /// embedded newlines, so a line-counted tail of eighty happily spent all eighty on the frames
        /// of one exception and showed nothing that led up to it.
        /// </para>
        /// <para>
        /// <paramref name="maxCharacters"/> is the backstop for the same failure in the other
        /// direction - sixty entries is a small number until one of them is a megabyte of serialised
        /// state. The oldest entries are dropped first, so what survives is always the newest.
        /// </para>
        /// </summary>
        internal string RenderTail(int entryCount, int maxCharacters)
        {
            if (entryCount <= 0 || maxCharacters <= 0)
            {
                return string.Empty;
            }

            var entries = Snapshot();
            int start = entries.Length > entryCount ? entries.Length - entryCount : 0;

            var formatted = new List<string>(entries.Length - start);
            int total = 0;
            for (int i = entries.Length - 1; i >= start; i--)
            {
                string line = entries[i].Format();
                if (formatted.Count > 0 && total + line.Length > maxCharacters)
                {
                    break;
                }

                formatted.Add(line);
                total += line.Length + 1;
            }

            formatted.Reverse();

            var builder = new StringBuilder(total);
            int omitted = entries.Length - formatted.Count;
            if (omitted > 0)
            {
                builder.Append("... ").Append(omitted.ToString(CultureInfo.InvariantCulture))
                    .AppendLine(" earlier entries not shown here; see the log file for all of them.");
            }

            foreach (string line in formatted)
            {
                builder.AppendLine(line);
            }

            return builder.ToString();
        }

        internal void Clear()
        {
            lock (_sync)
            {
                Array.Clear(_entries, 0, _entries.Length);
                _next = 0;
                _count = 0;
                _dropped = 0;
            }
        }
    }
}
