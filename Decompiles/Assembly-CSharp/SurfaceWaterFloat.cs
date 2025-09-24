using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000563 RID: 1379
public class SurfaceWaterFloat : MonoBehaviour
{
	// Token: 0x0600313B RID: 12603 RVA: 0x000DA860 File Offset: 0x000D8A60
	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		position.y += this.waterLevel;
		Gizmos.DrawWireSphere(position, 0.2f);
	}

	// Token: 0x0600313C RID: 12604 RVA: 0x000DA895 File Offset: 0x000D8A95
	private void Awake()
	{
		this.parentRegion = base.GetComponentInParent<SurfaceWaterRegion>();
	}

	// Token: 0x0600313D RID: 12605 RVA: 0x000DA8A4 File Offset: 0x000D8AA4
	private void OnTriggerEnter2D(Collider2D collision)
	{
		SurfaceWaterFloater component = collision.GetComponent<SurfaceWaterFloater>();
		if (component && this.floaters.AddIfNotPresent(component))
		{
			component.SetInWater(true);
		}
	}

	// Token: 0x0600313E RID: 12606 RVA: 0x000DA8D8 File Offset: 0x000D8AD8
	private void OnTriggerExit2D(Collider2D collision)
	{
		SurfaceWaterFloater component = collision.GetComponent<SurfaceWaterFloater>();
		if (component && this.floaters.Remove(component))
		{
			component.SetInWater(false);
		}
	}

	// Token: 0x0600313F RID: 12607 RVA: 0x000DA90C File Offset: 0x000D8B0C
	private void FixedUpdate()
	{
		Transform transform = base.transform;
		float num = transform.position.y + this.waterLevel;
		Quaternion rotation = transform.rotation;
		float flowSpeed = this.parentRegion.FlowSpeed;
		bool flag = Mathf.Abs(flowSpeed) > 0.001f && Mathf.Abs(rotation.eulerAngles.z) > 1f;
		for (int i = this.floaters.Count - 1; i >= 0; i--)
		{
			SurfaceWaterFloater surfaceWaterFloater = this.floaters[i];
			float floatMultiplier = surfaceWaterFloater.FloatMultiplier;
			if (floatMultiplier > Mathf.Epsilon)
			{
				if (flag)
				{
					surfaceWaterFloater.MoveWithSurface(flowSpeed, rotation);
				}
				else
				{
					Vector2 vector = surfaceWaterFloater.transform.position;
					float num2 = 1f - (vector.y - num) / surfaceWaterFloater.FloatHeight;
					if (num2 > 0f)
					{
						float num3 = -Physics2D.gravity.y * surfaceWaterFloater.GravityScale * (num2 - surfaceWaterFloater.Velocity * this.bounceDamp);
						num3 *= floatMultiplier;
						surfaceWaterFloater.AddForce(num3);
						surfaceWaterFloater.AddFlowSpeed(flowSpeed, rotation);
					}
					surfaceWaterFloater.Dampen();
				}
			}
		}
	}

	// Token: 0x04003499 RID: 13465
	[SerializeField]
	private float waterLevel;

	// Token: 0x0400349A RID: 13466
	[SerializeField]
	private float bounceDamp;

	// Token: 0x0400349B RID: 13467
	private readonly List<SurfaceWaterFloater> floaters = new List<SurfaceWaterFloater>();

	// Token: 0x0400349C RID: 13468
	private SurfaceWaterRegion parentRegion;
}
