#nullable enable

using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal static class ShadeCharmPlacer
    {
        private static bool s_registered;

        public static void PopulateScene(string? sceneName, Transform? heroTransform)
        {
            EnsureRegistered();
            ShadeCharmPlacementService.PopulateScene(sceneName, heroTransform);
        }

        private static void EnsureRegistered()
        {
            if (s_registered)
            {
                return;
            }

            ShadeCharmPlacementService.RegisterHandler(new GroundPlacementHandler(), ShadeCharmPlacementKind.Ground, ShadeCharmPlacementKind.GroundAnchor);
            ShadeCharmPlacementService.RegisterHandler(new ContainerPlacementHandler(), ShadeCharmPlacementKind.Container);
            ShadeCharmPlacementService.RegisterHandler(new ShopPlacementHandler(), ShadeCharmPlacementKind.ShopListing);
            ShadeCharmPlacementService.RegisterHandler(new BossDropPlacementHandler(), ShadeCharmPlacementKind.BossDrop);
            s_registered = true;
        }
    }
}
