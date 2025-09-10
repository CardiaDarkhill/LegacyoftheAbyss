using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x0200021E RID: 542
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Custom/Brightness Effect")]
public class BrightnessEffect : ImageEffectBase, IPostprocessModule
{
	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06001426 RID: 5158 RVA: 0x0005AF0F File Offset: 0x0005910F
	public string EffectKeyword
	{
		get
		{
			return "BRIGHTNESS_EFFECT_ENABLED";
		}
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x0005AF16 File Offset: 0x00059116
	private void Awake()
	{
		this.brightnessMultiply = 1f;
		this.contrastMultiply = 1f;
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x0005AF2E File Offset: 0x0005912E
	public void SetBrightness(float value)
	{
		this._Brightness = value;
		if (GameCameraTextureDisplay.Instance)
		{
			GameCameraTextureDisplay.Instance.UpdateBrightness(this._Brightness);
		}
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x0005AF53 File Offset: 0x00059153
	public void SetContrast(float value)
	{
		this._Contrast = value;
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x0005AF5C File Offset: 0x0005915C
	public void ExtraEffectFadeTo(float brightness, float contrast, float fadeTime, float delay)
	{
		if (this.extraEffectFadeRoutine != null)
		{
			base.StopCoroutine(this.extraEffectFadeRoutine);
			this.extraEffectFadeRoutine = null;
		}
		if (fadeTime > 0f || delay > 0f)
		{
			this.extraEffectFadeRoutine = base.StartCoroutine(this.ExtraEffectFadeToRoutine(brightness, contrast, fadeTime, delay));
			return;
		}
		this.brightnessMultiply = brightness;
		this.contrastMultiply = contrast;
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x0005AFBB File Offset: 0x000591BB
	private IEnumerator ExtraEffectFadeToRoutine(float brightness, float contrast, float fadeTime, float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (fadeTime > 0f)
		{
			float initialBrightness = this.brightnessMultiply;
			float initialContrast = this.contrastMultiply;
			for (float elapsed = 0f; elapsed < fadeTime; elapsed += Time.deltaTime)
			{
				float t = elapsed / fadeTime;
				this.brightnessMultiply = Mathf.Lerp(initialBrightness, brightness, t);
				this.contrastMultiply = Mathf.Lerp(initialContrast, contrast, t);
				yield return null;
			}
		}
		this.brightnessMultiply = brightness;
		this.contrastMultiply = contrast;
		this.extraEffectFadeRoutine = null;
		yield break;
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x0005AFE7 File Offset: 0x000591E7
	public void UpdateProperties(Material material)
	{
		material.SetFloat(BrightnessEffect._brightnessProp, this._Brightness * this.brightnessMultiply);
		material.SetFloat(BrightnessEffect._contrastProp, this._Contrast * this.contrastMultiply);
	}

	// Token: 0x04001269 RID: 4713
	[Range(0f, 2f)]
	public float _Brightness = 1f;

	// Token: 0x0400126A RID: 4714
	[Range(0f, 2f)]
	public float _Contrast = 1f;

	// Token: 0x0400126B RID: 4715
	private float brightnessMultiply;

	// Token: 0x0400126C RID: 4716
	private float contrastMultiply;

	// Token: 0x0400126D RID: 4717
	private Coroutine extraEffectFadeRoutine;

	// Token: 0x0400126E RID: 4718
	private static readonly int _brightnessProp = Shader.PropertyToID("_Brightness");

	// Token: 0x0400126F RID: 4719
	private static readonly int _contrastProp = Shader.PropertyToID("_Contrast");
}
