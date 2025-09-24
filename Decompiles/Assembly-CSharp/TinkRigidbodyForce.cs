using System;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class TinkRigidbodyForce : MonoBehaviour
{
	// Token: 0x06000646 RID: 1606 RVA: 0x000204CC File Offset: 0x0001E6CC
	private void Awake()
	{
		this.tinkEffect.HitInDirection += this.OnHitInDirection;
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x000204E8 File Offset: 0x0001E6E8
	private void OnHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		switch (direction)
		{
		case HitInstance.HitDirection.Left:
			this.body.linearVelocity = new Vector2(-this.horizontalHitForce.x, this.horizontalHitForce.y);
			return;
		case HitInstance.HitDirection.Right:
			this.body.linearVelocity = this.horizontalHitForce;
			return;
		case HitInstance.HitDirection.Up:
			this.body.linearVelocity = this.upHitForce;
			return;
		case HitInstance.HitDirection.Down:
		{
			float num = this.body.transform.position.x - source.transform.position.x;
			this.body.linearVelocity = ((num > 0f) ? this.horizontalHitForce : new Vector2(-this.horizontalHitForce.x, this.horizontalHitForce.y));
			return;
		}
		default:
			throw new ArgumentOutOfRangeException("direction", direction, null);
		}
	}

	// Token: 0x0400061E RID: 1566
	[SerializeField]
	private TinkEffect tinkEffect;

	// Token: 0x0400061F RID: 1567
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x04000620 RID: 1568
	[SerializeField]
	private Vector2 horizontalHitForce;

	// Token: 0x04000621 RID: 1569
	[SerializeField]
	private Vector2 upHitForce;
}
