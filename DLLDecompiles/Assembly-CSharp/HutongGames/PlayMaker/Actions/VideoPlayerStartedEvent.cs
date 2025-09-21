using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011EE RID: 4590
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send the started event from a VideoPlayer.")]
	public class VideoPlayerStartedEvent : FsmStateAction
	{
		// Token: 0x06007A61 RID: 31329 RVA: 0x0024C476 File Offset: 0x0024A676
		public override void Reset()
		{
			this.gameObject = null;
			this.onStartedEvent = null;
		}

		// Token: 0x06007A62 RID: 31330 RVA: 0x0024C486 File Offset: 0x0024A686
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.started += this.OnStarted;
			}
		}

		// Token: 0x06007A63 RID: 31331 RVA: 0x0024C4B3 File Offset: 0x0024A6B3
		public override void OnExit()
		{
			if (this._vp != null)
			{
				this._vp.started -= this.OnStarted;
			}
		}

		// Token: 0x06007A64 RID: 31332 RVA: 0x0024C4DA File Offset: 0x0024A6DA
		private void OnStarted(VideoPlayer source)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			base.Fsm.Event(this.onStartedEvent);
		}

		// Token: 0x06007A65 RID: 31333 RVA: 0x0024C4FD File Offset: 0x0024A6FD
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007AAA RID: 31402
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007AAB RID: 31403
		[Tooltip("event sent when VideoPlayer started")]
		public FsmEvent onStartedEvent;

		// Token: 0x04007AAC RID: 31404
		private GameObject go;

		// Token: 0x04007AAD RID: 31405
		private VideoPlayer _vp;
	}
}
