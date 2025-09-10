using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011E5 RID: 4581
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set The Camera GameObject to draw to when VideoPlayer.renderMode is set to either Video.VideoTarget.CameraBackPlane or Video.VideoTarget.CameraFrontPlane.")]
	public class VideoPlayerSetTargetCamera : FsmStateAction
	{
		// Token: 0x06007A31 RID: 31281 RVA: 0x0024BE7D File Offset: 0x0024A07D
		public override void Reset()
		{
			this.gameObject = null;
			this.targetCamera = null;
			this.everyFrame = false;
		}

		// Token: 0x06007A32 RID: 31282 RVA: 0x0024BE94 File Offset: 0x0024A094
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A33 RID: 31283 RVA: 0x0024BEB0 File Offset: 0x0024A0B0
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007A34 RID: 31284 RVA: 0x0024BEB8 File Offset: 0x0024A0B8
		private void ExecuteAction()
		{
			if (this._vp != null && this.targetCamera.Value != null)
			{
				this._vp.targetCamera = this.targetCamera.Value.GetComponent<Camera>();
			}
		}

		// Token: 0x06007A35 RID: 31285 RVA: 0x0024BEF6 File Offset: 0x0024A0F6
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A80 RID: 31360
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A81 RID: 31361
		[RequiredField]
		[Tooltip("The Camera GameObject")]
		[CheckForComponent(typeof(Camera))]
		public FsmGameObject targetCamera;

		// Token: 0x04007A82 RID: 31362
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A83 RID: 31363
		private GameObject go;

		// Token: 0x04007A84 RID: 31364
		private VideoPlayer _vp;
	}
}
