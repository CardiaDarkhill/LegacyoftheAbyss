using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011E1 RID: 4577
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set where the video content will be drawn")]
	public class VideoPlayerSetRenderMode : FsmStateAction
	{
		// Token: 0x06007A19 RID: 31257 RVA: 0x0024BC04 File Offset: 0x00249E04
		public override void Reset()
		{
			this.gameObject = null;
			this.renderMode = null;
			this.everyFrame = false;
		}

		// Token: 0x06007A1A RID: 31258 RVA: 0x0024BC1B File Offset: 0x00249E1B
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A1B RID: 31259 RVA: 0x0024BC37 File Offset: 0x00249E37
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007A1C RID: 31260 RVA: 0x0024BC3F File Offset: 0x00249E3F
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.renderMode = (VideoRenderMode)this.renderMode.Value;
			}
		}

		// Token: 0x06007A1D RID: 31261 RVA: 0x0024BC6A File Offset: 0x00249E6A
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A6C RID: 31340
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A6D RID: 31341
		[RequiredField]
		[Tooltip("where the video content will be drawn")]
		[ObjectType(typeof(VideoRenderMode))]
		public FsmEnum renderMode;

		// Token: 0x04007A6E RID: 31342
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A6F RID: 31343
		private GameObject go;

		// Token: 0x04007A70 RID: 31344
		private VideoPlayer _vp;
	}
}
