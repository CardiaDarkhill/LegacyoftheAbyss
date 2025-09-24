using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011D8 RID: 4568
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Pauses the playback of a VideoPlayer.")]
	public class VideoPlayerPause : FsmStateAction
	{
		// Token: 0x060079EB RID: 31211 RVA: 0x0024B6B1 File Offset: 0x002498B1
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060079EC RID: 31212 RVA: 0x0024B6BA File Offset: 0x002498BA
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.Pause();
			}
			base.Finish();
		}

		// Token: 0x060079ED RID: 31213 RVA: 0x0024B6E1 File Offset: 0x002498E1
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A49 RID: 31305
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A4A RID: 31306
		private GameObject go;

		// Token: 0x04007A4B RID: 31307
		private VideoPlayer _vp;
	}
}
