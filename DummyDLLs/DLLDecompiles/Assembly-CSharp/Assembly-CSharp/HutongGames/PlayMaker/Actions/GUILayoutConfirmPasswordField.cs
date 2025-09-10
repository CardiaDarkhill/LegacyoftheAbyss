using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EE9 RID: 3817
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
	public class GUILayoutConfirmPasswordField : GUILayoutAction
	{
		// Token: 0x06006B34 RID: 27444 RVA: 0x002165AC File Offset: 0x002147AC
		public override void Reset()
		{
			this.text = null;
			this.maxLength = 25;
			this.style = "TextField";
			this.mask = "*";
			this.changedEvent = null;
			this.confirm = false;
			this.password = null;
		}

		// Token: 0x06006B35 RID: 27445 RVA: 0x00216608 File Offset: 0x00214808
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

		// Token: 0x04006A89 RID: 27273
		[UIHint(UIHint.Variable)]
		[Tooltip("The password Text")]
		public FsmString text;

		// Token: 0x04006A8A RID: 27274
		[Tooltip("The Maximum Length of the field")]
		public FsmInt maxLength;

		// Token: 0x04006A8B RID: 27275
		[Tooltip("The Style of the Field")]
		public FsmString style;

		// Token: 0x04006A8C RID: 27276
		[Tooltip("Event sent when field content changed")]
		public FsmEvent changedEvent;

		// Token: 0x04006A8D RID: 27277
		[Tooltip("Replacement character to hide the password")]
		public FsmString mask;

		// Token: 0x04006A8E RID: 27278
		[Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
		public FsmBool confirm;

		// Token: 0x04006A8F RID: 27279
		[Tooltip("Confirmation content")]
		public FsmString password;
	}
}
