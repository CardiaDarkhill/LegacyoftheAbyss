using System;
using HutongGames.PlayMaker;

// Token: 0x0200038B RID: 907
[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfFirstBossScene : FSMUtility.CheckFsmStateAction
{
	// Token: 0x1700031F RID: 799
	// (get) Token: 0x06001EDD RID: 7901 RVA: 0x0008D3D7 File Offset: 0x0008B5D7
	public override bool IsTrue
	{
		get
		{
			return BossSequenceController.BossIndex < 1;
		}
	}
}
