using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011B6 RID: 4534
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send the frameReady event from a VideoPlayer when a new frame is ready.")]
	public class VideoPlayerFrameReadyEvent : FsmStateAction
	{
		// Token: 0x06007920 RID: 31008 RVA: 0x00249C47 File Offset: 0x00247E47
		public override void Reset()
		{
			this.gameObject = null;
			this.onFrameReadyEvent = null;
		}

		// Token: 0x06007921 RID: 31009 RVA: 0x00249C57 File Offset: 0x00247E57
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.frameReady += this.OnFrameReady;
			}
		}

		// Token: 0x06007922 RID: 31010 RVA: 0x00249C84 File Offset: 0x00247E84
		public override void OnExit()
		{
			if (this._vp != null)
			{
				this._vp.frameReady -= this.OnFrameReady;
			}
		}

		// Token: 0x06007923 RID: 31011 RVA: 0x00249CAB File Offset: 0x00247EAB
		private void OnFrameReady(VideoPlayer source, long frameIndex)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			Fsm.EventData.IntData = (int)frameIndex;
			base.Fsm.Event(this.onFrameReadyEvent);
		}

		// Token: 0x06007924 RID: 31012 RVA: 0x00249CDA File Offset: 0x00247EDA
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007981 RID: 31105
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007982 RID: 31106
		[Tooltip("event sent when a new frame is ready.")]
		public FsmEvent onFrameReadyEvent;

		// Token: 0x04007983 RID: 31107
		private GameObject go;

		// Token: 0x04007984 RID: 31108
		private VideoPlayer _vp;
	}
}
