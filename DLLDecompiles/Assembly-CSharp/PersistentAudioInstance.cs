using System;
using UnityEngine;

// Token: 0x02000129 RID: 297
public sealed class PersistentAudioInstance : MonoBehaviour
{
	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000924 RID: 2340 RVA: 0x0002A7AF File Offset: 0x000289AF
	public string Key
	{
		get
		{
			return this.key;
		}
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x06000925 RID: 2341 RVA: 0x0002A7B7 File Offset: 0x000289B7
	public float FadeInRate
	{
		get
		{
			return this.fadeInRate;
		}
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06000926 RID: 2342 RVA: 0x0002A7BF File Offset: 0x000289BF
	public float FadeOutRate
	{
		get
		{
			return this.fadeOutRate;
		}
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06000927 RID: 2343 RVA: 0x0002A7C7 File Offset: 0x000289C7
	public bool AlsoSetOtherChangeRate
	{
		get
		{
			return this.alsoSetOtherChangeRate;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06000928 RID: 2344 RVA: 0x0002A7CF File Offset: 0x000289CF
	public AudioSource AudioSource
	{
		get
		{
			return this.audioSource;
		}
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06000929 RID: 2345 RVA: 0x0002A7D7 File Offset: 0x000289D7
	public bool KeepRelativePositionInNewScene
	{
		get
		{
			return this.keepRelativePositionInNewScene;
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x0600092A RID: 2346 RVA: 0x0002A7DF File Offset: 0x000289DF
	public bool AdoptNewInstancePosition
	{
		get
		{
			return this.adoptNewInstancePosition;
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x0600092B RID: 2347 RVA: 0x0002A7E7 File Offset: 0x000289E7
	public bool AdoptPreviousPlayingState
	{
		get
		{
			return this.adoptPreviousPlayingState;
		}
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x0600092C RID: 2348 RVA: 0x0002A7EF File Offset: 0x000289EF
	// (set) Token: 0x0600092D RID: 2349 RVA: 0x0002A7F7 File Offset: 0x000289F7
	public bool IsFromPreviousScene { get; set; }

	// Token: 0x0600092E RID: 2350 RVA: 0x0002A800 File Offset: 0x00028A00
	private void Awake()
	{
		this.hasAudioSource = (this.audioSource != null);
		if (!this.hasAudioSource)
		{
			this.audioSource = base.gameObject.GetComponent<AudioSource>();
			this.hasAudioSource = (this.audioSource != null);
			if (!this.hasAudioSource)
			{
				return;
			}
		}
		if (string.IsNullOrEmpty(this.key))
		{
			this.key = "PersistentAudioInstance";
		}
		this.volumeBlendController = this.audioSource.GetComponent<VolumeBlendController>();
		this.hasVolumeBlendController = (this.volumeBlendController != null);
		if (this.hasVolumeBlendController)
		{
			this.modifier = this.volumeBlendController.GetModifier("PersistentAudioInstance");
		}
		else
		{
			this.targetVolume = this.audioSource.volume;
		}
		PersistentAudioManager.AddInstance(this);
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x0002A8C5 File Offset: 0x00028AC5
	private void Start()
	{
		base.transform.SetParent(null);
		Object.DontDestroyOnLoad(base.gameObject);
		this.shouldUpdate = true;
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x0002A8E5 File Offset: 0x00028AE5
	private void OnValidate()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.gameObject.GetComponent<AudioSource>();
		}
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x0002A906 File Offset: 0x00028B06
	private void OnDestroy()
	{
		PersistentAudioManager.RemoveInstance(this);
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x0002A910 File Offset: 0x00028B10
	public void UpdateVolume()
	{
		if (!this.shouldUpdate && !this.destroyedQueued)
		{
			return;
		}
		if (!this.hasAudioSource)
		{
			if (this.destroyedQueued)
			{
				Object.Destroy(base.gameObject);
			}
			return;
		}
		if (this.trySync)
		{
			this.trySync = false;
			if (this.syncTarget != null)
			{
				if (this.audioSource.clip == this.syncTarget.clip)
				{
					this.audioSource.timeSamples = this.syncTarget.timeSamples;
				}
				this.syncTarget = null;
			}
		}
		float num = this.destroyedQueued ? this.fadeOutRate : this.fadeInRate;
		this.instanceVolume = Mathf.MoveTowards(this.instanceVolume, this.targetVolume, Time.deltaTime * num);
		this.SetVolume(this.instanceVolume);
		if (this.instanceVolume == this.targetVolume)
		{
			if (this.destroyedQueued)
			{
				Object.Destroy(base.gameObject);
			}
			this.shouldUpdate = false;
		}
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x0002AA0A File Offset: 0x00028C0A
	private void SetVolume(float volume)
	{
		if (this.hasVolumeBlendController)
		{
			this.modifier.Volume = volume;
			return;
		}
		this.audioSource.volume = volume;
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x0002AA2D File Offset: 0x00028C2D
	private float GetCurrentVolume()
	{
		if (this.hasVolumeBlendController)
		{
			return this.modifier.Volume;
		}
		return this.audioSource.volume;
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x0002AA4E File Offset: 0x00028C4E
	public void SetChangeRate(float changeRate)
	{
		this.fadeInRate = changeRate;
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x0002AA57 File Offset: 0x00028C57
	public void SetTargetVolume(float targetVolume)
	{
		targetVolume = Mathf.Clamp01(targetVolume);
		if (this.targetVolume != targetVolume)
		{
			this.targetVolume = targetVolume;
			this.shouldUpdate = true;
		}
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x0002AA78 File Offset: 0x00028C78
	public void MarkForDestroy()
	{
		if (!this.IsFromPreviousScene || this.destroyedQueued)
		{
			return;
		}
		this.destroyedQueued = true;
		this.targetVolume = 0f;
		this.shouldUpdate = true;
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x0002AAA4 File Offset: 0x00028CA4
	public void QueueFadeUp()
	{
		this.instanceVolume = 0f;
		this.SetVolume(0f);
		this.shouldUpdate = true;
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x0002AAC3 File Offset: 0x00028CC3
	public void SetSyncTarget(AudioSource audioSource)
	{
		this.syncTarget = audioSource;
		this.trySync = (this.syncTarget != null);
	}

	// Token: 0x040008D7 RID: 2263
	[SerializeField]
	private string key;

	// Token: 0x040008D8 RID: 2264
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040008D9 RID: 2265
	[SerializeField]
	private float fadeInRate = 1f;

	// Token: 0x040008DA RID: 2266
	[SerializeField]
	private float fadeOutRate = 1f;

	// Token: 0x040008DB RID: 2267
	[Tooltip("If true, other instances will fade out at same rate that is is fading in.")]
	[SerializeField]
	private bool alsoSetOtherChangeRate;

	// Token: 0x040008DC RID: 2268
	[Tooltip("If true, will attach self to camera in new scene so that sound source moves with camera during positioning.")]
	[SerializeField]
	private bool keepRelativePositionInNewScene;

	// Token: 0x040008DD RID: 2269
	[Tooltip("If true, relocate any existing instance(s) to this instance's position upon registration.")]
	[SerializeField]
	private bool adoptNewInstancePosition;

	// Token: 0x040008DE RID: 2270
	[Tooltip("If true, adopt the previous instance's playing state (including its clip and play position) when registered.")]
	[SerializeField]
	private bool adoptPreviousPlayingState;

	// Token: 0x040008E0 RID: 2272
	private bool hasVolumeBlendController;

	// Token: 0x040008E1 RID: 2273
	private VolumeBlendController volumeBlendController;

	// Token: 0x040008E2 RID: 2274
	private VolumeModifier modifier;

	// Token: 0x040008E3 RID: 2275
	private bool destroyedQueued;

	// Token: 0x040008E4 RID: 2276
	private bool shouldUpdate;

	// Token: 0x040008E5 RID: 2277
	private float targetVolume = 1f;

	// Token: 0x040008E6 RID: 2278
	private float instanceVolume = 1f;

	// Token: 0x040008E7 RID: 2279
	private bool hasAudioSource;

	// Token: 0x040008E8 RID: 2280
	private bool trySync;

	// Token: 0x040008E9 RID: 2281
	private AudioSource syncTarget;
}
