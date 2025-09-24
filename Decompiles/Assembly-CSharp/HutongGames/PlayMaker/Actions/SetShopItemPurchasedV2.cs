using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001382 RID: 4994
	public class SetShopItemPurchasedV2 : FsmStateAction
	{
		// Token: 0x06008074 RID: 32884 RVA: 0x0025E72F File Offset: 0x0025C92F
		public override void Reset()
		{
			this.Target = null;
			this.SubItemIndex = null;
			this.IsWaitingBool = null;
		}

		// Token: 0x06008075 RID: 32885 RVA: 0x0025E748 File Offset: 0x0025C948
		public override void OnEnter()
		{
			this.IsWaitingBool.Value = false;
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				ShopItemStats component = safe.GetComponent<ShopItemStats>();
				if (component != null)
				{
					this.IsWaitingBool.Value = true;
					component.SetPurchased(delegate
					{
						this.IsWaitingBool.Value = false;
						GameCameras.instance.HUDIn();
					}, this.SubItemIndex.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007FD2 RID: 32722
		public FsmOwnerDefault Target;

		// Token: 0x04007FD3 RID: 32723
		public FsmInt SubItemIndex;

		// Token: 0x04007FD4 RID: 32724
		[UIHint(UIHint.Variable)]
		public FsmBool IsWaitingBool;
	}
}
