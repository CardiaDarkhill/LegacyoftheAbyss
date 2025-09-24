#nullable enable

using System;
using System.Linq;
using Xunit;

namespace LegacyoftheAbyss.Tests
{
    public sealed class ShadeCharmPlacementDatabaseTests
    {
        [Fact]
        public void LoadsBoneBottomPlacementsFromJson()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene("BoneBottom");

            Assert.NotNull(placements);
            Assert.Equal(11, placements.Count);

            var soulEater = placements.First(p => p.CharmId == Shade.ShadeCharmId.SoulEater);
            Assert.NotNull(soulEater.AnchorOffset);
            Assert.Equal(2.2f, soulEater.AnchorOffset!.X, 3);
            Assert.Equal(0.6f, soulEater.AnchorOffset.Y, 3);
        }

        [Fact]
        public void LoadsMossGrottoPlacementsFromJson()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene("MossGrotto");

            Assert.NotNull(placements);
            Assert.Equal(11, placements.Count);

            var kingsoul = placements.First(p => p.CharmId == Shade.ShadeCharmId.Kingsoul);
            Assert.NotNull(kingsoul.AnchorOffset);
            Assert.Equal(1.5f, kingsoul.AnchorOffset!.X, 3);
            Assert.Equal(-2.0f, kingsoul.AnchorOffset.Y, 3);
        }

        [Fact]
        public void ReturnsEmptyWhenSceneNameMissing()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene(null);
            Assert.Empty(placements);
        }
    }
}
