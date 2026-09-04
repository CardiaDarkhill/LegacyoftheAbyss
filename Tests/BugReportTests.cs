using System;
using System.IO;
using System.Linq;
using LegacyoftheAbyss.Diagnostics;
using UnityEngine;
using Xunit;

/// <summary>
/// Covers the parts of bug capture that are plain managed code: the two ring buffers, the report id
/// and slug rules, and the markdown/CSV rendering.
/// <para>
/// Scope note matches the rest of this suite - anything needing a live Unity player loop is out, so
/// <c>BugReportSystem</c> itself (MonoBehaviour, IMGUI, ScreenCapture) is not exercised here. What is
/// exercised is everything a report's *contents* depend on, which is where a silent regression would
/// actually cost something: a report that writes but has lost its oldest lines, or drops the column a
/// diagnosis needed, still looks like a success at the call site.
/// </para>
/// </summary>
public class BugReportTests
{
    private static BugReportLogEntry Entry(string message, string level = "Info", string source = "Test")
    {
        return new BugReportLogEntry(new DateTime(2026, 8, 22, 17, 32, 45, DateTimeKind.Utc), 12.5f, level, source, message);
    }

    [Fact]
    public void LogRingKeepsTheMostRecentEntriesInOrder()
    {
        var ring = new BugReportLogRing(BugReportLogRing.MinimumCapacity);
        for (int i = 0; i < BugReportLogRing.MinimumCapacity + 5; i++)
        {
            ring.Add(Entry("line " + i));
        }

        var snapshot = ring.Snapshot();

        Assert.Equal(BugReportLogRing.MinimumCapacity, snapshot.Length);
        // Oldest first, and the five that fell off the back are the five oldest.
        Assert.Equal("line 5", snapshot[0].Message);
        Assert.Equal("line " + (BugReportLogRing.MinimumCapacity + 4), snapshot[snapshot.Length - 1].Message);
    }

    [Fact]
    public void LogRingCountsWhatItDropped()
    {
        var ring = new BugReportLogRing(BugReportLogRing.MinimumCapacity);
        for (int i = 0; i < BugReportLogRing.MinimumCapacity + 3; i++)
        {
            ring.Add(Entry("line " + i));
        }

        Assert.Equal(3, ring.DroppedCount);

        // The report has to say so: a log that silently starts partway through reads as "nothing
        // happened before this", which is a different and wrong conclusion.
        Assert.Contains("3 earlier line(s) dropped", ring.Render(), StringComparison.Ordinal);
    }

    [Fact]
    public void LogRingFoldsARepeatedLineInsteadOfSpendingTheRing()
    {
        var ring = new BugReportLogRing(BugReportLogRing.MinimumCapacity);
        ring.Add(Entry("before"));
        for (int i = 0; i < 500; i++)
        {
            ring.Add(new BugReportLogEntry(
                new DateTime(2026, 8, 22, 17, 32, 45, DateTimeKind.Utc),
                12.5f + (i * 0.01f),
                "Error",
                "Unity Log",
                "NullReferenceException"));
        }

        ring.Add(Entry("after"));

        var snapshot = ring.Snapshot();

        // The whole point: a plugin throwing once a frame used to evict every other line in the ring
        // within a minute, and several reports in a row arrived with nothing else left to read.
        Assert.Equal(3, snapshot.Length);
        Assert.Equal(0, ring.DroppedCount);
        Assert.Equal("before", snapshot[0].Message);
        Assert.Equal("after", snapshot[2].Message);
        Assert.Equal(500, snapshot[1].RepeatCount);
        Assert.Contains("(x500 over", snapshot[1].Format(), StringComparison.Ordinal);
    }

    [Fact]
    public void LogRingOnlyFoldsConsecutiveMatches()
    {
        var ring = new BugReportLogRing(BugReportLogRing.MinimumCapacity);
        ring.Add(Entry("same"));
        ring.Add(Entry("different"));
        ring.Add(Entry("same"));

        var snapshot = ring.Snapshot();

        // Folding across a gap would claim an ordering that never happened.
        Assert.Equal(3, snapshot.Length);
        Assert.All(snapshot, entry => Assert.Equal(1, entry.RepeatCount));
    }

    [Fact]
    public void LogRingCapacityIsClampedToASaneRange()
    {
        Assert.Equal(BugReportLogRing.MinimumCapacity, new BugReportLogRing(0).Capacity);
        Assert.Equal(BugReportLogRing.MinimumCapacity, new BugReportLogRing(-100).Capacity);
        Assert.Equal(BugReportLogRing.MaximumCapacity, new BugReportLogRing(int.MaxValue).Capacity);
        Assert.Equal(500, new BugReportLogRing(500).Capacity);
    }

    [Fact]
    public void LogRingRendersLevelAndSource()
    {
        var ring = new BugReportLogRing(BugReportLogRing.MinimumCapacity);
        ring.Add(Entry("something broke", "Error", "Unity"));

        string rendered = ring.Render();

        Assert.Contains("Error", rendered, StringComparison.Ordinal);
        Assert.Contains("Unity", rendered, StringComparison.Ordinal);
        Assert.Contains("something broke", rendered, StringComparison.Ordinal);
    }

    [Fact]
    public void EventRingKeepsTheMostRecentEvents()
    {
        var ring = new BugReportEventRing(BugReportEventRing.MinimumCapacity);
        Assert.Equal(BugReportEventRing.MinimumCapacity, ring.Capacity);

        for (int i = 0; i < ring.Capacity + 3; i++)
        {
            ring.Add("test", "event " + i, null, i, i);
        }

        var events = ring.Snapshot();

        Assert.Equal(ring.Capacity, events.Length);
        Assert.Equal("event 3", events[0].Summary);
        Assert.Equal("event " + (ring.Capacity + 2), events[events.Length - 1].Summary);
    }

    /// <summary>
    /// The per-frame emitters are the reason this matters: one boss attack repeating for under a
    /// second wrote several hundred identical rows and flushed the rest of the window out of the
    /// ring. A repeat has to cost one slot, not one slot per frame.
    /// </summary>
    [Fact]
    public void RepeatedEventsCoalesceInsteadOfFloodingTheRing()
    {
        var ring = new BugReportEventRing(BugReportEventRing.MinimumCapacity);

        ring.Add("shade-damage", "took damage", "Lace via hero damager", 10f, 1);
        for (int i = 1; i <= 50; i++)
        {
            ring.Add("shade-damage", "took damage", "Lace via hero damager", 10f + (i * 0.016f), 1 + i);
        }
        ring.Add("shade-damage", "took damage", "Lace via Battle Range", 11f, 60);

        var events = ring.Snapshot();

        Assert.Equal(2, events.Length);
        Assert.Equal(50, events[0].Repeats);
        Assert.Equal(10f, events[0].Realtime);
        Assert.Equal(10.8f, events[0].LastRealtime, 3);
        // A different detail is a different event, so it must not be folded into the run above it.
        Assert.Equal(0, events[1].Repeats);
    }

    [Fact]
    public void CoalescedRepeatsAreReportedAsACountAndDuration()
    {
        var ring = new BugReportEventRing(BugReportEventRing.MinimumCapacity);
        ring.Add("shade-damage", "took damage", "Lace", 10f, 1);
        ring.Add("shade-damage", "took damage", "Lace", 10.5f, 2);

        Assert.Contains("(x2 over 0.5s)", ring.RenderTail(10, 10.5f), StringComparison.Ordinal);
        Assert.Contains(",1,0.5", ring.ToCsv(10.5f), StringComparison.Ordinal);
    }

    [Fact]
    public void EventCsvTimesRowsRelativeToTheCaptureAndEscapesDetail()
    {
        var ring = new BugReportEventRing(BugReportEventRing.MinimumCapacity);
        ring.Add("shade-proxy-entered", "Lace Boss/Multihitter", "layer=Player, tag=Player", 97f, 5820);

        string csv = ring.ToCsv(100f);

        Assert.StartsWith("t_rel,realtime,frame,category,summary,detail", csv, StringComparison.Ordinal);
        Assert.Contains("-3,97,5820,shade-proxy-entered", csv, StringComparison.Ordinal);
        // The commas inside the detail must not become extra columns.
        Assert.Contains("\"layer=Player, tag=Player\"", csv, StringComparison.Ordinal);
    }

    [Fact]
    public void EventRingCapacityIsClampedToItsBounds()
    {
        Assert.Equal(BugReportEventRing.MinimumCapacity, new BugReportEventRing(0).Capacity);
        Assert.Equal(BugReportEventRing.MaximumCapacity, new BugReportEventRing(int.MaxValue).Capacity);
    }

    [Fact]
    public void FlightRecorderSizesItselfFromTheWindowAndInterval()
    {
        Assert.Equal(300, BugReportFlightRecorder.CapacityFor(30f, 0.1f));
        Assert.Equal(60, BugReportFlightRecorder.CapacityFor(30f, 0.5f));

        // A zero interval would ask for an infinite buffer; the floor is what stops that.
        Assert.Equal(
            BugReportFlightRecorder.CapacityFor(30f, BugReportFlightRecorder.MinimumIntervalSeconds),
            BugReportFlightRecorder.CapacityFor(30f, 0f));
    }

    [Fact]
    public void FlightRecorderKeepsTheMostRecentWindow()
    {
        var recorder = new BugReportFlightRecorder(1f, 0.5f);
        Assert.Equal(2, recorder.Capacity);

        recorder.Add(new BugReportFlightSample { Realtime = 1f, Scene = "first" });
        recorder.Add(new BugReportFlightSample { Realtime = 2f, Scene = "second" });
        recorder.Add(new BugReportFlightSample { Realtime = 3f, Scene = "third" });

        var samples = recorder.Snapshot();

        Assert.Equal(2, samples.Length);
        Assert.Equal("second", samples[0].Scene);
        Assert.Equal("third", samples[1].Scene);
    }

    [Fact]
    public void FlightCsvTimesRowsRelativeToTheCapture()
    {
        var recorder = new BugReportFlightRecorder(10f, 1f);
        recorder.Add(new BugReportFlightSample { Realtime = 97f, Scene = "Bone_01" });
        recorder.Add(new BugReportFlightSample { Realtime = 100f, Scene = "Bone_01" });

        var lines = recorder.ToCsv(100f).Split('\n').Where(line => line.Length > 0).ToArray();

        Assert.StartsWith("t_rel,realtime,scene", lines[0], StringComparison.Ordinal);
        // Rows lead into the capture, so the history reads as negative seconds up to roughly zero.
        Assert.StartsWith("-3,", lines[1], StringComparison.Ordinal);
        Assert.StartsWith("0,", lines[2], StringComparison.Ordinal);
    }

    [Fact]
    public void FlightCsvQuotesValuesContainingSeparators()
    {
        var recorder = new BugReportFlightRecorder(10f, 1f);
        recorder.Add(new BugReportFlightSample { Realtime = 1f, HeroFlags = "onGround,attacking" });

        string csv = recorder.ToCsv(1f);

        Assert.Contains("\"onGround,attacking\"", csv, StringComparison.Ordinal);
    }

    [Fact]
    public void FlightCsvRecordsShadeAbsence()
    {
        var recorder = new BugReportFlightRecorder(10f, 1f);
        recorder.Add(new BugReportFlightSample { Realtime = 1f, ShadePresent = false });
        recorder.Add(new BugReportFlightSample { Realtime = 2f, ShadePresent = true, ShadeHp = 4 });

        var rows = recorder.ToCsv(2f).Split('\n').Where(line => line.Length > 0).Skip(1).ToArray();

        Assert.Contains(",0,", rows[0], StringComparison.Ordinal);
        Assert.Contains(",1,", rows[1], StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("Shade clips through the floor", "shade-clips-through-the-floor")]
    [InlineData("  Focus  heals   twice!  ", "focus-heals-twice")]
    [InlineData("HP/soul desync (again)", "hp-soul-desync-again")]
    [InlineData("", "report")]
    [InlineData("   ", "report")]
    [InlineData("!!!", "report")]
    [InlineData(null, "report")]
    public void SlugifyProducesAFilesystemSafeName(string title, string expected)
    {
        Assert.Equal(expected, BugReportStore.Slugify(title));
    }

    [Fact]
    public void SlugifyIsBoundedSoLongTitlesStayValidPaths()
    {
        string slug = BugReportStore.Slugify(new string('a', 500));

        Assert.True(slug.Length <= 48, "Slug was " + slug.Length + " characters.");
    }

    [Fact]
    public void SlugifyCutsAtAWordBoundary()
    {
        string slug = BugReportStore.Slugify("This is a test bug to test the bug report feature");

        // A hard cut at the character limit lands mid-word ("...-bug-report-featur"), which reads as
        // a corrupted folder name rather than a shortened one.
        Assert.Equal("this-is-a-test-bug-to-test-the-bug-report", slug);
        Assert.DoesNotContain("featur", slug, StringComparison.Ordinal);
    }

    [Fact]
    public void SlugifyStillTruncatesAnUnbrokenWord()
    {
        // No hyphen to back off to, so the hard cut is all that is left.
        string slug = BugReportStore.Slugify(new string('b', 200));

        Assert.Equal(new string('b', 48), slug);
    }

    [Fact]
    public void SlugsNeverEndInASeparator()
    {
        Assert.Equal("trailing", BugReportStore.Slugify("trailing!!!"));
        Assert.Equal("a-b", BugReportStore.Slugify("a  ...  b  ---"));
    }

    [Fact]
    public void ReportIdsSortChronologically()
    {
        string earlier = BugReportStore.BuildReportId(new DateTime(2026, 8, 22, 9, 0, 0), "b title");
        string later = BugReportStore.BuildReportId(new DateTime(2026, 8, 22, 17, 32, 45), "a title");

        Assert.Equal("20260822-090000-b-title", earlier);
        Assert.Equal("20260822-173245-a-title", later);
        // Ordinal ordering has to match time ordering, or a folder listing stops being a timeline.
        Assert.True(string.CompareOrdinal(earlier, later) < 0);
    }

    [Fact]
    public void FirstMessageLineBecomesTheTitle()
    {
        BugReportStore.SplitMessage("Shade falls through floor\nHappens after a teleport\nEvery time", out var title, out var body);

        Assert.Equal("Shade falls through floor", title);
        Assert.Equal("Happens after a teleport\nEvery time", body);
    }

    [Fact]
    public void SingleLineMessagesAreAllTitle()
    {
        BugReportStore.SplitMessage("Soul bar renders behind the HUD", out var title, out var body);

        Assert.Equal("Soul bar renders behind the HUD", title);
        Assert.Equal(string.Empty, body);
    }

    [Fact]
    public void WindowsLineEndingsDoNotLeakIntoTheTitle()
    {
        BugReportStore.SplitMessage("Title here\r\nBody here", out var title, out var body);

        Assert.Equal("Title here", title);
        Assert.Equal("Body here", body);
    }

    [Fact]
    public void EmptyMessagesStillProduceATitle()
    {
        BugReportStore.SplitMessage("   ", out var title, out var body);

        Assert.Equal("Untitled report", title);
        Assert.Equal(string.Empty, body);
    }

    [Fact]
    public void LogTailKeepsTheMostRecentEntries()
    {
        var ring = new BugReportLogRing(100);
        for (int i = 0; i < 20; i++)
        {
            ring.Add(Entry("line " + i));
        }

        string tail = ring.RenderTail(3, 100000);

        Assert.Contains("line 19", tail, StringComparison.Ordinal);
        Assert.Contains("line 17", tail, StringComparison.Ordinal);
        Assert.DoesNotContain("line 16", tail, StringComparison.Ordinal);
        Assert.Contains("17 earlier entries not shown", tail, StringComparison.Ordinal);
    }

    [Fact]
    public void LogTailIsCountedInEntriesNotRenderedLines()
    {
        // The case this replaced a line-counted tail for: one Unity exception entry carries its whole
        // stack trace as embedded newlines. Counted in lines, a single entry like this consumed the
        // entire budget and the report quoted nothing but stack frames.
        var ring = new BugReportLogRing(100);
        ring.Add(Entry("context before the throw"));
        ring.Add(Entry("NullReferenceException\n  at A ()\n  at B ()\n  at C ()\n  at D ()\n  at E ()"));

        string tail = ring.RenderTail(2, 100000);

        Assert.Contains("context before the throw", tail, StringComparison.Ordinal);
        Assert.Contains("at E ()", tail, StringComparison.Ordinal);
    }

    [Fact]
    public void LogTailIsCappedByCharactersAsWell()
    {
        var ring = new BugReportLogRing(100);
        ring.Add(Entry("old and small"));
        ring.Add(Entry(new string('x', 5000)));

        string tail = ring.RenderTail(10, 1000);

        // The newest entry is always kept even when it alone busts the budget; the older one is what
        // gets dropped, because a tail that discards the most recent line is no tail at all.
        Assert.Contains("xxxx", tail, StringComparison.Ordinal);
        Assert.DoesNotContain("old and small", tail, StringComparison.Ordinal);
        Assert.Contains("1 earlier entries not shown", tail, StringComparison.Ordinal);
    }

    [Fact]
    public void LogTailHandlesDegenerateBounds()
    {
        var ring = new BugReportLogRing(100);
        ring.Add(Entry("anything"));

        Assert.Equal(string.Empty, ring.RenderTail(0, 1000));
        Assert.Equal(string.Empty, ring.RenderTail(10, 0));
        Assert.Equal(string.Empty, new BugReportLogRing(100).RenderTail(10, 1000));
    }

    [Fact]
    public void IndexLinesStartUncheckedAndCarryTheTriageFacts()
    {
        var state = new BugReportState
        {
            ReportId = "20260822-173245-shade-clips",
            Title = "Shade clips through the floor",
            Scene = "Bone_East_01",
            ModVersion = "1.4.2",
            Trigger = "hotkey"
        };

        string line = BugReportStore.FormatIndexLine(state);

        Assert.StartsWith("- [ ] `20260822-173245-shade-clips`", line, StringComparison.Ordinal);
        Assert.Contains("Shade clips through the floor", line, StringComparison.Ordinal);
        Assert.Contains("Bone_East_01", line, StringComparison.Ordinal);
        Assert.Contains("1.4.2", line, StringComparison.Ordinal);
    }

    [Fact]
    public void IndexLinesSurviveAMissingSnapshot()
    {
        string line = BugReportStore.FormatIndexLine(new BugReportState { ReportId = "20260822-173245-report" });

        Assert.Contains("Untitled report", line, StringComparison.Ordinal);
        Assert.Contains("scene: ?", line, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderedReportLeadsWithTheAnswersATriageNeeds()
    {
        var state = new BugReportState
        {
            ReportId = "20260822-173245-shade-clips",
            Title = "Shade clips through the floor",
            Message = "Shade clips through the floor\nOnly after teleporting while focusing.",
            Trigger = "hotkey",
            CapturedLocal = "2026-08-22 17:32:45",
            ModVersion = "1.4.2",
            Scene = "Bone_East_01",
            SceneHistory = new[] { "Bone_01", "Bone_East_01" },
            Hero = new BugReportHeroState { Present = true, X = 12.5f, Y = -3f, Health = 4, MaxHealth = 5, ActorState = "idle" },
            Shade = new BugReportShadeState
            {
                Present = true,
                X = 12.9f,
                Y = -9.5f,
                Flags = "teleporting|focusing",
                Character = "The Knight (Knight Moveset)",
                Hp = 2,
                MaxHp = 4,
                EquippedCharms = new[] { "SteadyBody", "SharpShadow" }
            }
        };

        string markdown = BugReportStore.RenderMarkdown(
            state,
            "log line one\nlog line two",
            "[-1.2s] shade-damage: took damage - Lace via Multihitter",
            hasLog: true,
            hasFlight: true,
            hasEvents: true,
            hasScreenshot: true);

        Assert.StartsWith("# Shade clips through the floor", markdown, StringComparison.Ordinal);
        Assert.Contains("Only after teleporting while focusing.", markdown, StringComparison.Ordinal);
        Assert.Contains("Bone_East_01", markdown, StringComparison.Ordinal);
        Assert.Contains("Bone_01 -> Bone_East_01", markdown, StringComparison.Ordinal);
        Assert.Contains("teleporting|focusing".Replace("|", "\\|"), markdown, StringComparison.Ordinal);

        // Which body it was decides how everything below it reads: the Shade flies and the
        // Knight walks, and nothing else in a report tells them apart.
        Assert.Contains("The Knight (Knight Moveset)", markdown, StringComparison.Ordinal);
        Assert.Contains("SteadyBody, SharpShadow", markdown, StringComparison.Ordinal);
        Assert.Contains(BugReportStore.FlightFileName, markdown, StringComparison.Ordinal);
        Assert.Contains(BugReportStore.EventFileName, markdown, StringComparison.Ordinal);
        Assert.Contains(BugReportStore.ScreenshotFileName, markdown, StringComparison.Ordinal);
        Assert.Contains("log line two", markdown, StringComparison.Ordinal);
        Assert.Contains("Lace via Multihitter", markdown, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderedReportSaysSoWhenThereWasNoShade()
    {
        var state = new BugReportState
        {
            ReportId = "20260822-173245-no-shade",
            Title = "Menu bug",
            Shade = new BugReportShadeState { Present = false }
        };

        string markdown = BugReportStore.RenderMarkdown(state, null, null, hasLog: false, hasFlight: false, hasEvents: false, hasScreenshot: false);

        Assert.Contains("No Shade instance was alive", markdown, StringComparison.Ordinal);
        // Nothing should advertise files that were never written.
        Assert.DoesNotContain(BugReportStore.EventFileName, markdown, StringComparison.Ordinal);
        Assert.DoesNotContain(BugReportStore.FlightFileName, markdown, StringComparison.Ordinal);
        Assert.DoesNotContain(BugReportStore.ScreenshotFileName, markdown, StringComparison.Ordinal);
        Assert.DoesNotContain(BugReportStore.LogFileName, markdown, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderedReportQuotesAnAutoCapturedException()
    {
        var state = new BugReportState
        {
            ReportId = "20260822-173245-exception",
            Title = "Exception: NullReferenceException",
            Trigger = "auto-exception",
            ExceptionMessage = "NullReferenceException: Object reference not set to an instance of an object",
            ExceptionStackTrace = "  at LegacyHelper.ShadeController.Update ()"
        };

        string markdown = BugReportStore.RenderMarkdown(state, null, null, hasLog: false, hasFlight: false, hasEvents: false, hasScreenshot: false);

        Assert.Contains("## Exception", markdown, StringComparison.Ordinal);
        Assert.Contains("LegacyHelper.ShadeController.Update", markdown, StringComparison.Ordinal);
    }

    [Fact]
    public void PipesInStateValuesDoNotBreakTheMarkdownTables()
    {
        var state = new BugReportState
        {
            ReportId = "20260822-173245-pipes",
            Title = "Flags",
            Hero = new BugReportHeroState { Present = true, Flags = "onGround|attacking|recoiling" }
        };

        string markdown = BugReportStore.RenderMarkdown(state, null, null, hasLog: false, hasFlight: false, hasEvents: false, hasScreenshot: false);

        // A raw pipe here would silently split the cell into three columns and the row would render
        // as nonsense.
        Assert.Contains("onGround\\|attacking\\|recoiling", markdown, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(3840, 2160, 1920, 1080)]
    [InlineData(2560, 1440, 1920, 1080)]
    [InlineData(1920, 1200, 960, 600)]
    [InlineData(100, 33, 10, 3)]
    public void ScaledHeightPreservesTheAspectRatio(int sourceWidth, int sourceHeight, int targetWidth, int expected)
    {
        Assert.Equal(expected, BugReportImage.ScaledHeight(sourceWidth, sourceHeight, targetWidth));
    }

    [Fact]
    public void ScaledHeightNeverCollapsesToZero()
    {
        // A very wide, very short source would otherwise round to a zero-height texture, which Unity
        // rejects outright and would cost the report its screenshot.
        Assert.Equal(1, BugReportImage.ScaledHeight(4000, 3, 8));
        Assert.Equal(1, BugReportImage.ScaledHeight(0, 0, 100));
    }

    [Fact]
    public void BoxDownscaleAveragesEachSourceBlock()
    {
        // 2x2 -> 1x1: the single output pixel is the mean of all four inputs.
        var source = new[]
        {
            new Color32(0, 0, 0, 255),
            new Color32(100, 100, 100, 255),
            new Color32(200, 200, 200, 255),
            new Color32(100, 100, 100, 255)
        };

        var scaled = BugReportImage.BoxDownscale(source, 2, 2, 1, 1);

        Assert.Single(scaled);
        Assert.Equal(100, scaled[0].r);
        Assert.Equal(255, scaled[0].a);
    }

    [Fact]
    public void BoxDownscaleKeepsAThinFeatureVisible()
    {
        // The reason this is a box filter rather than point sampling. One bright column in an
        // otherwise black 4x1 row must survive a 4:1 reduction as a dimmer pixel, not vanish -
        // a one-pixel outline is exactly what a Shade rendering report is about.
        var source = new[]
        {
            new Color32(0, 0, 0, 255),
            new Color32(0, 0, 0, 255),
            new Color32(255, 255, 255, 255),
            new Color32(0, 0, 0, 255)
        };

        var scaled = BugReportImage.BoxDownscale(source, 4, 1, 1, 1);

        Assert.True(scaled[0].r > 0, "The bright column was sampled away entirely.");
    }

    [Fact]
    public void BoxDownscaleCoversEveryOutputPixelOnANonIntegerRatio()
    {
        // 3 -> 2 does not divide evenly; every destination pixel must still get a source block.
        var source = new Color32[9];
        for (int i = 0; i < source.Length; i++)
        {
            source[i] = new Color32(50, 60, 70, 255);
        }

        var scaled = BugReportImage.BoxDownscale(source, 3, 3, 2, 2);

        Assert.Equal(4, scaled.Length);
        Assert.All(scaled, pixel =>
        {
            Assert.Equal(50, pixel.r);
            Assert.Equal(255, pixel.a);
        });
    }

    [Fact]
    public void BoxDownscaleRejectsInconsistentInput()
    {
        Assert.Throws<ArgumentNullException>(() => BugReportImage.BoxDownscale(null!, 2, 2, 1, 1));
        Assert.Throws<ArgumentException>(() => BugReportImage.BoxDownscale(new Color32[2], 4, 4, 2, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => BugReportImage.BoxDownscale(new Color32[4], 2, 2, 0, 1));
    }

    private sealed class FlagBag
    {
        public bool onGround = true;
        public bool attacking = false;
        public bool dead = true;
        public int notABool = 5;
    }

    [Fact]
    public void OnlyTheSetFlagsAreDescribed()
    {
        string described = BugReportStateCollector.DescribeTrueBoolFields(new FlagBag());

        Assert.Equal("onGround|dead", described);
    }

    [Fact]
    public void DescribingFlagsToleratesNull()
    {
        Assert.Equal(string.Empty, BugReportStateCollector.DescribeTrueBoolFields(null));
    }

    [Fact]
    public void StateSerialisesToJsonWithoutTheNullNoise()
    {
        var state = new BugReportState
        {
            ReportId = "20260822-173245-json",
            Title = "Json",
            Scene = "Bone_01",
            Shade = new BugReportShadeState { Present = true, Soul = 33 }
        };

        string json = BugReportStore.SerializeState(state);

        Assert.Contains("\"ReportId\": \"20260822-173245-json\"", json, StringComparison.Ordinal);
        Assert.Contains("\"Soul\": 33", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"Hero\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void WritingAReportProducesAReadableFolderAndIndexesIt()
    {
        // The one test here that touches disk. Everything above checks the *content* of a report;
        // this checks that a report actually lands - directory creation, the file set, and the index
        // append are exactly the steps that fail silently, because Write swallows its own IO errors
        // rather than letting a failed capture take the game down.
        var payload = new BugReportPayload
        {
            State = new BugReportState
            {
                Title = "Round trip check",
                Message = "Round trip check\nWritten by the test suite.",
                Trigger = "test",
                Scene = "TestScene",
                ModVersion = "0.0.0-test"
            },
            LogText = "a log line",
            FlightCsv = "t_rel,realtime\n0,1",
            ScreenshotPng = new byte[] { 1, 2, 3, 4 }
        };

        var result = BugReportStore.Write(payload);

        try
        {
            Assert.True(result.Success, result.Error);
            Assert.True(Directory.Exists(result.Folder));
            Assert.True(File.Exists(Path.Combine(result.Folder, BugReportStore.ReportFileName)));
            Assert.True(File.Exists(Path.Combine(result.Folder, BugReportStore.StateFileName)));
            Assert.True(File.Exists(Path.Combine(result.Folder, BugReportStore.LogFileName)));
            Assert.True(File.Exists(Path.Combine(result.Folder, BugReportStore.FlightFileName)));
            Assert.True(File.Exists(Path.Combine(result.Folder, BugReportStore.ScreenshotFileName)));

            Assert.Contains(
                result.ReportId,
                File.ReadAllText(Path.Combine(BugReportStore.Root, BugReportStore.IndexFileName)),
                StringComparison.Ordinal);

            // No byte-order mark on anything. It is invisible in an editor, but it becomes part of the
            // first CSV header cell and prefixes the leading "#" of the markdown, so both files stop
            // parsing cleanly for anything stricter than a human eye.
            foreach (string name in new[]
            {
                BugReportStore.ReportFileName,
                BugReportStore.StateFileName,
                BugReportStore.LogFileName,
                BugReportStore.FlightFileName
            })
            {
                var bytes = File.ReadAllBytes(Path.Combine(result.Folder, name));
                Assert.False(
                    bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF,
                    name + " was written with a UTF-8 BOM.");
            }

            Assert.StartsWith("t_rel,", File.ReadAllText(Path.Combine(result.Folder, BugReportStore.FlightFileName)), StringComparison.Ordinal);
        }
        finally
        {
            try
            {
                Directory.Delete(result.Folder, recursive: true);
            }
            catch
            {
            }
        }
    }
}
