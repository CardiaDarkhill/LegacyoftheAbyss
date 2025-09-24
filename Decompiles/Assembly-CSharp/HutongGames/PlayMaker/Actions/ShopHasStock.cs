using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001385 RID: 4997
	public class ShopHasStock : ShopCheck
	{
		// Token: 0x0600807F RID: 32895 RVA: 0x0025E86A File Offset: 0x0025CA6A
		protected override bool CheckShop(ShopMenuStock shop)
		{
			return shop.StockLeft();
		}
	}
}
