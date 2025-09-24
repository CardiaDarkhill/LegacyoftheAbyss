using System;
using UnityEngine;

// Token: 0x020004C5 RID: 1221
public class ConveyorSpeedZone : MonoBehaviour
{
	// Token: 0x06002C00 RID: 11264 RVA: 0x000C0C9C File Offset: 0x000BEE9C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		HeroController component = collision.GetComponent<HeroController>();
		if (component)
		{
			component.SetConveyorSpeed(this.speed);
		}
	}

	// Token: 0x04002D5C RID: 11612
	public float speed;
}
