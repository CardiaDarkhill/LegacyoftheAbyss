using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public partial class LegacyHelper
{
    internal static class RemotePlayBridge
    {
        private const int MaxEventsPerPoll = 64;

        private static readonly Dictionary<int, KeyCode> ScancodeToKey = new()
        {
            { 4, KeyCode.A },
            { 5, KeyCode.B },
            { 6, KeyCode.C },
            { 7, KeyCode.D },
            { 8, KeyCode.E },
            { 9, KeyCode.F },
            { 10, KeyCode.G },
            { 11, KeyCode.H },
            { 12, KeyCode.I },
            { 13, KeyCode.J },
            { 14, KeyCode.K },
            { 15, KeyCode.L },
            { 16, KeyCode.M },
            { 17, KeyCode.N },
            { 18, KeyCode.O },
            { 19, KeyCode.P },
            { 20, KeyCode.Q },
            { 21, KeyCode.R },
            { 22, KeyCode.S },
            { 23, KeyCode.T },
            { 24, KeyCode.U },
            { 25, KeyCode.V },
            { 26, KeyCode.W },
            { 27, KeyCode.X },
            { 28, KeyCode.Y },
            { 29, KeyCode.Z },
            { 30, KeyCode.Alpha1 },
            { 31, KeyCode.Alpha2 },
            { 32, KeyCode.Alpha3 },
            { 33, KeyCode.Alpha4 },
            { 34, KeyCode.Alpha5 },
            { 35, KeyCode.Alpha6 },
            { 36, KeyCode.Alpha7 },
            { 37, KeyCode.Alpha8 },
            { 38, KeyCode.Alpha9 },
            { 39, KeyCode.Alpha0 },
            { 40, KeyCode.Return },
            { 41, KeyCode.Escape },
            { 42, KeyCode.Backspace },
            { 43, KeyCode.Tab },
            { 44, KeyCode.Space },
            { 45, KeyCode.Minus },
            { 46, KeyCode.Equals },
            { 47, KeyCode.LeftBracket },
            { 48, KeyCode.RightBracket },
            { 49, KeyCode.Backslash },
            { 51, KeyCode.Semicolon },
            { 52, KeyCode.Quote },
            { 53, KeyCode.BackQuote },
            { 54, KeyCode.Comma },
            { 55, KeyCode.Period },
            { 56, KeyCode.Slash },
            { 57, KeyCode.CapsLock },
            { 58, KeyCode.F1 },
            { 59, KeyCode.F2 },
            { 60, KeyCode.F3 },
            { 61, KeyCode.F4 },
            { 62, KeyCode.F5 },
            { 63, KeyCode.F6 },
            { 64, KeyCode.F7 },
            { 65, KeyCode.F8 },
            { 66, KeyCode.F9 },
            { 67, KeyCode.F10 },
            { 68, KeyCode.F11 },
            { 69, KeyCode.F12 },
            { 73, KeyCode.Insert },
            { 74, KeyCode.Home },
            { 75, KeyCode.PageUp },
            { 76, KeyCode.Delete },
            { 77, KeyCode.End },
            { 78, KeyCode.PageDown },
            { 79, KeyCode.RightArrow },
            { 80, KeyCode.LeftArrow },
            { 81, KeyCode.DownArrow },
            { 82, KeyCode.UpArrow },
            { 83, KeyCode.Numlock },
            { 84, KeyCode.KeypadDivide },
            { 85, KeyCode.KeypadMultiply },
            { 86, KeyCode.KeypadMinus },
            { 87, KeyCode.KeypadPlus },
            { 88, KeyCode.KeypadEnter },
            { 89, KeyCode.Keypad1 },
            { 90, KeyCode.Keypad2 },
            { 91, KeyCode.Keypad3 },
            { 92, KeyCode.Keypad4 },
            { 93, KeyCode.Keypad5 },
            { 94, KeyCode.Keypad6 },
            { 95, KeyCode.Keypad7 },
            { 96, KeyCode.Keypad8 },
            { 97, KeyCode.Keypad9 },
            { 98, KeyCode.Keypad0 },
            { 99, KeyCode.KeypadPeriod },
            { 224, KeyCode.LeftControl },
            { 225, KeyCode.LeftShift },
            { 226, KeyCode.LeftAlt },
            { 227, KeyCode.LeftWindows },
            { 228, KeyCode.RightControl },
            { 229, KeyCode.RightShift },
            { 230, KeyCode.RightAlt },
            { 231, KeyCode.RightWindows }
        };

        private static readonly RemotePlayInput_t[] EventBuffer = new RemotePlayInput_t[MaxEventsPerPoll];
        private static readonly Dictionary<KeyCode, bool> KeyStates = new();
        private static readonly HashSet<KeyCode> KeysPressed = new();

        private static bool attemptedInitialize;
        private static bool isActive;
        private static IntPtr remotePlayPtr;

        internal static void Initialize()
        {
            if (attemptedInitialize)
            {
                return;
            }

            attemptedInitialize = true;

            try
            {
                remotePlayPtr = SteamAPI_SteamRemotePlay_v003();
                if (remotePlayPtr == IntPtr.Zero)
                {
                    LogInfo("Steam Remote Play interface not available.");
                    return;
                }

                if (!SteamAPI_ISteamRemotePlay_BEnableRemotePlayTogetherDirectInput(remotePlayPtr))
                {
                    LogWarning("Steam Remote Play Together direct input could not be enabled.");
                    remotePlayPtr = IntPtr.Zero;
                    return;
                }

                isActive = true;
                LogInfo("Steam Remote Play Together direct input enabled.");
            }
            catch (DllNotFoundException)
            {
                LogInfo("Steam Remote Play not detected (steam_api64.dll missing).");
            }
            catch (EntryPointNotFoundException ex)
            {
                LogWarning($"Steam Remote Play entry point missing: {ex.Message}");
            }
            catch (Exception ex)
            {
                LogWarning($"Steam Remote Play initialization failed: {ex}");
            }
        }

        internal static void Update()
        {
            if (!isActive || remotePlayPtr == IntPtr.Zero)
            {
                return;
            }

            KeysPressed.Clear();

            uint eventCount;
            try
            {
                eventCount = SteamAPI_ISteamRemotePlay_GetInput(remotePlayPtr, EventBuffer, (uint)EventBuffer.Length);
            }
            catch
            {
                return;
            }

            for (int i = 0; i < eventCount && i < EventBuffer.Length; i++)
            {
                ProcessEvent(EventBuffer[i]);
            }

            if (KeyStates.Count > 0)
            {
                try
                {
                    if (SteamAPI_ISteamRemotePlay_GetSessionCount(remotePlayPtr) == 0)
                    {
                        KeyStates.Clear();
                    }
                }
                catch
                {
                }
            }
        }

        internal static void Shutdown()
        {
            if (!isActive)
            {
                return;
            }

            try
            {
                if (remotePlayPtr != IntPtr.Zero)
                {
                    SteamAPI_ISteamRemotePlay_DisableRemotePlayTogetherDirectInput(remotePlayPtr);
                }
            }
            catch
            {
            }

            isActive = false;
            remotePlayPtr = IntPtr.Zero;
            KeyStates.Clear();
            KeysPressed.Clear();
        }

        internal static bool IsKeyHeld(KeyCode key)
        {
            if (!isActive || key == KeyCode.None)
            {
                return false;
            }

            return KeyStates.TryGetValue(key, out bool held) && held;
        }

        internal static bool WasKeyPressed(KeyCode key)
        {
            if (!isActive || key == KeyCode.None)
            {
                return false;
            }

            return KeysPressed.Contains(key);
        }

        private static void ProcessEvent(in RemotePlayInput_t input)
        {
            switch (input.m_eType)
            {
                case ERemotePlayInputType.KeyDown:
                    if (TryMapScancode(input.m_val.m_Key.m_eScancode, out var keyDown))
                    {
                        SetKeyState(keyDown, true);
                    }
                    break;
                case ERemotePlayInputType.KeyUp:
                    if (TryMapScancode(input.m_val.m_Key.m_eScancode, out var keyUp))
                    {
                        SetKeyState(keyUp, false);
                    }
                    break;
                case ERemotePlayInputType.MouseButtonDown:
                    if (TryMapMouseButton(input.m_val.m_eMouseButton, out var mouseDown))
                    {
                        SetKeyState(mouseDown, true);
                    }
                    break;
                case ERemotePlayInputType.MouseButtonUp:
                    if (TryMapMouseButton(input.m_val.m_eMouseButton, out var mouseUp))
                    {
                        SetKeyState(mouseUp, false);
                    }
                    break;
            }
        }

        private static void SetKeyState(KeyCode key, bool held)
        {
            if (held)
            {
                KeyStates[key] = true;
                KeysPressed.Add(key);
            }
            else
            {
                KeyStates.Remove(key);
            }
        }

        private static bool TryMapScancode(int scancode, out KeyCode key)
        {
            return ScancodeToKey.TryGetValue(scancode, out key);
        }

        private static bool TryMapMouseButton(ERemotePlayMouseButton button, out KeyCode key)
        {
            switch (button)
            {
                case ERemotePlayMouseButton.Left:
                    key = KeyCode.Mouse0;
                    return true;
                case ERemotePlayMouseButton.Right:
                    key = KeyCode.Mouse1;
                    return true;
                case ERemotePlayMouseButton.Middle:
                    key = KeyCode.Mouse2;
                    return true;
                case ERemotePlayMouseButton.X1:
                    key = KeyCode.Mouse3;
                    return true;
                case ERemotePlayMouseButton.X2:
                    key = KeyCode.Mouse4;
                    return true;
                default:
                    key = KeyCode.None;
                    return false;
            }
        }

        [DllImport("steam_api64", EntryPoint = "SteamAPI_SteamRemotePlay_v003")]
        private static extern IntPtr SteamAPI_SteamRemotePlay_v003();

        [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamRemotePlay_BEnableRemotePlayTogetherDirectInput")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamAPI_ISteamRemotePlay_BEnableRemotePlayTogetherDirectInput(IntPtr self);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamRemotePlay_DisableRemotePlayTogetherDirectInput")]
        private static extern void SteamAPI_ISteamRemotePlay_DisableRemotePlayTogetherDirectInput(IntPtr self);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamRemotePlay_GetInput")]
        private static extern uint SteamAPI_ISteamRemotePlay_GetInput(IntPtr self, [Out] RemotePlayInput_t[] input, uint maxEvents);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamRemotePlay_GetSessionCount")]
        private static extern uint SteamAPI_ISteamRemotePlay_GetSessionCount(IntPtr self);

        private enum ERemotePlayInputType
        {
            Unknown,
            MouseMotion,
            MouseButtonDown,
            MouseButtonUp,
            MouseWheel,
            KeyDown,
            KeyUp
        }

        private enum ERemotePlayMouseButton
        {
            Left = 0x0001,
            Right = 0x0002,
            Middle = 0x0010,
            X1 = 0x0020,
            X2 = 0x0040
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct RemotePlayInput_t
        {
            public uint m_unSessionID;
            public ERemotePlayInputType m_eType;
            public RemotePlayInputValue m_val;
        }

        [StructLayout(LayoutKind.Explicit, Size = 56, Pack = 8)]
        private struct RemotePlayInputValue
        {
            [FieldOffset(0)]
            public RemotePlayInputMouseMotion_t m_MouseMotion;

            [FieldOffset(0)]
            public ERemotePlayMouseButton m_eMouseButton;

            [FieldOffset(0)]
            public RemotePlayInputMouseWheel_t m_MouseWheel;

            [FieldOffset(0)]
            public RemotePlayInputKey_t m_Key;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct RemotePlayInputMouseMotion_t
        {
            public int m_nDeltaX;
            public int m_nDeltaY;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct RemotePlayInputMouseWheel_t
        {
            public int m_eDirection;
            public float m_flAmount;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct RemotePlayInputKey_t
        {
            public int m_eScancode;
            public uint m_unModifiers;
            public uint m_unKeycode;
        }
    }
}
