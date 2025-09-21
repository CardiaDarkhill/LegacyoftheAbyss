using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011D1 RID: 4561
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get Internal texture in which video content is placed. (ReadOnly)")]
	public class VideoPlayerGetTexture : FsmStateAction
	{
		// Token: 0x060079C1 RID: 31169 RVA: 0x0024B1A7 File Offset: 0x002493A7
		public override void Reset()
		{
			this.gameObject = null;
			this.texture = null;
			this.everyFrame = false;
		}

		// Token: 0x060079C2 RID: 31170 RVA: 0x0024B1BE File Offset: 0x002493BE
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060079C3 RID: 31171 RVA: 0x0024B1DA File Offset: 0x002493DA
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079C4 RID: 31172 RVA: 0x0024B1E2 File Offset: 0x002493E2
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.texture.Value = this._vp.texture;
				return;
			}
		}

		// Token: 0x060079C5 RID: 31173 RVA: 0x0024B209 File Offset: 0x00249409
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A24 RID: 31268
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A25 RID: 31269
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Internal texture in which video content is placed")]
		public FsmTexture texture;

		// Token: 0x04007A26 RID: 31270
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A27 RID: 31271
		private GameObject go;

		// Token: 0x04007A28 RID: 31272
		private VideoPlayer _vp;
	}
}
