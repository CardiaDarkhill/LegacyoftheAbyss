using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EE1 RID: 3809
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Begin a GUILayout block of GUI controls in a fixed screen area. NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	public class GUILayoutBeginArea : FsmStateAction
	{
		// Token: 0x06006B1B RID: 27419 RVA: 0x00215DA0 File Offset: 0x00213FA0
		public override void Reset()
		{
			this.screenRect = null;
			this.left = 0f;
			this.top = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
			this.style = "";
		}

		// Token: 0x06006B1C RID: 27420 RVA: 0x00215E10 File Offset: 0x00214010
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
			GUILayout.BeginArea(this.rect, GUIContent.none, this.style.Value);
		}

		// Token: 0x04006A61 RID: 27233
		[UIHint(UIHint.Variable)]
		[Tooltip("Screen area.")]
		public FsmRect screenRect;

		// Token: 0x04006A62 RID: 27234
		[Tooltip("Left screen coordinate.")]
		public FsmFloat left;

		// Token: 0x04006A63 RID: 27235
		[Tooltip("Top screen coordinate.")]
		public FsmFloat top;

		// Token: 0x04006A64 RID: 27236
		[Tooltip("Width of area.")]
		public FsmFloat width;

		// Token: 0x04006A65 RID: 27237
		[Tooltip("Height of area.")]
		public FsmFloat height;

		// Token: 0x04006A66 RID: 27238
		[Tooltip("Use normalized screen coordinates (0-1).")]
		public FsmBool normalized;

		// Token: 0x04006A67 RID: 27239
		[Tooltip("Optional GUIStyle name in current GUISkin.")]
		public FsmString style;

		// Token: 0x04006A68 RID: 27240
		private Rect rect;
	}
}
