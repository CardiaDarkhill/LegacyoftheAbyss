using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011DB RID: 4571
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send event from a VideoPlayer when the player preparation is complete.")]
	public class VideoPlayerPreparedCompletedEvent : FsmStateAction
	{
		// Token: 0x060079F7 RID: 31223 RVA: 0x0024B801 File Offset: 0x00249A01
		public override void Reset()
		{
			this.gameObject = null;
			this.OnPreparedCompletedEvent = null;
		}

		// Token: 0x060079F8 RID: 31224 RVA: 0x0024B811 File Offset: 0x00249A11
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.prepareCompleted += this.OnPreparedCompleted;
			}
		}

		// Token: 0x060079F9 RID: 31225 RVA: 0x0024B83E File Offset: 0x00249A3E
		public override void OnExit()
		{
			if (this._vp != null)
			{
				this._vp.prepareCompleted -= this.OnPreparedCompleted;
			}
		}

		// Token: 0x060079FA RID: 31226 RVA: 0x0024B865 File Offset: 0x00249A65
		private void OnPreparedCompleted(VideoPlayer source)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			base.Fsm.Event(this.OnPreparedCompletedEvent);
		}

		// Token: 0x060079FB RID: 31227 RVA: 0x0024B888 File Offset: 0x00249A88
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A52 RID: 31314
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A53 RID: 31315
		[Tooltip("event invoked when the player preparation is complete.")]
		public FsmEvent OnPreparedCompletedEvent;

		// Token: 0x04007A54 RID: 31316
		private GameObject go;

		// Token: 0x04007A55 RID: 31317
		private VideoPlayer _vp;
	}
}
