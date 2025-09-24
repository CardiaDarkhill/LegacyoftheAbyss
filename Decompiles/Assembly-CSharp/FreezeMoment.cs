using System;
using GlobalEnums;
using HutongGames.PlayMaker;

// Token: 0x02000431 RID: 1073
public class FreezeMoment : FsmStateAction
{
	// Token: 0x06002522 RID: 9506 RVA: 0x000AAC09 File Offset: 0x000A8E09
	public override void Reset()
	{
		this.FreezeMomentType = null;
	}

	// Token: 0x06002523 RID: 9507 RVA: 0x000AAC14 File Offset: 0x000A8E14
	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if (instance)
		{
			instance.FreezeMoment((FreezeMomentTypes)this.FreezeMomentType.Value, null);
		}
		base.Finish();
	}

	// Token: 0x040022F0 RID: 8944
	[ObjectType(typeof(FreezeMomentTypes))]
	public FsmEnum FreezeMomentType;
}
