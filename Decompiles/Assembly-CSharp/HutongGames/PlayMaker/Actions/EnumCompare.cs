using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F45 RID: 3909
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Compares 2 Enum values and sends Events based on the result.")]
	public class EnumCompare : FsmStateAction
	{
		// Token: 0x06006CC2 RID: 27842 RVA: 0x0021E7E2 File Offset: 0x0021C9E2
		public override void Reset()
		{
			this.enumVariable = null;
			this.compareTo = null;
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CC3 RID: 27843 RVA: 0x0021E80E File Offset: 0x0021CA0E
		public override void OnEnter()
		{
			this.DoEnumCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CC4 RID: 27844 RVA: 0x0021E824 File Offset: 0x0021CA24
		public override void OnUpdate()
		{
			this.DoEnumCompare();
		}

		// Token: 0x06006CC5 RID: 27845 RVA: 0x0021E82C File Offset: 0x0021CA2C
		private void DoEnumCompare()
		{
			if (this.enumVariable == null || this.compareTo == null)
			{
				return;
			}
			bool flag = object.Equals(this.enumVariable.Value, this.compareTo.Value);
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

		// Token: 0x04006C7A RID: 27770
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The first Enum Variable.")]
		public FsmEnum enumVariable;

		// Token: 0x04006C7B RID: 27771
		[MatchFieldType("enumVariable")]
		[Tooltip("The second Enum Variable.")]
		public FsmEnum compareTo;

		// Token: 0x04006C7C RID: 27772
		[Tooltip("Event to send if the values are equal.")]
		public FsmEvent equalEvent;

		// Token: 0x04006C7D RID: 27773
		[Tooltip("Event to send if the values are not equal.")]
		public FsmEvent notEqualEvent;

		// Token: 0x04006C7E RID: 27774
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the true/false result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006C7F RID: 27775
		[Tooltip("Repeat every frame. Useful if the enum is changing over time.")]
		public bool everyFrame;
	}
}
