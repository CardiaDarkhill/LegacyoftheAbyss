using System;
using System.IO;
using System.Linq;
using System.Reflection;
using LegacyoftheAbyss.Shade;
using Xunit;

/// <summary>
/// Difficulty is stored per save profile. These pin down the two ways that goes quietly wrong: a
/// setting that exists on the presets but not in what gets saved, and a slot that loses its
/// difficulty on the way to or from disk.
/// </summary>
public class ShadeDifficultySettingsTests
{
    /// <summary>
    /// Every setting on the type has to be wired into both halves of the config round trip. A
    /// preset now holds one of these rather than a parallel list of the same ten values, so the way
    /// an eleventh setting goes wrong is no longer the preset forgetting it - it is
    /// <c>ApplyTo</c> or <c>CaptureFrom</c> forgetting it, which is just as silent: the setting
    /// behaves correctly on the profile it was set on and stays global everywhere else.
    /// <para>
    /// Driven off the property list rather than a written-out set, so a new setting is covered by
    /// existing, not by being remembered.
    /// </para>
    /// </summary>
    [Fact]
    public void EveryDifficultyValueSurvivesTheConfigRoundTrip()
    {
        var properties = typeof(ShadeDifficultySettings)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite)
            .ToList();

        Assert.NotEmpty(properties);

        // Every value moved off its default, so a property that never reaches the config comes back
        // as the config's default and fails rather than coincidentally matching.
        var settings = new ShadeDifficultySettings();
        foreach (var property in properties)
        {
            object current = property.GetValue(settings)!;
            object moved = current switch
            {
                float f => (object)(f + 0.125f),
                int i => (object)(i + 1),
                bool b => (object)!b,
                _ => throw new InvalidOperationException(
                    $"ShadeDifficultySettings.{property.Name} is a {property.PropertyType.Name}, which this test "
                    + "does not know how to move off its default. Teach it, rather than exempting the property."),
            };

            property.SetValue(settings, moved);
        }

        var config = new ModConfig();
        settings.ApplyTo(config);
        var restored = ShadeDifficultySettings.CaptureFrom(config);

        foreach (var property in properties)
        {
            Assert.True(
                Equals(property.GetValue(settings), property.GetValue(restored)),
                $"ShadeDifficultySettings.{property.Name} did not survive ApplyTo/CaptureFrom, so it would "
                + "stay global rather than following the save profile.");
        }
    }

    [Fact]
    public void EveryPresetSurvivesCaptureAndApply()
    {
        foreach (var preset in DifficultyPreset.All)
        {
            var config = new ModConfig();
            preset.ApplyTo(config);

            var captured = ShadeDifficultySettings.CaptureFrom(config);

            // Onto a config holding something else entirely, so a value that failed to carry shows
            // up as the other preset's rather than coincidentally matching.
            var restored = new ModConfig();
            DifficultyPreset.AbyssPreset.ApplyTo(restored);
            captured.ApplyTo(restored);

            Assert.Equal(preset.Name, DifficultyPreset.IdentifyName(restored));
        }
    }

    [Fact]
    public void SettingsMatchThemselvesAndNotEachOther()
    {
        var easy = CaptureOf(DifficultyPreset.EasyPreset);
        var abyss = CaptureOf(DifficultyPreset.AbyssPreset);

        Assert.True(easy.Matches(easy.Clone()));
        Assert.False(easy.Matches(abyss));
        Assert.False(easy.Matches(null));
    }

    [Fact]
    public void ASlotRemembersItsDifficultyAcrossAReload()
    {
        string root = Path.Combine(Path.GetTempPath(), "loa-difficulty-" + Guid.NewGuid().ToString("N"));
        try
        {
            var repository = new ShadeSaveSlotRepository(4, root);

            Assert.Null(repository.GetDifficulty(0));
            Assert.True(repository.SetDifficulty(0, CaptureOf(DifficultyPreset.AbyssPreset)));

            // The same values again are not a change, so the file is not rewritten.
            Assert.False(repository.SetDifficulty(0, CaptureOf(DifficultyPreset.AbyssPreset)));

            repository.SetDifficulty(1, CaptureOf(DifficultyPreset.EasyPreset));

            var reloaded = new ShadeSaveSlotRepository(4, root);

            var slotOne = reloaded.GetDifficulty(0);
            var slotTwo = reloaded.GetDifficulty(1);
            Assert.NotNull(slotOne);
            Assert.NotNull(slotTwo);

            // The whole point: two profiles, two difficulties, neither reading the other's.
            Assert.True(slotOne!.Matches(CaptureOf(DifficultyPreset.AbyssPreset)));
            Assert.True(slotTwo!.Matches(CaptureOf(DifficultyPreset.EasyPreset)));
            Assert.Null(reloaded.GetDifficulty(2));
        }
        finally
        {
            TryDelete(root);
        }
    }

    /// <summary>
    /// A slot whose only content is a difficulty still has to reach disk. The repository deletes
    /// records it considers empty, and difficulty is set before a new file has any charms in it.
    /// </summary>
    [Fact]
    public void ADifficultyAloneKeepsASlotOnDisk()
    {
        string root = Path.Combine(Path.GetTempPath(), "loa-difficulty-" + Guid.NewGuid().ToString("N"));
        try
        {
            var repository = new ShadeSaveSlotRepository(4, root);
            repository.SetDifficulty(2, CaptureOf(DifficultyPreset.HardPreset));

            var reloaded = new ShadeSaveSlotRepository(4, root);
            Assert.NotNull(reloaded.GetDifficulty(2));
        }
        finally
        {
            TryDelete(root);
        }
    }

    private static ShadeDifficultySettings CaptureOf(DifficultyPreset preset)
    {
        var config = new ModConfig();
        preset.ApplyTo(config);
        return ShadeDifficultySettings.CaptureFrom(config);
    }

    private static void TryDelete(string root)
    {
        try
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
        }
        catch
        {
        }
    }
}
