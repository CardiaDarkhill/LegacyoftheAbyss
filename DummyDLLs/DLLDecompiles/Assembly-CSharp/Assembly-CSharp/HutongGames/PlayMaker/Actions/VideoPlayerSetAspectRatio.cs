using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011DD RID: 4573
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Defines how the video content will be stretched to fill the target area.")]
	public class VideoPlayerSetAspectRatio : FsmStateAction
	{
		// Token: 0x06007A03 RID: 31235 RVA: 0x0024B98F File Offset: 0x00249B8F
		public override void Reset()
		{
			this.gameObject = null;
			this.aspectRatio = VideoAspectRatio.NoScaling;
		}

		// Token: 0x06007A04 RID: 31236 RVA: 0x0024B9A9 File Offset: 0x00249BA9
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			base.Finish();
		}

		// Token: 0x06007A05 RID: 31237 RVA: 0x0024B9BD File Offset: 0x00249BBD
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.aspectRatio = (VideoAspectRatio)this.aspectRatio.Value;
			}
		}

		// Token: 0x06007A06 RID: 31238 RVA: 0x0024B9E8 File Offset: 0x00249BE8
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A5A RID: 31322
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A5B RID: 31323
		[RequiredField]
		[Tooltip("The AspectRatio type")]
		[ObjectType(typeof(VideoAspectRatio))]
		public FsmEnum aspectRatio;

		// Token: 0x04007A5C RID: 31324
		private GameObject go;

		// Token: 0x04007A5D RID: 31325
		private VideoPlayer _vp;
	}
}
