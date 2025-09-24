using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDE RID: 3550
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the animator in recording mode, and allocates a circular buffer of size frameCount. After this call, the recorder starts collecting up to frameCount frames in the buffer. Note it is not possible to start playback until a call to StopRecording is made")]
	public class AnimatorStartRecording : ComponentAction<Animator>
	{
		// Token: 0x060066AF RID: 26287 RVA: 0x0020830E File Offset: 0x0020650E
		public override void Reset()
		{
			this.gameObject = null;
			this.frameCount = 0;
		}

		// Token: 0x060066B0 RID: 26288 RVA: 0x00208323 File Offset: 0x00206523
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.StartRecording(this.frameCount.Value);
			}
			base.Finish();
		}

		// Token: 0x04006609 RID: 26121
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400660A RID: 26122
		[RequiredField]
		[Tooltip("The number of frames (updates) that will be recorded. If frameCount is 0, the recording will continue until the user calls StopRecording. The maximum value for frameCount is 10000.")]
		public FsmInt frameCount;
	}
}
