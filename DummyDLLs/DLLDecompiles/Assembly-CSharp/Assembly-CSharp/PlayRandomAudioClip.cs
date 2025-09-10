using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200012D RID: 301
public class PlayRandomAudioClip : MonoBehaviour
{
	// Token: 0x06000957 RID: 2391 RVA: 0x0002AFFA File Offset: 0x000291FA
	private void Reset()
	{
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x0002B008 File Offset: 0x00029208
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.Play();
		}
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x0002B018 File Offset: 0x00029218
	public void Play()
	{
		this.source.clip = this.clips[Random.Range(0, this.clips.Length)];
		this.source.pitch = this.pitchRange.GetRandomValue();
		this.source.volume = this.volumeRange.GetRandomValue();
		this.source.Play();
	}

	// Token: 0x040008F5 RID: 2293
	[SerializeField]
	private AudioSource source;

	// Token: 0x040008F6 RID: 2294
	[SerializeField]
	private AudioClip[] clips;

	// Token: 0x040008F7 RID: 2295
	[SerializeField]
	private MinMaxFloat pitchRange = new MinMaxFloat(1f, 1f);

	// Token: 0x040008F8 RID: 2296
	[SerializeField]
	private MinMaxFloat volumeRange = new MinMaxFloat(1f, 1f);

	// Token: 0x040008F9 RID: 2297
	[SerializeField]
	private bool onEnable;
}
