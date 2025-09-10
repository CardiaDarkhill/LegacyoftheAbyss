using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200017D RID: 381
public sealed class ConchProjectileCollision : MonoBehaviour
{
	// Token: 0x06000C76 RID: 3190 RVA: 0x00036F81 File Offset: 0x00035181
	private void Awake()
	{
		this.layerMask = 1 << this.layer;
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x00036F94 File Offset: 0x00035194
	private void OnDisable()
	{
		this.previousCollider = null;
		this.previousNormal = Vector2.zero;
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x00036FA8 File Offset: 0x000351A8
	private void OnValidate()
	{
		if (this.projectileVelocityManager == null)
		{
			this.projectileVelocityManager = base.GetComponent<ProjectileVelocityManager>();
		}
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x00036FC4 File Offset: 0x000351C4
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!this.isActive)
		{
			return;
		}
		if (other.gameObject.layer != this.layer)
		{
			return;
		}
		this.CheckCollision(true);
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x00036FEA File Offset: 0x000351EA
	private void OnCollisionStay2D(Collision2D other)
	{
		if (!this.isActive)
		{
			return;
		}
		if (other.gameObject.layer != this.layer)
		{
			return;
		}
		this.CheckCollision(true);
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00037010 File Offset: 0x00035210
	private void CheckCollision(bool checkHit = false)
	{
		ConchProjectileCollision.<>c__DisplayClass20_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		Bounds bounds = this.collider2D.bounds;
		CS$<>8__locals1.max = bounds.max;
		CS$<>8__locals1.min = bounds.min;
		CS$<>8__locals1.center = bounds.center;
		CS$<>8__locals1.didHit = false;
		if (this.direction.y > 0f)
		{
			this.<CheckCollision>g__CheckTop|20_0(ref CS$<>8__locals1);
			if (this.direction.x > 0f)
			{
				this.<CheckCollision>g__CheckRight|20_1(ref CS$<>8__locals1);
			}
			else
			{
				this.<CheckCollision>g__CheckLeft|20_3(ref CS$<>8__locals1);
			}
		}
		else
		{
			this.<CheckCollision>g__CheckBottom|20_2(ref CS$<>8__locals1);
			if (this.direction.x > 0f)
			{
				this.<CheckCollision>g__CheckRight|20_1(ref CS$<>8__locals1);
			}
			else
			{
				this.<CheckCollision>g__CheckLeft|20_3(ref CS$<>8__locals1);
			}
		}
		Vector2 linearVelocity = this.rigidbody2D.linearVelocity;
		if (linearVelocity.x == 0f || linearVelocity.y == 0f)
		{
			this.projectileVelocityManager.DesiredVelocity = this.projectileVelocityManager.DesiredVelocity;
		}
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x0003710B File Offset: 0x0003530B
	public void SetDirection(Vector2 direction)
	{
		this.previousNormal = Vector2.zero;
		this.previousCollider = null;
		this.direction = direction;
		this.isActive = true;
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x0003712D File Offset: 0x0003532D
	public void StateExited()
	{
		this.isActive = false;
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x00037174 File Offset: 0x00035374
	[CompilerGenerated]
	private void <CheckCollision>g__CheckTop|20_0(ref ConchProjectileCollision.<>c__DisplayClass20_0 A_1)
	{
		this.topRays.Clear();
		this.topRays.Add(new Vector2(A_1.min.x, A_1.max.y));
		this.topRays.Add(new Vector2(A_1.center.x, A_1.max.y));
		this.topRays.Add(A_1.max);
		for (int i = 0; i < 3; i++)
		{
			RaycastHit2D raycastHit2D = Helper.Raycast2D(this.topRays[i], Vector2.up, 0.15f, this.layerMask);
			Collider2D collider = raycastHit2D.collider;
			if (!(collider == null) && (!(this.previousNormal == raycastHit2D.normal) || !(collider == this.previousCollider)))
			{
				this.control.SendEvent("ROOF");
				this.previousNormal = raycastHit2D.normal;
				this.previousCollider = collider;
				A_1.didHit = true;
				return;
			}
		}
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x00037284 File Offset: 0x00035484
	[CompilerGenerated]
	private void <CheckCollision>g__CheckRight|20_1(ref ConchProjectileCollision.<>c__DisplayClass20_0 A_1)
	{
		this.rightRays.Clear();
		this.rightRays.Add(A_1.max);
		this.rightRays.Add(new Vector2(A_1.max.x, A_1.center.y));
		this.rightRays.Add(new Vector2(A_1.max.x, A_1.min.y));
		for (int i = 0; i < 3; i++)
		{
			RaycastHit2D raycastHit2D = Helper.Raycast2D(this.rightRays[i], Vector2.right, 0.15f, this.layerMask);
			Collider2D collider = raycastHit2D.collider;
			if (collider != null && (!(this.previousNormal == raycastHit2D.normal) || !(collider == this.previousCollider)))
			{
				this.control.SendEvent("WALL R");
				this.previousNormal = raycastHit2D.normal;
				this.previousCollider = collider;
				A_1.didHit = true;
				return;
			}
		}
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x00037394 File Offset: 0x00035594
	[CompilerGenerated]
	private void <CheckCollision>g__CheckBottom|20_2(ref ConchProjectileCollision.<>c__DisplayClass20_0 A_1)
	{
		this.bottomRays.Clear();
		this.bottomRays.Add(new Vector2(A_1.max.x, A_1.min.y));
		this.bottomRays.Add(new Vector2(A_1.center.x, A_1.min.y));
		this.bottomRays.Add(A_1.min);
		for (int i = 0; i < 3; i++)
		{
			RaycastHit2D raycastHit2D = Helper.Raycast2D(this.bottomRays[i], -Vector2.up, 0.15f, this.layerMask);
			Collider2D collider = raycastHit2D.collider;
			if (collider != null && (!(this.previousNormal == raycastHit2D.normal) || !(collider == this.previousCollider)))
			{
				this.control.SendEvent("FLOOR");
				this.previousNormal = raycastHit2D.normal;
				this.previousCollider = collider;
				A_1.didHit = true;
				return;
			}
		}
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x000374A8 File Offset: 0x000356A8
	[CompilerGenerated]
	private void <CheckCollision>g__CheckLeft|20_3(ref ConchProjectileCollision.<>c__DisplayClass20_0 A_1)
	{
		this.leftRays.Clear();
		this.leftRays.Add(A_1.min);
		this.leftRays.Add(new Vector2(A_1.min.x, A_1.center.y));
		this.leftRays.Add(new Vector2(A_1.min.x, A_1.max.y));
		for (int i = 0; i < 3; i++)
		{
			RaycastHit2D raycastHit2D = Helper.Raycast2D(this.leftRays[i], -Vector2.right, 0.15f, this.layerMask);
			Collider2D collider = raycastHit2D.collider;
			if (collider != null && (!(this.previousNormal == raycastHit2D.normal) || !(collider == this.previousCollider)))
			{
				this.control.SendEvent("WALL L");
				this.previousNormal = raycastHit2D.normal;
				this.previousCollider = collider;
				A_1.didHit = true;
				return;
			}
		}
	}

	// Token: 0x04000BF3 RID: 3059
	[SerializeField]
	private PlayMakerFSM control;

	// Token: 0x04000BF4 RID: 3060
	[SerializeField]
	private Rigidbody2D rigidbody2D;

	// Token: 0x04000BF5 RID: 3061
	[SerializeField]
	private Collider2D collider2D;

	// Token: 0x04000BF6 RID: 3062
	[SerializeField]
	private ProjectileVelocityManager projectileVelocityManager;

	// Token: 0x04000BF7 RID: 3063
	[SerializeField]
	private Vector2 direction;

	// Token: 0x04000BF8 RID: 3064
	[SerializeField]
	private int layer = 8;

	// Token: 0x04000BF9 RID: 3065
	private bool isActive;

	// Token: 0x04000BFA RID: 3066
	public const float RAYCAST_LENGTH = 0.15f;

	// Token: 0x04000BFB RID: 3067
	private Collider2D previousCollider;

	// Token: 0x04000BFC RID: 3068
	private Vector2 previousNormal;

	// Token: 0x04000BFD RID: 3069
	private List<Vector2> topRays = new List<Vector2>();

	// Token: 0x04000BFE RID: 3070
	private List<Vector2> rightRays = new List<Vector2>();

	// Token: 0x04000BFF RID: 3071
	private List<Vector2> bottomRays = new List<Vector2>();

	// Token: 0x04000C00 RID: 3072
	private List<Vector2> leftRays = new List<Vector2>();

	// Token: 0x04000C01 RID: 3073
	private int layerMask;
}
