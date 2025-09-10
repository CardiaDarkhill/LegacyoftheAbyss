using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001374 RID: 4980
	public class SetCurrentInventoryPane : FsmStateAction
	{
		// Token: 0x06008047 RID: 32839 RVA: 0x0025DFB3 File Offset: 0x0025C1B3
		public override void Reset()
		{
			this.PaneList = null;
			this.PaneIndex = null;
			this.PreviousPane = null;
			this.CurrentPane = null;
			this.CurrentPaneIndex = null;
		}

		// Token: 0x06008048 RID: 32840 RVA: 0x0025DFD8 File Offset: 0x0025C1D8
		public override void OnEnter()
		{
			GameObject safe = this.PaneList.GetSafe(this);
			if (safe)
			{
				InventoryPaneList component = safe.GetComponent<InventoryPaneList>();
				if (component)
				{
					int num = this.PaneIndex.Value;
					if (num < 0)
					{
						string nextPaneOpen = InventoryPaneList.NextPaneOpen;
						num = component.GetPaneIndex(nextPaneOpen);
						if (num < 0)
						{
							num = this.CurrentPaneIndex.Value;
						}
					}
					InventoryPane currentPane = this.CurrentPane.Value ? this.CurrentPane.Value.GetComponent<InventoryPane>() : null;
					this.PreviousPane.Value = this.CurrentPane.Value;
					InventoryPane inventoryPane = component.SetCurrentPane(num, currentPane);
					this.CurrentPane.Value = inventoryPane.gameObject;
					this.CurrentPaneIndex.Value = component.GetPaneIndex(inventoryPane);
				}
			}
			base.Finish();
		}

		// Token: 0x04007FAF RID: 32687
		public FsmOwnerDefault PaneList;

		// Token: 0x04007FB0 RID: 32688
		public FsmInt PaneIndex;

		// Token: 0x04007FB1 RID: 32689
		[UIHint(UIHint.Variable)]
		public FsmGameObject CurrentPane;

		// Token: 0x04007FB2 RID: 32690
		[UIHint(UIHint.Variable)]
		public FsmInt CurrentPaneIndex;

		// Token: 0x04007FB3 RID: 32691
		[UIHint(UIHint.Variable)]
		public FsmGameObject PreviousPane;
	}
}
