using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011B0 RID: 4528
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the frame rate of the clip in frames/second. (readonly)")]
	public class VideoClipGetFrameRate : FsmStateAction
	{
		// Token: 0x060078FC RID: 30972 RVA: 0x0024960E File Offset: 0x0024780E
		public override void Reset()
		{
			this.gameObject = null;
			this.orVideoClip = new FsmObject
			{
				UseVariable = true
			};
			this.frameRate = null;
			this.everyFrame = false;
		}

		// Token: 0x060078FD RID: 30973 RVA: 0x00249637 File Offset: 0x00247837
		public override void OnEnter()
		{
			this.GetVideoClip();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078FE RID: 30974 RVA: 0x00249653 File Offset: 0x00247853
		public override void OnUpdate()
		{
			this.GetVideoClip();
			this.ExecuteAction();
		}

		// Token: 0x060078FF RID: 30975 RVA: 0x00249661 File Offset: 0x00247861
		private void ExecuteAction()
		{
			if (this._vc != null)
			{
				this.frameRate.Value = (float)this._vc.frameRate;
			}
		}

		// Token: 0x06007900 RID: 30976 RVA: 0x00249688 File Offset: 0x00247888
		private void GetVideoClip()
		{
			if (this.orVideoClip.IsNone)
			{
				this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (this.go != null)
				{
					this._vp = this.go.GetComponent<VideoPlayer>();
					if (this._vp != null)
					{
						this._vc = this._vp.clip;
						return;
					}
				}
			}
			else
			{
				this._vc = (this.orVideoClip.Value as VideoClip);
			}
		}

		// Token: 0x0400795B RID: 31067
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400795C RID: 31068
		[UIHint(UIHint.Variable)]
		[Tooltip("Or The video clip of the VideoPlayer. Leave to none, else gameObject is ignored")]
		public FsmObject orVideoClip;

		// Token: 0x0400795D RID: 31069
		[UIHint(UIHint.Variable)]
		[Tooltip("The frame rate of the clip in frames/second")]
		public FsmFloat frameRate;

		// Token: 0x0400795E RID: 31070
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400795F RID: 31071
		private GameObject go;

		// Token: 0x04007960 RID: 31072
		private VideoPlayer _vp;

		// Token: 0x04007961 RID: 31073
		private VideoClip _vc;
	}
}
