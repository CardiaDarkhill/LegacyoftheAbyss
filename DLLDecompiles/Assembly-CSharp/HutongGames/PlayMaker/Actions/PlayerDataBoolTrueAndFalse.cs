using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCC RID: 3276
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Checks whether a player bool is true and another is false. Sends event.")]
	public class PlayerDataBoolTrueAndFalse : FsmStateAction
	{
		// Token: 0x060061B6 RID: 25014 RVA: 0x001EF107 File Offset: 0x001ED307
		public override void Reset()
		{
			this.trueBool = null;
			this.falseBool = null;
			this.isTrue = null;
			this.isFalse = null;
		}

		// Token: 0x060061B7 RID: 25015 RVA: 0x001EF128 File Offset: 0x001ED328
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				return;
			}
			if (instance.GetPlayerDataBool(this.trueBool.Value) && !instance.GetPlayerDataBool(this.falseBool.Value))
			{
				base.Fsm.Event(this.isTrue);
			}
			else
			{
				base.Fsm.Event(this.isFalse);
			}
			base.Finish();
		}

		// Token: 0x04005FE9 RID: 24553
		[RequiredField]
		public FsmString trueBool;

		// Token: 0x04005FEA RID: 24554
		[RequiredField]
		public FsmString falseBool;

		// Token: 0x04005FEB RID: 24555
		[Tooltip("Event to send if conditions met.")]
		public FsmEvent isTrue;

		// Token: 0x04005FEC RID: 24556
		[Tooltip("Event to send if conditions not met.")]
		public FsmEvent isFalse;
	}
}
