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
using GlobalEnums;

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
                    ShadeInventoryPaneIntegration.BindInput(shadePane, __instance);
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

        private static bool menuTransferActive;
        private static bool menuTransferShadeUsesController;
        private static bool menuTransferHornetControllerEnabled = true;
        private static bool menuTransferHornetKeyboardEnabled;
        private static ShadeInputConfig menuTransferSavedBindings;
        private static byte[] menuTransferSavedHeroBindings;
        private static GameSettingsSnapshot menuTransferSavedGameSettings;
        private static GamepadType menuTransferSavedActiveGamepadType = GamepadType.NONE;
        private static bool menuTransferHasActiveGamepadType;

        private struct GameSettingsSnapshot
        {
            internal bool hasData;
            internal string jumpKey;
            internal string attackKey;
            internal string dashKey;
            internal string castKey;
            internal string superDashKey;
            internal string dreamNailKey;
            internal string quickMapKey;
            internal string quickCastKey;
            internal string tauntKey;
            internal string inventoryKey;
            internal string inventoryMapKey;
            internal string inventoryJournalKey;
            internal string inventoryToolsKey;
            internal string inventoryQuestsKey;
            internal string upKey;
            internal string downKey;
            internal string leftKey;
            internal string rightKey;
            internal ControllerMapping controllerMapping;

            internal static GameSettingsSnapshot Capture(GameSettings settings)
            {
                var snapshot = new GameSettingsSnapshot();
                if (settings == null)
                    return snapshot;

                snapshot.hasData = true;
                snapshot.jumpKey = settings.jumpKey;
                snapshot.attackKey = settings.attackKey;
                snapshot.dashKey = settings.dashKey;
                snapshot.castKey = settings.castKey;
                snapshot.superDashKey = settings.superDashKey;
                snapshot.dreamNailKey = settings.dreamNailKey;
                snapshot.quickMapKey = settings.quickMapKey;
                snapshot.quickCastKey = settings.quickCastKey;
                snapshot.tauntKey = settings.tauntKey;
                snapshot.inventoryKey = settings.inventoryKey;
                snapshot.inventoryMapKey = settings.inventoryMapKey;
                snapshot.inventoryJournalKey = settings.inventoryJournalKey;
                snapshot.inventoryToolsKey = settings.inventoryToolsKey;
                snapshot.inventoryQuestsKey = settings.inventoryQuestsKey;
                snapshot.upKey = settings.upKey;
                snapshot.downKey = settings.downKey;
                snapshot.leftKey = settings.leftKey;
                snapshot.rightKey = settings.rightKey;
                snapshot.controllerMapping = CloneControllerMapping(settings.controllerMapping);
                return snapshot;
            }

            internal void Restore(GameSettings settings)
            {
                if (!hasData || settings == null)
                    return;

                settings.jumpKey = jumpKey;
                settings.attackKey = attackKey;
                settings.dashKey = dashKey;
                settings.castKey = castKey;
                settings.superDashKey = superDashKey;
                settings.dreamNailKey = dreamNailKey;
                settings.quickMapKey = quickMapKey;
                settings.quickCastKey = quickCastKey;
                settings.tauntKey = tauntKey;
                settings.inventoryKey = inventoryKey;
                settings.inventoryMapKey = inventoryMapKey;
                settings.inventoryJournalKey = inventoryJournalKey;
                settings.inventoryToolsKey = inventoryToolsKey;
                settings.inventoryQuestsKey = inventoryQuestsKey;
                settings.upKey = upKey;
                settings.downKey = downKey;
                settings.leftKey = leftKey;
                settings.rightKey = rightKey;
                settings.controllerMapping = CloneControllerMapping(controllerMapping);
            }
        }

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

        private static void UpdateMenuTransfer()
        {
            bool menuActive = false;
            try
            {
                menuActive = MenuStateUtility.IsMenuActive();
            }
            catch
            {
                menuActive = false;
            }

            if (menuActive)
            {
                if (!menuTransferActive)
                {
                    ActivateMenuTransfer();
                }
            }
            else if (menuTransferActive)
            {
                DeactivateMenuTransfer();
            }
        }

        private static void ActivateMenuTransfer()
        {
            menuTransferActive = true;
            menuTransferShadeUsesController = false;
            menuTransferSavedBindings = null;
            menuTransferSavedHeroBindings = null;
            menuTransferSavedGameSettings = default;
            menuTransferHasActiveGamepadType = false;

            ReleaseTrackedDevices(null);

            try
            {
                var cfg = ModConfig.Instance;
                if (cfg != null)
                {
                    menuTransferHornetControllerEnabled = cfg.hornetControllerEnabled;
                    menuTransferHornetKeyboardEnabled = cfg.hornetKeyboardEnabled;

                    if (!menuTransferHornetControllerEnabled)
                    {
                        cfg.hornetControllerEnabled = true;
                    }

                    if (!cfg.hornetKeyboardEnabled)
                    {
                        cfg.hornetKeyboardEnabled = true;
                    }

                    var shadeConfig = cfg.shadeInput;
                    if (shadeConfig != null && shadeConfig.UsesControllerBindings())
                    {
                        ShadeInputConfig savedBindings = null;
                        try
                        {
                            savedBindings = shadeConfig.Clone();
                        }
                        catch
                        {
                            savedBindings = null;
                        }

                        if (savedBindings != null)
                        {
                            menuTransferShadeUsesController = true;
                            menuTransferSavedBindings = savedBindings;

                            try
                            {
                                shadeConfig.ResetToDefaults();
                            }
                            catch
                            {
                                try
                                {
                                    shadeConfig.CopyBindingsFrom(ShadeInputConfig.CreateDefault());
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                menuTransferShadeUsesController = false;
                menuTransferSavedBindings = null;
            }

            try
            {
                var handler = TryGetInputHandler();
                var settings = TryGetGameSettings();
                menuTransferSavedGameSettings = GameSettingsSnapshot.Capture(settings);

                if (handler != null)
                {
                    var actions = handler.inputActions;
                    if (actions != null)
                    {
                        try
                        {
                            menuTransferSavedHeroBindings = actions.SaveData();
                        }
                        catch
                        {
                            menuTransferSavedHeroBindings = null;
                        }
                    }

                    menuTransferSavedActiveGamepadType = handler.activeGamepadType;
                    menuTransferHasActiveGamepadType = true;

                    ApplyVanillaHornetBindings(handler, settings);
                }
            }
            catch
            {
                menuTransferSavedHeroBindings = null;
                menuTransferHasActiveGamepadType = false;
            }
        }

        private static void DeactivateMenuTransfer()
        {
            try
            {
                var cfg = ModConfig.Instance;
                if (cfg != null)
                {
                    cfg.hornetControllerEnabled = menuTransferHornetControllerEnabled;
                    cfg.hornetKeyboardEnabled = menuTransferHornetKeyboardEnabled;
                    if (menuTransferShadeUsesController && menuTransferSavedBindings != null)
                    {
                        var shadeConfig = cfg.shadeInput;
                        if (shadeConfig != null)
                        {
                            shadeConfig.CopyBindingsFrom(menuTransferSavedBindings);
                        }
                    }
                }
            }
            catch
            {
            }

            try
            {
                var handler = TryGetInputHandler();
                var settings = TryGetGameSettings();

                if (settings != null && menuTransferSavedGameSettings.hasData)
                {
                    menuTransferSavedGameSettings.Restore(settings);
                }

                if (handler != null)
                {
                    if (menuTransferHasActiveGamepadType)
                    {
                        handler.activeGamepadType = menuTransferSavedActiveGamepadType;
                    }

                    if (menuTransferSavedHeroBindings != null)
                    {
                        try
                        {
                            handler.inputActions?.LoadData(menuTransferSavedHeroBindings);
                        }
                        catch
                        {
                        }
                    }

                    if (settings != null && menuTransferSavedGameSettings.hasData)
                    {
                        try
                        {
                            settings.SaveKeyboardSettings();
                        }
                        catch
                        {
                        }

                        try
                        {
                            var gamepadType = menuTransferSavedGameSettings.controllerMapping != null
                                ? menuTransferSavedGameSettings.controllerMapping.gamepadType
                                : handler.activeGamepadType;
                            settings.SaveGamepadSettings(gamepadType);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }

            menuTransferActive = false;
            menuTransferShadeUsesController = false;
            menuTransferSavedBindings = null;
            menuTransferSavedHeroBindings = null;
            menuTransferSavedGameSettings = default;
            menuTransferHasActiveGamepadType = false;
        }

        private static InputHandler TryGetInputHandler()
        {
            try
            {
                var handler = InputHandler.UnsafeInstance;
                if (handler != null)
                    return handler;
            }
            catch
            {
            }

            try
            {
                var gm = MenuStateUtility.TryGetGameManager();
                if (gm != null)
                    return gm.inputHandler;
            }
            catch
            {
            }

            try
            {
                return GameManager.instance?.inputHandler;
            }
            catch
            {
                return null;
            }
        }

        private static GameSettings TryGetGameSettings()
        {
            try
            {
                var gm = MenuStateUtility.TryGetGameManager();
                if (gm != null)
                    return gm.gameSettings;
            }
            catch
            {
            }

            try
            {
                return GameManager.instance?.gameSettings;
            }
            catch
            {
                return null;
            }
        }

        private static void ApplyVanillaHornetBindings(InputHandler handler, GameSettings settings)
        {
            if (handler == null)
                return;

            bool keyboardReset = false;
            bool controllerReset = false;

            try
            {
                handler.ResetDefaultKeyBindings();
                keyboardReset = true;
            }
            catch
            {
                keyboardReset = false;
            }

            var actions = handler.inputActions as HeroActions;
            if (!keyboardReset)
            {
                ApplyFallbackHornetKeyboardBindings(actions, settings);
            }

            try
            {
                handler.ResetDefaultControllerButtonBindings();
                controllerReset = true;
            }
            catch
            {
                controllerReset = false;
            }

            if (!controllerReset)
            {
                ApplyFallbackHornetControllerBindings(actions, settings);
            }

            if (settings != null)
            {
                try
                {
                    settings.SaveKeyboardSettings();
                }
                catch
                {
                }

                try
                {
                    if (settings.controllerMapping != null)
                    {
                        settings.controllerMapping.gamepadType = handler.activeGamepadType;
                    }
                    settings.SaveGamepadSettings(handler.activeGamepadType);
                }
                catch
                {
                }
            }
        }

        private static void ApplyFallbackHornetKeyboardBindings(HeroActions actions, GameSettings settings)
        {
            if (actions == null)
                return;

            BindKey(actions.Jump, Key.Z);
            BindKey(actions.Attack, Key.X);
            BindKey(actions.Dash, Key.C);
            BindKey(actions.Cast, Key.A);
            BindKey(actions.SuperDash, Key.S);
            BindKey(actions.DreamNail, Key.D);
            BindKey(actions.QuickMap, Key.Tab);
            BindKey(actions.OpenInventory, Key.I);
            BindKey(actions.OpenInventoryMap, Key.M);
            BindKey(actions.OpenInventoryJournal, Key.J);
            BindKey(actions.OpenInventoryTools, Key.Q);
            BindKey(actions.OpenInventoryQuests, Key.T);
            BindKey(actions.QuickCast, Key.F);
            BindKey(actions.Taunt, Key.V);
            BindKey(actions.Up, Key.UpArrow);
            BindKey(actions.Down, Key.DownArrow);
            BindKey(actions.Left, Key.LeftArrow);
            BindKey(actions.Right, Key.RightArrow);

            if (settings == null)
                return;

            settings.jumpKey = Key.Z.ToString();
            settings.attackKey = Key.X.ToString();
            settings.dashKey = Key.C.ToString();
            settings.castKey = Key.A.ToString();
            settings.superDashKey = Key.S.ToString();
            settings.dreamNailKey = Key.D.ToString();
            settings.quickMapKey = Key.Tab.ToString();
            settings.inventoryKey = Key.I.ToString();
            settings.inventoryMapKey = Key.M.ToString();
            settings.inventoryJournalKey = Key.J.ToString();
            settings.inventoryToolsKey = Key.Q.ToString();
            settings.inventoryQuestsKey = Key.T.ToString();
            settings.quickCastKey = Key.F.ToString();
            settings.tauntKey = Key.V.ToString();
            settings.upKey = Key.UpArrow.ToString();
            settings.downKey = Key.DownArrow.ToString();
            settings.leftKey = Key.LeftArrow.ToString();
            settings.rightKey = Key.RightArrow.ToString();
        }

        private static void ApplyFallbackHornetControllerBindings(HeroActions actions, GameSettings settings)
        {
            if (actions == null)
                return;

            AddControllerBinding(actions.Jump, InputControlType.Action1);
            AddControllerBinding(actions.Attack, InputControlType.Action3);
            AddControllerBinding(actions.Dash, InputControlType.RightTrigger);
            AddControllerBinding(actions.Cast, InputControlType.Action2);
            AddControllerBinding(actions.SuperDash, InputControlType.LeftTrigger);
            AddControllerBinding(actions.DreamNail, InputControlType.Action4);
            AddControllerBinding(actions.QuickMap, InputControlType.LeftBumper);
            AddControllerBinding(actions.QuickCast, InputControlType.RightBumper);
            AddControllerBinding(actions.Taunt, InputControlType.RightStickButton);
            AddControllerBinding(actions.OpenInventory, InputControlType.Back);
            AddControllerBinding(actions.Up, InputControlType.DPadUp);
            AddControllerBinding(actions.Up, InputControlType.LeftStickUp);
            AddControllerBinding(actions.Down, InputControlType.DPadDown);
            AddControllerBinding(actions.Down, InputControlType.LeftStickDown);
            AddControllerBinding(actions.Left, InputControlType.DPadLeft);
            AddControllerBinding(actions.Left, InputControlType.LeftStickLeft);
            AddControllerBinding(actions.Right, InputControlType.DPadRight);
            AddControllerBinding(actions.Right, InputControlType.LeftStickRight);

            if (settings == null)
                return;

            if (settings.controllerMapping == null)
            {
                settings.controllerMapping = new ControllerMapping();
            }

            settings.controllerMapping.jump = InputControlType.Action1;
            settings.controllerMapping.attack = InputControlType.Action3;
            settings.controllerMapping.dash = InputControlType.RightTrigger;
            settings.controllerMapping.cast = InputControlType.Action2;
            settings.controllerMapping.superDash = InputControlType.LeftTrigger;
            settings.controllerMapping.dreamNail = InputControlType.Action4;
            settings.controllerMapping.quickMap = InputControlType.LeftBumper;
            settings.controllerMapping.quickCast = InputControlType.RightBumper;
            settings.controllerMapping.taunt = InputControlType.RightStickButton;
        }

        private static void BindKey(PlayerAction action, Key key)
        {
            if (action == null)
                return;

            action.ClearBindings();
            action.AddBinding(new KeyBindingSource(new[] { key }));
        }

        private static void AddControllerBinding(PlayerAction action, InputControlType control)
        {
            if (action == null)
                return;

            action.AddBinding(new DeviceBindingSource(control));
        }

        private static ControllerMapping CloneControllerMapping(ControllerMapping mapping)
        {
            if (mapping == null)
                return null;

            return new ControllerMapping
            {
                gamepadType = mapping.gamepadType,
                jump = mapping.jump,
                attack = mapping.attack,
                dash = mapping.dash,
                cast = mapping.cast,
                superDash = mapping.superDash,
                dreamNail = mapping.dreamNail,
                quickMap = mapping.quickMap,
                quickCast = mapping.quickCast,
                taunt = mapping.taunt
            };
        }

        internal static bool ShouldBlockShadeDeviceInput()
        {
            UpdateMenuTransfer();

            try
            {
                if (menuTransferActive)
                {
                    return false;
                }

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
            UpdateMenuTransfer();

            if (menuTransferActive)
            {
                ReleaseTrackedDevices(handler);
                return;
            }

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
            UpdateMenuTransfer();

            if (menuTransferActive)
            {
                SetDeviceRestricted(device, false);
                return false;
            }

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

        internal static bool ShouldSuppressShadeOption(ShadeBindingOption option)
        {
            if (!menuTransferActive || !menuTransferShadeUsesController)
                return false;

            return option.type == ShadeBindingOptionType.Controller;
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

        internal readonly struct MenuTransferSaveScope : IDisposable
        {
            private readonly bool restore;

            public MenuTransferSaveScope(bool isActive)
            {
                restore = isActive;
                if (restore)
                {
                    try
                    {
                        var cfg = ModConfig.Instance;
                        if (cfg != null)
                        {
                            cfg.hornetControllerEnabled = menuTransferHornetControllerEnabled;
                            cfg.hornetKeyboardEnabled = menuTransferHornetKeyboardEnabled;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            public void Dispose()
            {
                if (!restore || !menuTransferActive)
                    return;

                try
                {
                    var cfg = ModConfig.Instance;
                    if (cfg != null)
                    {
                        cfg.hornetControllerEnabled = true;
                        cfg.hornetKeyboardEnabled = true;
                    }
                }
                catch
                {
                }
            }
        }

        internal static MenuTransferSaveScope CreateSaveScope()
        {
            return new MenuTransferSaveScope(menuTransferActive);
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
