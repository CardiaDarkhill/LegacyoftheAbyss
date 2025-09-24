using System;
using HutongGames.PlayMaker;

// Token: 0x02000325 RID: 805
[ActionCategory("Hollow Knight")]
public class SetWalkerSpeed : WalkerAction
{
	// Token: 0x06001C3F RID: 7231 RVA: 0x00083956 File Offset: 0x00081B56
	public bool IsUsingSingleSpeed()
	{
		return !this.WalkSpeed.IsNone;
	}

	// Token: 0x06001C40 RID: 7232 RVA: 0x00083966 File Offset: 0x00081B66
	public override void Reset()
	{
		base.Reset();
		this.WalkSpeedL = new FsmFloat();
		this.WalkSpeedR = new FsmFloat();
		this.WalkSpeed = new FsmFloat();
	}

	// Token: 0x06001C41 RID: 7233 RVA: 0x00083990 File Offset: 0x00081B90
	protected override void Apply(Walker walker)
	{
		float walkSpeedL = this.WalkSpeedL.Value;
		float value = this.WalkSpeedR.Value;
		if (this.IsUsingSingleSpeed())
		{
			walkSpeedL = -this.WalkSpeed.Value;
			value = this.WalkSpeed.Value;
		}
		if (!this.WalkSpeedL.IsNone)
		{
			walker.walkSpeedL = walkSpeedL;
		}
		if (!this.WalkSpeedR.IsNone)
		{
			walker.walkSpeedR = value;
		}
	}

	// Token: 0x04001B4C RID: 6988
	[HideIf("IsUsingSingleSpeed")]
	public FsmFloat WalkSpeedL;

	// Token: 0x04001B4D RID: 6989
	[HideIf("IsUsingSingleSpeed")]
	public FsmFloat WalkSpeedR;

	// Token: 0x04001B4E RID: 6990
	public FsmFloat WalkSpeed;
}
