using System;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class AudioSourceVisibilityPause : MonoBehaviour
{
	// Token: 0x060008A9 RID: 2217 RVA: 0x00028984 File Offset: 0x00026B84
	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
			if (this.audioSource == null)
			{
				Debug.LogError(string.Format("{0} is missing it's audio source. Destroying script to prevent null ref.", this), base.gameObject);
				if (Application.isPlaying)
				{
					Object.Destroy(this);
				}
			}
		}
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x000289DC File Offset: 0x00026BDC
	private void Reset()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x000289EA File Offset: 0x00026BEA
	private void OnBecameVisible()
	{
		this.audioSource.UnPause();
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x000289F7 File Offset: 0x00026BF7
	private void OnBecameInvisible()
	{
		this.audioSource.Pause();
	}

	// Token: 0x04000850 RID: 2128
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000851 RID: 2129
	private bool hasAudioSource;
}
