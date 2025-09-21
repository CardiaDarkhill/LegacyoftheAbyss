using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001381 RID: 4993
	public class SetShopItemPurchased : FsmStateAction
	{
		// Token: 0x06008070 RID: 32880 RVA: 0x0025E699 File Offset: 0x0025C899
		public override void Reset()
		{
			this.Target = null;
			this.IsWaitingBool = null;
		}

		// Token: 0x06008071 RID: 32881 RVA: 0x0025E6AC File Offset: 0x0025C8AC
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
					}, 0);
				}
			}
			base.Finish();
		}

		// Token: 0x04007FD0 RID: 32720
		public FsmOwnerDefault Target;

		// Token: 0x04007FD1 RID: 32721
		[UIHint(UIHint.Variable)]
		public FsmBool IsWaitingBool;
	}
}
