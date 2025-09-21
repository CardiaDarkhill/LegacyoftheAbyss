using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F47 RID: 3911
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if the value of a Float variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class FloatChanged : FsmStateAction
	{
		// Token: 0x06006CCC RID: 27852 RVA: 0x0021E96D File Offset: 0x0021CB6D
		public override void Reset()
		{
			this.floatVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006CCD RID: 27853 RVA: 0x0021E984 File Offset: 0x0021CB84
		public override void OnEnter()
		{
			if (this.floatVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.floatVariable.Value;
		}

		// Token: 0x06006CCE RID: 27854 RVA: 0x0021E9AC File Offset: 0x0021CBAC
		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.floatVariable.Value != this.previousValue)
			{
				this.previousValue = this.floatVariable.Value;
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}

		// Token: 0x04006C84 RID: 27780
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to watch for a change.")]
		public FsmFloat floatVariable;

		// Token: 0x04006C85 RID: 27781
		[Tooltip("Event to send if the float variable changes.")]
		public FsmEvent changedEvent;

		// Token: 0x04006C86 RID: 27782
		[UIHint(UIHint.Variable)]
		[Tooltip("Set to True if the float variable changes.")]
		public FsmBool storeResult;

		// Token: 0x04006C87 RID: 27783
		private float previousValue;
	}
}
