using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EE4 RID: 3812
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout BeginHorizontal.")]
	public class GUILayoutBeginHorizontal : GUILayoutAction
	{
		// Token: 0x06006B25 RID: 27429 RVA: 0x00216187 File Offset: 0x00214387
		public override void Reset()
		{
			base.Reset();
			this.text = "";
			this.image = null;
			this.tooltip = "";
			this.style = "";
		}

		// Token: 0x06006B26 RID: 27430 RVA: 0x002161C8 File Offset: 0x002143C8
		public override void OnGUI()
		{
			GUILayout.BeginHorizontal(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
		}

		// Token: 0x04006A70 RID: 27248
		[Tooltip("Texture to display.")]
		public FsmTexture image;

		// Token: 0x04006A71 RID: 27249
		[Tooltip("Text to display.")]
		public FsmString text;

		// Token: 0x04006A72 RID: 27250
		[Tooltip("The tooltip associated with this control. See {{GUI Tooltip}}")]
		public FsmString tooltip;

		// Token: 0x04006A73 RID: 27251
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;
	}
}
