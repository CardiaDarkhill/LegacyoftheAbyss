using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F5A RID: 3930
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Compare 2 Object Variables and send events based on the result.")]
	public class ObjectCompare : FsmStateAction
	{
		// Token: 0x06006D2B RID: 27947 RVA: 0x0021F878 File Offset: 0x0021DA78
		public override void Reset()
		{
			this.objectVariable = null;
			this.compareTo = null;
			this.storeResult = null;
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D2C RID: 27948 RVA: 0x0021F8A4 File Offset: 0x0021DAA4
		public override void OnEnter()
		{
			this.DoObjectCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D2D RID: 27949 RVA: 0x0021F8BA File Offset: 0x0021DABA
		public override void OnUpdate()
		{
			this.DoObjectCompare();
		}

		// Token: 0x06006D2E RID: 27950 RVA: 0x0021F8C4 File Offset: 0x0021DAC4
		private void DoObjectCompare()
		{
			bool flag = this.objectVariable.Value == this.compareTo.Value;
			this.storeResult.Value = flag;
			base.Fsm.Event(flag ? this.equalEvent : this.notEqualEvent);
		}

		// Token: 0x04006CED RID: 27885
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Readonly]
		[Tooltip("The Object Variable to compare.")]
		public FsmObject objectVariable;

		// Token: 0x04006CEE RID: 27886
		[RequiredField]
		[Tooltip("The value to compare it to.")]
		public FsmObject compareTo;

		// Token: 0x04006CEF RID: 27887
		[Tooltip("Event to send if the 2 object values are equal.")]
		public FsmEvent equalEvent;

		// Token: 0x04006CF0 RID: 27888
		[Tooltip("Event to send if the 2 object values are not equal.")]
		public FsmEvent notEqualEvent;

		// Token: 0x04006CF1 RID: 27889
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a variable.")]
		public FsmBool storeResult;

		// Token: 0x04006CF2 RID: 27890
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
