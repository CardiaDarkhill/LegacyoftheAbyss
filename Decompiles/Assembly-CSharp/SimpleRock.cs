using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x020005C6 RID: 1478
public class SimpleRock : MonoBehaviour
{
	// Token: 0x060034BD RID: 13501 RVA: 0x000EA174 File Offset: 0x000E8374
	private void Start()
	{
		base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, (float)Random.Range(0, 360));
		this.rb = base.GetComponent<Rigidbody2D>();
		this.setZ = Random.Range(base.transform.position.z, base.transform.position.z + 0.0009999f);
		base.transform.SetPositionZ(this.setZ);
	}

	// Token: 0x060034BE RID: 13502 RVA: 0x000EA20C File Offset: 0x000E840C
	private void FixedUpdate()
	{
		if (!this.spun)
		{
			if (this.stepCounter >= 1)
			{
				float torque = this.rb.linearVelocity.x * -7.5f;
				this.rb.AddTorque(torque);
				this.spun = true;
				return;
			}
			this.stepCounter++;
		}
	}

	// Token: 0x060034BF RID: 13503 RVA: 0x000EA264 File Offset: 0x000E8464
	private void OnTriggerEnter(Collider other)
	{
		PhysLayers layer = (PhysLayers)other.gameObject.layer;
		if (layer == PhysLayers.ENEMIES || layer == PhysLayers.HERO_BOX)
		{
			Vector2 force = new Vector2(Random.Range(-100f, 100f), Random.Range(--0f, 40f));
			this.rb.AddForce(force);
			this.rb.AddTorque(Random.Range(-50f, 50f));
		}
	}

	// Token: 0x0400382F RID: 14383
	private int stepCounter;

	// Token: 0x04003830 RID: 14384
	private bool spun;

	// Token: 0x04003831 RID: 14385
	private Rigidbody2D rb;

	// Token: 0x04003832 RID: 14386
	private float setZ;
}
