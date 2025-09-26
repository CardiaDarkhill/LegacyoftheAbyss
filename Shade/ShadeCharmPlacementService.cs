#nullable enable

using System;
using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal static class ShadeCharmPlacementService
    {
        private static readonly Dictionary<ShadeCharmPlacementKind, IShadeCharmPlacementHandler> Handlers = new();
        private static readonly List<IShadeCharmPlacementHandler> UniqueHandlers = new();
        private static readonly ManualLogSource PlacementLogger = BepInEx.Logging.Logger.CreateLogSource("ShadeCharmPlacement");
        private static bool s_initialized;

        internal static void RegisterHandler(IShadeCharmPlacementHandler handler, params ShadeCharmPlacementKind[] kinds)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (kinds == null || kinds.Length == 0)
            {
                throw new ArgumentException("At least one placement kind must be provided.", nameof(kinds));
            }

            foreach (var kind in kinds)
            {
                Handlers[kind] = handler;
            }

            if (!UniqueHandlers.Contains(handler))
            {
                UniqueHandlers.Add(handler);
            }
        }

        internal static void PopulateScene(string? sceneName, Transform? heroTransform)
        {
            EnsureInitialized();

            if (string.IsNullOrWhiteSpace(sceneName) || heroTransform == null)
            {
                return;
            }

            var placements = ShadeCharmPlacementDatabase.GetPlacementsForScene(sceneName);
            var context = new ShadeCharmPlacementContext(sceneName, heroTransform);
            var grouped = new Dictionary<IShadeCharmPlacementHandler, List<ShadeCharmPlacementDefinition>>();

            foreach (var placement in placements)
            {
                if (placement == null)
                {
                    continue;
                }

                if (IsPlacementAlreadySatisfied(placement))
                {
                    LogInfo($"Skipping {DescribePlacement(placement)} in scene '{sceneName}' because it is already collected.");
                    continue;
                }

                if (!Handlers.TryGetValue(placement.PlacementKind, out var handler))
                {
                    LogWarning($"No handler registered for placement kind {placement.PlacementKind} ({DescribePlacement(placement)}).");
                    continue;
                }

                if (!grouped.TryGetValue(handler, out var list))
                {
                    list = new List<ShadeCharmPlacementDefinition>();
                    grouped[handler] = list;
                }

                list.Add(placement);
            }

            if (UniqueHandlers.Count == 0)
            {
                return;
            }

            foreach (var handler in UniqueHandlers)
            {
                grouped.TryGetValue(handler, out var definitions);
                try
                {
                    IReadOnlyList<ShadeCharmPlacementDefinition> placementsForHandler = definitions != null
                        ? definitions
                        : Array.Empty<ShadeCharmPlacementDefinition>();
                    if (placementsForHandler.Count > 0)
                    {
                        LogInfo($"Dispatching {placementsForHandler.Count} {handler.GetType().Name} placements for scene '{sceneName}'.");
                    }
                    handler.Populate(context, placementsForHandler);
                }
                catch (Exception ex)
                {
                    LogWarning($"Placement handler {handler.GetType().Name} failed for scene '{sceneName}': {ex}");
                }
            }
        }

        internal static void EnsureInitialized()
        {
            if (s_initialized)
            {
                return;
            }

            s_initialized = true;
            try
            {
                ShadeCharmPlacementDatabase.Reload();
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to load shade charm placements: {ex}");
            }
        }

        internal static void ReloadDefinitions()
        {
            try
            {
                ShadeCharmPlacementDatabase.Reload();
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to reload shade charm placements: {ex}");
            }
        }

        internal static bool IsCharmAlreadyCollected(ShadeCharmId charmId)
        {
            try
            {
                return ShadeRuntime.IsCharmCollected(charmId);
            }
            catch
            {
                return false;
            }
        }

        internal static bool IsPlacementAlreadySatisfied(ShadeCharmPlacementDefinition placement)
        {
            if (placement == null)
            {
                return true;
            }

            try
            {
                return placement.ItemKind switch
                {
                    ShadeCharmPlacementItemKind.Notch => ShadeRuntime.GetNotchCapacity() >= Mathf.Max(0, placement.NotchTargetCapacity ?? 0),
                    _ => ShadeRuntime.IsCharmCollected(placement.CharmId)
                };
            }
            catch
            {
                return false;
            }
        }

        internal static string DescribePlacement(ShadeCharmPlacementDefinition placement)
        {
            if (placement == null)
            {
                return "placement";
            }

            return placement.ItemKind switch
            {
                ShadeCharmPlacementItemKind.Notch => $"notch (target capacity {Mathf.Max(0, placement.NotchTargetCapacity ?? 0)})",
                _ => $"charm {placement.CharmId}"
            };
        }

        internal static void LogInfo(string message)
        {
            if (!ShouldLog())
            {
                return;
            }

            try
            {
                PlacementLogger.LogInfo($"[CharmPlacement] {message}");
            }
            catch
            {
            }
        }

        internal static void LogWarning(string message)
        {
            if (!ShouldLog())
            {
                return;
            }

            try
            {
                PlacementLogger.LogWarning($"[CharmPlacement] {message}");
            }
            catch
            {
            }
        }

        private static bool ShouldLog()
        {
            try
            {
                return ModConfig.Instance.logShade;
            }
            catch
            {
                return true;
            }
        }
    }
}
