using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EF5 RID: 3829
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Label for an Int Variable.")]
	public class GUILayoutIntLabel : GUILayoutAction
	{
		// Token: 0x06006B57 RID: 27479 RVA: 0x00216B15 File Offset: 0x00214D15
		public override void Reset()
		{
			base.Reset();
			this.prefix = "";
			this.style = "";
			this.intVariable = null;
		}

		// Token: 0x06006B58 RID: 27480 RVA: 0x00216B44 File Offset: 0x00214D44
		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.prefix.Value + this.intVariable.Value.ToString()), base.LayoutOptions);
				return;
			}
			GUILayout.Label(new GUIContent(this.prefix.Value + this.intVariable.Value.ToString()), this.style.Value, base.LayoutOptions);
		}

		// Token: 0x04006AA2 RID: 27298
		[Tooltip("Text to put before the int variable.")]
		public FsmString prefix;

		// Token: 0x04006AA3 RID: 27299
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Int variable to display.")]
		public FsmInt intVariable;

		// Token: 0x04006AA4 RID: 27300
		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;
	}
}
