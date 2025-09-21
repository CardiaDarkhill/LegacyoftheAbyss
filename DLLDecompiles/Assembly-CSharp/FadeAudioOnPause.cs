using System;
using UnityEngine;

// Token: 0x0200011B RID: 283
public sealed class FadeAudioOnPause : MonoBehaviour
{
	// Token: 0x060008BC RID: 2236 RVA: 0x00028D64 File Offset: 0x00026F64
	private void Awake()
	{
		this.hasAudioSource = (this.audioSource != null);
		if (!this.hasAudioSource)
		{
			this.audioSource = base.GetComponent<AudioSource>();
			this.hasModifier = (this.audioSource != null);
		}
		VolumeBlendController component = base.GetComponent<VolumeBlendController>();
		if (component != null)
		{
			this.volumeModifier = component.GetModifier("PAUSE_FADE");
			this.hasModifier = true;
		}
		if (this.hasModifier)
		{
			this.volume = 1f;
			return;
		}
		if (this.hasAudioSource)
		{
			this.originalVolume = this.audioSource.volume;
			this.volume = this.audioSource.volume;
		}
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x00028E0F File Offset: 0x0002700F
	private void OnEnable()
	{
		this.gm = GameManager.instance;
		if (this.gm)
		{
			this.gm.GamePausedChange += this.GamePausedChange;
		}
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x00028E40 File Offset: 0x00027040
	private void OnDisable()
	{
		if (this.gm)
		{
			this.gm.GamePausedChange -= this.GamePausedChange;
		}
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x00028E68 File Offset: 0x00027068
	private void Update()
	{
		if (this.fadeState > FadeAudioOnPause.FadeState.None)
		{
			this.volume = Mathf.MoveTowards(this.volume, this.targetVolume, this.fadeRate * Time.unscaledDeltaTime);
			this.SetVolume(this.volume);
			if (Mathf.Approximately(this.volume, this.targetVolume))
			{
				this.fadeState = FadeAudioOnPause.FadeState.None;
			}
		}
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x00028EC7 File Offset: 0x000270C7
	private void SetVolume(float volume)
	{
		this.volume = volume;
		if (this.hasModifier)
		{
			this.volumeModifier.Volume = volume;
			return;
		}
		if (this.hasAudioSource)
		{
			this.audioSource.volume = volume;
		}
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00028EFC File Offset: 0x000270FC
	private void GamePausedChange(bool ispaused)
	{
		if (ispaused)
		{
			this.targetVolume = this.pausedVolume;
			if (this.fadeState != FadeAudioOnPause.FadeState.FadingDown)
			{
				this.fadeState = FadeAudioOnPause.FadeState.FadingDown;
				if (this.hasModifier)
				{
					this.originalVolume = 1f;
					return;
				}
				if (this.hasAudioSource)
				{
					this.originalVolume = this.audioSource.volume;
					this.volume = this.audioSource.volume;
					return;
				}
			}
		}
		else if (this.fadeState != FadeAudioOnPause.FadeState.FadingUp)
		{
			this.fadeState = FadeAudioOnPause.FadeState.FadingUp;
			this.targetVolume = this.originalVolume;
		}
	}

	// Token: 0x04000864 RID: 2148
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000865 RID: 2149
	[SerializeField]
	private float fadeRate = 1f;

	// Token: 0x04000866 RID: 2150
	[SerializeField]
	private float pausedVolume = 0.25f;

	// Token: 0x04000867 RID: 2151
	private float volume;

	// Token: 0x04000868 RID: 2152
	private bool hasAudioSource;

	// Token: 0x04000869 RID: 2153
	private bool hasModifier;

	// Token: 0x0400086A RID: 2154
	private VolumeModifier volumeModifier;

	// Token: 0x0400086B RID: 2155
	private FadeAudioOnPause.FadeState fadeState;

	// Token: 0x0400086C RID: 2156
	private float targetVolume;

	// Token: 0x0400086D RID: 2157
	private float originalVolume = 1f;

	// Token: 0x0400086E RID: 2158
	private GameManager gm;

	// Token: 0x02001462 RID: 5218
	private enum FadeState
	{
		// Token: 0x04008304 RID: 33540
		None,
		// Token: 0x04008305 RID: 33541
		FadingUp,
		// Token: 0x04008306 RID: 33542
		FadingDown
	}
}
