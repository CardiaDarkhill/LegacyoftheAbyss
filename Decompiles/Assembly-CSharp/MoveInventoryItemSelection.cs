using System;
using HutongGames.PlayMaker;

// Token: 0x02000696 RID: 1686
public class MoveInventoryItemSelection : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	// Token: 0x06003C44 RID: 15428 RVA: 0x001094F1 File Offset: 0x001076F1
	public override void Reset()
	{
		base.Reset();
		this.Direction = null;
		this.CancelEvent = null;
	}

	// Token: 0x06003C45 RID: 15429 RVA: 0x00109507 File Offset: 0x00107707
	protected override void DoAction(InventoryItemManager itemManager)
	{
		if (!itemManager.MoveSelection((InventoryItemManager.SelectionDirection)this.Direction.Value))
		{
			base.Fsm.Event(this.CancelEvent);
		}
	}

	// Token: 0x04003E46 RID: 15942
	[ObjectType(typeof(InventoryItemManager.SelectionDirection))]
	public FsmEnum Direction;

	// Token: 0x04003E47 RID: 15943
	public FsmEvent CancelEvent;
}
