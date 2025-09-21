using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011D6 RID: 4566
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether the player will wait for the first frame to be loaded into the texture before starting playback when VideoPlayer.playOnAwake is on")]
	public class VideoPlayerGetWaitForFirstFrame : FsmStateAction
	{
		// Token: 0x060079DF RID: 31199 RVA: 0x0024B4D6 File Offset: 0x002496D6
		public override void Reset()
		{
			this.gameObject = null;
			this.isWaitingForFirstFrame = null;
			this.isWaitingForFirstFrameEvent = null;
			this.isNotWaitingForFirstFrameEvent = null;
			this.everyframe = false;
		}

		// Token: 0x060079E0 RID: 31200 RVA: 0x0024B4FB File Offset: 0x002496FB
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x060079E1 RID: 31201 RVA: 0x0024B517 File Offset: 0x00249717
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079E2 RID: 31202 RVA: 0x0024B520 File Offset: 0x00249720
		private void ExecuteAction()
		{
			if (this._vp == null)
			{
				return;
			}
			if (this._vp.waitForFirstFrame)
			{
				this.isWaitingForFirstFrame.Value = true;
				if (this._isWaitingForFirstFrame != 1)
				{
					base.Fsm.Event(this.isWaitingForFirstFrameEvent);
				}
				this._isWaitingForFirstFrame = 1;
				return;
			}
			this.isWaitingForFirstFrame.Value = false;
			if (this._isWaitingForFirstFrame != 0)
			{
				base.Fsm.Event(this.isNotWaitingForFirstFrameEvent);
			}
			this._isWaitingForFirstFrame = 0;
		}

		// Token: 0x060079E3 RID: 31203 RVA: 0x0024B5A3 File Offset: 0x002497A3
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A3D RID: 31293
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A3E RID: 31294
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isWaitingForFirstFrame;

		// Token: 0x04007A3F RID: 31295
		[Tooltip("Event sent if content will wait for first frame")]
		public FsmEvent isWaitingForFirstFrameEvent;

		// Token: 0x04007A40 RID: 31296
		[Tooltip("Event sent if content will not wait for first frame")]
		public FsmEvent isNotWaitingForFirstFrameEvent;

		// Token: 0x04007A41 RID: 31297
		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		// Token: 0x04007A42 RID: 31298
		private GameObject go;

		// Token: 0x04007A43 RID: 31299
		private VideoPlayer _vp;

		// Token: 0x04007A44 RID: 31300
		private int _isWaitingForFirstFrame = -1;
	}
}
