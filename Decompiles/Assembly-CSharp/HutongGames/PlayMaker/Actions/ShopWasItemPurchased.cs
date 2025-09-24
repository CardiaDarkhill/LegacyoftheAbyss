using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001387 RID: 4999
	public class ShopWasItemPurchased : ShopCheck
	{
		// Token: 0x06008083 RID: 32899 RVA: 0x0025E88A File Offset: 0x0025CA8A
		protected override bool CheckShop(ShopMenuStock shop)
		{
			return shop.WasItemPurchased;
		}
	}
}
