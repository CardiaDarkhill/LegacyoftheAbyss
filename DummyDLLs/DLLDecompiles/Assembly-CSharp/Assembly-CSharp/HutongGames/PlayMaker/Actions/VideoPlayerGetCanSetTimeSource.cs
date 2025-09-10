using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011BE RID: 4542
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether the time source followed by the video player can be changed. (Read Only)")]
	public class VideoPlayerGetCanSetTimeSource : FsmStateAction
	{
		// Token: 0x06007950 RID: 31056 RVA: 0x0024A2EF File Offset: 0x002484EF
		public override void Reset()
		{
			this.gameObject = null;
			this.canSetTimeSource = null;
			this.canSetTimeSourceEvent = null;
			this.canNotSetTimeSourceEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06007951 RID: 31057 RVA: 0x0024A314 File Offset: 0x00248514
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007952 RID: 31058 RVA: 0x0024A330 File Offset: 0x00248530
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007953 RID: 31059 RVA: 0x0024A338 File Offset: 0x00248538
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.canSetTimeSource.Value = this._vp.canSetTimeUpdateMode;
				base.Fsm.Event(this._vp.canSetTime ? this.canSetTimeSourceEvent : this.canNotSetTimeSourceEvent);
			}
		}

		// Token: 0x06007954 RID: 31060 RVA: 0x0024A38F File Offset: 0x0024858F
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079B2 RID: 31154
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079B3 RID: 31155
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetTimeSource;

		// Token: 0x040079B4 RID: 31156
		[Tooltip("Event sent if timeSource can be set")]
		public FsmEvent canSetTimeSourceEvent;

		// Token: 0x040079B5 RID: 31157
		[Tooltip("Event sent if timeSource can not be set")]
		public FsmEvent canNotSetTimeSourceEvent;

		// Token: 0x040079B6 RID: 31158
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040079B7 RID: 31159
		private GameObject go;

		// Token: 0x040079B8 RID: 31160
		private VideoPlayer _vp;
	}
}
