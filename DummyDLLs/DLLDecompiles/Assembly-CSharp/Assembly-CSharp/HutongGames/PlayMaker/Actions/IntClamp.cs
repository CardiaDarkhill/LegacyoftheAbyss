using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F81 RID: 3969
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Clamp the value of an Integer Variable to a Min/Max range.")]
	public class IntClamp : FsmStateAction
	{
		// Token: 0x06006DE1 RID: 28129 RVA: 0x002218F7 File Offset: 0x0021FAF7
		public override void Reset()
		{
			this.intVariable = null;
			this.minValue = null;
			this.maxValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DE2 RID: 28130 RVA: 0x00221915 File Offset: 0x0021FB15
		public override void OnEnter()
		{
			this.DoClamp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DE3 RID: 28131 RVA: 0x0022192B File Offset: 0x0021FB2B
		public override void OnUpdate()
		{
			this.DoClamp();
		}

		// Token: 0x06006DE4 RID: 28132 RVA: 0x00221933 File Offset: 0x0021FB33
		private void DoClamp()
		{
			this.intVariable.Value = Mathf.Clamp(this.intVariable.Value, this.minValue.Value, this.maxValue.Value);
		}

		// Token: 0x04006D95 RID: 28053
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The variable to clamp.")]
		public FsmInt intVariable;

		// Token: 0x04006D96 RID: 28054
		[RequiredField]
		[Tooltip("Minimum allowed value.")]
		public FsmInt minValue;

		// Token: 0x04006D97 RID: 28055
		[RequiredField]
		[Tooltip("Maximum allowed value.")]
		public FsmInt maxValue;

		// Token: 0x04006D98 RID: 28056
		[Tooltip("Perform this action every frame.")]
		public bool everyFrame;
	}
}
