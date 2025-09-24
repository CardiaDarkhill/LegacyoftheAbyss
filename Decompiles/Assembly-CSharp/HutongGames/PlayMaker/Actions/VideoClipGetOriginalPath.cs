using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011B2 RID: 4530
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the video clip path in the project's assets. (readonly)")]
	public class VideoClipGetOriginalPath : FsmStateAction
	{
		// Token: 0x06007908 RID: 30984 RVA: 0x0024981E File Offset: 0x00247A1E
		public override void Reset()
		{
			this.gameObject = null;
			this.orVideoClip = new FsmObject
			{
				UseVariable = true
			};
			this.originalPath = null;
			this.everyFrame = false;
		}

		// Token: 0x06007909 RID: 30985 RVA: 0x00249847 File Offset: 0x00247A47
		public override void OnEnter()
		{
			this.GetVideoClip();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600790A RID: 30986 RVA: 0x00249863 File Offset: 0x00247A63
		public override void OnUpdate()
		{
			this.GetVideoClip();
			this.ExecuteAction();
		}

		// Token: 0x0600790B RID: 30987 RVA: 0x00249871 File Offset: 0x00247A71
		private void ExecuteAction()
		{
			if (this._vc != null)
			{
				this.originalPath.Value = this._vc.originalPath;
			}
		}

		// Token: 0x0600790C RID: 30988 RVA: 0x00249898 File Offset: 0x00247A98
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

		// Token: 0x04007969 RID: 31081
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400796A RID: 31082
		[UIHint(UIHint.Variable)]
		[Tooltip("Or The video clip of the VideoPlayer. Leave to none, else gameObject is ignored")]
		public FsmObject orVideoClip;

		// Token: 0x0400796B RID: 31083
		[UIHint(UIHint.Variable)]
		[Tooltip("The video clip path in the project's assets")]
		public FsmString originalPath;

		// Token: 0x0400796C RID: 31084
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400796D RID: 31085
		private GameObject go;

		// Token: 0x0400796E RID: 31086
		private VideoPlayer _vp;

		// Token: 0x0400796F RID: 31087
		private VideoClip _vc;
	}
}
