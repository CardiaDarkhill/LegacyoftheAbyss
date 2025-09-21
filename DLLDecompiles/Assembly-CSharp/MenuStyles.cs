using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020006EC RID: 1772
public class MenuStyles : MonoBehaviour
{
	// Token: 0x17000749 RID: 1865
	// (get) Token: 0x06003FA2 RID: 16290 RVA: 0x00118B48 File Offset: 0x00116D48
	public int CurrentStyle
	{
		get
		{
			if (this.currentSettings.StyleIndex < 0 || this.currentSettings.StyleIndex >= this.Styles.Length)
			{
				this.currentSettings.StyleIndex = Mathf.Clamp(this.currentSettings.StyleIndex, 0, this.Styles.Length - 1);
			}
			return this.currentSettings.StyleIndex;
		}
	}

	// Token: 0x06003FA3 RID: 16291 RVA: 0x00118BAC File Offset: 0x00116DAC
	private void Awake()
	{
		MenuStyles.Instance = this;
		foreach (MenuStyles.MenuStyle menuStyle in this.Styles)
		{
			if (menuStyle.StyleObject)
			{
				menuStyle.StyleObject.SetActive(false);
			}
		}
		Platform.OnSaveStoreStateChanged += this.OnSaveStoreStateChanged;
	}

	// Token: 0x06003FA4 RID: 16292 RVA: 0x00118C02 File Offset: 0x00116E02
	private void Start()
	{
		this.started = true;
		this.LoadRecentMenuStyle(false);
	}

	// Token: 0x06003FA5 RID: 16293 RVA: 0x00118C12 File Offset: 0x00116E12
	private void OnDestroy()
	{
		MenuStyles.Instance = null;
		Platform.OnSaveStoreStateChanged -= this.OnSaveStoreStateChanged;
	}

	// Token: 0x06003FA6 RID: 16294 RVA: 0x00118C2B File Offset: 0x00116E2B
	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			this.LoadRecentMenuStyle(this.started);
		}
	}

	// Token: 0x06003FA7 RID: 16295 RVA: 0x00118C3C File Offset: 0x00116E3C
	private void LoadRecentMenuStyle(bool fade)
	{
		this.currentSettings = this.StyleDefault;
		if (Platform.Current.IsSaveStoreMounted)
		{
			foreach (MenuStyles.StyleSettingsPlatform styleSettingsPlatform in this.StylePlatforms)
			{
				if (Application.platform == styleSettingsPlatform.Platform)
				{
					this.currentSettings = styleSettingsPlatform.Settings;
					break;
				}
			}
			if (Platform.Current.LocalSharedData.HasKey("lastVersion") && Platform.Current.LocalSharedData.GetString("lastVersion", "0.0.0.0") == this.currentSettings.AutoChangeVersion)
			{
				int num = Mathf.Clamp(Platform.Current.LocalSharedData.GetInt("menuStyle", this.currentSettings.StyleIndex), 0, this.Styles.Length - 1);
				if (num >= 0 && num < this.Styles.Length && this.Styles[num].IsAvailable)
				{
					this.currentSettings.StyleIndex = num;
				}
			}
			string key = "unlockedMenuStyle";
			if (Platform.Current.RoamingSharedData.HasKey(key))
			{
				string @string = Platform.Current.RoamingSharedData.GetString(key, "");
				Platform.Current.RoamingSharedData.DeleteKey(key);
				Platform.Current.RoamingSharedData.Save();
				for (int j = 0; j < this.Styles.Length; j++)
				{
					if (this.Styles[j].UnlockKey == @string && this.Styles[j].IsAvailable)
					{
						this.currentSettings.StyleIndex = j;
						break;
					}
				}
			}
		}
		this.SetStyle(this.currentSettings.StyleIndex, fade, true);
	}

	// Token: 0x06003FA8 RID: 16296 RVA: 0x00118DF0 File Offset: 0x00116FF0
	public void SetStyle(int index, bool fade, bool save = true)
	{
		if (index < 0 || index >= this.Styles.Length)
		{
			return;
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.SwitchStyle(index, fade, this.currentSettings.StyleIndex));
		this.currentSettings.StyleIndex = index;
		if (!save)
		{
			return;
		}
		Platform.Current.LocalSharedData.SetString("lastVersion", this.currentSettings.AutoChangeVersion);
		Platform.Current.LocalSharedData.SetInt("menuStyle", this.currentSettings.StyleIndex);
		Platform.Current.LocalSharedData.Save();
	}

	// Token: 0x06003FA9 RID: 16297 RVA: 0x00118E9D File Offset: 0x0011709D
	private IEnumerator SwitchStyle(int index, bool fade, int oldIndex)
	{
		yield return null;
		MenuStyles.MenuStyle menuStyle = this.Styles[oldIndex];
		MenuStyles.MenuStyle newStyle = this.Styles[index];
		AudioSource[] componentsInChildren = menuStyle.StyleObject.GetComponentsInChildren<AudioSource>();
		menuStyle.SetInitialAudioVolumes(componentsInChildren);
		yield return base.StartCoroutine(this.Fade(oldIndex, MenuStyles.FadeType.Down, fade, componentsInChildren));
		for (int i = 0; i < this.Styles.Length; i++)
		{
			MenuStyles.MenuStyle menuStyle2 = this.Styles[i];
			bool flag = index == i;
			menuStyle2.StyleObject.SetActive(flag);
			if (flag && menuStyle2.Foreground)
			{
				menuStyle2.Foreground.AlphaSelf = (float)(this.isInSubMenu ? 0 : 1);
			}
		}
		GameCameras instance = GameCameras.instance;
		if (instance && instance.colorCorrectionCurves)
		{
			MenuStyles.MenuStyle.CameraCurves cameraColorCorrection = newStyle.CameraColorCorrection;
			instance.colorCorrectionCurves.saturation = cameraColorCorrection.Saturation;
			instance.colorCorrectionCurves.redChannel = cameraColorCorrection.RedChannel;
			instance.colorCorrectionCurves.greenChannel = cameraColorCorrection.GreenChannel;
			instance.colorCorrectionCurves.blueChannel = cameraColorCorrection.BlueChannel;
		}
		CustomSceneManager.SetLighting(newStyle.AmbientColor, newStyle.AmbientIntensity);
		BlurPlane.SetVibranceOffset(newStyle.BlurPlaneVibranceOffset);
		componentsInChildren = newStyle.StyleObject.GetComponentsInChildren<AudioSource>();
		newStyle.SetInitialAudioVolumes(componentsInChildren);
		yield return base.StartCoroutine(this.Fade(index, MenuStyles.FadeType.Up, fade, componentsInChildren));
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x06003FAA RID: 16298 RVA: 0x00118EC1 File Offset: 0x001170C1
	private IEnumerator Fade(int styleIndex, MenuStyles.FadeType fadeType, bool fade, AudioSource[] audioSources)
	{
		float toAlpha = (float)((fadeType == MenuStyles.FadeType.Down) ? 1 : 0);
		if (!this.BlackSolid)
		{
			yield break;
		}
		Color color = this.BlackSolid.color;
		float startAlpha = color.a;
		if (fade)
		{
			for (float elapsed = 0f; elapsed < this.FadeTime; elapsed += Time.deltaTime)
			{
				float t = elapsed / this.FadeTime;
				color.a = Mathf.Lerp(startAlpha, toAlpha, t);
				this.BlackSolid.color = color;
				for (int i = 0; i < audioSources.Length; i++)
				{
					float num = this.Styles[styleIndex].InitialAudioVolumes[i];
					audioSources[i].volume = ((fadeType == MenuStyles.FadeType.Down) ? Mathf.Lerp(num, 0f, t) : Mathf.Lerp(0f, num, t));
				}
				yield return null;
			}
			for (int j = 0; j < audioSources.Length; j++)
			{
				float volume = this.Styles[styleIndex].InitialAudioVolumes[j];
				if (fadeType == MenuStyles.FadeType.Down)
				{
					audioSources[j].volume = 0f;
				}
				else
				{
					audioSources[j].volume = volume;
				}
			}
		}
		color.a = toAlpha;
		this.BlackSolid.color = color;
		yield break;
	}

	// Token: 0x06003FAB RID: 16299 RVA: 0x00118EF0 File Offset: 0x001170F0
	public void StopAudio()
	{
		AudioSource[] componentsInChildren = this.Styles[this.CurrentStyle].StyleObject.GetComponentsInChildren<AudioSource>();
		base.StartCoroutine(this.FadeOutAudio(componentsInChildren));
	}

	// Token: 0x06003FAC RID: 16300 RVA: 0x00118F23 File Offset: 0x00117123
	private IEnumerator FadeOutAudio(AudioSource[] audioSources)
	{
		for (float elapsed = 0f; elapsed <= this.FadeTime; elapsed += Time.deltaTime)
		{
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].volume = Mathf.Lerp(0f, 1f, elapsed / this.FadeTime);
			}
			yield return null;
		}
		for (int i = 0; i < audioSources.Length; i++)
		{
			audioSources[i].volume = 0f;
		}
		yield break;
	}

	// Token: 0x06003FAD RID: 16301 RVA: 0x00118F3C File Offset: 0x0011713C
	public void SetInSubMenu(bool value)
	{
		this.isInSubMenu = value;
		MenuStyles.MenuStyle menuStyle = this.Styles[this.CurrentStyle];
		if (menuStyle.Foreground)
		{
			menuStyle.Foreground.FadeTo((float)(value ? 0 : 1), this.ForegroundFadeTime, null, false, null);
		}
	}

	// Token: 0x0400414D RID: 16717
	public static MenuStyles Instance;

	// Token: 0x0400414E RID: 16718
	public MenuStyles.MenuStyle[] Styles;

	// Token: 0x0400414F RID: 16719
	[Space]
	public MenuStyles.StyleSettings StyleDefault;

	// Token: 0x04004150 RID: 16720
	public MenuStyles.StyleSettingsPlatform[] StylePlatforms;

	// Token: 0x04004151 RID: 16721
	[Space]
	public SpriteRenderer BlackSolid;

	// Token: 0x04004152 RID: 16722
	public float FadeTime = 0.25f;

	// Token: 0x04004153 RID: 16723
	public float ForegroundFadeTime = 0.15f;

	// Token: 0x04004154 RID: 16724
	private Coroutine fadeRoutine;

	// Token: 0x04004155 RID: 16725
	private MenuStyles.StyleSettings currentSettings;

	// Token: 0x04004156 RID: 16726
	private bool isInSubMenu;

	// Token: 0x04004157 RID: 16727
	private bool started;

	// Token: 0x020019DD RID: 6621
	[Serializable]
	public class MenuStyle
	{
		// Token: 0x170010D2 RID: 4306
		// (get) Token: 0x06009556 RID: 38230 RVA: 0x002A51D0 File Offset: 0x002A33D0
		public bool IsAvailable
		{
			get
			{
				if (!this.Enabled)
				{
					return false;
				}
				GameManager instance = GameManager.instance;
				return string.IsNullOrEmpty(this.UnlockKey) || (!DemoHelper.IsDemoMode && Platform.Current.RoamingSharedData.GetBool(this.UnlockKey, false));
			}
		}

		// Token: 0x06009557 RID: 38231 RVA: 0x002A5210 File Offset: 0x002A3410
		public void SetInitialAudioVolumes(AudioSource[] sources)
		{
			if (this.InitialAudioVolumes != null && this.InitialAudioVolumes.Length != 0)
			{
				return;
			}
			this.InitialAudioVolumes = new float[sources.Length];
			for (int i = 0; i < this.InitialAudioVolumes.Length; i++)
			{
				this.InitialAudioVolumes[i] = sources[i].volume;
			}
		}

		// Token: 0x04009772 RID: 38770
		public bool Enabled = true;

		// Token: 0x04009773 RID: 38771
		public string DisplayName;

		// Token: 0x04009774 RID: 38772
		public GameObject StyleObject;

		// Token: 0x04009775 RID: 38773
		public NestedFadeGroupBase Foreground;

		// Token: 0x04009776 RID: 38774
		public MenuStyles.MenuStyle.CameraCurves CameraColorCorrection;

		// Token: 0x04009777 RID: 38775
		public Color AmbientColor;

		// Token: 0x04009778 RID: 38776
		public float AmbientIntensity;

		// Token: 0x04009779 RID: 38777
		public float BlurPlaneVibranceOffset;

		// Token: 0x0400977A RID: 38778
		public string UnlockKey;

		// Token: 0x0400977B RID: 38779
		[NonSerialized]
		public float[] InitialAudioVolumes;

		// Token: 0x02001C26 RID: 7206
		[Serializable]
		public class CameraCurves
		{
			// Token: 0x0400A026 RID: 40998
			[Range(0f, 5f)]
			public float Saturation = 1f;

			// Token: 0x0400A027 RID: 40999
			public AnimationCurve RedChannel = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			});

			// Token: 0x0400A028 RID: 41000
			public AnimationCurve GreenChannel = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			});

			// Token: 0x0400A029 RID: 41001
			public AnimationCurve BlueChannel = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			});
		}
	}

	// Token: 0x020019DE RID: 6622
	[Serializable]
	public struct StyleSettings
	{
		// Token: 0x0400977C RID: 38780
		public int StyleIndex;

		// Token: 0x0400977D RID: 38781
		public string AutoChangeVersion;
	}

	// Token: 0x020019DF RID: 6623
	[Serializable]
	public struct StyleSettingsPlatform
	{
		// Token: 0x0400977E RID: 38782
		public RuntimePlatform Platform;

		// Token: 0x0400977F RID: 38783
		public MenuStyles.StyleSettings Settings;
	}

	// Token: 0x020019E0 RID: 6624
	private enum FadeType
	{
		// Token: 0x04009781 RID: 38785
		Up,
		// Token: 0x04009782 RID: 38786
		Down
	}
}
