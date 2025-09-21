using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011B4 RID: 4532
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send error event from a VideoPlayer.")]
	public class VideoPlayerErrorEvent : FsmStateAction
	{
		// Token: 0x06007914 RID: 30996 RVA: 0x00249AAE File Offset: 0x00247CAE
		public override void Reset()
		{
			this.gameObject = null;
			this.onErrorEvent = null;
		}

		// Token: 0x06007915 RID: 30997 RVA: 0x00249ABE File Offset: 0x00247CBE
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.errorReceived += this.OnErrorReceived;
			}
		}

		// Token: 0x06007916 RID: 30998 RVA: 0x00249AEB File Offset: 0x00247CEB
		public override void OnExit()
		{
			if (this._vp != null)
			{
				this._vp.errorReceived -= this.OnErrorReceived;
			}
		}

		// Token: 0x06007917 RID: 30999 RVA: 0x00249B12 File Offset: 0x00247D12
		private void OnErrorReceived(VideoPlayer source, string errorMessage)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			Fsm.EventData.StringData = errorMessage;
			base.Fsm.Event(this.onErrorEvent);
		}

		// Token: 0x06007918 RID: 31000 RVA: 0x00249B40 File Offset: 0x00247D40
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007979 RID: 31097
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400797A RID: 31098
		[Tooltip("event sent when VideoPlayer throws an error")]
		public FsmEvent onErrorEvent;

		// Token: 0x0400797B RID: 31099
		private GameObject go;

		// Token: 0x0400797C RID: 31100
		private VideoPlayer _vp;
	}
}
