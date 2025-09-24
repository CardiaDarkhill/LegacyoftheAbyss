using System;
using UnityEngine;

// Token: 0x020003FE RID: 1022
public class LilyPad : MonoBehaviour
{
	// Token: 0x060022C0 RID: 8896 RVA: 0x0009FA7B File Offset: 0x0009DC7B
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x0009FA8C File Offset: 0x0009DC8C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		float num = collision.transform.position.x - base.transform.position.x;
		float num2 = Random.Range(this.minSpeed, this.maxSpeed);
		if (num < 0f)
		{
			this.rb.linearVelocity = new Vector2(num2, 0f);
			return;
		}
		this.rb.linearVelocity = new Vector2(-num2, 0f);
	}

	// Token: 0x04002190 RID: 8592
	private Rigidbody2D rb;

	// Token: 0x04002191 RID: 8593
	public float minSpeed;

	// Token: 0x04002192 RID: 8594
	public float maxSpeed;
}
