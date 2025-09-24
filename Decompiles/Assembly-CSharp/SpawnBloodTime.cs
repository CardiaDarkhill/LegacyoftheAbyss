using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class SpawnBloodTime : SpawnBlood
{
	// Token: 0x06000A01 RID: 2561 RVA: 0x0002D663 File Offset: 0x0002B863
	public override void Reset()
	{
		base.Reset();
		this.delay = new FsmFloat(0.1f);
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x0002D680 File Offset: 0x0002B880
	public override void OnEnter()
	{
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x0002D682 File Offset: 0x0002B882
	public override void OnUpdate()
	{
		base.OnUpdate();
		if (Time.timeAsDouble <= this.nextSpawnTime)
		{
			return;
		}
		this.nextSpawnTime = Time.timeAsDouble + (double)this.delay.Value;
		base.Spawn();
	}

	// Token: 0x0400098C RID: 2444
	public FsmFloat delay;

	// Token: 0x0400098D RID: 2445
	private double nextSpawnTime;
}
