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

    // HUD layout, applied live and rereadable with Ctrl+F5. These exist because placing borrowed
    // art by reasoning about its dimensions kept being wrong in ways only an eye catches. The
    // defaults were dialled in against the running game, not derived.
    public bool hudFrameEnabled = true;
    public bool hudFrameMirror = false;
    public float hudFrameRotation = 90f;
    public float hudFrameScale = 1f;
    public float hudFrameOffsetX = -26f;
    public float hudFrameOffsetY = 0f;

    /// <summary>Where the plate's socket sits within it, as a fraction of its drawn size, y down.</summary>
    public float hudFrameSocketX = 0.704f;

    public float hudFrameSocketY = 0.568f;

    public float hudOrbScale = 1f;
    public float hudOrbOffsetX = 120f;
    public float hudOrbOffsetY = 0f;

    public float hudMaskScale = 1f;
    public float hudMaskSpacing = 6f;
    // The companion's buff bar, under the mask row. Sized and placed here rather than derived,
    // because a row of status icons is exactly the kind of placement that cannot be reasoned out
    // from the art; Ctrl+F5 rereads these against the running screen.
    public bool hudBuffBarEnabled = true;
    public float hudBuffIconSize = 34f;
    public float hudBuffIconScale = 1f;
    public float hudBuffIconSpacing = 6f;
    public float hudBuffBarOffsetX = -150f;
    public float hudBuffBarOffsetY = -14f;

    public float hudMaskRowOffsetX = -120f;
    public float hudMaskRowOffsetY = 12f;
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
    // nail cooldown is what the game permits,
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
    // Enemy/Other.
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

    // Which character each companion slot wears, indexed by slot id (0 is the original Shade).
    // Shorter than the companion count, or holding an unknown name, falls back to the Shade.
    // Replace, not the default Auto: Newtonsoft appends into a collection that already holds
    // items, so a field initializer here would prepend its defaults to every loaded config and
    // shift every companion onto the wrong character.
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> companionCharacters = new List<string>();

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
    // The Knight's rig comes out of Hollow Knight at its own scale, which stands it nearly as tall
    // as Hornet; it should be a little over half her height. Multiplies the rig's own scale, and
    // the companion body's collider with it so the hurtbox does not stay a head taller than the
    // art. Cosmetic tuning - raise it if the Knight reads as too small beside her.
    public float knightScale = 0.57f;
    // Give the Shade a clone of Hornet's hero light. Scene darkness is a shader cutout fed by a
    // camera that renders that object, so this is what lets the Shade be seen - and light its own
    // surroundings - in a dark room away from Hornet.
    public bool shadeLightEnabled = true;
    // Both of these are the values reached when the Shade is clear of Hornet's own light. They
    // fade to nothing as it closes on her, because two overlapping lights wash the pair out - so
    // the Shade lights what she is not lighting rather than doubling up on what she is.
    // Peak alpha multiplier on the cloned light. 1 matches Hornet; above that saturates it.
    public float shadeLightIntensity = 2.5f;
    // Peak radius multiplier, reached at the edge of Hornet's own light. 1 matches her radius.
    public float shadeLightRadiusScale = 0.5f;
    // World-unit distance from Hornet over which the fade above happens. 0 measures it from her
    // own light instead, which reads larger than the light looks because these sprites carry a
    // wide soft falloff.
    public float shadeLightFalloffRadius = 10f;
    // The Knight is a second player rather than a companion drifting near Hornet, so it carries a
    // stronger light of its own. These multiply the two peaks above when a companion is wearing the
    // Knight; the distance fade still applies on top, so its light still yields where hers reaches.
    public float knightLightRadiusMultiplier = 2f;
    public float knightLightIntensityMultiplier = 1.5f;
    // Keeps the Knight inside the camera's view, so the second player can always see themselves.
    // Silksong's camera cannot be split - tk2dCamera writes the projection matrix directly and has
    // no viewport support - so confining the Knight to the visible area is what stands in for it.
    public bool knightCameraLeashEnabled = true;
    // World units held back from the screen edge, so the Knight stops just inside it rather than
    // half off it. Small on purpose: this margin plus the Knight's own half-height is the band of
    // visible screen it can never reach, and the frame is much shorter than it is wide, so a
    // generous value reads as a wall with room clearly left above it.
    public float knightCameraLeashMargin = 0.75f;
    // Pull the camera toward the midpoint between Hornet and whichever companion is out, so both
    // stay on screen for longer before the leash above bites. Biases the camera's follow target, so
    // scene bounds, lock areas and the game's own damping all still apply. Applies to the Shade as
    // well as the Knight - the Shade is leashed closer, so it simply asks for a smaller lean.
    // Toggled in-game by "Co-op Camera" on the Shade settings screen.
    public bool companionCameraBiasEnabled = true;
    // How far the view may widen once the pair no longer fit the frame, as a share of the normal
    // shot. 0.25 is a quarter wider; 0 disables the zoom and leaves only the lean. Applied by
    // raising the camera's field of view, which the darkness pass follows on its own.
    public float companionCameraMaxZoom = 0.25f;
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

    // --- Difficulty ------------------------------------------------------------------
    // Four damage multipliers rather than one per character, because the presets below need
    // to weaken melee without touching casts. Each is applied at the one place its damage is
    // finally computed - see DamageEnemies_Start_Mod for Hornet's two (split on attackType)
    // and ShadeController.Charms/Spells for the Shade's.
    /// <summary>Multiplies Hornet's needle (and needle-derived) damage. Was the whole of her damage.</summary>
    public float hornetDamageMultiplier = 1f;
    /// <summary>Multiplies everything of Hornet's that is not a needle strike - silk skills, tools, spells.</summary>
    public float hornetSilkSkillDamageMultiplier = 1f;
    /// <summary>Multiplies the Shade's nail. Was the whole of its damage, spells included.</summary>
    public float shadeDamageMultiplier = 1f;
    /// <summary>Multiplies all six of the Shade's spells.</summary>
    public float shadeSpellDamageMultiplier = 1f;
    public int bindHornetHeal = 3;
    public int bindShadeHeal = 2;
    public int focusHornetHeal = 1;
    public int focusShadeHeal = 1;
    // The Shade's mask count as a share of Hornet's, rounded up, recomputed whenever she gains
    // a mask. Stepped in tenths by the menu; the lowest step is not 10% but a flat one mask,
    // because "10% of Hornet" is one mask for most of the game anyway and stops being one
    // exactly when the run is hardest.
    public float shadeMaskFraction = DefaultShadeMaskFraction;
    // Whether the Shade may channel Focus while on full masks. Off matches Hornet's own rule;
    // on lets it burn SOUL purely to heal her.
    public bool shadeFocusAtFullMasks = false;
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

    /// <summary>Half of Hornet's masks, rounded up - what the Shade had before the setting existed.</summary>
    public const float DefaultShadeMaskFraction = 0.5f;

    /// <summary>
    /// Lowest <see cref="shadeMaskFraction"/> step. Means "one mask, whatever Hornet has" rather
    /// than a literal tenth - see the field's comment.
    /// </summary>
    public const float MinShadeMaskFraction = 0.1f;

    /// <summary>
    /// Computes the Shade's mask count from Hornet's. Rounds up, and never returns less than one.
    /// <para>
    /// Lives here rather than beside the Shade because three unrelated places need the same answer -
    /// the controller's spawn-time baseline, its recompute when Hornet gains a mask, and the HUD's
    /// fallback before it has explicit stats. Keep it in one place; three copies of
    /// <c>(maxHealth + 1) / 2</c> have to be kept in step by hand.
    /// </para>
    /// </summary>
    public static int ComputeShadeMaskCount(int hornetMaxMasks)
    {
        if (hornetMaxMasks <= 0)
        {
            return 0;
        }

        var config = Instance;
        float fraction = Mathf.Clamp(config != null ? config.shadeMaskFraction : DefaultShadeMaskFraction, MinShadeMaskFraction, 1f);
        if (fraction <= MinShadeMaskFraction + 0.001f)
        {
            // The lowest step is "Always 1", not a tenth. See shadeMaskFraction.
            return 1;
        }

        return Mathf.Max(1, Mathf.CeilToInt(hornetMaxMasks * fraction));
    }

    private static ModConfig? instance;
    private static readonly JsonSerializerSettings FallbackJsonSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.None,
        NullValueHandling = NullValueHandling.Ignore
    };

    public static ModConfig Instance => instance ??= Load();

    /// <summary>
    /// Rereads config.json from disk, discarding whatever is in memory. Bound to Ctrl+F5 so the HUD
    /// tuning values can be dialled in against the running game rather than one build at a time.
    /// </summary>
    public static void Reload()
    {
        instance = null;
        _ = Instance;
    }

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
            // Snap to the menu's tenths as well as clamping: a hand-edited 0.55 would otherwise
            // survive here and then jump the first time the slider is nudged.
            instance.shadeMaskFraction = Mathf.Clamp(Mathf.Round(instance.shadeMaskFraction * 10f) / 10f, MinShadeMaskFraction, 1f);
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



/// <summary>
/// The three difficulty presets the Difficulty menu offers, and the values each one stands for.
/// <para>
/// Each preset is the complete set of difficulty values, not a delta, so applying one always lands
/// on a known state regardless of what was there before. <see cref="Identify"/> reads back the other
/// way, so the menu can label a hand-tuned set as Custom without anything having to be stored.
/// </para>
/// </summary>
public sealed class DifficultyPreset
{
    public const string Easy = "Easy";
    public const string Normal = "Normal";
    public const string Abyss = "Abyss";
    public const string Custom = "Custom";

    public string Name { get; private set; } = Custom;
    public string Description { get; private set; } = string.Empty;
    public float HornetNeedleDamage { get; private set; } = 1f;
    public float HornetSilkSkillDamage { get; private set; } = 1f;
    public float ShadeNailDamage { get; private set; } = 1f;
    public float ShadeSpellDamage { get; private set; } = 1f;
    public int BindHornetHeal { get; private set; } = 3;
    public int BindShadeHeal { get; private set; } = 2;
    public int FocusHornetHeal { get; private set; } = 1;
    public int FocusShadeHeal { get; private set; } = 1;
    public float ShadeMaskFraction { get; private set; } = ModConfig.DefaultShadeMaskFraction;
    public bool ShadeFocusAtFullMasks { get; private set; }

    public static readonly DifficultyPreset EasyPreset = new DifficultyPreset
    {
        Name = Easy,
        Description = "Silksong as it ships, with the Shade helping. Nothing is weakened to pay for it, so fights resolve faster than they were built to."
    };

    public static readonly DifficultyPreset NormalPreset = new DifficultyPreset
    {
        Name = Normal,
        Description = "Hornet and the Shade both deal 20% less damage and Hornet's Bind heals one mask less on each of them, aiming to keep the vanilla difficulty curve with a second fighter on the field.",
        HornetNeedleDamage = 0.8f,
        HornetSilkSkillDamage = 0.8f,
        ShadeNailDamage = 0.8f,
        ShadeSpellDamage = 0.8f,
        BindHornetHeal = 2,
        BindShadeHeal = 1
    };

    public static readonly DifficultyPreset AbyssPreset = new DifficultyPreset
    {
        Name = Abyss,
        Description = "Needle and nail fall to 60%, the Shade carries fewer masks and its Focus no longer reaches Hornet. Demands sharper combat than vanilla Silksong, not just a longer fight.",
        HornetNeedleDamage = 0.6f,
        HornetSilkSkillDamage = 0.8f,
        ShadeNailDamage = 0.6f,
        ShadeSpellDamage = 0.8f,
        BindHornetHeal = 2,
        BindShadeHeal = 1,
        FocusHornetHeal = 0,
        ShadeMaskFraction = 0.4f
    };

    /// <summary>Every preset, in the order the menu cycles through them.</summary>
    public static readonly DifficultyPreset[] All = { EasyPreset, NormalPreset, AbyssPreset };

    /// <summary>Explanation shown for a set of values that matches no preset.</summary>
    public const string CustomDescription = "Values tuned by hand. Selecting a preset replaces every difficulty setting on this screen.";

    public void ApplyTo(ModConfig config)
    {
        if (config == null)
        {
            return;
        }

        config.hornetDamageMultiplier = HornetNeedleDamage;
        config.hornetSilkSkillDamageMultiplier = HornetSilkSkillDamage;
        config.shadeDamageMultiplier = ShadeNailDamage;
        config.shadeSpellDamageMultiplier = ShadeSpellDamage;
        config.bindHornetHeal = BindHornetHeal;
        config.bindShadeHeal = BindShadeHeal;
        config.focusHornetHeal = FocusHornetHeal;
        config.focusShadeHeal = FocusShadeHeal;
        config.shadeMaskFraction = ShadeMaskFraction;
        config.shadeFocusAtFullMasks = ShadeFocusAtFullMasks;
    }

    public bool Matches(ModConfig config)
    {
        if (config == null)
        {
            return false;
        }

        return Mathf.Approximately(config.hornetDamageMultiplier, HornetNeedleDamage)
            && Mathf.Approximately(config.hornetSilkSkillDamageMultiplier, HornetSilkSkillDamage)
            && Mathf.Approximately(config.shadeDamageMultiplier, ShadeNailDamage)
            && Mathf.Approximately(config.shadeSpellDamageMultiplier, ShadeSpellDamage)
            && config.bindHornetHeal == BindHornetHeal
            && config.bindShadeHeal == BindShadeHeal
            && config.focusHornetHeal == FocusHornetHeal
            && config.focusShadeHeal == FocusShadeHeal
            && Mathf.Approximately(config.shadeMaskFraction, ShadeMaskFraction)
            && config.shadeFocusAtFullMasks == ShadeFocusAtFullMasks;
    }

    /// <summary>The preset <paramref name="config"/> currently matches, or null for a custom set.</summary>
    public static DifficultyPreset? Identify(ModConfig config)
    {
        foreach (var preset in All)
        {
            if (preset.Matches(config))
            {
                return preset;
            }
        }

        return null;
    }

    public static string IdentifyName(ModConfig config) => Identify(config)?.Name ?? Custom;

    public static string IdentifyDescription(ModConfig config) => Identify(config)?.Description ?? CustomDescription;
}
