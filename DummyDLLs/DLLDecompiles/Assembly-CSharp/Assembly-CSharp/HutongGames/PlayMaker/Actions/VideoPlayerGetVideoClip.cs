using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011D5 RID: 4565
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("the videoClip of a VideoPlayer.")]
	public class VideoPlayerGetVideoClip : FsmStateAction
	{
		// Token: 0x060079D9 RID: 31193 RVA: 0x0024B434 File Offset: 0x00249634
		public override void Reset()
		{
			this.gameObject = null;
			this.videoClip = null;
			this.everyFrame = false;
		}

		// Token: 0x060079DA RID: 31194 RVA: 0x0024B44B File Offset: 0x0024964B
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060079DB RID: 31195 RVA: 0x0024B467 File Offset: 0x00249667
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079DC RID: 31196 RVA: 0x0024B46F File Offset: 0x0024966F
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.videoClip.Value = this._vp.clip;
				return;
			}
		}

		// Token: 0x060079DD RID: 31197 RVA: 0x0024B496 File Offset: 0x00249696
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A38 RID: 31288
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A39 RID: 31289
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The video clip of the VideoPlayer")]
		public FsmObject videoClip;

		// Token: 0x04007A3A RID: 31290
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A3B RID: 31291
		private GameObject go;

		// Token: 0x04007A3C RID: 31292
		private VideoPlayer _vp;
	}
}
