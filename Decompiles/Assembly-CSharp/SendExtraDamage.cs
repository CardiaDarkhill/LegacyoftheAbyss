using System;
using HutongGames.PlayMaker;

// Token: 0x020002E6 RID: 742
[ActionCategory("Hollow Knight")]
[Note("DEPRECATED")]
public class SendExtraDamage : FsmStateAction
{
	// Token: 0x06001A34 RID: 6708 RVA: 0x00078A97 File Offset: 0x00076C97
	public override void Reset()
	{
	}

	// Token: 0x06001A35 RID: 6709 RVA: 0x00078A99 File Offset: 0x00076C99
	public override void OnEnter()
	{
		base.Finish();
	}

	// Token: 0x04001926 RID: 6438
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x04001927 RID: 6439
	public FsmEnum extraDamageType;
}
