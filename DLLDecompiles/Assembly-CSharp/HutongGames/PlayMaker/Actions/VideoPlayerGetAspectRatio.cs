using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011B7 RID: 4535
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get how the video content will be stretched to fill the target area.")]
	public class VideoPlayerGetAspectRatio : FsmStateAction
	{
		// Token: 0x06007926 RID: 31014 RVA: 0x00249D1A File Offset: 0x00247F1A
		public override void Reset()
		{
			this.gameObject = null;
			this.aspectRatio = null;
			this.everyFrame = false;
		}

		// Token: 0x06007927 RID: 31015 RVA: 0x00249D31 File Offset: 0x00247F31
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007928 RID: 31016 RVA: 0x00249D4D File Offset: 0x00247F4D
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007929 RID: 31017 RVA: 0x00249D55 File Offset: 0x00247F55
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.aspectRatio.Value = this._vp.aspectRatio;
			}
		}

		// Token: 0x0600792A RID: 31018 RVA: 0x00249D80 File Offset: 0x00247F80
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007985 RID: 31109
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007986 RID: 31110
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The AspectRatio type")]
		[ObjectType(typeof(VideoAspectRatio))]
		public FsmEnum aspectRatio;

		// Token: 0x04007987 RID: 31111
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007988 RID: 31112
		private GameObject go;

		// Token: 0x04007989 RID: 31113
		private VideoPlayer _vp;
	}
}
