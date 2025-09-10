using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011DA RID: 4570
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Initiates playback engine preparation of a VideoPlayer. The preparation consists of reserving the resources needed for playback, and preloading some or all of the content to be played. After this is done, frames can be received immediately.")]
	public class VideoPlayerPrepare : FsmStateAction
	{
		// Token: 0x060079F3 RID: 31219 RVA: 0x0024B791 File Offset: 0x00249991
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060079F4 RID: 31220 RVA: 0x0024B79A File Offset: 0x0024999A
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.Prepare();
			}
			base.Finish();
		}

		// Token: 0x060079F5 RID: 31221 RVA: 0x0024B7C1 File Offset: 0x002499C1
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A4F RID: 31311
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A50 RID: 31312
		private GameObject go;

		// Token: 0x04007A51 RID: 31313
		private VideoPlayer _vp;
	}
}
