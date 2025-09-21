using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000544 RID: 1348
public class RosaryCacheHanging : RosaryCache
{
	// Token: 0x06003049 RID: 12361 RVA: 0x000D5330 File Offset: 0x000D3530
	protected override void RespondToHit(HitInstance damageInstance, Vector2 hitPos)
	{
		Vector2 force = damageInstance.GetHitDirectionAsVector(HitInstance.TargetType.Regular) * this.hitForce;
		if (this.reactToHit)
		{
			this.reactToHit.AddForceAtPosition(force, this.reactToHit.transform.TransformPoint(this.forcePositionOffset), ForceMode2D.Impulse);
		}
	}

	// Token: 0x0400332B RID: 13099
	[SerializeField]
	[FormerlySerializedAs("lowestBody")]
	private Rigidbody2D reactToHit;

	// Token: 0x0400332C RID: 13100
	[SerializeField]
	private float hitForce = 10f;

	// Token: 0x0400332D RID: 13101
	[SerializeField]
	private Vector2 forcePositionOffset;
}
