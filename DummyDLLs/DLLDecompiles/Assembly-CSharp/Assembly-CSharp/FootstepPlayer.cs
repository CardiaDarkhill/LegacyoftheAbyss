using System;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class FootstepPlayer : MonoBehaviour
{
	// Token: 0x060008C3 RID: 2243 RVA: 0x00028FAC File Offset: 0x000271AC
	protected virtual void Awake()
	{
		this.footstepsTable == null;
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
			this.audioSource == null;
		}
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x00028FE2 File Offset: 0x000271E2
	protected virtual void OnValidate()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00028FFE File Offset: 0x000271FE
	public void PlayFootstep()
	{
		this.footstepsTable.PlayOneShot(this.audioSource, false);
	}

	// Token: 0x0400086F RID: 2159
	[SerializeField]
	protected RandomAudioClipTable footstepsTable;

	// Token: 0x04000870 RID: 2160
	[SerializeField]
	protected AudioSource audioSource;
}
