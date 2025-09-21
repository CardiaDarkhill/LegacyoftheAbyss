using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EE6 RID: 3814
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Begins a vertical control group. The group must be closed with GUILayoutEndVertical action.")]
	public class GUILayoutBeginVertical : GUILayoutAction
	{
		// Token: 0x06006B2B RID: 27435 RVA: 0x00216319 File Offset: 0x00214519
		public override void Reset()
		{
			base.Reset();
			this.text = "";
			this.image = null;
			this.tooltip = "";
			this.style = "";
		}

		// Token: 0x06006B2C RID: 27436 RVA: 0x00216358 File Offset: 0x00214558
		public override void OnGUI()
		{
			GUILayout.BeginVertical(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
		}

		// Token: 0x04006A7B RID: 27259
		[Tooltip("The texture to display.")]
		public FsmTexture image;

		// Token: 0x04006A7C RID: 27260
		[Tooltip("The text to display.")]
		public FsmString text;

		// Token: 0x04006A7D RID: 27261
		[Tooltip("The tooltip associated with this control. See {{GUI Tooltip}}")]
		public FsmString tooltip;

		// Token: 0x04006A7E RID: 27262
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;
	}
}
