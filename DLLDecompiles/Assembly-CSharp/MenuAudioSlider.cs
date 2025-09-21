using System;
using System.Globalization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Token: 0x020006D9 RID: 1753
public class MenuAudioSlider : MonoBehaviour
{
	// Token: 0x06003F5D RID: 16221 RVA: 0x00117CF8 File Offset: 0x00115EF8
	private void Start()
	{
		this.gs = GameManager.instance.gameSettings;
		if (this.textUI)
		{
			this.fixVerticalAlign = this.textUI.GetComponent<FixVerticalAlign>();
			this.hasVerticalAlign = this.fixVerticalAlign;
		}
		this.UpdateValue();
		base.gameObject.AddComponentIfNotPresent<SliderRightStickInput>();
	}

	// Token: 0x06003F5E RID: 16222 RVA: 0x00117D58 File Offset: 0x00115F58
	public void UpdateValue()
	{
		this.textUI.text = this.slider.value.ToString(CultureInfo.InvariantCulture);
		if (this.hasVerticalAlign)
		{
			this.fixVerticalAlign.AlignAuto();
		}
	}

	// Token: 0x06003F5F RID: 16223 RVA: 0x00117D9C File Offset: 0x00115F9C
	public void RefreshValueFromSettings()
	{
		if (this.gs == null)
		{
			this.gs = GameManager.instance.gameSettings;
		}
		switch (this.audioSetting)
		{
		case MenuAudioSlider.AudioSettingType.MasterVolume:
			this.slider.value = this.gs.masterVolume;
			this.UpdateValue();
			return;
		case MenuAudioSlider.AudioSettingType.MusicVolume:
			this.slider.value = this.gs.musicVolume;
			this.UpdateValue();
			return;
		case MenuAudioSlider.AudioSettingType.SoundVolume:
			this.slider.value = this.gs.soundVolume;
			this.UpdateValue();
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06003F60 RID: 16224 RVA: 0x00117E38 File Offset: 0x00116038
	public void UpdateTextValue(float newValue)
	{
		this.textUI.text = newValue.ToString(CultureInfo.InvariantCulture);
	}

	// Token: 0x06003F61 RID: 16225 RVA: 0x00117E54 File Offset: 0x00116054
	private float GetVolumeLevel(float sourceLevel)
	{
		return new MinMaxFloat(this.slider.minValue, this.slider.maxValue).GetTBetween(sourceLevel);
	}

	// Token: 0x06003F62 RID: 16226 RVA: 0x00117E88 File Offset: 0x00116088
	public void SetMasterLevel(float masterLevel)
	{
		float value = global::Helper.LinearToDecibel(this.GetVolumeLevel(masterLevel));
		this.masterMixer.SetFloat("MasterVolume", value);
		this.gs.masterVolume = masterLevel;
	}

	// Token: 0x06003F63 RID: 16227 RVA: 0x00117EC0 File Offset: 0x001160C0
	public void SetMusicLevel(float musicLevel)
	{
		float value = global::Helper.LinearToDecibel(this.GetVolumeLevel(musicLevel));
		this.masterMixer.SetFloat("MusicVolume", value);
		this.gs.musicVolume = musicLevel;
	}

	// Token: 0x06003F64 RID: 16228 RVA: 0x00117EF8 File Offset: 0x001160F8
	public void SetSoundLevel(float soundLevel)
	{
		float value = global::Helper.LinearToDecibel(this.GetVolumeLevel(soundLevel));
		this.masterMixer.SetFloat("SFXVolume", value);
		this.uiMixer.SetFloat("UIVolume", value);
		this.cinematicMixer.SetFloat("CinematicVolume", value);
		this.gs.soundVolume = soundLevel;
	}

	// Token: 0x04004125 RID: 16677
	[SerializeField]
	[Tooltip("The slider being controlled.")]
	private Slider slider;

	// Token: 0x04004126 RID: 16678
	[SerializeField]
	[Tooltip("The Text UI object that displays the value of the slider.")]
	private Text textUI;

	// Token: 0x04004127 RID: 16679
	[SerializeField]
	[Tooltip("The master audio mixer containing the variables we want to set.")]
	private AudioMixer masterMixer;

	// Token: 0x04004128 RID: 16680
	[SerializeField]
	private AudioMixer uiMixer;

	// Token: 0x04004129 RID: 16681
	[SerializeField]
	private AudioMixer cinematicMixer;

	// Token: 0x0400412A RID: 16682
	[SerializeField]
	[Tooltip("The setting to load when this control is loaded.")]
	private MenuAudioSlider.AudioSettingType audioSetting;

	// Token: 0x0400412B RID: 16683
	private GameSettings gs;

	// Token: 0x0400412C RID: 16684
	private FixVerticalAlign fixVerticalAlign;

	// Token: 0x0400412D RID: 16685
	private bool hasVerticalAlign;

	// Token: 0x020019D6 RID: 6614
	private enum AudioSettingType
	{
		// Token: 0x04009752 RID: 38738
		MasterVolume,
		// Token: 0x04009753 RID: 38739
		MusicVolume,
		// Token: 0x04009754 RID: 38740
		SoundVolume
	}
}
