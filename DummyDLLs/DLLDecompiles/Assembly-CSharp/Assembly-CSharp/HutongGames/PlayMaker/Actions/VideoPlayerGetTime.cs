using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011D2 RID: 4562
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get The player current time in seconds.")]
	public class VideoPlayerGetTime : FsmStateAction
	{
		// Token: 0x060079C7 RID: 31175 RVA: 0x0024B249 File Offset: 0x00249449
		public override void Reset()
		{
			this.gameObject = null;
			this.time = null;
			this.everyFrame = false;
		}

		// Token: 0x060079C8 RID: 31176 RVA: 0x0024B260 File Offset: 0x00249460
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060079C9 RID: 31177 RVA: 0x0024B27C File Offset: 0x0024947C
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079CA RID: 31178 RVA: 0x0024B284 File Offset: 0x00249484
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.time.Value = (float)this._vp.time;
				return;
			}
		}

		// Token: 0x060079CB RID: 31179 RVA: 0x0024B2AC File Offset: 0x002494AC
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A29 RID: 31273
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A2A RID: 31274
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The player current time in seconds")]
		public FsmFloat time;

		// Token: 0x04007A2B RID: 31275
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A2C RID: 31276
		private GameObject go;

		// Token: 0x04007A2D RID: 31277
		private VideoPlayer _vp;
	}
}
