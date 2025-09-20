#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    public enum ShadeCharmId
    {
        WaywardCompass,
        Sprintmaster,
        Dashmaster,
        ShamanStone,
        SpellTwister,
        QuickSlash,
        MarkOfPride,
        Longnail,
        SoulCatcher,
        FragileStrength,
        SoulEater,
        Grubsong,
        QuickFocus,
        DeepFocus,
        ShapeOfUnn,
        SteadyBody,
        StalwartShell,
        FuryOfTheFallen,
        NailmastersGlory,
        FragileHeart,
        SharpShadow
    }

    internal sealed class ShadeCharmDefinition
    {
        public ShadeCharmDefinition(
            string id,
            ShadeCharmStatModifiers? statModifiers = null,
            ShadeCharmAbilityToggles? abilityToggles = null,
            ShadeCharmHooks? hooks = null,
            string? displayName = null,
            string? description = null,
            int notchCost = 0,
            Color? fallbackTint = null,
            ShadeCharmId? enumId = null,
            string? iconName = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            StatModifiers = statModifiers ?? ShadeCharmStatModifiers.Identity;
            AbilityToggles = abilityToggles ?? ShadeCharmAbilityToggles.None;
            Hooks = hooks ?? ShadeCharmHooks.Empty;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? Id : displayName;
            Description = description ?? string.Empty;
            NotchCost = Mathf.Max(0, notchCost);
            FallbackTint = fallbackTint ?? Color.white;
            EnumId = enumId;
            Icon = ShadeCharmIconLoader.TryLoadIcon(iconName, Id, DisplayName, EnumId?.ToString());
        }

        public string Id { get; }

        public ShadeCharmStatModifiers StatModifiers { get; }

        public ShadeCharmAbilityToggles AbilityToggles { get; }

        public ShadeCharmHooks Hooks { get; }

        public string DisplayName { get; }

        public string Description { get; }

        public int NotchCost { get; }

        public Color FallbackTint { get; }

        public ShadeCharmId? EnumId { get; }

        public Sprite? Icon { get; }

        public override string ToString() => DisplayName;
    }

    internal readonly struct ShadeCharmStatModifiers
    {
        public static ShadeCharmStatModifiers Identity => new ShadeCharmStatModifiers();

        public ShadeCharmStatModifiers()
        {
            MoveSpeedMultiplier = 1f;
            MoveSpeedFlatBonus = 0f;
            SprintSpeedMultiplier = 1f;
            SprintSpeedFlatBonus = 0f;
            SprintDashSpeedMultiplier = 1f;
            SprintDashSpeedFlatBonus = 0f;
            SprintDashDurationMultiplier = 1f;
            SprintDashDurationFlatDelta = 0f;
            SprintDashCooldownMultiplier = 1f;
            SprintDashCooldownFlatDelta = 0f;
            FireCooldownMultiplier = 1f;
            FireCooldownFlatDelta = 0f;
            NailCooldownMultiplier = 1f;
            NailCooldownFlatDelta = 0f;
            ShriekCooldownMultiplier = 1f;
            ShriekCooldownFlatDelta = 0f;
            QuakeCooldownMultiplier = 1f;
            QuakeCooldownFlatDelta = 0f;
            TeleportCooldownMultiplier = 1f;
            TeleportCooldownFlatDelta = 0f;
            ProjectileSoulCostMultiplier = 1f;
            ProjectileSoulCostFlatDelta = 0;
            ShriekSoulCostMultiplier = 1f;
            ShriekSoulCostFlatDelta = 0;
            QuakeSoulCostMultiplier = 1f;
            QuakeSoulCostFlatDelta = 0;
            FocusSoulCostMultiplier = 1f;
            FocusSoulCostFlatDelta = 0;
            ShadeSoulCapacityMultiplier = 1f;
            ShadeSoulCapacityFlatBonus = 0;
        }

        public float MoveSpeedMultiplier { get; init; }

        public float MoveSpeedFlatBonus { get; init; }

        public float SprintSpeedMultiplier { get; init; }

        public float SprintSpeedFlatBonus { get; init; }

        public float SprintDashSpeedMultiplier { get; init; }

        public float SprintDashSpeedFlatBonus { get; init; }

        public float SprintDashDurationMultiplier { get; init; }

        public float SprintDashDurationFlatDelta { get; init; }

        public float SprintDashCooldownMultiplier { get; init; }

        public float SprintDashCooldownFlatDelta { get; init; }

        public float FireCooldownMultiplier { get; init; }

        public float FireCooldownFlatDelta { get; init; }

        public float NailCooldownMultiplier { get; init; }

        public float NailCooldownFlatDelta { get; init; }

        public float ShriekCooldownMultiplier { get; init; }

        public float ShriekCooldownFlatDelta { get; init; }

        public float QuakeCooldownMultiplier { get; init; }

        public float QuakeCooldownFlatDelta { get; init; }

        public float TeleportCooldownMultiplier { get; init; }

        public float TeleportCooldownFlatDelta { get; init; }

        public float ProjectileSoulCostMultiplier { get; init; }

        public int ProjectileSoulCostFlatDelta { get; init; }

        public float ShriekSoulCostMultiplier { get; init; }

        public int ShriekSoulCostFlatDelta { get; init; }

        public float QuakeSoulCostMultiplier { get; init; }

        public int QuakeSoulCostFlatDelta { get; init; }

        public float FocusSoulCostMultiplier { get; init; }

        public int FocusSoulCostFlatDelta { get; init; }

        public float ShadeSoulCapacityMultiplier { get; init; }

        public int ShadeSoulCapacityFlatBonus { get; init; }

        public ShadeCharmStatModifiers Combine(in ShadeCharmStatModifiers other)
        {
            return new ShadeCharmStatModifiers
            {
                MoveSpeedMultiplier = MoveSpeedMultiplier * other.MoveSpeedMultiplier,
                MoveSpeedFlatBonus = MoveSpeedFlatBonus + other.MoveSpeedFlatBonus,
                SprintSpeedMultiplier = SprintSpeedMultiplier * other.SprintSpeedMultiplier,
                SprintSpeedFlatBonus = SprintSpeedFlatBonus + other.SprintSpeedFlatBonus,
                SprintDashSpeedMultiplier = SprintDashSpeedMultiplier * other.SprintDashSpeedMultiplier,
                SprintDashSpeedFlatBonus = SprintDashSpeedFlatBonus + other.SprintDashSpeedFlatBonus,
                SprintDashDurationMultiplier = SprintDashDurationMultiplier * other.SprintDashDurationMultiplier,
                SprintDashDurationFlatDelta = SprintDashDurationFlatDelta + other.SprintDashDurationFlatDelta,
                SprintDashCooldownMultiplier = SprintDashCooldownMultiplier * other.SprintDashCooldownMultiplier,
                SprintDashCooldownFlatDelta = SprintDashCooldownFlatDelta + other.SprintDashCooldownFlatDelta,
                FireCooldownMultiplier = FireCooldownMultiplier * other.FireCooldownMultiplier,
                FireCooldownFlatDelta = FireCooldownFlatDelta + other.FireCooldownFlatDelta,
                NailCooldownMultiplier = NailCooldownMultiplier * other.NailCooldownMultiplier,
                NailCooldownFlatDelta = NailCooldownFlatDelta + other.NailCooldownFlatDelta,
                ShriekCooldownMultiplier = ShriekCooldownMultiplier * other.ShriekCooldownMultiplier,
                ShriekCooldownFlatDelta = ShriekCooldownFlatDelta + other.ShriekCooldownFlatDelta,
                QuakeCooldownMultiplier = QuakeCooldownMultiplier * other.QuakeCooldownMultiplier,
                QuakeCooldownFlatDelta = QuakeCooldownFlatDelta + other.QuakeCooldownFlatDelta,
                TeleportCooldownMultiplier = TeleportCooldownMultiplier * other.TeleportCooldownMultiplier,
                TeleportCooldownFlatDelta = TeleportCooldownFlatDelta + other.TeleportCooldownFlatDelta,
                ProjectileSoulCostMultiplier = ProjectileSoulCostMultiplier * other.ProjectileSoulCostMultiplier,
                ProjectileSoulCostFlatDelta = ProjectileSoulCostFlatDelta + other.ProjectileSoulCostFlatDelta,
                ShriekSoulCostMultiplier = ShriekSoulCostMultiplier * other.ShriekSoulCostMultiplier,
                ShriekSoulCostFlatDelta = ShriekSoulCostFlatDelta + other.ShriekSoulCostFlatDelta,
                QuakeSoulCostMultiplier = QuakeSoulCostMultiplier * other.QuakeSoulCostMultiplier,
                QuakeSoulCostFlatDelta = QuakeSoulCostFlatDelta + other.QuakeSoulCostFlatDelta,
                FocusSoulCostMultiplier = FocusSoulCostMultiplier * other.FocusSoulCostMultiplier,
                FocusSoulCostFlatDelta = FocusSoulCostFlatDelta + other.FocusSoulCostFlatDelta,
                ShadeSoulCapacityMultiplier = ShadeSoulCapacityMultiplier * other.ShadeSoulCapacityMultiplier,
                ShadeSoulCapacityFlatBonus = ShadeSoulCapacityFlatBonus + other.ShadeSoulCapacityFlatBonus
            };
        }
    }

    internal readonly struct ShadeCharmAbilityToggles
    {
        public static ShadeCharmAbilityToggles None => default;

        public bool? EnableProjectile { get; init; }

        public bool? UpgradeProjectile { get; init; }

        public bool? EnableDescendingDark { get; init; }

        public bool? UpgradeDescendingDark { get; init; }

        public bool? EnableShriek { get; init; }

        public bool? UpgradeShriek { get; init; }

        public ShadeCharmAbilityToggles Merge(in ShadeCharmAbilityToggles other)
        {
            return new ShadeCharmAbilityToggles
            {
                EnableProjectile = other.EnableProjectile ?? EnableProjectile,
                UpgradeProjectile = other.UpgradeProjectile ?? UpgradeProjectile,
                EnableDescendingDark = other.EnableDescendingDark ?? EnableDescendingDark,
                UpgradeDescendingDark = other.UpgradeDescendingDark ?? UpgradeDescendingDark,
                EnableShriek = other.EnableShriek ?? EnableShriek,
                UpgradeShriek = other.UpgradeShriek ?? UpgradeShriek
            };
        }
    }

    internal sealed class ShadeCharmHooks
    {
        public static ShadeCharmHooks Empty { get; } = new ShadeCharmHooks();

        public Action<ShadeCharmContext>? OnApplied { get; init; }

        public Action<ShadeCharmContext>? OnRemoved { get; init; }

        public Action<ShadeCharmContext, float>? OnUpdate { get; init; }

        public Action<ShadeCharmContext, ShadeCharmDamageEvent>? OnShadeDamaged { get; init; }
    }

    internal readonly struct ShadeCharmContext
    {
        public ShadeCharmContext(LegacyHelper.ShadeController controller, ShadeCharmLoadoutSnapshot snapshot)
        {
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));
            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        }

        public LegacyHelper.ShadeController Controller { get; }

        public ShadeCharmLoadoutSnapshot Snapshot { get; }
    }

    internal readonly struct ShadeCharmDamageEvent
    {
        public ShadeCharmDamageEvent(int attemptedDamage, int actualDamage, bool wasHazard, bool wasPrevented, bool wasLethal)
        {
            AttemptedDamage = Mathf.Max(0, attemptedDamage);
            ActualDamage = Mathf.Max(0, actualDamage);
            WasHazard = wasHazard;
            WasPrevented = wasPrevented;
            WasLethal = wasLethal;
        }

        public int AttemptedDamage { get; }

        public int ActualDamage { get; }

        public bool WasHazard { get; }

        public bool WasPrevented { get; }

        public bool WasLethal { get; }
    }

    internal readonly struct ShadeCharmStatBaseline
    {
        public float MoveSpeed { get; init; }

        public float SprintMultiplier { get; init; }

        public float SprintDashMultiplier { get; init; }

        public float SprintDashDuration { get; init; }

        public float SprintDashCooldown { get; init; }

        public float FireCooldown { get; init; }

        public float NailCooldown { get; init; }

        public float ShriekCooldown { get; init; }

        public float QuakeCooldown { get; init; }

        public float TeleportCooldown { get; init; }

        public int ProjectileSoulCost { get; init; }

        public int ShriekSoulCost { get; init; }

        public int QuakeSoulCost { get; init; }

        public int FocusSoulCost { get; init; }

        public int ShadeSoulCapacity { get; init; }

        public static ShadeCharmStatBaseline CreateDefault()
        {
            return new ShadeCharmStatBaseline
            {
                MoveSpeed = 10f,
                SprintMultiplier = 2.5f,
                SprintDashMultiplier = 7.5f,
                SprintDashDuration = 0.075f,
                SprintDashCooldown = 1f,
                FireCooldown = 0.25f,
                NailCooldown = 0.3f,
                ShriekCooldown = 0.5f,
                QuakeCooldown = 1.1f,
                TeleportCooldown = 1.5f,
                ProjectileSoulCost = 33,
                ShriekSoulCost = 33,
                QuakeSoulCost = 33,
                FocusSoulCost = 33,
                ShadeSoulCapacity = 99
            };
        }
    }

    internal sealed class ShadeCharmLoadoutSnapshot
    {
        public ShadeCharmLoadoutSnapshot(
            float moveSpeed,
            float sprintMultiplier,
            float sprintDashMultiplier,
            float sprintDashDuration,
            float sprintDashCooldown,
            float fireCooldown,
            float nailCooldown,
            float shriekCooldown,
            float quakeCooldown,
            float teleportCooldown,
            int projectileSoulCost,
            int shriekSoulCost,
            int quakeSoulCost,
            int focusSoulCost,
            int shadeSoulCapacity,
            ShadeCharmAbilityToggles abilityOverrides,
            IReadOnlyList<ShadeCharmDefinition> definitions)
        {
            MoveSpeed = moveSpeed;
            SprintMultiplier = sprintMultiplier;
            SprintDashMultiplier = sprintDashMultiplier;
            SprintDashDuration = sprintDashDuration;
            SprintDashCooldown = sprintDashCooldown;
            FireCooldown = fireCooldown;
            NailCooldown = nailCooldown;
            ShriekCooldown = shriekCooldown;
            QuakeCooldown = quakeCooldown;
            TeleportCooldown = teleportCooldown;
            ProjectileSoulCost = projectileSoulCost;
            ShriekSoulCost = shriekSoulCost;
            QuakeSoulCost = quakeSoulCost;
            FocusSoulCost = focusSoulCost;
            ShadeSoulCapacity = shadeSoulCapacity;
            AbilityOverrides = abilityOverrides;
            Definitions = definitions;
        }

        public float MoveSpeed { get; }

        public float SprintMultiplier { get; }

        public float SprintDashMultiplier { get; }

        public float SprintDashDuration { get; }

        public float SprintDashCooldown { get; }

        public float FireCooldown { get; }

        public float NailCooldown { get; }

        public float ShriekCooldown { get; }

        public float QuakeCooldown { get; }

        public float TeleportCooldown { get; }

        public int ProjectileSoulCost { get; }

        public int ShriekSoulCost { get; }

        public int QuakeSoulCost { get; }

        public int FocusSoulCost { get; }

        public int ShadeSoulCapacity { get; }

        public ShadeCharmAbilityToggles AbilityOverrides { get; }

        public IReadOnlyList<ShadeCharmDefinition> Definitions { get; }

        public static ShadeCharmLoadoutSnapshot FromBaseline(ShadeCharmStatBaseline baseline)
        {
            return new ShadeCharmLoadoutSnapshot(
                baseline.MoveSpeed,
                baseline.SprintMultiplier,
                baseline.SprintDashMultiplier,
                baseline.SprintDashDuration,
                baseline.SprintDashCooldown,
                baseline.FireCooldown,
                baseline.NailCooldown,
                baseline.ShriekCooldown,
                baseline.QuakeCooldown,
                baseline.TeleportCooldown,
                baseline.ProjectileSoulCost,
                baseline.ShriekSoulCost,
                baseline.QuakeSoulCost,
                baseline.FocusSoulCost,
                baseline.ShadeSoulCapacity,
                ShadeCharmAbilityToggles.None,
                Array.Empty<ShadeCharmDefinition>());
        }
    }

    internal static class ShadeCharmCalculator
    {
        public static ShadeCharmLoadoutSnapshot BuildSnapshot(
            ShadeCharmStatBaseline baseline,
            IEnumerable<ShadeCharmDefinition>? loadout)
        {
            var defs = new List<ShadeCharmDefinition>();
            var mods = ShadeCharmStatModifiers.Identity;
            var toggles = ShadeCharmAbilityToggles.None;

            if (loadout != null)
            {
                foreach (var charm in loadout)
                {
                    if (charm == null)
                    {
                        continue;
                    }

                    defs.Add(charm);
                    mods = mods.Combine(charm.StatModifiers);
                    toggles = toggles.Merge(charm.AbilityToggles);
                }
            }

            float moveSpeed = ClampNonNegative(baseline.MoveSpeed * mods.MoveSpeedMultiplier + mods.MoveSpeedFlatBonus);
            float sprintMultiplier = ClampNonNegative(baseline.SprintMultiplier * mods.SprintSpeedMultiplier + mods.SprintSpeedFlatBonus);
            float sprintDashMultiplier = ClampNonNegative(baseline.SprintDashMultiplier * mods.SprintDashSpeedMultiplier + mods.SprintDashSpeedFlatBonus);
            float sprintDashDuration = ClampNonNegative(baseline.SprintDashDuration * mods.SprintDashDurationMultiplier + mods.SprintDashDurationFlatDelta);
            float sprintDashCooldown = ClampNonNegative(baseline.SprintDashCooldown * mods.SprintDashCooldownMultiplier + mods.SprintDashCooldownFlatDelta);
            float fireCooldown = ClampNonNegative(baseline.FireCooldown * mods.FireCooldownMultiplier + mods.FireCooldownFlatDelta);
            float nailCooldown = ClampNonNegative(baseline.NailCooldown * mods.NailCooldownMultiplier + mods.NailCooldownFlatDelta);
            float shriekCooldown = ClampNonNegative(baseline.ShriekCooldown * mods.ShriekCooldownMultiplier + mods.ShriekCooldownFlatDelta);
            float quakeCooldown = ClampNonNegative(baseline.QuakeCooldown * mods.QuakeCooldownMultiplier + mods.QuakeCooldownFlatDelta);
            float teleportCooldown = ClampNonNegative(baseline.TeleportCooldown * mods.TeleportCooldownMultiplier + mods.TeleportCooldownFlatDelta);
            int projectileSoulCost = ClampInt(RoundToInt(baseline.ProjectileSoulCost * mods.ProjectileSoulCostMultiplier + mods.ProjectileSoulCostFlatDelta), 0);
            int shriekSoulCost = ClampInt(RoundToInt(baseline.ShriekSoulCost * mods.ShriekSoulCostMultiplier + mods.ShriekSoulCostFlatDelta), 0);
            int quakeSoulCost = ClampInt(RoundToInt(baseline.QuakeSoulCost * mods.QuakeSoulCostMultiplier + mods.QuakeSoulCostFlatDelta), 0);
            int focusSoulCost = ClampInt(RoundToInt(baseline.FocusSoulCost * mods.FocusSoulCostMultiplier + mods.FocusSoulCostFlatDelta), 0);
            int shadeSoulCapacity = ClampInt(RoundToInt(baseline.ShadeSoulCapacity * mods.ShadeSoulCapacityMultiplier + mods.ShadeSoulCapacityFlatBonus), 1);

            return new ShadeCharmLoadoutSnapshot(
                moveSpeed,
                sprintMultiplier,
                sprintDashMultiplier,
                sprintDashDuration,
                sprintDashCooldown,
                fireCooldown,
                nailCooldown,
                shriekCooldown,
                quakeCooldown,
                teleportCooldown,
                projectileSoulCost,
                shriekSoulCost,
                quakeSoulCost,
                focusSoulCost,
                shadeSoulCapacity,
                toggles,
                defs.ToArray());
        }

        private static float ClampNonNegative(float value) => value < 0f ? 0f : value;

        private static int ClampInt(int value, int min) => value < min ? min : value;

        private static int RoundToInt(float value)
        {
            return (int)MathF.Round(value, MidpointRounding.AwayFromZero);
        }
    }

    internal static class ShadeCharmIconLoader
    {
        private static readonly Dictionary<string, Sprite?> Cache = new(StringComparer.OrdinalIgnoreCase);

        public static Sprite? TryLoadIcon(params string?[]? names)
        {
            if (names == null || names.Length == 0)
            {
                return null;
            }

            foreach (var name in names)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                lock (Cache)
                {
                    if (Cache.TryGetValue(name, out var cached))
                    {
                        if (cached != null)
                        {
                            return cached;
                        }
                    }
                }

                var sprite = LoadIconInternal(name!);
                lock (Cache)
                {
                    Cache[name!] = sprite;
                }

                if (sprite != null)
                {
                    return sprite;
                }
            }

            return null;
        }

        private static Sprite? LoadIconInternal(string rawName)
        {
            var candidates = BuildCandidates(rawName).ToArray();
            foreach (var candidate in candidates)
            {
                if (ModPaths.TryGetAssetPath(out var path, candidate)
                    || ModPaths.TryGetAssetPath(out path, "CharmIcons", candidate)
                    || ModPaths.TryGetAssetPath(out path, "Charms", candidate))
                {
                    var sprite = LoadSprite(path);
                    if (sprite != null)
                    {
                        return sprite;
                    }
                }
            }

            return null;
        }

        private static IEnumerable<string> BuildCandidates(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                yield break;
            }

            string trimmed = input.Trim();
            string withoutExt = Path.GetFileNameWithoutExtension(trimmed);
            if (string.IsNullOrWhiteSpace(withoutExt))
            {
                yield break;
            }

            var bases = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                withoutExt,
                withoutExt.Replace(' ', '_'),
                withoutExt.Replace(' ', '-'),
                RemoveNonAlphanumeric(withoutExt)
            };

            string snake = string.Concat(withoutExt.Select((ch, idx) =>
                idx > 0 && char.IsUpper(ch) && char.IsLower(withoutExt[idx - 1]) ? "_" + char.ToLowerInvariant(ch) : char.ToString(char.ToLowerInvariant(ch))));
            bases.Add(snake);

            foreach (var basis in bases.Where(b => !string.IsNullOrWhiteSpace(b)))
            {
                string normalized = basis.Trim();
                if (!normalized.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    yield return normalized + ".png";
                }
                yield return normalized;
            }
        }

        private static string RemoveNonAlphanumeric(string value)
        {
            var chars = value.Where(char.IsLetterOrDigit).ToArray();
            return new string(chars);
        }

        private static Sprite? LoadSprite(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return null;
                }

                var bytes = File.ReadAllBytes(path);
                if (bytes == null || bytes.Length == 0)
                {
                    return null;
                }

                var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                if (!ImageConversion.LoadImage(tex, bytes, false))
                {
                    UnityEngine.Object.Destroy(tex);
                    return null;
                }

                tex.filterMode = FilterMode.Point;
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.name = Path.GetFileName(path);
                tex.hideFlags = HideFlags.HideAndDontSave;

                var sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 64f);
                sprite.name = Path.GetFileNameWithoutExtension(path);
                sprite.hideFlags = HideFlags.HideAndDontSave;
                return sprite;
            }
            catch
            {
                return null;
            }
        }
    }
}
