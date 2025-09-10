using System;
using HutongGames.PlayMaker;

// Token: 0x02000699 RID: 1689
public class TryMoveInventorySelectionPage : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	// Token: 0x06003C4D RID: 15437 RVA: 0x00109639 File Offset: 0x00107839
	public override void Reset()
	{
		base.Reset();
		this.Direction = null;
		this.StoreValue = null;
		this.trueEvent = null;
		this.falseEvent = null;
	}

	// Token: 0x06003C4E RID: 15438 RVA: 0x00109660 File Offset: 0x00107860
	protected override void DoAction(InventoryItemManager itemManager)
	{
		if (!this.Direction.IsNone)
		{
			bool flag = itemManager.MoveSelectionPage((InventoryItemManager.SelectionDirection)this.Direction.Value);
			this.StoreValue.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.trueEvent);
				return;
			}
			base.Fsm.Event(this.falseEvent);
		}
	}

	// Token: 0x04003E4C RID: 15948
	[ObjectType(typeof(InventoryItemManager.SelectionDirection))]
	public FsmEnum Direction;

	// Token: 0x04003E4D RID: 15949
	[UIHint(UIHint.Variable)]
	public FsmBool StoreValue;

	// Token: 0x04003E4E RID: 15950
	public FsmEvent trueEvent;

	// Token: 0x04003E4F RID: 15951
	public FsmEvent falseEvent;
}
