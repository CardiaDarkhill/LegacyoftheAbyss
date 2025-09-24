using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000183 RID: 387
public sealed class TouchGroundResult : FixedUpdateCache
{
	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000CA2 RID: 3234 RVA: 0x00038563 File Offset: 0x00036763
	// (set) Token: 0x06000CA3 RID: 3235 RVA: 0x0003856B File Offset: 0x0003676B
	public bool IsTouchingGround { get; private set; }

	// Token: 0x06000CA4 RID: 3236 RVA: 0x00038574 File Offset: 0x00036774
	public void Update(Collider2D collider2D, bool forced)
	{
		Vector3 position = collider2D.transform.position;
		if (forced || base.ShouldUpdate() || position != this.lastPosition)
		{
			this.lastPosition = position;
			Bounds bounds = collider2D.bounds;
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			Vector3 center = bounds.center;
			Vector2 origin = new Vector2(min.x, center.y);
			Vector2 origin2 = center;
			Vector2 origin3 = new Vector2(max.x, center.y);
			TouchGroundResult.<>c__DisplayClass5_0 CS$<>8__locals1;
			CS$<>8__locals1.rayLengthY = bounds.extents.y + 0.16f;
			Physics2D.SyncTransforms();
			this.IsTouchingGround = (TouchGroundResult.<Update>g__IsRayHitting|5_0(origin, ref CS$<>8__locals1) || TouchGroundResult.<Update>g__IsRayHitting|5_0(origin2, ref CS$<>8__locals1) || TouchGroundResult.<Update>g__IsRayHitting|5_0(origin3, ref CS$<>8__locals1));
		}
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x00038654 File Offset: 0x00036854
	[CompilerGenerated]
	internal static bool <Update>g__IsRayHitting|5_0(Vector2 origin, ref TouchGroundResult.<>c__DisplayClass5_0 A_1)
	{
		RaycastHit2D raycastHit2D;
		return Helper.IsRayHittingNoTriggers(origin, Vector2.down, A_1.rayLengthY, 8448, out raycastHit2D) && !SteepSlope.IsSteepSlope(raycastHit2D.collider);
	}

	// Token: 0x04000C2D RID: 3117
	private Vector3 lastPosition;
}
