using System;
using UnityEngine;

// Token: 0x020002A1 RID: 673
public class BrokenVesselGlob : MonoBehaviour
{
	// Token: 0x060017AE RID: 6062 RVA: 0x0006B350 File Offset: 0x00069550
	private void OnEnable()
	{
		base.transform.localScale = new Vector3(2f, 2f, 2f);
		this.gasParticle.Play();
		this.rb.linearVelocity = new Vector3(this.rb.linearVelocity.x, -17.5f);
		this.timer = 5f;
	}

	// Token: 0x060017AF RID: 6063 RVA: 0x0006B3BC File Offset: 0x000695BC
	private void Update()
	{
		this.FaceAngle();
		this.ProjectileSquash();
		if (this.timer >= 0f)
		{
			this.timer -= Time.deltaTime;
			return;
		}
		base.gameObject.Recycle();
	}

	// Token: 0x060017B0 RID: 6064 RVA: 0x0006B3F5 File Offset: 0x000695F5
	private void FixedUpdate()
	{
		this.rb.AddForce(this.force);
	}

	// Token: 0x060017B1 RID: 6065 RVA: 0x0006B408 File Offset: 0x00069608
	private void FaceAngle()
	{
		Vector2 linearVelocity = this.rb.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f;
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x060017B2 RID: 6066 RVA: 0x0006B454 File Offset: 0x00069654
	private void ProjectileSquash()
	{
		float num = 1f - this.rb.linearVelocity.magnitude * this.stretchFactor * 0.01f;
		float num2 = 1f + this.rb.linearVelocity.magnitude * this.stretchFactor * 0.01f;
		if (num2 < this.stretchMinX)
		{
			num2 = this.stretchMinX;
		}
		if (num > this.stretchMaxY)
		{
			num = this.stretchMaxY;
		}
		num *= this.scaleModifier;
		num2 *= this.scaleModifier;
		base.transform.localScale = new Vector3(num2, num, base.transform.localScale.z);
	}

	// Token: 0x04001663 RID: 5731
	public ParticleSystem gasParticle;

	// Token: 0x04001664 RID: 5732
	public Rigidbody2D rb;

	// Token: 0x04001665 RID: 5733
	private float timer;

	// Token: 0x04001666 RID: 5734
	private Vector2 force = new Vector2(0f, 25f);

	// Token: 0x04001667 RID: 5735
	private float stretchFactor = 2f;

	// Token: 0x04001668 RID: 5736
	private float stretchMinX = 1f;

	// Token: 0x04001669 RID: 5737
	private float stretchMaxY = 2f;

	// Token: 0x0400166A RID: 5738
	private float scaleModifier = 2f;
}
