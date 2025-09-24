using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011CA RID: 4554
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether frameReady events are enabled")]
	public class VideoPlayerGetSendFrameReadyEvents : FsmStateAction
	{
		// Token: 0x06007997 RID: 31127 RVA: 0x0024AC38 File Offset: 0x00248E38
		public override void Reset()
		{
			this.gameObject = null;
			this.isSendingFrameReadyEvents = null;
			this.isSendingFrameReadyEventsEvent = null;
			this.isNotSendingFrameReadyEventsEvent = null;
		}

		// Token: 0x06007998 RID: 31128 RVA: 0x0024AC56 File Offset: 0x00248E56
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
		}

		// Token: 0x06007999 RID: 31129 RVA: 0x0024AC64 File Offset: 0x00248E64
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x0600799A RID: 31130 RVA: 0x0024AC6C File Offset: 0x00248E6C
		private void ExecuteAction()
		{
			if (this._vp == null)
			{
				return;
			}
			if (this._vp.sendFrameReadyEvents)
			{
				this.isSendingFrameReadyEvents.Value = true;
				if (this._isSendingFrameReadyEvents != 1)
				{
					base.Fsm.Event(this.isSendingFrameReadyEventsEvent);
				}
				this._isSendingFrameReadyEvents = 1;
				return;
			}
			this.isSendingFrameReadyEvents.Value = false;
			if (this._isSendingFrameReadyEvents != 0)
			{
				base.Fsm.Event(this.isNotSendingFrameReadyEventsEvent);
			}
			this._isSendingFrameReadyEvents = 0;
		}

		// Token: 0x0600799B RID: 31131 RVA: 0x0024ACEF File Offset: 0x00248EEF
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079FB RID: 31227
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079FC RID: 31228
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isSendingFrameReadyEvents;

		// Token: 0x040079FD RID: 31229
		[Tooltip("Event sent if frameReady events are sent")]
		public FsmEvent isSendingFrameReadyEventsEvent;

		// Token: 0x040079FE RID: 31230
		[Tooltip("Event sent if frameReady events are not sent")]
		public FsmEvent isNotSendingFrameReadyEventsEvent;

		// Token: 0x040079FF RID: 31231
		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		// Token: 0x04007A00 RID: 31232
		private GameObject go;

		// Token: 0x04007A01 RID: 31233
		private VideoPlayer _vp;

		// Token: 0x04007A02 RID: 31234
		private int _isSendingFrameReadyEvents = -1;
	}
}
