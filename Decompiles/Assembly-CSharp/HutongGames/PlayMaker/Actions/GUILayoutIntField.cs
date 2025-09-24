using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EF4 RID: 3828
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Text Field to edit an Int Variable. Optionally send an event if the text has been edited.")]
	public class GUILayoutIntField : GUILayoutAction
	{
		// Token: 0x06006B54 RID: 27476 RVA: 0x00216A28 File Offset: 0x00214C28
		public override void Reset()
		{
			base.Reset();
			this.intVariable = null;
			this.style = "";
			this.changedEvent = null;
		}

		// Token: 0x06006B55 RID: 27477 RVA: 0x00216A50 File Offset: 0x00214C50
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			if (!string.IsNullOrEmpty(this.style.Value))
			{
				this.intVariable.Value = int.Parse(GUILayout.TextField(this.intVariable.Value.ToString(), this.style.Value, base.LayoutOptions));
			}
			else
			{
				this.intVariable.Value = int.Parse(GUILayout.TextField(this.intVariable.Value.ToString(), base.LayoutOptions));
			}
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
				return;
			}
			GUI.changed = changed;
		}

		// Token: 0x04006A9F RID: 27295
		[UIHint(UIHint.Variable)]
		[Tooltip("Int Variable to show in the edit field.")]
		public FsmInt intVariable;

		// Token: 0x04006AA0 RID: 27296
		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;

		// Token: 0x04006AA1 RID: 27297
		[Tooltip("Optional event to send when the value changes.")]
		public FsmEvent changedEvent;
	}
}
