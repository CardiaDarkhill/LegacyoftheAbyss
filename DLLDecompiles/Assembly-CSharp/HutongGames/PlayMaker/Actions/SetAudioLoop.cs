using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E3F RID: 3647
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets looping on the AudioSource component on a Game Object.")]
	public class SetAudioLoop : ComponentAction<AudioSource>
	{
		// Token: 0x0600686D RID: 26733 RVA: 0x0020D216 File Offset: 0x0020B416
		public override void Reset()
		{
			this.gameObject = null;
			this.loop = false;
		}

		// Token: 0x0600686E RID: 26734 RVA: 0x0020D22B File Offset: 0x0020B42B
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.audio.loop = this.loop.Value;
			}
			base.Finish();
		}

		// Token: 0x0400679E RID: 26526
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("A GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400679F RID: 26527
		[Tooltip("Set the Audio Source looping.")]
		public FsmBool loop;
	}
}
