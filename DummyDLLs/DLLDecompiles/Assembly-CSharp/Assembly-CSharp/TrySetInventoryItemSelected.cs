using System;
using HutongGames.PlayMaker;

// Token: 0x02000695 RID: 1685
public class TrySetInventoryItemSelected : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	// Token: 0x06003C40 RID: 15424 RVA: 0x0010941D File Offset: 0x0010761D
	public bool IsUsingCustomObject()
	{
		return this.CustomObject.Value;
	}

	// Token: 0x06003C41 RID: 15425 RVA: 0x0010942F File Offset: 0x0010762F
	public override void Reset()
	{
		base.Reset();
		this.SelectedAction = null;
		this.CustomObject = null;
		this.JustDisplay = null;
		this.successEvent = null;
		this.failedEvent = null;
	}

	// Token: 0x06003C42 RID: 15426 RVA: 0x0010945C File Offset: 0x0010765C
	protected override void DoAction(InventoryItemManager itemManager)
	{
		if (this.IsUsingCustomObject())
		{
			itemManager.SetSelected(this.CustomObject.Value, this.JustDisplay.Value);
			return;
		}
		if (!this.SelectedAction.IsNone)
		{
			InventoryItemManager.SelectedActionType selectedAction = (InventoryItemManager.SelectedActionType)this.SelectedAction.Value;
			if (itemManager.SetSelected(selectedAction, this.JustDisplay.Value))
			{
				itemManager.InstantScroll();
				base.Fsm.Event(this.successEvent);
				return;
			}
			base.Fsm.Event(this.failedEvent);
		}
	}

	// Token: 0x04003E41 RID: 15937
	[ObjectType(typeof(InventoryItemManager.SelectedActionType))]
	public FsmEnum SelectedAction;

	// Token: 0x04003E42 RID: 15938
	public FsmGameObject CustomObject;

	// Token: 0x04003E43 RID: 15939
	[HideIf("IsUsingCustomObject")]
	public FsmBool JustDisplay;

	// Token: 0x04003E44 RID: 15940
	public FsmEvent successEvent;

	// Token: 0x04003E45 RID: 15941
	public FsmEvent failedEvent;
}
