using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011E4 RID: 4580
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set the video source type. It is valid to set both a VideoClip and a URL in the player. This property controls which one will get used for playback. When setting a new clip or URL, the source will automatically change to make the associated type current.")]
	public class VideoPlayerSetSource : FsmStateAction
	{
		// Token: 0x06007A2B RID: 31275 RVA: 0x0024BDD7 File Offset: 0x00249FD7
		public override void Reset()
		{
			this.gameObject = null;
			this.source = null;
			this.everyFrame = false;
		}

		// Token: 0x06007A2C RID: 31276 RVA: 0x0024BDEE File Offset: 0x00249FEE
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A2D RID: 31277 RVA: 0x0024BE0A File Offset: 0x0024A00A
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007A2E RID: 31278 RVA: 0x0024BE12 File Offset: 0x0024A012
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.source = (VideoSource)this.source.Value;
			}
		}

		// Token: 0x06007A2F RID: 31279 RVA: 0x0024BE3D File Offset: 0x0024A03D
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A7B RID: 31355
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A7C RID: 31356
		[RequiredField]
		[Tooltip("The source type")]
		[ObjectType(typeof(VideoSource))]
		public FsmEnum source;

		// Token: 0x04007A7D RID: 31357
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A7E RID: 31358
		private GameObject go;

		// Token: 0x04007A7F RID: 31359
		private VideoPlayer _vp;
	}
}
