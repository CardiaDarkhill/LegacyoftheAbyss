using System;
using HutongGames.PlayMaker;

// Token: 0x02000391 RID: 913
[ActionCategory("Hollow Knight/GG")]
public class GGCheckBoundCharms : FSMUtility.CheckFsmStateAction
{
	// Token: 0x17000325 RID: 805
	// (get) Token: 0x06001EEB RID: 7915 RVA: 0x0008D4C3 File Offset: 0x0008B6C3
	public override bool IsTrue
	{
		get
		{
			return BossSequenceController.BoundCharms;
		}
	}
}
