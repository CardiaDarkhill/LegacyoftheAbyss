using System;
using HutongGames.PlayMaker;

// Token: 0x0200038E RID: 910
[ActionCategory("Hollow Knight/GG")]
public class GGCheckBossSequenceList : FSMUtility.CheckFsmStateAction
{
	// Token: 0x06001EE3 RID: 7907 RVA: 0x0008D411 File Offset: 0x0008B611
	public override void Reset()
	{
		this.tierList = null;
		base.Reset();
	}

	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06001EE4 RID: 7908 RVA: 0x0008D420 File Offset: 0x0008B620
	public override bool IsTrue
	{
		get
		{
			return BossSequenceController.CheckIfSequence((BossSequence)this.tierList.Value);
		}
	}

	// Token: 0x04001DB1 RID: 7601
	[ObjectType(typeof(BossSequence))]
	public FsmObject tierList;
}
