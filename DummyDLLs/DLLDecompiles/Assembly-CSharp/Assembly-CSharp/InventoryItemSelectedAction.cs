using System;
using HutongGames.PlayMaker;

// Token: 0x02000697 RID: 1687
public class InventoryItemSelectedAction : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	// Token: 0x06003C47 RID: 15431 RVA: 0x0010953A File Offset: 0x0010773A
	public override void Reset()
	{
		base.Reset();
		this.Action = null;
		this.SuccessEvent = null;
		this.FailureEvent = null;
	}

	// Token: 0x06003C48 RID: 15432 RVA: 0x00109558 File Offset: 0x00107758
	protected override void DoAction(InventoryItemManager itemManager)
	{
		bool flag = false;
		switch ((InventoryItemSelectedAction.ItemAction)this.Action.Value)
		{
		case InventoryItemSelectedAction.ItemAction.Submit:
			flag = itemManager.SubmitButtonSelected();
			break;
		case InventoryItemSelectedAction.ItemAction.Cancel:
			flag = itemManager.CancelButtonSelected();
			break;
		case InventoryItemSelectedAction.ItemAction.Option:
			break;
		case InventoryItemSelectedAction.ItemAction.SubmitRelease:
			flag = itemManager.SubmitButtonReleaseSelected();
			break;
		case InventoryItemSelectedAction.ItemAction.Extra:
			flag = itemManager.ExtraButtonSelected();
			break;
		case InventoryItemSelectedAction.ItemAction.ExtraRelease:
			flag = itemManager.ExtraButtonReleaseSelected();
			break;
		case InventoryItemSelectedAction.ItemAction.Super:
			flag = itemManager.SuperButtonSelected();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		base.Fsm.Event(flag ? this.SuccessEvent : this.FailureEvent);
	}

	// Token: 0x04003E48 RID: 15944
	[ObjectType(typeof(InventoryItemSelectedAction.ItemAction))]
	public FsmEnum Action;

	// Token: 0x04003E49 RID: 15945
	public FsmEvent SuccessEvent;

	// Token: 0x04003E4A RID: 15946
	public FsmEvent FailureEvent;

	// Token: 0x0200199C RID: 6556
	public enum ItemAction
	{
		// Token: 0x04009666 RID: 38502
		Submit,
		// Token: 0x04009667 RID: 38503
		Cancel,
		// Token: 0x04009668 RID: 38504
		Option,
		// Token: 0x04009669 RID: 38505
		SubmitRelease,
		// Token: 0x0400966A RID: 38506
		Extra,
		// Token: 0x0400966B RID: 38507
		ExtraRelease,
		// Token: 0x0400966C RID: 38508
		Super
	}
}
