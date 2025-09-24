using System;
using UnityEngine;

// Token: 0x02000135 RID: 309
[RequireComponent(typeof(AudioSource))]
public class SpawnableAudioSource : MonoBehaviour
{
	// Token: 0x0600098F RID: 2447 RVA: 0x0002BCDA File Offset: 0x00029EDA
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x0002BCE8 File Offset: 0x00029EE8
	protected void OnEnable()
	{
		this.framesPassed = 0;
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x0002BCF1 File Offset: 0x00029EF1
	protected void Update()
	{
		this.framesPassed++;
		if (this.framesPassed > 5 && !this.audioSource.isPlaying)
		{
			this.Recycle<SpawnableAudioSource>();
		}
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x0002BD1D File Offset: 0x00029F1D
	public void Stop()
	{
		this.audioSource.Stop();
	}

	// Token: 0x04000933 RID: 2355
	private AudioSource audioSource;

	// Token: 0x04000934 RID: 2356
	private const int MinimumFrames = 5;

	// Token: 0x04000935 RID: 2357
	private int framesPassed;
}
