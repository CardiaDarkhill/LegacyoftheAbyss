using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class CreateGameObjectPool : FsmStateAction
{
	// Token: 0x06000147 RID: 327 RVA: 0x00007BA4 File Offset: 0x00005DA4
	public override void Reset()
	{
		this.prefab = null;
		this.amount = null;
		this.useExisting = new FsmBool(true);
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00007BC8 File Offset: 0x00005DC8
	public override void OnEnter()
	{
		if (this.prefab.Value)
		{
			int num = this.amount.Value;
			if (this.useExisting.Value)
			{
				List<GameObject> pooled = ObjectPool.GetPooled(this.prefab.Value, null, false);
				num -= pooled.Count;
			}
			if (num > 0)
			{
				ObjectPool.CreatePool(this.prefab.Value, num, false);
			}
		}
		base.Finish();
	}

	// Token: 0x040000EB RID: 235
	public FsmGameObject prefab;

	// Token: 0x040000EC RID: 236
	public FsmInt amount;

	// Token: 0x040000ED RID: 237
	public FsmBool useExisting;
}
