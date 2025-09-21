using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011B8 RID: 4536
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the Destination for the audio embedded in the video.")]
	public class VideoPlayerGetAudioOutputMode : FsmStateAction
	{
		// Token: 0x0600792C RID: 31020 RVA: 0x00249DC0 File Offset: 0x00247FC0
		public override void Reset()
		{
			this.gameObject = null;
			this.audioOutputMode = VideoAudioOutputMode.AudioSource;
			this.everyFrame = false;
		}

		// Token: 0x0600792D RID: 31021 RVA: 0x00249DE1 File Offset: 0x00247FE1
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600792E RID: 31022 RVA: 0x00249DFD File Offset: 0x00247FFD
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x0600792F RID: 31023 RVA: 0x00249E05 File Offset: 0x00248005
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.audioOutputMode.Value = this._vp.audioOutputMode;
			}
		}

		// Token: 0x06007930 RID: 31024 RVA: 0x00249E30 File Offset: 0x00248030
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x0400798A RID: 31114
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400798B RID: 31115
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The AudioOutputMode type")]
		[ObjectType(typeof(VideoAudioOutputMode))]
		public FsmEnum audioOutputMode;

		// Token: 0x0400798C RID: 31116
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400798D RID: 31117
		private GameObject go;

		// Token: 0x0400798E RID: 31118
		private VideoPlayer _vp;
	}
}
