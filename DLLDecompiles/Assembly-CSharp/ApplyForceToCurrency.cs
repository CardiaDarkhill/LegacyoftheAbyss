using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200048E RID: 1166
public sealed class ApplyForceToCurrency : MonoBehaviour
{
	// Token: 0x06002A0E RID: 10766 RVA: 0x000B6788 File Offset: 0x000B4988
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.CompareTag("Geo"))
		{
			this.AddForce(other.rigidbody);
		}
	}

	// Token: 0x06002A0F RID: 10767 RVA: 0x000B67A8 File Offset: 0x000B49A8
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Geo"))
		{
			this.AddForce(other.GetComponent<Rigidbody2D>());
		}
	}

	// Token: 0x06002A10 RID: 10768 RVA: 0x000B67C4 File Offset: 0x000B49C4
	private void AddForce(Rigidbody2D rb)
	{
		if (rb != null)
		{
			float num = Mathf.Sign(base.transform.lossyScale.x);
			rb.AddForce(new Vector2(this.xForce.GetRandomValue() * num, this.yForce.GetRandomValue()), ForceMode2D.Impulse);
		}
	}

	// Token: 0x04002A88 RID: 10888
	[SerializeField]
	private MinMaxFloat xForce = new MinMaxFloat(-9f, -3f);

	// Token: 0x04002A89 RID: 10889
	[SerializeField]
	private MinMaxFloat yForce = new MinMaxFloat(0f, 0f);
}
