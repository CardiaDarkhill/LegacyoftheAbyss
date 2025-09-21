using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011B9 RID: 4537
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Number of audio tracks found in the data source currently configured on a videoPlayer. For URL sources, this will only be set once the source preparation is completed. See VideoPlayer.Prepare.")]
	public class VideoPlayerGetAudioTrackCount : FsmStateAction
	{
		// Token: 0x06007932 RID: 31026 RVA: 0x00249E70 File Offset: 0x00248070
		public override void Reset()
		{
			this.gameObject = null;
			this.audioTrackCount = null;
			this.everyFrame = false;
		}

		// Token: 0x06007933 RID: 31027 RVA: 0x00249E87 File Offset: 0x00248087
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007934 RID: 31028 RVA: 0x00249EA3 File Offset: 0x002480A3
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007935 RID: 31029 RVA: 0x00249EAC File Offset: 0x002480AC
		private void ExecuteAction()
		{
			if (!(this._vp != null))
			{
				return;
			}
			if (this._vp.isPrepared)
			{
				base.Fsm.Event(this.isNotPrepared);
				this.audioTrackCount.Value = 0;
				return;
			}
			this.audioTrackCount.Value = (int)this._vp.audioTrackCount;
		}

		// Token: 0x06007936 RID: 31030 RVA: 0x00249F09 File Offset: 0x00248109
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x0400798F RID: 31119
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007990 RID: 31120
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Number of audio tracks found in the data source currently configured")]
		public FsmInt audioTrackCount;

		// Token: 0x04007991 RID: 31121
		[Tooltip("Event sent if source is not prepared")]
		public FsmEvent isNotPrepared;

		// Token: 0x04007992 RID: 31122
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007993 RID: 31123
		private GameObject go;

		// Token: 0x04007994 RID: 31124
		private VideoPlayer _vp;
	}
}
