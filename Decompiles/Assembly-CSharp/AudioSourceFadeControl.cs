using System;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class AudioSourceFadeControl : MonoBehaviour
{
	// Token: 0x06000883 RID: 2179 RVA: 0x00028153 File Offset: 0x00026353
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (this.useStartVolume)
		{
			this.volumeUp = this.audioSource.volume;
		}
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0002817C File Offset: 0x0002637C
	private void OnDisable()
	{
		if (this.restartTimeOnEnd)
		{
			this.audioSource.Stop();
			this.audioSource.time = 0f;
		}
		if (this.finishFadeOutOnDisable && this.currentState == AudioSourceFadeControl.State.FadingDown)
		{
			this.audioSource.volume = 0f;
		}
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x000281D0 File Offset: 0x000263D0
	private void Update()
	{
		AudioSourceFadeControl.State state = this.currentState;
		if (state != AudioSourceFadeControl.State.FadingDown)
		{
			if (state == AudioSourceFadeControl.State.FadingUp)
			{
				this.audioSource.volume += Time.deltaTime * this.fadeSpeed;
				if (this.audioSource.volume >= this.volumeUp)
				{
					this.audioSource.volume = this.volumeUp;
					this.EndFade();
					return;
				}
			}
		}
		else
		{
			this.audioSource.volume -= Time.deltaTime * this.fadeSpeed;
			if (this.audioSource.volume <= 0f)
			{
				this.audioSource.volume = 0f;
				this.EndFade();
				if (this.disableAfterFadeDown)
				{
					base.gameObject.SetActive(false);
				}
				if (this.restartTimeOnEnd)
				{
					this.audioSource.time = 0f;
				}
			}
		}
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x000282AD File Offset: 0x000264AD
	private void EndFade()
	{
		this.currentState = AudioSourceFadeControl.State.Idle;
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x000282B6 File Offset: 0x000264B6
	public void FadeUp()
	{
		this.currentState = AudioSourceFadeControl.State.FadingUp;
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x000282BF File Offset: 0x000264BF
	public void FadeDown()
	{
		this.currentState = AudioSourceFadeControl.State.FadingDown;
	}

	// Token: 0x04000825 RID: 2085
	public float fadeSpeed = 1f;

	// Token: 0x04000826 RID: 2086
	public float volumeUp;

	// Token: 0x04000827 RID: 2087
	public bool useStartVolume = true;

	// Token: 0x04000828 RID: 2088
	[SerializeField]
	private bool disableAfterFadeDown;

	// Token: 0x04000829 RID: 2089
	[SerializeField]
	private bool restartTimeOnEnd;

	// Token: 0x0400082A RID: 2090
	[SerializeField]
	private bool finishFadeOutOnDisable;

	// Token: 0x0400082B RID: 2091
	private AudioSource audioSource;

	// Token: 0x0400082C RID: 2092
	[NonSerialized]
	private AudioSourceFadeControl.State currentState;

	// Token: 0x0200145E RID: 5214
	private enum State
	{
		// Token: 0x040082F5 RID: 33525
		Idle,
		// Token: 0x040082F6 RID: 33526
		FadingDown,
		// Token: 0x040082F7 RID: 33527
		FadingUp
	}
}
