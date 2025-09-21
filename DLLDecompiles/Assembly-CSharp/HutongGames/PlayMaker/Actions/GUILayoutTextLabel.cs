using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EFB RID: 3835
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Label for simple text.")]
	public class GUILayoutTextLabel : GUILayoutAction
	{
		// Token: 0x06006B69 RID: 27497 RVA: 0x00216F79 File Offset: 0x00215179
		public override void Reset()
		{
			base.Reset();
			this.text = "";
			this.style = "";
		}

		// Token: 0x06006B6A RID: 27498 RVA: 0x00216FA4 File Offset: 0x002151A4
		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.text.Value), base.LayoutOptions);
				return;
			}
			GUILayout.Label(new GUIContent(this.text.Value), this.style.Value, base.LayoutOptions);
		}

		// Token: 0x04006AB9 RID: 27321
		[Tooltip("Text to display.")]
		public FsmString text;

		// Token: 0x04006ABA RID: 27322
		[Tooltip("Optional GUIStyle in the active GUISkin.")]
		public FsmString style;
	}
}
