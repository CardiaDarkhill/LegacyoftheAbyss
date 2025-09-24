using System;
using HutongGames.PlayMaker;

// Token: 0x020006AF RID: 1711
public class SendInventoryPaneInputEvent : FSMUtility.GetComponentFsmStateAction<InventoryPaneBase>
{
	// Token: 0x06003D95 RID: 15765 RVA: 0x0010EC1A File Offset: 0x0010CE1A
	public override void Reset()
	{
		base.Reset();
		this.InputEvent = null;
	}

	// Token: 0x06003D96 RID: 15766 RVA: 0x0010EC2C File Offset: 0x0010CE2C
	protected override void DoAction(InventoryPaneBase pane)
	{
		if (!this.InputEvent.IsNone)
		{
			InventoryPaneBase.InputEventType eventType = (InventoryPaneBase.InputEventType)this.InputEvent.Value;
			pane.SendInputEvent(eventType);
		}
	}

	// Token: 0x04003F3E RID: 16190
	[ObjectType(typeof(InventoryPaneBase.InputEventType))]
	public FsmEnum InputEvent;
}
