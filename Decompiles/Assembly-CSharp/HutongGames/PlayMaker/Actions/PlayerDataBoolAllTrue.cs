using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCA RID: 3274
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class PlayerDataBoolAllTrue : FsmStateAction
	{
		// Token: 0x060061AF RID: 25007 RVA: 0x001EEFB8 File Offset: 0x001ED1B8
		public override void Reset()
		{
			this.stringVariables = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
		}

		// Token: 0x060061B0 RID: 25008 RVA: 0x001EEFD6 File Offset: 0x001ED1D6
		public override void OnEnter()
		{
			this.DoAllTrue();
		}

		// Token: 0x060061B1 RID: 25009 RVA: 0x001EEFE0 File Offset: 0x001ED1E0
		private void DoAllTrue()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				return;
			}
			if (this.stringVariables.Length == 0)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < this.stringVariables.Length; i++)
			{
				if (!instance.GetPlayerDataBool(this.stringVariables[i].Value))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				base.Fsm.Event(this.trueEvent);
			}
			else
			{
				base.Fsm.Event(this.falseEvent);
			}
			this.storeResult.Value = flag;
		}

		// Token: 0x04005FE0 RID: 24544
		[RequiredField]
		public FsmString[] stringVariables;

		// Token: 0x04005FE1 RID: 24545
		[Tooltip("Event to send if all the Bool variables are True.")]
		public FsmEvent trueEvent;

		// Token: 0x04005FE2 RID: 24546
		[Tooltip("Event to send if not all the bool variables are true.")]
		public FsmEvent falseEvent;

		// Token: 0x04005FE3 RID: 24547
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04005FE4 RID: 24548
		private bool boolCheck;
	}
}
