using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F5F RID: 3935
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Compares 2 Strings and sends Events based on the result.")]
	public class StringCompare : FsmStateAction
	{
		// Token: 0x06006D44 RID: 27972 RVA: 0x0021FCF3 File Offset: 0x0021DEF3
		public override void Reset()
		{
			this.stringVariable = null;
			this.compareTo = "";
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D45 RID: 27973 RVA: 0x0021FD28 File Offset: 0x0021DF28
		public override void OnEnter()
		{
			this.DoStringCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D46 RID: 27974 RVA: 0x0021FD3E File Offset: 0x0021DF3E
		public override void OnUpdate()
		{
			this.DoStringCompare();
		}

		// Token: 0x06006D47 RID: 27975 RVA: 0x0021FD48 File Offset: 0x0021DF48
		private void DoStringCompare()
		{
			if (this.stringVariable == null || this.compareTo == null)
			{
				return;
			}
			bool flag = string.IsNullOrEmpty(this.stringVariable.Value) ? string.IsNullOrEmpty(this.compareTo.Value) : (this.stringVariable.Value == this.compareTo.Value);
			if (this.storeResult != null)
			{
				this.storeResult.Value = flag;
			}
			if (flag && this.equalEvent != null)
			{
				base.Fsm.Event(this.equalEvent);
				return;
			}
			if (!flag && this.notEqualEvent != null)
			{
				base.Fsm.Event(this.notEqualEvent);
			}
		}

		// Token: 0x04006D05 RID: 27909
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String Variable to compare.")]
		public FsmString stringVariable;

		// Token: 0x04006D06 RID: 27910
		[Tooltip("Compare to this text.")]
		public FsmString compareTo;

		// Token: 0x04006D07 RID: 27911
		[Tooltip("Event to send if strings are equal.")]
		public FsmEvent equalEvent;

		// Token: 0x04006D08 RID: 27912
		[Tooltip("Event to send if strings are not equal.")]
		public FsmEvent notEqualEvent;

		// Token: 0x04006D09 RID: 27913
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the true/false result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006D0A RID: 27914
		[Tooltip("Repeat every frame. Useful if any of the strings are changing over time.")]
		public bool everyFrame;
	}
}
