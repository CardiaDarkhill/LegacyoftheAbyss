using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFC RID: 3068
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the comparison of 2 Integers.")]
	public class CompareHP : FsmStateAction
	{
		// Token: 0x06005DCF RID: 24015 RVA: 0x001D9654 File Offset: 0x001D7854
		public override void Reset()
		{
			this.hp = 0;
			this.integer2 = 0;
			this.equal = null;
			this.lessThan = null;
			this.greaterThan = null;
			this.everyFrame = false;
		}

		// Token: 0x06005DD0 RID: 24016 RVA: 0x001D9685 File Offset: 0x001D7885
		public override void OnEnter()
		{
			this.healthManager = this.enemy.Value.GetComponent<HealthManager>();
			this.DoIntCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DD1 RID: 24017 RVA: 0x001D96B1 File Offset: 0x001D78B1
		public override void OnUpdate()
		{
			this.DoIntCompare();
		}

		// Token: 0x06005DD2 RID: 24018 RVA: 0x001D96BC File Offset: 0x001D78BC
		private void DoIntCompare()
		{
			if (this.healthManager != null)
			{
				this.hp = this.healthManager.hp;
			}
			if (this.hp == this.integer2.Value)
			{
				base.Fsm.Event(this.equal);
				return;
			}
			if (this.hp < this.integer2.Value)
			{
				base.Fsm.Event(this.lessThan);
				return;
			}
			if (this.hp > this.integer2.Value)
			{
				base.Fsm.Event(this.greaterThan);
			}
		}

		// Token: 0x06005DD3 RID: 24019 RVA: 0x001D9756 File Offset: 0x001D7956
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThan) && FsmEvent.IsNullOrEmpty(this.greaterThan))
			{
				return "Action sends no events!";
			}
			return "";
		}

		// Token: 0x04005A24 RID: 23076
		[RequiredField]
		public FsmGameObject enemy;

		// Token: 0x04005A25 RID: 23077
		public FsmInt integer2;

		// Token: 0x04005A26 RID: 23078
		[Tooltip("Event sent if Int 1 equals Int 2")]
		public FsmEvent equal;

		// Token: 0x04005A27 RID: 23079
		[Tooltip("Event sent if Int 1 is less than Int 2")]
		public FsmEvent lessThan;

		// Token: 0x04005A28 RID: 23080
		[Tooltip("Event sent if Int 1 is greater than Int 2")]
		public FsmEvent greaterThan;

		// Token: 0x04005A29 RID: 23081
		public bool everyFrame;

		// Token: 0x04005A2A RID: 23082
		private int hp;

		// Token: 0x04005A2B RID: 23083
		private HealthManager healthManager;
	}
}
