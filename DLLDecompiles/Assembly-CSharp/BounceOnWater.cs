using System;
using UnityEngine;

// Token: 0x0200049B RID: 1179
public sealed class BounceOnWater : MonoBehaviour
{
	// Token: 0x06002A9D RID: 10909 RVA: 0x000B8E85 File Offset: 0x000B7085
	private void Awake()
	{
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody2D>();
			if (this.rb == null)
			{
				Debug.LogError("BounceOnTrigger requires a Rigidbody2D component to be assigned or present on the GameObject.", this);
			}
		}
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x000B8EBA File Offset: 0x000B70BA
	private void OnValidate()
	{
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody2D>();
		}
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x000B8ED8 File Offset: 0x000B70D8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.rb == null)
		{
			return;
		}
		if (!collision.CompareTag("Water Surface"))
		{
			return;
		}
		Vector2 a = Vector2.Reflect(this.rb.linearVelocity, this.normal);
		this.rb.linearVelocity = a * this.bounceMultiplier;
	}

	// Token: 0x04002B5B RID: 11099
	[Header("Bounce Settings")]
	[Tooltip("Adjust the bounciness of the reflection (1 = perfect reflection, <1 = dampened, >1 = amplified).")]
	[SerializeField]
	private float bounceMultiplier = 0.5f;

	// Token: 0x04002B5C RID: 11100
	[SerializeField]
	private Vector2 normal = Vector2.up;

	// Token: 0x04002B5D RID: 11101
	[Tooltip("The Rigidbody2D component of the object.")]
	[SerializeField]
	private Rigidbody2D rb;
}
