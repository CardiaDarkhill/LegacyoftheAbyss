using System;
using System.Collections;
using GlobalSettings;
using UnityEngine;

// Token: 0x020003BA RID: 954
public class HeroPerformanceRegion : MonoBehaviour
{
	// Token: 0x14000062 RID: 98
	// (add) Token: 0x0600200F RID: 8207 RVA: 0x00091D44 File Offset: 0x0008FF44
	// (remove) Token: 0x06002010 RID: 8208 RVA: 0x00091D78 File Offset: 0x0008FF78
	public static event Action StartedPerforming;

	// Token: 0x14000063 RID: 99
	// (add) Token: 0x06002011 RID: 8209 RVA: 0x00091DAC File Offset: 0x0008FFAC
	// (remove) Token: 0x06002012 RID: 8210 RVA: 0x00091DE0 File Offset: 0x0008FFE0
	public static event Action StoppedPerforming;

	// Token: 0x17000356 RID: 854
	// (get) Token: 0x06002013 RID: 8211 RVA: 0x00091E13 File Offset: 0x00090013
	// (set) Token: 0x06002014 RID: 8212 RVA: 0x00091E2D File Offset: 0x0009002D
	public static bool IsPerforming
	{
		get
		{
			return HeroPerformanceRegion._instance && HeroPerformanceRegion._instance.isPerforming;
		}
		set
		{
			if (HeroPerformanceRegion._instance)
			{
				HeroPerformanceRegion._instance.SetIsPerforming(value);
			}
		}
	}

	// Token: 0x06002015 RID: 8213 RVA: 0x00091E48 File Offset: 0x00090048
	private void OnDrawGizmosSelected()
	{
		Vector2 v = base.transform.position + this.centreOffset;
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(v, this.innerSize);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(v, this.outerSize);
	}

	// Token: 0x06002016 RID: 8214 RVA: 0x00091EB0 File Offset: 0x000900B0
	private void Awake()
	{
		this.amplifyEffect.gameObject.SetActive(false);
		this.amplifyEffectB = Object.Instantiate<Animator>(this.amplifyEffect, this.amplifyEffect.transform.parent);
		this.amplifyAudioSource.loop = true;
		this.amplifyAudioSource.playOnAwake = false;
		this.amplifyAudioSource.Stop();
	}

	// Token: 0x06002017 RID: 8215 RVA: 0x00091F12 File Offset: 0x00090112
	private void Start()
	{
		if (HeroPerformanceRegion._instance == null)
		{
			HeroPerformanceRegion._instance = this;
		}
	}

	// Token: 0x06002018 RID: 8216 RVA: 0x00091F27 File Offset: 0x00090127
	private void OnDestroy()
	{
		if (HeroPerformanceRegion._instance != this)
		{
			return;
		}
		HeroPerformanceRegion._instance = null;
		HeroPerformanceRegion.StartedPerforming = null;
		HeroPerformanceRegion.StoppedPerforming = null;
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x00091F4C File Offset: 0x0009014C
	private void SetIsPerforming(bool value)
	{
		if (HeroPerformanceRegion._instance.isPerforming == value)
		{
			return;
		}
		HeroPerformanceRegion._instance.isPerforming = value;
		if (this.amplifyAudioFadeRoutine != null)
		{
			base.StopCoroutine(this.amplifyAudioFadeRoutine);
			this.amplifyAudioFadeRoutine = null;
		}
		if (HeroPerformanceRegion._instance.isPerforming)
		{
			Action startedPerforming = HeroPerformanceRegion.StartedPerforming;
			if (startedPerforming != null)
			{
				startedPerforming();
			}
			if (!OverrideNeedolinLoop.IsOverridden && Gameplay.MusicianCharmTool.IsEquipped)
			{
				GameObject gameObject;
				Coroutine coroutine;
				if (this.amplifyEffectIsB)
				{
					this.amplifyEffectIsB = false;
					gameObject = this.amplifyEffect.gameObject;
					coroutine = this.endAmplifyEffectRoutineA;
					this.endAmplifyEffectRoutineA = null;
				}
				else
				{
					this.amplifyEffectIsB = true;
					gameObject = this.amplifyEffectB.gameObject;
					coroutine = this.endAmplifyEffectRoutineB;
					this.endAmplifyEffectRoutineB = null;
				}
				if (coroutine != null)
				{
					base.StopCoroutine(coroutine);
				}
				gameObject.SetActive(false);
				gameObject.SetActive(true);
				GameCameras.instance.forceCameraAspect.SetFovOffset(Gameplay.MusicianCharmFovOffset, Gameplay.MusicianCharmFovStartDuration, Gameplay.MusicianCharmFovStartCurve);
				this.amplifyAudioSource.volume = 0f;
				this.amplifyAudioSource.Play();
				if (this.needolinAudioSource.isPlaying)
				{
					this.amplifyAudioSource.timeSamples = this.needolinAudioSource.timeSamples;
				}
				else
				{
					Debug.LogError("Needolin audio is not already playing, so amplify can't sync!", this);
				}
				this.amplifyAudioFadeRoutine = this.StartTimerRoutine(0f, 2f, delegate(float t)
				{
					this.amplifyAudioSource.volume = t * 0.4f;
				}, null, null, false);
				return;
			}
		}
		else
		{
			Action stoppedPerforming = HeroPerformanceRegion.StoppedPerforming;
			if (stoppedPerforming != null)
			{
				stoppedPerforming();
			}
			if (this.amplifyEffectIsB)
			{
				if (this.endAmplifyEffectRoutineB == null && this.amplifyEffectB.gameObject.activeSelf)
				{
					this.endAmplifyEffectRoutineB = base.StartCoroutine(this.EndAmplifyEffect());
				}
			}
			else if (this.endAmplifyEffectRoutineA == null && this.amplifyEffect.gameObject.activeSelf)
			{
				this.endAmplifyEffectRoutineA = base.StartCoroutine(this.EndAmplifyEffect());
			}
			GameCameras.instance.forceCameraAspect.SetFovOffset(0f, Gameplay.MusicianCharmFovEndDuration, Gameplay.MusicianCharmFovEndCurve);
			this.amplifyAudioFadeRoutine = this.StartTimerRoutine(0f, 1.2f, delegate(float t)
			{
				this.amplifyAudioSource.volume = Mathf.Clamp01(1f - t) * 0.4f;
			}, null, delegate
			{
				this.amplifyAudioSource.Stop();
			}, false);
		}
	}

	// Token: 0x0600201A RID: 8218 RVA: 0x00092178 File Offset: 0x00090378
	private IEnumerator EndAmplifyEffect()
	{
		bool wasAmplifyEffectB = this.amplifyEffectIsB;
		Animator effect = this.amplifyEffectIsB ? this.amplifyEffectB : this.amplifyEffect;
		effect.Play(HeroPerformanceRegion._disappearAnimId);
		yield return null;
		yield return new WaitForSeconds(effect.GetCurrentAnimatorStateInfo(0).length);
		effect.gameObject.SetActive(false);
		if (wasAmplifyEffectB)
		{
			this.endAmplifyEffectRoutineB = null;
		}
		else
		{
			this.endAmplifyEffectRoutineA = null;
		}
		yield break;
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x00092187 File Offset: 0x00090387
	public static HeroPerformanceRegion.AffectedState GetAffectedState(Transform transform, bool ignoreRange)
	{
		if (!HeroPerformanceRegion.IsPerforming)
		{
			return HeroPerformanceRegion.AffectedState.None;
		}
		return HeroPerformanceRegion._instance.InternalGetAffectedRange(transform, ignoreRange);
	}

	// Token: 0x0600201C RID: 8220 RVA: 0x0009219E File Offset: 0x0009039E
	public static HeroPerformanceRegion.AffectedState GetAffectedStateWithRadius(Transform transform, float radius)
	{
		if (!HeroPerformanceRegion.IsPerforming)
		{
			return HeroPerformanceRegion.AffectedState.None;
		}
		return HeroPerformanceRegion._instance.InternalGetAffectedRangeWithRadius(transform, radius);
	}

	// Token: 0x0600201D RID: 8221 RVA: 0x000921B5 File Offset: 0x000903B5
	public static bool IsPlayingInRange(Vector2 position, float radius)
	{
		return HeroPerformanceRegion.IsPerforming && HeroPerformanceRegion._instance.InternalIsInRange(position, radius);
	}

	// Token: 0x0600201E RID: 8222 RVA: 0x000921CC File Offset: 0x000903CC
	private HeroPerformanceRegion.AffectedState InternalGetAffectedRange(Transform otherTransform, bool ignoreRange)
	{
		if (ignoreRange)
		{
			return HeroPerformanceRegion.AffectedState.ActiveInner;
		}
		Vector2 centre = base.transform.position + this.centreOffset;
		Vector3 position = otherTransform.position;
		if (HeroPerformanceRegion.IsInRange(position, centre, this.innerSize))
		{
			return HeroPerformanceRegion.AffectedState.ActiveInner;
		}
		if (HeroPerformanceRegion.IsInRange(position, centre, this.outerSize))
		{
			return HeroPerformanceRegion.AffectedState.ActiveOuter;
		}
		return HeroPerformanceRegion.AffectedState.None;
	}

	// Token: 0x0600201F RID: 8223 RVA: 0x00092230 File Offset: 0x00090430
	private HeroPerformanceRegion.AffectedState InternalGetAffectedRangeWithRadius(Transform otherTransform, float radius)
	{
		Vector2 vector = base.transform.position + this.centreOffset;
		Vector2 posInRadius = HeroPerformanceRegion.GetPosInRadius(vector, otherTransform.position, radius);
		if (HeroPerformanceRegion.IsInRange(posInRadius, vector, this.innerSize))
		{
			return HeroPerformanceRegion.AffectedState.ActiveInner;
		}
		if (HeroPerformanceRegion.IsInRange(posInRadius, vector, this.outerSize))
		{
			return HeroPerformanceRegion.AffectedState.ActiveOuter;
		}
		return HeroPerformanceRegion.AffectedState.None;
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x0009228F File Offset: 0x0009048F
	private bool InternalIsInRange(Vector2 position, float radius)
	{
		return Vector2.Distance(base.transform.position + this.centreOffset, position) <= radius;
	}

	// Token: 0x06002021 RID: 8225 RVA: 0x000922B8 File Offset: 0x000904B8
	private static bool IsInRange(Vector2 pos, Vector2 centre, Vector2 size)
	{
		float num = Gameplay.MusicianCharmTool.IsEquipped ? Gameplay.MusicianCharmNeedolinRangeMult : 1f;
		Vector2 b = size * (num * 0.5f);
		Vector2 vector = centre - b;
		Vector2 vector2 = centre + b;
		return pos.x >= vector.x && pos.x <= vector2.x && pos.y >= vector.y && pos.y <= vector2.y;
	}

	// Token: 0x06002022 RID: 8226 RVA: 0x0009233C File Offset: 0x0009053C
	public static Vector2 GetPosInRadius(Vector2 noiseSourcePos, Vector2 otherPos, float radius)
	{
		Vector2 vector = noiseSourcePos - otherPos;
		Vector2 b = Mathf.Clamp(vector.magnitude, 0f, radius) * vector.normalized;
		return otherPos + b;
	}

	// Token: 0x04001F13 RID: 7955
	private const float AMPLIFY_FADE_UP_TIME = 2f;

	// Token: 0x04001F14 RID: 7956
	private const float AMPLIFY_FADE_DOWN_TIME = 1.2f;

	// Token: 0x04001F15 RID: 7957
	private const float AMPLIFY_VOLUME = 0.4f;

	// Token: 0x04001F16 RID: 7958
	[SerializeField]
	private Vector2 centreOffset;

	// Token: 0x04001F17 RID: 7959
	[SerializeField]
	private Vector2 innerSize;

	// Token: 0x04001F18 RID: 7960
	[SerializeField]
	private Vector2 outerSize;

	// Token: 0x04001F19 RID: 7961
	[SerializeField]
	private Animator amplifyEffect;

	// Token: 0x04001F1A RID: 7962
	[SerializeField]
	private AudioSource amplifyAudioSource;

	// Token: 0x04001F1B RID: 7963
	[SerializeField]
	private AudioSource needolinAudioSource;

	// Token: 0x04001F1C RID: 7964
	private bool amplifyEffectIsB;

	// Token: 0x04001F1D RID: 7965
	private Animator amplifyEffectB;

	// Token: 0x04001F1E RID: 7966
	private Coroutine amplifyAudioFadeRoutine;

	// Token: 0x04001F1F RID: 7967
	private Coroutine endAmplifyEffectRoutineA;

	// Token: 0x04001F20 RID: 7968
	private Coroutine endAmplifyEffectRoutineB;

	// Token: 0x04001F21 RID: 7969
	private static readonly int _disappearAnimId = Animator.StringToHash("Disappear");

	// Token: 0x04001F22 RID: 7970
	private static HeroPerformanceRegion _instance;

	// Token: 0x04001F23 RID: 7971
	private bool isPerforming;

	// Token: 0x0200166D RID: 5741
	public enum AffectedState
	{
		// Token: 0x04008AC5 RID: 35525
		None,
		// Token: 0x04008AC6 RID: 35526
		ActiveInner,
		// Token: 0x04008AC7 RID: 35527
		ActiveOuter
	}
}
