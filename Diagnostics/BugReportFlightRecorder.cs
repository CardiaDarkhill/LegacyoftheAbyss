using System;
using System.Globalization;
using System.Text;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>One periodic sample of the state a Shade bug is usually explained by.</summary>
    internal struct BugReportFlightSample
    {
        internal float Realtime;
        internal string? Scene;
        internal float TimeScale;
        internal float FrameMs;

        internal float HeroX;
        internal float HeroY;
        internal string? HeroState;
        internal string? HeroFlags;
        internal int HeroHp;
        internal int HeroMaxHp;
        internal int Silk;

        internal bool ShadePresent;
        internal float ShadeX;
        internal float ShadeY;
        internal float ShadeVelX;
        internal float ShadeVelY;
        internal int ShadeHp;
        internal int ShadeMaxHp;
        internal int ShadeSoul;
        internal string? ShadeFlags;
    }

    /// <summary>
    /// Rolling window of recent samples, dumped alongside a bug report.
    /// <para>
    /// This exists because of the gap between a bug happening and a human reacting to it. By the time
    /// you have registered "the Shade just teleported into the floor" and reached for the hotkey, a
    /// point-in-time snapshot describes the aftermath, not the cause. A few seconds of history at a
    /// steady interval turns "it ended up here" into "it was doing this, then this".
    /// </para>
    /// <para>
    /// Sized in seconds rather than samples so the window means the same thing regardless of the
    /// configured interval.
    /// </para>
    /// </summary>
    internal sealed class BugReportFlightRecorder
    {
        internal const float MinimumIntervalSeconds = 0.02f;
        internal const float MinimumWindowSeconds = 1f;
        internal const float MaximumWindowSeconds = 300f;

        private readonly BugReportFlightSample[] _samples;
        private readonly object _sync = new object();
        private int _next;
        private int _count;

        internal BugReportFlightRecorder(float windowSeconds, float intervalSeconds)
        {
            IntervalSeconds = Math.Max(MinimumIntervalSeconds, intervalSeconds);
            WindowSeconds = Math.Min(MaximumWindowSeconds, Math.Max(MinimumWindowSeconds, windowSeconds));
            _samples = new BugReportFlightSample[CapacityFor(WindowSeconds, IntervalSeconds)];
        }

        internal float IntervalSeconds { get; }

        internal float WindowSeconds { get; }

        internal int Capacity => _samples.Length;

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

        internal static int CapacityFor(float windowSeconds, float intervalSeconds)
        {
            float interval = Math.Max(MinimumIntervalSeconds, intervalSeconds);
            float window = Math.Min(MaximumWindowSeconds, Math.Max(MinimumWindowSeconds, windowSeconds));
            int capacity = (int)Math.Ceiling(window / interval);
            return capacity < 1 ? 1 : capacity;
        }

        internal void Add(in BugReportFlightSample sample)
        {
            lock (_sync)
            {
                _samples[_next] = sample;
                _next = (_next + 1) % _samples.Length;
                if (_count < _samples.Length)
                {
                    _count++;
                }
            }
        }

        /// <summary>Oldest sample first.</summary>
        internal BugReportFlightSample[] Snapshot()
        {
            lock (_sync)
            {
                var result = new BugReportFlightSample[_count];
                int start = (_next - _count + _samples.Length) % _samples.Length;
                for (int i = 0; i < _count; i++)
                {
                    result[i] = _samples[(start + i) % _samples.Length];
                }

                return result;
            }
        }

        internal void Clear()
        {
            lock (_sync)
            {
                Array.Clear(_samples, 0, _samples.Length);
                _next = 0;
                _count = 0;
            }
        }

        /// <summary>
        /// CSV rather than JSON: this is the one artefact in a report that is a few hundred rows of the
        /// same shape, and a table stays skimmable by eye and greppable by column.
        /// <paramref name="captureRealtime"/> is the moment the report was filed, so the leading column
        /// reads as "seconds before capture" - negative, counting up to roughly zero.
        /// </summary>
        internal string ToCsv(float captureRealtime)
        {
            var samples = Snapshot();
            var builder = new StringBuilder();
            builder.AppendLine("t_rel,realtime,scene,timescale,frame_ms,hero_x,hero_y,hero_state,hero_flags,hero_hp,hero_max_hp,silk,shade_present,shade_x,shade_y,shade_vel_x,shade_vel_y,shade_hp,shade_max_hp,shade_soul,shade_flags");

            foreach (var sample in samples)
            {
                builder
                    .Append(F(sample.Realtime - captureRealtime)).Append(',')
                    .Append(F(sample.Realtime)).Append(',')
                    .Append(Escape(sample.Scene)).Append(',')
                    .Append(F(sample.TimeScale)).Append(',')
                    .Append(F(sample.FrameMs)).Append(',')
                    .Append(F(sample.HeroX)).Append(',')
                    .Append(F(sample.HeroY)).Append(',')
                    .Append(Escape(sample.HeroState)).Append(',')
                    .Append(Escape(sample.HeroFlags)).Append(',')
                    .Append(I(sample.HeroHp)).Append(',')
                    .Append(I(sample.HeroMaxHp)).Append(',')
                    .Append(I(sample.Silk)).Append(',')
                    .Append(sample.ShadePresent ? "1" : "0").Append(',')
                    .Append(F(sample.ShadeX)).Append(',')
                    .Append(F(sample.ShadeY)).Append(',')
                    .Append(F(sample.ShadeVelX)).Append(',')
                    .Append(F(sample.ShadeVelY)).Append(',')
                    .Append(I(sample.ShadeHp)).Append(',')
                    .Append(I(sample.ShadeMaxHp)).Append(',')
                    .Append(I(sample.ShadeSoul)).Append(',')
                    .Append(Escape(sample.ShadeFlags))
                    .AppendLine();
            }

            return builder.ToString();
        }

        private static string F(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static string I(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static string Escape(string? value) => CsvText.Escape(value);
    }
}
