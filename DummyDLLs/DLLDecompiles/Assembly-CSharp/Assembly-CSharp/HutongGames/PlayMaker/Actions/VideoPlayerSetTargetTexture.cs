using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011E8 RID: 4584
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set the RenderTexture to draw to when VideoPlayer.renderMode is set to Video.VideoTarget.RenderTexture.")]
	public class VideoPlayerSetTargetTexture : FsmStateAction
	{
		// Token: 0x06007A43 RID: 31299 RVA: 0x0024C078 File Offset: 0x0024A278
		public override void Reset()
		{
			this.gameObject = null;
			this.targetTexture = null;
		}

		// Token: 0x06007A44 RID: 31300 RVA: 0x0024C088 File Offset: 0x0024A288
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			base.Finish();
		}

		// Token: 0x06007A45 RID: 31301 RVA: 0x0024C09C File Offset: 0x0024A29C
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.targetTexture = (RenderTexture)this.targetTexture.Value;
				return;
			}
		}

		// Token: 0x06007A46 RID: 31302 RVA: 0x0024C0C8 File Offset: 0x0024A2C8
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A8F RID: 31375
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A90 RID: 31376
		[RequiredField]
		[Tooltip("The RenderTexture")]
		public FsmTexture targetTexture;

		// Token: 0x04007A91 RID: 31377
		private GameObject go;

		// Token: 0x04007A92 RID: 31378
		private VideoPlayer _vp;
	}
}
