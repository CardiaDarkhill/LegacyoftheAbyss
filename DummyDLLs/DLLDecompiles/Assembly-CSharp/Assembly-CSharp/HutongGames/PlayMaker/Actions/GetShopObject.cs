using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001383 RID: 4995
	public class GetShopObject : FSMUtility.GetComponentFsmStateAction<ShopOwnerBase>
	{
		// Token: 0x06008078 RID: 32888 RVA: 0x0025E7D5 File Offset: 0x0025C9D5
		public override void Reset()
		{
			base.Reset();
			this.StoreObject = null;
		}

		// Token: 0x06008079 RID: 32889 RVA: 0x0025E7E4 File Offset: 0x0025C9E4
		protected override void DoAction(ShopOwnerBase owner)
		{
			this.StoreObject.Value = owner.ShopObject;
		}

		// Token: 0x04007FD5 RID: 32725
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;
	}
}
