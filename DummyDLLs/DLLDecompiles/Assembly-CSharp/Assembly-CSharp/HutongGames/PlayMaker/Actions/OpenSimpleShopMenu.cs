using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200138C RID: 5004
	public class OpenSimpleShopMenu : FsmStateAction
	{
		// Token: 0x06008094 RID: 32916 RVA: 0x0025EB52 File Offset: 0x0025CD52
		public override void Reset()
		{
			this.Target = null;
			this.NoStockEvent = null;
			this.CancelledEvent = null;
			this.PurchasedEvent = null;
		}

		// Token: 0x06008095 RID: 32917 RVA: 0x0025EB70 File Offset: 0x0025CD70
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.shopOwner = safe.GetComponent<SimpleShopMenuOwner>();
				if (this.shopOwner)
				{
					this.shopOwner.ClosedNoPurchase += this.OnClosedNoPurchase;
					this.shopOwner.ClosedPurchase += this.OnClosedPurchase;
					if (!this.shopOwner.OpenMenu())
					{
						this.OnNoStock();
					}
				}
			}
		}

		// Token: 0x06008096 RID: 32918 RVA: 0x0025EBEC File Offset: 0x0025CDEC
		public override void OnExit()
		{
			if (this.shopOwner)
			{
				this.shopOwner.ClosedNoPurchase -= this.OnClosedNoPurchase;
				this.shopOwner.ClosedPurchase -= this.OnClosedPurchase;
				this.shopOwner = null;
			}
		}

		// Token: 0x06008097 RID: 32919 RVA: 0x0025EC3B File Offset: 0x0025CE3B
		private void OnNoStock()
		{
			base.Fsm.Event(this.NoStockEvent);
			base.Finish();
		}

		// Token: 0x06008098 RID: 32920 RVA: 0x0025EC54 File Offset: 0x0025CE54
		private void OnClosedNoPurchase()
		{
			base.Fsm.Event(this.CancelledEvent);
			base.Finish();
		}

		// Token: 0x06008099 RID: 32921 RVA: 0x0025EC6D File Offset: 0x0025CE6D
		private void OnClosedPurchase()
		{
			base.Fsm.Event(this.PurchasedEvent);
			base.Finish();
		}

		// Token: 0x04007FE5 RID: 32741
		[CheckForComponent(typeof(SimpleShopMenuOwner))]
		public FsmOwnerDefault Target;

		// Token: 0x04007FE6 RID: 32742
		public FsmEvent NoStockEvent;

		// Token: 0x04007FE7 RID: 32743
		public FsmEvent CancelledEvent;

		// Token: 0x04007FE8 RID: 32744
		public FsmEvent PurchasedEvent;

		// Token: 0x04007FE9 RID: 32745
		private SimpleShopMenuOwner shopOwner;
	}
}
