using System;
using HutongGames.PlayMaker;

// Token: 0x02000392 RID: 914
[ActionCategory("Hollow Knight/GG")]
public class GGResetBossSequenceController : FsmStateAction
{
	// Token: 0x06001EED RID: 7917 RVA: 0x0008D4D2 File Offset: 0x0008B6D2
	public override void OnEnter()
	{
		BossSequenceController.Reset();
		base.Finish();
	}
}
