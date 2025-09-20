#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using InControl;
using LegacyoftheAbyss.Shade;
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
                try { hud.SetVisible(gameplay && ModConfig.Instance.shadeEnabled); } catch { }
            }
            if (!gameplay)
            {
                DestroyShadeInstance();
                return;
            }

            if (!ModConfig.Instance.shadeEnabled)
            {
                DestroyShadeInstance();
                return;
            }

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
                if (!ModConfig.Instance.shadeEnabled)
                    return;

                if (ModConfig.Instance.shadeEnabled && helper != null)
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
                if (ShadeRuntime.PersistentState.HasData)
                {
                    ShadeRuntime.EnsureMinimumHealth(1);
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(UIManager), nameof(UIManager.TogglePauseGame))]
    private class UIManager_TogglePauseGame_Patch
    {
        private static bool Prefix(UIManager __instance)
        {
            try
            {
                if (ShadeSettingsMenu.HandlePauseToggle(__instance))
                    return false;
            }
            catch { }
            return true;
        }
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.PauseGameToggle))]
    private class GameManager_PauseGameToggle_Patch
    {
        private static bool Prefix(GameManager __instance, ref IEnumerator __result)
        {
            try
            {
                var ui = UIManager.instance;
                if (ui == null && __instance != null)
                    ui = __instance.ui;
                if (ui != null && ShadeSettingsMenu.HandlePauseToggle(ui))
                {
                    __result = Skip();
                    return false;
                }
            }
            catch { }
            return true;
        }

        private static IEnumerator Skip()
        {
            yield break;
        }
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.PauseGameToggleByMenu))]
    private class GameManager_PauseGameToggleByMenu_Patch
    {
        private static bool Prefix(GameManager __instance, ref IEnumerator __result)
        {
            try
            {
                var ui = UIManager.instance;
                if (ui == null && __instance != null)
                    ui = __instance.ui;
                if (ui != null && ShadeSettingsMenu.HandlePauseToggle(ui))
                {
                    __result = Skip();
                    return false;
                }
            }
            catch { }
            return true;
        }

        private static IEnumerator Skip()
        {
            yield break;
        }
    }

    [HarmonyPatch(typeof(UIManager), nameof(UIManager.ShowMenu))]
    private class UIManager_ShowMenu_Patch
    {
        private static bool Prefix(UIManager __instance, MenuScreen menu, ref IEnumerator __result)
        {
            try
            {
                if (!ShadeSettingsMenu.IsShowing || menu == null || __instance == null)
                    return true;

                if (menu == __instance.pauseMenuScreen || menu == __instance.optionsMenuScreen || menu == __instance.gameOptionsMenuScreen)
                {
                    __result = EmptyEnumerator();
                    return false;
                }
            }
            catch { }
            return true;
        }

        private static IEnumerator EmptyEnumerator()
        {
            yield break;
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

    [HarmonyPatch]
    private class InventoryPaneList_EnsureShadePane_Patch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var type = typeof(InventoryPaneList);
            if (type == null)
            {
                yield break;
            }

            string[] candidates = { "Awake", "Start", "OnEnable" };
            foreach (string name in candidates)
            {
                var method = AccessTools.Method(type, name);
                if (method != null)
                {
                    yield return method;
                }
            }
        }

        private static void Postfix(InventoryPaneList __instance)
        {
            if (__instance == null)
            {
                return;
            }

            try
            {
                ShadeInventoryPaneIntegration.EnsurePane(__instance);
            }
            catch (Exception ex)
            {
                if (ModConfig.Instance.logMenu)
                {
                    try { Debug.LogWarning($"[ShadeInventory] Failed to ensure shade pane: {ex}"); }
                    catch { }
                }
            }
        }
    }

    [HarmonyPatch(typeof(InventoryPaneInput), "PressSubmit")]
    private class InventoryPaneInput_PressSubmit_Shade
    {
        private static bool Prefix(InventoryPaneInput __instance)
        {
            try
            {
                var shadePane = ShadeInventoryPaneIntegration.TryGetShadePane(__instance);
                if (shadePane != null)
                {
                    shadePane.HandleSubmit();
                    return false;
                }
            }
            catch { }

            return true;
        }
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
                if (!ModConfig.Instance.shadeEnabled)
                    return;

                if (ModConfig.Instance.shadeEnabled && helper != null)
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

                if (ModConfig.Instance.shadeEnabled && helper != null)
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
        private static bool Prefix()
        {
            try
            {
                var cfg = ModConfig.Instance;
                return cfg != null && cfg.hornetKeyboardEnabled;
            }
            catch
            {
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), "MapDefaultKeyboardLayout")]
    private class BlockDefaultKeyboardMap
    {
        private static bool Prefix()
        {
            try
            {
                var cfg = ModConfig.Instance;
                return cfg != null && cfg.hornetKeyboardEnabled;
            }
            catch
            {
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), "MapControllerButtons")]
    private class ControlControllerMapping
    {
        private static bool Prefix()
        {
            try
            {
                var cfg = ModConfig.Instance;
                return cfg == null || cfg.hornetControllerEnabled;
            }
            catch
            {
                return true;
            }
        }
    }

    private static class InputDeviceBlocker
    {
        private static readonly HashSet<InputDevice> trackedShadeDevices = new();
        private static readonly List<InputDevice> cleanupList = new();

        private static void UpdateDeviceState(InputHandler handler, InputDevice device, bool exclude)
        {
            if (device == null || device == InputDevice.Null)
                return;

            try
            {
                var actions = handler != null ? handler.inputActions : null;
                if (actions != null)
                {
                    var list = actions.ExcludeDevices;
                    if (list != null)
                    {
                        bool contains = list.Contains(device);
                        if (exclude)
                        {
                            if (!contains)
                                list.Add(device);
                            if (actions.Device == device)
                                actions.Device = null;
                        }
                        else if (contains)
                        {
                            list.Remove(device);
                        }
                    }
                }
            }
            catch
            {
            }

            ApplyPassive(device, exclude);
        }

        private static void ApplyPassive(InputDevice device, bool passive)
        {
            if (device == null || device == InputDevice.Null)
                return;

            try
            {
                if (passive)
                {
                    bool added = trackedShadeDevices.Add(device);
                    if (added || !device.Passive)
                    {
                        device.Passive = true;
                    }
                }
                else if (trackedShadeDevices.Remove(device))
                {
                    if (device.Passive)
                        device.Passive = false;
                }
            }
            catch
            {
            }
        }

        private static void ReleaseTrackedDevices(InputHandler handler)
        {
            if (trackedShadeDevices.Count == 0)
                return;

            cleanupList.Clear();
            cleanupList.AddRange(trackedShadeDevices);
            foreach (var device in cleanupList)
            {
                UpdateDeviceState(handler, device, false);
            }
            cleanupList.Clear();
        }

        private static void CleanupDetachedDevices(InputHandler handler, IList<InputDevice> devices)
        {
            if (trackedShadeDevices.Count == 0)
                return;

            cleanupList.Clear();
            foreach (var device in trackedShadeDevices)
            {
                if (device == null || device == InputDevice.Null)
                {
                    cleanupList.Add(device);
                    continue;
                }

                if (devices == null || !ContainsDevice(devices, device))
                {
                    cleanupList.Add(device);
                }
            }

            if (cleanupList.Count > 0)
            {
                foreach (var device in cleanupList)
                {
                    UpdateDeviceState(handler, device, false);
                }
                cleanupList.Clear();
            }
        }

        private static bool ContainsDevice(IList<InputDevice> list, InputDevice device)
        {
            if (list == null || device == null || device == InputDevice.Null)
                return false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == device)
                    return true;
            }
            return false;
        }

        private static bool ShadeUsesAllControllers(ShadeInputConfig config, int targetIndex, int deviceCount)
        {
            if (config == null)
                return false;
            for (int i = 0; i < deviceCount; i++)
            {
                if (i == targetIndex)
                    continue;
                if (!config.IsControllerIndexInUse(i))
                    return false;
            }
            return true;
        }

        internal static void RefreshShadeDevices(InputHandler handler)
        {
            if (!InputManager.IsSetup)
            {
                ReleaseTrackedDevices(handler);
                return;
            }

            try
            {
                if (handler == null)
                {
                    ReleaseTrackedDevices(null);
                    return;
                }

                var cfg = ModConfig.Instance;
                if (cfg == null || !cfg.hornetControllerEnabled)
                {
                    ReleaseTrackedDevices(handler);
                    return;
                }

                var shadeConfig = cfg.shadeInput;
                if (shadeConfig == null || !shadeConfig.UsesControllerBindings())
                {
                    ReleaseTrackedDevices(handler);
                    return;
                }

                var devices = InputManager.Devices;
                if (devices == null || devices.Count == 0)
                {
                    ReleaseTrackedDevices(handler);
                    return;
                }

                for (int i = 0; i < devices.Count; i++)
                {
                    ShouldIgnoreDevice(handler, devices[i]);
                }

                CleanupDetachedDevices(handler, devices);
            }
            catch
            {
            }
        }

        internal static bool ShouldIgnoreDevice(InputHandler handler, InputDevice device)
        {
            bool ignore = false;

            try
            {
                if (device != null && device != InputDevice.Null && !device.IsUnknown)
                {
                    var cfg = ModConfig.Instance;
                    if (cfg != null && cfg.hornetControllerEnabled)
                    {
                        var shadeConfig = cfg.shadeInput;
                        if (shadeConfig != null && shadeConfig.UsesControllerBindings())
                        {
                            var devices = InputManager.Devices;
                            if (devices != null && devices.Count > 1)
                            {
                                int index = devices.IndexOf(device);
                                if (index >= 0 && shadeConfig.IsControllerIndexInUse(index))
                                {
                                    ignore = !ShadeUsesAllControllers(shadeConfig, index, devices.Count);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                ignore = false;
            }

            UpdateDeviceState(handler, device, ignore);
            return ignore;
        }

        internal static void ReleaseDevice(InputHandler handler, InputDevice device)
        {
            UpdateDeviceState(handler, device, false);
        }

        internal static void EnsureLastActiveController(InputHandler handler)
        {
            if (handler == null)
                return;
            try
            {
                var actions = handler.inputActions;
                if (actions == null)
                    return;
                if (handler.lastActiveController == BindingSourceType.None)
                {
                    handler.lastActiveController = actions.LastInputType;
                    handler.lastInputDeviceStyle = actions.LastDeviceStyle;
                }
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputManager), "UpdateActiveDevice")]
    private class InputManager_UpdateActiveDevice_BlockShadeDevices
    {
        private static void Prefix()
        {
            try
            {
                InputDeviceBlocker.RefreshShadeDevices(InputHandler.UnsafeInstance);
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.UpdateActiveController))]
    private class InputHandler_UpdateActiveController_BlockShadeDevice
    {
        private static bool Prefix(InputHandler __instance)
        {
            try
            {
                if (!InputDeviceBlocker.ShouldIgnoreDevice(__instance, InputManager.ActiveDevice))
                    return true;
                InputDeviceBlocker.EnsureLastActiveController(__instance);
                return false;
            }
            catch
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), "ControllerActivated")]
    private class InputHandler_ControllerActivated_BlockShadeDevice
    {
        private static bool Prefix(InputHandler __instance, InputDevice inputDevice)
        {
            try
            {
                if (InputDeviceBlocker.ShouldIgnoreDevice(__instance, inputDevice))
                    return false;
            }
            catch
            {
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(InputHandler), "ControllerDetached")]
    private class InputHandler_ControllerDetached_ReleaseShadeDevice
    {
        private static void Postfix(InputHandler __instance, InputDevice inputDevice)
        {
            try
            {
                InputDeviceBlocker.ReleaseDevice(__instance, inputDevice);
            }
            catch
            {
            }
        }
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
                    UnityEngine.Object.Destroy(__instance.gameObject);
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
                if (ModConfig.Instance.logShade)
                    UnityEngine.Debug.Log($"[ShadeDebug] NailSlash spawned: {__instance.name} scale={scale} parent={parent}\n{System.Environment.StackTrace}");
            }
            catch (System.Exception ex)
            {
                if (ModConfig.Instance.logShade)
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
                    var ui = UnityEngine.Object.FindFirstObjectByType<UIManager>();
                    if (ui != null)
                        ui.StartCoroutine(ShadeSettingsMenu.Show(ui));
                }
                catch { }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UIManager), nameof(UIManager.UIGoToPauseMenu))]
    private class UIManager_UIGoToPauseMenu_HideShadeMenu
    {
        private static void Prefix(UIManager __instance)
        {
            if (ShadeSettingsMenu.IsShowing)
                ShadeSettingsMenu.HideImmediate(__instance);
        }
    }

    [HarmonyPatch(typeof(UIManager), nameof(UIManager.UIClosePauseMenu))]
    private class UIManager_UIClosePauseMenu_ClearShadeMenu
    {
        private static void Prefix(UIManager __instance)
        {
            if (ShadeSettingsMenu.IsShowing)
                ShadeSettingsMenu.HideImmediate(__instance);
            ShadeSettingsMenu.Clear();
        }
    }
}
#nullable restore
