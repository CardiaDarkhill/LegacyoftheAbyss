using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F57 RID: 3927
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if the value of an integer variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class IntChanged : FsmStateAction
	{
		// Token: 0x06006D1C RID: 27932 RVA: 0x0021F610 File Offset: 0x0021D810
		public override void Reset()
		{
			this.intVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006D1D RID: 27933 RVA: 0x0021F627 File Offset: 0x0021D827
		public override void OnEnter()
		{
			if (this.intVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.intVariable.Value;
		}

		// Token: 0x06006D1E RID: 27934 RVA: 0x0021F650 File Offset: 0x0021D850
		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.intVariable.Value != this.previousValue)
			{
				this.previousValue = this.intVariable.Value;
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}

		// Token: 0x04006CDF RID: 27871
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The int variable to test.")]
		public FsmInt intVariable;

		// Token: 0x04006CE0 RID: 27872
		[Tooltip("Event to send if changed.")]
		public FsmEvent changedEvent;

		// Token: 0x04006CE1 RID: 27873
		[UIHint(UIHint.Variable)]
		[Tooltip("Set to true if changed, otherwise False.")]
		public FsmBool storeResult;

		// Token: 0x04006CE2 RID: 27874
		private int previousValue;
	}
}
