using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011C2 RID: 4546
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Number of frames in the current video content of a VideoPlayer. Note that this value may be adjusted as the frameCount changes during playback.")]
	public class VideoPlayerGetFrameCount : FsmStateAction
	{
		// Token: 0x06007967 RID: 31079 RVA: 0x0024A59A File Offset: 0x0024879A
		public override void Reset()
		{
			this.gameObject = null;
			this.frameCount = null;
			this.everyFrame = false;
		}

		// Token: 0x06007968 RID: 31080 RVA: 0x0024A5B1 File Offset: 0x002487B1
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007969 RID: 31081 RVA: 0x0024A5CD File Offset: 0x002487CD
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x0600796A RID: 31082 RVA: 0x0024A5D5 File Offset: 0x002487D5
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.frameCount.Value = (int)this._vp.frameCount;
				return;
			}
		}

		// Token: 0x0600796B RID: 31083 RVA: 0x0024A5FD File Offset: 0x002487FD
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079C7 RID: 31175
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079C8 RID: 31176
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Number of frames in the current video content of a VideoPlayer. Note that this value may be adjusted as the frameCount changes during playback.")]
		public FsmInt frameCount;

		// Token: 0x040079C9 RID: 31177
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040079CA RID: 31178
		private GameObject go;

		// Token: 0x040079CB RID: 31179
		private VideoPlayer _vp;
	}
}
