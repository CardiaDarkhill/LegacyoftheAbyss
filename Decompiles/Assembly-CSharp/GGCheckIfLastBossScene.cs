using System;
using HutongGames.PlayMaker;

// Token: 0x0200038C RID: 908
[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfLastBossScene : FSMUtility.CheckFsmStateAction
{
	// Token: 0x17000320 RID: 800
	// (get) Token: 0x06001EDF RID: 7903 RVA: 0x0008D3E9 File Offset: 0x0008B5E9
	public override bool IsTrue
	{
		get
		{
			return BossSequenceController.BossIndex >= BossSequenceController.BossCount;
		}
	}
}
