using System;
using HutongGames.PlayMaker;

// Token: 0x02000390 RID: 912
[ActionCategory("Hollow Knight/GG")]
public class GGCheckBoundNail : FSMUtility.CheckFsmStateAction
{
	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06001EE9 RID: 7913 RVA: 0x0008D4B4 File Offset: 0x0008B6B4
	public override bool IsTrue
	{
		get
		{
			return BossSequenceController.BoundNail;
		}
	}
}
