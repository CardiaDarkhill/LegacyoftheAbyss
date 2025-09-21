using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F40 RID: 3904
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if the value of a Bool Variable has changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class BoolChanged : FsmStateAction
	{
		// Token: 0x06006CAA RID: 27818 RVA: 0x0021E3BD File Offset: 0x0021C5BD
		public override void Reset()
		{
			this.boolVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006CAB RID: 27819 RVA: 0x0021E3D4 File Offset: 0x0021C5D4
		public override void OnEnter()
		{
			if (this.boolVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.boolVariable.Value;
		}

		// Token: 0x06006CAC RID: 27820 RVA: 0x0021E3FB File Offset: 0x0021C5FB
		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.boolVariable.Value != this.previousValue)
			{
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}

		// Token: 0x04006C63 RID: 27747
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variable to watch for changes.")]
		public FsmBool boolVariable;

		// Token: 0x04006C64 RID: 27748
		[Tooltip("Event to send if the variable changes.")]
		public FsmEvent changedEvent;

		// Token: 0x04006C65 RID: 27749
		[UIHint(UIHint.Variable)]
		[Tooltip("Set to True if changed.")]
		public FsmBool storeResult;

		// Token: 0x04006C66 RID: 27750
		private bool previousValue;
	}
}
