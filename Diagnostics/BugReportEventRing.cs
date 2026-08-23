using System;
using System.Globalization;
using System.Text;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>One discrete thing that happened, as opposed to a periodic state sample.</summary>
    internal struct BugReportEvent
    {
        internal float Realtime;
        internal int Frame;
        internal string? Category;
        internal string? Summary;
        internal string? Detail;

        /// <summary>Times this event repeated identically before something else happened.</summary>
        internal int Repeats;

        /// <summary>Realtime of the last repeat, equal to <see cref="Realtime"/> when there were none.</summary>
        internal float LastRealtime;
    }

    /// <summary>
    /// Rolling window of discrete events, dumped alongside a bug report as <c>events.csv</c>.
    /// <para>
    /// The flight recorder answers "what state was everything in?" on a timer; this answers "what
    /// happened, and to what?". The two are not interchangeable. A report where the Shade was hit by
    /// a boss grab and Hornet was then teleported into the attack showed the effect perfectly - one
    /// flight row had her somewhere else, several units away and a mask down - and named nothing at
    /// all, because cause and effect landed inside a single sampling interval and the object
    /// responsible never appeared in any artefact. Sampling faster would not have fixed that; only
    /// recording the events themselves does.
    /// </para>
    /// <para>
    /// Deliberately independent of the <c>log*</c> config flags. Those decide what is worth writing
    /// to a console during normal play; a filed bug report needs the history regardless, and the
    /// most useful line is routinely the one that was gated off.
    /// </para>
    /// </summary>
    internal sealed class BugReportEventRing
    {
        internal const int MinimumCapacity = 32;
        internal const int MaximumCapacity = 4096;
        internal const int DefaultCapacity = 512;

        /// <summary>Events quoted inline in report.md. The full ring still goes to events.csv.</summary>
        internal const int InlineTailEntries = 40;

        private readonly BugReportEvent[] _events;
        private readonly object _sync = new object();
        private int _next;
        private int _count;

        internal BugReportEventRing(int capacity)
        {
            _events = new BugReportEvent[Math.Min(MaximumCapacity, Math.Max(MinimumCapacity, capacity))];
        }

        internal int Capacity => _events.Length;

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

        /// <summary>
        /// Appends an event, or coalesces it into the previous one when it is identical.
        /// <para>
        /// Several of the emitters sit on per-frame paths - the Shade's damage check runs against
        /// every overlapping hazard every frame, and the game's own hero damage check is called from
        /// <c>OnTriggerStay2D</c>. Left uncoalesced, one boss attack wrote several hundred identical
        /// rows and pushed the whole rest of the window out of the ring: the first report to use this
        /// covered eight seconds of a thirty-second recording and lost the three earlier moments that
        /// were the ones worth correlating. Collapsing a repeat into a count loses nothing - "this
        /// held for 0.8s" is what the repeat actually means - and keeps the window honest.
        /// </para>
        /// </summary>
        internal void Add(string? category, string? summary, string? detail, float realtime, int frame)
        {
            lock (_sync)
            {
                if (_count > 0)
                {
                    int newest = (_next - 1 + _events.Length) % _events.Length;
                    ref var previous = ref _events[newest];
                    if (string.Equals(previous.Category, category, StringComparison.Ordinal) &&
                        string.Equals(previous.Summary, summary, StringComparison.Ordinal) &&
                        string.Equals(previous.Detail, detail, StringComparison.Ordinal))
                    {
                        previous.Repeats++;
                        previous.LastRealtime = realtime;
                        return;
                    }
                }

                _events[_next] = new BugReportEvent
                {
                    Realtime = realtime,
                    Frame = frame,
                    Category = category,
                    Summary = summary,
                    Detail = detail,
                    Repeats = 0,
                    LastRealtime = realtime
                };

                _next = (_next + 1) % _events.Length;
                if (_count < _events.Length)
                {
                    _count++;
                }
            }
        }

        /// <summary>Oldest event first.</summary>
        internal BugReportEvent[] Snapshot()
        {
            lock (_sync)
            {
                var result = new BugReportEvent[_count];
                int start = (_next - _count + _events.Length) % _events.Length;
                for (int i = 0; i < _count; i++)
                {
                    result[i] = _events[(start + i) % _events.Length];
                }

                return result;
            }
        }

        internal void Clear()
        {
            lock (_sync)
            {
                Array.Clear(_events, 0, _events.Length);
                _next = 0;
                _count = 0;
            }
        }

        /// <summary>
        /// CSV, matching flight.csv so the two line up by their shared <c>realtime</c> column.
        /// <paramref name="captureRealtime"/> is the moment the report was filed, so <c>t_rel</c>
        /// reads as "seconds before capture".
        /// </summary>
        internal string ToCsv(float captureRealtime)
        {
            var events = Snapshot();
            var builder = new StringBuilder();
            builder.AppendLine("t_rel,realtime,frame,category,summary,detail,repeats,held_seconds");

            foreach (var entry in events)
            {
                builder
                    .Append(F(entry.Realtime - captureRealtime)).Append(',')
                    .Append(F(entry.Realtime)).Append(',')
                    .Append(entry.Frame.ToString(CultureInfo.InvariantCulture)).Append(',')
                    .Append(Escape(entry.Category)).Append(',')
                    .Append(Escape(entry.Summary)).Append(',')
                    .Append(Escape(entry.Detail)).Append(',')
                    .Append(entry.Repeats.ToString(CultureInfo.InvariantCulture)).Append(',')
                    .Append(F(entry.LastRealtime - entry.Realtime))
                    .AppendLine();
            }

            return builder.ToString();
        }

        /// <summary>The last <paramref name="maxEntries"/> events, for quoting inline in report.md.</summary>
        internal string RenderTail(int maxEntries, float captureRealtime)
        {
            var events = Snapshot();
            int start = Math.Max(0, events.Length - Math.Max(1, maxEntries));
            var builder = new StringBuilder();

            for (int i = start; i < events.Length; i++)
            {
                var entry = events[i];
                builder
                    .Append('[').Append(F(entry.Realtime - captureRealtime)).Append("s] ")
                    .Append(entry.Category)
                    .Append(": ")
                    .Append(entry.Summary);

                if (!string.IsNullOrEmpty(entry.Detail))
                {
                    builder.Append(" - ").Append(entry.Detail);
                }

                if (entry.Repeats > 0)
                {
                    builder
                        .Append(" (x").Append((entry.Repeats + 1).ToString(CultureInfo.InvariantCulture))
                        .Append(" over ").Append(F(entry.LastRealtime - entry.Realtime)).Append("s)");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string F(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (value!.IndexOf(',') < 0 && value.IndexOf('"') < 0 && value.IndexOf('\n') < 0)
            {
                return value;
            }

            return "\"" + value.Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", string.Empty) + "\"";
        }
    }
}
