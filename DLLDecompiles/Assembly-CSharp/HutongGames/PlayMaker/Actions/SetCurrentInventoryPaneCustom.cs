using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001376 RID: 4982
	public class SetCurrentInventoryPaneCustom : FsmStateAction
	{
		// Token: 0x0600804D RID: 32845 RVA: 0x0025E1A2 File Offset: 0x0025C3A2
		public override void Reset()
		{
			this.PaneList = null;
			this.NewPane = null;
			this.PreviousPane = null;
			this.CurrentPane = null;
			this.CurrentPaneIndex = null;
		}

		// Token: 0x0600804E RID: 32846 RVA: 0x0025E1C8 File Offset: 0x0025C3C8
		public override void OnEnter()
		{
			GameObject value = this.NewPane.Value;
			GameObject safe = this.PaneList.GetSafe(this);
			if (safe && value)
			{
				InventoryPane component = value.GetComponent<InventoryPane>();
				InventoryPaneList component2 = safe.GetComponent<InventoryPaneList>();
				if (component2 && component)
				{
					this.PreviousPane.Value = this.CurrentPane.Value;
					InventoryPane inventoryPane = component2.BeginPane(component, 0);
					this.CurrentPane.Value = inventoryPane.gameObject;
					this.CurrentPaneIndex.Value = component2.GetPaneIndex(inventoryPane);
				}
			}
			base.Finish();
		}

		// Token: 0x04007FB9 RID: 32697
		public FsmOwnerDefault PaneList;

		// Token: 0x04007FBA RID: 32698
		public FsmGameObject NewPane;

		// Token: 0x04007FBB RID: 32699
		[UIHint(UIHint.Variable)]
		public FsmGameObject CurrentPane;

		// Token: 0x04007FBC RID: 32700
		[UIHint(UIHint.Variable)]
		public FsmInt CurrentPaneIndex;

		// Token: 0x04007FBD RID: 32701
		[UIHint(UIHint.Variable)]
		public FsmGameObject PreviousPane;
	}
}
