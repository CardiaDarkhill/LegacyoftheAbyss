using System;
using UnityEngine;

// Token: 0x020001BD RID: 445
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Currency)")]
public class CurrencyCollectable : FakeCollectable
{
	// Token: 0x06001144 RID: 4420 RVA: 0x0005108B File Offset: 0x0004F28B
	public override void Get(bool showPopup = true)
	{
		this.GetMultiple(1, showPopup);
	}

	// Token: 0x06001145 RID: 4421 RVA: 0x00051095 File Offset: 0x0004F295
	protected override void GetMultiple(int amount, bool showPopup)
	{
		base.Get(showPopup);
		CurrencyManager.AddCurrency(amount, this.currencyType, true);
	}

	// Token: 0x04001049 RID: 4169
	[Space]
	[SerializeField]
	private CurrencyType currencyType;
}
