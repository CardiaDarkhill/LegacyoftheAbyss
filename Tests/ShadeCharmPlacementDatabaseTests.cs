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
            Assert.Equal(10, placements.Count);

            var grubsong = placements.First(p => p.CharmId == Shade.ShadeCharmId.Grubsong);
            Assert.NotNull(grubsong.AnchorOffset);
            Assert.Equal(-1.6f, grubsong.AnchorOffset!.X, 3);
            Assert.Equal(0.8f, grubsong.AnchorOffset.Y, 3);
        }

        [Fact]
        public void LoadsMossGrottoPlacementsFromJson()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene("MossGrotto");

            Assert.NotNull(placements);
            Assert.Equal(10, placements.Count);

            var kingsoul = placements.First(p => p.CharmId == Shade.ShadeCharmId.Kingsoul);
            Assert.NotNull(kingsoul.AnchorOffset);
            Assert.Equal(1.5f, kingsoul.AnchorOffset!.X, 3);
            Assert.Equal(-2.0f, kingsoul.AnchorOffset.Y, 3);
        }

        [Fact]
        public void LoadsTut04WorldPlacement()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene("Tut_04");

            Assert.Single(placements);

            var soulEater = placements.First();
            Assert.Equal(Shade.ShadeCharmId.SoulEater, soulEater.CharmId);
            Assert.NotNull(soulEater.WorldPosition);
            Assert.Equal(74.772f, soulEater.WorldPosition!.X, 3);
            Assert.Equal(8.587f, soulEater.WorldPosition.Y, 3);
            Assert.Equal(0.004f, soulEater.WorldPosition.Z, 3);
        }

        [Fact]
        public void LoadsTut01WorldPlacement()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene("Tut_01");

            Assert.Single(placements);

            var fury = placements.First();
            Assert.Equal(Shade.ShadeCharmId.FuryOfTheFallen, fury.CharmId);
            Assert.NotNull(fury.WorldPosition);
            Assert.Equal(82.005f, fury.WorldPosition!.X, 3);
            Assert.Equal(9.568f, fury.WorldPosition.Y, 3);
            Assert.Equal(0.004f, fury.WorldPosition.Z, 3);
        }

        [Fact]
        public void LoadsBonetownWorldPlacement()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene("Bonetown");

            Assert.Single(placements);

            var compass = placements.First();
            Assert.Equal(Shade.ShadeCharmId.WaywardCompass, compass.CharmId);
            Assert.NotNull(compass.WorldPosition);
            Assert.Equal(301.342f, compass.WorldPosition!.X, 3);
            Assert.Equal(24.568f, compass.WorldPosition.Y, 3);
            Assert.Equal(0.004f, compass.WorldPosition.Z, 3);
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
