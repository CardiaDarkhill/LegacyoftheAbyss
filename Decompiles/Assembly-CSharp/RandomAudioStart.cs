using System;
using UnityEngine;

// Token: 0x02000534 RID: 1332
public class RandomAudioStart : MonoBehaviour
{
	// Token: 0x06002FC4 RID: 12228 RVA: 0x000D246F File Offset: 0x000D066F
	private void Start()
	{
		this.started = true;
		this.SafeStart();
	}

	// Token: 0x06002FC5 RID: 12229 RVA: 0x000D247E File Offset: 0x000D067E
	private void OnEnable()
	{
		if (this.started)
		{
			this.SafeStart();
		}
	}

	// Token: 0x06002FC6 RID: 12230 RVA: 0x000D248E File Offset: 0x000D068E
	private void OnDisable()
	{
		if (this.registeredHeroEvent && this.hc != null)
		{
			this.hc.heroInPosition -= this.OnHeroInPosition;
		}
		this.hc = null;
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x000D24C4 File Offset: 0x000D06C4
	private void SafeStart()
	{
		if (!this.waitForHeroInPosition)
		{
			this.DoRandomAudioStart();
			return;
		}
		this.hc = HeroController.instance;
		if (this.hc != null && !this.hc.isHeroInPosition)
		{
			this.hc.heroInPosition += this.OnHeroInPosition;
			this.registeredHeroEvent = true;
			return;
		}
		this.DoRandomAudioStart();
	}

	// Token: 0x06002FC8 RID: 12232 RVA: 0x000D252B File Offset: 0x000D072B
	private void OnHeroInPosition(bool forceDirect)
	{
		this.DoRandomAudioStart();
		if (this.hc != null)
		{
			this.hc.heroInPosition -= this.OnHeroInPosition;
		}
		this.registeredHeroEvent = false;
	}

	// Token: 0x06002FC9 RID: 12233 RVA: 0x000D2560 File Offset: 0x000D0760
	private void DoRandomAudioStart()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
			if (this.audioSource == null)
			{
				return;
			}
		}
		float num = Random.Range(this.timeMin, this.timeMax);
		if (this.audioSource.clip != null && num > this.audioSource.clip.length)
		{
			num = 0f;
		}
		this.audioSource.time = num;
		this.audioSource.pitch = Random.Range(this.pitchMin, this.pitchMax);
		this.audioSource.Play();
	}

	// Token: 0x04003287 RID: 12935
	public AudioSource audioSource;

	// Token: 0x04003288 RID: 12936
	public float timeMin;

	// Token: 0x04003289 RID: 12937
	public float timeMax = 1f;

	// Token: 0x0400328A RID: 12938
	public float pitchMin = 1f;

	// Token: 0x0400328B RID: 12939
	public float pitchMax = 1f;

	// Token: 0x0400328C RID: 12940
	[SerializeField]
	private bool waitForHeroInPosition;

	// Token: 0x0400328D RID: 12941
	private bool started;

	// Token: 0x0400328E RID: 12942
	private bool registeredHeroEvent;

	// Token: 0x0400328F RID: 12943
	private HeroController hc;
}
