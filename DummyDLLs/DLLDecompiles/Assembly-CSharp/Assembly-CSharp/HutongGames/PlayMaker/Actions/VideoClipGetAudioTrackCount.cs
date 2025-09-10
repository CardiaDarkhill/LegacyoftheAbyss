using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011AE RID: 4526
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the number of audio tracks in a videoClip. (ReadOnly)")]
	public class VideoClipGetAudioTrackCount : FsmStateAction
	{
		// Token: 0x060078F0 RID: 30960 RVA: 0x002493FE File Offset: 0x002475FE
		public override void Reset()
		{
			this.gameObject = null;
			this.orVideoClip = new FsmObject
			{
				UseVariable = true
			};
			this.audioTrackCount = null;
			this.everyFrame = false;
		}

		// Token: 0x060078F1 RID: 30961 RVA: 0x00249427 File Offset: 0x00247627
		public override void OnEnter()
		{
			this.GetVideoClip();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078F2 RID: 30962 RVA: 0x00249443 File Offset: 0x00247643
		public override void OnUpdate()
		{
			this.GetVideoClip();
			this.ExecuteAction();
		}

		// Token: 0x060078F3 RID: 30963 RVA: 0x00249451 File Offset: 0x00247651
		private void ExecuteAction()
		{
			if (this._vc != null)
			{
				this.audioTrackCount.Value = (int)this._vc.audioTrackCount;
			}
		}

		// Token: 0x060078F4 RID: 30964 RVA: 0x00249478 File Offset: 0x00247678
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

		// Token: 0x0400794D RID: 31053
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400794E RID: 31054
		[UIHint(UIHint.Variable)]
		[Tooltip("Or The video clip of the VideoPlayer. Leave to none, else gameObject is ignored")]
		public FsmObject orVideoClip;

		// Token: 0x0400794F RID: 31055
		[UIHint(UIHint.Variable)]
		[Tooltip("the number of audio tracks")]
		public FsmInt audioTrackCount;

		// Token: 0x04007950 RID: 31056
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007951 RID: 31057
		private GameObject go;

		// Token: 0x04007952 RID: 31058
		private VideoPlayer _vp;

		// Token: 0x04007953 RID: 31059
		private VideoClip _vc;
	}
}
