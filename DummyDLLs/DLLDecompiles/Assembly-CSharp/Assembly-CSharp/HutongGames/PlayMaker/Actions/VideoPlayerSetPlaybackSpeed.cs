using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011DF RID: 4575
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set the factor by which the basic playback rate will be multiplied.")]
	public class VideoPlayerSetPlaybackSpeed : FsmStateAction
	{
		// Token: 0x06007A0D RID: 31245 RVA: 0x0024BAC1 File Offset: 0x00249CC1
		public override void Reset()
		{
			this.gameObject = null;
			this.playbackSpeed = null;
			this.everyFrame = false;
		}

		// Token: 0x06007A0E RID: 31246 RVA: 0x0024BAD8 File Offset: 0x00249CD8
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A0F RID: 31247 RVA: 0x0024BAF4 File Offset: 0x00249CF4
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007A10 RID: 31248 RVA: 0x0024BAFC File Offset: 0x00249CFC
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.playbackSpeed = this.playbackSpeed.Value;
			}
		}

		// Token: 0x06007A11 RID: 31249 RVA: 0x0024BB22 File Offset: 0x00249D22
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A62 RID: 31330
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A63 RID: 31331
		[RequiredField]
		[Tooltip("The factor by which the basic playback rate will be multiplied.")]
		public FsmFloat playbackSpeed;

		// Token: 0x04007A64 RID: 31332
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A65 RID: 31333
		private GameObject go;

		// Token: 0x04007A66 RID: 31334
		private VideoPlayer _vp;
	}
}
