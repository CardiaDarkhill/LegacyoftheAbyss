using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EE7 RID: 3815
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Box.")]
	public class GUILayoutBox : GUILayoutAction
	{
		// Token: 0x06006B2E RID: 27438 RVA: 0x002163AE File Offset: 0x002145AE
		public override void Reset()
		{
			base.Reset();
			this.text = "";
			this.image = null;
			this.tooltip = "";
			this.style = "";
		}

		// Token: 0x06006B2F RID: 27439 RVA: 0x002163F0 File Offset: 0x002145F0
		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Box(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
				return;
			}
			GUILayout.Box(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
		}

		// Token: 0x04006A7F RID: 27263
		[Tooltip("Image to display in the Box.")]
		public FsmTexture image;

		// Token: 0x04006A80 RID: 27264
		[Tooltip("Text to display in the Box.")]
		public FsmString text;

		// Token: 0x04006A81 RID: 27265
		[Tooltip("The tooltip associated with this control. See {{GUI Tooltip}}")]
		public FsmString tooltip;

		// Token: 0x04006A82 RID: 27266
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;
	}
}
