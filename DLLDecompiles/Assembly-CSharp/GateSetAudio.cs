using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200011D RID: 285
public class GateSetAudio : MonoBehaviour
{
	// Token: 0x060008C7 RID: 2247 RVA: 0x0002901A File Offset: 0x0002721A
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.tag == "Player")
		{
			this.atmosSnapshot.TransitionTo(this.transitionTime);
			this.enviroSnapshot.TransitionTo(this.transitionTime);
		}
	}

	// Token: 0x04000871 RID: 2161
	public AudioMixerSnapshot atmosSnapshot;

	// Token: 0x04000872 RID: 2162
	public AudioMixerSnapshot enviroSnapshot;

	// Token: 0x04000873 RID: 2163
	public float transitionTime;
}
