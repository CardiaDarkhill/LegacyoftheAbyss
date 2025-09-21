using System;
using HutongGames.PlayMaker;

// Token: 0x0200042C RID: 1068
[Tooltip("Queue Auto Save without queuing normal save.")]
public sealed class QueueAutoSave : FsmStateAction
{
	// Token: 0x06002514 RID: 9492 RVA: 0x000AAAEE File Offset: 0x000A8CEE
	public override void Reset()
	{
		this.nameEnum = null;
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x000AAAF8 File Offset: 0x000A8CF8
	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance)
		{
			unsafeInstance.QueueAutoSave((AutoSaveName)this.nameEnum.Value);
		}
		base.Finish();
	}

	// Token: 0x040022EC RID: 8940
	[ObjectType(typeof(AutoSaveName))]
	public FsmEnum nameEnum;
}
