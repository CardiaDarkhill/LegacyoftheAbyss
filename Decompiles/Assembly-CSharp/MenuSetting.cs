using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006EB RID: 1771
public class MenuSetting : MonoBehaviour
{
	// Token: 0x06003F9C RID: 16284 RVA: 0x001187A3 File Offset: 0x001169A3
	private void Awake()
	{
		this.vibration = base.GetComponent<VibrationPlayer>();
	}

	// Token: 0x06003F9D RID: 16285 RVA: 0x001187B1 File Offset: 0x001169B1
	private void Start()
	{
		this.gm = GameManager.instance;
		this.gs = this.gm.gameSettings;
	}

	// Token: 0x06003F9E RID: 16286 RVA: 0x001187D0 File Offset: 0x001169D0
	public void RefreshValueFromGameSettings(bool alsoApplySetting = false)
	{
		if (this.gs == null)
		{
			this.gs = GameManager.instance.gameSettings;
		}
		MenuSetting.MenuSettingType menuSettingType = this.settingType;
		switch (menuSettingType)
		{
		case MenuSetting.MenuSettingType.FullScreen:
			this.optionList.SetOptionTo(this.gs.fullScreen);
			break;
		case MenuSetting.MenuSettingType.VSync:
			this.optionList.SetOptionTo(this.gs.vSync);
			break;
		case (MenuSetting.MenuSettingType)13:
		case MenuSetting.MenuSettingType.MonitorSelect:
		case MenuSetting.MenuSettingType.SwitchFrameCap:
			break;
		case MenuSetting.MenuSettingType.ParticleLevel:
			this.optionList.SetOptionTo(this.gs.particleEffectsLevel);
			break;
		case MenuSetting.MenuSettingType.ShaderQuality:
			this.optionList.SetOptionTo((int)this.gs.shaderQuality);
			break;
		default:
			switch (menuSettingType)
			{
			case MenuSetting.MenuSettingType.GameBackerCredits:
				this.optionList.SetOptionTo(this.gs.backerCredits);
				break;
			case MenuSetting.MenuSettingType.NativeAchievements:
				this.optionList.SetOptionTo(this.gs.showNativeAchievementPopups);
				break;
			case MenuSetting.MenuSettingType.ControllerRumble:
				this.optionList.SetOptionTo(this.gs.vibrationSetting);
				break;
			case MenuSetting.MenuSettingType.PlayerVoice:
				this.optionList.SetOptionTo(this.gs.playerVoiceEnabled ? 1 : 0);
				break;
			case MenuSetting.MenuSettingType.HudVisibility:
				this.optionList.SetOptionTo(this.gs.hudScaleSetting);
				break;
			case MenuSetting.MenuSettingType.CameraShake:
				this.optionList.SetOptionTo(this.gs.cameraShakeSetting);
				break;
			}
			break;
		}
		if (alsoApplySetting)
		{
			this.UpdateSetting(this.optionList.selectedOptionIndex);
		}
	}

	// Token: 0x06003F9F RID: 16287 RVA: 0x0011895F File Offset: 0x00116B5F
	public void ChangeSetting(int settingIndex)
	{
		this.UpdateSetting(settingIndex);
		if (this.settingType == MenuSetting.MenuSettingType.ControllerRumble && this.vibration)
		{
			this.vibration.Play();
		}
	}

	// Token: 0x06003FA0 RID: 16288 RVA: 0x0011898C File Offset: 0x00116B8C
	public void UpdateSetting(int settingIndex)
	{
		if (this.gs == null)
		{
			this.gs = GameManager.instance.gameSettings;
		}
		MenuSetting.MenuSettingType menuSettingType = this.settingType;
		switch (menuSettingType)
		{
		case MenuSetting.MenuSettingType.FullScreen:
			this.gs.fullScreen = settingIndex;
			if (settingIndex == 0)
			{
				Screen.fullScreenMode = FullScreenMode.Windowed;
				return;
			}
			if (settingIndex != 2)
			{
				Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
				return;
			}
			Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
			return;
		case MenuSetting.MenuSettingType.VSync:
			if (settingIndex == 0)
			{
				this.gs.vSync = 0;
				QualitySettings.vSyncCount = 0;
				return;
			}
			this.gs.vSync = 1;
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = -1;
			UIManager.instance.DisableFrameCapSetting();
			return;
		case (MenuSetting.MenuSettingType)13:
		case MenuSetting.MenuSettingType.MonitorSelect:
		case MenuSetting.MenuSettingType.SwitchFrameCap:
			break;
		case MenuSetting.MenuSettingType.ParticleLevel:
			if (settingIndex == 0)
			{
				this.gs.particleEffectsLevel = 0;
				this.gm.RefreshParticleSystems();
				return;
			}
			this.gs.particleEffectsLevel = 1;
			this.gm.RefreshParticleSystems();
			return;
		case MenuSetting.MenuSettingType.ShaderQuality:
			this.gs.shaderQuality = ((settingIndex == 0) ? ShaderQualities.Low : ShaderQualities.High);
			return;
		default:
			switch (menuSettingType)
			{
			case MenuSetting.MenuSettingType.GameBackerCredits:
				this.gs.backerCredits = ((settingIndex == 0) ? 0 : 1);
				return;
			case MenuSetting.MenuSettingType.NativeAchievements:
				this.gs.showNativeAchievementPopups = ((settingIndex == 0) ? 0 : 1);
				return;
			case (MenuSetting.MenuSettingType)36:
				break;
			case MenuSetting.MenuSettingType.ControllerRumble:
				VibrationManager.VibrationSetting = (VibrationManager.VibrationSettings)settingIndex;
				this.gs.vibrationSetting = settingIndex;
				return;
			case MenuSetting.MenuSettingType.PlayerVoice:
				this.gs.playerVoiceEnabled = (settingIndex == 1);
				break;
			case MenuSetting.MenuSettingType.HudVisibility:
			{
				HudGlobalHide.IsHidden = (settingIndex == 2);
				HudGlobalHide.IsReduced = (HudScalePositioner.IsReduced = (settingIndex == 1));
				int num = settingIndex;
				if (num == 2)
				{
					GameSettings.LoadInt("HudScaleSetting", ref num, 0);
				}
				this.gs.hudScaleSetting = num;
				return;
			}
			case MenuSetting.MenuSettingType.CameraShake:
				CameraShakeManager.ShakeSetting = (CameraShakeManager.ShakeSettings)settingIndex;
				this.gs.cameraShakeSetting = settingIndex;
				return;
			default:
				return;
			}
			break;
		}
	}

	// Token: 0x04004147 RID: 16711
	public MenuSetting.MenuSettingType settingType;

	// Token: 0x04004148 RID: 16712
	public MenuOptionHorizontal optionList;

	// Token: 0x04004149 RID: 16713
	private VibrationPlayer vibration;

	// Token: 0x0400414A RID: 16714
	private GameManager gm;

	// Token: 0x0400414B RID: 16715
	protected GameSettings gs;

	// Token: 0x0400414C RID: 16716
	private bool verboseMode;

	// Token: 0x020019DC RID: 6620
	public enum MenuSettingType
	{
		// Token: 0x04009764 RID: 38756
		Resolution = 10,
		// Token: 0x04009765 RID: 38757
		FullScreen,
		// Token: 0x04009766 RID: 38758
		VSync,
		// Token: 0x04009767 RID: 38759
		MonitorSelect = 14,
		// Token: 0x04009768 RID: 38760
		SwitchFrameCap,
		// Token: 0x04009769 RID: 38761
		ParticleLevel,
		// Token: 0x0400976A RID: 38762
		ShaderQuality,
		// Token: 0x0400976B RID: 38763
		GameLanguage = 33,
		// Token: 0x0400976C RID: 38764
		GameBackerCredits,
		// Token: 0x0400976D RID: 38765
		NativeAchievements,
		// Token: 0x0400976E RID: 38766
		ControllerRumble = 37,
		// Token: 0x0400976F RID: 38767
		PlayerVoice,
		// Token: 0x04009770 RID: 38768
		HudVisibility,
		// Token: 0x04009771 RID: 38769
		CameraShake
	}
}
