using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200042A RID: 1066
public class SaveGameV2 : FsmStateAction
{
	// Token: 0x0600250E RID: 9486 RVA: 0x000AAA17 File Offset: 0x000A8C17
	public override void Reset()
	{
		this.createAutoSave = null;
		this.nameEnum = null;
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x000AAA28 File Offset: 0x000A8C28
	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance)
		{
			if (this.createAutoSave.Value)
			{
				unsafeInstance.SaveGameWithAutoSave((AutoSaveName)this.nameEnum.Value, null);
			}
			else
			{
				unsafeInstance.SaveGame(null);
			}
		}
		else
		{
			Debug.LogError("Failed to save. Missing Game Manager.");
		}
		base.Finish();
	}

	// Token: 0x040022E8 RID: 8936
	public FsmBool createAutoSave;

	// Token: 0x040022E9 RID: 8937
	[ObjectType(typeof(AutoSaveName))]
	public FsmEnum nameEnum;
}
