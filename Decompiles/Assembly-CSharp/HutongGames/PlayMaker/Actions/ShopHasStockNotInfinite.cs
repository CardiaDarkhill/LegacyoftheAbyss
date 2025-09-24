using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001386 RID: 4998
	public class ShopHasStockNotInfinite : ShopCheck
	{
		// Token: 0x06008081 RID: 32897 RVA: 0x0025E87A File Offset: 0x0025CA7A
		protected override bool CheckShop(ShopMenuStock shop)
		{
			return shop.StockLeftNotInfinite();
		}
	}
}
