using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001388 RID: 5000
	public class CountShopItems : FsmStateAction
	{
		// Token: 0x06008085 RID: 32901 RVA: 0x0025E89A File Offset: 0x0025CA9A
		public override void Reset()
		{
			this.Target = null;
			this.MatchFlag = null;
			this.PurchasedOnly = null;
			this.StoreCount = null;
		}

		// Token: 0x06008086 RID: 32902 RVA: 0x0025E8B8 File Offset: 0x0025CAB8
		public override void OnEnter()
		{
			this.StoreCount.Value = 0;
			this.DoAction();
			base.Finish();
		}

		// Token: 0x06008087 RID: 32903 RVA: 0x0025E8D4 File Offset: 0x0025CAD4
		private void DoAction()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			ShopOwnerBase component = safe.GetComponent<ShopOwnerBase>();
			IEnumerable<ShopItem> stock;
			if (component)
			{
				stock = component.Stock;
			}
			else
			{
				ShopMenuStock component2 = safe.GetComponent<ShopMenuStock>();
				if (!component2)
				{
					return;
				}
				stock = component2.EnumerateStock();
			}
			ShopItem.TypeFlags typeFlags = (ShopItem.TypeFlags)this.MatchFlag.Value;
			int value = CountShopItems.CountShopItemsFromStock(stock, typeFlags, this.PurchasedOnly.Value);
			this.StoreCount.Value = value;
		}

		// Token: 0x06008088 RID: 32904 RVA: 0x0025E95C File Offset: 0x0025CB5C
		public static int CountShopItemsFromStock(IEnumerable<ShopItem> stock, ShopItem.TypeFlags typeFlags, bool purchasedOnly)
		{
			if (stock == null)
			{
				return 0;
			}
			int num = 0;
			foreach (ShopItem shopItem in stock)
			{
				if (shopItem && (!purchasedOnly || shopItem.IsPurchased) && (typeFlags == ShopItem.TypeFlags.None || (typeFlags & shopItem.GetTypeFlags()) != ShopItem.TypeFlags.None))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x04007FD7 RID: 32727
		public FsmOwnerDefault Target;

		// Token: 0x04007FD8 RID: 32728
		[ObjectType(typeof(ShopItem.TypeFlags))]
		public FsmEnum MatchFlag;

		// Token: 0x04007FD9 RID: 32729
		public FsmBool PurchasedOnly;

		// Token: 0x04007FDA RID: 32730
		[UIHint(UIHint.Variable)]
		public FsmInt StoreCount;
	}
}
