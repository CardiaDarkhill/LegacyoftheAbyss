using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011BF RID: 4543
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check if the VideoPlayer can step forwards into the video content. (Read Only)")]
	public class VideoPlayerGetCanStep : FsmStateAction
	{
		// Token: 0x06007956 RID: 31062 RVA: 0x0024A3CF File Offset: 0x002485CF
		public override void Reset()
		{
			this.gameObject = null;
			this.canStep = null;
			this.canStepEvent = null;
			this.canNotStepEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06007957 RID: 31063 RVA: 0x0024A3F4 File Offset: 0x002485F4
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007958 RID: 31064 RVA: 0x0024A410 File Offset: 0x00248610
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007959 RID: 31065 RVA: 0x0024A418 File Offset: 0x00248618
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.canStep.Value = this._vp.canStep;
				base.Fsm.Event(this._vp.canSetTime ? this.canStepEvent : this.canNotStepEvent);
			}
		}

		// Token: 0x0600795A RID: 31066 RVA: 0x0024A46F File Offset: 0x0024866F
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079B9 RID: 31161
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079BA RID: 31162
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canStep;

		// Token: 0x040079BB RID: 31163
		[Tooltip("Event sent if time can be set")]
		public FsmEvent canStepEvent;

		// Token: 0x040079BC RID: 31164
		[Tooltip("Event sent if time can not be set")]
		public FsmEvent canNotStepEvent;

		// Token: 0x040079BD RID: 31165
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040079BE RID: 31166
		private GameObject go;

		// Token: 0x040079BF RID: 31167
		private VideoPlayer _vp;
	}
}
