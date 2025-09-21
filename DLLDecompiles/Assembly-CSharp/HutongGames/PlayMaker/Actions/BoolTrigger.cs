using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD3 RID: 3027
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends an event if bool true, and resets bool to false.")]
	public class BoolTrigger : FsmStateAction
	{
		// Token: 0x06005CDD RID: 23773 RVA: 0x001D3132 File Offset: 0x001D1332
		public override void Reset()
		{
			this.boolVariable = null;
			this.triggerEvent = null;
		}

		// Token: 0x06005CDE RID: 23774 RVA: 0x001D3142 File Offset: 0x001D1342
		public override void OnEnter()
		{
			if (this.boolVariable.Value)
			{
				base.Fsm.Event(this.triggerEvent);
				this.boolVariable.Value = false;
			}
			base.Finish();
		}

		// Token: 0x04005878 RID: 22648
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Readonly]
		[Tooltip("The Bool variable to test.")]
		public FsmBool boolVariable;

		// Token: 0x04005879 RID: 22649
		[Tooltip("Event to send if the Bool variable is True.")]
		public FsmEvent triggerEvent;
	}
}
