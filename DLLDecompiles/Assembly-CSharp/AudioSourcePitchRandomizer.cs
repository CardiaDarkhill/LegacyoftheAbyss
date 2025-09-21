using System;
using UnityEngine;

// Token: 0x02000114 RID: 276
[RequireComponent(typeof(AudioSource))]
public class AudioSourcePitchRandomizer : MonoBehaviour
{
	// Token: 0x06000898 RID: 2200 RVA: 0x0002866E File Offset: 0x0002686E
	private void Awake()
	{
		this.RandomisePitch();
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x00028676 File Offset: 0x00026876
	private void OnEnable()
	{
		this.RandomisePitch();
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x0002867E File Offset: 0x0002687E
	public void RandomisePitch()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.pitch = Random.Range(this.pitchLower, this.pitchUpper);
	}

	// Token: 0x0400083D RID: 2109
	[Header("Randomize Pitch")]
	[Range(0.75f, 1f)]
	public float pitchLower = 1f;

	// Token: 0x0400083E RID: 2110
	[Range(1f, 1.25f)]
	public float pitchUpper = 1f;

	// Token: 0x0400083F RID: 2111
	private AudioSource audioSource;
}
