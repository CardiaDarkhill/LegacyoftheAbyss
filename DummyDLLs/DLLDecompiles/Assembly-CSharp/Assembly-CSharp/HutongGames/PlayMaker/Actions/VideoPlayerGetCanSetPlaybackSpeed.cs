using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011BB RID: 4539
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether the playback speed can be changed on a VideoPlayer. (Read Only)")]
	public class VideoPlayerGetCanSetPlaybackSpeed : FsmStateAction
	{
		// Token: 0x0600793E RID: 31038 RVA: 0x0024A02B File Offset: 0x0024822B
		public override void Reset()
		{
			this.gameObject = null;
			this.canSetPlaybackSpeed = null;
			this.canSetTimePlaybackSpeed = null;
			this.canNotSetTimePlaybackSpeed = null;
			this.everyFrame = false;
		}

		// Token: 0x0600793F RID: 31039 RVA: 0x0024A050 File Offset: 0x00248250
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007940 RID: 31040 RVA: 0x0024A06C File Offset: 0x0024826C
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007941 RID: 31041 RVA: 0x0024A074 File Offset: 0x00248274
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.canSetPlaybackSpeed.Value = this._vp.canSetPlaybackSpeed;
				base.Fsm.Event(this._vp.canSetTime ? this.canSetTimePlaybackSpeed : this.canNotSetTimePlaybackSpeed);
			}
		}

		// Token: 0x06007942 RID: 31042 RVA: 0x0024A0CB File Offset: 0x002482CB
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x0400799C RID: 31132
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400799D RID: 31133
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetPlaybackSpeed;

		// Token: 0x0400799E RID: 31134
		[Tooltip("Event sent if PlaybackSpeed can be set")]
		public FsmEvent canSetTimePlaybackSpeed;

		// Token: 0x0400799F RID: 31135
		[Tooltip("Event sent if PlaybackSpeed can not be set")]
		public FsmEvent canNotSetTimePlaybackSpeed;

		// Token: 0x040079A0 RID: 31136
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040079A1 RID: 31137
		private GameObject go;

		// Token: 0x040079A2 RID: 31138
		private VideoPlayer _vp;
	}
}
