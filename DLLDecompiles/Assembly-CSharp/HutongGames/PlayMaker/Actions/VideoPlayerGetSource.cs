using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011CC RID: 4556
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the video source type. It is valid to set both a VideoClip and a URL in the player. This property controls which one will get used for playback. When setting a new clip or URL, the source will automatically change to make the associated type current.")]
	public class VideoPlayerGetSource : FsmStateAction
	{
		// Token: 0x060079A3 RID: 31139 RVA: 0x0024AE4A File Offset: 0x0024904A
		public override void Reset()
		{
			this.gameObject = null;
			this.source = null;
			this.everyFrame = false;
		}

		// Token: 0x060079A4 RID: 31140 RVA: 0x0024AE61 File Offset: 0x00249061
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060079A5 RID: 31141 RVA: 0x0024AE7D File Offset: 0x0024907D
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079A6 RID: 31142 RVA: 0x0024AE85 File Offset: 0x00249085
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.source.Value = this._vp.source;
			}
		}

		// Token: 0x060079A7 RID: 31143 RVA: 0x0024AEB0 File Offset: 0x002490B0
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A0B RID: 31243
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A0C RID: 31244
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The source type")]
		[ObjectType(typeof(VideoSource))]
		public FsmEnum source;

		// Token: 0x04007A0D RID: 31245
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A0E RID: 31246
		private GameObject go;

		// Token: 0x04007A0F RID: 31247
		private VideoPlayer _vp;
	}
}
