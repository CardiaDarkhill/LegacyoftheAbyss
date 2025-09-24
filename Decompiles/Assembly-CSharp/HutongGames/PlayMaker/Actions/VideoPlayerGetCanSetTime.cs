using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011BD RID: 4541
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether current time can be changed using the time or timeFrames property on a VideoPlayer. (Read Only)")]
	public class VideoPlayerGetCanSetTime : FsmStateAction
	{
		// Token: 0x0600794A RID: 31050 RVA: 0x0024A20F File Offset: 0x0024840F
		public override void Reset()
		{
			this.gameObject = null;
			this.canSetTime = null;
			this.canSetTimeEvent = null;
			this.canNotSetTimeEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x0600794B RID: 31051 RVA: 0x0024A234 File Offset: 0x00248434
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600794C RID: 31052 RVA: 0x0024A250 File Offset: 0x00248450
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x0600794D RID: 31053 RVA: 0x0024A258 File Offset: 0x00248458
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.canSetTime.Value = this._vp.canSetTime;
				base.Fsm.Event(this._vp.canSetTime ? this.canSetTimeEvent : this.canNotSetTimeEvent);
			}
		}

		// Token: 0x0600794E RID: 31054 RVA: 0x0024A2AF File Offset: 0x002484AF
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079AB RID: 31147
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079AC RID: 31148
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetTime;

		// Token: 0x040079AD RID: 31149
		[Tooltip("Event sent if time can be set")]
		public FsmEvent canSetTimeEvent;

		// Token: 0x040079AE RID: 31150
		[Tooltip("Event sent if time can not be set")]
		public FsmEvent canNotSetTimeEvent;

		// Token: 0x040079AF RID: 31151
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040079B0 RID: 31152
		private GameObject go;

		// Token: 0x040079B1 RID: 31153
		private VideoPlayer _vp;
	}
}
