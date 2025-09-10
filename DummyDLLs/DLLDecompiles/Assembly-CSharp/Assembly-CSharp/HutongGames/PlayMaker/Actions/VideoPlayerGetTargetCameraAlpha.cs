using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011CE RID: 4558
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the Overall transparency level of the target camera plane video.")]
	public class VideoPlayerGetTargetCameraAlpha : FsmStateAction
	{
		// Token: 0x060079AF RID: 31151 RVA: 0x0024AFC3 File Offset: 0x002491C3
		public override void Reset()
		{
			this.gameObject = null;
			this.alpha = null;
			this.everyFrame = false;
		}

		// Token: 0x060079B0 RID: 31152 RVA: 0x0024AFDA File Offset: 0x002491DA
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060079B1 RID: 31153 RVA: 0x0024AFF6 File Offset: 0x002491F6
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079B2 RID: 31154 RVA: 0x0024AFFE File Offset: 0x002491FE
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.alpha.Value = this._vp.targetCameraAlpha;
			}
		}

		// Token: 0x060079B3 RID: 31155 RVA: 0x0024B024 File Offset: 0x00249224
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A15 RID: 31253
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A16 RID: 31254
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Overall transparency level")]
		public FsmFloat alpha;

		// Token: 0x04007A17 RID: 31255
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A18 RID: 31256
		private GameObject go;

		// Token: 0x04007A19 RID: 31257
		private VideoPlayer _vp;
	}
}
