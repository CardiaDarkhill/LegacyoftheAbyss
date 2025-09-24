using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EC9 RID: 3785
	[Tooltip("GUI base action - don't use!")]
	public abstract class GUIAction : FsmStateAction
	{
		// Token: 0x06006AD8 RID: 27352 RVA: 0x00215198 File Offset: 0x00213398
		public override void Reset()
		{
			this.screenRect = null;
			this.left = 0f;
			this.top = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
		}

		// Token: 0x06006AD9 RID: 27353 RVA: 0x002151F8 File Offset: 0x002133F8
		public override void OnGUI()
		{
			this.rect = ((!this.screenRect.IsNone) ? this.screenRect.Value : default(Rect));
			if (!this.left.IsNone)
			{
				this.rect.x = this.left.Value;
			}
			if (!this.top.IsNone)
			{
				this.rect.y = this.top.Value;
			}
			if (!this.width.IsNone)
			{
				this.rect.width = this.width.Value;
			}
			if (!this.height.IsNone)
			{
				this.rect.height = this.height.Value;
			}
			if (this.normalized.Value)
			{
				this.rect.x = this.rect.x * (float)Screen.width;
				this.rect.width = this.rect.width * (float)Screen.width;
				this.rect.y = this.rect.y * (float)Screen.height;
				this.rect.height = this.rect.height * (float)Screen.height;
			}
		}

		// Token: 0x04006A1C RID: 27164
		[UIHint(UIHint.Variable)]
		[Tooltip("Screen rectangle.")]
		public FsmRect screenRect;

		// Token: 0x04006A1D RID: 27165
		[Tooltip("Left coordinate of screen rectangle.")]
		public FsmFloat left;

		// Token: 0x04006A1E RID: 27166
		[Tooltip("Top coordinate of screen rectangle.")]
		public FsmFloat top;

		// Token: 0x04006A1F RID: 27167
		[Tooltip("Width of screen rectangle.")]
		public FsmFloat width;

		// Token: 0x04006A20 RID: 27168
		[Tooltip("Height of screen rectangle.")]
		public FsmFloat height;

		// Token: 0x04006A21 RID: 27169
		[RequiredField]
		[Tooltip("Use normalized screen coordinates (0-1).")]
		public FsmBool normalized;

		// Token: 0x04006A22 RID: 27170
		internal Rect rect;
	}
}
