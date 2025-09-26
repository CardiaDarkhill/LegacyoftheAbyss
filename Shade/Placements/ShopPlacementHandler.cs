#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GlobalEnums;
using HarmonyLib;
using TeamCherry.Localization;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed class ShopPlacementHandler : IShadeCharmPlacementHandler
    {
        internal static ShopPlacementHandler? Instance { get; private set; }

        private static readonly FieldInfo? ShopOwnerTitleField = AccessTools.Field(typeof(ShopOwnerBase), "shopTitle");
        private static readonly FieldInfo? ShopItemPlayerDataBoolField = AccessTools.Field(typeof(ShopItem), "playerDataBoolName");
        private static readonly FieldInfo? ShopItemExtraPlayerDataBoolsField = AccessTools.Field(typeof(ShopItem), "setExtraPlayerDataBools");

        private readonly List<ShadeCharmPlacementDefinition> _activePlacements = new();
        private readonly Dictionary<SimpleShopMenuOwner, ShopStockInfo> _activeStock = new();
        private readonly Dictionary<ShopOwnerBase, ShopOwnerStockInfo> _activeOwnerStock = new();
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
            foreach (var info in _activeOwnerStock.Values)
            {
                info.Dispose();
            }
            _activeOwnerStock.Clear();

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
                if (!MatchesOwner(placement, owner, out var failureReason))
                {
                    LogPlacementSkip(owner, placement, failureReason);
                    continue;
                }

                if (!MeetsShopConditions(placement, out var conditionFailure))
                {
                    LogPlacementSkip(owner, placement, conditionFailure);
                    continue;
                }

                if (ShadeCharmPlacementService.IsCharmAlreadyCollected(placement.CharmId))
                {
                    LogPlacementSkip(owner, placement, "charm already collected");
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

        private void LogPlacementSkip(Component owner, ShadeCharmPlacementDefinition placement, string? reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                return;
            }

            try
            {
                string ownerName = owner?.name ?? "<null>";
                string hierarchyPath = owner != null ? ShadeCharmPlacementHelpers.GetHierarchyPath(owner.transform) : string.Empty;
                if (!string.IsNullOrWhiteSpace(hierarchyPath))
                {
                    ownerName = $"{ownerName} ({hierarchyPath})";
                }

                ShadeCharmPlacementService.LogInfo($"Skipped injecting charm {placement.CharmId} for '{ownerName}' in scene '{_sceneName}': {reason}.");
            }
            catch
            {
            }
        }

        private static string DescribeStockPlayerDataBools(ShopItem[]? stock)
        {
            if (stock == null)
            {
                return string.Empty;
            }

            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in stock)
            {
                if (item == null)
                {
                    continue;
                }

                string primary = GetPlayerDataBoolName(item);
                if (!string.IsNullOrWhiteSpace(primary))
                {
                    names.Add(primary);
                }

                foreach (string extra in GetExtraPlayerDataBools(item))
                {
                    if (!string.IsNullOrWhiteSpace(extra))
                    {
                        names.Add(extra);
                    }
                }
            }

            return names.Count == 0 ? string.Empty : string.Join(", ", names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase));
        }

        internal void AugmentStock(ShopOwnerBase owner, ref ShopItem[] stock)
        {
            if (owner == null)
            {
                return;
            }

            if (_activePlacements.Count == 0)
            {
                if (_activeOwnerStock.Remove(owner, out var stale))
                {
                    stale.Dispose();
                }

                return;
            }

            var matches = new List<ShadeCharmPlacementDefinition>();
            foreach (var placement in _activePlacements)
            {
                if (!MatchesOwner(placement, owner, stock, out var failureReason))
                {
                    LogPlacementSkip(owner, placement, failureReason);
                    continue;
                }

                if (!MeetsShopConditions(placement, out var conditionFailure))
                {
                    LogPlacementSkip(owner, placement, conditionFailure);
                    continue;
                }

                if (ShadeCharmPlacementService.IsCharmAlreadyCollected(placement.CharmId))
                {
                    LogPlacementSkip(owner, placement, "charm already collected");
                    continue;
                }

                matches.Add(placement);
            }

            if (matches.Count == 0)
            {
                if (_activeOwnerStock.Remove(owner, out var stale))
                {
                    stale.Dispose();
                }

                return;
            }

            if (!_activeOwnerStock.TryGetValue(owner, out var info))
            {
                info = new ShopOwnerStockInfo(this);
                _activeOwnerStock[owner] = info;
            }

            var items = info.ResolveItems(matches);
            if (items.Count == 0)
            {
                if (_activeOwnerStock.Remove(owner, out var stale))
                {
                    stale.Dispose();
                }

                return;
            }

            int baseCount = stock?.Length ?? 0;
            var combined = new ShopItem[baseCount + items.Count];
            if (baseCount > 0 && stock != null)
            {
                Array.Copy(stock, combined, baseCount);
            }

            for (int i = 0; i < items.Count; i++)
            {
                combined[baseCount + i] = items[i];
            }

            stock = combined;

            if (!info.HasLogged)
            {
                ShadeCharmPlacementService.LogInfo($"Injected {items.Count} charm shop item(s) for '{owner.name}' in scene '{_sceneName}'.");
                info.HasLogged = true;
            }
        }

        private static bool MatchesOwner(ShadeCharmPlacementDefinition placement, Component owner, out string? failureReason)
            => MatchesOwner(placement, owner, null, out failureReason);

        private static bool MatchesOwner(ShadeCharmPlacementDefinition placement, Component owner, ShopItem[]? existingStock, out string? failureReason)
        {
            failureReason = null;

            var shop = placement.Shop;
            if (shop == null)
            {
                failureReason = "no shop metadata";
                return false;
            }

            bool hasOwnerIdentifier =
                !string.IsNullOrWhiteSpace(shop.OwnerPath) ||
                !string.IsNullOrWhiteSpace(shop.OwnerName) ||
                (shop.OwnerNameContainsAll != null && shop.OwnerNameContainsAll.Length > 0) ||
                (shop.StockContainsAnyPlayerDataBools != null && shop.StockContainsAnyPlayerDataBools.Length > 0);

            if (!string.IsNullOrWhiteSpace(shop.OwnerPath))
            {
                string currentPath = ShadeCharmPlacementHelpers.GetHierarchyPath(owner.transform);
                if (!string.Equals(currentPath, shop.OwnerPath, StringComparison.OrdinalIgnoreCase))
                {
                    failureReason = $"owner path '{currentPath}' did not match required '{shop.OwnerPath}'";
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(shop.OwnerName))
            {
                if (!string.Equals(owner.name, shop.OwnerName, StringComparison.OrdinalIgnoreCase))
                {
                    failureReason = $"owner name '{owner.name}' did not match required '{shop.OwnerName}'";
                    return false;
                }
            }

            if (shop.OwnerNameContainsAll is { Length: > 0 })
            {
                string ownerTokens = BuildOwnerTokenString(owner);
                foreach (string token in shop.OwnerNameContainsAll)
                {
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        continue;
                    }

                    if (ownerTokens.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        failureReason = $"owner tokens '{ownerTokens}' missing required token '{token}'";
                        return false;
                    }
                }
            }

            if (shop.StockContainsAnyPlayerDataBools is { Length: > 0 })
            {
                if (existingStock == null)
                {
                    failureReason = "existing stock unavailable while purchase flag hints provided";
                    return false;
                }

                if (!StockContainsAnyPlayerDataBool(existingStock, shop.StockContainsAnyPlayerDataBools))
                {
                    failureReason =
                        $"existing stock did not expose required purchase flags ({string.Join(", ", shop.StockContainsAnyPlayerDataBools ?? Array.Empty<string>())}); found [{DescribeStockPlayerDataBools(existingStock)}]";
                    return false;
                }
            }

            if (!hasOwnerIdentifier)
            {
                ShadeCharmPlacementService.LogWarning($"Shop placement for charm {placement.CharmId} in scene '{Instance?._sceneName}' does not specify any owner matching data; skipping.");
                failureReason = "no owner identifiers configured";
                return false;
            }

            return true;
        }

        private static string BuildOwnerTokenString(Component owner)
        {
            string ownerName = owner.name ?? string.Empty;
            string typeName = owner.GetType().FullName ?? owner.GetType().Name;
            string hierarchyPath = ShadeCharmPlacementHelpers.GetHierarchyPath(owner.transform) ?? string.Empty;
            string sceneName = string.Empty;
            string scenePath = string.Empty;

            try
            {
                var scene = owner.gameObject.scene;
                sceneName = scene.name ?? string.Empty;
                scenePath = scene.path ?? string.Empty;
            }
            catch
            {
            }

            string title = string.Empty;
            try
            {
                if (owner is SimpleShopMenuOwner menuOwner)
                {
                    title = menuOwner.ShopTitle ?? string.Empty;
                }
                else if (ShopOwnerTitleField != null && owner is ShopOwnerBase)
                {
                    if (ShopOwnerTitleField.GetValue(owner) is LocalisedString localised)
                    {
                        title = $"{localised.Sheet}:{localised.Key}";
                    }
                }
            }
            catch
            {
            }

            string rootName = string.Empty;
            try
            {
                rootName = owner.transform?.root?.name ?? string.Empty;
            }
            catch
            {
            }

            return string.Join("|", new[]
            {
                ownerName,
                typeName,
                hierarchyPath,
                sceneName,
                scenePath,
                title,
                rootName
            }.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        private static bool StockContainsAnyPlayerDataBool(ShopItem[]? stock, string[]? boolNames)
        {
            if (stock == null || boolNames == null || boolNames.Length == 0)
            {
                return false;
            }

            foreach (string candidate in boolNames)
            {
                if (string.IsNullOrWhiteSpace(candidate))
                {
                    continue;
                }

                if (StockContainsPlayerDataBool(stock, candidate))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool StockContainsPlayerDataBool(ShopItem[] stock, string boolName)
        {
            foreach (var item in stock)
            {
                if (item == null)
                {
                    continue;
                }

                string primary = GetPlayerDataBoolName(item);
                if (!string.IsNullOrWhiteSpace(primary) &&
                    string.Equals(primary, boolName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                foreach (string extra in GetExtraPlayerDataBools(item))
                {
                    if (string.Equals(extra, boolName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string GetPlayerDataBoolName(ShopItem item)
        {
            if (ShopItemPlayerDataBoolField == null || item == null)
            {
                return string.Empty;
            }

            try
            {
                return ShopItemPlayerDataBoolField.GetValue(item) as string ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static IEnumerable<string> GetExtraPlayerDataBools(ShopItem item)
        {
            if (ShopItemExtraPlayerDataBoolsField == null || item == null)
            {
                yield break;
            }

            string[]? values = null;
            try
            {
                values = ShopItemExtraPlayerDataBoolsField.GetValue(item) as string[];
            }
            catch
            {
            }

            if (values == null)
            {
                yield break;
            }

            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    yield return value;
                }
            }
        }

        private static bool MeetsShopConditions(ShadeCharmPlacementDefinition placement, out string? failureReason)
        {
            failureReason = null;

            var shop = placement.Shop;
            if (shop == null)
            {
                return true;
            }

            if (shop.RequireCollected is { Length: > 0 })
            {
                foreach (var required in shop.RequireCollected)
                {
                    if (!ShadeCharmPlacementService.IsCharmAlreadyCollected(required))
                    {
                        failureReason = $"required charm {required} has not been collected";
                        return false;
                    }
                }
            }

            if (shop.RequireNotCollected is { Length: > 0 })
            {
                foreach (var forbidden in shop.RequireNotCollected)
                {
                    if (ShadeCharmPlacementService.IsCharmAlreadyCollected(forbidden))
                    {
                        failureReason = $"forbidden charm {forbidden} was already collected";
                        return false;
                    }
                }
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

        private sealed class ShopOwnerStockInfo : IDisposable
        {
            private readonly ShopPlacementHandler _handler;
            private readonly Dictionary<ShadeCharmPlacementDefinition, ShopOwnerItemEntry> _items = new();

            internal ShopOwnerStockInfo(ShopPlacementHandler handler)
            {
                _handler = handler;
            }

            internal bool HasLogged { get; set; }

            internal List<ShopItem> ResolveItems(IReadOnlyList<ShadeCharmPlacementDefinition> definitions)
            {
                var result = new List<ShopItem>(definitions.Count);
                var remaining = new HashSet<ShadeCharmPlacementDefinition>(_items.Keys);

                foreach (var definition in definitions)
                {
                    if (definition == null)
                    {
                        continue;
                    }

                    remaining.Remove(definition);

                    if (!_items.TryGetValue(definition, out var entry) || entry == null)
                    {
                        entry = _handler.CreateOwnerItem(definition);
                        if (entry == null)
                        {
                            continue;
                        }

                        _items[definition] = entry;
                    }

                    if (entry.Item != null)
                    {
                        result.Add(entry.Item);
                    }
                }

                foreach (var stale in remaining)
                {
                    if (_items.TryGetValue(stale, out var entry))
                    {
                        entry.Dispose();
                        _items.Remove(stale);
                    }
                }

                return result;
            }

            public void Dispose()
            {
                foreach (var entry in _items.Values)
                {
                    entry.Dispose();
                }

                _items.Clear();
            }
        }

        private sealed class ShopOwnerItemEntry : IDisposable
        {
            internal ShopOwnerItemEntry(ShopItem item, ShadeCharmSavedItem savedItem)
            {
                Item = item;
                SavedItem = savedItem;
            }

            internal ShopItem Item { get; }

            internal ShadeCharmSavedItem SavedItem { get; }

            public void Dispose()
            {
                try
                {
                    if (SavedItem != null)
                    {
                        if (Application.isPlaying)
                        {
                            UnityEngine.Object.Destroy(SavedItem);
                        }
                        else
                        {
                            UnityEngine.Object.DestroyImmediate(SavedItem);
                        }
                    }
                }
                catch
                {
                }

                try
                {
                    if (Item != null)
                    {
                        if (Application.isPlaying)
                        {
                            UnityEngine.Object.Destroy(Item);
                        }
                        else
                        {
                            UnityEngine.Object.DestroyImmediate(Item);
                        }
                    }
                }
                catch
                {
                }
            }
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

        private ShopOwnerItemEntry? CreateOwnerItem(ShadeCharmPlacementDefinition definition)
        {
            try
            {
                var shopData = definition.Shop ?? new ShadeCharmPlacementShopData();
                var item = ShopItem.CreateTemp($"ShadeCharm_{definition.CharmId}_ShopItem");
                item.hideFlags = HideFlags.HideAndDontSave;

                ShadeCharmSavedItem savedItem = ShadeCharmSavedItem.Create(definition.CharmId);
                savedItem.hideFlags = HideFlags.HideAndDontSave;

                var definitionInventory = ShadeRuntime.Charms;
                ShadeCharmDefinition? charmDefinition = definitionInventory?.GetDefinition(definition.CharmId);

                string displayName = charmDefinition?.DisplayName ?? definition.CharmId.ToString();
                string description = charmDefinition?.Description ?? string.Empty;
                var localisedName = new LocalisedString("LegacyAbyss", displayName);
                var localisedDescription = new LocalisedString("LegacyAbyss", description);

                AccessTools.Field(typeof(ShopItem), "displayName").SetValue(item, localisedName);
                AccessTools.Field(typeof(ShopItem), "description").SetValue(item, localisedDescription);
                AccessTools.Field(typeof(ShopItem), "descriptionMultiple").SetValue(item, localisedDescription);

                var sprite = ShadeCharmSavedItem.ResolveCharmSprite(definition.CharmId, out _);
                AccessTools.Field(typeof(ShopItem), "itemSprite").SetValue(item, sprite);

                AccessTools.Field(typeof(ShopItem), "currencyType").SetValue(item, CurrencyType.Money);
                AccessTools.Field(typeof(ShopItem), "cost").SetValue(item, Math.Max(0, shopData.GeoCost ?? 0));
                AccessTools.Field(typeof(ShopItem), "savedItem").SetValue(item, savedItem);

                return new ShopOwnerItemEntry(item, savedItem);
            }
            catch (Exception ex)
            {
                ShadeCharmPlacementService.LogWarning($"Failed to create shop item for charm {definition.CharmId}: {ex}");
                return null;
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

        [HarmonyPatch(typeof(ShopOwner), nameof(ShopOwner.Stock), MethodType.Getter)]
        private static class ShopOwner_get_Stock_Patch
        {
            private static void Postfix(ShopOwner __instance, ref ShopItem[] __result)
            {
                var handler = Instance;
                if (handler == null || __instance == null)
                {
                    return;
                }

                var stock = __result;
                handler.AugmentStock(__instance, ref stock);
                __result = stock;
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
