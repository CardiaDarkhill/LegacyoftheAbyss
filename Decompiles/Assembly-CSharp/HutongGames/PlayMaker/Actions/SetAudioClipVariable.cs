using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E3E RID: 3646
	[ActionCategory(ActionCategory.Audio)]
	public class SetAudioClipVariable : FsmStateAction
	{
		// Token: 0x0600686A RID: 26730 RVA: 0x0020D1E0 File Offset: 0x0020B3E0
		public override void Reset()
		{
			this.audioClipVariable = null;
			this.audioClip = null;
		}

		// Token: 0x0600686B RID: 26731 RVA: 0x0020D1F0 File Offset: 0x0020B3F0
		public override void OnEnter()
		{
			this.audioClipVariable.Value = this.audioClip.Value;
			base.Finish();
		}

		// Token: 0x0400679C RID: 26524
		[ObjectType(typeof(AudioClip))]
		[Tooltip("The AudioClip variable.")]
		public FsmObject audioClipVariable;

		// Token: 0x0400679D RID: 26525
		[ObjectType(typeof(AudioClip))]
		[Tooltip("The AudioClip to store.")]
		public FsmObject audioClip;
	}
}
