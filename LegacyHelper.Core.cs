#nullable disable
using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using UnityEngine.SceneManagement;

[BepInPlugin("com.legacyoftheabyss.helper", "Legacy of the Abyss - Helper", "0.1.0")]
public partial class LegacyHelper : BaseUnityPlugin
{
    private static GameObject helper;
    private static bool loggedStartupFields;
    private static SimpleHUD hud;
    private static bool registeredEnterSceneHandler;
    private bool loggedMissingUI;
    private bool loggedMissingPauseMenu;

    private const float SceneSpawnProtectionSeconds = 1.5f;

    internal static LegacyHelper Instance { get; private set; }

    // Persist shade state across scene transitions
    internal static bool HasSavedShadeState => ShadeRuntime.PersistentState.HasData;

    internal static void SaveShadeState(int curHp, int maxHp, int soul, bool? canTakeDamage = null)
    {
        ShadeRuntime.CaptureState(curHp, maxHp, soul, canTakeDamage);
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

    private void Awake()
    {
        Instance = this;
        ModConfig.Load();
        LoggingManager.Initialize();
        var harmony = new Harmony("com.legacyoftheabyss.helper");
        harmony.PatchAll();

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
        LoggingManager.Update();
        HandleDebugInput();
        var ui = UnityEngine.Object.FindFirstObjectByType<UIManager>();
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

            if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
            {
                return;
            }

            bool unlocked = ShadeRuntime.ToggleDebugUnlockAllCharms();
            if (unlocked)
            {
                Logger?.LogInfo("Shade debug: temporarily unlocked all shade charms.");
            }
            else
            {
                Logger?.LogInfo("Shade debug: restored shade charm unlock state.");
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

        LoggingManager.Flush();
        ModConfig.Save();
    }

    // no cached UI needed; menu injects when available
    

    private static void HandleFinishedEnteringScene()
    {
        try
        {
            var gm = GameManager.instance;
            if (gm == null || gm.hero_ctrl == null) return;
            Vector3 spawnPosAtControl = gm.hero_ctrl.transform.position;
            SpawnShadeAtPosition(spawnPosAtControl);
            ShadeCharmPlacer.PopulateScene(SceneManager.GetActiveScene().name, gm.hero_ctrl.transform);
        }
        catch { }
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
                    sc.SuppressHazardDamage(SceneSpawnProtectionSeconds);
                    sc.ApplySceneTransitionProtection(SceneSpawnProtectionSeconds);
                    sc.TriggerSpawnEntrance();
                    SaveShadeState(sc.GetCurrentHP(), sc.GetMaxHP(), sc.GetShadeSoul(), sc.GetCanTakeDamage());
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
        if (ShadeRuntime.TryGetPersistentState(out var savedHp, out var savedMax, out var savedSoul, out var savedCanTakeDamage))
        {
            scNew.RestorePersistentState(savedHp, savedMax, savedSoul, savedCanTakeDamage);
        }

        scNew.SuppressHazardDamage(SceneSpawnProtectionSeconds);
        scNew.ApplySceneTransitionProtection(SceneSpawnProtectionSeconds);

        var sr = helper.AddComponent<SpriteRenderer>();

        var hornetRenderer = gm.hero_ctrl.GetComponentInChildren<SpriteRenderer>();
        if (hornetRenderer != null)
        {
            sr.sortingLayerID = hornetRenderer.sortingLayerID;
            sr.sortingOrder = hornetRenderer.sortingOrder + 1;
        }

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
                try { hud.SetVisible(false); }
                catch { }
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
                try
                {
                    bool gameplay = gm != null && gm.IsGameplayScene();
                    hud.SetVisible(gameplay);
                }
                catch
                {
                }
            }
        }

        ShadeSettingsMenu.NotifyShadeToggleChanged();
        ModConfig.Save();
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

    internal static void DisableStartup(GameManager gm)
    {
        if (gm == null) return;
        var fields = gm.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        bool log = !loggedStartupFields;
        loggedStartupFields = true;
        foreach (var f in fields)
        {
            if (f.FieldType != typeof(bool)) continue;
            var name = f.Name.ToLower();
            if (name.Contains("logo") || (name.Contains("save") && name.Contains("reminder")))
            {
                try { f.SetValue(gm, false); } catch { }
            }
        }
    }
}
#nullable restore

