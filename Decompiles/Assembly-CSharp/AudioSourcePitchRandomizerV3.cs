using System;
using UnityEngine;

// Token: 0x02000116 RID: 278
[RequireComponent(typeof(AudioSource))]
public class AudioSourcePitchRandomizerV3 : MonoBehaviour
{
	// Token: 0x0600089F RID: 2207 RVA: 0x0002875E File Offset: 0x0002695E
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.pitch = Random.Range(this.pitchLower, this.pitchUpper);
		if (this.playAfterPitchSet)
		{
			this.audioSource.Play();
		}
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x0002879B File Offset: 0x0002699B
	private void OnEnable()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.pitch = Random.Range(this.pitchLower, this.pitchUpper);
		if (this.playAfterPitchSet)
		{
			this.audioSource.Play();
		}
	}

	// Token: 0x04000844 RID: 2116
	[Header("Randomize Pitch")]
	public float pitchLower = 1f;

	// Token: 0x04000845 RID: 2117
	public float pitchUpper = 1f;

	// Token: 0x04000846 RID: 2118
	public bool playAfterPitchSet;

	// Token: 0x04000847 RID: 2119
	private AudioSource audioSource;
}
