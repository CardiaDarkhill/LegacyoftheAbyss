using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011EF RID: 4591
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Advances the current time by one frame immediately of a VideoPlayer.")]
	public class VideoPlayerStepForward : FsmStateAction
	{
		// Token: 0x06007A67 RID: 31335 RVA: 0x0024C53D File Offset: 0x0024A73D
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06007A68 RID: 31336 RVA: 0x0024C546 File Offset: 0x0024A746
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			if (this._vp != null)
			{
				this._vp.StepForward();
				return;
			}
			base.Finish();
		}

		// Token: 0x06007A69 RID: 31337 RVA: 0x0024C56E File Offset: 0x0024A76E
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007AAE RID: 31406
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007AAF RID: 31407
		private GameObject go;

		// Token: 0x04007AB0 RID: 31408
		private VideoPlayer _vp;
	}
}
