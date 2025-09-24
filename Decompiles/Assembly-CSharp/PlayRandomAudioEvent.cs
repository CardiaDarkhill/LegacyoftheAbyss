using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200012E RID: 302
public class PlayRandomAudioEvent : MonoBehaviour
{
	// Token: 0x0600095B RID: 2395 RVA: 0x0002B0AE File Offset: 0x000292AE
	private void OnEnable()
	{
		if (this.useOwnAudio)
		{
			this.myAudioSource = base.GetComponent<AudioSource>();
		}
		if (this.playOnEnable && (!this.onStartForFirst || this.hasStarted))
		{
			this.Play();
		}
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x0002B0E2 File Offset: 0x000292E2
	private void Start()
	{
		this.hasStarted = true;
		if (this.onStartForFirst && this.playOnEnable)
		{
			this.Play();
		}
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x0002B101 File Offset: 0x00029301
	private void OnDisable()
	{
		if (this.stopOnDisable && this.spawnedAudioSource)
		{
			this.spawnedAudioSource.Stop();
			this.spawnedAudioSource = null;
		}
		if (this.useOwnAudio)
		{
			this.nextPlayTime = 0.0;
		}
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x0002B144 File Offset: 0x00029344
	public void Play()
	{
		if (ObjectPool.IsCreatingPool)
		{
			return;
		}
		if (Time.timeAsDouble < this.nextPlayTime)
		{
			return;
		}
		this.nextPlayTime = Time.timeAsDouble + (double)this.limitFrequency;
		if (this.useOwnAudio)
		{
			if (this.myAudioSource)
			{
				if (this.stopLastAudioOnPlay)
				{
					this.myAudioSource.Stop();
				}
				if (this.table)
				{
					this.table.PlayOneShot(this.myAudioSource, this.forcePlay);
				}
				else
				{
					this.myAudioSource.pitch = this.audioEvent.SelectPitch();
					PlayRandomAudioEvent.PlayOnSourceMethods playOnSourceMethods = this.playOnSourceMethod;
					if (playOnSourceMethods != PlayRandomAudioEvent.PlayOnSourceMethods.Play)
					{
						if (playOnSourceMethods != PlayRandomAudioEvent.PlayOnSourceMethods.PlayOneShot)
						{
							throw new ArgumentOutOfRangeException();
						}
						this.myAudioSource.PlayOneShot(this.audioEvent.GetClip());
					}
					else
					{
						this.myAudioSource.clip = this.audioEvent.GetClip();
						this.myAudioSource.Play();
					}
					this.audioEvent.PlayVibrationRandom();
				}
			}
		}
		else
		{
			Action onRecycled = null;
			if (this.stopOnDisable)
			{
				onRecycled = delegate()
				{
					this.spawnedAudioSource = null;
				};
			}
			if (this.stopLastAudioOnPlay && this.spawnedAudioSource != null)
			{
				this.spawnedAudioSource.Stop();
			}
			if (this.table)
			{
				this.spawnedAudioSource = this.table.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, this.forcePlay, 1f, onRecycled);
			}
			else
			{
				this.spawnedAudioSource = this.audioEvent.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, onRecycled);
			}
			if (this.keepPlayingInNextScene)
			{
				PlayAudioAndRecycle component = this.spawnedAudioSource.GetComponent<PlayAudioAndRecycle>();
				if (component)
				{
					component.KeepAliveThroughNextScene = true;
				}
			}
		}
		UnityEvent onPlayAudio = this.OnPlayAudio;
		if (onPlayAudio == null)
		{
			return;
		}
		onPlayAudio.Invoke();
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x0002B313 File Offset: 0x00029513
	public void StopLast()
	{
		if (!this.stopOnDisable)
		{
			Debug.LogError("Can't stop event when \"stopOnDisable\" is false");
			return;
		}
		if (this.spawnedAudioSource)
		{
			this.spawnedAudioSource.Stop();
			this.spawnedAudioSource = null;
		}
	}

	// Token: 0x040008FA RID: 2298
	[EnsurePrefab]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	// Token: 0x040008FB RID: 2299
	[Space]
	[SerializeField]
	private RandomAudioClipTable table;

	// Token: 0x040008FC RID: 2300
	[SerializeField]
	[ModifiableProperty]
	[Conditional("table", true, false, false)]
	private bool forcePlay;

	// Token: 0x040008FD RID: 2301
	[SerializeField]
	[ModifiableProperty]
	[Conditional("table", false, false, false)]
	private AudioEventRandom audioEvent;

	// Token: 0x040008FE RID: 2302
	[Space]
	[SerializeField]
	private bool playOnEnable;

	// Token: 0x040008FF RID: 2303
	[SerializeField]
	[ModifiableProperty]
	[Conditional("playOnEnable", true, false, false)]
	private bool onStartForFirst;

	// Token: 0x04000900 RID: 2304
	[SerializeField]
	private bool stopOnDisable;

	// Token: 0x04000901 RID: 2305
	[SerializeField]
	private bool stopLastAudioOnPlay;

	// Token: 0x04000902 RID: 2306
	[SerializeField]
	private bool useOwnAudio;

	// Token: 0x04000903 RID: 2307
	[SerializeField]
	[ModifiableProperty]
	[Conditional("useOwnAudio", true, false, false)]
	private PlayRandomAudioEvent.PlayOnSourceMethods playOnSourceMethod;

	// Token: 0x04000904 RID: 2308
	[SerializeField]
	private float limitFrequency;

	// Token: 0x04000905 RID: 2309
	[SerializeField]
	private bool keepPlayingInNextScene;

	// Token: 0x04000906 RID: 2310
	public UnityEvent OnPlayAudio;

	// Token: 0x04000907 RID: 2311
	private AudioSource myAudioSource;

	// Token: 0x04000908 RID: 2312
	private double nextPlayTime;

	// Token: 0x04000909 RID: 2313
	private bool hasStarted;

	// Token: 0x0400090A RID: 2314
	private AudioSource spawnedAudioSource;

	// Token: 0x0200146A RID: 5226
	private enum PlayOnSourceMethods
	{
		// Token: 0x0400831E RID: 33566
		Play,
		// Token: 0x0400831F RID: 33567
		PlayOneShot
	}
}
