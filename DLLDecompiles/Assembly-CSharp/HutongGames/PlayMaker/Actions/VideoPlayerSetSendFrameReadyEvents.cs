using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011E2 RID: 4578
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set Whether frameReady events are enabled")]
	public class VideoPlayerSetSendFrameReadyEvents : FsmStateAction
	{
		// Token: 0x06007A1F RID: 31263 RVA: 0x0024BCAA File Offset: 0x00249EAA
		public override void Reset()
		{
			this.gameObject = null;
			this.sendFrameReadyEvents = null;
		}

		// Token: 0x06007A20 RID: 31264 RVA: 0x0024BCBA File Offset: 0x00249EBA
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
		}

		// Token: 0x06007A21 RID: 31265 RVA: 0x0024BCC8 File Offset: 0x00249EC8
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007A22 RID: 31266 RVA: 0x0024BCD0 File Offset: 0x00249ED0
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.sendFrameReadyEvents = this.sendFrameReadyEvents.Value;
			}
		}

		// Token: 0x06007A23 RID: 31267 RVA: 0x0024BCF6 File Offset: 0x00249EF6
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A71 RID: 31345
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A72 RID: 31346
		[RequiredField]
		[Tooltip("The Value")]
		public FsmBool sendFrameReadyEvents;

		// Token: 0x04007A73 RID: 31347
		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		// Token: 0x04007A74 RID: 31348
		private GameObject go;

		// Token: 0x04007A75 RID: 31349
		private VideoPlayer _vp;
	}
}
