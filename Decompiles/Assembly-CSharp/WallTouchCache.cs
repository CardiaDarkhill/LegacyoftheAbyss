using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000182 RID: 386
public sealed class WallTouchCache : FixedUpdateCache
{
	// Token: 0x06000C9F RID: 3231 RVA: 0x00038338 File Offset: 0x00036538
	public void Update(Collider2D collider2D, CollisionSide side, bool force)
	{
		Vector3 position = collider2D.transform.position;
		if (force || base.ShouldUpdate() || position != this.lastPosition)
		{
			this.lastPosition = position;
			Bounds bounds = collider2D.bounds;
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			Vector3 center = bounds.center;
			this.top.Reset();
			this.mid.Reset();
			this.bottom.Reset();
			Physics2D.SyncTransforms();
			if (side == CollisionSide.left)
			{
				Vector2 origin = new Vector2(min.x, max.y);
				Vector2 origin2 = new Vector2(min.x, center.y);
				Vector2 origin3 = new Vector2(min.x, min.y);
				RaycastHit2D hit;
				Helper.IsRayHittingNoTriggers(origin, Vector2.left, 0.1f, 8448, out hit);
				RaycastHit2D hit2;
				Helper.IsRayHittingNoTriggers(origin2, Vector2.left, 0.1f, 8448, out hit2);
				RaycastHit2D hit3;
				Helper.IsRayHittingNoTriggers(origin3, Vector2.left, 0.1f, 8448, out hit3);
				this.top.hit = hit;
				this.mid.hit = hit2;
				this.bottom.hit = hit3;
				return;
			}
			if (side != CollisionSide.right)
			{
				return;
			}
			Vector2 origin4 = new Vector2(max.x, max.y);
			Vector2 origin5 = new Vector2(max.x, center.y);
			Vector2 origin6 = new Vector2(max.x, min.y);
			RaycastHit2D hit4;
			Helper.IsRayHittingNoTriggers(origin4, Vector2.right, 0.1f, 8448, out hit4);
			RaycastHit2D hit5;
			Helper.IsRayHittingNoTriggers(origin5, Vector2.right, 0.1f, 8448, out hit5);
			RaycastHit2D hit6;
			Helper.IsRayHittingNoTriggers(origin6, Vector2.right, 0.1f, 8448, out hit6);
			this.top.hit = hit4;
			this.mid.hit = hit5;
			this.bottom.hit = hit6;
		}
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x00038517 File Offset: 0x00036717
	public void Reset()
	{
		this.top.Reset();
		this.mid.Reset();
		this.bottom.Reset();
	}

	// Token: 0x04000C27 RID: 3111
	public readonly WallTouchCache.HitInfo top = new WallTouchCache.HitInfo();

	// Token: 0x04000C28 RID: 3112
	public readonly WallTouchCache.HitInfo mid = new WallTouchCache.HitInfo();

	// Token: 0x04000C29 RID: 3113
	public readonly WallTouchCache.HitInfo bottom = new WallTouchCache.HitInfo();

	// Token: 0x04000C2A RID: 3114
	private Vector3 lastPosition;

	// Token: 0x04000C2B RID: 3115
	private const float rayLength = 0.1f;

	// Token: 0x020014B0 RID: 5296
	public sealed class HitInfo
	{
		// Token: 0x17000CFE RID: 3326
		// (get) Token: 0x06008448 RID: 33864 RVA: 0x0026B206 File Offset: 0x00269406
		public bool HasCollider
		{
			get
			{
				if (!this.cachedCollider2D)
				{
					this.cachedCollider2D = true;
					this.collider2D = this.hit.collider;
					this.hasCollider2D = this.collider2D;
				}
				return this.hasCollider2D;
			}
		}

		// Token: 0x17000CFF RID: 3327
		// (get) Token: 0x06008449 RID: 33865 RVA: 0x0026B23F File Offset: 0x0026943F
		public Collider2D Collider2D
		{
			get
			{
				if (!this.cachedCollider2D)
				{
					this.cachedCollider2D = true;
					this.collider2D = this.hit.collider;
					this.hasCollider2D = this.collider2D;
				}
				return this.collider2D;
			}
		}

		// Token: 0x17000D00 RID: 3328
		// (get) Token: 0x0600844A RID: 33866 RVA: 0x0026B278 File Offset: 0x00269478
		public bool IsSteepSlope
		{
			get
			{
				if (!this.cachedSlope)
				{
					this.cachedSlope = true;
					if (this.HasCollider)
					{
						this.isSteepSlope = SteepSlope.IsSteepSlope(this.Collider2D);
					}
				}
				return this.isSteepSlope;
			}
		}

		// Token: 0x17000D01 RID: 3329
		// (get) Token: 0x0600844B RID: 33867 RVA: 0x0026B2A8 File Offset: 0x002694A8
		public bool IsNonSlider
		{
			get
			{
				if (!this.cachedSlider)
				{
					this.cachedSlider = true;
					if (this.HasCollider)
					{
						NonSlider nonSlider;
						this.isNonSlider = (NonSlider.TryGetNonSlider(this.Collider2D, out nonSlider) && nonSlider.IsActive);
					}
				}
				return this.isNonSlider;
			}
		}

		// Token: 0x0600844C RID: 33868 RVA: 0x0026B2F0 File Offset: 0x002694F0
		public void Reset()
		{
			this.hasCollider2D = false;
			this.cachedCollider2D = false;
			this.cachedSlope = false;
			this.cachedSlider = false;
			this.collider2D = null;
			this.isSteepSlope = false;
			this.isNonSlider = false;
		}

		// Token: 0x04008438 RID: 33848
		public RaycastHit2D hit;

		// Token: 0x04008439 RID: 33849
		private bool hasCollider2D;

		// Token: 0x0400843A RID: 33850
		private bool cachedCollider2D;

		// Token: 0x0400843B RID: 33851
		private bool cachedSlope;

		// Token: 0x0400843C RID: 33852
		private bool cachedSlider;

		// Token: 0x0400843D RID: 33853
		private Collider2D collider2D;

		// Token: 0x0400843E RID: 33854
		private bool isSteepSlope;

		// Token: 0x0400843F RID: 33855
		private bool isNonSlider;
	}
}
