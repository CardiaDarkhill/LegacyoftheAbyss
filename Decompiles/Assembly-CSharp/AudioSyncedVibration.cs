using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200079E RID: 1950
public sealed class AudioSyncedVibration : MonoBehaviour
{
	// Token: 0x060044C6 RID: 17606 RVA: 0x0012CF29 File Offset: 0x0012B129
	private void Start()
	{
		this.hasSyncSource = this.syncSource;
	}

	// Token: 0x060044C7 RID: 17607 RVA: 0x0012CF3C File Offset: 0x0012B13C
	private void OnDisable()
	{
		if (this.stopOnDisable)
		{
			this.StopVibration();
			return;
		}
		if (this.loop && this.emission != null)
		{
			this.emission.IsLooping = false;
			this.emission = null;
		}
	}

	// Token: 0x060044C8 RID: 17608 RVA: 0x0012CF70 File Offset: 0x0012B170
	private void Update()
	{
		if (this.hasSyncSource && this.emission != null)
		{
			float time = this.syncSource.time;
			if (time < this.previousTime)
			{
				this.emission.SetPlaybackTime(time);
			}
			else if (Mathf.Abs(this.syncSource.time - this.emission.Time) >= this.syncThreshold)
			{
				this.emission.SetPlaybackTime(time);
			}
			this.previousTime = time;
		}
	}

	// Token: 0x060044C9 RID: 17609 RVA: 0x0012CFE7 File Offset: 0x0012B1E7
	public void PlayVibration()
	{
		this.PlayVibration(0f);
	}

	// Token: 0x060044CA RID: 17610 RVA: 0x0012CFF4 File Offset: 0x0012B1F4
	public void PlayVibration(float fadeDuration)
	{
		this.StopVibration();
		VibrationData vibrationData = this.vibrationDataAsset;
		bool isLooping = this.loop;
		bool isRealtime = this.isRealTime;
		this.emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, "", isRealtime);
		if (this.hasSyncSource && this.emission != null)
		{
			float time = this.syncSource.time;
			this.emission.SetPlaybackTime(time);
			this.previousTime = time;
		}
		this.FadeInEmission(fadeDuration);
	}

	// Token: 0x060044CB RID: 17611 RVA: 0x0012D074 File Offset: 0x0012B274
	private void FadeInEmission(float duration)
	{
		if (duration <= 0f)
		{
			return;
		}
		if (this.emission == null)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.emission.SetStrength(0f);
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.FadeRoutine(1f, duration));
	}

	// Token: 0x060044CC RID: 17612 RVA: 0x0012D0E0 File Offset: 0x0012B2E0
	public void FadeOut(float duration)
	{
		if (duration <= 0f)
		{
			return;
		}
		if (this.emission == null)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			this.emission.Stop();
			this.emission = null;
			return;
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.FadeRoutine(0f, duration));
	}

	// Token: 0x060044CD RID: 17613 RVA: 0x0012D14B File Offset: 0x0012B34B
	private IEnumerator FadeRoutine(float targetStrength, float fade)
	{
		float inverse = 1f / fade;
		float start = this.emission.Strength;
		float t = 0f;
		while (t < 1f)
		{
			yield return null;
			float deltaTime = Time.deltaTime;
			t += deltaTime * inverse;
			this.emission.SetStrength(Mathf.Lerp(start, targetStrength, t));
		}
		this.emission.SetStrength(targetStrength);
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x060044CE RID: 17614 RVA: 0x0012D168 File Offset: 0x0012B368
	public void StopVibration()
	{
		VibrationEmission vibrationEmission = this.emission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		this.emission = null;
	}

	// Token: 0x040045B4 RID: 17844
	[SerializeField]
	private AudioSource syncSource;

	// Token: 0x040045B5 RID: 17845
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	// Token: 0x040045B6 RID: 17846
	[SerializeField]
	private float syncThreshold = 1f;

	// Token: 0x040045B7 RID: 17847
	[SerializeField]
	private bool loop;

	// Token: 0x040045B8 RID: 17848
	[SerializeField]
	private bool isRealTime;

	// Token: 0x040045B9 RID: 17849
	[SerializeField]
	private bool stopOnDisable;

	// Token: 0x040045BA RID: 17850
	private VibrationEmission emission;

	// Token: 0x040045BB RID: 17851
	private bool hasSyncSource;

	// Token: 0x040045BC RID: 17852
	private float previousTime;

	// Token: 0x040045BD RID: 17853
	private Coroutine fadeRoutine;
}
