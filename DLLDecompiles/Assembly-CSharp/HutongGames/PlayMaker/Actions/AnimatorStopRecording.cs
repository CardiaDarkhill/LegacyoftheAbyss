using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE0 RID: 3552
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Stops the animator record mode. It will lock the recording buffer's contents in its current state. The data get saved for subsequent playback with StartPlayback.")]
	public class AnimatorStopRecording : ComponentAction<Animator>
	{
		// Token: 0x060066B5 RID: 26293 RVA: 0x0020839F File Offset: 0x0020659F
		public override void Reset()
		{
			this.gameObject = null;
			this.recorderStartTime = null;
			this.recorderStopTime = null;
		}

		// Token: 0x060066B6 RID: 26294 RVA: 0x002083B8 File Offset: 0x002065B8
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.StopRecording();
				this.recorderStartTime.Value = this.cachedComponent.recorderStartTime;
				this.recorderStopTime.Value = this.cachedComponent.recorderStopTime;
			}
			base.Finish();
		}

		// Token: 0x0400660C RID: 26124
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400660D RID: 26125
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The recorder StartTime")]
		public FsmFloat recorderStartTime;

		// Token: 0x0400660E RID: 26126
		[UIHint(UIHint.Variable)]
		[Tooltip("The recorder StopTime")]
		public FsmFloat recorderStopTime;
	}
}
