using System;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class WaterDetector : MonoBehaviour
{
	// Token: 0x060016D8 RID: 5848 RVA: 0x00066DD9 File Offset: 0x00064FD9
	private void Awake()
	{
		this.collider = base.GetComponent<BoxCollider2D>();
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x00066DE8 File Offset: 0x00064FE8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Rigidbody2D component = collision.GetComponent<Rigidbody2D>();
		if (component)
		{
			this.Splash(component.linearVelocity.y * component.mass / 40f);
		}
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x00066E24 File Offset: 0x00065024
	private void OnTriggerExit2D(Collider2D collision)
	{
		Rigidbody2D component = collision.GetComponent<Rigidbody2D>();
		if (component)
		{
			this.Splash(component.linearVelocity.y * component.mass / 40f);
		}
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x00066E60 File Offset: 0x00065060
	public void Splash(float force)
	{
		if (!this.water)
		{
			this.water = base.transform.parent.GetComponent<WaterPhysics>();
		}
		this.water.Splash(base.transform.position.x, force);
	}

	// Token: 0x04001558 RID: 5464
	private WaterPhysics water;

	// Token: 0x04001559 RID: 5465
	private BoxCollider2D collider;
}
