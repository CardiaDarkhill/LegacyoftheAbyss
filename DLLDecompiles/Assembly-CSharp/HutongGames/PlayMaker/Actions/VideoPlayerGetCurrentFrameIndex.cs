using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011C1 RID: 4545
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the frame index currently being displayed by the player.")]
	public class VideoPlayerGetCurrentFrameIndex : FsmStateAction
	{
		// Token: 0x06007961 RID: 31073 RVA: 0x0024A4F7 File Offset: 0x002486F7
		public override void Reset()
		{
			this.gameObject = null;
			this.frameIndex = null;
			this.everyFrame = false;
		}

		// Token: 0x06007962 RID: 31074 RVA: 0x0024A50E File Offset: 0x0024870E
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007963 RID: 31075 RVA: 0x0024A52A File Offset: 0x0024872A
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007964 RID: 31076 RVA: 0x0024A532 File Offset: 0x00248732
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.frameIndex.Value = (int)this._vp.frame;
				return;
			}
		}

		// Token: 0x06007965 RID: 31077 RVA: 0x0024A55A File Offset: 0x0024875A
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079C2 RID: 31170
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079C3 RID: 31171
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("the frame index currently being displayed by the player.")]
		public FsmInt frameIndex;

		// Token: 0x040079C4 RID: 31172
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040079C5 RID: 31173
		private GameObject go;

		// Token: 0x040079C6 RID: 31174
		private VideoPlayer _vp;
	}
}
