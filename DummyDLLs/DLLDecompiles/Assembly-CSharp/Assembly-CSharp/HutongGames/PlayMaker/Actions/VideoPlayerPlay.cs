using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011D9 RID: 4569
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Starts playback of a VideoPlayer. Initiates the preparation if not already done, in which case the playback will not start instantly.")]
	public class VideoPlayerPlay : FsmStateAction
	{
		// Token: 0x060079EF RID: 31215 RVA: 0x0024B721 File Offset: 0x00249921
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060079F0 RID: 31216 RVA: 0x0024B72A File Offset: 0x0024992A
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.Play();
			}
			base.Finish();
		}

		// Token: 0x060079F1 RID: 31217 RVA: 0x0024B751 File Offset: 0x00249951
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A4C RID: 31308
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A4D RID: 31309
		private GameObject go;

		// Token: 0x04007A4E RID: 31310
		private VideoPlayer _vp;
	}
}
