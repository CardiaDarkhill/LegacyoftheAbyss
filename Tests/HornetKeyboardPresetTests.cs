#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

/// <summary>
/// The left-side keyboard preset writes the player's saved layout and binds the live actions from
/// one table. These pin down what a hand-written second copy of it got wrong before: two rows
/// naming the same thing, so one key silently did nothing.
/// </summary>
public class HornetKeyboardPresetTests
{
    [Fact]
    public void EveryPresetRowWritesItsOwnSettingsField()
    {
        var settings = new GameSettings();
        var fields = typeof(GameSettings)
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.FieldType == typeof(string))
            .ToList();

        Assert.NotEmpty(fields);

        var claimed = new Dictionary<string, string>();
        foreach (var row in HornetInput.LeftSideLayout)
        {
            var before = fields.ToDictionary(f => f.Name, f => (string?)f.GetValue(settings));
            row.Save(settings, row.Key.ToString());

            var written = fields
                .Where(f => !string.Equals((string?)f.GetValue(settings), before[f.Name]))
                .Select(f => f.Name)
                .ToList();

            Assert.True(
                written.Count == 1,
                $"The preset row for {row.Key} wrote {written.Count} settings field(s) [{string.Join(", ", written)}]; "
                + "each row owns exactly one.");

            Assert.False(
                claimed.ContainsKey(written[0]),
                $"Both {claimed.GetValueOrDefault(written[0])} and {row.Key} write GameSettings.{written[0]}, "
                + "so one of them is lost.");

            claimed[written[0]] = row.Key.ToString();
        }
    }

    /// <summary>
    /// Two rows on the same key would leave one action bound to a key another action also claims,
    /// which is how the number row came to open the wrong tabs.
    /// </summary>
    [Fact]
    public void NoTwoPresetRowsUseTheSameKey()
    {
        var keys = HornetInput.LeftSideLayout.Select(row => row.Key).ToList();
        Assert.Equal(keys.Count, keys.Distinct().Count());
    }
}
