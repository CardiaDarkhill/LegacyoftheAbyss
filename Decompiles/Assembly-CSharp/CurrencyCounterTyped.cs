using System;

// Token: 0x02000627 RID: 1575
public abstract class CurrencyCounterTyped<T> : CurrencyCounterBase
{
	// Token: 0x140000B4 RID: 180
	// (add) Token: 0x06003824 RID: 14372 RVA: 0x000F8218 File Offset: 0x000F6418
	// (remove) Token: 0x06003825 RID: 14373 RVA: 0x000F824C File Offset: 0x000F644C
	public static event Action<T, CurrencyCounterBase.StateEvents> CounterStateChanged;

	// Token: 0x17000666 RID: 1638
	// (get) Token: 0x06003826 RID: 14374
	protected abstract T CounterType { get; }

	// Token: 0x06003827 RID: 14375 RVA: 0x000F827F File Offset: 0x000F647F
	protected override void SendStateChangedEvent(CurrencyCounterBase.StateEvents stateEvent)
	{
		base.SendStateChangedEvent(stateEvent);
		Action<T, CurrencyCounterBase.StateEvents> counterStateChanged = CurrencyCounterTyped<T>.CounterStateChanged;
		if (counterStateChanged == null)
		{
			return;
		}
		counterStateChanged(this.CounterType, stateEvent);
	}

	// Token: 0x06003828 RID: 14376 RVA: 0x000F82A0 File Offset: 0x000F64A0
	public static void RegisterTempCounterStateChangedHandler(Func<T, CurrencyCounterBase.StateEvents, bool> handler)
	{
		CurrencyCounterTyped<T>.<>c__DisplayClass6_0 CS$<>8__locals1 = new CurrencyCounterTyped<T>.<>c__DisplayClass6_0();
		CS$<>8__locals1.handler = handler;
		if (CS$<>8__locals1.handler == null)
		{
			return;
		}
		CurrencyCounterTyped<T>.CounterStateChanged += CS$<>8__locals1.<RegisterTempCounterStateChangedHandler>g__Temp|0;
	}
}
