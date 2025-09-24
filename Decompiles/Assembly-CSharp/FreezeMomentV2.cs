using System;
using GlobalEnums;
using HutongGames.PlayMaker;

// Token: 0x02000432 RID: 1074
public class FreezeMomentV2 : FsmStateAction
{
	// Token: 0x06002525 RID: 9509 RVA: 0x000AAC54 File Offset: 0x000A8E54
	public override void Reset()
	{
		this.FreezeMomentType = null;
		this.WaitForFinish = null;
	}

	// Token: 0x06002526 RID: 9510 RVA: 0x000AAC64 File Offset: 0x000A8E64
	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if (instance)
		{
			if (this.WaitForFinish.Value)
			{
				instance.FreezeMoment((FreezeMomentTypes)this.FreezeMomentType.Value, new Action(base.Finish));
				return;
			}
			instance.FreezeMoment((FreezeMomentTypes)this.FreezeMomentType.Value, null);
		}
		base.Finish();
	}

	// Token: 0x040022F1 RID: 8945
	[ObjectType(typeof(FreezeMomentTypes))]
	public FsmEnum FreezeMomentType;

	// Token: 0x040022F2 RID: 8946
	public FsmBool WaitForFinish;
}
