using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011D7 RID: 4567
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send the loopPointReached event from a VideoPlayer.")]
	public class VideoPlayerLoopPointReachedEvent : FsmStateAction
	{
		// Token: 0x060079E5 RID: 31205 RVA: 0x0024B5EA File Offset: 0x002497EA
		public override void Reset()
		{
			this.gameObject = null;
			this.OnLoopPointReachedEvent = null;
		}

		// Token: 0x060079E6 RID: 31206 RVA: 0x0024B5FA File Offset: 0x002497FA
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.loopPointReached += this.OnLoopPointReached;
			}
		}

		// Token: 0x060079E7 RID: 31207 RVA: 0x0024B627 File Offset: 0x00249827
		public override void OnExit()
		{
			if (this._vp != null)
			{
				this._vp.loopPointReached -= this.OnLoopPointReached;
			}
		}

		// Token: 0x060079E8 RID: 31208 RVA: 0x0024B64E File Offset: 0x0024984E
		private void OnLoopPointReached(VideoPlayer source)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			base.Fsm.Event(this.OnLoopPointReachedEvent);
		}

		// Token: 0x060079E9 RID: 31209 RVA: 0x0024B671 File Offset: 0x00249871
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A45 RID: 31301
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A46 RID: 31302
		[Tooltip("event invoked when the player reaches the end of the content to play.")]
		public FsmEvent OnLoopPointReachedEvent;

		// Token: 0x04007A47 RID: 31303
		private GameObject go;

		// Token: 0x04007A48 RID: 31304
		private VideoPlayer _vp;
	}
}
