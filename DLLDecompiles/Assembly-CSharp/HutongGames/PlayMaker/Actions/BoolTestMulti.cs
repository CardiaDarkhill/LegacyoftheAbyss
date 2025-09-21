using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCE RID: 3022
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if all the given Bool Variables are are equal to thier Bool States.")]
	public class BoolTestMulti : FsmStateAction
	{
		// Token: 0x06005CC3 RID: 23747 RVA: 0x001D2DC2 File Offset: 0x001D0FC2
		public override void Reset()
		{
			this.boolVariables = null;
			this.boolStates = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06005CC4 RID: 23748 RVA: 0x001D2DEE File Offset: 0x001D0FEE
		public override void OnEnter()
		{
			this.DoAllTrue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CC5 RID: 23749 RVA: 0x001D2E04 File Offset: 0x001D1004
		public override void OnUpdate()
		{
			this.DoAllTrue();
		}

		// Token: 0x06005CC6 RID: 23750 RVA: 0x001D2E0C File Offset: 0x001D100C
		private void DoAllTrue()
		{
			if (this.boolVariables.Length == 0 || this.boolStates.Length == 0)
			{
				return;
			}
			if (this.boolVariables.Length != this.boolStates.Length)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < this.boolVariables.Length; i++)
			{
				if (this.boolVariables[i].Value != this.boolStates[i].Value)
				{
					flag = false;
					break;
				}
			}
			this.storeResult.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.trueEvent);
				return;
			}
			base.Fsm.Event(this.falseEvent);
		}

		// Token: 0x0400585B RID: 22619
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("This must be the same number used for Bool States.")]
		public FsmBool[] boolVariables;

		// Token: 0x0400585C RID: 22620
		[RequiredField]
		[Tooltip("This must be the same number used for Bool Variables.")]
		public FsmBool[] boolStates;

		// Token: 0x0400585D RID: 22621
		public FsmEvent trueEvent;

		// Token: 0x0400585E RID: 22622
		public FsmEvent falseEvent;

		// Token: 0x0400585F RID: 22623
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		// Token: 0x04005860 RID: 22624
		public bool everyFrame;
	}
}
