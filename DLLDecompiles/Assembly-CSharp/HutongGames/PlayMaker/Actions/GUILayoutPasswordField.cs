using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EF7 RID: 3831
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
	public class GUILayoutPasswordField : GUILayoutAction
	{
		// Token: 0x06006B5D RID: 27485 RVA: 0x00216CBE File Offset: 0x00214EBE
		public override void Reset()
		{
			this.text = null;
			this.maxLength = 25;
			this.style = "TextField";
			this.mask = "*";
			this.changedEvent = null;
		}

		// Token: 0x06006B5E RID: 27486 RVA: 0x00216CFC File Offset: 0x00214EFC
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.text.Value = GUILayout.PasswordField(this.text.Value, this.mask.Value[0], this.style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
				return;
			}
			GUI.changed = changed;
		}

		// Token: 0x04006AA9 RID: 27305
		[UIHint(UIHint.Variable)]
		[Tooltip("The password Text")]
		public FsmString text;

		// Token: 0x04006AAA RID: 27306
		[Tooltip("The Maximum Length of the field")]
		public FsmInt maxLength;

		// Token: 0x04006AAB RID: 27307
		[Tooltip("The Style of the Field")]
		public FsmString style;

		// Token: 0x04006AAC RID: 27308
		[Tooltip("Event sent when field content changed")]
		public FsmEvent changedEvent;

		// Token: 0x04006AAD RID: 27309
		[Tooltip("Replacement character to hide the password")]
		public FsmString mask;
	}
}
