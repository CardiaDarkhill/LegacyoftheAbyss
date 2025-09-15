using System.Collections;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class LegacyHelper
{
    [HarmonyPatch(typeof(GameManager), "BeginScene")]
    private class GameManager_BeginScene_Patch
    {
        private static void Postfix(GameManager __instance)
        {
            DisableStartup(__instance);
            bool gameplay = __instance.IsGameplayScene();
            if (hud != null)
            {
                try { hud.SetVisible(gameplay); } catch { }
            }
            if (!gameplay) return;

            if (!registeredEnterSceneHandler)
            {
                try
                {
                    __instance.OnFinishedEnteringScene += HandleFinishedEnteringScene;
                    registeredEnterSceneHandler = true;
                }
                catch { }
            }

            if (hud == null)
            {
                var hudGO = new UnityEngine.GameObject("SimpleHUD");
                UnityEngine.Object.DontDestroyOnLoad(hudGO);
                hud = hudGO.AddComponent<SimpleHUD>();
                hud.Init(__instance.playerData);
            }
            else
            {
                try { hud.SetPlayerData(__instance.playerData); } catch { }
            }
        }
    }

    // Revive shade (at least 1 HP) when Hornet dies
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.PlayerDead))]
    private class GameManager_PlayerDead_Patch
    {
        private static void Postfix(GameManager __instance)
        {
            try
            {
                if (helper != null)
                {
                    var sc = helper.GetComponent<ShadeController>();
                    if (sc != null)
                    {
                        sc.ReviveToAtLeast(1);
                        SaveShadeState(sc.GetCurrentHP(), sc.GetMaxHP(), sc.GetShadeSoul(), sc.GetCanTakeDamage());
                        return;
                    }
                }
                // Fallback: ensure saved state revives next spawn
                if (savedShadeMax > 0)
                {
                    savedShadeHP = Mathf.Max(savedShadeHP, 1);
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(GameManager), "Awake")]
    private class GameManager_Awake_Patch
    {
        private static void Postfix(GameManager __instance)
        {
            // New GameManager instance; ensure we re-register scene-enter handler next time.
            registeredEnterSceneHandler = false;
            DisableStartup(__instance);
        }
    }

    [HarmonyPatch(typeof(GameManager), "Start")]
    private class GameManager_Start_Patch
    {
        private static void Postfix(GameManager __instance) => DisableStartup(__instance);
    }

    [HarmonyPatch(typeof(StartManager), "Start")]
    private class StartManager_Start_Enumerator_Patch
    {
        private static void Prefix(StartManager __instance)
        {
            if (__instance.startManagerAnimator != null)
                __instance.startManagerAnimator.SetBool("WillShowQuote", false);
        }

        private static void Postfix(StartManager __instance, ref IEnumerator __result)
        {
            if (__result == null) return;
            var fields = __result.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(bool) && f.Name.Contains("showIntroSequence"))
                {
                    f.SetValue(__result, false);
                    if (__instance.startManagerAnimator != null)
                        __instance.startManagerAnimator.Play("LoadingIcon", 0, 1f);
                    break;
                }
            }
        }
    }

    [HarmonyPatch(typeof(RestBenchHelper), "SetOnBench")]
    private class RestBenchHelper_SetOnBench_Patch
    {
        private static void Postfix(bool onBench)
        {
            if (!onBench) return;
            try
            {
                if (helper != null)
                {
                    var sc = helper.GetComponent<ShadeController>();
                    if (sc != null)
                    {
                        sc.FullHealFromBench();
                        SaveShadeState(sc.GetCurrentHP(), sc.GetMaxHP(), sc.GetShadeSoul(), sc.GetCanTakeDamage());
                    }
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(DamageEnemies), "Start")]
    private class DamageEnemies_Start_Mod
    {
        private static void Postfix(DamageEnemies __instance)
        {
            try
            {
                var t = typeof(DamageEnemies);
                bool src = false; bool hero = false;
                try { src = (bool)(t.GetField("sourceIsHero", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(__instance) ?? false); } catch { }
                try { hero = (bool)(t.GetField("isHeroDamage", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(__instance) ?? false); } catch { }
                if (src || hero)
                {
                    __instance.damageDealt = Mathf.Max(1, Mathf.RoundToInt(__instance.damageDealt * ModConfig.Instance.hornetDamageMultiplier));
                }
            }
            catch { }
        }
    }

    // Trigger shade heal on explicit Bind completion event
    [HarmonyPatch(typeof(HeroController), "BindCompleted")]
    private class HeroController_BindCompleted_Patch
    {
        private static void Prefix(HeroController __instance, out int __state)
        {
            __state = 0;
            try { if (__instance != null && __instance.playerData != null) __state = __instance.playerData.health; } catch { }
        }

        private static void Postfix(HeroController __instance, int __state)
        {
            try
            {
                var pd = __instance?.playerData;
                if (pd != null)
                {
                    int healed = pd.health - __state;
                    int desired = ModConfig.Instance.bindHornetHeal;
                    if (healed != desired)
                    {
                        pd.health = Mathf.Clamp(__state + desired, 0, pd.maxHealth);
                    }
                }

                if (helper != null)
                {
                    var sc = helper.GetComponent<ShadeController>();
                    if (sc != null)
                    {
                        sc.ApplyBindHealFromHornet(__instance != null ? __instance.transform : null);
                    }
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(InputHandler), "MapKeyboardLayoutFromGameSettings")]
    private class BlockKeyboardRebinding
    {
        private static bool Prefix() => false; // skip
    }

    [HarmonyPatch(typeof(InputHandler), "MapDefaultKeyboardLayout")]
    private class BlockDefaultKeyboardMap
    {
        private static bool Prefix() => false; // skip
    }

    // When a SpellGetOrb completes collection (appears during spell acquisition sequences),
    // advance shade spell progression.
    [HarmonyPatch(typeof(SpellGetOrb), "Collect")]
    private class SpellGetOrb_Collect_Patch
    {
        private static void Postfix()
        {
            try { NotifyHornetSpellUnlocked(); } catch { }
        }
    }

    [HarmonyPatch(typeof(NailSlash), "Awake")]
    private class NailSlash_Awake_Log
    {
        private static bool Prefix(NailSlash __instance)
        {
            if (ShadeController.suppressActivateOnSlash)
            {
                Transform parent = __instance.transform.parent;
                if (parent != ShadeController.expectedSlashParent)
                {
                    Object.Destroy(__instance.gameObject);
                    return false;
                }
            }
            return true;
        }

        private static void Postfix(NailSlash __instance)
        {
            try
            {
                var tr = __instance.transform;
                var scale = tr.localScale;
                string parent = tr.parent ? tr.parent.name : "(null)";
                UnityEngine.Debug.Log($"[ShadeDebug] NailSlash spawned: {__instance.name} scale={scale} parent={parent}\n{System.Environment.StackTrace}");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.Log($"[ShadeDebug] NailSlash log error: {ex}");
            }
        }
    }

    [HarmonyPatch(typeof(NailSlash), nameof(NailSlash.StartSlash))]
    private class NailSlash_StartSlash_Log
    {
        private static void Postfix(NailSlash __instance)
        {
            try
            {
                bool isShade = __instance.GetComponent("ShadeSlashMarker") != null;
                string owner = isShade ? "Shade" : "Hornet";
                string parent = __instance.transform.parent ? __instance.transform.parent.name : "(null)";
            }
            catch { }
        }
    }

    // Prevent shade slashes from triggering Hornet pogo/bounce logic
    [HarmonyPatch(typeof(NailSlash), "DoDownspikeBounce")]
    private class NailSlash_DoDownspikeBounce_Block
    {
        private static bool Prefix(NailSlash __instance)
        {
            return __instance.transform.GetComponentInParent<ShadeController>() == null;
        }
    }

    [HarmonyPatch(typeof(NailSlash), "DownBounce")]
    private class NailSlash_DownBounce_Block
    {
        private static bool Prefix(NailSlash __instance)
        {
            return __instance.transform.GetComponentInParent<ShadeController>() == null;
        }
    }

    [HarmonyPatch(typeof(NailSlash), nameof(NailSlash.QueueBounce))]
    private class NailSlash_QueueBounce_Block
    {
        private static bool Prefix(NailSlash __instance)
        {
            return __instance.transform.GetComponentInParent<ShadeController>() == null;
        }
    }

    [HarmonyPatch(typeof(UIManager), nameof(UIManager.ShowMenu))]
    private class UIManager_ShowMenu_AddShadeButton
    {
        private static IEnumerator Postfix(IEnumerator __result, UIManager __instance, MenuScreen menu)
        {
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }
            try
            {
                if (menu == __instance.pauseMenuScreen)
                {
                    ShadeSettingsMenu.Inject(__instance);
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(PauseMenuButton), "OnSubmit")]
    private class PauseMenuButton_OnSubmit_Shade
    {
        private static bool Prefix(PauseMenuButton __instance, BaseEventData eventData)
        {
            if (__instance != null && __instance.gameObject.name == "ShadeSettingsButton")
            {
                try
                {
                    var ui = UIManager.instance;
                    if (ui != null)
                        ui.StartCoroutine(ShadeSettingsMenu.Show(ui));
                }
                catch { }
                return false;
            }
            return true;
        }
    }
}
