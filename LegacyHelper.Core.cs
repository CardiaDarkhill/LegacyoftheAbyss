#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using UnityEngine.SceneManagement;

// The version must stay PluginInfo.PLUGIN_VERSION, which BepInEx.PluginInfoProps generates from the
// csproj's <Version>. Hardcode it and every duplicate copy under BepInEx/plugins reports the same
// version, leaving BepInEx's "skip the older copy" logic nothing to compare - it then loads whichever
// its directory scan reaches last.
[BepInPlugin("com.legacyoftheabyss.helper", "Legacy of the Abyss - Helper", LegacyoftheAbyss.PluginInfo.PLUGIN_VERSION)]
public partial class LegacyHelper : BaseUnityPlugin
{
    private static GameObject helper;
    private static SimpleHUD hud;
    private static bool registeredEnterSceneHandler;
    private bool loggedMissingUI;
    private bool loggedMissingPauseMenu;
    // UIManager is DontDestroyOnLoad, so once resolved it stays valid for the session.
    // Caching it keeps Update off the per-frame full-scene scan that FindFirstObjectByType does.
    private UIManager cachedUI;

    private const float SceneSpawnProtectionSeconds = 1.5f;

    private static bool fragileGreedActive;

    internal static bool FragileGreedActive => fragileGreedActive;

    internal static void SetFragileGreedActive(bool active)
    {
        fragileGreedActive = active;
    }

    internal static LegacyHelper Instance { get; private set; }

    internal static void LogInfo(string message) => Instance?.Logger?.LogInfo(message);

    internal static void LogWarning(string message) => Instance?.Logger?.LogWarning(message);

    // Persist shade state across scene transitions
    internal static bool HasSavedShadeState => ShadeRuntime.PersistentState.HasData;

    internal static void SaveShadeState(int curHp, int maxHp, int lifebloodCur, int lifebloodMax, int soul, bool? canTakeDamage = null, int? baseMaxHp = null)
    {
        ShadeRuntime.CaptureState(curHp, maxHp, lifebloodCur, lifebloodMax, soul, canTakeDamage, baseMaxHp);
    }

    // Called when Hornet gains a new spell. Advances Shade's unlock/upgrade track.
    internal static void NotifyHornetSpellUnlocked()
    {
        ShadeRuntime.NotifyHornetSpellUnlocked();
    }

    internal static float GetEffectiveSfxVolume()
    {
        try
        {
            var gm = GameManager.instance;
            if (gm != null)
            {
                var settings = gm.gameSettings;
                if (settings != null)
                {
                    float master = Mathf.Clamp01(settings.masterVolume / 10f);
                    float sound = Mathf.Clamp01(settings.soundVolume / 10f);
                    return Mathf.Clamp01(master * sound);
                }
            }
        }
        catch
        {
        }

        return 1f;
    }

    /// <summary>
    /// <c>Harmony.PatchAll</c>, but a patch class that fails takes only itself down.
    /// <para>
    /// The stock call processes every annotated class and rethrows the first failure straight out of
    /// <c>Awake</c>, which leaves the plugin dead: no HUD, no Shade, no bug reporter, nothing. That
    /// is a wildly disproportionate outcome for one bad patch, and it happened - a single
    /// <c>AmbiguousMatchException</c> from patching an overloaded method by name cost the mod every
    /// other patch it has. Losing one feature and logging why is always the better failure.
    /// </para>
    /// <para>
    /// The two <c>Apply</c> calls below are separate for the same reason, and predate this.
    /// </para>
    /// </summary>
    private void PatchAllTolerantly(Harmony harmony)
    {
        int patched = 0;
        int failed = 0;

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            try
            {
                var processor = harmony.CreateClassProcessor(type);
                if (processor == null)
                {
                    continue;
                }

                var result = processor.Patch();
                if (result != null && result.Count > 0)
                {
                    patched++;
                }
            }
            catch (Exception ex)
            {
                failed++;
                try
                {
                    Logger?.LogError($"Harmony patch class '{type?.FullName}' failed and was skipped: {ex.Message}");
                }
                catch
                {
                }
            }
        }

        if (failed > 0)
        {
            try { Logger?.LogWarning($"Harmony: {patched} patch class(es) applied, {failed} skipped after errors."); }
            catch { }
        }
    }

    private void Awake()
    {
        Instance = this;
        ModConfig.Load();
        LoggingManager.Initialize(Logger);
        LegacyoftheAbyss.Diagnostics.BugReportSystem.Install(Logger);
        var harmony = new Harmony("com.legacyoftheabyss.helper");
        PatchAllTolerantly(harmony);

        // After PatchAll, never inside it - see the remarks on EnemyAiRetargeting. A throw from this
        // one must not be able to cost the rest of the mod its patches.
        EnemyAiRetargeting.Apply(harmony);
        ShadeGrabRetargeting.Apply(harmony);

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            foreach (var go in scene.GetRootGameObjects())
            {
                var name = go.name.ToLowerInvariant();
                if (name.Contains("team cherry") || (name.Contains("save") && name.Contains("reminder")))
                    go.SetActive(false);
            }
        };

    }

    private void Update()
    {
        // While a bug report is being composed the keyboard belongs to that overlay. Without this
        // gate, typing the message also toggles damage logging and dumps Hornet's position for
        // every stray backtick or F1 in it.
        if (!LegacyoftheAbyss.Diagnostics.BugReportSystem.IsCapturingText)
        {
            LoggingManager.Update();
            HandleDebugInput();
        }

        var ui = cachedUI;
        if (ui == null)
        {
            ui = UnityEngine.Object.FindFirstObjectByType<UIManager>();
            cachedUI = ui;
        }

        if (ui == null)
        {
            if (!loggedMissingUI)
            {
                if (ModConfig.Instance.logGeneral)
                    Logger.LogInfo("UIManager not found yet");
                loggedMissingUI = true;
            }
            return;
        }
        if (ui.pauseMenuScreen == null)
        {
            if (!loggedMissingPauseMenu)
            {
                if (ModConfig.Instance.logGeneral)
                    Logger.LogInfo("pauseMenuScreen not available yet");
                loggedMissingPauseMenu = true;
            }
            return;
        }
        loggedMissingUI = false;
        loggedMissingPauseMenu = false;
        ShadeSettingsMenu.Inject(ui);
    }

    private void HandleDebugInput()
    {
        try
        {
            if (!Input.GetKeyDown(KeyCode.BackQuote))
            {
                return;
            }

            bool ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (ctrlHeld)
            {
                bool unlocked = ShadeRuntime.ToggleDebugUnlockAllCharms();
                if (unlocked)
                {
                    Logger?.LogInfo("Shade debug: temporarily unlocked all shade charms.");
                }
                else
                {
                    Logger?.LogInfo("Shade debug: restored shade charm unlock state.");
                }

                return;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                var gm = GameManager.instance;
                var hero = gm == null ? null : gm.hero_ctrl;
                if (hero == null)
                {
                    Logger?.LogWarning("Hornet location requested, but the hero controller is unavailable.");
                    return;
                }

                Vector3 position = hero.transform.position;
                string sceneName = SceneManager.GetActiveScene().name;
                Logger?.LogInfo($"Hornet location ({sceneName}): X={position.x:F3}, Y={position.y:F3}, Z={position.z:F3}");
                return;
            }
        }
        catch (Exception ex)
        {
            Logger?.LogWarning($"Shade debug toggle failed: {ex}");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        LegacyoftheAbyss.Diagnostics.BugReportSystem.Shutdown();
        LoggingManager.Flush();
        ModConfig.Save();
    }

    // no cached UI needed; menu injects when available
    

    private static void HandleFinishedEnteringScene()
    {
        try
        {
            var gm = GameManager.instance;
            if (gm == null)
            {
                return;
            }

            ShadeRuntime.SyncActiveSlot(gm);

            string sceneName = SceneManager.GetActiveScene().name;
            ShadeRuntime.HandleSceneEntered(sceneName);

            if (gm.hero_ctrl == null)
            {
                return;
            }
            Vector3 spawnPosAtControl = gm.hero_ctrl.transform.position;
            SpawnShadeAtPosition(spawnPosAtControl);
            ShadeCharmPlacer.PopulateScene(sceneName, gm.hero_ctrl.transform);
        }
        catch { }
    }

    /// <summary>
    /// Retires a Shade's decoded sprite sheets: destroys the sprites first, then the textures they
    /// were cut from, after a short grace period.
    /// <para>
    /// This lives on the plugin rather than on ShadeController for one reason - lifetime.
    /// <c>Object.Destroy(obj, delay)</c> and any coroutine started on the Shade are both bound to
    /// things that die with the scene, so a Shade destroyed as part of a scene unload had its pending
    /// texture destroys silently dropped. That is the common case, not the rare one: the Shade is
    /// respawned on every scene change, so each transition leaked a full sheet set. LegacyHelper is
    /// the BepInEx plugin behaviour and survives scene loads, so its coroutine always gets to finish.
    /// </para>
    /// <para>
    /// The grace period is there because VFX the Shade spawned - projectiles, slam auras - can outlive
    /// it by a moment and are still drawing these sprites.
    /// </para>
    /// </summary>
    internal static void RetireShadeSpriteAssets(List<Texture2D> textures, IEnumerable<Sprite[]> spriteGroups, float delay)
    {
        var retiredTextures = new List<Texture2D>();
        if (textures != null)
        {
            foreach (var texture in textures)
            {
                if (texture != null)
                {
                    retiredTextures.Add(texture);
                }
            }
        }

        var retiredSprites = new List<Sprite>();
        if (spriteGroups != null)
        {
            foreach (var group in spriteGroups)
            {
                if (group == null)
                {
                    continue;
                }

                foreach (var sprite in group)
                {
                    if (sprite != null)
                    {
                        retiredSprites.Add(sprite);
                    }
                }
            }
        }

        if (retiredTextures.Count == 0 && retiredSprites.Count == 0)
        {
            return;
        }

        var host = Instance;
        if (host == null)
        {
            // No plugin behaviour to run the coroutine on: better to free immediately than to leak.
            DestroyRetiredShadeSpriteAssets(retiredTextures, retiredSprites);
            return;
        }

        try
        {
            host.StartCoroutine(RetireShadeSpriteAssetsRoutine(retiredTextures, retiredSprites, delay));
        }
        catch
        {
            DestroyRetiredShadeSpriteAssets(retiredTextures, retiredSprites);
        }
    }

    private static IEnumerator RetireShadeSpriteAssetsRoutine(List<Texture2D> textures, List<Sprite> sprites, float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        DestroyRetiredShadeSpriteAssets(textures, sprites);
    }

    private static void DestroyRetiredShadeSpriteAssets(List<Texture2D> textures, List<Sprite> sprites)
    {
        int destroyedSprites = 0;
        int destroyedTextures = 0;

        // Sprites first: each one holds a reference to its source texture.
        if (sprites != null)
        {
            foreach (var sprite in sprites)
            {
                if (sprite == null) continue;
                UnityEngine.Object.Destroy(sprite);
                destroyedSprites++;
            }

            sprites.Clear();
        }

        if (textures != null)
        {
            foreach (var texture in textures)
            {
                if (texture == null) continue;
                UnityEngine.Object.Destroy(texture);
                destroyedTextures++;
            }

            textures.Clear();
        }

        if ((destroyedTextures > 0 || destroyedSprites > 0) && ModConfig.Instance.logShade)
        {
            LogInfo($"Released retired shade art: {destroyedTextures} textures, {destroyedSprites} sprites.");
        }
    }

    private static void DestroyShadeInstance()
    {
        if (helper == null)
            return;

        try
        {
            UnityEngine.Object.Destroy(helper);
        }
        catch
        {
        }

        helper = null;
    }

    private static void SpawnShadeAtPosition(Vector3 pos)
    {
        var gm = GameManager.instance;
        if (gm == null || gm.hero_ctrl == null) return;

        if (!ModConfig.Instance.shadeEnabled)
        {
            DestroyShadeInstance();
            return;
        }

        if (helper != null)
        {
            try
            {
                var sc = helper.GetComponent<ShadeController>();
                if (sc != null)
                {
                    sc.TeleportToPosition(pos);
                    // Hornet's renderer is a new instance in the new scene, so the layer/material
                    // the Shade inherited from the previous one has to be re-derived.
                    sc.ApplyRenderingSettings();
                    sc.SuppressHazardDamage(SceneSpawnProtectionSeconds);
                    sc.ApplySceneTransitionProtection(SceneSpawnProtectionSeconds);
                    sc.TriggerSpawnEntrance();
                    SaveShadeState(sc.GetCurrentNormalHP(), sc.GetMaxNormalHP(), sc.GetCurrentLifeblood(), sc.GetMaxLifeblood(), sc.GetShadeSoul(), sc.GetCanTakeDamage(), sc.GetBaseMaxHP());
                    RequestShadeLoadoutRecompute();
                }
                else
                {
                    helper.transform.position = pos;
                }
            }
            catch { }
            return;
        }

        // Create fresh helper at the captured position
        helper = new GameObject("HelperShade");
        try { helper.tag = "Recoiler"; } catch { }
        helper.transform.position = pos;

        var scNew = helper.AddComponent<ShadeController>();
        scNew.Init(gm.hero_ctrl.transform);
        if (ShadeRuntime.TryGetPersistentState(out var savedHp, out var savedMax, out var savedLifeblood, out var savedLifebloodMax, out var savedSoul, out var savedCanTakeDamage, out var savedBaseMax))
        {
            scNew.RestorePersistentState(savedHp, savedMax, savedBaseMax, savedLifeblood, savedLifebloodMax, savedSoul, savedCanTakeDamage);
        }

        scNew.SuppressHazardDamage(SceneSpawnProtectionSeconds);
        scNew.ApplySceneTransitionProtection(SceneSpawnProtectionSeconds);

        var sr = helper.AddComponent<SpriteRenderer>();
        ApplyShadeSpriteRendering(sr);

        scNew.TriggerSpawnEntrance();
        RequestShadeLoadoutRecompute();
    }

    internal static void SetShadeEnabled(bool enabled)
    {
        if (ModConfig.Instance.shadeEnabled == enabled)
        {
            ShadeSettingsMenu.NotifyShadeToggleChanged();
            return;
        }

        ModConfig.Instance.shadeEnabled = enabled;

        if (!enabled)
        {
            DestroyShadeInstance();
            if (hud != null)
            {
                hud.SetVisible(false);
            }
        }
        else
        {
            var gm = GameManager.instance;
            if (gm != null && gm.hero_ctrl != null)
            {
                SpawnShadeAtPosition(gm.hero_ctrl.transform.position);
            }
            if (hud != null)
            {
                hud.SetVisible(gm != null && gm.IsGameplayScene());
            }
        }

        ShadeSettingsMenu.NotifyShadeToggleChanged();
        ModConfig.Save();
    }

    /// <summary>
    /// Applies <paramref name="skinId"/> and refreshes the live Shade's sprites in place, so the
    /// change is visible as soon as the pause menu closes.
    /// </summary>
    internal static void SetShadeSkin(string skinId)
    {
        if (!ShadeSkinManager.SelectSkin(skinId))
        {
            return;
        }

        RefreshShadeSkin();
    }

    internal static void RefreshShadeSkin()
    {
        try
        {
            if (helper != null)
            {
                var controller = helper.GetComponent<ShadeController>();
                controller?.ReloadSkinSprites();
            }
        }
        catch
        {
        }
    }

    internal static void RequestShadeLoadoutRecompute()
    {
        try
        {
            if (helper != null)
            {
                var controller = helper.GetComponent<ShadeController>();
                if (controller != null)
                {
                    controller.QueueCharmLoadoutRecompute();
                }
            }
        }
        catch (Exception e)
        {
            if (ModConfig.Instance.logGeneral)
            {
                Instance?.Logger?.LogWarning($"Failed to recompute shade charm loadout: {e}");
            }
        }

        ShadeSettingsMenu.NotifyCharmLoadoutChanged();
    }

    internal static bool TryGetShadeController(out ShadeController controller)
    {
        controller = helper != null ? helper.GetComponent<ShadeController>() : null;
        return controller != null;
    }

    /// <summary>
    /// Switches off the studio logos and the save-reminder prompt, found by name because the game
    /// exposes no single flag for them. Matching by name is a heuristic, so nothing here is required
    /// to hit: a rename costs the skipped intro, not the mod.
    /// </summary>
    internal static void DisableStartup(GameManager gm)
    {
        if (gm == null) return;

        foreach (var f in gm.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (f.FieldType != typeof(bool) || f.IsInitOnly) continue;

            var name = f.Name.ToLower();
            if (name.Contains("logo") || (name.Contains("save") && name.Contains("reminder")))
            {
                f.SetValue(gm, false);
            }
        }
    }

}
#nullable restore

