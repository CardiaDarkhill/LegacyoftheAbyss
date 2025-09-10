using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class SpawnBlood : FsmStateAction
{
	// Token: 0x060009FD RID: 2557 RVA: 0x0002D528 File Offset: 0x0002B728
	public override void Reset()
	{
		this.spawnPoint = new FsmGameObject
		{
			UseVariable = true
		};
		this.position = new FsmVector3();
		this.spawnMin = null;
		this.spawnMax = null;
		this.speedMin = null;
		this.speedMax = null;
		this.angleMin = null;
		this.angleMax = null;
		this.colorOverride = new FsmColor
		{
			UseVariable = true
		};
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x0002D58E File Offset: 0x0002B78E
	public override void OnEnter()
	{
		this.Spawn();
		base.Finish();
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x0002D59C File Offset: 0x0002B79C
	protected void Spawn()
	{
		Vector3 a = this.position.Value;
		if (this.spawnPoint.Value)
		{
			a += this.spawnPoint.Value.transform.position;
		}
		BloodSpawner.SpawnBlood(a, (short)this.spawnMin.Value, (short)this.spawnMax.Value, this.speedMin.Value, this.speedMax.Value, this.angleMin.Value, this.angleMax.Value, this.colorOverride.IsNone ? null : new Color?(this.colorOverride.Value), 0f);
	}

	// Token: 0x04000983 RID: 2435
	public FsmGameObject spawnPoint;

	// Token: 0x04000984 RID: 2436
	public FsmVector3 position;

	// Token: 0x04000985 RID: 2437
	public FsmInt spawnMin;

	// Token: 0x04000986 RID: 2438
	public FsmInt spawnMax;

	// Token: 0x04000987 RID: 2439
	public FsmFloat speedMin;

	// Token: 0x04000988 RID: 2440
	public FsmFloat speedMax;

	// Token: 0x04000989 RID: 2441
	public FsmFloat angleMin;

	// Token: 0x0400098A RID: 2442
	public FsmFloat angleMax;

	// Token: 0x0400098B RID: 2443
	public FsmColor colorOverride;
}
