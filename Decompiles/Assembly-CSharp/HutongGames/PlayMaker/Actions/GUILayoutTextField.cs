using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EFA RID: 3834
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Text Field. Optionally send an event if the text has been edited.")]
	public class GUILayoutTextField : GUILayoutAction
	{
		// Token: 0x06006B66 RID: 27494 RVA: 0x00216EC5 File Offset: 0x002150C5
		public override void Reset()
		{
			base.Reset();
			this.text = null;
			this.maxLength = 25;
			this.style = "TextField";
			this.changedEvent = null;
		}

		// Token: 0x06006B67 RID: 27495 RVA: 0x00216EF8 File Offset: 0x002150F8
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.text.Value = GUILayout.TextField(this.text.Value, this.maxLength.Value, this.style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
				return;
			}
			GUI.changed = changed;
		}

		// Token: 0x04006AB5 RID: 27317
		[UIHint(UIHint.Variable)]
		[Tooltip("Link the text field to a String Variable.")]
		public FsmString text;

		// Token: 0x04006AB6 RID: 27318
		[Tooltip("The max number of characters that can be entered.")]
		public FsmInt maxLength;

		// Token: 0x04006AB7 RID: 27319
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;

		// Token: 0x04006AB8 RID: 27320
		[Tooltip("An optional Event to send when the text field value changes.")]
		public FsmEvent changedEvent;
	}
}
