using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD2 RID: 3026
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class BoolTestV2 : FsmStateAction
	{
		// Token: 0x06005CD8 RID: 23768 RVA: 0x001D30B4 File Offset: 0x001D12B4
		public override void Reset()
		{
			this.BoolVariable = null;
			this.ExpectedValue = null;
			this.IsExpected = null;
			this.IsNotExpected = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005CD9 RID: 23769 RVA: 0x001D30D9 File Offset: 0x001D12D9
		public override void OnEnter()
		{
			this.EvalEvents();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CDA RID: 23770 RVA: 0x001D30EF File Offset: 0x001D12EF
		public override void OnUpdate()
		{
			this.EvalEvents();
		}

		// Token: 0x06005CDB RID: 23771 RVA: 0x001D30F7 File Offset: 0x001D12F7
		private void EvalEvents()
		{
			base.Fsm.Event((this.BoolVariable.Value == this.ExpectedValue.Value) ? this.IsExpected : this.IsNotExpected);
		}

		// Token: 0x04005873 RID: 22643
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmBool BoolVariable;

		// Token: 0x04005874 RID: 22644
		public FsmBool ExpectedValue;

		// Token: 0x04005875 RID: 22645
		public FsmEvent IsExpected;

		// Token: 0x04005876 RID: 22646
		public FsmEvent IsNotExpected;

		// Token: 0x04005877 RID: 22647
		public bool EveryFrame;
	}
}
