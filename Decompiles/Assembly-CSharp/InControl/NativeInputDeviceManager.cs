using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using InControl.NativeDeviceProfiles;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000916 RID: 2326
	public class NativeInputDeviceManager : InputDeviceManager
	{
		// Token: 0x0600528A RID: 21130 RVA: 0x00179448 File Offset: 0x00177648
		public NativeInputDeviceManager()
		{
			this.attachedDevices = new List<NativeInputDevice>();
			this.detachedDevices = new List<NativeInputDevice>();
			this.systemDeviceProfiles = new List<InputDeviceProfile>(NativeInputDeviceProfileList.Profiles.Length);
			this.customDeviceProfiles = new List<InputDeviceProfile>();
			this.deviceEvents = new uint[32];
			this.AddSystemDeviceProfiles();
			NativeInputOptions options = default(NativeInputOptions);
			options.enableXInput = (InputManager.NativeInputEnableXInput ? 1 : 0);
			options.enableMFi = (InputManager.NativeInputEnableMFi ? 1 : 0);
			options.preventSleep = (InputManager.NativeInputPreventSleep ? 1 : 0);
			if (InputManager.NativeInputUpdateRate > 0U)
			{
				options.updateRate = (ushort)InputManager.NativeInputUpdateRate;
			}
			else
			{
				options.updateRate = (ushort)Mathf.FloorToInt(1f / Time.fixedDeltaTime);
			}
			Native.Init(options);
		}

		// Token: 0x0600528B RID: 21131 RVA: 0x00179514 File Offset: 0x00177714
		public override void Destroy()
		{
			Native.Stop();
		}

		// Token: 0x0600528C RID: 21132 RVA: 0x0017951C File Offset: 0x0017771C
		public override void Update(ulong updateTick, float deltaTime)
		{
			IntPtr source;
			int num = Native.GetDeviceEvents(out source);
			if (num > 0)
			{
				Utility.ArrayExpand<uint>(ref this.deviceEvents, num);
				MarshalUtility.Copy(source, this.deviceEvents, num);
				int num2 = 0;
				uint num3 = this.deviceEvents[num2++];
				int num4 = 0;
				while ((long)num4 < (long)((ulong)num3))
				{
					uint num5 = this.deviceEvents[num2++];
					StringBuilder stringBuilder = new StringBuilder(256);
					stringBuilder.Append("Attached native device with handle " + num5.ToString() + ":\n");
					InputDeviceInfo inputDeviceInfo;
					if (Native.GetDeviceInfo(num5, out inputDeviceInfo))
					{
						stringBuilder.AppendFormat("Name: {0}\n", inputDeviceInfo.name);
						stringBuilder.AppendFormat("Driver Type: {0}\n", inputDeviceInfo.driverType);
						stringBuilder.AppendFormat("Location ID: {0}\n", inputDeviceInfo.location);
						stringBuilder.AppendFormat("Serial Number: {0}\n", inputDeviceInfo.serialNumber);
						stringBuilder.AppendFormat("Vendor ID: 0x{0:x}\n", inputDeviceInfo.vendorID);
						stringBuilder.AppendFormat("Product ID: 0x{0:x}\n", inputDeviceInfo.productID);
						stringBuilder.AppendFormat("Version Number: 0x{0:x}\n", inputDeviceInfo.versionNumber);
						stringBuilder.AppendFormat("Buttons: {0}\n", inputDeviceInfo.numButtons);
						stringBuilder.AppendFormat("Analogs: {0}\n", inputDeviceInfo.numAnalogs);
						this.DetectDevice(num5, inputDeviceInfo);
					}
					Logger.LogInfo(stringBuilder.ToString());
					num4++;
				}
				uint num6 = this.deviceEvents[num2++];
				int num7 = 0;
				while ((long)num7 < (long)((ulong)num6))
				{
					uint deviceHandle = this.deviceEvents[num2++];
					Logger.LogInfo("Detached native device with handle " + deviceHandle.ToString() + ":");
					NativeInputDevice nativeInputDevice = this.FindAttachedDevice(deviceHandle);
					if (nativeInputDevice != null)
					{
						this.DetachDevice(nativeInputDevice);
					}
					else
					{
						Logger.LogWarning("Couldn't find device to detach with handle: " + deviceHandle.ToString());
					}
					num7++;
				}
			}
		}

		// Token: 0x0600528D RID: 21133 RVA: 0x00179720 File Offset: 0x00177920
		private void DetectDevice(uint deviceHandle, InputDeviceInfo deviceInfo)
		{
			InputDeviceProfile inputDeviceProfile = null;
			inputDeviceProfile = (inputDeviceProfile ?? this.customDeviceProfiles.Find((InputDeviceProfile profile) => profile.Matches(deviceInfo)));
			inputDeviceProfile = (inputDeviceProfile ?? this.systemDeviceProfiles.Find((InputDeviceProfile profile) => profile.Matches(deviceInfo)));
			inputDeviceProfile = (inputDeviceProfile ?? this.customDeviceProfiles.Find((InputDeviceProfile profile) => profile.LastResortMatches(deviceInfo)));
			inputDeviceProfile = (inputDeviceProfile ?? this.systemDeviceProfiles.Find((InputDeviceProfile profile) => profile.LastResortMatches(deviceInfo)));
			if (inputDeviceProfile == null || inputDeviceProfile.IsNotHidden)
			{
				NativeInputDevice nativeInputDevice = this.FindDetachedDevice(deviceInfo) ?? new NativeInputDevice();
				nativeInputDevice.Initialize(deviceHandle, deviceInfo, inputDeviceProfile);
				this.AttachDevice(nativeInputDevice);
			}
		}

		// Token: 0x0600528E RID: 21134 RVA: 0x001797E6 File Offset: 0x001779E6
		private void AttachDevice(NativeInputDevice device)
		{
			this.detachedDevices.Remove(device);
			this.attachedDevices.Add(device);
			InputManager.AttachDevice(device);
		}

		// Token: 0x0600528F RID: 21135 RVA: 0x00179807 File Offset: 0x00177A07
		private void DetachDevice(NativeInputDevice device)
		{
			this.attachedDevices.Remove(device);
			this.detachedDevices.Add(device);
			InputManager.DetachDevice(device);
		}

		// Token: 0x06005290 RID: 21136 RVA: 0x00179828 File Offset: 0x00177A28
		private NativeInputDevice FindAttachedDevice(uint deviceHandle)
		{
			int count = this.attachedDevices.Count;
			for (int i = 0; i < count; i++)
			{
				NativeInputDevice nativeInputDevice = this.attachedDevices[i];
				if (nativeInputDevice.Handle == deviceHandle)
				{
					return nativeInputDevice;
				}
			}
			return null;
		}

		// Token: 0x06005291 RID: 21137 RVA: 0x00179868 File Offset: 0x00177A68
		private NativeInputDevice FindDetachedDevice(InputDeviceInfo deviceInfo)
		{
			ReadOnlyCollection<NativeInputDevice> arg = new ReadOnlyCollection<NativeInputDevice>(this.detachedDevices);
			if (NativeInputDeviceManager.CustomFindDetachedDevice != null)
			{
				return NativeInputDeviceManager.CustomFindDetachedDevice(deviceInfo, arg);
			}
			return NativeInputDeviceManager.SystemFindDetachedDevice(deviceInfo, arg);
		}

		// Token: 0x06005292 RID: 21138 RVA: 0x0017989C File Offset: 0x00177A9C
		private static NativeInputDevice SystemFindDetachedDevice(InputDeviceInfo deviceInfo, ReadOnlyCollection<NativeInputDevice> detachedDevices)
		{
			int count = detachedDevices.Count;
			for (int i = 0; i < count; i++)
			{
				NativeInputDevice nativeInputDevice = detachedDevices[i];
				if (nativeInputDevice.Info.HasSameVendorID(deviceInfo) && nativeInputDevice.Info.HasSameProductID(deviceInfo) && nativeInputDevice.Info.HasSameSerialNumber(deviceInfo))
				{
					return nativeInputDevice;
				}
			}
			for (int j = 0; j < count; j++)
			{
				NativeInputDevice nativeInputDevice2 = detachedDevices[j];
				if (nativeInputDevice2.Info.HasSameVendorID(deviceInfo) && nativeInputDevice2.Info.HasSameProductID(deviceInfo) && nativeInputDevice2.Info.HasSameLocation(deviceInfo))
				{
					return nativeInputDevice2;
				}
			}
			for (int k = 0; k < count; k++)
			{
				NativeInputDevice nativeInputDevice3 = detachedDevices[k];
				if (nativeInputDevice3.Info.HasSameVendorID(deviceInfo) && nativeInputDevice3.Info.HasSameProductID(deviceInfo) && nativeInputDevice3.Info.HasSameVersionNumber(deviceInfo))
				{
					return nativeInputDevice3;
				}
			}
			for (int l = 0; l < count; l++)
			{
				NativeInputDevice nativeInputDevice4 = detachedDevices[l];
				if (nativeInputDevice4.Info.HasSameLocation(deviceInfo))
				{
					return nativeInputDevice4;
				}
			}
			return null;
		}

		// Token: 0x06005293 RID: 21139 RVA: 0x001799CF File Offset: 0x00177BCF
		private void AddSystemDeviceProfile(InputDeviceProfile deviceProfile)
		{
			if (deviceProfile != null && deviceProfile.IsSupportedOnThisPlatform)
			{
				this.systemDeviceProfiles.Add(deviceProfile);
			}
		}

		// Token: 0x06005294 RID: 21140 RVA: 0x001799E8 File Offset: 0x00177BE8
		private void AddSystemDeviceProfiles()
		{
			for (int i = 0; i < NativeInputDeviceProfileList.Profiles.Length; i++)
			{
				InputDeviceProfile deviceProfile = InputDeviceProfile.CreateInstanceOfType(NativeInputDeviceProfileList.Profiles[i]);
				this.AddSystemDeviceProfile(deviceProfile);
			}
		}

		// Token: 0x06005295 RID: 21141 RVA: 0x00179A1C File Offset: 0x00177C1C
		public static bool CheckPlatformSupport(ICollection<string> errors)
		{
			if (Application.platform != RuntimePlatform.OSXPlayer && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.tvOS)
			{
				return false;
			}
			try
			{
				NativeVersionInfo nativeVersionInfo;
				Native.GetVersionInfo(out nativeVersionInfo);
				Logger.LogInfo(string.Concat(new string[]
				{
					"InControl Native (version ",
					nativeVersionInfo.major.ToString(),
					".",
					nativeVersionInfo.minor.ToString(),
					".",
					nativeVersionInfo.patch.ToString(),
					")"
				}));
			}
			catch (DllNotFoundException ex)
			{
				if (errors != null)
				{
					errors.Add(ex.Message + Utility.PluginFileExtension() + " could not be found or is missing a dependency.");
				}
				return false;
			}
			return true;
		}

		// Token: 0x06005296 RID: 21142 RVA: 0x00179AF8 File Offset: 0x00177CF8
		internal static bool Enable()
		{
			List<string> list = new List<string>();
			if (NativeInputDeviceManager.CheckPlatformSupport(list))
			{
				if (InputManager.NativeInputEnableMFi)
				{
					InputManager.HideDevicesWithProfile(typeof(XboxOneSBluetoothMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(XboxSeriesXBluetoothMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(PlayStation4MacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(PlayStation5USBMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(PlayStation5BluetoothMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(SteelseriesNimbusMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(HoriPadUltimateMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(NintendoSwitchProMacNativeProfile));
				}
				InputManager.AddDeviceManager<NativeInputDeviceManager>();
				return true;
			}
			foreach (string str in list)
			{
				Logger.LogError("Error enabling NativeInputDeviceManager: " + str);
			}
			return false;
		}

		// Token: 0x040052B8 RID: 21176
		public static Func<InputDeviceInfo, ReadOnlyCollection<NativeInputDevice>, NativeInputDevice> CustomFindDetachedDevice;

		// Token: 0x040052B9 RID: 21177
		private readonly List<NativeInputDevice> attachedDevices;

		// Token: 0x040052BA RID: 21178
		private readonly List<NativeInputDevice> detachedDevices;

		// Token: 0x040052BB RID: 21179
		private readonly List<InputDeviceProfile> systemDeviceProfiles;

		// Token: 0x040052BC RID: 21180
		private readonly List<InputDeviceProfile> customDeviceProfiles;

		// Token: 0x040052BD RID: 21181
		private uint[] deviceEvents;
	}
}
