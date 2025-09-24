using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000132 RID: 306
public class SetSceneAudio : MonoBehaviour
{
	// Token: 0x0600097F RID: 2431 RVA: 0x0002BAD6 File Offset: 0x00029CD6
	private void Start()
	{
		this.atmosSnapshot.TransitionTo(this.transitionTime);
		this.enviroSnapshot.TransitionTo(this.transitionTime);
		this.actorSnapshot.TransitionTo(this.transitionTime);
	}

	// Token: 0x04000926 RID: 2342
	public AudioMixerSnapshot atmosSnapshot;

	// Token: 0x04000927 RID: 2343
	public AudioMixerSnapshot enviroSnapshot;

	// Token: 0x04000928 RID: 2344
	public AudioMixerSnapshot actorSnapshot;

	// Token: 0x04000929 RID: 2345
	public float transitionTime;
}
