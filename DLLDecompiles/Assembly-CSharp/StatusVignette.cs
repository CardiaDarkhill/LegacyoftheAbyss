using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GlobalEnums;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000732 RID: 1842
public class StatusVignette : MonoBehaviour
{
	// Token: 0x060041CB RID: 16843 RVA: 0x00121848 File Offset: 0x0011FA48
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<Animator>(ref this.vignettes, typeof(StatusVignette.Vignettes));
		ArrayForEnumAttribute.EnsureArraySize<StatusVignette.FadeParams>(ref this.statuses, typeof(StatusVignette.StatusTypes));
		ArrayForEnumAttribute.EnsureArraySize<StatusVignette.TempFadeParams>(ref this.tempStatuses, typeof(StatusVignette.TempStatusTypes));
	}

	// Token: 0x060041CC RID: 16844 RVA: 0x00121894 File Offset: 0x0011FA94
	private void Awake()
	{
		this.OnValidate();
		if (StatusVignette._instance)
		{
			return;
		}
		StatusVignette._instance = this;
		this.fadeRoutines = new Coroutine[this.vignettes.Length];
		this.previousFadeParams = new StatusVignette.FadeParams[this.vignettes.Length];
		this.currentStatuses = new List<StatusVignette.StatusTypes>[this.vignettes.Length];
		for (int i = 0; i < this.vignettes.Length; i++)
		{
			this.currentStatuses[i] = new List<StatusVignette.StatusTypes>();
			this.vignettes[i].gameObject.SetActive(false);
		}
		this.currentTempStatuses = new bool[this.tempStatuses.Length];
		this.tempFadeRoutines = new Coroutine[this.tempStatuses.Length];
		this.mixTValues = new float[this.vignettes.Length, 2];
		this.initialYScale = base.transform.localScale.y;
	}

	// Token: 0x060041CD RID: 16845 RVA: 0x00121978 File Offset: 0x0011FB78
	private void OnEnable()
	{
		ForceCameraAspect.MainCamHeightMultChanged += this.OnMainCamHeightMultChanged;
		this.OnMainCamHeightMultChanged(ForceCameraAspect.CurrentMainCamHeightMult);
		ForceCameraAspect.ViewportAspectChanged += this.OnCameraAspectChanged;
		this.OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
		if (!this.gm)
		{
			this.gm = GameManager.instance;
			this.gm.GameStateChange += this.OnGameStateChanged;
		}
		Animator[] array = this.vignettes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(StatusVignette._playbackTimeProp, 0f);
		}
		for (int j = 0; j < this.vignettes.Length; j++)
		{
			this.RefreshStatus((StatusVignette.Vignettes)j);
		}
	}

	// Token: 0x060041CE RID: 16846 RVA: 0x00121A30 File Offset: 0x0011FC30
	private void OnDisable()
	{
		ForceCameraAspect.MainCamHeightMultChanged -= this.OnMainCamHeightMultChanged;
		ForceCameraAspect.ViewportAspectChanged -= this.OnCameraAspectChanged;
		if (this.gm)
		{
			this.gm.GameStateChange -= this.OnGameStateChanged;
			this.gm = null;
		}
		for (int i = 0; i < this.fadeRoutines.Length; i++)
		{
			Coroutine coroutine = this.fadeRoutines[i];
			if (coroutine != null)
			{
				base.StopCoroutine(coroutine);
				this.fadeRoutines[i] = null;
			}
		}
	}

	// Token: 0x060041CF RID: 16847 RVA: 0x00121AB9 File Offset: 0x0011FCB9
	private void OnDestroy()
	{
		if (StatusVignette._instance == this)
		{
			StatusVignette._instance = null;
		}
	}

	// Token: 0x060041D0 RID: 16848 RVA: 0x00121AD0 File Offset: 0x0011FCD0
	private void OnMainCamHeightMultChanged(float heightMult)
	{
		Transform transform = base.transform;
		Vector3 localScale = transform.localScale;
		localScale.y = this.initialYScale * heightMult;
		transform.localScale = localScale;
	}

	// Token: 0x060041D1 RID: 16849 RVA: 0x00121B00 File Offset: 0x0011FD00
	private void OnCameraAspectChanged(float aspect)
	{
		Transform transform = base.transform;
		Vector3 localScale = transform.localScale;
		localScale.x = localScale.y * aspect;
		transform.localScale = localScale;
	}

	// Token: 0x060041D2 RID: 16850 RVA: 0x00121B30 File Offset: 0x0011FD30
	public static void AddStatus(StatusVignette.StatusTypes status)
	{
		if (!StatusVignette._instance)
		{
			return;
		}
		StatusVignette.Vignettes vignetteForStatus = StatusVignette.GetVignetteForStatus(status);
		if (StatusVignette._instance.currentStatuses[(int)vignetteForStatus].AddIfNotPresent(status))
		{
			StatusVignette._instance.RefreshStatus(vignetteForStatus);
		}
	}

	// Token: 0x060041D3 RID: 16851 RVA: 0x00121B70 File Offset: 0x0011FD70
	public static void RemoveStatus(StatusVignette.StatusTypes status)
	{
		if (!StatusVignette._instance)
		{
			return;
		}
		StatusVignette.Vignettes vignetteForStatus = StatusVignette.GetVignetteForStatus(status);
		if (StatusVignette._instance.currentStatuses[(int)vignetteForStatus].Remove(status))
		{
			StatusVignette._instance.RefreshStatus(vignetteForStatus);
		}
	}

	// Token: 0x060041D4 RID: 16852 RVA: 0x00121BB0 File Offset: 0x0011FDB0
	public static void AddTempStatus(StatusVignette.TempStatusTypes status)
	{
		if (!StatusVignette._instance)
		{
			return;
		}
		if (StatusVignette._instance.currentTempStatuses[(int)status])
		{
			StatusVignette._instance.StopCoroutine(StatusVignette._instance.tempFadeRoutines[(int)status]);
		}
		else
		{
			StatusVignette._instance.currentTempStatuses[(int)status] = true;
		}
		StatusVignette._instance.tempFadeRoutines[(int)status] = StatusVignette._instance.StartCoroutine(StatusVignette._instance.TempFadeRoutine(status));
	}

	// Token: 0x060041D5 RID: 16853 RVA: 0x00121C20 File Offset: 0x0011FE20
	private static StatusVignette.Vignettes GetVignetteForStatus(StatusVignette.StatusTypes statusType)
	{
		StatusVignette.Vignettes result;
		switch (statusType)
		{
		case StatusVignette.StatusTypes.InMaggotRegion:
		case StatusVignette.StatusTypes.Maggoted:
			result = StatusVignette.Vignettes.Maggot;
			break;
		case StatusVignette.StatusTypes.InFrostWater:
			result = StatusVignette.Vignettes.FrostWater;
			break;
		case StatusVignette.StatusTypes.InRageMode:
			result = StatusVignette.Vignettes.Fury;
			break;
		case StatusVignette.StatusTypes.Voided:
			result = StatusVignette.Vignettes.Void;
			break;
		case StatusVignette.StatusTypes.InCoalRegion:
			result = StatusVignette.Vignettes.Flame;
			break;
		default:
			throw new ArgumentOutOfRangeException("statusType", statusType, null);
		}
		return result;
	}

	// Token: 0x060041D6 RID: 16854 RVA: 0x00121C74 File Offset: 0x0011FE74
	private static StatusVignette.Vignettes GetVignetteForTempStatus(StatusVignette.TempStatusTypes statusType)
	{
		StatusVignette.Vignettes result;
		if (statusType != StatusVignette.TempStatusTypes.FlameDamage)
		{
			if (statusType != StatusVignette.TempStatusTypes.FlameDamageLavaBell)
			{
				throw new ArgumentOutOfRangeException("statusType", statusType, null);
			}
			result = StatusVignette.Vignettes.LavaBell;
		}
		else
		{
			result = StatusVignette.Vignettes.Flame;
		}
		return result;
	}

	// Token: 0x060041D7 RID: 16855 RVA: 0x00121CA8 File Offset: 0x0011FEA8
	private void OnGameStateChanged(GameState gameState)
	{
		bool flag = this.isSuppressed;
		bool flag2 = gameState == GameState.CUTSCENE;
		this.isSuppressed = flag2;
		if (this.isSuppressed == flag)
		{
			return;
		}
		for (int i = 0; i < this.vignettes.Length; i++)
		{
			this.RefreshStatus((StatusVignette.Vignettes)i);
		}
	}

	// Token: 0x060041D8 RID: 16856 RVA: 0x00121CF4 File Offset: 0x0011FEF4
	private void RefreshStatus(StatusVignette.Vignettes vignetteType)
	{
		Coroutine coroutine = this.fadeRoutines[(int)vignetteType];
		if (coroutine != null)
		{
			base.StopCoroutine(coroutine);
			this.fadeRoutines[(int)vignetteType] = null;
		}
		StatusVignette.FadeParams fadeParams = this.previousFadeParams[(int)vignetteType];
		List<StatusVignette.StatusTypes> list = this.currentStatuses[(int)vignetteType];
		if (!this.isSuppressed && list.Count > 0)
		{
			List<StatusVignette.StatusTypes> list2 = list;
			StatusVignette.StatusTypes statusTypes = list2[list2.Count - 1];
			fadeParams = (this.previousFadeParams[(int)vignetteType] = this.statuses[(int)statusTypes]);
			this.fadeRoutines[(int)vignetteType] = base.StartCoroutine(this.FadeRoutine(true, fadeParams, vignetteType));
			return;
		}
		if (fadeParams != null)
		{
			this.fadeRoutines[(int)vignetteType] = base.StartCoroutine(this.FadeRoutine(false, fadeParams, vignetteType));
		}
	}

	// Token: 0x060041D9 RID: 16857 RVA: 0x00121D9E File Offset: 0x0011FF9E
	private IEnumerator FadeRoutine(bool isFadingUp, StatusVignette.FadeParams fadeParams, StatusVignette.Vignettes vignetteType)
	{
		StatusVignette.<>c__DisplayClass36_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.vignetteType = vignetteType;
		CS$<>8__locals1.fadeParams = fadeParams;
		int vignetteIndex = (int)CS$<>8__locals1.vignetteType;
		Animator vignette = this.vignettes[vignetteIndex];
		float duration;
		AnimationCurve curve;
		float targetAlpha;
		if (isFadingUp)
		{
			duration = CS$<>8__locals1.fadeParams.AppearDuration;
			curve = CS$<>8__locals1.fadeParams.AppearCurve;
			targetAlpha = CS$<>8__locals1.fadeParams.AppearTargetAlpha;
		}
		else
		{
			duration = CS$<>8__locals1.fadeParams.DisappearDuration;
			curve = CS$<>8__locals1.fadeParams.DisappearCurve;
			targetAlpha = 0f;
		}
		float startAlpha = vignette.gameObject.activeInHierarchy ? vignette.GetFloat(StatusVignette._playbackTimeProp) : 0f;
		vignette.gameObject.SetActive(true);
		if (CS$<>8__locals1.fadeParams.AudioSource)
		{
			CS$<>8__locals1.fadeParams.AudioSource.Play();
			if (CS$<>8__locals1.fadeParams.EnterClip)
			{
				CS$<>8__locals1.fadeParams.AudioSource.PlayOneShot(CS$<>8__locals1.fadeParams.EnterClip);
			}
		}
		for (float elapsed = 0f; elapsed <= duration; elapsed += Time.deltaTime)
		{
			float time = elapsed / duration;
			float t = curve.Evaluate(time);
			this.<FadeRoutine>g__SetT|36_0(Mathf.LerpUnclamped(startAlpha, targetAlpha, t), ref CS$<>8__locals1);
			yield return null;
		}
		this.<FadeRoutine>g__SetT|36_0(Mathf.LerpUnclamped(startAlpha, targetAlpha, curve.Evaluate(1f)), ref CS$<>8__locals1);
		if (!isFadingUp)
		{
			vignette.gameObject.SetActive(false);
			if (CS$<>8__locals1.fadeParams.AudioSource)
			{
				CS$<>8__locals1.fadeParams.AudioSource.Stop();
				if (CS$<>8__locals1.fadeParams.ExitClip)
				{
					CS$<>8__locals1.fadeParams.AudioSource.PlayOneShot(CS$<>8__locals1.fadeParams.ExitClip);
				}
			}
			this.previousFadeParams[vignetteIndex] = null;
			this.fadeRoutines[vignetteIndex] = null;
			yield break;
		}
		float elapsedLoop = 0f;
		for (;;)
		{
			float time2 = elapsedLoop % CS$<>8__locals1.fadeParams.LoopDuration / CS$<>8__locals1.fadeParams.LoopDuration;
			float t2 = CS$<>8__locals1.fadeParams.LoopCurve.Evaluate(time2);
			float num = CS$<>8__locals1.fadeParams.LoopFadeCurve.Evaluate(elapsedLoop / CS$<>8__locals1.fadeParams.LoopDuration);
			this.<FadeRoutine>g__SetT|36_0(CS$<>8__locals1.fadeParams.LoopAlphaRange.GetLerpUnclampedValue(t2) * num, ref CS$<>8__locals1);
			yield return null;
			elapsedLoop += Time.deltaTime;
		}
	}

	// Token: 0x060041DA RID: 16858 RVA: 0x00121DC2 File Offset: 0x0011FFC2
	private IEnumerator TempFadeRoutine(StatusVignette.TempStatusTypes statusType)
	{
		StatusVignette.Vignettes vignetteType = StatusVignette.GetVignetteForTempStatus(statusType);
		StatusVignette.TempFadeParams fadeParams = this.tempStatuses[(int)statusType];
		for (float elapsed = 0f; elapsed <= fadeParams.Duration; elapsed += Time.unscaledDeltaTime)
		{
			float time = elapsed / fadeParams.Duration;
			float num = fadeParams.Curve.Evaluate(time);
			this.SetVignetteTValue(vignetteType, 1, num * fadeParams.TargetAlpha);
			yield return null;
		}
		this.SetVignetteTValue(vignetteType, 1, 0f);
		this.currentTempStatuses[(int)statusType] = false;
		this.tempFadeRoutines[(int)statusType] = null;
		yield break;
	}

	// Token: 0x060041DB RID: 16859 RVA: 0x00121DD8 File Offset: 0x0011FFD8
	private void SetVignetteTValue(StatusVignette.Vignettes vignetteType, int mixIndex, float value)
	{
		this.mixTValues[(int)vignetteType, mixIndex] = value;
		float num = 0f;
		for (int i = 0; i < 2; i++)
		{
			num = Mathf.Max(num, this.mixTValues[(int)vignetteType, i]);
		}
		Animator animator = this.vignettes[(int)vignetteType];
		animator.gameObject.SetActive(num > Mathf.Epsilon);
		animator.SetFloat(StatusVignette._playbackTimeProp, num);
	}

	// Token: 0x060041DC RID: 16860 RVA: 0x00121E40 File Offset: 0x00120040
	public static void SetFrostVignetteAmount(float percentage)
	{
		if (!StatusVignette._instance)
		{
			return;
		}
		if (Math.Abs(StatusVignette._instance.frostVignetteTargetValue - percentage) <= Mathf.Epsilon)
		{
			return;
		}
		StatusVignette._instance.frostVignetteTargetValue = percentage;
		ref Coroutine ptr = ref StatusVignette._instance.fadeRoutines[2];
		if (ptr == null)
		{
			ptr = StatusVignette._instance.StartCoroutine(StatusVignette._instance.FrostVignetteLerp());
		}
	}

	// Token: 0x060041DD RID: 16861 RVA: 0x00121EA9 File Offset: 0x001200A9
	private IEnumerator FrostVignetteLerp()
	{
		Animator vignette = StatusVignette._instance.vignettes[2];
		if (!vignette)
		{
			yield break;
		}
		float previousTargetValue = this.frostVignetteTargetValue;
		bool wasFadingOut = false;
		this.frostVignetteFadeGroup.AlphaSelf = 1f;
		vignette.SetFloat(StatusVignette._playbackTimeProp, 0f);
		for (;;)
		{
			float num = this.frostVignetteTargetValue - previousTargetValue;
			bool flag = num < 0f || (wasFadingOut && num == 0f);
			float num2 = Mathf.Max(Mathf.Epsilon, 1E-05f);
			if (flag && this.frostVignetteFadeGroup.AlphaSelf <= num2)
			{
				break;
			}
			if (!vignette.gameObject.activeSelf)
			{
				vignette.gameObject.SetActive(true);
			}
			if (flag)
			{
				this.frostVignetteFadeGroup.AlphaSelf = Mathf.Lerp(this.frostVignetteFadeGroup.AlphaSelf, this.frostVignetteTargetValue, this.frostVignetteLerpSpeed * Time.unscaledDeltaTime);
			}
			else
			{
				this.frostVignetteFadeGroup.AlphaSelf = Mathf.Lerp(this.frostVignetteFadeGroup.AlphaSelf, 1f, this.frostVignetteLerpSpeed * Time.deltaTime);
				float value = Mathf.Lerp(vignette.GetFloat(StatusVignette._playbackTimeProp), this.frostVignetteTargetValue, this.frostVignetteLerpSpeed * Time.deltaTime);
				vignette.SetFloat(StatusVignette._playbackTimeProp, value);
			}
			previousTargetValue = this.frostVignetteTargetValue;
			wasFadingOut = flag;
			yield return null;
		}
		if (vignette.gameObject.activeSelf)
		{
			vignette.gameObject.SetActive(false);
		}
		StatusVignette._instance.fadeRoutines[2] = null;
		yield break;
	}

	// Token: 0x060041E0 RID: 16864 RVA: 0x00121ED1 File Offset: 0x001200D1
	[CompilerGenerated]
	private void <FadeRoutine>g__SetT|36_0(float t, ref StatusVignette.<>c__DisplayClass36_0 A_2)
	{
		this.SetVignetteTValue(A_2.vignetteType, 0, t);
		if (A_2.fadeParams.AudioSource)
		{
			A_2.fadeParams.AudioSource.volume = t;
		}
	}

	// Token: 0x0400435D RID: 17245
	[SerializeField]
	[ArrayForEnum(typeof(StatusVignette.Vignettes))]
	private Animator[] vignettes;

	// Token: 0x0400435E RID: 17246
	[SerializeField]
	[ArrayForEnum(typeof(StatusVignette.StatusTypes))]
	private StatusVignette.FadeParams[] statuses;

	// Token: 0x0400435F RID: 17247
	[SerializeField]
	[ArrayForEnum(typeof(StatusVignette.TempStatusTypes))]
	private StatusVignette.TempFadeParams[] tempStatuses;

	// Token: 0x04004360 RID: 17248
	[SerializeField]
	private float frostVignetteLerpSpeed;

	// Token: 0x04004361 RID: 17249
	[SerializeField]
	private NestedFadeGroupBase frostVignetteFadeGroup;

	// Token: 0x04004362 RID: 17250
	private Coroutine[] fadeRoutines;

	// Token: 0x04004363 RID: 17251
	private StatusVignette.FadeParams[] previousFadeParams;

	// Token: 0x04004364 RID: 17252
	private List<StatusVignette.StatusTypes>[] currentStatuses;

	// Token: 0x04004365 RID: 17253
	private Coroutine[] tempFadeRoutines;

	// Token: 0x04004366 RID: 17254
	private bool[] currentTempStatuses;

	// Token: 0x04004367 RID: 17255
	private float[,] mixTValues;

	// Token: 0x04004368 RID: 17256
	private float initialYScale;

	// Token: 0x04004369 RID: 17257
	private float frostVignetteTargetValue;

	// Token: 0x0400436A RID: 17258
	private GameManager gm;

	// Token: 0x0400436B RID: 17259
	private bool isSuppressed;

	// Token: 0x0400436C RID: 17260
	private static readonly int _playbackTimeProp = Animator.StringToHash("Playback Time");

	// Token: 0x0400436D RID: 17261
	private static StatusVignette _instance;

	// Token: 0x02001A17 RID: 6679
	private enum Vignettes
	{
		// Token: 0x0400986C RID: 39020
		Maggot,
		// Token: 0x0400986D RID: 39021
		Flame,
		// Token: 0x0400986E RID: 39022
		Frost,
		// Token: 0x0400986F RID: 39023
		FrostWater,
		// Token: 0x04009870 RID: 39024
		Fury,
		// Token: 0x04009871 RID: 39025
		Void,
		// Token: 0x04009872 RID: 39026
		LavaBell
	}

	// Token: 0x02001A18 RID: 6680
	public enum StatusTypes
	{
		// Token: 0x04009874 RID: 39028
		InMaggotRegion,
		// Token: 0x04009875 RID: 39029
		Maggoted,
		// Token: 0x04009876 RID: 39030
		InFrostWater,
		// Token: 0x04009877 RID: 39031
		InRageMode,
		// Token: 0x04009878 RID: 39032
		Voided,
		// Token: 0x04009879 RID: 39033
		InCoalRegion
	}

	// Token: 0x02001A19 RID: 6681
	public enum TempStatusTypes
	{
		// Token: 0x0400987B RID: 39035
		FlameDamage,
		// Token: 0x0400987C RID: 39036
		FlameDamageLavaBell
	}

	// Token: 0x02001A1A RID: 6682
	[Serializable]
	private class FadeParams
	{
		// Token: 0x0400987D RID: 39037
		public float AppearTargetAlpha = 1f;

		// Token: 0x0400987E RID: 39038
		public AnimationCurve AppearCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x0400987F RID: 39039
		public float AppearDuration;

		// Token: 0x04009880 RID: 39040
		public MinMaxFloat LoopAlphaRange = new MinMaxFloat(0f, 1f);

		// Token: 0x04009881 RID: 39041
		public AnimationCurve LoopCurve = AnimationCurve.Constant(0f, 1f, 1f);

		// Token: 0x04009882 RID: 39042
		public AnimationCurve LoopFadeCurve = AnimationCurve.Constant(0f, 1f, 1f);

		// Token: 0x04009883 RID: 39043
		public float LoopDuration;

		// Token: 0x04009884 RID: 39044
		public AnimationCurve DisappearCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x04009885 RID: 39045
		public float DisappearDuration;

		// Token: 0x04009886 RID: 39046
		public AudioSource AudioSource;

		// Token: 0x04009887 RID: 39047
		public AudioClip EnterClip;

		// Token: 0x04009888 RID: 39048
		public AudioClip ExitClip;
	}

	// Token: 0x02001A1B RID: 6683
	[Serializable]
	private class TempFadeParams
	{
		// Token: 0x04009889 RID: 39049
		public float TargetAlpha = 1f;

		// Token: 0x0400988A RID: 39050
		public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		// Token: 0x0400988B RID: 39051
		public float Duration;
	}
}
