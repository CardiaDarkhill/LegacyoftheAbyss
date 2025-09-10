using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000324 RID: 804
[ActionCategory("Hollow Knight")]
public class SetWalkerFacing : WalkerAction
{
	// Token: 0x06001C3C RID: 7228 RVA: 0x000838D7 File Offset: 0x00081AD7
	public override void Reset()
	{
		base.Reset();
		this.walkRight = new FsmBool
		{
			UseVariable = true
		};
		this.randomStartDir = new FsmBool();
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x000838FC File Offset: 0x00081AFC
	protected override void Apply(Walker walker)
	{
		if (this.randomStartDir.Value)
		{
			walker.ChangeFacing((Random.Range(0, 2) == 0) ? -1 : 1);
			return;
		}
		if (!this.walkRight.IsNone)
		{
			walker.ChangeFacing(this.walkRight.Value ? 1 : -1);
		}
	}

	// Token: 0x04001B4A RID: 6986
	public FsmBool walkRight;

	// Token: 0x04001B4B RID: 6987
	public FsmBool randomStartDir;
}
