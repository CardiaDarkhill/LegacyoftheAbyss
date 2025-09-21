using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011E7 RID: 4583
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set the Material texture property which is targeted when VideoPlayer.renderMode is set to Video.VideoTarget.MaterialOverride.")]
	public class VideoPlayerSetTargetMaterialProperty : FsmStateAction
	{
		// Token: 0x06007A3D RID: 31293 RVA: 0x0024BFD7 File Offset: 0x0024A1D7
		public override void Reset()
		{
			this.gameObject = null;
			this.property = null;
			this.everyFrame = false;
		}

		// Token: 0x06007A3E RID: 31294 RVA: 0x0024BFEE File Offset: 0x0024A1EE
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A3F RID: 31295 RVA: 0x0024C00A File Offset: 0x0024A20A
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007A40 RID: 31296 RVA: 0x0024C012 File Offset: 0x0024A212
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.targetMaterialProperty = this.property.Value;
			}
		}

		// Token: 0x06007A41 RID: 31297 RVA: 0x0024C038 File Offset: 0x0024A238
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A8A RID: 31370
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A8B RID: 31371
		[RequiredField]
		[Tooltip("The Material texture property")]
		public FsmString property;

		// Token: 0x04007A8C RID: 31372
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A8D RID: 31373
		private GameObject go;

		// Token: 0x04007A8E RID: 31374
		private VideoPlayer _vp;
	}
}
