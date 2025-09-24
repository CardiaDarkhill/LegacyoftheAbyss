using System;
using HutongGames.PlayMaker;

// Token: 0x02000323 RID: 803
[ActionCategory("Hollow Knight")]
public class CancelWalkerTurn : WalkerAction
{
	// Token: 0x06001C3A RID: 7226 RVA: 0x000838C7 File Offset: 0x00081AC7
	protected override void Apply(Walker walker)
	{
		walker.CancelTurn();
	}
}
