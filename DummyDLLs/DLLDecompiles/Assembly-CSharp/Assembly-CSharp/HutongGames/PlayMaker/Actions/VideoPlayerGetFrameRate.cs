using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011C3 RID: 4547
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the frame rate of the clip or URL in frames/second.")]
	public class VideoPlayerGetFrameRate : FsmStateAction
	{
		// Token: 0x0600796D RID: 31085 RVA: 0x0024A63D File Offset: 0x0024883D
		public override void Reset()
		{
			this.gameObject = null;
			this.frameRate = null;
			this.everyFrame = false;
		}

		// Token: 0x0600796E RID: 31086 RVA: 0x0024A654 File Offset: 0x00248854
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600796F RID: 31087 RVA: 0x0024A670 File Offset: 0x00248870
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007970 RID: 31088 RVA: 0x0024A678 File Offset: 0x00248878
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.frameRate.Value = this._vp.frameRate;
				return;
			}
		}

		// Token: 0x06007971 RID: 31089 RVA: 0x0024A69F File Offset: 0x0024889F
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079CC RID: 31180
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079CD RID: 31181
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The frame rate of the clip or URL in frames/second.")]
		public FsmFloat frameRate;

		// Token: 0x040079CE RID: 31182
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040079CF RID: 31183
		private GameObject go;

		// Token: 0x040079D0 RID: 31184
		private VideoPlayer _vp;
	}
}
