using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001375 RID: 4981
	public class SetNextInventoryPane : FsmStateAction
	{
		// Token: 0x0600804A RID: 32842 RVA: 0x0025E0B8 File Offset: 0x0025C2B8
		public override void Reset()
		{
			this.PaneList = null;
			this.Direction = new FsmInt(1);
			this.PreviousPane = null;
			this.CurrentPane = null;
			this.CurrentPaneIndex = null;
		}

		// Token: 0x0600804B RID: 32843 RVA: 0x0025E0E8 File Offset: 0x0025C2E8
		public override void OnEnter()
		{
			GameObject safe = this.PaneList.GetSafe(this);
			if (safe)
			{
				InventoryPaneList component = safe.GetComponent<InventoryPaneList>();
				if (component && this.CurrentPane.Value != null)
				{
					InventoryPane inventoryPane = this.CurrentPane.Value.GetComponent<InventoryPane>();
					if (inventoryPane)
					{
						this.PreviousPane.Value = this.CurrentPane.Value;
						inventoryPane = component.SetNextPane(this.Direction.Value, inventoryPane);
						this.CurrentPane.Value = inventoryPane.gameObject;
						this.CurrentPaneIndex.Value = component.GetPaneIndex(inventoryPane);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007FB4 RID: 32692
		public FsmOwnerDefault PaneList;

		// Token: 0x04007FB5 RID: 32693
		public FsmInt Direction;

		// Token: 0x04007FB6 RID: 32694
		[UIHint(UIHint.Variable)]
		public FsmGameObject CurrentPane;

		// Token: 0x04007FB7 RID: 32695
		[UIHint(UIHint.Variable)]
		public FsmInt CurrentPaneIndex;

		// Token: 0x04007FB8 RID: 32696
		[UIHint(UIHint.Variable)]
		public FsmGameObject PreviousPane;
	}
}
