using System;
using HutongGames.PlayMaker;

// Token: 0x0200063C RID: 1596
[ActionCategory("Dialogue")]
public class QuestYesNo : YesNoAction
{
	// Token: 0x06003945 RID: 14661 RVA: 0x000FC291 File Offset: 0x000FA491
	public override void Reset()
	{
		base.Reset();
		this.Quest = null;
	}

	// Token: 0x06003946 RID: 14662 RVA: 0x000FC2A0 File Offset: 0x000FA4A0
	protected override void DoOpen()
	{
		QuestYesNoBox.Open(delegate
		{
			base.SendEvent(true);
		}, delegate
		{
			base.SendEvent(false);
		}, this.ReturnHUDAfter.Value, (FullQuestBase)this.Quest.Value, true);
	}

	// Token: 0x06003947 RID: 14663 RVA: 0x000FC2DB File Offset: 0x000FA4DB
	protected override void DoForceClose()
	{
		QuestYesNoBox.ForceClose();
	}

	// Token: 0x04003C13 RID: 15379
	[ObjectType(typeof(FullQuestBase))]
	[RequiredField]
	public FsmObject Quest;
}
