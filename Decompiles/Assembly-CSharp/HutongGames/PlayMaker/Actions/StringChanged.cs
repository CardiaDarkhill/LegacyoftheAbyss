using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F5E RID: 3934
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if the value of a string variable has changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class StringChanged : FsmStateAction
	{
		// Token: 0x06006D40 RID: 27968 RVA: 0x0021FC76 File Offset: 0x0021DE76
		public override void Reset()
		{
			this.stringVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006D41 RID: 27969 RVA: 0x0021FC8D File Offset: 0x0021DE8D
		public override void OnEnter()
		{
			if (this.stringVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.stringVariable.Value;
		}

		// Token: 0x06006D42 RID: 27970 RVA: 0x0021FCB4 File Offset: 0x0021DEB4
		public override void OnUpdate()
		{
			if (this.stringVariable.Value != this.previousValue)
			{
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}

		// Token: 0x04006D01 RID: 27905
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String Variable to test.")]
		public FsmString stringVariable;

		// Token: 0x04006D02 RID: 27906
		[Tooltip("Event to send if changed.")]
		public FsmEvent changedEvent;

		// Token: 0x04006D03 RID: 27907
		[UIHint(UIHint.Variable)]
		[Tooltip("Set to True if changed, otherwise False.")]
		public FsmBool storeResult;

		// Token: 0x04006D04 RID: 27908
		private string previousValue;
	}
}
