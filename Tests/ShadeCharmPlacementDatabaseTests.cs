#nullable enable

using System;
using System.Linq;
using Xunit;

namespace LegacyoftheAbyss.Tests
{
    /// <summary>
    /// These assert the *shape* of the placement lookup rather than exact counts. Earlier versions
    /// pinned "BoneBottom has exactly 10 anchors" style numbers, which meant every edit to
    /// Assets/charm_placements.json broke the suite for no good reason and the failures got ignored.
    /// The behaviour actually worth locking down is the matching rules: exact scene names, token
    /// matching via sceneContainsAll, and unscoped entries (shop listings / boss drops) that are
    /// deliberately offered to every scene because they are resolved by shop owner or enemy name
    /// at runtime instead.
    /// </summary>
    public sealed class ShadeCharmPlacementDatabaseTests
    {
        private static bool IsSceneScoped(Shade.ShadeCharmPlacementDefinition placement)
            => !string.IsNullOrWhiteSpace(placement.SceneName)
               || (placement.SceneContainsAll != null && placement.SceneContainsAll.Length > 0);

        [Fact]
        public void UnscopedPlacementsAreOfferedToEveryScene()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();

            var unscoped = Shade.ShadeCharmPlacementDatabase.GetAllPlacements()
                .Where(p => !IsSceneScoped(p))
                .ToList();

            Assert.NotEmpty(unscoped);
            Assert.All(unscoped, p => Assert.Contains(
                p.PlacementKind,
                new[] { Shade.ShadeCharmPlacementKind.ShopListing, Shade.ShadeCharmPlacementKind.BossDrop }));

            foreach (var sceneName in new[] { "Tut_01", "BoneBottom", "MossGrotto" })
            {
                var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene(sceneName);
                foreach (var global in unscoped)
                {
                    Assert.Contains(global, placements);
                }
            }
        }

        [Fact]
        public void LoadsBoneBottomShopListingFromJson()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene("BoneBottom");

            Assert.NotNull(placements);

            var anchors = placements
                .Where(p => p.PlacementKind == Shade.ShadeCharmPlacementKind.GroundAnchor)
                .ToList();
            Assert.NotEmpty(anchors);
            Assert.All(anchors, p => Assert.NotNull(p.AnchorOffset));

            var stalwart = anchors.First(p => p.CharmId == Shade.ShadeCharmId.StalwartShell);
            Assert.Equal(1.7f, stalwart.AnchorOffset!.X, 3);
            Assert.Equal(-1.1f, stalwart.AnchorOffset.Y, 3);

            var soulCatcherListing = placements.First(p =>
                p.PlacementKind == Shade.ShadeCharmPlacementKind.ShopListing
                && p.CharmId == Shade.ShadeCharmId.SoulCatcher
                && p.Shop?.OwnerNameContainsAll != null
                && p.Shop.OwnerNameContainsAll.Contains("bone", StringComparer.OrdinalIgnoreCase));

            Assert.Equal(150, soulCatcherListing.Shop!.GeoCost);
            Assert.NotNull(soulCatcherListing.Shop.StockContainsAnyPlayerDataBools);
            Assert.Contains("PurchasedBonebottomFaithToken", soulCatcherListing.Shop.StockContainsAnyPlayerDataBools!);
            Assert.NotNull(soulCatcherListing.Shop.RequireNotCollected);
            Assert.Contains(Shade.ShadeCharmId.VoidHeart, soulCatcherListing.Shop.RequireNotCollected!);
        }

        [Fact]
        public void LoadsMossGrottoAnchorsFromJson()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene("MossGrotto");

            var anchors = placements
                .Where(p => p.PlacementKind == Shade.ShadeCharmPlacementKind.GroundAnchor)
                .ToList();

            Assert.NotEmpty(anchors);
            Assert.All(anchors, p => Assert.NotNull(p.AnchorOffset));

            var voidHeart = anchors.First(p => p.CharmId == Shade.ShadeCharmId.VoidHeart);
            Assert.Equal(0.2f, voidHeart.AnchorOffset!.X, 3);
            Assert.Equal(2.7f, voidHeart.AnchorOffset.Y, 3);
        }

        [Fact]
        public void SceneTokenPlacementsDoNotLeakIntoUnrelatedScenes()
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene("MossGrotto");

            Assert.DoesNotContain(placements, p =>
                p.SceneContainsAll != null
                && p.SceneContainsAll.Contains("bottom", StringComparer.OrdinalIgnoreCase));
        }

        [Theory]
        [InlineData("Tut_04", Shade.ShadeCharmId.SoulEater, 74.772f, 8.587f, 0.004f)]
        [InlineData("Tut_01", Shade.ShadeCharmId.FuryOfTheFallen, 101.005f, 16.568f, 0.004f)]
        [InlineData("Bonetown", Shade.ShadeCharmId.WaywardCompass, 301.342f, 24.568f, 0.004f)]
        public void LoadsExactSceneWorldPlacement(string sceneName, Shade.ShadeCharmId charmId, float x, float y, float z)
        {
            Shade.ShadeCharmPlacementDatabase.Reload();
            var placements = Shade.ShadeCharmPlacementDatabase.GetPlacementsForScene(sceneName);

            var sceneScoped = placements.Where(IsSceneScoped).ToList();
            var placement = Assert.Single(sceneScoped);

            Assert.Equal(Shade.ShadeCharmPlacementKind.Ground, placement.PlacementKind);
            Assert.Equal(charmId, placement.CharmId);
            Assert.NotNull(placement.WorldPosition);
            Assert.Equal(x, placement.WorldPosition!.X, 3);
            Assert.Equal(y, placement.WorldPosition.Y, 3);
            Assert.Equal(z, placement.WorldPosition.Z, 3);
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
