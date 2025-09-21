using System;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class FadeAudioOnAwake : MonoBehaviour
{
	// Token: 0x060008B5 RID: 2229 RVA: 0x00028B38 File Offset: 0x00026D38
	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.gameObject.GetComponent<AudioSource>();
			if (this.audioSource == null)
			{
				base.enabled = false;
				return;
			}
		}
		this.volumeBlendController = base.gameObject.GetComponent<VolumeBlendController>();
		if (this.volumeBlendController != null)
		{
			this.hasBlend = true;
			this.modifier = this.volumeBlendController.GetModifier("FadeAudioOnAwake");
		}
		if (this.useInitialVolumeAsTarget)
		{
			this.targetVolume = this.audioSource.volume;
		}
		if (this.setVolume)
		{
			this.SetVolume(this.startVolume);
		}
		this.targetVolume = Mathf.Clamp01(this.targetVolume);
		if (this.waitForHeroInPosition)
		{
			this.waitingForHero = true;
			this.hc = HeroController.instance;
		}
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x00028C0E File Offset: 0x00026E0E
	private void OnValidate()
	{
		if (this.audioSource != null)
		{
			this.audioSource = this.audioSource.GetComponent<AudioSource>();
		}
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x00028C30 File Offset: 0x00026E30
	private void Update()
	{
		if (this.waitingForHero)
		{
			if (!this.hc.isHeroInPosition)
			{
				return;
			}
			this.waitingForHero = false;
		}
		float num = Mathf.MoveTowards(this.audioSource.volume, this.targetVolume, this.fadeRate * Time.deltaTime);
		this.SetVolume(num);
		if (num == this.targetVolume)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x00028C94 File Offset: 0x00026E94
	[ContextMenu("Record Target Volume")]
	private void RecordTargetVolume()
	{
		this.useInitialVolumeAsTarget = false;
		if (this.audioSource != null)
		{
			this.audioSource = this.audioSource.GetComponent<AudioSource>();
		}
		if (this.audioSource != null)
		{
			this.targetVolume = this.audioSource.volume;
		}
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x00028CE6 File Offset: 0x00026EE6
	private void SetVolume(float volume)
	{
		if (this.hasBlend && this.modifier.IsValid)
		{
			this.modifier.Volume = volume;
			return;
		}
		this.audioSource.volume = volume;
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x00028D16 File Offset: 0x00026F16
	private float GetVolume()
	{
		if (this.hasBlend && this.modifier.IsValid)
		{
			return this.modifier.Volume;
		}
		return this.audioSource.volume;
	}

	// Token: 0x04000857 RID: 2135
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000858 RID: 2136
	[SerializeField]
	private FadeAudioOnAwake.FadeType fadeType;

	// Token: 0x04000859 RID: 2137
	[SerializeField]
	private float fadeRate = 2f;

	// Token: 0x0400085A RID: 2138
	[SerializeField]
	private bool setVolume;

	// Token: 0x0400085B RID: 2139
	[SerializeField]
	private float startVolume;

	// Token: 0x0400085C RID: 2140
	[SerializeField]
	private float targetVolume = 1f;

	// Token: 0x0400085D RID: 2141
	[SerializeField]
	private bool useInitialVolumeAsTarget;

	// Token: 0x0400085E RID: 2142
	[SerializeField]
	private bool waitForHeroInPosition;

	// Token: 0x0400085F RID: 2143
	private bool hasBlend;

	// Token: 0x04000860 RID: 2144
	private VolumeBlendController volumeBlendController;

	// Token: 0x04000861 RID: 2145
	private VolumeModifier modifier;

	// Token: 0x04000862 RID: 2146
	private bool waitingForHero;

	// Token: 0x04000863 RID: 2147
	private HeroController hc;

	// Token: 0x02001461 RID: 5217
	[Serializable]
	private enum FadeType
	{
		// Token: 0x04008301 RID: 33537
		FadeOut,
		// Token: 0x04008302 RID: 33538
		FadeIn
	}
}
