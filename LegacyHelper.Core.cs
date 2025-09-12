using System;
using System.Collections;
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

    // Persist shade state across scene transitions
    internal static int savedShadeHP = -1;
    internal static int savedShadeMax = -1;
    internal static int savedShadeSoul = -1;
    internal static bool HasSavedShadeState => savedShadeMax > 0;

    internal static void SaveShadeState(int curHp, int maxHp, int soul)
    {
        savedShadeMax = Mathf.Max(1, maxHp);
        savedShadeHP = Mathf.Clamp(curHp, 0, savedShadeMax);
        savedShadeSoul = Mathf.Max(0, soul);
    }

    private void Awake()
    {
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
    

    private static void HandleFinishedEnteringScene()
    {
        try
        {
            var gm = GameManager.instance;
            if (gm == null || gm.hero_ctrl == null) return;
            Vector3 spawnPosAtControl = gm.hero_ctrl.transform.position;
            gm.StartCoroutine(SpawnShadeAfterDelay(spawnPosAtControl, 0.5f));
        }
        catch { }
    }

    private static IEnumerator SpawnShadeAfterDelay(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);
        var gm = GameManager.instance;
        if (gm == null || gm.hero_ctrl == null) yield break;

        if (helper != null)
        {
            try
            {
                var sc = helper.GetComponent<ShadeController>();
                if (sc != null)
                {
                    sc.TeleportToPosition(pos);
                    SaveShadeState(sc.GetCurrentHP(), sc.GetMaxHP(), sc.GetShadeSoul());
                }
                else
                {
                    helper.transform.position = pos;
                }
            }
            catch { }
            yield break;
        }

        // Create fresh helper at the captured position
        helper = new GameObject("HelperShade");
        try { helper.tag = "Recoiler"; } catch { }
        helper.transform.position = pos;

        var scNew = helper.AddComponent<ShadeController>();
        scNew.Init(gm.hero_ctrl.transform);
        if (HasSavedShadeState)
        {
            scNew.RestorePersistentState(savedShadeHP, savedShadeMax, savedShadeSoul);
        }

        var sr = helper.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateDebugSprite();
        sr.color = Color.black;

        var hornetRenderer = gm.hero_ctrl.GetComponentInChildren<SpriteRenderer>();
        if (hornetRenderer != null)
        {
            sr.sortingLayerID = hornetRenderer.sortingLayerID;
            sr.sortingOrder = hornetRenderer.sortingOrder + 1;
        }
    }

    private static Sprite GenerateDebugSprite()
    {
        var tex = new Texture2D(160, 160);
        for (int x = 0; x < 160; x++)
            for (int y = 0; y < 160; y++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 160, 160), new Vector2(0.5f, 0.5f));
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
