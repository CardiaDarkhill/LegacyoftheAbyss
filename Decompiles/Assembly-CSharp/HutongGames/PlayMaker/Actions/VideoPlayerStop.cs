using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011F0 RID: 4592
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Stops playback of a VideoPlayer. Pauses the playback and sets the current time to 0.")]
	public class VideoPlayerStop : FsmStateAction
	{
		// Token: 0x06007A6B RID: 31339 RVA: 0x0024C5AE File Offset: 0x0024A7AE
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06007A6C RID: 31340 RVA: 0x0024C5B7 File Offset: 0x0024A7B7
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.Stop();
			}
			base.Finish();
		}

		// Token: 0x06007A6D RID: 31341 RVA: 0x0024C5DE File Offset: 0x0024A7DE
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007AB1 RID: 31409
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007AB2 RID: 31410
		private GameObject go;

		// Token: 0x04007AB3 RID: 31411
		private VideoPlayer _vp;
	}
}
