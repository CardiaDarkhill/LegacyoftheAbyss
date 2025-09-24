using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011CF RID: 4559
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the Material texture property which is targeted when VideoPlayer.renderMode is set to Video.VideoTarget.MaterialOverride.")]
	public class VideoPlayerGetTargetMaterialProperty : FsmStateAction
	{
		// Token: 0x060079B5 RID: 31157 RVA: 0x0024B064 File Offset: 0x00249264
		public override void Reset()
		{
			this.gameObject = null;
			this.property = null;
			this.everyFrame = false;
		}

		// Token: 0x060079B6 RID: 31158 RVA: 0x0024B07B File Offset: 0x0024927B
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060079B7 RID: 31159 RVA: 0x0024B097 File Offset: 0x00249297
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079B8 RID: 31160 RVA: 0x0024B09F File Offset: 0x0024929F
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.property.Value = this._vp.targetMaterialProperty;
			}
		}

		// Token: 0x060079B9 RID: 31161 RVA: 0x0024B0C5 File Offset: 0x002492C5
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A1A RID: 31258
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A1B RID: 31259
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Material texture property")]
		public FsmString property;

		// Token: 0x04007A1C RID: 31260
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A1D RID: 31261
		private GameObject go;

		// Token: 0x04007A1E RID: 31262
		private VideoPlayer _vp;
	}
}
