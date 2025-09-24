using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000318 RID: 792
public sealed class SharedDamagedGroup : MonoBehaviour
{
	// Token: 0x06001BEF RID: 7151 RVA: 0x0008219D File Offset: 0x0008039D
	public bool PreventDamage(Collider2D col)
	{
		return this.damagedColliders.Add(col);
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x000821AB File Offset: 0x000803AB
	public void PreventDamage(IHitResponder hitResponder)
	{
		this.damagePrevented.Add(hitResponder);
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x000821BA File Offset: 0x000803BA
	public void ClearDamagePrevented()
	{
		this.damagePrevented.Clear();
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x000821C8 File Offset: 0x000803C8
	public bool HasDamaged(IHitResponder hitResponder)
	{
		if (this.damagePrevented.Contains(hitResponder))
		{
			return true;
		}
		foreach (DamageEnemies damageEnemies in this.damagers)
		{
			damageEnemies.TryClearRespondedList();
			if (damageEnemies.HasResponded(hitResponder))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x00082238 File Offset: 0x00080438
	public void DamageStart(DamageEnemies damager)
	{
		SharedDamagedGroup.DamageInfo damageInfo;
		if (!this.damageInfos.TryGetValue(damager, out damageInfo))
		{
			damageInfo = (this.damageInfos[damager] = new SharedDamagedGroup.DamageInfo());
			this.damagers.Add(damager);
		}
		bool flag = !damageInfo.started;
		damageInfo.started = true;
		if (flag)
		{
			if (this.activeDamagerCount == 0)
			{
				this.ClearDamagePrevented();
				this.damagedColliders.Clear();
			}
			this.activeDamagerCount++;
		}
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x000822B0 File Offset: 0x000804B0
	public void DamageEnd(DamageEnemies damager)
	{
		SharedDamagedGroup.DamageInfo damageInfo;
		if (this.damageInfos.TryGetValue(damager, out damageInfo))
		{
			bool started = damageInfo.started;
			damageInfo.started = false;
			if (started)
			{
				this.activeDamagerCount--;
				if (this.activeDamagerCount <= 0)
				{
					this.activeDamagerCount = 0;
					this.ClearDamagePrevented();
					this.damagedColliders.Clear();
				}
			}
		}
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x0008230B File Offset: 0x0008050B
	public void RemoveDamager(DamageEnemies damager)
	{
		this.DamageEnd(damager);
		if (this.damageInfos.Remove(damager))
		{
			this.damagers.Remove(damager);
		}
	}

	// Token: 0x04001AED RID: 6893
	private readonly HashSet<Collider2D> damagedColliders = new HashSet<Collider2D>();

	// Token: 0x04001AEE RID: 6894
	private readonly HashSet<IHitResponder> damagePrevented = new HashSet<IHitResponder>();

	// Token: 0x04001AEF RID: 6895
	private readonly List<DamageEnemies> damagers = new List<DamageEnemies>();

	// Token: 0x04001AF0 RID: 6896
	private readonly Dictionary<DamageEnemies, SharedDamagedGroup.DamageInfo> damageInfos = new Dictionary<DamageEnemies, SharedDamagedGroup.DamageInfo>();

	// Token: 0x04001AF1 RID: 6897
	private int activeDamagerCount;

	// Token: 0x020015F3 RID: 5619
	private sealed class DamageInfo
	{
		// Token: 0x0400893C RID: 35132
		public DamageEnemies damager;

		// Token: 0x0400893D RID: 35133
		public bool started;
	}
}
