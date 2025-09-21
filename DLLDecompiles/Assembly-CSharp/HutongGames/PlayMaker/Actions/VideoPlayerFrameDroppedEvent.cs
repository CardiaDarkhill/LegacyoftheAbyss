using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011B5 RID: 4533
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send the framedropped event from a VideoPlayer when playback detects it does not keep up with the time source..")]
	public class VideoPlayerFrameDroppedEvent : FsmStateAction
	{
		// Token: 0x0600791A RID: 31002 RVA: 0x00249B80 File Offset: 0x00247D80
		public override void Reset()
		{
			this.gameObject = null;
			this.onFrameDroppedEvent = null;
		}

		// Token: 0x0600791B RID: 31003 RVA: 0x00249B90 File Offset: 0x00247D90
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.frameDropped += this.OnFrameDropped;
			}
		}

		// Token: 0x0600791C RID: 31004 RVA: 0x00249BBD File Offset: 0x00247DBD
		public override void OnExit()
		{
			if (this._vp != null)
			{
				this._vp.frameDropped -= this.OnFrameDropped;
			}
		}

		// Token: 0x0600791D RID: 31005 RVA: 0x00249BE4 File Offset: 0x00247DE4
		private void OnFrameDropped(VideoPlayer source)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			base.Fsm.Event(this.onFrameDroppedEvent);
		}

		// Token: 0x0600791E RID: 31006 RVA: 0x00249C07 File Offset: 0x00247E07
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x0400797D RID: 31101
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400797E RID: 31102
		[Tooltip("event sent when playback detects it does not keep up with the time source.")]
		public FsmEvent onFrameDroppedEvent;

		// Token: 0x0400797F RID: 31103
		private GameObject go;

		// Token: 0x04007980 RID: 31104
		private VideoPlayer _vp;
	}
}
