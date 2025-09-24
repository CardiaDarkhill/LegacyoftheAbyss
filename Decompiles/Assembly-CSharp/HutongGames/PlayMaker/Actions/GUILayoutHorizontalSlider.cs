using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EF3 RID: 3827
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("A Horizontal Slider linked to a Float Variable.")]
	public class GUILayoutHorizontalSlider : GUILayoutAction
	{
		// Token: 0x06006B51 RID: 27473 RVA: 0x0021696E File Offset: 0x00214B6E
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.leftValue = 0f;
			this.rightValue = 100f;
			this.changedEvent = null;
		}

		// Token: 0x06006B52 RID: 27474 RVA: 0x002169A4 File Offset: 0x00214BA4
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUILayout.HorizontalSlider(this.floatVariable.Value, this.leftValue.Value, this.rightValue.Value, base.LayoutOptions);
			}
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
				return;
			}
			GUI.changed = changed;
		}

		// Token: 0x04006A9B RID: 27291
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to link the slider to. Moving the slider changes the value, and changes in the value move the slider.")]
		public FsmFloat floatVariable;

		// Token: 0x04006A9C RID: 27292
		[RequiredField]
		[Tooltip("The value of the float variable when slider is all the way to the left.")]
		public FsmFloat leftValue;

		// Token: 0x04006A9D RID: 27293
		[RequiredField]
		[Tooltip("The value of the float variable when slider is all the way to the right.")]
		public FsmFloat rightValue;

		// Token: 0x04006A9E RID: 27294
		[Tooltip("An optional fsm event to send when the value changes.")]
		public FsmEvent changedEvent;
	}
}
