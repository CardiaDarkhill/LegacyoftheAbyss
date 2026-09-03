#nullable enable

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>
    /// Shared formatting for the CSV files a bug report carries.
    /// <para>
    /// The event ring and the flight recorder both write one, and both had grown their own copy of
    /// the same escaping - which is the kind of duplication that only shows up when one of them is
    /// fixed and the other is not.
    /// </para>
    /// </summary>
    internal static class CsvText
    {
        /// <summary>
        /// Quotes a field only when it needs it: a value carrying a comma, a quote or a newline.
        /// Embedded newlines become spaces rather than being escaped, because a report is read as
        /// much by eye as by a spreadsheet and a wrapped row is unreadable either way.
        /// </summary>
        internal static string Escape(string? value)
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
