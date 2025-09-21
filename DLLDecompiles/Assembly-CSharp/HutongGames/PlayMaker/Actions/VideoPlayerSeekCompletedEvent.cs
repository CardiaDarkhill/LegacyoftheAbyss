using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011DC RID: 4572
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send event from a VideoPlayer after a seek operation completes..")]
	public class VideoPlayerSeekCompletedEvent : FsmStateAction
	{
		// Token: 0x060079FD RID: 31229 RVA: 0x0024B8C8 File Offset: 0x00249AC8
		public override void Reset()
		{
			this.gameObject = null;
			this.OnSeekCompletedEvent = null;
		}

		// Token: 0x060079FE RID: 31230 RVA: 0x0024B8D8 File Offset: 0x00249AD8
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.seekCompleted += this.OnSeekCompleted;
			}
		}

		// Token: 0x060079FF RID: 31231 RVA: 0x0024B905 File Offset: 0x00249B05
		public override void OnExit()
		{
			if (this._vp != null)
			{
				this._vp.seekCompleted -= this.OnSeekCompleted;
			}
		}

		// Token: 0x06007A00 RID: 31232 RVA: 0x0024B92C File Offset: 0x00249B2C
		private void OnSeekCompleted(VideoPlayer source)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			base.Fsm.Event(this.OnSeekCompletedEvent);
		}

		// Token: 0x06007A01 RID: 31233 RVA: 0x0024B94F File Offset: 0x00249B4F
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A56 RID: 31318
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A57 RID: 31319
		[Tooltip("event invoked when the player preparation is complete.")]
		public FsmEvent OnSeekCompletedEvent;

		// Token: 0x04007A58 RID: 31320
		private GameObject go;

		// Token: 0x04007A59 RID: 31321
		private VideoPlayer _vp;
	}
}
