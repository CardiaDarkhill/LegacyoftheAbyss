using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ECC RID: 3788
	[Tooltip("GUI base action - don't use!")]
	public abstract class GUIContentAction : GUIAction
	{
		// Token: 0x06006AE0 RID: 27360 RVA: 0x0021541F File Offset: 0x0021361F
		public override void Reset()
		{
			base.Reset();
			this.image = null;
			this.text = "";
			this.tooltip = "";
			this.style = "";
		}

		// Token: 0x06006AE1 RID: 27361 RVA: 0x0021545E File Offset: 0x0021365E
		public override void OnGUI()
		{
			base.OnGUI();
			this.content = new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value);
		}

		// Token: 0x04006A25 RID: 27173
		[Tooltip("Optional image to display.")]
		public FsmTexture image;

		// Token: 0x04006A26 RID: 27174
		[Tooltip("Optional text to display.")]
		public FsmString text;

		// Token: 0x04006A27 RID: 27175
		[Tooltip("Optional tooltip. Accessed by {{GUI Tooltip}}")]
		public FsmString tooltip;

		// Token: 0x04006A28 RID: 27176
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;

		// Token: 0x04006A29 RID: 27177
		internal GUIContent content;
	}
}
