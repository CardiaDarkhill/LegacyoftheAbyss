using System;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class AudioEventAnimationEvents : MonoBehaviour
{
	// Token: 0x0600081E RID: 2078 RVA: 0x0002682F File Offset: 0x00024A2F
	[UsedImplicitly]
	private bool DisablePlayOnSource()
	{
		return this.audioSourcePrefab;
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0002683C File Offset: 0x00024A3C
	public void PlayAudioEvent(int index)
	{
		AudioEventRandom audioEventRandom = this.audioEvents[index];
		if (this.audioSourcePrefab || !this.playOnSource)
		{
			audioEventRandom.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
			return;
		}
		this.playOnSource.pitch = audioEventRandom.SelectPitch();
		this.playOnSource.PlayOneShot(audioEventRandom.GetClip(), audioEventRandom.Volume);
	}

	// Token: 0x040007C2 RID: 1986
	[SerializeField]
	private AudioSource audioSourcePrefab;

	// Token: 0x040007C3 RID: 1987
	[SerializeField]
	[ModifiableProperty]
	[Conditional("DisablePlayOnSource", false, true, false)]
	private AudioSource playOnSource;

	// Token: 0x040007C4 RID: 1988
	[SerializeField]
	private AudioEventRandom[] audioEvents;
}
