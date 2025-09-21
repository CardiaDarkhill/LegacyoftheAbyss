using System;
using UnityEngine;

// Token: 0x0200032E RID: 814
public class EnemyPusher : MonoBehaviour
{
	// Token: 0x06001C89 RID: 7305 RVA: 0x000850E8 File Offset: 0x000832E8
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.root.gameObject.name != collision.otherCollider.transform.root.gameObject.name)
		{
			Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
		}
	}
}
