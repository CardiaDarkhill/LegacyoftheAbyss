using System;
using HutongGames.PlayMaker;

// Token: 0x02000698 RID: 1688
public class MoveInventorySelectionPage : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	// Token: 0x06003C4A RID: 15434 RVA: 0x001095FC File Offset: 0x001077FC
	public override void Reset()
	{
		base.Reset();
		this.Direction = null;
	}

	// Token: 0x06003C4B RID: 15435 RVA: 0x0010960B File Offset: 0x0010780B
	protected override void DoAction(InventoryItemManager itemManager)
	{
		if (!this.Direction.IsNone)
		{
			itemManager.MoveSelectionPage((InventoryItemManager.SelectionDirection)this.Direction.Value);
		}
	}

	// Token: 0x04003E4B RID: 15947
	[ObjectType(typeof(InventoryItemManager.SelectionDirection))]
	public FsmEnum Direction;
}
