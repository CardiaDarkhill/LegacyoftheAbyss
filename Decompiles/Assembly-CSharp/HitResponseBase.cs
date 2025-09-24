using System;
using UnityEngine;

// Token: 0x020004FF RID: 1279
public abstract class HitResponseBase : DebugDrawColliderRuntimeAdder
{
	// Token: 0x1400008F RID: 143
	// (add) Token: 0x06002DBB RID: 11707 RVA: 0x000C8204 File Offset: 0x000C6404
	// (remove) Token: 0x06002DBC RID: 11708 RVA: 0x000C823C File Offset: 0x000C643C
	public event HitResponseBase.HitInDirectionDelegate HitInDirection;

	// Token: 0x17000527 RID: 1319
	// (get) Token: 0x06002DBD RID: 11709
	// (set) Token: 0x06002DBE RID: 11710
	public abstract bool IsActive { get; set; }

	// Token: 0x06002DBF RID: 11711 RVA: 0x000C8271 File Offset: 0x000C6471
	protected void SendHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		HitResponseBase.HitInDirectionDelegate hitInDirection = this.HitInDirection;
		if (hitInDirection == null)
		{
			return;
		}
		hitInDirection(source, direction);
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x000C8285 File Offset: 0x000C6485
	public override void AddDebugDrawComponent()
	{
		if (!base.enabled)
		{
			return;
		}
		if (base.gameObject.layer == 8)
		{
			DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.TerrainCollider, false);
			return;
		}
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.None, false);
	}

	// Token: 0x02001801 RID: 6145
	// (Invoke) Token: 0x06008FA6 RID: 36774
	public delegate void HitInDirectionDelegate(GameObject source, HitInstance.HitDirection direction);
}
