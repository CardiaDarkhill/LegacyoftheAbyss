using System;
using UnityEngine;

// Token: 0x02000115 RID: 277
[RequireComponent(typeof(AudioSource))]
public class AudioSourcePitchRandomizerV2 : MonoBehaviour
{
	// Token: 0x0600089C RID: 2204 RVA: 0x000286C6 File Offset: 0x000268C6
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.pitch = Random.Range(this.pitchLower, this.pitchUpper);
		if (this.playAfterPitchSet)
		{
			this.audioSource.Play();
		}
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x00028703 File Offset: 0x00026903
	private void OnEnable()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.pitch = Random.Range(this.pitchLower, this.pitchUpper);
		if (this.playAfterPitchSet)
		{
			this.audioSource.Play();
		}
	}

	// Token: 0x04000840 RID: 2112
	[Header("Randomize Pitch")]
	[Range(0.1f, 1f)]
	public float pitchLower = 1f;

	// Token: 0x04000841 RID: 2113
	[Range(1f, 10f)]
	public float pitchUpper = 1f;

	// Token: 0x04000842 RID: 2114
	public bool playAfterPitchSet;

	// Token: 0x04000843 RID: 2115
	private AudioSource audioSource;
}
