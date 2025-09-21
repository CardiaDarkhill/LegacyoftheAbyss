using System;
using HutongGames.PlayMaker;

// Token: 0x02000694 RID: 1684
public class SetInventoryItemSelected : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	// Token: 0x06003C3C RID: 15420 RVA: 0x0010937D File Offset: 0x0010757D
	public bool IsUsingCustomObject()
	{
		return this.CustomObject.Value;
	}

	// Token: 0x06003C3D RID: 15421 RVA: 0x0010938F File Offset: 0x0010758F
	public override void Reset()
	{
		base.Reset();
		this.SelectedAction = null;
		this.CustomObject = null;
		this.JustDisplay = null;
	}

	// Token: 0x06003C3E RID: 15422 RVA: 0x001093AC File Offset: 0x001075AC
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
			itemManager.SetSelected(selectedAction, this.JustDisplay.Value);
			itemManager.InstantScroll();
		}
	}

	// Token: 0x04003E3E RID: 15934
	[ObjectType(typeof(InventoryItemManager.SelectedActionType))]
	public FsmEnum SelectedAction;

	// Token: 0x04003E3F RID: 15935
	public FsmGameObject CustomObject;

	// Token: 0x04003E40 RID: 15936
	[HideIf("IsUsingCustomObject")]
	public FsmBool JustDisplay;
}
