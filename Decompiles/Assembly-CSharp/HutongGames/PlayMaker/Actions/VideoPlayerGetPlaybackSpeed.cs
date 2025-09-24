using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011C7 RID: 4551
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the factor by which the basic playback rate will be multiplied.")]
	public class VideoPlayerGetPlaybackSpeed : FsmStateAction
	{
		// Token: 0x06007985 RID: 31109 RVA: 0x0024A9DE File Offset: 0x00248BDE
		public override void Reset()
		{
			this.gameObject = null;
			this.playbackSpeed = null;
			this.everyFrame = false;
		}

		// Token: 0x06007986 RID: 31110 RVA: 0x0024A9F5 File Offset: 0x00248BF5
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007987 RID: 31111 RVA: 0x0024AA11 File Offset: 0x00248C11
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007988 RID: 31112 RVA: 0x0024AA19 File Offset: 0x00248C19
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.playbackSpeed = this._vp.playbackSpeed;
			}
		}

		// Token: 0x06007989 RID: 31113 RVA: 0x0024AA3F File Offset: 0x00248C3F
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079E9 RID: 31209
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079EA RID: 31210
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The factor by which the basic playback rate will be multiplied.")]
		public FsmFloat playbackSpeed;

		// Token: 0x040079EB RID: 31211
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040079EC RID: 31212
		private GameObject go;

		// Token: 0x040079ED RID: 31213
		private VideoPlayer _vp;
	}
}
