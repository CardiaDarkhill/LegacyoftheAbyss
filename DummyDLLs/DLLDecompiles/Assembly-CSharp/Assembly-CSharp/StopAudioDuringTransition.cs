using System;
using UnityEngine;

// Token: 0x02000137 RID: 311
public class StopAudioDuringTransition : MonoBehaviour
{
	// Token: 0x06000996 RID: 2454 RVA: 0x0002BD49 File Offset: 0x00029F49
	private void OnValidate()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0002BD68 File Offset: 0x00029F68
	private void OnEnable()
	{
		if (this.audioSource == null)
		{
			return;
		}
		HeroController instance = HeroController.instance;
		if (!instance)
		{
			return;
		}
		if (!instance.cState.transitioning)
		{
			return;
		}
		if (!this.audioSource.loop)
		{
			this.audioSource.Stop();
		}
	}

	// Token: 0x04000936 RID: 2358
	[SerializeField]
	private AudioSource audioSource;
}
