using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000533 RID: 1331
public class PushReactNonPhysical : MonoBehaviour
{
	// Token: 0x06002FC0 RID: 12224 RVA: 0x000D2304 File Offset: 0x000D0504
	private void OnValidate()
	{
		if (this.minDirection.x < 0f)
		{
			this.minDirection.x = 0f;
		}
		if (this.minDirection.y < 0f)
		{
			this.minDirection.y = 0f;
		}
	}

	// Token: 0x06002FC1 RID: 12225 RVA: 0x000D2355 File Offset: 0x000D0555
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.centreOffset), 0.15f);
	}

	// Token: 0x06002FC2 RID: 12226 RVA: 0x000D2378 File Offset: 0x000D0578
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (Time.timeAsDouble < this.cooldownFinishTime)
		{
			return;
		}
		this.cooldownFinishTime = Time.timeAsDouble + (double)this.cooldownDuration;
		Vector2 vector = base.transform.TransformPoint(this.centreOffset) - collision.transform.position;
		if (Mathf.Abs(vector.x) < this.minDirection.x)
		{
			vector.x = this.minDirection.x * Mathf.Sign(vector.x);
		}
		if (Mathf.Abs(vector.y) < this.minDirection.y)
		{
			vector.y = this.minDirection.y * Mathf.Sign(vector.y);
		}
		vector.Normalize();
		this.body.AddForce(vector * this.forceMagnitude.GetRandomValue(), ForceMode2D.Impulse);
	}

	// Token: 0x04003281 RID: 12929
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x04003282 RID: 12930
	[SerializeField]
	private Vector2 centreOffset;

	// Token: 0x04003283 RID: 12931
	[SerializeField]
	private MinMaxFloat forceMagnitude;

	// Token: 0x04003284 RID: 12932
	[SerializeField]
	private Vector2 minDirection;

	// Token: 0x04003285 RID: 12933
	[SerializeField]
	private float cooldownDuration;

	// Token: 0x04003286 RID: 12934
	private double cooldownFinishTime;
}
