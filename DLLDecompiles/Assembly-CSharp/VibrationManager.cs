using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;

// Token: 0x020007AA RID: 1962
public static class VibrationManager
{
	// Token: 0x170007BC RID: 1980
	// (get) Token: 0x06004554 RID: 17748 RVA: 0x0012EBEF File Offset: 0x0012CDEF
	// (set) Token: 0x06004555 RID: 17749 RVA: 0x0012EBF6 File Offset: 0x0012CDF6
	public static VibrationManager.VibrationSettings VibrationSetting
	{
		get
		{
			return VibrationManager._vibrationSetting;
		}
		set
		{
			if (VibrationManager._vibrationSetting != value)
			{
				VibrationManager._vibrationSetting = value;
				if (VibrationManager._vibrationSetting == VibrationManager.VibrationSettings.Off)
				{
					VibrationManager.StopAllVibration();
				}
			}
		}
	}

	// Token: 0x170007BD RID: 1981
	// (get) Token: 0x06004556 RID: 17750 RVA: 0x0012EC14 File Offset: 0x0012CE14
	public static float StrengthMultiplier
	{
		get
		{
			switch (VibrationManager._vibrationSetting)
			{
			case VibrationManager.VibrationSettings.On:
				return 1f * VibrationManager.internalStrength;
			case VibrationManager.VibrationSettings.Reduced:
				return ConfigManager.ReducedControllerRumble * VibrationManager.internalStrength;
			case VibrationManager.VibrationSettings.Off:
				return 0f;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	// Token: 0x170007BE RID: 1982
	// (get) Token: 0x06004557 RID: 17751 RVA: 0x0012EC5E File Offset: 0x0012CE5E
	public static float InternalStrength
	{
		get
		{
			return VibrationManager.internalStrength;
		}
	}

	// Token: 0x06004558 RID: 17752 RVA: 0x0012EC65 File Offset: 0x0012CE65
	private static void Init()
	{
		if (VibrationManager.initialised)
		{
			return;
		}
		VibrationManager.vibrationManagerUpdater = new GameObject("Vibration Manager Updater").AddComponent<VibrationManagerUpdater>();
		VibrationManager.initialised = true;
	}

	// Token: 0x06004559 RID: 17753 RVA: 0x0012EC89 File Offset: 0x0012CE89
	public static void FadeVibration(float strength, float duration)
	{
		if (duration <= 0f)
		{
			VibrationManager.internalStrength = strength;
			return;
		}
		VibrationManager.Init();
		VibrationManager.targetStrength = strength;
		VibrationManager.transitionRate = (VibrationManager.targetStrength - VibrationManager.internalStrength) / duration;
		VibrationManager.vibrationManagerUpdater.enabled = true;
	}

	// Token: 0x0600455A RID: 17754 RVA: 0x0012ECC4 File Offset: 0x0012CEC4
	public static bool Update()
	{
		if (VibrationManager.transitionRate == 0f)
		{
			return false;
		}
		VibrationManager.internalStrength += VibrationManager.transitionRate * Time.unscaledDeltaTime;
		if (VibrationManager.transitionRate > 0f)
		{
			if (VibrationManager.internalStrength >= VibrationManager.targetStrength)
			{
				VibrationManager.internalStrength = VibrationManager.targetStrength;
				VibrationManager.transitionRate = 0f;
			}
		}
		else if (VibrationManager.internalStrength <= VibrationManager.targetStrength)
		{
			VibrationManager.internalStrength = VibrationManager.targetStrength;
			VibrationManager.transitionRate = 0f;
		}
		return VibrationManager.transitionRate != 0f;
	}

	// Token: 0x0600455B RID: 17755 RVA: 0x0012ED54 File Offset: 0x0012CF54
	public static VibrationMixer GetMixer()
	{
		Platform platform = Platform.Current;
		if (platform != null)
		{
			VibrationManager.IVibrationMixerProvider vibrationMixerProvider = platform as VibrationManager.IVibrationMixerProvider;
			if (vibrationMixerProvider != null)
			{
				VibrationMixer vibrationMixer = vibrationMixerProvider.GetVibrationMixer();
				if (vibrationMixer != null)
				{
					return vibrationMixer;
				}
			}
		}
		InputDevice activeDevice = InputManager.ActiveDevice;
		if (activeDevice != null && activeDevice.IsAttached)
		{
			VibrationManager.IVibrationMixerProvider vibrationMixerProvider2 = activeDevice as VibrationManager.IVibrationMixerProvider;
			if (vibrationMixerProvider2 != null)
			{
				VibrationMixer vibrationMixer2 = vibrationMixerProvider2.GetVibrationMixer();
				if (vibrationMixer2 != null)
				{
					return vibrationMixer2;
				}
			}
		}
		return null;
	}

	// Token: 0x0600455C RID: 17756 RVA: 0x0012EDB4 File Offset: 0x0012CFB4
	public static VibrationEmission PlayVibrationClipOneShot(VibrationData vibrationData, VibrationTarget? vibrationTarget = null, bool isLooping = false, string tag = "", bool isRealtime = false)
	{
		if (VibrationManager._vibrationSetting == VibrationManager.VibrationSettings.Off)
		{
			return null;
		}
		VibrationMixer mixer = VibrationManager.GetMixer();
		if (mixer == null)
		{
			return null;
		}
		VibrationEmission vibrationEmission = mixer.PlayEmission(vibrationData, vibrationTarget ?? new VibrationTarget(VibrationMotors.All), isLooping, tag, isRealtime);
		if (vibrationEmission != null)
		{
			vibrationEmission.BaseStrength = vibrationData.Strength;
		}
		return vibrationEmission;
	}

	// Token: 0x0600455D RID: 17757 RVA: 0x0012EE10 File Offset: 0x0012D010
	public static VibrationEmission PlayVibrationClipOneShot(VibrationEmission emission)
	{
		if (VibrationManager._vibrationSetting == VibrationManager.VibrationSettings.Off)
		{
			return null;
		}
		if (emission == null)
		{
			return null;
		}
		emission.SetPlaybackTime(0f);
		VibrationMixer mixer = VibrationManager.GetMixer();
		if (mixer == null)
		{
			return null;
		}
		return mixer.PlayEmission(emission);
	}

	// Token: 0x0600455E RID: 17758 RVA: 0x0012EE4C File Offset: 0x0012D04C
	public static void StopAllVibration()
	{
		if (VibrationManager._vibrationSetting == VibrationManager.VibrationSettings.Off)
		{
			return;
		}
		VibrationMixer mixer = VibrationManager.GetMixer();
		if (mixer == null)
		{
			return;
		}
		mixer.StopAllEmissions();
	}

	// Token: 0x0600455F RID: 17759 RVA: 0x0012EE74 File Offset: 0x0012D074
	public static void StopAllVibrationsWithTag(string tag)
	{
		if (VibrationManager._vibrationSetting == VibrationManager.VibrationSettings.Off)
		{
			return;
		}
		VibrationMixer mixer = VibrationManager.GetMixer();
		if (mixer == null)
		{
			return;
		}
		mixer.StopAllEmissionsWithTag(tag);
	}

	// Token: 0x06004560 RID: 17760 RVA: 0x0012EE9C File Offset: 0x0012D09C
	public static void GetVibrationsWithTag(string tag, List<VibrationEmission> emissions)
	{
		if (VibrationManager._vibrationSetting == VibrationManager.VibrationSettings.Off)
		{
			return;
		}
		if (emissions == null)
		{
			return;
		}
		VibrationMixer mixer = VibrationManager.GetMixer();
		if (mixer == null)
		{
			return;
		}
		bool flag = string.IsNullOrEmpty(tag);
		if (!flag)
		{
			tag = tag.Trim();
		}
		for (int i = 0; i < mixer.PlayingEmissionCount; i++)
		{
			VibrationEmission playingEmission = mixer.GetPlayingEmission(i);
			if (flag || !(playingEmission.Tag != tag))
			{
				emissions.Add(playingEmission);
			}
		}
	}

	// Token: 0x0400461E RID: 17950
	private static VibrationManager.VibrationSettings _vibrationSetting;

	// Token: 0x0400461F RID: 17951
	private static float internalStrength = 1f;

	// Token: 0x04004620 RID: 17952
	private static float targetStrength = 0f;

	// Token: 0x04004621 RID: 17953
	private static float transitionRate;

	// Token: 0x04004622 RID: 17954
	private static bool initialised;

	// Token: 0x04004623 RID: 17955
	private static VibrationManagerUpdater vibrationManagerUpdater;

	// Token: 0x02001A85 RID: 6789
	public enum VibrationSettings
	{
		// Token: 0x040099C9 RID: 39369
		On,
		// Token: 0x040099CA RID: 39370
		Reduced,
		// Token: 0x040099CB RID: 39371
		Off
	}

	// Token: 0x02001A86 RID: 6790
	public interface IVibrationMixerProvider
	{
		// Token: 0x06009730 RID: 38704
		VibrationMixer GetVibrationMixer();
	}
}
