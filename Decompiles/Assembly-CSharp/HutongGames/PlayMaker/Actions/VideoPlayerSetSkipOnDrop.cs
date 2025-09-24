using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011E3 RID: 4579
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set whether the player is allowed to skips frames to catch up with current time.")]
	public class VideoPlayerSetSkipOnDrop : FsmStateAction
	{
		// Token: 0x06007A25 RID: 31269 RVA: 0x0024BD36 File Offset: 0x00249F36
		public override void Reset()
		{
			this.gameObject = null;
			this.skipOnDrop = null;
			this.everyFrame = false;
		}

		// Token: 0x06007A26 RID: 31270 RVA: 0x0024BD4D File Offset: 0x00249F4D
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A27 RID: 31271 RVA: 0x0024BD69 File Offset: 0x00249F69
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007A28 RID: 31272 RVA: 0x0024BD71 File Offset: 0x00249F71
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.skipOnDrop = this.skipOnDrop.Value;
			}
		}

		// Token: 0x06007A29 RID: 31273 RVA: 0x0024BD97 File Offset: 0x00249F97
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A76 RID: 31350
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A77 RID: 31351
		[Tooltip("The Value")]
		[RequiredField]
		public FsmBool skipOnDrop;

		// Token: 0x04007A78 RID: 31352
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A79 RID: 31353
		private GameObject go;

		// Token: 0x04007A7A RID: 31354
		private VideoPlayer _vp;
	}
}
