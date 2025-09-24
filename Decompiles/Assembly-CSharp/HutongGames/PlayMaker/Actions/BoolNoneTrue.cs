using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F41 RID: 3905
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if all the Bool Variables are False.\nSend an event or store the result.")]
	public class BoolNoneTrue : FsmStateAction
	{
		// Token: 0x06006CAE RID: 27822 RVA: 0x0021E441 File Offset: 0x0021C641
		public override void Reset()
		{
			this.boolVariables = null;
			this.sendEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CAF RID: 27823 RVA: 0x0021E45F File Offset: 0x0021C65F
		public override void OnEnter()
		{
			this.DoNoneTrue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CB0 RID: 27824 RVA: 0x0021E475 File Offset: 0x0021C675
		public override void OnUpdate()
		{
			this.DoNoneTrue();
		}

		// Token: 0x06006CB1 RID: 27825 RVA: 0x0021E480 File Offset: 0x0021C680
		private void DoNoneTrue()
		{
			if (this.boolVariables.Length == 0)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < this.boolVariables.Length; i++)
			{
				if (this.boolVariables[i].Value)
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

		// Token: 0x04006C67 RID: 27751
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variables to check.")]
		public FsmBool[] boolVariables;

		// Token: 0x04006C68 RID: 27752
		[Tooltip("Event to send if none of the Bool variables are True.")]
		public FsmEvent sendEvent;

		// Token: 0x04006C69 RID: 27753
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006C6A RID: 27754
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
