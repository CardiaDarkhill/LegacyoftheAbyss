using System;
using BepInEx.Logging;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>
    /// Taps the whole BepInEx logging chain into a <see cref="BugReportLogRing"/>.
    /// <para>
    /// Registering as a listener rather than wrapping this mod's own <c>ManualLogSource</c> is
    /// deliberate: the log lines that explain a Shade bug are frequently *not* ours - they are the
    /// Unity source reporting a NullReference deep in a PlayMaker FSM, or another plugin complaining
    /// about the object we just patched. A listener sees all of it from every source.
    /// </para>
    /// </summary>
    internal sealed class BugReportLogCollector : ILogListener
    {
        private readonly BugReportLogRing _ring;

        internal BugReportLogCollector(BugReportLogRing ring)
        {
            _ring = ring ?? throw new ArgumentNullException(nameof(ring));
        }

        public LogLevel LogLevelFilter => LogLevel.All;

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                return;
            }

            try
            {
                // ToString() on the payload can run arbitrary game code. A throw here would propagate
                // back into whoever was logging, so a bad log line must never be able to take out the
                // caller - it just costs us that one line of context.
                string message;
                try
                {
                    message = eventArgs.Data?.ToString() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    message = "<log payload threw " + ex.GetType().Name + ">";
                }

                _ring.Add(new BugReportLogEntry(
                    DateTime.UtcNow,
                    BugReportClock.Realtime,
                    eventArgs.Level.ToString(),
                    eventArgs.Source?.SourceName ?? "?",
                    message));
            }
            catch
            {
            }
        }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Last known value of <c>Time.realtimeSinceStartup</c>, published once per frame from the main
    /// thread. <c>UnityEngine.Time</c> throws when touched from anywhere else, and log events arrive
    /// on whichever thread produced them, so the timestamps in the ring have to come from here.
    /// </summary>
    internal static class BugReportClock
    {
        private static float s_realtime;

        internal static float Realtime => s_realtime;

        internal static void Tick(float realtime)
        {
            s_realtime = realtime;
        }
    }
}
