using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011EC RID: 4588
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Sets the VideoClip of a VideoPlayer.")]
	public class VideoPlayerSetVideoClip : FsmStateAction
	{
		// Token: 0x06007A58 RID: 31320 RVA: 0x0024C36B File Offset: 0x0024A56B
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06007A59 RID: 31321 RVA: 0x0024C374 File Offset: 0x0024A574
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.clip = (this.videoClip.Value as VideoClip);
			}
			base.Finish();
		}

		// Token: 0x06007A5A RID: 31322 RVA: 0x0024C3AB File Offset: 0x0024A5AB
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007AA2 RID: 31394
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007AA3 RID: 31395
		[ObjectType(typeof(VideoClip))]
		[Tooltip("The VideoClip.")]
		public FsmObject videoClip;

		// Token: 0x04007AA4 RID: 31396
		private GameObject go;

		// Token: 0x04007AA5 RID: 31397
		private VideoPlayer _vp;
	}
}
