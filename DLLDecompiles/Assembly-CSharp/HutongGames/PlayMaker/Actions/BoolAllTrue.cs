using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F3E RID: 3902
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if all the given Bool Variables are True.")]
	public class BoolAllTrue : FsmStateAction
	{
		// Token: 0x06006CA0 RID: 27808 RVA: 0x0021E27A File Offset: 0x0021C47A
		public override void Reset()
		{
			this.boolVariables = null;
			this.sendEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CA1 RID: 27809 RVA: 0x0021E298 File Offset: 0x0021C498
		public override void OnEnter()
		{
			this.DoAllTrue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CA2 RID: 27810 RVA: 0x0021E2AE File Offset: 0x0021C4AE
		public override void OnUpdate()
		{
			this.DoAllTrue();
		}

		// Token: 0x06006CA3 RID: 27811 RVA: 0x0021E2B8 File Offset: 0x0021C4B8
		private void DoAllTrue()
		{
			if (this.boolVariables.Length == 0)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < this.boolVariables.Length; i++)
			{
				if (!this.boolVariables[i].Value)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = flag;
		}

		// Token: 0x04006C5B RID: 27739
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Readonly]
		[Tooltip("The Bool variables to check.")]
		public FsmBool[] boolVariables;

		// Token: 0x04006C5C RID: 27740
		[Tooltip("Event to send if all the Bool variables are True.")]
		public FsmEvent sendEvent;

		// Token: 0x04006C5D RID: 27741
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006C5E RID: 27742
		[Tooltip("Repeat every frame while the state is active. Useful if you're waiting for all to be true.")]
		public bool everyFrame;
	}
}
