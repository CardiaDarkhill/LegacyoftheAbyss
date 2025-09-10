using System;
using HutongGames.PlayMaker;

// Token: 0x0200038D RID: 909
[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfBossSequence : FSMUtility.CheckFsmStateAction
{
	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06001EE1 RID: 7905 RVA: 0x0008D402 File Offset: 0x0008B602
	public override bool IsTrue
	{
		get
		{
			return BossSequenceController.IsInSequence;
		}
	}
}
