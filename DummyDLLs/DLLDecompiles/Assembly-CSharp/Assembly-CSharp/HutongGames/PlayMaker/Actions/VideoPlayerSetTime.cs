using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011E9 RID: 4585
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Sets the time value of a VideoPlayer.")]
	public class VideoPlayerSetTime : FsmStateAction
	{
		// Token: 0x06007A48 RID: 31304 RVA: 0x0024C108 File Offset: 0x0024A308
		public override void Reset()
		{
			this.gameObject = null;
			this.time = null;
			this.canNotSetTime = null;
			this.everyFrame = false;
		}

		// Token: 0x06007A49 RID: 31305 RVA: 0x0024C128 File Offset: 0x0024A328
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null && !this._vp.canSetTime)
			{
				base.Fsm.Event(this.canNotSetTime);
			}
			else
			{
				this.ExecuteAction();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A4A RID: 31306 RVA: 0x0024C17D File Offset: 0x0024A37D
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007A4B RID: 31307 RVA: 0x0024C185 File Offset: 0x0024A385
		private void ExecuteAction()
		{
			if (this._vp != null && this._vp.canSetTime)
			{
				this._vp.time = (double)this.time.Value;
			}
		}

		// Token: 0x06007A4C RID: 31308 RVA: 0x0024C1B9 File Offset: 0x0024A3B9
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A93 RID: 31379
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A94 RID: 31380
		[RequiredField]
		[Tooltip("The time Value")]
		public FsmFloat time;

		// Token: 0x04007A95 RID: 31381
		[Tooltip("Event sent if time can not be set")]
		public FsmEvent canNotSetTime;

		// Token: 0x04007A96 RID: 31382
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A97 RID: 31383
		private GameObject go;

		// Token: 0x04007A98 RID: 31384
		private VideoPlayer _vp;
	}
}
