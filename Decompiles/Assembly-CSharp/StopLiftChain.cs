using System;
using HutongGames.PlayMaker;

// Token: 0x020007BF RID: 1983
[ActionCategory("Hollow Knight")]
public class StopLiftChain : LiftChainAction
{
	// Token: 0x060045D9 RID: 17881 RVA: 0x0012FBB1 File Offset: 0x0012DDB1
	protected override void Apply(LiftChain liftChain)
	{
		liftChain.Stop();
	}
}
