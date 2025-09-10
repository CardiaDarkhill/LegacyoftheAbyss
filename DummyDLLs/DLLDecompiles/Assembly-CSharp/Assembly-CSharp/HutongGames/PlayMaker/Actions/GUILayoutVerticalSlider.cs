using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EFE RID: 3838
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("A Vertical Slider linked to a Float Variable.")]
	public class GUILayoutVerticalSlider : GUILayoutAction
	{
		// Token: 0x06006B76 RID: 27510 RVA: 0x002173C6 File Offset: 0x002155C6
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.topValue = 100f;
			this.bottomValue = 0f;
			this.changedEvent = null;
		}

		// Token: 0x06006B77 RID: 27511 RVA: 0x002173FC File Offset: 0x002155FC
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUILayout.VerticalSlider(this.floatVariable.Value, this.topValue.Value, this.bottomValue.Value, base.LayoutOptions);
			}
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
				return;
			}
			GUI.changed = changed;
		}

		// Token: 0x04006ACA RID: 27338
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float Variable linked to the slider value.")]
		public FsmFloat floatVariable;

		// Token: 0x04006ACB RID: 27339
		[RequiredField]
		[Tooltip("The value of the variable at the top of the slider.")]
		public FsmFloat topValue;

		// Token: 0x04006ACC RID: 27340
		[RequiredField]
		[Tooltip("The value of the variable at the bottom of the slider.")]
		public FsmFloat bottomValue;

		// Token: 0x04006ACD RID: 27341
		[Tooltip("Optional Event to send when the slider value changes.")]
		public FsmEvent changedEvent;
	}
}
