using System;
using System.Collections.Generic;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000933 RID: 2355
	public class UnityInputDeviceManager : InputDeviceManager
	{
		// Token: 0x06005396 RID: 21398 RVA: 0x0017E543 File Offset: 0x0017C743
		public UnityInputDeviceManager()
		{
			this.systemDeviceProfiles = new List<InputDeviceProfile>(UnityInputDeviceProfileList.Profiles.Length);
			this.customDeviceProfiles = new List<InputDeviceProfile>();
			this.AddSystemDeviceProfiles();
			this.QueryJoystickInfo();
			this.AttachDevices();
		}

		// Token: 0x06005397 RID: 21399 RVA: 0x0017E57C File Offset: 0x0017C77C
		public override void Update(ulong updateTick, float deltaTime)
		{
			this.deviceRefreshTimer += deltaTime;
			if (this.deviceRefreshTimer >= 1f)
			{
				this.deviceRefreshTimer = 0f;
				this.QueryJoystickInfo();
				if (this.JoystickInfoHasChanged)
				{
					Logger.LogInfo("Change in attached Unity joysticks detected; refreshing device list.");
					this.DetachDevices();
					this.AttachDevices();
				}
			}
		}

		// Token: 0x06005398 RID: 21400 RVA: 0x0017E5D4 File Offset: 0x0017C7D4
		private void QueryJoystickInfo()
		{
			this.joystickNames = Input.GetJoystickNames();
			this.joystickCount = this.joystickNames.Length;
			this.joystickHash = 527 + this.joystickCount;
			for (int i = 0; i < this.joystickCount; i++)
			{
				this.joystickHash = this.joystickHash * 31 + this.joystickNames[i].GetHashCode();
			}
		}

		// Token: 0x17000B83 RID: 2947
		// (get) Token: 0x06005399 RID: 21401 RVA: 0x0017E63A File Offset: 0x0017C83A
		private bool JoystickInfoHasChanged
		{
			get
			{
				return this.joystickHash != this.lastJoystickHash || this.joystickCount != this.lastJoystickCount;
			}
		}

		// Token: 0x0600539A RID: 21402 RVA: 0x0017E660 File Offset: 0x0017C860
		private void AttachDevices()
		{
			try
			{
				for (int i = 0; i < this.joystickCount; i++)
				{
					this.DetectJoystickDevice(i + 1, this.joystickNames[i]);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
				Logger.LogError(ex.StackTrace);
			}
			this.lastJoystickCount = this.joystickCount;
			this.lastJoystickHash = this.joystickHash;
		}

		// Token: 0x0600539B RID: 21403 RVA: 0x0017E6D0 File Offset: 0x0017C8D0
		private void DetachDevices()
		{
			int count = this.devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputManager.DetachDevice(this.devices[i]);
			}
			this.devices.Clear();
		}

		// Token: 0x0600539C RID: 21404 RVA: 0x0017E711 File Offset: 0x0017C911
		public void ReloadDevices()
		{
			this.QueryJoystickInfo();
			this.DetachDevices();
			this.AttachDevices();
		}

		// Token: 0x0600539D RID: 21405 RVA: 0x0017E725 File Offset: 0x0017C925
		private void AttachDevice(UnityInputDevice device)
		{
			this.devices.Add(device);
			InputManager.AttachDevice(device);
		}

		// Token: 0x0600539E RID: 21406 RVA: 0x0017E73C File Offset: 0x0017C93C
		private bool HasAttachedDeviceWithJoystickId(int unityJoystickId)
		{
			int count = this.devices.Count;
			for (int i = 0; i < count; i++)
			{
				UnityInputDevice unityInputDevice = this.devices[i] as UnityInputDevice;
				if (unityInputDevice != null && unityInputDevice.JoystickId == unityJoystickId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600539F RID: 21407 RVA: 0x0017E784 File Offset: 0x0017C984
		private void DetectJoystickDevice(int unityJoystickId, string unityJoystickName)
		{
			if (this.HasAttachedDeviceWithJoystickId(unityJoystickId))
			{
				return;
			}
			if (unityJoystickName.IndexOf("webcam", StringComparison.OrdinalIgnoreCase) != -1)
			{
				return;
			}
			if (InputManager.UnityVersion < new VersionInfo(4, 5, 0, 0) && (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) && unityJoystickName == "Unknown Wireless Controller")
			{
				return;
			}
			if (InputManager.UnityVersion >= new VersionInfo(4, 6, 3, 0) && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) && string.IsNullOrEmpty(unityJoystickName))
			{
				return;
			}
			InputDeviceProfile inputDeviceProfile = this.DetectDevice(unityJoystickName);
			if (inputDeviceProfile == null)
			{
				UnityInputDevice device = new UnityInputDevice(unityJoystickId, unityJoystickName);
				this.AttachDevice(device);
				Logger.LogWarning(string.Concat(new string[]
				{
					"Device ",
					unityJoystickId.ToString(),
					" with name \"",
					unityJoystickName,
					"\" does not match any supported profiles and will be considered an unknown controller."
				}));
				return;
			}
			if (!inputDeviceProfile.IsHidden)
			{
				UnityInputDevice device2 = new UnityInputDevice(inputDeviceProfile, unityJoystickId, unityJoystickName);
				this.AttachDevice(device2);
				Logger.LogInfo(string.Concat(new string[]
				{
					"Device ",
					unityJoystickId.ToString(),
					" matched profile ",
					inputDeviceProfile.GetType().Name,
					" (",
					inputDeviceProfile.DeviceName,
					")"
				}));
				return;
			}
			Logger.LogInfo(string.Concat(new string[]
			{
				"Device ",
				unityJoystickId.ToString(),
				" matching profile ",
				inputDeviceProfile.GetType().Name,
				" (",
				inputDeviceProfile.DeviceName,
				") is hidden and will not be attached."
			}));
		}

		// Token: 0x060053A0 RID: 21408 RVA: 0x0017E918 File Offset: 0x0017CB18
		private InputDeviceProfile DetectDevice(string unityJoystickName)
		{
			InputDeviceProfile inputDeviceProfile = null;
			InputDeviceInfo deviceInfo = new InputDeviceInfo
			{
				name = unityJoystickName
			};
			return (((inputDeviceProfile ?? this.customDeviceProfiles.Find((InputDeviceProfile profile) => profile.Matches(deviceInfo))) ?? this.systemDeviceProfiles.Find((InputDeviceProfile profile) => profile.Matches(deviceInfo))) ?? this.customDeviceProfiles.Find((InputDeviceProfile profile) => profile.LastResortMatches(deviceInfo))) ?? this.systemDeviceProfiles.Find((InputDeviceProfile profile) => profile.LastResortMatches(deviceInfo));
		}

		// Token: 0x060053A1 RID: 21409 RVA: 0x0017E9AF File Offset: 0x0017CBAF
		private void AddSystemDeviceProfile(InputDeviceProfile deviceProfile)
		{
			if (deviceProfile != null && deviceProfile.IsSupportedOnThisPlatform)
			{
				this.systemDeviceProfiles.Add(deviceProfile);
			}
		}

		// Token: 0x060053A2 RID: 21410 RVA: 0x0017E9C8 File Offset: 0x0017CBC8
		private void AddSystemDeviceProfiles()
		{
			for (int i = 0; i < UnityInputDeviceProfileList.Profiles.Length; i++)
			{
				InputDeviceProfile deviceProfile = InputDeviceProfile.CreateInstanceOfType(UnityInputDeviceProfileList.Profiles[i]);
				this.AddSystemDeviceProfile(deviceProfile);
			}
		}

		// Token: 0x04005383 RID: 21379
		private const float deviceRefreshInterval = 1f;

		// Token: 0x04005384 RID: 21380
		private float deviceRefreshTimer;

		// Token: 0x04005385 RID: 21381
		private readonly List<InputDeviceProfile> systemDeviceProfiles;

		// Token: 0x04005386 RID: 21382
		private readonly List<InputDeviceProfile> customDeviceProfiles;

		// Token: 0x04005387 RID: 21383
		private string[] joystickNames;

		// Token: 0x04005388 RID: 21384
		private int lastJoystickCount;

		// Token: 0x04005389 RID: 21385
		private int lastJoystickHash;

		// Token: 0x0400538A RID: 21386
		private int joystickCount;

		// Token: 0x0400538B RID: 21387
		private int joystickHash;
	}
}
