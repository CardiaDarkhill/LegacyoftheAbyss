using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000134 RID: 308
public class SnapshotOnActivation : MonoBehaviour
{
	// Token: 0x0600098C RID: 2444 RVA: 0x0002BCAC File Offset: 0x00029EAC
	private void OnEnable()
	{
		this.activationSnapshot.TransitionTo(this.transitionTime);
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x0002BCBF File Offset: 0x00029EBF
	private void OnDisable()
	{
		this.deactivationSnapshot.TransitionTo(this.transitionTime);
	}

	// Token: 0x04000930 RID: 2352
	public AudioMixerSnapshot activationSnapshot;

	// Token: 0x04000931 RID: 2353
	public AudioMixerSnapshot deactivationSnapshot;

	// Token: 0x04000932 RID: 2354
	public float transitionTime;
}
