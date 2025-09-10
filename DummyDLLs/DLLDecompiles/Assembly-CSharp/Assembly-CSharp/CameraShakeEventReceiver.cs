using System;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class CameraShakeEventReceiver : EventBase
{
	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x06000A37 RID: 2615 RVA: 0x0002E42E File Offset: 0x0002C62E
	public override string InspectorInfo
	{
		get
		{
			return string.Format("{0} - {1}", this.minIntensity.ToString(), this.maxIntensity.ToString());
		}
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x0002E45C File Offset: 0x0002C65C
	private void OnDrawGizmosSelected()
	{
		if (this.radius > 0f)
		{
			Gizmos.color = Color.cyan;
			Vector3 original = base.transform.TransformPoint(this.offset);
			float? z = new float?(0f);
			Gizmos.DrawWireSphere(original.Where(null, null, z), this.radius);
		}
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0002E4C5 File Offset: 0x0002C6C5
	private void OnValidate()
	{
		if (this.minIntensity > this.maxIntensity)
		{
			this.maxIntensity = this.minIntensity;
		}
		if (this.radius < 0f)
		{
			this.radius = 0f;
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0002E4F9 File Offset: 0x0002C6F9
	private void Start()
	{
		if (this.cameraReference)
		{
			this.cameraReference.CameraShakedWorldForce += this.OnCameraShaked;
		}
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0002E51F File Offset: 0x0002C71F
	private void OnDestroy()
	{
		if (this.cameraReference)
		{
			this.cameraReference.CameraShakedWorldForce -= this.OnCameraShaked;
		}
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0002E548 File Offset: 0x0002C748
	private void OnCameraShaked(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity)
	{
		if (intensity < this.minIntensity || intensity > this.maxIntensity)
		{
			return;
		}
		if (this.radius > 0f && Vector2.SqrMagnitude(base.transform.TransformPoint(this.offset) - cameraPosition) > this.radius * this.radius)
		{
			return;
		}
		base.CallReceivedEvent();
	}

	// Token: 0x040009BE RID: 2494
	[SerializeField]
	private CameraManagerReference cameraReference;

	// Token: 0x040009BF RID: 2495
	[SerializeField]
	private float radius;

	// Token: 0x040009C0 RID: 2496
	[SerializeField]
	private Vector2 offset;

	// Token: 0x040009C1 RID: 2497
	[Space]
	[SerializeField]
	private CameraShakeWorldForceIntensities minIntensity = CameraShakeWorldForceIntensities.Medium;

	// Token: 0x040009C2 RID: 2498
	[SerializeField]
	private CameraShakeWorldForceIntensities maxIntensity = CameraShakeWorldForceIntensities.Intense;
}
