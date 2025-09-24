using System;
using UnityEngine;

// Token: 0x02000337 RID: 823
public class FadeOutAudioSource : MonoBehaviour
{
	// Token: 0x06001CC6 RID: 7366 RVA: 0x0008601D File Offset: 0x0008421D
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06001CC7 RID: 7367 RVA: 0x0008602B File Offset: 0x0008422B
	private void Start()
	{
		this.hasStarted = true;
		this.resetVolume = (this.source ? this.source.volume : 0f);
	}

	// Token: 0x06001CC8 RID: 7368 RVA: 0x00086059 File Offset: 0x00084259
	private void OnEnable()
	{
		if (this.hasStarted && this.resetVolumeOnEnable && this.source)
		{
			this.source.volume = this.resetVolume;
		}
	}

	// Token: 0x06001CC9 RID: 7369 RVA: 0x00086089 File Offset: 0x00084289
	private void OnDisable()
	{
		this.Cancel();
	}

	// Token: 0x06001CCA RID: 7370 RVA: 0x00086091 File Offset: 0x00084291
	public void Cancel()
	{
		if (this.fadeRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.fadeRoutine);
		this.OnFadeEnd();
	}

	// Token: 0x06001CCB RID: 7371 RVA: 0x000860AE File Offset: 0x000842AE
	public void StartFade(float duration)
	{
		this.StartFade(duration, this.endBehaviour);
	}

	// Token: 0x06001CCC RID: 7372 RVA: 0x000860C0 File Offset: 0x000842C0
	public void StartFade(float duration, FadeOutAudioSource.EndBehaviours endBehaviourOverride)
	{
		if (!this.source)
		{
			return;
		}
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.startVolume = this.source.volume;
		this.endBehaviour = endBehaviourOverride;
		this.fadeRoutine = this.StartTimerRoutine(0f, duration, new Action<float>(this.OnFade), null, new Action(this.OnFadeEnd), false);
	}

	// Token: 0x06001CCD RID: 7373 RVA: 0x0008613C File Offset: 0x0008433C
	private void OnFade(float t)
	{
		if (this.source)
		{
			this.source.volume = Mathf.Lerp(this.startVolume, 0f, t);
		}
	}

	// Token: 0x06001CCE RID: 7374 RVA: 0x00086168 File Offset: 0x00084368
	private void OnFadeEnd()
	{
		if (this.source)
		{
			this.source.Stop();
		}
		this.fadeRoutine = null;
		switch (this.endBehaviour)
		{
		case FadeOutAudioSource.EndBehaviours.None:
			return;
		case FadeOutAudioSource.EndBehaviours.Disable:
			base.gameObject.SetActive(false);
			return;
		case FadeOutAudioSource.EndBehaviours.Recycle:
			base.gameObject.Recycle();
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x04001C26 RID: 7206
	[SerializeField]
	private bool resetVolumeOnEnable;

	// Token: 0x04001C27 RID: 7207
	[SerializeField]
	private FadeOutAudioSource.EndBehaviours endBehaviour;

	// Token: 0x04001C28 RID: 7208
	private Coroutine fadeRoutine;

	// Token: 0x04001C29 RID: 7209
	private float resetVolume;

	// Token: 0x04001C2A RID: 7210
	private bool hasStarted;

	// Token: 0x04001C2B RID: 7211
	private float startVolume;

	// Token: 0x04001C2C RID: 7212
	private AudioSource source;

	// Token: 0x02001600 RID: 5632
	public enum EndBehaviours
	{
		// Token: 0x04008968 RID: 35176
		None,
		// Token: 0x04008969 RID: 35177
		Disable,
		// Token: 0x0400896A RID: 35178
		Recycle
	}
}
