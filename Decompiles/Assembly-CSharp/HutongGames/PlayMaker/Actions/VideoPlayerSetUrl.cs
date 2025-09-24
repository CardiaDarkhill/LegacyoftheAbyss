using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011EB RID: 4587
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Sets the url value of a VideoPlayer.")]
	public class VideoPlayerSetUrl : FsmStateAction
	{
		// Token: 0x06007A53 RID: 31315 RVA: 0x0024C2E1 File Offset: 0x0024A4E1
		public override void Reset()
		{
			this.gameObject = null;
			this.url = null;
		}

		// Token: 0x06007A54 RID: 31316 RVA: 0x0024C2F1 File Offset: 0x0024A4F1
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			base.Finish();
		}

		// Token: 0x06007A55 RID: 31317 RVA: 0x0024C305 File Offset: 0x0024A505
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this._vp.url = this.url.Value;
			}
		}

		// Token: 0x06007A56 RID: 31318 RVA: 0x0024C32B File Offset: 0x0024A52B
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A9E RID: 31390
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A9F RID: 31391
		[RequiredField]
		[Tooltip("The url Value")]
		public FsmString url;

		// Token: 0x04007AA0 RID: 31392
		private GameObject go;

		// Token: 0x04007AA1 RID: 31393
		private VideoPlayer _vp;
	}
}
