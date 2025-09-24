using System;
using HutongGames.PlayMaker;

// Token: 0x020007BE RID: 1982
[ActionCategory("Hollow Knight")]
public class MoveLiftChain : LiftChainAction
{
	// Token: 0x060045D7 RID: 17879 RVA: 0x0012FB74 File Offset: 0x0012DD74
	protected override void Apply(LiftChain liftChain)
	{
		if (this.goUp.IsNone)
		{
			return;
		}
		if (this.goUp.Value)
		{
			liftChain.GoUp();
			return;
		}
		liftChain.GoDown();
	}

	// Token: 0x04004682 RID: 18050
	public FsmBool goUp = new FsmBool();
}
