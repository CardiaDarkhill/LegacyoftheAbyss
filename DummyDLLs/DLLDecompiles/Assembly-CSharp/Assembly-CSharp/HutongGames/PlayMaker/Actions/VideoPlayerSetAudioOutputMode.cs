using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011DE RID: 4574
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Defines Destination for the audio embedded in the video.")]
	public class VideoPlayerSetAudioOutputMode : FsmStateAction
	{
		// Token: 0x06007A08 RID: 31240 RVA: 0x0024BA28 File Offset: 0x00249C28
		public override void Reset()
		{
			this.gameObject = null;
			this.audioOutputMode = VideoAudioOutputMode.AudioSource;
		}

		// Token: 0x06007A09 RID: 31241 RVA: 0x0024BA42 File Offset: 0x00249C42
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			base.Finish();
		}

		// Token: 0x06007A0A RID: 31242 RVA: 0x0024BA56 File Offset: 0x00249C56
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.audioOutputMode = (VideoAudioOutputMode)this.audioOutputMode.Value;
			}
		}

		// Token: 0x06007A0B RID: 31243 RVA: 0x0024BA81 File Offset: 0x00249C81
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A5E RID: 31326
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A5F RID: 31327
		[RequiredField]
		[Tooltip("The AudioOutputMode type")]
		[ObjectType(typeof(VideoAudioOutputMode))]
		public FsmEnum audioOutputMode;

		// Token: 0x04007A60 RID: 31328
		private GameObject go;

		// Token: 0x04007A61 RID: 31329
		private VideoPlayer _vp;
	}
}
