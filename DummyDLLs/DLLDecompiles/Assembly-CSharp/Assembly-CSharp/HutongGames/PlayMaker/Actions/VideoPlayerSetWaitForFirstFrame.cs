using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011ED RID: 4589
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set whether the player will wait for the first frame to be loaded into the texture before starting playback when VideoPlayer.playOnAwake is on")]
	public class VideoPlayerSetWaitForFirstFrame : FsmStateAction
	{
		// Token: 0x06007A5C RID: 31324 RVA: 0x0024C3EB File Offset: 0x0024A5EB
		public override void Reset()
		{
			this.gameObject = null;
			this.waitForFirstFrame = null;
		}

		// Token: 0x06007A5D RID: 31325 RVA: 0x0024C3FB File Offset: 0x0024A5FB
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			base.Finish();
		}

		// Token: 0x06007A5E RID: 31326 RVA: 0x0024C40F File Offset: 0x0024A60F
		private void ExecuteAction()
		{
			if (this._vp == null)
			{
				return;
			}
			this._vp.waitForFirstFrame = this.waitForFirstFrame.Value;
		}

		// Token: 0x06007A5F RID: 31327 RVA: 0x0024C436 File Offset: 0x0024A636
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007AA6 RID: 31398
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007AA7 RID: 31399
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool waitForFirstFrame;

		// Token: 0x04007AA8 RID: 31400
		private GameObject go;

		// Token: 0x04007AA9 RID: 31401
		private VideoPlayer _vp;
	}
}
