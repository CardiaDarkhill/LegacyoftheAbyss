using System;
using UnityEngine;
using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9D RID: 3485
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Transition to an audio snapshot. Easy and fun.")]
	public class TransitionToAudioSnapshot : ComponentAction<AudioSource>
	{
		// Token: 0x06006537 RID: 25911 RVA: 0x001FE7E7 File Offset: 0x001FC9E7
		public override void Reset()
		{
			this.snapshot = null;
			this.transitionTime = 1f;
		}

		// Token: 0x06006538 RID: 25912 RVA: 0x001FE800 File Offset: 0x001FCA00
		public override void OnEnter()
		{
			AudioMixerSnapshot audioMixerSnapshot = this.snapshot.Value as AudioMixerSnapshot;
			if (audioMixerSnapshot != null)
			{
				audioMixerSnapshot.TransitionTo(this.transitionTime.Value);
			}
			base.Finish();
		}

		// Token: 0x0400642A RID: 25642
		[ObjectType(typeof(AudioMixerSnapshot))]
		public FsmObject snapshot;

		// Token: 0x0400642B RID: 25643
		public FsmFloat transitionTime;
	}
}
