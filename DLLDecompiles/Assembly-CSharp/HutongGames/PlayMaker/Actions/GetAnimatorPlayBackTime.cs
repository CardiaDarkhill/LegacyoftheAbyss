using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E00 RID: 3584
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the playback position in the recording buffer. When in playback mode (use  AnimatorStartPlayback), this value is used for controlling the current playback position in the buffer (in seconds). The value can range between recordingStartTime and recordingStopTime See Also: StartPlayback, StopPlayback.")]
	public class GetAnimatorPlayBackTime : ComponentAction<Animator>
	{
		// Token: 0x06006755 RID: 26453 RVA: 0x00209D8D File Offset: 0x00207F8D
		public override void Reset()
		{
			this.gameObject = null;
			this.playBackTime = null;
			this.everyFrame = false;
		}

		// Token: 0x06006756 RID: 26454 RVA: 0x00209DA4 File Offset: 0x00207FA4
		public override void OnEnter()
		{
			this.GetPlayBackTime();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006757 RID: 26455 RVA: 0x00209DBA File Offset: 0x00207FBA
		public override void OnUpdate()
		{
			this.GetPlayBackTime();
		}

		// Token: 0x06006758 RID: 26456 RVA: 0x00209DC2 File Offset: 0x00207FC2
		private void GetPlayBackTime()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.playBackTime.Value = this.cachedComponent.playbackTime;
			}
		}

		// Token: 0x040066A2 RID: 26274
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066A3 RID: 26275
		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The playBack time of the animator.")]
		public FsmFloat playBackTime;

		// Token: 0x040066A4 RID: 26276
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;
	}
}
