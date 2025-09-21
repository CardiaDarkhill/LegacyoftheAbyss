using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011EA RID: 4586
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Sets Time source followed by the VideoPlayer when reading content.")]
	public class VideoPlayerSetTimeSource : FsmStateAction
	{
		// Token: 0x06007A4E RID: 31310 RVA: 0x0024C1F9 File Offset: 0x0024A3F9
		public override void Reset()
		{
			this.gameObject = null;
			this.timeSource = VideoTimeUpdateMode.DSPTime;
			this.canNotSetTime = null;
		}

		// Token: 0x06007A4F RID: 31311 RVA: 0x0024C21C File Offset: 0x0024A41C
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null && !this._vp.canSetTime)
			{
				base.Fsm.Event(this.canNotSetTime);
			}
			else
			{
				this.ExecuteAction();
			}
			base.Finish();
		}

		// Token: 0x06007A50 RID: 31312 RVA: 0x0024C269 File Offset: 0x0024A469
		private void ExecuteAction()
		{
			if (this._vp != null && this._vp.canSetTime)
			{
				this._vp.timeUpdateMode = (VideoTimeUpdateMode)this.timeSource.Value;
			}
		}

		// Token: 0x06007A51 RID: 31313 RVA: 0x0024C2A1 File Offset: 0x0024A4A1
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A99 RID: 31385
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A9A RID: 31386
		[RequiredField]
		[Tooltip("The timeSource Value")]
		[ObjectType(typeof(VideoTimeUpdateMode))]
		public FsmEnum timeSource;

		// Token: 0x04007A9B RID: 31387
		[Tooltip("Event sent if time can not be set")]
		public FsmEvent canNotSetTime;

		// Token: 0x04007A9C RID: 31388
		private GameObject go;

		// Token: 0x04007A9D RID: 31389
		private VideoPlayer _vp;
	}
}
