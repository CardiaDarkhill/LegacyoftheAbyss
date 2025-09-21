using System;
using UnityEngine;

// Token: 0x0200013B RID: 315
public sealed class TransitionAudioFader : MonoBehaviour
{
	// Token: 0x170000CD RID: 205
	// (get) Token: 0x060009B0 RID: 2480 RVA: 0x0002C061 File Offset: 0x0002A261
	// (set) Token: 0x060009B1 RID: 2481 RVA: 0x0002C069 File Offset: 0x0002A269
	public float Volume { get; private set; }

	// Token: 0x060009B2 RID: 2482 RVA: 0x0002C074 File Offset: 0x0002A274
	private void Awake()
	{
		if (!this.ignoreAudioSource)
		{
			this.hasAudioSource = (this.audioSource != null);
			if (this.audioSource == null)
			{
				this.audioSource = base.gameObject.GetComponent<AudioSource>();
				this.hasAudioSource = (this.audioSource != null);
				this.audioSource;
			}
		}
		this.gm = GameManager.instance;
		VolumeBlendController component = base.gameObject.GetComponent<VolumeBlendController>();
		if (component)
		{
			this.volumeModifier = component.GetModifier("TransitionAudioFader");
		}
		if (this.gm != null && (this.gm.IsInSceneTransition || this.gm.IsLoadingSceneTransition))
		{
			this.targetVolume = (this.Volume = this.minVolume);
			this.SetVolume(this.minVolume);
		}
		this.RegisterEvents();
		TransitionAudioFader.uniqueList.Add(this);
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x0002C164 File Offset: 0x0002A364
	private void OnValidate()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.gameObject.GetComponent<AudioSource>();
		}
		if (this.audioSource != null && !Application.isPlaying)
		{
			this.targetVolume = this.audioSource.volume;
		}
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x0002C1B6 File Offset: 0x0002A3B6
	private void OnDestroy()
	{
		this.UnregisterEvents();
		TransitionAudioFader.uniqueList.Remove(this);
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x0002C1CC File Offset: 0x0002A3CC
	private void Update()
	{
		this.Volume = Mathf.MoveTowards(this.Volume, this.targetVolume, this.transitionRate * Time.deltaTime);
		this.SetVolume(this.Volume);
		base.enabled = (this.Volume != this.targetVolume);
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x0002C220 File Offset: 0x0002A420
	private void RegisterEvents()
	{
		if (!this.registeredEvents)
		{
			this.registeredEvents = true;
			AudioManager.OnAppliedActorSnapshot += this.OnAppliedActorSnapshot;
			if (!this.ignoreFadeOutOnLevelUnload && this.gm != null)
			{
				this.gm.UnloadingLevel += this.OnUnloadingLevel;
			}
		}
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x0002C27C File Offset: 0x0002A47C
	private void UnregisterEvents()
	{
		if (this.registeredEvents)
		{
			this.registeredEvents = false;
			AudioManager.OnAppliedActorSnapshot -= this.OnAppliedActorSnapshot;
			if (!this.ignoreFadeOutOnLevelUnload && this.gm != null)
			{
				this.gm.UnloadingLevel -= this.OnUnloadingLevel;
			}
		}
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x0002C2D6 File Offset: 0x0002A4D6
	private void OnAppliedActorSnapshot()
	{
		this.targetVolume = Mathf.Clamp01(this.maxVolume);
		base.enabled = (this.Volume != this.targetVolume);
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x0002C300 File Offset: 0x0002A500
	private void OnUnloadingLevel()
	{
		this.targetVolume = (this.targetVolume = Mathf.Clamp01(this.minVolume));
		base.enabled = (this.Volume != this.targetVolume);
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x0002C340 File Offset: 0x0002A540
	public static void FadeOutAllFaders()
	{
		TransitionAudioFader.uniqueList.ReserveListUsage();
		foreach (TransitionAudioFader transitionAudioFader in TransitionAudioFader.uniqueList.List)
		{
			transitionAudioFader.OnUnloadingLevel();
		}
		TransitionAudioFader.uniqueList.ReleaseListUsage();
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x0002C3A8 File Offset: 0x0002A5A8
	private void SetVolume(float volume)
	{
		if (this.volumeModifier != null)
		{
			this.volumeModifier.Volume = volume;
			return;
		}
		if (this.hasAudioSource)
		{
			this.audioSource.volume = volume;
		}
	}

	// Token: 0x04000940 RID: 2368
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000941 RID: 2369
	[SerializeField]
	private bool ignoreAudioSource;

	// Token: 0x04000942 RID: 2370
	[SerializeField]
	private bool ignoreFadeOutOnLevelUnload;

	// Token: 0x04000943 RID: 2371
	[Space]
	[SerializeField]
	private float transitionRate = 4f;

	// Token: 0x04000944 RID: 2372
	[Range(0f, 1f)]
	[SerializeField]
	private float targetVolume = 1f;

	// Token: 0x04000945 RID: 2373
	[Range(0f, 1f)]
	[SerializeField]
	private float minVolume;

	// Token: 0x04000946 RID: 2374
	[Range(0f, 1f)]
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x04000948 RID: 2376
	private static UniqueList<TransitionAudioFader> uniqueList = new UniqueList<TransitionAudioFader>();

	// Token: 0x04000949 RID: 2377
	private VolumeModifier volumeModifier;

	// Token: 0x0400094A RID: 2378
	private bool hasAudioSource;

	// Token: 0x0400094B RID: 2379
	private bool registeredEvents;

	// Token: 0x0400094C RID: 2380
	private GameManager gm;
}
