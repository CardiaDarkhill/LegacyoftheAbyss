using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EF2 RID: 3826
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Label for a Float Variable.")]
	public class GUILayoutFloatLabel : GUILayoutAction
	{
		// Token: 0x06006B4E RID: 27470 RVA: 0x002168A1 File Offset: 0x00214AA1
		public override void Reset()
		{
			base.Reset();
			this.prefix = "";
			this.style = "";
			this.floatVariable = null;
		}

		// Token: 0x06006B4F RID: 27471 RVA: 0x002168D0 File Offset: 0x00214AD0
		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.prefix.Value + this.floatVariable.Value.ToString()), base.LayoutOptions);
				return;
			}
			GUILayout.Label(new GUIContent(this.prefix.Value + this.floatVariable.Value.ToString()), this.style.Value, base.LayoutOptions);
		}

		// Token: 0x04006A98 RID: 27288
		[Tooltip("Text to put before the float variable.")]
		public FsmString prefix;

		// Token: 0x04006A99 RID: 27289
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to display.")]
		public FsmFloat floatVariable;

		// Token: 0x04006A9A RID: 27290
		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;
	}
}
