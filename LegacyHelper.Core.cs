using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
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

    // Persist shade state across scene transitions
    internal static int savedShadeHP = -1;
    internal static int savedShadeMax = -1;
    internal static int savedShadeSoul = -1;
    internal static int savedShadeSpellProgress = 0; // 0..6 progression
    internal static bool savedShadeCanTakeDamage = true;
    internal static bool HasSavedShadeState => savedShadeMax > 0;

    internal static void SaveShadeState(int curHp, int maxHp, int soul, bool? canTakeDamage = null)
    {
        savedShadeMax = Mathf.Max(1, maxHp);
        savedShadeHP = Mathf.Clamp(curHp, 0, savedShadeMax);
        savedShadeSoul = Mathf.Max(0, soul);
        if (canTakeDamage.HasValue) savedShadeCanTakeDamage = canTakeDamage.Value;
    }

    // Called when Hornet gains a new spell. Advances Shade's unlock/upgrade track.
    internal static void NotifyHornetSpellUnlocked()
    {
        savedShadeSpellProgress = Mathf.Clamp(savedShadeSpellProgress + 1, 0, 6);
    }

    private void Awake()
    {
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

    private void OnDestroy()
    {
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
                    sc.TriggerSpawnEntrance();
                    SaveShadeState(sc.GetCurrentHP(), sc.GetMaxHP(), sc.GetShadeSoul(), sc.GetCanTakeDamage());
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
        if (HasSavedShadeState)
        {
            scNew.RestorePersistentState(savedShadeHP, savedShadeMax, savedShadeSoul, savedShadeCanTakeDamage);
        }

        var sr = helper.AddComponent<SpriteRenderer>();

        var hornetRenderer = gm.hero_ctrl.GetComponentInChildren<SpriteRenderer>();
        if (hornetRenderer != null)
        {
            sr.sortingLayerID = hornetRenderer.sortingLayerID;
            sr.sortingOrder = hornetRenderer.sortingOrder + 1;
        }

        scNew.TriggerSpawnEntrance();
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
