using System;
using HutongGames.PlayMaker;

// Token: 0x02000322 RID: 802
[ActionCategory("Hollow Knight")]
public class StartWalker : WalkerAction
{
	// Token: 0x06001C37 RID: 7223 RVA: 0x00083871 File Offset: 0x00081A71
	public override void Reset()
	{
		base.Reset();
		this.walkRight = new FsmBool
		{
			UseVariable = true
		};
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x0008388B File Offset: 0x00081A8B
	protected override void Apply(Walker walker)
	{
		if (this.walkRight.IsNone)
		{
			walker.StartMoving();
		}
		else
		{
			walker.Go(this.walkRight.Value ? 1 : -1);
		}
		walker.ClearTurnCooldown();
	}

	// Token: 0x04001B49 RID: 6985
	public FsmBool walkRight;
}
