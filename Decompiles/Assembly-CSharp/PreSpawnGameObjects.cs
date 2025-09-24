using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000026 RID: 38
[ActionCategory("Hollow Knight")]
public class PreSpawnGameObjects : FsmStateAction
{
	// Token: 0x06000150 RID: 336 RVA: 0x00007CF8 File Offset: 0x00005EF8
	public override void Reset()
	{
		this.prefab = null;
		this.storeArray = null;
		this.spawnAmount = null;
		this.spawnAmountMultiplier = 1;
	}

	// Token: 0x06000151 RID: 337 RVA: 0x00007D1C File Offset: 0x00005F1C
	public override void OnEnter()
	{
		if (this.prefab.Value && !this.storeArray.IsNone && this.spawnAmount.Value > 0 && this.spawnAmountMultiplier.Value > 0)
		{
			int num = this.spawnAmount.Value * this.spawnAmountMultiplier.Value;
			this.storeArray.Resize(num);
			for (int i = 0; i < num; i++)
			{
				this.storeArray.Values[i] = Object.Instantiate<GameObject>(this.prefab.Value);
				((GameObject)this.storeArray.Values[i]).SetActive(false);
			}
		}
		base.Finish();
	}

	// Token: 0x040000F3 RID: 243
	public FsmGameObject prefab;

	// Token: 0x040000F4 RID: 244
	[UIHint(UIHint.Variable)]
	[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
	public FsmArray storeArray;

	// Token: 0x040000F5 RID: 245
	public FsmInt spawnAmount;

	// Token: 0x040000F6 RID: 246
	public FsmInt spawnAmountMultiplier;
}
