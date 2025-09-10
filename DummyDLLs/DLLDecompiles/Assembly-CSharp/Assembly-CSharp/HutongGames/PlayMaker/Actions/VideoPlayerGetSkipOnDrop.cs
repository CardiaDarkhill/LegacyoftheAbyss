using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011CB RID: 4555
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether the player is allowed to skips frames to catch up with current time.")]
	public class VideoPlayerGetSkipOnDrop : FsmStateAction
	{
		// Token: 0x0600799D RID: 31133 RVA: 0x0024AD36 File Offset: 0x00248F36
		public override void Reset()
		{
			this.gameObject = null;
			this.skipOnDrop = null;
			this.doesSkipOnDropEvent = null;
			this.DoNotSkipOnDropEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x0600799E RID: 31134 RVA: 0x0024AD5B File Offset: 0x00248F5B
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600799F RID: 31135 RVA: 0x0024AD77 File Offset: 0x00248F77
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079A0 RID: 31136 RVA: 0x0024AD80 File Offset: 0x00248F80
		private void ExecuteAction()
		{
			if (this._vp == null)
			{
				return;
			}
			if (this._vp.skipOnDrop)
			{
				this.skipOnDrop.Value = true;
				if (this._canSetSkipOnDrop != 1)
				{
					base.Fsm.Event(this.doesSkipOnDropEvent);
				}
				this._canSetSkipOnDrop = 1;
				return;
			}
			this.skipOnDrop.Value = false;
			if (this._canSetSkipOnDrop != 0)
			{
				base.Fsm.Event(this.DoNotSkipOnDropEvent);
			}
			this._canSetSkipOnDrop = 0;
		}

		// Token: 0x060079A1 RID: 31137 RVA: 0x0024AE03 File Offset: 0x00249003
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A03 RID: 31235
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A04 RID: 31236
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool skipOnDrop;

		// Token: 0x04007A05 RID: 31237
		[Tooltip("Event sent if SkipOnDrop is true")]
		public FsmEvent doesSkipOnDropEvent;

		// Token: 0x04007A06 RID: 31238
		[Tooltip("Event sent if SkipOnDrop is false")]
		public FsmEvent DoNotSkipOnDropEvent;

		// Token: 0x04007A07 RID: 31239
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A08 RID: 31240
		private int _canSetSkipOnDrop = -1;

		// Token: 0x04007A09 RID: 31241
		private GameObject go;

		// Token: 0x04007A0A RID: 31242
		private VideoPlayer _vp;
	}
}
