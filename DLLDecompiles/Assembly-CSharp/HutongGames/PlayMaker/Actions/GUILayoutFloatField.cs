using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EF1 RID: 3825
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Text Field to edit a Float Variable. Optionally send an event if the text has been edited.")]
	public class GUILayoutFloatField : GUILayoutAction
	{
		// Token: 0x06006B4B RID: 27467 RVA: 0x002167B6 File Offset: 0x002149B6
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.style = "";
			this.changedEvent = null;
		}

		// Token: 0x06006B4C RID: 27468 RVA: 0x002167DC File Offset: 0x002149DC
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			if (!string.IsNullOrEmpty(this.style.Value))
			{
				this.floatVariable.Value = float.Parse(GUILayout.TextField(this.floatVariable.Value.ToString(), this.style.Value, base.LayoutOptions));
			}
			else
			{
				this.floatVariable.Value = float.Parse(GUILayout.TextField(this.floatVariable.Value.ToString(), base.LayoutOptions));
			}
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
				return;
			}
			GUI.changed = changed;
		}

		// Token: 0x04006A95 RID: 27285
		[UIHint(UIHint.Variable)]
		[Tooltip("Float Variable to show in the edit field.")]
		public FsmFloat floatVariable;

		// Token: 0x04006A96 RID: 27286
		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;

		// Token: 0x04006A97 RID: 27287
		[Tooltip("Optional event to send when the value changes.")]
		public FsmEvent changedEvent;
	}
}
