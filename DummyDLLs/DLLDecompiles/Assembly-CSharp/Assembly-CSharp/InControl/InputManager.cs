using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200090E RID: 2318
	public static class InputManager
	{
		// Token: 0x140000FA RID: 250
		// (add) Token: 0x060051DE RID: 20958 RVA: 0x00176DE4 File Offset: 0x00174FE4
		// (remove) Token: 0x060051DF RID: 20959 RVA: 0x00176E18 File Offset: 0x00175018
		public static event Action OnSetup;

		// Token: 0x140000FB RID: 251
		// (add) Token: 0x060051E0 RID: 20960 RVA: 0x00176E4C File Offset: 0x0017504C
		// (remove) Token: 0x060051E1 RID: 20961 RVA: 0x00176E80 File Offset: 0x00175080
		public static event Action OnSetupCompleted;

		// Token: 0x140000FC RID: 252
		// (add) Token: 0x060051E2 RID: 20962 RVA: 0x00176EB4 File Offset: 0x001750B4
		// (remove) Token: 0x060051E3 RID: 20963 RVA: 0x00176EE8 File Offset: 0x001750E8
		public static event Action<ulong, float> OnUpdate;

		// Token: 0x140000FD RID: 253
		// (add) Token: 0x060051E4 RID: 20964 RVA: 0x00176F1C File Offset: 0x0017511C
		// (remove) Token: 0x060051E5 RID: 20965 RVA: 0x00176F50 File Offset: 0x00175150
		public static event Action OnReset;

		// Token: 0x140000FE RID: 254
		// (add) Token: 0x060051E6 RID: 20966 RVA: 0x00176F84 File Offset: 0x00175184
		// (remove) Token: 0x060051E7 RID: 20967 RVA: 0x00176FB8 File Offset: 0x001751B8
		public static event Action<InputDevice> OnDeviceAttached;

		// Token: 0x140000FF RID: 255
		// (add) Token: 0x060051E8 RID: 20968 RVA: 0x00176FEC File Offset: 0x001751EC
		// (remove) Token: 0x060051E9 RID: 20969 RVA: 0x00177020 File Offset: 0x00175220
		public static event Action<InputDevice> OnDeviceDetached;

		// Token: 0x14000100 RID: 256
		// (add) Token: 0x060051EA RID: 20970 RVA: 0x00177054 File Offset: 0x00175254
		// (remove) Token: 0x060051EB RID: 20971 RVA: 0x00177088 File Offset: 0x00175288
		public static event Action<InputDevice> OnActiveDeviceChanged;

		// Token: 0x14000101 RID: 257
		// (add) Token: 0x060051EC RID: 20972 RVA: 0x001770BC File Offset: 0x001752BC
		// (remove) Token: 0x060051ED RID: 20973 RVA: 0x001770F0 File Offset: 0x001752F0
		internal static event Action<ulong, float> OnUpdateDevices;

		// Token: 0x14000102 RID: 258
		// (add) Token: 0x060051EE RID: 20974 RVA: 0x00177124 File Offset: 0x00175324
		// (remove) Token: 0x060051EF RID: 20975 RVA: 0x00177158 File Offset: 0x00175358
		internal static event Action<ulong, float> OnCommitDevices;

		// Token: 0x17000B2A RID: 2858
		// (get) Token: 0x060051F0 RID: 20976 RVA: 0x0017718B File Offset: 0x0017538B
		// (set) Token: 0x060051F1 RID: 20977 RVA: 0x00177192 File Offset: 0x00175392
		public static bool CommandWasPressed { get; private set; }

		// Token: 0x17000B2B RID: 2859
		// (get) Token: 0x060051F2 RID: 20978 RVA: 0x0017719A File Offset: 0x0017539A
		// (set) Token: 0x060051F3 RID: 20979 RVA: 0x001771A1 File Offset: 0x001753A1
		public static bool InvertYAxis { get; set; }

		// Token: 0x17000B2C RID: 2860
		// (get) Token: 0x060051F4 RID: 20980 RVA: 0x001771A9 File Offset: 0x001753A9
		// (set) Token: 0x060051F5 RID: 20981 RVA: 0x001771B0 File Offset: 0x001753B0
		public static bool IsSetup { get; private set; }

		// Token: 0x17000B2D RID: 2861
		// (get) Token: 0x060051F6 RID: 20982 RVA: 0x001771B8 File Offset: 0x001753B8
		// (set) Token: 0x060051F7 RID: 20983 RVA: 0x001771BF File Offset: 0x001753BF
		public static IMouseProvider MouseProvider { get; private set; }

		// Token: 0x17000B2E RID: 2862
		// (get) Token: 0x060051F8 RID: 20984 RVA: 0x001771C7 File Offset: 0x001753C7
		// (set) Token: 0x060051F9 RID: 20985 RVA: 0x001771CE File Offset: 0x001753CE
		public static IKeyboardProvider KeyboardProvider { get; private set; }

		// Token: 0x17000B2F RID: 2863
		// (get) Token: 0x060051FA RID: 20986 RVA: 0x001771D6 File Offset: 0x001753D6
		// (set) Token: 0x060051FB RID: 20987 RVA: 0x001771DD File Offset: 0x001753DD
		internal static string Platform { get; private set; }

		// Token: 0x17000B30 RID: 2864
		// (get) Token: 0x060051FC RID: 20988 RVA: 0x001771E5 File Offset: 0x001753E5
		[Obsolete("Use InputManager.CommandWasPressed instead.")]
		public static bool MenuWasPressed
		{
			get
			{
				return InputManager.CommandWasPressed;
			}
		}

		// Token: 0x060051FD RID: 20989 RVA: 0x001771EC File Offset: 0x001753EC
		internal static bool SetupInternal()
		{
			if (InputManager.IsSetup)
			{
				return false;
			}
			Logger.LogInfo("InControl (version " + InputManager.Version.ToString() + ")");
			InputManager.Platform = Utility.GetPlatformName(true);
			InputManager.enabled = true;
			InputManager.initialTime = 0f;
			InputManager.currentTime = 0f;
			InputManager.lastUpdateTime = 0f;
			InputManager.currentTick = 0UL;
			InputManager.applicationIsFocused = true;
			InputManager.deviceManagers.Clear();
			InputManager.deviceManagerTable.Clear();
			InputManager.devices.Clear();
			InputManager.Devices = InputManager.devices.AsReadOnly();
			InputManager.activeDevice = InputDevice.Null;
			InputManager.activeDevices.Clear();
			InputManager.ActiveDevices = InputManager.activeDevices.AsReadOnly();
			InputManager.playerActionSets.Clear();
			InputManager.MouseProvider = new UnityMouseProvider();
			InputManager.MouseProvider.Setup();
			InputManager.KeyboardProvider = new UnityKeyboardProvider();
			InputManager.KeyboardProvider.Setup();
			InputManager.IsSetup = true;
			bool flag = true;
			if (InputManager.EnableNativeInput && NativeInputDeviceManager.Enable())
			{
				Logger.LogInfo("[InControl] NativeInputDeviceManager enabled.");
				flag = false;
			}
			if (InputManager.EnableXInput && flag)
			{
				XInputDeviceManager.Enable();
				Logger.LogInfo("[InControl] XInputDeviceManager enabled.");
			}
			if (InputManager.OnSetup != null)
			{
				InputManager.OnSetup();
				InputManager.OnSetup = null;
			}
			if (flag)
			{
				InputManager.AddDeviceManager<UnityInputDeviceManager>();
				Logger.LogInfo("UnityInputDeviceManager enabled.");
			}
			Action onSetupCompleted = InputManager.OnSetupCompleted;
			if (onSetupCompleted != null)
			{
				onSetupCompleted();
			}
			return true;
		}

		// Token: 0x060051FE RID: 20990 RVA: 0x0017735C File Offset: 0x0017555C
		internal static void ResetInternal()
		{
			if (InputManager.OnReset != null)
			{
				InputManager.OnReset();
			}
			InputManager.OnSetup = null;
			InputManager.OnUpdate = null;
			InputManager.OnReset = null;
			InputManager.OnActiveDeviceChanged = null;
			InputManager.OnDeviceAttached = null;
			InputManager.OnDeviceDetached = null;
			InputManager.OnUpdateDevices = null;
			InputManager.OnCommitDevices = null;
			InputManager.DestroyDeviceManagers();
			InputManager.DestroyDevices();
			InputManager.playerActionSets.Clear();
			InputManager.MouseProvider.Reset();
			InputManager.KeyboardProvider.Reset();
			InputManager.IsSetup = false;
		}

		// Token: 0x060051FF RID: 20991 RVA: 0x001773D8 File Offset: 0x001755D8
		public static void Update()
		{
			InputManager.UpdateInternal();
		}

		// Token: 0x06005200 RID: 20992 RVA: 0x001773E0 File Offset: 0x001755E0
		internal static void UpdateInternal()
		{
			InputManager.AssertIsSetup();
			if (InputManager.OnSetup != null)
			{
				InputManager.OnSetup();
				InputManager.OnSetup = null;
			}
			if (!InputManager.enabled)
			{
				return;
			}
			if (InputManager.SuspendInBackground && !InputManager.applicationIsFocused)
			{
				return;
			}
			InputManager.currentTick += 1UL;
			InputManager.UpdateCurrentTime();
			float num = InputManager.currentTime - InputManager.lastUpdateTime;
			InputManager.MouseProvider.Update();
			InputManager.KeyboardProvider.Update();
			InputManager.UpdateDeviceManagers(num);
			InputManager.CommandWasPressed = false;
			InputManager.UpdateDevices(num);
			InputManager.CommitDevices(num);
			InputDevice inputDevice = InputManager.ActiveDevice;
			InputManager.UpdateActiveDevice();
			InputManager.UpdatePlayerActionSets(num);
			if (inputDevice != InputManager.ActiveDevice && InputManager.OnActiveDeviceChanged != null)
			{
				InputManager.OnActiveDeviceChanged(InputManager.ActiveDevice);
			}
			if (InputManager.OnUpdate != null)
			{
				InputManager.OnUpdate(InputManager.currentTick, num);
			}
			InputManager.lastUpdateTime = InputManager.currentTime;
		}

		// Token: 0x06005201 RID: 20993 RVA: 0x001774B8 File Offset: 0x001756B8
		public static void Reload()
		{
			InputManager.ResetInternal();
			InputManager.SetupInternal();
		}

		// Token: 0x06005202 RID: 20994 RVA: 0x001774C5 File Offset: 0x001756C5
		private static void AssertIsSetup()
		{
			if (!InputManager.IsSetup)
			{
				throw new Exception("InputManager is not initialized. Call InputManager.Setup() first.");
			}
		}

		// Token: 0x06005203 RID: 20995 RVA: 0x001774DC File Offset: 0x001756DC
		private static void SetZeroTickOnAllControls()
		{
			int count = InputManager.devices.Count;
			for (int i = 0; i < count; i++)
			{
				ReadOnlyCollection<InputControl> controls = InputManager.devices[i].Controls;
				int count2 = controls.Count;
				for (int j = 0; j < count2; j++)
				{
					InputControl inputControl = controls[j];
					if (inputControl != null)
					{
						inputControl.SetZeroTick();
					}
				}
			}
		}

		// Token: 0x06005204 RID: 20996 RVA: 0x00177540 File Offset: 0x00175740
		public static void ClearInputState()
		{
			int count = InputManager.devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputManager.devices[i].ClearInputState();
			}
			int count2 = InputManager.playerActionSets.Count;
			for (int j = 0; j < count2; j++)
			{
				InputManager.playerActionSets[j].ClearInputState();
			}
			InputManager.activeDevice = InputDevice.Null;
		}

		// Token: 0x06005205 RID: 20997 RVA: 0x001775A5 File Offset: 0x001757A5
		internal static void OnApplicationFocus(bool focusState)
		{
			if (!focusState)
			{
				if (InputManager.SuspendInBackground)
				{
					InputManager.ClearInputState();
				}
				InputManager.SetZeroTickOnAllControls();
			}
			InputManager.applicationIsFocused = focusState;
		}

		// Token: 0x06005206 RID: 20998 RVA: 0x001775C1 File Offset: 0x001757C1
		internal static void OnApplicationPause(bool pauseState)
		{
		}

		// Token: 0x06005207 RID: 20999 RVA: 0x001775C3 File Offset: 0x001757C3
		internal static void OnApplicationQuit()
		{
			InputManager.ResetInternal();
		}

		// Token: 0x06005208 RID: 21000 RVA: 0x001775CA File Offset: 0x001757CA
		internal static void OnLevelWasLoaded()
		{
			InputManager.SetZeroTickOnAllControls();
			InputManager.UpdateInternal();
		}

		// Token: 0x06005209 RID: 21001 RVA: 0x001775D8 File Offset: 0x001757D8
		public static void AddDeviceManager(InputDeviceManager deviceManager)
		{
			InputManager.AssertIsSetup();
			Type type = deviceManager.GetType();
			if (InputManager.deviceManagerTable.ContainsKey(type))
			{
				Logger.LogError("A device manager of type '" + type.Name + "' already exists; cannot add another.");
				return;
			}
			InputManager.deviceManagers.Add(deviceManager);
			InputManager.deviceManagerTable.Add(type, deviceManager);
			deviceManager.Update(InputManager.currentTick, InputManager.currentTime - InputManager.lastUpdateTime);
		}

		// Token: 0x0600520A RID: 21002 RVA: 0x00177646 File Offset: 0x00175846
		public static void AddDeviceManager<T>() where T : InputDeviceManager, new()
		{
			InputManager.AddDeviceManager(Activator.CreateInstance<T>());
		}

		// Token: 0x0600520B RID: 21003 RVA: 0x00177658 File Offset: 0x00175858
		public static T GetDeviceManager<T>() where T : InputDeviceManager
		{
			InputDeviceManager inputDeviceManager;
			if (InputManager.deviceManagerTable.TryGetValue(typeof(T), out inputDeviceManager))
			{
				return inputDeviceManager as T;
			}
			return default(T);
		}

		// Token: 0x0600520C RID: 21004 RVA: 0x00177692 File Offset: 0x00175892
		public static bool HasDeviceManager<T>() where T : InputDeviceManager
		{
			return InputManager.deviceManagerTable.ContainsKey(typeof(T));
		}

		// Token: 0x0600520D RID: 21005 RVA: 0x001776A8 File Offset: 0x001758A8
		private static void UpdateCurrentTime()
		{
			if (InputManager.initialTime < 1E-45f)
			{
				InputManager.initialTime = Time.realtimeSinceStartup;
			}
			InputManager.currentTime = Mathf.Max(0f, Time.realtimeSinceStartup - InputManager.initialTime);
		}

		// Token: 0x0600520E RID: 21006 RVA: 0x001776DC File Offset: 0x001758DC
		private static void UpdateDeviceManagers(float deltaTime)
		{
			int count = InputManager.deviceManagers.Count;
			for (int i = 0; i < count; i++)
			{
				InputManager.deviceManagers[i].Update(InputManager.currentTick, deltaTime);
			}
		}

		// Token: 0x0600520F RID: 21007 RVA: 0x00177718 File Offset: 0x00175918
		private static void DestroyDeviceManagers()
		{
			int count = InputManager.deviceManagers.Count;
			for (int i = 0; i < count; i++)
			{
				InputManager.deviceManagers[i].Destroy();
			}
			InputManager.deviceManagers.Clear();
			InputManager.deviceManagerTable.Clear();
		}

		// Token: 0x06005210 RID: 21008 RVA: 0x00177760 File Offset: 0x00175960
		private static void DestroyDevices()
		{
			int count = InputManager.devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputManager.devices[i].OnDetached();
			}
			InputManager.devices.Clear();
			InputManager.activeDevice = InputDevice.Null;
		}

		// Token: 0x06005211 RID: 21009 RVA: 0x001777A8 File Offset: 0x001759A8
		private static void UpdateDevices(float deltaTime)
		{
			int count = InputManager.devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputManager.devices[i].Update(InputManager.currentTick, deltaTime);
			}
			if (InputManager.OnUpdateDevices != null)
			{
				InputManager.OnUpdateDevices(InputManager.currentTick, deltaTime);
			}
		}

		// Token: 0x06005212 RID: 21010 RVA: 0x001777FC File Offset: 0x001759FC
		private static void CommitDevices(float deltaTime)
		{
			int count = InputManager.devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputDevice inputDevice = InputManager.devices[i];
				inputDevice.Commit(InputManager.currentTick, deltaTime);
				if (inputDevice.CommandWasPressed)
				{
					InputManager.CommandWasPressed = true;
				}
			}
			if (InputManager.OnCommitDevices != null)
			{
				InputManager.OnCommitDevices(InputManager.currentTick, deltaTime);
			}
		}

		// Token: 0x06005213 RID: 21011 RVA: 0x0017785C File Offset: 0x00175A5C
		private static void UpdateActiveDevice()
		{
			InputManager.activeDevices.Clear();
			int count = InputManager.devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputDevice inputDevice = InputManager.devices[i];
				if (inputDevice.LastInputAfter(InputManager.ActiveDevice) && !inputDevice.Passive)
				{
					InputManager.ActiveDevice = inputDevice;
				}
				if (inputDevice.IsActive)
				{
					InputManager.activeDevices.Add(inputDevice);
				}
			}
		}

		// Token: 0x06005214 RID: 21012 RVA: 0x001778C4 File Offset: 0x00175AC4
		public static void AttachDevice(InputDevice inputDevice)
		{
			InputManager.AssertIsSetup();
			if (!inputDevice.IsSupportedOnThisPlatform)
			{
				return;
			}
			if (inputDevice.IsAttached)
			{
				return;
			}
			if (!InputManager.devices.Contains(inputDevice))
			{
				InputManager.devices.Add(inputDevice);
				InputManager.devices.Sort((InputDevice d1, InputDevice d2) => d1.SortOrder.CompareTo(d2.SortOrder));
			}
			inputDevice.OnAttached();
			if (InputManager.OnDeviceAttached != null)
			{
				InputManager.OnDeviceAttached(inputDevice);
			}
		}

		// Token: 0x06005215 RID: 21013 RVA: 0x00177944 File Offset: 0x00175B44
		public static void DetachDevice(InputDevice inputDevice)
		{
			if (!InputManager.IsSetup)
			{
				return;
			}
			if (!inputDevice.IsAttached)
			{
				return;
			}
			InputManager.devices.Remove(inputDevice);
			if (InputManager.ActiveDevice == inputDevice)
			{
				InputManager.ActiveDevice = InputDevice.Null;
			}
			inputDevice.OnDetached();
			if (InputManager.OnDeviceDetached != null)
			{
				InputManager.OnDeviceDetached(inputDevice);
			}
		}

		// Token: 0x06005216 RID: 21014 RVA: 0x00177998 File Offset: 0x00175B98
		public static void HideDevicesWithProfile(Type type)
		{
			if (type.IsSubclassOf(typeof(InputDeviceProfile)))
			{
				InputDeviceProfile.Hide(type);
			}
		}

		// Token: 0x06005217 RID: 21015 RVA: 0x001779B2 File Offset: 0x00175BB2
		internal static void AttachPlayerActionSet(PlayerActionSet playerActionSet)
		{
			if (!InputManager.playerActionSets.Contains(playerActionSet))
			{
				InputManager.playerActionSets.Add(playerActionSet);
			}
		}

		// Token: 0x06005218 RID: 21016 RVA: 0x001779CC File Offset: 0x00175BCC
		internal static void DetachPlayerActionSet(PlayerActionSet playerActionSet)
		{
			InputManager.playerActionSets.Remove(playerActionSet);
		}

		// Token: 0x06005219 RID: 21017 RVA: 0x001779DC File Offset: 0x00175BDC
		internal static void UpdatePlayerActionSets(float deltaTime)
		{
			int count = InputManager.playerActionSets.Count;
			for (int i = 0; i < count; i++)
			{
				InputManager.playerActionSets[i].Update(InputManager.currentTick, deltaTime);
			}
		}

		// Token: 0x17000B31 RID: 2865
		// (get) Token: 0x0600521A RID: 21018 RVA: 0x00177A18 File Offset: 0x00175C18
		public static bool AnyKeyIsPressed
		{
			get
			{
				return KeyCombo.Detect(true).IncludeCount > 0;
			}
		}

		// Token: 0x17000B32 RID: 2866
		// (get) Token: 0x0600521B RID: 21019 RVA: 0x00177A36 File Offset: 0x00175C36
		// (set) Token: 0x0600521C RID: 21020 RVA: 0x00177A46 File Offset: 0x00175C46
		public static InputDevice ActiveDevice
		{
			get
			{
				return InputManager.activeDevice ?? InputDevice.Null;
			}
			private set
			{
				InputManager.activeDevice = (value ?? InputDevice.Null);
			}
		}

		// Token: 0x17000B33 RID: 2867
		// (get) Token: 0x0600521D RID: 21021 RVA: 0x00177A57 File Offset: 0x00175C57
		// (set) Token: 0x0600521E RID: 21022 RVA: 0x00177A5E File Offset: 0x00175C5E
		public static bool Enabled
		{
			get
			{
				return InputManager.enabled;
			}
			set
			{
				if (InputManager.enabled != value)
				{
					if (value)
					{
						InputManager.SetZeroTickOnAllControls();
						InputManager.UpdateInternal();
					}
					else
					{
						InputManager.ClearInputState();
						InputManager.SetZeroTickOnAllControls();
					}
					InputManager.enabled = value;
				}
			}
		}

		// Token: 0x17000B34 RID: 2868
		// (get) Token: 0x0600521F RID: 21023 RVA: 0x00177A87 File Offset: 0x00175C87
		// (set) Token: 0x06005220 RID: 21024 RVA: 0x00177A8E File Offset: 0x00175C8E
		public static bool SuspendInBackground { get; set; }

		// Token: 0x17000B35 RID: 2869
		// (get) Token: 0x06005221 RID: 21025 RVA: 0x00177A96 File Offset: 0x00175C96
		// (set) Token: 0x06005222 RID: 21026 RVA: 0x00177A9D File Offset: 0x00175C9D
		public static bool EnableNativeInput { get; internal set; }

		// Token: 0x17000B36 RID: 2870
		// (get) Token: 0x06005223 RID: 21027 RVA: 0x00177AA5 File Offset: 0x00175CA5
		// (set) Token: 0x06005224 RID: 21028 RVA: 0x00177AAC File Offset: 0x00175CAC
		public static bool EnableXInput { get; internal set; }

		// Token: 0x17000B37 RID: 2871
		// (get) Token: 0x06005225 RID: 21029 RVA: 0x00177AB4 File Offset: 0x00175CB4
		// (set) Token: 0x06005226 RID: 21030 RVA: 0x00177ABB File Offset: 0x00175CBB
		public static uint XInputUpdateRate { get; internal set; }

		// Token: 0x17000B38 RID: 2872
		// (get) Token: 0x06005227 RID: 21031 RVA: 0x00177AC3 File Offset: 0x00175CC3
		// (set) Token: 0x06005228 RID: 21032 RVA: 0x00177ACA File Offset: 0x00175CCA
		public static uint XInputBufferSize { get; internal set; }

		// Token: 0x17000B39 RID: 2873
		// (get) Token: 0x06005229 RID: 21033 RVA: 0x00177AD2 File Offset: 0x00175CD2
		// (set) Token: 0x0600522A RID: 21034 RVA: 0x00177AD9 File Offset: 0x00175CD9
		public static bool NativeInputEnableXInput { get; internal set; }

		// Token: 0x17000B3A RID: 2874
		// (get) Token: 0x0600522B RID: 21035 RVA: 0x00177AE1 File Offset: 0x00175CE1
		// (set) Token: 0x0600522C RID: 21036 RVA: 0x00177AE8 File Offset: 0x00175CE8
		public static bool NativeInputEnableMFi { get; internal set; }

		// Token: 0x17000B3B RID: 2875
		// (get) Token: 0x0600522D RID: 21037 RVA: 0x00177AF0 File Offset: 0x00175CF0
		// (set) Token: 0x0600522E RID: 21038 RVA: 0x00177AF7 File Offset: 0x00175CF7
		public static bool NativeInputPreventSleep { get; internal set; }

		// Token: 0x17000B3C RID: 2876
		// (get) Token: 0x0600522F RID: 21039 RVA: 0x00177AFF File Offset: 0x00175CFF
		// (set) Token: 0x06005230 RID: 21040 RVA: 0x00177B06 File Offset: 0x00175D06
		public static uint NativeInputUpdateRate { get; internal set; }

		// Token: 0x17000B3D RID: 2877
		// (get) Token: 0x06005231 RID: 21041 RVA: 0x00177B0E File Offset: 0x00175D0E
		// (set) Token: 0x06005232 RID: 21042 RVA: 0x00177B15 File Offset: 0x00175D15
		public static bool EnableICade { get; internal set; }

		// Token: 0x17000B3E RID: 2878
		// (get) Token: 0x06005233 RID: 21043 RVA: 0x00177B1D File Offset: 0x00175D1D
		internal static VersionInfo UnityVersion
		{
			get
			{
				if (InputManager.unityVersion == null)
				{
					InputManager.unityVersion = new VersionInfo?(VersionInfo.UnityVersion());
				}
				return InputManager.unityVersion.Value;
			}
		}

		// Token: 0x17000B3F RID: 2879
		// (get) Token: 0x06005234 RID: 21044 RVA: 0x00177B44 File Offset: 0x00175D44
		public static ulong CurrentTick
		{
			get
			{
				return InputManager.currentTick;
			}
		}

		// Token: 0x17000B40 RID: 2880
		// (get) Token: 0x06005235 RID: 21045 RVA: 0x00177B4B File Offset: 0x00175D4B
		public static float CurrentTime
		{
			get
			{
				return InputManager.currentTime;
			}
		}

		// Token: 0x04005267 RID: 21095
		public static readonly VersionInfo Version = VersionInfo.InControlVersion();

		// Token: 0x04005271 RID: 21105
		private static readonly List<InputDeviceManager> deviceManagers = new List<InputDeviceManager>();

		// Token: 0x04005272 RID: 21106
		private static readonly Dictionary<Type, InputDeviceManager> deviceManagerTable = new Dictionary<Type, InputDeviceManager>();

		// Token: 0x04005273 RID: 21107
		private static readonly List<InputDevice> devices = new List<InputDevice>();

		// Token: 0x04005274 RID: 21108
		private static InputDevice activeDevice = InputDevice.Null;

		// Token: 0x04005275 RID: 21109
		private static readonly List<InputDevice> activeDevices = new List<InputDevice>();

		// Token: 0x04005276 RID: 21110
		private static readonly List<PlayerActionSet> playerActionSets = new List<PlayerActionSet>();

		// Token: 0x04005277 RID: 21111
		public static ReadOnlyCollection<InputDevice> Devices;

		// Token: 0x04005278 RID: 21112
		public static ReadOnlyCollection<InputDevice> ActiveDevices;

		// Token: 0x0400527F RID: 21119
		private static bool applicationIsFocused;

		// Token: 0x04005280 RID: 21120
		private static float initialTime;

		// Token: 0x04005281 RID: 21121
		private static float currentTime;

		// Token: 0x04005282 RID: 21122
		private static float lastUpdateTime;

		// Token: 0x04005283 RID: 21123
		private static ulong currentTick;

		// Token: 0x04005284 RID: 21124
		private static VersionInfo? unityVersion;

		// Token: 0x04005285 RID: 21125
		private static bool enabled;
	}
}
