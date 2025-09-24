using System;
using HutongGames.PlayMaker;

// Token: 0x0200030A RID: 778
[ActionCategory("Hollow Knight")]
public class CheckCanSeeHero : FsmStateAction
{
	// Token: 0x06001B9D RID: 7069 RVA: 0x00080D2D File Offset: 0x0007EF2D
	public override void Reset()
	{
		this.storeResult = new FsmBool();
	}

	// Token: 0x06001B9E RID: 7070 RVA: 0x00080D3A File Offset: 0x0007EF3A
	public override void OnEnter()
	{
		this.source = base.Owner.GetComponent<LineOfSightDetector>();
		this.Apply();
		if (!this.everyFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x06001B9F RID: 7071 RVA: 0x00080D61 File Offset: 0x0007EF61
	public override void OnUpdate()
	{
		this.Apply();
	}

	// Token: 0x06001BA0 RID: 7072 RVA: 0x00080D6C File Offset: 0x0007EF6C
	private void Apply()
	{
		if (this.source != null)
		{
			if (!this.storeResult.IsNone)
			{
				this.storeResult.Value = this.source.CanSeeHero;
			}
			if (this.source.CanSeeHero)
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
				return;
			}
		}
		else if (!this.storeResult.IsNone)
		{
			this.storeResult.Value = false;
		}
	}

	// Token: 0x04001AA0 RID: 6816
	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	// Token: 0x04001AA1 RID: 6817
	public FsmString sendEvent;

	// Token: 0x04001AA2 RID: 6818
	public FsmEventTarget eventTarget;

	// Token: 0x04001AA3 RID: 6819
	public bool everyFrame;

	// Token: 0x04001AA4 RID: 6820
	private LineOfSightDetector source;
}
