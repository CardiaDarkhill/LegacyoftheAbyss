using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000618 RID: 1560
public class CompletionStyles : MonoBehaviour
{
	// Token: 0x0600378D RID: 14221 RVA: 0x000F4EF5 File Offset: 0x000F30F5
	private void OnEnable()
	{
		this.UpdateStyle();
	}

	// Token: 0x0600378E RID: 14222 RVA: 0x000F4EFD File Offset: 0x000F30FD
	private void Start()
	{
		this.UpdateStyle();
	}

	// Token: 0x0600378F RID: 14223 RVA: 0x000F4F08 File Offset: 0x000F3108
	private void UpdateStyle()
	{
		foreach (CompletionStyles.CompletionStyle completionStyle in this.Styles)
		{
			if (completionStyle.StyleObject)
			{
				completionStyle.StyleObject.SetActive(false);
			}
		}
		string @string = Platform.Current.RoamingSharedData.GetString("unlockedMenuStyle", null);
		for (int j = 0; j < this.Styles.Length; j++)
		{
			CompletionStyles.CompletionStyle completionStyle2 = this.Styles[j];
			if (string.IsNullOrEmpty(completionStyle2.ReadMenuStyleKey) || !(@string != completionStyle2.ReadMenuStyleKey))
			{
				this.SetStyle(j);
				return;
			}
		}
	}

	// Token: 0x06003790 RID: 14224 RVA: 0x000F4FA8 File Offset: 0x000F31A8
	public void SetStyle(int index)
	{
		if (index < 0 || index >= this.Styles.Length)
		{
			Debug.LogError("Menu Style \"" + index.ToString() + "\" is out of bounds.");
			return;
		}
		GameManager instance = GameManager.instance;
		CompletionStyles.CompletionStyle completionStyle = this.Styles[index];
		if (completionStyle.MusicSnapshot)
		{
			instance.AudioManager.ApplyMusicSnapshot(completionStyle.MusicSnapshot, 0f, 0f);
		}
		for (int i = 0; i < this.Styles.Length; i++)
		{
			CompletionStyles.CompletionStyle completionStyle2 = this.Styles[i];
			if (completionStyle2.StyleObject)
			{
				bool active = index == i;
				completionStyle2.StyleObject.SetActive(active);
			}
		}
		instance.sm.IsGradeOverridden = true;
		GameCameras instance2 = GameCameras.instance;
		if (instance2 && instance2.colorCorrectionCurves)
		{
			CompletionStyles.CompletionStyle.CameraCurves cameraColorCorrection = completionStyle.CameraColorCorrection;
			instance2.colorCorrectionCurves.saturation = cameraColorCorrection.Saturation;
			instance2.colorCorrectionCurves.redChannel = cameraColorCorrection.RedChannel;
			instance2.colorCorrectionCurves.greenChannel = cameraColorCorrection.GreenChannel;
			instance2.colorCorrectionCurves.blueChannel = cameraColorCorrection.BlueChannel;
		}
		CustomSceneManager.SetLighting(completionStyle.AmbientColor, completionStyle.AmbientIntensity);
		BlurPlane.SetVibranceOffset(completionStyle.BlurPlaneVibranceOffset);
	}

	// Token: 0x04003A84 RID: 14980
	public CompletionStyles.CompletionStyle[] Styles;

	// Token: 0x0200192B RID: 6443
	[Serializable]
	public class CompletionStyle
	{
		// Token: 0x040094A5 RID: 38053
		public string ReadMenuStyleKey;

		// Token: 0x040094A6 RID: 38054
		public GameObject StyleObject;

		// Token: 0x040094A7 RID: 38055
		public CompletionStyles.CompletionStyle.CameraCurves CameraColorCorrection;

		// Token: 0x040094A8 RID: 38056
		public Color AmbientColor;

		// Token: 0x040094A9 RID: 38057
		public float AmbientIntensity;

		// Token: 0x040094AA RID: 38058
		public float BlurPlaneVibranceOffset;

		// Token: 0x040094AB RID: 38059
		public AudioMixerSnapshot MusicSnapshot;

		// Token: 0x02001C21 RID: 7201
		[Serializable]
		public class CameraCurves
		{
			// Token: 0x0400A013 RID: 40979
			[Range(0f, 5f)]
			public float Saturation = 1f;

			// Token: 0x0400A014 RID: 40980
			public AnimationCurve RedChannel = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			});

			// Token: 0x0400A015 RID: 40981
			public AnimationCurve GreenChannel = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			});

			// Token: 0x0400A016 RID: 40982
			public AnimationCurve BlueChannel = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 1f)
			});
		}
	}
}
