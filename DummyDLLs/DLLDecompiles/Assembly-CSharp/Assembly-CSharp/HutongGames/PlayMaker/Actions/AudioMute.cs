using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E37 RID: 3639
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Mute/un-mute the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioMute : ComponentAction<AudioSource>
	{
		// Token: 0x0600684F RID: 26703 RVA: 0x0020CB18 File Offset: 0x0020AD18
		public override void Reset()
		{
			this.gameObject = null;
			this.mute = false;
		}

		// Token: 0x06006850 RID: 26704 RVA: 0x0020CB2D File Offset: 0x0020AD2D
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.audio.mute = this.mute.Value;
			}
			base.Finish();
		}

		// Token: 0x04006780 RID: 26496
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an Audio Source component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006781 RID: 26497
		[RequiredField]
		[Tooltip("Check to mute, uncheck to un-mute.")]
		public FsmBool mute;
	}
}
