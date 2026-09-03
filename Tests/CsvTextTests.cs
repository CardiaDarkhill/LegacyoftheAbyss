using LegacyoftheAbyss.Diagnostics;
using Xunit;

/// <summary>
/// The escaping both of a bug report's CSV files share. It was two copies until they were merged,
/// so these pin the behaviour that has to survive being one.
/// </summary>
public class CsvTextTests
{
    [Theory]
    [InlineData("plain", "plain")]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("has space", "has space")]
    public void OrdinaryFieldsAreLeftAlone(string input, string expected)
    {
        Assert.Equal(expected, CsvText.Escape(input));
    }

    [Fact]
    public void ACommaForcesQuotes()
    {
        Assert.Equal("\"a,b\"", CsvText.Escape("a,b"));
    }

    [Fact]
    public void QuotesAreDoubledAndTheFieldIsQuoted()
    {
        Assert.Equal("\"say \"\"hi\"\"\"", CsvText.Escape("say \"hi\""));
    }

    [Fact]
    public void NewlinesBecomeSpacesRatherThanWrappingTheRow()
    {
        // A report is read by eye as much as by a spreadsheet, and a row that wraps is unreadable
        // either way - so the newline is spent rather than escaped.
        Assert.Equal("\"one two\"", CsvText.Escape("one\ntwo"));
        Assert.Equal("\"one two\"", CsvText.Escape("one\r\ntwo"));
    }
}
