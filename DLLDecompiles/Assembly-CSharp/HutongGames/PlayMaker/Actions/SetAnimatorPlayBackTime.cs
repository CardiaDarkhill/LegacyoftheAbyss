using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E12 RID: 3602
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the playback position in the recording buffer. When in playback mode (use AnimatorStartPlayback), this value is used for controlling the current playback position in the buffer (in seconds). The value can range between recordingStartTime and recordingStopTime ")]
	public class SetAnimatorPlayBackTime : ComponentAction<Animator>
	{
		// Token: 0x060067B3 RID: 26547 RVA: 0x0020AE7D File Offset: 0x0020907D
		public override void Reset()
		{
			this.gameObject = null;
			this.playbackTime = null;
			this.everyFrame = false;
		}

		// Token: 0x060067B4 RID: 26548 RVA: 0x0020AE94 File Offset: 0x00209094
		public override void OnEnter()
		{
			this.DoPlaybackTime();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067B5 RID: 26549 RVA: 0x0020AEAA File Offset: 0x002090AA
		public override void OnUpdate()
		{
			this.DoPlaybackTime();
		}

		// Token: 0x060067B6 RID: 26550 RVA: 0x0020AEB2 File Offset: 0x002090B2
		private void DoPlaybackTime()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.playbackTime = this.playbackTime.Value;
			}
		}

		// Token: 0x040066F6 RID: 26358
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066F7 RID: 26359
		[Tooltip("The playback time")]
		public FsmFloat playbackTime;

		// Token: 0x040066F8 RID: 26360
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;
	}
}
