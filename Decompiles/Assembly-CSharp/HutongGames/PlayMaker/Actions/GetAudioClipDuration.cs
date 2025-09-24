using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5B RID: 3163
	public class GetAudioClipDuration : FsmStateAction
	{
		// Token: 0x06005FBB RID: 24507 RVA: 0x001E5A9B File Offset: 0x001E3C9B
		public override void Reset()
		{
			this.Clip = null;
			this.StoreDuration = null;
		}

		// Token: 0x06005FBC RID: 24508 RVA: 0x001E5AAC File Offset: 0x001E3CAC
		public override void OnEnter()
		{
			AudioClip audioClip = this.Clip.Value as AudioClip;
			this.StoreDuration.Value = (audioClip ? audioClip.length : 0f);
			base.Finish();
		}

		// Token: 0x04005D15 RID: 23829
		[ObjectType(typeof(AudioClip))]
		public FsmObject Clip;

		// Token: 0x04005D16 RID: 23830
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDuration;
	}
}
