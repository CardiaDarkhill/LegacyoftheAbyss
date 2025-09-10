using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011BC RID: 4540
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether it's possible to set if the player can skips frames to catch up with current time. (Read Only)")]
	public class VideoPlayerGetCanSkipOnDrop : FsmStateAction
	{
		// Token: 0x06007944 RID: 31044 RVA: 0x0024A10B File Offset: 0x0024830B
		public override void Reset()
		{
			this.gameObject = null;
			this.canSetSkipOnDrop = null;
			this.canSetSkipOnDropEvent = null;
			this.canNotSetSkipOnDropEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06007945 RID: 31045 RVA: 0x0024A130 File Offset: 0x00248330
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007946 RID: 31046 RVA: 0x0024A14C File Offset: 0x0024834C
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007947 RID: 31047 RVA: 0x0024A154 File Offset: 0x00248354
		private void ExecuteAction()
		{
			if (this._vp.canSetSkipOnDrop)
			{
				this.canSetSkipOnDrop.Value = true;
				if (this._canSetSkipOnDrop != 1)
				{
					base.Fsm.Event(this.canSetSkipOnDropEvent);
				}
				this._canSetSkipOnDrop = 1;
				return;
			}
			this.canSetSkipOnDrop.Value = false;
			if (this._canSetSkipOnDrop != 0)
			{
				base.Fsm.Event(this.canNotSetSkipOnDropEvent);
			}
			this._canSetSkipOnDrop = 0;
		}

		// Token: 0x06007948 RID: 31048 RVA: 0x0024A1C8 File Offset: 0x002483C8
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079A3 RID: 31139
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079A4 RID: 31140
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetSkipOnDrop;

		// Token: 0x040079A5 RID: 31141
		[Tooltip("Event sent if SkipOnDrop can be set")]
		public FsmEvent canSetSkipOnDropEvent;

		// Token: 0x040079A6 RID: 31142
		[Tooltip("Event sent if SkipOnDrop can not be set")]
		public FsmEvent canNotSetSkipOnDropEvent;

		// Token: 0x040079A7 RID: 31143
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040079A8 RID: 31144
		private int _canSetSkipOnDrop = -1;

		// Token: 0x040079A9 RID: 31145
		private GameObject go;

		// Token: 0x040079AA RID: 31146
		private VideoPlayer _vp;
	}
}
