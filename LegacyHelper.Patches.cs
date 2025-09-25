#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using InControl;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GlobalEnums;

public partial class LegacyHelper
{
    [HarmonyPatch(typeof(GameManager), "BeginScene")]
    private class GameManager_BeginScene_Patch
    {
        private static void Postfix(GameManager __instance)
        {
            if (__instance != null)
            {
                ShadeRuntime.SyncActiveSlot(__instance);
            }

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
                        SaveShadeState(sc.GetCurrentNormalHP(), sc.GetMaxNormalHP(), sc.GetCurrentLifeblood(), sc.GetMaxLifeblood(), sc.GetShadeSoul(), sc.GetCanTakeDamage(), sc.GetBaseMaxHP());
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

    [HarmonyPatch(typeof(HealthManager), "SpawnCurrency")]
    private class HealthManager_SpawnCurrency_Patch
    {
        private static void Prefix(ref int smallGeoCount, ref int mediumGeoCount, ref int largeGeoCount, ref int largeSmoothGeoCount)
        {
            if (!FragileGreedActive)
            {
                return;
            }

            smallGeoCount = ApplyMultiplier(smallGeoCount);
            mediumGeoCount = ApplyMultiplier(mediumGeoCount);
            largeGeoCount = ApplyMultiplier(largeGeoCount);
            largeSmoothGeoCount = ApplyMultiplier(largeSmoothGeoCount);
        }

        private static int ApplyMultiplier(int value)
        {
            if (value <= 0)
            {
                return value;
            }

            int scaled = Mathf.CeilToInt(value * 1.5f);
            return Mathf.Max(1, scaled);
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
                var shadePane = ShadeInventoryPaneIntegration.TryGetShadePane(__instance) ?? ShadeInventoryPane.ActivePane;
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

    [HarmonyPatch(typeof(InventoryPaneInput), "PressDirection")]
    private class InventoryPaneInput_PressDirection_Shade
    {
        private static void Postfix(InventoryPaneInput __instance, InventoryPaneBase.InputEventType direction)
        {
            try
            {
                var shadePane = ShadeInventoryPaneIntegration.TryGetShadePane(__instance) ?? ShadeInventoryPane.ActivePane;
                shadePane?.HandleDirectionalInput(direction);
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InventoryPaneList), nameof(InventoryPaneList.BeginPane))]
    private class InventoryPaneList_BeginPane_BindShadeInput
    {
        private static void Postfix(InventoryPaneList __instance, InventoryPane pane)
        {
            try
            {
                if (__instance == null || pane == null)
                {
                    return;
                }

                var shadePane = pane as ShadeInventoryPane;
                if (shadePane == null)
                {
                    shadePane = pane.RootPane as ShadeInventoryPane;
                }

                if (shadePane != null)
                {
                    ShadeInventoryPaneIntegration.BindInput(shadePane, __instance, captureFocus: true);
                }
            }
            catch
            {
            }
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
                        SaveShadeState(sc.GetCurrentNormalHP(), sc.GetMaxNormalHP(), sc.GetCurrentLifeblood(), sc.GetMaxLifeblood(), sc.GetShadeSoul(), sc.GetCanTakeDamage(), sc.GetBaseMaxHP());
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
                if (cfg == null)
                    return true;

                if (cfg.hornetControllerEnabled)
                    return true;

                var shadeConfig = cfg.shadeInput;
                if (shadeConfig != null && shadeConfig.UsesControllerBindings())
                    return true;

                return false;
            }
            catch
            {
                return true;
            }
        }
    }

    internal static class InputDeviceBlocker
    {
        private static readonly HashSet<InputDevice> restrictedShadeDevices = new();
        private static readonly List<InputDevice> cleanupList = new();
        private static readonly HashSet<string> AllowedHeroActions = new(StringComparer.Ordinal)
        {
            "Pause",
            "openInventory",
            "openInventoryMap",
            "openInventoryJournal",
            "openInventoryTools",
            "openInventoryQuests",
            "QuickMap"
        };

        private static void SetDeviceRestricted(InputDevice device, bool restrict)
        {
            if (device == null || device == InputDevice.Null)
                return;

            if (restrict)
            {
                restrictedShadeDevices.Add(device);
            }
            else
            {
                restrictedShadeDevices.Remove(device);
            }
        }

        private static void ReleaseTrackedDevices(InputHandler handler)
        {
            if (restrictedShadeDevices.Count == 0)
                return;

            cleanupList.Clear();
            cleanupList.AddRange(restrictedShadeDevices);
            foreach (var device in cleanupList)
            {
                SetDeviceRestricted(device, false);
            }
            cleanupList.Clear();
        }

        private static void CleanupDetachedDevices(InputHandler handler, IList<InputDevice> devices)
        {
            if (restrictedShadeDevices.Count == 0)
                return;

            cleanupList.Clear();
            cleanupList.AddRange(restrictedShadeDevices);
            foreach (var device in cleanupList)
            {
                if (device == null || device == InputDevice.Null)
                {
                    SetDeviceRestricted(device, false);
                    continue;
                }

                if (devices == null || !ContainsDevice(devices, device))
                {
                    SetDeviceRestricted(device, false);
                }
            }

            cleanupList.Clear();
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

        internal static bool ShouldBlockShadeDeviceInput()
        {
            try
            {
                var gm = MenuStateUtility.TryGetGameManager();
                if (ReferenceEquals(gm, null))
                {
                    return false;
                }

                if (gm.GameState != GameState.PLAYING)
                {
                    return false;
                }

                var ui = MenuStateUtility.TryGetUiManager(gm);
                if (MenuStateUtility.IsMenuActive(gm, ui))
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
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
                var cfg = ModConfig.Instance;
                if (cfg == null)
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
            bool restrict = false;

            try
            {
                if (device != null && device != InputDevice.Null && !device.IsUnknown)
                {
                    var cfg = ModConfig.Instance;
                    if (cfg != null)
                    {
                        var shadeConfig = cfg.shadeInput;
                        if (shadeConfig != null && shadeConfig.UsesControllerBindings())
                        {
                            var devices = InputManager.Devices;
                            if (devices != null && devices.Count > 0)
                            {
                                int index = devices.IndexOf(device);
                                if (index >= 0 && shadeConfig.IsControllerIndexInUse(index))
                                {
                                    bool hornetWantsController = cfg.hornetControllerEnabled;
                                    bool shadeUsesAll = ShadeUsesAllControllers(shadeConfig, index, devices.Count);
                                    if (!shadeUsesAll || !hornetWantsController)
                                    {
                                        restrict = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                restrict = false;
            }

            bool block = false;
            if (restrict)
            {
                block = ShouldBlockShadeDeviceInput();
            }

            SetDeviceRestricted(device, block);
            return block;
        }

        internal static void ReleaseDevice(InputHandler handler, InputDevice device)
        {
            SetDeviceRestricted(device, false);
        }

        internal static bool IsRestrictedDevice(InputDevice device)
        {
            return device != null && device != InputDevice.Null && restrictedShadeDevices.Contains(device);
        }

        internal static bool AllowsHeroAction(PlayerAction action)
        {
            try
            {
                if (action == null)
                    return false;
                string name = action.Name;
                if (string.IsNullOrEmpty(name))
                    return false;
                return AllowedHeroActions.Contains(name);
            }
            catch
            {
                return false;
            }
        }

        internal static bool ShouldSuppressShadeOption(ShadeBindingOption option) => false;

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

        internal readonly struct MenuTransferSaveScope : IDisposable
        {
            public void Dispose()
            {
            }
        }

        internal static MenuTransferSaveScope CreateSaveScope() => default;
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.OnAwake))]
    private class InputHandler_OnAwake_MenuInputBridge
    {
        private static void Postfix(InputHandler __instance)
        {
            try
            {
                MenuInputBridge.Initialize(__instance);
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.OnDestroy))]
    private class InputHandler_OnDestroy_MenuInputBridge
    {
        private static void Prefix(InputHandler __instance)
        {
            try
            {
                MenuInputBridge.OnDestroyed(__instance);
            }
            catch
            {
            }
        }
    }

    private static class MenuInputBridge
    {
        private const int KeyboardUpBindingId = 1;
        private const int KeyboardDownBindingId = 2;
        private const int KeyboardLeftBindingId = 3;
        private const int KeyboardRightBindingId = 4;
        private const int ControllerUpBindingId = 5;
        private const int ControllerDownBindingId = 6;
        private const int ControllerLeftBindingId = 7;
        private const int ControllerRightBindingId = 8;
        private const int KeyboardCancelBindingId = 9;
        private const int ControllerInventoryBindingId = 10;
        private const int KeyboardConfirmBindingId = 11;

        private static InputHandler handler;
        private static bool subscribed;
        private static BindingSourceType? pendingSimulatedBindingSource;
        private static int pendingSimulatedBindingFrame = -1;

        internal static void Initialize(InputHandler instance)
        {
            handler = instance;
            EnsureBindings(instance?.inputActions);
            if (!subscribed)
            {
                InputHandler.OnUpdateHeroActions += HandleActionsUpdated;
                subscribed = true;
            }
        }

        internal static void OnDestroyed(InputHandler instance)
        {
            if (ReferenceEquals(handler, instance))
            {
                handler = null;
            }

            if (subscribed)
            {
                InputHandler.OnUpdateHeroActions -= HandleActionsUpdated;
                subscribed = false;
            }
        }

        private static void HandleActionsUpdated(HeroActions actions)
        {
            try
            {
                EnsureBindings(actions);
            }
            catch
            {
            }
        }

        private static void EnsureBindings(HeroActions actions)
        {
            if (actions == null)
            {
                return;
            }

            RemoveSavedPlaceholders(actions);

            AddBinding(actions.Up, new ShadeKeyboardMovementBinding(KeyboardUpBindingId, ShadeAction.MoveUp));
            AddBinding(actions.Down, new ShadeKeyboardMovementBinding(KeyboardDownBindingId, ShadeAction.MoveDown));
            AddBinding(actions.Left, new ShadeKeyboardMovementBinding(KeyboardLeftBindingId, ShadeAction.MoveLeft));
            AddBinding(actions.Right, new ShadeKeyboardMovementBinding(KeyboardRightBindingId, ShadeAction.MoveRight));

            AddBinding(actions.Up, new ShadeControllerMovementBinding(ControllerUpBindingId, ShadeAction.MoveUp));
            AddBinding(actions.Down, new ShadeControllerMovementBinding(ControllerDownBindingId, ShadeAction.MoveDown));
            AddBinding(actions.Left, new ShadeControllerMovementBinding(ControllerLeftBindingId, ShadeAction.MoveLeft));
            AddBinding(actions.Right, new ShadeControllerMovementBinding(ControllerRightBindingId, ShadeAction.MoveRight));

            AddBinding(actions.MenuCancel, new ShadeKeyboardBackBinding(KeyboardCancelBindingId));
            AddBinding(actions.OpenInventory, new ShadeControllerInventoryBinding(ControllerInventoryBindingId));
            AddBinding(actions.MenuSubmit, new ShadeKeyboardConfirmBinding(KeyboardConfirmBindingId));
        }

        private static void AddBinding(PlayerAction action, ShadeMenuBindingSourceBase binding)
        {
            if (action == null)
            {
                return;
            }

            foreach (var existing in action.Bindings)
            {
                if (existing is ShadeMenuBindingSourceBase other && other.Equals(binding))
                {
                    return;
                }
            }

            action.AddBinding(binding);
        }

        private static void PrunePlaceholderBindings(PlayerAction action)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                var unfiltered = action.UnfilteredBindings;
                if (unfiltered == null || unfiltered.Count == 0)
                    return;

                for (int i = unfiltered.Count - 1; i >= 0; i--)
                {
                    var binding = unfiltered[i];
                    if (binding == null)
                        continue;

                    if (IsPlaceholderBinding(binding))
                    {
                        action.RemoveBinding(binding);
                    }
                }
            }
            catch
            {
            }
        }

        internal static void RemoveSavedPlaceholders(HeroActions actions)
        {
            if (actions == null)
            {
                return;
            }

            PrunePlaceholderBindings(actions.Up);
            PrunePlaceholderBindings(actions.Down);
            PrunePlaceholderBindings(actions.Left);
            PrunePlaceholderBindings(actions.Right);
            PrunePlaceholderBindings(actions.MenuCancel);
            PrunePlaceholderBindings(actions.OpenInventory);
            PrunePlaceholderBindings(actions.MenuSubmit);
        }

        internal static bool IsShadePlaceholderBinding(BindingSource binding)
        {
            if (binding is ShadeMenuBindingSourceBase)
            {
                return true;
            }

            return IsPlaceholderBinding(binding);
        }

        private static bool IsPlaceholderBinding(BindingSource binding)
        {
            if (binding == null)
                return false;

            try
            {
                if (binding is DeviceBindingSource deviceBinding)
                {
                    return deviceBinding.Control == InputControlType.None;
                }

                if (binding is KeyBindingSource keyBinding)
                {
                    var combo = keyBinding.Control;
                    return combo.IncludeCount == 0 && combo.ExcludeCount == 0;
                }
            }
            catch
            {
            }

            return false;
        }

        private static bool IsMenuActive()
        {
            try
            {
                return MenuStateUtility.IsMenuActive();
            }
            catch
            {
                return false;
            }
        }

        private static InputHandler TryGetHandler()
        {
            if (handler != null)
                return handler;
            try
            {
                return InputHandler.UnsafeInstance;
            }
            catch
            {
                return null;
            }
        }

        private static bool HornetControllerBindingsEnabled()
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

        private static bool HornetKeyboardBindingsEnabled()
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

        private static void RegisterSimulatedBinding(BindingSourceType sourceType)
        {
            pendingSimulatedBindingSource = sourceType;
            pendingSimulatedBindingFrame = Time.frameCount;
        }

        private static bool ShadeInventoryKeyPressed()
        {
            var current = TryGetHandler();
            if (current == null)
                return false;

            var actions = current.inputActions;
            if (actions == null)
                return false;

            return IsActionKeyboardBindingPressed(actions.OpenInventory)
                || IsActionKeyboardBindingPressed(actions.OpenInventoryMap)
                || IsActionKeyboardBindingPressed(actions.OpenInventoryJournal)
                || IsActionKeyboardBindingPressed(actions.OpenInventoryTools)
                || IsActionKeyboardBindingPressed(actions.OpenInventoryQuests);
        }

        private static bool IsActionKeyboardBindingPressed(PlayerAction action)
        {
            if (action == null)
                return false;

            foreach (var binding in action.Bindings)
            {
                if (binding == null || binding is ShadeMenuBindingSourceBase)
                    continue;

                if (binding is KeyBindingSource keyBinding)
                {
                    if (IsKeyComboPressed(keyBinding.Control))
                        return true;
                    continue;
                }

                if (binding is MouseBindingSource mouseBinding)
                {
                    if (IsMousePressed(mouseBinding.Control))
                        return true;
                }
            }

            return false;
        }

        private static bool IsKeyComboPressed(KeyCombo combo)
        {
            if (combo.IncludeCount == 0)
                return false;

            for (int i = 0; i < combo.IncludeCount; i++)
            {
                if (!IsKeyPressed(combo.GetInclude(i)))
                    return false;
            }

            return true;
        }

        private static bool IsKeyPressed(Key key)
        {
            if (key == Key.None)
                return false;

            try
            {
                var mappings = UnityKeyboardProvider.KeyMappings;
                int index = (int)key;
                if (index >= 0 && index < mappings.Length)
                {
                    return mappings[index].IsPressed;
                }
            }
            catch
            {
            }

            return false;
        }

        private static bool IsMousePressed(Mouse mouse)
        {
            if (mouse == Mouse.None)
                return false;

            try
            {
                return mouse switch
                {
                    Mouse.LeftButton => Input.GetMouseButton(0),
                    Mouse.RightButton => Input.GetMouseButton(1),
                    Mouse.MiddleButton => Input.GetMouseButton(2),
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        private static float GetShadeControllerDirectionalValue(ShadeAction action)
        {
            float value = ShadeInput.GetActionValue(action, ShadeBindingOptionType.Controller);
            float dpad = GetShadeControllerDPadValue(action);
            return Mathf.Max(value, dpad);
        }

        private static float GetShadeControllerDPadValue(ShadeAction action)
        {
            var device = TryGetShadeController();
            if (device == null)
                return 0f;

            try
            {
                return action switch
                {
                    ShadeAction.MoveUp => GetDpadDirectionValue(device, InputControlType.DPadUp, InputControlType.DPadY, true),
                    ShadeAction.MoveDown => GetDpadDirectionValue(device, InputControlType.DPadDown, InputControlType.DPadY, false),
                    ShadeAction.MoveLeft => GetDpadDirectionValue(device, InputControlType.DPadLeft, InputControlType.DPadX, false),
                    ShadeAction.MoveRight => GetDpadDirectionValue(device, InputControlType.DPadRight, InputControlType.DPadX, true),
                    _ => 0f
                };
            }
            catch
            {
                return 0f;
            }
        }

        private static float GetDpadDirectionValue(InputDevice device, InputControlType buttonControl, InputControlType axisControl, bool positiveAxis)
        {
            if (device == null || device == InputDevice.Null)
                return 0f;

            if (IsControlPressed(device, buttonControl))
                return 1f;

            float axisContribution = 0f;
            try
            {
                var axis = device.GetControl(axisControl);
                if (axis != null && axis != InputControl.Null)
                {
                    float value = Mathf.Clamp(axis.Value, -1f, 1f);
                    axisContribution = positiveAxis ? Mathf.Max(0f, value) : Mathf.Max(0f, -value);
                }
            }
            catch
            {
            }

            if (axisContribution > 0f)
                return Mathf.Clamp01(axisContribution);

            try
            {
                var dpad = device.DPad;
                if (dpad != null && dpad != TwoAxisInputControl.Null)
                {
                    float composite = axisControl switch
                    {
                        InputControlType.DPadX => positiveAxis ? Mathf.Max(0f, dpad.X) : Mathf.Max(0f, -dpad.X),
                        InputControlType.DPadY => positiveAxis ? Mathf.Max(0f, dpad.Y) : Mathf.Max(0f, -dpad.Y),
                        _ => 0f
                    };
                    if (composite > 0f)
                        return Mathf.Clamp01(composite);
                }
            }
            catch
            {
            }

            return 0f;
        }

        private static float GetShadeControllerBackValue()
        {
            var device = TryGetShadeController();
            if (device == null)
                return 0f;

            try
            {
                if (IsControlPressed(device, InputControlType.Action6))
                    return 1f;
                if (IsControlPressed(device, InputControlType.Back))
                    return 1f;
                if (IsControlPressed(device, InputControlType.Select))
                    return 1f;
            }
            catch
            {
            }

            return 0f;
        }

        private static bool IsControlPressed(InputDevice device, InputControlType controlType)
        {
            try
            {
                var control = device.GetControl(controlType);
                return control != null && control != InputControl.Null && control.IsPressed;
            }
            catch
            {
                return false;
            }
        }

        private static InputDevice TryGetShadeController()
        {
            try
            {
                var cfg = ShadeInput.Config;
                if (cfg == null)
                    return null;
                int index = Mathf.Max(-1, cfg.controllerDeviceIndex);
                if (index < 0)
                    return null;
                var devices = InputManager.Devices;
                if (devices == null || devices.Count == 0)
                    return null;
                if (index >= devices.Count)
                    index = devices.Count - 1;
                return devices[index];
            }
            catch
            {
                return null;
            }
        }

        internal static bool ShouldBypassActiveControllerUpdate(InputHandler instance)
        {
            if (!pendingSimulatedBindingSource.HasValue)
                return false;

            if (Time.frameCount != pendingSimulatedBindingFrame)
            {
                pendingSimulatedBindingSource = null;
                return false;
            }

            try
            {
                var actions = instance?.inputActions;
                if (actions == null)
                {
                    pendingSimulatedBindingSource = null;
                    return false;
                }

                if (actions.LastInputType == pendingSimulatedBindingSource.Value)
                {
                    pendingSimulatedBindingSource = null;
                    return true;
                }
            }
            catch
            {
                pendingSimulatedBindingSource = null;
            }

            return false;
        }

        private abstract class ShadeMenuBindingSourceBase : BindingSource
        {
            private readonly int id;

            protected ShadeMenuBindingSourceBase(int id)
            {
                this.id = id;
            }

            protected abstract float ComputeValue();
            protected virtual bool ShouldActivate() => true;
            protected abstract BindingSourceType SourceType { get; }
            protected abstract InputDeviceClass SourceClass { get; }
            protected abstract string SourceName { get; }
            protected abstract string SourceDeviceName { get; }

            public override float GetValue(InputDevice inputDevice)
            {
                if (!ShouldActivate())
                    return 0f;
                float value = Mathf.Clamp01(ComputeValue());
                if (value > 0f)
                    RegisterSimulatedBinding(SourceType);
                return value;
            }

            public override bool GetState(InputDevice inputDevice) => GetValue(inputDevice) >= 0.5f;

            public override BindingSourceType BindingSourceType => SourceType;

            public override string Name => SourceName;

            public override string DeviceName => SourceDeviceName;

            public override InputDeviceClass DeviceClass => SourceClass;

            public override InputDeviceStyle DeviceStyle => InputDeviceStyle.Unknown;

            public override bool IsValid => true;

            public override bool Equals(BindingSource other)
            {
                return other is ShadeMenuBindingSourceBase binding && binding.id == id && binding.GetType() == GetType();
            }

            public override bool Equals(object obj)
            {
                return obj is ShadeMenuBindingSourceBase binding && binding.id == id && binding.GetType() == GetType();
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (GetType().GetHashCode() * 397) ^ id;
                }
            }

            public override void Save(BinaryWriter writer)
            {
                try
                {
                    WritePlaceholderData(writer);
                }
                catch
                {
                }
            }

            public override void Load(BinaryReader reader, ushort dataFormatVersion)
            {
                try
                {
                    ReadPlaceholderData(reader);
                }
                catch
                {
                }
            }

            private void WritePlaceholderData(BinaryWriter writer)
            {
                if (writer == null)
                    return;

                switch (SourceType)
                {
                    case BindingSourceType.DeviceBindingSource:
                    case BindingSourceType.UnknownDeviceBindingSource:
                        writer.Write((int)InputControlType.None);
                        break;
                    case BindingSourceType.KeyBindingSource:
                        writer.Write(0);
                        writer.Write(0UL);
                        writer.Write(0);
                        writer.Write(0UL);
                        break;
                    case BindingSourceType.MouseBindingSource:
                        writer.Write((int)Mouse.None);
                        break;
                    default:
                        writer.Write(0);
                        break;
                }
            }

            private void ReadPlaceholderData(BinaryReader reader)
            {
                if (reader == null)
                    return;

                switch (SourceType)
                {
                    case BindingSourceType.DeviceBindingSource:
                    case BindingSourceType.UnknownDeviceBindingSource:
                        _ = reader.ReadInt32();
                        break;
                    case BindingSourceType.KeyBindingSource:
                        _ = reader.ReadInt32();
                        _ = reader.ReadUInt64();
                        _ = reader.ReadInt32();
                        _ = reader.ReadUInt64();
                        break;
                    case BindingSourceType.MouseBindingSource:
                        _ = reader.ReadInt32();
                        break;
                    default:
                        break;
                }
            }
        }

        private sealed class ShadeKeyboardMovementBinding : ShadeMenuBindingSourceBase
        {
            private readonly ShadeAction action;

            public ShadeKeyboardMovementBinding(int id, ShadeAction action) : base(id)
            {
                this.action = action;
            }

            protected override float ComputeValue()
            {
                return ShadeInput.GetActionValue(action, ShadeBindingOptionType.Key);
            }

            protected override bool ShouldActivate()
            {
                return IsMenuActive() && HornetControllerBindingsEnabled();
            }

            protected override BindingSourceType SourceType => BindingSourceType.DeviceBindingSource;
            protected override InputDeviceClass SourceClass => InputDeviceClass.Controller;
            protected override string SourceName => $"Shade {action} Keyboard";
            protected override string SourceDeviceName => "Shade Keyboard";
        }

        private sealed class ShadeControllerMovementBinding : ShadeMenuBindingSourceBase
        {
            private readonly ShadeAction action;

            public ShadeControllerMovementBinding(int id, ShadeAction action) : base(id)
            {
                this.action = action;
            }

            protected override float ComputeValue()
            {
                return GetShadeControllerDirectionalValue(action);
            }

            protected override bool ShouldActivate()
            {
                return IsMenuActive() && HornetKeyboardBindingsEnabled();
            }

            protected override BindingSourceType SourceType => BindingSourceType.KeyBindingSource;
            protected override InputDeviceClass SourceClass => InputDeviceClass.Keyboard;
            protected override string SourceName => $"Shade {action} Controller";
            protected override string SourceDeviceName => "Shade Controller";
        }

        private sealed class ShadeKeyboardBackBinding : ShadeMenuBindingSourceBase
        {
            public ShadeKeyboardBackBinding(int id) : base(id)
            {
            }

            protected override float ComputeValue()
            {
                return ShadeInventoryKeyPressed() ? 1f : 0f;
            }

            protected override bool ShouldActivate()
            {
                return IsMenuActive() && HornetControllerBindingsEnabled();
            }

            protected override BindingSourceType SourceType => BindingSourceType.DeviceBindingSource;
            protected override InputDeviceClass SourceClass => InputDeviceClass.Controller;
            protected override string SourceName => "Shade Inventory Shortcut";
            protected override string SourceDeviceName => "Shade Keyboard";
        }

        private sealed class ShadeControllerInventoryBinding : ShadeMenuBindingSourceBase
        {
            public ShadeControllerInventoryBinding(int id) : base(id)
            {
            }

            protected override float ComputeValue()
            {
                return GetShadeControllerBackValue();
            }

            protected override bool ShouldActivate()
            {
                return IsMenuActive() && HornetKeyboardBindingsEnabled();
            }

            protected override BindingSourceType SourceType => BindingSourceType.DeviceBindingSource;
            protected override InputDeviceClass SourceClass => InputDeviceClass.Keyboard;
            protected override string SourceName => "Shade Controller Back";
            protected override string SourceDeviceName => "Shade Controller";
        }

        private sealed class ShadeKeyboardConfirmBinding : ShadeMenuBindingSourceBase
        {
            public ShadeKeyboardConfirmBinding(int id) : base(id)
            {
            }

            protected override float ComputeValue()
            {
                return ShadeInput.GetActionValue(ShadeAction.Fire, ShadeBindingOptionType.Key);
            }

            protected override bool ShouldActivate()
            {
                return IsMenuActive() && HornetControllerBindingsEnabled();
            }

            protected override BindingSourceType SourceType => BindingSourceType.DeviceBindingSource;
            protected override InputDeviceClass SourceClass => InputDeviceClass.Controller;
            protected override string SourceName => "Shade Confirm Keyboard";
            protected override string SourceDeviceName => "Shade Keyboard";
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.GetButtonBindingForAction))]
    private class InputHandler_GetButtonBindingForAction_MenuInputBridge
    {
        private static void Postfix(PlayerAction action, ref InputControlType __result)
        {
            if (__result != InputControlType.None)
                return;

            try
            {
                if (action == null)
                    return;

                foreach (var binding in action.Bindings)
                {
                    if (binding is DeviceBindingSource deviceBinding)
                    {
                        if (MenuInputBridge.IsShadePlaceholderBinding(binding))
                            continue;

                        if (deviceBinding.Control != InputControlType.None)
                        {
                            __result = deviceBinding.Control;
                            return;
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), "GetKeyBindingForActionBinding")]
    private class InputHandler_GetKeyBindingForActionBinding_MenuInputBridge
    {
        private static bool Prefix(PlayerAction action, BindingSource bindingSource, ref InputHandler.KeyOrMouseBinding __result)
        {
            try
            {
                if (MenuInputBridge.IsShadePlaceholderBinding(bindingSource))
                {
                    __result = new InputHandler.KeyOrMouseBinding(InControl.Key.None);
                    return false;
                }
            }
            catch
            {
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.GetKeyBindingForAction))]
    private class InputHandler_GetKeyBindingForAction_MenuInputBridge
    {
        private static void Postfix(InputHandler __instance, PlayerAction action, ref InputHandler.KeyOrMouseBinding __result)
        {
            try
            {
                if (!InputHandler.KeyOrMouseBinding.IsNone(__result))
                    return;

                if (__instance == null || action == null)
                    return;

                var actions = __instance.inputActions;
                if (actions == null || !actions.Actions.Contains(action))
                    return;

                foreach (var binding in action.Bindings)
                {
                    if (MenuInputBridge.IsShadePlaceholderBinding(binding))
                        continue;

                    if (binding is KeyBindingSource keyBinding)
                    {
                        var combo = keyBinding.Control;
                        if (combo.IncludeCount == 0)
                            continue;

                        if (combo.IncludeCount == 1)
                        {
                            __result = new InputHandler.KeyOrMouseBinding(combo.GetInclude(0));
                            return;
                        }

                        continue;
                    }

                    if (binding is MouseBindingSource mouseBinding)
                    {
                        __result = new InputHandler.KeyOrMouseBinding(mouseBinding.Control);
                        return;
                    }
                }
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.SendKeyBindingsToGameSettings))]
    private class InputHandler_SendKeyBindingsToGameSettings_MenuInputBridge
    {
        private static void Prefix(InputHandler __instance)
        {
            try
            {
                MenuInputBridge.RemoveSavedPlaceholders(__instance?.inputActions);
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.SendButtonBindingsToGameSettings))]
    private class InputHandler_SendButtonBindingsToGameSettings_MenuInputBridge
    {
        private static void Prefix(InputHandler __instance)
        {
            try
            {
                MenuInputBridge.RemoveSavedPlaceholders(__instance?.inputActions);
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(PlayerAction), nameof(PlayerAction.Update))]
    private class PlayerAction_Update_BlockShadeGameplay
    {
        private static void Prefix(PlayerAction __instance, ref InputDevice device)
        {
            try
            {
                if (device == null || device == InputDevice.Null)
                    return;
                if (__instance == null)
                    return;
                if (__instance.Owner is HeroActions && InputDeviceBlocker.IsRestrictedDevice(device) && InputDeviceBlocker.ShouldBlockShadeDeviceInput())
                {
                    if (!InputDeviceBlocker.AllowsHeroAction(__instance))
                    {
                        device = InputDevice.Null;
                    }
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
                if (MenuInputBridge.ShouldBypassActiveControllerUpdate(__instance))
                    return false;

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
                LegacyHelper.ShadeController.LogSlashState("NailSlash.Awake", __instance != null ? __instance.gameObject : null, includeStackTrace: true);
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
                LegacyHelper.ShadeController.LogSlashState("NailSlash.StartSlash", __instance != null ? __instance.gameObject : null, includeStackTrace: false);
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
