using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011E6 RID: 4582
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set the Overall transparency level of the target camera plane video.")]
	public class VideoPlayerSetTargetCameraAlpha : FsmStateAction
	{
		// Token: 0x06007A37 RID: 31287 RVA: 0x0024BF36 File Offset: 0x0024A136
		public override void Reset()
		{
			this.gameObject = null;
			this.alpha = null;
			this.everyFrame = false;
		}

		// Token: 0x06007A38 RID: 31288 RVA: 0x0024BF4D File Offset: 0x0024A14D
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A39 RID: 31289 RVA: 0x0024BF69 File Offset: 0x0024A169
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007A3A RID: 31290 RVA: 0x0024BF71 File Offset: 0x0024A171
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.targetCameraAlpha = this.alpha.Value;
			}
		}

		// Token: 0x06007A3B RID: 31291 RVA: 0x0024BF97 File Offset: 0x0024A197
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A85 RID: 31365
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A86 RID: 31366
		[RequiredField]
		[Tooltip("The Overall transparency level")]
		public FsmFloat alpha;

		// Token: 0x04007A87 RID: 31367
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A88 RID: 31368
		private GameObject go;

		// Token: 0x04007A89 RID: 31369
		private VideoPlayer _vp;
	}
}
