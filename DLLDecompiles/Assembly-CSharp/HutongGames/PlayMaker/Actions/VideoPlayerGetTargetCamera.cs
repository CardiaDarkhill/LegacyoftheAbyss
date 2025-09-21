using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011CD RID: 4557
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the Camera GameObject to draw to when VideoPlayer.renderMode is set to either Video.VideoTarget.CameraBackPlane or Video.VideoTarget.CameraFrontPlane.")]
	public class VideoPlayerGetTargetCamera : FsmStateAction
	{
		// Token: 0x060079A9 RID: 31145 RVA: 0x0024AEF0 File Offset: 0x002490F0
		public override void Reset()
		{
			this.gameObject = null;
			this.targetCamera = null;
			this.everyFrame = false;
		}

		// Token: 0x060079AA RID: 31146 RVA: 0x0024AF07 File Offset: 0x00249107
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060079AB RID: 31147 RVA: 0x0024AF23 File Offset: 0x00249123
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079AC RID: 31148 RVA: 0x0024AF2C File Offset: 0x0024912C
		private void ExecuteAction()
		{
			if (!(this._vp != null))
			{
				return;
			}
			if (this._vp.targetCamera != null)
			{
				this.targetCamera.Value = this._vp.targetCamera.gameObject;
				return;
			}
			this.targetCamera.Value = null;
		}

		// Token: 0x060079AD RID: 31149 RVA: 0x0024AF83 File Offset: 0x00249183
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A10 RID: 31248
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A11 RID: 31249
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Camera GameObject")]
		public FsmGameObject targetCamera;

		// Token: 0x04007A12 RID: 31250
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A13 RID: 31251
		private GameObject go;

		// Token: 0x04007A14 RID: 31252
		private VideoPlayer _vp;
	}
}
