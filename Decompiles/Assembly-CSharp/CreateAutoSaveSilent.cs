using System;
using HutongGames.PlayMaker;

// Token: 0x0200042D RID: 1069
[Tooltip("Silently create an Auto Save without normal save. (No Save Icon Shown)")]
public sealed class CreateAutoSaveSilent : FsmStateAction
{
	// Token: 0x06002517 RID: 9495 RVA: 0x000AAB37 File Offset: 0x000A8D37
	public override void Reset()
	{
		this.nameEnum = null;
	}

	// Token: 0x06002518 RID: 9496 RVA: 0x000AAB40 File Offset: 0x000A8D40
	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance)
		{
			unsafeInstance.CreateRestorePoint((AutoSaveName)this.nameEnum.Value, null);
		}
		base.Finish();
	}

	// Token: 0x040022ED RID: 8941
	[ObjectType(typeof(AutoSaveName))]
	public FsmEnum nameEnum;
}
