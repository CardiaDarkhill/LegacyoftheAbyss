#nullable enable
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LegacyoftheAbyss.Shade;
using UnityEngine;

public partial class LegacyHelper
{
    /// <summary>
    /// Marks every uncollected Legacy of the Abyss pickup on the map while the Abyssal Compass is
    /// equipped.
    /// <para>
    /// A pin is a child of the room's own <see cref="GameMapScene"/> object, the way the game's own
    /// markers are: parenting hands us pan, zoom, and per-zone visibility with no per-frame code.
    /// Pins are room-level - <c>GameMap.GetMapPosition</c> needs a room's world size to place
    /// anything inside it, and that is only known while standing in the room.
    /// </para>
    /// </summary>
    internal static class ShadePickupMapMarkers
    {
        private const string PinName = "ShadePickupPin";

        /// <summary>Pin height in map-local units when no pin exists to measure against.</summary>
        private const float FallbackPinHeight = 0.6f;

        private static readonly List<GameObject> Pins = new List<GameObject>();
        private static bool loggedMissingSprite;

        /// <summary>
        /// Rebuilds the pins for <paramref name="map"/>. Must run after <c>GameMap.SetupMap</c>,
        /// not instead of it: that method walks every child of every room object and forces it
        /// active or inactive, so a pin created earlier is switched off again in any room the
        /// player has not mapped.
        /// </summary>
        internal static void Refresh(GameMap map)
        {
            ClearPins();

            if (map == null || !ShouldDisplay())
            {
                return;
            }

            var placements = ShadeCharmPlacementDatabase.GetAllPlacements();
            if (placements == null || placements.Count == 0)
            {
                return;
            }

            var sprite = ShadeCharmIconLoader.TryLoadIcon("Shade_Pin");
            if (sprite == null)
            {
                if (!loggedMissingSprite)
                {
                    loggedMissingSprite = true;
                    LogWarning("Shade_Pin.png could not be loaded; the Abyssal Compass will not mark pickups on the map.");
                }

                return;
            }

            var scenes = BuildSceneLookup(map);
            float pinHeight = ResolvePinHeight(map);
            bool showCollected = ModConfig.Instance.debugShowCollectedPickupsOnMap;

            // Counted per room before anything is drawn, so a room with several pickups can fan them
            // out rather than stacking them all on the same spot. Pins are room-level - see the note
            // on this class - so the count is the only detail a room can carry.
            var perRoom = new Dictionary<GameMapScene, int>();
            foreach (var placement in placements)
            {
                if (placement == null || string.IsNullOrWhiteSpace(placement.SceneName))
                {
                    continue;
                }

                if (!showCollected && ShadeCharmPlacementService.IsPlacementAlreadySatisfied(placement))
                {
                    continue;
                }

                if (!scenes.TryGetValue(placement.SceneName!, out var scene) || scene == null)
                {
                    continue;
                }

                perRoom.TryGetValue(scene, out int count);
                perRoom[scene] = count + 1;
            }

            int placed = 0;
            foreach (var room in perRoom)
            {
                for (int i = 0; i < room.Value; i++)
                {
                    CreatePin(room.Key, sprite, pinHeight, i, room.Value);
                    placed++;
                }
            }

            if (ModConfig.Instance.logGeneral)
            {
                LogInfo($"Abyssal Compass: {placed} pickup pin(s) placed across {perRoom.Count} mapped rooms"
                    + (showCollected ? ", collected ones included." : "."));
            }
        }

        private static bool ShouldDisplay()
        {
            if (!ModConfig.Instance.shadeEnabled)
            {
                return false;
            }

            var charms = ShadeRuntime.Charms;
            return charms != null && charms.IsEquipped(ShadeCharmId.WaywardCompass);
        }

        private static Dictionary<string, GameMapScene> BuildSceneLookup(GameMap map)
        {
            var lookup = new Dictionary<string, GameMapScene>(System.StringComparer.OrdinalIgnoreCase);
            foreach (var scene in map.GetComponentsInChildren<GameMapScene>(true))
            {
                if (scene == null)
                {
                    continue;
                }

                string name = scene.Name;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    lookup[name] = scene;
                }
            }

            return lookup;
        }

        /// <summary>
        /// Height a pin should draw at, in the map's own units, measured off one of the game's
        /// pins. Nothing else on the map is a reliable yardstick - room sprites vary by orders of
        /// magnitude - so a missing template falls back to a fixed size rather than a guess scaled
        /// from something unrelated.
        /// </summary>
        private static float ResolvePinHeight(GameMap map)
        {
            foreach (var pin in map.GetComponentsInChildren<MapPin>(true))
            {
                var renderer = pin != null ? pin.GetComponent<SpriteRenderer>() : null;
                if (renderer == null || renderer.sprite == null)
                {
                    continue;
                }

                float height = renderer.sprite.bounds.size.y * Mathf.Abs(renderer.transform.lossyScale.y);
                if (height > 0.0001f)
                {
                    return height;
                }
            }

            return FallbackPinHeight;
        }

        /// <summary>
        /// One pin. <paramref name="slot"/> and <paramref name="total"/> spread a room's pins along
        /// a row centred on it, so two pickups in one room read as two rather than as one.
        /// </summary>
        private static void CreatePin(GameMapScene scene, Sprite sprite, float pinHeight, int slot, int total)
        {
            var go = new GameObject(PinName) { layer = scene.gameObject.layer };
            var pinTransform = go.transform;
            pinTransform.SetParent(scene.transform, false);

            // Negative z for the same reason the game's compass icon uses it: in front of the room
            // sprite it sits on.
            pinTransform.localPosition = new Vector3(0f, 0f, -1f);
            pinTransform.localRotation = Quaternion.identity;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;

            var sceneRenderer = scene.GetComponent<SpriteRenderer>();
            if (sceneRenderer != null)
            {
                renderer.sortingLayerID = sceneRenderer.sortingLayerID;
                renderer.sortingOrder = sceneRenderer.sortingOrder + 1;
            }

            float spriteHeight = sprite.bounds.size.y * Mathf.Abs(scene.transform.lossyScale.y);
            float pinScale = 1f;
            if (spriteHeight > 0.0001f)
            {
                pinScale = pinHeight / spriteHeight;
                pinTransform.localScale = new Vector3(pinScale, pinScale, 1f);
            }

            if (total > 1)
            {
                // Stepped by the pin's own drawn width, so the row is as wide as it needs to be and
                // no wider - a room sprite is not a reliable yardstick for anything else.
                float step = sprite.bounds.size.x * pinScale;
                float offset = (slot - (total - 1) * 0.5f) * step;
                pinTransform.localPosition = new Vector3(offset, 0f, -1f);
            }

            Pins.Add(go);
        }

        private static void ClearPins()
        {
            foreach (var pin in Pins)
            {
                if (pin == null)
                {
                    continue;
                }

                // Destroy only lands at the end of the frame, and SetupMap has already run for this
                // one - hide it now so a stale pin cannot draw alongside its replacement.
                pin.SetActive(false);
                Object.Destroy(pin);
            }

            Pins.Clear();
        }
    }

    /// <summary>
    /// Resolved by shape rather than named through the attribute, so an unrecognised assembly costs
    /// the pins rather than every patch the mod has.
    /// </summary>
    [HarmonyPatch]
    private class GameMap_SetupMap_ShadePickupPins
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (var method in AccessTools.GetDeclaredMethods(typeof(GameMap)))
            {
                if (method.Name == nameof(GameMap.SetupMap) && method.GetParameters().Length <= 1)
                {
                    yield return method;
                }
            }
        }

        private static void Postfix(GameMap __instance)
        {
            ShadePickupMapMarkers.Refresh(__instance);
        }
    }
}
#nullable restore
