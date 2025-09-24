using System;
using UnityEngine.Audio;

// Token: 0x0200045F RID: 1119
public static class AudioMixerExtensions
{
	// Token: 0x0600282A RID: 10282 RVA: 0x000B1DBA File Offset: 0x000AFFBA
	public static void TransitionToSafe(this AudioMixerSnapshot snapshot, float timeToReach)
	{
		if (snapshot == null)
		{
			return;
		}
		snapshot.TransitionTo(timeToReach);
	}
}
