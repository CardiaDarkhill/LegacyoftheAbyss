using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001343 RID: 4931
	public class RespondToCurrencyCounterEvents : FsmStateAction
	{
		// Token: 0x06007F6E RID: 32622 RVA: 0x0025B650 File Offset: 0x00259850
		public override void Reset()
		{
			this.CurrencyType = null;
			this.StateEvent = null;
			this.Response = null;
		}

		// Token: 0x06007F6F RID: 32623 RVA: 0x0025B667 File Offset: 0x00259867
		public override void OnEnter()
		{
			CurrencyCounterTyped<global::CurrencyType>.CounterStateChanged += this.OnCurrencyCounterStateChanged;
		}

		// Token: 0x06007F70 RID: 32624 RVA: 0x0025B67A File Offset: 0x0025987A
		public override void OnExit()
		{
			CurrencyCounterTyped<global::CurrencyType>.CounterStateChanged -= this.OnCurrencyCounterStateChanged;
		}

		// Token: 0x06007F71 RID: 32625 RVA: 0x0025B690 File Offset: 0x00259890
		private void OnCurrencyCounterStateChanged(CurrencyType currencyType, CurrencyCounterBase.StateEvents stateEvent)
		{
			CurrencyType currencyType2 = (CurrencyType)this.CurrencyType.Value;
			CurrencyCounterBase.StateEvents stateEvents = (CurrencyCounterBase.StateEvents)this.StateEvent.Value;
			if (currencyType != currencyType2)
			{
				return;
			}
			if (stateEvent != stateEvents)
			{
				return;
			}
			base.Fsm.Event(this.Response);
		}

		// Token: 0x04007EFA RID: 32506
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		// Token: 0x04007EFB RID: 32507
		[ObjectType(typeof(CurrencyCounterBase.StateEvents))]
		public FsmEnum StateEvent;

		// Token: 0x04007EFC RID: 32508
		public FsmEvent Response;
	}
}
