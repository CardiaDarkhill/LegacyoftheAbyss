using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011B3 RID: 4531
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the size in pixels of a videoClip")]
	public class VideoClipGetSize : FsmStateAction
	{
		// Token: 0x0600790E RID: 30990 RVA: 0x00249926 File Offset: 0x00247B26
		public override void Reset()
		{
			this.gameObject = null;
			this.orVideoClip = new FsmObject
			{
				UseVariable = true
			};
			this.width = null;
			this.height = null;
			this.size = null;
			this.everyFrame = false;
		}

		// Token: 0x0600790F RID: 30991 RVA: 0x0024995D File Offset: 0x00247B5D
		public override void OnEnter()
		{
			this.GetVideoClip();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007910 RID: 30992 RVA: 0x00249979 File Offset: 0x00247B79
		public override void OnUpdate()
		{
			this.GetVideoClip();
			this.ExecuteAction();
		}

		// Token: 0x06007911 RID: 30993 RVA: 0x00249988 File Offset: 0x00247B88
		private void ExecuteAction()
		{
			if (this._vc != null)
			{
				if (!this.width.IsNone)
				{
					this.width.Value = (int)this._vc.width;
				}
				if (!this.width.IsNone)
				{
					this.height.Value = (int)this._vc.height;
				}
				if (!this.size.IsNone)
				{
					this.size.Value = new Vector2((float)this._vc.width, (float)this._vc.height);
				}
				return;
			}
		}

		// Token: 0x06007912 RID: 30994 RVA: 0x00249A20 File Offset: 0x00247C20
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

		// Token: 0x04007970 RID: 31088
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007971 RID: 31089
		[UIHint(UIHint.Variable)]
		[Tooltip("Or The video clip of the VideoPlayer. Leave to none, else gameObject is ignored")]
		public FsmObject orVideoClip;

		// Token: 0x04007972 RID: 31090
		[UIHint(UIHint.Variable)]
		[Tooltip("The width of the VideoClip")]
		public FsmInt width;

		// Token: 0x04007973 RID: 31091
		[UIHint(UIHint.Variable)]
		[Tooltip("The height of the VideoClip")]
		public FsmInt height;

		// Token: 0x04007974 RID: 31092
		[UIHint(UIHint.Variable)]
		[Tooltip("The width and height of the VideoClip")]
		public FsmVector2 size;

		// Token: 0x04007975 RID: 31093
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007976 RID: 31094
		private GameObject go;

		// Token: 0x04007977 RID: 31095
		private VideoPlayer _vp;

		// Token: 0x04007978 RID: 31096
		private VideoClip _vc;
	}
}
