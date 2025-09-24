using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001236 RID: 4662
	public class HeroChargeEffectRespond : FsmStateAction
	{
		// Token: 0x06007B70 RID: 31600 RVA: 0x0024F9D2 File Offset: 0x0024DBD2
		public override void Reset()
		{
			this.ChargeBurstEvent = null;
			this.ChargeEndEvent = null;
		}

		// Token: 0x06007B71 RID: 31601 RVA: 0x0024F9E2 File Offset: 0x0024DBE2
		public override void OnEnter()
		{
			this.charge = ManagerSingleton<HeroChargeEffects>.Instance;
			this.charge.ChargeBurst += this.OnChargeBurst;
			this.charge.ChargeEnd += this.OnChargeEnd;
		}

		// Token: 0x06007B72 RID: 31602 RVA: 0x0024FA1D File Offset: 0x0024DC1D
		private void OnChargeBurst()
		{
			base.Fsm.Event(this.ChargeBurstEvent);
		}

		// Token: 0x06007B73 RID: 31603 RVA: 0x0024FA30 File Offset: 0x0024DC30
		private void OnChargeEnd()
		{
			base.Fsm.Event(this.ChargeEndEvent);
			base.Finish();
		}

		// Token: 0x06007B74 RID: 31604 RVA: 0x0024FA49 File Offset: 0x0024DC49
		public override void OnExit()
		{
			this.charge.ChargeBurst -= this.OnChargeBurst;
			this.charge.ChargeEnd -= this.OnChargeEnd;
		}

		// Token: 0x04007BAD RID: 31661
		public FsmEvent ChargeBurstEvent;

		// Token: 0x04007BAE RID: 31662
		public FsmEvent ChargeEndEvent;

		// Token: 0x04007BAF RID: 31663
		private HeroChargeEffects charge;
	}
}
