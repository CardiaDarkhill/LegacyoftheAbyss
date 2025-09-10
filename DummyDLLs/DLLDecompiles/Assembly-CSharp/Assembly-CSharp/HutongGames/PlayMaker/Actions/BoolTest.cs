using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F43 RID: 3907
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class BoolTest : FsmStateAction
	{
		// Token: 0x06006CB8 RID: 27832 RVA: 0x0021E5BD File Offset: 0x0021C7BD
		public override void Reset()
		{
			this.boolVariable = null;
			this.isTrue = null;
			this.isFalse = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CB9 RID: 27833 RVA: 0x0021E5DB File Offset: 0x0021C7DB
		public override void OnEnter()
		{
			base.Fsm.Event(this.boolVariable.Value ? this.isTrue : this.isFalse);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CBA RID: 27834 RVA: 0x0021E611 File Offset: 0x0021C811
		public override void OnUpdate()
		{
			base.Fsm.Event(this.boolVariable.Value ? this.isTrue : this.isFalse);
		}

		// Token: 0x04006C70 RID: 27760
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Readonly]
		[Tooltip("The Bool variable to test.")]
		public FsmBool boolVariable;

		// Token: 0x04006C71 RID: 27761
		[Tooltip("Event to send if the Bool variable is True.")]
		public FsmEvent isTrue;

		// Token: 0x04006C72 RID: 27762
		[Tooltip("Event to send if the Bool variable is False.")]
		public FsmEvent isFalse;

		// Token: 0x04006C73 RID: 27763
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
