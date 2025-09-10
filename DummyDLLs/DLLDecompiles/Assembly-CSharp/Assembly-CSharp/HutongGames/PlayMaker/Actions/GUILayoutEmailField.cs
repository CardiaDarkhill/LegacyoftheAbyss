using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EEA RID: 3818
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
	public class GUILayoutEmailField : GUILayoutAction
	{
		// Token: 0x06006B37 RID: 27447 RVA: 0x0021668F File Offset: 0x0021488F
		public override void Reset()
		{
			this.text = null;
			this.maxLength = 25;
			this.style = "TextField";
			this.valid = true;
			this.changedEvent = null;
		}

		// Token: 0x06006B38 RID: 27448 RVA: 0x002166C8 File Offset: 0x002148C8
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.text.Value = GUILayout.TextField(this.text.Value, this.style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
				return;
			}
			GUI.changed = changed;
		}

		// Token: 0x04006A90 RID: 27280
		[UIHint(UIHint.Variable)]
		[Tooltip("The email Text")]
		public FsmString text;

		// Token: 0x04006A91 RID: 27281
		[Tooltip("The Maximum Length of the field")]
		public FsmInt maxLength;

		// Token: 0x04006A92 RID: 27282
		[Tooltip("The Style of the Field")]
		public FsmString style;

		// Token: 0x04006A93 RID: 27283
		[Tooltip("Event sent when field content changed")]
		public FsmEvent changedEvent;

		// Token: 0x04006A94 RID: 27284
		[Tooltip("Email valid format flag")]
		public FsmBool valid;
	}
}
