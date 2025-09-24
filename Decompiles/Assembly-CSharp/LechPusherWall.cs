using System;
using UnityEngine;

// Token: 0x0200024A RID: 586
public class LechPusherWall : MonoBehaviour
{
	// Token: 0x06001552 RID: 5458 RVA: 0x00060910 File Offset: 0x0005EB10
	private void OnCollisionEnter2D(Collision2D other)
	{
		Rigidbody2D rigidbody = other.rigidbody;
		if (rigidbody)
		{
			rigidbody.AddForce(this.force, ForceMode2D.Impulse);
		}
	}

	// Token: 0x040013F6 RID: 5110
	[SerializeField]
	private Vector2 force;
}
