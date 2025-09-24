using System;
using HutongGames.PlayMaker;

// Token: 0x0200042B RID: 1067
public class QueueSaveGameV2 : FsmStateAction
{
	// Token: 0x06002511 RID: 9489 RVA: 0x000AAA8A File Offset: 0x000A8C8A
	public override void Reset()
	{
		this.createAutoSave = null;
		this.nameEnum = null;
	}

	// Token: 0x06002512 RID: 9490 RVA: 0x000AAA9C File Offset: 0x000A8C9C
	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance)
		{
			if (this.createAutoSave.Value)
			{
				unsafeInstance.QueueAutoSave((AutoSaveName)this.nameEnum.Value);
			}
			unsafeInstance.QueueSaveGame();
		}
		base.Finish();
	}

	// Token: 0x040022EA RID: 8938
	public FsmBool createAutoSave;

	// Token: 0x040022EB RID: 8939
	[ObjectType(typeof(AutoSaveName))]
	public FsmEnum nameEnum;
}
