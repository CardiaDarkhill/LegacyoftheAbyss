using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011B1 RID: 4529
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the length of the video clip in seconds. (readonly)")]
	public class VideoClipGetLength : FsmStateAction
	{
		// Token: 0x06007902 RID: 30978 RVA: 0x00249716 File Offset: 0x00247916
		public override void Reset()
		{
			this.gameObject = null;
			this.orVideoClip = new FsmObject
			{
				UseVariable = true
			};
			this.length = null;
			this.everyFrame = false;
		}

		// Token: 0x06007903 RID: 30979 RVA: 0x0024973F File Offset: 0x0024793F
		public override void OnEnter()
		{
			this.GetVideoClip();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007904 RID: 30980 RVA: 0x0024975B File Offset: 0x0024795B
		public override void OnUpdate()
		{
			this.GetVideoClip();
			this.ExecuteAction();
		}

		// Token: 0x06007905 RID: 30981 RVA: 0x00249769 File Offset: 0x00247969
		private void ExecuteAction()
		{
			if (this._vc != null)
			{
				this.length.Value = (float)this._vc.length;
			}
		}

		// Token: 0x06007906 RID: 30982 RVA: 0x00249790 File Offset: 0x00247990
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

		// Token: 0x04007962 RID: 31074
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007963 RID: 31075
		[UIHint(UIHint.Variable)]
		[Tooltip("Or the video clip of the VideoPlayer. Leave to none, else gameObject is ignored")]
		public FsmObject orVideoClip;

		// Token: 0x04007964 RID: 31076
		[UIHint(UIHint.Variable)]
		[Tooltip("The length of the video clip in seconds")]
		public FsmFloat length;

		// Token: 0x04007965 RID: 31077
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007966 RID: 31078
		private GameObject go;

		// Token: 0x04007967 RID: 31079
		private VideoPlayer _vp;

		// Token: 0x04007968 RID: 31080
		private VideoClip _vc;
	}
}
