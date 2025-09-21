using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011AF RID: 4527
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the length of the VideoClip in frames. (readonly)")]
	public class VideoClipGetFrameCount : FsmStateAction
	{
		// Token: 0x060078F6 RID: 30966 RVA: 0x00249506 File Offset: 0x00247706
		public override void Reset()
		{
			this.gameObject = null;
			this.orVideoClip = new FsmObject
			{
				UseVariable = true
			};
			this.frameCount = null;
			this.everyFrame = false;
		}

		// Token: 0x060078F7 RID: 30967 RVA: 0x0024952F File Offset: 0x0024772F
		public override void OnEnter()
		{
			this.GetVideoClip();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078F8 RID: 30968 RVA: 0x0024954B File Offset: 0x0024774B
		public override void OnUpdate()
		{
			this.GetVideoClip();
			this.ExecuteAction();
		}

		// Token: 0x060078F9 RID: 30969 RVA: 0x00249559 File Offset: 0x00247759
		private void ExecuteAction()
		{
			if (this._vc != null)
			{
				this.frameCount.Value = (int)this._vc.frameCount;
			}
		}

		// Token: 0x060078FA RID: 30970 RVA: 0x00249580 File Offset: 0x00247780
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

		// Token: 0x04007954 RID: 31060
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007955 RID: 31061
		[UIHint(UIHint.Variable)]
		[Tooltip("Or The video clip of the VideoPlayer. Leave to none, else gameObject is ignored")]
		public FsmObject orVideoClip;

		// Token: 0x04007956 RID: 31062
		[UIHint(UIHint.Variable)]
		[Tooltip("The length of the VideoClip in frames")]
		public FsmInt frameCount;

		// Token: 0x04007957 RID: 31063
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007958 RID: 31064
		private GameObject go;

		// Token: 0x04007959 RID: 31065
		private VideoPlayer _vp;

		// Token: 0x0400795A RID: 31066
		private VideoClip _vc;
	}
}
