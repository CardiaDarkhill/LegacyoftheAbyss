using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EF6 RID: 3830
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Label.")]
	public class GUILayoutLabel : GUILayoutAction
	{
		// Token: 0x06006B5A RID: 27482 RVA: 0x00216BE2 File Offset: 0x00214DE2
		public override void Reset()
		{
			base.Reset();
			this.text = "";
			this.image = null;
			this.tooltip = "";
			this.style = "";
		}

		// Token: 0x06006B5B RID: 27483 RVA: 0x00216C24 File Offset: 0x00214E24
		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
				return;
			}
			GUILayout.Label(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
		}

		// Token: 0x04006AA5 RID: 27301
		[Tooltip("Texture to display.")]
		public FsmTexture image;

		// Token: 0x04006AA6 RID: 27302
		[Tooltip("Text to display.")]
		public FsmString text;

		// Token: 0x04006AA7 RID: 27303
		[Tooltip("The tooltip associated with this control. See {{GUI Tooltip}}")]
		public FsmString tooltip;

		// Token: 0x04006AA8 RID: 27304
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;
	}
}
