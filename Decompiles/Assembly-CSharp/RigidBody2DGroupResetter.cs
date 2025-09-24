using System;
using UnityEngine;

// Token: 0x02000541 RID: 1345
public class RigidBody2DGroupResetter : MonoBehaviour
{
	// Token: 0x06003030 RID: 12336 RVA: 0x000D4B3E File Offset: 0x000D2D3E
	private void OnDrawGizmosSelected()
	{
		if (this.setRadialVelocity)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.TransformPoint(this.radialVelocityCentre), this.radialVelocityMagnitude);
		}
	}

	// Token: 0x06003031 RID: 12337 RVA: 0x000D4B73 File Offset: 0x000D2D73
	private void Awake()
	{
		this.bodies = base.GetComponentsInChildren<Rigidbody2D>();
		this.initialBodyDatas = new RigidBody2DGroupResetter.BodyData[this.bodies.Length];
	}

	// Token: 0x06003032 RID: 12338 RVA: 0x000D4B94 File Offset: 0x000D2D94
	private void OnEnable()
	{
		Vector3 b = base.transform.TransformPoint(this.radialVelocityCentre);
		for (int i = 0; i < this.bodies.Length; i++)
		{
			Rigidbody2D rigidbody2D = this.bodies[i];
			Transform transform = rigidbody2D.transform;
			this.initialBodyDatas[i] = new RigidBody2DGroupResetter.BodyData
			{
				Position = transform.localPosition,
				Angle = transform.localEulerAngles.z
			};
			if (this.setRadialVelocity && !rigidbody2D.isKinematic)
			{
				Vector3 normalized = (transform.position - b).normalized;
				rigidbody2D.linearVelocity = normalized * this.radialVelocityMagnitude;
			}
		}
	}

	// Token: 0x06003033 RID: 12339 RVA: 0x000D4C5C File Offset: 0x000D2E5C
	private void OnDisable()
	{
		for (int i = 0; i < this.bodies.Length; i++)
		{
			Rigidbody2D rigidbody2D = this.bodies[i];
			Transform transform = rigidbody2D.transform;
			RigidBody2DGroupResetter.BodyData bodyData = this.initialBodyDatas[i];
			rigidbody2D.linearVelocity = Vector2.zero;
			rigidbody2D.angularVelocity = 0f;
			transform.localPosition = bodyData.Position;
			transform.SetLocalRotation2D(bodyData.Angle);
		}
	}

	// Token: 0x0400330D RID: 13069
	[SerializeField]
	private bool setRadialVelocity;

	// Token: 0x0400330E RID: 13070
	[SerializeField]
	private Vector2 radialVelocityCentre;

	// Token: 0x0400330F RID: 13071
	[SerializeField]
	private float radialVelocityMagnitude;

	// Token: 0x04003310 RID: 13072
	private RigidBody2DGroupResetter.BodyData[] initialBodyDatas;

	// Token: 0x04003311 RID: 13073
	private Rigidbody2D[] bodies;

	// Token: 0x0200184A RID: 6218
	private struct BodyData
	{
		// Token: 0x0400917C RID: 37244
		public Vector2 Position;

		// Token: 0x0400917D RID: 37245
		public float Angle;
	}
}
