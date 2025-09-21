using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010C2 RID: 4290
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Gets the Left n characters from a String Variable.")]
	public class GetStringLeft : FsmStateAction
	{
		// Token: 0x06007452 RID: 29778 RVA: 0x0023A240 File Offset: 0x00238440
		public override void Reset()
		{
			this.stringVariable = null;
			this.charCount = 0;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06007453 RID: 29779 RVA: 0x0023A263 File Offset: 0x00238463
		public override void OnEnter()
		{
			this.DoGetStringLeft();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007454 RID: 29780 RVA: 0x0023A279 File Offset: 0x00238479
		public override void OnUpdate()
		{
			this.DoGetStringLeft();
		}

		// Token: 0x06007455 RID: 29781 RVA: 0x0023A284 File Offset: 0x00238484
		private void DoGetStringLeft()
		{
			if (this.stringVariable.IsNone)
			{
				return;
			}
			if (this.storeResult.IsNone)
			{
				return;
			}
			this.storeResult.Value = this.stringVariable.Value.Substring(0, Mathf.Clamp(this.charCount.Value, 0, this.stringVariable.Value.Length));
		}

		// Token: 0x04007495 RID: 29845
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The string to get characters from.")]
		public FsmString stringVariable;

		// Token: 0x04007496 RID: 29846
		[Tooltip("Number of characters to get.")]
		public FsmInt charCount;

		// Token: 0x04007497 RID: 29847
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a string variable.")]
		public FsmString storeResult;

		// Token: 0x04007498 RID: 29848
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
