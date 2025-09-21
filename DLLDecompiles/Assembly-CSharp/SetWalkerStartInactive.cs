using System;
using HutongGames.PlayMaker;

// Token: 0x02000327 RID: 807
[ActionCategory("Hollow Knight")]
public class SetWalkerStartInactive : WalkerAction
{
	// Token: 0x06001C46 RID: 7238 RVA: 0x00083A6C File Offset: 0x00081C6C
	public override void Reset()
	{
		base.Reset();
		this.startInactive = new FsmBool();
	}

	// Token: 0x06001C47 RID: 7239 RVA: 0x00083A7F File Offset: 0x00081C7F
	protected override void Apply(Walker walker)
	{
		walker.startInactive = this.startInactive.Value;
	}

	// Token: 0x04001B52 RID: 6994
	public FsmBool startInactive;
}
