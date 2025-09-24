using System;
using HutongGames.PlayMaker;

// Token: 0x02000642 RID: 1602
public abstract class YesNoAction : FsmStateAction
{
	// Token: 0x06003982 RID: 14722 RVA: 0x000FCB1D File Offset: 0x000FAD1D
	public override void Reset()
	{
		this.YesEvent = null;
		this.NoEvent = null;
		this.ReturnHUDAfter = null;
	}

	// Token: 0x06003983 RID: 14723 RVA: 0x000FCB34 File Offset: 0x000FAD34
	public override void OnEnter()
	{
		this.succeeded = false;
		this.DoOpen();
	}

	// Token: 0x06003984 RID: 14724 RVA: 0x000FCB43 File Offset: 0x000FAD43
	protected void SendEvent(bool isYes)
	{
		this.succeeded = true;
		base.Fsm.Event(isYes ? this.YesEvent : this.NoEvent);
		base.Finish();
	}

	// Token: 0x06003985 RID: 14725 RVA: 0x000FCB6E File Offset: 0x000FAD6E
	public override void OnExit()
	{
		if (!this.succeeded)
		{
			this.DoForceClose();
		}
	}

	// Token: 0x06003986 RID: 14726
	protected abstract void DoOpen();

	// Token: 0x06003987 RID: 14727
	protected abstract void DoForceClose();

	// Token: 0x04003C3F RID: 15423
	public FsmEvent YesEvent;

	// Token: 0x04003C40 RID: 15424
	public FsmEvent NoEvent;

	// Token: 0x04003C41 RID: 15425
	public FsmBool ReturnHUDAfter;

	// Token: 0x04003C42 RID: 15426
	private bool succeeded;
}
