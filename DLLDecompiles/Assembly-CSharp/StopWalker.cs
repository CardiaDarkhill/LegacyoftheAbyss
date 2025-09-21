using System;
using HutongGames.PlayMaker;

// Token: 0x02000321 RID: 801
[ActionCategory("Hollow Knight")]
public class StopWalker : WalkerAction
{
	// Token: 0x06001C35 RID: 7221 RVA: 0x00083860 File Offset: 0x00081A60
	protected override void Apply(Walker walker)
	{
		walker.Stop(Walker.StopReasons.Controlled);
	}
}
