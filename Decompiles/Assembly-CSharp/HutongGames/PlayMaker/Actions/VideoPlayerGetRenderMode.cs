using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011C9 RID: 4553
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get where the video content will be drawn")]
	public class VideoPlayerGetRenderMode : FsmStateAction
	{
		// Token: 0x06007991 RID: 31121 RVA: 0x0024AB92 File Offset: 0x00248D92
		public override void Reset()
		{
			this.gameObject = null;
			this.renderMode = null;
			this.everyFrame = false;
		}

		// Token: 0x06007992 RID: 31122 RVA: 0x0024ABA9 File Offset: 0x00248DA9
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007993 RID: 31123 RVA: 0x0024ABC5 File Offset: 0x00248DC5
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007994 RID: 31124 RVA: 0x0024ABCD File Offset: 0x00248DCD
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.renderMode.Value = this._vp.renderMode;
			}
		}

		// Token: 0x06007995 RID: 31125 RVA: 0x0024ABF8 File Offset: 0x00248DF8
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079F6 RID: 31222
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079F7 RID: 31223
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("where the video content will be drawn")]
		[ObjectType(typeof(VideoRenderMode))]
		public FsmEnum renderMode;

		// Token: 0x040079F8 RID: 31224
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040079F9 RID: 31225
		private GameObject go;

		// Token: 0x040079FA RID: 31226
		private VideoPlayer _vp;
	}
}
