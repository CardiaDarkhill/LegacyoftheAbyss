using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001285 RID: 4741
	public class SetNextInventoryCollectableItemSelectable : FsmStateAction
	{
		// Token: 0x06007CB3 RID: 31923 RVA: 0x00254319 File Offset: 0x00252519
		public override void Reset()
		{
			this.selectableType = null;
		}

		// Token: 0x06007CB4 RID: 31924 RVA: 0x00254322 File Offset: 0x00252522
		public override void OnEnter()
		{
			InventoryCollectableItemSelectionHelper.LastSelectionUpdate = (InventoryCollectableItemSelectionHelper.SelectionType)this.selectableType.Value;
			base.Finish();
		}

		// Token: 0x04007CCE RID: 31950
		[ObjectType(typeof(InventoryCollectableItemSelectionHelper.SelectionType))]
		public FsmEnum selectableType;
	}
}
