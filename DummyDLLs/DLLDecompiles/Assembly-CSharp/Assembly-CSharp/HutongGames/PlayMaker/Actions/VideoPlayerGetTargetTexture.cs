using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011D0 RID: 4560
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the RenderTexture to draw to when VideoPlayer.renderMode is set to Video.VideoTarget.RenderTexture.")]
	public class VideoPlayerGetTargetTexture : FsmStateAction
	{
		// Token: 0x060079BB RID: 31163 RVA: 0x0024B105 File Offset: 0x00249305
		public override void Reset()
		{
			this.gameObject = null;
			this.targetTexture = null;
			this.everyFrame = false;
		}

		// Token: 0x060079BC RID: 31164 RVA: 0x0024B11C File Offset: 0x0024931C
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060079BD RID: 31165 RVA: 0x0024B138 File Offset: 0x00249338
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079BE RID: 31166 RVA: 0x0024B140 File Offset: 0x00249340
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.targetTexture.Value = this._vp.targetTexture;
				return;
			}
		}

		// Token: 0x060079BF RID: 31167 RVA: 0x0024B167 File Offset: 0x00249367
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A1F RID: 31263
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A20 RID: 31264
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The RenderTexture")]
		public FsmTexture targetTexture;

		// Token: 0x04007A21 RID: 31265
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A22 RID: 31266
		private GameObject go;

		// Token: 0x04007A23 RID: 31267
		private VideoPlayer _vp;
	}
}
