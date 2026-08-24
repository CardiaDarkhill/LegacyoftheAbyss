using System;
using System.Collections.Generic;
using System.IO;
using LegacyoftheAbyss.Diagnostics;
using Newtonsoft.Json;
using UnityEngine;

internal static class ModPaths
{
    /// <summary>
    /// Environment override for the user-data folder, mainly so a portable/dev setup can point the
    /// mod at a scratch directory. Ignored when unset or blank.
    /// </summary>
    private const string UserDataOverrideVariable = "LEGACYOFTHEABYSS_DATA";

    private const string UserDataFolderName = "LegacyoftheAbyss";

    internal static readonly string Root = Path.GetDirectoryName(typeof(ModPaths).Assembly.Location) ?? Directory.GetCurrentDirectory();
    internal static readonly string Assets = Path.Combine(Root, "Assets");
    internal static readonly string Logs = Path.Combine(Assets, "logs");

    /// <summary>
    /// Where everything the *player* owns lives - config and shade save slots.
    /// <para>
    /// This deliberately is not <see cref="Assets"/>. Thunderstore-style managers (r2modman, the
    /// Thunderstore app) update a mod by deleting and re-extracting its whole package folder, so
    /// anything written next to the plugin DLL is destroyed on every update - which is exactly how
    /// shade progression kept disappearing. <c>BepInEx/config/</c> is owned by the loader rather
    /// than by any package, survives mod updates and reinstalls, and is a flat folder the player
    /// can copy to another machine by hand.
    /// </para>
    /// </summary>
    internal static readonly string UserData = ResolveUserDataRoot();

    internal static readonly string Config = Path.Combine(UserData, "config.json");

    private static readonly string CleanupRoot = Path.GetFullPath(Path.Combine(Root, "..", "LegacyCleanup"));

    private static string ResolveUserDataRoot()
    {
        string resolved = ResolveUserDataRootCore();

        try
        {
            Directory.CreateDirectory(resolved);
        }
        catch
        {
        }

        MigrateLegacyUserData(resolved);
        return resolved;
    }

    private static string ResolveUserDataRootCore()
    {
        try
        {
            string? overridePath = Environment.GetEnvironmentVariable(UserDataOverrideVariable);
            if (!string.IsNullOrWhiteSpace(overridePath))
            {
                return Path.GetFullPath(overridePath);
            }
        }
        catch
        {
        }

        try
        {
            string configPath = BepInEx.Paths.ConfigPath;
            if (!string.IsNullOrWhiteSpace(configPath))
            {
                return Path.Combine(configPath, UserDataFolderName);
            }
        }
        catch
        {
            // BepInEx.Paths is only populated by the loader; unit tests run without it.
        }

        // Fall back to walking up for a BepInEx folder, in case the loader statics are unavailable
        // but the on-disk layout is the usual one.
        try
        {
            var directory = new DirectoryInfo(Root);
            while (directory != null)
            {
                if (string.Equals(directory.Name, "BepInEx", StringComparison.OrdinalIgnoreCase))
                {
                    return Path.Combine(directory.FullName, "config", UserDataFolderName);
                }

                directory = directory.Parent;
            }
        }
        catch
        {
        }

        // Last resort: the old in-package location. Not update-safe, but better than not saving.
        return Assets;
    }

    /// <summary>
    /// One-time pickup of a save that predates the move out of the plugin folder. Copies rather than
    /// moves - the originals are about to be deleted by the next mod update anyway, and leaving them
    /// in place means a botched migration is recoverable. Never overwrites an existing destination
    /// file, so this is a no-op on every run after the first.
    /// </summary>
    private static void MigrateLegacyUserData(string destinationRoot)
    {
        try
        {
            if (string.Equals(Path.GetFullPath(destinationRoot), Path.GetFullPath(Assets), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!Directory.Exists(Assets))
            {
                return;
            }

            foreach (string pattern in new[] { "config.json", "shade_slot_*.json" })
            {
                foreach (string source in Directory.GetFiles(Assets, pattern))
                {
                    string destination = Path.Combine(destinationRoot, Path.GetFileName(source));
                    if (File.Exists(destination))
                    {
                        continue;
                    }

                    File.Copy(source, destination);
                }
            }
        }
        catch
        {
        }
    }

    private static IEnumerable<string> GetAssetSearchRoots()
    {
        var order = new[]
        {
            Assets,
            Path.Combine(Root, "..", "Assets"),
            Path.Combine(CleanupRoot, "LegacyoftheAbyss", "Assets"),
            Path.Combine(CleanupRoot, "Assets"),
            Path.Combine(CleanupRoot, "LegacyoftheAbyss"),
            CleanupRoot
        };

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var candidate in order)
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                continue;
            }

            string full = Path.GetFullPath(candidate);
            if (seen.Add(full))
            {
                yield return full;
            }
        }
    }

    internal static bool TryGetAssetPath(out string fullPath, params string[] parts)
    {
        fullPath = string.Empty;
        if (parts == null || parts.Length == 0)
        {
            return false;
        }

        string relative = Path.Combine(parts);
        foreach (var root in GetAssetSearchRoots())
        {
            string candidate = Path.Combine(root, relative);
            if (File.Exists(candidate))
            {
                fullPath = candidate;
                return true;
            }
        }

        fullPath = Path.Combine(Assets, relative);
        return File.Exists(fullPath);
    }

    internal static string GetAssetPath(params string[] parts)
    {
        return TryGetAssetPath(out var resolved, parts)
            ? resolved
            : Path.Combine(Assets, Path.Combine(parts));
    }

    internal static string GetAssetDirectory(params string[] parts)
    {
        string relative = Path.Combine(parts);
        foreach (var root in GetAssetSearchRoots())
        {
            string candidate = Path.Combine(root, relative);
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        return Path.Combine(Assets, relative);
    }
}

[Serializable]
public class ModConfig
{
    public bool logDamage = true;
    public bool logGeneral = true;
    public bool logMenu = true;
    public bool logShade = true;
    public bool logHud = true;
    // Enables the developer HP/soul cheat keys polled by SimpleHUD.Update. Off by default
    // so shipped builds do not poll six keys every frame or expose the cheats.
    public bool debugKeysEnabled = false;
    public bool shadeUnlockPopupsMuted = false;
    public float shadeUnlockPopupDelaySeconds = 0.75f;
    public float shadeUnlockPopupDurationSeconds = 3.5f;
    public bool shadeEnabled = true;
    // Lets alerted enemies chase/face/shoot the Shade instead of Hornet when it is the nearer
    // target. See LegacyHelper.EnemyAiRetargeting for how, and ShadeAggroTargeting for when.
    public bool shadeEnemyTargetingEnabled = true;
    // Whether a boss attack lands on whoever is standing in it rather than always on Hornet.
    // Separate from shadeEnemyTargetingEnabled because it reaches into hero damage and
    // repositioning, so it is the first thing worth switching off if a boss starts behaving
    // oddly - the Shade will still be chased, it just stops sharing attacks.
    public bool shadeBossAttackSharingEnabled = true;

    // --- Shade AI --------------------------------------------------------------------
    // Hands the Shade to an AI driver that picks a target, closes on it and attacks, rather
    // than waiting for a second player. The pause menu has a "Shade AI" screen for the few
    // settings worth deciding and an "Advanced AI Options" screen under it for the rest; the
    // three marked config-only below are not on either, because they are not worth the screen
    // space. The AI also has a rebindable hotkey in Controls. See Shade/Ai/ and
    // LegacyHelper.ShadeController.Ai.cs.
    //
    // Note the AI does not touch assist mode. It fights on the same terms the player does and
    // can be killed; turn assist mode on yourself if you want an invincible Shade.
    public bool shadeAiEnabled = false;
    // Fraction of the Shade's theoretical maximum attack rate the AI is allowed to use. The
    // nail cooldown is what the game permits, not what a person achieves while also dodging,
    // and an AI swinging at the cap trivialises fights. Derived from the live cooldown, so
    // Quick Slash still speeds it up. Clamped 0.05-1.
    public float shadeAiAttackSpeedFraction = 0.5f;
    // Whether the AI steps out of attacks and hazards it can see. Off makes it stand and
    // trade, which is mostly interesting with assist mode on.
    public bool shadeAiAvoidAttacks = true;
    // Whether the AI holds SOUL back and channels Focus when it or Hornet is running low.
    public bool shadeAiHealWhenLow = true;
    // Health fractions below which it does that. The Shade has to be damaged either way -
    // Focus refuses to channel at full health, so healing Hornet is a side effect of the
    // Shade healing itself near her.
    public float shadeAiSelfHealBelow = 0.5f;
    public float shadeAiHornetHealBelow = 0.4f;
    // How far from the Shade an enemy has to be before the AI walks over to it. Clamped 2-40.
    public float shadeAiEngageRadius = 14f;
    // Config-only. How many enemies one cast has to land on before the AI thinks it worth the
    // SOUL. A single enemy tanky enough to qualify on its own does too - see below. Clamped 1-8.
    public int shadeAiSpellClusterSize = 2;
    // Config-only. How many of the Shade's own nail hits one enemy has to survive before it is
    // worth a spell by itself. This stands in for a boss flag, because the game does not expose one:
    // HealthManager's enemy types are Regular/Shade/Armoured and the journal only knows
    // Enemy/Other. An earlier flat "200 HP is a boss" test classified ordinary Ant enemies as
    // bosses and burned a full meter on three fireballs at one of them. Clamped 1-100.
    public int shadeAiSpellWorthNailHits = 20;
    // While an AI drives the Shade there is no second player, so Hornet has no reason to be locked
    // to one input device. With this on, the keyboard/controller split the two-player presets set up
    // is ignored for as long as the AI is driving and Hornet answers to both at once, exactly as she
    // does in the unmodded game. Turn it off to keep the split regardless.
    public bool shadeAiVanillaControls = true;
    // Whether the Shade steers around terrain instead of pressing into it. Local steering only - it
    // handles corners and pillars, not a dead-end it has to back out of.
    public bool shadeAiPathAroundTerrain = true;
    // Whether the "Command Shade" binding (middle mouse, or the left stick of the first pad) opens
    // the targeting reticle that tells an AI-driven Shade where to stand. Tap twice to make it hold
    // where it is; aim first to send it somewhere. Off if you would rather not risk the press.
    public bool shadeAiCommandEnabled = true;
    // How often the AI rebuilds its list of enemies. That scan walks every HealthManager in
    // the scene, so it is the one part of this that is not free; the positions of everything
    // already on the list are re-read every frame regardless.
    public float shadeAiScanIntervalSeconds = 0.35f;

    public string shadeSkin = "Default";

    // --- Shade rendering -------------------------------------------------------------
    // These have no pause-menu screen: they are read once at startup and applied whenever the
    // Shade next spawns, so editing config.json and relaunching is the whole workflow.
    // Sorting layer the Shade's sprite is drawn on. Silksong's layers, in draw order, are
    // Default / Far BG 2 / Far BG 1 / Mid BG / Immediate BG / Actors / Player / Tiles /
    // MID Dressing / Immediate FG / Scene Border / Far FG / Vignette / Over / HUD /
    // Inventory - weather, fog and vignette all live above "Player", so the Shade has to sit
    // on a character layer to be occluded by them. Blank falls back to whatever layer
    // Hornet's own renderer is using. See LegacyHelper.ShadeRendering.cs.
    public string shadeSortingLayer = "Player";
    // Draw order within shadeSortingLayer. When the Shade shares Hornet's layer this is an
    // offset from her order (1 = just in front of her); on any other layer it is absolute.
    public int shadeSortingOrderOffset = 1;
    // Draw the Shade with a copy of Hornet's own sprite material rather than Unity's default
    // one, so scene darkness, character tinting and appearance regions treat it as a
    // character instead of an unlit overlay. Toggleable because it is the one visual change
    // here that depends on a game-side shader we do not control.
    public bool shadeUseHornetMaterial = true;
    // Trailing black-wisp emitter that follows the Shade, scaled by its current SOUL.
    public bool shadeShadowParticlesEnabled = true;
    // Global multiplier on that emitter, 0 (off) to 2 (twice the tuned density).
    public float shadeShadowParticleIntensity = 1f;
    // Anti-alias the large skin-selector preview. The source art is low-resolution HK1
    // sprite work shown at ~900px, so point filtering reads as heavy pixelation there.
    public bool shadeSkinPreviewSmoothing = true;
    // The same filtering applied to the in-game Shade sheets. Off by default - at gameplay
    // scale the crisp pixel look is a legitimate preference, so this is opt-in.
    public bool shadeSpriteSmoothing = false;
    public bool hornetKeyboardEnabled = false;
    public bool hornetControllerEnabled = true;
    public float hornetDamageMultiplier = 1f;
    public float shadeDamageMultiplier = 1f;
    public int bindHornetHeal = 3;
    public int bindShadeHeal = 2;
    public int focusHornetHeal = 1;
    public int focusShadeHeal = 1;
    public ShadeInputConfig shadeInput = ShadeInputConfig.CreateDefault();

    // --- Bug reporting ---------------------------------------------------------------
    // Press the hotkey while a bug is on screen: the game freezes, a screenshot and a full
    // state snapshot are taken, and whatever you type is written next to them under
    // BepInEx/config/LegacyoftheAbyss/bug_reports/. See Diagnostics/BugReportSystem.cs.
    // All of these are read once when the reporter installs, so a change needs a restart.
    public bool bugReportsEnabled = true;
    // Any UnityEngine.KeyCode name. Stored as a string rather than the enum because
    // ModConfig has a JsonUtility fallback path that would render it as a bare integer,
    // and a config file nobody can read by eye is a config file nobody edits.
    public string bugReportHotkey = DefaultBugReportHotkey;
    public bool bugReportScreenshot = true;
    // Longest edge, in pixels, the screenshot is scaled down to. A 4K frame is about 8 MB of PNG
    // per report; 1920 lands nearer 1.5 MB and is still far more detail than any description.
    // Set to 0 to keep the native resolution - worth doing when chasing a rendering or aliasing
    // bug, where the artifact may not survive a resample.
    public int bugReportScreenshotMaxWidth = 1920;
    // Log lines kept in memory for the report. These come from every BepInEx source, not
    // just this mod, because the line that explains a Shade bug is often the game's own.
    public int bugReportLogLines = 800;
    // Rolling samples of Hornet/Shade position, health and state flags, so a report covers
    // the seconds before you reacted rather than only the aftermath.
    public bool bugReportFlightRecorderEnabled = true;
    public float bugReportFlightRecorderSeconds = 30f;
    public float bugReportFlightRecorderIntervalSeconds = 0.1f;
    // Rolling record of discrete events - hero repositions, what the Shade's aggro proxy walked
    // into, every damage decision made about the Shade. The flight recorder says what state things
    // were in; this says what happened to them, which is what a report needs to name a culprit.
    // Independent of the log* flags on purpose: those are console noise settings, and the line that
    // explains a bug is routinely one they had switched off.
    public bool bugReportEventRecorderEnabled = true;
    public int bugReportEventRecorderCapacity = BugReportEventRing.DefaultCapacity;
    // File a report automatically when mod code throws. Capped per session, and deduped by
    // exception plus first stack frame, so a throw inside Update does not write one report
    // per frame.
    public bool bugReportAutoCaptureExceptions = true;
    public int bugReportAutoCaptureLimit = 5;

    /// <summary>Layer <see cref="shadeSortingLayer"/> falls back to when the saved value is blank.</summary>
    public const string DefaultShadeSortingLayer = "Player";

    /// <summary>Hotkey <see cref="bugReportHotkey"/> falls back to when the saved value is blank or unparseable.</summary>
    public const string DefaultBugReportHotkey = "F8";

    /// <summary>Upper bound on <see cref="shadeShadowParticleIntensity"/>, enforced on load.</summary>
    public const float MaxShadowParticleIntensity = 2f;

    private static ModConfig? instance;
    private static readonly JsonSerializerSettings FallbackJsonSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.None,
        NullValueHandling = NullValueHandling.Ignore
    };

    public static ModConfig Instance => instance ??= Load();

    public static ModConfig Load()
    {
        try
        {
            if (File.Exists(ModPaths.Config))
            {
                string json = File.ReadAllText(ModPaths.Config);
                instance = Deserialize(json) ?? new ModConfig();
            }
            else
            {
                instance = new ModConfig();
                Save();
            }

            instance.shadeInput ??= ShadeInputConfig.CreateDefault();
            if (string.IsNullOrWhiteSpace(instance.shadeSkin))
            {
                instance.shadeSkin = "Default";
            }
            if (string.IsNullOrWhiteSpace(instance.shadeSortingLayer))
            {
                instance.shadeSortingLayer = DefaultShadeSortingLayer;
            }
            instance.shadeShadowParticleIntensity = Mathf.Clamp(instance.shadeShadowParticleIntensity, 0f, MaxShadowParticleIntensity);
            if (string.IsNullOrWhiteSpace(instance.bugReportHotkey))
            {
                instance.bugReportHotkey = DefaultBugReportHotkey;
            }
            instance.bugReportLogLines = Mathf.Clamp(instance.bugReportLogLines, BugReportLogRing.MinimumCapacity, BugReportLogRing.MaximumCapacity);
            instance.bugReportFlightRecorderSeconds = Mathf.Clamp(instance.bugReportFlightRecorderSeconds, BugReportFlightRecorder.MinimumWindowSeconds, BugReportFlightRecorder.MaximumWindowSeconds);
            instance.bugReportFlightRecorderIntervalSeconds = Mathf.Max(instance.bugReportFlightRecorderIntervalSeconds, BugReportFlightRecorder.MinimumIntervalSeconds);
            instance.bugReportEventRecorderCapacity = Mathf.Clamp(instance.bugReportEventRecorderCapacity, BugReportEventRing.MinimumCapacity, BugReportEventRing.MaximumCapacity);
            instance.bugReportAutoCaptureLimit = Mathf.Max(0, instance.bugReportAutoCaptureLimit);
            instance.bugReportScreenshotMaxWidth = Mathf.Max(0, instance.bugReportScreenshotMaxWidth);
            instance.shadeAiEngageRadius = Mathf.Clamp(instance.shadeAiEngageRadius, 2f, 40f);
            instance.shadeAiSpellClusterSize = Mathf.Clamp(instance.shadeAiSpellClusterSize, 1, 8);
            instance.shadeAiSpellWorthNailHits = Mathf.Clamp(instance.shadeAiSpellWorthNailHits, 1, 100);
            instance.shadeAiAttackSpeedFraction = Mathf.Clamp(instance.shadeAiAttackSpeedFraction, 0.05f, 1f);
            instance.shadeAiSelfHealBelow = Mathf.Clamp01(instance.shadeAiSelfHealBelow);
            instance.shadeAiHornetHealBelow = Mathf.Clamp01(instance.shadeAiHornetHealBelow);
            instance.shadeAiScanIntervalSeconds = Mathf.Clamp(instance.shadeAiScanIntervalSeconds, 0.05f, 2f);
        }
        catch
        {
            instance = new ModConfig();
        }

        return instance;
    }

    public static void Save()
    {
        try
        {
            var directory = Path.GetDirectoryName(ModPaths.Config);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = Serialize(Instance);
            File.WriteAllText(ModPaths.Config, json);
        }
        catch
        {
        }
    }

    private static string Serialize(ModConfig config)
    {
        try
        {
            string json = JsonConvert.SerializeObject(config, FallbackJsonSettings);
            if (!string.IsNullOrWhiteSpace(json))
            {
                return json;
            }
        }
        catch
        {
        }

        if (TrySerializeWithUnity(config, out var unityJson))
        {
            return unityJson;
        }

        return string.Empty;
    }

    private static bool TrySerializeWithUnity(ModConfig config, out string json)
    {
        try
        {
            json = JsonUtility.ToJson(config, true);
            if (!string.IsNullOrWhiteSpace(json))
            {
                return true;
            }
        }
        catch
        {
        }

        json = string.Empty;
        return false;
    }

    private static ModConfig? Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var config = JsonConvert.DeserializeObject<ModConfig>(json, FallbackJsonSettings);
            if (config != null)
            {
                return config;
            }
        }
        catch
        {
        }

        if (TryDeserializeWithUnity(json, out var unityConfig))
        {
            return unityConfig;
        }

        return null;
    }

    private static bool TryDeserializeWithUnity(string json, out ModConfig? config)
    {
        try
        {
            config = JsonUtility.FromJson<ModConfig>(json);
            if (config != null)
            {
                return true;
            }
        }
        catch
        {
        }

        config = null;
        return false;
    }
}
