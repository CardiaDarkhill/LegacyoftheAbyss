using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E38 RID: 3640
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Pauses playing the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioPause : ComponentAction<AudioSource>
	{
		// Token: 0x06006852 RID: 26706 RVA: 0x0020CB6C File Offset: 0x0020AD6C
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006853 RID: 26707 RVA: 0x0020CB75 File Offset: 0x0020AD75
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.audio.Pause();
			}
			base.Finish();
		}

		// Token: 0x04006782 RID: 26498
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an Audio Source component.")]
		public FsmOwnerDefault gameObject;
	}
}
