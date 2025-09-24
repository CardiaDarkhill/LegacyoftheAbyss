using System;
using UnityEngine;

// Token: 0x020000D4 RID: 212
public class CurrencyCounterAppearRegion : TrackTriggerObjects
{
	// Token: 0x1700007D RID: 125
	// (get) Token: 0x060006BB RID: 1723 RVA: 0x0002236E File Offset: 0x0002056E
	protected override bool RequireEnabled
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x00022374 File Offset: 0x00020574
	protected override void OnInsideStateChanged(bool isInside)
	{
		foreach (CurrencyCounterAppearRegion.CounterInfo counterInfo in this.showCounters)
		{
			CurrencyCounterAppearRegion.CounterType counterType = counterInfo.CounterType;
			if (counterType != CurrencyCounterAppearRegion.CounterType.Currency)
			{
				if (counterType == CurrencyCounterAppearRegion.CounterType.CollectableItem)
				{
					if (isInside)
					{
						ItemCurrencyCounter.Show(counterInfo.CollectableItem);
					}
					else
					{
						ItemCurrencyCounter.Hide(counterInfo.CollectableItem);
					}
				}
			}
			else if (isInside)
			{
				CurrencyCounter.Show(counterInfo.CurrencyType, false);
			}
			else
			{
				CurrencyCounter.Hide(counterInfo.CurrencyType);
			}
		}
	}

	// Token: 0x0400069B RID: 1691
	[SerializeField]
	private CurrencyCounterAppearRegion.CounterInfo[] showCounters;

	// Token: 0x02001443 RID: 5187
	private enum CounterType
	{
		// Token: 0x040082A0 RID: 33440
		Currency,
		// Token: 0x040082A1 RID: 33441
		CollectableItem
	}

	// Token: 0x02001444 RID: 5188
	[Serializable]
	private struct CounterInfo
	{
		// Token: 0x06008308 RID: 33544 RVA: 0x00267E9C File Offset: 0x0026609C
		private bool IsCurrencyType()
		{
			return this.CounterType == CurrencyCounterAppearRegion.CounterType.Currency;
		}

		// Token: 0x040082A2 RID: 33442
		public CurrencyCounterAppearRegion.CounterType CounterType;

		// Token: 0x040082A3 RID: 33443
		[ModifiableProperty]
		[Conditional("IsCurrencyType", true, true, true)]
		public CurrencyType CurrencyType;

		// Token: 0x040082A4 RID: 33444
		[ModifiableProperty]
		[Conditional("IsCurrencyType", false, true, true)]
		public CollectableItem CollectableItem;
	}
}
