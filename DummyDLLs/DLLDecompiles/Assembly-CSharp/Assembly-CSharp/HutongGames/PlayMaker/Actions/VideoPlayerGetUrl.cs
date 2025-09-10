using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011D4 RID: 4564
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the file or HTTP URL that the player will read content from.")]
	public class VideoPlayerGetUrl : FsmStateAction
	{
		// Token: 0x060079D3 RID: 31187 RVA: 0x0024B392 File Offset: 0x00249592
		public override void Reset()
		{
			this.gameObject = null;
			this.url = null;
			this.everyFrame = false;
		}

		// Token: 0x060079D4 RID: 31188 RVA: 0x0024B3A9 File Offset: 0x002495A9
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060079D5 RID: 31189 RVA: 0x0024B3C5 File Offset: 0x002495C5
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079D6 RID: 31190 RVA: 0x0024B3CD File Offset: 0x002495CD
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.url.Value = this._vp.url;
				return;
			}
		}

		// Token: 0x060079D7 RID: 31191 RVA: 0x0024B3F4 File Offset: 0x002495F4
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A33 RID: 31283
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A34 RID: 31284
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The file or HTTP URL that the player will read content from.")]
		public FsmString url;

		// Token: 0x04007A35 RID: 31285
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A36 RID: 31286
		private GameObject go;

		// Token: 0x04007A37 RID: 31287
		private VideoPlayer _vp;
	}
}
