using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200052D RID: 1325
public class PlayChainSound : MonoBehaviour
{
	// Token: 0x06002F94 RID: 12180 RVA: 0x000D1821 File Offset: 0x000CFA21
	private void Awake()
	{
		if (!this.source)
		{
			this.source = base.GetComponent<AudioSource>();
		}
	}

	// Token: 0x06002F95 RID: 12181 RVA: 0x000D183C File Offset: 0x000CFA3C
	public void PlayHitSound(Vector3 position)
	{
		if (this.hitSounds.Length == 0)
		{
			return;
		}
		AudioClip clip = this.hitSounds[Random.Range(0, this.hitSounds.Length)];
		this.PlaySound(position, clip, this.hitSoundPitch);
	}

	// Token: 0x06002F96 RID: 12182 RVA: 0x000D1878 File Offset: 0x000CFA78
	public void PlayBrokenHitSound(Vector3 position)
	{
		if (this.brokenHitSounds.Length == 0)
		{
			return;
		}
		AudioClip clip = this.brokenHitSounds[Random.Range(0, this.brokenHitSounds.Length)];
		this.PlaySound(position, clip, this.brokenHitSoundPitch);
	}

	// Token: 0x06002F97 RID: 12183 RVA: 0x000D18B4 File Offset: 0x000CFAB4
	public void PlayTouchSound(Vector3 position)
	{
		if (this.touchSounds.Length == 0)
		{
			return;
		}
		AudioClip clip = this.touchSounds[Random.Range(0, this.touchSounds.Length)];
		this.PlaySound(position, clip, this.touchSoundPitch);
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x000D18F0 File Offset: 0x000CFAF0
	private void PlaySound(Vector3 position, AudioClip clip, MinMaxFloat pitch)
	{
		if (this.source)
		{
			this.source.pitch = pitch.GetRandomValue();
			this.source.PlayOneShot(clip);
			return;
		}
		new AudioEvent
		{
			Clip = clip,
			Volume = 1f,
			PitchMin = pitch.Start,
			PitchMax = pitch.End
		}.SpawnAndPlayOneShot(position, null);
	}

	// Token: 0x04003254 RID: 12884
	[SerializeField]
	private AudioSource source;

	// Token: 0x04003255 RID: 12885
	[Space]
	[SerializeField]
	private AudioClip[] hitSounds;

	// Token: 0x04003256 RID: 12886
	[SerializeField]
	private MinMaxFloat hitSoundPitch = new MinMaxFloat(1f, 1f);

	// Token: 0x04003257 RID: 12887
	[SerializeField]
	private AudioClip[] brokenHitSounds;

	// Token: 0x04003258 RID: 12888
	[SerializeField]
	private MinMaxFloat brokenHitSoundPitch = new MinMaxFloat(1f, 1f);

	// Token: 0x04003259 RID: 12889
	[SerializeField]
	private AudioClip[] touchSounds;

	// Token: 0x0400325A RID: 12890
	[SerializeField]
	private MinMaxFloat touchSoundPitch = new MinMaxFloat(1f, 1f);
}
