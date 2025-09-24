#nullable enable

using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed class ShopPlacementHandler : IShadeCharmPlacementHandler
    {
        internal static ShopPlacementHandler? Instance { get; private set; }

        private readonly List<ShadeCharmPlacementDefinition> _activePlacements = new();
        private readonly Dictionary<SimpleShopMenuOwner, ShopStockInfo> _activeStock = new();
        private string _sceneName = string.Empty;

        internal ShopPlacementHandler()
        {
            Instance = this;
        }

        public void Populate(in ShadeCharmPlacementContext context, IReadOnlyList<ShadeCharmPlacementDefinition> placements)
        {
            _sceneName = context.SceneName;
            _activePlacements.Clear();
            _activeStock.Clear();

            if (placements == null || placements.Count == 0)
            {
                return;
            }

            foreach (var placement in placements)
            {
                if (placement.Shop == null)
                {
                    continue;
                }

                _activePlacements.Add(placement);
            }
        }

        internal void AugmentStock(SimpleShopMenuOwner owner, List<ISimpleShopItem> stock)
        {
            if (owner == null || stock == null)
            {
                return;
            }

            if (_activePlacements.Count == 0)
            {
                _activeStock.Remove(owner);
                return;
            }

            var matches = new List<ShadeCharmPlacementDefinition>();
            foreach (var placement in _activePlacements)
            {
                if (!MatchesOwner(placement, owner))
                {
                    continue;
                }

                if (ShadeCharmPlacementService.IsCharmAlreadyCollected(placement.CharmId))
                {
                    continue;
                }

                matches.Add(placement);
            }

            if (matches.Count == 0)
            {
                _activeStock.Remove(owner);
                return;
            }

            int startIndex = stock.Count;
            var added = new List<ShadeCharmShopItem?>(matches.Count);
            foreach (var definition in matches)
            {
                var item = new ShadeCharmShopItem(definition);
                added.Add(item);
                stock.Add(item);
            }

            _activeStock[owner] = new ShopStockInfo(startIndex, added);
            ShadeCharmPlacementService.LogInfo($"Injected {added.Count} charm shop item(s) for '{owner.name}' in scene '{_sceneName}'.");
        }

        internal bool TryHandlePurchase(SimpleShopMenuOwner owner, int purchaseIndex)
        {
            if (owner == null)
            {
                return false;
            }

            if (!_activeStock.TryGetValue(owner, out var info))
            {
                return false;
            }

            if (info.Items.Count == 0)
            {
                _activeStock.Remove(owner);
                return false;
            }

            if (purchaseIndex < info.StartIndex)
            {
                return false;
            }

            int localIndex = purchaseIndex - info.StartIndex;
            if (localIndex < 0 || localIndex >= info.Items.Count)
            {
                return false;
            }

            ShadeCharmShopItem? item = info.Items[localIndex];
            if (item == null)
            {
                return true;
            }

            if (!item.Grant())
            {
                return true;
            }

            info.Items[localIndex] = null;

            bool anyRemaining = false;
            foreach (ShadeCharmShopItem? remaining in info.Items)
            {
                if (remaining != null)
                {
                    anyRemaining = true;
                    break;
                }
            }

            if (!anyRemaining)
            {
                _activeStock.Remove(owner);
            }

            return true;
        }

        private static bool MatchesOwner(ShadeCharmPlacementDefinition placement, SimpleShopMenuOwner owner)
        {
            var shop = placement.Shop;
            if (shop == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(shop.OwnerPath))
            {
                string currentPath = ShadeCharmPlacementHelpers.GetHierarchyPath(owner.transform);
                if (!string.Equals(currentPath, shop.OwnerPath, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(shop.OwnerName))
            {
                if (!string.Equals(owner.name, shop.OwnerName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(shop.OwnerPath) && string.IsNullOrWhiteSpace(shop.OwnerName))
            {
                ShadeCharmPlacementService.LogWarning($"Shop placement for charm {placement.CharmId} in scene '{Instance?._sceneName}' does not specify an ownerPath or ownerName; skipping.");
                return false;
            }

            return true;
        }

        private sealed class ShopStockInfo
        {
            internal ShopStockInfo(int startIndex, List<ShadeCharmShopItem?> items)
            {
                StartIndex = startIndex;
                Items = items;
            }

            internal int StartIndex { get; }

            internal List<ShadeCharmShopItem?> Items { get; }
        }

        private sealed class ShadeCharmShopItem : ISimpleShopItem
        {
            private readonly ShadeCharmPlacementDefinition _definition;
            private readonly ShadeCharmPlacementShopData _shopData;

            internal ShadeCharmShopItem(ShadeCharmPlacementDefinition definition)
            {
                _definition = definition;
                _shopData = definition.Shop ?? new ShadeCharmPlacementShopData();
            }

            public string GetDisplayName()
            {
                try
                {
                    if (ShadeRuntime.Charms is { } inventory)
                    {
                        var def = inventory.GetDefinition(_definition.CharmId);
                        if (def != null)
                        {
                            return def.DisplayName;
                        }
                    }
                }
                catch
                {
                }

                return _definition.CharmId.ToString();
            }

            public Sprite GetIcon()
            {
                try
                {
                    return ShadeCharmSavedItem.ResolveCharmSprite(_definition.CharmId, out _);
                }
                catch
                {
                    return ShadeCharmSavedItem.ResolveCharmSprite(_definition.CharmId, out _);
                }
            }

            public int GetCost()
            {
                return Math.Max(0, _shopData.GeoCost ?? 0);
            }

            public bool DelayPurchase()
            {
                return _shopData.DelayPurchase ?? false;
            }

            internal bool Grant()
            {
                try
                {
                    if (ShadeCharmPlacementService.IsCharmAlreadyCollected(_definition.CharmId))
                    {
                        return true;
                    }

                    var savedItem = ShadeCharmSavedItem.Create(_definition.CharmId);
                    try
                    {
                        savedItem.Get();
                    }
                    finally
                    {
                        if (Application.isPlaying)
                        {
                            UnityEngine.Object.Destroy(savedItem);
                        }
                        else
                        {
                            UnityEngine.Object.DestroyImmediate(savedItem);
                        }
                    }

                    ShadeCharmPlacementService.LogInfo($"Charm {_definition.CharmId} purchased from shop.");
                    return true;
                }
                catch (Exception ex)
                {
                    ShadeCharmPlacementService.LogWarning($"Failed to grant charm {_definition.CharmId} from shop: {ex}");
                    return false;
                }
            }
        }

        [HarmonyPatch(typeof(SimpleShopMenu), nameof(SimpleShopMenu.SetStock))]
        private static class SimpleShopMenu_SetStock_Patch
        {
            private static void Prefix(SimpleShopMenuOwner newOwner, List<ISimpleShopItem> newShopItems)
            {
                var handler = Instance;
                if (handler == null || newOwner == null || newShopItems == null)
                {
                    return;
                }

                handler.AugmentStock(newOwner, newShopItems);
            }
        }

        [HarmonyPatch(typeof(SimpleShopMenuOwner), nameof(SimpleShopMenuOwner.ClosedMenu))]
        private static class SimpleShopMenuOwner_ClosedMenu_Patch
        {
            private static void Prefix(SimpleShopMenuOwner __instance, bool didPurchase, ref int purchaseIndex)
            {
                if (!didPurchase)
                {
                    return;
                }

                var handler = Instance;
                if (handler == null)
                {
                    return;
                }

                if (handler.TryHandlePurchase(__instance, purchaseIndex))
                {
                    purchaseIndex = -1;
                }
            }
        }

        [HarmonyPatch(typeof(SimpleShopMenuOwner), nameof(SimpleShopMenuOwner.PurchaseNoClose))]
        private static class SimpleShopMenuOwner_PurchaseNoClose_Patch
        {
            private static bool Prefix(SimpleShopMenuOwner __instance, ref int purchaseIndex)
            {
                var handler = Instance;
                if (handler == null)
                {
                    return true;
                }

                if (!handler.TryHandlePurchase(__instance, purchaseIndex))
                {
                    return true;
                }

                purchaseIndex = -1;
                return false;
            }
        }
    }
}
