using System;
using HutongGames.PlayMaker;

// Token: 0x02000326 RID: 806
[ActionCategory("Hollow Knight")]
public class GetWalkerSpeed : WalkerAction
{
	// Token: 0x06001C43 RID: 7235 RVA: 0x00083A06 File Offset: 0x00081C06
	public override void Reset()
	{
		base.Reset();
		this.WalkSpeedL = new FsmFloat();
		this.WalkSpeedR = new FsmFloat();
		this.WalkSpeed = new FsmFloat();
	}

	// Token: 0x06001C44 RID: 7236 RVA: 0x00083A2F File Offset: 0x00081C2F
	protected override void Apply(Walker walker)
	{
		this.WalkSpeed.Value = walker.walkSpeedR;
		this.WalkSpeedL.Value = walker.walkSpeedL;
		this.WalkSpeedR.Value = walker.walkSpeedR;
	}

	// Token: 0x04001B4F RID: 6991
	[UIHint(UIHint.Variable)]
	public FsmFloat WalkSpeedL;

	// Token: 0x04001B50 RID: 6992
	[UIHint(UIHint.Variable)]
	public FsmFloat WalkSpeedR;

	// Token: 0x04001B51 RID: 6993
	[UIHint(UIHint.Variable)]
	public FsmFloat WalkSpeed;
}
