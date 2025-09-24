using System;
using HutongGames.PlayMaker;

// Token: 0x0200063D RID: 1597
[ActionCategory("Dialogue")]
public class QuestYesNoV2 : YesNoAction
{
	// Token: 0x0600394B RID: 14667 RVA: 0x000FC2FC File Offset: 0x000FA4FC
	public override void Reset()
	{
		base.Reset();
		this.Quest = null;
		this.BeginQuest = true;
	}

	// Token: 0x0600394C RID: 14668 RVA: 0x000FC318 File Offset: 0x000FA518
	protected override void DoOpen()
	{
		QuestYesNoBox.Open(delegate
		{
			base.SendEvent(true);
		}, delegate
		{
			base.SendEvent(false);
		}, this.ReturnHUDAfter.Value, (FullQuestBase)this.Quest.Value, this.BeginQuest.Value);
	}

	// Token: 0x0600394D RID: 14669 RVA: 0x000FC368 File Offset: 0x000FA568
	protected override void DoForceClose()
	{
		QuestYesNoBox.ForceClose();
	}

	// Token: 0x04003C14 RID: 15380
	[ObjectType(typeof(FullQuestBase))]
	[RequiredField]
	public FsmObject Quest;

	// Token: 0x04003C15 RID: 15381
	public FsmBool BeginQuest;
}
