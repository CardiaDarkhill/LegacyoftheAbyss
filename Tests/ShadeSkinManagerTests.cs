using System;
using System.IO;
using System.Linq;
using LegacyoftheAbyss.Shade;
using Xunit;

[Collection(ModConfigCollection.Name)]
public class ShadeSkinManagerTests : IDisposable
{
    private readonly string root;
    private readonly string originalSkin;

    public ShadeSkinManagerTests()
    {
        originalSkin = ModConfig.Instance.shadeSkin;
        // Start from a known selection so these tests do not depend on whatever the
        // on-disk config happens to hold, and do not depend on each other's ordering.
        ModConfig.Instance.shadeSkin = ShadeSkinManager.DefaultSkinId;
        root = Path.Combine(Path.GetTempPath(), "LotaSkinTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
    }

    public void Dispose()
    {
        // SelectSkin persists, so put the file back the way we found it.
        ModConfig.Instance.shadeSkin = originalSkin;
        ModConfig.Save();
        ShadeSkinManager.Reload();
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

    private void WriteSheet(string relativeDirectory, string fileName)
    {
        string directory = string.IsNullOrEmpty(relativeDirectory) ? root : Path.Combine(root, relativeDirectory);
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, fileName), relativeDirectory + "/" + fileName);
    }

    private string SkinDir(string name) => Path.Combine(root, ShadeSkinManager.SkinsFolderName, name);

    [Fact]
    public void DefaultSkinIsAlwaysFirstEvenWithNoSkinFolders()
    {
        ShadeSkinManager.Reload(root);

        var skins = ShadeSkinManager.Skins;
        Assert.Single(skins);
        Assert.Equal(ShadeSkinManager.DefaultSkinId, skins[0].Id);
        Assert.True(skins[0].IsDefault);
    }

    [Fact]
    public void ManifestControlsOrderAndDisplayNamesAndUnlistedFoldersAreAppended()
    {
        Directory.CreateDirectory(SkinDir("Zeta"));
        Directory.CreateDirectory(SkinDir("Alpha"));
        Directory.CreateDirectory(SkinDir("4 Horn"));
        Directory.CreateDirectory(SkinDir("Low Horn"));
        File.WriteAllText(
            Path.Combine(root, ShadeSkinManager.SkinsFolderName, "skins.json"),
            "{\"skins\":[{\"id\":\"Low Horn\",\"displayName\":\"Low Horn\"},{\"id\":\"4 Horn\",\"displayName\":\"Four Horn\"}]}");

        ShadeSkinManager.Reload(root);

        var ids = ShadeSkinManager.Skins.Select(s => s.Id).ToArray();
        Assert.Equal(new[] { ShadeSkinManager.DefaultSkinId, "Low Horn", "4 Horn", "Alpha", "Zeta" }, ids);
        Assert.Equal("Four Horn", ShadeSkinManager.Skins.Single(s => s.Id == "4 Horn").DisplayName);
        // A folder with no manifest entry falls back to its folder name.
        Assert.Equal("Alpha", ShadeSkinManager.Skins.Single(s => s.Id == "Alpha").DisplayName);
    }

    [Fact]
    public void MissingManifestStillDiscoversFoldersAlphabetically()
    {
        Directory.CreateDirectory(SkinDir("Zeta"));
        Directory.CreateDirectory(SkinDir("Alpha"));

        ShadeSkinManager.Reload(root);

        Assert.Equal(
            new[] { ShadeSkinManager.DefaultSkinId, "Alpha", "Zeta" },
            ShadeSkinManager.Skins.Select(s => s.Id).ToArray());
    }

    [Fact]
    public void ManifestEntriesWithoutAFolderAreIgnored()
    {
        Directory.CreateDirectory(SkinDir("Alpha"));
        File.WriteAllText(
            Path.Combine(root, ShadeSkinManager.SkinsFolderName, "skins.json"),
            "{\"skins\":[{\"id\":\"Ghost\"},{\"id\":\"Alpha\"}]}");

        ShadeSkinManager.Reload(root);

        Assert.Equal(
            new[] { ShadeSkinManager.DefaultSkinId, "Alpha" },
            ShadeSkinManager.Skins.Select(s => s.Id).ToArray());
    }

    [Fact]
    public void SkinOverridesResolveToTheSkinFolder()
    {
        WriteSheet(string.Empty, "Shade_Idle_Sheet.png");
        WriteSheet(Path.Combine(ShadeSkinManager.SkinsFolderName, "Cozy Shade"), "Shade_Idle_Sheet.png");
        ShadeSkinManager.Reload(root);

        var cozy = ShadeSkinManager.Skins.Single(s => s.Id == "Cozy Shade");
        Assert.Equal(
            Path.Combine(SkinDir("Cozy Shade"), "Shade_Idle_Sheet.png"),
            ShadeSkinManager.ResolveSpritePath(cozy, "Shade_Idle_Sheet.png"));
    }

    [Fact]
    public void SheetsASkinDoesNotOverrideFallBackToTheDefaultSet()
    {
        WriteSheet(string.Empty, "Abyss_Shriek_sheet.png");
        WriteSheet(Path.Combine(ShadeSkinManager.SkinsFolderName, "Cozy Shade"), "Shade_Idle_Sheet.png");
        ShadeSkinManager.Reload(root);

        var cozy = ShadeSkinManager.Skins.Single(s => s.Id == "Cozy Shade");
        Assert.Equal(
            Path.Combine(root, "Abyss_Shriek_sheet.png"),
            ShadeSkinManager.ResolveSpritePath(cozy, "Abyss_Shriek_sheet.png"));
    }

    [Fact]
    public void DefaultSkinAlwaysResolvesToTheBuiltInSet()
    {
        WriteSheet(string.Empty, "Shade_Idle_Sheet.png");
        WriteSheet(Path.Combine(ShadeSkinManager.SkinsFolderName, "Cozy Shade"), "Shade_Idle_Sheet.png");
        ShadeSkinManager.Reload(root);

        var builtIn = ShadeSkinManager.Skins.Single(s => s.IsDefault);
        Assert.Equal(
            Path.Combine(root, "Shade_Idle_Sheet.png"),
            ShadeSkinManager.ResolveSpritePath(builtIn, "Shade_Idle_Sheet.png"));
    }

    [Fact]
    public void SelectSkinPersistsTheChoiceAndIsCaseInsensitive()
    {
        Directory.CreateDirectory(SkinDir("Cozy Shade"));
        ShadeSkinManager.Reload(root);

        Assert.True(ShadeSkinManager.SelectSkin("cozy shade"));
        Assert.Equal("Cozy Shade", ModConfig.Instance.shadeSkin);
        Assert.Equal("Cozy Shade", ShadeSkinManager.SelectedSkinId);

        // Re-selecting the active skin is a no-op so callers can skip the sprite reload.
        Assert.False(ShadeSkinManager.SelectSkin("Cozy Shade"));
    }

    [Fact]
    public void SelectSkinRejectsUnknownIds()
    {
        Directory.CreateDirectory(SkinDir("Cozy Shade"));
        ShadeSkinManager.Reload(root);
        ShadeSkinManager.SelectSkin("Cozy Shade");

        Assert.False(ShadeSkinManager.SelectSkin("Not A Skin"));
        Assert.Equal("Cozy Shade", ShadeSkinManager.SelectedSkinId);
    }

    [Fact]
    public void ConfiguredSkinThatNoLongerExistsFallsBackToDefault()
    {
        ShadeSkinManager.Reload(root);
        ModConfig.Instance.shadeSkin = "Deleted Skin";

        Assert.Equal(ShadeSkinManager.DefaultSkinId, ShadeSkinManager.SelectedSkinId);
    }
}
