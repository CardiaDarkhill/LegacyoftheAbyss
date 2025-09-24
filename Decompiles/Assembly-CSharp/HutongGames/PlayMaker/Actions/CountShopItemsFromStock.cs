using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001389 RID: 5001
	public class CountShopItemsFromStock : FsmStateAction
	{
		// Token: 0x0600808A RID: 32906 RVA: 0x0025E9D4 File Offset: 0x0025CBD4
		public override void Reset()
		{
			this.StockList = null;
			this.MatchFlag = null;
			this.PurchasedOnly = null;
			this.StoreCount = null;
		}

		// Token: 0x0600808B RID: 32907 RVA: 0x0025E9F2 File Offset: 0x0025CBF2
		public override void OnEnter()
		{
			this.StoreCount.Value = 0;
			this.DoAction();
			base.Finish();
		}

		// Token: 0x0600808C RID: 32908 RVA: 0x0025EA0C File Offset: 0x0025CC0C
		private void DoAction()
		{
			ShopItemList shopItemList = this.StockList.Value as ShopItemList;
			ShopItem.TypeFlags typeFlags = (ShopItem.TypeFlags)this.MatchFlag.Value;
			int value = CountShopItems.CountShopItemsFromStock((shopItemList != null) ? shopItemList.ShopItems : null, typeFlags, this.PurchasedOnly.Value);
			this.StoreCount.Value = value;
		}

		// Token: 0x04007FDB RID: 32731
		[ObjectType(typeof(ShopItemList))]
		public FsmObject StockList;

		// Token: 0x04007FDC RID: 32732
		[ObjectType(typeof(ShopItem.TypeFlags))]
		public FsmEnum MatchFlag;

		// Token: 0x04007FDD RID: 32733
		public FsmBool PurchasedOnly;

		// Token: 0x04007FDE RID: 32734
		[UIHint(UIHint.Variable)]
		public FsmInt StoreCount;
	}
}
